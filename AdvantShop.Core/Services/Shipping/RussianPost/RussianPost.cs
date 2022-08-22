using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping.RussianPost.TrackingApi;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping.RussianPost.Api;
using AdvantShop.Shipping.RussianPost.PickPoints;

namespace AdvantShop.Shipping.RussianPost
{
    [ShippingKey("RussianPost")]
    public class RussianPost : BaseShippingWithCargo, IShippingSupportingSyncOfOrderStatus, IShippingSupportingTheHistoryOfMovement, IShippingLazyData, IShippingSupportingPaymentCashOnDelivery, IShippingWithBackgroundMaintenance
    {
        #region Ctor

        private readonly string _login;
        private readonly string _password;
        private readonly string _token;
        private readonly string _pointIndex;
        private readonly List<EnMailType> _deliveryTypesOld;
        private readonly List<Tuple<EnMailType, EnMailCategory, EnTransportType>> _localDeliveryTypes;
        private readonly List<Tuple<EnMailType, EnMailCategory, EnTransportType>> _internationalDeliveryTypes;
        private readonly bool _courier;
        private readonly bool _fragile;
        private readonly EnTypeNotification? _typeNotification;
        private readonly bool _smsNotification;
        private readonly int _increaseDeliveryTime;
        private readonly string _yaMapsApiKey;
        private readonly bool _deliveryWithCod;
        private readonly bool _deliveryToOps;

        private readonly bool _statusesSync;
        private readonly string _trackingLogin;
        private readonly string _trackingPassword;

        private readonly RussianPostApiService _russianPostApiService;
        private readonly RussianPostTrackingApiService _russianPostTrackingApiService;

        public const string KeyNameOrderRussianPostIdInOrderAdditionalData = "OrderRussianPostId";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public RussianPost(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items) : base(method, preOrder, items)
        {
            _login = _method.Params.ElementOrDefault(RussianPostTemplate.Login);
            _password = _method.Params.ElementOrDefault(RussianPostTemplate.Password);
            _token = _method.Params.ElementOrDefault(RussianPostTemplate.Token);
            _russianPostApiService = new RussianPostApiService(_login, _password, _token);
            _pointIndex = _method.Params.ElementOrDefault(RussianPostTemplate.PointIndex);

            _localDeliveryTypes = new List<Tuple<EnMailType, EnMailCategory, EnTransportType>>();
            _internationalDeliveryTypes = new List<Tuple<EnMailType, EnMailCategory, EnTransportType>>();

            _deliveryTypesOld =
                method.Params.ContainsKey(RussianPostTemplate.DeliveryTypes)
                    ? (method.Params.ElementOrDefault(RussianPostTemplate.DeliveryTypes) ?? string.Empty)
                        .Split(",")
                        .Select(x => GetEnMailTypeByInt(x.TryParseInt()))
                        .ToList()
                    : null;
            var typeInsure = (EnTypeInsure)method.Params.ElementOrDefault(RussianPostTemplate.TypeInsure).TryParseInt();
            var mailCategories = EnMailCategory.AsList();

            if (method.Params.ContainsKey(RussianPostTemplate.LocalDeliveryTypes))
            {
                string[] tempArr;
                foreach (var strDelivery in (method.Params.ElementOrDefault(RussianPostTemplate.LocalDeliveryTypes) ?? string.Empty).Split(","))
                {
                    if ((tempArr = strDelivery.Split("\\")).Length >= 2)
                    {
                        var mailType = EnMailType.Parse(tempArr[0]);
                        if (tempArr.Length > 2 || !RussianPostAvailableOption.LocalTransportTypeAvailable.ContainsKey(mailType))
                            _localDeliveryTypes.Add(new Tuple<EnMailType, EnMailCategory, EnTransportType>(
                                mailType,
                                EnMailCategory.Parse(tempArr[1]),
                                tempArr.Length > 2 ? EnTransportType.Parse(tempArr[2]) : null
                                ));
                        else
                            foreach (var transportType in RussianPostAvailableOption.LocalTransportTypeAvailable[mailType])
                                _localDeliveryTypes.Add(new Tuple<EnMailType, EnMailCategory, EnTransportType>(
                                    mailType,
                                    EnMailCategory.Parse(tempArr[1]),
                                    transportType
                                    ));
                    }
                }
            }
            else if (method.Params.ContainsKey(RussianPostTemplate.OldLocalDeliveryTypes))
            {
                // поддержка настроек ранних версий
                string[] tempArr;
                foreach (var strDelivery in (method.Params.ElementOrDefault(RussianPostTemplate.OldLocalDeliveryTypes) ?? string.Empty).Split(","))
                {
                    if ((tempArr = strDelivery.Split("_")).Length >= 2)
                    {
                        var mailType = GetEnMailTypeByInt(tempArr[0].TryParseInt());
                        if (tempArr.Length > 2 || !RussianPostAvailableOption.LocalTransportTypeAvailable.ContainsKey(mailType))
                            _localDeliveryTypes.Add(new Tuple<EnMailType, EnMailCategory, EnTransportType>(
                                mailType,
                                GetEnMailCategoryByInt(tempArr[1].TryParseInt()),
                                tempArr.Length > 2 ? GetEnTransportTypeByInt(tempArr[2].TryParseInt()) : null
                                ));
                        else
                            foreach (var transportType in RussianPostAvailableOption.LocalTransportTypeAvailable[mailType])
                                _localDeliveryTypes.Add(new Tuple<EnMailType, EnMailCategory, EnTransportType>(
                                    mailType,
                                    GetEnMailCategoryByInt(tempArr[1].TryParseInt()),
                                    transportType
                                    ));
                    }
                }
            }
            else if (_deliveryTypesOld != null)
            {
                // поддержка настроек ранних версий
                foreach (var deliveryType in _deliveryTypesOld)
                {
                    foreach(var deliveryCategory in mailCategories)
                    {
                        if (typeInsure == EnTypeInsure.WithDeclaredValue && !IsDeclareCategory(deliveryCategory))
                            continue;
                        else if (typeInsure == EnTypeInsure.WithoutDeclaredValue && IsDeclareCategory(deliveryCategory))
                            continue;

                        if (RussianPostAvailableOption.LocalTransportTypeAvailable.ContainsKey(deliveryType))
                        {
                            foreach (var transportType in RussianPostAvailableOption.LocalTransportTypeAvailable[deliveryType])
                                _localDeliveryTypes.Add(new Tuple<EnMailType, EnMailCategory, EnTransportType>(
                                    deliveryType,
                                    deliveryCategory,
                                    transportType
                                    ));
                        }
                        else
                            _localDeliveryTypes.Add(new Tuple<EnMailType, EnMailCategory, EnTransportType>(
                                deliveryType,
                                deliveryCategory,
                                null
                                ));
                    }
                }
            }

            if (method.Params.ContainsKey(RussianPostTemplate.InternationalDeliveryTypes))
            {
                string[] tempArr;
                foreach (var strDelivery in (method.Params.ElementOrDefault(RussianPostTemplate.InternationalDeliveryTypes) ?? string.Empty).Split(","))
                {
                    if ((tempArr = strDelivery.Split("\\")).Length >= 2)
                    {
                        _internationalDeliveryTypes.Add(new Tuple<EnMailType, EnMailCategory, EnTransportType>(
                            EnMailType.Parse(tempArr[0]),
                            EnMailCategory.Parse(tempArr[1]),
                            tempArr.Length > 2 ? EnTransportType.Parse(tempArr[2]) : null
                            ));
                    }
                }
            }
            else if (method.Params.ContainsKey(RussianPostTemplate.OldInternationalDeliveryTypes))
            {
                // поддержка настроек ранних версий
                string[] tempArr;
                foreach (var strDelivery in (method.Params.ElementOrDefault(RussianPostTemplate.OldInternationalDeliveryTypes) ?? string.Empty).Split(","))
                {
                    if ((tempArr = strDelivery.Split("_")).Length >= 2)
                    {
                        _internationalDeliveryTypes.Add(new Tuple<EnMailType, EnMailCategory, EnTransportType>(
                            GetEnMailTypeByInt(tempArr[0].TryParseInt()),
                            GetEnMailCategoryByInt(tempArr[1].TryParseInt()),
                            tempArr.Length > 2 ? GetEnTransportTypeByInt(tempArr[2].TryParseInt()) : null
                            ));
                    }
                }
            }
            else if (_deliveryTypesOld != null)
            {
                // поддержка настроек ранних версий
                foreach (var deliveryType in _deliveryTypesOld)
                {
                    foreach (var deliveryCategory in mailCategories)
                    {
                        if (typeInsure == EnTypeInsure.WithDeclaredValue && !IsDeclareCategory(deliveryCategory))
                            continue;
                        else if (typeInsure == EnTypeInsure.WithoutDeclaredValue && IsDeclareCategory(deliveryCategory))
                            continue;

                        if (RussianPostAvailableOption.InternationalTransportTypeAvailable.ContainsKey(deliveryType))
                        {
                            foreach (var transportType in RussianPostAvailableOption.InternationalTransportTypeAvailable[deliveryType])
                                _internationalDeliveryTypes.Add(new Tuple<EnMailType, EnMailCategory, EnTransportType>(
                                    deliveryType,
                                    deliveryCategory,
                                    transportType
                                    ));
                        }
                        else
                            _internationalDeliveryTypes.Add(new Tuple<EnMailType, EnMailCategory, EnTransportType>(
                                deliveryType,
                                deliveryCategory,
                                null
                                ));

                    }
                }
            }


            _courier = method.Params.ElementOrDefault(RussianPostTemplate.Courier).TryParseBool();
            _fragile = method.Params.ElementOrDefault(RussianPostTemplate.Fragile).TryParseBool();
            _typeNotification = (EnTypeNotification?)method.Params.ElementOrDefault(RussianPostTemplate.TypeNotification).TryParseInt(true);
            _smsNotification = method.Params.ElementOrDefault(RussianPostTemplate.SmsNotification).TryParseBool();
            _statusesSync = method.Params.ElementOrDefault(RussianPostTemplate.StatusesSync).TryParseBool();
            _increaseDeliveryTime = _method.ExtraDeliveryTime;
            _yaMapsApiKey = _method.Params.ElementOrDefault(RussianPostTemplate.YaMapsApiKey);
            _deliveryWithCod = method.Params.ElementOrDefault(RussianPostTemplate.DeliveryWithCod).TryParseBool();
            _deliveryToOps = method.Params.ElementOrDefault(RussianPostTemplate.DeliveryToOps).TryParseBool();

            _trackingLogin = _method.Params.ElementOrDefault(RussianPostTemplate.TrackingLogin);
            _trackingPassword = _method.Params.ElementOrDefault(RussianPostTemplate.TrackingPassword);
            _russianPostTrackingApiService = new RussianPostTrackingApiService(_trackingLogin, _trackingPassword);

            var statusesReference = method.Params.ElementOrDefault(RussianPostTemplate.StatusesReference);
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
        }

