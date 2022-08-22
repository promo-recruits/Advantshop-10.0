using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping.PickPoint.Api;
using AdvantShop.Core.Services.Shipping.PickPoint.Postamats;
using AdvantShop.Orders;

namespace AdvantShop.Shipping.PickPoint
{
    [ShippingKey("PickPoint")]
    public class PickPoint : BaseShippingWithCargo, IShippingLazyData, IShippingSupportingPaymentCashOnDelivery, IShippingSupportingTheHistoryOfMovement, IShippingWithBackgroundMaintenance, IShippingSupportingSyncOfOrderStatus
    {
        #region Ctor

        private readonly string _login;
        private readonly string _password;
        private readonly string _ikn;
        private readonly string _fromRegion;
        private readonly string _fromCity;
        private readonly GettingType _gettingType;
        private readonly DeliveryMode _deliveryMode;
        private readonly TypeViewPoints _typeViewPoints;
        private readonly bool _showAddressComment;
        private readonly string _yaMapsApiKey;
        private readonly bool _statusesSync;

        private readonly PickPointApiService _pickPointApiService;

        public PickPointApiService PickPointApiService { get { return _pickPointApiService; } }
        public string Ikn { get { return _ikn; } }
        public string FromRegion { get { return _fromRegion; } }
        public string FromCity { get { return _fromCity; } }
        public GettingType GettingType { get { return _gettingType; } }
        public DeliveryMode DeliveryMode { get { return _deliveryMode; } }

        public const string KeyNameOrderPickPointInvoiceNumberInOrderAdditionalData = "OrderPickPointInvoiceNumber";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public PickPoint(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items) : base(method, preOrder, items)
        {
            _login = _method.Params.ElementOrDefault(PickPointTemplate.Login);
            _password = _method.Params.ElementOrDefault(PickPointTemplate.Password);
            _ikn = _method.Params.ElementOrDefault(PickPointTemplate.Ikn);
            _fromRegion = _method.Params.ElementOrDefault(PickPointTemplate.FromRegion);
            _fromCity = _method.Params.ElementOrDefault(PickPointTemplate.FromCity);
            _gettingType = (GettingType)_method.Params.ElementOrDefault(PickPointTemplate.GettingType).TryParseInt();
            _deliveryMode = (DeliveryMode)_method.Params.ElementOrDefault(PickPointTemplate.DeliveryMode).TryParseInt();
            _typeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(PickPointTemplate.TypeViewPoints).TryParseInt();
            _showAddressComment = method.Params.ElementOrDefault(PickPointTemplate.ShowAddressComment).TryParseBool();
            _yaMapsApiKey = _method.Params.ElementOrDefault(PickPointTemplate.YaMapsApiKey);
            _statusesSync = method.Params.ElementOrDefault(PickPointTemplate.StatusesSync).TryParseBool();

            var statusesReference = method.Params.ElementOrDefault(PickPointTemplate.StatusesReference);
            if (!string.IsNullOrEmpty(statusesReference))
            {
                string[] arr = null;
                _statusesReference =
                    statusesReference.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToDictionary(x => (arr = x.Split(','))[0].TryParseInt(),
                            x => arr.Length > 1 ? arr[1].TryParseInt(true) : null);
            }
            else
                _statusesReference = new Dictionary<int, int?>();

            _pickPointApiService = new PickPointApiService(_login, _password);
        }

        #endregion

        #region IShippingSupportingTheHistoryOfMovement

        public bool ActiveHistoryOfMovement => true;

        public List<HistoryOfMovement> GetHistoryOfMovement(Order order)
        {
            return null;
        }

