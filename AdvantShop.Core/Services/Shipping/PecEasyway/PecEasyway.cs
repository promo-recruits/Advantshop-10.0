//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Shipping.PecEasyway.Api;

namespace AdvantShop.Shipping.PecEasyway
{
    [ShippingKey("PecEasyway")]
    public class PecEasyway : BaseShippingWithCargo, IShippingLazyData, IShippingSupportingTheHistoryOfMovement, IShippingSupportingSyncOfOrderStatus, IShippingSupportingPaymentCashOnDelivery
    {
        private readonly string _login;
        private readonly string _password;
        private readonly string _locationFrom;
        private readonly int _increaseDeliveryTime;
        private readonly TypeViewPoints _typeViewPoints;
        private readonly string _yaMapsApiKey;
        private readonly List<int> _deliveryTypes;
        private readonly bool _statusesSync;
        private readonly bool _orderNoPickup;

        private readonly PecEasywayApiService _pecApi;

        public const string KeyNameOrderIdInOrderAdditionalData = "PecEasywayOrderId";
        public const string KeyNameOrderIsCanceledInOrderAdditionalData = "PecEasywayOrderIsCanceled";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public PecEasyway(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items) : base(method, preOrder, items)
        {
            _login = _method.Params.ElementOrDefault(PecEasywayTemplate.Login);
            _password = _method.Params.ElementOrDefault(PecEasywayTemplate.Password);
            _locationFrom = _method.Params.ElementOrDefault(PecEasywayTemplate.LocationFrom);
            _typeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(PecEasywayTemplate.TypeViewPoints).TryParseInt();
            _yaMapsApiKey = _method.Params.ElementOrDefault(PecEasywayTemplate.YaMapsApiKey);
            _increaseDeliveryTime = _method.ExtraDeliveryTime;
            _deliveryTypes = (method.Params.ElementOrDefault(PecEasywayTemplate.DeliveryTypes) ?? string.Empty).Split(",").Select(x => x.TryParseInt()).ToList();
            _statusesSync = method.Params.ElementOrDefault(PecEasywayTemplate.StatusesSync).TryParseBool();
            _orderNoPickup = method.Params.ElementOrDefault(PecEasywayTemplate.OrderNoPickup).TryParseBool();

            var statusesReference = method.Params.ElementOrDefault(PecEasywayTemplate.StatusesReference);
            if (!string.IsNullOrEmpty(statusesReference))
            {
                string[] arr = null;
                _statusesReference =
                    statusesReference.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToDictionary(x => (arr = x.Split(','))[0],
                            x => arr.Length > 1 ? arr[1].TryParseInt(true) : null);
            }
            else
                _statusesReference = new Dictionary<string, int?>();

            _pecApi = new PecEasywayApiService(_login, _password);
        }

        #region IShippingSupportingSyncOfOrderStatus

        public void SyncStatusOfOrder(Order order)
        {
            var pecOrderId = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameOrderIdInOrderAdditionalData);
            if (pecOrderId.IsNotEmpty())
            {
                var getStatuses = _pecApi.GetOrdersStatuses(pecOrderId);
                if (getStatuses != null && getStatuses.Success)
                {
                    var statusInfo = 
                        getStatuses.Result
                            .Where(x => 
                                StatusesReference.ContainsKey(x.StatusCode)
                                && StatusesReference[x.StatusCode].HasValue)
                            .OrderByDescending(x => x.Date)
                            .FirstOrDefault();

                    var pecOrderStatus = statusInfo != null && StatusesReference.ContainsKey(statusInfo.StatusCode)
                        ? StatusesReference[statusInfo.StatusCode]
                        : null;

                    if (pecOrderStatus.HasValue &&
                        order.OrderStatusId != pecOrderStatus.Value &&
                        OrderStatusService.GetOrderStatus(pecOrderStatus.Value) != null)
                    {
                        var lastOrderStatusHistory =
                            OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                .OrderByDescending(item => item.Date)
                                .FirstOrDefault();

                        if (lastOrderStatusHistory == null ||
                            lastOrderStatusHistory.Date < statusInfo.Date)
                        {
                            OrderStatusService.ChangeOrderStatus(order.OrderID,
                                pecOrderStatus.Value, "Синхронизация статусов для ПЭК:EASYWAY");
                        }
                    }
                }
            }
        }