        public RussianPostApiService RussianPostApiService
        {
            get { return _russianPostApiService; }
        }

        public bool SmsNotification
        {
            get { return _smsNotification; }
        }

        public bool Courier
        {
            get { return _courier; }
        }

        public bool Fragile
        {
            get { return _fragile; }
        }

        public bool DeliveryWithCod
        {
            get { return _deliveryWithCod; }
        }

        public EnTypeNotification? TypeNotification
        {
            get { return _typeNotification; }
        }

        public float DefaultWeight
        {
            get { return _defaultWeight; }
        }

        public float DefaultHeight
        {
            get { return _defaultHeight; }
        }

        public float DefaultLength
        {
            get { return _defaultLength; }
        }

        public float DefaultWidth
        {
            get { return _defaultWidth; }
        }

        #endregion

        #region Statuses

        public void SyncStatusOfOrder(Order order)
        {
            if (!string.IsNullOrEmpty(order.TrackNumber))
            {
                var history = _russianPostTrackingApiService.GetBarcodeHistory(order.TrackNumber);
                if (history.Body != null && history.Body.Response != null &&
                    history.Body.Response.OperationHistoryData != null &&
                    history.Body.Response.OperationHistoryData.HistoryRecords != null &&
                    history.Body.Response.OperationHistoryData.HistoryRecords.Count > 0)
                {
                    var statusInfo = 
                        history.Body.Response.OperationHistoryData.HistoryRecords
                            .Where(record =>
                            {
                                var tKey = record.OperationParameters.OperType.Id.ToString();
                                var tAttrKey =
                                    record.OperationParameters.OperAttr != null
                                        ? $"{tKey}_{record.OperationParameters.OperAttr.Id}"
                                        : null;
                                return
                                    (StatusesReference.ContainsKey(tKey) 
                                        && StatusesReference[tKey].HasValue)
                                    || (tAttrKey != null
                                        && StatusesReference.ContainsKey(tAttrKey)
                                        && StatusesReference[tAttrKey].HasValue);
                            })
                            .OrderByDescending(x => x.OperationParameters.OperDate)
                            .FirstOrDefault();

                    if (statusInfo is null)
                        return;
                    
                    var typeAndAttrKey = statusInfo.OperationParameters.OperAttr != null 
                        ? string.Format("{0}_{1}", statusInfo.OperationParameters.OperType.Id, statusInfo.OperationParameters.OperAttr.Id) 
                        : null;
                    var typeKey = statusInfo.OperationParameters.OperType.Id.ToString();

                    var russianPostOrderStatus = typeAndAttrKey != null && StatusesReference.ContainsKey(typeAndAttrKey)
                        ? StatusesReference[typeAndAttrKey]
                        : StatusesReference.ContainsKey(typeKey) 
                            ? StatusesReference[typeKey]
                            : null;

                    if (russianPostOrderStatus.HasValue &&
                        order.OrderStatusId != russianPostOrderStatus.Value &&
                        OrderStatusService.GetOrderStatus(russianPostOrderStatus.Value) != null)
                    {
                        var lastOrderStatusHistory =
                            OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                .OrderByDescending(item => item.Date)
                                .FirstOrDefault();

                        if (lastOrderStatusHistory == null ||
                            lastOrderStatusHistory.Date < statusInfo.OperationParameters.OperDate)
                        {
                            OrderStatusService.ChangeOrderStatus(order.OrderID,
                                russianPostOrderStatus.Value, "Синхронизация статусов для Почты России");
                        }
                    }
                }
            }
        }

        public bool SyncByAllOrders => false;
        public void SyncStatusOfOrders(IEnumerable<Order> orders) => throw new NotImplementedException();

        public bool StatusesSync
        {
            get => _statusesSync;
        }

        private Dictionary<string, int?> _statusesReference;
        public Dictionary<string, int?> StatusesReference
        {
            get => _statusesReference;
        }

        #endregion

        #region IShippingSupportingTheHistoryOfMovement

        public bool ActiveHistoryOfMovement
        {
            get { return true; }
        }
        public List<HistoryOfMovement> GetHistoryOfMovement(Order order)
        {
            if (!string.IsNullOrEmpty(order.TrackNumber) && !string.IsNullOrEmpty(_trackingLogin) && !string.IsNullOrEmpty(_trackingPassword))
            {
                var history = _russianPostTrackingApiService.GetBarcodeHistory(order.TrackNumber);
                if (history.Body != null && history.Body.Response != null &&
                    history.Body.Response.OperationHistoryData != null &&
                    history.Body.Response.OperationHistoryData.HistoryRecords != null &&
                    history.Body.Response.OperationHistoryData.HistoryRecords.Count > 0)
                {
                    return history.Body.Response.OperationHistoryData.HistoryRecords
                        .OrderByDescending(x => x.OperationParameters.OperDate).Select(statusInfo =>
                            new HistoryOfMovement()
                            {
                                Code = statusInfo.OperationParameters.OperType.Id.ToString(),
                                Name = statusInfo.OperationParameters.OperAttr.Name.IsNotEmpty()
                                    ? statusInfo.OperationParameters.OperAttr.Name
                                    : statusInfo.OperationParameters.OperType.Name,
                                Date = statusInfo.OperationParameters.OperDate,
                                Comment = statusInfo.AddressParameters != null &&
                                          statusInfo.AddressParameters.OperationAddress != null
                                    ? string.Join(", ", new[]
                                        {
                                            statusInfo.AddressParameters.OperationAddress.Index,
                                            statusInfo.AddressParameters.OperationAddress.Description
                                        }
                                        .Where(x => x.IsNotEmpty()))
                                    : null
                            }).ToList();
                }
            }

            return null;
        }

        public PointInfo GetPointInfo(Order order)
        {
            if (order.OrderPickPoint == null || order.OrderPickPoint.PickPointId.IsNullOrEmpty())
                return null;

            var points = GetAllPointsAsync().GetAwaiter().GetResult();

            if (points == null || points.Count == 0)
                return null;

            var russianPostPoint = points.FirstOrDefault(x => x.DeliveryPointIndex == order.OrderPickPoint.PickPointId);

            return russianPostPoint != null
                ? new PointInfo()
                {
                    Address = russianPostPoint.Address.AddressFromStreet() + (russianPostPoint.BrandName.IsNotEmpty() ? string.Format(" ({0})", russianPostPoint.BrandName) : null),
                    TimeWork = russianPostPoint.WorkTime,
                    Comment = russianPostPoint.GettoDescription
                }
                : null;
        }

