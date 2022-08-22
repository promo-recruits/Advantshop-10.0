using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Pec;
using AdvantShop.Shipping.Pec.Api;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Pec
{
    public class PecCreateOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public PecCreateOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId, Shipping.Pec.Pec.KeyNameCargoCodeInOrderAdditionalData);
            if (!string.IsNullOrEmpty(orderAdditionalData))
            {
                Errors.Add("Заказ уже передан");
                return false;
            }

            var order = OrderService.GetOrder(_orderId);
            if (order == null)
            {
                Errors.Add("Заказ не найден");
                return false;
            }

            if (order.ShippingMethod == null || order.ShippingMethod.ShippingType != ((ShippingKeyAttribute)
                typeof(Shipping.Pec.Pec).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                Errors.Add("Неверный метод доставки");
                return false;
            }

            var orderPickPoint = OrderService.GetOrderPickPoint(_orderId);
            if (orderPickPoint == null || orderPickPoint.AdditionalData.IsNullOrEmpty())
            {
                Errors.Add("Нет данных о параметрах рассчета доставки");
                return false;
            }

            PecCalculateOption pecCalculateOption = null;

            try
            {
                pecCalculateOption =
                    JsonConvert.DeserializeObject<PecCalculateOption>(orderPickPoint.AdditionalData);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }

            if (pecCalculateOption == null)
            {
                Errors.Add("Нет данных о параметрах рассчета доставки");
                return false;
            }

            var preOrder = PreOrder.CreateFromOrder(order);
            preOrder.IsFromAdminArea = true;
            var preOrderItems = order.OrderItems.Select(x => new PreOrderItem(x)).ToList();
            var pecMethod = new Shipping.Pec.Pec(order.ShippingMethod, preOrder, preOrderItems);

            if (pecMethod.SenderCityId == null)
            {
                Errors.Add("Не указан город отправитель");
                return false;
            }

            var paymentsCash = new[]
            {
                AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof(Payment.CashOnDelivery)),
            };

            var isDeliveryTypeToPoint = !pecCalculateOption.WithDelivery;
            var dimensions = pecMethod.GetDimensions().Select(x => x / 1000d).ToArray();// конвертируем сами, чтобы получить большую точность (float для таких значений сильно ограничен);
            var weight = pecMethod.GetTotalWeight();
            var mustPay = !order.Payed && order.PaymentMethod != null && paymentsCash.Contains(order.PaymentMethod.PaymentKey);

            var senderCity = FindSenderCity(pecMethod);
            if (senderCity.IsNullOrEmpty())
            {
                Errors.Add("Не указан город отправитель");
                return false;
            }

            string receiverCity = null;
            if (order.OrderCustomer != null)
            {
                // делаем замену только для поиска, сами данные пользователя не изменяем
                string outCountry, outRegion, outDistrict, outCity, outZip;
                ShippingReplaceGeoService.ReplaceGeo(
                        order.ShippingMethod.ShippingType,
                        order.OrderCustomer.Country, order.OrderCustomer.Region, order.OrderCustomer.District, order.OrderCustomer.City, order.OrderCustomer.Zip,
                        out outCountry, out outRegion, out outDistrict, out outCity, out outZip);

                receiverCity = FindReceiverCity(outCity, outDistrict, pecMethod);
            }
            if (receiverCity.IsNullOrEmpty())
            {
                Errors.Add("Не указан или не найден город получателя");
                return false;
            }

            var orderSum = order.Sum;
            var shippingCost = order.ShippingCostWithDiscount;
            var shippingCurrency = order.ShippingMethod.ShippingCurrency;

            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                //order.OrderItems.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                orderSum = orderSum.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            //var payer = new DeliveryPayer { Type = mustPay ? EnPaymentType.Receiver : EnPaymentType.Seller };
            var payer = new DeliveryPayer { Type = EnPaymentType.Seller };
            var pecOrder = new PreregistrationSubmitParams
            {
                Sender = new Sender
                {
                    City = senderCity,
                    Inn = pecMethod.SenderInn,
                    Title = pecMethod.SenderTitle,
                    Fs = pecMethod.SenderFs,
                    Phone = pecMethod.SenderPhone,
                    PhoneAdditional = pecMethod.SenderPhoneAdditional
                },
                Cargos = new List<PreregistrationSubmitCargo> {
                    new PreregistrationSubmitCargo
                    {
                        Common = new CargoCommon
                        {
                            Type = pecCalculateOption.TransportingType == EnTransportingType.Car.Value
                                ? EnTransportingTypeCargo.Car
                                : pecCalculateOption.TransportingType == EnTransportingType.Avia.Value
                                    ? EnTransportingTypeCargo.Avia
                                    : EnTransportingTypeCargo.Easyway,
                            Height = dimensions[2],
                            Width = dimensions[1],
                            Length = dimensions[0],
                            Weight = Math.Round(weight, 3),
                            DeclaredCost = orderSum - shippingCost,
                            OrderNumber = order.Number,
                            PositionsCount = 1
                        },
                        Receiver = new Receiver
                        {
                            City = receiverCity,
                            Title = order.OrderCustomer != null
                                ? string.Join(" ", (new[] { order.OrderCustomer.LastName, order.OrderCustomer.FirstName, order.OrderCustomer.Patronymic })
                                        .Where(x => !string.IsNullOrEmpty(x)))
                                : null,
                            Person = order.OrderCustomer != null
                                ? string.Join(" ", (new[] { order.OrderCustomer.LastName, order.OrderCustomer.FirstName, order.OrderCustomer.Patronymic })
                                        .Where(x => !string.IsNullOrEmpty(x)))
                                : null,
                            Phone = order.OrderCustomer != null
                                ? order.OrderCustomer.Phone
                                : null,
                            WarehouseId = isDeliveryTypeToPoint
                                ? orderPickPoint.PickPointId
                                : null,
                            AddressStock = isDeliveryTypeToPoint
                                ? null
                                : string.Join(
                                    ", ",
                                    new[] {
                                        order.OrderCustomer.Street,
                                        order.OrderCustomer.House, order.OrderCustomer.Structure,
                                        order.OrderCustomer.Apartment
                                    }.Where(x => x.IsNotEmpty()))
                        },
                        Services = new Services
                        {
                            Transporting = new Transporting{ Payer = payer },
                            HardPacking = new Ing{ Enabled = false },
                            Insurance = new Insurance 
                            {
                                Cost = orderSum - shippingCost,
                                Payer = payer,
                                Enabled = pecMethod.WithInsure
                            },
                            Sealing = new Sealing { Enabled = false },
                            Strapping = new Strapping { Enabled = false },
                            DocumentsReturning = new Ing { Enabled = false },
                            Delivery = new ServicesDelivery 
                            {
                                Enabled = !isDeliveryTypeToPoint,
                                Payer = !isDeliveryTypeToPoint ? payer : null
                            },
                            CashOnDelivery = mustPay
                                ? new CashOnDelivery
                                {
                                    Enabled = true,
                                    IncludeTes = true,
                                    ActualCost = orderSum - shippingCost,
                                    CashOnDeliverySum = orderSum,
                                    OrderNumber = order.Number,
                                    SellerInn = pecMethod.SenderInn,
                                    SellerTitle = string.Format("{0} {1}", pecMethod.SenderFs, pecMethod.SenderTitle),
                                    SellerPhone = pecMethod.SenderPhone,
                                }
                                : null
                        }
                    }
                }
            };

            var result = pecMethod.PecApiService.Preregistration(pecOrder);

            if (result != null)
            {
                if (result.Success
                    && result.Result != null)
                {
                    if (result.Result.Cargos != null && result.Result.Cargos.Count > 0)
                    {
                        OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                            Shipping.Pec.Pec.KeyNameCargoCodeInOrderAdditionalData,
                            result.Result.Cargos[0].CargoCode);

                        order.TrackNumber = result.Result.Cargos[0].CargoCode;
                        OrderService.UpdateOrderMain(order);

                        Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, order.ShippingMethod.ShippingType);
                    }

                    return true;
                }
                else if (pecMethod.PecApiService.LastActionErrors != null)
                {
                    Errors.AddRange(pecMethod.PecApiService.LastActionErrors);
                }
            }
            else if (pecMethod.PecApiService.LastActionErrors != null)
            {
                Errors.AddRange(pecMethod.PecApiService.LastActionErrors);
            }

            return false;
        }

        private string FindReceiverCity(string city, string district, Shipping.Pec.Pec pecMethod)
        {
            if (city.IsNotEmpty())
            {
                var brancheAndCity = pecMethod.FindBrancheAndCity(city, district, pecMethod.PecApiService);
                if (brancheAndCity != null)
                    return brancheAndCity.CityTitle.IsNotEmpty() ? brancheAndCity.CityTitle : brancheAndCity.BranchTitle;
            }
            return null;
        }

        private string FindSenderCity(Shipping.Pec.Pec pecMethod)
        {
            var findResult = pecMethod.PecApiService.FindBranchesById(pecMethod.SenderCityId.Value);
            if (findResult != null)
            {
                if (findResult.Success && findResult.Result.Success && findResult.Result.Items != null)
                {
                    var cityItem = findResult.Result.Items.FirstOrDefault();
                    if (cityItem != null)
                        return cityItem.CityTitle.IsNotEmpty() ? cityItem.CityTitle : cityItem.BranchTitle;
                }
                else if (pecMethod.PecApiService.LastActionErrors != null)
                {
                    Errors.AddRange(pecMethod.PecApiService.LastActionErrors);
                }
            }

            return null;
        }
    }
}
