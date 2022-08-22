using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AdvantShop.Areas.Api.Model.Orders;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Webhook.Models.Api;
using AdvantShop.Core.SQL2;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Api;

namespace AdvantShop.Areas.Api.Handlers.Orders
{
    public class GetOrders : EntitiesHandler<FilterOrdersModel, OrderModel>
    {
        public GetOrders(FilterOrdersModel filterModel) : base(filterModel) { }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select(
                "[Order].[OrderID]",
                "[Order].[Number]",
                "[OrderCurrency].[CurrencyCode]",
                "[Order].[Sum]",
                "[Order].[OrderDate]",

                "[Order].[CustomerComment]",
                "[Order].[AdminOrderComment]",

                "[Order].[PaymentMethodName]",
                "[Order].[PaymentCost]",

                "[Order].[ShippingMethodName]",
                "[Order].[ShippingCost]",
                "[Order].[ShippingTaxType]",
                "[Order].[TrackNumber]",
                "[Order].[DeliveryDate]",
                "[Order].[DeliveryTime]",

                "[Order].[OrderDiscount]",
                "[Order].[OrderDiscountValue]",

                "[Order].[BonusCardNumber]",
                "[Order].[BonusCost]",

                "[Order].[LpId]",

                "[Order].[PaymentDate]",
                "[Order].[IsDraft]",

                "[OrderCustomer].[CustomerID]",
                "[OrderCustomer].[FirstName]",
                "[OrderCustomer].[LastName]",
                "[OrderCustomer].[Patronymic]",
                "[OrderCustomer].[Organization]",
                "[OrderCustomer].[Email]",
                "[OrderCustomer].[Phone]",
                "[OrderCustomer].[Country]",
                "[OrderCustomer].[Region]",
                "[OrderCustomer].[District]",
                "[OrderCustomer].[City]",
                "[OrderCustomer].[Zip]",
                "[OrderCustomer].[CustomField1]",
                "[OrderCustomer].[CustomField2]",
                "[OrderCustomer].[CustomField3]",
                "[OrderCustomer].[Street]",
                "[OrderCustomer].[House]",
                "[OrderCustomer].[Apartment]",
                "[OrderCustomer].[Structure]",
                "[OrderCustomer].[Entrance]",
                "[OrderCustomer].[Floor]",

                "[Order].[OrderStatusID]",
                "[OrderStatus].[StatusName]",
                "[OrderStatus].[IsCanceled]".AsSqlField("StatusIsCanceled"),
                "[OrderStatus].[IsCompleted]".AsSqlField("StatusIsCompleted"),
                "[OrderStatus].[Hidden]".AsSqlField("StatusHidden"),

                "[Order].[OrderSourceId]",
                "[OrderSource].[Name]".AsSqlField("SourceName"),
                "[OrderSource].[Main]".AsSqlField("SourceMain"),
                "[OrderSource].[Type]".AsSqlField("SourceType")
                );

            paging.From("[Order].[Order]");
            paging.Left_Join("[Order].[OrderCustomer] ON [Order].[OrderID]=[OrderCustomer].[OrderID]");
            paging.Left_Join("[Order].[OrderStatus] ON [OrderStatus].[OrderStatusID]=[Order].[OrderStatusID]");
            paging.Left_Join("[Order].[OrderCurrency] ON [Order].[OrderID] = [OrderCurrency].[OrderID]");
            paging.Left_Join("[Order].[OrderSource] on [Order].[OrderSourceId] = [OrderSource].[Id]");

            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            paging.Where("[Order].[IsDraft] != 1");

            if (FilterModel.CustomerId.HasValue)
                paging.Where("[OrderCustomer].[CustomerID] = {0}", FilterModel.CustomerId.Value);

            if (FilterModel.StatusId.HasValue)
                paging.Where("[Order].[OrderStatusID] = {0}", FilterModel.StatusId.Value);

            if (FilterModel.SumFrom.HasValue)
            {
                paging.Where("[Order].[Sum] >= {0}", FilterModel.SumFrom.Value);
            }
            if (FilterModel.SumTo.HasValue)
            {
                paging.Where("[Order].[Sum] <= {0}", FilterModel.SumTo.Value);
            }

            if (FilterModel.IsPaid.HasValue)
            {
                paging.Where(FilterModel.IsPaid.Value ? "PaymentDate is not null" : "PaymentDate is null");
            }

            DateTime from, to;
            if (!string.IsNullOrWhiteSpace(FilterModel.DateFrom) && DateTime.TryParse(FilterModel.DateFrom, out from))
            {
                paging.Where("OrderDate >= {0}", from);
            }
            if (!string.IsNullOrWhiteSpace(FilterModel.DateTo) && DateTime.TryParse(FilterModel.DateTo, out to))
            {
                paging.Where("OrderDate <= {0}", to);
            }

            return paging;
        }

        protected override SqlPaging Sorting(SqlPaging paging)
        {
            if (string.IsNullOrEmpty(FilterModel.Sorting) || FilterModel.SortingType == FilterSortingType.None)
            {
                paging.OrderBy("[Order].[OrderID]");

                return paging;
            }

            /*var sorting = FilterModel.Sorting;

            var field = paging.SelectFields().FirstOrDefault(x => x.FieldName.Equals(sorting, StringComparison.OrdinalIgnoreCase));
            if (field != null)
            {
                if (FilterModel.SortingType == FilterSortingType.Asc)
                    paging.OrderBy(sorting);
                else
                    paging.OrderByDesc(sorting);
            }*/

            return paging;
        }