        public PointInfo GetPointInfo(Order order)
        {
            if (order.OrderPickPoint == null || order.OrderPickPoint.PickPointId.IsNullOrEmpty())
                return null;

            var postamat = PostamatService.Get(order.OrderPickPoint.PickPointId);

            PointInfo pointInfo = null;

            if (postamat != null)
            {
                var descriptions = new List<string>();

                descriptions.Add($"{postamat.TypeTitle} {postamat.OwnerName}");

                if (postamat.Cash == (byte)CashType.Yes || postamat.Card == (byte)CardType.Yes)
                {
                    descriptions.Add(string.Format("Способы оплаты: {0}",
                        string.Join(", ", new[]
                        {
                                postamat.Cash == (byte)CashType.Yes ? "наличные" : null,
                                postamat.Card == (byte)CardType.Yes
                                    ? "банковские карты" + (postamat.PayPassAvailable
                                          ? " (возможность бесконтактной оплаты)"
                                          : null)
                                    : null
                        }.Where(x => x.IsNotEmpty()))));
                }
                if (postamat.AddressDescription.IsNotEmpty())
                    descriptions.Add(postamat.AddressDescription);

                pointInfo = new PointInfo()
                {
                    Address = postamat.Address,
                    TimeWork = postamat.WorkTimeSMS,
                    Comment = string.Join("<br>", descriptions)
                };
            }

            return pointInfo;
        }

        #endregion

        #region Statuses

        public bool StatusesSync
        {
            get => _statusesSync;
        }
        public bool SyncByAllOrders => true;

        private Dictionary<int, int?> _statusesReference;
        public Dictionary<int, int?> StatusesReference
        {
            get => _statusesReference;
        }

        public void SyncStatusOfOrder(Order order) => throw new NotImplementedException();

        public void SyncStatusOfOrders(IEnumerable<Order> orders)
        {
            var invoiceNumbers = new Dictionary<string, Order>();
            string invoiceNumber;
            foreach (var order in orders)
                if ((invoiceNumber = OrderService.GetOrderAdditionalData(order.OrderID,
                    KeyNameOrderPickPointInvoiceNumberInOrderAdditionalData)).IsNotEmpty())
                    invoiceNumbers.Add(invoiceNumber, order);

            var history = _pickPointApiService.GetInvoicesChangeState(
                dateFrom: DateTime.Now.Subtract(TimeSpan.FromHours(3)), // за последние 3 часа
                dateTo: DateTime.Now);

            if (history != null)
            {
                // Переворачиваем массив, чтобы последние записи были первыми
                // Будет влиять, когда в тоже время несколько раз произошла смена статуса (последний будет выше)
                history.Reverse();
                var processedInvoiceNumbers = new HashSet<string>();
                foreach (var state in history.OrderByDescending(x => x.ChangeDT))
                {
                    if (!processedInvoiceNumbers.Contains(state.InvoiceNumber))
                    {
                        if (invoiceNumbers.ContainsKey(state.InvoiceNumber))
                        {
                            var order = invoiceNumbers[state.InvoiceNumber];

                            var pickPointOrderStatus = state.VisualStateCode.HasValue && StatusesReference.ContainsKey(state.VisualStateCode.Value)
                                ? StatusesReference[state.VisualStateCode.Value]
                                : state.StateCode.HasValue && StatusesReference.ContainsKey(state.StateCode.Value)
                                    ? StatusesReference[state.StateCode.Value]
                                    : null;

                            if (pickPointOrderStatus.HasValue &&
                                order.OrderStatusId != pickPointOrderStatus.Value)
                            {
                                if (OrderStatusService.GetOrderStatus(pickPointOrderStatus.Value) != null)
                                {
                                    var lastOrderStatusHistory =
                                        OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                            .OrderByDescending(item => item.Date)
                                            .FirstOrDefault();

                                    if (lastOrderStatusHistory == null ||
                                        lastOrderStatusHistory.Date < state.ChangeDT)
                                    {
                                        OrderStatusService.ChangeOrderStatus(order.OrderID,
                                            pickPointOrderStatus.Value, "Синхронизация статусов для PickPoint");
                                    }
                                
                                }
                                processedInvoiceNumbers.Add(state.InvoiceNumber);
                            }

                        }
                    }
                }
            }
        }

        #endregion Statuses

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var city = _preOrder.CityDest;
            var region = _preOrder.RegionDest;
            var country = _preOrder.CountryDest;
            var countryRepo = Repository.CountryService.GetCountryByName(country);
            var countryIso3 = countryRepo != null ? countryRepo.Iso3 : null;

            var shippingOptions = new List<BaseShippingOption>();

            if (string.IsNullOrEmpty(city) || (string.IsNullOrEmpty(_fromCity) && string.IsNullOrEmpty(_fromRegion)))
                return shippingOptions;

