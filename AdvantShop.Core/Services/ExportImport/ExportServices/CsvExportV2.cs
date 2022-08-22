using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.Statistic;
using CsvHelper;
using Newtonsoft.Json;

namespace AdvantShop.ExportImport
{
    public class CsvExportV2
    {
        private const int MaxCellLength = 60000;
        private readonly string _path;
        private readonly string _encodeType;
        private readonly string _delimiter;
        private readonly string _columSeparator;
        private readonly string _propertySeparator;
        private readonly List<EProductField> _fieldMapping;
        private readonly List<CSVField> _modulesFieldMapping;
        private readonly bool _csvExportNoInCategory;
        private readonly bool _csvExportSorting;
        private bool _useCommonStatistic;

        private readonly int _productsCount;
        //private readonly int _categoriesCount;
        private readonly int _maxProductCategoriesCount;
        private readonly List<string> _propertyNames;

        private readonly Dictionary<ICSVExportImport, List<CSVField>> _mappedExportImportModules;

        private readonly IEnumerable<ExportFeedProductModel> _products;

        private CsvExportV2(IEnumerable<ExportFeedProductModel> products, ExportFeedSettings options, int productsCount, int categoriesCount, bool useCommonStatistic)
        {
            var csvOptions = JsonConvert.DeserializeObject<ExportFeedCsvV2Options>(options.AdvancedSettings);

            _path = options.FileFullPath;
            _encodeType = csvOptions.CsvEnconing.Default(EncodingsEnum.Windows1251.StrName());
            var delimiter = csvOptions.CsvSeparator.IsNotEmpty() ? csvOptions.CsvSeparator.TryParseEnum<SeparatorsEnum>() : SeparatorsEnum.SemicolonSeparated;
            _delimiter = delimiter != SeparatorsEnum.Custom ? delimiter.StrName() : csvOptions.CsvSeparatorCustom;
            _columSeparator = csvOptions.CsvColumSeparator;
            _propertySeparator = csvOptions.CsvPropertySeparator;
            _fieldMapping = csvOptions.FieldMapping;
            _csvExportNoInCategory = csvOptions.CsvExportNoInCategory;
            _modulesFieldMapping = csvOptions.ModuleFieldMapping ?? new List<CSVField>();
            _csvExportSorting = csvOptions.CsvCategorySort;
            _useCommonStatistic = useCommonStatistic;
            _products = products;

            _productsCount = productsCount;
            //_categoriesCount = categoriesCount;
            _maxProductCategoriesCount = ExportFeedCsvV2Service.GetMaxProductCategoriesCount(options.ExportFeedId, options, csvOptions);

            _propertyNames =
                _fieldMapping.Contains(EProductField.Property)
                    ? ExportFeedCsvV2Service.GetProductPropertyNames(options.ExportFeedId, options, csvOptions)
                    : new List<string>();

            _mappedExportImportModules = new Dictionary<ICSVExportImport, List<CSVField>>();
            foreach (var csvExportImportModule in AttachedModules.GetModules<ICSVExportImport>())
            {
                var classInstance = (ICSVExportImport)Activator.CreateInstance(csvExportImportModule, null);
                if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                {
                    var mappedModuleFields = classInstance.GetCSVFields().Where(mf => _modulesFieldMapping.Select(f => f.StrName).Contains(mf.StrName)).ToList();
                    if (mappedModuleFields.Any() && !_mappedExportImportModules.ContainsKey(classInstance))
                        _mappedExportImportModules.Add(classInstance, mappedModuleFields);
                }
            }
        }

        public static CsvExportV2 Factory(IEnumerable<ExportFeedProductModel> products, ExportFeedSettings options, int productsCount, int categoriesCount, bool useCommonStatistic = false)
        {
            return new CsvExportV2(products, options, productsCount, categoriesCount, useCommonStatistic);
        }

        private CsvWriter InitWriter()
        {
            var streamWriter = new StreamWriter(_path, false, Encoding.GetEncoding(_encodeType));

            var writer = new CsvWriter(streamWriter);
            writer.Configuration.Delimiter = _delimiter;
            return writer;
        }

