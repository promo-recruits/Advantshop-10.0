using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.Statistic;
using AdvantShop.FullSearch;
using AdvantShop.Taxes;
using CsvHelper;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Diagnostics;
using AdvantShop.SEO;

namespace AdvantShop.ExportImport
{
    public class CsvImport
    {
        private readonly string _fullPath;
        private readonly bool _disablebProduct;
        private readonly bool _hasHeadrs;
        private readonly bool _skipOriginalPhoto;
        private Dictionary<string, int> _fieldMapping;
        private readonly string _separators;
        private readonly string _encodings;
        private readonly string _columnSeparator;
        private readonly string _propertySeparator;
        private readonly bool _importRemains;
        private readonly bool _updatePhotos;
        private readonly bool _updateNameAndDescription;
        private readonly bool _downloadRemotePhoto;
        private readonly bool _onlyUpdateProducts;
        private readonly int _defaultCurrencyId;
        private readonly bool _trackChanges;
        private readonly string _modifiedBy;
        private bool _useCommonStatistic;
        private bool _useMassPrecalc;

        private readonly Dictionary<ICSVExportImport, List<CSVField>> _modulesAndFields;

        private CsvImport(
            string filePath, 
            bool hasHeadrs, 
            bool disablebProduct, 
            string separators, 
            string encodings, 
            Dictionary<string, int> fieldMapping, 
            string columSeparator, 
            string propertySeparator, 
            bool skipOriginalPhoto, 
            bool importRemains, 
            bool updatePhotos, 
            bool updateNameAndDescription, 
            bool downloadRemotePhoto,
            bool useCommonStatistic,
            bool onlyUpdateProducts,
            bool trackChanges,
            string modifiedBy)
        {
            _fullPath = filePath;
            _hasHeadrs = hasHeadrs;
            _disablebProduct = disablebProduct;
            _fieldMapping = fieldMapping;
            _encodings = encodings;
            _separators = separators;
            _columnSeparator = columSeparator;
            _propertySeparator = propertySeparator;
            _skipOriginalPhoto = skipOriginalPhoto;
            _importRemains = importRemains;
            _updatePhotos = updatePhotos;
            _updateNameAndDescription = updateNameAndDescription;
            _downloadRemotePhoto = downloadRemotePhoto;
            _useCommonStatistic = useCommonStatistic;
            _onlyUpdateProducts = onlyUpdateProducts;
            _trackChanges = trackChanges;
            _modifiedBy = modifiedBy;

            _modulesAndFields = new Dictionary<ICSVExportImport, List<CSVField>>();
            foreach (var csvExportImportModule in AttachedModules.GetModules<ICSVExportImport>())
            {
                var classInstance = (ICSVExportImport)Activator.CreateInstance(csvExportImportModule, null);
                if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                {
                    var moduleFields = classInstance.GetCSVFields().ToList();
                    if (moduleFields.Any() && !_modulesAndFields.ContainsKey(classInstance))
                        _modulesAndFields.Add(classInstance, moduleFields);
                }
            }

            _defaultCurrencyId = SettingsCatalog.DefaultCurrency.CurrencyId;
        }

        public static CsvImport Factory(
            string filePath,
            bool hasHeadrs,
            bool disablebProduct,
            string separators,
            string encodings,
            Dictionary<string, int> fieldMapping,
            string columSeparator,
            string propertySeparator,
            bool skipOriginalPhoto = false,
            bool remains = false,
            bool updatePhotos = false,
            bool updateNameAndDescription = true,
            bool downloadRemotePhoto = true,
            bool useCommonStatistic = true,
            bool onlyUpdateProducts = false,
            bool trackChanges = false,
            string modifiedBy = null)
        {
            return new CsvImport(filePath, hasHeadrs, disablebProduct, separators, encodings, fieldMapping, columSeparator, propertySeparator,
                                 skipOriginalPhoto, remains, updatePhotos, updateNameAndDescription, downloadRemotePhoto, useCommonStatistic, 
                                 onlyUpdateProducts, trackChanges, modifiedBy);
        }

        public static CsvImport Factory(
            string filePath, 
            bool hasHeadrs, 
            bool skipOriginalPhoto = false, 
            bool remains = false, 
            bool updatePhotos = false, 
            bool updateNameAndDescription = true, 
            bool downloadRemotePhoto = true,
            bool useCommonStatistic = true,
            bool onlyUpdateProducts = false)
        {
            return new CsvImport(filePath, hasHeadrs, false, null, null, null, null, null, 
                                 skipOriginalPhoto, remains, updatePhotos, updateNameAndDescription, downloadRemotePhoto, useCommonStatistic, 
                                 onlyUpdateProducts, false, null);
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var reader = new CsvReader(new StreamReader(_fullPath, Encoding.GetEncoding(_encodings ?? EncodingsEnum.Utf8.StrName())));
            //var reader = new CsvReader(new StreamReader(_fullPath));
            reader.Configuration.Delimiter = _separators ?? SeparatorsEnum.SemicolonSeparated.StrName();
            if (hasHeaderRecord.HasValue)
                reader.Configuration.HasHeaderRecord = (bool)hasHeaderRecord;
            else
                reader.Configuration.HasHeaderRecord = _hasHeadrs;
            return reader;
        }

