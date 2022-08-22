//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.Localization;
using AdvantShop.Statistic;
using CsvHelper;
using Newtonsoft.Json;

namespace AdvantShop.ExportImport
{
    public class CsvExport
    {
        private const int MaxCellLength = 60000;
        private readonly string _path;
        private readonly string _encodeType;
        private readonly string _delimiter;
        private readonly string _columSeparator;
        private readonly string _propertySeparator;
        private readonly List<ProductFields> _fieldMapping;
        private readonly List<CSVField> _modulesFieldMapping;
        private readonly bool _csvExportNoInCategory;
        private readonly bool _csvExportSorting;
        private bool _useCommonStatistic;

        private readonly int _productsCount;
        private readonly int _categoriesCount;

        private readonly Dictionary<ICSVExportImport, List<CSVField>> _mappedExportImportModules;

        private readonly IEnumerable<ExportFeedProductModel> _products;

        private CsvExport(IEnumerable<ExportFeedProductModel> products, ExportFeedSettings options, int productsCount, int categoriesCount, bool useCommonStatistic)
        {
            var csvOptions = JsonConvert.DeserializeObject<ExportFeedCsvOptions>(options.AdvancedSettings);

            _path = options.FileFullPath;
            _encodeType = csvOptions.CsvEnconing.IsNullOrEmpty() ? EncodingsEnum.Windows1251.StrName() : csvOptions.CsvEnconing;
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
            _categoriesCount = categoriesCount;

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

        public static CsvExport Factory(IEnumerable<ExportFeedProductModel> products, ExportFeedSettings options, int productsCount, int categoriesCount, bool useCommonStatistic = false)
        {
            return new CsvExport(products, options, productsCount, categoriesCount, useCommonStatistic);
        }

        private CsvWriter InitWriter()
        {
            var streamWriter = new StreamWriter(_path, false, Encoding.GetEncoding(_encodeType));
            //streamWriter.NewLine = "\r\n";

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

                    foreach (ExportFeedCsvProduct product in _products)
                    {
                        if ((!CommonStatistic.IsRun || CommonStatistic.IsBreaking) && _useCommonStatistic) return;

                        DoForCommonStatistic(() => CommonStatistic.RowPosition++);

                        if (_fieldMapping.Contains(ProductFields.Description) && product.Description.Length > MaxCellLength)
                        {
                            DoForCommonStatistic(() => CommonStatistic.WriteLog(string.Format(LocalizationService.GetResource("Core.ExportImport.ExportCsv.TooLargeDescription"), product.Name, product.ArtNo)));
                            DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                            continue;
                        }

                        if (_fieldMapping.Contains(ProductFields.BriefDescription) && product.BriefDescription.Length > MaxCellLength)
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
                writer.WriteField(item.StrName());
            }

            if (_csvExportSorting)
            {
                writer.WriteField(ProductFields.Sorting.StrName());
            }

            foreach (var item in _modulesFieldMapping)
            {
                writer.WriteField(item.StrName);
            }

            writer.NextRecord();
        }

        private void WriteItem(ICsvWriter writer, ExportFeedCsvProduct model)
        {
            foreach (var item in _fieldMapping)
            {
                switch (item)
                {
                    case ProductFields.Sku:
                        writer.WriteField(model.ArtNo); break;

                    case ProductFields.Name:
                        writer.WriteField(model.Name); break;

                    case ProductFields.ParamSynonym:
                        writer.WriteField(model.UrlPath); break;

                    case ProductFields.Category:
                        writer.WriteField(model.Category); break;

                    case ProductFields.Sorting:
                        writer.WriteField(model.Sorting); break;

                    case ProductFields.Enabled:
                        writer.WriteField(model.Enabled); break;


                    case ProductFields.Price:
                        writer.WriteField(model.Price); break;

                    case ProductFields.Amount:
                        writer.WriteField(model.Amount); break;

                    case ProductFields.PurchasePrice:
                        writer.WriteField(model.PurchasePrice); break;

                    case ProductFields.MultiOffer:
                        writer.WriteField(model.MultiOffer); break;

                    case ProductFields.Currency:
                        writer.WriteField(model.Currency); break;

                    case ProductFields.Unit:
                        writer.WriteField(model.Unit); break;

                    case ProductFields.ShippingPrice:
                        writer.WriteField(model.ShippingPrice); break;

                    case ProductFields.YandexDeliveryDays:
                        writer.WriteField(model.YandexDeliveryDays); break;

                    case ProductFields.Discount:
                        writer.WriteField(model.Discount); break;

                    case ProductFields.DiscountAmount:
                        writer.WriteField(model.DiscountAmount); break;

                    case ProductFields.Weight:
                        writer.WriteField(model.Weight); break;

                    case ProductFields.Size:
                        writer.WriteField(model.Size); break;

                    case ProductFields.BriefDescription:
                        writer.WriteField(model.BriefDescription); break;

                    case ProductFields.Description:
                        writer.WriteField(model.Description); break;

                    case ProductFields.Title:
                        writer.WriteField(model.Title); break;

                    case ProductFields.H1:
                        writer.WriteField(model.H1); break;

                    case ProductFields.MetaKeywords:
                        writer.WriteField(model.MetaKeywords); break;

                    case ProductFields.MetaDescription:
                        writer.WriteField(model.MetaDescription); break;

                    case ProductFields.Markers:
                        writer.WriteField(model.Markers); break;

                    case ProductFields.Photos:
                        writer.WriteField(model.Photos); break;

                    case ProductFields.Videos:
                        writer.WriteField(model.Videos); break;

                    case ProductFields.Properties:
                        writer.WriteField(model.Properties); break;

                    case ProductFields.Producer:
                        writer.WriteField(model.Producer); break;

                    case ProductFields.OrderByRequest:
                        writer.WriteField(model.OrderByRequest); break;

                    case ProductFields.Related:
                        writer.WriteField(model.Related); break;

                    case ProductFields.Alternative:
                        writer.WriteField(model.Alternative); break;

                    case ProductFields.CustomOption:
                        writer.WriteField(model.CustomOption); break;

                    case ProductFields.SalesNotes:
                        writer.WriteField(model.SalesNote); break;

                    case ProductFields.Gtin:
                        writer.WriteField(model.Gtin); break;

                    case ProductFields.GoogleProductCategory:
                        writer.WriteField(model.GoogleProductCategory); break;

                    case ProductFields.YandexTypePrefix:
                        writer.WriteField(model.YandexTypePrefix); break;

                    case ProductFields.YandexModel:
                        writer.WriteField(model.YandexModel); break;

                    case ProductFields.YandexName:
                        writer.WriteField(model.YandexName); break;

                    case ProductFields.YandexSizeUnit:
                        writer.WriteField(model.YandexSizeUnit); break;

                    case ProductFields.YandexDiscounted:
                        writer.WriteField(model.YandexDiscounted); break;

                    case ProductFields.YandexDiscountCondition:
                        writer.WriteField(model.YandexDiscountCondition); break;

                    case ProductFields.YandexDiscountReason:
                        writer.WriteField(model.YandexDiscountReason); break;

                    case ProductFields.Adult:
                        writer.WriteField(model.Adult); break;

                    case ProductFields.ManufacturerWarranty:
                        writer.WriteField(model.ManufacturerWarranty); break;

                    case ProductFields.Tags:
                        writer.WriteField(model.Tags); break;

                    case ProductFields.Gifts:
                        writer.WriteField(model.Gifts); break;

                    case ProductFields.MinAmount:
                        writer.WriteField(model.MinAmount); break;

                    case ProductFields.MaxAmount:
                        writer.WriteField(model.MaxAmount); break;

                    case ProductFields.Multiplicity:
                        writer.WriteField(model.Multiplicity); break;

                    case ProductFields.Bid:
                        writer.WriteField(model.Bid); break;

                    case ProductFields.BarCode:
                        writer.WriteField(model.BarCode); break;

                    case ProductFields.Tax:
                        writer.WriteField(model.Tax); break;

                    case ProductFields.PaymentMethodType:
                        writer.WriteField(model.PaymentMethodType); break;

                    case ProductFields.PaymentSubjectType:
                        writer.WriteField(model.PaymentSubjectType); break;


                    case ProductFields.AvitoProductProperties:
                        {
                            var avitoProductProperties = ExportFeedAvitoService.GetProductPropertiesInString(model.ProductId, _columSeparator, _propertySeparator);
                            writer.WriteField(avitoProductProperties);
                            break;
                        }

                    case ProductFields.ManualRatio:
                        writer.WriteField(model.ManualRatio); break;

                    case ProductFields.ModifiedDate:
                    {
                        var lastChanges = ChangeHistoryService.GetLast(model.ProductId, ChangeHistoryObjType.Product);
                        writer.WriteField(lastChanges != null ? Culture.ConvertDate(lastChanges.ModificationTime) : "");
                        break;
                    }

                    //case ProductFields.Store: writer.WriteField(model.Store); break;
                    //case ProductFields.Funnel: writer.WriteField(model.Funnel); break;
                    //case ProductFields.Vk: writer.WriteField(model.Vk); break;
                    //case ProductFields.Instagram: writer.WriteField(model.Instagram); break;
                    //case ProductFields.Yandex: writer.WriteField(model.Yandex); break;
                    //case ProductFields.Avito: writer.WriteField(model.Avito); break;
                    //case ProductFields.Google: writer.WriteField(model.Google); break;
                    //case ProductFields.Facebook: writer.WriteField(model.Facebook); break;
                    //case ProductFields.Bonus: writer.WriteField(model.Bonus); break;
                    //case ProductFields.Referal: writer.WriteField(model.Referal); break;

                }
            }

            if (_csvExportSorting)
            {
                writer.WriteField(model.Sorting);
            }

            foreach (var moduleInstance in _mappedExportImportModules.Keys)
            {
                foreach (var moduleField in _mappedExportImportModules[moduleInstance])
                {
                    writer.WriteField(moduleInstance.PrepareField(moduleField, model.ProductId, _columSeparator, _propertySeparator));
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