        public bool SyncByAllOrders => false;
        public void SyncStatusOfOrders(IEnumerable<Order> orders) => throw new NotImplementedException();

        public bool StatusesSync
        {
            get { return _statusesSync; }
        }

        private Dictionary<string, int?> _statusesReference;
        public Dictionary<string, int?> StatusesReference
        {
            get
            {
                return _statusesReference;
            }
        }

        #endregion

        #region IShippingSupportingTheHistoryOfMovement

        public bool ActiveHistoryOfMovement
        {
            get { return true; }
        }

        public List<HistoryOfMovement> GetHistoryOfMovement(Order order)
        {
            return null;
        }

        public PointInfo GetPointInfo(Order order)
        {
            if (order.OrderPickPoint == null || order.OrderPickPoint.PickPointId.IsNullOrEmpty())
                return null;

            var points = GetAllPoints();

            if (points == null || points.Count == 0)
                return null;

            var hermesPoint = points.FirstOrDefault(x => x.Guid == order.OrderPickPoint.PickPointId);

            return hermesPoint != null
                ? new PointInfo()
                {
                    Address = hermesPoint.Address,
                    TimeWork = hermesPoint.Schedule,
                    Comment = hermesPoint.TripDescription,
                    Phone = hermesPoint.Phone
                }
                : null;
        }

        #endregion IShippingSupportingTheHistoryOfMovement

        public PecEasywayApiService PecEasywayApiService
        {
            get { return _pecApi; }
        }