        public List<string[]> ReadFirst2()
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
        private void Process(Func<Product, Product> func = null, Action onBeforeMassActions = null)
        {
            try
            {
                _process(func, onBeforeMassActions);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
                DoForCommonStatistic(() =>
                {
                    CommonStatistic.WriteLog(ex.Message);
                    CommonStatistic.TotalErrorRow++;
                });
            }
        }

        public Task<bool> ProcessThroughACommonStatistic(
            string currentProcess, 
            string currentProcessName,
            Action onBeforeImportAction = null,
            Func<Product, Product> func = null,
            Action onBeforeMassActions = null,
            Action onAfterImportAction = null)
        {
            return CommonStatistic.StartNew(() =>
                {
                    if (onBeforeImportAction != null)
                        onBeforeImportAction();

                    _useCommonStatistic = true;
                    Process(func, onBeforeMassActions);

                    if (onAfterImportAction != null)
                        onAfterImportAction();
                },
                currentProcess,
                currentProcessName);
        }

        private void _process(Func<Product, Product> func = null, Action onBeforeMassActions = null)
        {
            Log(LocalizationService.GetResource("Core.ExportImport.ImportCsv.StartImport"));

            if (_fieldMapping == null)
                MapFields();

            if (_fieldMapping == null)
            {
                throw new Exception("can mapping colums");
            }

            var startAt = DateTime.Now;

            DoForCommonStatistic(() => CommonStatistic.TotalRow = GetRowCount());

            var somePostProcessing = _fieldMapping.ContainsKey(ProductFields.Related.StrName()) ||
                                     _fieldMapping.ContainsKey(ProductFields.Alternative.StrName()) ||
                                     _fieldMapping.ContainsKey(ProductFields.Gifts.StrName()) ||
                                     _fieldMapping.ContainsKey(ProductFields.Weight.StrName()) ||
                                     _fieldMapping.ContainsKey(ProductFields.Size.StrName()) ||
                                     _fieldMapping.ContainsKey(ProductFields.BarCode.StrName());

            foreach (var moduleFields in _modulesAndFields.Values)
            {
                somePostProcessing |= moduleFields.Any(moduleField => _fieldMapping.ContainsKey(moduleField.StrName));
            }

            if (somePostProcessing)
                DoForCommonStatistic(() => CommonStatistic.TotalRow *= 2);

            ProcessRows(false, _columnSeparator, _propertySeparator, func);
            if (somePostProcessing)
                ProcessRows(true, _columnSeparator, _propertySeparator);

            if (onBeforeMassActions != null)
                onBeforeMassActions();

            if (_disablebProduct)
            {
                Log(LocalizationService.GetResource("Core.ExportImport.ImportCsv.DisablingProducts"));
                ProductService.DisableAllProducts(startAt);
            }

            CategoryService.RecalculateProductsCountManual();
            CategoryService.SetCategoryHierarchicallyEnabled(0);
            LuceneSearch.CreateAllIndexInBackground();

            // PreCalcProductParamsMass was moved to adding/updating each product
            // but for gifts and other updates, we use it at the end
            if (_useMassPrecalc)
                ProductService.PreCalcProductParamsMassInBackground();

            CacheManager.Clean();
            FileHelpers.DeleteFilesFromImageTempInBackground();
            FileHelpers.DeleteFile(_fullPath);

            Log(LocalizationService.GetResource("Core.ExportImport.ImportCsv.ImportCompleted"));
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

        private void ProcessRows(bool onlyPostProcess, string columSeparator, string propertySeparator, Func<Product, Product> func = null)
        {
            if (!File.Exists(_fullPath))
                return;

            var productFields = PrepareProductFields();

            using (var csv = InitReader())
            {
                while (csv.Read())
                {
                    if ((!CommonStatistic.IsRun || CommonStatistic.IsBreaking) && _useCommonStatistic)
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }
                    try
                    {
                        var productInStrings = PrepareRow(productFields, csv);
                        if (productInStrings == null)
                        {
                            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                            continue;
                        }

                        if (!onlyPostProcess)
                            UpdateInsertProduct(productInStrings, columSeparator, propertySeparator, _skipOriginalPhoto, _importRemains, _updatePhotos, _updateNameAndDescription, _downloadRemotePhoto, _onlyUpdateProducts, func);
                        else
                            PostProcess(productInStrings, PrepareModuleRow(csv), _modulesAndFields, _columnSeparator, _propertySeparator);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Warn(ex);
                        DoForCommonStatistic(() =>
                        {
                            CommonStatistic.WriteLog(string.Format("{0}: {1}", CommonStatistic.RowPosition, ex.Message));
                            CommonStatistic.TotalErrorRow++;
                        });
                    }
                }
            }
        }

        private List<CsvProductFieldMapping> PrepareProductFields()
        {
            var productFields = new List<CsvProductFieldMapping>();

            foreach (ProductFields productField in Enum.GetValues(typeof(ProductFields)))
            {
                var name = productField.StrName();
                int index;
                if (!_fieldMapping.TryGetValue(name, out index))
                    continue;

                productFields.Add(new CsvProductFieldMapping()
                {
                    ProductFields = productField,
                    Name = name,
                    Index = index,
                    Status = productField.Status()
                });
            }

            return productFields;
        }

