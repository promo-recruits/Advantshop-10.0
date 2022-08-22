﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Shipping.Grastin;
using AdvantShop.Core.Services.Shipping.Grastin.Api;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models.Orders.Grastin;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Grastin
{
    public class GrastinGetFormSendOrderForPartner
    {
        private readonly int _orderId;

        public GrastinGetFormSendOrderForPartner(int orderId)
        {
            _orderId = orderId;
        }

        public SendOrderForPartnerModel Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return null;

            if (order.ShippingMethod == null || order.ShippingMethod.ShippingType != ((ShippingKeyAttribute)
                typeof(Shipping.Grastin.Grastin).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                //Errors.Add("Неверный метод доставки");
                return null;
            }

            var shippingMethod = order.ShippingMethod;
            var preOrder = PreOrder.CreateFromOrder(order);
            preOrder.IsFromAdminArea = true;
            var items = order.OrderItems.Select(x => new PreOrderItem(x)).ToList();

            var grastinMethod = new Shipping.Grastin.Grastin(shippingMethod, preOrder, items);

            var orderSum = order.Sum;
            //var shippingCost = order.ShippingCostWithDiscount;
            var shippingCurrency = shippingMethod != null ? shippingMethod.ShippingCurrency : null;

            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                //order.OrderItems.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                //shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                orderSum = orderSum.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            var model = new SendOrderForPartnerModel()
            {
                OrderId = _orderId,
                OrderNumber = order.Number,
                OrderPrefix = grastinMethod.OrderPrefix,
                Seats = 1,
                AssessedCost = grastinMethod.Insure ? orderSum : 1f,
                Weight = grastinMethod.GetTotalWeight()
            };

            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.Cash)),
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.CashOnDelivery)),
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.PickPoint))
            };
            model.CashOnDelivery = !order.Payed || (order.PaymentMethod != null && paymentsCash.Contains(order.PaymentMethod.PaymentKey));

            GrastinEventWidgetData grastinEventWidget = null;
            var orderPickPoint = OrderService.GetOrderPickPoint(_orderId);
            if (orderPickPoint != null && orderPickPoint.AdditionalData.IsNotEmpty())
            {
                try
                {
                    grastinEventWidget = JsonConvert.DeserializeObject<GrastinEventWidgetData>(orderPickPoint.AdditionalData);
                }
                catch (Exception)
                {
                    // ignored
                }
            }


            if (order.OrderCustomer != null)
            {
                model.Buyer = string.Join(" ",
                    (new[] { order.OrderCustomer.LastName, order.OrderCustomer.FirstName, order.OrderCustomer.Patronymic })
                        .Where(x => !string.IsNullOrEmpty(x)));

                var phone = order.OrderCustomer.StandardPhone.HasValue
                    ? order.OrderCustomer.StandardPhone.Value.ToString()
                    : string.Empty;

                if (phone.StartsWith("7"))
                    phone = "8" + phone.Substring(1);

                if (phone.Length == 10)
                    phone = "8" + phone;

                model.Phone = phone;
                model.Email = order.OrderCustomer.Email;

                if (grastinEventWidget == null || grastinEventWidget.DeliveryType != EnDeliveryType.PickPoint)
                {
                    var comments = new List<string>();

                    if (order.OrderCustomer.Apartment.IsNotEmpty())
                        comments.Add("кв. " + order.OrderCustomer.Apartment);

                    if (order.OrderCustomer.Structure.IsNotEmpty())
                        comments.Add("строение/Корпус " + order.OrderCustomer.Structure);

                    if (order.OrderCustomer.Entrance.IsNotEmpty())
                        comments.Add("подъезд " + order.OrderCustomer.Entrance);

                    if (order.OrderCustomer.Floor.IsNotEmpty())
                        comments.Add("этаж " + order.OrderCustomer.Floor);

                    model.Comment = string.Join(", ", comments);
                }
            }

            model.PointId = grastinEventWidget != null
                ? grastinEventWidget.PickPointId
                : null;

            model.TakeWarehouse = grastinEventWidget != null
                ? grastinEventWidget.CityFrom
                : null;

            if (string.IsNullOrEmpty(model.TakeWarehouse))
                model.TakeWarehouse = grastinMethod.WidgetFromCity;

            var service = new GrastinApiService(grastinMethod.ApiKey);

            var city = grastinEventWidget != null && !string.IsNullOrEmpty(grastinEventWidget.CityTo)
                ? grastinEventWidget.CityTo
                : order.OrderCustomer != null ? order.OrderCustomer.City : string.Empty;

            var partnerSelfPickup = service.GetPartnerSelfPickups(city);
            model.AddressPoints = partnerSelfPickup != null
                ? partnerSelfPickup.Select(x => new SelectListItem()
                {
                    Text = x.Address,
                    Value = x.Id,
                }).ToList()
                : new List<SelectListItem>();

            var takeWarehouses = service.GetWarehouses();
            model.TakeWarehouses = takeWarehouses != null
                ? takeWarehouses.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Name
                }).ToList()
                : new List<SelectListItem>();

            return model;
        }
    }
}
