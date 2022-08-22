using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using AdvantShop.Statistic;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.SQL;
using AdvantShop.FilePath;
using AdvantShop.SEO;

namespace AdvantShop.ExportImport
{
    public class CsvImportCategories
    {
        private readonly string _fullPath;
        private readonly bool _hasHeadrs;

        private Dictionary<string, int> _fieldMapping;
        private readonly string _separator;
        private readonly string _encodings;
        private readonly bool _updateNameAndDescription;
        private readonly bool _downloadRemotePhoto;
        private readonly bool _updatePhotos;
        private readonly string _changedBy;
        private bool _useCommonStatistic;

        private readonly List<CategoryImportMapping> _categoryMapping = new List<CategoryImportMapping>();

        public CsvImportCategories(
            string filePath, 
            bool hasHeadrs, 
            string separator, 
            string encodings, 
            Dictionary<string, int> fieldMapping, 
            bool updateNameAndDescription = true,
            bool downloadRemotePhoto = true,
            bool useCommonStatistic = true,
            bool updatePhotos = true,
            string changedBy = null)
        {
            _fullPath = filePath;
            _hasHeadrs = hasHeadrs;
            _fieldMapping = fieldMapping;
            _encodings = encodings;
            _separator = separator;
            _updateNameAndDescription = updateNameAndDescription;
            _downloadRemotePhoto = downloadRemotePhoto;
            _useCommonStatistic = useCommonStatistic;
            _updatePhotos = updatePhotos;
            _changedBy = changedBy;
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var reader = new CsvReader(new StreamReader(_fullPath, Encoding.GetEncoding(_encodings ?? EncodingsEnum.Utf8.StrName())));

            reader.Configuration.Delimiter = _separator ?? SeparatorsEnum.SemicolonSeparated.StrName();
            reader.Configuration.HasHeaderRecord = hasHeaderRecord ?? _hasHeadrs;

            return reader;
        }

        public List<string[]> ReadFirstRecord()
        {
            var list = new List<string[]>();
            using (var csv = InitReader())
            {
                int count = 0;
                while (csv.Read())
                {
                    if (count == 2)
                        break;

                    if (csv.CurrentRecord != null)
                        list.Add(csv.CurrentRecord);
                    count++;
                }
            }
            return list;
        }

        // private - чтобы нельзя было запускать импорт при уже запущеном,
        // через данный метот это не контролируется
        private void Process(Func<Category, Category> func = null)
        {
            try
            {
                _process(func);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                DoForCommonStatistic(() =>
                {
                    CommonStatistic.WriteLog(ex.Message);
                    CommonStatistic.TotalErrorRow++;
                });
            }
            finally
            {
            }
        }

        public Task<bool> ProcessThroughACommonStatistic(
            string currentProcess,
            string currentProcessName,
            Action onBeforeImportAction = null,
            Func<Category, Category> func = null,
            Action onAfterImportAction = null)
        {
            return CommonStatistic.StartNew(() =>
                {
                    if (onBeforeImportAction != null)
                        onBeforeImportAction();

                    _useCommonStatistic = true;
                    Process(func);

                    if (onAfterImportAction != null)
                        onAfterImportAction();
                },
                currentProcess,
                currentProcessName);
        }

        private void _process(Func<Category, Category> func = null)
        {
            Log("Начало импорта");

            if (_fieldMapping == null)
                MapFields();

            if (_fieldMapping == null)
                throw new Exception("can mapping colums");


            DoForCommonStatistic(() => CommonStatistic.TotalRow = GetRowCount());

            var postProcessing = _fieldMapping.ContainsKey(CategoryFields.ParentCategory.StrName());

            if (postProcessing)
                DoForCommonStatistic(() => CommonStatistic.TotalRow *= 2);

            ProcessRows(false, func);
            if (postProcessing)
                ProcessRows(true);

            ProductService.PreCalcProductParamsMass();
            CategoryService.RecalculateProductsCountManual();
            CategoryService.SetCategoryHierarchicallyEnabled(0);
            LuceneSearch.CreateAllIndexInBackground();

            CacheManager.Clean();
            FileHelpers.DeleteFilesFromImageTempInBackground();
            FileHelpers.DeleteFile(_fullPath);

            Log("Окончание импорта");
        }