        private Dictionary<ProductFields, object> PrepareRow(List<CsvProductFieldMapping> productFields, ICsvReader csv)
        {
            // Step by rows
            var productInStrings = new Dictionary<ProductFields, object>();

            foreach (var productField in productFields)
            {
                switch (productField.Status)
                {
                    case CsvFieldStatus.String:
                        GetString(productField, csv, productInStrings);
                        break;
                    case CsvFieldStatus.StringRequired:
                        GetStringRequired(productField, csv, productInStrings);
                        break;
                    case CsvFieldStatus.NotEmptyString:
                        GetStringNotNull(productField, csv, productInStrings);
                        break;
                    case CsvFieldStatus.Float:
                        if (!GetDecimal(productField, csv, productInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.NullableFloat:
                        if (!GetNullableDecimal(productField, csv, productInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.Int:
                        if (!GetInt(productField, csv, productInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.DateTime:
                        if (!GetDateTime(productField, csv, productInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.NullableDateTime:
                        if (!GetNullableDateTime(productField, csv, productInStrings))
                            return null;
                        break;
                }
            }
            return productInStrings;
        }

        private Dictionary<CSVField, object> PrepareModuleRow(ICsvReader csv)
        {
            var productInStrings = new Dictionary<CSVField, object>();
            foreach (var moduleFields in _modulesAndFields.Values)
            {
                foreach (var moduleField in moduleFields)
                {
                    var nameField = moduleField.StrName;
                    if (_fieldMapping.ContainsKey(nameField))
                        productInStrings.Add(moduleField, TrimAnyWay(csv[_fieldMapping[nameField]]));
                }
            }
            return productInStrings;
        }

        private bool GetString(CsvProductFieldMapping field, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            productInStrings.Add(field.ProductFields, TrimAnyWay(csv[field.Index]));
            return true;
        }

        private bool GetStringNotNull(CsvProductFieldMapping field, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var tempValue = TrimAnyWay(csv[field.Index]);
            if (!string.IsNullOrEmpty(tempValue))
                productInStrings.Add(field.ProductFields, tempValue);

            return true;
        }

        private bool GetStringRequired(CsvProductFieldMapping field, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);

            if (!string.IsNullOrEmpty(value))
                productInStrings.Add(field.ProductFields, value);
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.CanNotEmpty"), ProductFields.Name.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDecimal(CsvProductFieldMapping field, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);

            if (string.IsNullOrEmpty(value))
                value = "0";

            float decValue;
            if (float.TryParse(value, out decValue))
            {
                productInStrings.Add(field.ProductFields, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                productInStrings.Add(field.ProductFields, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDecimal(CsvProductFieldMapping field, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);

            if (string.IsNullOrEmpty(value))
            {
                productInStrings.Add(field.ProductFields, default(float?));
                return true;
            }

            float decValue;
            if (float.TryParse(value, out decValue))
            {
                productInStrings.Add(field.ProductFields, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                productInStrings.Add(field.ProductFields, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetInt(CsvProductFieldMapping field, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);
            if (string.IsNullOrEmpty(value))
                value = "0";

            int intValue;
            if (int.TryParse(value, out intValue))
            {
                productInStrings.Add(field.ProductFields, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDateTime(CsvProductFieldMapping field, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);
            if (string.IsNullOrEmpty(value))
                value = default(DateTime).ToString(CultureInfo.InvariantCulture);

            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                productInStrings.Add(field.ProductFields, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                productInStrings.Add(field.ProductFields, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDateTime(CsvProductFieldMapping field, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);

            if (string.IsNullOrEmpty(value))
            {
                productInStrings.Add(field.ProductFields, default(DateTime?));
                return true;
            }

            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                productInStrings.Add(field.ProductFields, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                productInStrings.Add(field.ProductFields, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private static string TrimAnyWay(string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.SupperTrim();
        }

        private void LogError(string message)
        {
            DoForCommonStatistic(() =>
            {
                CommonStatistic.WriteLog(message);
                CommonStatistic.TotalErrorRow++;
                //CommonStatistic.RowPosition++;
            });
        }

        private static bool useMultiThreadImport = false;

        public void UpdateInsertProduct(Dictionary<ProductFields, object> productInStrings, string columSeparator, string propertySeparator, bool skipOriginalPhoto, bool importRemains, bool updatePhotos, bool updateNameAndDescription, bool downloadRemotePhoto, bool onlyUpdateProducts, Func<Product, Product> func = null)
        {
            if (useMultiThreadImport)
            {
                var added = false;
                while (!added)
                {
                    int workerThreads;
                    int asyncIoThreads;
                    ThreadPool.GetAvailableThreads(out workerThreads, out asyncIoThreads);
                    if (workerThreads != 0)
                    {
                        //ThreadPool.QueueUserWorkItem(UpdateInsertProductWorker, productInStrings);
                        Task.Factory.StartNew(() => UpdateInsertProductWorker(columSeparator, propertySeparator, productInStrings, skipOriginalPhoto, importRemains, updatePhotos, updateNameAndDescription, downloadRemotePhoto, onlyUpdateProducts, func), TaskCreationOptions.LongRunning);
                        added = true;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            else
            {
                UpdateInsertProductWorker(columSeparator, propertySeparator, productInStrings, skipOriginalPhoto, importRemains, updatePhotos, updateNameAndDescription, downloadRemotePhoto, onlyUpdateProducts, func);
            }
        }

        private void UpdateInsertProductWorker(string columSeparator, string propertySeparator, Dictionary<ProductFields, object> productInStrings, 
                                               bool skipOriginalPhoto, bool importRemains, bool updatePhotos, bool updateNameAndDescription, bool downloadRemotePhoto, 
                                               bool onlyUpdateProducts, Func<Product, Product> func = null)
        {
            try
            {
                bool addingNew;
                Product product = null;
                if (productInStrings.ContainsKey(ProductFields.Sku) && productInStrings[ProductFields.Sku].AsString().IsNullOrEmpty())
                    throw new Exception("SKU can not be empty");

                var artNo = productInStrings.ContainsKey(ProductFields.Sku) ? productInStrings[ProductFields.Sku].AsString().SupperTrim() : string.Empty;
                if (string.IsNullOrEmpty(artNo))
                {
                    addingNew = true;
                }
                else
                {
                    product = ProductService.GetProduct(artNo);
                    addingNew = product == null;
                }

                if (addingNew)
                {
                    if (onlyUpdateProducts)
                    {
                        productInStrings.Clear();
                        DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                        return;
                    }

                    product = new Product
                    {
                        ArtNo = string.IsNullOrEmpty(artNo) ? null : artNo,
                        Multiplicity = 1,
                        CurrencyID = _defaultCurrencyId,
                        Enabled = true
                    };
                }

                if (updateNameAndDescription || addingNew)
                {
                    if (productInStrings.ContainsKey(ProductFields.Name))
                        product.Name = productInStrings[ProductFields.Name].AsString();
                    else
                        product.Name = product.Name ?? string.Empty;
                }

                if (productInStrings.ContainsKey(ProductFields.Enabled))
                {
                    product.Enabled = productInStrings[ProductFields.Enabled].AsString().SupperTrim().Equals("+");
                }

                if (productInStrings.ContainsKey(ProductFields.Currency))
                {
                    var currency = CurrencyService.GetCurrencyByIso3(productInStrings[ProductFields.Currency].AsString().SupperTrim());
                    if (currency != null)
                        product.CurrencyID = currency.CurrencyId;
                    else
                        throw new Exception("Currency not found");
                }
                else if (addingNew)
                { 
                    var currencies = CurrencyService.GetAllCurrencies(true);
                    if (currencies == null || currencies.Count == 0)
                        throw new Exception("Currency not found");

                    var currency = currencies.FirstOrDefault(x => x.Rate == 1) ??
                                   currencies.FirstOrDefault(x => x.Iso3 == SettingsCatalog.DefaultCurrencyIso3);

                    if (currency != null)
                        product.CurrencyID = currency.CurrencyId;
                    else
                        throw new Exception("Currency not found");
                }


                if (productInStrings.ContainsKey(ProductFields.OrderByRequest))
                    product.AllowPreOrder = productInStrings[ProductFields.OrderByRequest].AsString().SupperTrim().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.Discount) ||
                    productInStrings.ContainsKey(ProductFields.DiscountAmount))
                {
                    var percent = !productInStrings.ContainsKey(ProductFields.Discount) || productInStrings[ProductFields.Discount] == null
                        ? product.Discount.Percent
                        : productInStrings[ProductFields.Discount].AsFloat();

                    var amount = !productInStrings.ContainsKey(ProductFields.DiscountAmount) || productInStrings[ProductFields.DiscountAmount] == null
                        ? product.Discount.Amount
                        : productInStrings[ProductFields.DiscountAmount].AsFloat();

                    product.Discount = new Discount(percent, amount);
                }

                if (updateNameAndDescription || addingNew)
                {
                    if (productInStrings.ContainsKey(ProductFields.BriefDescription))
                    {
                        var descr = productInStrings[ProductFields.BriefDescription].AsString();
                        if (descr.IsLongerThan(ProductService.MaxDescLength))
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.TextLengthLimit", ProductFields.BriefDescription.Localize(), CommonStatistic.RowPosition + 2, ProductService.MaxDescLength));
                        else
                            product.BriefDescription = descr;
                    }

                    if (productInStrings.ContainsKey(ProductFields.Description))
                    {
                        var descr = productInStrings[ProductFields.Description].AsString();
                        if (descr.IsLongerThan(ProductService.MaxDescLength))
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.TextLengthLimit", ProductFields.Description.Localize(), CommonStatistic.RowPosition + 2, ProductService.MaxDescLength));
                        else
                            product.Description = descr;
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.SalesNotes))
                    product.SalesNote = productInStrings[ProductFields.SalesNotes].AsString();

                if (productInStrings.ContainsKey(ProductFields.Gtin))
                    product.Gtin = productInStrings[ProductFields.Gtin].AsString();

                if (productInStrings.ContainsKey(ProductFields.GoogleProductCategory))
                    product.GoogleProductCategory = productInStrings[ProductFields.GoogleProductCategory].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexTypePrefix))
                    product.YandexTypePrefix = productInStrings[ProductFields.YandexTypePrefix].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexModel))
                    product.YandexModel = productInStrings[ProductFields.YandexModel].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexName))
                    product.YandexName = productInStrings[ProductFields.YandexName].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexSizeUnit))
                    product.YandexSizeUnit = productInStrings[ProductFields.YandexSizeUnit].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexDiscounted))
                    product.YandexProductDiscounted = productInStrings[ProductFields.YandexDiscounted].AsString().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.YandexDiscountCondition))
                {
                    var yandexDiscountCondition = productInStrings[ProductFields.YandexDiscountCondition].AsString();
                    if (!string.IsNullOrEmpty(yandexDiscountCondition))
                    {
                        var condition = Enum.GetValues(typeof(EYandexDiscountCondition)).Cast<EYandexDiscountCondition>().FirstOrDefault(x => yandexDiscountCondition.Equals(x.StrName(), StringComparison.OrdinalIgnoreCase));
                        if (condition != EYandexDiscountCondition.None)
                            product.YandexProductDiscountCondition = condition;
                        else
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", ProductFields.YandexDiscountCondition.Localize(), yandexDiscountCondition, CommonStatistic.RowPosition + 2));
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.YandexDiscountReason))
                    product.YandexProductDiscountReason = productInStrings[ProductFields.YandexDiscountReason].AsString();

                if (productInStrings.ContainsKey(ProductFields.Adult))
                    product.Adult = productInStrings[ProductFields.Adult].AsString().SupperTrim().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.ManufacturerWarranty))
                    product.ManufacturerWarranty = productInStrings[ProductFields.ManufacturerWarranty].AsString().SupperTrim().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.ShippingPrice))
                    product.ShippingPrice = productInStrings[ProductFields.ShippingPrice].AsNullableFloat();

                if (productInStrings.ContainsKey(ProductFields.YandexDeliveryDays))
                {
                    var yandexDeliveryDays = productInStrings[ProductFields.YandexDeliveryDays].AsString();
                    product.YandexDeliveryDays = yandexDeliveryDays.Length > 5
                        ? yandexDeliveryDays.Substring(0, 5)
                        : yandexDeliveryDays;
                }

                if (productInStrings.ContainsKey(ProductFields.Unit))
                    product.Unit = productInStrings[ProductFields.Unit].AsString();


                if (productInStrings.ContainsKey(ProductFields.MultiOffer))
                {
                    OfferService.OffersFromString(product, productInStrings[ProductFields.MultiOffer].AsString(), columSeparator, propertySeparator, importRemains);
                }
                else
                {
                    OfferService.OfferFromFields(product, productInStrings.ContainsKey(ProductFields.Price) ? productInStrings[ProductFields.Price].AsFloat() : (float?)null,
                                                 productInStrings.ContainsKey(ProductFields.PurchasePrice) ? productInStrings[ProductFields.PurchasePrice].AsFloat() : (float?)null,
                                                 productInStrings.ContainsKey(ProductFields.Amount) ? productInStrings[ProductFields.Amount].AsFloat() : (float?)null, importRemains);
                }

                if (productInStrings.ContainsKey(ProductFields.ParamSynonym))
                {
                    var prodUrl = productInStrings[ProductFields.ParamSynonym].AsString().IsNotEmpty()
                                      ? productInStrings[ProductFields.ParamSynonym].AsString()
                                      : !string.IsNullOrEmpty(product.Name) ? product.Name : product.ArtNo;

                    product.UrlPath = UrlService.GetAvailableValidUrl(product.ProductId, ParamType.Product, prodUrl);
                }
                else
                {
                    product.UrlPath = product.UrlPath ??
                                      UrlService.GetAvailableValidUrl(product.ProductId, ParamType.Product,
                                          !string.IsNullOrEmpty(product.Name) ? product.Name : product.ArtNo);

                }

                var meta = (!addingNew ? MetaInfoService.GetMetaInfo(product.ProductId, MetaType.Product) : null) ?? new MetaInfo();
                product.Meta = new MetaInfo(0, product.ProductId, MetaType.Product,
                    (productInStrings.ContainsKey(ProductFields.Title) 
                        ? productInStrings[ProductFields.Title].AsString() 
                        : meta.Title).DefaultOrEmpty(),
                    (productInStrings.ContainsKey(ProductFields.MetaKeywords)
                        ? productInStrings[ProductFields.MetaKeywords].AsString()
                        : meta.MetaKeywords).DefaultOrEmpty(),
                    (productInStrings.ContainsKey(ProductFields.MetaDescription)
                        ? productInStrings[ProductFields.MetaDescription].AsString()
                        : meta.MetaDescription).DefaultOrEmpty(),
                    (productInStrings.ContainsKey(ProductFields.H1)
                        ? productInStrings[ProductFields.H1].AsString()
                        : meta.H1).DefaultOrEmpty());

                if (productInStrings.ContainsKey(ProductFields.Markers))
                    ProductService.MarkersFromString(product, productInStrings[ProductFields.Markers].AsString(), columSeparator);

                if (productInStrings.ContainsKey(ProductFields.Producer))
                    product.BrandId = BrandService.BrandFromString(productInStrings[ProductFields.Producer].AsString());

                if (productInStrings.ContainsKey(ProductFields.MinAmount))
                    product.MinAmount = productInStrings[ProductFields.MinAmount].AsFloat() != 0 ? productInStrings[ProductFields.MinAmount].AsFloat() : (float?)null;

                if (productInStrings.ContainsKey(ProductFields.MaxAmount))
                    product.MaxAmount = productInStrings[ProductFields.MaxAmount].AsFloat() != 0 ? productInStrings[ProductFields.MaxAmount].AsFloat() : (float?)null;

                if (productInStrings.ContainsKey(ProductFields.Multiplicity))
                    product.Multiplicity = productInStrings[ProductFields.Multiplicity].AsFloat() != 0 ? productInStrings[ProductFields.Multiplicity].AsFloat() : 1;

                //if (productInStrings.ContainsKey(ProductFields.Fee))
                //product.Fee = productInStrings[ProductFields.Fee].AsFloat() != 0 ? productInStrings[ProductFields.Fee].AsFloat() : 0;

                if (productInStrings.ContainsKey(ProductFields.Bid))
                    product.Bid = productInStrings[ProductFields.Bid].AsFloat() != 0 ? productInStrings[ProductFields.Bid].AsFloat() : 0;

                //if (productInStrings.ContainsKey(ProductFields.Cpa))
                //product.Cpa = productInStrings[ProductFields.Cpa].AsString().Trim().Equals("-");

                #region sales channels

                //SalesChannelService.DeleteExcludedProductSalesChannels(product.ProductId);

                //if (productInStrings.ContainsKey(ProductFields.Store) && productInStrings[ProductFields.Store].AsString().Trim().Equals("-"))
                //{
                //    SalesChannelService.SetExcludedProductSalesChannel(ESalesChannelsKeys.Store.ToString(), product.ProductId);
                //}
                //if (productInStrings.ContainsKey(ProductFields.Funnel) && productInStrings[ProductFields.Funnel].AsString().Trim().Equals("-"))
                //{
                //    SalesChannelService.SetExcludedProductSalesChannel(ESalesChannelsKeys.Funnel.ToString(), product.ProductId);
                //}
                //if (productInStrings.ContainsKey(ProductFields.Vk)&& productInStrings[ProductFields.Vk].AsString().Trim().Equals("-"))
                //{
                //    SalesChannelService.SetExcludedProductSalesChannel(ESalesChannelsKeys.Vk.ToString(), product.ProductId);
                //}
                //if (productInStrings.ContainsKey(ProductFields.Instagram)&& productInStrings[ProductFields.Instagram].AsString().Trim().Equals("-"))
                //{
                //    SalesChannelService.SetExcludedProductSalesChannel(ESalesChannelsKeys.Instagram.ToString(), product.ProductId);
                //}
                //if (productInStrings.ContainsKey(ProductFields.Yandex)&& productInStrings[ProductFields.Yandex].AsString().Trim().Equals("-"))
                //{
                //    SalesChannelService.SetExcludedProductSalesChannel(ESalesChannelsKeys.Yandex.ToString(), product.ProductId);
                //}
                //if (productInStrings.ContainsKey(ProductFields.Avito)&& productInStrings[ProductFields.Avito].AsString().Trim().Equals("-"))
                //{
                //    SalesChannelService.SetExcludedProductSalesChannel(ESalesChannelsKeys.Avito.ToString(), product.ProductId);
                //}
                //if (productInStrings.ContainsKey(ProductFields.Google)&& productInStrings[ProductFields.Google].AsString().Trim().Equals("-"))
                //{
                //    SalesChannelService.SetExcludedProductSalesChannel(ESalesChannelsKeys.Google.ToString(), product.ProductId);
                //}
                //if (productInStrings.ContainsKey(ProductFields.Facebook)&& productInStrings[ProductFields.Facebook].AsString().Trim().Equals("-"))
                //{
                //    SalesChannelService.SetExcludedProductSalesChannel(ESalesChannelsKeys.Facebook.ToString(), product.ProductId);
                //}
                //if (productInStrings.ContainsKey(ProductFields.Bonus)&& productInStrings[ProductFields.Bonus].AsString().Trim().Equals("-"))
                //{
                //    SalesChannelService.SetExcludedProductSalesChannel(ESalesChannelsKeys.Bonus.ToString(), product.ProductId);
                //}
                //if (productInStrings.ContainsKey(ProductFields.Referal)&& productInStrings[ProductFields.Referal].AsString().Trim().Equals("-"))
                //{
                //    SalesChannelService.SetExcludedProductSalesChannel(ESalesChannelsKeys.Referal.ToString(), product.ProductId);
                //}

                #endregion

                if (productInStrings.ContainsKey(ProductFields.Tax))
                {
                    var taxName = productInStrings[ProductFields.Tax].AsString();
                    if (!string.IsNullOrEmpty(taxName))
                    {
                        var tax = TaxService.GetTaxes().FirstOrDefault(x => x.Name.ToLower() == taxName.ToLower());
                        if (tax != null)
                            product.TaxId = tax.TaxId;
                        else
                        {
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", 
                                ProductFields.Tax.Localize(), taxName, CommonStatistic.RowPosition + 2));
                        }
                    }
                    else
                    {
                        product.TaxId = null;
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.PaymentMethodType))
                {
                    var paymentMethodType = productInStrings[ProductFields.PaymentMethodType].AsString();
                    if (!string.IsNullOrEmpty(paymentMethodType))
                    {
                        ePaymentMethodType type;
                        if (Enum.TryParse<ePaymentMethodType>(paymentMethodType, out type))
                            product.PaymentMethodType = type;
                        else
                        {
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", 
                                ProductFields.PaymentMethodType.Localize(), paymentMethodType, CommonStatistic.RowPosition + 2));
                        }
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.PaymentSubjectType))
                {
                    var paymentSubjectType = productInStrings[ProductFields.PaymentSubjectType].AsString();
                    if (!string.IsNullOrEmpty(paymentSubjectType))
                    {
                        ePaymentSubjectType type;
                        if (Enum.TryParse<ePaymentSubjectType>(paymentSubjectType, out type))
                            product.PaymentSubjectType = type;
                        else
                        {
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound",
                                ProductFields.PaymentSubjectType.Localize(), paymentSubjectType, CommonStatistic.RowPosition + 2));
                        }
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.ManualRatio))
                {
                    var ratio = productInStrings[ProductFields.ManualRatio].AsFloat();
                    if(ratio > 0 && ratio <= 5)
                    {
                        product.ManualRatio = ratio;
                    }
                }

                product.ModifiedBy = !string.IsNullOrEmpty(_modifiedBy) ? _modifiedBy : "CSV Import";


                if (func != null)
                    product = func(product);

                if (product.Offers != null && product.Offers.Count != 0)
                {
                    var artNoExist = false;
                    foreach (var offer in product.Offers)
                        if (OfferService.IsArtNoExist(offer.ArtNo, offer.OfferId))
                        {
                            artNoExist = true;
                            Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.OfferSkuIsBusy", offer.ArtNo));
                            break;
                        }

                    if (artNoExist)
                        product.Offers = OfferService.GetProductOffers(product.ProductId);
                }

                if (!addingNew)
                {
                    ProductService.UpdateProduct(product, false, _trackChanges, new ChangedBy(product.ModifiedBy));

                    if (!_trackChanges)
                        ProductHistoryService.ProductChanged(product, new ChangedBy(product.ModifiedBy));

                    DoForCommonStatistic(() => CommonStatistic.TotalUpdateRow++);
                    Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProductUpdated", product.ArtNo));
                }
                else
                {
                    if (!(SaasDataService.IsSaasEnabled && ProductService.GetProductsCount("[Enabled] = 1") >= SaasDataService.CurrentSaasData.ProductsCount))
                    {
                        ProductService.AddProduct(product, false, true, new ChangedBy(product.ModifiedBy));
                        DoForCommonStatistic(() => CommonStatistic.TotalAddRow++);
                        Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProductAdded", product.ArtNo));
                    }
                    else
                    {
                        Log(LocalizationService.GetResource("Core.ExportImport.ImportCsv.ProductsLimitRiched"));
                    }
                }

                if (product.ProductId > 0)
                {
                    OtherFields(productInStrings, product.ProductId, columSeparator, propertySeparator,
                        skipOriginalPhoto, updatePhotos, downloadRemotePhoto, product.ArtNo);
                    ProductService.PreCalcProductParams(product.ProductId);
                }
                else
                {
                    LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProductNotAdded", product.ArtNo));
                }
            }
            catch (Exception e)
            {
                Debug.Log.Warn(e);
                LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProcessRowError", CommonStatistic.RowPosition + 2, e.Message));
            }

