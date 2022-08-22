//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Shipping.Sdek.Api;

namespace AdvantShop.Shipping.Sdek
{
    [ShippingKey("Sdek")]
    public class Sdek : BaseShippingWithCargo, IShippingSupportingSyncOfOrderStatus, IShippingLazyData, IShippingSupportingTheHistoryOfMovement, IShippingSupportingPaymentCashOnDelivery, IShippingWithBackgroundMaintenance
    {
        #region Ctor

        private readonly string _authLogin;
        private readonly string _authPassword;
        private readonly List<string> _tariffs;
        private readonly string _cityFrom;
        private readonly int? _cityFromId;
        private readonly int _deliveryNote;
        private readonly bool _statusesSync;
        private readonly bool _showPointsAsList;
        private readonly bool _allowInspection;
        private readonly bool _showSdekWidjet;
        private readonly bool _showAddressComment;
        private readonly string _yaMapsApiKey;
        private readonly bool _withInsure;

        private readonly SdekApiService20 _sdekApiService20;

        public const string KeyNameDispatchNumberInOrderAdditionalData = "SdekDispatchNumber";
        public const string KeyNameSdekOrderUuidInOrderAdditionalData = "SdekOrderUuid";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB", "UAH", "KZT", "BYN", "CNY" }; } }

        public Sdek(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items) : base(method, preOrder, items)
        {
            _authLogin = _method.Params.ElementOrDefault(SdekTemplate.AuthLogin);
            _authPassword = _method.Params.ElementOrDefault(SdekTemplate.AuthPassword);
            if (_method.Params.ContainsKey(SdekTemplate.CalculateTariffs))
                _tariffs = (_method.Params.ElementOrDefault(SdekTemplate.CalculateTariffs) ?? string.Empty).Split(",").ToList();
            else
                _tariffs = (_method.Params.ElementOrDefault(SdekTemplate.TariffOldParam) ?? string.Empty).Split(",").ToList();
            _cityFrom = _method.Params.ElementOrDefault(SdekTemplate.CityFrom);
            _cityFromId = _method.Params.ElementOrDefault(SdekTemplate.CityFromId).TryParseInt(true);
            _deliveryNote = _method.Params.ElementOrDefault(SdekTemplate.DeliveryNote).TryParseInt();
            _statusesSync = method.Params.ElementOrDefault(SdekTemplate.StatusesSync).TryParseBool();
            _allowInspection = method.Params.ElementOrDefault(SdekTemplate.AllowInspection).TryParseBool();
            _showPointsAsList = method.Params.ElementOrDefault(SdekTemplate.ShowPointsAsList).TryParseBool();
            _showSdekWidjet = method.Params.ElementOrDefault(SdekTemplate.ShowSdekWidjet).TryParseBool();
            _showAddressComment = method.Params.ElementOrDefault(SdekTemplate.ShowAddressComment).TryParseBool();
            _withInsure = method.Params.ElementOrDefault(SdekTemplate.WithInsure).TryParseBool();
            _yaMapsApiKey = _method.Params.ElementOrDefault(SdekTemplate.YaMapsApiKey);
            _sdekApiService20 = new SdekApiService20(_authLogin, _authPassword);
        }

        #endregion

        public string CityFrom => _cityFrom;
        public int? CityFromId => _cityFromId;
        public bool AllowInspection => _allowInspection;
        public bool WithInsure => _withInsure;
        public SdekApiService20 SdekApiService20 => _sdekApiService20;

        #region Statuses

        public void SyncStatusOfOrder(Order order)
        {
            var sdekOrderUuid = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameSdekOrderUuidInOrderAdditionalData);
            var sdekOrderNumber = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameDispatchNumberInOrderAdditionalData);

            GetOrderResult orderResult = null;
            if (sdekOrderUuid.IsNotEmpty())
                orderResult = _sdekApiService20.GetOrder(sdekOrderUuid.TryParseGuid(), null, null);
            if (orderResult == null && sdekOrderNumber.IsNotEmpty())
                orderResult = _sdekApiService20.GetOrder(null, sdekOrderNumber, null);

