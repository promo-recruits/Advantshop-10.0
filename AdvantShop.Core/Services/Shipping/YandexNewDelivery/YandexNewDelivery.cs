using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Shipping.YandexNewDelivery;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Core;

namespace AdvantShop.Shipping.ShippingYandexNewDelivery
{
    // docs https://yandex.ru/dev/delivery-3/doc/dg/reference/put-delivery-options.html
    // docs https://yandex.ru/dev/delivery-3/doc/dg/reference/post-orders.html

    [ShippingKey("YandexNewDelivery")]
    public class YandexNewDelivery : BaseShippingWithCargo, IShippingSupportingSyncOfOrderStatus, IShippingSupportingTheHistoryOfMovement, IShippingLazyData, IShippingSupportingPaymentCashOnDelivery
    {
        #region Ctor

        private readonly string _authorizationToken;
        private readonly int _shopId;
        private readonly string _warehouseId;
        private readonly string _widgetApiKey;
        private readonly string _cityFrom;
        private readonly bool _statusesSync;
        private readonly TypeViewPoints _typeViewPoints;
        private readonly string _yaMapsApiKey;

        private readonly PriceDisplayType _priceDisplayType;

        private readonly EExportOrderExternalIdType _exportOrderExternalIdType;

        private const string Url = "https://api.delivery.yandex.ru";
        public const string KeyNameOrderIdInOrderAdditionalData = "YandexNewDeliveryOrderId";

