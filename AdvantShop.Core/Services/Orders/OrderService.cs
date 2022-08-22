//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Loging;
using AdvantShop.Core.Services.Loging.TrafficSource;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Core.Services.Triggers.DeferredDatas;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Payment;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Taxes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Debug = AdvantShop.Diagnostics.Debug;

namespace AdvantShop.Orders
{
    public class OrderService
    {
        #region OrderInformation

        public static OrderInformation GetOrderHistoryFromReader(SqlDataReader reader)
        {
            return new OrderInformation
            {
                OrderID = SQLDataHelper.GetInt(reader, "OrderID"),
                OrderNumber = SQLDataHelper.GetString(reader, "Number"),
                ShippingMethod = SQLDataHelper.GetString(reader, "ShippingMethod"),
                ShippingMethodName = SQLDataHelper.GetString(reader, "ShippingMethodName"),
                ArchivedPaymentName = SQLDataHelper.GetString(reader, "PaymentMethodName"),
                PaymentMethodID = SQLDataHelper.GetInt(reader, "PaymentMethodID"),
                Status = SQLDataHelper.GetString(reader, "StatusName"),
                StatusID = SQLDataHelper.GetInt(reader, "OrderStatusID"),
                PreviousStatus = SQLDataHelper.GetString(reader, "PreviousStatus"),
                Sum = SQLDataHelper.GetFloat(reader, "Sum"),
                OrderDate = SQLDataHelper.GetDateTime(reader, "OrderDate"),
                Payed = SQLDataHelper.GetNullableDateTime(reader, "PaymentDate") != null,
                ProductsHtml = string.Empty,
                CurrencyCode = SQLDataHelper.GetString(reader, "CurrencyCode"),
                CurrencyValue = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                CurrencySymbol = SQLDataHelper.GetString(reader, "CurrencySymbol"),
                IsCodeBefore = SQLDataHelper.GetBoolean(reader, "IsCodeBefore"),
                ManagerId = SQLDataHelper.GetNullableInt(reader, "ManagerId"),
                ManagerName = SQLDataHelper.GetString(reader, "ManagerName"),
                TrackNumber = SQLDataHelper.GetString(reader, "TrackNumber")
            };
        }

        public static List<OrderInformation> GetCustomerOrderHistory(Guid customerId)
        {
            return SQLDataAccess.ExecuteReadList("[Order].[sp_GetCustomerOrderHistory]", CommandType.StoredProcedure,
                GetOrderHistoryFromReader, new SqlParameter("@CustomerID", customerId));
        }

        #endregion OrderInformation

        #region OrderItems