            var weight = GetTotalWeight();
            var dimensions = GetDimensions(rate: 10);

            var points = GetPoints(countryIso3, country, region, city, weight, dimensions, _totalPrice);

            if (points.Count == 0)
                return shippingOptions;

            var calcParams = new CalcTariffParams
            {
                Depth = dimensions[0],
                Width = dimensions[1],
                Length = dimensions[2],
                Weight = weight,
                Ikn = _ikn,
                FromRegion = _fromRegion,
                FromCity = _fromCity,
                GettingType = _gettingType
            };

            string selectedPickpointId = null;
            if (_preOrder.ShippingOption != null &&
                _preOrder.ShippingOption.ShippingType ==
                ((ShippingKeyAttribute)typeof(PickPoint).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                if (_preOrder.ShippingOption.GetType() == typeof(PickPointOption) && ((PickPointOption)_preOrder.ShippingOption).SelectedPoint != null)
                    selectedPickpointId = ((PickPointOption)_preOrder.ShippingOption).SelectedPoint.Code;

                if (_preOrder.ShippingOption.GetType() == typeof(PickPointWidjetOption))
                    selectedPickpointId = ((PickPointWidjetOption)_preOrder.ShippingOption).PickpointId;

                if (_preOrder.ShippingOption.GetType() == typeof(PickPointDeliveryMapOption))
                    selectedPickpointId = ((PickPointDeliveryMapOption)_preOrder.ShippingOption).PickpointId;
            }

            if (selectedPickpointId.IsNullOrEmpty() || !points.Any(point => selectedPickpointId.Equals(point.Code, StringComparison.OrdinalIgnoreCase)))
                selectedPickpointId = points[0].Code;

            calcParams.PostamatNumber = selectedPickpointId;

            var delivertyCalc = _pickPointApiService.Calc(calcParams);

            if (delivertyCalc.ErrorCode == 0 && delivertyCalc.ErrorMessage.IsNullOrEmpty())
            {
                var typeService =
                    _deliveryMode == DeliveryMode.Priority
                        ? "Priority"
                        : "Standard";
                var rate = delivertyCalc.Services
                                        .Where(service => string.Equals(service.DeliveryMode, typeService, StringComparison.OrdinalIgnoreCase))
                                        .Sum(service => service.Tariff);
                if (rate > 0f)
                {
                    var deliveryMin = _deliveryMode == DeliveryMode.Priority
                        ? delivertyCalc.DPMinPriority
                        : delivertyCalc.DPMin;
                    var deliveryMax = _deliveryMode == DeliveryMode.Priority
                        ? delivertyCalc.DPMaxPriority
                        : delivertyCalc.DPMax;
                    var deliveryTime =
                        deliveryMax > 0 && deliveryMin > 0
                            ? (deliveryMax > deliveryMin
                                  ? (deliveryMin + _method.ExtraDeliveryTime) + "-"
                                  : string.Empty)
                              + (deliveryMax + _method.ExtraDeliveryTime) + " дн."
                            : null;

                    if (_typeViewPoints == TypeViewPoints.List ||
                        (_typeViewPoints == TypeViewPoints.YaWidget && _yaMapsApiKey.IsNullOrEmpty()))
                    {
                        var shippingOption = new PickPointOption(_method, _totalPrice)
                        {
                            Rate = rate,
                            DeliveryTime = deliveryTime,
                            ShippingPoints = points,
                            //SelectedPoint = points.FirstOrDefault(point => point.Code == selectedPickpointId)
                        };

                        shippingOption.IsAvailablePaymentCashOnDelivery = points.Any(
                            point => (point.Cash || point.Card) && point.Code == selectedPickpointId);

                        return new List<PickPointOption>() { shippingOption };
                    }
                    else
                    {
                        if (_typeViewPoints == TypeViewPoints.PickPointWidjet)
                        {
                            var shippingOption = new PickPointWidjetOption(_method, _totalPrice)
                            {
                                Rate = rate,
                                DeliveryTime = deliveryTime,
                                CurrentPoints = points,
                            };

                            shippingOption.WidgetConfigParams = ConfigWidget(shippingOption, city);
                            shippingOption.IsAvailablePaymentCashOnDelivery = points.Any(
                                point => (point.Cash || point.Card) && point.Code == selectedPickpointId);

                            return new List<PickPointWidjetOption>() { shippingOption };
                        }

                        var option = new PickPointDeliveryMapOption(_method, _totalPrice)
                        {
                            Rate = rate,
                            DeliveryTime = deliveryTime,
                            CurrentPoints = points,
                        };

                        SetMapData(option, countryIso3, country, region, city, weight, dimensions);
                        return new List<PickPointDeliveryMapOption>() { option };
                    }
                }
            }

            return shippingOptions;
        }

