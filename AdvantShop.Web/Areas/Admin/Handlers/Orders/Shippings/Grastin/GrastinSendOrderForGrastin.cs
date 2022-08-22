﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Shipping.Grastin.Api;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models.Orders.Grastin;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Grastin
{
    public class GrastinSendOrderForGrastin
    {
        private readonly SendOrderForGrastinModel _model;

        public List<string> Errors { get; set; }

        public GrastinSendOrderForGrastin(SendOrderForGrastinModel model)
        {
            _model = model;
        }

        private string GetDeliveryTime(EnCourierService? serviceType, string dTime)
        {
            var newTime = serviceType != EnCourierService.TransportCompany ? dTime : string.Empty;
            //якобы они целые часы принимаю без минут
            newTime = newTime.Replace(":00", "");
            return newTime;
        }

        public bool Execute()
        {
            var order = OrderService.GetOrder(_model.OrderId);
            if (order != null)
            {
                var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
                if (shippingMethod != null &&
                    shippingMethod.ShippingType ==
                    ((ShippingKeyAttribute)
                        typeof (Shipping.Grastin.Grastin).GetCustomAttributes(typeof (ShippingKeyAttribute), false)
                            .First())
                        .Value)
                {
                    var grastinMethod = new Shipping.Grastin.Grastin(shippingMethod, null, null);

                    var service = new GrastinApiService(grastinMethod.ApiKey);

                    var address = _model.Service == EnCourierService.PickupWithoutPaying
                                  || _model.Service == EnCourierService.PickupWithPayment
                                  || _model.Service == EnCourierService.PickupWithCashServices
                                  || _model.Service == EnCourierService.PickupWithCreditCard
                                  || _model.Service == EnCourierService.ExchangeReturnOfGoodsOnPickup
                        ? _model.AddressPoint
                        : _model.AddressCourier.RemoveInvalidXmlChars().RemoveEscapeXmlChars();


                    var grastinOrder = new GrastinOrder()
                    {
                        Number = string.Format("{0}{1}", grastinMethod.OrderPrefix, order.Number),
                        Address = address,
                        Comment = _model.Comment.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        DeiveryTimeFrom = GetDeliveryTime(_model.Service , _model.DeiveryTimeFrom),
                        DeiveryTimeTo = GetDeliveryTime(_model.Service , _model.DeiveryTimeTo),
                        DeiveryDate = _model.DeliveryDate.Value,
                        Buyer = _model.Buyer.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        AssessedCost = _model.AssessedCost,
                        Phone1 = _model.Phone.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Phone2 = _model.Phone2.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Service = _model.Service.Value,
                        Seats = _model.Seats,
                        //IsTest = false,
                        TakeWarehouse = _model.TakeWarehouse,
                        SiteName = SettingsMain.ShopName.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Email = _model.Email,
                        CargoType = _model.CargoType.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        BarCode = _model.BarCode.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                    };

                    if (_model.Service == EnCourierService.TransportCompany)
                    {
                        grastinOrder.OfficeId = _model.OfficeId;
                        grastinOrder.TypeRecipient = _model.TypeRecipient;
                        grastinOrder.Index = _model.Index.RemoveInvalidXmlChars().RemoveEscapeXmlChars();
                        grastinOrder.AddressForTransportCompany = _model.AddressCourier.RemoveInvalidXmlChars().RemoveEscapeXmlChars();
                        grastinOrder.BuyerForTransportCompany = _model.Buyer.RemoveInvalidXmlChars().RemoveEscapeXmlChars();
                        grastinOrder.PhoneForTransportCompany = _model.Phone;
                        if (_model.TypeRecipient == EnTypeRecipient.Individual)
                            grastinOrder.Passport = _model.Passport.RemoveInvalidXmlChars().RemoveEscapeXmlChars();
                        if (_model.TypeRecipient == EnTypeRecipient.Organization)
                        {
                            grastinOrder.Organization = _model.Organization.RemoveInvalidXmlChars().RemoveEscapeXmlChars();
                            grastinOrder.Inn = _model.Inn.RemoveInvalidXmlChars().RemoveEscapeXmlChars();
                            grastinOrder.Kpp = _model.Kpp.RemoveInvalidXmlChars().RemoveEscapeXmlChars();
                        }
                    }

                    if (order.OrderItems != null && order.OrderItems.Count > 0)
                    {
                        var recalculateOrderItems = new RecalculateOrderItemsToSum(order.OrderItems);
                        recalculateOrderItems.AcceptableDifference = 0.1f;

                        var orderItems = recalculateOrderItems.ToSum(order.Sum - order.ShippingCostWithDiscount);
                        grastinOrder.Products = orderItems.Select(x => new GrastinProduct()
                        {
                            ArtNo = x.ArtNo.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                            Name = x.Name.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                            Price = x.Price,
                            Amount = x.Amount
                        }).ToList();

                        if (order.ShippingCostWithDiscount > 0)
                        {
                            grastinOrder.Products.Add(new GrastinProduct()
                            {
                                ArtNo = "Доставка",
                                Name = "Доставка",
                                Price = order.ShippingCostWithDiscount,
                                Amount = 1
                            });
                        }

                        grastinOrder.OrderSum = grastinOrder.Products.Sum(x => x.Price*x.Amount);
                    }
                    else
                    {
                        grastinOrder.OrderSum = order.Sum;
                    }

                    var withoutPayments = new[]
                    {
                        EnCourierService.DeliverWithoutPayment,
                        EnCourierService.DeliverWithoutPaymentContactlessDelivery,
                        EnCourierService.PickupWithoutPaying,
                        EnCourierService.GreatDeliveryWithoutPayment, 
                    };
                    if (withoutPayments.Contains(_model.Service.Value) || !_model.CashOnDelivery)
                        grastinOrder.OrderSum = 0f;

                    var response = service.AddGrastinOrder(new GrastinOrderCourier() { Orders = new List<GrastinOrder>() { grastinOrder } });

                    if (response != null && response.Count == 1)
                    {
                        if (string.IsNullOrEmpty(response[0].Error))
                        {
                            OrderService.AddUpdateOrderAdditionalData(order.OrderID, Shipping.Grastin.Grastin.KeyNameIsSendOrderInOrderAdditionalData, true.ToString());

                            order.DeliveryDate = grastinOrder.DeiveryDate;
                            if (_model.Service != EnCourierService.TransportCompany)
                                order.DeliveryTime = string.Format("с {0} по {1}", grastinOrder.DeiveryTimeFrom,
                                    grastinOrder.DeiveryTimeTo);

                            var trackChanges = !order.IsDraft;

                            OrderService.UpdateOrderMain(order, updateModules: false, trackChanges: trackChanges);

                            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, order.ShippingMethod.ShippingType);

                            return true;
                        }

                        Errors = new List<string>() { response[0].Error };
                    }
                    else
                    {
                        Errors = service.LastActionErrors;
                    }
                }
            }

            return false;
        }
    }
}
