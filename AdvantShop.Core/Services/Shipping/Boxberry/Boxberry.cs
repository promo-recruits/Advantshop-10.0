//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Linq;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping.Boxberry;
using AdvantShop.Orders;
using System;

namespace AdvantShop.Shipping.Boxberry
{
    [ShippingKey("Boxberry")]
    public class Boxberry : BaseShippingWithCargo, IShippingSupportingSyncOfOrderStatus, IShippingSupportingTheHistoryOfMovement, IShippingLazyData, IShippingSupportingPaymentCashOnDelivery
    {
        #region Ctor

        private readonly string _apiUrl;
        private readonly string _token;
        private readonly string _integrationToken;
        private readonly string _receptionPointCode;
        private readonly bool _calculateCourierOld;
        private readonly bool _statusesSync;
        private readonly int _increaseDeliveryTime;
        private readonly TypeOption _typeOption;
        private readonly List<TypeDelivery> _deliveryTypes;
        private readonly string _yaMapsApiKey;
        private readonly bool _withInsure;

        private readonly BoxberryApiService _boxberryApiService;

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public Boxberry(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items) : base(method, preOrder, items)
        {
            _apiUrl = _method.Params.ElementOrDefault(BoxberryTemplate.ApiUrl);
            _token = _method.Params.ElementOrDefault(BoxberryTemplate.Token);

            if (_apiUrl == null)
                _apiUrl = _token.IsNotEmpty() ? "http://api.boxberry.de/json.php" : "http://api.boxberry.ru/json.php";

            _integrationToken = _method.Params.ElementOrDefault(BoxberryTemplate.IntegrationToken);
            _receptionPointCode = _method.Params.ElementOrDefault(BoxberryTemplate.ReceptionPointCode);
            _statusesSync = method.Params.ElementOrDefault(BoxberryTemplate.StatusesSync).TryParseBool();
            _typeOption = (TypeOption)method.Params.ElementOrDefault(BoxberryTemplate.TypeOption).TryParseInt();
            _yaMapsApiKey = _method.Params.ElementOrDefault(BoxberryTemplate.YaMapsApiKey);
            _withInsure = method.Params.ElementOrDefault(BoxberryTemplate.WithInsure).TryParseBool();
            _boxberryApiService = new BoxberryApiService(_apiUrl, _token, _receptionPointCode);
            _increaseDeliveryTime = _method.ExtraDeliveryTime;

            if (method.Params.ContainsKey(BoxberryTemplate.DeliveryTypes))
            {
                _deliveryTypes = (method.Params.ElementOrDefault(BoxberryTemplate.DeliveryTypes) ?? string.Empty).Split(",").Select(x => x.TryParseInt()).Cast<TypeDelivery>().ToList();
            }
            else
            {
                _calculateCourierOld = Convert.ToBoolean(_method.Params.ElementOrDefault(BoxberryTemplate.CalculateCourier));

                _deliveryTypes = Enum.GetValues(typeof(TypeDelivery)).Cast<TypeDelivery>().ToList();
                if (_calculateCourierOld == false)
                    _deliveryTypes.Remove(TypeDelivery.Courier);
            }

        }

        #endregion

        #region Statuses