        #endregion IShippingSupportingTheHistoryOfMovement

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var shippingOptions = new List<BaseShippingOption>();

            string countryIso2 = null;
            if (_preOrder.CountryDest.IsNotEmpty())
                countryIso2 = CountryService.GetIso2(_preOrder.CountryDest);

            var isInternational = !"ru".Equals(countryIso2, StringComparison.OrdinalIgnoreCase);
            var countryCode = isInternational
                ? CountryService.Iso2ToIso3Number(countryIso2).TryParseInt(true)
                : null;
            var zip = isInternational ? null : LoadZip(); // индекс при расчете нужен только для россии (иначе из-за сервис перестает считать)

            var settigns = CacheManager.Get(string.Format("RussianPostAccountSettings_{0}", _token), 60,
                () => _russianPostApiService.GetAccountSettings());

            if (settigns != null)
            {
                var shippingPoint = settigns.ShippingPoints.FirstOrDefault(x => x.Enabled == true && x.OperatorIndex == _pointIndex);
                if (shippingPoint != null)
                {

                    IList<AvailableProduct> availableProducts = isInternational
                        // международная доставка
                        ? shippingPoint.AvailableProducts.Where(x => IsInternational(x) && _internationalDeliveryTypes.Any(dt => dt.Item1 == x.MailType && dt.Item2 == x.MailCategory)).ToList()
                        // по России
                        : shippingPoint.AvailableProducts.Where(x => !IsInternational(x) && _localDeliveryTypes.Any(dt => dt.Item1 == x.MailType && dt.Item2 == x.MailCategory)).ToList(); 

                    if (isInternational && !countryCode.HasValue)
                    {
                        return shippingOptions;
                    }

                    if (availableProducts.Count > 0)
                    {
                        var orderCost = _totalPrice;
                        int weight = (int)GetTotalWeight(1000);
                        var dimensionsHelp = GetDimensions(rate: 10);
                        var dimensions = new Dimension()
                        {
                            Length = (int)Math.Ceiling(dimensionsHelp[0]),
                            Width = (int)Math.Ceiling(dimensionsHelp[1]),
                            Height = (int)Math.Ceiling(dimensionsHelp[2]),
                        };

                        string selectedPoint = null;
                        if (_preOrder.ShippingOption != null &&
                            _preOrder.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(RussianPost).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                        {
                            if (_preOrder.ShippingOption.GetType() == typeof(RussianPostPointDeliveryMapOption))
                                selectedPoint = ((RussianPostPointDeliveryMapOption)_preOrder.ShippingOption).PickpointId;
                        }

                        var calcMail = new HashSet<int>();

                        var tasks = new List<Task<BaseShippingOption>>();

                        foreach (var availableProduct in availableProducts/*.OrderBy(x => x.MailType.ToString())*/)
                        {
                            // если не возможности отображать карту
                            // убираем продуктыы, для которых эти данные обязательны (отображение на карте)
                            if (_yaMapsApiKey.IsNullOrEmpty() && (availableProduct.MailType == EnMailType.ECOM || IsPochtamatsTariff(availableProduct.MailType, availableProduct.MailCategory)))
                                continue;

                            if (IsCodCategory(availableProduct.MailCategory))
                                continue;// убираем данный тип из расчетов, т.к. это будет определяться выбраным типом оплаты (наложенный платеж)

                            var hash = 17 ^ availableProduct.MailType.ToString().GetHashCode() ^ availableProduct.MailCategory.ToString().GetHashCode();

                            if (calcMail.Contains(hash))
                                continue;
                            else
                                calcMail.Add(hash);

                            if (isInternational)
                            {
                                if (RussianPostAvailableOption.InternationalTransportTypeAvailable.ContainsKey(availableProduct.MailType))
                                    foreach (var transportType in RussianPostAvailableOption.InternationalTransportTypeAvailable[availableProduct.MailType])
                                    {
                                        if (_internationalDeliveryTypes.Any(dt => dt.Item1 == availableProduct.MailType && dt.Item2 == availableProduct.MailCategory && dt.Item3 == transportType))
                                            tasks.Add(CalcOptionsAsync(shippingPoint, availableProduct.MailType, availableProduct.MailCategory, transportType, zip, countryCode, selectedPoint,
                                                  orderCost, weight, dimensions, availableProducts, settigns, isInternational, toOps: false));
                                    }
                                else
                                    tasks.Add(CalcOptionsAsync(shippingPoint, availableProduct.MailType, availableProduct.MailCategory, null, zip, countryCode, selectedPoint, orderCost,
                                              weight, dimensions, availableProducts, settigns, isInternational, toOps: false));
                            }
                            else
                            {
                                if (RussianPostAvailableOption.LocalTransportTypeAvailable.ContainsKey(availableProduct.MailType))
                                    foreach (var transportType in RussianPostAvailableOption.LocalTransportTypeAvailable[availableProduct.MailType])
                                    {
                                        if (_localDeliveryTypes.Any(dt => dt.Item1 == availableProduct.MailType && dt.Item2 == availableProduct.MailCategory && dt.Item3 == transportType))
                                        {
                                            tasks.Add(CalcOptionsAsync(shippingPoint, availableProduct.MailType, availableProduct.MailCategory, transportType, zip, countryCode, selectedPoint,
                                                orderCost, weight, dimensions, availableProducts, settigns, isInternational, toOps: false));
                                            
                                            // до востредования
                                            if (_deliveryToOps && RussianPostAvailableOption.DeliveryToOpsAvailable.Contains(availableProduct.MailType) &&
                                                // в ЕКОМ и почтамат не надо считать как "до востредования"
                                                availableProduct.MailType != EnMailType.ECOM && !IsPochtamatsTariff(availableProduct.MailType, availableProduct.MailCategory))
                                                tasks.Add(CalcOptionsAsync(shippingPoint, availableProduct.MailType, availableProduct.MailCategory, transportType, zip, countryCode, selectedPoint,
                                                    orderCost, weight, dimensions, availableProducts, settigns, isInternational, toOps: true));
                                        }
                                    }
                                else
                                {
                                    tasks.Add(CalcOptionsAsync(shippingPoint, availableProduct.MailType, availableProduct.MailCategory, null, zip, countryCode, selectedPoint, orderCost,
                                              weight, dimensions, availableProducts, settigns, isInternational, toOps: false));

                                    // до востредования
                                    if (_deliveryToOps && RussianPostAvailableOption.DeliveryToOpsAvailable.Contains(availableProduct.MailType) &&
                                        // в ЕКОМ и почтамат не надо считать как "до востредования"
                                        availableProduct.MailType != EnMailType.ECOM && !IsPochtamatsTariff(availableProduct.MailType, availableProduct.MailCategory))
                                        tasks.Add(CalcOptionsAsync(shippingPoint, availableProduct.MailType, availableProduct.MailCategory, null, zip, countryCode, selectedPoint, orderCost,
                                            weight, dimensions, availableProducts, settigns, isInternational, toOps: true));
                                }
                            }
                        }

                        Task.WaitAll(tasks.ToArray(), TimeSpan.FromMinutes(1));
                        tasks.Where(x => x.Exception != null).ForEach(Debug.Log.Warn);
                        tasks.Where(x => x.Exception == null).Select(x => x.Result).Where(x => x != null).ForEach(shippingOptions.Add);
                    }
                }
            }

            return shippingOptions.OrderBy(x => x.Rate);
        }

