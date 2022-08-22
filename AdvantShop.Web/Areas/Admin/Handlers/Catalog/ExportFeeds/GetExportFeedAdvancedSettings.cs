using System;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models.Catalog.ExportFeeds;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class GetExportFeedAdvancedSettings
    {
        private int _exportFeedId;
        private EExportFeedType _exportFeedType;
        private string _advancedSettings;


        public GetExportFeedAdvancedSettings(int exportFeedId, EExportFeedType exportFeedType, string advancedSettings)
        {
            _exportFeedId = exportFeedId;
            _exportFeedType = exportFeedType;
            _advancedSettings = advancedSettings;
        }

        public object Execute()
        {
            switch (_exportFeedType)
            {
                case EExportFeedType.Reseller:
                    return new ExportFeedSettingsResellerModel(JsonConvert.DeserializeObject<ExportFeedResellerOptions>(_advancedSettings));                    
                case EExportFeedType.YandexMarket:
                    return new ExportFeedSettingsYandexModel(JsonConvert.DeserializeObject<ExportFeedYandexOptions>(_advancedSettings)) { ExportFeedId = _exportFeedId };
                case EExportFeedType.Csv:     
                    return new ExportFeedSettingsCsvModel(JsonConvert.DeserializeObject<ExportFeedCsvOptions>(_advancedSettings));
                case EExportFeedType.CsvV2:
                    return new ExportFeedSettingsCsvModel(JsonConvert.DeserializeObject<ExportFeedCsvOptions>(_advancedSettings));
                case EExportFeedType.GoogleMerchentCenter:
                    return new ExportFeedSettingsGoogleModel(JsonConvert.DeserializeObject<ExportFeedGoogleMerchantCenterOptions>(_advancedSettings));
                case EExportFeedType.Avito:
                    return new ExportFeedSettingsAvitoModel(JsonConvert.DeserializeObject<ExportFeedAvitoOptions>(_advancedSettings));
                case EExportFeedType.Facebook:
                    return new ExportFeedSettingsFacebookModel(JsonConvert.DeserializeObject<ExportFeedFacebookOptions>(_advancedSettings));
                default:
                    throw new NotImplementedException("No implementation for exportfeed type " + _exportFeedType);
            }

            return null;
        }
    }
}
