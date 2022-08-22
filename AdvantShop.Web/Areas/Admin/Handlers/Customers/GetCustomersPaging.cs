using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Customers;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Handlers.Customers
{
    public class GetCustomersPaging
    {
        private readonly CustomersFilterModel _filterModel;
        private SqlPaging _paging;

        private bool _exportToCsv;

        public GetCustomersPaging(CustomersFilterModel filterModel, bool exportToCsv = false)
        {
            _filterModel = filterModel;

            _exportToCsv = exportToCsv;
        }

        public FilterResult<AdminCustomerModel> Execute()
        {
            var model = new FilterResult<AdminCustomerModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<AdminCustomerModel>();
            
            return model;
        }

        public List<Guid> GetItemsIds(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<Guid>(fieldName);
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "[Customer].CustomerID",
                "[Customer].Email".AsSqlField("Email"),
                "[Customer].Phone".AsSqlField("Phone"),
                "[Customer].ManagerId",
                "[Customer].Rating",

                "(Select [Customer].Lastname + ' ' + [Customer].Firstname + ' ' + ISNULL([Customer].Patronymic,''))".AsSqlField("Name"),
                "[Customer].Organization",

                ("(Select Top(1) [Order].Number From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                 "WHERE [CustomerId] = [Customer].[CustomerId] Order by [OrderDate] Desc)").AsSqlField("LastOrderNumber"),

                ("(Select Top(1) [Order].OrderId From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                 "WHERE [CustomerId] = [Customer].[CustomerId] Order by [OrderDate] Desc)").AsSqlField("LastOrderId"),

                ("(Select ISNULL(SUM([Sum]),0) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                 "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null)")
                    .AsSqlField("OrdersSum"),

                ("(Select COUNT([Order].[OrderId]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                 "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null)")
                    .AsSqlField("OrdersCount"),

                "(Select Top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = [Customer].[CustomerID])"
                    .AsSqlField("Location"),

                "(Select [FirstName] + ' ' + [LastName] From  [Customers].[Customer]  WHERE [CustomerId] = [Managers].[CustomerId])"
                    .AsSqlField("ManagerName"),

                "[Customer].RegistrationDateTime".AsSqlField("RegistrationDateTime"),
                "[Customer].BirthDay".AsSqlField("BirthDay")
                );

            if (_exportToCsv)
            {
                _paging.Select(
                    "[Customer].Firstname".AsSqlField("FirstName"),
                    "[Customer].Lastname".AsSqlField("LastName"),
                    "[Customer].Patronymic",
                    "(Select Top(1) [Country] From [Customers].[Contact] Where [Contact].[CustomerID] = [Customer].[CustomerID])".AsSqlField("Country"),
                    "(Select Top(1) [Zone] From [Customers].[Contact] Where [Contact].[CustomerID] = [Customer].[CustomerID])".AsSqlField("Region"),
                    "(Select Top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = [Customer].[CustomerID])".AsSqlField("City"),
                    "[Card].CardNumber"
                    );
            }

            _paging.From("[Customers].[Customer]");
            _paging.Left_Join("[Customers].[Managers] ON [Customer].[ManagerId] = [Managers].[ManagerId]");
            _paging.Left_Join("[Bonus].[Card] ON [Card].[CardId] = [Customer].[CustomerID]");
            _paging.Where("[Customer].[CustomerRole] = {0}", _filterModel.Role.HasValue ? (int)_filterModel.Role.Value : (int)Role.User);

            Sorting();
            Filter();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                _paging.Where(
                    "([Customer].Email LIKE '%'+{0}+'%' OR [Customer].Firstname + ' ' + [Customer].Lastname LIKE '%'+{0}+'%' OR [Customer].Lastname + ' ' + [Customer].Firstname LIKE '%'+{0}+'%' OR [Customer].Phone LIKE '%'+{0}+'%' OR [Customer].Organization LIKE '%'+{0}+'%')",
                    _filterModel.Search);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.Name))
            {
                _paging.Where("([Customer].Lastname + ' ' + [Customer].Firstname LIKE '%'+{0}+'%' OR [Customer].Firstname + ' ' + [Customer].Lastname LIKE '%'+{0}+'%')", _filterModel.Name);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.Email))
            {
                _paging.Where("[Customer].Email LIKE '%'+{0}+'%'", _filterModel.Email);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.Phone))
            {
                long? standartPhone = StringHelper.ConvertToStandardPhone(_filterModel.Phone, true, true);
                _paging.Where("convert(nvarchar, StandardPhone) LIKE '%'+{0}+'%'", standartPhone != null ? standartPhone.ToString() : "null");
            }

            if (_filterModel.ManagerId.HasValue)
            {
                if (_filterModel.ManagerId.Value == -1)
                {
                    _paging.Where("[Customer].ManagerId IS NULL");
                }
                else
                {
                    _paging.Where("[Customer].ManagerId = {0}", _filterModel.ManagerId.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.LastOrderNumber))
            {
                _paging.Where(
                    "(Select Top(1) [Order].Number From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                    "WHERE [CustomerId] = [Customer].[CustomerId] Order by [OrderDate] Desc) LIKE '%'+{0}+'%'",
                    _filterModel.LastOrderNumber);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.Location))
            {
                _paging.Where(
                    "(Select Top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = [Customer].[CustomerID]) LIKE '%'+{0}+'%'",
                    _filterModel.Location);
            }

            if (_filterModel.OrdersCountFrom != null)
            {
                _paging.Where(
                    "(Select COUNT([Order].[OrderId]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) >= {0}", _filterModel.OrdersCountFrom);
            }

            if (_filterModel.OrdersCountTo != null)
            {
                _paging.Where(
                    "(Select COUNT([Order].[OrderId]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) <= {0}", _filterModel.OrdersCountTo);
            }

            if (_filterModel.OrderSumFrom != null)
            {
                _paging.Where(
                    "(Select SUM([Sum]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) >= {0}",
                    _filterModel.OrderSumFrom);
            }

            if (_filterModel.OrderSumTo != null)
            {
                _paging.Where(
                    "(Select SUM([Sum]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) <= {0}",
                    _filterModel.OrderSumTo);
            }

            if (_filterModel.LastOrderFrom != null)
            {
                _paging.Where("LastOrderId >= {0}", _filterModel.LastOrderFrom);
            }

            if (_filterModel.LastOrderTo != null)
            {
                _paging.Where("LastOrderId <= {0}", _filterModel.LastOrderTo);
            }

            DateTime from, to;

            if (!string.IsNullOrWhiteSpace(_filterModel.RegistrationDateTimeFrom) && DateTime.TryParse(_filterModel.RegistrationDateTimeFrom, out from))
            {
                _paging.Where("RegistrationDateTime >= {0}", from);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.RegistrationDateTimeTo) && DateTime.TryParse(_filterModel.RegistrationDateTimeTo, out to))
            {
                _paging.Where("RegistrationDateTime <= {0}", to);
            }

            if (_filterModel.GroupId != 0)
            {
                _paging.Where("[Customer].CustomerGroupId = {0}", _filterModel.GroupId);
            }

            DateTime fromLastOrder, toLastOrder;

            if (!string.IsNullOrWhiteSpace(_filterModel.LastOrderDateTimeFrom) && DateTime.TryParse(_filterModel.LastOrderDateTimeFrom, out fromLastOrder))
            {
                _paging.Where(
                    "(Select top(1) OrderDate " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] " +
                    "Order by [OrderDate] Desc) >= {0}",
                    fromLastOrder);
            }
            if (!string.IsNullOrWhiteSpace(_filterModel.LastOrderDateTimeTo) && DateTime.TryParse(_filterModel.LastOrderDateTimeTo, out toLastOrder))
            {
                _paging.Where(
                    "(Select top(1) OrderDate " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] " +
                    "Order by [OrderDate] Desc) <= {0}",
                    toLastOrder);
            }

            if (_filterModel.AverageCheckFrom != null)
            {
                _paging.Where(
                    "(Select SUM([Sum])/Count([Order].OrderId) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) >= {0}",
                    _filterModel.AverageCheckFrom);
            }
            if (_filterModel.AverageCheckTo != null)
            {
                _paging.Where(
                    "(Select SUM([Sum])/Count([Order].OrderId) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) <= {0}",
                    _filterModel.AverageCheckTo.Value);
            }

            if (_filterModel.SocialType != null)
            {
                switch (_filterModel.SocialType)
                {
                    case "all":
                        _paging.Where(
                            "(Exists(Select 1 From [Customers].[VkUser] Where VkUser.CustomerId = [Customer].[CustomerId]) OR " +
                            "Exists(Select 1 From [Customers].[FacebookUser] Where FacebookUser.CustomerId = [Customer].[CustomerId]) OR " +
                            "Exists(Select 1 From [Customers].[TelegramUser] Where TelegramUser.CustomerId = [Customer].[CustomerId]) OR " +
                            "Exists(Select 1 From [Customers].[InstagramUser] Where InstagramUser.CustomerId = [Customer].[CustomerId]))");
                        break;
                    case "vk":
                        _paging.Where("Exists(Select 1 From [Customers].[VkUser] Where VkUser.CustomerId = [Customer].[CustomerId])");
                        break;
                    case "fb":
                        _paging.Where("Exists(Select 1 From [Customers].[FacebookUser] Where FacebookUser.CustomerId = [Customer].[CustomerId])");
                        break;
                    case "instagram":
                        _paging.Where("Exists(Select 1 From [Customers].[InstagramUser] Where InstagramUser.CustomerId = [Customer].[CustomerId])");
                        break;
                    case "telegram":
                        _paging.Where("Exists(Select 1 From [Customers].[TelegramUser] Where TelegramUser.CustomerId = [Customer].[CustomerId])");
                        break;
                }
            }

            if (_filterModel.CustomerFields != null)
            {
                foreach (var fieldFilter in _filterModel.CustomerFields.Where(x => x.Value != null))
                {
                    var fieldsFilterModel = fieldFilter.Value;
                    if (fieldsFilterModel.DateFrom.HasValue)
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                   "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateFrom.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.DateTo.HasValue)
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateTo.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.From.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.From.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.To.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.To.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.ValueExact.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value = {1}))",
                            fieldFilter.Key, fieldsFilterModel.ValueExact);
                    }

                    if (fieldsFilterModel.Value.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value like '%' + {1} + '%'))",
                            fieldFilter.Key, fieldsFilterModel.Value);
                    }
                }
            }

            if (_filterModel.Subscription != null)
            {
                _paging.Left_Join("[Customers].[Subscription] On [Customer].Email = [Subscription].[Email]");
                _paging.Where("[Subscription].[Subscribe] = {0}", _filterModel.Subscription.Value);
            }

            if (_filterModel.HasBonusCard.HasValue)
                _paging.Where("[Card].CardNumber IS " + (_filterModel.HasBonusCard.Value ? "NOT NULL" : "NULL"));

            var customer = CustomerContext.CurrentCustomer;
            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersCustomerConstraint == ManagersCustomerConstraint.Assigned)
                    {
                        _paging.Where("[Customer].ManagerId = {0}", manager.ManagerId);
                    }
                    else if (SettingsManager.ManagersCustomerConstraint == ManagersCustomerConstraint.AssignedAndFree)
                    {
                        _paging.Where("([Customer].ManagerId = {0} or [Customer].ManagerId is null)", manager.ManagerId);
                    }
                }
            }

            if(_filterModel.Tags != null && _filterModel.Tags.Count > 0)
            {
                _paging.Where("(SELECT COUNT(*) FROM [Customers].[TagMap] WHERE [TagMap].[CustomerId] = [Customer].CustomerID AND [TagMap].[TagId] in (" + 
                    string.Join(",", _filterModel.Tags.Select(x => x.ToString()).ToArray()) + ")) >= " + _filterModel.Tags.Count.ToString());
            }

            if (_filterModel.CustomerSegment.HasValue)
            {
                _paging.Left_Join("[Customers].[CustomerSegment_Customer] On [Customer].CustomerID = [CustomerSegment_Customer].[CustomerId]");
                _paging.Where("[CustomerSegment_Customer].[SegmentId] = {0}", _filterModel.CustomerSegment.Value);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("RegistrationDateTime");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower().Replace("formatted", "");

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sorting);
                }
                else
                {
                    _paging.OrderByDesc(sorting);
                }
            }
        }
    }
}