        private async Task<BaseShippingOption> CalcOptionsAsync(ShippingPoint shippingPoint, EnMailType enMailType, EnMailCategory mailCategory, EnTransportType transportType, string zip, int? countryCode,
            string selectedPoint, float orderCost, int weight, Dimension dimensions, IEnumerable<AvailableProduct> availableProducts, AccountSettings settigns, bool isInternational, bool toOps)
        {
            // Проверка на превышение веса для типа отправления
            if (RussianPostAvailableOption.MaxWeightMailType.ContainsKey(enMailType) &&
                weight > RussianPostAvailableOption.MaxWeightMailType[enMailType])
                return null;
            if (isInternational && RussianPostAvailableOption.MaxWeightForInternationalMailType.ContainsKey(enMailType) &&
                weight > RussianPostAvailableOption.MaxWeightForInternationalMailType[enMailType])
                return null;

            // Проверка на минимальный вес для типа отправления
            if (RussianPostAvailableOption.MinWeightMailType.ContainsKey(enMailType) &&
                weight < RussianPostAvailableOption.MinWeightMailType[enMailType])
                return null;

            // Проверка на превышение габаритов для типа отправления
            if (RussianPostAvailableOption.MaxSumDimensionsMailType.ContainsKey(enMailType))
            {
                var sumDimensions = (dimensions.Height + dimensions.Length + dimensions.Width) * 10;
                if (sumDimensions > RussianPostAvailableOption.MaxSumDimensionsMailType[enMailType])
                    return null;
            }

            // Проверка на превышение габарита для типа отправления
            if (RussianPostAvailableOption.MaxDimensionMailType.ContainsKey(enMailType))
            {
                var maxDimensions = new[] { dimensions.Height, dimensions.Length, dimensions.Width }.Max() * 10;
                if (maxDimensions > RussianPostAvailableOption.MaxDimensionMailType[enMailType])
                    return null;
            }

            // Проверка на превышение габаритов для типа отправления
            if (RussianPostAvailableOption.MaxDimensionsMailType.ContainsKey(enMailType))
            {
                if (dimensions.Length * 10 > RussianPostAvailableOption.MaxDimensionsMailType[enMailType][0] ||
                    dimensions.Width * 10 > RussianPostAvailableOption.MaxDimensionsMailType[enMailType][1] ||
                    dimensions.Height * 10 > RussianPostAvailableOption.MaxDimensionsMailType[enMailType][2])
                    return null;
            }

            // Проверка на превышение габаритов для типа отправления (сумма длины и периметра наибольшего поперечного сечения)
            if (RussianPostAvailableOption.MaxLength2Height2WidthDimensionsMailType.ContainsKey(enMailType))
            {
                var sumDimensions = (dimensions.Length + 2 * dimensions.Height + 2 * dimensions.Width) * 10;
                if (sumDimensions > RussianPostAvailableOption.MaxLength2Height2WidthDimensionsMailType[enMailType])
                    return null;
            }

            var isDeclare = IsDeclareCategory(mailCategory);
            int declareValue = (int)(orderCost * 100);

            // ECOM
            int? goodValue = null;
            RussianPostPoint deliveryPoint = null;
            List<RussianPostPoint> deliveryPoints = null;
            if (enMailType == EnMailType.ECOM)
            {
                if (EnDimensionType.GetDimensionType(dimensions) == null)
                    return null;

                var points = 
                    _preOrder.RegionDest.IsNotEmpty() && _preOrder.CityDest.IsNotEmpty()
                        ? await GetPointsCityAsync(_preOrder.RegionDest, _preOrder.CityDest, _preOrder.DistrictDest, weight, dimensions).ConfigureAwait(false)
                        : null;
                if (points == null || points.Count == 0)
                    return null;

                deliveryPoints = CastPoints(points);
                deliveryPoint = selectedPoint != null
                    ? deliveryPoints.FirstOrDefault(x => x.Code == selectedPoint) ?? deliveryPoints[0]
                    : deliveryPoints[0];

                goodValue = (int)(orderCost * 100);
            }

            if (IsPochtamatsTariff(enMailType, mailCategory))
            {
                var dimensionType = EnDimensionType.GetDimensionType(dimensions);
                if (dimensionType == null || dimensionType == EnDimensionType.Oversized)
                    return null;

                var points = 
                    _preOrder.RegionDest.IsNotEmpty() && _preOrder.CityDest.IsNotEmpty()
                        ? GetPochtamatsCity(_preOrder.RegionDest, _preOrder.CityDest, _preOrder.DistrictDest, weight, dimensions)
                        : null;
                if (points == null || points.Count == 0)
                    return null;

                deliveryPoints = CastPoints(points);
                deliveryPoint = selectedPoint != null
                    ? deliveryPoints.FirstOrDefault(x => x.Code == selectedPoint) ?? deliveryPoints[0]
                    : deliveryPoints[0];
            }

            if (toOps)
            {
                var points = 
                    _preOrder.RegionDest.IsNotEmpty() && _preOrder.CityDest.IsNotEmpty()
                        ? GetOpsCity(_preOrder.RegionDest, _preOrder.CityDest, _preOrder.DistrictDest, weight, dimensions)
                        : null;
                if (points == null || points.Count == 0)
                    return null;

                deliveryPoints = CastPoints(points);
                deliveryPoint = selectedPoint != null
                    ? deliveryPoints.FirstOrDefault(x => x.Code == selectedPoint) ?? deliveryPoints[0]
                    : deliveryPoints[0];
            }

            // для расчета по России нужен корректный индекс
            // для доставки в ПВЗ и за границей индекс не важен
            if (deliveryPoint == null && !isInternational && (zip.IsNullOrEmpty() || !zip.IsInt() || zip.Length != 6))
                return null;
            
            // для вывода точек доставки нужна работающая карта
            if (_yaMapsApiKey.IsNullOrEmpty() && deliveryPoint != null && deliveryPoints != null && deliveryPoints.Count > 0)
                return null;

            //var calc = await CalculateAsync(shippingPoint.OperatorIndex, enMailType, mailCategory, transportType, zip, countryCode, deliveryPoint,
            //    isDeclare ? declareValue : (int?)null, goodValue, weight, dimensions).ConfigureAwait(false);
            var calcTariff = await CalculateTariffAsync(shippingPoint.OperatorIndex, enMailType, mailCategory, transportType, zip, countryCode, deliveryPoint,
                isDeclare ? declareValue : (int?)null, goodValue, weight, dimensions, settigns).ConfigureAwait(false);

            //if (calc != null && calc.TotalRateNoVat > 0)
            if (calcTariff != null && calcTariff.Errors == null)
            {
                var cashProduct = !mailCategory.Value.StartsWith("COMBINED_")
                    ? availableProducts
                        .OrderByDescending(x =>
                            (_deliveryWithCod && (x.MailCategory == EnMailCategory.WithCompulsoryPayment || x.MailCategory == EnMailCategory.WithDeclaredValueAndCompulsoryPayment)) ||
                            (!_deliveryWithCod && x.MailCategory == EnMailCategory.WithDeclaredValueAndCashOnDelivery)
                                ? 1
                                : 0)
                        .FirstOrDefault(x => 
                            x.MailType == enMailType && 
                            (x.MailCategory == EnMailCategory.WithDeclaredValueAndCashOnDelivery || x.MailCategory == EnMailCategory.WithCompulsoryPayment
                            || x.MailCategory == EnMailCategory.WithDeclaredValueAndCompulsoryPayment))
                    : availableProducts
                        .FirstOrDefault(x =>
                            x.MailType == enMailType && (x.MailCategory == EnMailCategory.CombinedWithDeclaredValueAndCashOnDelivery));

                var cashMailCategory = cashProduct != null ? cashProduct.MailCategory : null;

                if (deliveryPoint != null && !deliveryPoint.Card && !deliveryPoint.Cash)
                    cashMailCategory = null;// для выбранной точки не доступно оплата при получении

                //CalculateResponse calcCash = cashMailCategory != null
                //    ? await CalculateAsync(shippingPoint.OperatorIndex, enMailType, cashMailCategory, transportType, zip, countryCode, deliveryPoint,
                //        declareValue, goodValue, weight, dimensions).ConfigureAwait(false)
                //    : null;

                var calcCashTariff = cashMailCategory != null
                    ? await CalculateTariffAsync(shippingPoint.OperatorIndex, enMailType, cashMailCategory, transportType, zip, countryCode, deliveryPoint,
                        declareValue, goodValue, weight, dimensions, settigns).ConfigureAwait(false)
                    : null;

                return
                    CreateOption(
                        //rate: GetDeliverySum(calc, withInsurance: isDeclare || goodValue.HasValue),
                        //rateCash: GetDeliverySum(calcCash != null && calcCash.TotalRateNoVat > 0 ? calcCash : calc, withInsurance: true),
                        rate: GetDeliverySum(calcTariff),
                        rateCash: GetDeliverySum(calcCashTariff != null && calcCashTariff.Errors == null ? calcCashTariff : calcTariff),
                        //deliveryTime: calc.DeliveryTime != null ? new DeliveryPeriod { MinDays = calc.DeliveryTime.MinDays, MaxDays = calc.DeliveryTime.MaxDays } : null,
                        deliveryTime: calcTariff.Delivery != null 
                            ? new DeliveryPeriod
                            {
                                MinDays = calcTariff.Delivery.Min, 
                                MaxDays = calcTariff.Delivery.Min != calcTariff.Delivery.Max 
                                    ? calcTariff.Delivery.Max 
                                    : (int?)null
                            } 
                            : null,
                        indexFrom: shippingPoint.OperatorIndex,
                        indexTo: deliveryPoint != null ? deliveryPoint.Code : zip,
                        countryCode: countryCode,
                        mailType: enMailType,
                        mailCategory: mailCategory,
                        cashMailCategory: cashMailCategory,
                        transportType: transportType,
                        //cashOnDeliveryAvailable: calcCash != null && calcCash.TotalRateNoVat > 0,
                        cashOnDeliveryAvailable: calcCashTariff != null && calcCashTariff.Errors == null,
                        deliveryPoint: deliveryPoint,
                        deliveryPoints: deliveryPoints,
                        weight: weight,
                        dimensions: dimensions,
                        toOps: toOps);
            }

            return null;
        }

