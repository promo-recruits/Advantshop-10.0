﻿using System.Collections.Generic;
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
    public class GrastinSendOrderForPartner
    {
        private readonly SendOrderForPartnerModel _model;

        public List<string> Errors { get; set; }

        public GrastinSendOrderForPartner(SendOrderForPartnerModel model)
        {
            _model = model;
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
                        typeof(Shipping.Grastin.Grastin).GetCustomAttributes(typeof(ShippingKeyAttribute), false)
                            .First())
                        .Value)
                {
                    var grastinMethod = new Shipping.Grastin.Grastin(shippingMethod, null, null);

                    var service = new GrastinApiService(grastinMethod.ApiKey);

                    var partnerOrder = new PartnerOrder()
                    {
                        Number = string.Format("{0}{1}", grastinMethod.OrderPrefix, order.Number),
                        Comment = _model.Comment.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Buyer = _model.Buyer.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Phone = _model.Phone.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Weight = _model.Weight,
                        Seats = _model.Seats,
                        //IsTest = false,
                        TakeWarehouse = _model.TakeWarehouse,
                        SiteName = SettingsMain.ShopName.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        Email = _model.Email,
                        CargoType = _model.CargoType.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        BarCode = _model.BarCode.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                        PickupId = _model.PointId,
                        AssessedCost = _model.AssessedCost
                    };

                    if (order.OrderItems != null && order.OrderItems.Count > 0)
                    {
                        var recalculateOrderItems = new RecalculateOrderItemsToSum(order.OrderItems);
                        recalculateOrderItems.AcceptableDifference = 0.1f;

                        var orderItems = recalculateOrderItems.ToSum(order.Sum - order.ShippingCostWithDiscount);
                        partnerOrder.Products = orderItems.Select(x => new GrastinProduct()
                        {
                            ArtNo = x.ArtNo.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                            Name = x.Name.RemoveInvalidXmlChars().RemoveEscapeXmlChars(),
                            Price = x.Price,
                            Amount = x.Amount
                        }).ToList();

                        if (order.ShippingCostWithDiscount > 0)
                        {
                            partnerOrder.Products.Add(new GrastinProduct()
                            {
                                ArtNo = "Доставка",
                                Name = "Доставка",
                                Price = order.ShippingCostWithDiscount,
                                Amount = 1
                            });
                        }

                        partnerOrder.OrderSum = partnerOrder.Products.Sum(x => x.Price * x.Amount);
                    }
                    else
                    {
                        partnerOrder.OrderSum = order.Sum;
                    }

                    if (!_model.CashOnDelivery)
                        partnerOrder.OrderSum = 0f;

                    var response = service.AddPartnerOrder(new PartnerOrderContainer() { Orders = new List<PartnerOrder>() { partnerOrder } });


                    if (response != null && response.Count == 1)
                    {
                        if (string.IsNullOrEmpty(response[0].Error))
                        {
                            OrderService.AddUpdateOrderAdditionalData(order.OrderID, Shipping.Grastin.Grastin.KeyNameIsSendOrderInOrderAdditionalData, true.ToString());

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
