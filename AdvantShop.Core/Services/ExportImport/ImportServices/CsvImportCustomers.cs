using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Statistic;
using AdvantShop.Customers;
using AdvantShop.Repository;
using CsvHelper;

namespace AdvantShop.ExportImport
{
    public class CsvImportCustomers
    {
        private readonly string _fullPath;
        private readonly bool _hasHeadrs;

        private readonly string _separator;
        private readonly string _encodings;

        private readonly int _defaultCustomerGroupId;

        private Dictionary<string, int> _fieldMapping;
        private bool _useCommonStatistic;

        public CsvImportCustomers(string filePath, bool hasHeadrs, string separator, string encodings, Dictionary<string, int> fieldMapping, int defaultCustomerGroupId,
            bool useCommonStatistic = true)
        {
            _fullPath = filePath;
            _hasHeadrs = hasHeadrs;
            _fieldMapping = fieldMapping;
            _encodings = encodings;
            _separator = separator;

            _defaultCustomerGroupId = defaultCustomerGroupId;
            _useCommonStatistic = useCommonStatistic;
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var reader = new CsvReader(new StreamReader(_fullPath, Encoding.GetEncoding(_encodings ?? EncodingsEnum.Utf8.StrName())));

            reader.Configuration.Delimiter = _separator ?? SeparatorsEnum.SemicolonSeparated.StrName();
            reader.Configuration.HasHeaderRecord = hasHeaderRecord ?? _hasHeadrs;

            return reader;
        }

        public List<string[]> ReadFirstRecord()
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
        private void Process()
        {
           try
           {
               _process();
           }
           catch (Exception ex)
           {
               Debug.Log.Error(ex);
               DoForCommonStatistic(() =>
               {
                   CommonStatistic.WriteLog(ex.Message);
                   CommonStatistic.TotalErrorRow++;
               });
           }
           finally
           {
           }
        }

        public Task<bool> ProcessThroughACommonStatistic(
            string currentProcess,
            string currentProcessName,
            Action onBeforeImportAction = null)
        {
            return CommonStatistic.StartNew(() =>
                {
                    if (onBeforeImportAction != null)
                        onBeforeImportAction();

                    _useCommonStatistic = true;
                    Process();
                },
                currentProcess,
                currentProcessName);
        }

        private void _process()
        {
            Log("Начало импорта");

            if (_fieldMapping == null)
                MapFields();

            if (_fieldMapping == null)
                throw new Exception("can mapping colums");


            DoForCommonStatistic(() => CommonStatistic.TotalRow = GetRowCount());


            ProcessRow();


            CacheManager.Clean();
            FileHelpers.DeleteFile(_fullPath);

            Log("Окончание импорта");
        }