        private async Task<CalculateResponse> CalculateAsync(string indexFrom, EnMailType enMailType, EnMailCategory mailCategory, EnTransportType transportType, string indexTo, int? countryCode, DeliveryPoint deliveryPoint,
            int? declaredValue, int? goodValue, int weight, Dimension dimensions)
        {
            return await _russianPostApiService.CalculateAsync(new CalculateOptions()
            {
                DeclaredValue = declaredValue,
                GoodValue = goodValue,
                Dimension = RussianPostAvailableOption.DimensionsNotRequired.Contains(enMailType) ? null : dimensions,
                Weight = weight,
                IndexFrom = indexFrom,
                IndexTo = deliveryPoint != null ? deliveryPoint.Address.Index : indexTo,
                DeliveryPointIndex = deliveryPoint != null ? deliveryPoint.DeliveryPointIndex : null,
                CountryCode = countryCode,
                MailCategory = mailCategory,
                MailType = enMailType,
                TransportType = transportType,
                DimensionType = enMailType == EnMailType.ECOM ? EnDimensionType.GetDimensionType(dimensions) : null,
                Courier =
                    _courier && RussianPostAvailableOption.CourierOptionAvailable.Contains(enMailType)
                        ? true
                        : (bool?) null,
                Fragile =
                    _fragile && RussianPostAvailableOption.FragileOptionAvailable.Contains(enMailType)
                        ? true
                        : (bool?) null,
                //PaymentMethod = EnPaymentMethod.Cashless,

                SmsNoticeRecipient =
                    _smsNotification &&
                    RussianPostAvailableOption.SmsNoticeOptionAvailable.ContainsKey(enMailType) &&
                    RussianPostAvailableOption.SmsNoticeOptionAvailable[enMailType].Contains(mailCategory)
                        ? 1 : (int?)null,
                WithOrderOfNotice =
                    _typeNotification == EnTypeNotification.WithOrderOfNotice &&
                    RussianPostAvailableOption.OrderOfNoticeOptionAvailable.ContainsKey(enMailType) &&
                    RussianPostAvailableOption.OrderOfNoticeOptionAvailable[enMailType].Contains(mailCategory)
                        ? true
                        : (bool?)null,
                WithSimpleNotice = 
                    _typeNotification == EnTypeNotification.WithSimpleNotice &&
                    RussianPostAvailableOption.SimpleNoticeOptionAvailable.ContainsKey(enMailType) &&
                    RussianPostAvailableOption.SimpleNoticeOptionAvailable[enMailType].Contains(mailCategory) 
                        ? true
                        : (bool?)null,
                WithElectronicNotice = 
                    _typeNotification == EnTypeNotification.WithElectronicNotice &&
                    RussianPostAvailableOption.ElectronicNoticeOptionAvailable.ContainsKey(enMailType) &&
                    RussianPostAvailableOption.ElectronicNoticeOptionAvailable[enMailType].Contains(mailCategory)
                        ? true
                        : (bool?)null
            }).ConfigureAwait(false);
        }

        private async Task<TariffApi.CalculateResponse> CalculateTariffAsync(string indexFrom, EnMailType enMailType, 
            EnMailCategory mailCategory, EnTransportType transportType, string indexTo, int? countryCode, RussianPostPoint deliveryPoint,
            int? declaredValue, int? goodValue, int weight, Dimension dimensions, AccountSettings settigns)
        {
            var calculateParams = new TariffApi.CalculateParams();

            calculateParams.ObjectId = GetTariffObjectId(enMailType, mailCategory, isInternational: countryCode.HasValue);

            if (calculateParams.ObjectId <= 0)
                return null;

            // ToDo: стоит проверять какие параметры принимаются объектом и на основе этого 
            // заполнять параметры с учетом остальных усовий
            // Вердикт - не стоит. Замечено, что их api возвращает параметры, которые для рассчета
            // не нужны. А также не возвращает параметры, которые нужны для рассчета. Передача, же 
            // лишних параметров и услуг не приводит к проблемам.
            // Upd: по некоторым лишним услугам выдает ошибку
            // Проверка по услугам ниже

            calculateParams.IndexFrom = indexFrom;
            calculateParams.IndexTo = deliveryPoint != null ? deliveryPoint.Code : indexTo; // deliveryPoint.Address.Index

            if (transportType != null)
            {
                if (transportType == EnTransportType.Avia)
                    calculateParams.IsAvia = TariffApi.EnIsAviaType.Avia;
                if (transportType == EnTransportType.Surface)
                    calculateParams.IsAvia = TariffApi.EnIsAviaType.Surface;
            }
            else if (countryCode.HasValue)
            {
                // замечено что сервис otpravka.pochta.ru международные отправления
                // отправляет в приоритете авиадаставкой
                calculateParams.IsAvia = TariffApi.EnIsAviaType.AviaOrSurface;
            }

            calculateParams.Country = countryCode;
            calculateParams.CountryTo = countryCode;

            calculateParams.Sumoc = declaredValue;

            if (mailCategory == EnMailCategory.WithDeclaredValueAndCashOnDelivery 
                || mailCategory == EnMailCategory.WithDeclaredValueAndCompulsoryPayment
                || mailCategory == EnMailCategory.CombinedWithDeclaredValueAndCashOnDelivery)
                calculateParams.Sumnp = declaredValue;

            calculateParams.Sumin = goodValue;

            calculateParams.Weight = weight;

            if (enMailType == EnMailType.ECOM)
            {
                var dimensionType = EnDimensionType.GetDimensionType(dimensions);
                if (dimensionType != null)
                {
                    if (dimensionType == EnDimensionType.S)
                        calculateParams.Pack = TariffApi.EnPackType.BoxS;
                    else if (dimensionType == EnDimensionType.M)
                        calculateParams.Pack = TariffApi.EnPackType.BoxM;
                    else if (dimensionType == EnDimensionType.L)
                        calculateParams.Pack = TariffApi.EnPackType.BoxL;
                    else if (dimensionType == EnDimensionType.XL)
                        calculateParams.Pack = TariffApi.EnPackType.BoxXL;
                    else if (dimensionType == EnDimensionType.Oversized)
                        calculateParams.Pack = TariffApi.EnPackType.Oversized;
                }
            }

            var deliveryParams = CacheManager.Get(
                "RussianPostDeliveryParams-" + calculateParams.ObjectId,
                60,
                () => TariffApi.RussianPostTariffApiService.GetDeliveryParams(calculateParams.ObjectId));
            var availableServices =
                deliveryParams?.Object
                    ?.FirstOrDefault(x => x.Id == calculateParams.ObjectId)
                    ?.Service
                    ?.Select(x => x.Id)
                    .ToList();
            calculateParams.Countinpack = settigns.PlannedMonthlyNumber;

            // Корпоративный клиент
            calculateParams.Services.Add(28);

            if (_courier 
                && RussianPostAvailableOption.CourierOptionAvailable.Contains(enMailType)
                && (availableServices is null || availableServices.Contains(26))
                )
                calculateParams.Services.Add(26);

            if (_fragile 
                && RussianPostAvailableOption.FragileOptionAvailable.Contains(enMailType)
                && (availableServices is null || availableServices.Contains(4))
                )
                calculateParams.Services.Add(4);

            if (_smsNotification
                && RussianPostAvailableOption.SmsNoticeOptionAvailable.ContainsKey(enMailType)
                && RussianPostAvailableOption.SmsNoticeOptionAvailable[enMailType].Contains(mailCategory)
                && (availableServices is null || availableServices.Contains(44))
                )
                calculateParams.Services.Add(44);

            if (_typeNotification == EnTypeNotification.WithOrderOfNotice
                && RussianPostAvailableOption.OrderOfNoticeOptionAvailable.ContainsKey(enMailType)
                && RussianPostAvailableOption.OrderOfNoticeOptionAvailable[enMailType].Contains(mailCategory)
                && (availableServices is null || availableServices.Contains(2))
                )
                calculateParams.Services.Add(2);

            if (_typeNotification == EnTypeNotification.WithSimpleNotice
                && RussianPostAvailableOption.SimpleNoticeOptionAvailable.ContainsKey(enMailType)
                && RussianPostAvailableOption.SimpleNoticeOptionAvailable[enMailType].Contains(mailCategory)
                && (availableServices is null || availableServices.Contains(1))
               )
                calculateParams.Services.Add(1);

            if (_typeNotification == EnTypeNotification.WithElectronicNotice
                && RussianPostAvailableOption.ElectronicNoticeOptionAvailable.ContainsKey(enMailType)
                && RussianPostAvailableOption.ElectronicNoticeOptionAvailable[enMailType].Contains(mailCategory)
                && (availableServices is null || availableServices.Contains(62))
               )
                calculateParams.Services.Add(62);

            // Посылка считается негабаритной, если сумма измерений трех сторон отправления превышает 120 см или одна из сторон отправления превышает 60 см.
            if (enMailType == EnMailType.PostalParcel && 
                (dimensions.Height + dimensions.Length + dimensions.Width) > 120 ||
                Math.Max(Math.Max(dimensions.Height.Value, dimensions.Length.Value), dimensions.Width.Value) > 60)
                if (availableServices is null || availableServices.Contains(12))
                    calculateParams.Services.Add(12);

            var termOfDelivery = calculateParams.IndexTo.IsNotEmpty();
            var result = await TariffApi.RussianPostTariffApiService.CalculateAsync(termOfDelivery, @params: calculateParams).ConfigureAwait(false);

            if (termOfDelivery && result != null && ((result.Errors == null && !result.Paynds.HasValue) || (result.Errors != null && result.Errors.Any(er => er.Type == 2))))
            {
                // Баг со стороны ПР
                // На "Бандероль с объявленной ценностью" замечено, что расчет стоимости доставки со сроками
                // не возвращает стоимость доставки. Если же запросить только расчет стоимости, то возвращает.

                var resultOnlyCalc = await TariffApi.RussianPostTariffApiService.CalculateAsync(termOfDelivery: false, @params: calculateParams).ConfigureAwait(false);
                if (resultOnlyCalc != null)
                    resultOnlyCalc.Delivery = result.Delivery;

                result = resultOnlyCalc;
            }

            return result;
        }

