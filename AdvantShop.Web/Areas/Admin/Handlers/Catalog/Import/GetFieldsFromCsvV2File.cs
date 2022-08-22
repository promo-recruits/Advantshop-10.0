using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetFieldsFromCsvV2File : BaseGetFieldsFromCsvFileHandler<object>
    {
        private readonly string _propertySeparator;
        private readonly string _propertyValueSeparator;

        private List<CSVField> _moduleFields;

        public GetFieldsFromCsvV2File(ImportProductsModel model, string outputFilePath) : base(model, outputFilePath)
        {
            _propertySeparator = model.PropertySeparator;
            _propertyValueSeparator = model.PropertyValueSeparator;
            
            _moduleFields = new List<CSVField>();
            foreach (var csvExportImportModule in AttachedModules.GetModules<ICSVExportImport>())
            {
                var classInstance = (ICSVExportImport)Activator.CreateInstance(csvExportImportModule, null);
                if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                    _moduleFields.AddRange(classInstance.GetCSVFields());
            }
        }

        protected override void LoadData()
        {
            CsvRows = CsvImportV2.Factory(OutputFilePath, false, false, ColumnSeparator, Encoding, null, _propertySeparator, _propertyValueSeparator, useCommonStatistic: false).ReadFirst2();
        }

        protected override object Handle()
        {
            // get headers
            for (int i = 0; i < CsvRows[0].Length; i++)
                Headers.Add(HaveHeader ? CsvRows[0][i].Reduce(50).Trim() : T("Admin.ImportProducts.Empty"));

            // get first item
            var dataRow = HaveHeader && CsvRows.Count > 1 ? CsvRows[1] : CsvRows[0];
            if (dataRow != null)
            {
                foreach (var data in dataRow)
                    FirstItem.Add(data.DefaultOrEmpty().Reduce(50).HtmlEncode());
            }

            // get all fields
            var fieldNames = new Dictionary<string, string>();
            foreach (EProductField item in Enum.GetValues(typeof(EProductField)))
            {
                AllFields.Add(item.ToString(), item.Localize());
                fieldNames.TryAddValue(item.Localize(), item.ToString());
            }
            foreach (var moduleField in _moduleFields)
            {
                if (!AllFields.ContainsKey(moduleField.StrName.ToLower()))
                    AllFields.Add(moduleField.StrName, moduleField.DisplayName);
                fieldNames.TryAddValue(moduleField.DisplayName, moduleField.StrName);
            }

            // get selected fields
            SelectedFields = new List<string>();
            foreach (var header in Headers)
            {
                var identPart = header.Split(':').FirstOrDefault();
                SelectedFields.Add(fieldNames.ContainsKey(header)
                    ? fieldNames[header]
                    : fieldNames.ElementOrDefault(identPart, EProductField.None.ToString()));
            }

            return new { FirstItem, AllFields, Headers, SelectedFields };
        }
    }
}
