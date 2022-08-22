using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Admin;
using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Crm.Leads;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class GetLeads
    {
        private LeadsFilterModel _filter;
        private readonly bool _exportToCsv;
        private SqlPaging _paging;

        public GetLeads(LeadsFilterModel filter)
        {
            _filter = filter;
        }

        public GetLeads(LeadsFilterModel filterModel, bool exportToCsv) : this(filterModel)
        {
            _exportToCsv = exportToCsv;
        }

        public LeadsFilterResult Execute()
        {
            var model = new LeadsFilterResult();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();

            var dealStatuses = DealStatusService.GetListWithCount(_filter.SalesFunnelId);
            foreach (var dealStatus in dealStatuses)
            {
                model.LeadsCount.Add(dealStatus.Id, dealStatus.LeadsCount);
            }

            if (model.TotalPageCount < _filter.Page && _filter.Page > 1)
                return model;
            
            model.DataItems = _paging.PageItemsList<LeadsFilterResultModel>();

            model.TotalString += " " +
                                 LocalizationService.GetResourceFormat("Admin.Leads.Grid.TotalPrice",
                                     PriceFormatService.FormatPrice(
                                         _paging.GetCustomData(
                                                 "Sum ([Lead].[Sum]*ISNULL([LeadCurrency].[CurrencyValue],1)) as totalPrice",
                                                 "", reader => Helpers.SQLDataHelper.GetFloat(reader, "totalPrice"),
                                                 true)
                                             .FirstOrDefault(),
                                         SettingsCatalog.DefaultCurrency));
            
            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("[Lead].Id");
        }

        public List<Guid?> GetCustomerIds()
        {
            GetPaging();

            return _paging.ItemsIds<Guid?>("Distinct [LeadCustomer].CustomerId").Where(x => x != null).ToList();
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filter.ItemsPerPage,
                CurrentPageIndex = _filter.Page
            };

            _paging.Select(
                "[Lead].Id",

                "[Lead].FirstName",
                "[Lead].LastName",
                "[Lead].Patronymic",

                "[Lead].Phone",
                "[Lead].Email",

                "[Lead].CustomerId",
                "[LeadCustomer].FirstName as CustomerFirstName",
                "[LeadCustomer].LastName as CustomerLastName",
                "[LeadCustomer].Patronymic as CustomerPatronymic",
                "[LeadCustomer].Organization",

                "(isNull([Lead].LastName, '') + isNull([Lead].FirstName, '') + isNull([Lead].Patronymic, ''))".AsSqlField("FullName"),
                "(isNull([LeadCustomer].LastName, '') + isNull([LeadCustomer].FirstName, '') + isNull([LeadCustomer].Patronymic, ''))".AsSqlField("CustomerFullName"),

                "[LeadCustomer].Phone as CustomerPhone",
                "[LeadCustomer].Email as CustomerEmail",

                "[Lead].SalesFunnelId",
                "[Lead].Sum",
                "isNull((Select Sum(Price*Amount) From [Order].[LeadItem] as items Where items.LeadId = [Lead].[Id]), 0)".AsSqlField("ProductsSum"),
                "isNull((Select Sum(Amount) From [Order].[LeadItem] as items Where items.LeadId = [Lead].[Id]), 0)".AsSqlField("ProductsCount"),

                "[LeadCurrency].CurrencyValue",
                "[LeadCurrency].CurrencyCode",
                "[LeadCurrency].CurrencySymbol",
                "[LeadCurrency].IsCodeBefore",

                "[Lead].ManagerId",
                "[ManagerCustomer].FirstName + ' ' + [ManagerCustomer].LastName".AsSqlField("ManagerName"),
                "[Lead].CreatedDate",
                "[DealStatus].[Name]".AsSqlField("DealStatusName")
                );

            if (_exportToCsv)
            {
                _paging.Select(
                    "[Lead].[Title]",
                    "[Lead].[Description]",
                    "(Select Top(1) [Country] From [Customers].[Contact] Where [Contact].[CustomerID] = [LeadCustomer].[CustomerID])".AsSqlField("Country"),
                    "(Select Top(1) [Zone] From [Customers].[Contact] Where [Contact].[CustomerID] = [LeadCustomer].[CustomerID])".AsSqlField("Region"),
                    "(Select Top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = [LeadCustomer].[CustomerID])".AsSqlField("City"),
                    "[LeadCustomer].BirthDay as CustomerBirthDay",
                    "[Lead].[Comment]"
                    //"[LeadCustomer].RegistrationDateTime as CustomerRegistrationDateTime",

                    //("(Select Top(1) [Order].Number From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                    // "WHERE [CustomerId] = [LeadCustomer].[CustomerId] Order by [OrderDate] Desc)")
                    //    .AsSqlField("LastOrderNumber"),

                    //("(Select Top(1) [Order].OrderId From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                    // "WHERE [CustomerId] = [LeadCustomer].[CustomerId] Order by [OrderDate] Desc)")
                    //    .AsSqlField("LastOrderId"),

                    //("(Select ISNULL(SUM([Sum]),0) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    // "WHERE [OrderCustomer].[CustomerId] = [LeadCustomer].[CustomerId] and [Order].[PaymentDate] is not null)")
                    //    .AsSqlField("OrdersSum"),

                    //("(Select COUNT([Order].[OrderId]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    // "WHERE [OrderCustomer].[CustomerId] = [LeadCustomer].[CustomerId] and [Order].[PaymentDate] is not null)")
                    //    .AsSqlField("OrdersCount")
                    );
            }

            _paging.From("[Order].[Lead]");
            _paging.Left_Join("[Customers].[Customer] as LeadCustomer on [Lead].[CustomerId] = [LeadCustomer].[CustomerId]");
            _paging.Left_Join("[Customers].[Managers] ON [Lead].[ManagerId] = [Managers].[ManagerID]");
            _paging.Left_Join("[Customers].[Customer] as ManagerCustomer ON [Managers].[CustomerId] = [ManagerCustomer].[CustomerId]");
            _paging.Left_Join("[CRM].[DealStatus] ON [DealStatus].[Id] = [Lead].[DealStatusId]");
            _paging.Left_Join("[Order].[LeadCurrency] ON [Lead].[Id] = [LeadCurrency].[LeadId]");

            Filter();
            Sorting();
        }

        private void Filter()
        {
            if (!string.IsNullOrWhiteSpace(_filter.CustomerId))
            {
                _paging.Where("Lead.CustomerId = {0}", _filter.CustomerId);
            }

            if (_filter.DealStatusId != null)
            {
                _paging.Where("DealStatusId = {0}", _filter.DealStatusId.Value);
            }
            else if (_filter.SalesFunnelId > 0)
            {
                _paging.Where("[Lead].SalesFunnelId = {0}", _filter.SalesFunnelId);
            }

            if (_filter.SalesFunnelId == 0)
            {
                _paging.Select("[SalesFunnel].[Name]".AsSqlField("SalesFunnelName"));
                _paging.Left_Join("[CRM].[SalesFunnel] On [SalesFunnel].[Id]=Lead.SalesFunnelId");
            }
            
            if (!string.IsNullOrWhiteSpace(_filter.Search))
            {
                _paging.Where(
                    "([Lead].Id LIKE '%'+{0}+'%' OR " +
                    "[Lead].FirstName LIKE '%'+{0}+'%' OR " +
                    "[Lead].LastName LIKE '%'+{0}+'%' OR " +
                    "[Lead].Patronymic LIKE '%'+{0}+'%' OR " +
                    "[Lead].Phone LIKE '%'+{0}+'%' OR " +
                    "[Lead].Email LIKE '%'+{0}+'%' OR " +
                    "[Lead].Description LIKE '%'+{0}+'%' OR " +
                    "(isNull([LeadCustomer].LastName, '') + ' ' + isNull([LeadCustomer].FirstName, '') + ' ' + isNull([LeadCustomer].Patronymic, '')) LIKE '%'+{0}+'%' OR " +
                    "(isNull([LeadCustomer].FirstName, '') + ' ' + isNull([LeadCustomer].LastName, '')) LIKE '%'+{0}+'%' OR " +
                    "(isNull([Lead].LastName, '') + ' ' + isNull([Lead].FirstName, '') + ' ' + isNull([Lead].Patronymic, '')) LIKE '%'+{0}+'%' OR " +
                    "(isNull([Lead].FirstName, '') + ' ' + isNull([Lead].LastName, '')) LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].Phone LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].Email LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].Organization LIKE '%'+{0}+'%') ",
                    _filter.Search);
            }

            if (!string.IsNullOrWhiteSpace(_filter.Name))
            {
                _paging.Where(
                    "([Lead].FirstName LIKE '%'+{0}+'%' OR [Lead].LastName LIKE '%'+{0}+'%' OR [Lead].Patronymic LIKE '%'+{0}+'%' OR " +
                    "(isNull([LeadCustomer].LastName, '') + ' ' + isNull([LeadCustomer].FirstName, '') + ' ' + isNull([LeadCustomer].Patronymic, '')) LIKE '%'+{0}+'%') ",
                    _filter.Name);
            }

            if (!string.IsNullOrWhiteSpace(_filter.Organization))
            {
                _paging.Where("([LeadCustomer].Organization LIKE '%'+{0}+'%')", _filter.Organization);
            }

            DateTime dateFrom, dateTo;
            if (_filter.CreatedDateFrom != null && DateTime.TryParse(_filter.CreatedDateFrom, out dateFrom))
            {
                _paging.Where("Lead.CreatedDate >= {0}", dateFrom);
            }
            if (_filter.CreatedDateTo != null && DateTime.TryParse(_filter.CreatedDateTo, out dateTo))
            {
                _paging.Where("Lead.CreatedDate <= {0}", dateTo);
            }
            
            if (_filter.SumFrom != null)
            {
                _paging.Where("Lead.Sum >= {0}", _filter.SumFrom.Value);
            }
            if (_filter.SumTo != null)
            {
                _paging.Where("Lead.Sum <= {0}", _filter.SumTo.Value);
            }

            if (_filter.ManagerId.HasValue)
            {
                if (_filter.ManagerId.Value == -1)
                {
                    _paging.Where("[Lead].ManagerId IS NULL");
                }
                else
                {
                    _paging.Where("[Lead].ManagerId = {0}", _filter.ManagerId.Value);
                }
            }

            if (_filter.OrderSourceId != null)
            {
                _paging.Where("Lead.OrderSourceId = {0}", _filter.OrderSourceId.Value);
            }

            if (_filter.FunnelId != null)
            {
                _paging.Where("Lead.SalesFunnelId = {0}", _filter.FunnelId.Value);
            }

            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.Assigned)
                    {
                        _paging.Where("Lead.ManagerId = {0}", manager.ManagerId);
                    }
                    else if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.AssignedAndFree)
                    {
                        _paging.Where("(Lead.ManagerId = {0} or Lead.ManagerId is null)", manager.ManagerId);
                    }

                    _paging.Where(
                        "(Exists ( " +
                        "Select 1 From [CRM].[SalesFunnel_Manager] " +
                        "Where (SalesFunnel_Manager.SalesFunnelId = Lead.SalesFunnelId and SalesFunnel_Manager.ManagerId = {0}) " +
                        ") OR " +
                        "(Select Count(*) From [CRM].[SalesFunnel_Manager] Where SalesFunnel_Manager.SalesFunnelId = Lead.SalesFunnelId) = 0)",
                        manager.ManagerId);
                }
            }

            if (!string.IsNullOrWhiteSpace(_filter.City))
            {
                _paging.Where(
                    "(Select Top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = [LeadCustomer].[CustomerID]) LIKE '%'+{0}+'%'",
                    _filter.City);
            }

            if (_filter.CustomerFields != null)
            {
                foreach (var fieldFilter in _filter.CustomerFields.Where(x => x.Value != null))
                {
                    var fieldsFilterModel = fieldFilter.Value;
                    if (fieldsFilterModel.DateFrom.HasValue)
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                   "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateFrom.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.DateTo.HasValue)
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateTo.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.From.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.From.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.To.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.To.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.ValueExact.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value = {1}))",
                            fieldFilter.Key, fieldsFilterModel.ValueExact);
                    }

                    if (fieldsFilterModel.Value.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value like '%' + {1} + '%'))",
                            fieldFilter.Key, fieldsFilterModel.Value);
                    }
                }
            }

            if (_filter.LeadFields != null)
            {
                foreach (var fieldFilter in _filter.LeadFields.Where(x => x.Value != null))
                {
                    var fieldsFilterModel = fieldFilter.Value;
                    if (fieldsFilterModel.DateFrom.HasValue)
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                   "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateFrom.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.DateTo.HasValue)
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value is not null and LeadFieldValuesMap.Value <> '' and LeadFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateTo.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.From.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.From.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.To.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.To.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value is not null and LeadFieldValuesMap.Value <> '' and LeadFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.ValueExact.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value = {1}))",
                            fieldFilter.Key, fieldsFilterModel.ValueExact);
                    }

                    if (fieldsFilterModel.Value.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value like '%' + {1} + '%'))",
                            fieldFilter.Key, fieldsFilterModel.Value);
                    }
                }
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filter.Sorting) || _filter.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("[Lead].Id");
                return;
            }

            string sorting = _filter.Sorting.ToLower().Replace("formatted", "");
            
            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filter.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(field);
                }
                else
                {
                    _paging.OrderByDesc(field);
                }
            }
        }
    }
}