        public bool OrderNoPickup
        {
            get { return _orderNoPickup; }
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var shippingOptions = new List<BaseShippingOption>();

            var city = _preOrder.CityDest;
            var region = _preOrder.RegionDest;
            var country = _preOrder.CountryDest;

            if (_locationFrom.IsNotEmpty() && city.IsNotEmpty())
            {
                var weight = GetTotalWeight();
                var dimensions = GetDimensions().Select(x => x / 1000d).ToArray();// конвертируем сами, чтобы получить большую точность (float для таких значений сильно ограничен)

                var calculateResponse = 
                    _pecApi.CalculateDelivery(_locationFrom,
                        locationTo: string.Join(", ", new[] { country, region, city }.Where(x => x.IsNotEmpty())),
                        weight: weight,
                        volume: dimensions.Aggregate(1d, (x, y) => x * y));

                if (calculateResponse != null && calculateResponse.Success && calculateResponse.Result != null)
                {
                    var deliveries = calculateResponse.Result
                        .Where(x => x.Total > 0)
                        .Where(x => _deliveryTypes.Contains(x.DeliveryType.Value))
                        .ToList();

                    var loadPoints = deliveries.Any(x => IsDeliveryTypeToPoint(x.DeliveryType));

                    List<PickupPoint> deliveryPoints = loadPoints
                        ? GetPointsCity(region, city)
                        : null;

                    string selectedPoint = null;
                    PickupPoint deliveryPoint = null;
                    if (_preOrder.ShippingOption != null &&
                        _preOrder.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(PecEasyway).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        if (_preOrder.ShippingOption.GetType() == typeof(PecEasywayPointDeliveryMapOption))
                            selectedPoint = ((PecEasywayPointDeliveryMapOption)_preOrder.ShippingOption).PickpointId;

                        if (_preOrder.ShippingOption.GetType() == typeof(PecEasywayListOption) && ((PecEasywayListOption)_preOrder.ShippingOption).SelectedPoint != null)
                            selectedPoint = ((PecEasywayListOption)_preOrder.ShippingOption).SelectedPoint.Code;
                    }

                    foreach (var delivery in deliveries)
                    {
                        if (IsDeliveryTypeToPoint(delivery.DeliveryType) &&
                            (deliveryPoints == null || deliveryPoints.Count == 0))
                            continue;

                        var calculateOption = new PecEasywayCalculateOption
                        {
                            LocationFrom = _locationFrom,
                            DeliveryType = delivery.DeliveryType.Value
                        };
                        var name = string.Format("{0} ({1})",
                                    _method.Name,
                                    delivery.DeliveryType.Localize().ToLower());

                        var minDeliveryTime = delivery.EstDeliveryTime != null ? delivery.EstDeliveryTime.Min.TryParseInt(true) : null;
                        var maxDeliveryTime = delivery.EstDeliveryTime != null ? delivery.EstDeliveryTime.Max.TryParseInt(true) : null;

                        var deliveryTimeStr = delivery.EstDeliveryTime != null
                                ? string.Format("{0:. дн\\.}{1}{2:. дн\\.}",
                                    minDeliveryTime.HasValue ? minDeliveryTime.Value + _increaseDeliveryTime : (int?)null,
                                    minDeliveryTime.HasValue && maxDeliveryTime.HasValue ? " - " : "",
                                    maxDeliveryTime + _increaseDeliveryTime)
                                : null;

                        var rate = delivery.Total;

                        if (!IsDeliveryTypeToPoint(delivery.DeliveryType))
                        {
                            var option = new PecEasywayOption(_method, _totalPrice)
                            {
                                DeliveryId = delivery.DeliveryType.Value,
                                Name = name,
                                Rate = rate,
                                BasePrice = rate,
                                PriceCash = rate,
                                DeliveryTime = deliveryTimeStr,
                                IsAvailablePaymentCashOnDelivery = true,
                                CalculateOption = calculateOption,
                            };

                            shippingOptions.Add(option);
                        }
                        else
                        {
                            List<PickupPoint> currentDeliveryPoints;
                            if (delivery.DeliveryType == EnDeliveryType.PVZ)
                                currentDeliveryPoints = deliveryPoints.Where(x => x.IsTerminal == false).ToList();
                            else
                                currentDeliveryPoints = deliveryPoints.Where(x => x.IsTerminal == true).ToList();

                            if (currentDeliveryPoints.Count == 0)
                                continue;

                            deliveryPoint = selectedPoint != null
                               ? currentDeliveryPoints.FirstOrDefault(x => x.Guid == selectedPoint) ?? currentDeliveryPoints[0]
                               : currentDeliveryPoints[0];

                            List<PecEasywayPoint> shippingPoints = CastPoints(currentDeliveryPoints);

                            if (_typeViewPoints == TypeViewPoints.List ||
                                (_typeViewPoints == TypeViewPoints.YaWidget && _yaMapsApiKey.IsNullOrEmpty()))
                            {
                                var option = new PecEasywayListOption(_method, _totalPrice)
                                {
                                    DeliveryId = delivery.DeliveryType.Value,
                                    Name = name,
                                    Rate = rate,
                                    BasePrice = rate,
                                    PriceCash = rate,
                                    DeliveryTime = deliveryTimeStr,
                                    IsAvailablePaymentCashOnDelivery = true,
                                    ShippingPoints = shippingPoints.OrderBy(x => x.Address).ToList(),
                                    SelectedPoint = shippingPoints.FirstOrDefault(x => x.Code.Equals(deliveryPoint.Guid, StringComparison.OrdinalIgnoreCase)),
                                    CalculateOption = calculateOption,
                                };

                                shippingOptions.Add(option);
                            }
                            else if (_typeViewPoints == TypeViewPoints.YaWidget)
                            {
                                var option = new PecEasywayPointDeliveryMapOption(_method, _totalPrice)
                                {
                                    DeliveryId = delivery.DeliveryType.Value,
                                    Name = name,
                                    Rate = rate,
                                    BasePrice = rate,
                                    PriceCash = rate,
                                    DeliveryTime = deliveryTimeStr,
                                    IsAvailablePaymentCashOnDelivery = true,
                                    CurrentPoints = shippingPoints,
                                    CalculateOption = calculateOption,
                                };
                                SetMapData(option, country, region, city, delivery.DeliveryType != EnDeliveryType.PVZ);

                                shippingOptions.Add(option);
                            }
                        }
                    }
                }
            }

            return shippingOptions;
        }

