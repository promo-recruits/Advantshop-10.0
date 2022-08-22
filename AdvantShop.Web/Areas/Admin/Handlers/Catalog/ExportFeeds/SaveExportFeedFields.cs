using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.Web.Infrastructure.Handlers;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class SaveExportFeedFields : AbstractCommandHandler
    {
        private readonly int _exportFeedId;
        private readonly List<string> _fields;
        private readonly List<CSVField> _moduleFields;

        private ExportFeed _exportFeed;
        private ExportFeedSettings _settings;

        public SaveExportFeedFields(int exportFeedId, List<string> fields)
        {
            _exportFeedId = exportFeedId;
            _fields = fields;
            _moduleFields = new List<CSVField>();
            foreach (var csvExportImportModule in AttachedModules.GetModules<ICSVExportImport>())
            {
                var classInstance = (ICSVExportImport)Activator.CreateInstance(csvExportImportModule, null);
                if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                    _moduleFields.AddRange(classInstance.GetCSVFields());
            }
        }

        protected override void Load()
        {
            _exportFeed = ExportFeedService.GetExportFeed(_exportFeedId);
            _settings = ExportFeedSettingsProvider.GetSettings(_exportFeedId);
        }

        protected override void Validate()
        {
            if (_exportFeed == null)
                throw new BlException("Выгрузка не найдена");
            if (_exportFeed.Type != EExportFeedType.Csv && _exportFeed.Type != EExportFeedType.Reseller && _exportFeed.Type != EExportFeedType.CsvV2)
                throw new BlException("Выгрузка не предусматривает настройку полей товара");
            if (_settings == null)
                throw new BlException("Ошибка при сохранении полей выгрузки");
            if (!_fields.Any(item => item != ProductFields.None.ToString()))
                throw new BlException("Не выбрано ни одно поле выгрузки");
        }

        protected override void Handle()
        {
            ExportFeedSettings options = null;
            switch (_exportFeed.Type)
            {
                case EExportFeedType.Csv:
                    var csvFieldMapping = _fields.Select(f => f.TryParseEnum<ProductFields>())
                        .Where(item => item != ProductFields.None && Enum.IsDefined(typeof(ProductFields), item)).Distinct().ToList();
                    var csvModuleFieldMapping = _moduleFields.Where(mf => _fields.Contains(mf.StrName) && !csvFieldMapping.Any(item => item.ToString() == mf.StrName)).Distinct().ToList();
                    options = JsonConvert.DeserializeObject<ExportFeedCsvOptions>(_settings.AdvancedSettings);
                    ((ExportFeedCsvOptions)options).FieldMapping = csvFieldMapping;
                    ((ExportFeedCsvOptions)options).ModuleFieldMapping = csvModuleFieldMapping;
                    break;
                case EExportFeedType.Reseller:
                    var resellerFieldMapping = _fields.Select(f => f.TryParseEnum<ProductFields>())
                        .Where(item => item != ProductFields.None && Enum.IsDefined(typeof(ProductFields), item)).Distinct().ToList();
                    var resellerModuleFieldMapping = _moduleFields.Where(mf => _fields.Contains(mf.StrName) && !resellerFieldMapping.Any(item => item.ToString() == mf.StrName)).Distinct().ToList();
                    options = JsonConvert.DeserializeObject<ExportFeedResellerOptions>(_settings.AdvancedSettings);
                    ((ExportFeedResellerOptions)options).FieldMapping = resellerFieldMapping;
                    ((ExportFeedResellerOptions)options).ModuleFieldMapping = resellerModuleFieldMapping;
                    break;
                case EExportFeedType.CsvV2:
                    var csvV2FieldMapping = _fields.Select(f => f.TryParseEnum<EProductField>())
                        .Where(item => item != EProductField.None && Enum.IsDefined(typeof(EProductField), item)).Distinct().ToList();
                    var csvV2ModuleFieldMapping = _moduleFields.Where(mf => _fields.Contains(mf.StrName) && !csvV2FieldMapping.Any(item => item.ToString() == mf.StrName)).Distinct().ToList();
                    options = JsonConvert.DeserializeObject<ExportFeedCsvV2Options>(_settings.AdvancedSettings);
                    ((ExportFeedCsvV2Options)options).FieldMapping = csvV2FieldMapping;
                    ((ExportFeedCsvV2Options)options).ModuleFieldMapping = csvV2ModuleFieldMapping;
                    break;
            }

            ExportFeedSettingsProvider.SetAdvancedSettings(_exportFeedId, JsonConvert.SerializeObject(options));
        }
    }
}