        private void SetMapData(PickPointDeliveryMapOption option, string countryIso3, string country, string region, string city, float weight, float[] dimensions)
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
                    { "countryIso", countryIso3 },
                    { "country", country },
                    { "region", region },
                    { "city", city },
                    { "totalPrice", _totalPrice.ToInvariantString() },
                    { "weight", weight },
                    { "dimensions0", dimensions[0] },
                    { "dimensions1", dimensions[1] },
                    { "dimensions2", dimensions[2] },
               };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        private Dictionary<string, object> ConfigWidget(PickPointWidjetOption shippingOption, string city)
        {
            var widgetConfigData = new Dictionary<string, object>();

            widgetConfigData.Add("city", city);
            //widgetConfigData.Add("cities", new[] {_preOrder.CityDest});
            widgetConfigData.Add("fromcity", _fromCity);
            widgetConfigData.Add("numbers", shippingOption.CurrentPoints.Select(point => point.Code).ToArray());
            widgetConfigData.Add("ikn", _ikn);

            return widgetConfigData;
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("city") || data["city"] == null
                || !data.ContainsKey("totalPrice") || data["totalPrice"] == null
                || !data.ContainsKey("weight") || !data.ContainsKey("dimensions0")
                || !data.ContainsKey("dimensions1") || !data.ContainsKey("dimensions2"))
                return null;

            var points = GetPoints(
                (string)data["countryIso"], 
                (string)data["country"],
                (string)data["region"],
                (string)data["city"],
                data["weight"].ToString().TryParseFloat(),
                new[] { 
                    data["dimensions0"].ToString().TryParseFloat(),
                    data["dimensions1"].ToString().TryParseFloat(),
                    data["dimensions2"].ToString().TryParseFloat()
                },
                data["totalPrice"].ToString().TryParseFloat());

