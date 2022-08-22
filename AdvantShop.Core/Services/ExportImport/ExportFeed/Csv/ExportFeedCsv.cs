//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdvantShop.ExportImport
{
    [ExportFeedKey("Csv")]
    public class ExportFeedCsv : BaseExportFeed
    {
        public ExportFeedCsv() : base()
        {
        }

        public ExportFeedCsv(bool useCommonStatistic) : base(useCommonStatistic)
        {
        }

        public override string Export(int exportFeedId)
        {
            try
            {
                var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
                var advancedSettings = JsonConvert.DeserializeObject<ExportFeedCsvOptions>(commonSettings.AdvancedSettings);

                var categories = ExportFeedCsvService.GetCategories(exportFeedId);
                var products = ExportFeedCsvService.GetProducts(exportFeedId, commonSettings, advancedSettings);
                var categoriesCount = ExportFeedCsvService.GetCategoriesCount(exportFeedId);
                var productsCount = ExportFeedCsvService.GetProductsCount(exportFeedId, commonSettings, advancedSettings);

                return Export(categories, products, commonSettings, categoriesCount, productsCount);
            }
            catch (Exception ex)
            {
                CsRowError(ex.Message);
                Debug.Log.Error(ex);
            }
            return null;
        }

        public override string Export(IEnumerable<ExportFeedCategories> categories, IEnumerable<ExportFeedProductModel> products, ExportFeedSettings options, int categoriesCount, int productsCount)
        {
            //var advancedSettings = JsonConvert.DeserializeObject<ExportFeedCsvOptions>(options.AdvancedSettings);

            try
            {
                //var fileName = options.FileFullName;
                var exportFile = new FileInfo(options.FileFullPath);
                if (!string.IsNullOrEmpty(exportFile.Directory.FullName))
                {
                    FileHelpers.CreateDirectory(exportFile.Directory.FullName);
                }

                CsSetFileName("../" + options.FileFullName);

                var originalFullPath = options.FileFullPath;
                options.FileName = options.FileFullName + tempPrefix;

                FileHelpers.DeleteFile(options.FileFullPath);

                CsvExport.Factory(products, options, productsCount, categoriesCount, useCommonStatistic: UseCommonStatistic).Process();

                FileHelpers.ReplaceFile(options.FileFullPath, originalFullPath);
            }
            catch (Exception ex)
            {
                CsRowError(ex.Message);
                Debug.Log.Error(ex);
            }
            return options.FileFullName;
        }

        public override void SetDefaultSettings(int exportFeedId)
        {
            var moduleFields = new List<CSVField>();
            foreach (var csvExportImportModule in AttachedModules.GetModules<ICSVExportImport>())
            {
                var classInstance = (ICSVExportImport)Activator.CreateInstance(csvExportImportModule, null);
                if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                    moduleFields.AddRange(classInstance.GetCSVFields());
            }
            ExportFeedSettingsProvider.SetSettings(exportFeedId,
                  new ExportFeedSettings()
                  {
                      Interval = 1,
                      IntervalType = Core.Scheduler.TimeIntervalType.Hours,
                      Active = false,

                      FileName = File.Exists(SettingsGeneral.AbsolutePath + "/export/catalog.csv") ? "export/catalog" + exportFeedId : "catalog",
                      FileExtention = "csv",

                      AdvancedSettings = JsonConvert.SerializeObject(
                          new ExportFeedCsvOptions()
                          {
                              CsvSeparator = SeparatorsEnum.SemicolonSeparated.ToString(),
                              CsvColumSeparator = ";",
                              CsvPropertySeparator = ":",
                              CsvEnconing = EncodingsEnum.Utf8.StrName(),

                              FieldMapping = new List<ProductFields>(Enum.GetValues(typeof(ProductFields)).OfType<ProductFields>().Where(item => item != ProductFields.None && item != ProductFields.Sorting && item != ProductFields.ExternalCategoryId).ToList()),
                              ModuleFieldMapping = moduleFields
                          }),
                      AdditionalUrlTags = string.Empty,
                      ExportAdult = true
                  });
        }

        public override List<string> GetAvailableVariables()
        {
            return new List<string> { "#STORE_NAME#", "#STORE_URL#", "#PRODUCT_NAME#", "#PRODUCT_ID#", "#PRODUCT_ARTNO#" };
        }

        public override List<ExportFeedCategories> GetCategories(int exportFeedId)
        {
            throw new NotImplementedException();
        }

        public override List<ExportFeedProductModel> GetProducts(int exportFeedId)
        {
            throw new NotImplementedException();
        }

        public override int GetProductsCount(int exportFeedId)
        {
            var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
            var advancedSettings = JsonConvert.DeserializeObject<ExportFeedCsvOptions>(commonSettings.AdvancedSettings);

            return ExportFeedCsvService.GetProductsCount(exportFeedId, commonSettings, advancedSettings);
        }

        public override int GetCategoriesCount(int exportFeedId)
        {
            return ExportFeedCsvService.GetCategoriesCount(exportFeedId);
        }

        public override List<string> GetAvailableFileExtentions()
        {
            return new List<string> { "csv", "txt" };
        }
    }
}