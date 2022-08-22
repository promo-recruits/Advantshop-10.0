using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Orders
{
    public static class OrderExtensions
    {

        public static int GetOrdersCount(this OrderStatus orderStatus)
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "SELECT Count(*) FROM [Order].[Order] WHERE [OrderStatusID] = @OrderStatusID and IsDraft = 0",
                CommandType.Text, new SqlParameter("@OrderStatusID", orderStatus.StatusID)));
        }

        public static bool ShowBillingLink(this Order order)
        {
            var show = order != null && order.OrderCustomer != null && !order.Payed && !order.IsDraft && !(order.OrderStatus != null && order.OrderStatus.IsCanceled);
            return show;
        }

        public static float GetOrderDiscountPrice(this Order order)
        {
            var totalDiscount = order.OrderDiscount > 0
                                    ? order.OrderDiscount*order.OrderItems.Where(x => !x.IgnoreOrderDiscount).Sum(x => x.Price*x.Amount)/100
                                    : 0;

            totalDiscount += order.OrderDiscountValue;

            return totalDiscount;
        }

        public static float GetOrderCouponPrice(this Order order)
        {
            var couponPrice = 0f;

            if (order.Coupon != null && order.Coupon.Value != 0)
            {
                switch (order.Coupon.Type)
                {
                    case CouponType.Fixed:
                        var productsPrice = order.OrderItems.Where(x => x.IsCouponApplied).Sum(x => x.Price*x.Amount);
                        couponPrice = productsPrice >= order.Coupon.Value ? order.Coupon.Value : productsPrice;
                        break;
                    case CouponType.Percent:
                        couponPrice = order.OrderItems.Where(x => x.IsCouponApplied).Sum(x => order.Coupon.Value*x.Price/100*x.Amount);
                        break;
                }
            }

            return couponPrice;
        }

        /// <summary>
        /// Street + House + Apartment + Structure + Entrance + Floor
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static string GetCustomerAddress(this OrderCustomer customer)
        {
            if (customer == null)
                return "";
            
            var result = customer.Street ?? "";

            if (!string.IsNullOrEmpty(customer.House))
                result += " " + LocalizationService.GetResource("Core.Orders.OrderContact.House") + " " + customer.House;

            if (!string.IsNullOrEmpty(customer.Structure))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Structure") + " " + customer.Structure;

            if (!string.IsNullOrEmpty(customer.Apartment))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Apartment") + " " + customer.Apartment;

            if (!string.IsNullOrEmpty(customer.Entrance))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Entrance") + " " + customer.Entrance;

            if (!string.IsNullOrEmpty(customer.Floor))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Floor") + " " + customer.Floor;


            return result;
        }

        public static string GetCustomerAddress(this CustomerContact customer)
        {
            if (customer == null)
                return "";

            var result = customer.Street ?? "";

            if (!string.IsNullOrEmpty(customer.House))
                result += " " + LocalizationService.GetResource("Core.Orders.OrderContact.House") + " " + customer.House;

            if (!string.IsNullOrEmpty(customer.Apartment))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Apartment") + " " + customer.Apartment;

            if (!string.IsNullOrEmpty(customer.Structure))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Structure") + " " + customer.Structure;

            if (!string.IsNullOrEmpty(customer.Entrance))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Entrance") + " " + customer.Entrance;

            if (!string.IsNullOrEmpty(customer.Floor))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Floor") + " " + customer.Floor;


            return result;
        }

        /// <summary>
        /// Street + House + Apartment
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static string GetCustomerShortAddress(this OrderCustomer customer)
        {
            if (customer == null)
                return "";
            
            var result = customer.Street ?? "";

            if (!string.IsNullOrEmpty(customer.House))
                result += " " + LocalizationService.GetResource("Core.Orders.OrderContact.House") + customer.House;

            if (!string.IsNullOrEmpty(customer.Apartment))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Apartment") + customer.Apartment;
            
            return result;
        }

        public static string GetCustomerAddress(this CheckoutAddress address)
        {
            if (address == null)
                return "";
            
            var result = address.Street ?? "";

            if (!string.IsNullOrEmpty(address.House))
                result += " " + LocalizationService.GetResource("Core.Orders.OrderContact.House") + " " + address.House;

            if (!string.IsNullOrEmpty(address.Apartment))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Apartment") + " " + address.Apartment;

            if (!string.IsNullOrEmpty(address.Structure))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Structure") + " " + address.Structure;

            if (!string.IsNullOrEmpty(address.Entrance))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Entrance") + " " + address.Entrance;

            if (!string.IsNullOrEmpty(address.Floor))
                result += ", " + LocalizationService.GetResource("Core.Orders.OrderContact.Floor") + " " + address.Floor;


            return result;
        }

        public static Card GetOrderBonusCard(this Order order)
        {
            Card bonusCard = null;
            if (order.BonusCardNumber != null)
                bonusCard = BonusSystemService.GetCard(order.BonusCardNumber);

            if (bonusCard == null && order.OrderCustomer != null)
            {
                var customer = CustomerService.GetCustomer(order.OrderCustomer.CustomerID);
                if (customer != null)
                    bonusCard = BonusSystemService.GetCard(customer.Id);
            }

            return bonusCard;
        }

        public static IEnumerable<OrderItem> GetOrderItemsForFiscal(this Order order, bool toIntegerAmount = false)
        {
            return GetOrderItemsForFiscal(order, order.OrderCurrency, toIntegerAmount);
        }

        public static IEnumerable<OrderItem> GetOrderItemsForFiscal(this Order order, Currency currency, bool toIntegerAmount = false)
        {
            currency = currency ?? order.OrderCurrency;

            var orderItems = toIntegerAmount
                ? order.OrderItems.CeilingAmountToInteger()
                : order.OrderItems;

            var recalculate =
                new RecalculateOrderItemsToSum(orderItems.ConvertCurrency(order.OrderCurrency, currency))
                {
                    AcceptableDifference = 0f
                };

            var sum = order.Sum.ConvertCurrency(order.OrderCurrency, currency);
            var shippingCostWithDiscount = order.ShippingCostWithDiscount.ConvertCurrency(order.OrderCurrency, currency);

            return recalculate.ToSum(sum - shippingCostWithDiscount);
        }
    }
}
