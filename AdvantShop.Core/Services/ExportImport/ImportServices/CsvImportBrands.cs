using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Repository;
using AdvantShop.Statistic;
using CsvHelper;

namespace AdvantShop.Core.Services.ExportImport.ImportServices
{
    public class CsvImportBrands
    {
        private readonly string _fullPath;
        private readonly bool _hasHeaders;
        private readonly string _columnSeparator;
        private readonly string _encodings;

        private Dictionary<string, int> _fieldMapping;
        private bool _useCommonStatistic;

        public CsvImportBrands(string fullPath, bool hasHeaders, string columnSeparator, string encodings,
            Dictionary<string, int> fieldMapping, bool useCommonStatistic = true)
        {
            _fullPath = fullPath;
            _hasHeaders = hasHeaders;
            _columnSeparator = columnSeparator;
            _encodings = encodings;
            _fieldMapping = fieldMapping;
            _useCommonStatistic = useCommonStatistic;
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var reader = new CsvReader(new StreamReader(_fullPath,
                Encoding.GetEncoding(_encodings ?? EncodingsEnum.Utf8.StrName())));

            reader.Configuration.Delimiter = _columnSeparator ?? SeparatorsEnum.SemicolonSeparated.StrName();
            reader.Configuration.HasHeaderRecord = hasHeaderRecord ?? _hasHeaders;

            return reader;
        }

