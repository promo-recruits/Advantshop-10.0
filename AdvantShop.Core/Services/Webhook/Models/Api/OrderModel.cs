using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Webhook.Models.Api
{
    public class OrderModel
    {
        public static OrderModel FromOrder(Order order)
        {
            if (order == null || order.IsDraft)
                return null;

            return new OrderModel
            {
                Id = order.OrderID,
                Number = order.Number,
                Currency = order.OrderCurrency != null ? order.OrderCurrency.CurrencyCode : null,
                Sum = order.Sum,
                Date = order.OrderDate,

                CustomerComment = order.CustomerComment,
                AdminComment = order.AdminOrderComment,

                PaymentName = order.ArchivedPaymentName,
                PaymentCost = order.PaymentCost,

                ShippingName = order.ArchivedShippingName,
                ShippingCost = order.ShippingCost,
                ShippingTaxName = order.ShippingTaxType.Localize(),
                TrackNumber = order.TrackNumber,
                DeliveryDate = order.DeliveryDate,
                DeliveryTime = order.DeliveryTime,

                OrderDiscount = order.OrderDiscount,
                OrderDiscountValue = order.OrderDiscountValue,

                BonusCardNumber = order.BonusCardNumber,
                BonusCost = order.BonusCost,

                LpId = order.LpId,

                IsPaid = order.Payed,

                Customer = OrderCustomerModel.FromOrderCustomer(order.OrderCustomer),

                Status = OrderStatusModel.FromOrderStatus(order.OrderStatus),

                Source = OrderSourceModel.FromOrderSource(order.OrderSource),

                Items = order.OrderItems != null
                    ? order.OrderItems.Select(OrderItemModel.FromOrderItem).ToList()
                    : new List<OrderItemModel>()
            };
        }

        public int Id { get; set; }
        public string Number { get; set; }
        public string Currency { get; set; }
        public float Sum { get; set; }
        public DateTime Date { get; set; }

        public string CustomerComment { get; set; }
        public string AdminComment { get; set; }

        public string PaymentName { get; set; }
        public float PaymentCost { get; set; }

        public string ShippingName { get; set; }
        public float ShippingCost { get; set; }
        public string ShippingTaxName { get; set; }
        public string TrackNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }

        public float OrderDiscount { get; set; }
        public float OrderDiscountValue { get; set; }

        public long? BonusCardNumber { get; set; }
        public float BonusCost { get; set; }

        public int? LpId { get; set; }

        public bool IsPaid { get; set; }
        public OrderCustomerModel Customer { get; set; }
        public OrderStatusModel Status { get; set; }
        public OrderSourceModel Source { get; set; }

        public List<OrderItemModel> Items { get; set; }
    }
}