        private void MapFields()
        {
            _fieldMapping = new Dictionary<string, int>();
            using (var csv = InitReader(false))
            {
                csv.Read();
                for (var i = 0; i < csv.CurrentRecord.Length; i++)
                {
                    if (csv.CurrentRecord[i] == ProductFields.None.StrName()) continue;
                    if (!_fieldMapping.ContainsKey(csv.CurrentRecord[i]))
                        _fieldMapping.Add(csv.CurrentRecord[i], i);
                }
            }
        }

        private long GetRowCount()
        {
            long count = 0;
            using (var csv = InitReader())
            {
                while (csv.Read())
                    count++;
            }
            return count;
        }

        private void ProcessRows(bool postProcess, Func<Category, Category> func = null)
        {
            if (!File.Exists(_fullPath)) return;
            using (var csv = InitReader())
            {
                while (csv.Read())
                {
                    if ((!CommonStatistic.IsRun || CommonStatistic.IsBreaking) && !_useCommonStatistic)
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }
                    try
                    {
                        var categoryInStrings = PrepareRow(csv);
                        if (categoryInStrings == null) continue;

                        var currentRowIndex = csv.Row;

                        if (!postProcess)
                            UpdateInsertCategory(categoryInStrings, currentRowIndex, func);
                        else
                            PostProcess(categoryInStrings, currentRowIndex);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                        DoForCommonStatistic(() =>
                        {
                            CommonStatistic.WriteLog(string.Format("{0}: {1}", CommonStatistic.RowPosition, ex.Message));
                            CommonStatistic.TotalErrorRow++;
                        });
                    }
                }
            }
        }

