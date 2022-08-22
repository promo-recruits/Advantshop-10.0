//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdvantShop.ExportImport
{
    [ExportFeedKey("Reseller")]
    public class ExportFeedReseller : BaseExportFeed
    {
        public ExportFeedReseller() : base()
        {
        }

        public ExportFeedReseller(bool useCommonStatistic) : base(useCommonStatistic)
        {
        }

        public static List<string> AvailableFileExtentions
        {
            get { return new List<string> { "csv" }; }
        }

        public override string Export(int exportFeedId)
        {
            try
            {
                var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
                var advancedSettings = JsonConvert.DeserializeObject<ExportFeedResellerOptions>(commonSettings.AdvancedSettings);

                advancedSettings.CsvSeparator = SeparatorsEnum.SemicolonSeparated.ToString();
                advancedSettings.CsvColumSeparator = ";";
                advancedSettings.CsvPropertySeparator = ":";
                advancedSettings.CsvEnconing = EncodingsEnum.Utf8.StrName();
                advancedSettings.CsvCategorySort = true;

                commonSettings.AdvancedSettings = JsonConvert.SerializeObject(advancedSettings);

                var categories = ExportFeedResellerService.GetCategories(exportFeedId, advancedSettings.ExportNotAvailable ?? false);
                var products = ExportFeedResellerService.GetProducts(exportFeedId, commonSettings, advancedSettings);
                var categoriesCount = ExportFeedResellerService.GetCategoriesCount(exportFeedId, advancedSettings.ExportNotAvailable ?? false);
                var productsCount = ExportFeedResellerService.GetProductsCount(exportFeedId, commonSettings, advancedSettings);

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
            try
            {
                var exportFile = new FileInfo(options.FileFullPath);
                if (!string.IsNullOrEmpty(exportFile.Directory.FullName))
                {
                    FileHelpers.CreateDirectory(exportFile.Directory.FullName);
                }

                CsSetFileName(GetDownloadableExportFeedFileLink(JsonConvert.DeserializeObject<ExportFeedResellerOptions>(options.AdvancedSettings)));

                var originalFullPath = options.FileFullPath;
                options.FileName = options.FileName + tempPrefix;
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
            var resellerCode = Guid.NewGuid();

            ExportFeedSettingsProvider.SetSettings(exportFeedId,
                  new ExportFeedSettings()
                  {
                      Interval = 1,
                      IntervalType = Core.Scheduler.TimeIntervalType.Hours,
                      Active = false,
                      
                      FileName = "export/resellers/" + resellerCode,
                      FileExtention = "csv",

                      AdvancedSettings = JsonConvert.SerializeObject(
                          new ExportFeedResellerOptions()
                          {
                              ResellerCode = resellerCode.ToString(),

                              CsvSeparator = SeparatorsEnum.SemicolonSeparated.ToString(),
                              CsvColumSeparator = ";",
                              CsvPropertySeparator = ":",
                              CsvEnconing = EncodingsEnum.Utf8.StrName(),
                              CsvCategorySort = true,

                              FieldMapping = new List<ProductFields>(Enum.GetValues(typeof(ProductFields)).OfType<ProductFields>().Where(item => item != ProductFields.None).ToList()),
                          }),
                      AdditionalUrlTags = string.Empty,
                      ExportAdult = true
                  });

            ExportFeedService.InsertCategory(exportFeedId, 0, false);
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
            var advancedSettings = JsonConvert.DeserializeObject<ExportFeedResellerOptions>(commonSettings.AdvancedSettings);

            return ExportFeedResellerService.GetProductsCount(exportFeedId, commonSettings, advancedSettings);
        }

        public override int GetCategoriesCount(int exportFeedId)
        {
            var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
            var advancedSettings = JsonConvert.DeserializeObject<ExportFeedResellerOptions>(commonSettings.AdvancedSettings);

            return ExportFeedResellerService.GetCategoriesCount(exportFeedId, advancedSettings.ExportNotAvailable ?? false);
        }

        public override string GetDownloadableExportFeedFileLink(int exportFeedId)
        {
            var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
            var advancedSettings = JsonConvert.DeserializeObject<ExportFeedResellerOptions>(commonSettings.AdvancedSettings);

            return GetDownloadableExportFeedFileLink(advancedSettings);
        }

        private string GetDownloadableExportFeedFileLink(ExportFeedResellerOptions advancedSettings)
        {
            return SettingsMain.SiteUrl + "/api/resellers/catalog?id=" + advancedSettings.ResellerCode;
        }

        public override List<string> GetAvailableFileExtentions()
        {
            return AvailableFileExtentions;
        }
    }
}