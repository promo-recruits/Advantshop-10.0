using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Sdek;
using AdvantShop.Shipping.Sdek.Api;
using AdvantShop.Taxes;
using AdvantShop.Web.Infrastructure.ActionResults;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Sdek
{
    public class CreateSdekOrder
    {
        private readonly int _orderId;

        public CreateSdekOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() {Error = "Order is null"};
            
            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != ((ShippingKeyAttribute) typeof(Shipping.Sdek.Sdek).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                return new CommandResult() { Error = "Order shipping method is not 'Sdek' type" };
            if (order.OrderCustomer == null)
                return new CommandResult() { Error = "Отсутствуют данные пользователя" };
           
            try
            {
                int? tariffId = null;
                bool? withInsure = null;
                bool? allowInspection = null;

                if (order.OrderPickPoint != null && order.OrderPickPoint.AdditionalData.IsNotEmpty())
                {
                    var calcOption = JsonConvert.DeserializeObject<SdekCalculateOption>(order.OrderPickPoint.AdditionalData);
                    if (calcOption != null)
                    {
                        if (calcOption.TariffId != 0)
                            tariffId = calcOption.TariffId;

                        withInsure = calcOption.WithInsure;
                        allowInspection = calcOption.AllowInspection;
                    }

                    if (!tariffId.HasValue)
                        // поддержка старых заказов с выбором одного тарифа (старых настроек)
                        tariffId = shippingMethod.Params[SdekTemplate.TariffOldParam].TryParseInt();
                }

                if (!tariffId.HasValue)
                    return new CommandResult() { Error = "Не удалось определить тариф" };

                var preOrder = PreOrder.CreateFromOrder(order);
                preOrder.IsFromAdminArea = true;
                var items = order.OrderItems.Select(x => new PreOrderItem(x)).ToList();
                var sdekMethod = new Shipping.Sdek.Sdek(shippingMethod, preOrder, items);
                
                SdekParamsSendOrder parametrs = new SdekParamsSendOrder(shippingMethod);
                ShippingReplaceGeoService.ReplaceGeo(
                    shippingMethod.ShippingType,
                    order.OrderCustomer);
                
                var cityFrom = sdekMethod.CityFromId ?? (SdekService.GetSdekCityId(sdekMethod.CityFrom, string.Empty, string.Empty, string.Empty, sdekMethod.SdekApiService20, out _, allowedObsoleteFindCity: true) ?? 0);
                var cityTo = SdekService.GetSdekCityId(order.OrderCustomer.City, order.OrderCustomer.District, order.OrderCustomer.Region, order.OrderCustomer.Country, sdekMethod.SdekApiService20, out _) ?? 0;

                var orderSum = order.Sum;
                var shippingCost = order.ShippingCostWithDiscount;
                var shippingTaxType = shippingMethod.TaxType;

                var shippingCurrency = shippingMethod.ShippingCurrency;
                if (shippingCurrency != null)
                {
                    // Конвертируем в валюту доставки
                    order.OrderItems.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                    shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                    orderSum = orderSum.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                }

                var phone = order.OrderCustomer.StandardPhone != null &&
                            order.OrderCustomer.StandardPhone.ToString().Length == 11 && order.OrderCustomer.StandardPhone.ToString()[0] == '7'
                    ? "+" + order.OrderCustomer.StandardPhone
                    : order.OrderCustomer.Phone;
                
                var sdekTariff = SdekTariffs.Tariffs.FirstOrDefault(item => item.TariffId == tariffId);

                var dimensionsInSm = sdekMethod.GetDimensions(rate:10);
                var vatRate = GetVatRate(shippingMethod.TaxType);

                var newOrder = new NewOrder()
                {
                    DeveloperKey = "96a1f68557c674d0224d760ed5455419",
                    Type = 1,
                    Number = order.Number,
                    TariffCode = tariffId.Value,
                    Comment = parametrs.Description,
                    FromLocation = new OrderLocation {Code = cityFrom},
                    DeliveryPoint = order.OrderPickPoint.PickPointId.IsNotEmpty()
                        ? order.OrderPickPoint.PickPointId
                        : null,
                    ToLocation = order.OrderPickPoint.PickPointId.IsNullOrEmpty()
                        ? new OrderLocation()
                        {
                            Code = cityTo,
                            PostalCode = order.OrderCustomer.Zip,
                            Address = string.Join(", ", new[]
                            {
                                order.OrderCustomer.Street,
                                string.Join(" ", new[]
                                {
                                    order.OrderCustomer.House,
                                    order.OrderCustomer.Structure.IsNotEmpty()
                                        ? $"стр/корп {order.OrderCustomer.Structure}"
                                        : null,
                                }.Where(x => x.IsNotEmpty())),
                                order.OrderCustomer.Apartment.IsNotEmpty()
                                    ? $"кв. {order.OrderCustomer.Apartment}"
                                    : null
                            }.Where(x => x.IsNotEmpty()))

                        }
                        : null,
                    DeliveryRecipientCost = !order.Payed && shippingCost > 0f
                        ? new MoneyParams()
                        {
                            Value = shippingCost,
                            VatRate = vatRate,
                            VatSum = vatRate.HasValue 
                                ? shippingCost * vatRate.Value / (100 + vatRate.Value)
                                : (float?)null
                        }
                        : null,
                    Recipient = new Recipient()
                    {
                        Name =
                            $"{order.OrderCustomer.LastName} {order.OrderCustomer.FirstName} {order.OrderCustomer.Patronymic}",
                        Email = order.OrderCustomer.Email,
                        Phones = new List<Phone> {new Phone {Number = phone}}
                    },
                    Packages = new List<OrderPackage>()
                    {
                        new OrderPackage()
                        {
                            Number = "1",
                            Weight = (long) sdekMethod.GetTotalWeight(1000),
                            Length = (long) Math.Ceiling(dimensionsInSm[0]),
                            Width = (long) Math.Ceiling(dimensionsInSm[1]),
                            Height = (long) Math.Ceiling(dimensionsInSm[2]),
                        }
                    },
                    Services = new List<ServiceParams>()
                };

                if (parametrs.SellerAddress.IsNotEmpty() ||
                    parametrs.SellerName.IsNotEmpty() ||
                    parametrs.SellerINN.IsNotEmpty() ||
                    parametrs.SellerPhone.IsNotEmpty() ||
                    parametrs.SellerOwnershipForm.IsNotEmpty())
                {
                    newOrder.Seller = new Seller()
                    {
                        Name = parametrs.SellerName,
                        Inn = parametrs.SellerINN,
                        Phone = parametrs.SellerPhone,
                        OwnershipForm = parametrs.SellerOwnershipForm.IsNotEmpty()
                            ? (EnOwnershipForm) parametrs.SellerOwnershipForm.TryParseInt()
                            : (EnOwnershipForm?) null,
                        Address = parametrs.SellerAddress
                    };
                }

                var inspection = allowInspection.HasValue 
                    ? allowInspection.Value 
                    : sdekMethod.AllowInspection; // поддержка старых заказов без запоминания в SdekCalculateOption
                
                if (inspection &&
                    (sdekTariff == null || !sdekTariff.Mode.EndsWith("-П"))) // недоступно для постаматов
                {
                    newOrder.Services.Add(new ServiceParams{Code = "INSPECTION_CARGO"});
                }
                
                // переносим вес из items, т.к. там учтены настройки метода
                var orderItems = order.OrderItems;
                foreach (var orderItem in orderItems)
                    orderItem.Weight = items.First(x => x.Id == orderItem.OrderItemID).Weight;
                
                var recalculateOrderItems = new RecalculateOrderItemsToSum(orderItems.CeilingAmountToInteger());
                recalculateOrderItems.AcceptableDifference = 0.1f;
                
                orderItems = recalculateOrderItems.ToSum(orderSum - shippingCost) as List<OrderItem>;
                newOrder.Packages[0].Items = new List<OrderPackageItem>();

                var insure = withInsure.HasValue 
                    ? withInsure.Value 
                    : sdekMethod.WithInsure; // поддержка старых заказов без запоминания в SdekCalculateOption
                
                foreach (var orderItem in orderItems)
                {
                    if (orderItem.Amount > 999f)
                    {
                        var newAmount = 1f;
                        orderItem.ConvertOrderItemToNewAmount(newAmount);
                        orderItem.Amount = newAmount;
                    }
                    var product = orderItem.ProductID != null ? AdvantShop.Catalog.ProductService.GetProduct(orderItem.ProductID.Value) : null;
                    vatRate = orderItem.TaxType.HasValue ? GetVatRate(orderItem.TaxType.Value) : null;
                    newOrder.Packages[0].Items.Add(new OrderPackageItem
                    {
                        Name = orderItem.Name,
                        NameI18n = orderItem.Name,
                        WareKey = orderItem.ArtNo,
                        Cost = insure || !order.Payed 
                            ? orderItem.Price 
                            : 0f,
                        Payment = new MoneyParams
                        {
                            Value = order.Payed 
                                ? 0f 
                                : orderItem.Price,
                            VatRate = !order.Payed
                                ? vatRate 
                                : null,
                            VatSum = !order.Payed && vatRate.HasValue 
                                ? orderItem.Price * vatRate.Value / (100 + vatRate.Value)
                                : (float?)null
                        },
                        Weight = (long)(Math.Ceiling(orderItem.Weight * 1000)),
                        Amount = (int)orderItem.Amount,
                        Url = product != null 
                            ? Configuration.SettingsMain.SiteUrl.Trim('/') + "/products/" + product.UrlPath 
                            : null
                    });
                }

                var result = sdekMethod.SdekApiService20.CreateOrder(newOrder);

                if (result?.Entity?.Uuid != null && result?.Requests != null && result.Requests[0].Errors == null)
                {
                    // запрашиваем данные по заказу
                    var getOrderResult = sdekMethod.SdekApiService20.GetOrder(result.Entity.Uuid.Value, null, null);
                    var requestCreate = getOrderResult?.Requests?.FirstOrDefault(x => 
                        string.Equals("CREATE", x.Type, StringComparison.OrdinalIgnoreCase));

                    // если нет конкретного результата, то запрашиваем заказ повторно через 1сек.
                    if (requestCreate?.State.Equals("SUCCESSFUL", StringComparison.OrdinalIgnoreCase) == false &&
                        requestCreate?.State.Equals("INVALID", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        Task.Delay(1000).Wait();
                        getOrderResult = sdekMethod.SdekApiService20.GetOrder(result.Entity.Uuid.Value, null, null);
                        requestCreate = getOrderResult?.Requests?.FirstOrDefault(x =>
                            string.Equals("CREATE", x.Type, StringComparison.OrdinalIgnoreCase));
                    }

                    // если возникла ошибка при добавлении
                    if (requestCreate?.State.Equals("INVALID", StringComparison.OrdinalIgnoreCase) == true)
                        return requestCreate.Errors != null
                            ? new CommandResult
                            {
                                Error = string.Join(". ", requestCreate.Errors.Select(x => x.Message))
                                    .Replace("выбранного тарифа вес должен быть меньше",
                                        "выбранного тарифа вес и объемный вес должен быть меньше",
                                        StringComparison.OrdinalIgnoreCase)
                            }
                            : new CommandResult {Error = "Запрос на создание заказа обработался с ошибкой"};
                    
                    // либо еще нет конкретного результата добавления, либо заказ добавился
                    string sdekNumber = null;

                    if (getOrderResult?.Entity?.CdekNumber.IsNotEmpty() == true)
                        sdekNumber = getOrderResult.Entity.CdekNumber;
                    OrderService.AddUpdateOrderAdditionalData(
                        order.OrderID, 
                        Shipping.Sdek.Sdek.KeyNameSdekOrderUuidInOrderAdditionalData,
                        result.Entity.Uuid.Value.ToString());
                    
                    if (sdekNumber.IsNotEmpty())
                    {
                        OrderService.AddUpdateOrderAdditionalData(
                            order.OrderID, 
                            Shipping.Sdek.Sdek.KeyNameDispatchNumberInOrderAdditionalData,
                            sdekNumber);
                        
                        order = OrderService.GetOrder(_orderId);
                        order.TrackNumber = sdekNumber;
                        OrderService.UpdateOrderMain(order);
                    }
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, shippingMethod.ShippingType);

                    return new CommandResult {Result = true};
                }

                return result?.Requests != null && result.Requests[0].Errors != null
                    ? new CommandResult
                    {
                        Error = string.Join(". ", result.Requests[0].Errors.Select(x => x.Message))
                            .Replace("выбранного тарифа вес должен быть меньше",
                                "выбранного тарифа вес и объемный вес должен быть меньше",
                                StringComparison.OrdinalIgnoreCase)
                    }
                    : sdekMethod.SdekApiService20.LastActionErrors != null &&
                      sdekMethod.SdekApiService20.LastActionErrors.Count > 0
                        ? new CommandResult {Error = string.Join(". ", sdekMethod.SdekApiService20.LastActionErrors)}
                        : new CommandResult {Error = "Что-то пошло не так"};
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = "Не удалось создать заказ: " + ex.Message };
            }
        }

        private int? GetVatRate(TaxType taxType)
        {
            switch (taxType)
            {
                case TaxType.Vat0:
                    return 0;
                case TaxType.Vat10:
                    return 10;
                case TaxType.Vat18:
                    return 18;
                case TaxType.Vat20:
                    return 20;
                default:
                    return null;
            }
        }
    }
}
