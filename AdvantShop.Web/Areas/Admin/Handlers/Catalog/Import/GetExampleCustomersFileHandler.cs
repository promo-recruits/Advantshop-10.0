using System;
using System.Collections.Generic;
using System.IO;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;
using CsvHelper;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetExampleCustomersFileHandler
    {
        private readonly string _columnSeparator;
        private readonly string _encoding;
        private readonly string _outputFilePath;

        private Dictionary<ECustomerFields, string> _customerExample;

        public GetExampleCustomersFileHandler(ImportCustomersModel model, string outputFilePath)
        {
            _columnSeparator = model.ColumnSeparator.IsNotEmpty() && model.ColumnSeparator.ToLower() == SeparatorsEnum.Custom.ToString().ToLower()
                ? model.CustomColumnSeparator
                : model.ColumnSeparator;
            _encoding = model.Encoding;
            _outputFilePath = outputFilePath;
        }

        private CsvWriter InitWriter()
        {
            var streamWriter = new StreamWriter(_outputFilePath, false, System.Text.Encoding.GetEncoding(_encoding));
            var writer = new CsvWriter(streamWriter);
            writer.Configuration.Delimiter = _columnSeparator;
            return writer;
        }

        public object Execute()
        {
            if (_columnSeparator.IsNullOrEmpty())
                throw new BlException(LocalizationService.GetResource("Admin.Import.Errors.NotColumnSeparator"));

            if (File.Exists(_outputFilePath))
                File.Delete(_outputFilePath);

            GetCustomerExample();
            using (var csvWriter = InitWriter())
            {
                var customerFields = CustomerFieldService.GetCustomerFields(true);
                foreach (ECustomerFields item in Enum.GetValues(typeof(ECustomerFields)))
                {
                    if (item == ECustomerFields.None || !_customerExample.ContainsKey(item))
                        continue;
                    csvWriter.WriteField(item.StrName().ToLower());
                }
                foreach (var additionalField in customerFields)
                {
                    csvWriter.WriteField(additionalField.Name);
                }
                csvWriter.NextRecord();

                foreach (ECustomerFields item in Enum.GetValues(typeof(ECustomerFields)))
                {
                    if (item == ECustomerFields.None || !_customerExample.ContainsKey(item))
                        continue;
                    csvWriter.WriteField(_customerExample[item]);
                }
                foreach (var additionalField in customerFields)
                {
                    csvWriter.WriteField(additionalField.Name);
                }
                csvWriter.NextRecord();
            }

            return new { Result = true, };
        }

        private void GetCustomerExample()
        {
            _customerExample = new Dictionary<ECustomerFields, string>
            {
                { ECustomerFields.CustomerId, Guid.NewGuid().ToString() },
                { ECustomerFields.FirstName, "Иван" },
                { ECustomerFields.LastName, "Иванов" },
                { ECustomerFields.Patronymic, "Иванович" },
                { ECustomerFields.Phone, "+79999999999" },
                { ECustomerFields.Email, "example@example.com" },
                { ECustomerFields.CustomerGroup, "Постоянный покупатель" },
                { ECustomerFields.Enabled, "True" },
                { ECustomerFields.AdminComment, "Комментарий администратора" },

                { ECustomerFields.City, "Москва" },
                { ECustomerFields.Region, "Московская область" },
                { ECustomerFields.Country, "Россия" },
                { ECustomerFields.Zip, "012345" },
                { ECustomerFields.Street, "Красная площадь" },
                { ECustomerFields.House, "1" },
                { ECustomerFields.Apartment, "1" },
                { ECustomerFields.Address, "Красная площадь, 1, Царские палаты" },
                { ECustomerFields.BirthDay, "1980-01-31" },
                { ECustomerFields.Organization, "ООО Ромашка" },
                { ECustomerFields.ManagerName, "Администратор Магазина" },
                { ECustomerFields.ManagerId, "1" },
            };
        }
    }
}