            productInStrings.Clear();
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }


        private void OtherFields(IDictionary<ProductFields, object> fields, int productId, string columSeparator, string propertySeparator, bool skipOriginalPhoto, bool updatePhotos, bool downloadRemotePhoto, string productArtNo)
        {
            //Category
            if (fields.ContainsKey(ProductFields.Category))
            {
                string sorting = string.Empty;
                if (fields.ContainsKey(ProductFields.Sorting))
                {
                    sorting = fields[ProductFields.Sorting].AsString();
                }
                var parentCategory = fields[ProductFields.Category].AsString();

                CategoryService.SubParseAndCreateCategory(parentCategory, productId, columSeparator, sorting, "CSV Import", _trackChanges);
            }

            if (fields.ContainsKey(ProductFields.ExternalCategoryId))
            {
                var sorting = fields.ContainsKey(ProductFields.Sorting)
                    ? fields[ProductFields.Sorting].AsString()
                    : string.Empty;

                var externalCategories = fields[ProductFields.ExternalCategoryId].AsString();
                CategoryService.AddProductToExternalCategories(externalCategories, productId, columSeparator, sorting);
            }

            //photo
            if (fields.ContainsKey(ProductFields.Photos))
            {
                string photos = fields[ProductFields.Photos].AsString();
                if (!string.IsNullOrEmpty(photos))
                {
                    if (updatePhotos)
                    {
                        PhotoService.DeleteProductPhotos(productId);
                    }
                    if (!PhotoService.PhotoFromString(productId, photos, columSeparator, propertySeparator, skipOriginalPhoto, downloadRemotePhoto))
                        Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.NotAllPhotosProcessed", productArtNo));
                }
            }

            //video
            if (fields.ContainsKey(ProductFields.Videos))
            {
                string videos = fields[ProductFields.Videos].AsString();
                ProductVideoService.VideoFromString(productId, videos);
            }

            //Properties
            if (fields.ContainsKey(ProductFields.Properties))
            {
                string properties = fields[ProductFields.Properties].AsString();
                PropertyService.PropertiesFromString(productId, properties, columSeparator, propertySeparator);
            }

            if (fields.ContainsKey(ProductFields.AvitoProductProperties))
            {
                string avitoProductProperties = fields[ProductFields.AvitoProductProperties].AsString();
                ExportFeedAvitoService.ImportProductProperties(productId, avitoProductProperties, columSeparator, propertySeparator);
            }

            if (fields.ContainsKey(ProductFields.CustomOption))
            {
                string customOption = fields[ProductFields.CustomOption].AsString();
                CustomOptionsService.CustomOptionsFromString(productId, customOption);
            }

            if (fields.ContainsKey(ProductFields.Tags) &&
                   (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveTags))
            {
                TagService.DeleteMap(productId, ETagType.Product);

                var i = 0;

                foreach (var tagName in fields[ProductFields.Tags].AsString().Split(new[] { columSeparator }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var trimTagName = tagName.SupperTrim();
                    var tag = TagService.Get(trimTagName);
                    if (tag == null)
                    {
                        var tagId = TagService.Add(new Tag
                        {
                            Name = trimTagName,
                            Enabled = true,
                            UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Tag, trimTagName)
                        });
                        TagService.AddMap(productId, tagId, ETagType.Product, i * 10);
                    }
                    else
                    {
                        TagService.AddMap(productId, tag.Id, ETagType.Product, i * 10);
                    }
                    i++;
                }
            }
        }