        public List<string[]> ReadFirstRecord()
        {
            var list = new List<string[]>();
            using (var csv = InitReader())
            {
                var count = 0;
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

        public Task<bool> ProcessThroughACommonStatistic(string currentProcess, string currentProcessName,
            Action onBeforeImportAction = null)
        {
            return CommonStatistic.StartNew(() =>
                {
                    onBeforeImportAction?.Invoke();

                    _useCommonStatistic = true;
                    Process();
                },
                currentProcess,
                currentProcessName);
        }

        private void Process()
        {
            try
            {
                _process();
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
        }

        private void _process()
        {
            DoForCommonStatistic(() => CommonStatistic.WriteLog("Start of import"));

            if (_fieldMapping == null)
                MapFields();

            if (_fieldMapping == null)
                throw new Exception("can't map colums");

            DoForCommonStatistic(() => CommonStatistic.TotalRow = GetRowCount());

            ProcessRows();

            CacheManager.Clean();
            FileHelpers.DeleteFile(_fullPath);

            DoForCommonStatistic(() => CommonStatistic.WriteLog("End of import"));
        }

        private void MapFields()
        {
            _fieldMapping = new Dictionary<string, int>();
            using (var csv = InitReader(false))
            {
                csv.Read();
                for (var i = 0; i < csv.CurrentRecord.Length; i++)
                {
                    if (csv.CurrentRecord[i] == ELeadFields.None.StrName()) continue;
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

        private void ProcessRows()
        {
            if (!File.Exists(_fullPath)) return;

            using (var csv = InitReader())
            {
                while (csv.Read())
                {
                    if (_useCommonStatistic && (!CommonStatistic.IsRun || CommonStatistic.IsBreaking))
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }

                    try
                    {
                        var brandInString = PrepareRow(csv);
                        if (brandInString == null)
                        {
                            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                            continue;
                        }

                        ProcessBrand(brandInString);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                        DoForCommonStatistic(() =>
                        {
                            CommonStatistic.WriteLog($"{CommonStatistic.RowPosition}: {ex.Message}");
                            CommonStatistic.TotalErrorRow++;
                        });
                    }
                }
            }
        }

        private Dictionary<EBrandFields, object> PrepareRow(ICsvReaderRow csv)
        {
            var brandInStrings = new Dictionary<EBrandFields, object>();

            foreach (EBrandFields field in Enum.GetValues(typeof(EBrandFields)))
            {
                switch (field.Status())
                {
                    case CsvFieldStatus.String:
                        GetString(field, csv, brandInStrings);
                        break;
                    case CsvFieldStatus.Int:
                        GetInt(field, csv, brandInStrings);
                        break;
                }
            }

            return brandInStrings;
        }

        private void ProcessBrand(IDictionary brandInStrings)
        {
            try
            {
                var name = brandInStrings.Contains(EBrandFields.Name)
                    ? Convert.ToString(brandInStrings[EBrandFields.Name])
                    : string.Empty;
                var urlPath = brandInStrings.Contains(EBrandFields.UrlPath)
                    ? Convert.ToString(brandInStrings[EBrandFields.UrlPath])
                    : string.Empty;

                Brand brand = null;
                if (!string.IsNullOrEmpty(name))
                    brand = BrandService.GetBrandByName(name);
                if (brand == null)
                {
                    brand = new Brand
                    {
                        Name = name,
                        UrlPath = urlPath
                    };
                }

                if (brandInStrings.Contains(EBrandFields.Description))
                    brand.Description = Convert.ToString(brandInStrings[EBrandFields.Description]);
                if (brandInStrings.Contains(EBrandFields.BriefDescription))
                    brand.BriefDescription = Convert.ToString(brandInStrings[EBrandFields.BriefDescription]);
                if (brandInStrings.Contains(EBrandFields.Enabled))
                    brand.Enabled = Convert.ToBoolean(brandInStrings[EBrandFields.Enabled]);
                if (brandInStrings.Contains(EBrandFields.BrandSiteUrl))
                    brand.BrandSiteUrl = Convert.ToString(brandInStrings[EBrandFields.BrandSiteUrl]);

                if (brandInStrings.Contains(EBrandFields.CountryId))
                {
                    var countryId = Convert.ToInt32(brandInStrings[EBrandFields.CountryId]);
                    var country = CountryService.GetCountry(countryId);
                    if (country != null)
                        brand.CountryId = country.CountryId;
                }

                if (brand.CountryId.IsDefault() && brandInStrings.Contains(EBrandFields.CountryName))
                {
                    var countryName = Convert.ToString(brandInStrings[EBrandFields.CountryName]);
                    var country = CountryService.GetCountryByName(countryName);
                    if (country != null)
                        brand.CountryId = country.CountryId;
                }

                if (brandInStrings.Contains(EBrandFields.CountryOfManufactureId))
                {
                    var countryOfManufactureId = Convert.ToInt32(brandInStrings[EBrandFields.CountryOfManufactureId]);
                    var countryOfManufacture = CountryService.GetCountry(countryOfManufactureId);
                    if (countryOfManufacture != null)
                        brand.CountryOfManufactureId = countryOfManufacture.CountryId;
                }

                if (brand.CountryId.IsDefault() && brandInStrings.Contains(EBrandFields.CountryOfManufactureName))
                {
                    var countryOfManufactureName =
                        Convert.ToString(brandInStrings[EBrandFields.CountryOfManufactureName]);
                    var countryOfManufacture = CountryService.GetCountryByName(countryOfManufactureName);
                    if (countryOfManufacture != null)
                        brand.CountryOfManufactureId = countryOfManufacture.CountryId;
                }

                if (brand.ID.IsDefault())
                {
                    BrandService.AddBrand(brand);
                    DoForCommonStatistic(() =>
                    {
                        CommonStatistic.TotalAddRow++;
                        CommonStatistic.WriteLog($"Brand added: {brand.ID} - {brand.Name}");
                    });
                }
                else
                {
                    BrandService.UpdateBrand(brand);
                    DoForCommonStatistic(() =>
                    {
                        CommonStatistic.TotalUpdateRow++;
                        CommonStatistic.WriteLog($"Brand updated: {brand.ID} - {brand.Name}");
                    });
                }

                if (brand.ID != 0 && brandInStrings.Contains(EBrandFields.Photo))
                {
                    PhotoFromString(brand.ID, Convert.ToString(brandInStrings[EBrandFields.Photo]));
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                DoForCommonStatistic(() =>
                {
                    CommonStatistic.TotalErrorRow++;
                    CommonStatistic.WriteLog(CommonStatistic.RowPosition + ": " + ex.Message);
                });
            }

            brandInStrings.Clear();
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }

        private void PhotoFromString(int brandId, string photo)
        {
            if (photo.Contains("http://") || photo.Contains("https://"))
            {
                var uri = new Uri(photo);

                var photoname = uri.PathAndQuery.Split('?')[0].Trim('/').Replace("/", "-");
                photoname = Path.GetInvalidFileNameChars()
                    .Aggregate(photoname, (current, c) => current.Replace(c.ToString(), ""));

                if (photoname.Contains("."))
                {
                    var fileExtention = photoname.Substring(photoname.LastIndexOf('.'));

                    if (!FileHelpers.GetAllowedFileExtensions(EAdvantShopFileTypes.Photo).Contains(fileExtention))
                        return;
                }

                if (photoname.Length > 100)
                {
                    photoname = photoname.Length - 245 > 0
                        ? photoname.Substring(photoname.Length - 245)
                        : photoname;
                }

                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));

                var filename = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoname);

                if (!FileHelpers.DownloadRemoteImageFile(photo, filename))
                    return;

                AddBrandPhoto(brandId, filename);
            }
            else
            {
                var filename = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photo);

                if (!File.Exists(filename))
                    return;

                AddBrandPhoto(brandId, filename);
            }
        }

        private void AddBrandPhoto(int brandId, string filename)
        {
            try
            {
                PhotoService.DeletePhotos(brandId, PhotoType.Brand);

                var photo = new Photo(0, brandId, PhotoType.Brand) {OriginName = filename};
                var tempName = PhotoService.AddPhoto(photo);

                if (string.IsNullOrWhiteSpace(tempName)) return;

                using (var image = Image.FromFile(filename))
                {
                    FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.BrandLogo, tempName),
                        SettingsPictureSize.BrandLogoWidth, SettingsPictureSize.BrandLogoHeight, image);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        #region Help methods

        private void GetString(EBrandFields rEnum, ICsvReaderRow csv, IDictionary<EBrandFields, object> brandInStrings)
        {
            var nameField = rEnum.StrName().ToLower();
            if (_fieldMapping.ContainsKey(nameField))
                brandInStrings.Add(rEnum, TrimAnyWay(csv[_fieldMapping[nameField]]));
        }

        private void GetInt(EBrandFields rEnum, ICsvReaderRow csv, IDictionary<EBrandFields, object> brandInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            if (int.TryParse(value, out var intValue))
            {
                brandInStrings.Add(rEnum, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"),
                    rEnum.Localize(), CommonStatistic.RowPosition + 2));
            }
        }

        private void LogError(string message)
        {
            DoForCommonStatistic(() =>
            {
                CommonStatistic.WriteLog(message);
                CommonStatistic.TotalErrorRow++;
            });
        }

        private static string TrimAnyWay(string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Trim();
        }

        private void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }

        #endregion Help methods
    }
}
