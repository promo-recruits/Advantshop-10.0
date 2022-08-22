//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Loging;
using AdvantShop.Core.Services.Loging.Calls;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Core.Services.Loging.Events;
using AdvantShop.Core.Services.Loging.Smses;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Core.Services.Triggers.DeferredDatas;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Customers
{
    public class CustomerService
    {
        private const string GuestCookieName = "customer_guest";

        private static IEmailLoger EmailLoger { get { return LogingManager.GetEmailLoger(); } }
        private static ISmsLoger SmsLoger { get { return LogingManager.GetSmsLoger(); } }
        private static IEventLoger EventLoger { get { return LogingManager.GetEventLoger(); } }
        private static ICallLoger CallLoger { get { return LogingManager.GetCallLoger(); } }

        public static void DeleteCustomer(Guid customerId)
        {
            var customer = GetCustomer(customerId);

            SubscriptionService.Unsubscribe(customer.EMail);
            CardService.Delete(customerId);
            SQLDataAccess.ExecuteNonQuery("[Customers].[sp_DeleteCustomer]", CommandType.StoredProcedure,
                new SqlParameter("@CustomerID", customerId));

            ModulesExecuter.DeleteCustomer(customerId);

            TriggerDeferredDataService.Delete(ETriggerObjectType.Customer, customer.InnerId);

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static void DeleteContact(Guid contactId)
        {
            SQLDataAccess.ExecuteNonQuery("[Customers].[sp_DeleteCustomerContact]", CommandType.StoredProcedure,
                new SqlParameter("@ContactID", contactId));

            ModulesExecuter.DeleteContact(contactId);

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static int GetCustomerGroupId(Guid customerId)
        {
            return
                SQLDataHelper.GetInt(
                    SQLDataAccess.ExecuteScalar(
                        "SELECT [CustomerGroupId] FROM [Customers].[Customer] WHERE [CustomerID] = @CustomerID",
                        CommandType.Text, new SqlParameter { ParameterName = "@CustomerID", Value = customerId }),
                    CustomerGroupService.DefaultCustomerGroup);
        }

        public static Customer GetCustomer(Guid customerId)
        {
            Customer customer;

            var cacheName = CacheNames.Customer + customerId;

            if (!CacheManager.TryGetValue(cacheName, out customer))
            {
                customer = GetCustomerFromDb(customerId);
                CacheManager.Insert(cacheName, customer ?? new Customer());
            }

            return customer != null && customer.Id != Guid.Empty ? customer : null;
        }

        public static Customer GetCustomerFromDb(Guid customerId)
        {
            return SQLDataAccess.ExecuteReadOne<Customer>(
                    "SELECT * FROM [Customers].[Customer] WHERE [CustomerID] = @CustomerID",
                    CommandType.Text,
                    GetFromSqlDataReader,
                    new SqlParameter("@CustomerID", customerId));
        }

        public static Customer GetCustomer(int innerId)
        {
            return
                SQLDataAccess.ExecuteReadOne<Customer>(
                    "SELECT * FROM [Customers].[Customer] WHERE [InnerId] = @InnerId",
                    CommandType.Text,
                    GetFromSqlDataReader,
                    new SqlParameter("@InnerId", innerId));
        }

        public static List<Customer> GetCustomersbyRole(Role role)
        {
            return
                SQLDataAccess.ExecuteReadList<Customer>(
                    "SELECT * FROM [Customers].[Customer] WHERE [CustomerRole] = @CustomerRole",
                    CommandType.Text,
                    GetFromSqlDataReader,
                    new SqlParameter("@CustomerRole", ((int)role).ToString()));
        }

        public static List<Customer> GetCustomersByRoles(Role role, params Role[] roles)
        {
            var rolesList = new List<int>() { (int)role };
            rolesList.AddRange(roles.Where(x => role != x).Select(x => (int)x));
            return
                SQLDataAccess.ExecuteReadList<Customer>(
                    string.Format("SELECT * FROM [Customers].[Customer] WHERE [CustomerRole] in ({0})", rolesList.AggregateString(",")),
                    CommandType.Text,
                    GetFromSqlDataReader);
        }

        public static List<Customer> GetCustomersForAutocomplete(string query)
        {
            if (query.IsDecimal())
            {
                return
                SQLDataAccess.ExecuteReadList<Customer>(
                    "SELECT * FROM [Customers].[Customer] " +
                    "WHERE [CustomerRole] = @CustomerRole AND (" +

                    (query.Length >= 6 ?
                    "[Phone] like '%' + @q + '%' " +
                    "OR [StandardPhone] like '%' + @q + '%' " +
                    "OR "
                    : "") +

                    "[Email] like @q + '%')",
                    CommandType.Text,
                    GetFromSqlDataReader,
                    new SqlParameter("@q", query),
                    new SqlParameter("@CustomerRole", ((int)Role.User).ToString()));
            }
            else
            {
                var translitKeyboard = StringHelper.TranslitToRusKeyboard(query);

                return
                    SQLDataAccess.ExecuteReadList<Customer>(
                        "SELECT * FROM [Customers].[Customer] " +
                        "WHERE [CustomerRole] = @CustomerRole AND " +
                        "([FirstName] like @q + '%' OR [FirstName] like @qtr + '%' " +
                        "OR [LastName] like @q + '%' OR [LastName] like @qtr + '%' " +
                        "OR [Organization] like @q + '%' OR [Organization] like @qtr + '%' " +
                        "OR [Phone] like '%' + @q + '%' " +
                        "OR [Email] like @q + '%')",
                        CommandType.Text,
                        GetFromSqlDataReader,
                        new SqlParameter("@q", query),
                        new SqlParameter("@qtr", translitKeyboard),
                        new SqlParameter("@CustomerRole", ((int)Role.User).ToString()));
            }
        }

        public static bool ExistsCustomer(Guid customerId)
        {
            bool isExist;
            var cacheName = CacheNames.Customer + customerId + "_ExistsCustomer";

            if (!CacheManager.TryGetValue(cacheName, out isExist))
            {
                isExist = SQLDataAccess.ExecuteScalar(
                              "SELECT [CustomerID] FROM [Customers].[Customer] WHERE [CustomerID] = @CustomerID",
                              CommandType.Text,
                              new SqlParameter("@CustomerID", customerId)) != null;

                CacheManager.Insert(cacheName, isExist, 1);
            }
            return isExist;
        }

        public static Customer GetCustomerByEmail(string email)
        {
            return SQLDataAccess.ExecuteReadOne<Customer>(
                "[Customers].[sp_GetCustomerByEmail]", CommandType.StoredProcedure,
                GetFromSqlDataReader, new SqlParameter("@Email", email));
        }

        public static List<Customer> GetCustomersByPhone(string phone)
        {
            var phoneLong = phone.TryParseLong(true);
            return SQLDataAccess.ExecuteReadList<Customer>(
                "Select * from Customers.Customer where Phone=@phone " + (phoneLong.HasValue ? "or StandardPhone=@phoneLong" : string.Empty), CommandType.Text,
                GetFromSqlDataReader,
                new SqlParameter("@phone", phone),
                new SqlParameter("@phoneLong", phoneLong ?? (object)DBNull.Value)
                );
        }

        public static Customer GetCustomerByPhone(string phone, long? standardPhone)
        {
            return SQLDataAccess.ExecuteReadOne(
                "Select top(1) * " +
                "From Customers.Customer " +
                "Where Phone=@Phone " + (standardPhone != null && standardPhone != 0 ? "or StandardPhone=@StandardPhone" : ""),
                CommandType.Text,
                GetFromSqlDataReader,
                new SqlParameter("@Phone", phone),
                new SqlParameter("@StandardPhone", standardPhone ?? (object)DBNull.Value));
        }


        public static Customer GetCustomerByEmailAndPhone(string email, string phone, long? standardPhone)
        {
            return SQLDataAccess.ExecuteReadOne(
                "Select top(1) * " +
                "From Customers.Customer " +
                "Where Email = @Email And (Phone=@Phone " + (standardPhone != null && standardPhone != 0 ? "or StandardPhone=@StandardPhone" : "") + ")",
                CommandType.Text,
                GetFromSqlDataReader,
                new SqlParameter("@Email", email),
                new SqlParameter("@Phone", phone),
                new SqlParameter("@StandardPhone", standardPhone ?? (object) DBNull.Value));
        }

        public static List<Customer> GetCustomerBirthdayInTheNextWeek()
        {
            var dateFrom = DateTime.Today.AddDays(-3);
            var dateTo = DateTime.Today.AddDays(7);
            var list =
                SQLDataAccess.ExecuteReadList<Customer>(
                    "SELECT TOP(5) * " +
                    "FROM [Customers].[Customer] " +
                    "Where Enabled = 1 and [BirthDay] is not null and " +
                    "(CustomerRole = " + (int)Role.Administrator + " or CustomerRole = " + (int)Role.Moderator + ") and " +
                    "( " +
                    "Convert(Date, '" + dateTo.Year + "' + '-' + Convert(Varchar, Month(BirthDay)) + '-' + Convert(Varchar,Day(BirthDay))) Between @DateFrom And @DateTo " +
                    (dateFrom.Year != dateTo.Year
                        ? "or Convert(Date, '" + dateFrom.Year + "' + '-'  + Convert(Varchar, Month(BirthDay)) + '-' + Convert(Varchar,Day(BirthDay))) Between @DateFrom And @DateTo "
                        : "") +
                    ") " +
                    "Order by Month(BirthDay), Day(BirthDay)",
                    CommandType.Text,
                    GetFromSqlDataReader,
                    new SqlParameter("@DateFrom", dateFrom),
                    new SqlParameter("@DateTo", dateTo));

            return list;
        }

        public static Customer GetCustomerByRole(Role role)
        {
            return SQLDataAccess.ExecuteReadOne<Customer>(
                "Select top(1) * from Customers.Customer where CustomerRole=@CustomerRole", CommandType.Text,
                GetFromSqlDataReader, new SqlParameter { ParameterName = "@CustomerRole", Value = role });
        }


        public static Customer GetCustomerByOpenAuthIdentifier(string identifier)
        {
            return SQLDataAccess.ExecuteReadOne<Customer>(
                "[Customers].[sp_GetCustomerByOpenAuthIdentifier]", CommandType.StoredProcedure,
                GetFromSqlDataReader, new SqlParameter { ParameterName = "@Identifier", Value = identifier });
        }

        public static Customer GetFromSqlDataReader(SqlDataReader reader)
        {
            var customer = new Customer(true)
            {
                Id = SQLDataHelper.GetGuid(reader, "CustomerID"),
                CustomerGroupId = SQLDataHelper.GetInt(reader, "CustomerGroupId", 0),
                EMail = SQLDataHelper.GetString(reader, "EMail"),
                FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                LastName = SQLDataHelper.GetString(reader, "LastName"),
                Patronymic = SQLDataHelper.GetString(reader, "Patronymic"),
                RegistrationDateTime = SQLDataHelper.GetDateTime(reader, "RegistrationDateTime"),
                Phone = SQLDataHelper.GetString(reader, "Phone"),
                StandardPhone = SQLDataHelper.GetNullableLong(reader, "StandardPhone"),
                Password = SQLDataHelper.GetString(reader, "Password"),
                CustomerRole = (Role)SQLDataHelper.GetInt(reader, "CustomerRole"),
                BonusCardNumber = SQLDataHelper.GetNullableLong(reader, "BonusCardNumber"),
                AdminComment = SQLDataHelper.GetString(reader, "AdminComment"),
                ManagerId = SQLDataHelper.GetNullableInt(reader, "ManagerId"),
                Rating = SQLDataHelper.GetInt(reader, "Rating"),
                Avatar = SQLDataHelper.GetString(reader, "Avatar"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                HeadCustomerId = SQLDataHelper.GetNullableGuid(reader, "HeadCustomerId"),
                BirthDay = SQLDataHelper.GetNullableDateTime(reader, "BirthDay"),
                City = SQLDataHelper.GetString(reader, "City"),
                InnerId = SQLDataHelper.GetInt(reader, "InnerId"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                Organization = SQLDataHelper.GetString(reader, "Organization"),
                ClientStatus = (CustomerClientStatus)SQLDataHelper.GetInt(reader, "ClientStatus"),
                RegisteredFrom = SQLDataHelper.GetString(reader, "RegisteredFrom")
            };

            return customer;
        }

        public static CustomerContact GetContactFromSqlDataReader(SqlDataReader reader)
        {
            var contact = new CustomerContact
            {
                ContactId = SQLDataHelper.GetGuid(reader, "ContactID"),
                CustomerGuid = SQLDataHelper.GetGuid(reader, "CustomerID"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                City = SQLDataHelper.GetString(reader, "City"),
                District = SQLDataHelper.GetString(reader, "District"),
                Country = SQLDataHelper.GetString(reader, "Country"),
                Zip = SQLDataHelper.GetString(reader, "Zip"),
                Region = SQLDataHelper.GetString(reader, "Zone"),
                CountryId = SQLDataHelper.GetInt(reader, "CountryID"),
                RegionId = SQLDataHelper.GetNullableInt(reader, "RegionID"),

                Street = SQLDataHelper.GetString(reader, "Street"),
                House = SQLDataHelper.GetString(reader, "House"),
                Apartment = SQLDataHelper.GetString(reader, "Apartment"),
                Structure = SQLDataHelper.GetString(reader, "Structure"),
                Entrance = SQLDataHelper.GetString(reader, "Entrance"),
                Floor = SQLDataHelper.GetString(reader, "Floor"),
            };

            return contact;
        }

        public static CustomerContact GetCustomerContact(string contactId)
        {
            var id = Guid.Empty;
            if (Guid.TryParse(contactId, out id))
            {
                return GetCustomerContact(id);
            }
            return null;
        }

        public static CustomerContact GetCustomerContact(Guid contactId)
        {
            var contact = SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Customers].[Contact] WHERE [ContactID] = @id",
                CommandType.Text,
                GetContactFromSqlDataReader,
                new SqlParameter("@id", contactId));

            return contact;
        }

        public static List<CustomerContact> GetCustomerContacts(Guid customerId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Customers].[Contact] WHERE CustomerId = @CustomerId",
                CommandType.Text, GetContactFromSqlDataReader,
                new SqlParameter("@CustomerId", customerId));
        }

        public static IList<Customer> GetCustomers()
        {
            return SQLDataAccess.ExecuteReadList<Customer>("SELECT * FROM [Customers].[Customer]", CommandType.Text,
                GetFromSqlDataReader);
        }

        public static IList<Customer> GetCustomers(DateTime from, DateTime to, int? topNum = null)
        {
            return SQLDataAccess.ExecuteReadList<Customer>(
                topNum.HasValue
                    ? "SELECT TOP(" + topNum.Value + ") * FROM [Customers].[Customer] Where RegistrationDateTime >= @from and RegistrationDateTime <= @to"
                    : "SELECT * FROM [Customers].[Customer] Where RegistrationDateTime >= @from and RegistrationDateTime <= @to",
                CommandType.Text,
                GetFromSqlDataReader,
                new SqlParameter("@from", from),
                new SqlParameter("@to", to));
        }

        public static Dictionary<Guid, long> GetCustomersPhones()
        {
            var dict = new Dictionary<Guid, long>();
            dict.AddRange(
                SQLDataAccess.ExecuteReadIEnumerable<KeyValuePair<Guid, long>>(
                    "SELECT distinct CustomerId, StandardPhone FROM [Customers].[Customer] where StandardPhone is not null",
                    CommandType.Text,
                    reader =>
                        new KeyValuePair<Guid, long>(SQLDataHelper.GetGuid(reader, "CustomerID"),
                            SQLDataHelper.GetLong(reader, "StandardPhone"))));
            return dict;
        }

        public static Dictionary<Guid, long> GetSubscribedCustomersPhones()
        {
            var dict = new Dictionary<Guid, long>();
            dict.AddRange(
                SQLDataAccess.ExecuteReadIEnumerable<KeyValuePair<Guid, long>>(
                    "SELECT distinct CustomerId, StandardPhone FROM [Customers].[Customer] " +
                    "INNER JOIN [Customers].[Subscription] ON [Subscription].[Email] = [Customer].[Email] " +
                    "where StandardPhone is not null AND Subscribe = 1",
                    CommandType.Text,
                    reader =>
                        new KeyValuePair<Guid, long>(SQLDataHelper.GetGuid(reader, "CustomerID"),
                            SQLDataHelper.GetLong(reader, "StandardPhone"))));
            return dict;
        }


        public static Guid AddContact(CustomerContact contact, Guid customerId)
        {
            var id = SQLDataAccess.ExecuteScalar(
                "INSERT INTO [Customers].[Contact] " +
                "(CustomerID, Name, Country, City, Zone, Zip, CountryID, RegionID, Street, House, Apartment, Structure, Entrance, Floor, District) OUTPUT Inserted.ContactID VALUES " +
                "(@CustomerID, @Name, @Country, @City, @Zone, @Zip, @CountryID, @RegionID, @Street, @House, @Apartment, @Structure, @Entrance, @Floor, @District); ",
                CommandType.Text,

                new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@Name", contact.Name ?? string.Empty),
                new SqlParameter("@Country", contact.Country ?? string.Empty),
                new SqlParameter("@City", contact.City ?? string.Empty),
                new SqlParameter("@District", contact.District ?? string.Empty),
                new SqlParameter("@Zone", contact.Region ?? string.Empty),
                new SqlParameter("@Zip", contact.Zip ?? string.Empty),
                new SqlParameter("@CountryID", contact.CountryId != 0 ? contact.CountryId : (object)DBNull.Value),
                new SqlParameter("@RegionID", contact.RegionId.HasValue && contact.RegionId > 0 ? contact.RegionId : (object)DBNull.Value),

                new SqlParameter("@Street", contact.Street ?? string.Empty),
                new SqlParameter("@House", contact.House ?? string.Empty),
                new SqlParameter("@Apartment", contact.Apartment ?? string.Empty),
                new SqlParameter("@Structure", contact.Structure ?? string.Empty),
                new SqlParameter("@Entrance", contact.Entrance ?? string.Empty),
                new SqlParameter("@Floor", contact.Floor ?? string.Empty)
                );

            contact.CustomerGuid = customerId;
            contact.ContactId = SQLDataHelper.GetGuid(id);

            ModulesExecuter.AddContact(contact);

            CacheManager.RemoveByPattern(CacheNames.Customer + customerId);

            return contact.ContactId;
        }

        public static void UpdateContact(CustomerContact contact)
        {
            SQLDataAccess.ExecuteNonQuery(
                //"[Customers].[sp_UpdateCustomerContact]", CommandType.StoredProcedure,
                "Update [Customers].[Contact] Set " +
                "Name=@Name, Country=@Country, City=@City, Zone=@Zone, Zip=@Zip, CountryID=@CountryID, RegionID=@RegionID, " +
                "Street=@Street, House=@House, Apartment=@Apartment, Structure=@Structure, Entrance=@Entrance, Floor=@Floor, District=@District " +
                "WHERE ContactID = @ContactID ",
                CommandType.Text,

                new SqlParameter("@ContactID", contact.ContactId),
                new SqlParameter("@Name", contact.Name ?? string.Empty),
                new SqlParameter("@Country", contact.Country ?? string.Empty),
                new SqlParameter("@City", contact.City ?? string.Empty),
                new SqlParameter("@District", contact.District ?? string.Empty),
                new SqlParameter("@Zone", contact.Region ?? string.Empty),
                new SqlParameter("@Zip", contact.Zip ?? string.Empty),
                new SqlParameter("@CountryID", contact.CountryId != 0 ? contact.CountryId : (object)DBNull.Value),
                new SqlParameter("@RegionID", contact.RegionId.HasValue && contact.RegionId > 0 ? contact.RegionId : (object)DBNull.Value),

                new SqlParameter("@Street", contact.Street ?? string.Empty),
                new SqlParameter("@House", contact.House ?? string.Empty),
                new SqlParameter("@Apartment", contact.Apartment ?? string.Empty),
                new SqlParameter("@Structure", contact.Structure ?? string.Empty),
                new SqlParameter("@Entrance", contact.Entrance ?? string.Empty),
                new SqlParameter("@Floor", contact.Floor ?? string.Empty)
            );

            ModulesExecuter.UpdateContact(contact);

            CacheManager.RemoveByPattern(CacheNames.Customer + contact.CustomerGuid);
        }

        public static bool UpdateCustomer(Customer customer)
        {
            if (customer == null)
                return false;

            SQLDataAccess.ExecuteNonQuery("[Customers].[sp_UpdateCustomerInfo]", CommandType.StoredProcedure,
                new SqlParameter("@CustomerID", customer.Id),
                new SqlParameter("@FirstName", customer.FirstName ?? String.Empty),
                new SqlParameter("@LastName", customer.LastName ?? String.Empty),
                new SqlParameter("@Patronymic", customer.Patronymic ?? String.Empty),
                new SqlParameter("@Phone", customer.Phone ?? String.Empty),
                new SqlParameter("@StandardPhone", customer.StandardPhone ?? (object)DBNull.Value),
                new SqlParameter("@Email", customer.EMail ?? string.Empty),
                new SqlParameter("@CustomerGroupId", customer.CustomerGroupId == 0 ? (object)DBNull.Value : customer.CustomerGroupId),
                new SqlParameter("@CustomerRole", customer.CustomerRole),
                new SqlParameter("@BonusCardNumber", customer.BonusCardNumber ?? (object)DBNull.Value),
                new SqlParameter("@AdminComment", customer.AdminComment ?? (object)DBNull.Value),
                new SqlParameter("@ManagerId", customer.ManagerId ?? (object)DBNull.Value),
                new SqlParameter("@Rating", customer.Rating),
                new SqlParameter("@Avatar", customer.Avatar ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", customer.Enabled),
                new SqlParameter("@HeadCustomerId", customer.HeadCustomerId ?? (object)DBNull.Value),
                new SqlParameter("@BirthDay", customer.BirthDay ?? (object)DBNull.Value),
                new SqlParameter("@City", customer.City ?? (object)DBNull.Value),
                new SqlParameter("@SortOrder", customer.SortOrder),
                new SqlParameter("@Organization", customer.Organization ?? (object)DBNull.Value),
                new SqlParameter("@ClientStatus", (int)customer.ClientStatus)
                );

            if (customer.EMail.IsNotEmpty() &&
                SubscriptionService.IsSubscribe(customer.EMail) != customer.SubscribedForNews)
            {
                if (customer.SubscribedForNews)
                {
                    SubscriptionService.Subscribe(customer.EMail);
                }
                else
                {
                    SubscriptionService.Unsubscribe(customer.EMail);
                }
            }

            var tags = customer.Tags;
            TagService.DeleteMap(customer.Id);
            if (tags != null && tags.Count != 0)
            {
                for (var i = 0; i < tags.Count; i++)
                {
                    var tag = TagService.Get(tags[i].Name);
                    tags[i].Id = tag == null ? TagService.Add(tags[i]) : tag.Id;
                    TagService.AddMap(customer.Id, tags[i].Id, i * 10);
                }
            }

            ModulesExecuter.UpdateCustomer(customer);

            CacheManager.RemoveByPattern(CacheNames.Customer + customer.Id);

            return true;
        }

        public static void UpdateCustomerEmail(Guid id, string email)
        {
            SQLDataAccess.ExecuteNonQuery("Update Customers.Customer Set Email = @Email Where CustomerID = @CustomerID",
                CommandType.Text, new SqlParameter("@CustomerID", id), new SqlParameter("@Email", email));

            ModulesExecuter.UpdateCustomer(id);

            CacheManager.RemoveByPattern(CacheNames.Customer + id);
        }

        public static Customer GetCustomerByEmailAndPassword(string email, string password, bool isHash)
        {
            return SQLDataAccess.ExecuteReadOne("[Customers].[sp_GetCustomerByEmailAndPassword]",
                CommandType.StoredProcedure, GetFromSqlDataReader,
                new SqlParameter("@Email", email),
                new SqlParameter("@Password", isHash ? password : SecurityHelper.GetPasswordHash(password)));
        }

        public static string ConvertToLinedAddress(CustomerContact cc)
        {
            var address = string.Empty;

            if (!string.IsNullOrWhiteSpace(cc.Country))
            {
                address += cc.Country + ", ";
            }

            if (!string.IsNullOrWhiteSpace(cc.Region) && cc.Region.Trim() != "-")
            {
                address += cc.Region + ", ";
            }

            if (!string.IsNullOrWhiteSpace(cc.City))
            {
                if (!string.IsNullOrWhiteSpace(cc.District))
                {
                    address += cc.District + ", ";
                }
                address += cc.City + ", ";
            }

            if (!string.IsNullOrWhiteSpace(cc.Zip) && cc.Zip.Trim() != "-")
            {
                address += cc.Zip + ", ";
            }

            if (!string.IsNullOrWhiteSpace(cc.Street))
            {
                address += 
                    LocalizationService.GetResource("Core.Orders.OrderContact.Street") + " " + cc.Street +
                    (!string.IsNullOrWhiteSpace(cc.House)
                        ? ", " + LocalizationService.GetResource("Admin.Js.CustomerView.House") + " " + cc.House
                        : "") +
                    (!string.IsNullOrWhiteSpace(cc.Structure)
                        ? ", " + LocalizationService.GetResource("Admin.Js.CustomerView.Struct") + " " + cc.Structure
                        : "") +
                    (!string.IsNullOrWhiteSpace(cc.Apartment)
                        ? ", " + LocalizationService.GetResource("Admin.Js.CustomerView.Ap") + " " + cc.Apartment
                        : "") +
                    (!string.IsNullOrWhiteSpace(cc.Entrance)
                        ? ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Entrance") + " " + cc.Entrance
                        : "") +
                    (!string.IsNullOrWhiteSpace(cc.Floor)
                        ? ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Floor") + " " + cc.Floor
                        : "");
            }

            return address;
        }

        public static bool ExistsEmail(string strUserEmail)
        {
            if (String.IsNullOrEmpty(strUserEmail))
            {
                return false;
            }

            bool boolRes =
                SQLDataAccess.ExecuteScalar<int>(
                    "SELECT COUNT(CustomerID) FROM [Customers].[Customer] WHERE [Email] = @Email;", CommandType.Text,
                    new SqlParameter("@Email", strUserEmail)) > 0;

            return boolRes;
        }

        public static void ChangePassword(Guid customerId, string strNewPassword, bool isPassHashed)
        {
            SQLDataAccess.ExecuteNonQuery("[Customers].[sp_ChangePassword]", CommandType.StoredProcedure,
                new SqlParameter { ParameterName = "@CustomerID", Value = customerId },
                new SqlParameter
                {
                    ParameterName = "@Password",
                    Value = isPassHashed ? strNewPassword : SecurityHelper.GetPasswordHash(strNewPassword)
                }
            );

            CacheManager.RemoveByPattern(CacheNames.Customer + customerId);

            if (CustomerContext.CustomerId == customerId)
            {
                Security.AuthorizeService.SignIn(CustomerContext.CurrentCustomer.EMail, strNewPassword, false, true);
            }

        }

        public static bool IsNewCustomerValid(Customer customer)
        {
            if (!string.IsNullOrEmpty(customer.EMail) && IsEmailExist(customer.EMail))
                return false;

            if ((!string.IsNullOrEmpty(customer.Phone) || (customer.StandardPhone != null && customer.StandardPhone != 0)) && IsPhoneExist(customer.Phone, customer.StandardPhone))
                return false;

            return true;
        }

        public static Guid InsertNewCustomer(Customer customer, List<CustomerFieldWithValue> customerFields = null)
        {
            if (!IsNewCustomerValid(customer))
                return Guid.Empty;

            var regFromUri = HttpContext.Current != null && HttpContext.Current.Request != null &&
                             CustomerContext.CurrentCustomer != null && !CustomerContext.CurrentCustomer.IsAdmin && !CustomerContext.CurrentCustomer.IsModerator
                ? HttpContext.Current.Request.GetUrlReferrer() ?? HttpContext.Current.Request.Url
                : null;

            var temp =
                SQLDataAccess.ExecuteReadOne("[Customers].[sp_AddCustomer]",
                    CommandType.StoredProcedure,
                    reader => new KeyValuePair<Guid, int>(SQLDataHelper.GetGuid(reader, "CustomerID"), SQLDataHelper.GetInt(reader, "InnerId")),

                    new SqlParameter("@CustomerID", customer.Id != Guid.Empty ? customer.Id : (object)DBNull.Value),
                    new SqlParameter("@CustomerGroupID", customer.CustomerGroupId),
                    new SqlParameter("@Password", SecurityHelper.GetPasswordHash(customer.Password)),
                    new SqlParameter("@FirstName", customer.FirstName ?? String.Empty),
                    new SqlParameter("@LastName", customer.LastName ?? String.Empty),
                    new SqlParameter("@Patronymic", customer.Patronymic ?? String.Empty),
                    new SqlParameter("@Phone", String.IsNullOrEmpty(customer.Phone) ? (object)DBNull.Value : customer.Phone),
                    new SqlParameter("@StandardPhone", customer.StandardPhone ?? (object)DBNull.Value),
                    new SqlParameter("@RegistrationDateTime", DateTime.Now),
                    new SqlParameter("@Email", customer.EMail ?? string.Empty),
                    new SqlParameter("@CustomerRole", customer.CustomerRole),
                    new SqlParameter("@BonusCardNumber", customer.BonusCardNumber ?? (object)DBNull.Value),
                    new SqlParameter("@AdminComment", customer.AdminComment ?? (object)DBNull.Value),
                    new SqlParameter("@ManagerId", customer.ManagerId ?? (object)DBNull.Value),
                    new SqlParameter("@Rating", customer.Rating),
                    new SqlParameter("@Enabled", customer.Enabled),
                    new SqlParameter("@HeadCustomerId", customer.HeadCustomerId ?? (object)DBNull.Value),
                    new SqlParameter("@BirthDay", customer.BirthDay ?? (object)DBNull.Value),
                    new SqlParameter("@City", customer.City ?? (object)DBNull.Value),
                    new SqlParameter("@Organization", customer.Organization ?? (object)DBNull.Value),
                    new SqlParameter("@ClientStatus", (int)customer.ClientStatus),
                    new SqlParameter("@RegisteredFrom", regFromUri != null ? regFromUri.GetLeftPart(UriPartial.Path).Reduce(500) : (object)DBNull.Value)
                );

            if (customer.SubscribedForNews)
                SubscriptionService.Subscribe(customer.EMail);

            customer.Id = temp.Key;
            customer.InnerId = temp.Value;

            var tags = customer.Tags;

            if (tags != null && tags.Count != 0)
            {
                for (var i = 0; i < tags.Count; i++)
                {
                    var tag = TagService.Get(tags[i].Name);
                    tags[i].Id = tag == null ? TagService.Add(tags[i]) : tag.Id;
                    TagService.AddMap(customer.Id, tags[i].Id, i * 10);
                }
            }

            if (customerFields != null && customerFields.Count > 0)
            {
                foreach (var customerField in customerFields)
                {
                    CustomerFieldService.AddUpdateMap(customer.Id, customerField.Id, customerField.Value ?? "", true);
                }
            }

            ModulesExecuter.AddCustomer(customer);
            TriggerProcessService.ProcessEvent(ETriggerEventType.CustomerCreated, customer);
            PartnerService.BindNewCustomer(customer);

            CacheManager.RemoveByPattern(CacheNames.Customer);

            return customer.Id;
        }

        public static string GetContactId(CustomerContact contact)
        {
            var res =
                SQLDataHelper.GetNullableGuid(SQLDataAccess.ExecuteScalar("[Customers].[sp_GetContactIDByContent]",
                    CommandType.StoredProcedure,
                    new SqlParameter("@Name", contact.Name),
                    new SqlParameter("@Country", contact.Country),
                    new SqlParameter("@City", contact.City),
                    new SqlParameter("@Zone", contact.Region ?? ""),
                    new SqlParameter("@Zip", contact.Zip ?? ""),
                    new SqlParameter("@Street", contact.Street ?? ""),
                    new SqlParameter("@CustomerID", contact.CustomerGuid)
                    ));
            return res == null ? null : res.ToString();
        }

        public static bool IsEmailExist(string email)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT([CustomerID]) FROM [Customers].[Customer] WHERE [Email] = @Email",
                CommandType.Text, new SqlParameter("@Email", email)) != 0;
        }

        public static bool IsPhoneExist(string phone, long? standardPhone)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "Select Count(CustomerId) " +
                    "From Customers.Customer " +
                    "Where Phone=@Phone " + (standardPhone != null && standardPhone != 0 ? "or StandardPhone=@StandardPhone" : string.Empty),
                    CommandType.Text,
                    new SqlParameter("@Phone", phone),
                    new SqlParameter("@StandardPhone", standardPhone ?? (object) DBNull.Value)) != 0;
        }

        public static bool AddOpenIdLinkCustomer(Guid customerGuid, string identifier)
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar(
                "Insert Into [Customers].[OpenIdLinkCustomer] (CustomerID, OpenIdIdentifier) Values (@CustomerID, @OpenIdIdentifier)",
                CommandType.Text,
                new SqlParameter("@CustomerID", customerGuid),
                new SqlParameter("@OpenIdIdentifier", identifier))) != 0;
        }

        public static bool IsExistOpenIdLinkCustomer(string identifier)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT([CustomerID]) FROM [Customers].[OpenIdLinkCustomer] WHERE [OpenIdIdentifier] = @OpenIdIdentifier",
                CommandType.Text,
                new SqlParameter("@OpenIdIdentifier", identifier)) != 0;
        }

        public static void ChangeCustomerGroup(Guid customerId, int customerGroupId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[Customer] Set CustomerGroupId = @CustomerGroupId WHERE CustomerID = @CustomerID",
                CommandType.Text, new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@CustomerGroupId", customerGroupId));

            ModulesExecuter.UpdateCustomer(customerId);

            CacheManager.RemoveByPattern(CacheNames.Customer + customerId);
        }

        public static void UpdateAdminComment(Guid customerId, string comment)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[Customer] Set AdminComment = @AdminComment WHERE CustomerID = @CustomerID",
                CommandType.Text,
                new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@AdminComment", comment));

            ModulesExecuter.UpdateCustomer(customerId);

            CacheManager.RemoveByPattern(CacheNames.Customer + customerId);
        }

        public static void UpdateCustomerRating(Guid customerId, int rating)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[Customer] Set [Rating] = @Rating WHERE CustomerID = @CustomerID",
                CommandType.Text,
                new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@Rating", rating));

            ModulesExecuter.UpdateCustomer(customerId);

            CacheManager.RemoveByPattern(CacheNames.Customer + customerId);
        }

        public static void ChangeCustomerManager(Guid customerId, int? managerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[Customer] Set ManagerId = @ManagerId WHERE CustomerID = @CustomerID",
                CommandType.Text,
                new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@ManagerId", managerId ?? (object)DBNull.Value));

            ModulesExecuter.UpdateCustomer(customerId);

            CacheManager.RemoveByPattern(CacheNames.Customer + customerId);
        }

        public static string GetCurrentCustomerManager()
        {
            return SQLDataAccess.ExecuteScalar<string>(
                "Select [Customer].FirstName + ' ' + [Customer].[LastName] From [Customers].[Customer] LEFT JOIN [Customers].[Managers] On [Customer].ManagerId = [Managers].ManagerId WHERE [Customer].CustomerID = @CustomerID",
                CommandType.Text,
                new SqlParameter("@CustomerID", CustomerContext.CurrentCustomer.Id));
        }

        public static bool CanDelete(Guid customerId)
        {
            List<string> messages;
            return CanDelete(customerId, out messages);
        }

        public static bool CanDelete(Guid customerId, out List<string> messages)
        {
            messages = new List<string>();
            var currentCustomer = CustomerContext.CurrentCustomer;
            if (currentCustomer == null)
                return false;
            var customer = GetCustomer(customerId);
            if (customer == null)
            {
                messages.Add(LocalizationService.GetResource("Core.Customers.ErrorDeleteCustomer.NotFound"));
                return false;
            }

            if (customer.Id == currentCustomer.Id)
                messages.Add(LocalizationService.GetResource("Core.Customers.ErrorDeleteCustomer.SelfDelete"));
            if (customer.IsAdmin && currentCustomer.IsModerator)
                messages.Add(LocalizationService.GetResource("Core.Customers.ErrorDeleteCustomer.IsAdmin"));

            Manager manager;
            if (customer.IsManager && (manager = ManagerService.GetManager(customer.Id)) != null)
            {
                string managerMessage;
                if (!ManagerService.CanDelete(manager.ManagerId, out managerMessage))
                    messages.Add(managerMessage);
            }

            return !messages.Any();
        }

        /// <summary>
        /// Get sended by site emails
        /// </summary>
        public static List<EmailLogItem> GetEmails(Guid customerId, string email)
        {
            try
            {
                var emails = MailService.GetLast(customerId) ?? new List<EmailLogItem>();

                if (!string.IsNullOrEmpty(email))
                {
                    var emailsFromService = EmailLoger.GetEmails(customerId, email);

                    if (emailsFromService != null && emailsFromService.Count > 0)
                        emails.AddRange(emailsFromService);
                }
                return emails;
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
            return null;
        }

        /// <summary>
        /// Get emails from imap host
        /// </summary>
        public static List<EmailImap> GetEmails(string email)
        {
            return CacheManager.Get("GetEmailsByImap" + email, 0.3, () => new ImapMailService().GetEmails(email));
        }

        public static EmailImap GetEmailImap(string uid, string folder)
        {
            var imapService = new ImapMailService();
            return imapService.GetEmail(uid, folder);
        }


        public static List<TextMessage> GetSms(Guid customerId, long phone)
        {
            return SmsLoger.GetSms(customerId, phone);
        }

        public static List<Event> GetEvent(Guid customerId)
        {
            return EventLoger.GetEvents(customerId);
        }

        public static List<Call> GetCalls(Guid customerId, string phone)
        {
            return CallLoger.GetCalls(customerId, phone);
        }

        public static bool CheckAccess(Customer customer)
        {
            var currentCustomer = CustomerContext.CurrentCustomer;

            if (currentCustomer.IsAdmin || currentCustomer.IsVirtual)
                return true;

            if (CustomerContext.CurrentCustomer.IsModerator)
            {
                var manager = ManagerService.GetManager(CustomerContext.CurrentCustomer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersCustomerConstraint == ManagersCustomerConstraint.Assigned &&
                        customer.ManagerId != manager.ManagerId)
                        return false;

                    if (SettingsManager.ManagersCustomerConstraint == ManagersCustomerConstraint.AssignedAndFree &&
                        customer.ManagerId != manager.ManagerId && customer.ManagerId != null)
                        return false;

                    return true;
                }
            }
            return false;
        }

        public static List<Customer> GetCustomersByDaysFromLastOrder(int days)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT c.* FROM [Customers].[Customer] as c " +
                "Where DATEDIFF(day, " +
                    "(Select top(1) [OrderDate] From [Order].[Order] " +
                        "Inner Join [Order].[OrderCustomer] On [OrderCustomer].[OrderId] = [Order].[OrderId] " +
                        "Where [OrderCustomer].[CustomerId] = c.CustomerId " +
                        "Order by [OrderDate] desc), " +
                    "getdate() ) = @days",
                CommandType.Text,
                GetFromSqlDataReader,
                new SqlParameter("@days", days));
        }

        public static List<Customer> GetCustomersByTriggersDateParams(DateTime date, bool ignoreYear, bool isBirthDay, int? customFieldId)
        {
            var sql = "";

            if (isBirthDay)
            {
                sql = ignoreYear
                    ? "SELECT * FROM [Customers].[Customer] Where BirthDay is not null and MONTH(BirthDay)=@dateMonth and DAY(BirthDay)=@dateDay"
                    : "SELECT * FROM [Customers].[Customer] Where BirthDay is not null and MONTH(BirthDay)=@dateMonth and DAY(BirthDay)=@dateDay and YEAR(BirthDay)=@dateYear";
            }
            else
            {
                sql = ignoreYear
                    ? "SELECT c.* FROM [Customers].[Customer] as c " +
                      "Inner Join [Customers].[CustomerFieldValuesMap] vm On c.CustomerId = vm.CustomerId " +
                      "Where vm.CustomerFieldId = @CustomFieldId and vm.[Value] is not null and ISDATE(vm.[Value]) = 1 and " +
                      "MONTH(convert(varchar,vm.[Value],120))=@dateMonth and DAY(convert(varchar,vm.[Value],120))=@dateDay"

                    : "SELECT c.* FROM [Customers].[Customer] as c " +
                      "Inner Join [Customers].[CustomerFieldValuesMap] vm On c.CustomerId = vm.CustomerId " +
                      "Where vm.CustomerFieldId = @CustomFieldId and vm.[Value] is not null and ISDATE(vm.[Value]) = 1 and " +
                      "MONTH(convert(varchar,vm.[Value],120))=@dateMonth and DAY(convert(varchar,vm.[Value],120))=@dateDay and YEAR(convert(varchar,vm.[Value],120))=@dateYear";
            }

            return SQLDataAccess.ExecuteReadList(sql, CommandType.Text, GetFromSqlDataReader,
                new SqlParameter("@dateMonth", date.Month),
                new SqlParameter("@dateDay", date.Day),
                new SqlParameter("@dateYear", date.Year),
                new SqlParameter("@CustomFieldId", customFieldId));
        }

        public static int GetCustomersCountByOrderDate(DateTime orderDateFrom, DateTime orderDateTo)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(DISTINCT [Customer].[CustomerID]) FROM [Customers].[Customer] " +
                "INNER JOIN [Order].[OrderCustomer] ON [OrderCustomer].[CustomerID] = [Customer].[CustomerID] " +
                "INNER JOIN [Order].[Order] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                "WHERE [Order].[OrderDate] >= @orderDateFrom AND [Order].[OrderDate] <=@orderDateTo",
                CommandType.Text,
                new SqlParameter("@orderDateFrom", orderDateFrom),
                new SqlParameter("@orderDateTo", orderDateTo));
        }

        public static bool IsTechDomainGuest()
        {
            var cookie = CommonHelper.GetCookie(GuestCookieName);
            return cookie != null && cookie.Value != null;
        }

        public static void SetTechDomainGuest()
        {
            CommonHelper.SetCookie(GuestCookieName, "true", true, true, true);
        }
    }
}