        public bool IsDeliveryTypeToPoint(EnDeliveryType deliveryType)
        {
            return deliveryType != null && deliveryType == EnDeliveryType.Terminal ||
                deliveryType == EnDeliveryType.TerminalAvia ||
                deliveryType == EnDeliveryType.PVZ;
        }

        private void SetMapData(PecEasywayPointDeliveryMapOption option, string country, string region, string city, bool isTerminal)
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
            option.PointParams.IsLazyPoints = (option.CurrentPoints != null ? option.CurrentPoints.Count : 0) > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "region", region },
                    { "city", city },
                    { "isTerminal", isTerminal },
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<PecEasywayPoint> points)
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
                                    p.Description.IsNotEmpty() ? "<br>" : "",
                                    p.Id,
                                    p.Code),
                            BalloonContentFooter = /*_showAddressComment
                                ?*/ p.AddressComment
                            //: null
                        }
                    }).ToList()
            };
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("region") || !data.ContainsKey("city") ||
                !data.ContainsKey("isTerminal"))
                return null;

            var region = (string)data["region"];
            var city = (string)data["city"];
            var isTerminal = data["isTerminal"].ToString().TryParseBool();

            var deliveryPoints = GetPointsCity(region, city);
            deliveryPoints = deliveryPoints.Where(x => x.IsTerminal == isTerminal).ToList();
            var points = CastPoints(deliveryPoints);

            return GetFeatureCollection(points);
        }

        public List<PickupPoint> GetAllPoints()
        {
            List<PickupPoint> points = null;
            var pointsCacheKey = string.Format("Pic-{0}-PickupPoints", (_login + _password).GetHashCode());
            if (!CacheManager.TryGetValue(pointsCacheKey, out points))
            {
                var pickupPoints = _pecApi.GetPickupPoints();
                if (pickupPoints != null && pickupPoints.Success == true)
                {
                    points = pickupPoints.Result;
                    if (points != null)
                        CacheManager.Insert(pointsCacheKey, points, 60 * 24);
                }
            }

            return points ?? new List<PickupPoint>();
        }

        private List<PickupPoint> GetPointsCity(string region, string city)
        {
            if (city.IsNullOrEmpty())
                return null;

            List<PickupPoint> points = GetAllPoints();

            if (points != null && points.Count > 0)
            {
                var regionFind = region.RemoveTypeFromRegion();

                points = points
                    // имеет координаты
                    .Where(x => x.Latitude.HasValue && x.Longitude.HasValue)
                    // нужного региона
                    .Where(x => (regionFind.IsNullOrEmpty() || x.Region.IsNotEmpty()) && x.Region.Contains(regionFind, StringComparison.OrdinalIgnoreCase))
                    // нужного города
                    .Where(x => x.City.IsNotEmpty() && x.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return points;
        }

        private static List<PecEasywayPoint> CastPoints(List<PickupPoint> points)
        {
            var result = new List<PecEasywayPoint>();
            foreach (var point in points)
            {
                result.Add(new PecEasywayPoint
                {
                    Id = point.Guid.GetHashCode(),
                    Code = point.Guid,
                    Address = point.Address,
                    Description = 
                    string.Format("{0}{1}",
                        point.Schedule,
                        point.Phone.IsNotEmpty() ? " " + point.Phone : null),
                    AddressComment = point.TripDescription,
                    PointX = point.Latitude.Value,
                    PointY = point.Longitude.Value,
                });
            }
            return result;
        }
    }

    public enum TypeViewPoints
    {
        [Localize("Через Яндекс.Карты")]
        YaWidget = 0,

        [Localize("Списком")]
        List = 2
    }

    public class PecEasywayPoint : BaseShippingPoint
    {
        public string AddressComment { get; set; }
        public float PointX { get; set; }
        public float PointY { get; set; }
    }
}