            if (orderResult?.Entity != null)
            {
                if (sdekOrderUuid.IsNullOrEmpty())
                    OrderService.AddUpdateOrderAdditionalData(
                        order.OrderID, 
                        KeyNameSdekOrderUuidInOrderAdditionalData,
                        orderResult.Entity.Uuid.ToString());
                
                if (sdekOrderNumber.IsNullOrEmpty() && orderResult.Entity.CdekNumber.IsNotEmpty())
                {
                    OrderService.AddUpdateOrderAdditionalData(
                        order.OrderID, 
                        KeyNameDispatchNumberInOrderAdditionalData,
                        orderResult.Entity.CdekNumber);
                    order.TrackNumber = orderResult.Entity.CdekNumber;
                    OrderService.UpdateOrderMain(order, changedBy: new OrderChangedBy("Синхронизация статусов для СДЭК"));
                }
                
                if (orderResult.Entity.Statuses != null && orderResult.Entity.Statuses.Count > 0)
                {
                    var lastStatus = orderResult.Entity.Statuses
                        .Where(x => 
                            StatusesReference.ContainsKey(x.Code) 
                            && StatusesReference[x.Code].HasValue)
                        .OrderByDescending(x => x.DateTime)
                        .FirstOrDefault();
                    
                    var sdekOrderStatus = lastStatus != null && StatusesReference.ContainsKey(lastStatus.Code)
                        ? StatusesReference[lastStatus.Code]
                        : null;

                    if (sdekOrderStatus.HasValue &&
                        order.OrderStatusId != sdekOrderStatus.Value &&
                        OrderStatusService.GetOrderStatus(sdekOrderStatus.Value) != null)
                    {
                        var lastOrderStatusHistory =
                            OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                .OrderByDescending(item => item.Date).FirstOrDefault();

                        if (lastOrderStatusHistory == null ||
                            lastOrderStatusHistory.Date < lastStatus.DateTime)
                        {
                            OrderStatusService.ChangeOrderStatus(order.OrderID,
                                sdekOrderStatus.Value, "Синхронизация статусов для СДЭК");
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
                if (_statusesReference == null)
                {
                    _statusesReference = new Dictionary<string, int?>
                    {
                        { "CREATED", _method.Params.ElementOrDefault(SdekTemplate.StatusCreated).TryParseInt(true)},
                        // { "", _method.Params.ElementOrDefault(SdekTemplate.StatusDeleted).TryParseInt(true)},
                        { "RECEIVED_AT_SHIPMENT_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtWarehouseOfSender).TryParseInt(true)},
                        { "READY_FOR_SHIPMENT_IN_SENDER_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForShipmentFromSenderWarehouse).TryParseInt(true)},
                        { "RETURNED_TO_SENDER_CITY_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToWarehouseOfSender).TryParseInt(true)},
                        { "TAKEN_BY_TRANSPORTER_FROM_SENDER_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusDeliveredToCarrierFromSenderWarehouse).TryParseInt(true)},
                        { "SENT_TO_TRANSIT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToTransitWarehouse).TryParseInt(true)},
                        { "ACCEPTED_IN_TRANSIT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtTransitWarehouse).TryParseInt(true)},
                        { "ACCEPTED_AT_TRANSIT_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtTransitWarehouse).TryParseInt(true)},
                        { "RETURNED_TO_TRANSIT_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToTransitWarehouse).TryParseInt(true)},
                        { "READY_FOR_SHIPMENT_IN_TRANSIT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForShipmentInTransitWarehouse).TryParseInt(true)},
                        { "TAKEN_BY_TRANSPORTER_FROM_TRANSIT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusDeliveredToCarrierInTransitWarehouse).TryParseInt(true)},
                        { "SENT_TO_SENDER_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToSenderCity).TryParseInt(true)},
                        { "SENT_TO_RECIPIENT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToWarehouseOfRecipient).TryParseInt(true)},
                        { "ACCEPTED_IN_SENDER_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtSenderCity).TryParseInt(true)},
                        { "ACCEPTED_IN_RECIPIENT_CITY", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtConsigneeWarehouse).TryParseInt(true)},
                        { "ACCEPTED_AT_RECIPIENT_CITY_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingDelivery).TryParseInt(true)},
                        { "ACCEPTED_AT_PICK_UP_POINT", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingFenceByClient).TryParseInt(true)},
                        { "TAKEN_BY_COURIER", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForDelivery).TryParseInt(true)},
                        { "RETURNED_TO_RECIPIENT_CITY_WAREHOUSE", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToConsigneeWarehouse).TryParseInt(true)},
                        { "DELIVERED", _method.Params.ElementOrDefault(SdekTemplate.StatusAwarded).TryParseInt(true)},
                        { "NOT_DELIVERED", _method.Params.ElementOrDefault(SdekTemplate.StatusNotAwarded).TryParseInt(true)},
                        // { "1", _method.Params.ElementOrDefault(SdekTemplate.StatusCreated).TryParseInt(true)},
                        // { "2", _method.Params.ElementOrDefault(SdekTemplate.StatusDeleted).TryParseInt(true)},
                        // { "3", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtWarehouseOfSender).TryParseInt(true)},
                        // { "6", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForShipmentFromSenderWarehouse).TryParseInt(true)},
                        // { "16", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToWarehouseOfSender).TryParseInt(true)},
                        // { "7", _method.Params.ElementOrDefault(SdekTemplate.StatusDeliveredToCarrierFromSenderWarehouse).TryParseInt(true)},
                        // { "21", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToTransitWarehouse).TryParseInt(true)},
                        // { "22", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtTransitWarehouse).TryParseInt(true)},
                        // { "13", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtTransitWarehouse).TryParseInt(true)},
                        // { "17", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToTransitWarehouse).TryParseInt(true)},
                        // { "19", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForShipmentInTransitWarehouse).TryParseInt(true)},
                        // { "20", _method.Params.ElementOrDefault(SdekTemplate.StatusDeliveredToCarrierInTransitWarehouse).TryParseInt(true)},
                        // { "27", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToSenderCity).TryParseInt(true)},
                        // { "8", _method.Params.ElementOrDefault(SdekTemplate.StatusSentToWarehouseOfRecipient).TryParseInt(true)},
                        // { "28", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtSenderCity).TryParseInt(true)},
                        // { "9", _method.Params.ElementOrDefault(SdekTemplate.StatusMetAtConsigneeWarehouse).TryParseInt(true)},
                        // { "10", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingDelivery).TryParseInt(true)},
                        // { "12", _method.Params.ElementOrDefault(SdekTemplate.StatusAcceptedAtConsigneeWarehouse_AwaitingFenceByClient).TryParseInt(true)},
                        // { "11", _method.Params.ElementOrDefault(SdekTemplate.StatusIssuedForDelivery).TryParseInt(true)},
                        // { "18", _method.Params.ElementOrDefault(SdekTemplate.StatusReturnedToConsigneeWarehouse).TryParseInt(true)},
                        // { "4", _method.Params.ElementOrDefault(SdekTemplate.StatusAwarded).TryParseInt(true)},
                        // { "5", _method.Params.ElementOrDefault(SdekTemplate.StatusNotAwarded).TryParseInt(true)},
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
            var sdekOrderUuid = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameSdekOrderUuidInOrderAdditionalData);
            var sdekOrderNumber = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameDispatchNumberInOrderAdditionalData);

            GetOrderResult orderResult = null;
            if (sdekOrderUuid.IsNotEmpty())
                orderResult = _sdekApiService20.GetOrder(sdekOrderUuid.TryParseGuid(), null, null);
            if (orderResult == null && sdekOrderNumber.IsNotEmpty())
                orderResult = _sdekApiService20.GetOrder(null, sdekOrderNumber, null);

            if (orderResult?.Entity?.Statuses != null)
            {
                return orderResult.Entity.Statuses
                    .OrderByDescending(x => x.DateTime)
                    .Select(stateInfo => new HistoryOfMovement()
                    {
                        Code = stateInfo.Code,
                        Name = stateInfo.Name,
                        Date = stateInfo.DateTime,
                        Comment = stateInfo.City
                    }).ToList();
            }

            return null;
        }

        public PointInfo GetPointInfo(Order order)
        {
            if (order.OrderPickPoint == null || order.OrderPickPoint.PickPointId.IsNullOrEmpty())
                return null;

            var cityToId = SdekService.GetSdekCityId(
                order.OrderCustomer.City, 
                order.OrderCustomer.District,
                order.OrderCustomer.Region,
                order.OrderCustomer.Country,
                service20: _sdekApiService20, 
                city: out _);

            if (!cityToId.HasValue)
                return null;
            
            var cityPoints = GetCityPoints(cityToId.Value);;

            var deliveryPoint = cityPoints.FirstOrDefault(x => order.OrderPickPoint.PickPointId == x.Code);

            return deliveryPoint != null
                ? new PointInfo()
                {
                    Phone = string.Join(", ", deliveryPoint.Phones.Select(x =>
                    {
                        var additional = x.Additional.IsNotEmpty() ? $"({x.Additional})" : null;
                        return $"{x.Number}{additional}";
                    })),
                    Address = deliveryPoint.Location.AddressFull,
                    TimeWork = deliveryPoint.WorkTime,
                    Comment = deliveryPoint.AddressComment
                }
                : null;
        }

        #endregion IShippingSupportingTheHistoryOfMovement

        #region IShippingWithBackgroundMaintenance

        public void ExecuteJob()
        {
            LoadSdekNumbers();
        }

        private void LoadSdekNumbers()
        {
            try
            {
                // получаем номер заказа сдэк для заказов, где его еще нет
                var list =
                    SQLDataAccess.ExecuteReadList(@"SELECT oad.* 
                    FROM [Order].[OrderAdditionalData] AS oad
                        INNER JOIN [Order].[Order] AS o ON o.[OrderID] = oad.[OrderID]
                        inner join [Order].[OrderStatus] os ON o.OrderStatusID = os.[OrderStatusID]
                    WHERE oad.[Name] = @KeyNameSdekOrderUuid AND o.OrderDate >= @MinOrderDate AND os.[IsCanceled] = 0 
                        AND os.[IsCompleted] = 0 AND o.[ShippingMethodID] = @ShippingMethodID 
                        AND NOT EXISTS(SELECT * FROM [Order].[OrderAdditionalData] AS oad2 WHERE oad2.[OrderID] = oad.[OrderID] AND oad2.[Name] = @KeyNameDispatchNumber)",
                        CommandType.Text,
                        reader => new Tuple<int, string>(SQLDataHelper.GetInt(reader, "OrderID"),
                            SQLDataHelper.GetString(reader, "Value")),
                        new SqlParameter("@ShippingMethodID", _method.ShippingMethodId),
                        new SqlParameter("@KeyNameSdekOrderUuid", KeyNameSdekOrderUuidInOrderAdditionalData),
                        new SqlParameter("@KeyNameDispatchNumber", KeyNameDispatchNumberInOrderAdditionalData),
                        new SqlParameter("@MinOrderDate", DateTime.Today.AddDays(-3)));

                foreach (var item in list)
                {
                    var orderId = item.Item1;
                    var sdekOrderUuid = item.Item2;

                    var order = OrderService.GetOrder(orderId);
                    if (order != null)
                    {
                        GetOrderResult orderResult = null;

                        if (sdekOrderUuid.IsNotEmpty())
                            orderResult = _sdekApiService20.GetOrder(sdekOrderUuid.TryParseGuid(), null, null);

                        if (orderResult?.Entity != null)
                        {
                            if (orderResult.Entity.CdekNumber.IsNotEmpty())
                            {
                                OrderService.AddUpdateOrderAdditionalData(
                                    order.OrderID,
                                    KeyNameDispatchNumberInOrderAdditionalData,
                                    orderResult.Entity.CdekNumber);
                                order.TrackNumber = orderResult.Entity.CdekNumber;
                                OrderService.UpdateOrderMain(order,
                                    changedBy: new OrderChangedBy("Получение трек-номеров СДЭК"));
                            }
                            else
                            {
                                var requestCreate = orderResult?.Requests?.FirstOrDefault(x => 
                                    string.Equals("CREATE", x.Type, StringComparison.OrdinalIgnoreCase));
                                if (requestCreate?.State.Equals("INVALID", StringComparison.OrdinalIgnoreCase) == true)
                                    OrderService.DeleteOrderAdditionalData(orderId, KeyNameSdekOrderUuidInOrderAdditionalData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
        }

        #endregion

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var options = new List<BaseShippingOption>();
            if (string.IsNullOrEmpty(_preOrder.CityDest) || string.IsNullOrEmpty(_cityFrom))
                return options;

            var sdekCityToId = SdekService.GetSdekCityId(_preOrder.CityDest, _preOrder.DistrictDest, _preOrder.RegionDest, _preOrder.CountryDest, service20: _sdekApiService20, city: out CityInfo cityTo);
            var sdekCityFromId = _cityFromId.HasValue ? _cityFromId.Value : SdekService.GetSdekCityId(_cityFrom, string.Empty, string.Empty, string.Empty, service20: _sdekApiService20, city: out _, allowedObsoleteFindCity: true);

            if (!sdekCityToId.HasValue || !sdekCityFromId.HasValue)
                return options;

            var calcTarifs = SdekTariffs.Tariffs.Where(item => _tariffs.Contains(item.TariffId.ToString())).ToList();
            // var dateExecute = DateTime.Now.Date.ToString("yyyy-MM-dd");
            var totalWeightInKg = GetTotalWeight();
            var totalWeightInG = (int)GetTotalWeight(1000);
            var dimensionsInSm = GetDimensions(rate:10);
            var pointsCity = calcTarifs.Any(x => x.Mode.EndsWith("-С") || x.Mode.EndsWith("-П")) 
                ? PreparePoint(sdekCityToId.Value, totalWeightInKg, dimensionsInSm)
                : null;
             var goods = PrepareGoods(dimensionsInSm, totalWeightInKg);

            var services = new List<ServiceParams>();
            // if (_withInsure)
            services.Add(new ServiceParams { Code = "INSURANCE", Parameter = ((int)Math.Ceiling(_totalPrice)).ToString() });

            foreach (var sdekTariff in calcTarifs)
            {
                if ((sdekTariff.MaxWeight.HasValue && sdekTariff.MaxWeight.Value < totalWeightInKg) ||
                    (sdekTariff.MinWeight.HasValue && sdekTariff.MinWeight.Value > totalWeightInKg))
                    continue;

                var typePoints = sdekTariff.Mode.EndsWith("-С")
                    ? "PVZ"
                    : sdekTariff.Mode.EndsWith("-П")
                        ? "POSTAMAT"
                        : null;

                var points = typePoints.IsNotEmpty()
                    ? pointsCity.Where(x => x.Type == typePoints).ToList()
                    : null;
                if (sdekTariff == null || ((sdekTariff.Mode.EndsWith("-С") || sdekTariff.Mode.EndsWith("-П")) && (points == null || points.Count == 0)))
                    continue;

                var calculateResult = _sdekApiService20.CalculatorTariff(new TariffParams()
                {
                    Type = 1,
                    TariffCode = sdekTariff.TariffId,
                    FromLocation = new TariffParamsLocation() {Code = sdekCityFromId.Value},
                    ToLocation = new TariffParamsLocation() {Code = sdekCityToId.Value},
                    Services = _allowInspection && typePoints != "POSTAMAT"
                        ? services.Concat(new[] {new ServiceParams {Code = "INSPECTION_CARGO"}}).ToList()
                        : services,
                    Packages = new List<TariffParamsPackage>()
                    {
                        new TariffParamsPackage()
                        {
                            Weight = totalWeightInG,
                            Length = (long)Math.Ceiling(dimensionsInSm[0]),
                            Width = (long)Math.Ceiling(dimensionsInSm[1]),
                            Height = (long)Math.Ceiling(dimensionsInSm[2]),
                        }
                    }
                });

                if (calculateResult.Errors == null)
                {
                    if (!string.Equals(
                        calculateResult.Currency, 
                        _method.ShippingCurrency?.Iso3, 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log.Warn(
                            $"Sdek: Валюты не совпадают. Указана {_method.ShippingCurrency?.Iso3}, " +
                            $"а возвращается {calculateResult.Currency}. Метод {_method.ShippingMethodId}.");
                        continue;
                    }
                    
                    string selectedPickpointId = null;
                    if (_preOrder.ShippingOption != null &&
                        _preOrder.ShippingOption.ShippingType ==
                        ((ShippingKeyAttribute)typeof(Sdek).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        if (_preOrder.ShippingOption.GetType() == typeof(SdekOption) && ((SdekOption)_preOrder.ShippingOption).SelectedPoint != null)
                            selectedPickpointId = ((SdekOption)_preOrder.ShippingOption).SelectedPoint.Code;

                        if (_preOrder.ShippingOption.GetType() == typeof(SdekWidjetOption))
                            selectedPickpointId = ((SdekWidjetOption)_preOrder.ShippingOption).PickpointId;

                        if (_preOrder.ShippingOption.GetType() == typeof(SdekPointDeliveryMapOption))
                            selectedPickpointId = ((SdekPointDeliveryMapOption)_preOrder.ShippingOption).PickpointId;
                    }

                    var insurePrice = calculateResult?.Services?.FirstOrDefault(x =>
                        string.Equals(x.Code, "INSURANCE", StringComparison.OrdinalIgnoreCase))?.Sum;
                    var basePrice = _withInsure
                        ? calculateResult.TotalSum
                        : calculateResult.TotalSum - (insurePrice ?? 0f);
                    var priceCash = calculateResult.TotalSum;

                    var tariffIsPvz = !sdekTariff.Mode.EndsWith("-Д");

                    if (!tariffIsPvz || _showPointsAsList || _yaMapsApiKey.IsNullOrEmpty())
                        options.Add(CreateOption(points, sdekTariff, calculateResult, selectedPickpointId, tariffIsPvz, 
                            cityTo, basePrice, priceCash, insurePrice));
                    else
                    {
                        if (_showSdekWidjet)
                            options.Add(CreateWidjetOption(points, goods, sdekTariff, calculateResult, selectedPickpointId, 
                                cityTo, basePrice, priceCash, insurePrice));
                        else
                            options.Add(CreatePointDeliveryMapOption(sdekCityToId, totalWeightInKg, dimensionsInSm, points, 
                                typePoints, sdekTariff, calculateResult, cityTo, basePrice, priceCash, insurePrice));
                    }
                }
            }

            return options;
        }

        private SdekPointDeliveryMapOption CreatePointDeliveryMapOption(int? sdekCityToId, float goodsWeight,
            float[] dimensionsInSm, List<SdekShippingPoint> points, string typePoints, SdekTariff sdekTariff, 
            TariffResult calculateResult, CityInfo cityTo, float basePrice, float priceCash, float? insurePrice)
        {
            var option = new SdekPointDeliveryMapOption(_method, _totalPrice)
            {
                Name = _method.Name + (sdekTariff.Mode.EndsWith("-С") ? " (пункты выдачи)" : " (постаматы)"),
                DeliveryId = sdekTariff.TariffId,
                Rate = basePrice,
                BasePrice = basePrice,
                PriceCash = priceCash,

                DeliveryTime = (calculateResult.PeriodMax > calculateResult.PeriodMin
                                   ? (calculateResult.PeriodMin + _method.ExtraDeliveryTime) + "-"
                                   : string.Empty)
                               + (calculateResult.PeriodMax + _method.ExtraDeliveryTime) + " дн.",

                CurrentPoints = points,
                CityTo = sdekCityToId.Value,
                IsAvailablePaymentCashOnDelivery = insurePrice.HasValue && (cityTo == null || !cityTo.PaymentLimit.HasValue || cityTo.PaymentLimit.Value == -1f || cityTo.PaymentLimit.Value >= _totalPrice),
                CalculateOption = new SdekCalculateOption
                {
                    TariffId = sdekTariff.TariffId, 
                    WithInsure = _withInsure, 
                    AllowInspection = _allowInspection
                }
            };

            //shippingOption.IsAvailableCashOnDelivery &= реализованно в SdekPointDeliveryMapOption

            SetMapData(option, goodsWeight, dimensionsInSm, typePoints);
            return option;
        }

        private SdekWidjetOption CreateWidjetOption(List<SdekShippingPoint> points, List<SdekGoods> goods,
            SdekTariff sdekTariff, TariffResult calculateResult, string selectedPickpointId, CityInfo cityTo, 
            float basePrice, float priceCash, float? insurePrice)
        {
            var shippingOption = new SdekWidjetOption(_method, _totalPrice)
            {
                Name = _method.Name + (sdekTariff.Mode.EndsWith("-С") ? " (пункты выдачи)" : " (постаматы)"),
                DeliveryId = sdekTariff.TariffId,
                Rate = basePrice,
                BasePrice = basePrice,
                PriceCash = priceCash,

                DeliveryTime = (calculateResult.PeriodMax > calculateResult.PeriodMin
                                   ? (calculateResult.PeriodMin + _method.ExtraDeliveryTime) + "-"
                                   : string.Empty)
                               + (calculateResult.PeriodMax + _method.ExtraDeliveryTime) + " дн.",

                TariffId = sdekTariff.TariffId.ToString(),
                WidgetConfigParams = ConfigWidget(goods),
                CurrentPoints = points.Select(point => (BaseShippingPoint)point).ToList(), // не меняю тип точек, чтобы после обновления у клиента не падал checkout 500 при конвертации типа из бд
                IsAvailablePaymentCashOnDelivery = insurePrice.HasValue && (cityTo == null || !cityTo.PaymentLimit.HasValue || cityTo.PaymentLimit.Value == -1f || cityTo.PaymentLimit.Value >= _totalPrice),
                CalculateOption = new SdekCalculateOption
                {
                    TariffId = sdekTariff.TariffId, 
                    WithInsure = _withInsure, 
                    AllowInspection = _allowInspection
                }
            };

            shippingOption.IsAvailablePaymentCashOnDelivery &= selectedPickpointId != null
                ? points.Any(
                    point => point.AllowedCod && point.Code == selectedPickpointId)
                : points.First().AllowedCod;
            return shippingOption;
        }

        private SdekOption CreateOption(List<SdekShippingPoint> points, SdekTariff sdekTariff, TariffResult calculateResult,
            string selectedPickpointId, bool tariffIsPvz, CityInfo cityTo, float basePrice, float priceCash, float? insurePrice)
        {
            var shippingOption = new SdekOption(_method, _totalPrice)
            {
                Name = _method.Name + (tariffIsPvz ? (sdekTariff.Mode.EndsWith("-С") ? " (пункты выдачи)" : " (постаматы)"): " (курьером)"),
                DeliveryId = sdekTariff.TariffId,
                Rate = basePrice,
                BasePrice = basePrice,
                PriceCash = priceCash,

                DeliveryTime = (calculateResult.PeriodMax > calculateResult.PeriodMin
                                   ? (calculateResult.PeriodMin + _method.ExtraDeliveryTime) + "-"
                                   : string.Empty)
                               + (calculateResult.PeriodMax + _method.ExtraDeliveryTime) + " дн.",

                ShippingPoints = tariffIsPvz ? points.Select(point => (BaseShippingPoint)point).ToList() : null, // не меняю тип точек, чтобы после обновления у клиента не падал checkout 500 при конвертации типа из бд
                TariffId = sdekTariff.TariffId.ToString(),
                //tariffIsPvz,
                HideAddressBlock = tariffIsPvz,
                IsAvailablePaymentCashOnDelivery = insurePrice.HasValue && (cityTo?.PaymentLimit == null || cityTo.PaymentLimit.Value == -1f || cityTo.PaymentLimit.Value >= _totalPrice),
                CalculateOption = new SdekCalculateOption
                {
                    TariffId = sdekTariff.TariffId, 
                    WithInsure = _withInsure, 
                    AllowInspection = _allowInspection
                }
            };

            if (tariffIsPvz)
                shippingOption.IsAvailablePaymentCashOnDelivery &= selectedPickpointId != null
                    ? points.Any(
                        point => point.AllowedCod && point.Code == selectedPickpointId)
                    : points[0].AllowedCod;
            return shippingOption;
        }

        private void SetMapData(SdekPointDeliveryMapOption option, float goodsWeight, float[] dimensionsInSm, string typePoints)
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
                    { "city", option.CityTo },
                    { "weight", goodsWeight.ToInvariantString() },
                    { "dimensions", string.Join("x", dimensionsInSm.Select(x => x.ToInvariantString())) },
                    { "typePoints", typePoints }
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("city") || data["city"] == null)
                return null;

            var city = data["city"].ToString().TryParseInt();
            var goodsWeight = data["weight"].ToString().TryParseFloat();
            var dimensionsInSm = data["dimensions"].ToString().Split('x').Select(x => x.TryParseFloat()).ToArray();
            var typePoints = (string)data["typePoints"];
            var points = typePoints.IsNotEmpty()
                ? PreparePoint(city, goodsWeight, dimensionsInSm).Where(x => x.Type.Equals(typePoints)).ToList()
                : null;

            return GetFeatureCollection(points ?? new List<SdekShippingPoint>());
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<SdekShippingPoint> points)
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
                            BalloonContentFooter = _showAddressComment
                                ? p.AddressComment
                                : null
                        }
                    }).ToList()
            };
        }

        private List<SdekGoods> PrepareGoods(float[] dimensions, float weight)
        {
            var goods = new List<SdekGoods>();
        
            goods.Add(new SdekGoods
            {
                Length = Math.Ceiling(dimensions[0]).ToString(CultureInfo.InvariantCulture),
                Width = Math.Ceiling(dimensions[1]).ToString(CultureInfo.InvariantCulture),
                Height = Math.Ceiling(dimensions[2]).ToString(CultureInfo.InvariantCulture),
                Weight = weight.ToInvariantString(),
            });
        
            return goods;
        }

        private List<DeliveryPoint> GetCityPoints(int sdekCityId)
        {
            return CacheManager.Get("Sdek_PreparePoint_" + sdekCityId, 60 * 24, () =>
            {
                var result = _sdekApiService20.GetDeliveryPoints(new DeliveryPointsFilter
                {
                    CityCode = sdekCityId,
                    Type = "ALL",
                    IsHandout = true
                });
                return result;
            });
        }

        private List<SdekShippingPoint> PreparePoint(int sdekCityToId, float goodsWeight, float[] dimensions)
        {
            var listShippingPoints = new List<SdekShippingPoint>();

            var sdekPvzList = GetCityPoints(sdekCityToId);

            foreach (var sdekPoint in sdekPvzList
                .Where(item =>
                    ((!item.WeightMax.HasValue || item.WeightMax >= goodsWeight) &&
                     (!item.WeightMin.HasValue || item.WeightMin < goodsWeight))
                    && (item.Dimensions == null || item.Dimensions.Any(d =>
                        dimensions[2] <= d.Height &&
                        dimensions[0] <= d.Depth &&
                        dimensions[1] <= d.Width))
                    )
                .OrderBy(x => x.Location.Address))
            {
                listShippingPoints.Add(new SdekShippingPoint
                {
                    Id = sdekPoint.Code.GetHashCode(),
                    Code = sdekPoint.Code,
                    Address = sdekPoint.Location.Address,
                    AddressComment = sdekPoint.AddressComment,
                    Description = string.Join("<br/>", new[] {
                        string.Join(", ", sdekPoint.Phones.Select(x =>
                        {
                            var additional = x.Additional.IsNotEmpty() ? $"({x.Additional})" : null;
                            return $"{x.Number}{additional}";
                        })),
                            sdekPoint.Note, 
                            sdekPoint.WorkTime}.Where(x => x.IsNotEmpty())) ,
                    PointX = sdekPoint.Location.Longitude,
                    PointY = sdekPoint.Location.Latitude,
                    AllowedCod = sdekPoint.AllowedCod,
                    Type = sdekPoint.Type
                });
            }

            return listShippingPoints;
        }

        private Dictionary<string, object> ConfigWidget(List<SdekGoods> goods)
        {
            var widgetConfigData = new Dictionary<string, object>();

            widgetConfigData.Add("defaultCity", _preOrder.CityDest);
            widgetConfigData.Add("cityFrom", _cityFrom);
            if (_preOrder.CountryDest.IsNotEmpty())
                widgetConfigData.Add("country", _preOrder.CountryDest);

            widgetConfigData.Add("goods", goods);
            widgetConfigData.Add("apikey", _yaMapsApiKey);

            return widgetConfigData;
        }

    }
}