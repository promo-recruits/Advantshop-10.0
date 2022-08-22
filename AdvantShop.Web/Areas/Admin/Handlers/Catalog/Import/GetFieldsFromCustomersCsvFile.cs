﻿using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetFieldsFromCustomersCsvFile : BaseGetFieldsFromCsvFileHandler<object>
    {
        public GetFieldsFromCustomersCsvFile(ImportCustomersModel model, string outputFilePath) : base(model, outputFilePath)
        {
        }

        protected override void LoadData()
        {
            CsvRows = new CsvImportCustomers(OutputFilePath, false, ColumnSeparator, Encoding, null, 0, useCommonStatistic: false).ReadFirstRecord();
        }

        protected override object HandleData()
        {
            foreach (ECustomerFields item in Enum.GetValues(typeof(ECustomerFields)))
            {
                if (item != ECustomerFields.None)
                    AllFields.Add(item.StrName().ToLower(), item.Localize());
            }

            foreach (var additionalField in CustomerFieldService.GetCustomerFields(true))
            {
                if (!AllFields.ContainsKey(additionalField.Name.ToLower()))
                    AllFields.Add(additionalField.Name.ToLower(), additionalField.Name);
            }

            return new { FirstItem, AllFields, Headers };
        }
    }
}