        private void Log(string message)
        {
            DoForCommonStatistic(() => CommonStatistic.WriteLog(message));
        }

        public void PostProcess(Dictionary<ProductFields, object> productInStrings, Dictionary<CSVField, object> moduleFieldValues, Dictionary<ICSVExportImport, List<CSVField>> modulesAndFields, string columnSeparator, string propertySeparator)
        {
            if (productInStrings.ContainsKey(ProductFields.Sku))
            {
                var artNo = productInStrings[ProductFields.Sku].AsString();
                int productId = ProductService.GetProductId(artNo);
                if (productId == 0)
                    return;

                //relations
                if (productInStrings.ContainsKey(ProductFields.Related))
                {
                    var linkproducts = productInStrings[ProductFields.Related].AsString();
                    ProductService.LinkedProductFromString(productId, linkproducts, RelatedType.Related, columnSeparator);
                }

                //relations
                if (productInStrings.ContainsKey(ProductFields.Alternative))
                {
                    var linkproducts = productInStrings[ProductFields.Alternative].AsString();
                    ProductService.LinkedProductFromString(productId, linkproducts, RelatedType.Alternative, columnSeparator);
                }

                //gifts
                if (productInStrings.ContainsKey(ProductFields.Gifts))
                {
                    _useMassPrecalc = true;
                    var linkproducts = productInStrings[ProductFields.Gifts].AsString();
                    OfferService.ProductGiftsFromString(productId, linkproducts, columnSeparator);
                }

                var productOffers = ((productInStrings.ContainsKey(ProductFields.Weight) || productInStrings.ContainsKey(ProductFields.Size)) && !FeaturesService.IsEnabled(EFeature.OfferWeightAndDimensions)) ||
                                    (productInStrings.ContainsKey(ProductFields.BarCode) && !FeaturesService.IsEnabled(EFeature.OfferBarCode))
                        ? OfferService.GetProductOffers(productId)
                        : null;
                bool changedProductOffers = false;


                if (productInStrings.ContainsKey(ProductFields.Weight))
                {
                    if (!FeaturesService.IsEnabled(EFeature.OfferWeightAndDimensions))
                    {
                        var weight = productInStrings[ProductFields.Weight].AsFloat();

                        foreach (var offer in productOffers)
                        {
                            offer.Weight = weight;
                            changedProductOffers = true;
                        }
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.Size))
                {
                    if (!FeaturesService.IsEnabled(EFeature.OfferWeightAndDimensions))
                    {
                        var dimensions = productInStrings[ProductFields.Size].AsString().Split(new[] { '|', 'x', 'х' }).Select(x => x.TryParseFloat()).ToList();

                        var length = dimensions.Count > 0 ? dimensions[0] : 0;
                        var width = dimensions.Count > 1 ? dimensions[1] : 0;
                        var height = dimensions.Count > 2 ? dimensions[2] : 0;

                        foreach (var offer in productOffers)
                        {
                            offer.Length = length;
                            offer.Width = width;
                            offer.Height = height;

                            changedProductOffers = true;
                        }
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.BarCode))
                {
                    if (!FeaturesService.IsEnabled(EFeature.OfferBarCode))
                    {
                        var barCode = productInStrings[ProductFields.BarCode].AsString();

                        foreach (var offer in productOffers)
                        {
                            offer.BarCode = barCode;
                            changedProductOffers = true;
                        }
                    }
                }

                if (changedProductOffers)
                    foreach (var offer in productOffers)
                        OfferService.UpdateOffer(offer);

                // modules
                foreach (var moduleInstance in modulesAndFields.Keys)
                {
                    foreach (var moduleField in modulesAndFields[moduleInstance].Where(moduleFieldValues.ContainsKey))
                    {
                        try
                        {
                            if (!moduleInstance.ProcessField(moduleField, productId, moduleFieldValues[moduleField].AsString(), columnSeparator, propertySeparator))
                                Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ModuleError", artNo, moduleInstance.ModuleStringId));
                        }
                        catch (Exception ex)
                        {
                            Debug.Log.Error("Csv import error during post-processing of module fields", ex);
                            Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ModuleError", artNo, moduleInstance.ModuleStringId));
                        }
                    }
                }
            }
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }

        protected void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }

        public class CsvProductFieldMapping
        {
            public ProductFields ProductFields { get; set; }

            public string Name { get; set; }

            public CsvFieldStatus Status { get; set; }
            public int Index { get; set; }
        }
    }
}