        private long GetTariffObjectId(EnMailType enMailType, EnMailCategory mailCategory, bool isInternational)
        {
            var mailTypeCode = string.Empty;
            var mailCategoryCode = string.Empty;

            // enMailType
            if (enMailType == EnMailType.PostalParcel)
                mailTypeCode = "4";

            else if (enMailType == EnMailType.OnlineParcel)
                mailTypeCode = "23";

            else if (enMailType == EnMailType.OnlineCourier)
                mailTypeCode = "24";

            else if (enMailType == EnMailType.Ems)
                mailTypeCode = "7";

            else if (enMailType == EnMailType.EmsOptimal)
                mailTypeCode = "34";

            else if (enMailType == EnMailType.EmsRT)
                mailTypeCode = "41";

            else if (enMailType == EnMailType.EmsTender)
                mailTypeCode = "52";

            else if (enMailType == EnMailType.Letter)
                mailTypeCode = "2";

            else if (enMailType == EnMailType.LetterClass1)
                mailTypeCode = "15";

            else if (enMailType == EnMailType.Banderol)
                mailTypeCode = "3";

            else if (enMailType == EnMailType.BusinessCourier)
                mailTypeCode = "30";

            else if (enMailType == EnMailType.BusinessCourierExpress)
                mailTypeCode = "31";

            else if (enMailType == EnMailType.ParcelClass1)
                mailTypeCode = "47";

            else if (enMailType == EnMailType.BanderolClass1)
                mailTypeCode = "16";

            else if (enMailType == EnMailType.VGPOClass1)
                mailTypeCode = "46";

            else if (enMailType == EnMailType.SmallPacket)
                mailTypeCode = "5";

            else if (enMailType == EnMailType.VSD)
                mailTypeCode = "";// нету такого объекта

            else if (enMailType == EnMailType.ECOM)
                mailTypeCode = "53";


            // mailCategory
            if (mailCategory == EnMailCategory.Simple)
                mailCategoryCode = "00";
            else if (mailCategory == EnMailCategory.Ordered)
                mailCategoryCode = "01";
            else if (mailCategory == EnMailCategory.Ordinary)
                mailCategoryCode = "03";
            else if (mailCategory == EnMailCategory.WithDeclaredValue)
                mailCategoryCode = "02";
            else if (mailCategory == EnMailCategory.WithDeclaredValueAndCashOnDelivery)
                mailCategoryCode = "04";
            else if (mailCategory == EnMailCategory.WithDeclaredValueAndCompulsoryPayment)
                mailCategoryCode = "06";
            else if (mailCategory == EnMailCategory.WithCompulsoryPayment)
                mailCategoryCode = "07";
            else if (mailCategory == EnMailCategory.CombinedOrdinary)
                mailCategoryCode = "08";
            else if (mailCategory == EnMailCategory.CombinedWithDeclaredValue)
                mailCategoryCode = "09";
            else if (mailCategory == EnMailCategory.CombinedWithDeclaredValueAndCashOnDelivery)
                mailCategoryCode = "10";

            return mailTypeCode.IsNotEmpty() && mailCategoryCode.IsNotEmpty()
                ? (mailTypeCode + mailCategoryCode + (isInternational ? "1" : "0")).TryParseLong()
                : 0;
        }

        private BaseShippingOption CreateOption(float rate, float rateCash, DeliveryPeriod deliveryTime, string indexFrom, string indexTo, int? countryCode, EnMailType mailType, EnMailCategory mailCategory, 
            EnMailCategory cashMailCategory, EnTransportType transportType, bool cashOnDeliveryAvailable, RussianPostPoint deliveryPoint, List<RussianPostPoint> deliveryPoints, int weight, Dimension dimensions,
            bool toOps)
        {
            var name = string.Format("{2} ({0} {1}{3}{4})",
                        mailType.Localize(),
                        mailCategory.Localize().ToLower(),
                        _method.Name,
                        transportType != null
                            ? " " + transportType.Localize().ToLower()
                            : string.Empty,
                        toOps
                            ? " до востребования"
                            : string.Empty);

            var deliveryTimeStr = deliveryTime != null && deliveryTime.MinDays.HasValue
                    ? string.Format("{0:. дн\\.}{1}{2:. дн\\.}",
                        deliveryTime.MinDays.HasValue ? deliveryTime.MinDays.Value + _increaseDeliveryTime : (int?)null,
                        deliveryTime.MinDays.HasValue && deliveryTime.MaxDays.HasValue ? " - " : "",
                        deliveryTime.MaxDays.HasValue ? deliveryTime.MaxDays.Value + _increaseDeliveryTime : (int?)null)
                    : null;

            if (deliveryPoint != null && deliveryPoints != null && deliveryPoints.Count > 0)
            {
                var option = new RussianPostPointDeliveryMapOption(_method, _totalPrice)
                {
                    DeliveryId = string.Format("{0}\\{1}\\{2}\\{3}", indexFrom, mailCategory, mailType, transportType).GetHashCode(),
                    Name = name,
                    Rate = rate,
                    BasePrice = rate,
                    PriceCash = rateCash,
                    CashMailCategory = cashMailCategory,
                    MailCategory = mailCategory,
                    DeliveryTime = deliveryTimeStr,
                    IsAvailablePaymentCashOnDelivery = cashOnDeliveryAvailable && (deliveryPoint.Cash || deliveryPoint.Card),
                    CalculateOption = new RussianPostCalculateOption
                    {
                        MailType = mailType,
                        MailCategory = mailCategory,
                        IndexFrom = indexFrom,
                        IndexTo = deliveryPoint.Code,
                        CountryCode = countryCode,
                        TransportType = transportType,
                        ToOps = toOps
                    },
                    CurrentPoints = deliveryPoints,
                };
                SetMapData(option, weight, dimensions, toOps);

                return option;
            }
            
            return new RussianPostOption(_method, _totalPrice)
            {
                DeliveryId = string.Format("{0}\\{1}\\{2}\\{3}", indexFrom, mailCategory, mailType, transportType).GetHashCode(),
                Name = name,
                Rate = rate,
                BasePrice = rate,
                PriceCash = rateCash,
                CashMailCategory = cashMailCategory,
                MailCategory = mailCategory,
                DeliveryTime = deliveryTimeStr,
                IsAvailablePaymentCashOnDelivery = cashOnDeliveryAvailable,
                CalculateOption = new RussianPostCalculateOption
                {
                    MailType = mailType,
                    MailCategory = mailCategory,
                    IndexFrom = indexFrom,
                    IndexTo = indexTo,
                    CountryCode = countryCode,
                    TransportType = transportType,
                },
            };
        }