        public void SyncStatusOfOrder(Order order)
        {
            // не используется ListStatusesFull, т.к. в нем возвращается время только до минут
            var statusInfoAnswer = _boxberryApiService.ListStatuses(order.TrackNumber.IsNotEmpty() ? order.TrackNumber : order.OrderID.ToString());

            if (statusInfoAnswer == null || statusInfoAnswer.Result == null || statusInfoAnswer.Result.Count <= 0)
                return;

            var statusInfo = 
                statusInfoAnswer.Result
                    .Where(x => StatusesReference.ContainsKey(x.Name) 
                                && StatusesReference[x.Name].HasValue)
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();
           
            var boxberryOrderStatus = statusInfo != null && StatusesReference.ContainsKey(statusInfo.Name)
                ? StatusesReference[statusInfo.Name]
                : null;

            if (boxberryOrderStatus.HasValue &&
                order.OrderStatusId != boxberryOrderStatus.Value &&
                OrderStatusService.GetOrderStatus(boxberryOrderStatus.Value) != null)
            {
                var lastOrderStatusHistory =
                    OrderStatusService.GetOrderStatusHistory(order.OrderID)
                        .OrderByDescending(item => item.Date)
                        .FirstOrDefault();

                if (lastOrderStatusHistory == null ||
                    lastOrderStatusHistory.Date < statusInfo.Date)
                {
                    OrderStatusService.ChangeOrderStatus(order.OrderID,
                        boxberryOrderStatus.Value, "Синхронизация статусов для Boxberry");
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
                if (_statusesReference == null)
                {
                    _statusesReference = new Dictionary<string, int?>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Загружен реестр ИМ", _method.Params.ElementOrDefault(BoxberryTemplate.Status_Created).TryParseInt(true)},
                        { "Принято к доставке", _method.Params.ElementOrDefault(BoxberryTemplate.Status_AcceptedForDelivery).TryParseInt(true)},
                        { "Заказ передан на доставку", _method.Params.ElementOrDefault(BoxberryTemplate.Status_AcceptedForDelivery).TryParseInt(true)},
                        { "Отправлен на сортировочный терминал", _method.Params.ElementOrDefault(BoxberryTemplate.Status_SentToSorting).TryParseInt(true)},
                        { "Передано на сортировку", _method.Params.ElementOrDefault(BoxberryTemplate.Status_TransferredToSorting).TryParseInt(true)},
                        { "Отправлено в город назначения", _method.Params.ElementOrDefault(BoxberryTemplate.Status_SentToDestinationCity).TryParseInt(true)},
                        { "В пути в город получателя", _method.Params.ElementOrDefault(BoxberryTemplate.Status_SentToDestinationCity).TryParseInt(true)},
                        { "Передано на курьерскую доставку", _method.Params.ElementOrDefault(BoxberryTemplate.Status_Courier).TryParseInt(true)},
                        { "Поступил в город для передачи курьеру", _method.Params.ElementOrDefault(BoxberryTemplate.Status_Courier).TryParseInt(true)},
                        { "Поступило в пункт выдачи", _method.Params.ElementOrDefault(BoxberryTemplate.Status_PickupPoint).TryParseInt(true)},
                        { "Доступен к получению в Пункте выдачи", _method.Params.ElementOrDefault(BoxberryTemplate.Status_PickupPoint).TryParseInt(true)},
                        { "Выдано", _method.Params.ElementOrDefault(BoxberryTemplate.Status_Delivered).TryParseInt(true)},
                        { "Успешно Выдан", _method.Params.ElementOrDefault(BoxberryTemplate.Status_Delivered).TryParseInt(true)},
                        { "Готовится к возврату", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnPreparing).TryParseInt(true)},
                        // дубль { "Заказ передан на возврат в Интернет-магазин", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnPreparing).TryParseInt(true)},
                        { "Отправлено в пункт приема", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnSentToReceivingPoint).TryParseInt(true)},
                        { "Заказ в пути в Интернет-магазин", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnSentToReceivingPoint).TryParseInt(true)},
                        { "Возвращено в пункт приема", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnReturnedToReceivingPoint).TryParseInt(true)},
                        { "Возвращено с курьерской доставки", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnByCourier).TryParseInt(true)},
                        // дубль { "Заказ передан на возврат в Интернет-магазин", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnByCourier).TryParseInt(true)},
                        { "Возвращено в ИМ", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnReturned).TryParseInt(true)},
                        { "Заказ возвращен в Интернет-магазин", _method.Params.ElementOrDefault(BoxberryTemplate.Status_ReturnReturned).TryParseInt(true)},
                    };
                }
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
            var statusInfoAnswer = _boxberryApiService.ListStatuses(order.TrackNumber.IsNotEmpty() ? order.TrackNumber : order.OrderID.ToString());

            if (statusInfoAnswer == null || statusInfoAnswer.Result == null || statusInfoAnswer.Result.Count <= 0)
                return null;

            return statusInfoAnswer.Result.OrderByDescending(x => x.Date).Select(statusInfo => new HistoryOfMovement()
            {
                Code = statusInfo.Name,
                Name = statusInfo.Name,
                Date = statusInfo.Date,
                Comment = statusInfo.Comment
            }).ToList();
        }

        public PointInfo GetPointInfo(Order order)
        {
            if (order.OrderPickPoint == null || order.OrderPickPoint.PickPointId.IsNullOrEmpty())
                return null;

            var boxberryPoint = _boxberryApiService.GetListPoints(string.Empty)
                .FirstOrDefault(x => order.OrderPickPoint.PickPointId == x.Code);

            return boxberryPoint != null
                ? new PointInfo()
                {
                    Phone = boxberryPoint.Phone,
                    Address = boxberryPoint.Address,
                    TimeWork = boxberryPoint.WorkSchedule,
                    //Comment = boxberryPoint.
                }
                : null;
        }

        #endregion IShippingSupportingTheHistoryOfMovement

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var orderPrice = _totalPrice;
            float orderWeight = (int)GetTotalWeight(1000);// граммы, поэтому целое

            var boxberryOptions = _boxberryApiService.GetBoxberryOptions();
            var hideDeliveryTime = false;
            if (boxberryOptions != null && boxberryOptions.Result != null && boxberryOptions.Result.Settings3 != null)
            {
                hideDeliveryTime = boxberryOptions.Result.Settings3.HideDeliveryDay == 1;
            }

            var result = new List<BaseShippingOption>();

            if (!string.IsNullOrEmpty(_preOrder.CityDest) && _deliveryTypes.Contains(TypeDelivery.PVZ))
            {
                var cities = _boxberryApiService.GetListCities();

                if (cities != null)
                {

                    cities.ForEach(x => x.Name.Replace("ё", "е"));
                    cities.ForEach(x => x.Region = x.Region?.RemoveTypeFromRegion());

                    var normolizeCityName = _preOrder.CityDest.Replace("ё", "е");
                    var normolizeRegionDest = _preOrder.RegionDest?.RemoveTypeFromRegion();
                    BoxberryCity boxberryCity =
                        GetCity(cities, normolizeCityName, _preOrder.DistrictDest, normolizeRegionDest)// по всем данным
                        ?? GetCity(cities, normolizeCityName, null, normolizeRegionDest);// без района
                    //?? GetCity(cities, normolizeCityName, null, null); // без района и региона (ненадо, считает не втот город)

                    if (boxberryCity != null)
                    {
                        string selectedPoint = null;
                        if (_preOrder.ShippingOption != null &&
                            _preOrder.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(Boxberry).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                        {
                            if (_preOrder.ShippingOption.GetType() == typeof(BoxberryWidgetOption))
                                selectedPoint = ((BoxberryWidgetOption)_preOrder.ShippingOption).PickpointId;

                            if (_preOrder.ShippingOption.GetType() == typeof(BoxberryPointDeliveryMapOption))
                                selectedPoint = ((BoxberryPointDeliveryMapOption)_preOrder.ShippingOption).PickpointId;
                        }

                        if (_typeOption == TypeOption.WidgetBoxberry || (_typeOption == TypeOption.YaWidget && _yaMapsApiKey.IsNullOrEmpty()))
                        {

                            var optionWidget = GetShippingWidgentOption(boxberryCity, selectedPoint, orderWeight, orderPrice, hideDeliveryTime);
                            if (optionWidget != null)
                            {
                                result.Add(optionWidget);
                            }
                        }

                        if (_typeOption == TypeOption.YaWidget && _yaMapsApiKey.IsNotEmpty())
                        {
                            var optionWidget = GetShippingYaWidgentOption(boxberryCity, selectedPoint, orderWeight, orderPrice, hideDeliveryTime);
                            if (optionWidget != null)
                            {
                                result.Add(optionWidget);
                            }
                        }
                    }
                }
            }
            if (_deliveryTypes.Contains(TypeDelivery.Courier))
            {
                var optionZip = GetShippingOptionZip(orderWeight, orderPrice, hideDeliveryTime);
                if (optionZip != null)
                {
                    result.Add(optionZip);
                }
            }

            return result;
        }

        private BoxberryCity GetCity(IEnumerable<BoxberryCity> cities, string city, string district, string region)
        {
            if (city.IsNotEmpty())
                cities = cities.Where(x => x.Name.Equals(city, StringComparison.OrdinalIgnoreCase));
            if (district.IsNotEmpty())
                cities = cities.Where(x => x.District.Contains(district, StringComparison.OrdinalIgnoreCase));
            if (region.IsNotEmpty())
                cities = cities.Where(x => region.Contains(x.Region, StringComparison.OrdinalIgnoreCase) || x.Region.Contains(region, StringComparison.OrdinalIgnoreCase));

            return cities.FirstOrDefault();
        }

        /*
         *boxberry.open(‘callback_function’,‘api_token’,‘custom_city’,’target_start’,’ordersum’,’weight’,’paysum’,’height’,’width’,’depth’)
         */
        private BoxberryWidgetOption GetShippingWidgentOption(BoxberryCity boxberryCity, string selectedPickpointId, float totalWeight, float totalPrice, bool hideDeliveryTime)
        {
            var dimensions = GetDimensions(rate: 10);
            
            //поворачиваем наилучшим способом под ограничения
            var dimensionsValidate = dimensions.OrderByDescending(x => x).ToArray();

            //Максимальные габариты отправления могут быть 120см*80см*50см
            if (dimensionsValidate[0] > 120 ||
                dimensionsValidate[1] > 80 ||
                dimensionsValidate[2] > 50)
                return null;

            var points = FilterPoints(_boxberryApiService.GetListPoints(boxberryCity.Code), totalWeight, dimensions);
            if (points == null || points.Count == 0 ||
                (points != null && points.Count > 0 && !string.IsNullOrEmpty(points[0].Error)))
            {
                return null;
            }
            var point = selectedPickpointId != null
                ? points.FirstOrDefault(x => selectedPickpointId == x.Code) ?? points[0]
                : points[0];

            var deliveryCost = _boxberryApiService.GetDeliveryCosts(
                  point.Code,
                  totalWeight,
                  _withInsure ? totalPrice : (float?)null,
                  0,
                  dimensions[0],
                  dimensions[1],
                  dimensions[2],
                  "",
                  null);

            if (deliveryCost == null)
                return null;

            var deliveryCostCash = point.OnlyPrepaidOrders != "Yes" 
                ? _boxberryApiService.GetDeliveryCosts(
                  point.Code,
                  totalWeight,
                  totalPrice,
                  0,
                  dimensions[0],
                  dimensions[1],
                  dimensions[2],
                  "",
                  totalPrice)
                : null;

            return new BoxberryWidgetOption(_method, _totalPrice)
            {
                Name = _method.Name + " (постаматы и пункты выдачи)",
                WidgetConfigData = new Dictionary<string, object>
                {
                    {"api_token", _integrationToken},
                    {"city", boxberryCity.Name }, // для проверки смены города через виджет
                    {"custom_city", string.Join(", ", new[] { _preOrder.RegionDest, boxberryCity.Name }.Where(x => x.IsNotEmpty())) }, // для виджета
                    {"targetstart", _receptionPointCode},
                    {"ordersum", _withInsure ? totalPrice : 0f},
                    {"weight", totalWeight},
                    //{"paysum", 0f}, // реализованно на уровне BoxberryWidgetOption
                    {"height", dimensions[0]},
                    {"width", dimensions[1]},
                    {"depth", dimensions[2]}
                },
                DeliveryTime =
                    hideDeliveryTime
                        ? string.Empty
                        : deliveryCost.DeliveryPeriod + _increaseDeliveryTime + " дн.",
                Rate = deliveryCost.Price,
                BasePrice = deliveryCost.Price,
                PriceCash = deliveryCostCash?.Price ?? deliveryCost.Price,
                HideAddressBlock = true,
                DisplayIndex = true,
                TotalOrderPrice = totalPrice,
                WithInsure = _withInsure,
                IsAvailablePaymentCashOnDelivery = deliveryCostCash != null
            };
        }

        private BoxberryPointDeliveryMapOption GetShippingYaWidgentOption(BoxberryCity boxberryCity, string selectedPickpointId, float totalWeight, float totalPrice, bool hideDeliveryTime)
        {
            var dimensions = GetDimensions(rate: 10);

            //поворачиваем наилучшим способом под ограничения
            var dimensionsValidate = dimensions.OrderByDescending(x => x).ToArray();

            //Максимальные габариты отправления могут быть 120см*80см*50см
            if (dimensionsValidate[0] > 120 ||
                dimensionsValidate[1] > 80 ||
                dimensionsValidate[2] > 50)
                return null;

            var points = FilterPoints(_boxberryApiService.GetListPoints(boxberryCity.Code), totalWeight, dimensions);
            if (points == null || points.Count == 0 ||
                (points != null && points.Count > 0 && !string.IsNullOrEmpty(points[0].Error)))
            {
                return null;
            }
            var point = selectedPickpointId != null
                ? points.FirstOrDefault(x => selectedPickpointId == x.Code) ?? points[0]
                : points[0];

            var deliveryCost = _boxberryApiService.GetDeliveryCosts(
                  point.Code,
                  totalWeight,
                  _withInsure ? totalPrice : (float?)null,
                  0,
                  dimensions[0],
                  dimensions[1],
                  dimensions[2],
                  "",
                  null);

            if (deliveryCost == null)
                return null;

            var deliveryCostCash = point.OnlyPrepaidOrders != "Yes" 
                ? _boxberryApiService.GetDeliveryCosts(
                  point.Code,
                  totalWeight,
                  totalPrice,
                  0,
                  dimensions[0],
                  dimensions[1],
                  dimensions[2],
                  "",
                  totalPrice)
                : null;

            List<BoxberryPoint> shippingPoints = CastPoints(points);

            var option = new BoxberryPointDeliveryMapOption(_method, _totalPrice)
            {
                Name = _method.Name + " (постоматы и пункты выдачи)",
                DeliveryTime =
                    hideDeliveryTime
                        ? string.Empty
                        : deliveryCost.DeliveryPeriod + _increaseDeliveryTime + " дн.",
                Rate = deliveryCost.Price,
                BasePrice = deliveryCost.Price,
                PriceCash = deliveryCostCash?.Price ?? deliveryCost.Price,
                IsAvailablePaymentCashOnDelivery = deliveryCostCash != null,
                CurrentPoints = shippingPoints,
            };
            SetMapData(option, boxberryCity.Code, totalWeight, dimensions);

            return option;
        }

        private List<BoxberryObjectPoint> FilterPoints(List<BoxberryObjectPoint> points, float totalWeight, float[] dimensions)
        {
            if (points == null)
                return points;

            var filterWeight = totalWeight / 1000;// переводим в кг
            var filterVolume = (dimensions[0] * dimensions[1] * dimensions[2]) / 1000000d; // переводим в м3
            return points.Where(
                x =>
                (x.LoadLimit == null ? filterWeight <= 15f : filterWeight <= x.LoadLimit.Value) &&
                (x.VolumeLimit == null || filterVolume <= x.VolumeLimit.Value))
                .ToList();
        }

        private static List<BoxberryPoint> CastPoints(List<BoxberryObjectPoint> points)
        {
            var result = new List<BoxberryPoint>();
            foreach(var point in points)
            {
                var gps = point.GPS.Split(',');

                result.Add(new BoxberryPoint
                {
                    Id = point.Code.GetHashCode(),
                    Code = point.Code,
                    Address = point.Address,
                    Description = string.Join("<br/>", new[] { point.Phone, point.WorkSchedule, point.TripDescription }.Where(x => x.IsNotEmpty())),
                    DeliveryPeriod = point.DeliveryPeriod,
                    OnlyPrepaidOrders = point.OnlyPrepaidOrders == "Yes",
                    PointX = gps[1].TryParseFloat(),
                    PointY = gps[0].TryParseFloat(),
                });
            }
            return result;
        }

        private void SetMapData(BoxberryPointDeliveryMapOption option, string boxberryCityCode, float totalWeight, float[] dimensions)
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
            option.MapParams.Destination = string.Join(", ", new[] { _preOrder.CountryDest, _preOrder.RegionDest, _preOrder.DistrictDest, _preOrder.CityDest }.Where(x => x.IsNotEmpty()));

            option.PointParams = new PointDelivery.PointParams();
            option.PointParams.IsLazyPoints = (option.CurrentPoints != null ? option.CurrentPoints.Count : 0) > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "city", boxberryCityCode },
                    { "weight", (int)totalWeight },
                    { "dimensions", string.Join("x", dimensions.Select(x => x.ToInvariantString())) },
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("city") || data["city"] == null
                || !data.ContainsKey("weight") || data["weight"] == null
                || !data.ContainsKey("dimensions") || data["dimensions"] == null)
                return null;

            var city = (string)data["city"];
            var weight = data["weight"].ToString().TryParseInt();
            var dimensions = data["dimensions"].ToString().Split('x').Select(x => x.TryParseFloat()).ToArray();
            var points = CastPoints(FilterPoints(_boxberryApiService.GetListPoints(city), weight, dimensions));

            return GetFeatureCollection(points);
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<BoxberryPoint> points)
        {
            return new PointDelivery.FeatureCollection
            {
                Features = points.Select(p =>
                    new PointDelivery.Feature
                    {
                        Id = p.Id,
                        Geometry = new PointDelivery.PointGeometry { PointX = p.PointY, PointY = p.PointX },
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
                            //BalloonContentFooter = _showAddressComment
                            //    ? p.AddressComment
                            //    : null
                        }
                    }).ToList()
            };
        }

        private BoxberryOption GetShippingOptionZip(float totalWeight, float totalPrice, bool hideDeliveryTime)
        {
            if (string.IsNullOrEmpty(_preOrder.ZipDest))
            {
                return null;
            }

            if (!_boxberryApiService.ZipCheck(_preOrder.ZipDest))
            {
                return null;
            }

            //до 15 кг
            if (totalWeight > 15000)
                return null;

            var dimensions = GetDimensions(rate: 10);

            //поворачиваем наилучшим способом под ограничения
            var dimensionsValidate = dimensions.OrderByDescending(x => x).ToArray();

            //Максимальные габариты отправления могут быть 120см*80см*50см
            if (dimensionsValidate[0] > 120 ||
                dimensionsValidate[1] > 80 ||
                dimensionsValidate[2] > 50)
                return null;

            var cities = _boxberryApiService.GetCourierListCities();
            var normolizeCityName = _preOrder.CityDest.Replace("ё", "е");
            if (!cities.Any(item => item.City.Replace("ё", "е").Equals(normolizeCityName, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            var deliveryCost = _boxberryApiService.GetDeliveryCosts(
                     string.Empty,
                     totalWeight,
                     _withInsure ? totalPrice : (float?)null,
                     0, dimensions[0], dimensions[1], dimensions[2],
                     _preOrder.ZipDest,
                     null);

            if (deliveryCost == null)
                return null;

            var deliveryCostCash = _boxberryApiService.GetDeliveryCosts(
                 string.Empty,
                 totalWeight,
                 totalPrice,
                 0, dimensions[0], dimensions[1], dimensions[2],
                 _preOrder.ZipDest,
                 totalPrice);

            var shippingOption = new BoxberryOption(_method, _totalPrice)
            {
                Name = _method.Name + " (курьером)",
                Rate = deliveryCost.Price,
                BasePrice = deliveryCost.Price,
                PriceCash = deliveryCostCash?.Price ?? deliveryCost.Price,
                DeliveryTime = hideDeliveryTime ? string.Empty : deliveryCost.DeliveryPeriod + _increaseDeliveryTime + " дн.",
                DisplayIndex = true,
                HideAddressBlock = false
            };
            return shippingOption;
        }

        #region ApiMethods

        public BoxberryOrderTrackNumber CreateOrUpdateOrder(Order order)
        {
            var dimensions = GetDimensions(rate: 10).Select(x => (int)Math.Ceiling(x)).ToArray();
            return _boxberryApiService.ParselCreate(order, (int)GetTotalWeight(1000), dimensions, _withInsure, _method.ShippingCurrency);
        }

        public BoxberryOrderDeleteAnswer DeleteOrder(string trackNumber)
        {
            return _boxberryApiService.ParselDelete(trackNumber);
        }

        //protected override int GetHashForCache()
        //{
        //    var totalPrice = _items.Sum(item => item.Price * item.Amount);
        //    var str =
        //        string.Format(
        //            "checkout/calculation?ClientId={0}&CityDest={1}&RegionDest={2}&ZipDest={3}&totalSum={4}&assessedSum={5}&totalWeight={6}&itemsCount={7}&paymentMethod=cash",
        //            _token,
        //            _preOrder.CityDest,
        //            _preOrder.RegionDest,
        //            _preOrder.ZipDest,
        //            _preOrder.
        //            Math.Ceiling(totalPrice),
        //            Math.Ceiling(totalPrice),
        //            _items.Sum(item => item.Weight * item.Amount).ToString().Replace(",", "."),
        //            Math.Ceiling(_items.Sum(x => x.Amount)));
        //    var hash = _method.ShippingMethodId ^ str.GetHashCode();
        //    return hash;
        //}

        #endregion
    }

    public enum TypeOption
    {
        [Localize("Через виджет Boxberry")]
        WidgetBoxberry = 0,

        [Localize("Через Яндекс.Карты")]
        YaWidget = 1
    }

    public enum TypeDelivery
    {
        [Localize("Доставка до пункта выдачи заказов")]
        PVZ = 0,

        [Localize("Доставка курьером")]
        Courier = 1
    }
}