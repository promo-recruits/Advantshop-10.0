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
    public class GrastinGetFormSendOrderForRussianPost
    {
        private readonly int _orderId;

        public GrastinGetFormSendOrderForRussianPost(int orderId)
        {
            _orderId = orderId;
        }

        public SendOrderForRussianPostModel Execute()
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

            var model = new SendOrderForRussianPostModel()
            {
                OrderId = _orderId,
                OrderNumber = order.Number,
                OrderPrefix = grastinMethod.OrderPrefix,
                AssessedCost = grastinMethod.Insure ? orderSum : 1f,
                DeliveryDate = DateTime.Now,
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
                    (new[] {order.OrderCustomer.LastName, order.OrderCustomer.FirstName, order.OrderCustomer.Patronymic})
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

                model.Index = order.OrderCustomer.Zip;
                model.Region = order.OrderCustomer.Region;
                model.City = order.OrderCustomer.City;

                var address = new List<string>() {order.OrderCustomer.Street};

                if (order.OrderCustomer.Apartment.IsNotEmpty())
                    address.Add("кв. " + order.OrderCustomer.Apartment);

                if (order.OrderCustomer.Structure.IsNotEmpty())
                    address.Add("строение/Корпус " + order.OrderCustomer.Structure);

                model.Address = string.Join(", ", address);

                var comments = new List<string>();

                if (order.OrderCustomer.Entrance.IsNotEmpty())
                    comments.Add("подъезд " + order.OrderCustomer.Entrance);

                if (order.OrderCustomer.Floor.IsNotEmpty())
                    comments.Add("этаж " + order.OrderCustomer.Floor);

                model.Comment = string.Join(", ", comments);
            }

            model.TakeWarehouse = grastinEventWidget != null
                ? grastinEventWidget.CityFrom
                : null;

            model.Services =
                Enum.GetValues(typeof (EnRussianPostService))
                    .Cast<EnRussianPostService>()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int) x).ToString()
                    })
                    .ToList();

            model.Service = EnRussianPostService.PostalShipping;

            if (string.IsNullOrEmpty(model.TakeWarehouse))
                model.TakeWarehouse = grastinMethod.WidgetFromCity;

            var service = new GrastinApiService(grastinMethod.ApiKey);

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