        private void SetMapData(RussianPostPointDeliveryMapOption option, int weight, Dimension dimensions, bool toOps)
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
                    { "region", _preOrder.RegionDest },
                    { "city", _preOrder.CityDest },
                    { "district", _preOrder.DistrictDest },
                    { "weight", weight },
                    { "dimensionsH", dimensions.Height },
                    { "dimensionsW", dimensions.Width },
                    { "dimensionsL", dimensions.Length },
                    {
                        "typePoints", 
                        option.CalculateOption.MailType == EnMailType.ECOM
                            ? "ecom"
                            : toOps
                                ? "ops"
                                : "pochtamats"
                    },
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("region") || !data.ContainsKey("city") || !data.ContainsKey("district")
                || !data.ContainsKey("weight") || !data.ContainsKey("dimensionsH") || 
                !data.ContainsKey("dimensionsW") || !data.ContainsKey("dimensionsL") || !data.ContainsKey("typePoints"))
                return null;

            var region = (string)data["region"];
            var city = (string)data["city"];
            var district = (string)data["district"];
            var weight = data["weight"].ToString().TryParseInt();
            var dimensions = new Dimension
            {
                Height = data["dimensionsH"].ToString().TryParseInt(true),
                Width = data["dimensionsW"].ToString().TryParseInt(true),
                Length = data["dimensionsL"].ToString().TryParseInt(true),
            };
            var typePoints = (string)data["typePoints"];

            List<RussianPostPoint> points = null;
            if (string.Equals(typePoints, "ecom", StringComparison.OrdinalIgnoreCase))
            {
                var deliveryPoints =
                    GetPointsCityAsync(region, city, district, weight, dimensions)
                        .GetAwaiter().GetResult();
                points = CastPoints(deliveryPoints);
            }
            else if (string.Equals(typePoints, "ops", StringComparison.OrdinalIgnoreCase))
            {
                var deliveryPoints =
                    GetOpsCity(region, city, district, weight, dimensions);
                points = CastPoints(deliveryPoints);
            }
            else if (string.Equals(typePoints, "pochtamats", StringComparison.OrdinalIgnoreCase))
            {
                var deliveryPoints =
                    GetPochtamatsCity(region, city, district, weight, dimensions);
                points = CastPoints(deliveryPoints);
            }