        protected void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }

        public void SaveProductsToCsv()
        {
            try
            {
                using (var writer = InitWriter())
                {
                    WriteHeader(writer);

                    if (_products == null) return;

                    foreach (ExportFeedCsvV2Product product in _products)
                    {
                        if ((!CommonStatistic.IsRun || CommonStatistic.IsBreaking) && _useCommonStatistic) return;

                        DoForCommonStatistic(() => CommonStatistic.RowPosition++);

                        if (_fieldMapping.Contains(EProductField.Description) && product.Description.Length > MaxCellLength)
                        {
                            DoForCommonStatistic(() => CommonStatistic.WriteLog(string.Format(LocalizationService.GetResource("Core.ExportImport.ExportCsv.TooLargeDescription"), product.Name, product.ArtNo)));
                            DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                            continue;
                        }

                        if (_fieldMapping.Contains(EProductField.BriefDescription) && product.BriefDescription.Length > MaxCellLength)
                        {
                            DoForCommonStatistic(() => CommonStatistic.WriteLog(string.Format(LocalizationService.GetResource("Core.ExportImport.ExportCsv.TooLargeBriefDescription"), product.Name, product.ArtNo)));
                            DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                            continue;
                        }

                        WriteItem(writer, product);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private void WriteHeader(ICsvWriter writer)
        {
            foreach (var item in _fieldMapping)
            {
                switch (item)
                {
                    case EProductField.Category:
                        for (int i = 0; i < _maxProductCategoriesCount; i++)
                        {
                            writer.WriteField(string.Format("{0}: {1}", item.Localize(), i + 1));
                            if (_csvExportSorting)
                                writer.WriteField(string.Format("{0}: {1}", EProductField.Sorting.Localize(), i + 1));
                        }
                        break;
                    case EProductField.Property:
                        foreach (var name in _propertyNames)
                            writer.WriteField(string.Format("{0}: {1}", item.Localize(), name));
                        break;
                    default:
                        writer.WriteField(item.Localize());
                        break;
                }
            }

            foreach (var item in _modulesFieldMapping)
            {
                writer.WriteField(item.DisplayName);
            }

            writer.NextRecord();
        }

        private void WriteItem(ICsvWriter writer, ExportFeedCsvV2Product model)
        {
            foreach (var item in _fieldMapping)
            {
                var mainOfferModel = model.Offers.FirstOrDefault() ?? new ExportFeedCsvV2Offer();
                switch (item)
                {
                    case EProductField.Code:
                        writer.WriteField(model.ArtNo); break;
                    case EProductField.Sku:
                        writer.WriteField(mainOfferModel.ArtNo); break;
                    case EProductField.Name:
                        writer.WriteField(model.Name); break;
                    case EProductField.Price:
                        writer.WriteField(mainOfferModel.Price); break;
                    case EProductField.PurchasePrice:
                        writer.WriteField(mainOfferModel.PurchasePrice); break;
                    case EProductField.Amount:
                        writer.WriteField(mainOfferModel.Amount); break;
                    case EProductField.Size:
                        writer.WriteField(mainOfferModel.Size); break;
                    case EProductField.Color:
                        writer.WriteField(mainOfferModel.Color); break;
                    case EProductField.OfferPhotos:
                        writer.WriteField(mainOfferModel.OfferPhotos); break;
                    case EProductField.Weight:
                        writer.WriteField(mainOfferModel.Weight); break;
                    case EProductField.Dimensions:
                        writer.WriteField(mainOfferModel.Dimensions); break;
                    case EProductField.BarCode:
                        writer.WriteField(mainOfferModel.BarCode); break;
                    case EProductField.ParamSynonym:
                        writer.WriteField(model.UrlPath); break;
                    case EProductField.Category:
                        for (int i = 0; i < _maxProductCategoriesCount; i++)
                        {
                            writer.WriteField(model.Categories.Count > i ? model.Categories[i].Path : null);
                            if (_csvExportSorting)
                                writer.WriteField(model.Categories.Count > i ? model.Categories[i].Sort : null);
                        }
                        break;
                    case EProductField.Enabled:
                        writer.WriteField(model.Enabled); break;
                    case EProductField.Currency:
                        writer.WriteField(model.Currency); break;
                    case EProductField.Photos:
                        writer.WriteField(model.Photos); break;
                    case EProductField.Property:
                        foreach (var name in _propertyNames)
                            writer.WriteField(model.Properties.ContainsKey(name) ? model.Properties[name] : null);
                        break;
                    case EProductField.Unit:
                        writer.WriteField(model.Unit); break;
                    case EProductField.Discount:
                        writer.WriteField(model.Discount); break;
                    case EProductField.DiscountAmount:
                        writer.WriteField(model.DiscountAmount); break;
                    case EProductField.ShippingPrice:
                        writer.WriteField(model.ShippingPrice); break;
                    case EProductField.BriefDescription:
                        writer.WriteField(model.BriefDescription); break;
                    case EProductField.Description:
                        writer.WriteField(model.Description); break;
                    case EProductField.SeoTitle:
                        writer.WriteField(model.Title); break;
                    case EProductField.SeoMetaKeywords:
                        writer.WriteField(model.MetaKeywords); break;
                    case EProductField.SeoMetaDescription:
                        writer.WriteField(model.MetaDescription); break;
                    case EProductField.SeoH1:
                        writer.WriteField(model.H1); break;
                    case EProductField.Related:
                        writer.WriteField(model.Related); break;
                    case EProductField.Alternative:
                        writer.WriteField(model.Alternative); break;
                    case EProductField.Videos:
                        writer.WriteField(model.Videos); break;
                    case EProductField.MarkerNew:
                        writer.WriteField(model.MarkerNew); break;
                    case EProductField.MarkerBestseller:
                        writer.WriteField(model.MarkerBestseller); break;
                    case EProductField.MarkerRecomended:
                        writer.WriteField(model.MarkerRecomended); break;
                    case EProductField.MarkerOnSale:
                        writer.WriteField(model.MarkerOnSale); break;
                    case EProductField.ManualRatio:
                        writer.WriteField(model.ManualRatio); break;
                    case EProductField.Producer:
                        writer.WriteField(model.Producer); break;
                    case EProductField.OrderByRequest:
                        writer.WriteField(model.OrderByRequest); break;
                    case EProductField.CustomOptions:
                        writer.WriteField(model.CustomOptions); break;
                    case EProductField.YandexSalesNotes:
                        writer.WriteField(model.YandexSalesNotes); break;
                    case EProductField.YandexDeliveryDays:
                        writer.WriteField(model.YandexDeliveryDays); break;
                    case EProductField.YandexTypePrefix:
                        writer.WriteField(model.YandexTypePrefix); break;
                    case EProductField.YandexName:
                        writer.WriteField(model.YandexName); break;
                    case EProductField.YandexModel:
                        writer.WriteField(model.YandexModel); break;
                    case EProductField.YandexSizeUnit:
                        writer.WriteField(model.YandexSizeUnit); break;
                    case EProductField.YandexBid:
                        writer.WriteField(model.YandexBid); break;
                    case EProductField.YandexDiscounted:
                        writer.WriteField(model.YandexDiscounted); break;
                    case EProductField.YandexDiscountCondition:
                        writer.WriteField(model.YandexDiscountCondition); break;
                    case EProductField.YandexDiscountReason:
                        writer.WriteField(model.YandexDiscountReason); break;
                    case EProductField.GoogleGtin:
                        writer.WriteField(model.GoogleGtin); break;
                    case EProductField.GoogleProductCategory:
                        writer.WriteField(model.GoogleProductCategory); break;
                    case EProductField.Adult:
                        writer.WriteField(model.Adult); break;
                    case EProductField.ManufacturerWarranty:
                        writer.WriteField(model.ManufacturerWarranty); break;
                    case EProductField.AvitoProductProperties:
                        writer.WriteField(model.AvitoProductProperties); break;
                    case EProductField.Tags:
                        writer.WriteField(model.Tags); break;
                    case EProductField.Gifts:
                        writer.WriteField(model.Gifts); break;
                    case EProductField.MinAmount:
                        writer.WriteField(model.MinAmount); break;
                    case EProductField.MaxAmount:
                        writer.WriteField(model.MaxAmount); break;
                    case EProductField.Multiplicity:
                        writer.WriteField(model.Multiplicity); break;
                    case EProductField.Tax:
                        writer.WriteField(model.Tax); break;
                    case EProductField.PaymentSubjectType:
                        writer.WriteField(model.PaymentSubjectType); break;
                    case EProductField.PaymentMethodType:
                        writer.WriteField(model.PaymentMethodType); break;
                    case EProductField.ModifiedDate:
                        writer.WriteField(model.ModifiedDate); break;
                }
            }

            foreach (var moduleInstance in _mappedExportImportModules.Keys)
            {
                foreach (var moduleField in _mappedExportImportModules[moduleInstance])
                {
                    writer.WriteField(moduleInstance.PrepareField(moduleField, model.ProductId, _columSeparator, _propertySeparator));
                }
            }

            writer.NextRecord();

            for (int i = 1; i < model.Offers.Count; i++)
            {
                WriteOffer(writer, model, model.Offers[i]);
            }
        }

        private void WriteOffer(ICsvWriter writer, ExportFeedCsvV2Product model, ExportFeedCsvV2Offer offerModel)
        {
            foreach (var item in _fieldMapping)
            {
                switch (item)
                {
                    case EProductField.Code:
                        writer.WriteField(model.ArtNo); break;
                    case EProductField.Sku:
                        writer.WriteField(offerModel.ArtNo); break;
                    case EProductField.Price:
                        writer.WriteField(offerModel.Price); break;
                    case EProductField.PurchasePrice:
                        writer.WriteField(offerModel.PurchasePrice); break;
                    case EProductField.Amount:
                        writer.WriteField(offerModel.Amount); break;
                    case EProductField.Size:
                        writer.WriteField(offerModel.Size); break;
                    case EProductField.Color:
                        writer.WriteField(offerModel.Color); break;
                    case EProductField.OfferPhotos:
                        writer.WriteField(offerModel.OfferPhotos); break;
                    case EProductField.Weight:
                        writer.WriteField(offerModel.Weight); break;
                    case EProductField.Dimensions:
                        writer.WriteField(offerModel.Dimensions); break;
                    case EProductField.BarCode:
                        writer.WriteField(offerModel.BarCode); break;
                    
                    case EProductField.Category:
                        for (int i = 0; i < _maxProductCategoriesCount; i++)
                        {
                            writer.WriteField(null);
                            if (_csvExportSorting)
                                writer.WriteField(null);
                        }
                        break;
                    case EProductField.Property:
                        foreach (var name in _propertyNames)
                            writer.WriteField(null);
                        break;
                    default:
                        writer.WriteField(string.Empty); break;
                }
            }

            foreach (var moduleInstance in _mappedExportImportModules.Keys)
            {
                foreach (var moduleField in _mappedExportImportModules[moduleInstance])
                {
                    writer.WriteField(string.Empty);
                }
            }

            writer.NextRecord();
        }

        public void Process()
        {
            try
            {
                DoForCommonStatistic(() => CommonStatistic.TotalRow = _productsCount);
                SaveProductsToCsv();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                DoForCommonStatistic(() => CommonStatistic.WriteLog(ex.Message));
            }
        }

        public Task<bool> ProcessThroughACommonStatistic(string currentProcess, string currentProcessName)
        {
            return CommonStatistic.StartNew(() =>
            {
                _useCommonStatistic = true;
                Process();
            },
            currentProcess,
            currentProcessName);
        }

    }

}