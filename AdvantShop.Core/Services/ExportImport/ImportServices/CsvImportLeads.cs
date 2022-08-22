using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Statistic;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AdvantShop.Core.Services.ExportImport.ImportServices
{
    public class CsvImportLeads
    {
        private readonly string _fullPath;
        private readonly bool _hasHeadrs;

        private readonly string _columnSeparator;
        private readonly string _propertySeparator;
        private readonly string _propertyValueSeparator;
        private readonly string _encodings;
        private readonly int _baseSalesFunnelId;

        private readonly bool _updateCustomer;
        private readonly bool _doNotDuplicate;

        private readonly List<LeadField> _leadFields;

        private Dictionary<string, int> _fieldMapping;
        private bool _useCommonStatistic;

        public CsvImportLeads(string filePath, bool hasHeadrs, string columnSeparator, string propertySeparator,
            string propertyValueSeparator, string encodings, Dictionary<string, int> fieldMapping, int baseSalesFunnelId = 0, bool updateCustomer = true,
            bool doNotDuplicate = false, bool useCommonStatistic = true)
        {
            _fullPath = filePath;
            _hasHeadrs = hasHeadrs;
            _fieldMapping = fieldMapping;
            _encodings = encodings;
            _columnSeparator = columnSeparator;
            _propertySeparator = propertySeparator;
            _propertyValueSeparator = propertyValueSeparator;
            _baseSalesFunnelId = baseSalesFunnelId;

            _updateCustomer = updateCustomer;
            _doNotDuplicate = doNotDuplicate;
            _useCommonStatistic = useCommonStatistic;

            _leadFields = LeadFieldService.GetLeadFields();
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var reader = new CsvReader(new StreamReader(_fullPath, Encoding.GetEncoding(_encodings ?? EncodingsEnum.Utf8.StrName())));

            reader.Configuration.Delimiter = _columnSeparator ?? SeparatorsEnum.SemicolonSeparated.StrName();
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

        public System.Threading.Tasks.Task<bool> ProcessThroughACommonStatistic(
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

        private void _process()
        {
            DoForCommonStatistic(() => CommonStatistic.WriteLog("Start of import"));

            if (_fieldMapping == null)
                MapFields();

            if (_fieldMapping == null)
                throw new Exception("can't map colums");

            DoForCommonStatistic(() => CommonStatistic.TotalRow = GetRowCount());

            ProcessRow();

            CacheManager.Clean();
            FileHelpers.DeleteFile(_fullPath);

            DoForCommonStatistic(() => CommonStatistic.WriteLog("End of import"));
        }

        private void MapFields()
        {
            _fieldMapping = new Dictionary<string, int>();
            using (var csv = InitReader(false))
            {
                csv.Read();
                for (var i = 0; i < csv.CurrentRecord.Length; i++)
                {
                    if (csv.CurrentRecord[i] == ELeadFields.None.StrName()) continue;
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
                        var leadInString = PrepareRow(csv);
                        if (leadInString == null)
                        {
                            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                            continue;
                        }

                        ProcessLead(leadInString, csv);
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

        private Dictionary<ELeadFields, object> PrepareRow(ICsvReader csv)
        {
            var leadInStrings = new Dictionary<ELeadFields, object>();

            foreach (ELeadFields field in Enum.GetValues(typeof(ELeadFields)))
            {
                switch (field.Status())
                {
                    case CsvFieldStatus.String:
                        GetString(field, csv, leadInStrings);
                        break;
                    case CsvFieldStatus.StringRequired:
                        GetStringRequired(field, csv, leadInStrings);
                        break;
                    case CsvFieldStatus.NotEmptyString:
                        GetStringNotNull(field, csv, leadInStrings);
                        break;
                    case CsvFieldStatus.Float:
                        if (!GetDecimal(field, csv, leadInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.NullableFloat:
                        if (!GetNullableDecimal(field, csv, leadInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.Int:
                        if (!GetInt(field, csv, leadInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.DateTime:
                        if (!GetDateTime(field, csv, leadInStrings))
                            return null;
                        break;
                    case CsvFieldStatus.NullableDateTime:
                        if (!GetNullableDateTime(field, csv, leadInStrings))
                            return null;
                        break;
                }
            }
            return leadInStrings;
        }

        private void ProcessLead(Dictionary<ELeadFields, object> leadInStrings, ICsvReader csv)
        {
            try
            {
                var orderSource = OrderSourceService.GetOrderSource(OrderType.LeadImport);
                Lead lead = new Lead
                {
                    OrderSourceId = orderSource != null ? orderSource.Id : 0
                };
                List<CustomerFieldWithValue> customerFields = null;

                if (leadInStrings.ContainsKey(ELeadFields.SalesFunnel))
                {
                    var salesFennelId = Convert.ToString(leadInStrings[ELeadFields.SalesFunnel]).TryParseInt();
                    if (salesFennelId != 0)
                    {
                        if (SalesFunnelService.Get(salesFennelId) != null)
                            lead.SalesFunnelId = salesFennelId;
                    }
                    else
                    {
                        var salesFennel = SalesFunnelService.GetList().FirstOrDefault(x => x.Name.ToLower() == Convert.ToString(leadInStrings[ELeadFields.SalesFunnel]).ToLower());
                        if (salesFennel != null)
                        {
                            lead.SalesFunnelId = salesFennel.Id;
                        }
                    }
                }
                if (lead.SalesFunnelId == 0)
                {
                    lead.SalesFunnelId = _baseSalesFunnelId;
                }

                if (leadInStrings.ContainsKey(ELeadFields.DealStatus))
                {
                    var dealStatusId = Convert.ToString(leadInStrings[ELeadFields.DealStatus]).TryParseInt();
                    if (dealStatusId != 0)
                    {
                        if (DealStatusService.Get(dealStatusId) != null)
                            lead.DealStatusId = dealStatusId;
                    }
                    else if (lead.SalesFunnelId != 0)
                    {
                        var dealStatus = DealStatusService.GetList(lead.SalesFunnelId).FirstOrDefault(x => x.Name.ToLower() == Convert.ToString(leadInStrings[ELeadFields.DealStatus]).ToLower());
                        if (dealStatus != null)
                        {
                            lead.DealStatusId = dealStatus.Id;
                        }
                    }
                }

                var customerId = leadInStrings.ContainsKey(ELeadFields.CustomerId)
                    ? Convert.ToString(leadInStrings[ELeadFields.CustomerId]).TryParseGuid()
                    : Guid.Empty;

                var email = lead.Email = leadInStrings.ContainsKey(ELeadFields.Email)
                    ? Convert.ToString(leadInStrings[ELeadFields.Email])
                    : string.Empty;

                var phone = lead.Phone = leadInStrings.ContainsKey(ELeadFields.Phone)
                    ? Convert.ToString(leadInStrings[ELeadFields.Phone])
                    : string.Empty;

                if (string.IsNullOrEmpty(email) && customerId == Guid.Empty && string.IsNullOrEmpty(phone))
                {
                    DoForCommonStatistic(() =>
                    {
                        CommonStatistic.TotalErrorRow++;
                        CommonStatistic.WriteLog(CommonStatistic.RowPosition + ": no email, customer id or phone");
                        CommonStatistic.RowPosition++;
                    });
                    return;
                }

                var leadFields = GetLeadFieldsWithValues(csv, lead.SalesFunnelId);

                if (customerId != Guid.Empty)
                {
                    lead.Customer = CustomerService.GetCustomer(customerId);
                    if (lead.Customer != null)
                    {
                        lead.CustomerId = lead.Customer.Id;
                        lead.Email = lead.Customer.EMail;
                    }
                }

                if (lead.Customer == null && !string.IsNullOrEmpty(email))
                {
                    lead.Customer = CustomerService.GetCustomerByEmail(email);
                    if (lead.Customer != null)
                    {
                        lead.CustomerId = lead.Customer.Id;
                        lead.Email = lead.Customer.EMail;
                    }
                }

                if (lead.Customer == null && !string.IsNullOrEmpty(phone))
                {
                    lead.Customer = CustomerService.GetCustomersByPhone(phone).FirstOrDefault();
                    if (lead.Customer != null)
                    {
                        lead.CustomerId = lead.Customer.Id;
                        lead.Email = lead.Customer.EMail;
                        lead.Phone = lead.Customer.Phone;
                    }
                }

                if (_updateCustomer || lead.Customer == null)
                {
                    if (lead.Customer == null)
                        lead.Customer = new Customer();

                    lead.Email = lead.Customer.EMail = email;
                    lead.Phone = lead.Customer.Phone = phone;
                    lead.Customer.StandardPhone = StringHelper.ConvertToStandardPhone(lead.Phone, true, true);

                    if (leadInStrings.ContainsKey(ELeadFields.FirstName))
                        lead.Customer.FirstName = Convert.ToString(leadInStrings[ELeadFields.FirstName]);

                    if (leadInStrings.ContainsKey(ELeadFields.LastName))
                        lead.Customer.LastName = Convert.ToString(leadInStrings[ELeadFields.LastName]);

                    if (leadInStrings.ContainsKey(ELeadFields.Patronymic))
                        lead.Customer.Patronymic = Convert.ToString(leadInStrings[ELeadFields.Patronymic]);

                    if (leadInStrings.ContainsKey(ELeadFields.Organization))
                        lead.Customer.Organization = Convert.ToString(leadInStrings[ELeadFields.Organization]);
                    
                    if (leadInStrings.ContainsKey(ELeadFields.BirthDay))
                        lead.Customer.BirthDay = (DateTime?)leadInStrings[ELeadFields.BirthDay];

                    if (leadInStrings.ContainsKey(ELeadFields.Title))
                        lead.Title = Convert.ToString(leadInStrings[ELeadFields.Title]);

                    if (leadInStrings.ContainsKey(ELeadFields.Description))
                        lead.Description = Convert.ToString(leadInStrings[ELeadFields.Description]);


                    if (leadInStrings.ContainsKey(ELeadFields.Country) || leadInStrings.ContainsKey(ELeadFields.Region) || leadInStrings.ContainsKey(ELeadFields.City))
                    {
                        var contact = new CustomerContact
                        {
                            Country = leadInStrings.ContainsKey(ELeadFields.Country)
                                ? Convert.ToString(leadInStrings[ELeadFields.Country])
                                : string.Empty,
                            Region = leadInStrings.ContainsKey(ELeadFields.Region)
                                ? Convert.ToString(leadInStrings[ELeadFields.Region])
                                : string.Empty,
                            City = leadInStrings.ContainsKey(ELeadFields.City)
                                ? Convert.ToString(leadInStrings[ELeadFields.City])
                                : string.Empty
                        };

                        if (lead.Customer.Contacts != null && lead.Customer.Contacts.Count > 0)
                        {
                            lead.Customer.Contacts[0].Country = contact.Country;
                            lead.Customer.Contacts[0].Region = contact.Region;
                            lead.Customer.Contacts[0].City = contact.City;
                        }
                        else
                        {
                            lead.Customer.Contacts.Add(contact);
                        }
                    }

                    customerFields = CustomerAdditionalFields(csv);
                }

                if (leadInStrings.ContainsKey(ELeadFields.ManagerName))
                {
                    var managerName = Convert.ToString(leadInStrings[ELeadFields.ManagerName]).ToLower();
                    var manager = ManagerService.GetManagersList().Where(x => managerName.Contains(x.FirstName.ToLower()) && managerName.Contains(x.LastName.ToLower())).FirstOrDefault();

                    if (manager != null)
                    {
                        lead.ManagerId = manager.ManagerId;
                    }
                }
                
                if (leadInStrings.ContainsKey(ELeadFields.MultiOffer))
                {
                    lead.LeadItems = LeadItemsFromString(Convert.ToString(leadInStrings[ELeadFields.MultiOffer]));
                    if (lead.LeadItems != null && lead.LeadItems.Count > 0)
                        lead.Sum = lead.LeadItems.Sum(x => x.Amount * x.Price);
                }


                if (lead.Customer.RegistredUser && _updateCustomer)
                {
                    UpdateCustomer(lead.Customer, customerFields);
                    DoForCommonStatistic(() =>
                    {
                        //CommonStatistic.TotalUpdateRow++;
                        CommonStatistic.WriteLog("Customer updated: " + lead.Customer.Id);
                    });
                }

                if (!_doNotDuplicate || !LeadService.GetLeadsByCustomer(lead.Customer.Id).Any(x => x.SalesFunnelId == lead.SalesFunnelId))
                {
                    LeadService.AddLead(lead, isFromAdminArea: true, trackChanges: false, customerFields: customerFields, leadFields: leadFields);
                    DoForCommonStatistic(() =>
                    {
                        CommonStatistic.TotalAddRow++;
                        CommonStatistic.WriteLog("Lead added: " + lead.Id);
                    });
                }
            }
            catch (Exception e)
            {
                Debug.Log.Error(e);
                DoForCommonStatistic(() =>
                {
                    CommonStatistic.TotalErrorRow++;
                    CommonStatistic.WriteLog(CommonStatistic.RowPosition + ": " + e.Message);
                });
            }

            leadInStrings.Clear();
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }

        private List<CustomerFieldWithValue> CustomerAdditionalFields(ICsvReader csv)
        {
            var customerAdditionalFields = new List<CustomerFieldWithValue>();
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
                                    ShowInCheckout= additionalField.ShowInCheckout
                                };

                                if (!tempField.Values.Any(x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase)))
                                {
                                    LogError(string.Format("Поле {0} в строке {1} имеет недопустимое значение", additionalField.Name, CommonStatistic.RowPosition + 2));
                                    continue;
                                }
                            }
                            break;
                    }

                    customerAdditionalFields.Add(new CustomerFieldWithValue
                    {
                        Id = additionalField.Id,
                        Value = value
                    });
                }
            }
            return customerAdditionalFields;
        }

        private List<LeadFieldWithValue> GetLeadFieldsWithValues(ICsvReader csv, int salesFunnelId)
        {
            var leadFields = new List<LeadFieldWithValue>();
            foreach (var field in _leadFields.Where(x => x.SalesFunnelId == salesFunnelId))
            {
                var nameField = field.Name.ToLower();
                if (_fieldMapping.ContainsKey(nameField))
                {
                    var value = csv[_fieldMapping[nameField]];

                    if (field.Required && value.IsNullOrEmpty())
                    {
                        LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.CanNotEmpty", field.Name, CommonStatistic.RowPosition + 2));
                        continue;
                    }

                    switch (field.FieldType)
                    {
                        case LeadFieldType.Text:
                        case LeadFieldType.TextArea:
                            break;

                        case LeadFieldType.Number:
                            if (value.IsNotEmpty() && !value.IsDecimal())
                            {
                                LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.MustBeNumber", field.Name, CommonStatistic.RowPosition + 2));
                                continue;
                            }
                            break;

                        case LeadFieldType.Date:
                            if (value.IsNotEmpty())
                            {
                                var dt = value.TryParseDateTime(true);
                                if (!dt.HasValue)
                                {
                                    LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.MustBeDateTime", field.Name, CommonStatistic.RowPosition + 2));
                                    continue;
                                }

                                value = dt.Value.ToString("yyyy-MM-dd");
                            }
                            break;

                        case LeadFieldType.Select:
                            if (value.IsNotEmpty())
                            {
                                var values = LeadFieldService.GetLeadFieldValues(field.Id);
                                if (!values.Any(x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase)))
                                {
                                    LogError(string.Format("Поле {0} в строке {1} имеет недопустимое значение", field.Name, CommonStatistic.RowPosition + 2));
                                    continue;
                                }
                            }
                            break;
                    }

                    leadFields.Add(new LeadFieldWithValue
                    {
                        Id = field.Id,
                        Value = value
                    });
                }
            }
            return leadFields;
        }

        private List<LeadItem> LeadItemsFromString(string leadItemsString)
        {
            var leadItems = new List<LeadItem>();

            foreach (string[] fields in leadItemsString.Replace("[", "").Replace("]", "")
                .Split(new[] { _propertySeparator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(str => str.Split(new[] { _propertyValueSeparator }, StringSplitOptions.RemoveEmptyEntries)))
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    fields[i] = fields[i].SupperTrim();
                }

                if (fields.Count() == 3)
                {
                    var offer = OfferService.GetOffer(fields[0]);
                    float? price = fields[1] == "null" ? null : fields[1].TryParseFloat(true);
                    if (offer != null)
                    {
                        leadItems.Add(new LeadItem(offer, fields[2].TryParseFloat(), price));
                    }
                }
            }

            return leadItems;
        }

        private void UpdateCustomer(Customer customer, List<CustomerFieldWithValue> customerFields = null)
        {
            CustomerService.UpdateCustomer(customer);

            if(customer.Contacts != null && customer.Contacts.Count > 0)
            {
                if (customer.Contacts[0].ContactId == Guid.Empty)
                    CustomerService.AddContact(customer.Contacts[0], customer.Id);
                else
                    CustomerService.UpdateContact(customer.Contacts[0]);
            }
            
            if (customerFields != null && customerFields.Count > 0)
            {
                foreach (var field in customerFields)
                    CustomerFieldService.AddUpdateMap(customer.Id, field.Id, field.Value ?? "");
            }
        }

        #region HelpMethods

        private bool GetString(ELeadFields rEnum, ICsvReaderRow csv, IDictionary<ELeadFields, object> leadInStrings)
        {
            var nameField = rEnum.StrName();
            if (_fieldMapping.ContainsKey(nameField))
                leadInStrings.Add(rEnum, TrimAnyWay(csv[_fieldMapping[nameField]]));
            return true;
        }

        private bool GetStringRequired(ELeadFields rEnum, ICsvReaderRow csv, IDictionary<ELeadFields, object> leadInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                leadInStrings.Add(rEnum, tempValue);
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.CanNotEmpty"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetStringNotNull(ELeadFields rEnum, ICsvReaderRow csv, IDictionary<ELeadFields, object> leadInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                leadInStrings.Add(rEnum, tempValue);
            return true;
        }

        private bool GetDecimal(ELeadFields rEnum, ICsvReaderRow csv, IDictionary<ELeadFields, object> leadInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            float decValue;
            if (float.TryParse(value, out decValue))
            {
                leadInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                leadInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDecimal(ELeadFields rEnum, ICsvReaderRow csv, IDictionary<ELeadFields, object> leadInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);

            if (string.IsNullOrEmpty(value))
            {
                leadInStrings.Add(rEnum, default(float?));
                return true;
            }

            float decValue;
            if (float.TryParse(value, out decValue))
            {
                leadInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                leadInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetInt(ELeadFields rEnum, ICsvReaderRow csv, IDictionary<ELeadFields, object> leadInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            int intValue;
            if (int.TryParse(value, out intValue))
            {
                leadInStrings.Add(rEnum, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDateTime(ELeadFields rEnum, ICsvReaderRow csv, IDictionary<ELeadFields, object> leadInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = default(DateTime).ToString(CultureInfo.InvariantCulture);
            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                leadInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                leadInStrings.Add(rEnum, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDateTime(ELeadFields rEnum, ICsvReaderRow csv, IDictionary<ELeadFields, object> leadInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);

            if (string.IsNullOrEmpty(value))
            {
                leadInStrings.Add(rEnum, default(DateTime?));
                return true;
            }

            DateTime dateValue;
            if (DateTime.TryParse(value, out dateValue))
            {
                leadInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                leadInStrings.Add(rEnum, dateValue);
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
            });
        }

        protected void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }

        #endregion
    }
}