        private Dictionary<CategoryFields, object> PrepareRow(ICsvReader csv)
        {
            var categoryInStrings = new Dictionary<CategoryFields, object>();

            foreach (CategoryFields field in Enum.GetValues(typeof(CategoryFields)))
            {
                switch (field.Status())
                {
                    case CsvFieldStatus.String:
                        GetString(field, csv, categoryInStrings);
                        break;
                    case CsvFieldStatus.StringRequired:
                        GetStringRequired(field, csv, categoryInStrings);
                        break;
                    case CsvFieldStatus.NotEmptyString:
                        GetStringNotNull(field, csv, categoryInStrings);
                        break;
                    case CsvFieldStatus.Float:
                        if (!GetDecimal(field, csv, categoryInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.NullableFloat:
                        if (!GetNullableDecimal(field, csv, categoryInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.Int:
                        if (!GetInt(field, csv, categoryInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.DateTime:
                        if (!GetDateTime(field, csv, categoryInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.NullableDateTime:
                        if (!GetNullableDateTime(field, csv, categoryInStrings))
                            return null;
                        break;
                }
            }
            return categoryInStrings;
        }


        private void UpdateInsertCategory(Dictionary<CategoryFields, object> categoryInStrings, int currentRowIndex, Func<Category, Category> func = null)
        {
            try
            {
                Category category = null;

                var csvCategoryId = categoryInStrings.ContainsKey(CategoryFields.CategoryId)
                                    ? Convert.ToString(categoryInStrings[CategoryFields.CategoryId])
                                    : string.Empty;
                var externalId = categoryInStrings.ContainsKey(CategoryFields.ExternalId)
                                ? Convert.ToString(categoryInStrings[CategoryFields.ExternalId])
                                : string.Empty;

                if (!string.IsNullOrWhiteSpace(externalId))
                    category = CategoryService.GetCategoryFromDbByExternalId(externalId);

                if (category == null && !string.IsNullOrWhiteSpace(csvCategoryId))
                {
                    // для совместимости с предыдущей логикой работы
                    //category = CategoryService.GetCategoryFromDbByExternalId(csvCategoryId);
                    //if (category != null)
                    //    externalId = csvCategoryId;
                    var categoryId = csvCategoryId.TryParseInt(true);
                    if (categoryId.HasValue) // && category == null
                        category = CategoryService.GetCategory(categoryId.Value);
                }

                var addingNew = category == null;

                if (addingNew)
                {
                    category = new Category
                    {
                        ExternalId = externalId,
                        DisplayStyle = ECategoryDisplayStyle.Tile
                    };
                }
                if (_updateNameAndDescription || addingNew)
                {
                    if (categoryInStrings.ContainsKey(CategoryFields.Name))
                        category.Name = categoryInStrings[CategoryFields.Name].AsString();
                    else
                        category.Name = category.Name ?? string.Empty;
                }

                category.ModifiedBy = !string.IsNullOrEmpty(_changedBy) ? _changedBy : "CSV Import";

                if (categoryInStrings.ContainsKey(CategoryFields.Slug))
                {
                    var url = categoryInStrings[CategoryFields.Slug].AsString().IsNotEmpty()
                                      ? categoryInStrings[CategoryFields.Slug].AsString()
                                      : (category.Name != "" ? category.Name.Reduce(50) : category.CategoryId.ToString());
                    category.UrlPath = UrlService.GetAvailableValidUrl(category.CategoryId, ParamType.Category, url);
                }
                else
                {
                    var url = category.Name != "" ? category.Name.Reduce(50) : category.CategoryId.ToString();

                    category.UrlPath = category.UrlPath ?? UrlService.GetAvailableValidUrl(category.CategoryId, ParamType.Category, url);
                }

                if (categoryInStrings.ContainsKey(CategoryFields.SortOrder))
                    category.SortOrder = Convert.ToString(categoryInStrings[CategoryFields.SortOrder]).TryParseInt();

                if (categoryInStrings.ContainsKey(CategoryFields.Enabled))
                    category.Enabled = categoryInStrings[CategoryFields.Enabled].AsString().Trim().Equals("+");
       
                if (categoryInStrings.ContainsKey(CategoryFields.Hidden))
                    category.Hidden = categoryInStrings[CategoryFields.Hidden].AsString().Trim().Equals("+");

                if (_updateNameAndDescription || addingNew)
                {
                    if (categoryInStrings.ContainsKey(CategoryFields.BriefDescription))
                        category.BriefDescription = categoryInStrings[CategoryFields.BriefDescription].AsString();

                    if (categoryInStrings.ContainsKey(CategoryFields.Description))
                        category.Description = categoryInStrings[CategoryFields.Description].AsString();
                }

                if (categoryInStrings.ContainsKey(CategoryFields.DisplayStyle))
                {
                    ECategoryDisplayStyle style;
                    Enum.TryParse(categoryInStrings[CategoryFields.DisplayStyle].AsString(), true, out style);

                    category.DisplayStyle = style;
                }

                if (categoryInStrings.ContainsKey(CategoryFields.Sorting))
                {
                    ESortOrder sorting;
                    Enum.TryParse(categoryInStrings[CategoryFields.Sorting].AsString(), true, out sorting);

                    category.Sorting = sorting;
                }

                if (categoryInStrings.ContainsKey(CategoryFields.DisplayBrandsInMenu))
                    category.DisplayBrandsInMenu = categoryInStrings[CategoryFields.DisplayBrandsInMenu].AsString().Trim().Equals("+");

                if (categoryInStrings.ContainsKey(CategoryFields.DisplaySubCategoriesInMenu))
                    category.DisplaySubCategoriesInMenu = categoryInStrings[CategoryFields.DisplaySubCategoriesInMenu].AsString().Trim().Equals("+");

                if (func != null)
                    category = func(category);

                if (!addingNew)
                {
                    CategoryService.UpdateCategory(category, false, true, new ChangedBy(category.ModifiedBy));
                    DoForCommonStatistic(() => CommonStatistic.TotalUpdateRow++);
                    Log("категория обновлена " + category.Name);
                }
                else
                {
                    category.CategoryId = CategoryService.AddCategory(category, false, true, new ChangedBy(category.ModifiedBy));
                    DoForCommonStatistic(() => CommonStatistic.TotalAddRow++);
                    Log("категория добавлена " + category.Name);
                }

                if (category.CategoryId >= 0)
                {
                    var idToUse = externalId.IsNullOrEmpty() ? csvCategoryId : externalId;
                    if (idToUse.IsNotEmpty() && !_categoryMapping.Any(x => x.CsvCategoryId == idToUse))
                    {
                        _categoryMapping.Add(new CategoryImportMapping(idToUse, category.CategoryId, currentRowIndex));
                    }
                    else if (idToUse.IsNullOrEmpty())
                    {
                        _categoryMapping.Add(new CategoryImportMapping(idToUse, category.CategoryId, currentRowIndex));
                    }

                    OtherFields(categoryInStrings, category.CategoryId, category.Name, addingNew);
                }
                else
                {
                    Log("Не удалось добавить категорию: " + category.Name);
                    DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                }
            }
            catch (Exception e)
            {
                DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                Log(CommonStatistic.RowPosition + ": " + e.Message);
            }

            categoryInStrings.Clear();
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }


        private void OtherFields(IDictionary<CategoryFields, object> fields, int categoryId, string categoryName, bool newCategory)
        {
            if (fields.ContainsKey(CategoryFields.Title) || fields.ContainsKey(CategoryFields.H1)
                || fields.ContainsKey(CategoryFields.MetaKeywords) || fields.ContainsKey(CategoryFields.MetaDescription))
            {

                var meta = new MetaInfo { ObjId = categoryId, Type = MetaType.Category };

                if (fields.ContainsKey(CategoryFields.Title))
                    meta.Title = fields[CategoryFields.Title].AsString();
                else
                    meta.Title = meta.Title ?? SettingsSEO.CategoryMetaTitle;

                if (fields.ContainsKey(CategoryFields.H1))
                    meta.H1 = fields[CategoryFields.H1].AsString();
                else
                    meta.H1 = meta.H1 ?? SettingsSEO.CategoryMetaH1;

                if (fields.ContainsKey(CategoryFields.MetaKeywords))
                    meta.MetaKeywords = fields[CategoryFields.MetaKeywords].AsString();
                else
                    meta.MetaKeywords = meta.MetaKeywords ?? SettingsSEO.CategoryMetaKeywords;

                if (fields.ContainsKey(CategoryFields.MetaDescription))
                    meta.MetaDescription = fields[CategoryFields.MetaDescription].AsString();
                else
                    meta.MetaDescription = meta.MetaDescription ?? SettingsSEO.CategoryMetaDescription;

                MetaInfoService.SetMeta(meta);
            }
            if (fields.ContainsKey(CategoryFields.Tags))
            {
                TagService.DeleteMap(categoryId, ETagType.Category);

                var i = 0;

                foreach (var tagName in fields[CategoryFields.Tags].AsString().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var tag = TagService.Get(tagName);
                    if (tag == null)
                    {
                        var tagId = TagService.Add(new Tag
                        {
                            Name = tagName,
                            Enabled = true,
                            UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Tag, tagName)
                        });
                        TagService.AddMap(categoryId, tagId, ETagType.Category, i * 10);
                    }
                    else
                    {
                        TagService.AddMap(categoryId, tag.Id, ETagType.Category, i * 10);
                    }

                    i++;
                }
            }

            if (newCategory || _updatePhotos)
            {
                if (fields.ContainsKey(CategoryFields.Picture))
                {
                    var photo = fields[CategoryFields.Picture].AsString();
                    if (!string.IsNullOrEmpty(photo))
                    {
                        PhotoFromString(photo, categoryId, categoryName, PhotoType.CategoryBig, CategoryImageType.Big,
                            SettingsPictureSize.BigCategoryImageWidth,
                            SettingsPictureSize.BigCategoryImageHeight);
                    }
                }

                if (fields.ContainsKey(CategoryFields.MiniPicture))
                {
                    var photo = fields[CategoryFields.MiniPicture].AsString();
                    if (!string.IsNullOrEmpty(photo))
                    {
                        PhotoFromString(photo, categoryId, categoryName, PhotoType.CategorySmall, CategoryImageType.Small,
                            SettingsPictureSize.SmallCategoryImageWidth,
                            SettingsPictureSize.SmallCategoryImageHeight);
                    }
                }

                if (fields.ContainsKey(CategoryFields.Icon))
                {
                    var photo = fields[CategoryFields.Icon].AsString();
                    if (!string.IsNullOrEmpty(photo))
                    {
                        PhotoFromString(photo, categoryId, categoryName, PhotoType.CategoryIcon, CategoryImageType.Icon,
                            SettingsPictureSize.IconCategoryImageWidth,
                            SettingsPictureSize.IconCategoryImageHeight);
                    }
                }
            }

            if (fields.ContainsKey(CategoryFields.PropertyGroups))
            {
                PropertyGroupService.DeleteGroupCategoriesByCategoryId(categoryId);

                foreach (var groupName in fields[CategoryFields.PropertyGroups].AsString().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var group = PropertyGroupService.Get(groupName);
                    if (group == null)
                    {
                        var groupId = PropertyGroupService.Add(new PropertyGroup() { Name = groupName });

                        PropertyGroupService.AddGroupToCategory(groupId, categoryId);
                    }
                    else
                    {
                        PropertyGroupService.AddGroupToCategory(group.PropertyGroupId, categoryId);
                    }
                }
            }
        }

        public void PostProcess(IDictionary<CategoryFields, object> fields, int currentRowIndex)
        {
            var rowPosition = CommonStatistic.RowPosition;
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);

            var csvCategoryId = fields.ContainsKey(CategoryFields.CategoryId) ? Convert.ToString(fields[CategoryFields.CategoryId]) : string.Empty;
            var externalId = fields.ContainsKey(CategoryFields.ExternalId) ? Convert.ToString(fields[CategoryFields.ExternalId]) : string.Empty;

            var idToUse = externalId.IsNullOrEmpty() ? csvCategoryId : externalId;
            if (idToUse.IsNotEmpty() && !_categoryMapping.Any(x => x.CsvCategoryId == idToUse))
                return;

            var categoryMapping = idToUse.IsNotEmpty()
                ? _categoryMapping.Find(x => x.CsvCategoryId == idToUse)
                : _categoryMapping.Find(x => x.CsvRowIndex == currentRowIndex);

            var categoryId = categoryMapping != null ? categoryMapping.CategoryId : - 1;
            
            if (!CategoryService.IsExistCategory(categoryId))
            {
                Log(rowPosition + ": " + "Category Id '" + csvCategoryId + "' not found");
                DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                return;
            }

            if (fields.ContainsKey(CategoryFields.ParentCategory))
            {
                var csvParentCategoryId = Convert.ToString(fields[CategoryFields.ParentCategory]);

                var parentCategoryMapping = _categoryMapping.Find(x => x.CsvCategoryId == csvParentCategoryId);

                int? parentCategoryId = parentCategoryMapping != null
                    ? parentCategoryMapping.CategoryId
                    : csvParentCategoryId.TryParseInt(true);

                if (parentCategoryId.HasValue && CategoryService.IsExistCategory(parentCategoryId.Value)
                    && categoryId != 0 && parentCategoryId.Value != categoryId)
                {
                    SQLDataAccess.ExecuteNonQuery(
                        "Update Catalog.Category Set ParentCategory=@ParentCategory where CategoryID = @CategoryID",
                        CommandType.Text,
                        new SqlParameter("@ParentCategory", parentCategoryId.Value),
                        new SqlParameter("@CategoryID", categoryId));
                }
                else
                {
                    Log(rowPosition + ": " + "Parent Category Id '" + csvParentCategoryId + "' not found");
                    DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                }
            }
        }

        private void PhotoFromString(string photo, int categoryId, string categoryName, PhotoType photoType, CategoryImageType imageType, int width, int height)
        {
            // if remote picture we must download it
            if (photo.Contains("http://") || photo.Contains("https://"))
            {
                var uri = new Uri(photo);

                if (!_downloadRemotePhoto)
                {
                    PhotoService.DeletePhotos(categoryId, photoType);
                    PhotoService.AddPhotoWithOrignName(new Photo(0, categoryId, photoType)
                    {
                        PhotoName = uri.AbsoluteUri,
                        OriginName = uri.AbsoluteUri,
                        PhotoSortOrder = 0,
                    });
                    return;
                }

                var photoname = uri.PathAndQuery.Trim('/').Replace("/", "-");
                photoname = Path.GetInvalidFileNameChars().Aggregate(photoname, (current, c) => current.Replace(c.ToString(), ""));

                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));

                if (string.IsNullOrWhiteSpace(photoname) ||
                    IsCategoryHasThisPhotoByName(categoryId, photoname, photoType) ||
                    !FileHelpers.DownloadRemoteImageFile(photo, FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoname)))
                {
                    //if error in download proccess
                    Log("не найдено изображение для категории: " + categoryName + " , путь к файлу: " + photo);
                    DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                    return;
                }

                photo = photoname;
            }

            photo = string.IsNullOrEmpty(photo) ? photo : photo.Trim();

            // temp picture folder
            var fullfilename = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photo);

            if (!File.Exists(fullfilename))
                return;

            PhotoService.DeletePhotos(categoryId, photoType);

            var tempName = PhotoService.AddPhoto(new Photo(0, categoryId, photoType) { OriginName = photo });
            if (!string.IsNullOrWhiteSpace(tempName))
            {
                using (Image image = Image.FromFile(fullfilename))
                {
                    FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageCategoryPathAbsolut(imageType, tempName),
                        width, height, image);
                }
            }
        }

        public static bool IsCategoryHasThisPhotoByName(int categoryId, string originName, PhotoType photoType)
        {
            var name = SQLDataAccess.ExecuteScalar<string>(
                    "select top 1 PhotoName from Catalog.Photo where ObjID=@categoryId and OriginName=@originName and type=@type",
                    CommandType.Text,
                    new SqlParameter("@categoryId", categoryId),
                    new SqlParameter("@originName", originName),
                    new SqlParameter("@type", photoType.ToString()));

            return name.IsNotEmpty();
        }


        #region Help methods

        private bool GetString(CategoryFields rEnum, ICsvReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (_fieldMapping.ContainsKey(nameField))
                categoryInStrings.Add(rEnum, TrimAnyWay(csv[_fieldMapping[nameField]]));
            return true;
        }

        private bool GetStringNotNull(CategoryFields rEnum, ICsvReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                categoryInStrings.Add(rEnum, tempValue);
            return true;
        }

        private bool GetStringRequired(CategoryFields rEnum, ICsvReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                categoryInStrings.Add(rEnum, tempValue);
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.CanNotEmpty"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDecimal(CategoryFields rEnum, ICsvReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            float decValue;
            if (float.TryParse(value, out decValue))
            {
                categoryInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                categoryInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDecimal(CategoryFields rEnum, ICsvReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
            {
                categoryInStrings.Add(rEnum, default(float?));
                return true;
            }

            float decValue;
            if (float.TryParse(value, out decValue))
            {
                categoryInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                categoryInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetInt(CategoryFields rEnum, ICsvReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            int intValue;
            if (int.TryParse(value, out intValue))
            {
                categoryInStrings.Add(rEnum, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDateTime(CategoryFields rEnum, ICsvReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = default(DateTime).ToString(CultureInfo.InvariantCulture);
            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                categoryInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                categoryInStrings.Add(rEnum, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDateTime(CategoryFields rEnum, ICsvReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);

            if (string.IsNullOrEmpty(value))
            {
                categoryInStrings.Add(rEnum, default(DateTime?));
                return true;
            }

            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                categoryInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                categoryInStrings.Add(rEnum, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private static string TrimAnyWay(string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Trim();
        }

        private void LogError(string message)
        {
            DoForCommonStatistic(() =>
            {
                CommonStatistic.WriteLog(message);
                CommonStatistic.TotalErrorRow++;
                CommonStatistic.RowPosition++;
            });
        }

        private void Log(string message)
        {
            DoForCommonStatistic(() => CommonStatistic.WriteLog(message));
        }

        protected void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }

        #endregion
    }

    public class CategoryImportMapping
    {
        /// <summary>
        /// categoryId in csv
        /// </summary>
        public string CsvCategoryId { get; set; }

        /// <summary>
        /// categoryId in db
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// position in csv file
        /// </summary>
        public long CsvRowIndex { get; set; }

        public CategoryImportMapping(string csvCategoryId, int categoryId, long csvRowIndex)
        {
            CsvCategoryId = csvCategoryId;
            CategoryId = categoryId;
            CsvRowIndex = csvRowIndex;
        }
    }
}