        private readonly YandexNewDeliveryApiService _yandexNewDeliveryApiService;
        private readonly string _courierOptionName;
        private readonly string _pickupOptionName;

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public YandexNewDelivery(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items)
            : base(method, preOrder, items)
        {
            _authorizationToken = _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.AuthorizationToken);
            _shopId = _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.ShopId).TryParseInt();
            _warehouseId = _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.WarehouseId);
            _widgetApiKey = _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.WidgetApiKey);
            _cityFrom = _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.CityFrom, "Москва");

            _yandexNewDeliveryApiService = new YandexNewDeliveryApiService(_authorizationToken);

            _typeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(YandexNewDeliveryTemplate.TypeViewPoints).TryParseInt();
            _yaMapsApiKey = _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.YaMapsApiKey);

            _statusesSync = _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.StatusesSync).TryParseBool();

            _priceDisplayType = (PriceDisplayType)_method.Params.ElementOrDefault(YandexNewDeliveryTemplate.PriceDisplayType).TryParseInt();

            _exportOrderExternalIdType = (EExportOrderExternalIdType)_method.Params.ElementOrDefault(YandexNewDeliveryTemplate.ExportOrderExternalIdType).TryParseInt();

            _courierOptionName = _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.CourierOptionName, "(Курьер)");
            _pickupOptionName = _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.PickupOptionName, "(Самовывоз)");
        }

        #endregion Ctor

        #region Statuses

        public void SyncStatusOfOrder(Order order)
        {
            var yaOrderId = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameOrderIdInOrderAdditionalData);

            // если нет данных о yandex order id
            if (!yaOrderId.IsNotEmpty())
                return;

            var statusInfoAnswer = _yandexNewDeliveryApiService.GetOrderStatuses(yaOrderId);
            // не получили информации от яндекса
            if (statusInfoAnswer == null || statusInfoAnswer.Statuses.Count <= 0)
                return;

            var yandexOrderStatuses = statusInfoAnswer.Statuses.OrderBy(x => x.Datetime).ToList();
            foreach (var status in yandexOrderStatuses)
            {
                var statusReference = StatusesReference.ContainsKey(status.Code)
                    ? StatusesReference[status.Code]
                    : null;

                if (!statusReference.HasValue || order.OrderStatusId == statusReference.Value || OrderStatusService.GetOrderStatus(statusReference.Value) == null)
                    continue;
                
                var lastOrderStatusHistory =
                    OrderStatusService.GetOrderStatusHistory(order.OrderID)
                        .OrderByDescending(item => item.Date)
                        .FirstOrDefault();

                if (lastOrderStatusHistory == null ||
                    lastOrderStatusHistory.Date < status.Datetime)
                {
                    OrderStatusService.ChangeOrderStatus(order.OrderID,
                    statusReference.Value, "Синхронизация статусов для Яндекс.Доставки");
                }
            }

            if (string.IsNullOrWhiteSpace(order.TrackNumber)
                && yandexOrderStatuses.Any(x => x.Code.Equals("DELIVERY_TRACK_RECEIVED", StringComparison.OrdinalIgnoreCase)))
            {
                // тянем трекномер
                var trackNumber = _yandexNewDeliveryApiService.GetOrderTrackNumber(yaOrderId);

                if (string.IsNullOrWhiteSpace(trackNumber))
                    return;
                
                var freshOrder = OrderService.GetOrder(order.OrderID);
                freshOrder.TrackNumber = order.TrackNumber = trackNumber;
                OrderService.UpdateOrderMain(freshOrder);
            }
        }

        public bool StatusesSync => _statusesSync;

        public bool SyncByAllOrders => false;
        public void SyncStatusOfOrders(IEnumerable<Order> orders) => throw new NotImplementedException();

        private Dictionary<string, int?> _statusesReference;

        private Dictionary<string, int?> StatusesReference
        {
            get
            {
                if (_statusesReference == null)
                {
                    _statusesReference = new Dictionary<string, int?>()
                    {
                        { "DRAFT", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DRAFT).TryParseInt(true) },
                        { "VALIDATING", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_VALIDATING).TryParseInt(true) },
                        { "VALIDATING_ERROR", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_VALIDATING_ERROR).TryParseInt(true) },
                        { "DELIVERY_PROCESSING_STARTED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_PROCESSING_STARTED).TryParseInt(true) },
                        { "DELIVERY_TRACK_RECEIVED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_TRACK_RECEIVED).TryParseInt(true) },
                        { "CREATED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_CREATED).TryParseInt(true) },
                        { "SENDER_SENT", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SENDER_SENT).TryParseInt(true) },
                        { "DELIVERY_LOADED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_LOADED).TryParseInt(true) },
                        { "SENDER_WAIT_FULFILLMENT", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SENDER_WAIT_FULFILMENT).TryParseInt(true) },
                        { "SENDER_WAIT_DELIVERY", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SENDER_WAIT_DELIVERY).TryParseInt(true) },
                        { "DELIVERY_AT_START", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_AT_START).TryParseInt(true) },
                        { "DELIVERY_AT_START_SORT", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_AT_START_SORT).TryParseInt(true) },
                        { "DELIVERY_TRANSPORTATION", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_TRANSPORTATION).TryParseInt(true) },
                        { "DELIVERY_ARRIVED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_ARRIVED).TryParseInt(true) },
                        { "DELIVERY_TRANSPORTATION_RECIPIENT", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_TRANSPORTATION_RECIPIENT).TryParseInt(true) },
                        { "DELIVERY_CUSTOMS_ARRIVED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_CUSTOMS_ARRIVED).TryParseInt(true) },
                        { "DELIVERY_CUSTOMS_CLEARED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_CUSTOMS_CLEARED).TryParseInt(true) },
                        { "DELIVERY_STORAGE_PERIOD_EXTENDED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_STORAGE_PERIOD_EXTENDED).TryParseInt(true) },
                        { "DELIVERY_STORAGE_PERIOD_EXPIRED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_STORAGE_PERIOD_EXPIRED).TryParseInt(true) },
                        { "DELIVERY_UPDATED_BY_SHOP", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_UPDATED_BY_SHOP).TryParseInt(true) },
                        { "DELIVERY_UPDATED_BY_RECIPIENT", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_UPDATED_BY_RECIPIENT).TryParseInt(true) },
                        { "DELIVERY_UPDATED_BY_DELIVERY", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_UPDATED_BY_DELIVERY).TryParseInt(true) },
                        { "DELIVERY_ARRIVED_PICKUP_POINT", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_ARRIVED_PICKUP_POINT).TryParseInt(true) },
                        { "DELIVERY_TRANSMITTED_TO_RECIPIENT", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_TRANSMITTED_TO_RECIPIENT).TryParseInt(true) },
                        { "DELIVERY_DELIVERED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_DELIVERED).TryParseInt(true) },
                        { "DELIVERY_ATTEMPT_FAILED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_ATTEMPT_FAILED).TryParseInt(true) },
                        { "DELIVERY_CAN_NOT_BE_COMPLETED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_DELIVERY_CAN_NOT_BE_COMPLETED).TryParseInt(true) },
                        { "RETURN_STARTED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_RETURN_STARTED).TryParseInt(true) },
                        { "SORTING_CENTER_CREATED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_CREATED).TryParseInt(true) },
                        { "SORTING_CENTER_LOADED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_LOADED).TryParseInt(true) },
                        { "SORTING_CENTER_AT_START", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_AT_START).TryParseInt(true) },
                        { "SORTING_CENTER_OUT_OF_STOCK", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_OUT_OF_STOCK).TryParseInt(true) },
                        { "SORTING_CENTER_AWAITING_CLARIFICATION", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_AWAITING_CLARIFICATION).TryParseInt(true) },
                        { "SORTING_CENTER_PREPARED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_PREPARED).TryParseInt(true) },
                        { "SORTING_CENTER_TRANSMITTED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_TRANSMITTED).TryParseInt(true) },
                        { "SORTING_CENTER_RETURN_PREPARING", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_PREPARING).TryParseInt(true) },
                        { "SORTING_CENTER_RETURN_RFF_PREPARING_FULFILLMENT", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RFF_PREPARING_FULFILLMENT).TryParseInt(true) },
                        { "SORTING_CENTER_RETURN_RFF_TRANSMITTED_FULFILLMENT", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RFF_TRANSMITTED_FULFILLMENT).TryParseInt(true) },
                        { "SORTING_CENTER_RETURN_RFF_ARRIVED_FULFILLMENT", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RFF_ARRIVED_FULFILLMENT).TryParseInt(true) },
                        { "SORTING_CENTER_RETURN_ARRIVED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_ARRIVED).TryParseInt(true) },
                        { "SORTING_CENTER_RETURN_PREPARING_SENDER", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_PREPARING_SENDER).TryParseInt(true) },
                        { "SORTING_CENTER_RETURN_TRANSFERRED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_TRANSFERRED).TryParseInt(true) },
                        { "SORTING_CENTER_RETURN_RETURNED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_RETURN_RETURNED).TryParseInt(true) },
                        { "SORTING_CENTER_CANCELED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_CANCELED).TryParseInt(true) },
                        { "SORTING_CENTER_ERROR", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_SORTING_CENTER_ERROR).TryParseInt(true) },
                        { "LOST", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_LOST).TryParseInt(true) },
                        { "CANCELLED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_CANCELLED).TryParseInt(true) },
                        { "CANCELLED_USER", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_CANCELLED_USER).TryParseInt(true) },
                        { "FINISHED", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_FINISHED).TryParseInt(true) },
                        { "ERROR", _method.Params.ElementOrDefault(YandexNewDeliveryTemplate.Status_ERROR).TryParseInt(true) },
                    };
                }
                return _statusesReference;
            }
        }

        #endregion Statuses

        #region IShippingSupportingTheHistoryOfMovement

        public bool ActiveHistoryOfMovement
        {
            get { return true; }
        }

        public List<HistoryOfMovement> GetHistoryOfMovement(Order order)
        {
            var yaOrderId = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameOrderIdInOrderAdditionalData);
            if (yaOrderId.IsNotEmpty())
            {
                var statusInfoAnswer = _yandexNewDeliveryApiService.GetOrderStatuses(yaOrderId);
                if (statusInfoAnswer != null && statusInfoAnswer.Statuses.Count > 0)
                {
                    return statusInfoAnswer.Statuses
                        .OrderByDescending(x => x.Datetime)
                        .Select(statusInfo => new HistoryOfMovement()
                        {
                            Code = statusInfo.Code,
                            Name = statusInfo.Description,
                            Date = statusInfo.Datetime,
                        }).ToList();
                }
            }

            return null;
        }

        public PointInfo GetPointInfo(Order order)
        {
            if (order.OrderPickPoint == null
                || order.OrderPickPoint.PickPointId.IsNullOrEmpty())
                return null;

            var pointId = order.OrderPickPoint.PickPointId.TryParseLong(true);
            if (pointId.HasValue)
            {
                var points = _yandexNewDeliveryApiService.GetPickupPointsInfo(new List<long> { pointId.Value });

                if (points == null || points.Count == 0)
                    return null;

                var yandexPoint = points[0];

                return yandexPoint != null
                    ? new PointInfo()
                    {
                        Address = yandexPoint.Address.AddressString,
                        TimeWork = GetScheduleString(yandexPoint.Schedule),
                        Comment = yandexPoint.Instruction,
                        Phone = yandexPoint.Phones != null
                            ? string.Join(", ",
                            yandexPoint.Phones.Select(p => p.InternalNumber.IsNullOrEmpty()
                                ? p.Number
                                : string.Format("{0} (доб. {1})", p.Number, p.InternalNumber)))
                            : null
                    }
                    : null;
            }
            return null;
        }

        private string GetScheduleString(List<YaNewDeliveryPickupPointSchedule> schedule)
        {
            if (schedule != null)
            {
                return string.Join(", ", schedule.OrderBy(s => s.Day).Select(s => string.Format("{0}: {1}-{2}", GetDayName(s.Day), s.From, s.To)));
            }
            return null;
        }

        private object GetDayName(byte day)
        {
            switch (day)
            {
                case 0:
                case 7:
                    return "воскресенье";

                case 1:
                    return "понедельник";

                case 2:
                    return "вторник";

                case 3:
                    return "среда";

                case 4:
                    return "четверг";

                case 5:
                    return "пятница";

                case 6:
                    return "суббота";
            }
            return string.Empty;
        }

        #endregion IShippingSupportingTheHistoryOfMovement

        private Dictionary<string, object> GetParam(int[] dimensions, float weight, bool? fullyPrepaid = null)
        {
            var cityGeoIdFrom = _yandexNewDeliveryApiService.GetCityGeoId(_cityFrom);
            var cityGeoIdTo = _yandexNewDeliveryApiService.GetCityGeoId(_preOrder.CityDest + ", " + _preOrder.RegionDest);

            var data = new Dictionary<string, object>()
            {
                { "senderId", _shopId },
                { "from", new { geoId = cityGeoIdFrom } },
                { "to", new { geoId = cityGeoIdTo } },
                {
                    "dimensions", new
                    {
                        length = dimensions[0],
                        width = dimensions[1],
                        height = dimensions[2],
                        weight = weight,
                    }
                },
                {
                    "shipment", new
                    {
                        warehouseId = _warehouseId,
                        includeNonDefault = false,
                    }
                },
                {
                    "cost", new
                    {
                        assessedValue = _totalPrice,
                        itemsSum = _totalPrice,
                        manualDeliveryForCustomer = _preOrder.ShippingOption?.FinalRate,
                        fullyPrepaid = fullyPrepaid
                    }
                }
            };

            return data;
        }

        private int[] GetCalcDimensions()
        {
            var dimensions = GetDimensions(rate: 10);
            var dim = new int[dimensions.Length];
            for (int i = 0; i < dim.Length; i++)
            {
                dim[i] = (int)Math.Ceiling(dimensions[i]);
            }

            return dim;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var shippingOptions = new List<BaseShippingOption>();

            var weight = GetTotalWeight();
            var dimensions = GetCalcDimensions();
            var data = GetParam(dimensions, weight);
            var dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            var responseData = _yandexNewDeliveryApiService.MakeRequest(Url + "/delivery-options", dataStr, "PUT");

            if (!string.IsNullOrEmpty(responseData))
            {
                try
                {
                    var dto = JsonConvert.DeserializeObject<List<YandexNewDeliveryListItem>>(responseData);

                    // не почта россии
                    var notPostDeliveryItems = dto.Where(x => x.Delivery.Type != "POST").ToList();
                    if (notPostDeliveryItems.Count > 0)
                    {
                        YandexNewDeliveryListItem selectedItemByTariff = null;
                        int? selectedTariffId = null;
                        long? selectedPickPointId = null;
                        string selectedDeliveryType = null;
                        if (_preOrder.ShippingOption != null &&
                            _preOrder.ShippingOption.MethodId == _method.ShippingMethodId &&
                            _preOrder.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(YandexNewDelivery).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                        {
                            if (_preOrder.ShippingOption.GetType() == typeof(YandexNewDeliveryWidgetOption))
                            {
                                selectedTariffId = ((YandexNewDeliveryWidgetOption)_preOrder.ShippingOption).TariffId;
                                selectedPickPointId = ((YandexNewDeliveryWidgetOption)_preOrder.ShippingOption).PickpointId;
                                selectedDeliveryType = ((YandexNewDeliveryWidgetOption)_preOrder.ShippingOption).DeliveryType;
                            }
                            if (_preOrder.ShippingOption.GetType() == typeof(YandexNewPointDeliveryListOption))
                            {
                                if (((YandexNewPointDeliveryListOption)_preOrder.ShippingOption).SelectedPoint != null)
                                    selectedPickPointId = ((YandexNewPointDeliveryListOption)_preOrder.ShippingOption).SelectedPoint.Code.TryParseLong(true);
                            }
                            if (_preOrder.ShippingOption.GetType() == typeof(YandexNewPointDeliveryMapOption))
                            {
                                selectedPickPointId = ((YandexNewPointDeliveryMapOption)_preOrder.ShippingOption).PickpointId.TryParseLong(true);
                            }
                        }

                        //float? rate = null;
                        if (selectedTariffId.HasValue)
                        {
                            selectedItemByTariff = notPostDeliveryItems.FirstOrDefault(x => x.TariffId == selectedTariffId.Value && (!selectedPickPointId.HasValue || x.PickupPointIds.Contains(selectedPickPointId.Value)));
                        }
                        else if (selectedPickPointId.HasValue)
                        {
                            selectedItemByTariff = notPostDeliveryItems.FirstOrDefault(x => x.PickupPointIds != null && x.PickupPointIds.Contains(selectedPickPointId.Value));
                        }

                        //if (selectedItemByTariff != null)
                        //    rate = selectedItemByTariff.Cost.CostForCustomer;

                        if (_typeViewPoints == TypeViewPoints.WidgetYandexDelivery && _widgetApiKey.IsNotEmpty())
                        {
                            var courierDeliveryItems = notPostDeliveryItems.Where(x => x.Delivery?.Type == "COURIER").ToList();
                            var pickupDeliveryItems = notPostDeliveryItems.Where(x => x.Delivery?.Type == "PICKUP").ToList();
                            if (courierDeliveryItems.Any())
                            {
                                shippingOptions.Add(new YandexNewDeliveryWidgetOption(_method, _totalPrice)
                                {
                                    Name = $"{_method.Name} {_courierOptionName}",
                                    Company = selectedDeliveryType == "COURIER" &&
                                              selectedItemByTariff?.Delivery?.Partner?.Name != null
                                        ? selectedItemByTariff.Delivery.Partner.Name
                                        : string.Empty,
                                    WidgetApiKey = _widgetApiKey,
                                    SenderId = _shopId,
                                    WarehouseId = _warehouseId,
                                    Weight = weight.ToString("F2", CultureInfo.InvariantCulture),
                                    Cost = _totalPrice.ToString("F2", CultureInfo.InvariantCulture),
                                    TotalLength = dimensions[0],
                                    TotalWidth = dimensions[1],
                                    TotalHeight = dimensions[2],
                                    Rate = selectedDeliveryType == "COURIER" &&
                                           selectedItemByTariff?.Cost?.CostForCustomer != null
                                        ? selectedItemByTariff.Cost.CostForCustomer
                                        : courierDeliveryItems.Min(x => x.Cost.CostForCustomer),
                                    DeliveryType = "COURIER",
                                    IsAvailablePaymentCashOnDelivery = selectedItemByTariff?.Services != null && selectedItemByTariff.Services.Any(x => x.Code == "CASH_SERVICE")
                                });
                            }

                            if (pickupDeliveryItems.Any())
                            {
                                YandexNewDeliveryPickupPointDto selectedPickupPoint = null;
                                if (selectedItemByTariff != null && selectedPickPointId.HasValue)
                                {
                                    var pickupPoints = _yandexNewDeliveryApiService.GetPickupPointsInfo(selectedItemByTariff.PickupPointIds);
                                    selectedPickupPoint =
                                        pickupPoints.FirstOrDefault(x => x.Id == selectedPickPointId.Value);// выбранный ПВЗ
                                }

                                shippingOptions.Add(new YandexNewDeliveryWidgetOption(_method, _totalPrice)
                                {
                                    Name = $"{_method.Name} {_pickupOptionName}",
                                    Company = selectedDeliveryType == "PICKUP" &&
                                              selectedItemByTariff?.Delivery?.Partner?.Name != null
                                        ? selectedItemByTariff.Delivery.Partner.Name
                                        : string.Empty,
                                    WidgetApiKey = _widgetApiKey,
                                    SenderId = _shopId,
                                    WarehouseId = _warehouseId,
                                    Weight = weight.ToString("F2", CultureInfo.InvariantCulture),
                                    Cost = _totalPrice.ToString("F2", CultureInfo.InvariantCulture),
                                    TotalLength = dimensions[0],
                                    TotalWidth = dimensions[1],
                                    TotalHeight = dimensions[2],
                                    Rate = selectedDeliveryType == "PICKUP" &&
                                           selectedItemByTariff?.Cost?.CostForCustomer != null
                                        ? selectedItemByTariff.Cost.CostForCustomer
                                        : pickupDeliveryItems.Min(x => x.Cost.CostForCustomer),
                                    DeliveryType = "PICKUP",
                                    IsAvailablePaymentCashOnDelivery = selectedPickupPoint != null &&// доступно только при выбранной КЛИЕНТОМ точке
                                                                       (selectedPickupPoint.SupportedFeatures.Cash || selectedPickupPoint.SupportedFeatures.Card)
                                });
                            }
                        }
                        else if (_typeViewPoints == TypeViewPoints.YaWidget || _typeViewPoints == TypeViewPoints.List ||
                                 (_typeViewPoints == TypeViewPoints.WidgetYandexDelivery && _widgetApiKey.IsNullOrEmpty()))
                        {
                            // ПВЗ
                            var deliveryItemsWithPoints = notPostDeliveryItems.Where(x => x.PickupPointIds != null && x.PickupPointIds.Count > 0).ToList();
                            if (deliveryItemsWithPoints.Count > 0)
                            {
                                //if (!rate.HasValue)
                                //    rate = deliveryItemsWithPoints.Min(x => x.Cost.CostForCustomer);

                                var pickupPointIds = deliveryItemsWithPoints.SelectMany(x => x.PickupPointIds).Distinct().ToList();
                                var pickupPoints = _yandexNewDeliveryApiService.GetPickupPointsInfo(pickupPointIds);
                                List<YandexNewPoint> shippingPoints = CastPoints(pickupPoints);
                                // наиболее выгодная доставка (самая дешевая)
                                YandexNewDeliveryListItem advantageousItem = deliveryItemsWithPoints.OrderBy(x => x.Cost.CostForCustomer).First();
                                YandexNewDeliveryPickupPointDto selectedPickupPoint = null;
                                if (selectedPickPointId.HasValue)
                                    selectedPickupPoint = pickupPoints.FirstOrDefault(x => x.Id == selectedPickPointId.Value);// выбранный ПВЗ

                                if (selectedPickupPoint == null)
                                {
                                    selectedPickupPoint = pickupPoints.First(x => advantageousItem.PickupPointIds.Contains(x.Id));// первый ПВЗ из выгодной доставки
                                    selectedItemByTariff = advantageousItem;// ПВЗ выгодной доставки то и тариф тоже ее
                                }
                                var selectedPoint = shippingPoints.First(x => x.Code.Equals(selectedPickupPoint.Id.ToString()));

                                if (_typeViewPoints == TypeViewPoints.YaWidget && _yaMapsApiKey.IsNotEmpty())
                                {
                                    var option = new YandexNewPointDeliveryMapOption(_method, _totalPrice)
                                    {
                                        Name = _method.Name + " (в пункт выдачи)",
                                        PickpointCompany = selectedItemByTariff?.Delivery?.Partner?.Name ?? string.Empty,
                                        Rate = GetCost((selectedItemByTariff ?? advantageousItem).Cost),
                                        PickpointAdditionalData = new YandexNewDeliveryAdditionalData()
                                        {
                                            deliveryType = (selectedItemByTariff ?? advantageousItem).Delivery.Type,
                                            partnerId = (selectedItemByTariff ?? advantageousItem).Delivery.Partner.Id,
                                            tariffId = (selectedItemByTariff ?? advantageousItem).TariffId
                                        },
                                        CurrentPoints = shippingPoints,
                                        IsAvailablePaymentCashOnDelivery = selectedPickPointId.HasValue &&// доступно только при выбранной КЛИЕНТОМ точке
                                                                           (selectedPickupPoint.SupportedFeatures.Cash || selectedPickupPoint.SupportedFeatures.Card)
                                    };

                                    SetMapData(option, _preOrder.CountryDest, _preOrder.RegionDest, _preOrder.CityDest, pickupPointIds);

                                    shippingOptions.Add(option);
                                }
                                else if (_typeViewPoints == TypeViewPoints.List || (_typeViewPoints == TypeViewPoints.WidgetYandexDelivery && _widgetApiKey.IsNullOrEmpty()))
                                {
                                    shippingOptions.Add(new YandexNewPointDeliveryListOption(_method, _totalPrice)
                                    {
                                        Name = _method.Name + " (в пункт выдачи)",
                                        PickpointCompany = selectedItemByTariff?.Delivery?.Partner?.Name ?? string.Empty,
                                        Rate = GetCost((selectedItemByTariff ?? advantageousItem).Cost),
                                        DeliveryTime = selectedItemByTariff != null && selectedItemByTariff.Delivery != null
                                            ? PrepareDeliveryTime(new[] { selectedItemByTariff.Delivery.CalculatedDeliveryDateMin.TryParseDateTime() },
                                            new[] { selectedItemByTariff.Delivery.CalculatedDeliveryDateMax.TryParseDateTime() })
                                            : null,
                                        PickpointAdditionalData = new YandexNewDeliveryAdditionalData()
                                        {
                                            deliveryType = (selectedItemByTariff ?? advantageousItem).Delivery.Type,
                                            partnerId = (selectedItemByTariff ?? advantageousItem).Delivery.Partner.Id,
                                            tariffId = (selectedItemByTariff ?? advantageousItem).TariffId
                                        },
                                        ShippingPoints = shippingPoints.OrderBy(x => x.Address).ToList(),
                                        SelectedPoint = selectedPoint,
                                        HideAddressBlock = true,
                                        IsAvailablePaymentCashOnDelivery = selectedPickupPoint.SupportedFeatures.Cash || selectedPickupPoint.SupportedFeatures.Card
                                    });
                                }
                            }

                            // курьер
                            var deliveryItemsWithOutPoints = notPostDeliveryItems.Where(x => x.PickupPointIds == null || x.PickupPointIds.Count <= 0);
                            foreach (var delivertyItem in deliveryItemsWithOutPoints)
                            {
                                shippingOptions.Add(new YandexNewDeliveryOption(_method, _totalPrice)
                                {
                                    Name = string.Format("{0} ({1} {2})", _method.Name, delivertyItem.Delivery.Partner.Name, GetNameDeliveryType(delivertyItem.Delivery.Type)),
                                    Rate = GetCost(delivertyItem.Cost),
                                    DeliveryId = string.Format("{0}_{1}_{2}", delivertyItem.Delivery.Type, delivertyItem.Delivery.Partner.Id, delivertyItem.TariffId).GetHashCode(),
                                    DeliveryTime = PrepareDeliveryTime(new[] { delivertyItem.Delivery.CalculatedDeliveryDateMin.TryParseDateTime() },
                                    new[] { delivertyItem.Delivery.CalculatedDeliveryDateMax.TryParseDateTime() }),
                                    PickpointAdditionalData = new YandexNewDeliveryAdditionalData
                                    {
                                        deliveryType = delivertyItem.Delivery.Type,
                                        partnerId = delivertyItem.Delivery.Partner.Id,
                                        tariffId = delivertyItem.TariffId
                                    },
                                    IsAvailablePaymentCashOnDelivery = delivertyItem.Services != null && delivertyItem.Services.Any(x => x.Code == "CASH_SERVICE")
                                });
                            }
                        }
                    }

                    // почта россии
                    var postDeliveryItems = dto.Where(x => x.Delivery.Type == "POST").ToList();
                    if (postDeliveryItems.Any())
                    {
                        foreach (var group in postDeliveryItems.GroupBy(x => x.TariffId))
                        {
                            var delivertyItem = group.First();

                            shippingOptions.Add(new YandexNewDeliveryOption(_method, _totalPrice)
                            {
                                Name = _method.Name + " (" + (string.IsNullOrEmpty(delivertyItem.TariffName)
                                    ? delivertyItem.Delivery.Partner.Name
                                    : delivertyItem.TariffName) + ")",
                                Rate = _typeViewPoints != TypeViewPoints.WidgetYandexDelivery
                                    ? GetCost(delivertyItem.Cost)
                                    : delivertyItem.Cost.CostForCustomer,
                                DeliveryId =
                                    $"{delivertyItem.Delivery.Type}_{delivertyItem.Delivery.Partner.Id}_{delivertyItem.TariffId}"
                                        .GetHashCode(),
                                DeliveryTime = PrepareDeliveryTime(group.Select(x => x.Delivery.CalculatedDeliveryDateMin.TryParseDateTime()),
                                group.Select(x => x.Delivery.CalculatedDeliveryDateMax.TryParseDateTime())),
                                PickpointAdditionalData = new YandexNewDeliveryAdditionalData()
                                {
                                    deliveryType = delivertyItem.Delivery.Type,
                                    partnerId = delivertyItem.Delivery.Partner.Id,
                                    tariffId = delivertyItem.TariffId
                                },
                                IsAvailablePaymentCashOnDelivery = delivertyItem.Services != null && delivertyItem.Services.Any(x => x.Code == "CASH_SERVICE")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(responseData, ex);
                }
            }

            return shippingOptions;
        }

        private string GetNameDeliveryType(string type)
        {
            if (string.Equals(type, "COURIER", StringComparison.OrdinalIgnoreCase))
                return "курьером";
            if (string.Equals(type, "PICKUP", StringComparison.OrdinalIgnoreCase))
                return "в пункт выдачи";
            if (string.Equals(type, "POST", StringComparison.OrdinalIgnoreCase))
                return "на почту";

            return string.Empty;
        }

        private List<YandexNewPoint> CastPoints(List<YandexNewDeliveryPickupPointDto> pickupPoints)
        {
            var result = new List<YandexNewPoint>();
            foreach (var point in pickupPoints)
            {
                result.Add(new YandexNewPoint
                {
                    Id = point.Id.GetHashCode(),
                    Code = point.Id.ToString(),
                    Address = point.Address.ShortAddressString,
                    Description = (point.Phones != null
                            ? string.Join(", ",
                            point.Phones.Select(p => p.InternalNumber.IsNullOrEmpty()
                                ? p.Number
                                : string.Format("{0} (доб. {1})", p.Number, p.InternalNumber)))
                            : null) + " "
                                    + GetScheduleString(point.Schedule),
                    AddressComment = point.Instruction,
                    PointX = (float)point.Address.Latitude,
                    PointY = (float)point.Address.Longitude,
                });
            }
            return result;
        }

        private void SetMapData(YandexNewPointDeliveryMapOption option, string country, string region, string city, List<long> pickupPointIds)
        {
            string lang = "en_US";
            switch (Localization.Culture.Language)
            {
                case Localization.Culture.SupportLanguage.Russian:
                    lang = "ru_RU";
                    break;

                case Localization.Culture.SupportLanguage.English:
                    lang = "en_US";
                    break;

                case Localization.Culture.SupportLanguage.Ukrainian:
                    lang = "uk_UA";
                    break;
            }
            option.MapParams = new PointDelivery.MapParams();
            option.MapParams.Lang = lang;
            option.MapParams.YandexMapsApikey = _yaMapsApiKey;
            option.MapParams.Destination = string.Join(", ", new[] { country, region, city }.Where(x => x.IsNotEmpty()));

            option.PointParams = new PointDelivery.PointParams();
            option.PointParams.IsLazyPoints = (option.CurrentPoints != null
                ? option.CurrentPoints.Count
                : 0) > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "pickupPointIds", string.Join(",", pickupPointIds) },
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("pickupPointIds"))
                return null;

            var pickupPointIds = data["pickupPointIds"].ToString()
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.TryParseLong()).ToList();

            var pickupPoints = _yandexNewDeliveryApiService.GetPickupPointsInfo(pickupPointIds);
            var points = CastPoints(pickupPoints);

            return GetFeatureCollection(points);
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<YandexNewPoint> points)
        {
            return new PointDelivery.FeatureCollection
            {
                Features = points.Select(p =>
                    new PointDelivery.Feature
                    {
                        Id = p.Id,
                        Geometry = new PointDelivery.PointGeometry { PointX = p.PointX, PointY = p.PointY },
                        Options = new PointDelivery.PointOptions { Preset = "islands#dotIcon" },
                        Properties = new PointDelivery.PointProperties
                        {
                            BalloonContentHeader = p.Address,
                            HintContent = p.Address,
                            BalloonContentBody =
                                string.Format("{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{3}')\">Выбрать</a>",
                                p.Description,
                                p.Description.IsNotEmpty()
                                    ? "<br>"
                                    : "",
                                p.Id,
                                p.Code),
                            BalloonContentFooter = /*_showAddressComment
                                ?*/ p.AddressComment
                            //: null
                        }
                    }).ToList()
            };
        }

        private string PrepareDeliveryTime(IEnumerable<DateTime> datesMin, IEnumerable<DateTime> datesMax)
        {
            if (datesMin == null || datesMax == null)
                return string.Empty;

            var minDate = datesMin.Min();
            var maxDate = datesMax.Max();

            var daysMin = (minDate - DateTime.Today).Days + _method.ExtraDeliveryTime;
            var daysMax = (maxDate - DateTime.Today).Days + _method.ExtraDeliveryTime;

            return daysMin != daysMax
                ? $"{daysMin}-{daysMax} дн."
                : $"{daysMin} дн.";
        }

        private float GetCost(YaNewDeliveryCost yandexCost)
        {
            switch (_priceDisplayType)
            {
                case PriceDisplayType.ForCustomer:
                    return yandexCost.CostForCustomer;

                case PriceDisplayType.ForSender:
                    return yandexCost.CostForSender;

                case PriceDisplayType.NoOptions:
                    return yandexCost.Cost;
            }
            return 0;
        }

        public bool CreateOrder(Order order, List<OrderItem> recalculatedOrderItems, bool retry = false)
        {
            var dimensions = GetCalcDimensions();

            var length = dimensions[0];
            var width = dimensions[1];
            var height = dimensions[2];
            var weight = GetTotalWeight();

            YandexNewDeliveryAdditionalData additionalData = null;
            try
            {
                if (order.OrderPickPoint != null && order.OrderPickPoint.AdditionalData.IsNotEmpty())
                {
                    additionalData = JsonConvert.DeserializeObject<YandexNewDeliveryAdditionalData>(order.OrderPickPoint.AdditionalData);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            var pickupPointId = order.OrderPickPoint?.PickPointId.TryParseLong(true);
            if (pickupPointId == 0)
                pickupPointId = null;

            long? tariffId = additionalData?.tariffId;

            var item = TryGetItem(weight, tariffId, pickupPointId, order.Payed);
            var cityGeoIdTo = _yandexNewDeliveryApiService.GetCityGeoId(order.OrderCustomer.City + ", " + order.OrderCustomer.Region);


            var phoneNumber = order.OrderCustomer.StandardPhone.ToString();
            var data = new
            {
                senderId = _shopId,
                //Есть ограничение на 10 символов
                externalId = _exportOrderExternalIdType == EExportOrderExternalIdType.OrderId
                    ? order.OrderID.ToString()
                    : order.Number,
                comment = order.CustomerComment,
                deliveryType = additionalData?.deliveryType ?? string.Empty,
                recipient = new YandexNewDeliveryRecipientDto()
                {
                    FirstName = order.OrderCustomer.FirstName,
                    MiddleName = order.OrderCustomer.Patronymic,
                    LastName = order.OrderCustomer.LastName,
                    Email = order.OrderCustomer.Email,
                    Address = new
                    {
                        country = order.OrderCustomer.Country,
                        region = order.OrderCustomer.Region,
                        locality = order.OrderCustomer.City,
                        street = order.OrderCustomer.Street,
                        house = order.OrderCustomer.House,
                        building = order.OrderCustomer.Structure,
                        apartment = order.OrderCustomer.Apartment,
                        postalCode = order.OrderCustomer.Zip,
                        geoId = cityGeoIdTo != 0
                            ? cityGeoIdTo
                            : default(int?)
                    },
                    PickupPointId = pickupPointId
                },
                cost = new
                {
                    manualDeliveryForCustomer = _preOrder.ShippingOption.FinalRate,
                    assessedValue = _totalPrice,
                    fullyPrepaid = order.Payed
                },
                contacts = new List<object>
                {
                    new
                    {
                        type = "RECIPIENT",
                        phone = phoneNumber.StartsWith("7")
                            ? $"+{phoneNumber}"
                            : phoneNumber,
                        firstName = order.OrderCustomer.FirstName,
                        middleName = order.OrderCustomer.Patronymic ?? "",
                        lastName = order.OrderCustomer.LastName ?? ""
                    }
                },
                deliveryOption =
                    item != null
                        ? new
                        {
                            tariffId = item.TariffId,
                            delivery = _preOrder.ShippingOption.FinalRate,
                            partnerId = additionalData?.partnerId ?? 0,
                            calculatedDeliveryDateMin = item.Delivery.CalculatedDeliveryDateMin,
                            calculatedDeliveryDateMax = item.Delivery.CalculatedDeliveryDateMax,
                            services = item.Services
                        }
                        : new
                        {
                            tariffId = additionalData?.tariffId ?? order.OrderPickPoint?.AdditionalData.TryParseInt() ?? 0,
                            delivery = _preOrder.ShippingOption.FinalRate,
                            partnerId = additionalData?.partnerId ?? 0,
                            calculatedDeliveryDateMin = (string)null,
                            calculatedDeliveryDateMax = (string)null,
                            services = (List<YaNewDeliveryService>)null
                        },
                shipment = !retry && item != null && item.Shipments.Count > 0 && additionalData != null
                    ? new
                    {
                        type = item.Shipments[0].Type,
                        warehouseFrom = _warehouseId,
                        warehouseTo = item.Shipments[0].Warehouse?.Id,
                        partnerTo = item.Shipments[0].Partner?.Id,
                        date = item.Shipments[0].Date
                    }
                    : null,
                places = new List<object>
                {
                    new
                    {
                        externalId = "1",
                        dimensions = new
                        {
                            length = length,
                            width = width,
                            height = height,
                            weight = weight
                        },
                        items = recalculatedOrderItems.Select(x => new YandexNewDeliveryProductItemDto(x)).ToList()
                    }
                }
            };

            var dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            string yndOrderId;
            try
            {
                yndOrderId = _yandexNewDeliveryApiService.MakeRequest(Url + "/orders", dataStr);
            }
            catch (BlException blException)
            {
                if (!retry && blException.Message.Contains("Реестры отправлены"))
                    return CreateOrder(order, recalculatedOrderItems, retry: true);

                throw;
            }

            if (string.IsNullOrEmpty(yndOrderId))
            {
                Debug.Log.Warn("Can't create order in yandex delivery");
                return false;
            }

            OrderService.AddUpdateOrderAdditionalData(order.OrderID, KeyNameOrderIdInOrderAdditionalData, yndOrderId);
            
            return true;
        }

        private YandexNewDeliveryListItem TryGetItem(float weight, long? selectedTariffId, long? selectedPickPointId, bool fullyPrepaid)
        {
            var dimensions = GetCalcDimensions();
            var data = GetParam(dimensions, weight, fullyPrepaid);
            var dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            var responseData = _yandexNewDeliveryApiService.MakeRequest(Url + "/delivery-options", dataStr, "PUT");

            if (!string.IsNullOrEmpty(responseData))
            {
                try
                {
                    var dto = JsonConvert.DeserializeObject<List<YandexNewDeliveryListItem>>(responseData);

                    if (selectedTariffId.HasValue)
                    {
                        return dto?.FirstOrDefault(x => x.TariffId == selectedTariffId.Value && (!selectedPickPointId.HasValue || x.PickupPointIds.Contains(selectedPickPointId.Value)));
                    }

                    if (selectedPickPointId.HasValue)
                    {
                        return dto?.FirstOrDefault(x => x.PickupPointIds != null && x.PickupPointIds.Contains(selectedPickPointId.Value));
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
            return null;
        }
    }

    public enum TypeViewPoints
    {
        [Localize("Через виджет Яндекс.Доставки")]
        WidgetYandexDelivery = 0,

        [Localize("Через Яндекс.Карты")]
        YaWidget = 1,

        [Localize("Списком")]
        List = 2
    }

    public class YandexNewPoint : BaseShippingPoint
    {
        public string AddressComment { get; set; }
        public float PointX { get; set; }
        public float PointY { get; set; }
    }

    public enum PriceDisplayType
    {
        [Localize("Тариф доставки без опций")]
        NoOptions = 0,

        [Localize("Тариф для магазина")]
        ForSender = 1,

        [Localize("Тариф для клиента")]
        ForCustomer = 2
    }

    public enum EExportOrderExternalIdType
    {
        [Localize("Уникальный идентификатор заказа")]
        OrderId = 0,

        [Localize("Номер заказа")]
        OrderNumber = 1,
    }
}