        private void MapFields()
        {
            _fieldMapping = new Dictionary<string, int>();
            using (var csv = InitReader(false))
            {
                csv.Read();
                for (var i = 0; i < csv.CurrentRecord.Length; i++)
                {
                    if (csv.CurrentRecord[i] == ECustomerFields.None.StrName()) continue;
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

        private void ProcessRow()
        {
            if (!File.Exists(_fullPath)) return;
            using (var csv = InitReader())
            {
                while (csv.Read())
                {
                    if (_useCommonStatistic && (!CommonStatistic.IsRun || CommonStatistic.IsBreaking))
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }
                    try
                    {
                        var customerInStrings = PrepareRow(csv);
                        if (customerInStrings == null)
                        {
                            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                            continue;
                        }

                        UpdateInsertCustomer(customerInStrings, csv);

                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                        DoForCommonStatistic(() =>
                        {
                            CommonStatistic.WriteLog(string.Format("{0}: {1}", CommonStatistic.RowPosition, ex.Message));
                            CommonStatistic.TotalErrorRow++;
                        });
                    }
                }
            }
        }

        private Dictionary<ECustomerFields, object> PrepareRow(ICsvReader csv)
        {
            var customerInStrings = new Dictionary<ECustomerFields, object>();

            foreach (ECustomerFields field in Enum.GetValues(typeof(ECustomerFields)))
            {
                switch (field.Status())
                {
                    case CsvFieldStatus.String:
                        GetString(field, csv, customerInStrings);
                        break;
                    case CsvFieldStatus.StringRequired:
                        GetStringRequired(field, csv, customerInStrings);
                        break;
                    case CsvFieldStatus.NotEmptyString:
                        GetStringNotNull(field, csv, customerInStrings);
                        break;
                    case CsvFieldStatus.Float:
                        if (!GetDecimal(field, csv, customerInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.NullableFloat:
                        if (!GetNullableDecimal(field, csv, customerInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.Int:
                        if (!GetInt(field, csv, customerInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.DateTime:
                        if (!GetDateTime(field, csv, customerInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.NullableDateTime:
                        if (!GetNullableDateTime(field, csv, customerInStrings))
                            return null;
                        break;
                }
            }
            return customerInStrings;
        }

        private void UpdateInsertCustomer(Dictionary<ECustomerFields, object> customerInStrings, ICsvReader csv)
        {
            try
            {
                Customer customer = null;

                var email = customerInStrings.ContainsKey(ECustomerFields.Email)
                    ? Convert.ToString(customerInStrings[ECustomerFields.Email])
                    : string.Empty;

                var customerId = customerInStrings.ContainsKey(ECustomerFields.CustomerId)
                    ? Convert.ToString(customerInStrings[ECustomerFields.CustomerId]).TryParseGuid()
                    : Guid.Empty;

                if (customerId != Guid.Empty)
                {
                    customer = CustomerService.GetCustomer(customerId);
                }

                if (string.IsNullOrEmpty(email) && customer == null)
                {
                    DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                    Log(CommonStatistic.RowPosition + ": no email and wrong customer id");

                    customerInStrings.Clear();
                    DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                    return;
                }
                
                if(customer == null && !string.IsNullOrEmpty(email))
                {
                    customer = CustomerService.GetCustomerByEmail(email);
                }

                if (customer == null)
                {
                    customer = new Customer (CustomerGroupService.DefaultCustomerGroup)
                    {
                        EMail = email,
                        Enabled = true
                    };
                }

                if (customerInStrings.ContainsKey(ECustomerFields.FirstName))
                    customer.FirstName = Convert.ToString(customerInStrings[ECustomerFields.FirstName]);

                if (customerInStrings.ContainsKey(ECustomerFields.LastName))
                    customer.LastName = Convert.ToString(customerInStrings[ECustomerFields.LastName]);

                if (customerInStrings.ContainsKey(ECustomerFields.Patronymic))
                    customer.Patronymic = Convert.ToString(customerInStrings[ECustomerFields.Patronymic]);

                if (customerInStrings.ContainsKey(ECustomerFields.BirthDay))
                    customer.BirthDay = (DateTime?)customerInStrings[ECustomerFields.BirthDay];

                if (customerInStrings.ContainsKey(ECustomerFields.Phone))
                {
                    customer.Phone = Convert.ToString(customerInStrings[ECustomerFields.Phone]);
                    customer.StandardPhone = Convert.ToString(customerInStrings[ECustomerFields.Phone]).TryParseLong();
                }

                if (customerInStrings.ContainsKey(ECustomerFields.Enabled))
                    customer.Enabled = Convert.ToBoolean(customerInStrings[ECustomerFields.Enabled]);

                if (customerInStrings.ContainsKey(ECustomerFields.AdminComment))
                    customer.AdminComment = Convert.ToString(customerInStrings[ECustomerFields.AdminComment]);

                if (customerInStrings.ContainsKey(ECustomerFields.Organization))
                    customer.Organization = Convert.ToString(customerInStrings[ECustomerFields.Organization]);

                if (customerInStrings.ContainsKey(ECustomerFields.CustomerGroup))
                {
                    var customerGroupName = Convert.ToString(customerInStrings[ECustomerFields.CustomerGroup]);
                    var customerGroup = CustomerGroupService.GetCustomerGroup(customerGroupName);
                    if (customerGroup == null)
                    {
                        if (!string.IsNullOrWhiteSpace(customerGroupName))
                        {
                            customerGroup = new CustomerGroup
                            {
                                GroupDiscount = 0,
                                MinimumOrderPrice = 0,
                                GroupName = customerGroupName
                            };
                            CustomerGroupService.AddCustomerGroup(customerGroup);
                        }
                        else
                        {
                            customerGroup = CustomerGroupService.GetCustomerGroup(_defaultCustomerGroupId);
                            if (customerGroup == null)
                                customerGroup = CustomerGroupService.GetDefaultCustomerGroup();
                        }
                    }
                    customer.CustomerGroupId = customerGroup.CustomerGroupId;
                }
                else
                {
                    customer.CustomerGroupId = _defaultCustomerGroupId;
                }

                if (customerInStrings.ContainsKey(ECustomerFields.ManagerId))
                {
                    var manager = ManagerService.GetManager(Convert.ToString(customerInStrings[ECustomerFields.ManagerId]).TryParseInt());

                    if (manager != null)
                    {
                        customer.ManagerId = manager.ManagerId;
                    }
                }

                if (!customer.ManagerId.HasValue && customerInStrings.ContainsKey(ECustomerFields.ManagerName))
                {
                    var managerName = Convert.ToString(customerInStrings[ECustomerFields.ManagerName]);
                    var manager = ManagerService.GetManagersList().Where(x => managerName.Contains(x.FirstName) && managerName.Contains(x.LastName)).FirstOrDefault();

                    if (manager != null)
                    {
                        customer.ManagerId = manager.ManagerId;
                    }
                }

                if (customer.Id == Guid.Empty)
                {
                    customer.Id = CustomerService.InsertNewCustomer(customer);
                    DoForCommonStatistic(() => CommonStatistic.TotalAddRow++);
                    Log("покупатель добавлен " + customer.GetFullName());
                }
                else
                {
                    CustomerService.UpdateCustomer(customer);
                    DoForCommonStatistic(() => CommonStatistic.TotalUpdateRow++);
                    Log("покупатель обновлен " + customer.GetFullName());
                }

                if (customer.Id != Guid.Empty)
                {
                    CustomerContactFields(customerInStrings, customer.Id);
                    CustomerAdditionalFields(csv, customer.Id);
                }
                else
                {
                    Log("Не удалось добавить покупателя: " + customer.GetFullName());
                    DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                }
            }
            catch (Exception e)
            {
                Debug.Log.Error(e);
                DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                Log(CommonStatistic.RowPosition + ": " + e.Message);
            }

            customerInStrings.Clear();
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }

        private void CustomerContactFields(IDictionary<ECustomerFields, object> fields, Guid customerId)
        {
            var contacts = CustomerService.GetCustomerContacts(customerId);
            var customerContact = contacts != null && contacts.Count > 0 ? contacts[0] : new CustomerContact();

            if (fields.ContainsKey(ECustomerFields.Country) && !string.IsNullOrEmpty(Convert.ToString(fields[ECustomerFields.Country])))
            {
                var country = CountryService.GetCountryByName(Convert.ToString(fields[ECustomerFields.Country]));
                if (country == null)
                {
                    country = new Country
                    {
                        Name = Convert.ToString(fields[ECustomerFields.Country]),
                        Iso2= string.Empty,
                        Iso3 = string.Empty
                    };
                    CountryService.Add(country);
                }
                customerContact.Country = country.Name;
                customerContact.CountryId = country.CountryId;
            }
            if (fields.ContainsKey(ECustomerFields.Region) && !string.IsNullOrEmpty(Convert.ToString(fields[ECustomerFields.Region])))
            {
                var region = RegionService.GetRegion(Convert.ToString(fields[ECustomerFields.Region]), customerContact.CountryId);
                if (region == null)
                {
                    region = new Region
                    {
                        CountryId = customerContact.CountryId,
                        Name = Convert.ToString(fields[ECustomerFields.Region])
                    };
                    RegionService.InsertRegion(region);
                }
                customerContact.Region = region.Name;
                customerContact.RegionId = region.RegionId;
            }

            if (fields.ContainsKey(ECustomerFields.City) && !string.IsNullOrEmpty(Convert.ToString(fields[ECustomerFields.City])))
            {
                var city = CityService.GetCityByName(Convert.ToString(fields[ECustomerFields.City]));
                if (city == null)
                {
                    city = new City
                    {
                        Name = Convert.ToString(fields[ECustomerFields.City]),
                        RegionId = (int)customerContact.RegionId
                    };
                    CityService.Add(city);
                }
                customerContact.City = city.Name;
            }

            if (fields.ContainsKey(ECustomerFields.Zip))
            {
                customerContact.Zip = Convert.ToString(fields[ECustomerFields.Zip]);
            }
            if (fields.ContainsKey(ECustomerFields.House))
            {
                customerContact.House = Convert.ToString(fields[ECustomerFields.House]);
            }
            if (fields.ContainsKey(ECustomerFields.Street))
            {
                customerContact.Street = Convert.ToString(fields[ECustomerFields.Street]);
            }
            if (fields.ContainsKey(ECustomerFields.Apartment))
            {
                customerContact.Apartment = Convert.ToString(fields[ECustomerFields.Apartment]);
            }

            if (customerContact.ContactId == Guid.Empty)
                CustomerService.AddContact(customerContact, customerId);
            else
                CustomerService.UpdateContact(customerContact);
        }

        private void CustomerAdditionalFields(ICsvReader csv, Guid customerId)
        {
            foreach (var additionalField in CustomerFieldService.GetCustomerFields(true))
            {
                var nameField = additionalField.Name.ToLower();
                if (_fieldMapping.ContainsKey(nameField))
                {
                    var value = csv[_fieldMapping[nameField]];

                    if (additionalField.Required && value.IsNullOrEmpty())
                    {
                        LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.CanNotEmpty"), additionalField.Name, CommonStatistic.RowPosition + 2));
                        continue;
                    }

                    switch (additionalField.FieldType)
                    {
                        case CustomerFieldType.Text:
                        case CustomerFieldType.TextArea:
                            break;

                        case CustomerFieldType.Number:
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (!value.IsDecimal())
                                {
                                    LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), additionalField.Name, CommonStatistic.RowPosition + 2));
                                    continue;
                                }
                            }
                            break;

                        case CustomerFieldType.Date:
                            if (!string.IsNullOrEmpty(value))
                            {
                                var dt = value.TryParseDateTime(true);
                                if (!dt.HasValue)
                                {
                                    LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), additionalField.Name, CommonStatistic.RowPosition + 2));
                                    continue;
                                }

                                value = new CustomerFieldWithValue
                                {
                                    FieldType = CustomerFieldType.Date,
                                    Value = dt.Value.ToString("yyyy-MM-dd")
                                }.Value;
                            }
                            break;

                        case CustomerFieldType.Select:
                            if (!string.IsNullOrEmpty(value))
                            {
                                var tempField = new CustomerFieldWithValue
                                {
                                    FieldType = CustomerFieldType.Select,
                                    Id = additionalField.Id,
                                    Name = additionalField.Name,
                                    Enabled = additionalField.Enabled,
                                    Required = additionalField.Required,
                                    ShowInRegistration = additionalField.ShowInRegistration,
                                    ShowInCheckout = additionalField.ShowInCheckout
                                };

                                if (!tempField.Values.Any(x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase)))
                                {
                                    LogError(string.Format("Поле {0} в строке {1} имеет недопустимое значение", additionalField.Name, CommonStatistic.RowPosition + 2));
                                    continue;
                                }
                            }
                            break;
                    }

                    CustomerFieldService.AddUpdateMap(customerId, additionalField.Id, value);
                }
            }
        }

        #region Help methods

        private bool GetString(ECustomerFields rEnum, ICsvReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (_fieldMapping.ContainsKey(nameField))
                customerInStrings.Add(rEnum, TrimAnyWay(csv[_fieldMapping[nameField]]));
            return true;
        }

        private bool GetStringNotNull(ECustomerFields rEnum, ICsvReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                customerInStrings.Add(rEnum, tempValue);
            return true;
        }

        private bool GetStringRequired(ECustomerFields rEnum, ICsvReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                customerInStrings.Add(rEnum, tempValue);
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.CanNotEmpty"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDecimal(ECustomerFields rEnum, ICsvReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            float decValue;
            if (float.TryParse(value, out decValue))
            {
                customerInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                customerInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDecimal(ECustomerFields rEnum, ICsvReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);

            if (string.IsNullOrEmpty(value))
            {
                customerInStrings.Add(rEnum, default(float?));
                return true;
            }

            float decValue;
            if (float.TryParse(value, out decValue))
            {
                customerInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                customerInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetInt(ECustomerFields rEnum, ICsvReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            int intValue;
            if (int.TryParse(value, out intValue))
            {
                customerInStrings.Add(rEnum, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDateTime(ECustomerFields rEnum, ICsvReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = default(DateTime).ToString(CultureInfo.InvariantCulture);
            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                customerInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                customerInStrings.Add(rEnum, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDateTime(ECustomerFields rEnum, ICsvReaderRow csv, IDictionary<ECustomerFields, object> customerInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);

            if (string.IsNullOrEmpty(value))
            {
                customerInStrings.Add(rEnum, default(DateTime?));
                return true;
            }

            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                customerInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                customerInStrings.Add(rEnum, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private static string TrimAnyWay(string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Trim();
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

        private void Log(string message)
        {
            DoForCommonStatistic(() => CommonStatistic.WriteLog(message));
        }

        protected void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }

        #endregion
    }
}