        private static OrderItem GetOrderItemFromReader(IDataReader reader)
        {
            return new OrderItem
            {
                OrderID = SQLDataHelper.GetInt(reader, "OrderId"),
                OrderItemID = SQLDataHelper.GetInt(reader, "OrderItemID"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Price = SQLDataHelper.GetFloat(reader, "Price"),
                Amount = SQLDataHelper.GetFloat(reader, "Amount"),
                ProductID = SQLDataHelper.GetNullableInt(reader, "ProductID"),
                ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                BarCode = SQLDataHelper.GetString(reader, "BarCode"),
                SupplyPrice = SQLDataHelper.GetFloat(reader, "SupplyPrice"),
                Weight = SQLDataHelper.GetFloat(reader, "Weight"),
                Color = SQLDataHelper.GetString(reader, "Color", null),
                Size = SQLDataHelper.GetString(reader, "Size", null),
                IsCouponApplied = SQLDataHelper.GetBoolean(reader, "IsCouponApplied"),
                PhotoID = SQLDataHelper.GetNullableInt(reader, "PhotoID"),
                DecrementedAmount = SQLDataHelper.GetFloat(reader, "DecrementedAmount"),
                IgnoreOrderDiscount = SQLDataHelper.GetBoolean(reader, "IgnoreOrderDiscount"),
                AccrueBonuses = SQLDataHelper.GetBoolean(reader, "AccrueBonuses"),
                TaxId = SQLDataHelper.GetNullableInt(reader, "TaxId"),
                TaxName = SQLDataHelper.GetString(reader, "TaxName"),
                TaxType = (TaxType)SQLDataHelper.GetInt(reader, "TaxType"),
                TaxRate = SQLDataHelper.GetNullableFloat(reader, "TaxRate"),
                TaxShowInPrice = SQLDataHelper.GetNullableBoolean(reader, "TaxShowInPrice"),
                Width = SQLDataHelper.GetFloat(reader, "Width"),
                Length = SQLDataHelper.GetFloat(reader, "Length"),
                Height = SQLDataHelper.GetFloat(reader, "Height"),
                PaymentMethodType = (ePaymentMethodType)(SQLDataHelper.GetInt(reader, "PaymentMethodType") != 0 ? SQLDataHelper.GetInt(reader, "PaymentMethodType") : 1),
                PaymentSubjectType = (ePaymentSubjectType)(SQLDataHelper.GetInt(reader, "PaymentSubjectType") != 0 ? SQLDataHelper.GetInt(reader, "PaymentSubjectType") : 1),
                IsGift = SQLDataHelper.GetBoolean(reader, "IsGift"),
                BookingServiceId = SQLDataHelper.GetNullableInt(reader, "BookingServiceId"),
                TypeItem = (TypeOrderItem)Enum.Parse(typeof(TypeOrderItem), SQLDataHelper.GetString(reader, "TypeItem"), true)
            };
        }

        public static List<OrderItem> GetOrderItems(int orderId)
        {
            var result =
                SQLDataAccess.ExecuteReadList("[Order].[sp_GetOrderItems]", CommandType.StoredProcedure,
                    GetOrderItemFromReader,
                    new SqlParameter("@OrderID", orderId));
            
            return result;
        }

        public static OrderItem GetOrderItem(int orderItemId)
        {
            var result =
                SQLDataAccess.ExecuteReadOne("Select * from [Order].[OrderItems] Where OrderItemId = @OrderItemId", CommandType.Text,
                    GetOrderItemFromReader,
                    new SqlParameter("@OrderItemId", orderItemId));

            return result;
        }

        public static List<EvaluatedCustomOptions> GetOrderCustomOptionsByOrderItemId(int orderItemId)
        {
            var result = SQLDataAccess.ExecuteReadList(
                    "[Order].[sp_GetSelectedOptionsByOrderItemId]",
                    CommandType.StoredProcedure,
                    reader => new EvaluatedCustomOptions
                    {
                        CustomOptionId = SQLDataHelper.GetInt(reader, "CustomOptionId"),
                        CustomOptionTitle = SQLDataHelper.GetString(reader, "CustomOptionTitle"),
                        OptionId = SQLDataHelper.GetInt(reader, "OptionId"),
                        OptionPriceBc = SQLDataHelper.GetFloat(reader, "OptionPriceBC"),
                        OptionPriceType = (OptionPriceType)SQLDataHelper.GetInt(reader, "OptionPriceType"),
                        OptionTitle = SQLDataHelper.GetString(reader, "OptionTitle")
                    },
                    new SqlParameter("@OrderItemId", orderItemId));
            return result;
        }

        private static void UpdateOrderedItem(int orderId, OrderItem item)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateOrderItem]", CommandType.StoredProcedure,
                new SqlParameter("@OrderItemID", item.OrderItemID),
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@Name", item.Name),
                new SqlParameter("@Price", item.Price),
                new SqlParameter("@Amount", item.Amount),
                new SqlParameter("@ProductID", item.ProductID ?? (object)DBNull.Value),
                new SqlParameter("@ArtNo", item.ArtNo ?? (object)DBNull.Value),
                new SqlParameter("@SupplyPrice", item.SupplyPrice),
                new SqlParameter("@Weight", item.Weight),
                new SqlParameter("@IsCouponApplied", item.IsCouponApplied),
                new SqlParameter("@Color", item.Color ?? (object)DBNull.Value),
                new SqlParameter("@Size", item.Size ?? (object)DBNull.Value),
                new SqlParameter("@DecrementedAmount", item.DecrementedAmount),
                new SqlParameter("@PhotoID", item.PhotoID != 0 && item.PhotoID != null ? item.PhotoID : (object)DBNull.Value),
                new SqlParameter("@IgnoreOrderDiscount", item.IgnoreOrderDiscount),
                new SqlParameter("@AccrueBonuses", item.AccrueBonuses),
                new SqlParameter("@TaxId", item.TaxId ?? (object)DBNull.Value),
                new SqlParameter("@TaxName", item.TaxName ?? (object)DBNull.Value),
                new SqlParameter("@TaxType", item.TaxType != null ? (int)item.TaxType : (object)DBNull.Value),
                new SqlParameter("@TaxRate", item.TaxRate ?? (object)DBNull.Value),
                new SqlParameter("@TaxShowInPrice", item.TaxShowInPrice ?? (object)DBNull.Value),
                new SqlParameter("@Width", item.Width),
                new SqlParameter("@Length", item.Length),
                new SqlParameter("@Height", item.Height),
                new SqlParameter("@PaymentMethodType", (int)item.PaymentMethodType != 0 ? (int)item.PaymentMethodType : 1),
                new SqlParameter("@PaymentSubjectType", (int)item.PaymentSubjectType != 0 ? (int)item.PaymentSubjectType : 1),
                new SqlParameter("@IsGift", item.IsGift),
                new SqlParameter("@BarCode", item.BarCode ?? (object)DBNull.Value)
                //new SqlParameter("@BookingServiceId", item.BookingServiceId ?? (object)DBNull.Value),
                //new SqlParameter("@TypeItem", item.TypeItem.ToString())
                );
        }

        private static void AddOrderedItem(int orderId, OrderItem item)
        {
            item.OrderItemID =
                SQLDataAccess.ExecuteScalar<int>("[Order].[sp_AddOrderItem]", CommandType.StoredProcedure,
                    new SqlParameter("@OrderID", orderId),
                    new SqlParameter("@Name", item.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Price", item.Price),
                    new SqlParameter("@Amount", item.Amount),
                    new SqlParameter("@ProductID", item.ProductID ?? (object)DBNull.Value),
                    new SqlParameter("@ArtNo", item.ArtNo ?? (object)DBNull.Value),
                    new SqlParameter("@SupplyPrice", item.SupplyPrice),
                    new SqlParameter("@Weight", item.Weight),
                    new SqlParameter("@IsCouponApplied", item.IsCouponApplied),
                    new SqlParameter("@Color", item.Color ?? (object)DBNull.Value),
                    new SqlParameter("@Size", item.Size ?? (object)DBNull.Value),
                    new SqlParameter("@DecrementedAmount", item.DecrementedAmount),
                    new SqlParameter("@PhotoID", item.PhotoID != 0 && item.PhotoID != null ? item.PhotoID : (object)DBNull.Value),
                    new SqlParameter("@IgnoreOrderDiscount", item.IgnoreOrderDiscount),
                    new SqlParameter("@AccrueBonuses", item.AccrueBonuses),
                    new SqlParameter("@TaxId", item.TaxId ?? (object)DBNull.Value),
                    new SqlParameter("@TaxName", item.TaxName ?? (object)DBNull.Value),
                    new SqlParameter("@TaxType", item.TaxType != null ? (int)item.TaxType : (object)DBNull.Value),
                    new SqlParameter("@TaxRate", item.TaxRate ?? (object)DBNull.Value),
                    new SqlParameter("@TaxShowInPrice", item.TaxShowInPrice ?? (object)DBNull.Value),
                    new SqlParameter("@Width", item.Width),
                    new SqlParameter("@Height", item.Height),
                    new SqlParameter("@Length", item.Length),
                    new SqlParameter("@PaymentMethodType", (int)item.PaymentMethodType != 0 ? (int)item.PaymentMethodType : 1),
                    new SqlParameter("@PaymentSubjectType", (int)item.PaymentSubjectType != 0 ? (int)item.PaymentSubjectType : 1),
                    new SqlParameter("@IsGift", item.IsGift),
                    new SqlParameter("@BookingServiceId", item.BookingServiceId ?? (object)DBNull.Value),
                    new SqlParameter("@TypeItem", item.TypeItem.ToString()),
                    new SqlParameter("@BarCode", item.BarCode ?? (object)DBNull.Value)
                    );

            if (item.SelectedOptions != null)
            {
                foreach (var option in item.SelectedOptions)
                {
                    AddOrderItemCustomOption(option, item.OrderItemID);
                }
            }
        }

        public static void AddOrderItemCustomOption(EvaluatedCustomOptions option, int orderItemId)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_AddOrderCustomOptions]", CommandType.StoredProcedure,
                new SqlParameter("@CustomOptionId", option.CustomOptionId),
                new SqlParameter("@OptionId", option.OptionId),
                new SqlParameter("@CustomOptionTitle", option.CustomOptionTitle),
                new SqlParameter("@OptionTitle", option.OptionTitle),
                new SqlParameter("@OptionPriceBC", option.OptionPriceBc),
                new SqlParameter("@OptionPriceType", option.OptionPriceType),
                new SqlParameter("@OrderItemID", orderItemId));
        }

        public static void DeleteOrderItemCustomOptions(int orderItemId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [Order].[OrderCustomOptions] Where [OrderedCartID] = @OrderItemID", CommandType.Text,
                new SqlParameter("@OrderItemID", orderItemId));
        }


        private static void AddUpdateOrderedItem(Order order, OrderItem item, OrderChangedBy changedBy = null, bool ignoreHistory = false)
        {
            var oldItem = GetOrderItem(item.OrderItemID);
            if (oldItem != null)
            {
                if (!ignoreHistory)
                    OrderHistoryService.ChangingOrderItem(order.OrderID, oldItem, item, changedBy);

                UpdateOrderedItem(order.OrderID, item);
            }
            else
            {
                if (!ignoreHistory)
                    OrderHistoryService.ChangingOrderItem(order.OrderID, null, item, changedBy);

                AddOrderedItem(order.OrderID, item);
            }

            if (SettingsCheckout.DecrementProductsCount)
            {
                UpdateDecrementedOfferAmount(item, order.Decremented, false);
            }
        }

        public static bool AddUpdateOrderItems(List<OrderItem> items, List<OrderItem> oldItems, Order order,
                                                OrderChangedBy changedBy = null, bool trackChanges = true,
                                                bool updateModules = true)
        {
            var itemsToDelete = new List<OrderItem>();

            foreach (var oldOrderItem in oldItems)
            {
                var isfound = items.Find(x => x.OrderItemID == oldOrderItem.OrderItemID) != null;

                if (!isfound)
                {
                    oldOrderItem.Amount = 0;
                    AddUpdateOrderedItem(order, oldOrderItem, changedBy, !trackChanges);
                    itemsToDelete.Add(oldOrderItem);
                }
            }

            if (itemsToDelete.Count > 0)
            {
                foreach (var item in itemsToDelete)
                {
                    DeleteOrderItem(order.OrderID, item, changedBy, trackChanges);
                }
            }

            foreach (var orderItem in items)
            {
                AddUpdateOrderedItem(order, orderItem, changedBy, !trackChanges);
            }

            var orderFromDb = GetOrder(order.OrderID);
            RefreshTotal(orderFromDb, !trackChanges, changedBy, updateModules);

            return false;
        }

        private static void UpdateDecrementedOfferAmount(OrderItem item, bool isOrderDecremented, bool increment)
        {
            if (!isOrderDecremented || item.Amount == item.DecrementedAmount || item.TypeItem != TypeOrderItem.Product)
                return;

            SQLDataAccess.ExecuteNonQuery(
                "Update Catalog.Offer Set Amount = Round(Amount " + (increment ? "+" : "-  @Amount") + " + @DecrementedAmount, 4)  Where Artno = @artno; " +
                "Update [Order].[OrderItems] Set DecrementedAmount = amount Where [OrderItemId] = @OrderItemId  ",
                CommandType.Text,
                new SqlParameter("@Amount", item.Amount),
                new SqlParameter("@DecrementedAmount", item.DecrementedAmount),
                new SqlParameter("@artno", item.ArtNo),
                new SqlParameter("@OrderItemId", item.OrderItemID));

            if (item.ProductID != null)
                ProductService.PreCalcProductParams(item.ProductID.Value);
        }

        public static void DeleteOrderItem(int orderId, OrderItem item, OrderChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
                OrderHistoryService.ChangingOrderItem(orderId, item, null, changedBy);

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Order].[OrderItems] WHERE [OrderItemID] = @OrderItemID",
                                          CommandType.Text,
                                          new SqlParameter("@OrderID", orderId),
                                          new SqlParameter("@OrderItemID", item.OrderItemID));
        }

        public static void ClearOrderItems(int orderId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Order].[OrderItems] WHERE [OrderID] = @OrderID",
                                          CommandType.Text, new SqlParameter("@OrderID", orderId));
        }

        #endregion OrderItems

        public static Order GetOrderFromReader(IDataReader reader)
        {
            return new Order
            {
                OrderID = SQLDataHelper.GetInt(reader, "OrderID"),
                Number = SQLDataHelper.GetString(reader, "Number"),
                Code = SQLDataHelper.GetGuid(reader, "Code"),
                OrderStatusId = SQLDataHelper.GetInt(reader, "OrderStatusID"),
                StatusComment = SQLDataHelper.GetString(reader, "StatusComment"),
                AdditionalTechInfo = SQLDataHelper.GetString(reader, "AdditionalTechInfo"),
                AdminOrderComment = SQLDataHelper.GetString(reader, "AdminOrderComment"),
                Sum = SQLDataHelper.GetFloat(reader, "Sum"),
                ShippingCost = SQLDataHelper.GetFloat(reader, "ShippingCost"),
                PaymentCost = SQLDataHelper.GetFloat(reader, "PaymentCost"),
                OrderDiscount = SQLDataHelper.GetFloat(reader, "OrderDiscount"),
                OrderDiscountValue = SQLDataHelper.GetFloat(reader, "OrderDiscountValue"),
                DiscountCost = SQLDataHelper.GetFloat(reader, "DiscountCost"),
                TaxCost = SQLDataHelper.GetFloat(reader, "TaxCost"),
                OrderDate = SQLDataHelper.GetDateTime(reader, "OrderDate"),
                SupplyTotal = SQLDataHelper.GetFloat(reader, "SupplyTotal"),
                ShippingMethodId = SQLDataHelper.GetInt(reader, "ShippingMethodID"),
                PaymentMethodId = SQLDataHelper.GetInt(reader, "PaymentMethodId"),
                AffiliateID = SQLDataHelper.GetInt(reader, "AffiliateID"),
                CustomerComment = SQLDataHelper.GetString(reader, "CustomerComment"),
                Decremented = SQLDataHelper.GetBoolean(reader, "Decremented"),
                PaymentDate =
                    SQLDataHelper.GetDateTime(reader, "PaymentDate") == DateTime.MinValue
                        ? null
                        : (DateTime?)SQLDataHelper.GetDateTime(reader, "PaymentDate"),
                ArchivedPaymentName = SQLDataHelper.GetString(reader, "PaymentMethodName"),
                ArchivedShippingName = SQLDataHelper.GetString(reader, "ShippingMethodName"),

                GroupName = SQLDataHelper.GetString(reader, "GroupName"),
                GroupDiscount = SQLDataHelper.GetFloat(reader, "GroupDiscount"),
                Certificate = SQLDataHelper.GetString(reader, "CertificateCode").IsNotEmpty()
                                    ? new OrderCertificate
                                    {
                                        Code = SQLDataHelper.GetString(reader, "CertificateCode"),
                                        Price = SQLDataHelper.GetFloat(reader, "CertificatePrice")
                                    }
                                    : null,
                Coupon = SQLDataHelper.GetString(reader, "CouponCode").IsNotEmpty()
                                ? new OrderCoupon
                                {
                                    Type = (CouponType)SQLDataHelper.GetInt(reader, "CouponType"),
                                    Code = SQLDataHelper.GetString(reader, "CouponCode"),
                                    Value = SQLDataHelper.GetFloat(reader, "CouponValue")
                                }
                                : null,
                BonusCost = SQLDataHelper.GetFloat(reader, "BonusCost"),
                BonusCardNumber = SQLDataHelper.GetNullableLong(reader, "BonusCardNumber"),
                ManagerId = SQLDataHelper.GetNullableInt(reader, "ManagerId"),

                UseIn1C = SQLDataHelper.GetBoolean(reader, "UseIn1C"),
                ModifiedDate = SQLDataHelper.GetDateTime(reader, "ModifiedDate"),
                ManagerConfirmed = SQLDataHelper.GetBoolean(reader, "ManagerConfirmed"),
                PreviousStatus = SQLDataHelper.GetString(reader, "PreviousStatus"),

                OrderSourceId = SQLDataHelper.GetInt(reader, "OrderSourceId"),
                CustomData = SQLDataHelper.GetString(reader, "CustomData"),
                IsDraft = SQLDataHelper.GetBoolean(reader, "IsDraft"),
                DeliveryDate = SQLDataHelper.GetNullableDateTime(reader, "DeliveryDate"),
                DeliveryTime = SQLDataHelper.GetString(reader, "DeliveryTime"),
                TrackNumber = SQLDataHelper.GetString(reader, "TrackNumber"),
                IsFromAdminArea = SQLDataHelper.GetBoolean(reader, "IsFromAdminArea"),
                LeadId = SQLDataHelper.GetNullableInt(reader, "LeadId"),
                ShippingTaxType = (TaxType)SQLDataHelper.GetInt(reader, "ShippingTaxType"),
                ShippingPaymentMethodType = (ePaymentMethodType)SQLDataHelper.GetInt(reader, "ShippingPaymentMethodType"),
                ShippingPaymentSubjectType = (ePaymentSubjectType)SQLDataHelper.GetInt(reader, "ShippingPaymentSubjectType"),
                LpId = SQLDataHelper.GetNullableInt(reader, "LpId"),
                IsSendedToGA = SQLDataHelper.GetBoolean(reader, "IsSendedToGA"),
                PayCode = SQLDataHelper.GetString(reader, "PayCode"),
                TotalWeight = SQLDataHelper.GetNullableFloat(reader, "TotalWeight"),
                TotalLength = SQLDataHelper.GetNullableFloat(reader, "TotalLength"),
                TotalWidth = SQLDataHelper.GetNullableFloat(reader, "TotalWidth"),
                TotalHeight = SQLDataHelper.GetNullableFloat(reader, "TotalHeight"),
                AvailablePaymentCashOnDelivery = SQLDataHelper.GetBoolean(reader, "AvailablePaymentCashOnDelivery"),
                AvailablePaymentPickPoint = SQLDataHelper.GetBoolean(reader, "AvailablePaymentPickPoint"),
            };
        }

        public static OrderAutocomplete GetOrderForAutocompleteFromReader(IDataReader reader)
        {
            return new OrderAutocomplete
            {
                OrderID = SQLDataHelper.GetInt(reader, "OrderID"),
                Number = SQLDataHelper.GetString(reader, "Number"),
                FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                LastName = SQLDataHelper.GetString(reader, "LastName"),
                Email = SQLDataHelper.GetString(reader, "Email"),
                MobilePhone = SQLDataHelper.GetString(reader, "Phone"),
                OrderDate = SQLDataHelper.GetDateTime(reader, "OrderDate"),
                Sum = SQLDataHelper.GetFloat(reader, "Sum"),
                StatusName = SQLDataHelper.GetString(reader, "StatusName")
            };
        }

        public static List<Order> GetAllOrders()
        {
            return SQLDataAccess.ExecuteReadList<Order>("SELECT * FROM [Order].[Order] Where IsDraft <> 1", CommandType.Text, GetOrderFromReader);
        }

        public static List<Order> GetAllOrders(DateTime from, DateTime to)
        {
            return SQLDataAccess.ExecuteReadList<Order>(
                "SELECT * FROM [Order].[Order] Where IsDraft <> 1 and OrderDate >= @from and OrderDate <= @to",
                CommandType.Text,
                GetOrderFromReader,
                new SqlParameter("@from", from),
                new SqlParameter("@to", to));
        }

        public static Dictionary<Guid, long> GetAllOrdersPhones()
        {
            var dict = new Dictionary<Guid, long>();
            dict.AddRange(
                SQLDataAccess.ExecuteReadIEnumerable<KeyValuePair<Guid, long>>(
                    "SELECT DISTINCT CustomerID, (select TOP(1) StandardPhone FROM [Order].[OrderCustomer] WHERE CustomerID = tbl.CustomerID ORDER BY OrderId DESC) AS StandardPhone FROM [Order].[OrderCustomer] AS tbl WHERE StandardPhone IS NOT NULL",
                    CommandType.Text,
                    reader =>
                        new KeyValuePair<Guid, long>(SQLDataHelper.GetGuid(reader, "CustomerID"),
                            SQLDataHelper.GetLong(reader, "StandardPhone"))));
            return dict;
        }

        public static List<Order> GetOrders(string email)
        {
            return
                SQLDataAccess.ExecuteReadList<Order>(
                    "SELECT * FROM [Order].[Order] inner join [Order].[OrderCustomer] on [OrderCustomer].[OrderID] = [Order].[OrderID] Where Email=@Email order by OrderDate desc",
                    CommandType.Text, GetOrderFromReader, new SqlParameter("@email", email));
        }

        public static List<Order> GetOrdersByPhone(string phone)
        {
            return
                SQLDataAccess.ExecuteReadList<Order>(
                    "SELECT * FROM [Order].[Order] inner join [Order].[OrderCustomer] on [OrderCustomer].[OrderID] = [Order].[OrderID] Where Phone=@Phone or StandardPhone=@Phone",
                    CommandType.Text, GetOrderFromReader, new SqlParameter("@Phone", phone));
        }

        public static int GetOrdersCountByCustomer(Guid customerId)
        {
            return Convert.ToInt32(
                SQLDataAccess.ExecuteScalar(
                    "SELECT Count([Order].[OrderId]) FROM [Order].[Order] inner join [Order].[OrderCustomer] on [OrderCustomer].[OrderID] = [Order].[OrderID] Where CustomerId=@CustomerId",
                    CommandType.Text, new SqlParameter("@CustomerId", customerId)));
        }

        public static List<OrderAutocomplete> GetOrdersForAutocomplete(string query)
        {
            if (query.IsDecimal())
            {
                return SQLDataAccess.ExecuteReadList<OrderAutocomplete>(
                "SELECT [Order].[OrderID], Number, FirstName, LastName, Email, Phone, OrderDate, [Order].[Sum], StatusName " +
                "FROM [Order].[Order] " +
                "INNER JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderID] " +
                "INNER JOIN [Order].[OrderStatus] ON [OrderStatus].[OrderStatusID] = [Order].[OrderStatusId] " +
                "WHERE [Number] LIKE '%' + @q + '%' " +
                "OR [Email] LIKE @q + '%' " +
                (query.Length >= 6 ?
                    "OR [Phone] LIKE '%' + @q + '%' " +
                    "OR [StandardPhone] LIKE '%' + @q + '%'"
                : "") +
                "OR TrackNumber = @q " +
                " Order by [Order].[OrderID] desc",
                CommandType.Text, GetOrderForAutocompleteFromReader, new SqlParameter("@q", query));
            }
            else
            {
                var translitKeyboard = StringHelper.TranslitToRusKeyboard(query);

                return SQLDataAccess.ExecuteReadList<OrderAutocomplete>(
                    "SELECT [Order].[OrderID], Number, FirstName, LastName, Email, Phone, OrderDate, [Order].[Sum], StatusName " +
                    "FROM [Order].[Order] " +
                    "INNER JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderID] " +
                    "INNER JOIN [Order].[OrderStatus] ON [OrderStatus].[OrderStatusID] = [Order].[OrderStatusId] " +
                    "WHERE [Number] LIKE '%' + @q + '%' " +
                    "OR [Email] LIKE @q + '%' " +
                    "OR [FirstName] LIKE @q + '%' OR [FirstName] like @qtr + '%' " +
                    "OR [LastName] LIKE @q + '%' OR [LastName] like @qtr + '%' " +
                    "OR [Phone] LIKE '%' + @q + '%' " +
                    "Order by [Order].[OrderID] desc",
                    CommandType.Text, GetOrderForAutocompleteFromReader, new SqlParameter("@q", query), new SqlParameter("@qtr", translitKeyboard));
            }
        }

        public static List<LastOrdersItem> GetLastOrders(int count)
        {
            return SQLDataAccess.Query<LastOrdersItem>(
                "SELECT TOP(@count) [Order].[OrderId], Number, OrderDate, Sum, [OrderCustomer].[CustomerId], FirstName, LastName, Patronymic,  [OrderCurrency].*, StatusName, Color " +
                "FROM [Order].[Order] " +
                "LEFT JOIN [Order].[OrderStatus] ON [OrderStatus].[OrderStatusID] = [Order].[OrderStatusID] " +
                "LEFT JOIN [Order].[OrderCurrency] ON [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                "LEFT JOIN [Order].[OrderCustomer] ON [OrderCustomer].[OrderId] = [Order].[OrderId] " +
                "WHERE IsDraft = 0 " +
                "ORDER BY orderdate desc",
                new { count }).ToList();
        }

        public static List<LastOrdersItem> GetLastOrders(int count, int? managerId)
        {
            return SQLDataAccess.Query<LastOrdersItem>(
                    "SELECT TOP(@count) [Order].[OrderId], Number, OrderDate, Sum, [OrderCustomer].[CustomerId], FirstName, LastName, Patronymic,  [OrderCurrency].*, StatusName, Color " +
                    "FROM [Order].[Order] " +
                    "LEFT JOIN [Order].[OrderStatus] ON [OrderStatus].[OrderStatusID] = [Order].[OrderStatusID] " +
                    "LEFT JOIN [Order].[OrderCurrency] ON [OrderCurrency].[OrderId] = [Order].[OrderId] " +
                    "LEFT JOIN [Order].[OrderCustomer] ON [OrderCustomer].[OrderId] = [Order].[OrderId] " +
                    "WHERE IsDraft = 0 " + (managerId != null ? " and [ManagerId] = @ManagerId " : " and [ManagerId] IS NULL ") +
                    "ORDER BY orderdate desc",
                    new { count, managerId }).ToList();
        }

        public static List<string> GetShippingMethods()
        {
            List<string> result = SQLDataAccess.ExecuteReadList("SELECT Name FROM [Order].[ShippingMethod]", CommandType.Text, reader => SQLDataHelper.GetString(reader, "Name").Trim());
            return result;
        }

        public static List<string> GetShippingMethodNamesFromOrder()
        {
            List<string> result = SQLDataAccess.ExecuteReadList("SELECT distinct ShippingMethodName FROM [Order].[Order]", CommandType.Text, reader => SQLDataHelper.GetString(reader, "ShippingMethodName").Trim());
            return result;
        }

        public static void DeleteOrder(int orderId)
        {
            var order = GetOrder(orderId);
            if (order == null)
                return;

            var prevStatus = order.OrderStatus;
            var user = CustomerContext.CurrentCustomer;
            var history = new OrderStatusHistory()
            {
                OrderID = orderId,
                CustomerID = user.IsAdmin || user.IsManager || user.IsModerator ? user.Id : (Guid?)null,
                CustomerName = user.IsAdmin || user.IsManager || user.IsModerator ? user.FirstName + " " + user.LastName : string.Empty,
                PreviousStatus = prevStatus != null ? prevStatus.StatusName : string.Empty,
                NewStatus = "Удален"
            };

            if (SettingsCheckout.DecrementProductsCount)
            {
                IncrementProductsCountAccordingOrder(order, history);
            }

            SQLDataAccess.ExecuteNonQuery("[Order].[sp_DeleteOrder]", CommandType.StoredProcedure, new SqlParameter("@OrderID", orderId));

            if (Settings1C.Enabled)
                SQLDataAccess.ExecuteNonQuery(
                    "Insert Into [Order].[DeletedOrders] ([OrderId],[DateTime]) Values (@OrderId, Getdate())", CommandType.Text,
                    new SqlParameter("@OrderId", orderId));

            OrderStatusService.AddOrderStatusHistory(history);
            OrderHistoryService.DeleteOrder(order, null);

            if (BonusSystem.IsActive)
            {
                BonusSystemService.CancelPurchase(order.BonusCardNumber, order.Number, orderId);
            }

            ModulesExecuter.OrderDeleted(orderId);
            ModulesExecuter.SendNotificationsOnOrderDeleted(orderId);

            TriggerDeferredDataService.Delete(ETriggerObjectType.Order, orderId);
        }

        public static int AddOrder(Order order, OrderChangedBy changedBy = null, bool trackChanges = true)
        {
            return AddOrder(order, changedBy, trackChanges, true);
        }

        public static int AddOrder(Order order, OrderChangedBy changedBy, bool trackChanges, bool autocompleteLeads)
        {
            AddOrderMain(order);
            if (order.OrderID != 0)
            {
                order.OrderCustomer.OrderID = order.OrderID;
                AddOrderCustomer(order.OrderID, order.OrderCustomer);

                AddOrderCurrency(order.OrderID, order.OrderCurrency);

                if (order.PaymentDetails != null)
                    AddPaymentDetails(order.OrderID, order.PaymentDetails);

                if (order.OrderPickPoint != null)
                {
                    order.OrderPickPoint.OrderId = order.OrderID;
                    AddUpdateOrderPickPoint(order.OrderID, order.OrderPickPoint);
                }

                if (order.OrderItems != null)
                    foreach (var row in order.OrderItems)
                    {
                        row.OrderID = order.OrderID;
                        AddUpdateOrderedItem(order, row, ignoreHistory: true);
                    }

                if (order.OrderCertificates != null)
                    foreach (var certificate in order.OrderCertificates)
                    {
                        certificate.OrderId = order.OrderID;
                        GiftCertificateService.AddCertificate(certificate);
                    }

                RefreshTotal(order, updateModules: false);
            }

            order.Number = GenerateNumber(order.OrderID);
            UpdateNumber(order.OrderID, order.Number);

            if (trackChanges)
                OrderHistoryService.NewOrder(order, changedBy);

            ModulesExecuter.OrderAdded(order);

            if (!order.IsDraft)
            {
                ModulesExecuter.SendNotificationsOnOrderAdded(order);
                SmsNotifier.SendSmsOnOrderAdded(order);
                BizProcessExecuter.OrderAdded(order);
                Core.Services.Api.ApiWebhookExecuter.OrderAdded(order);

                TriggerProcessService.ProcessEvent(ETriggerEventType.OrderCreated, order);
                if (autocompleteLeads)
                    LeadService.CompleteLeadsOnNewOrder(order);
                PartnerService.OnOrderAdded(order);
            }

            var loger = LogingManager.GetTrafficSourceLoger();
            loger.LogOrderTafficSource(order.OrderID, TrafficSourceType.Order, order.IsFromAdminArea);

            return order.OrderID;
        }

        private static void AddOrderCurrency(int orderId, OrderCurrency orderCurrency)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO [Order].[OrderCurrency] (OrderID, CurrencyCode, CurrencyNumCode, CurrencyValue, CurrencySymbol, IsCodeBefore, RoundNumbers, EnablePriceRounding) VALUES (@OrderID, @CurrencyCode, @CurrencyNumCode, @CurrencyValue, @CurrencySymbol, @IsCodeBefore, @RoundNumbers, @EnablePriceRounding)",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@CurrencyCode", orderCurrency.CurrencyCode),
                new SqlParameter("@CurrencyNumCode", orderCurrency.CurrencyNumCode),
                new SqlParameter("@CurrencyValue", orderCurrency.CurrencyValue),
                new SqlParameter("@CurrencySymbol", orderCurrency.CurrencySymbol),
                new SqlParameter("@IsCodeBefore", orderCurrency.IsCodeBefore),
                new SqlParameter("@EnablePriceRounding", orderCurrency.EnablePriceRounding),
                new SqlParameter("@RoundNumbers", orderCurrency.RoundNumbers));
        }

        public static void AddUpdateOrderPickPoint(int orderId, OrderPickPoint pickPoint)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"if (select count(orderid) from [Order].[OrderPickPoint] where orderid=@orderid) = 0
                begin
                    INSERT INTO [Order].[OrderPickPoint] (OrderID, PickPointId, PickPointAddress, AdditionalData) VALUES (@OrderID, @PickPointId, @PickPointAddress, @AdditionalData)
                end
                else
                begin
                    Update [Order].[OrderPickPoint] set PickPointId=@PickPointId, PickPointAddress=@PickPointAddress, AdditionalData=@AdditionalData Where OrderID=@OrderID
                end",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@PickPointId", pickPoint.PickPointId ?? string.Empty),
                new SqlParameter("@PickPointAddress", pickPoint.PickPointAddress ?? string.Empty),
                new SqlParameter("@AdditionalData", pickPoint.AdditionalData ?? string.Empty));
        }

        public static void DeleteOrderPickPoint(int orderID)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Order].[OrderPickPoint] where OrderID= @OrderID",
               CommandType.Text,
               new SqlParameter("@OrderID", orderID));
        }

        public static void AddUpdateOrderAdditionalData(int orderId, string key, string value)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT 1 FROM [Order].[OrderAdditionalData] WHERE [OrderID] = @OrderID AND [Name] = @Name)
                begin
                    INSERT INTO [Order].[OrderAdditionalData] ([OrderID],[Name],[Value]) VALUES (@OrderID, @Name, @Value)
                end
                else
                begin
                    Update [Order].[OrderAdditionalData] set [Value]=@Value Where [OrderID] = @OrderID AND [Name] = @Name
                end",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@Name", key),
                new SqlParameter("@Value", value ?? (object)DBNull.Value));
        }

        public static Dictionary<string, string> GetOrderAdditionalData(int orderId)
        {
            return SQLDataAccess.ExecuteReadDictionary<string, string>(
                "SELECT [Name], [Value] FROM [Order].[OrderAdditionalData] WHERE [OrderID] = @OrderID",
                CommandType.Text,
                "Name",
                "Value",
                new SqlParameter("@OrderID", orderId));
        }

        public static string GetOrderAdditionalData(int orderId, string key)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT [Value] FROM [Order].[OrderAdditionalData] WHERE [OrderID] = @OrderID AND [Name] = @Name",
                CommandType.Text,
                reader =>
                    SQLDataHelper.GetString(reader, "Value"),
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@Name", key));
        }

        public static void DeleteOrderAdditionalData(int orderId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Order].[OrderAdditionalData] where OrderID= @OrderID",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId));
        }

        public static void DeleteOrderAdditionalData(int orderId, string key)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete from [Order].[OrderAdditionalData] where [OrderID] = @OrderID AND [Name] = @Name",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@Name", key));
        }

        public static void AddOrderCustomer(int orderId, OrderCustomer customer)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO [Order].[OrderCustomer] ([OrderId],[CustomerID],[CustomerIP],[FirstName],[LastName],[Patronymic],[Email],[Phone],[StandardPhone],Country,Region,District,City,Zip,CustomField1,CustomField2,CustomField3,Street,House,Apartment,Structure,Entrance,Floor,Organization) " +
                " VALUES (@OrderId,@CustomerID,@CustomerIP,@FirstName,@LastName, @Patronymic,@Email,@Phone,@StandardPhone,@Country,@Region,@District,@City,@Zip,@CustomField1,@CustomField2,@CustomField3,@Street,@House,@Apartment,@Structure,@Entrance,@Floor,@Organization)",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@CustomerID", customer.CustomerID),
                new SqlParameter("@CustomerIP", customer.CustomerIP ?? string.Empty),
                new SqlParameter("@FirstName", customer.FirstName ?? string.Empty),
                new SqlParameter("@LastName", customer.LastName ?? string.Empty),
                new SqlParameter("@Patronymic", customer.Patronymic ?? string.Empty),
                new SqlParameter("@Email", customer.Email ?? string.Empty),
                new SqlParameter("@Phone", customer.Phone ?? string.Empty),
                new SqlParameter("@StandardPhone", customer.StandardPhone ?? (object)DBNull.Value),

                new SqlParameter("@Country", customer.Country ?? string.Empty),
                new SqlParameter("@Region", customer.Region ?? string.Empty),
                new SqlParameter("@District", customer.District ?? string.Empty),
                new SqlParameter("@City", customer.City ?? string.Empty),
                new SqlParameter("@Zip", customer.Zip ?? string.Empty),
                new SqlParameter("@CustomField1", customer.CustomField1 ?? string.Empty),
                new SqlParameter("@CustomField2", customer.CustomField2 ?? string.Empty),
                new SqlParameter("@CustomField3", customer.CustomField3 ?? string.Empty),

                new SqlParameter("@Street", customer.Street ?? string.Empty),
                new SqlParameter("@House", customer.House ?? string.Empty),
                new SqlParameter("@Apartment", customer.Apartment ?? string.Empty),
                new SqlParameter("@Structure", customer.Structure ?? string.Empty),
                new SqlParameter("@Entrance", customer.Entrance ?? string.Empty),
                new SqlParameter("@Floor", customer.Floor ?? string.Empty),
                new SqlParameter("@Organization", customer.Organization ?? (object)DBNull.Value)
                );

            var modules = AttachedModules.GetModules<ISendMails>();
            foreach (var moduleType in modules)
            {
                var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                moduleObject.SubscribeEmail(new Subscription
                {
                    Email = customer.Email,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Phone = customer.Phone,
                    CustomerType = EMailRecipientType.OrderCustomer
                });
            }
        }

        public static void CreateCustomerByOrderCustomer(OrderCustomer orderCustomer, string couponCode = null)
        {
            var customer = CustomerService.GetCustomer(orderCustomer.CustomerID);
            if (customer != null)
                return;

            if (!string.IsNullOrWhiteSpace(orderCustomer.Email))
                customer = CustomerService.GetCustomerByEmail(orderCustomer.Email);

            if (customer != null)
                return;

            var c = (Customer)orderCustomer;

            var id = CustomerService.InsertNewCustomer(c);
            if (id != Guid.Empty)
            {
                if (c.Contacts != null && c.Contacts.Count > 0)
                    CustomerService.AddContact(c.Contacts[0], c.Id);

                Partner partner;
                if (!string.IsNullOrEmpty(couponCode) &&
                    (partner = PartnerService.GetPartnerByCoupon(couponCode)) != null && partner.Enabled)
                {
                    PartnerService.AddBindedCustomer(new BindedCustomer
                    {
                        CustomerId = c.Id,
                        PartnerId = partner.Id,
                        CouponCode = couponCode
                    });
                }
            }
        }

        public static void UpdateOrderCustomer(OrderCustomer customer, OrderChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
                OrderHistoryService.ChangingCustomer(customer, changedBy);

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[OrderCustomer] " +
                "SET [CustomerID] = @CustomerID, [FirstName] = @FirstName, LastName=@LastName, Patronymic=@Patronymic, Email=@Email, Phone=@Phone, StandardPhone=@StandardPhone, " +
                "Country = @Country, Region = @Region, District = @District, City = @City, Zip = @Zip, CustomField1 = @CustomField1, CustomField2 = @CustomField2, CustomField3 = @CustomField3, " +
                "Street = @Street, House = @House, Apartment = @Apartment, Structure = @Structure, Entrance = @Entrance, Floor = @Floor, Organization=@Organization " +
                "WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@OrderID", customer.OrderID),
                new SqlParameter("@CustomerID", customer.CustomerID),
                new SqlParameter("@FirstName", customer.FirstName ?? string.Empty),
                new SqlParameter("@LastName", customer.LastName ?? string.Empty),
                new SqlParameter("@Patronymic", customer.Patronymic ?? string.Empty),
                new SqlParameter("@Email", customer.Email ?? string.Empty),
                new SqlParameter("@Phone", customer.Phone ?? string.Empty),
                new SqlParameter("@StandardPhone", customer.StandardPhone ?? (object)DBNull.Value),

                new SqlParameter("@Country", customer.Country ?? string.Empty),
                new SqlParameter("@Region", customer.Region ?? string.Empty),
                new SqlParameter("@District", customer.District ?? string.Empty),
                new SqlParameter("@City", customer.City ?? string.Empty),
                new SqlParameter("@Zip", customer.Zip ?? string.Empty),
                new SqlParameter("@CustomField1", customer.CustomField1 ?? string.Empty),
                new SqlParameter("@CustomField2", customer.CustomField2 ?? string.Empty),
                new SqlParameter("@CustomField3", customer.CustomField3 ?? string.Empty),

                new SqlParameter("@Street", customer.Street ?? string.Empty),
                new SqlParameter("@House", customer.House ?? string.Empty),
                new SqlParameter("@Apartment", customer.Apartment ?? string.Empty),
                new SqlParameter("@Structure", customer.Structure ?? string.Empty),
                new SqlParameter("@Entrance", customer.Entrance ?? string.Empty),
                new SqlParameter("@Floor", customer.Floor ?? string.Empty),
                new SqlParameter("@Organization", customer.Organization ?? (object)DBNull.Value)
                );
        }

        private static void AddOrderMain(Order ord)
        {
            if (ord.Code == Guid.Empty)
                ord.Code = Guid.NewGuid();

            ord.OrderDate = ord.OrderDate.AddTicks(-(ord.OrderDate.Ticks % TimeSpan.TicksPerSecond));
            ord.GroupName = ord.GroupName ?? (CustomerGroupService.GetCustomerGroup() != null
                                ? CustomerGroupService.GetCustomerGroup().GroupName
                                : "");

            ord.OrderID = SQLDataAccess.ExecuteScalar<int>(
                        "INSERT INTO [Order].[Order] " +
                            "([Number], [ShippingMethodID], [PaymentMethodID], [AffiliateID], " +
                             "[OrderDate], [PaymentDate], [CustomerComment], [StatusComment], " +
                             "[AdditionalTechInfo],[AdminOrderComment], [ShippingCost],[PaymentCost], [OrderStatusID], " +
                             "[ShippingMethodName],[PaymentMethodName], [GroupName], [GroupDiscount], [OrderDiscount], " +
                             "[CertificateCode], [CertificatePrice], [CouponCode], [CouponType], [CouponValue], [BonusCost], [BonusCardNumber], " +
                             "[ManagerId],  [UseIn1C], [ModifiedDate], [Code], [ManagerConfirmed], [OrderSourceId], CustomData, IsDraft, " +
                             "DeliveryDate, DeliveryTime, TrackNumber, IsFromAdminArea, OrderDiscountValue, LeadId, ShippingTaxType, LpId," +
                             "TotalWeight, TotalLength, TotalWidth, TotalHeight, AvailablePaymentCashOnDelivery, AvailablePaymentPickPoint," +
                             "ShippingPaymentMethodType, ShippingPaymentSubjectType) " +
                        "VALUES " +
                            "(@Number, @ShippingMethodID, @PaymentMethodID, @AffiliateID, " +
                             "@OrderDate, null, @CustomerComment, @StatusComment, " +
                             "@AdditionalTechInfo, @AdminOrderComment, @ShippingCost,@PaymentCost, @OrderStatusID, " +
                             "@ShippingMethodName, @PaymentMethodName, @GroupName, @GroupDiscount,@OrderDiscount, " +
                             "@CertificateCode, @CertificatePrice, @CouponCode, @CouponType, @CouponValue, @BonusCost, @BonusCardNumber, " +
                             "@ManagerId,  @UseIn1C, Getdate(), @Code, @ManagerConfirmed, @OrderSourceId, @CustomData, @IsDraft, " +
                             "@DeliveryDate, @DeliveryTime, @TrackNumber, @IsFromAdminArea, @OrderDiscountValue, @LeadId, @ShippingTaxType, @LpId," +
                             "@TotalWeight, @TotalLength, @TotalWidth, @TotalHeight, @AvailablePaymentCashOnDelivery, @AvailablePaymentPickPoint," +
                             "@ShippingPaymentMethodType, @ShippingPaymentSubjectType); " +
                        "SELECT scope_identity();",
                CommandType.Text,

                new SqlParameter("@Number", ord.Number ?? string.Empty),
                new SqlParameter("@ShippingMethodID", ord.ShippingMethodId != 0 ? ord.ShippingMethodId : (object)DBNull.Value),
                new SqlParameter("@PaymentMethodID", ord.PaymentMethodId != 0 ? ord.PaymentMethodId : (object)DBNull.Value),
                new SqlParameter("@ShippingMethodName", ord.ArchivedShippingName ?? string.Empty),
                new SqlParameter("@ShippingTaxType", (int)ord.ShippingTaxType),
                new SqlParameter("@PaymentMethodName", ord.PaymentMethodName ?? string.Empty),
                new SqlParameter("@OrderStatusID", ord.OrderStatusId),
                new SqlParameter("@AffiliateID", ord.AffiliateID),
                new SqlParameter("@ShippingCost", ord.ShippingCost),
                new SqlParameter("@PaymentCost", ord.PaymentCost),
                new SqlParameter("@OrderDate", ord.OrderDate),
                new SqlParameter("@CustomerComment", ord.CustomerComment ?? string.Empty),
                new SqlParameter("@StatusComment", ord.StatusComment ?? string.Empty),
                new SqlParameter("@AdditionalTechInfo", ord.AdditionalTechInfo ?? string.Empty),
                new SqlParameter("@AdminOrderComment", ord.AdminOrderComment ?? string.Empty),
                new SqlParameter("@GroupName", ord.GroupName),
                new SqlParameter("@GroupDiscount", ord.GroupDiscount),
                new SqlParameter("@OrderDiscount", ord.OrderDiscount),
                new SqlParameter("@OrderDiscountValue", ord.OrderDiscountValue),
                new SqlParameter("@CertificatePrice", ord.Certificate != null ? (object)ord.Certificate.Price : DBNull.Value),
                new SqlParameter("@CertificateCode", ord.Certificate != null ? (object)ord.Certificate.Code : DBNull.Value),
                new SqlParameter("@CouponCode", ord.Coupon != null ? (object)ord.Coupon.Code : DBNull.Value),
                new SqlParameter("@CouponType", ord.Coupon != null ? (object)ord.Coupon.Type : DBNull.Value),
                new SqlParameter("@CouponValue", ord.Coupon != null ? (object)ord.Coupon.Value : DBNull.Value),
                new SqlParameter("@BonusCost", ord.BonusCost),
                new SqlParameter("@BonusCardNumber", ord.BonusCardNumber ?? (object)DBNull.Value),
                new SqlParameter("@ManagerId", ord.ManagerId ?? (object)DBNull.Value),
                new SqlParameter("@UseIn1C", ord.UseIn1C),
                new SqlParameter("@Code", ord.Code),
                new SqlParameter("@ManagerConfirmed", ord.ManagerConfirmed),
                new SqlParameter("@OrderSourceId", ord.OrderSourceId),
                new SqlParameter("@CustomData", ord.CustomData ?? (object)DBNull.Value),
                new SqlParameter("@IsDraft", ord.IsDraft),
                new SqlParameter("@DeliveryDate", ord.DeliveryDate ?? (object)DBNull.Value),
                new SqlParameter("@DeliveryTime", ord.DeliveryTime ?? string.Empty),
                new SqlParameter("@TrackNumber", ord.TrackNumber ?? (object)DBNull.Value),
                new SqlParameter("@IsFromAdminArea", ord.IsFromAdminArea),
                new SqlParameter("@LeadId", ord.LeadId ?? (object)DBNull.Value),
                new SqlParameter("@LpId", ord.LpId ?? (object)DBNull.Value),
                new SqlParameter("@TotalWeight", ord.TotalWeight ?? (object)DBNull.Value),
                new SqlParameter("@TotalLength", ord.TotalLength ?? (object)DBNull.Value),
                new SqlParameter("@TotalWidth", ord.TotalWidth ?? (object)DBNull.Value),
                new SqlParameter("@TotalHeight", ord.TotalHeight ?? (object)DBNull.Value),
                new SqlParameter("@AvailablePaymentCashOnDelivery", ord.AvailablePaymentCashOnDelivery),
                new SqlParameter("@AvailablePaymentPickPoint", ord.AvailablePaymentPickPoint),
                new SqlParameter("@ShippingPaymentMethodType", (int)ord.ShippingPaymentMethodType),
                new SqlParameter("@ShippingPaymentSubjectType", (int)ord.ShippingPaymentSubjectType)
            );
        }

        public static void AddPaymentDetails(int orderId, PaymentDetails details)
        {
            if (details != null)
                SQLDataAccess.ExecuteNonQuery("[Order].[sp_AddPaymentDetails]", CommandType.StoredProcedure,
                                                new SqlParameter("@OrderID", orderId),
                                                new SqlParameter("@CompanyName", details.CompanyName ?? string.Empty),
                                                new SqlParameter("@INN", details.INN ?? string.Empty),
                                                new SqlParameter("@Phone", details.Phone ?? string.Empty),
                                                new SqlParameter("@Contract", details.Contract ?? string.Empty),
                                                new SqlParameter("@IsCashOnDeliveryPayment", details.IsCashOnDeliveryPayment),
                                                new SqlParameter("@IsPickPointPayment", details.IsPickPointPayment));
        }

        public static void UpdatePaymentDetails(int orderId, PaymentDetails details, OrderChangedBy changedBy = null, bool trackChanges = true)
        {
            if (details == null)
                return;

            if (trackChanges)
                OrderHistoryService.ChangingPaymentDetails(orderId, details, changedBy);

            SQLDataAccess.ExecuteNonQuery(
                "if(EXISTS(select orderid from [Order].[PaymentDetails] where OrderID=@OrderID)) " +
                " Update [Order].[PaymentDetails] Set CompanyName=@CompanyName, INN=@INN, phone=@phone, Contract=@Contract, IsCashOnDeliveryPayment=@IsCashOnDeliveryPayment, IsPickPointPayment=@IsPickPointPayment Where OrderID=@OrderID" +
                " else " +
                " insert into [Order].[PaymentDetails] (OrderID, CompanyName,  INN, phone, Contract,[IsCashOnDeliveryPayment],[IsPickPointPayment]) values(@OrderID, @CompanyName, @INN, @phone, @Contract,@IsCashOnDeliveryPayment,@IsPickPointPayment) ", CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@CompanyName", details.CompanyName ?? string.Empty),
                new SqlParameter("@INN", details.INN ?? string.Empty),
                new SqlParameter("@Phone", details.Phone ?? string.Empty),
                new SqlParameter("@Contract", details.Contract ?? string.Empty),
                new SqlParameter("@IsCashOnDeliveryPayment", details.IsCashOnDeliveryPayment),
                new SqlParameter("@IsPickPointPayment", details.IsPickPointPayment));
        }

        public static void UpdateNumber(int id, string number)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateOrderNumber]", CommandType.StoredProcedure, new SqlParameter("@OrderID", id), new SqlParameter("@Number", number));
        }

        public static void UpdateAdminOrderComment(int id, string adminOrderComment, OrderChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
                OrderHistoryService.ChangingAdminComment(id, adminOrderComment, changedBy);

            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateOrderAdminOrderComment]", CommandType.StoredProcedure, new SqlParameter("@OrderID", id),
                                        new SqlParameter("@AdminOrderComment", adminOrderComment ?? string.Empty));

            ModulesExecuter.UpdateComments(id);
        }

        public static void UpdateCustomerComment(int orderid, string customerComment)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Order].[Order] set CustomerComment=@customerComment where OrderId=@OrderID", CommandType.Text, new SqlParameter("@OrderID", orderid),
                new SqlParameter("@customerComment", customerComment ?? string.Empty));

            ModulesExecuter.UpdateComments(orderid);
        }

        public static void UpdateStatusComment(int id, string statusComment, OrderChangedBy changedBy = null, bool trackChanges = true)
        {
            var order = GetOrder(id);

            if (order == null)
                throw new Exception("Order is null");

            var prevComment = order.StatusComment;

            if (string.Equals(prevComment, statusComment.DefaultOrEmpty()))
                return;

            if (trackChanges)
                OrderHistoryService.ChangingStatusComment(id, statusComment, changedBy);

            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateOrderStatusComment]", CommandType.StoredProcedure,
                                            new SqlParameter("@OrderID", id), new SqlParameter("@StatusComment", statusComment ?? string.Empty));
        }

        public static StatusInfo GetStatusInfo(string orderNum)
        {
            return SQLDataAccess.ExecuteReadOne(
                "Select os.StatusName, os.Hidden, o.StatusComment, o.PreviousStatus " +
                "From [Order].[Order] o " +
                "Left Join [Order].OrderStatus os On o.OrderStatusId = os.OrderStatusId " +
                "Where o.Number = @Number",
                CommandType.Text,
                reader => new StatusInfo()
                {
                    Status = SQLDataHelper.GetString(reader, "StatusName"),
                    Comment = SQLDataHelper.GetString(reader, "StatusComment"),
                    Hidden = SQLDataHelper.GetBoolean(reader, "Hidden"),
                    PreviousStatus = SQLDataHelper.GetString(reader, "PreviousStatus")
                },
                new SqlParameter("@Number", orderNum));
        }

        public static void PayOrder(int orderId, bool pay, bool updateModules = true, bool trackChanges = true, OrderChangedBy changedBy = null)
        {
            var order = GetOrder(orderId);
            if (order == null)
                throw new Exception("Can't pay empty order");

            if (pay && order.Payed || !pay && !order.Payed)
                return;

            order.PaymentDate = pay ? DateTime.Now : default(DateTime?);

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[Order] SET [PaymentDate] = @PaymentDate, ModifiedDate = Getdate() WHERE [OrderID] = @OrderID", CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@PaymentDate", order.PaymentDate ?? (object)DBNull.Value));

            var mail = new PayOrderTemplate(order, pay);

            MailService.SendMailNow(SettingsMail.EmailForOrders, mail);

            if (trackChanges)
                OrderHistoryService.ChangingPayDate(order.OrderID, pay, changedBy);

            if (pay)
            {
                foreach (var certificate in GiftCertificateService.GetOrderCertificates(orderId))
                {
                    GiftCertificateService.SendCertificateMails(certificate);
                }

                if (BonusSystem.IsActive)
                    BonusSystemService.Confirm(order.BonusCardNumber, order.Number, orderId);

                TriggerProcessService.ProcessEvent(ETriggerEventType.OrderPaied, order);
                LeadService.CompleteLeadsOnOrderPaid(order);
                BookingService.OnOrderPaid(order,
                    changedBy != null
                        ? new ChangedBy(changedBy.Name) { CustomerId = changedBy.CustomerId }
                        : null,
                    trackChanges);
                PartnerService.ProcessOrderReward(order);
            }

            Core.Services.Api.ApiWebhookExecuter.OrderPaymentStatusChanged(order);

            CacheManager.RemoveByPattern(CacheNames.SQLPagingCount);
            CacheManager.RemoveByPattern(CacheNames.SQLPagingItems);
            
            if (updateModules)
            {
                ModulesExecuter.PayOrder(orderId, pay);
                ModulesExecuter.SendNotificationsOnPayOrder(orderId, pay);
            }
        }

        public static bool ManagerConfirmOrder(int orderId, bool confirm)
        {
            var order = GetOrder(orderId);
            if (order == null)
                throw new Exception("Can't pay empty order");

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[Order] SET [ManagerConfirmed] = @ManagerConfirmed, ModifiedDate = Getdate() WHERE [OrderID] = @OrderID", CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@ManagerConfirmed", confirm));

            return true;
        }

        public static bool ManagerConfirmOrders(bool confirm)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[Order] SET [ManagerConfirmed] = @ManagerConfirmed, ModifiedDate = Getdate()",
                CommandType.Text,
                new SqlParameter("@ManagerConfirmed", confirm));

            return true;
        }

        public static void SetSendedToGA(int orderId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[Order] SET [IsSendedToGA] = @IsSendedToGA, ModifiedDate = GETDATE() WHERE [OrderID] = @OrderID", CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@IsSendedToGA", true));
        }

        public static string GeneratePayCode(int orderId)
        {
            var code = "";
            while (string.IsNullOrEmpty(code) || GetOrderIdByPayCode(code) != 0)
            {
                code = Strings.GetRandomUrlString(10);
            }
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[Order] SET [PayCode] = @PayCode, ModifiedDate = GETDATE() WHERE [OrderID] = @OrderID", CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@PayCode", code));
            return code;
        }

        public static PaymentDetails GetPaymentDetails(int orderId)
        {
            return SQLDataAccess.ExecuteReadOne("[Order].[sp_GetPaymentDetails]", CommandType.StoredProcedure,
                reader => new PaymentDetails
                {
                    CompanyName = SQLDataHelper.GetString(reader, "CompanyName"),
                    INN = SQLDataHelper.GetString(reader, "INN"),
                    Phone = SQLDataHelper.GetString(reader, "Phone"),
                    Contract = SQLDataHelper.GetString(reader, "Contract"),
                    IsCashOnDeliveryPayment = SQLDataHelper.GetBoolean(reader, "IsCashOnDeliveryPayment"),
                    IsPickPointPayment = SQLDataHelper.GetBoolean(reader, "IsPickPointPayment")
                },
                new SqlParameter("@OrderID", orderId));
        }

        public static Order GetOrder(int orderId)
        {
            return SQLDataAccess.ExecuteReadOne<Order>(
                "SELECT * FROM [Order].[Order] WHERE [OrderID] = @OrderID",
                CommandType.Text,
                GetOrderFromReader,
                new SqlParameter("@OrderID", orderId));
        }

        public static int GetOrderIdByNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return 0;

            return SQLDataAccess.ExecuteScalar<int>("[Order].[sp_GetOrderIdByNumber]", CommandType.StoredProcedure,
                new SqlParameter("@Number", number));
        }

        public static int GetOrderIdByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return 0;

            return SQLDataAccess.ExecuteScalar<int>("SELECT [OrderID] FROM [Order].[Order] WHERE [Code] = @Code",
                CommandType.Text, new SqlParameter("@Code", code));
        }

        public static int GetOrderIdByPayCode(string payCode)
        {
            if (string.IsNullOrWhiteSpace(payCode))
                return 0;

            return SQLDataAccess.ExecuteScalar<int>("SELECT [OrderID] FROM [Order].[Order] WHERE [PayCode] = @PayCode",
                CommandType.Text, new SqlParameter("@PayCode", payCode));
        }

        public static int GetOrderIdByLeadId(int leadId)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT top(1) [OrderID] FROM [Order].[Order] WHERE [LeadId] = @leadId",
                CommandType.Text, new SqlParameter("@leadId", leadId));
        }

        public static string GetOrderNumberById(int orderId)
        {
            return SQLDataAccess.ExecuteScalar<string>(
             "SELECT [Number] FROM [Order].[Order] WHERE [OrderId] = @OrderId",
             CommandType.Text,
             new SqlParameter("@OrderId", orderId));
        }

        public static int GetCountOrder(string number)
        {
            int retCount = 0;

            if (!string.IsNullOrEmpty(number))
            {
                retCount = SQLDataAccess.ExecuteScalar<int>("[Order].[sp_GetCountOrderByNumber]", CommandType.StoredProcedure, new SqlParameter("@Number", number));
            }

            return retCount;
        }

        public static string GenerateNumber(int orderId)
        {
            var currentNumber = GetOrderNumberById(orderId);

            if (currentNumber.IsNotEmpty())
                return currentNumber;

            string format = null;
            int iterationNumber = 0;

            do
            {
                format = SettingsCheckout.OrderNumberFormat;

                if (string.IsNullOrWhiteSpace(format))
                    return orderId.ToString();

                if (format.Contains("#NUMBER#"))
                    format = format.Replace("#NUMBER#", orderId.ToString());

                if (format.Contains("#YEAR#") || format.Contains("#MONTH#") || format.Contains("#DAY#"))
                {
                    var now = DateTime.Now;
                    format =
                        format.Replace("#YEAR#", now.ToString("yy"))
                              .Replace("#MONTH#", now.ToString("MM"))
                              .Replace("#DAY#", now.ToString("dd"));
                }

                format = CreateRandomForFormat(format);
            } while (GetCountOrder(format) != 0 && ++iterationNumber < 10);

            return format;
        }

        private static string CreateRandomForFormat(string format)
        {
            const int maxRandomLength = 9;//для int максимум можем возвести в 9 степень
            string formatMaxRandom = new String('0', maxRandomLength);

            var random = new Random();
            var startStr = "#R";
            var endStr = "R#";
            var currentIndex = 0;
            int startStrIndex, endStrIndex;
            var formatsReplace = new List<Tuple<int, int, string>>();

            while (currentIndex < format.Length - 1 &&
                   (startStrIndex = format.IndexOf(startStr, currentIndex, StringComparison.Ordinal)) > -1 &&
                   (endStrIndex = format.IndexOf(endStr, startStrIndex, StringComparison.Ordinal)) > -1)
            {
                var countR = endStrIndex - startStrIndex;
                var valid = format
                    .Substring(startStrIndex + 1, countR)
                    .All(c => c == 'R');

                if (valid)
                {
                    var randomValue = "";

                    int counRandom = countR / maxRandomLength;
                    for (int i = 0; i < counRandom; i++)
                        randomValue += random.Next(0, (int)Math.Pow(10, maxRandomLength)).ToString(formatMaxRandom);

                    var remainder = countR % maxRandomLength;
                    if (remainder > 0)
                        randomValue += random.Next(0, (int)Math.Pow(10, remainder)).ToString(new String('0', remainder));

                    formatsReplace.Add(
                        new Tuple<int, int, string>(startStrIndex, endStrIndex + endStr.Length - 1, randomValue));

                    currentIndex = endStrIndex + endStr.Length;
                }
                else
                {
                    currentIndex = startStrIndex + 1;
                }
            }

            // Переворазиваем замену, чтобы заменять с конца (При замене длина строки будем меняться)
            formatsReplace.Reverse();
            foreach (var tupleReplace in formatsReplace)
                format = format
                    .Remove(tupleReplace.Item1, tupleReplace.Item2 - tupleReplace.Item1 + 1)
                    .Insert(tupleReplace.Item1, tupleReplace.Item3);

            return format;
        }

        public static string SerializeToXml(List<Order> orders, bool isAdvanced = false)
        {
            using (var ms = new MemoryStream())
            using (var writer = XmlWriter.Create(ms, new XmlWriterSettings { Encoding = Encoding.Unicode, Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Orders");
                foreach (var order in orders)
                {
                    SerializeToXml(order, writer, isAdvanced);
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                return Encoding.Unicode.GetString(ms.ToArray());
            }
        }

        public static void SerializeToXml(Order order)
        {
            SerializeToXml(new List<Order> { order });
        }

        private static void SerializeToXml(Order order, XmlWriter writer, bool isAdvanced = false)
        {
            var customer = order.OrderCustomer;
            var currency = order.OrderCurrency;

            if (currency == null)
            {
                Debug.Log.Error("Order SerializeToXml currency is null");
                return;
            }

            var totalDiscount = order.TotalDiscount;
            var currencyValue = order.OrderCurrency.CurrencyValue;

            writer.WriteStartElement("Order");
            writer.WriteAttributeString("OrderID", order.OrderID.ToString());
            writer.WriteAttributeString("Number", order.Number);
            writer.WriteAttributeString("OrderStatusId", order.OrderStatusId.ToString());
            writer.WriteAttributeString("OrderStatus", order.OrderStatus != null ? order.OrderStatus.StatusName : string.Empty);
            writer.WriteAttributeString("OrderStatusComment", order.StatusComment);
            writer.WriteAttributeString("CustomerEmail",
                customer != null && customer.Email.IsNotEmpty() ? customer.Email : string.Empty);
            writer.WriteAttributeString("OfferType", "Obsolete");
            writer.WriteAttributeString("Date", order.OrderDate.ToString());
            writer.WriteAttributeString("IsPaid", order.PaymentDate != null ? "1" : "0");
            writer.WriteAttributeString("PaymentDate", order.PaymentDate != null ? order.PaymentDate.ToString() : string.Empty);
            writer.WriteAttributeString("DeliveryDate", order.DeliveryDate != null ? order.DeliveryDate.Value.ToShortDateString() : string.Empty);
            writer.WriteAttributeString("DeliveryTime", order.DeliveryTime ?? string.Empty);

            writer.WriteAttributeString("Comments", order.CustomerComment);
            writer.WriteAttributeString("AdminComments", order.AdminOrderComment);
            writer.WriteAttributeString("DiscountPercent", (order.OrderDiscount * currencyValue).ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("DiscountValue", (totalDiscount * currencyValue + order.OrderDiscountValue).ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("ShippingCost", (order.ShippingCost * currencyValue).ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CustomerIP", customer != null ? order.OrderCustomer.CustomerIP : string.Empty);
            writer.WriteAttributeString("ShippingMethod", order.ArchivedShippingName);
            writer.WriteAttributeString("BillingMethod", order.PaymentMethodName);
            writer.WriteAttributeString("OrderSourceId", order.OrderSourceId.ToString());
            writer.WriteAttributeString("OrderSource", order.OrderSource != null ? order.OrderSource.Name : string.Empty);

            if (isAdvanced)
            {
                writer.WriteAttributeString("PaymentCost", (order.PaymentCost * currencyValue).ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("BonusCost", (order.BonusCost * currencyValue).ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("TaxCost",
                    (order.Taxes != null ? (order.Taxes.Sum(tax => tax.Sum) ?? 0f) * currencyValue : 0).ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("TaxInPrice",
                    (order.Taxes != null && order.Taxes.Count > 0
                        ? Convert.ToInt32(order.Taxes.FirstOrDefault().ShowInPrice)
                        : 0).ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("ModifiedDate", order.ModifiedDate.ToString());
            }

            var orderItemsCount = order.OrderItems.Count;
            //var discountOnProduct = (int)(totalDiscount / orderItemsCount);
            var bonusOnProduct = (int)(order.BonusCost / orderItemsCount);
            var paymentCostOnProduct = (int)(order.PaymentCost / orderItemsCount);
            var discountSum = 0f;
            var bonusSum = 0;
            var paymentSum = 0;
            var count = 0;

            var orderItemsSum = order.OrderItems.Sum(x => x.Amount * x.Price);

            if (order.OrderItems.Count != 0)
            {
                writer.WriteStartElement("Products");
                foreach (OrderItem item in order.OrderItems)
                {
                    writer.WriteStartElement("Product");
                    writer.WriteAttributeString("ID", item.ArtNo ?? item.ProductID.ToString());
                    writer.WriteAttributeString("Name",
                        string.Format("{0}{1}{2}",
                            item.Name,
                            item.SelectedOptions.Count > 0 ? " | +" : string.Empty,
                            string.Join(" | +",
                                item.SelectedOptions.Select(x => string.Format("{0}{1}{2}",
                                    x.CustomOptionTitle,
                                    x.OptionTitle.IsNotEmpty() ? ":" : string.Empty,
                                    x.OptionTitle)))));
                    writer.WriteAttributeString("Цвет", item.Color);
                    writer.WriteAttributeString("Размер", item.Size);
                    writer.WriteAttributeString("Amount", item.Amount.ToString());
                    writer.WriteAttributeString("Price", (item.Price * currencyValue).ToString("F2", CultureInfo.InvariantCulture));
                    //for 1c, because this filds 1c are waiting
                    writer.WriteAttributeString("Currency", string.Empty);

                    if (isAdvanced)
                    {
                        var discountOnProduct =
                            (float)Math.Round(item.Amount * item.Price / orderItemsSum * totalDiscount, 2);

                        writer.WriteAttributeString("Discount",
                            ((orderItemsCount != count + 1 ? discountOnProduct : totalDiscount - discountSum) * currencyValue).ToString("F2", CultureInfo.InvariantCulture));

                        writer.WriteAttributeString("Bonus",
                            ((orderItemsCount != count + 1 ? bonusOnProduct : order.BonusCost - bonusSum) * currencyValue).ToString(CultureInfo.InvariantCulture));

                        writer.WriteAttributeString("Payment",
                            ((orderItemsCount != count + 1 ? paymentCostOnProduct : order.PaymentCost - paymentSum) * currencyValue).ToString(CultureInfo.InvariantCulture));

                        discountSum += discountOnProduct;
                        bonusSum += bonusOnProduct;
                        paymentSum += paymentCostOnProduct;
                    }

                    if (item.ProductID != null)
                    {
                        var product = ProductService.GetProduct((int)item.ProductID);
                        writer.WriteAttributeString("Unit", product == null ? "" : product.Unit);
                    }

                    writer.WriteEndElement();
                    count++;
                }
                writer.WriteEndElement();
            }

            if (customer != null)
            {
                var customerName = StringHelper.AggregateStrings(" ", customer.LastName, customer.FirstName, customer.Patronymic);
                var orderTrafficSource = OrderTrafficSourceService.Get(order.OrderID, TrafficSourceType.Order);

                writer.WriteStartElement("Customers");
                writer.WriteStartElement("Customer");

                writer.WriteAttributeString("GoogleClientID", orderTrafficSource != null && orderTrafficSource.GoogleClientId != null ? orderTrafficSource.GoogleClientId : string.Empty);

                if (orderTrafficSource != null && !string.IsNullOrEmpty(orderTrafficSource.YandexClientId))
                    writer.WriteAttributeString("YandexClientID", orderTrafficSource.YandexClientId);

                writer.WriteAttributeString("Surname", customer.LastName ?? string.Empty);
                writer.WriteAttributeString("Name", customer.FirstName ?? string.Empty);
                writer.WriteAttributeString("Patronymic", customer.Patronymic ?? string.Empty);
                writer.WriteAttributeString("Email", customer.Email ?? string.Empty);
                writer.WriteAttributeString("CustomerType", string.Empty);
                writer.WriteAttributeString("OrganizationName", customer.Organization ?? string.Empty);

                writer.WriteAttributeString("City", string.Empty);
                writer.WriteAttributeString("Address", string.Empty);
                writer.WriteAttributeString("Zip", string.Empty);

                //for 1c, because this filds 1c are waiting
                writer.WriteAttributeString("Phone", customer.Phone);
                writer.WriteAttributeString("Fax", string.Empty);
                writer.WriteAttributeString("ShippingName", customerName);
                writer.WriteAttributeString("BillingName", customerName);
                writer.WriteAttributeString("ContactName", customerName);

                //for 1c, because this filds 1c are waiting
                writer.WriteAttributeString("ShippingEmail", string.Empty);
                writer.WriteAttributeString("BillingEmail", string.Empty);

                //writer.WriteAttributeString("BillingEmail", order.BillingContact.Name);

                writer.WriteAttributeString("ShippingCountry", customer.Country ?? "");
                writer.WriteAttributeString("BillingCountry", customer.Country ?? "");
                writer.WriteAttributeString("ShippingZone", customer.Region ?? "");
                writer.WriteAttributeString("BillingZone", customer.Region ?? "");
                writer.WriteAttributeString("ShippingDistrict", customer.District ?? "");
                writer.WriteAttributeString("BillingDistrict", customer.District ?? "");

                writer.WriteAttributeString("ShippingCity", customer.City ?? "");
                writer.WriteAttributeString("BillingCity", customer.City ?? "");
                writer.WriteAttributeString("ShippingAddress", order.OrderPickPoint != null ? order.OrderPickPoint.PickPointAddress : customer.GetCustomerAddress());
                writer.WriteAttributeString("BillingAddress", order.OrderPickPoint != null ? order.OrderPickPoint.PickPointAddress : customer.GetCustomerAddress());
                writer.WriteAttributeString("ShippingZip", customer.Zip ?? "");
                writer.WriteAttributeString("BillingZip", customer.Zip ?? "");
                //for 1c, because this filds 1c are waiting
                writer.WriteAttributeString("ShippingPhone", string.Empty);
                writer.WriteAttributeString("BillingPhone", string.Empty);
                writer.WriteAttributeString("ShippingFax", string.Empty);
                writer.WriteAttributeString("BillingFax", string.Empty);

                writer.WriteEndElement(); //Customer
                writer.WriteEndElement(); //Customers
            }

            if (order.PaymentDetails != null)
            {
                writer.WriteStartElement("PaymentDetails");
                writer.WriteAttributeString("CompanyName", order.PaymentDetails.CompanyName ?? "");
                writer.WriteAttributeString("INN", order.PaymentDetails.INN ?? "");
                writer.WriteAttributeString("Phone", order.PaymentDetails.Phone ?? "");
                writer.WriteAttributeString("Contract", order.PaymentDetails.Contract ?? "");
                writer.WriteEndElement(); //PaymentDetails
            }

            writer.WriteEndElement();
        }

        public static List<OrderPriceDiscount> GetOrderPricesDiscounts()
        {
            var result = new List<OrderPriceDiscount>();

            if (SettingsCheckout.EnableDiscountModule)
            {
                result = CacheManager.Get(CacheNames.GetOrderPriceDiscountCacheObjectName(), 20,
                    () =>
                        SQLDataAccess.ExecuteReadList(
                            "SELECT PriceRange, PercentDiscount FROM [Order].OrderPriceDiscount ORDER BY PriceRange",
                            CommandType.Text,
                            reader => new OrderPriceDiscount
                            {
                                PercentDiscount = SQLDataHelper.GetDouble(reader, "PercentDiscount"),
                                PriceRange = SQLDataHelper.GetFloat(reader, "PriceRange")
                            })
                        ?? new List<OrderPriceDiscount>());
            }
            return result;
        }

        public static float GetDiscount(float price)
        {
            return GetDiscount(GetOrderPricesDiscounts(), price);
        }

        public static float GetDiscount(List<OrderPriceDiscount> table, float price)
        {
            var currency = CurrencyService.CurrentCurrency;

            return table == null
                       ? 0
                       : (float)
                         table.Where(dr => dr.PriceRange / currency.Rate <= price).OrderBy(dr => dr.PriceRange).DefaultIfEmpty(
                             new OrderPriceDiscount { PercentDiscount = 0 }).Last().PercentDiscount;
        }

        public static bool? IsDecremented(int orderId)
        {
            return SQLDataAccess.ExecuteScalar<bool>("[Order].[sp_IsDecremented]", CommandType.StoredProcedure, new SqlParameter("@OrderID", orderId));
        }

        public static Order RefreshTotal(Order order, bool ignoreHistory = true, OrderChangedBy changedBy = null, bool updateModules = true)
        {
            float totalPrice = 0;
            float totalProductsPrice = 0;
            float productsIgnoreDiscountPrice = 0;
            float totalDiscount = 0;
            float supplyTotal = 0;
            float bonusPrice = order.BonusCost;

            if (order.OrderItems.Count > 0)
            {
                totalProductsPrice = order.OrderItems.Sum(x => PriceService.SimpleRoundPrice(x.Price * x.Amount, order.OrderCurrency));
                productsIgnoreDiscountPrice = order.OrderItems.Where(x => x.IgnoreOrderDiscount).Sum(x => PriceService.SimpleRoundPrice(x.Price * x.Amount, order.OrderCurrency));
                supplyTotal = order.OrderItems.Sum(x => x.SupplyPrice * x.Amount);
            }
            else if (order.OrderCertificates.Count > 0)
            {
                totalProductsPrice = order.OrderCertificates.Sum(item => item.Sum);
            }

            totalDiscount += order.OrderDiscount > 0 ? (order.OrderDiscount * (totalProductsPrice - productsIgnoreDiscountPrice) / 100).RoundPrice(order.OrderCurrency.CurrencyValue, order.OrderCurrency) : 0;
            totalDiscount += order.OrderDiscountValue;

            if (order.Certificate != null)
            {
                totalDiscount += order.Certificate.Price != 0 ? order.Certificate.Price : 0;
            }

            if (order.Coupon != null)
            {
                switch (order.Coupon.Type)
                {
                    case CouponType.Fixed:
                        var productsPrice =
                            order.OrderItems.Where(item => item.IsCouponApplied).Sum(p => p.Price * p.Amount);
                        totalDiscount += productsPrice >= order.Coupon.Value ? order.Coupon.Value : productsPrice;
                        break;

                    case CouponType.Percent:
                        totalDiscount +=
                            order.OrderItems.Where(item => item.IsCouponApplied)
                                 .Sum(item => order.Coupon.Value * item.Price / 100 * item.Amount);
                        break;
                }
            }

            if (order.BonusCost > 0)
            {
                bonusPrice = BonusSystemService.GetBonusCost(totalProductsPrice - totalDiscount + order.ShippingCost, totalProductsPrice - totalDiscount, order.BonusCost);
            }

            totalDiscount = totalDiscount.RoundPrice(order.OrderCurrency.CurrencyValue, order.OrderCurrency);

            totalPrice = (totalProductsPrice - totalDiscount - bonusPrice + order.ShippingCost + order.PaymentCost).RoundPrice(order.OrderCurrency.CurrencyValue, order.OrderCurrency);

            if (totalPrice < 0) totalPrice = 0;

            order.Sum = totalPrice;
            order.SupplyTotal = supplyTotal;
            order.BonusCost = bonusPrice;
            order.DiscountCost = totalDiscount;
            order.TaxCost = 0;

            if (!ignoreHistory)
            {
                var refreshTotalOrder = new OnRefreshTotalOrder()
                {
                    Sum = order.Sum,
                    TaxCost = order.TaxCost,
                    BonusCost = order.BonusCost
                };
                OrderHistoryService.ChangingOrderTotal(order.OrderID, refreshTotalOrder, changedBy);
            }

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[Order] SET [Sum] = @Sum, [SupplyTotal] = @SupplyTotal, [BonusCost] = @BonusCost, [DiscountCost] = @DiscountCost WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@OrderID", order.OrderID),
                new SqlParameter("@Sum", order.Sum),
                new SqlParameter("@SupplyTotal", order.SupplyTotal),
                new SqlParameter("@BonusCost", order.BonusCost),
                new SqlParameter("@DiscountCost", order.DiscountCost));

            if (updateModules)
                ModulesExecuter.OrderUpdated(order);

            return order;
        }

        public static int GetLastOrderId()
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT TOP 1 OrderID FROM [Order].[Order] order by OrderDate desc", CommandType.Text);
        }

        public static int GetLastDbOrderId()
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("SELECT IDENT_CURRENT('[Order].[Order]')", CommandType.Text));
        }

        public static void ResetOrderID(int newOrderId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DBCC CHECKIDENT ('Order.Order', RESEED, @OrderId);", CommandType.Text,
                new SqlParameter("@OrderId", newOrderId));
        }

        public static void UpdateOrderMain(Order order, bool updateModules = true, OrderChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
                OrderHistoryService.ChangingOrderMain(order, changedBy);

            order.GroupName = order.GroupName ?? (CustomerGroupService.GetCustomerGroup() != null
                                  ? CustomerGroupService.GetCustomerGroup().GroupName
                                  : "");

            var draftChanged = false;

            if (updateModules && !order.IsDraft)
            {
                var prevOrder = GetOrder(order.OrderID);
                if (prevOrder != null && prevOrder.IsDraft)
                    draftChanged = true;
            }

            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Order].[Order]
                       SET [Number] = @Number
                          ,[ShippingMethodID] = @ShippingMethodID
                          ,[PaymentMethodID] = @PaymentMethodID
                          ,[AffiliateID] = @AffiliateID
                          ,[OrderDiscount] = @OrderDiscount
                          ,[CustomerComment] = @CustomerComment
                          ,[AdditionalTechInfo] = @AdditionalTechInfo
                          ,[AdminOrderComment] = @AdminOrderComment
                          ,[Decremented] = @Decremented
                          ,[ShippingCost] = @ShippingCost
                          ,[PaymentCost] = @PaymentCost
                          ,[TaxCost] = @TaxCost
                          ,[SupplyTotal] = @SupplyTotal
                          ,[Sum] = @Sum
                          ,[ShippingMethodName] = @ShippingMethodName
                          ,[PaymentMethodName] = @PaymentMethodName
                          ,[GroupName] = @GroupName
                          ,[GroupDiscount] = @GroupDiscount
                          ,[OrderDate] = @OrderDate
                          ,[CertificateCode] = @CertificateCode
                          ,[CertificatePrice] = @CertificatePrice
                          ,[ManagerId] = @ManagerId
                          ,[UseIn1C] = @UseIn1C
                          ,[ModifiedDate] = Getdate()
                          ,[ManagerConfirmed] = @ManagerConfirmed
                          ,[OrderSourceId] = @OrderSourceId
                          ,[BonusCost] = @BonusCost
                          ,[BonusCardNumber] = @BonusCardNumber
                          ,[CustomData] = @CustomData
                          ,[IsDraft] = @IsDraft
                          ,[DeliveryDate] = @DeliveryDate
                          ,[DeliveryTime] = @DeliveryTime
                          ,[TrackNumber] = @TrackNumber
                          ,[OrderDiscountValue] = @OrderDiscountValue
                          ,[ShippingTaxType] = @ShippingTaxType
                          ,[CouponCode] = @CouponCode
                          ,[CouponType] = @CouponType
                          ,[CouponValue] = @CouponValue
                          ,[TotalWeight] = @TotalWeight
                          ,[TotalLength] = @TotalLength
                          ,[TotalWidth] = @TotalWidth
                          ,[TotalHeight] = @TotalHeight
                          ,[LeadId] = @LeadId
                          ,[AvailablePaymentCashOnDelivery] = @AvailablePaymentCashOnDelivery
                          ,[AvailablePaymentPickPoint] = @AvailablePaymentPickPoint
                          ,[ShippingPaymentMethodType] = @ShippingPaymentMethodType
                          ,[ShippingPaymentSubjectType] = @ShippingPaymentSubjectType
                     WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@Number", order.Number),
                new SqlParameter("@ShippingMethodID", order.ShippingMethodId == 0 ? (object)DBNull.Value : order.ShippingMethodId),
                new SqlParameter("@PaymentMethodID", order.PaymentMethodId == 0 ? (object)DBNull.Value : order.PaymentMethodId),
                new SqlParameter("@AffiliateID", order.AffiliateID),
                new SqlParameter("@OrderDiscount", order.OrderDiscount),
                new SqlParameter("@CustomerComment", order.CustomerComment ?? string.Empty),
                new SqlParameter("@AdditionalTechInfo", order.AdditionalTechInfo ?? string.Empty),
                new SqlParameter("@AdminOrderComment", order.AdminOrderComment ?? string.Empty),
                new SqlParameter("@Decremented", order.Decremented),
                new SqlParameter("@ShippingCost", order.ShippingCost),
                new SqlParameter("@PaymentCost", order.PaymentCost),
                new SqlParameter("@TaxCost", order.TaxCost),
                new SqlParameter("@SupplyTotal", order.SupplyTotal),
                new SqlParameter("@Sum", order.Sum),
                new SqlParameter("@OrderID", order.OrderID),
                new SqlParameter("@ShippingMethodName", order.ArchivedShippingName ?? ""),
                new SqlParameter("@PaymentMethodName", order.PaymentMethodName ?? ""),
                new SqlParameter("@GroupName", order.GroupName),
                new SqlParameter("@GroupDiscount", order.GroupDiscount),
                new SqlParameter("@OrderDate", order.OrderDate),
                new SqlParameter("@CertificateCode", order.Certificate != null ? (object)order.Certificate.Code : DBNull.Value),
                new SqlParameter("@CertificatePrice", order.Certificate != null ? (object)order.Certificate.Price : DBNull.Value),
                new SqlParameter("@ManagerId", order.ManagerId ?? (object)DBNull.Value),
                new SqlParameter("@UseIn1C", order.UseIn1C),
                new SqlParameter("@ManagerConfirmed", order.ManagerConfirmed),
                new SqlParameter("@OrderSourceId", order.OrderSourceId),
                new SqlParameter("@BonusCost", order.BonusCost),
                new SqlParameter("@BonusCardNumber", order.BonusCardNumber ?? (object)DBNull.Value),
                new SqlParameter("@CustomData", order.CustomData ?? (object)DBNull.Value),
                new SqlParameter("@IsDraft", order.IsDraft),
                new SqlParameter("@DeliveryDate", order.DeliveryDate ?? (object)DBNull.Value),
                new SqlParameter("@DeliveryTime", order.DeliveryTime ?? string.Empty),
                new SqlParameter("@TrackNumber", order.TrackNumber ?? (object)DBNull.Value),
                new SqlParameter("@OrderDiscountValue", order.OrderDiscountValue),
                new SqlParameter("@ShippingTaxType", (int)order.ShippingTaxType),
                new SqlParameter("@CouponCode", order.Coupon != null ? order.Coupon.Code : (object)DBNull.Value),
                new SqlParameter("@CouponType", order.Coupon != null ? order.Coupon.Type : (object)DBNull.Value),
                new SqlParameter("@CouponValue", order.Coupon != null ? order.Coupon.Value : (object)DBNull.Value),
                new SqlParameter("@TotalWeight", order.TotalWeight ?? (object)DBNull.Value),
                new SqlParameter("@TotalLength", order.TotalLength ?? (object)DBNull.Value),
                new SqlParameter("@TotalWidth", order.TotalWidth ?? (object)DBNull.Value),
                new SqlParameter("@TotalHeight", order.TotalHeight ?? (object)DBNull.Value),
                new SqlParameter("@LeadId", order.LeadId ?? (object)DBNull.Value),
                new SqlParameter("@AvailablePaymentCashOnDelivery", order.AvailablePaymentCashOnDelivery),
                new SqlParameter("@AvailablePaymentPickPoint", order.AvailablePaymentPickPoint),
                new SqlParameter("@ShippingPaymentMethodType", (int)order.ShippingPaymentMethodType),
                new SqlParameter("@ShippingPaymentSubjectType", (int)order.ShippingPaymentSubjectType)
                );

            if (updateModules)
            {
                if (draftChanged)
                {
                    ModulesExecuter.OrderAdded(order);
                    ModulesExecuter.SendNotificationsOnOrderAdded(order);
                    LeadService.CompleteLeadsOnNewOrder(order);
                }
                else
                {
                    ModulesExecuter.OrderUpdated(order);
                    ModulesExecuter.SendNotificationsOnOrderUpdated(order);
                }
            }
        }

        public static void DecrementProductsCountAccordingOrder(Order order, OrderStatusHistory history = null)
        {
            if (Settings1C.Enabled && Settings1C.DisableProductsDecremention)
                return;

            SQLDataAccess.ExecuteNonQuery("[Order].[sp_DecrementProductsCountAccordingOrder]",
                                            CommandType.StoredProcedure,
                                            new SqlParameter("@orderId", order.OrderID));

            order.Decremented = true;

            foreach (var orderItem in GetOrderItems(order.OrderID).Where(x => x.ProductID.HasValue))
            {
                if (!orderItem.ProductID.HasValue)
                    continue;

                ProductHistoryService.ProductAmountChangedByOrderStatus(orderItem.ProductID.Value, orderItem.ArtNo,
                    false, order.Number, history?.NewStatus, history?.CustomerName);
                ProductService.PreCalcProductParams(orderItem.ProductID.Value);
            }
        }

        public static void IncrementProductsCountAccordingOrder(Order order, OrderStatusHistory history = null)
        {
            if (Settings1C.Enabled && Settings1C.DisableProductsDecremention)
                return;

            SQLDataAccess.ExecuteNonQuery("[Order].[sp_IncrementProductsCountAccordingOrder]",
                                            CommandType.StoredProcedure,
                                            new SqlParameter("@orderId", order.OrderID));

            foreach (var orderItem in GetOrderItems(order.OrderID))
            {
                if (!orderItem.ProductID.HasValue)
                    continue;

                ProductHistoryService.ProductAmountChangedByOrderStatus(orderItem.ProductID.Value, orderItem.ArtNo,
                    true, order.Number, history?.NewStatus, history?.CustomerName);
                
                ProductService.PreCalcProductParams(orderItem.ProductID.Value);
            }
        }

        public static OrderCustomer GetOrderCustomer(int orderId)
        {
            return
                SQLDataAccess.Query<OrderCustomer>("SELECT * FROM [Order].[OrderCustomer] WHERE [OrderID] = @orderId",
                    new { orderId }).FirstOrDefault();
        }

        public static OrderCustomer GetOrderCustomer(string orderNumber)
        {
            return
                SQLDataAccess.Query<OrderCustomer>("SELECT * FROM [Order].[OrderCustomer] WHERE [OrderID] = (Select OrderID from [Order].[Order] Where Number=@orderNumber)",
                    new { orderNumber }).FirstOrDefault();
        }

        public static List<string> GetOrderCustomersEmails()
        {
            return SQLDataAccess.ExecuteReadColumn<string>(
                "SELECT DISTINCT [Email] FROM [Order].[OrderCustomer] WHERE [Email] IS NOT NULL AND [Email] <> ''",
                CommandType.Text,
                "Email");
        }

        public static OrderCurrency GetOrderCurrency(int orderId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Order].[OrderCurrency] WHERE [OrderID] = @OrderID",
                CommandType.Text,
                reader =>
                new OrderCurrency
                {
                    CurrencyCode = SQLDataHelper.GetString(reader, "CurrencyCode"),
                    CurrencyNumCode = SQLDataHelper.GetInt(reader, "CurrencyNumCode"),
                    CurrencyValue = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                    CurrencySymbol = SQLDataHelper.GetString(reader, "CurrencySymbol"),
                    IsCodeBefore = SQLDataHelper.GetBoolean(reader, "IsCodeBefore"),
                    EnablePriceRounding = SQLDataHelper.GetBoolean(reader, "EnablePriceRounding"),
                    RoundNumbers = SQLDataHelper.GetFloat(reader, "RoundNumbers"),
                }, new SqlParameter("@OrderID", orderId));
        }

        public static OrderPickPoint GetOrderPickPoint(int orderId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Order].[OrderPickPoint] WHERE [OrderID] = @OrderID",
                CommandType.Text,
                reader =>
                new OrderPickPoint
                {
                    OrderId = orderId,
                    PickPointId = SQLDataHelper.GetString(reader, "PickPointId"),
                    PickPointAddress = SQLDataHelper.GetString(reader, "PickPointAddress"),
                    AdditionalData = SQLDataHelper.GetString(reader, "AdditionalData", ""),
                }, new SqlParameter("@OrderID", orderId));
        }

        public static void UpdateOrderCurrency(int orderId, string currencyCode, float currencyValue, OrderChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
                OrderHistoryService.ChangingCurrency(orderId, currencyCode, currencyValue, changedBy);

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[OrderCurrency] SET [CurrencyCode] = @CurrencyCode, [CurrencyValue] = @CurrencyValue WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@CurrencyCode", currencyCode),
                new SqlParameter("@CurrencyValue", currencyValue));
        }

        public static Order GetOrderByNumber(string orderNumber)
        {
            return GetOrder(GetOrderIdByNumber(orderNumber));
        }

        public static Order GetOrderByCode(string code)
        {
            return GetOrder(GetOrderIdByCode(code));
        }

        public static Order GetOrderByPayCode(string payCode)
        {
            return GetOrder(GetOrderIdByPayCode(payCode));
        }

        public static void CancelOrder(int orderID)
        {
            OrderStatusService.ChangeOrderStatus(orderID, OrderStatusService.CanceledOrderStatus, LocalizationService.GetResource("Core.Orders.Order.OrderCanceled"));
            UpdateStatusComment(orderID, LocalizationService.GetResource("Core.Orders.Order.UserCanceledOrder"));
        }

        public static void SendSetOrderManagerMail(int orderId, int managerId)
        {
            var manager = ManagerService.GetManager(managerId);
            if (!manager.Enabled || !manager.HasRoleAction(RoleAction.Orders))
                return;

            var mailTpl = new SetOrderManagerMailTemplate(manager.FullName, orderId);
            MailService.SendMailNow(manager.CustomerId, manager.Email, mailTpl);
        }

        public static void SendOrderMail(Order order, float? bonusPlus = null)
        {
            if (bonusPlus == null && order.BonusCardNumber != null)
            {
                var purchase = PurchaseService.GetByOrderId(order.OrderID);
                if (purchase != null)
                    bonusPlus = (float)purchase.NewBonusAmount;
            }

            SendOrderMail(order, order.TotalDiscount, bonusPlus ?? 0, order.ArchivedShippingName, order.PaymentMethodName);
        }

        public static void SendOrderMail(Order order, float totalDiscount, float bonusPlus, string shippingName, string paymentName)
        {
            var customer = order.OrderCustomer;
            var email = customer.Email;

            var mail = GetMailTemplate(order, totalDiscount, bonusPlus, shippingName, paymentName);

            if (!string.IsNullOrWhiteSpace(email))
            {
                MailService.SendMailNow(customer.CustomerID, email, mail);
            }
            MailService.SendMailNow(SettingsMail.EmailForOrders, mail, replyTo: email);
        }

        public static MailTemplate GetMailTemplate(Order order)
        {
            var bonusPlus = 0f;
            if (order.BonusCardNumber != null)
            {
                var purchase = PurchaseService.GetByOrderId(order.OrderID);
                if (purchase != null)
                    bonusPlus = (float)purchase.NewBonusAmount;
            }

            return GetMailTemplate(order, order.TotalDiscount, bonusPlus, order.ArchivedShippingName, order.PaymentMethodName);
        }

        public static MailTemplate GetMailTemplate(Order order, float totalDiscount, float bonusPlus, string shippingName, string paymentName)
        {
            var orderItemsHtml = GenerateOrderItemsHtml(order.OrderItems, order.OrderCurrency,
                                                        order.OrderItems.Sum(oi => oi.Price * oi.Amount),
                                                        order.OrderDiscount, order.OrderDiscountValue,
                                                        order.Coupon, order.Certificate,
                                                        totalDiscount,
                                                        order.ShippingCost, order.PaymentCost,
                                                        order.TaxCost,
                                                        order.BonusCost,
                                                        bonusPlus);

            const string format = "<div class='l-row'><div class='l-name vi cs-light' style='display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px;'>{0}:</div><div class='l-value vi' style='display: inline-block; margin: 5px 0;'>{1}</div></div>";

            var customer = order.OrderCustomer;

            // Build a new mail
            var customerSb = new StringBuilder();
            customerSb.AppendFormat(format, SettingsCheckout.CustomerFirstNameField, customer.FirstName);

            if (SettingsCheckout.IsShowLastName && !string.IsNullOrEmpty(customer.LastName))
                customerSb.AppendFormat(format, LocalizationService.GetResource("User.Registration.LastName"), customer.LastName);

            if (SettingsCheckout.IsShowPatronymic && !string.IsNullOrEmpty(customer.Patronymic))
                customerSb.AppendFormat(format, LocalizationService.GetResource("User.Registration.Patronymic"), customer.Patronymic);

            if (SettingsCheckout.IsShowPhone && !string.IsNullOrEmpty(customer.Phone))
                customerSb.AppendFormat(format, SettingsCheckout.CustomerPhoneField, customer.Phone);

            if (SettingsCheckout.IsShowCountry && !string.IsNullOrEmpty(customer.Country))
                customerSb.AppendFormat(format, LocalizationService.GetResource("User.Registration.Country"), customer.Country);

            if (SettingsCheckout.IsShowState && customer.Region.IsNotEmpty())
                customerSb.AppendFormat(format, LocalizationService.GetResource("User.Registration.Region"), customer.Region);

            if (SettingsCheckout.IsShowDistrict && !string.IsNullOrEmpty(customer.District))
                customerSb.AppendFormat(format, LocalizationService.GetResource("User.Registration.District"), customer.District);

            if (SettingsCheckout.IsShowCity && !string.IsNullOrEmpty(customer.City))
                customerSb.AppendFormat(format, LocalizationService.GetResource("User.Registration.City"), customer.City);

            if (SettingsCheckout.IsShowZip && !string.IsNullOrEmpty(customer.Zip))
                customerSb.AppendFormat(format, LocalizationService.GetResource("User.Registration.Zip"), customer.Zip);

            if (SettingsCheckout.IsShowAddress)
            {
                var address = customer.GetCustomerAddress();

                customerSb.AppendFormat(format, LocalizationService.GetResource("User.Registration.Address"),
                    string.IsNullOrEmpty(address)
                        ? LocalizationService.GetResource("User.Registration.AddressEmpty")
                        : address);
            }

            var additionalCustomerFields = string.Empty;

            foreach (var customerField in CustomerFieldService.GetCustomerFieldsWithValue(customer.CustomerID).Where(x => x.ShowInCheckout))
            {
                additionalCustomerFields += string.Format(format, customerField.Name, customerField.Value);
            }

            if (SettingsCheckout.IsShowCustomShippingField1 && customer.CustomField1.IsNotEmpty())
            {
                customerSb.AppendFormat(format, SettingsCheckout.CustomShippingField1, customer.CustomField1);
            }

            if (SettingsCheckout.IsShowCustomShippingField2 && customer.CustomField2.IsNotEmpty())
            {
                customerSb.AppendFormat(format, SettingsCheckout.CustomShippingField2, customer.CustomField2);
            }

            if (SettingsCheckout.IsShowCustomShippingField3 && customer.CustomField3.IsNotEmpty())
            {
                customerSb.AppendFormat(format, SettingsCheckout.CustomShippingField3, customer.CustomField3);
            }

            var email = customer.Email;
            var mail = new NewOrderMailTemplate(order.Number, order.Code.ToString(), email, customerSb.ToString(),
                                                shippingName + (order.OrderPickPoint != null ? "<br />" + order.OrderPickPoint.PickPointAddress : ""),
                                                paymentName,
                                                orderItemsHtml,
                                                order.OrderCurrency.CurrencyCode,
                                                order.Sum.ToString(),
                                                order.CustomerComment,
                                                GetBillingLinkHash(order),
                                                customer.FirstName,
                                                customer.LastName,
                                                order.Manager != null ? order.Manager.FullName : string.Empty,
                                                order.PaymentDetails != null ? order.PaymentDetails.INN : string.Empty,
                                                order.PaymentDetails != null ? order.PaymentDetails.CompanyName : string.Empty,
                                                additionalCustomerFields,
                                                order.OrderCustomer.City,
                                                order.OrderCustomer.GetCustomerAddress(),
                                                order.PayCode, order.OrderID);

            return mail;
        }

        public static string GenerateOrderItemsHtml(List<OrderItem> orderItems, Currency currency, float productsPrice,
                                                    float orderDiscountPercent, float orderDiscountValue, OrderCoupon coupon, OrderCertificate certificate,
                                                    float totalDiscount, float shippingPrice, float paymentPrice, float taxesTotal, float bonusPrice,
                                                    float newBonus)
        {
            var orderItemsHtml = new StringBuilder();

            orderItemsHtml.Append("<table class='orders-table' style='border-collapse: collapse; width: 100%;'>");
            orderItemsHtml.Append("<tr class='orders-table-header'>");
            orderItemsHtml.AppendFormat("<th class='photo' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: left;'>{0}</th>", LocalizationService.GetResource("Core.Orders.Order.Letter.Goods"));
            orderItemsHtml.Append("<th class='name' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: left; width: 50%;'></th>");
            orderItemsHtml.AppendFormat("<th class='price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: center;'>{0}</th>", LocalizationService.GetResource("Core.Orders.Order.Letter.Price"));
            orderItemsHtml.AppendFormat("<th class='amount' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: center; white-space:nowrap;'>{0}</th>", LocalizationService.GetResource("Core.Orders.Order.Letter.Count"));
            orderItemsHtml.AppendFormat("<th class='total-price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0 20px 0; text-align: center;'>{0}</th>", LocalizationService.GetResource("Core.Orders.Order.Letter.Cost"));
            orderItemsHtml.Append("</tr>");

            //var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            // Добавление заказанных товаров
            foreach (var item in orderItems)
            {
                orderItemsHtml.Append("<tr>");

                Photo photo;
                if (item.PhotoID.HasValue && item.PhotoID != 0 && (photo = PhotoService.GetPhoto((int)item.PhotoID)) != null)
                {
                    orderItemsHtml.AppendFormat(
                        "<td class='photo' style='border-bottom: 1px solid #e3e3e3; margin-right: 15px; padding: 20px 5px 20px 0; padding-left: 20px; text-align: left; width:{1}px;'><img style='border:none;display:block;outline:none;text-decoration:none;max-width:100%;height:auto;' src='{0}' /></td>",
                        FoldersHelper.GetImageProductPath(ProductImageType.XSmall, photo.PhotoName, false), SettingsPictureSize.XSmallProductImageWidth);
                }
                else
                {
                    orderItemsHtml.AppendFormat("<td>&nbsp;</td>");
                }

                var product = item.ProductID.HasValue ? ProductService.GetProduct(item.ProductID.Value) : null;

                orderItemsHtml.AppendFormat("<td class='name' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: left; min-width:150px; width: 50%;'>" +
                                                    "<div class='description' style='display: inline-block;'>" +
                                                        "<div class='prod-name' style='font-size: 14px; margin-bottom: 5px;'><a href='{0}' class='cs-link' style='color: #0764c3; text-decoration: none;'>{1}</a></div> " +
                                                        "{2} {3} {4}" +
                                                    "</div>" +
                                            "</td>",
                                            product != null ?
                                                        SettingsMain.SiteUrl.Trim('/') + "/" + UrlService.GetLink(ParamType.Product, product.UrlPath, product.ProductId)
                                                        : "",
                                            item.ArtNo + (!string.IsNullOrEmpty(item.ArtNo) ? ", " : "") + item.Name,
                                            item.Color.IsNotEmpty() ? "<div class='prod-option' style='margin-bottom: 5px;'><span class='cs-light' style='color: #acacac;'>" + SettingsCatalog.ColorsHeader + ":</span><span class='value cs-link' style='padding-left: 10px;'>" + item.Color + "</span></div>" : "",
                                            item.Size.IsNotEmpty() ? "<div class='prod-option' style='margin-bottom: 5px;'><span class='cs-light' style='color: #acacac;'>" + SettingsCatalog.SizesHeader + ":</span><span class='value cs-link' style='padding-left: 10px;'>" + item.Size + "</span></div>" : "",
                                            RenderSelectedOptions(item.SelectedOptions, currency));
                orderItemsHtml.AppendFormat("<td class='price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: center; white-space: nowrap;'>{0}</td>", item.Price.FormatPrice(currency));
                orderItemsHtml.AppendFormat("<td class='amount' style='border-bottom: 1px solid #e3e3e3; padding: 20px 5px 20px 0; text-align: center;'>{0}</td>", item.Amount);
                orderItemsHtml.AppendFormat("<td class='total-price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0 20px 0; text-align: center;  white-space: nowrap;'>{0}</td>", PriceService.SimpleRoundPrice(item.Price * item.Amount, currency).FormatPrice(currency));
                orderItemsHtml.Append("</tr>");
            }

            const string footerFormat = "<tr>" +
                                            "<td class='footer-name' colspan='4' style='border-bottom: none; padding: 5px; text-align: right;'>{0}:</td>" +
                                            "<td class='footer-value' style='border-bottom: none; padding: 5px 0; text-align: center;'>{1}</td>" +
                                        "</tr>";

            const string footerMinusFormat = "<tr>" +
                                                "<td class='footer-name' colspan='4' style='border-bottom: none; padding: 5px; text-align: right;'>{0}:</td>" +
                                                "<td class='footer-value' style='border-bottom: none; padding: 5px 0; text-align: center;'>-{1}</td>" +
                                            "</tr>";

            // Стоимость заказа
            orderItemsHtml.AppendFormat(footerFormat, LocalizationService.GetResource("Core.Orders.Order.Letter.OrderCost"), PriceService.SimpleRoundPrice(productsPrice, currency).FormatPrice(currency));

            if (orderDiscountPercent != 0 || orderDiscountValue != 0)
            {
                var productsIgnoreDiscountPrice = orderItems.Where(item => item.IgnoreOrderDiscount).Sum(item => item.Price * item.Amount);
                orderItemsHtml.AppendFormat(footerMinusFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.Discount"), PriceFormatService.FormatDiscountPercent(productsPrice - productsIgnoreDiscountPrice, orderDiscountPercent, orderDiscountValue, false));
            }

            if (bonusPrice != 0)
                orderItemsHtml.AppendFormat(footerMinusFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.Bonuses"), bonusPrice.FormatPrice(currency));

            if (certificate != null)
                orderItemsHtml.AppendFormat(footerMinusFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.Certificate"), certificate.Price.FormatPrice(currency));

            if (coupon != null)
            {
                float couponValue;
                string couponString = null;
                switch (coupon.Type)
                {
                    case CouponType.Fixed:
                        var productPrice = orderItems.Where(p => p.IsCouponApplied).Sum(p => p.Price * p.Amount);
                        couponValue = productPrice >= coupon.Value ? coupon.Value : productPrice;
                        couponString = String.Format("-{0} ", PriceFormatService.FormatPrice(couponValue.RoundPrice(currency.Rate, currency), currency));
                        break;

                    case CouponType.Percent:
                        couponValue = orderItems.Where(p => p.IsCouponApplied).Sum(p => coupon.Value * p.Price / 100 * p.Amount);
                        couponString = String.Format("-{0} ({1}%)", PriceFormatService.FormatPrice(couponValue.RoundPrice(currency.Rate, currency), currency),
                                                       PriceFormatService.FormatPriceInvariant(coupon.Value));
                        break;
                }
                orderItemsHtml.AppendFormat(footerMinusFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.Coupon"), couponString);//coupon.Value.FormatPriceInvariant());
            }

            // Стоимость доставки
            if (shippingPrice != 0)
                orderItemsHtml.AppendFormat(footerFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.ShippingCost"), shippingPrice.FormatPrice(currency));

            if (paymentPrice != 0)
                orderItemsHtml.AppendFormat(footerFormat,
                    paymentPrice > 0 ? LocalizationService.GetResource("Core.Orders.Order.Letter.PaymentCost") : LocalizationService.GetResource("Core.Orders.Order.Letter.PaymentDiscount"), paymentPrice.FormatPrice(currency));

            var total = productsPrice - totalDiscount - bonusPrice + shippingPrice + paymentPrice;
            if (total < 0) total = 0;

            // Итого
            orderItemsHtml.AppendFormat(footerFormat,
                "<b>" + LocalizationService.GetResource("Core.Orders.Order.Letter.Total") + "</b>",
                "<b>" + PriceService.SimpleRoundPrice(total, currency).FormatPrice(currency) + "</b>");

            if (newBonus > 0)
            {
                orderItemsHtml.AppendFormat(footerFormat,
                    LocalizationService.GetResource("Core.Orders.Order.Letter.NewBonus"), newBonus.ToString("F2"));
            }

            orderItemsHtml.Append("</table>");

            return orderItemsHtml.ToString();
        }

        public static string GenerateCertificatesHtml(List<GiftCertificate> orderCetificates, Currency currency, float paymentPrice, float taxesTotal)
        {
            var certificatesHtml = new StringBuilder();

            certificatesHtml.Append("<table class='orders-table' style='border-collapse: collapse; width: 100%;'>");
            certificatesHtml.Append("<tr>");
            certificatesHtml.AppendFormat("<td style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: left;'>{0}</td>", LocalizationService.GetResource("Core.Orders.Order.Letter.Certificate"));
            certificatesHtml.AppendFormat("<td style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center; width: 150px;'>{0}</td>", LocalizationService.GetResource("Core.Orders.Order.Letter.Price"));
            certificatesHtml.Append("</tr>");

            // Добавление заказанных сертификатов
            foreach (var item in orderCetificates)
            {
                certificatesHtml.Append("<tr>");
                certificatesHtml.AppendFormat("<td style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</td>", item.CertificateCode);
                certificatesHtml.AppendFormat("<td style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</td>", item.Sum.FormatPrice(currency));
                certificatesHtml.Append("</tr>");
            }

            const string footerFormat = "<tr>" +
                                            "<td style='border-bottom: none; padding: 5px 0; text-align: right;'>{0}:</td>" +
                                            "<td style='border-bottom: none; padding: 5px 0; text-align: center;'>{1}</td>" +
                                        "</tr>";

            // Налоги
            float taxesExcluded = 0f;
            TaxElement tax;
            var taxValue = TaxService.CalculateCertificateTax(orderCetificates.Sum(cert => cert.Sum), out tax);
            if (taxValue.HasValue)
            {
                if (tax.ShowInPrice)
                    taxesExcluded = taxValue.Value;

                certificatesHtml.AppendFormat(footerFormat,
                    (tax.ShowInPrice ? LocalizationService.GetResource("Core.Tax.IncludeTax") : "") + " " +
                    tax.Name,
                    (tax.ShowInPrice ? "" : "+") + taxValue.Value.FormatPrice(currency));
            }

            if (paymentPrice != 0)
            {
                certificatesHtml.AppendFormat(footerFormat,
                    paymentPrice > 0
                        ? LocalizationService.GetResource("Core.Orders.Order.Letter.PaymentCost")
                        : LocalizationService.GetResource("Core.Orders.Order.Letter.PaymentDiscount"),
                    paymentPrice.FormatPrice(currency));
            }

            // Итого
            certificatesHtml.AppendFormat(footerFormat,
                "<b>" + LocalizationService.GetResource("Core.Orders.Order.Letter.Total") + "</b>",
                "<b>" + (orderCetificates.Sum(cert => cert.Sum) + paymentPrice + taxesExcluded).FormatPrice(currency) + "</b>");

            certificatesHtml.Append("</table>");

            return certificatesHtml.ToString();
        }

        public static string GenerateCustomerContactsHtml(OrderCustomer customer)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(customer.FirstName))
                sb.AppendFormat("Имя" + " {0}<br/>", customer.FirstName);

            if (!string.IsNullOrEmpty(customer.LastName))
                sb.AppendFormat("Фамилия" + " {0}<br/>", customer.LastName);

            if (!string.IsNullOrEmpty(customer.Country))
                sb.AppendFormat("Страна" + " {0}<br/>", customer.Country);

            if (!string.IsNullOrEmpty(customer.Region))
                sb.AppendFormat("Регион" + " {0}<br/>", customer.Region);

            if (!string.IsNullOrEmpty(customer.City))
                sb.AppendFormat("Город" + " {0}<br/>", customer.City);

            if (!string.IsNullOrEmpty(customer.Zip))
                sb.AppendFormat("Индекс" + " {0}<br/>", customer.Zip);

            if (!string.IsNullOrEmpty(customer.GetCustomerAddress()))
                sb.AppendFormat("Адрес" + ": {0}<br/>", customer.GetCustomerAddress());

            return sb.ToString();
        }

        public static string RenderSelectedOptions(List<EvaluatedCustomOptions> evlist, Currency currency)
        {
            if (evlist == null || evlist.Count == 0)
                return string.Empty;

            var res = new StringBuilder("<div class=\"customoptions\">");

            foreach (EvaluatedCustomOptions evco in evlist)
            {
                res.Append(evco.CustomOptionTitle + ": " + evco.OptionTitle + " ");
                if (evco.OptionPriceBc > 0)
                {
                    res.Append(evco.OptionPriceType == OptionPriceType.Fixed
                        ? "+" + evco.OptionPriceBc.FormatPrice(currency)
                        : evco.FormatPrice);
                }
                res.Append("<br />");
            }
            res.Append("</div>");

            return res.ToString();
        }

        public static bool IsPaidOrder(int orderId)
        {
            return Convert.ToInt32(
                SQLDataAccess.ExecuteScalar(
                "Select COUNT([PaymentDate]) FROM [Order].[Order] WHERE OrderID = @OrderID AND [PaymentDate] is not null",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId))) > 0;
        }

        public static string GetBillingLinkHash(Order order)
        {
            if (order == null || order.OrderCustomer == null)
                return string.Empty;

            return (order.OrderID + order.Number + order.OrderCustomer.CustomerID).Md5(false).ToString();
        }

        public static void UpdateOrderManager(int orderId, int? managerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[Order] SET [ManagerId] = @ManagerId WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@ManagerId", managerId ?? (object)DBNull.Value));

            if (managerId.HasValue)
                SendSetOrderManagerMail(orderId, managerId.Value);

            BizProcessExecuter.OrderManagerAssigned(GetOrder(orderId));
        }

        [Obsolete("Использовать UpdateOrder")]
        public static void ChangeUseIn1C(int orderId, bool useIn1C)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Order].[Order] Set UseIn1C = @UseIn1C, ModifiedDate = Getdate() Where OrderId=@OrderId",
                CommandType.Text,
                new SqlParameter("@OrderId", orderId),
                new SqlParameter("@UseIn1C", useIn1C));
        }

        public static List<int> GetDeletedOrders(DateTime? from, DateTime? to)
        {
            var query = "SELECT OrderId FROM [Order].[DeletedOrders]";
            var queryParams = new List<SqlParameter>();

            if (from != null && to != null)
            {
                query += " Where [DateTime] >= @From and [DateTime] <= @To";
                queryParams.Add(new SqlParameter("@From", from));
                queryParams.Add(new SqlParameter("@To", to));
            }

            return SQLDataAccess.ExecuteReadList(query, CommandType.Text, reader => SQLDataHelper.GetInt(reader, "OrderId"), queryParams.ToArray());
        }

        public static List<Order> GetOrdersFor1C(DateTime from, DateTime to, bool onlyUseIn1C)
        {
            var query = "SELECT * FROM [Order].[Order] WHERE IsDraft <> 1 and ";
            var queryParams = new List<SqlParameter>();

            if (onlyUseIn1C)
            {
                query += "[UseIn1C] = 1 and ";
            }

            query += "([OrderDate] >= @From and [OrderDate] <= @To or [ModifiedDate] >= @From and [ModifiedDate] <= @To)";
            queryParams.Add(new SqlParameter("@From", from));
            queryParams.Add(new SqlParameter("@To", to));

            return SQLDataAccess.ExecuteReadList(query, CommandType.Text, GetOrderFromReader, queryParams.ToArray());
        }

        public static Order CreateOrder(Lead lead)
        {
            return CreateOrder(lead, null, true);
        }

        public static Order CreateOrder(Lead lead, ChangedBy changedBy, bool autocompleteLeads)
        {
            try
            {
                var order = new Order()
                {
                    OrderCurrency = (Currency)lead.LeadCurrency,
                    OrderStatusId = OrderStatusService.DefaultOrderStatus,
                    CustomerComment = lead.Comment,
                    ManagerId = lead.ManagerId,
                    OrderDate = DateTime.Now,
                    OrderDiscount = lead.Discount,
                    OrderDiscountValue = lead.DiscountValue,
                    OrderSourceId = lead.OrderSourceId,

                    DeliveryDate = lead.DeliveryDate,
                    DeliveryTime = lead.DeliveryTime,
                    OrderPickPoint = !string.IsNullOrEmpty(lead.ShippingPickPoint)
                        ? JsonConvert.DeserializeObject<OrderPickPoint>(lead.ShippingPickPoint)
                        : null,

                    LeadId = lead.Id,
                    IsFromAdminArea = true
                };

                if (SettingsCrm.OrderStatusIdFromLead != 0 &&
                    OrderStatusService.GetOrderStatus(SettingsCrm.OrderStatusIdFromLead) != null)
                {
                    order.OrderStatusId = SettingsCrm.OrderStatusIdFromLead;
                }

                order.OrderCustomer = new OrderCustomer()
                {
                    Email = lead.Email,
                    Phone = lead.Phone,
                    StandardPhone = !string.IsNullOrWhiteSpace(lead.Phone) ? StringHelper.ConvertToStandardPhone(lead.Phone, true, true) : null,
                    FirstName = lead.FirstName,
                    LastName = lead.LastName,
                    Patronymic = lead.Patronymic,
                    Organization = lead.Organization,
                    Country = lead.Country,
                    Region = lead.Region,
                    City = lead.City,
                    District = lead.District,
                    Zip = lead.Zip,
                };

                if (lead.CustomerId != null && lead.Customer != null)
                {
                    order.OrderCustomer.CustomerID = lead.CustomerId.Value;
                    
                    order.GroupName = lead.Customer.CustomerGroup.GroupName;
                    order.GroupDiscount = lead.Customer.CustomerGroup.GroupDiscount;
                }
                
                if (lead.LeadItems != null && lead.LeadItems.Count > 0)
                {
                    foreach (var item in lead.LeadItems)
                        order.OrderItems.Add((OrderItem)item);
                }
                else
                {
                    order.OrderItems.Add(new OrderItem()
                    {
                        Name = !string.IsNullOrEmpty(lead.Title) ? lead.Title : "Оплата заказа",
                        ArtNo = "",
                        Price = lead.Sum,
                        Amount = 1,
                    });
                }

                if (lead.ShippingMethodId != 0 || !string.IsNullOrEmpty(lead.ShippingName))
                {
                    order.ShippingMethodId = lead.ShippingMethodId;
                    order.ArchivedShippingName = lead.ShippingName;
                    order.ShippingCost = lead.ShippingCost;
                }
                else
                {
                    ShippingMethod shipping = null;

                    var id = SettingsCheckout.BuyInOneClickDefaultShippingMethod;
                    if (id != 0)
                        shipping = ShippingMethodService.GetShippingMethod(id);

                    if (shipping == null || !shipping.Enabled)
                        shipping = ShippingMethodService.GetAllShippingMethods(true).FirstOrDefault();

                    if (shipping != null)
                    {
                        var preOrder = new PreOrder()
                        {
                            CountryDest = order.OrderCustomer.Country,
                            RegionDest = order.OrderCustomer.Region,
                            DistrictDest = order.OrderCustomer.District,
                            CityDest = order.OrderCustomer.City ?? SettingsMain.City,
                            Currency = order.OrderCurrency,
                            ZipDest = order.OrderCustomer.Zip
                        };

                        if (preOrder.CountryDest == null)
                        {
                            var country = CountryService.GetCountry(SettingsMain.SellerCountryId);
                            if (country != null)
                                preOrder.CountryDest = country.Name;
                        }

                        if (preOrder.RegionDest == null)
                        {
                            var region = RegionService.GetRegion(SettingsMain.SellerRegionId);
                            if (region != null)
                                preOrder.RegionDest = region.Name;
                        }

                        var items = order.OrderItems.Select(shpItem => new PreOrderItem(shpItem)).ToList();

                        var shippingManager = new ShippingManager(preOrder, items, false, lead.Sum - lead.ShippingCost);
                        var shippingRate = shippingManager.GetOptions().FirstOrDefault(x => x.MethodId == shipping.ShippingMethodId);
                        if (shippingRate != null)
                        {
                            order.ShippingMethodId = shipping.ShippingMethodId;
                            order.ShippingCost = shippingRate.FinalRate;
                            order.ArchivedShippingName = shipping.Name;
                            order.ShippingTaxType = shipping.TaxType;
                            order.ShippingPaymentMethodType = shipping.PaymentMethodType;
                            order.ShippingPaymentSubjectType = shipping.PaymentSubjectType;
                        }
                    }
                }

                var leadFields = LeadFieldService.GetLeadFieldsWithValue(lead.Id);
                if (leadFields.Any())
                {
                    order.AdminOrderComment = string.Empty;
                    foreach (var field in leadFields)
                    {
                        if (field.Value.IsNullOrEmpty())
                            continue;
                        order.AdminOrderComment += string.Format("{0}: {1}\r\n", field.Name, field.FieldType == Core.Services.Crm.LeadFields.LeadFieldType.Date ? field.ValueDateFormat : field.Value);
                    }
                }

                order.OrderID = AddOrder(order, changedBy != null ? new OrderChangedBy(changedBy.Name) : null, true, autocompleteLeads);

                OrderStatusService.ChangeOrderStatus(order.OrderID, order.OrderStatusId, LocalizationService.GetResource("Core.OrderStatus.Created"), false);

                if (order.OrderCustomer != null)
                {
                    SendOrderMail(order);
                    SmsNotifier.SendSmsOnOrderAdded(order);
                }

                // Update lead status
                var funnel = SalesFunnelService.Get(lead.SalesFunnelId);
                if (funnel != null)
                {
                    var dealStatus = DealStatusService.GetList(funnel.Id).FirstOrDefault(x => x.Status == SalesFunnelStatusType.FinalSuccess);
                    if (dealStatus != null)
                    {
                        lead.DealStatusId = dealStatus.Id;
                    }
                    else
                    {
                        lead.SalesFunnelId = SettingsCrm.DefaultSalesFunnelId;
                        lead.DealStatusId = SettingsCrm.DefaultFinalDealStatusId;
                    }
                    LeadService.UpdateLead(lead, false, changedBy);
                }

                LeadsHistoryService.AddOrder(lead.Id, order, changedBy);

                return order;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }

        public static Order CreateOrder(Booking booking)
        {
            return CreateOrder(booking, null);
        }

        public static Order CreateOrder(Booking booking, ChangedBy changedBy)
        {
            try
            {
                if (booking.OrderId.HasValue)
                    throw new Exception("Для брони уже создан заказ");

                var order = new Order()
                {
                    OrderCurrency = (Currency)booking.BookingCurrency,
                    OrderStatusId = OrderStatusService.DefaultOrderStatus,
                    ManagerId = booking.ManagerId,
                    OrderDate = DateTime.Now,
                    OrderDiscount = booking.BookingDiscount,
                    OrderDiscountValue = booking.BookingDiscountValue,
                    OrderSourceId = booking.OrderSourceId,
                    PaymentMethodId = booking.PaymentMethodId.HasValue ? booking.PaymentMethodId.Value : 0,
                    ArchivedPaymentName = booking.ArchivedPaymentName,
                    PaymentCost = booking.PaymentCost,
                    PaymentDetails = booking.PaymentDetails,

                    IsFromAdminArea = true
                };

                if (booking.CustomerId != null && booking.Customer != null)
                {
                    order.OrderCustomer = (OrderCustomer)booking.Customer;
                    var contact = booking.Customer.Contacts.FirstOrDefault();

                    if (contact != null)
                    {
                        order.OrderCustomer.Country = contact.Country;
                        order.OrderCustomer.City = contact.City;
                        order.OrderCustomer.District = contact.District;
                        order.OrderCustomer.Region = contact.Region;
                        order.OrderCustomer.Zip = contact.Zip;
                        order.OrderCustomer.Apartment = contact.Apartment;
                        order.OrderCustomer.Entrance = contact.Entrance;
                        order.OrderCustomer.Floor = contact.Floor;
                        order.OrderCustomer.House = contact.House;
                        order.OrderCustomer.Street = contact.Street;
                        order.OrderCustomer.Structure = contact.Structure;
                    }
                }
                else
                {
                    order.OrderCustomer = new OrderCustomer()
                    {
                        CustomerID = Guid.NewGuid(),
                        FirstName = booking.FirstName,
                        LastName = booking.LastName,
                        Patronymic = booking.Patronymic,
                        Email = booking.Email,
                        Phone = booking.Phone,
                        StandardPhone = booking.StandardPhone,
                    };
                }

                if (booking.BookingItems != null && booking.BookingItems.Count > 0)
                {
                    foreach (var item in booking.BookingItems)
                        order.OrderItems.Add((OrderItem)item);
                }
                else
                {
                    order.OrderItems.Add(new OrderItem()
                    {
                        Name = "Оплата брони " + booking.Id,
                        ArtNo = "",
                        Price = booking.Sum,
                        Amount = 1,
                        TypeItem = TypeOrderItem.BookingService
                    });
                }

                var orderChangedBy = changedBy != null ? new OrderChangedBy(changedBy.Name) { CustomerId = changedBy.CustomerId } : new OrderChangedBy(CustomerContext.CurrentCustomer);
                orderChangedBy.Name += " (FromBooking)";
                order.OrderID = AddOrder(order, orderChangedBy, true);

                if (order.OrderCustomer != null && !string.IsNullOrWhiteSpace(order.OrderCustomer.Email))
                {
                    SendOrderMail(order);
                }

                booking.OrderId = order.OrderID;
                BookingService.Update(booking, trackChanges: false);
                BookingHistoryService.AddOrder(booking.Id, order, changedBy);

                if (booking.Payed)
                {
                    PayOrder(order.OrderID, true, true, true, orderChangedBy);
                }

                return order;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }

        public static bool CheckAccess(Order order)
        {
            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsAdmin || customer.IsVirtual)
                return true;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersOrderConstraint == ManagersOrderConstraint.Assigned &&
                        order.ManagerId != manager.ManagerId)
                        return false;

                    if (SettingsManager.ManagersOrderConstraint == ManagersOrderConstraint.AssignedAndFree &&
                        order.ManagerId != manager.ManagerId && order.ManagerId != null)
                        return false;

                    return true;
                }
            }
            return false;
        }

        [Obsolete("Use RecalculateOrderItemsToSum", true)]
        public static List<OrderItem> RecalculateItemsPriceIncludingAllDiscounts(List<OrderItem> oldItems, float shipping, float total, bool fixAmount = false, bool secondTry = false, bool onlyWithPrice = false)
        {
            if (oldItems == null || !oldItems.Any() || oldItems.Sum(x => x.Amount * x.Price) == 0)
                return oldItems;

            var newItems = (onlyWithPrice ? oldItems.Where(x => x.Price > 0).ToList() : oldItems).DeepClone();

            if (fixAmount)
            {
                foreach (var item in newItems.Where(x => x.Amount % 1 > 0))
                {
                    item.Price = item.Price * item.Amount;
                    item.Weight = item.Weight * item.Amount;

                    var min = Math.Min(Math.Min(item.Length, item.Width), item.Height);
                    if (item.Length == min)
                        item.Length = item.Length * item.Amount;
                    else if (item.Width == min)
                        item.Width = item.Width * item.Amount;
                    else if (item.Height == min)
                        item.Height = item.Height * item.Amount;

                    item.Amount = 1;
                }
            }

            var productsTotal = oldItems.Sum(x => (float)Math.Round(x.Amount * x.Price, 2));

            var div = total - shipping - productsTotal;
            if (div != 0)
            {
                foreach (var item in newItems)
                {
                    item.Price = (float) Math.Round(item.Price + (item.Price/productsTotal*div), 2);
                }

                var newTotal = newItems.Sum(x => (float) Math.Round(x.Amount * x.Price, 2));
                if (newTotal != total - shipping)
                {
                    var item = newItems.FirstOrDefault(x => x.Amount == 1f) ?? newItems.Last();
                    item.Price = (float) Math.Round(item.Price + ((total - shipping - newTotal) / item.Amount), 2);
                }
            }

            if (!fixAmount && !secondTry)
            {
                var productsTotalNew = newItems.Sum(x => (float)Math.Round(x.Amount * x.Price, 2));
                var divNew = total - shipping - productsTotalNew;
                if (divNew != 0)
                    return RecalculateItemsPriceIncludingAllDiscounts(oldItems, shipping, total, true, true, onlyWithPrice);
            }

            return onlyWithPrice ? newItems.Where(x => x.Price > 0).ToList() : newItems;
        }

        public static List<int> GetProductIdsByCustomer(Guid customerId)
        {
            return SQLDataAccess.Query<int>(
                "Select Distinct [OrderItems].[ProductId] " +
                "From [Order].[Order] " +
                "Inner Join [Order].[OrderCustomer] on [OrderCustomer].[OrderID] = [Order].[OrderID] " +
                "Inner Join [Order].[OrderItems] on [OrderItems].[OrderID] = [Order].[OrderID] " +
                "Where CustomerId=@customerId and [OrderItems].[ProductId] is not null and IsDraft = 0",
                new { customerId }).ToList();
        }

        public static bool IsCustomerHasPaidOrderWithProducts(Guid customerId, List<int> productIds)
        {
            var check =
                productIds != null && productIds.Count > 0 &&
                SQLDataHelper.GetBoolean(SQLDataAccess.ExecuteScalar(
                    "Select 1 " +
                    "From [Order].[Order] " +
                    "Inner Join [Order].[OrderCustomer] on [OrderCustomer].[OrderID] = [Order].[OrderID] " +
                    "Inner Join [Order].[OrderItems] on [OrderItems].[OrderID] = [Order].[OrderID] " +
                    "Where CustomerId=@customerId and " +
                    "[OrderItems].[ProductId] in (" + String.Join(",", productIds) + ") and " +
                    "IsDraft = 0 and PaymentDate is not null",
                    CommandType.Text,
                    new SqlParameter("@customerId", customerId)));

            return check;
        }

        public static bool IsCustomerHasConfirmedOrders(Guid customerId)
        {
            return SQLDataAccess2.ExecuteScalar<bool>(
                "IF EXISTS (SELECT 1 FROM [Order].[Order] o INNER JOIN [Order].OrderCustomer oc ON oc.OrderId = o.OrderID " +
                "INNER JOIN [Order].OrderStatus os ON os.OrderStatusID = o.OrderStatusID " +
                "WHERE os.IsCanceled = 0 AND oc.CustomerID = @CustomerId) " +
                "SELECT 1 ELSE SELECT 0", new { customerId });
        }

        public static int? GetNextOrder(int currentOrderId, Manager manager)
        {
            var sql = "SELECT TOP(1) [OrderID] FROM [Order].[Order] WHERE [OrderID] > @OrderID";
            if (manager != null && manager.Enabled && SettingsManager.ManagersOrderConstraint != ManagersOrderConstraint.All)
            {
                if (SettingsManager.ManagersOrderConstraint == ManagersOrderConstraint.Assigned)
                    sql += " AND [Order].ManagerId = @ManagerId";
                else if (SettingsManager.ManagersOrderConstraint == ManagersOrderConstraint.AssignedAndFree)
                    sql += " AND ([Order].ManagerId = @ManagerId or [Order].ManagerId is null)";
            }
            sql += " ORDER BY [OrderID]";

            return SQLDataAccess.ExecuteReadOne<int?>(sql, CommandType.Text,
                reader => SQLDataHelper.GetNullableInt(reader, "OrderID"),
                new SqlParameter("@OrderID", currentOrderId),
                new SqlParameter("@ManagerId", manager != null ? manager.ManagerId : 0));
        }

        public static int? GetPrevOrder(int currentOrderId, Manager manager)
        {
            var sql = "SELECT TOP(1) [OrderID] FROM [Order].[Order] WHERE [OrderID] < @OrderID";
            if (manager != null && manager.Enabled && SettingsManager.ManagersOrderConstraint != ManagersOrderConstraint.All)
            {
                if (SettingsManager.ManagersOrderConstraint == ManagersOrderConstraint.Assigned)
                    sql += " AND [Order].ManagerId = @ManagerId";
                else if (SettingsManager.ManagersOrderConstraint == ManagersOrderConstraint.AssignedAndFree)
                    sql += " AND ([Order].ManagerId = @ManagerId or [Order].ManagerId is null)";
            }
            sql += " ORDER BY [OrderID] DESC";

            return SQLDataAccess.ExecuteReadOne<int?>(sql, CommandType.Text,
                reader => SQLDataHelper.GetNullableInt(reader, "OrderID"),
                new SqlParameter("@OrderID", currentOrderId),
                new SqlParameter("@ManagerId", manager != null ? manager.ManagerId : 0));
        }

        public static string GetOrderTrackNumber(int orderId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT [TrackNumber] FROM [Order].[Order] WHERE [OrderID] = @OrderId",
                CommandType.Text,
                reader =>
                    SQLDataHelper.GetString(reader, "TrackNumber"),
                new SqlParameter("@OrderId", orderId));
        }
    }
}