            return points != null
                ? GetFeatureCollection(points)
                : null;
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<RussianPostPoint> points)
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

        private string LoadZip()
        {
            string zip = null;

            if (_preOrder.ZipDest.IsNotEmpty())
            {
                zip = _preOrder.ZipDest;
            }
            else if (_preOrder.CityDest.IsNotEmpty() && _preOrder.RegionDest.IsNotEmpty())
            {
                zip = CacheManager.Get(
                    "RussianPost_CityIndex_" +
                    (_preOrder.CityDest + "_" + _preOrder.DistrictDest + "_" + _preOrder.RegionDest).GetHashCode(),
                    60 * 24,
                    () =>
                    {
                        var pickPoints = PickPointsServices.Find(
                            _preOrder.RegionDest.RemoveTypeFromRegion(),
                            _preOrder.CityDest,
                            EnTypePoint.Ops);
                        if (pickPoints != null && pickPoints.Count > 0)
                        {
                            PickPointRussianPost pickPointRussianPost = null;
                            if (_preOrder.DistrictDest.IsNotEmpty())
                                pickPointRussianPost = pickPoints.FirstOrDefault(x =>
                                    x.Area.Contains(_preOrder.DistrictDest, StringComparison.OrdinalIgnoreCase));
                            if (pickPointRussianPost == null)
                                pickPointRussianPost = pickPoints.FirstOrDefault();

                            if (pickPointRussianPost != null)
                                return pickPointRussianPost.Id.ToString();
                        }
                        
                        var postOfficesCodes = _russianPostApiService.GetCityPostOfficesCodes(
                            settlement: _preOrder.CityDest,
                            region: _preOrder.RegionDest.RemoveTypeFromRegion(),
                            district: _preOrder.DistrictDest
                        );

                        if (postOfficesCodes != null && postOfficesCodes.Count > 0)
                        {
                            // С 9 начинаются офисы/постоматы партнеров, остальные являются индексами ОПС
                            return postOfficesCodes.FirstOrDefault(code => !code.StartsWith("9"));
                        }

                        return null;
                    });
            }
            return zip;
        }

        public static bool IsDeclareCategory(EnMailCategory mailCategory)
        {
            return mailCategory == EnMailCategory.WithDeclaredValue ||
                mailCategory == EnMailCategory.WithDeclaredValueAndCashOnDelivery ||
                mailCategory == EnMailCategory.WithDeclaredValueAndCompulsoryPayment ||
                mailCategory == EnMailCategory.CombinedWithDeclaredValue ||
                mailCategory == EnMailCategory.CombinedWithDeclaredValueAndCashOnDelivery;
        }
        
        public static bool IsCodCategory(EnMailCategory mailCategory)
        {
            return mailCategory == EnMailCategory.WithCompulsoryPayment ||
                   mailCategory == EnMailCategory.WithDeclaredValueAndCompulsoryPayment ||
                   mailCategory == EnMailCategory.WithDeclaredValueAndCashOnDelivery ||
                   mailCategory == EnMailCategory.CombinedWithDeclaredValueAndCashOnDelivery;
        }

        public static bool IsInternational(AvailableProduct availableProduct)
        {
            return availableProduct.ProductType.StartsWith("international_", StringComparison.OrdinalIgnoreCase) || availableProduct.MailType == EnMailType.SmallPacket;
        }

        public static bool IsPochtamatsTariff(EnMailType mailType, EnMailCategory mailCategory)
        {
            // тариф для постаматов
            return mailType == EnMailType.OnlineParcel &&
                   (mailCategory == EnMailCategory.CombinedOrdinary ||
                    mailCategory == EnMailCategory.CombinedWithDeclaredValue ||
                    mailCategory == EnMailCategory.CombinedWithDeclaredValueAndCashOnDelivery);
        }
        
        public async Task<List<DeliveryPoint>> GetAllPointsAsync()
        {
            List<DeliveryPoint> points = null;
            var pointsCacheKey = string.Format("RussianPostApi-{0}-delivery-point", _token);
            if (!CacheManager.TryGetValue(pointsCacheKey, out points))
            {
                points = await _russianPostApiService.GetDeliveryPointsAsync().ConfigureAwait(false);
                if (points != null)
                    CacheManager.Insert(pointsCacheKey, points, 60 * 24);
            }

            return points ?? new List<DeliveryPoint>();
        }

        private async Task<List<DeliveryPoint>> GetPointsCityAsync(string region, string city, string district, int weight, Dimension dimensions)
        {
            if (city.IsNullOrEmpty())
                return null;

            List<DeliveryPoint> points = await GetAllPointsAsync().ConfigureAwait(false);

            if (points != null && points.Count > 0)
            {
                var regionFind = (region ?? string.Empty).RemoveTypeFromRegion();

                var dimensionType = EnDimensionType.GetDimensionType(dimensions);

                points = points
                    // имеет адрес
                    .Where(x => x.Address != null && !x.Closed && !x.TemporaryClosed)
                    // проходит по весу
                    .Where(x => x.WeightLimit.HasValue == false || weight <= x.WeightLimit.Value)
                    // проходит по габаритам
                    .Where(x => x.DimensionLimit == null || dimensionType <= x.DimensionLimit)
                    // нужного региона
                    .Where(x => (regionFind.IsNullOrEmpty() || x.Address.Region.IsNotEmpty()) && x.Address.Region.IndexOf(regionFind, StringComparison.OrdinalIgnoreCase) != -1)
                    // нужного города
                    .Where(x => x.Address.Place.IsNotEmpty() && x.Address.Place.IndexOf(city, StringComparison.OrdinalIgnoreCase) != -1)
                    .ToList();

                if (district.IsNotEmpty() && points.Any(x => x.Address.Area.IsNotEmpty() && x.Address.Area.Contains(district, StringComparison.OrdinalIgnoreCase)))
                    points = points
                        // дофильтровываем по району
                        .Where(x => x.Address.Area.IsNotEmpty() && x.Address.Area.Contains(district, StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }

            return points;
        }

        private static List<RussianPostPoint> CastPoints(List<DeliveryPoint> points)
        {
            var result = new List<RussianPostPoint>();
            foreach (var point in points)
            {
                result.Add(new RussianPostPoint
                {
                    Id = point.Id,
                    Code = point.DeliveryPointIndex,
                    Address = point.Address.AddressFromStreet() + (point.BrandName.IsNotEmpty() ? string.Format(" ({0})", point.BrandName): null),
                    Description = point.WorkTime,
                    AddressComment = point.GettoDescription,
                    PointX = point.Latitude.TryParseFloat(),
                    PointY = point.Longitude.TryParseFloat(),
                    Cash = point.CashPayment,
                    Card = point.CardPayment
                });
            }
            return result;
        }

        private List<PickPointRussianPost> GetPochtamatsCity(string region, string city, string district, int weight, Dimension dimensions)
        {
            if (city.IsNullOrEmpty())
                return null;

            var points = PickPointsServices.Find(region, city, EnTypePoint.Aps);

            if (points != null && points.Count > 0)
            {
                var dimensionType = EnDimensionType.GetDimensionType(dimensions);
                var weightInKg = weight / 1000f;

                points = points
                    // имеет адрес
                    .Where(x => x.Address.IsNotEmpty())
                    // проходит по весу
                    .Where(x => x.WeightLimit.HasValue == false || weightInKg <= x.WeightLimit.Value)
                    // проходит по габаритам
                    .Where(x => x.DimensionLimit == null || dimensionType <= x.DimensionLimit)
                    .ToList();

                if (district.IsNotEmpty() && points.Any(x => x.Area.IsNotEmpty() && x.Area.Contains(district, StringComparison.OrdinalIgnoreCase)))
                    points = points
                        // дофильтровываем по району
                        .Where(x => x.Area.IsNotEmpty() && x.Area.Contains(district, StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }

            return points;
        }

        private List<PickPointRussianPost> GetOpsCity(string region, string city, string district, int weight, Dimension dimensions)
        {
            if (city.IsNullOrEmpty())
                return null;

            var points = PickPointsServices.Find(region, city, EnTypePoint.Ops);

            if (points != null && points.Count > 0)
            {
                var dimensionType = EnDimensionType.GetDimensionType(dimensions);
                var weightInKg = weight / 1000f;

                points = points
                    // имеет адрес
                    .Where(x => x.Address.IsNotEmpty())
                    // проходит по весу
                    .Where(x => x.WeightLimit.HasValue == false || weightInKg <= x.WeightLimit.Value)
                    // проходит по габаритам
                    .Where(x => x.DimensionLimit == null || dimensionType <= x.DimensionLimit)
                    .ToList();

                if (district.IsNotEmpty() && points.Any(x => x.Area.IsNotEmpty() && x.Area.Contains(district, StringComparison.OrdinalIgnoreCase)))
                    points = points
                        // дофильтровываем по району
                        .Where(x => x.Area.IsNotEmpty() && x.Area.Contains(district, StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }

            return points;
        }

        private static List<RussianPostPoint> CastPoints(List<PickPointRussianPost> points)
        {
            var result = new List<RussianPostPoint>();
            foreach (var point in points)
            {
                result.Add(new RussianPostPoint
                {
                    Id = point.Id,
                    Code = point.Id.ToString(),
                    Address = point.Address + (point.BrandName.IsNotEmpty() ? string.Format(" ({0})", point.BrandName): null),
                    Description = point.WorkTime,
                    AddressComment = point.AddressDescription,
                    PointX = point.Latitude,
                    PointY = point.Longitude,
                    Cash = point.Cash ?? false,
                    Card = point.Card ?? false
                });
            }
            return result;
        }

        private float GetDeliverySum(CalculateResponse deliveryCost, bool withInsurance = true, bool withFragile = true, bool withNotice = true)
        {
            return ((deliveryCost.TotalRateNoVat + deliveryCost.TotalVat) -
                (withInsurance || deliveryCost.InsuranceRate == null ? 0 : (deliveryCost.InsuranceRate.Rate + deliveryCost.InsuranceRate.Vat)) -
                (withFragile || deliveryCost.FragileRate == null ? 0 : (deliveryCost.FragileRate.Rate + deliveryCost.FragileRate.Vat)) -
                (withNotice || deliveryCost.NoticeRate == null ? 0 : (deliveryCost.NoticeRate.Rate + deliveryCost.NoticeRate.Vat))) / 100F;
        }

        private float GetDeliverySum(TariffApi.CalculateResponse deliveryCost)
        {
            return (deliveryCost.Paynds ?? 0F) / 100F;
        }

        #region IShippingWithBackgroundMaintenance

        public void ExecuteJob()
        {
            if ((_login.IsNotEmpty() && _password.IsNotEmpty()) || _token.IsNotEmpty())
            {
                SyncPickPoints(_russianPostApiService);
            }
        }

        public static void SyncPickPoints(RussianPostApiService russianPostApiService)
        {
            var lattDateSync = Configuration.SettingProvider.Items["RussianPostLastDatePickPointsSync"].TryParseDateTime(true);
            try
            {
               var currentDateTime = DateTime.UtcNow;

                if (!lattDateSync.HasValue || (currentDateTime - lattDateSync.Value.ToUniversalTime() > TimeSpan.FromHours(23)))
                {
                    // пишем в начале импорта, чтобы, если запустят в паралель еще
                    // то не прошло по условию времени последнего запуска
                    Configuration.SettingProvider.Items["RussianPostLastDatePickPointsSync"] = currentDateTime.ToString("O");

                    PickPointsServices.Sync(russianPostApiService);
                }
            }
            catch (Exception ex)
            {
                // возвращаем предыдущее заначение, чтобы при следующем запуске снова сработало
                Configuration.SettingProvider.Items["RussianPostLastDatePickPointsSync"] = lattDateSync.HasValue ? lattDateSync.Value.ToString("O") : null;
                Debug.Log.Warn(ex);
            }
        }

        #endregion

        #region Help

        public static EnMailType GetEnMailTypeByInt(int oldEnumIntValue)
        {
            switch (oldEnumIntValue)
            {
                case 0:
                    return EnMailType.PostalParcel;

                case 1:
                    return EnMailType.OnlineParcel;

                case 2:
                    return EnMailType.OnlineCourier;

                case 3:
                    return EnMailType.Ems;

                case 4:
                    return EnMailType.EmsOptimal;

                case 5:
                    return EnMailType.EmsRT;

                case 6:
                    return EnMailType.EmsTender;

                case 7:
                    return EnMailType.Letter;

                case 8:
                    return EnMailType.LetterClass1;

                case 9:
                    return EnMailType.Banderol;

                case 10:
                    return EnMailType.BusinessCourier;

                case 11:
                    return EnMailType.BusinessCourierExpress;

                case 12:
                    return EnMailType.ParcelClass1;

                case 13:
                    return EnMailType.BanderolClass1;

                case 14:
                    return EnMailType.VGPOClass1;

                case 15:
                    return EnMailType.SmallPacket;

                case 16:
                    return EnMailType.VSD;

                case 17:
                    return EnMailType.ECOM;

                default:
                    return null;
            }
        }

        public static EnMailCategory GetEnMailCategoryByInt(int oldEnumIntValue)
        {
            switch (oldEnumIntValue)
            {
                case 0:
                    return EnMailCategory.Simple;

                case 1:
                    return EnMailCategory.Ordinary;

                case 2:
                    return EnMailCategory.Ordered;

                case 3:
                    return EnMailCategory.WithDeclaredValue;

                case 4:
                    return EnMailCategory.WithDeclaredValueAndCashOnDelivery;

                case 5:
                    return EnMailCategory.WithCompulsoryPayment;

                case 6:
                    return EnMailCategory.WithDeclaredValueAndCompulsoryPayment;

                default:
                    return null;
            }
        }

        public static EnTransportType GetEnTransportTypeByInt(int oldEnumIntValue)
        {
            switch (oldEnumIntValue)
            {
                case 0:
                    return EnTransportType.Standard;

                case 1:
                    return EnTransportType.Surface;

                case 2:
                    return EnTransportType.Avia;

                case 3:
                    return EnTransportType.Combined;

                case 4:
                    return EnTransportType.Express;

                default:
                    return null;
            }
        }

        #endregion
    }

    public class RussianPostPoint : BaseShippingPoint
    {
        public string AddressComment { get; set; }
        public float PointX { get; set; }
        public float PointY { get; set; }
        
        public bool Cash { get; set; }
        public bool Card { get; set; }
    }

    public class DeliveryPeriod
    {
        public int? MaxDays { get; set; }
        public int? MinDays { get; set; }
    }

    public enum EnTypeNotification
    {
        /// <summary>
        /// Заказное
        /// </summary>
        [Localize("Заказное")]
        WithOrderOfNotice = 0,

        /// <summary>
        /// Простое
        /// </summary>
        [Localize("Простое")]
        WithSimpleNotice = 1,

        /// <summary>
        /// Электронное
        /// </summary>
        [Localize("Электронное")]
        WithElectronicNotice = 2,
    }

    [Obsolete]
    public enum EnTypeInsure
    {
        /// <summary>
        /// С объявленной ценностью и без
        /// </summary>
        [Localize("С объявленной ценностью и без")]
        Both = 2,

        /// <summary>
        /// С объявленной ценностью
        /// </summary>
        [Localize("С объявленной ценностью")]
        WithDeclaredValue = 0,

        /// <summary>
        /// Без объявленной ценности
        /// </summary>
        [Localize("Без объявленной ценности")]
        WithoutDeclaredValue = 1,
    }
}
