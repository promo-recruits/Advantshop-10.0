//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping.Grastin;
using AdvantShop.Core.Services.Shipping.Grastin.Api;
using AdvantShop.Orders;
using AdvantShop.Repository;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Grastin
{
    [ShippingKey("Grastin")]
    public class Grastin : BaseShippingWithCargo, IShippingSupportingSyncOfOrderStatus, IShippingLazyData, IShippingSupportingPaymentCashOnDelivery, IShippingSupportingPaymentPickPoint
    {
        #region Ctor

        private readonly string _widgetFromCity;
        private readonly bool _widgetFromCityHide;
        private readonly bool _widgetFromCityNoChange;
        private readonly string _widgetHidePartnersShort;
        private readonly string _widgetHidePartnersFull;
        private readonly bool _widgetHideCost;
        private readonly bool _widgetHideDuration;
        private readonly string _widgetExtrachargeTypen;
        private readonly float _widgetExtracharge;
        private readonly int _widgetAddDuration;
        private readonly string _widgetHidePartnersJson;
        private readonly string _apiKey;
        private readonly string _orderPrefix;
        private readonly EnCourierService _typePaymentDelivery;
        private readonly EnCourierService _typePaymentPickup;
        private readonly EnTypeCalc _typeCalc;
        private readonly List<EnTypeDelivery> _activeDeliveryTypes;
        private readonly bool _insure;
        private readonly bool _statusesSync;
        private readonly string _moscowRegionId;
        private readonly string _saintPetersburgRegionId;
        private readonly string _nizhnyNovgorodRegionId;
        private readonly string _orelRegionId;
        private readonly string _krasnodarRegionId;
        private readonly string _boxberryRegionId;
        private readonly string _partnerRegionId;
        private readonly float _extracharge;
        private readonly int _increaseDeliveryTime;
        private readonly bool _excludeCostOrderprocessing;
        private readonly string _yaMapsApiKey;
        private readonly bool _showDrivingDescriptionPoint;

        private readonly GrastinApiService _grastinApiService;
        private readonly List<EnTypeContract> _activeContracts;
        private readonly Dictionary<EnTypeContract, string> _grastinRegionIds;

        public const string KeyNameIsSendOrderInOrderAdditionalData = "GrastinSendOrder";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public Grastin(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items) : base(method, preOrder, items)
        {
            _widgetFromCity = method.Params.ElementOrDefault(GrastinTemplate.WidgetFromCity);
            _widgetFromCityHide = method.Params.ElementOrDefault(GrastinTemplate.WidgetFromCityHide).TryParseBool();
            _widgetFromCityNoChange = method.Params.ElementOrDefault(GrastinTemplate.WidgetFromCityNoChange).TryParseBool();
            _widgetHidePartnersShort = method.Params.ElementOrDefault(GrastinTemplate.WidgetHidePartnersShort);
            _widgetHidePartnersFull = method.Params.ElementOrDefault(GrastinTemplate.WidgetHidePartnersFull);
            _widgetHideCost = method.Params.ElementOrDefault(GrastinTemplate.WidgetHideCost).TryParseBool();
            _widgetHideDuration = method.Params.ElementOrDefault(GrastinTemplate.WidgetHideDuration).TryParseBool();
            _widgetExtrachargeTypen = method.Params.ElementOrDefault(GrastinTemplate.WidgetExtrachargeTypen);
            _widgetExtracharge = method.Params.ElementOrDefault(GrastinTemplate.WidgetExtracharge).TryParseFloat();
            _widgetAddDuration = method.Params.ElementOrDefault(GrastinTemplate.WidgetAddDuration).TryParseInt();
            _widgetHidePartnersJson = method.Params.ElementOrDefault(GrastinTemplate.WidgetHidePartnersJson);
            _apiKey = method.Params.ElementOrDefault(GrastinTemplate.ApiKey);
            _orderPrefix = method.Params.ElementOrDefault(GrastinTemplate.OrderPrefix);
            _typePaymentDelivery = (EnCourierService)method.Params.ElementOrDefault(GrastinTemplate.TypePaymentDelivery).TryParseInt();
            _typePaymentPickup = (EnCourierService)method.Params.ElementOrDefault(GrastinTemplate.TypePaymentPickup).TryParseInt();
            _typeCalc = (EnTypeCalc)method.Params.ElementOrDefault(GrastinTemplate.TypeCalc).TryParseInt();
            _activeDeliveryTypes = (method.Params.ElementOrDefault(GrastinTemplate.ActiveDeliveryTypes) ?? string.Empty).Split(",").Select(x => x.TryParseInt()).Cast<EnTypeDelivery>().ToList();
            _insure = method.Params.ElementOrDefault(GrastinTemplate.Insure).TryParseBool();
            _statusesSync = method.Params.ElementOrDefault(GrastinTemplate.StatusesSync).TryParseBool();
            _moscowRegionId = method.Params.ElementOrDefault(GrastinTemplate.MoscowRegionId);
            _saintPetersburgRegionId = method.Params.ElementOrDefault(GrastinTemplate.SaintPetersburgRegionId);
            _nizhnyNovgorodRegionId = method.Params.ElementOrDefault(GrastinTemplate.NizhnyNovgorodRegionId);
            _orelRegionId = method.Params.ElementOrDefault(GrastinTemplate.OrelRegionId);
            _krasnodarRegionId = method.Params.ElementOrDefault(GrastinTemplate.KrasnodarRegionId);
            _boxberryRegionId = method.Params.ElementOrDefault(GrastinTemplate.BoxberryRegionId);
            _partnerRegionId = method.Params.ElementOrDefault(GrastinTemplate.PartnerRegionId);
            _extracharge = 0f;
            _increaseDeliveryTime = _method.ExtraDeliveryTime;
            _excludeCostOrderprocessing = method.Params.ElementOrDefault(GrastinTemplate.ExcludeCostOrderprocessing).TryParseBool();
            _yaMapsApiKey = _method.Params.ElementOrDefault(GrastinTemplate.YaMapsApiKey);
            _showDrivingDescriptionPoint = method.Params.ElementOrDefault(GrastinTemplate.ShowDrivingDescriptionPoint).TryParseBool();

            _activeContracts = new List<EnTypeContract>();
            if (!string.IsNullOrEmpty(_moscowRegionId))
                _activeContracts.Add(EnTypeContract.Moscow);
            if (!string.IsNullOrEmpty(_saintPetersburgRegionId))
                _activeContracts.Add(EnTypeContract.SaintPetersburg);
            if (!string.IsNullOrEmpty(_nizhnyNovgorodRegionId))
                _activeContracts.Add(EnTypeContract.NizhnyNovgorod);
            if (!string.IsNullOrEmpty(_orelRegionId))
                _activeContracts.Add(EnTypeContract.Orel);
            if (!string.IsNullOrEmpty(_boxberryRegionId))
                _activeContracts.Add(EnTypeContract.Boxberry);
            if (!string.IsNullOrEmpty(_partnerRegionId))
                _activeContracts.Add(EnTypeContract.Partner);
            if (!string.IsNullOrEmpty(_krasnodarRegionId))
                _activeContracts.Add(EnTypeContract.Krasnodar);

            _grastinApiService = new GrastinApiService(_apiKey);
            _grastinRegionIds = new Dictionary<EnTypeContract, string>
            {
                {EnTypeContract.Moscow, _moscowRegionId },
                {EnTypeContract.SaintPetersburg, _saintPetersburgRegionId },
                {EnTypeContract.NizhnyNovgorod, _nizhnyNovgorodRegionId },
                {EnTypeContract.Orel, _orelRegionId },
                {EnTypeContract.Krasnodar, _krasnodarRegionId },
                {EnTypeContract.Partner, _partnerRegionId },
            };
        }

        #endregion

        #region Statuses

        public void SyncStatusOfOrder(Order order) => throw new NotImplementedException();

        public bool SyncByAllOrders => true;
        public void SyncStatusOfOrders(IEnumerable<Order> orders)
        {
            var orderNumbers = new Dictionary<string, Order>(StringComparer.OrdinalIgnoreCase);
            foreach (var order in orders)
            {
                if (OrderService.GetOrderAdditionalData(order.OrderID, KeyNameIsSendOrderInOrderAdditionalData).IsNotEmpty())
                    orderNumbers.Add($"{OrderPrefix}{order.Number}", order);

                if (orderNumbers.Count >= 100)
                {
                    UpdateStatusOrders(orderNumbers);
                    orderNumbers.Clear();
                }
            }

            UpdateStatusOrders(orderNumbers);
        }

        private void UpdateStatusOrders(Dictionary<string, Order> orderNumbers)
        {
            if (orderNumbers.Count <= 0)
                return;

            var service = new GrastinApiService(ApiKey);
            var ordersInfo = service.GetOrderInfo(new OrderInfoContainer() {Orders = orderNumbers.Keys.ToList()});
            if (ordersInfo != null)
            {
                foreach (var orderInfo in ordersInfo)
                {
                    if (!orderNumbers.ContainsKey(orderInfo.Number))
                        continue;

                    var order = orderNumbers[orderInfo.Number];
                    var grastinOrderStatus = StatusesReference.ContainsKey(orderInfo.Status)
                        ? StatusesReference[orderInfo.Status]
                        : null;

                    if (grastinOrderStatus.HasValue &&
                        order.OrderStatusId != grastinOrderStatus.Value &&
                        OrderStatusService.GetOrderStatus(grastinOrderStatus.Value) != null)
                    {
                        var lastOrderStatusHistory =
                            OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                .OrderByDescending(item => item.Date).FirstOrDefault();

                        if (lastOrderStatusHistory == null ||
                            lastOrderStatusHistory.Date < orderInfo.StatusDateTime)
                        {
                            OrderStatusService.ChangeOrderStatus(order.OrderID,
                                grastinOrderStatus.Value, "Синхронизация статусов для Grastin");
                        }
                    }
                }
            }
        }

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
                    _statusesReference = new Dictionary<string, int?>
                    {
                        { "draft", _method.Params.ElementOrDefault(GrastinTemplate.StatusDraft).TryParseInt(true)},
                        { "new", _method.Params.ElementOrDefault(GrastinTemplate.StatusNew).TryParseInt(true)},
                        { "return", _method.Params.ElementOrDefault(GrastinTemplate.StatusReturn).TryParseInt(true)},
                        { "done", _method.Params.ElementOrDefault(GrastinTemplate.StatusDone).TryParseInt(true)},
                        { "shipping", _method.Params.ElementOrDefault(GrastinTemplate.StatusShipping).TryParseInt(true)},
                        { "received", _method.Params.ElementOrDefault(GrastinTemplate.StatusReceived).TryParseInt(true)},
                        { "canceled", _method.Params.ElementOrDefault(GrastinTemplate.StatusCanceled).TryParseInt(true)},
                        { "prepared for shipment", _method.Params.ElementOrDefault(GrastinTemplate.StatusPreparedForShipment).TryParseInt(true)},
                        { "problem", _method.Params.ElementOrDefault(GrastinTemplate.StatusProblem).TryParseInt(true)},
                        { "returned to customer", _method.Params.ElementOrDefault(GrastinTemplate.StatusReturnedToCustomer).TryParseInt(true)},
                        { "decommissioned", _method.Params.ElementOrDefault(GrastinTemplate.StatusDecommissioned).TryParseInt(true)},
                    };
                }
                return _statusesReference;
            }
        }

        #endregion

        #region Properties

        public string ApiKey
        {
            get { return _apiKey; }
        }

        public string OrderPrefix
        {
            get { return _orderPrefix; }
        }

        public string WidgetFromCity
        {
            get { return _widgetFromCity; }
        }

        public bool Insure
        {
            get { return _insure; }
        }

        public EnCourierService TypePaymentDelivery
        {
            get { return _typePaymentDelivery; }
        }

        public EnCourierService TypePaymentPickup
        {
            get { return _typePaymentPickup; }
        }

        private void DeactivateContract(EnTypeContract typeContract)
        {
            // Отключаем метод, чтобы эконопить кол-во запросов (ограничение 10к в день),
            // для большой нагрузки это очень мало.

            _activeContracts.Remove(typeContract);

            var nameParam = string.Empty;

            if (typeContract == EnTypeContract.Moscow)
                nameParam = GrastinTemplate.MoscowRegionId;

            if (typeContract == EnTypeContract.SaintPetersburg)
                nameParam = GrastinTemplate.SaintPetersburgRegionId;

            if (typeContract == EnTypeContract.NizhnyNovgorod)
                nameParam = GrastinTemplate.NizhnyNovgorodRegionId;

            if (typeContract == EnTypeContract.Orel)
                nameParam = GrastinTemplate.OrelRegionId;

            if (typeContract == EnTypeContract.Krasnodar)
                nameParam = GrastinTemplate.KrasnodarRegionId;

            if (typeContract == EnTypeContract.Partner)
                nameParam = GrastinTemplate.PartnerRegionId;

            if (typeContract == EnTypeContract.Boxberry)
                nameParam = GrastinTemplate.BoxberryRegionId;

            if (!string.IsNullOrEmpty(nameParam))
                ShippingMethodService.UpdateShippingParams(_method.ShippingMethodId, new Dictionary<string, string>()
                {
                    {
                        nameParam,
                        string.Empty
                    }
                });
        }

        #endregion

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var shippingOptions = new List<BaseShippingOption>();

            int weight = (int)GetTotalWeight(rate: 1000); // в граммах
            var dimensions = GetDimensions(rate: 10); // в см

            var orderCost = _totalPrice;

            if (_typeCalc == EnTypeCalc.Widget || _typeCalc == EnTypeCalc.ApiAndWidget)
            {
                if (!string.IsNullOrWhiteSpace(_preOrder.CityDest))
                {
                    // при весе больше 25 виджет падает
                    if (weight < 25000)
                    {
                        var widgetOption = new GrastinWidgetOption(_method, _totalPrice, updateRateAndTime: _typeCalc == EnTypeCalc.Widget)
                        {
                            WidgetConfigData = GetConfig(),
                            IsAvailableCashOnDelivery = true
                        };


                        if (_typeCalc == EnTypeCalc.ApiAndWidget)
                        {
                            var preorderOption = _preOrder.ShippingOption != null &&
                                                 _preOrder.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(Grastin).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value &&
                                                 _preOrder.ShippingOption.GetType() == typeof(GrastinWidgetOption)
                                ? ((GrastinWidgetOption)_preOrder.ShippingOption)
                                : null;

                            if (preorderOption != null && preorderOption.PickpointAdditionalDataObj != null)
                            {
                                switch (preorderOption.PickpointAdditionalDataObj.Partner)
                                {
                                    case EnPartner.Partner:
                                    case EnPartner.Grastin:
                                        UpdateGrastinRate(widgetOption, preorderOption, orderCost, weight, dimensions);
                                        break;

                                    case EnPartner.Boxberry:
                                        UpdateBoxberryRate(widgetOption, preorderOption, orderCost, weight);
                                        break;
                                }
                            }
                            else
                            {
                                SetFirstRate(widgetOption, orderCost, weight, dimensions);
                            }

                            if (widgetOption.Rate > 0f)
                                shippingOptions.Add(widgetOption);
                        }
                        else
                        {
                            shippingOptions.Add(widgetOption);
                        }
                    }
                }
            }
            else if (_typeCalc == EnTypeCalc.Api || _typeCalc == EnTypeCalc.ApiAndYaWidget)
            {
                if (!string.IsNullOrWhiteSpace(_preOrder.CityDest))
                {
                    var city = NormalizeCity(_preOrder.CityDest);
                    var typeContracts = GetTypeContractsGrastinByGeoData(city, _preOrder.RegionDest);

                    if (typeContracts.Count > 0)
                    {

                        if (typeContracts.Any(x => _activeContracts.Contains(x)))
                        {
                            if (_activeDeliveryTypes.Contains(EnTypeDelivery.Pickpoint) && (_typeCalc == EnTypeCalc.Api || _yaMapsApiKey.IsNullOrEmpty()))
                                shippingOptions.AddRange(GetShippingOptionsWithPoints(typeContracts, orderCost, weight, dimensions, city, _insure));

                            if (_activeDeliveryTypes.Contains(EnTypeDelivery.Courier))
                                shippingOptions.AddRange(GetShippingOptions(typeContracts, orderCost, weight, city, _preOrder.ZipDest, _insure));
                        }
                    }

                    if (_activeContracts.Contains(EnTypeContract.Boxberry))
                    {
                        if (_activeDeliveryTypes.Contains(EnTypeDelivery.Pickpoint) && (_typeCalc == EnTypeCalc.Api || _yaMapsApiKey.IsNullOrEmpty()))
                            shippingOptions.AddRange(GetShippingOptionsWithPoints(new List<EnTypeContract> { EnTypeContract.Boxberry }, orderCost, weight, dimensions, city, _insure));

                        if (_activeDeliveryTypes.Contains(EnTypeDelivery.Courier))
                            shippingOptions.AddRange(GetShippingOptions(new List<EnTypeContract> { EnTypeContract.Boxberry }, orderCost, weight, city, _preOrder.ZipDest, _insure));
                    }

                    if (_typeCalc == EnTypeCalc.ApiAndYaWidget && _yaMapsApiKey.IsNotEmpty() && _activeDeliveryTypes.Contains(EnTypeDelivery.Pickpoint))
                    {
                        var option = GrastinPointDeliveryMapOption(typeContracts, orderCost, weight, dimensions, city, _insure);
                        if (option != null)
                            shippingOptions.Add(option);
                    }
                }
            }

            return shippingOptions;
        }

        #region Calc Options By Api

        #region Without points

        private List<GrastinOption> GetShippingOptions(List<EnTypeContract> typeContracts, float orderCost, int weight, string cityDest, string zipDest, bool insure)
        {
            var list = new List<GrastinOption>();

            var grastinContracts = typeContracts
                .Where(x => _grastinRegionIds.ContainsKey(x) && _grastinRegionIds[x].IsNotEmpty())
                .Where(x => x != EnTypeContract.Partner)
                .ToList();
            if (grastinContracts.Count > 0)
            {
                if (grastinContracts.Count > 1)
                    throw new ArgumentException("в списке должен быть один элемент"); // не должно срабатывать (в списке должен быть один)

                list.AddRange(GetGrastinOptions(grastinContracts.First(), orderCost, weight, cityDest, insure));
            }
            else if (typeContracts.Contains(EnTypeContract.Boxberry))
            {
                list.AddRange(GetBoxberryOptions(orderCost, weight, cityDest, zipDest, insure));
            }

            return list;
        }

        private List<GrastinOption> GetBoxberryOptions(float orderCost, int weight, string cityDest, string zipDest, bool insure)
        {
            var list = new List<GrastinOption>();

            var deliveryCost = CalcDeliveryCostByCourierUsingBoxberry(orderCost, weight, cityDest, zipDest);

            if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
            {
                var rate = GetDeliverySum(deliveryCost, insure);
                list.Add(new GrastinOption(_method, _totalPrice)
                {
                    Name = _method.Name + " (Курьерская доставка Boxberry)",
                    Rate = rate,
                    BasePrice = rate,
                    PriceCash = GetDeliverySum(deliveryCost, insure, true),
                    PickpointAdditionalData = new GrastinEventWidgetData
                    {
                        DeliveryType = EnDeliveryType.Courier,
                        CityFrom = _widgetFromCity,
                        CityTo = cityDest,
                        Cost = rate,
                        Partner = EnPartner.Boxberry,
                        PickPointId = string.Empty
                    },
                });
            }

            return list;
        }

        private List<GrastinOption> GetGrastinOptions(EnTypeContract typeContract, float orderCost, int weight, string cityDest, bool insure)
        {
            var list = new List<GrastinOption>();

            var deliveryCost = CalcDeliveryCostByCourierUsingGrastin(typeContract, orderCost, weight, cityDest);

            if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
            {
                var rate = GetDeliverySum(deliveryCost, insure);
                list.Add(new GrastinOption(_method, _totalPrice)
                {
                    Name = _method.Name + " (Курьерская доставка Грастин)",
                    Rate = rate,
                    BasePrice = rate,
                    PriceCash = GetDeliverySum(deliveryCost, insure, true),
                    PickpointAdditionalData = new GrastinEventWidgetData
                    {
                        DeliveryType = EnDeliveryType.Courier,
                        CityFrom = _widgetFromCity,
                        CityTo = cityDest,
                        Cost = rate,
                        Partner = EnPartner.Grastin,
                        PickPointId = string.Empty
                    },
                });

            }

            return list;
        }

        #endregion

        #region With points

        private List<GrastinPointOption> GetShippingOptionsWithPoints(List<EnTypeContract> typeContracts, float orderCost, int weight, float[] dimensions, string cityDest, bool insure)
        {
            var list = new List<GrastinPointOption>();

            var preorderOption = _preOrder.ShippingOption != null &&
                _preOrder.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof (Grastin).GetCustomAttributes(typeof (ShippingKeyAttribute), false).First()).Value &&
                _preOrder.ShippingOption.GetType() == typeof (GrastinPointOption)
                    ? ((GrastinPointOption) _preOrder.ShippingOption)
                    : new GrastinPointOption();

            var grastinContracts = typeContracts
                .Where(x => _grastinRegionIds.ContainsKey(x) && _grastinRegionIds[x].IsNotEmpty())
                .ToList();
            if (grastinContracts.Count > 0)
            {
                list.AddRange(GetGrastinOptionsWithPoints(grastinContracts, orderCost, weight, dimensions, cityDest, preorderOption, insure));
            }
            else if (typeContracts.Contains(EnTypeContract.Boxberry))
            {
                list.AddRange(GetBoxberryOptionsWithPoints(orderCost, weight, cityDest, preorderOption, insure));
            }

            return list;
        }

        private List<GrastinPointOption> GetBoxberryOptionsWithPoints(float orderCost, int weight, string cityDest, GrastinPointOption preorderOption, bool insure)
        {
            var list = new List<GrastinPointOption>();

            var pickPointId = preorderOption != null && preorderOption.SelectedPoint != null
                ? preorderOption.SelectedPoint.Code
                : null;

            List<SelfpickupBoxberry> points;
            SelfpickupBoxberry selectedPoint;
            var deliveryCost = CalcDeliveryCostToPickPointUsingBoxberry(orderCost, weight, cityDest, pickPointId, out points, out selectedPoint);

            if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
            {
                list.Add(GetBoxberryOptionWithPoints(cityDest, deliveryCost, points, selectedPoint, insure));
            }

            return list;
        }

        private GrastinPointOption GetBoxberryOptionWithPoints(string cityDest, List<CostResponse> deliveryCost, List<SelfpickupBoxberry> points, SelfpickupBoxberry selectedPoint, bool insure)
        {
            var rate = GetDeliverySum(deliveryCost, insure);

            var shippingPoints = new List<GrastinPoint>();
            var selectedGrastinPoint = new GrastinPoint();

            foreach (var point in points)
            {
                var grastinPoint = new GrastinPoint()
                {
                    Id = point.Id.GetHashCode(),
                    Code = point.Id,
                    Address = point.Name,
                    Description = point.DrivingDescription,
                    Scheldule = point.Schedule,
                    DeliveryTime =
                        !string.IsNullOrWhiteSpace(point.DeliveryPeriod) 
                            ? string.Format("{0} д.", point.DeliveryPeriod.IsInt() 
                                ? (point.DeliveryPeriod.TryParseInt() + _increaseDeliveryTime).ToString()
                                : point.DeliveryPeriod) 
                            : null,
                    TypePoint = EnPartner.Boxberry,
                };

                if (selectedPoint == point)
                    selectedGrastinPoint = grastinPoint;

                shippingPoints.Add(grastinPoint);
            }

            var shippingOption = new GrastinPointOption(_method, _totalPrice)
            {
                Name = _method.Name + " (Самовывоз Boxberry)",
                Rate = rate,
                BasePrice = rate,
                PriceCash = GetDeliverySum(deliveryCost, insure, true),
                DeliveryTime = selectedGrastinPoint.DeliveryTime,
                PickpointAdditionalData = new GrastinEventWidgetData
                {
                    DeliveryType = EnDeliveryType.PickPoint,
                    CityFrom = _widgetFromCity,
                    CityTo = cityDest,
                    Cost = rate,
                    Partner = EnPartner.Boxberry,
                    PickPointId = selectedGrastinPoint.Code
                },
                ShippingPoints = shippingPoints,
                SelectedPoint = selectedGrastinPoint
            };

            return shippingOption;
        }

        private List<GrastinPointOption> GetGrastinOptionsWithPoints(List<EnTypeContract> typeContracts, float orderCost, int weight, float[] dimensions, string cityDest, GrastinPointOption preorderOption, bool insure)
        {
            var list = new List<GrastinPointOption>();

            var pickPointId = preorderOption != null && preorderOption.SelectedPoint != null
                ? preorderOption.SelectedPoint.Code
                : null;

            List<ISelfpickupGrastin> points;
            ISelfpickupGrastin selectedPoint;
            var deliveryCost = CalcDeliveryCostToPickPointUsingGrastin(typeContracts, orderCost, weight, dimensions, cityDest, pickPointId, out points, out selectedPoint);

            if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
            {
                list.Add(GetGrastinOptionWithPoints(deliveryCost, cityDest, preorderOption, points, selectedPoint, insure));
            }

            return list;
        }

        private GrastinPointOption GetGrastinOptionWithPoints(List<CostResponse> deliveryCost, string cityDest, GrastinPointOption preorderOption, List<ISelfpickupGrastin> points, ISelfpickupGrastin selectedPoint, bool insure)
        {
            var rate = GetDeliverySum(deliveryCost, insure);

            var shippingPoints = new List<GrastinPoint>();
            var selectedGrastinPoint = new GrastinPoint();

            var activePartner = _activeContracts.Contains(EnTypeContract.Partner);
            var activeGrastin = _activeContracts.Contains(EnTypeContract.Moscow) ||
                                _activeContracts.Contains(EnTypeContract.SaintPetersburg) ||
                                _activeContracts.Contains(EnTypeContract.NizhnyNovgorod) ||
                                _activeContracts.Contains(EnTypeContract.Orel) ||
                                _activeContracts.Contains(EnTypeContract.Krasnodar);

            foreach (var point in points)
            {
                if (point.TypePoint == EnPartner.Partner && !activePartner)
                    continue;
                if (point.TypePoint == EnPartner.Grastin && !activeGrastin)
                    continue;

                var grastinPoint = new GrastinPoint()
                {
                    Id = point.Id.GetHashCode(),
                    Code = point.Id,
                    Address = point.ToString(),
                    Description = point.DrivingDescription,
                    Phone = point.Phone,
                    Scheldule = point.TimeTable,
                    LinkDriving = point.LinkDrivingDescription,
                    TypePoint = point.TypePoint
                };

                if (selectedPoint == point)
                    selectedGrastinPoint = grastinPoint;

                shippingPoints.Add(grastinPoint);
            }

            var shippingOption = new GrastinPointOption(_method, _totalPrice)
            {
                Name = _method.Name + " (Самовывоз Грастин)",
                Rate = rate,
                BasePrice = rate,
                PriceCash = GetDeliverySum(deliveryCost, insure, true),
                PickpointAdditionalData = new GrastinEventWidgetData
                {
                    DeliveryType = EnDeliveryType.PickPoint,
                    CityFrom = _widgetFromCity,
                    CityTo = cityDest,
                    Cost = rate,
                    Partner = selectedPoint != null ? selectedPoint.TypePoint : EnPartner.Grastin,
                    PickPointId = selectedGrastinPoint.Code
                },
                ShippingPoints = shippingPoints,
                SelectedPoint = selectedGrastinPoint
            };
            return shippingOption;
        }

        #endregion

        #endregion

        #region Calc Options For Widget

        private void SetFirstRate(GrastinWidgetOption widgetOption, float orderCost, int weight, float[] dimensions)
        {
            var isCalcRate = false;

            var city = _preOrder.CityDest;
            city = NormalizeCity(city);
            var typeContracts = GetTypeContractsGrastinByGeoData(city, _preOrder.RegionDest);
            if (typeContracts.Any(x => _activeContracts.Contains(x)))
            {
                List<CostResponse> deliveryCost = null;
                ISelfpickupGrastin selectedPoint;
                List<ISelfpickupGrastin> points;

                var grastinContracts = typeContracts
                    .Where(x => _grastinRegionIds.ContainsKey(x) && _grastinRegionIds[x].IsNotEmpty())
                    .ToList();

                if (grastinContracts.Count > 0)
                    deliveryCost = CalcDeliveryCostToPickPointUsingGrastin(grastinContracts,
                        orderCost, weight, dimensions, city, null, out points, out selectedPoint);

                if (deliveryCost == null || deliveryCost.Count == 0 || deliveryCost[0].Status != "Ok" || deliveryCost[0].ShippingCost <= 0)
                {
                    grastinContracts = grastinContracts
                        .Where(x => x != EnTypeContract.Partner)
                        .ToList();

                    if (grastinContracts.Count > 1)
                        throw new ArgumentException("в списке должен быть один элемент"); // не должно срабатывать (в списке должен быть один)

                    if (grastinContracts.Count > 0)
                        deliveryCost = CalcDeliveryCostByCourierUsingGrastin(grastinContracts.First(), orderCost, weight, city);
                }

                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                    isCalcRate = true;
                }
            }

            if (!isCalcRate && _activeContracts.Contains(EnTypeContract.Boxberry))
            {
                List<CostResponse> deliveryCost = null;
                SelfpickupBoxberry selectedPoint = null;
                List<SelfpickupBoxberry> points;

                deliveryCost = CalcDeliveryCostToPickPointUsingBoxberry(orderCost, weight, city,
                    null, out points, out selectedPoint);

                if (deliveryCost == null || deliveryCost.Count == 0 || deliveryCost[0].Status != "Ok" || deliveryCost[0].ShippingCost <= 0)
                    deliveryCost = CalcDeliveryCostByCourierUsingBoxberry(orderCost, weight, city, _preOrder.ZipDest);


                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                    if (selectedPoint != null)
                    {
                        //widgetOption.IsAvailableCashOnDelivery = selectedPoint.FullPrePayment;
                        widgetOption.DeliveryTime =
                            !string.IsNullOrWhiteSpace(selectedPoint.DeliveryPeriod)
                                ? string.Format("{0} д.", selectedPoint.DeliveryPeriod.IsInt()
                                    ? (selectedPoint.DeliveryPeriod.TryParseInt() +
                                       _increaseDeliveryTime).ToString()
                                    : selectedPoint.DeliveryPeriod)
                                : null;
                    }
                    isCalcRate = true;
                }
            }
        }

        private void UpdateGrastinRate(GrastinWidgetOption widgetOption, GrastinWidgetOption preorderOption, float orderCost,
            int weight, float[] dimensions)
        {
            var city = _preOrder.CityDest;
            city = NormalizeCity(city);
            var typeContracts = GetTypeContractsGrastinByGeoData(city, _preOrder.RegionDest);
            if (typeContracts.Any(x => _activeContracts.Contains(x)))
            {
                List<CostResponse> deliveryCost = null;
                ISelfpickupGrastin selectedPoint;
                List<ISelfpickupGrastin> points;

                var grastinContracts = typeContracts
                    .Where(x => _grastinRegionIds.ContainsKey(x) && _grastinRegionIds[x].IsNotEmpty())
                    .ToList();

                if (grastinContracts.Count > 0 && preorderOption.PickpointAdditionalDataObj.DeliveryType ==
                    EnDeliveryType.PickPoint)
                    deliveryCost = CalcDeliveryCostToPickPointUsingGrastin(grastinContracts,
                        orderCost, weight, dimensions, city, preorderOption.PickpointId, out points,
                        out selectedPoint);

                if (preorderOption.PickpointAdditionalDataObj.DeliveryType ==
                    EnDeliveryType.Courier)
                {
                    grastinContracts = grastinContracts
                        .Where(x => x != EnTypeContract.Partner)
                        .ToList();

                    if (grastinContracts.Count > 1)
                        throw new ArgumentException("в списке должен быть один элемент"); // не должно срабатывать (в списке должен быть один)

                    if (grastinContracts.Count > 0)
                        deliveryCost = CalcDeliveryCostByCourierUsingGrastin(grastinContracts.First(), orderCost, weight, city);
                }

                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                }
            }
        }

        private void UpdateBoxberryRate(GrastinWidgetOption widgetOption, GrastinWidgetOption preorderOption, float orderCost,
            int weight)
        {
            if (_activeContracts.Contains(EnTypeContract.Boxberry))
            {
                List<CostResponse> deliveryCost = null;
                SelfpickupBoxberry selectedPoint = null;
                List<SelfpickupBoxberry> points;
                var city = preorderOption.PickpointAdditionalDataObj.CityTo.IsNotEmpty()
                    ? preorderOption.PickpointAdditionalDataObj.CityTo
                    : _preOrder.CityDest;

                city = NormalizeCity(city);

                if (preorderOption.PickpointAdditionalDataObj.DeliveryType ==
                    EnDeliveryType.PickPoint)
                    deliveryCost = CalcDeliveryCostToPickPointUsingBoxberry(orderCost, weight, city,
                        preorderOption.PickpointId, out points, out selectedPoint);

                if (preorderOption.PickpointAdditionalDataObj.DeliveryType ==
                    EnDeliveryType.Courier)
                    deliveryCost =
                        CalcDeliveryCostByCourierUsingBoxberry(orderCost, weight, city, _preOrder.ZipDest);


                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    var rate = GetDeliverySum(deliveryCost, _insure);
                    widgetOption.Rate = rate;
                    widgetOption.BasePrice = rate;
                    widgetOption.PriceCash = GetDeliverySum(deliveryCost, _insure, true);
                    if (selectedPoint != null)
                    {
                        //widgetOption.IsAvailableCashOnDelivery = selectedPoint.FullPrePayment;
                        widgetOption.DeliveryTime =
                            !string.IsNullOrWhiteSpace(selectedPoint.DeliveryPeriod)
                                ? string.Format("{0} д.", selectedPoint.DeliveryPeriod.IsInt()
                                    ? (selectedPoint.DeliveryPeriod.TryParseInt() +
                                       _increaseDeliveryTime).ToString()
                                    : selectedPoint.DeliveryPeriod)
                                : null;
                    }
                }
            }
        }

        private Dictionary<string, string> GetConfig()
        {
            var _widgetConfigData = new Dictionary<string, string>();

            _widgetConfigData.Add("data-no-weight", "1");

            if (!string.IsNullOrEmpty(_widgetFromCity))
                _widgetConfigData.Add("data-from-city", _widgetFromCity);

            if (_widgetFromCityHide)
                _widgetConfigData.Add("data-from-hide", "1");

            //if (_widgetFromCityNoChange)
            _widgetConfigData.Add("data-from-single", "1");

            if (_preOrder.CityDest.IsNotEmpty())
            {
                _widgetConfigData.Add("data-to-city", _preOrder.CityDest);
                _widgetConfigData.Add("data-to-hide", "1");
            }

            if (_typeCalc == EnTypeCalc.ApiAndWidget)
            {
                var hidePartners = new List<string>() { "hermespikup", "dpdpikup", "post", "postpackageonline", "postcourieronline" };
                if (!_activeContracts.Contains(EnTypeContract.Moscow) && 
                    !_activeContracts.Contains(EnTypeContract.SaintPetersburg) && 
                    !_activeContracts.Contains(EnTypeContract.NizhnyNovgorod) &&
                    !_activeContracts.Contains(EnTypeContract.Orel) && 
                    !_activeContracts.Contains(EnTypeContract.Krasnodar))
                    hidePartners.AddRange(new []{ "grastinpikup", "grastincourier"});
                else
                {
                    var typeContract = GetTypeContractsGrastinByGeoData(_preOrder.CityDest, _preOrder.RegionDest, doNotCheckPartner: true);//вернет пустой или с одним элементом список
                    if (!typeContract.All(x => _activeContracts.Contains(x)))
                        hidePartners.AddRange(new[] { "grastinpikup", "grastincourier" });
                }
                if (!_activeContracts.Contains(EnTypeContract.Partner))
                    hidePartners.AddRange(new[] { "partnerpikup" });
                if (!_activeContracts.Contains(EnTypeContract.Boxberry))
                    hidePartners.AddRange(new[] { "boxberrypikup", "boxberrycourier" });

                if (_widgetHidePartnersShort.IsNotEmpty())
                    hidePartners.AddRange(
                        _widgetHidePartnersShort
                            .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                            .Where(x => !hidePartners.Contains(x, StringComparer.OrdinalIgnoreCase)));

                _widgetConfigData.Add("data-no-partners", string.Join(",", hidePartners));
            }
            else
            {
                if (!string.IsNullOrEmpty(_widgetHidePartnersFull))
                    _widgetConfigData.Add("data-no-partners", _widgetHidePartnersFull);
            }

            if (_widgetHideCost)
                _widgetConfigData.Add("data-no-cost", "1");

            if (_widgetHideDuration)
                _widgetConfigData.Add("data-no-duration", "1");

            if (_widgetExtracharge > 0f)
                _widgetConfigData.Add("data-add-cost", string.Format("{0}{1}", _widgetExtracharge.ToString(CultureInfo.InvariantCulture), _widgetExtrachargeTypen));
            //if (_method.Extracharge > 0f)
            //{
            //    if (_method.ExtrachargeType == Payment.ExtrachargeType.Percent)
            //    {
            //        if (_method.ExtrachargeFromOrder)
            //            _widgetConfigData.Add("data-add-cost", Math.Round(_method.Extracharge * _totalPrice / 100, 2).ToInvariantString() + "Руб");
            //        else
            //            _widgetConfigData.Add("data-add-cost", _method.Extracharge.ToInvariantString() + "%");
            //    }
            //    else
            //        _widgetConfigData.Add("data-add-cost", _method.Extracharge.ToInvariantString() + "Руб");
            //}

            //if (_widgetAddDuration > 0f)
            //    _widgetConfigData.Add("data-add-duration", _widgetAddDuration.ToString());
            if (_increaseDeliveryTime > 0f)
                _widgetConfigData.Add("data-add-duration", _increaseDeliveryTime.ToString());

            var weight = GetTotalWeight();
            if (weight > 0f)
                _widgetConfigData.Add("data-weight-base", Math.Ceiling(weight).ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrEmpty(_widgetHidePartnersJson))
                _widgetConfigData.Add("data-no-partners-obj", Uri.EscapeDataString(_widgetHidePartnersJson));

            return _widgetConfigData;
        }

        #region ApiAndYaWidget

        private GrastinPointDeliveryMapOption GrastinPointDeliveryMapOption(List<EnTypeContract> typeContracts, float orderCost, int weight, float[] dimensions, string cityDest, bool insure)
        {
            var preorderOption = _preOrder.ShippingOption != null &&
                                 _preOrder.ShippingOption.GetType() == typeof(GrastinPointDeliveryMapOption) &&
                                 _preOrder.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(Grastin).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value
                ? ((GrastinPointDeliveryMapOption)_preOrder.ShippingOption)
                : null;

            float? rate = null, priceCash = null;
            string deliveryTime = null;

            var pickPointId = preorderOption != null && preorderOption.PickpointAdditionalDataObj != null
                ? preorderOption.PickpointAdditionalDataObj.PickPointId
                : null;
            EnPartner typePoint = preorderOption != null && preorderOption.PickpointAdditionalDataObj != null
                ? preorderOption.PickpointAdditionalDataObj.Partner
                : EnPartner.None;

            List<ISelfpickupGrastin> grastinPoints = null;
            List<SelfpickupBoxberry> boxberryPoints = null;

            var grastinContracts = typeContracts
                .Where(x => _grastinRegionIds.ContainsKey(x) && _grastinRegionIds[x].IsNotEmpty())
                .ToList();
            if (grastinContracts.Count > 0)
            {
                ISelfpickupGrastin selectedPointGrastin;
                var deliveryCost = CalcDeliveryCostToPickPointUsingGrastin(grastinContracts, orderCost, weight, dimensions, cityDest, pickPointId, out grastinPoints, out selectedPointGrastin);

                if ((typePoint == EnPartner.None || 
                        ((typePoint == EnPartner.Grastin || typePoint == EnPartner.Partner) &&
                        selectedPointGrastin.Id == pickPointId)) &&
                    deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    rate = GetDeliverySum(deliveryCost, insure);
                    priceCash = GetDeliverySum(deliveryCost, insure, true);
                }
            }

            if (_activeContracts.Contains(EnTypeContract.Boxberry))
            {
                SelfpickupBoxberry selectedPointBoxberry;
                var deliveryCost = CalcDeliveryCostToPickPointUsingBoxberry(orderCost, weight, cityDest, pickPointId, out boxberryPoints, out selectedPointBoxberry);

                if (((typePoint == EnPartner.None && rate == null) || (typePoint == EnPartner.Boxberry && selectedPointBoxberry.Id == pickPointId)) &&
                    deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Status == "Ok" && deliveryCost[0].ShippingCost > 0)
                {
                    rate = GetDeliverySum(deliveryCost, insure);
                    priceCash = GetDeliverySum(deliveryCost, insure, true);
                    deliveryTime = !string.IsNullOrWhiteSpace(selectedPointBoxberry.DeliveryPeriod)
                                        ? string.Format("{0} д.", selectedPointBoxberry.DeliveryPeriod.IsInt()
                                            ? (selectedPointBoxberry.DeliveryPeriod.TryParseInt() + _increaseDeliveryTime).ToString()
                                            : selectedPointBoxberry.DeliveryPeriod)
                                        : null;
                }
            }

            if ((grastinPoints != null && grastinPoints.Count > 0) || (boxberryPoints != null && boxberryPoints.Count > 0))
            {
                var option = new GrastinPointDeliveryMapOption(_method, _totalPrice)
                {
                    Name = _method.Name + " (Самовывоз)",
                    Rate = rate ?? 0f,
                    BasePrice = rate ?? 0f,
                    PriceCash = priceCash ?? 0f,
                    DeliveryTime = deliveryTime,
                    GrastinPoints = grastinPoints,
                    BoxberryPoints = boxberryPoints,
                    CityFrom = _widgetFromCity,
                    CityTo = cityDest,
                    IsAvailableCashOnDelivery = true,
                    ShowDrivingDescriptionPoint = _showDrivingDescriptionPoint
                };

                SetMapData(option, weight, dimensions);

                return option;
            }

            return null;
        }

        private void SetMapData(GrastinPointDeliveryMapOption option, int weight, float[] dimensions)
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
            option.MapParams.Destination = string.Join(", ", new[] { _preOrder.CountryDest, _preOrder.RegionDest, _preOrder.CityDest }.Where(x => x.IsNotEmpty()));

            option.PointParams = new PointDelivery.PointParams();
            option.PointParams.IsLazyPoints = (option.GrastinPoints != null ? option.GrastinPoints.Count : 0) + (option.BoxberryPoints != null ? option.BoxberryPoints.Count : 0) > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "city", option.CityTo },
                    { "region", _preOrder.RegionDest },
                    { "grastinPoints", option.GrastinPoints != null }, // null - значит не активен или неактуально для данного города
                    { "boxberryPoints", option.BoxberryPoints != null },// null - значит не активен
                    { "weight", weight },
                    { "dimensions0", dimensions[0] },
                    { "dimensions1", dimensions[1] },
                    { "dimensions2", dimensions[2] },
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.GrastinPoints, option.BoxberryPoints);
            }
        }

        private PointDelivery.FeatureCollection GetFeatureCollection(List<ISelfpickupGrastin> GrastinPoints, List<SelfpickupBoxberry> BoxberryPoints)
        {
            var featureCollection = new PointDelivery.FeatureCollection() { Features = new List<PointDelivery.Feature>() };

            if (GrastinPoints != null)
            {
                featureCollection.Features.AddRange(GrastinPoints.Select(p =>
                        new PointDelivery.Feature
                        {
                            Id = p.Id.GetHashCode(),
                            Geometry = new PointDelivery.PointGeometry { PointX = p.PointX, PointY = p.PointY },
                            Options = new PointDelivery.PointOptions { Preset = "islands#dotIcon" },
                            Properties = new PointDelivery.PointProperties
                            {
                                BalloonContentHeader = p.ToString(),
                                HintContent = p.ToString(),
                                BalloonContentBody =
                                    string.Format("{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{3}#{4}')\">Выбрать</a>",
                                                    string.Join("<br>", new[] 
                                                        {
                                                            p.TimeTable,
                                                            p.Metrostation.IsNotEmpty() ? p.Metrostation : null,
                                                            p.AcceptsPaymentCards ? "Возможна оплата банковской картой" : null,
                                                            p is Selfpickup && ((Selfpickup)p).DressingRoom ? "Есть примерочная комната" : null,
                                                            p.Phone,
                                                        }.Where(x => !string.IsNullOrEmpty(x))),
                                                    "<br>",
                                                    p.Id.GetHashCode(),
                                                    p.TypePoint,
                                                    p.Id),
                                BalloonContentFooter = _showDrivingDescriptionPoint 
                                    ? string.Join("<br>", new[]
                                        {
                                            p.DrivingDescription,
                                            p.LinkDrivingDescription.IsNotEmpty() ? string.Format("<a target=\"_blank\" href=\"{0}\">Показать схему</a>", p.LinkDrivingDescription) : null
                                        }.Where(x => !string.IsNullOrEmpty(x)))
                                    : null
                            }
                        }));
            }

            if (BoxberryPoints != null)
            {
                featureCollection.Features.AddRange(BoxberryPoints.Select(p =>
                        new PointDelivery.Feature
                        {
                            Id = p.Id.GetHashCode(),
                            Geometry = new PointDelivery.PointGeometry { PointX = p.PointX, PointY = p.PointY },
                            Options = new PointDelivery.PointOptions { Preset = "islands#dotIcon" },
                            Properties = new PointDelivery.PointProperties
                            {
                                BalloonContentHeader = p.Name,
                                HintContent = p.Name,
                                BalloonContentBody =
                                    string.Format("{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{3}#{4}')\">Выбрать</a>",
                                                    string.Join("<br>", new[] 
                                                        {
                                                            p.Schedule,
                                                            p.Acquiring ? "Возможна оплата банковской картой" : null,
                                                        }.Where(x => !string.IsNullOrEmpty(x))),
                                                    "<br>",
                                                    p.Id.GetHashCode(),
                                                    EnPartner.Boxberry,
                                                    p.Id),
                                BalloonContentFooter = _showDrivingDescriptionPoint
                                    ? string.Join("<br>", new[]
                                        {
                                            p.DrivingDescription,
                                        }.Where(x => !string.IsNullOrEmpty(x)))
                                    :null
                            }
                        }));
            }

            return featureCollection;
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("city") || data["city"] == null || !data.ContainsKey("region") || data["region"] == null
                || !data.ContainsKey("weight") || !data.ContainsKey("dimensions0") || 
                !data.ContainsKey("dimensions1") || !data.ContainsKey("dimensions2"))
                return null;

            var city = (string)data["city"];
            var region = (string)data["region"];
            var weight = data["weight"].ToString().TryParseInt();
            var dimensions = new float[]
            {
                data["dimensions0"].ToString().TryParseFloat(),
                data["dimensions1"].ToString().TryParseFloat(),
                data["dimensions2"].ToString().TryParseFloat(),
            };
            var grastinPoints = data.ContainsKey("grastinPoints") && data["grastinPoints"] != null && (bool)data["grastinPoints"]
                ? GetPointsGrastin(city, weight, dimensions, GetTypeContractsGrastinByGeoData(city, region))
                : null;
            var boxberryPoints = data.ContainsKey("boxberryPoints") && data["boxberryPoints"] != null && (bool)data["boxberryPoints"]
                ? GetPointsBoxberry(city)
                : null;

            return GetFeatureCollection(grastinPoints, boxberryPoints);
        }

        #endregion

        #endregion

        #region Help methods

        private List<CostResponse> CalcShipingCost(string regionId, float orderSum, float assessedCost, int weight, bool isSelfPickup, string pickupId = null, string postcodeId = null, bool isRegional = false)
        {
            var deliveryCost = _grastinApiService.CalcShipingCost(new CalcShipingCostContainer()
            {
                Orders = new List<CalcShipingCostOrder>()
                {
                    new CalcShipingCostOrder()
                    {
                        Number = "123",
                        RegionId = regionId,
                        Weight = weight,
                        OrderSum = orderSum,
                        AssessedCost = assessedCost,
                        IsSelfPickup = isSelfPickup,
                        IsRegional = isRegional,
                        PickupId = pickupId, // уточнеие от поддержки: Idpickup используется только для региона Боксберри
                        PostcodeId = postcodeId
                    }
                }
            });
            return deliveryCost;
        }

        private float GetDeliverySum(List<CostResponse> deliveryCost, bool withInsurance, bool cachOnDelivery = false)
        {
            var rate =
                deliveryCost[0].ShippingCost +
                deliveryCost[0].ShippingCostDistance +
                (cachOnDelivery ? deliveryCost[0].Commission : 0f) +
                (withInsurance ? deliveryCost[0].SafetyStock : 0f) +
                deliveryCost[0].AdditionalShippingCosts +
                (_excludeCostOrderprocessing ? 0f : deliveryCost[0].OrderProcessing) +
                _extracharge;
            return rate;
        }

        private List<EnTypeContract> GetTypeContractsGrastinByGeoData(string cityDesc, string regionDest, bool doNotCheckPartner = false)
        {
            var result = new List<EnTypeContract>();

            var isMoscow = cityDesc.Equals("москва", StringComparison.OrdinalIgnoreCase) ||
                           (!string.IsNullOrWhiteSpace(regionDest) &&
                            (regionDest.Equals("москва", StringComparison.OrdinalIgnoreCase) ||
                             regionDest.Equals("московская область", StringComparison.OrdinalIgnoreCase)));

            var isSaintPetersburg =
                cityDesc.Equals("санкт-петербург", StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(regionDest) &&
                 (regionDest.Equals("санкт-петербург", StringComparison.OrdinalIgnoreCase)));

            var isNizhnyNovgorod = cityDesc.Equals("нижний новгород", StringComparison.OrdinalIgnoreCase);
            var isOrel = cityDesc.Equals("орёл", StringComparison.OrdinalIgnoreCase) ||
                         cityDesc.Equals("орел", StringComparison.OrdinalIgnoreCase);
            var isKrasnodar = cityDesc.Equals("краснодар", StringComparison.OrdinalIgnoreCase);

            if (isMoscow || isSaintPetersburg || isNizhnyNovgorod || isOrel || isKrasnodar)
            {
                result.Add(isMoscow
                    ? EnTypeContract.Moscow
                    : isSaintPetersburg
                        ? EnTypeContract.SaintPetersburg
                        : isNizhnyNovgorod
                            ? EnTypeContract.NizhnyNovgorod
                            : isOrel
                                ? EnTypeContract.Orel
                                : EnTypeContract.Krasnodar);
            }

            // Партнерские ПВЗ также могут быть в Питере, Москве и др.
            if (!doNotCheckPartner && _activeContracts.Contains(EnTypeContract.Partner))
            {
                var city = cityDesc;
                city = NormalizeCity(city);

                if (_grastinApiService.GetPartnerSelfPickups()
                    .Any(x => !string.IsNullOrWhiteSpace(x.City) &&
                              NormalizeCity(x.City).Equals(city, StringComparison.OrdinalIgnoreCase)))
                    result.Add(EnTypeContract.Partner);
            }

            return result;
        }

        private List<CostResponse> CalcDeliveryCostToPickPointUsingGrastin(List<EnTypeContract> typeContracts, float orderCost,
            int weight, float[] dimensions, string cityDest, string pickPointId, out List<ISelfpickupGrastin> points, out ISelfpickupGrastin selectedPoint)
        {
            points = GetPointsGrastin(cityDest, weight, dimensions, typeContracts);

            if (points.Count > 0)
            {
                selectedPoint = pickPointId.IsNotEmpty()
                    ? points.FirstOrDefault(x => x.Id == pickPointId) ?? points[0]
                    : points[0];

                var typeContract = selectedPoint.TypePoint == EnPartner.Partner
                    ? typeContracts.Select(x => (EnTypeContract?)x).FirstOrDefault(x => x == EnTypeContract.Partner)
                    : typeContracts.Select(x => (EnTypeContract?)x).FirstOrDefault(x => x != EnTypeContract.Partner);

                if (typeContract.HasValue && _grastinRegionIds.ContainsKey(typeContract.Value) &&
                    !string.IsNullOrEmpty(_grastinRegionIds[typeContract.Value]))
                {
                    var deliveryCost = CalcShipingCost(_grastinRegionIds[typeContract.Value], orderCost, orderCost, weight, true, selectedPoint.Id, isRegional: selectedPoint.RegionalPoint);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(typeContract.Value);

                    return deliveryCost;
                }
            }

            selectedPoint = null;
            return null;
        }

        private List<ISelfpickupGrastin> GetPointsGrastin(string cityDest, int weight, float[] dimensions, List<EnTypeContract> typeContracts)
        {
            var moscowCities = new List<string>()
            {
                "Москва", "Балашиха", "Видное", "Бутово", "Долгопрудный", "Ивантеевка", "Королев", "Люберцы", "Мытищи", "Одинцово", "Реутов", "Фрязино", "Химки", "Щелково", "Ново-Переделкино" //"Чехов", "Подольск", "Домодедово", "Зеленоград"
            };

            var isMoscow = cityDest.Equals("москва", StringComparison.OrdinalIgnoreCase);

            if (isMoscow)
            {
                var moscowReg = RegionService.GetRegionByName("Москва");
                if(moscowReg != null)
                    foreach (var city in CityService.GetCitiesByRegion(moscowReg.RegionId))
                    {
                        moscowCities.Add(NormalizeCity(city.Name));
                    }
            }

            var points = new List<ISelfpickupGrastin>();
            if ((_activeContracts.Contains(EnTypeContract.Moscow) && typeContracts.Contains(EnTypeContract.Moscow)) ||
                (_activeContracts.Contains(EnTypeContract.SaintPetersburg) && typeContracts.Contains(EnTypeContract.SaintPetersburg)) ||
                (_activeContracts.Contains(EnTypeContract.NizhnyNovgorod) && typeContracts.Contains(EnTypeContract.NizhnyNovgorod)) ||
                (_activeContracts.Contains(EnTypeContract.Orel) && typeContracts.Contains(EnTypeContract.Orel)) ||
                (_activeContracts.Contains(EnTypeContract.Krasnodar) && typeContracts.Contains(EnTypeContract.Krasnodar)))
            {
                if (isMoscow)
                {
                    points.AddRange(_grastinApiService.GetGrastinSelfPickups()
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(x.City) &&
                            moscowCities.Contains(NormalizeCity(x.City), StringComparer.OrdinalIgnoreCase))
                        .Where(x => x.IssuesLargeSize || (weight < 25000 && dimensions[0] < 190 && dimensions[1] < 50 && dimensions[2] < 70))
                        .Where(x => !x.IssuesOnlyLargeSize || (weight >= 25000 || dimensions[0] >= 190 || dimensions[1] >= 50 || dimensions[2] >= 70))
                        .OrderBy(x => x.Name)
                    );
                }
                else
                {
                    points.AddRange(_grastinApiService.GetGrastinSelfPickups()
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(x.City) &&
                            NormalizeCity(x.City).Equals(cityDest, StringComparison.OrdinalIgnoreCase))
                        .Where(x => x.IssuesLargeSize || (weight < 25000 && dimensions[0] < 190 && dimensions[1] < 50 && dimensions[2] < 70))
                        .Where(x => !x.IssuesOnlyLargeSize || (weight >= 25000 || dimensions[0] >= 190 || dimensions[1] >= 50 || dimensions[2] >= 70))
                        .OrderBy(x => x.Name)
                    );
                }
            }

            if (_activeContracts.Contains(EnTypeContract.Partner) && typeContracts.Contains(EnTypeContract.Partner))
            {
                if(isMoscow)
                {
                    points.AddRange(_grastinApiService.GetPartnerSelfPickups()
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(x.City) &&
                            moscowCities.Contains(NormalizeCity(x.City), StringComparer.OrdinalIgnoreCase))
                        .OrderBy(x => x.Address));
                }
                else
                {
                    points.AddRange(_grastinApiService.GetPartnerSelfPickups(cityDest)
                        //.Where(x =>
                        //    !string.IsNullOrWhiteSpace(x.City) &&
                        //    x.City.Equals(cityDest, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(x => x.Address));
                }
            }

            return points;
        }

        private List<CostResponse> CalcDeliveryCostByCourierUsingGrastin(EnTypeContract typeContract, float orderCost, int weight, 
            string cityDest)
        {
            if (_grastinRegionIds.ContainsKey(typeContract) && 
                !string.IsNullOrEmpty(_grastinRegionIds[typeContract]))
            {
                var regionId = 
                    typeContract == EnTypeContract.Moscow && !IsMoscow() // расчет в московскую область
                    ? cityDest // вместо id контракта передаем название города
                    : _grastinRegionIds[typeContract];
                var deliveryCost =
                    CalcShipingCost(regionId, orderCost, orderCost, weight, false);

                if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                    if (typeContract != EnTypeContract.Moscow || IsMoscow()) // не деактивируем для МО 
                        DeactivateContract(typeContract);

                return deliveryCost;
            }

            return null;
        }

        private List<CostResponse> CalcDeliveryCostToPickPointUsingBoxberry(float orderCost, int weight,
            string cityDest, string pickPointId, out List<SelfpickupBoxberry> points, out SelfpickupBoxberry selectedPoint)
        {
            if (!string.IsNullOrEmpty(_boxberryRegionId))
            {
                points = GetPointsBoxberry(cityDest);

                if (points.Count > 0)
                {
                    selectedPoint = pickPointId.IsNotEmpty()
                        ? points.FirstOrDefault(x => x.Id == pickPointId) ?? points[0]
                        : points[0];

                    var deliveryCost = CalcShipingCost(_boxberryRegionId, orderCost, orderCost, weight, true, selectedPoint.Id);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(EnTypeContract.Boxberry);

                    return deliveryCost;
                }
            }

            points = null;
            selectedPoint = null;

            return null;
        }

        private List<SelfpickupBoxberry> GetPointsBoxberry(string cityDest)
        {
            return _grastinApiService.GetBoxberrySelfPickup(cityDest)
                .OrderBy(x => x.Name)
                .ToList();
        }

        private List<CostResponse> CalcDeliveryCostByCourierUsingBoxberry(float orderCost, int weight,
            string cityDest, string zipDest)
        {
            if (!string.IsNullOrEmpty(_boxberryRegionId))
            {
                var postCodes = _grastinApiService.GetBoxberryPostCode(cityDest);

                if (postCodes != null && postCodes.Count > 0)
                {
                    var selectedPostCode = postCodes.FirstOrDefault(x => x.Name.StartsWith(zipDest ?? string.Empty)) ?? postCodes[0];

                    var deliveryCost = CalcShipingCost(_boxberryRegionId, orderCost, orderCost, weight, false, null, selectedPostCode.Id);

                    if (deliveryCost != null && deliveryCost.Count > 0 && deliveryCost[0].Error == "Contract for the delivery region not found")
                        DeactivateContract(EnTypeContract.Boxberry);

                    return deliveryCost;
                }
            }

            return null;
        }

        private bool IsMoscow()
        {
            // нельзя расчитать доставку курьером за Москву
            return !string.IsNullOrWhiteSpace(_preOrder.CityDest) && _preOrder.CityDest.Equals("москва", StringComparison.OrdinalIgnoreCase);
        }

        private string NormalizeCity(string city)
        {
            if (city.Equals("орёл", StringComparison.OrdinalIgnoreCase))
                return "Орел";
            //if (city.Contains("ё") || city.Contains("Ё"))
            //    city = city.Replace('ё', 'е').Replace('Ё', 'Е');
            return city;
        }

        #endregion
    }

    public enum EnTypeCalc
    {
        /// <summary>
        /// Через Api
        /// </summary>
        [Localize("Через Api")]
        Api = 0,

        /// <summary>
        /// Через Api с выбором через виджет
        /// </summary>
        [Localize("Через Api с выбором через виджет")]
        ApiAndWidget = 1,

        /// <summary>
        /// Через виджет
        /// </summary>
        [Localize("Через виджет, со всеми типами доставки (не рекомендуется)")]
        Widget = 2,

        /// <summary>
        /// Через Api с выбором через наш виджет
        /// </summary>
        [Localize("Через Api с выбором через Яндекс.Карты")]
        ApiAndYaWidget = 3,
    }

    public enum EnTypeContract
    {
        /// <summary>
        /// Грастин Москва
        /// </summary>
        [Localize("Грастин Москва")]
        Moscow = 0,

        /// <summary>
        /// Грастин Санкт-Петербург
        /// </summary>
        [Localize("Грастин Санкт-Петербург")]
        SaintPetersburg = 1,

        /// <summary>
        /// Грастин Нижний Новгород
        /// </summary>
        [Localize("Грастин Нижний Новгород")]
        NizhnyNovgorod = 2,

        /// <summary>
        /// Грастин Орёл
        /// </summary>
        [Localize("Грастин Орёл")]
        Orel = 3,

        /// <summary>
        /// Грастин Краснодар
        /// </summary>
        [Localize("Грастин Краснодар")]
        Krasnodar = 5,

        /// <summary>
        /// Boxberry
        /// </summary>
        [Localize("Партнерские ПВЗ")]
        Partner = 4,

        /// <summary>
        /// Boxberry
        /// </summary>
        [Localize("Boxberry")]
        Boxberry = 30,

    }

    public enum EnTypeDelivery
    {
        /// <summary>
        /// Самовывоз
        /// </summary>
        [Localize("Самовывоз")]
        Pickpoint = 0,

        /// <summary>
        /// Курьер
        /// </summary>
        [Localize("Курьер")]
        Courier = 1,
    }

    [JsonConverter(typeof(GrastinEnumConverter))]
    public enum EnPartner
    {
        None = 0,
        Grastin = 1,
        Hermes = 2,
        RussianPost = 3,
        Boxberry = 4,
        DPD = 5,
        Partner = 6,
    }
}
