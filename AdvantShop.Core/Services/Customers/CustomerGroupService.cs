//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Customers
{
    public class CustomerGroupService
    {
        private const int DefaultCustomerGroupId = 1;

        private static int _defaultCustomerGroup;
        public static int DefaultCustomerGroup
        {
            get
            {
                if (_defaultCustomerGroup != 0)
                    return _defaultCustomerGroup;

                var list = GetCustomerGroupListIds();

                _defaultCustomerGroup = list.Contains(DefaultCustomerGroupId) || list.Count == 0
                    ? DefaultCustomerGroupId
                    : list[0];

                return _defaultCustomerGroup;
            }
        }

        private const string CustomerGroupCacheKey = "CustomerGroup_";

        public static CustomerGroup GetDefaultCustomerGroup()
        {
            return GetCustomerGroup(DefaultCustomerGroup);
        }

        public static CustomerGroup GetCustomerGroup(int customerGroupId = 1)
        {
            return CacheManager.Get(CustomerGroupCacheKey + customerGroupId, 20,
                () =>
                    SQLDataAccess.ExecuteReadOne(
                        "SELECT * FROM [Customers].[CustomerGroup] WHERE CustomerGroupId = @CustomerGroupId",
                        CommandType.Text, GetCustomerGroupFromReader,
                        new SqlParameter("@CustomerGroupId", customerGroupId)));
        }

        public static CustomerGroup GetCustomerGroup(string groupName)
        {
            return SQLDataAccess.ExecuteReadOne(
                        "SELECT * FROM [Customers].[CustomerGroup] WHERE GroupName = @GroupName",
                        CommandType.Text, GetCustomerGroupFromReader,
                        new SqlParameter("@GroupName", groupName.Trim()));
        }

        public static List<CustomerGroup> GetCustomerGroupList()
        {
            return SQLDataAccess.ExecuteReadList<CustomerGroup>("SELECT * FROM [Customers].[CustomerGroup] order by GroupDiscount", CommandType.Text, GetCustomerGroupFromReader);
        }

        public static List<int> GetCustomerGroupListIds()
        {
            return CacheManager.Get(CustomerGroupCacheKey + "List", 20,
                () =>
                    SQLDataAccess.ExecuteReadList(
                        "SELECT [CustomerGroupId] FROM [Customers].[CustomerGroup] order by GroupDiscount",
                        CommandType.Text, reader => SQLDataHelper.GetInt(reader, "CustomerGroupId")));
        }


        public static float GetMinimumOrderPrice(int? customerGroupId = null)
        {
            var price = GetMinimumOrderPriceByGroup(customerGroupId);
            return price == 0 ? 0 : price.RoundPrice();
        }

        public static float GetMinimumOrderPriceByGroup(int? customerGroupId = null)
        {
            var groupId = customerGroupId ?? (CustomerContext.CurrentCustomer != null ? CustomerContext.CurrentCustomer.CustomerGroupId : DefaultCustomerGroup);
            var group = GetCustomerGroup(groupId);

            return group != null ? group.MinimumOrderPrice : 0;
        }


        private static CustomerGroup GetCustomerGroupFromReader(SqlDataReader reader)
        {
            return new CustomerGroup
            {
                CustomerGroupId = SQLDataHelper.GetInt(reader, "CustomerGroupId"),
                GroupName = SQLDataHelper.GetString(reader, "GroupName"),
                GroupDiscount = SQLDataHelper.GetFloat(reader, "GroupDiscount"),
                MinimumOrderPrice = SQLDataHelper.GetFloat(reader, "MinimumOrderPrice")
            };
        }

        public static void AddCustomerGroup(CustomerGroup customerGroup)
        {
            customerGroup.CustomerGroupId = SQLDataAccess.ExecuteScalar<int>("INSERT INTO [Customers].[CustomerGroup] ([GroupName], [GroupDiscount],MinimumOrderPrice) VALUES (@GroupName, @GroupDiscount, @MinimumOrderPrice); SELECT SCOPE_IdENTITY();",
                                                                                CommandType.Text,
                                                                                new SqlParameter("@GroupName", customerGroup.GroupName),
                                                                                new SqlParameter("@GroupDiscount", customerGroup.GroupDiscount),
                                                                                new SqlParameter("@MinimumOrderPrice", customerGroup.MinimumOrderPrice)
                                                                                );
            CacheManager.RemoveByPattern(CustomerGroupCacheKey);
        }

        public static void UpdateCustomerGroup(CustomerGroup customerGroup)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Customers].[CustomerGroup] SET [GroupName] = @GroupName, [GroupDiscount] = @GroupDiscount, MinimumOrderPrice=@MinimumOrderPrice " +
                " WHERE CustomerGroupId = @CustomerGroupId", CommandType.Text,
                new SqlParameter("@CustomerGroupId", customerGroup.CustomerGroupId),
                new SqlParameter("@GroupName", customerGroup.GroupName),
                new SqlParameter("@GroupDiscount", customerGroup.GroupDiscount),
                new SqlParameter("@MinimumOrderPrice", customerGroup.MinimumOrderPrice));
            CacheManager.RemoveByPattern(CustomerGroupCacheKey);
        }

        public static void DeleteCustomerGroup(int customerGroupId)
        {
            if (customerGroupId == DefaultCustomerGroup)
                return;
            SQLDataAccess.ExecuteNonQuery("UPDATE [Customers].[Customer] set CustomerGroupId = @NewCustomerGroupId Where CustomerGroupId=@OldCustomerGroupId",
                                            CommandType.Text,
                                            new SqlParameter("@OldCustomerGroupId", customerGroupId),
                                            new SqlParameter("@NewCustomerGroupId", DefaultCustomerGroup));

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Customers].[CustomerGroup] WHERE CustomerGroupId = @CustomerGroupId",
                                            CommandType.Text, new SqlParameter("@CustomerGroupId", customerGroupId));

            CacheManager.RemoveByPattern(CustomerGroupCacheKey);
        }

        public static List<CustomerGroup> GetCustomerGroupsWithCount(DateTime from, DateTime to)
        {
            return
                    SQLDataAccess.ExecuteReadList<CustomerGroup>(
                        "SELECT *, (Select Count(CustomerID) from [Customers].[Customer] where Customer.CustomerGroupId = [CustomerGroup].CustomerGroupID and RegistrationDateTime >= @from and RegistrationDateTime <= @to) as CustomersCount  FROM [Customers].[CustomerGroup] order by CustomersCount Desc",
                        CommandType.Text,
                        reader =>new CustomerGroup
                        {
                            CustomerGroupId = SQLDataHelper.GetInt(reader, "CustomerGroupId"),
                            GroupName = SQLDataHelper.GetString(reader, "GroupName"),
                            GroupDiscount = SQLDataHelper.GetFloat(reader, "GroupDiscount"),
                            MinimumOrderPrice = SQLDataHelper.GetFloat(reader, "MinimumOrderPrice"),
                            CustomersCount = SQLDataHelper.GetInt(reader, "CustomersCount")
                        },
                        new SqlParameter("@from", from),
                        new SqlParameter("@to", to));
        }
    }
}