        protected override List<OrderModel> FillItems(SqlPaging paging)
        {
            return paging.PageItemsList<OrderModel>(GetFromReader);
        }

        private OrderModel GetFromReader(IDataReader reader)
        {
            var item = new OrderModel
            {
                Id = SQLDataHelper.GetInt(reader, "OrderID"),
                Number = SQLDataHelper.GetString(reader, "Number"),
                Currency = SQLDataHelper.GetString(reader, "CurrencyCode"),
                Sum = SQLDataHelper.GetFloat(reader, "Sum"),
                Date = SQLDataHelper.GetDateTime(reader, "OrderDate"),

                CustomerComment = SQLDataHelper.GetString(reader, "CustomerComment"),
                AdminComment = SQLDataHelper.GetString(reader, "AdminOrderComment"),

                PaymentName = SQLDataHelper.GetString(reader, "PaymentMethodName"),
                PaymentCost = SQLDataHelper.GetFloat(reader, "PaymentCost"),

                ShippingName = SQLDataHelper.GetString(reader, "ShippingMethodName"),
                ShippingCost = SQLDataHelper.GetFloat(reader, "ShippingCost"),
                ShippingTaxName = ((Taxes.TaxType)SQLDataHelper.GetInt(reader, "ShippingTaxType")).Localize(),
                TrackNumber = SQLDataHelper.GetString(reader, "TrackNumber"),
                DeliveryDate = SQLDataHelper.GetNullableDateTime(reader, "DeliveryDate"),
                DeliveryTime = SQLDataHelper.GetString(reader, "DeliveryTime"),

                OrderDiscount = SQLDataHelper.GetFloat(reader, "OrderDiscount"),
                OrderDiscountValue = SQLDataHelper.GetFloat(reader, "OrderDiscountValue"),

                BonusCardNumber = SQLDataHelper.GetNullableLong(reader, "BonusCardNumber"),
                BonusCost = SQLDataHelper.GetFloat(reader, "BonusCost"),

                LpId = SQLDataHelper.GetNullableInt(reader, "LpId"),

                IsPaid = SQLDataHelper.GetNullableDateTime(reader, "PaymentDate").HasValue,
            };
            
            var statusId = SQLDataHelper.GetInt(reader, "OrderStatusID");
            var sourceId = SQLDataHelper.GetInt(reader, "OrderSourceId");

            item.Customer = new OrderCustomerModel
            {
                CustomerId = SQLDataHelper.GetGuid(reader, "CustomerID"),
                FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                LastName = SQLDataHelper.GetString(reader, "LastName"),
                Patronymic = SQLDataHelper.GetString(reader, "Patronymic"),
                Organization = SQLDataHelper.GetString(reader, "Organization"),
                Email = SQLDataHelper.GetString(reader, "Email"),
                Phone = SQLDataHelper.GetString(reader, "Phone"),
                Country = SQLDataHelper.GetString(reader, "Country"),
                Region = SQLDataHelper.GetString(reader, "Region"),
                District = SQLDataHelper.GetString(reader, "District"),
                City = SQLDataHelper.GetString(reader, "City"),
                Zip = SQLDataHelper.GetString(reader, "Zip"),
                CustomField1 = SQLDataHelper.GetString(reader, "CustomField1"),
                CustomField2 = SQLDataHelper.GetString(reader, "CustomField2"),
                CustomField3 = SQLDataHelper.GetString(reader, "CustomField3"),
                Street = SQLDataHelper.GetString(reader, "Street"),
                House = SQLDataHelper.GetString(reader, "House"),
                Apartment = SQLDataHelper.GetString(reader, "Apartment"),
                Structure = SQLDataHelper.GetString(reader, "Structure"),
                Entrance = SQLDataHelper.GetString(reader, "Entrance"),
                Floor = SQLDataHelper.GetString(reader, "Floor"),
            };

            item.Status = statusId > 0
                ? new OrderStatusModel
                    {
                        Id = statusId,
                        Name = SQLDataHelper.GetString(reader, "StatusName"),
                        IsCanceled = SQLDataHelper.GetBoolean(reader, "StatusIsCanceled"),
                        IsCompleted = SQLDataHelper.GetBoolean(reader, "StatusIsCompleted"),
                        Hidden = SQLDataHelper.GetBoolean(reader, "StatusHidden"),
                    }
                : null;

            item.Source = sourceId > 0
                ? new OrderSourceModel
                    {
                        Id = sourceId,
                        Name = SQLDataHelper.GetString(reader, "SourceName"),
                        Main = SQLDataHelper.GetBoolean(reader, "SourceMain"),
                        Type = SQLDataHelper.GetString(reader, "SourceType").TryParseEnum<Core.Services.Orders.OrderType>().ToString(),
                    }
                : null;

            if (FilterModel.LoadItems)
                item.Items = AdvantShop.Orders.OrderService.GetOrderItems(item.Id).Select(OrderItemModel.FromOrderItem).ToList();
            else
                item.Items = null;

            return item;
        }
    }
}