            return GetFeatureCollection(points);
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<PickPointShippingPoint> points)
        {
            if (points == null)
                return null;

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
                            BalloonContentFooter = _showAddressComment
                                ? p.AddressComment
                                : null
                        }
                    }).ToList()
            };
        }

        private List<PickPointShippingPoint> GetPoints(string countryIso3, string country, string region, string city, float weight, float[] dimensions, float totalCost)
        {
            var listShippingPoints = new List<PickPointShippingPoint>();

            var postamats = PostamatService.Find(countryIso3, country, region, city, _ikn).AsEnumerable();
            if (postamats == null)
                return listShippingPoints;

            var dimensionSum = dimensions.Sum();

            postamats = postamats
                // с координатами
                .Where(x => x.Latitude > 0f && x.Longitude > 0f)
                // принимает такой вес посылки
                .Where(x => x.MaxWeight == null || weight <= x.MaxWeight)
                // принимает посылку таких габаритов
                .Where(x => x.DimensionSum == null || dimensionSum <= x.DimensionSum)
                .Where(x => x.MaxHeight == null || dimensions[2] <= x.MaxHeight)
                .Where(x => x.MaxWidth == null || dimensions[1] <= x.MaxWidth)
                .Where(x => x.MaxLength == null || dimensions[0] <= x.MaxLength)
                // может принять посылку такой стоимости
                .Where(x => x.AmountTo == null || totalCost <= x.AmountTo);

            if (_typeViewPoints == TypeViewPoints.List)
                postamats = postamats.OrderBy(postamat => postamat.Address);

            foreach (var postamat in postamats)
            {
                var point = new PickPointShippingPoint()
                {
                    Id = postamat.Id,
                    Code = postamat.Number,
                    Address = $"{postamat.Address} ({postamat.TypeTitle} {postamat.OwnerName})",
                    PointX = postamat.Longitude,
                    PointY = postamat.Latitude,
                    Cash = postamat.Cash == (byte)CashType.Yes,
                    Card = postamat.Card == (byte)CardType.Yes,
                    //PayPassAvailable = postamat.PayPassAvailable
                };

                if (_typeViewPoints == TypeViewPoints.List)
                {
                    var descriptions = new List<string>();
                    if (postamat.WorkTimeSMS.IsNotEmpty())
                        descriptions.Add($"Время работы: {postamat.WorkTimeSMS}");

                    if (postamat.Cash == (byte)CashType.Yes || postamat.Card == (byte)CardType.Yes)
                    {
                        descriptions.Add(string.Format("Способы оплаты: {0}",
                            string.Join(", ", new[]
                            {
                                postamat.Cash == (byte)CashType.Yes ? "наличные" : null,
                                postamat.Card == (byte)CardType.Yes
                                    ? "банковские карты" + 
                                        (postamat.PayPassAvailable
                                          ? " (возможность бесконтактной оплаты)"
                                          : null)
                                    : null
                            }.Where(x => x.IsNotEmpty()))));
                    }

                    if (postamat.AddressDescription.IsNotEmpty())
                        descriptions.Add(postamat.AddressDescription);

                    point.Description = string.Join("<br>", descriptions);
                }
                else if (_typeViewPoints == TypeViewPoints.YaWidget)
                {
                    var descriptions = new List<string>();

                    if (postamat.WorkTimeSMS.IsNotEmpty())
                        descriptions.Add($"Время работы: {postamat.WorkTimeSMS}");

                    if (postamat.Cash == (byte)CashType.Yes || postamat.Card == (byte)CardType.Yes)
                    {
                        descriptions.Add(string.Format("Способы оплаты: {0}",
                            string.Join(", ", new[]
                            {
                                postamat.Cash == (byte)CashType.Yes ? "наличные" : null,
                                postamat.Card == (byte)CardType.Yes
                                    ? "банковские карты" +
                                        (postamat.PayPassAvailable
                                          ? " (возможность бесконтактной оплаты)"
                                          : null)
                                    : null
                            }.Where(x => x.IsNotEmpty()))));
                    }

                    point.Description = string.Join("<br>", descriptions);

                    if (_showAddressComment)
                        point.AddressComment = postamat.AddressDescription;

                }

                listShippingPoints.Add(point);
            }

            return listShippingPoints;
        }

        #region IShippingWithBackgroundMaintenance

        public void ExecuteJob()
        {
            if (_login.IsNotEmpty() && _password.IsNotEmpty() && _ikn.IsNotEmpty())
            {
                SyncPostamats(_pickPointApiService, _ikn);
            }
        }

        public static void SyncPostamats(PickPointApiService apiClient, string ikn)
        {
            // общая настройка, т.к. справочники общие, не зависят от настроек
            var lattDateSync = Configuration.SettingProvider.Items["PickPointLastDateServiceSync"].TryParseDateTime(true);
            try
            {
                var currentDateTime = DateTime.UtcNow;

                if (!lattDateSync.HasValue || (currentDateTime - lattDateSync.Value.ToUniversalTime() > TimeSpan.FromHours(23)))
                {
                    // пишем в начале импорта, чтобы, если запустят в паралель еще
                    // то не прошло по условию времени последнего запуска
                    Configuration.SettingProvider.Items["PickPointLastDateServiceSync"] = currentDateTime.ToString("O");

                    PostamatService.Sync(apiClient, ikn);
                }
            }
            catch (Exception ex)
            {
                // возвращаем предыдущее заначение, чтобы при следующем запуске снова сработало
                Configuration.SettingProvider.Items["PickPointLastDateServiceSync"] = lattDateSync.HasValue ? lattDateSync.Value.ToString("O") : null;
                Diagnostics.Debug.Log.Warn(ex);
            }
        }

        #endregion
    }

    public enum TypeViewPoints
    {
        [Localize("Через Яндекс.Карты")]
        YaWidget = 0,

        [Localize("Через виджет PickPoint")]
        PickPointWidjet = 1,

        [Localize("Списком")]
        List = 2
    }
}
