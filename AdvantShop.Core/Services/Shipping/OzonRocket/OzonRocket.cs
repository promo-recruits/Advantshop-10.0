﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping.OzonRocket.Api;

namespace AdvantShop.Shipping.OzonRocket
{
    [ShippingKey("OzonRocket")]
    public class OzonRocket : BaseShippingWithCargo, IShippingSupportingPaymentCashOnDelivery, IShippingLazyData, IShippingSupportingTheHistoryOfMovement, IShippingSupportingSyncOfOrderStatus

    {
        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public const string KeyNameOzonRocketOrderIdInOrderAdditionalData = "OzonRocketOrderId";
        public const string KeyNameOzonRocketOrderPostingNumberInOrderAdditionalData = "OzonRocketOrderPostingNumber";
        public const string KeyNameOzonRocketOrderPostingIdInOrderAdditionalData = "OzonRocketOrderPostingId";
        public const string KeyNameOzonRocketLogisticOrderNumberInOrderAdditionalData = "OzonRocketLogisticOrderNumber";
        public const string KeyNameOzonRocketOrderIsCanceledInOrderAdditionalData = "OzonRocketOrderIsCanceled";

        public const long ObjectTypePickPoint = 52895552000;
        public const long ObjectTypePostamat = 4635044131000;
        
        #region Ctor

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly long _fromPlaceId;
        private readonly TypeViewPoints _typeViewPoints;
        private readonly bool _showAddressComment;
        private readonly string _yaMapsApiKey;
        private readonly bool _statusesSync;
        private readonly List<DeliveryType> _deliveryTypes;
        private readonly int _increaseDeliveryTime;

        private readonly string _ozonRocketWidgetToken;
        private readonly bool _ozonRocketWidgetShowDeliveryPrice;
        private readonly bool _ozonRocketWidgetShowDeliveryTime;

        private readonly bool _allowPartialDelivery;
        private readonly bool _allowUncovering;

        private readonly IOzonRocketClient _ozonRocketClient;

        public long FromPlaceId => _fromPlaceId;
        public bool AllowPartialDelivery => _allowPartialDelivery;
        public bool AllowUncovering => _allowUncovering;
        public IOzonRocketClient OzonRocketClient => _ozonRocketClient;

        public OzonRocket(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items) : base(method, preOrder, items)
        {
            _clientId = _method.Params.ElementOrDefault(OzonRocketTemplate.ClientId);
            _clientSecret = _method.Params.ElementOrDefault(OzonRocketTemplate.ClientSecret);
            _fromPlaceId = _method.Params.ElementOrDefault(OzonRocketTemplate.FromPlaceId).TryParseLong();
            _typeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(OzonRocketTemplate.TypeViewPoints).TryParseInt();
            _showAddressComment = method.Params.ElementOrDefault(OzonRocketTemplate.ShowAddressComment).TryParseBool();
            _yaMapsApiKey = _method.Params.ElementOrDefault(OzonRocketTemplate.YaMapsApiKey);
            _statusesSync = method.Params.ElementOrDefault(OzonRocketTemplate.StatusesSync).TryParseBool();
            _deliveryTypes = (method.Params.ElementOrDefault(OzonRocketTemplate.DeliveryTypes) ?? string.Empty).Split(",").Select(x => DeliveryType.Parse(x)).ToList();
            _increaseDeliveryTime = _method.ExtraDeliveryTime;
        
            _ozonRocketWidgetToken = _method.Params.ElementOrDefault(OzonRocketTemplate.OzonRocketWidgetToken);
            _ozonRocketWidgetShowDeliveryPrice = method.Params.ElementOrDefault(OzonRocketTemplate.OzonRocketWidgetShowDeliveryPrice, "True").TryParseBool();
            _ozonRocketWidgetShowDeliveryTime = method.Params.ElementOrDefault(OzonRocketTemplate.OzonRocketWidgetShowDeliveryTime, "True").TryParseBool();

            _allowPartialDelivery = method.Params.ElementOrDefault(OzonRocketTemplate.AllowPartialDelivery).TryParseBool();
            _allowUncovering = method.Params.ElementOrDefault(OzonRocketTemplate.AllowUncovering).TryParseBool();
    
            var statusesReference = method.Params.ElementOrDefault(OzonRocketTemplate.StatusesReference);
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

            _ozonRocketClient = Api.OzonRocketClient.Create(_clientId, _clientSecret);
        }

        #endregion

        #region IShippingSupportingSyncOfOrderStatus

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
            var postingNumbers = new Dictionary<string, Order>();
            string postingNumber;
            foreach (var order in orders)
            { 
                if ((postingNumber = OrderService.GetOrderAdditionalData(order.OrderID,
                    KeyNameOzonRocketOrderPostingNumberInOrderAdditionalData)).IsNotEmpty())
                    postingNumbers.Add(postingNumber, order);
                
                if (postingNumbers.Count >= 100)
                {
                    UpdateStatusOrders(postingNumbers);
                    postingNumbers.Clear();
                }
            }

            UpdateStatusOrders(postingNumbers);
        }

        private void UpdateStatusOrders(Dictionary<string, Order> postingNumbers)
        {
            if (postingNumbers.Count <= 0)
                return;

            var history = _ozonRocketClient.Tracking.ByPostingNumbersOrIds(postingNumbers.Keys.ToArray());

            if (history != null)
            {
                foreach (var trackingItem in history)
                {
                    if (postingNumbers.ContainsKey(trackingItem.PostingNumber))
                    {
                        var order = postingNumbers[trackingItem.PostingNumber];

                        var lastEvent = trackingItem.Events
                            .Where(x => x.EventId.HasValue)
                            .Where(x => x.Moment.HasValue)
                            .Where(x => 
                                StatusesReference.ContainsKey(x.EventId.Value) 
                                && StatusesReference[x.EventId.Value].HasValue)
                            .OrderByDescending(x => x.Moment.Value)
                            .FirstOrDefault();

                        if (lastEvent == null)
                            continue;

                        var ozonRocketOrderStatus = StatusesReference.ContainsKey(lastEvent.EventId.Value)
                            ? StatusesReference[lastEvent.EventId.Value]
                            : null;

                        if (ozonRocketOrderStatus.HasValue &&
                            order.OrderStatusId != ozonRocketOrderStatus.Value &&
                            OrderStatusService.GetOrderStatus(ozonRocketOrderStatus.Value) != null)
                        {
                            var lastOrderStatusHistory =
                                OrderStatusService.GetOrderStatusHistory(order.OrderID)
                                    .OrderByDescending(item => item.Date)
                                    .FirstOrDefault();

                            if (lastOrderStatusHistory == null ||
                                lastOrderStatusHistory.Date < lastEvent.Moment)
                            {
                                OrderStatusService.ChangeOrderStatus(order.OrderID,
                                    ozonRocketOrderStatus.Value, "Синхронизация статусов для Ozon Rocket");
                            }
                        }
                    }
                }
            }
        }

        #endregion Statuses

        #region IShippingSupportingTheHistoryOfMovement

        public bool ActiveHistoryOfMovement => true;

        public List<HistoryOfMovement> GetHistoryOfMovement(Order order)
        {
            var postingNumber = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameOzonRocketOrderPostingNumberInOrderAdditionalData);
            if (postingNumber.IsNotEmpty())
            {
                var tracking = _ozonRocketClient.Tracking.ByPostingNumber(postingNumber);
                if (tracking != null)
                {
                    var storageExpirationDate = tracking.TrackingHeader?.StorageExpirationDate;
                    var estimateTimeArrival = tracking.TrackingHeader?.EstimateTimeArrival;
                    return tracking.Items
                        .OrderByDescending(x => x.Moment)
                        .Select(status => new HistoryOfMovement()
                        {
                            Code = status.PlaceId,
                            Name = status.Action,
                            Date = status.Moment.HasValue ? status.Moment.Value.DateTime : default(DateTime),
                            Comment =
                                string.Join(
                                    ". ",
                                    new[]
                                    {
                                        estimateTimeArrival.HasValue
                                            ? "Ожидаемая дата прибытия " +
                                              estimateTimeArrival.Value.ToString(Configuration.SettingsMain
                                                  .ShortDateFormat)
                                            : string.Empty,
                                        storageExpirationDate.HasValue
                                            ? "Хранение до " +
                                              storageExpirationDate.Value.ToString(Configuration.SettingsMain
                                                  .ShortDateFormat)
                                            : string.Empty
                                    }.Where(x => x.IsNotEmpty()))
                        }).ToList();
                }
            }
            return null;
        }

        public PointInfo GetPointInfo(Order order)
        {
            if (order.OrderPickPoint == null 
                || order.OrderPickPoint.PickPointId.IsNullOrEmpty()
                || order.OrderPickPoint.AdditionalData.IsNullOrEmpty())
                return null;
            
            OzonRocketCalculateOption ozonRocketCalculateOption = null;

            try
            {
                ozonRocketCalculateOption =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<OzonRocketCalculateOption>(order.OrderPickPoint.AdditionalData);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }

            if (ozonRocketCalculateOption?.IsCourier == true)
                return null;

            var variants = _ozonRocketClient.Delivery.GetVariantsByIds(order.OrderPickPoint.PickPointId.TryParseLong());

            if (variants == null || variants.Count <= 0)
                return null;

            var variant = variants.First();

            if (variant.ObjectTypeId == 52895497000) // курьер
                return null;

            return new PointInfo()
            {
                Address = variant.Address,
                TimeWork = variant.WorkingHours != null
                    ? string.Join("<br>",
                        variant.WorkingHours.Select(x =>
                                $"{x.Date:M} {x.Periods.First().Min.Hours}:{x.Periods.First().Min.Minutes} - {x.Periods.Last().Max.Hours}:{x.Periods.Last().Max.Minutes}"))
                    : null,
                Comment = variant.HowToGet,
                Phone = variant.Phone
            };
        }

        #endregion

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var options = new List<BaseShippingOption>();

            if (_fromPlaceId == 0)
                return options;

            if (_preOrder.CityDest.IsNullOrEmpty()
                || _preOrder.RegionDest.IsNullOrEmpty())
                return options;

            var totalWeightInGrams = GetTotalWeight(1000);
            // if (totalWeightInGrams > 25_000)
            //     return options;

            var dimensions = GetDimensions();
            // if (dimensions[0] > 1500 ||
            //     dimensions[1] > 800 ||
            //     dimensions[2] > 800)
            //     return options;

            // if (_totalPrice > 250_000)
            //     return options;

            if (_deliveryTypes.Contains(DeliveryType.Postamat) || _deliveryTypes.Contains(DeliveryType.PickPoint))
                options.Add(GetOptionWithPoints(totalWeightInGrams, dimensions));
            if (_deliveryTypes.Contains(DeliveryType.Courier))
                options.Add(GetOptionCourier(totalWeightInGrams, dimensions));

            return options;
        }

        private BaseShippingOption GetOptionCourier(float totalWeightInGrams, float[] dimensions)
        {
            var variants = _ozonRocketClient.Delivery.GetVariantsByAddress(
                new GetDeliveryVariantsByAddressParams()
                {
                    DeliveryType = DeliveryType.Courier,
                    Address = $"{_preOrder.RegionDest}, {_preOrder.CityDest}",
                    Packages = new List<PackageByAddress>()
                    {
                        new PackageByAddress()
                        {
                            Count = 1,
                            Price = _totalPrice,
                            EstimatedPrice = _totalPrice,
                            Dimensions = new DimensionsPackageByAddress()
                            {
                                Weight = (int)totalWeightInGrams,
                                Height = (int)dimensions[2],
                                Width = (int)dimensions[1],
                                Length = (int)dimensions[0],
                            }
                        }
                    }
                });
            var variant = variants
                // принимает такой вес посылки
                .Where(x => !x.MinWeight.HasValue || totalWeightInGrams >= x.MinWeight)
                .Where(x => !x.MaxWeight.HasValue || totalWeightInGrams <= x.MaxWeight)
                // может принять посылку такой стоимости
                .Where(x => !x.MinPrice.HasValue || _totalPrice >= x.MinPrice)
                .Where(x => !x.MaxPrice.HasValue || _totalPrice <= x.MaxPrice)
                // принимает посылку таких габаритов
                .Where(x => !x.RestrictionHeight.HasValue || dimensions[2] <= x.RestrictionHeight)
                .Where(x => !x.RestrictionWidth.HasValue || dimensions[1] <= x.RestrictionWidth)
                .Where(x => !x.RestrictionLength.HasValue || dimensions[0] <= x.RestrictionLength)
                .FirstOrDefault();

            if (variant == null)
                return null;

            var calculateInformation = _ozonRocketClient.Delivery.Calculate(
                new GetDeliveryCalculateInformationParams()
                {
                    FromPlaceId = _fromPlaceId,
                    DestinationAddress = $"{_preOrder.RegionDest}, {_preOrder.CityDest}",
                    Packages = new List<PackageByAddress>()
                    {
                        new PackageByAddress()
                        {
                            Count = 1,
                            Price = _totalPrice,
                            EstimatedPrice = _totalPrice,
                            Dimensions = new DimensionsPackageByAddress()
                            {
                                Weight = (int)totalWeightInGrams,
                                Height = (int)dimensions[2],
                                Width = (int)dimensions[1],
                                Length = (int)dimensions[0],
                            }
                        }
                    },
                });

            var courierDeliveryInfo =
                calculateInformation?.DeliveryInfos
                    .FirstOrDefault(x => x.DeliveryType == DeliveryType.Courier);
            var shippingCost = courierDeliveryInfo?.Price;

            if (!shippingCost.HasValue)
                return null;
            
            var calculateOption = new OzonRocketCalculateOption()
            {
                FromPlaceId = _fromPlaceId,
                DeliveryVariantId = variant.Id,
                IsCourier = true
            };

            var deliveryTime = courierDeliveryInfo?.DeliveryTermInDays;
            // var deliveryTime = _ozonRocketClient.Delivery.GetDeliveryTimeInDays(
            //     new GetDeliveryTimeParams()
            //     {
            //         DeliveryVariantId = variant.Id,
            //         FromPlaceId = _fromPlaceId
            //     });
            
            return new OzonRocketOption(_method, _totalPrice)
            {
                Name = $"{_method.Name} (Курьер)",
                Rate = shippingCost.Value,
                DeliveryTime = deliveryTime.HasValue ? (deliveryTime + _increaseDeliveryTime) + " дн." : null,
                CalculateOption = calculateOption,
                IsAvailablePaymentCashOnDelivery = variant.CardPaymentAvailable || !variant.IsCashForbidden,
                HideAddressBlock = false
            };
        }

        private BaseShippingOption GetOptionWithPoints(float totalWeightInGrams, float[] dimensions)
        {
            var points = GetPoints(_preOrder.CityDest, _totalPrice, totalWeightInGrams, dimensions);

            if (points == null || points.Count == 0)
                return null;

            var allowedPickPoint = _deliveryTypes.Contains(DeliveryType.PickPoint);
            var allowedPostamat = _deliveryTypes.Contains(DeliveryType.Postamat);

            var pointsPickPoint =
                points
                    // Самовывоз или Постамат
                    .Where(x => 
                        (allowedPickPoint && x.ObjectTypeId == ObjectTypePickPoint) 
                        || (allowedPostamat && x.ObjectTypeId == ObjectTypePostamat))
                    .ToList();

            if (pointsPickPoint.Count > 0)
                return CreatePickPointOption(pointsPickPoint, totalWeightInGrams, dimensions, _preOrder.CityDest);

            return null;
        }

        private BaseShippingOption CreatePickPointOption(List<OzonRocketShippingPoint> pointsPickPoint, 
            float totalWeightInGrams, float[] dimensions, string city)
        {
            long selectedPickpointId = 0;
            if (_preOrder.ShippingOption != null &&
                _preOrder.ShippingOption.ShippingType ==
                ((ShippingKeyAttribute) typeof(OzonRocket).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First())
                .Value)
            {
                if (_preOrder.ShippingOption.GetType() == typeof(OzonRocketOption) &&
                    ((OzonRocketOption) _preOrder.ShippingOption).SelectedPoint != null)
                    selectedPickpointId = ((OzonRocketOption) _preOrder.ShippingOption).SelectedPoint.Code.TryParseLong();

                if (_preOrder.ShippingOption.GetType() == typeof(OzonRocketWidgetOption))
                    selectedPickpointId = ((OzonRocketWidgetOption)_preOrder.ShippingOption).PointId.TryParseLong();
                
                if (_preOrder.ShippingOption.GetType() == typeof(OzonRocketDeliveryMapOption))
                    selectedPickpointId = ((OzonRocketDeliveryMapOption)_preOrder.ShippingOption).PickpointId.TryParseLong();
            }

            var tempString = selectedPickpointId.ToString();
            var selectedPickpoint =
                selectedPickpointId != 0
                    ? pointsPickPoint.FirstOrDefault(x => x.Code == tempString)
                    : null;
            if (selectedPickpoint == null)
            {
                selectedPickpoint = pointsPickPoint[0];
                selectedPickpointId = selectedPickpoint.Code.TryParseLong();
            }

            var calculateInformation = _ozonRocketClient.Delivery.Calculate(
                new GetDeliveryCalculateInformationParams()
                {
                    FromPlaceId = _fromPlaceId,
                    DestinationAddress = $"{_preOrder.RegionDest}, {city}",
                    Packages = new List<PackageByAddress>()
                    {
                        new PackageByAddress()
                        {
                            Count = 1,
                            Price = _totalPrice,
                            EstimatedPrice = _totalPrice,
                            Dimensions = new DimensionsPackageByAddress()
                            {
                                Weight = (int)totalWeightInGrams,
                                Height = (int)dimensions[2],
                                Width = (int)dimensions[1],
                                Length = (int)dimensions[0],
                            }
                        }
                    },
                });

            var deliveryTypePoint =
                selectedPickpoint.ObjectTypeId == ObjectTypePickPoint
                    ? DeliveryType.PickPoint
                    : DeliveryType.Postamat;

            var pointDeliveryInfo =
                calculateInformation?.DeliveryInfos
                    .FirstOrDefault(x => x.DeliveryType == deliveryTypePoint);
            var shippingCost = pointDeliveryInfo?.Price;

            if (shippingCost.HasValue)
            {
                var calculateOption = new OzonRocketCalculateOption()
                {
                    FromPlaceId = _fromPlaceId,
                    DeliveryVariantId = selectedPickpointId,
                    IsCourier = false
                };

                var deliveryTime = pointDeliveryInfo?.DeliveryTermInDays;
                // var deliveryTime = _ozonRocketClient.Delivery.GetDeliveryTimeInDays(
                //     new GetDeliveryTimeParams()
                //     {
                //         DeliveryVariantId = selectedPickpointId,
                //         FromPlaceId = _fromPlaceId
                //     });
                
                BaseShippingOption shippingOption = null;

                if (_typeViewPoints == TypeViewPoints.YaWidget && _yaMapsApiKey.IsNotEmpty())
                {
                    // tempString = selectedPickpointId.ToString();
                    var option = new OzonRocketDeliveryMapOption(_method, _totalPrice)
                    {
                        Name = $"{_method.Name} (Самовывоз)",
                        Rate = shippingCost.Value,
                        DeliveryTime = deliveryTime.HasValue ? (deliveryTime + _increaseDeliveryTime) + " дн." : null,
                        CurrentPoints = pointsPickPoint,
                        // SelectedPoint = pointsPickPoint.FirstOrDefault(x => x.Code == tempString),
                        CalculateOption = calculateOption
                    };

                    SetMapData(option, _preOrder.CountryDest, _preOrder.RegionDest, _preOrder.DistrictDest, _preOrder.CityDest, totalWeightInGrams, dimensions, _totalPrice);

                    shippingOption = option;
                }
                else if (_typeViewPoints == TypeViewPoints.OzonRocketWidget && _ozonRocketWidgetToken.IsNotEmpty())
                {
                    var option = new OzonRocketWidgetOption(_method, _totalPrice)
                    {
                        Name = $"{_method.Name} (Самовывоз)",
                        Rate = shippingCost.Value,
                        DeliveryTime = deliveryTime.HasValue ? (deliveryTime + _increaseDeliveryTime) + " дн." : null,
                        CalculateOption = calculateOption,
                        CurrentPoints = pointsPickPoint
                    };

                    ConfigShiptorWidget(option, city, _totalPrice, totalWeightInGrams, dimensions);
                
                    shippingOption = option;
                }
                else
                {
                    // tempString = selectedPickpointId.ToString();
                    shippingOption = new OzonRocketOption(_method, _totalPrice)
                    {
                        Name = $"{_method.Name} (Самовывоз)",
                        Rate = shippingCost.Value,
                        DeliveryTime = deliveryTime.HasValue ? (deliveryTime + _increaseDeliveryTime) + " дн." : null,
                        ShippingPoints = pointsPickPoint,
                        // SelectedPoint = pointsPickPoint.FirstOrDefault(x => x.Code == tempString),
                        CalculateOption = calculateOption,
                        HideAddressBlock = true
                    };
                }
                shippingOption.IsAvailablePaymentCashOnDelivery = pointsPickPoint.Any(
                    point => (point.Cash || point.Card) && point.Code == selectedPickpointId.ToString());

                return shippingOption;
            }

            return null;
        }

        private void ConfigShiptorWidget(OzonRocketWidgetOption option, string city, float orderCost, float weight, float[] dimensions)
        {
            option.WidgetConfigData = new Dictionary<string, string>();
            
            option.WidgetConfigData.Add("token", _ozonRocketWidgetToken);
            option.WidgetConfigData.Add("defaultcity", city);

            if (!_deliveryTypes.Contains(DeliveryType.PickPoint))
                option.WidgetConfigData.Add("hidepvz", "true");
            if (!_deliveryTypes.Contains(DeliveryType.Postamat))
                option.WidgetConfigData.Add("hidepostamat", "true");
            
            option.WidgetConfigData.Add("packages", $"[[{(int)weight}, {(int)dimensions[1]}, {(int)dimensions[2]}, {(int)dimensions[0]}]]");
            option.WidgetConfigData.Add("fromplaceid", _fromPlaceId.ToString());
            option.WidgetConfigData.Add("showdeliveryprice", _ozonRocketWidgetShowDeliveryPrice.ToLowerString());
            option.WidgetConfigData.Add("showdeliverytime", _ozonRocketWidgetShowDeliveryTime.ToLowerString());

            if (_method.ExtrachargeInNumbers != 0f)
                option.WidgetConfigData.Add("deliverypricemarkupfix",
                    Repository.Currencies.CurrencyService.ConvertCurrency(
                            _method.ExtrachargeInNumbers,
                            _method.ShippingCurrency != null ? _method.ShippingCurrency.Rate : 1f, 1f)
                        .ToInvariantString());
            
            // Если указан deliverypricemarkupfix, deliverypricemarkuppercent не применяется.
            if (_method.ExtrachargeInPercents != 0f)
                option.WidgetConfigData.Add("deliverypricemarkuppercent", 
                    _method.ExtrachargeInPercents.ToInvariantString());
            
            if (_method.ExtraDeliveryTime != 0)
                option.WidgetConfigData.Add("deliverytimemarkup", _method.ExtraDeliveryTime.ToString());
        }

        private void SetMapData(OzonRocketDeliveryMapOption option, string country, string region, string district, string city,
            float weight, float[] dimensions, float totalPrice)
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
            option.MapParams = new Shipping.PointDelivery.MapParams();
            option.MapParams.Lang = lang;
            option.MapParams.YandexMapsApikey = _yaMapsApiKey;
            option.MapParams.Destination = string.Join(", ", new[] { country, region, district, city }.Where(x => x.IsNotEmpty()));

            option.PointParams = new Shipping.PointDelivery.PointParams();
            option.PointParams.IsLazyPoints = (option.CurrentPoints != null ? option.CurrentPoints.Count : 0) > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "city", city },
                    { "weight", weight },
                    { "totalPrice", totalPrice.ToInvariantString() },
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
        
        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("city") || data["city"] == null
                || !data.ContainsKey("totalPrice") || data["totalPrice"] == null
                || !data.ContainsKey("weight") || !data.ContainsKey("dimensions0")
                || !data.ContainsKey("dimensions1") || !data.ContainsKey("dimensions2"))
                return null;

            var points = GetPoints(
                (string)data["city"],
                data["totalPrice"].ToString().TryParseFloat(),
                data["weight"].ToString().TryParseFloat(),
                new[] { 
                    data["dimensions0"].ToString().TryParseFloat(),
                    data["dimensions1"].ToString().TryParseFloat(),
                    data["dimensions2"].ToString().TryParseFloat()
                });

            if (points == null || points.Count == 0)
                return null;

            var allowedPickPoint = _deliveryTypes.Contains(DeliveryType.PickPoint);
            var allowedPostamat = _deliveryTypes.Contains(DeliveryType.Postamat);

            var pointsPickPoint =
                points
                    // Самовывоз или Постамат
                    .Where(x => 
                        (allowedPickPoint && x.ObjectTypeId == ObjectTypePickPoint) 
                        || (allowedPostamat && x.ObjectTypeId == ObjectTypePostamat))
                    .ToList();

            return GetFeatureCollection(pointsPickPoint);
        }

        public Shipping.PointDelivery.FeatureCollection GetFeatureCollection(List<OzonRocketShippingPoint> points)
        {
            if (points == null)
                return null;

            return new Shipping.PointDelivery.FeatureCollection
            {
                Features = points.Select(p =>
                    new Shipping.PointDelivery.Feature
                    {
                        Id = p.Id,
                        Geometry = new Shipping.PointDelivery.PointGeometry { PointX = p.PointX, PointY = p.PointY },
                        Options = new Shipping.PointDelivery.PointOptions { Preset = "islands#dotIcon" },
                        Properties = new Shipping.PointDelivery.PointProperties
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

        public List<OzonRocketShippingPoint> GetPoints(string city, float totalPrice, float totalWeightInGrams, float[] dimensions)
        {
            return GetPointsCity(city)
                // с координатами
                ?.Where(x => x.Lat > 0f && x.Long > 0f)
                // принимает такой вес посылки
                .Where(x => !x.MinWeight.HasValue || totalWeightInGrams >= x.MinWeight)
                .Where(x => !x.MaxWeight.HasValue || totalWeightInGrams <= x.MaxWeight)
                // может принять посылку такой стоимости
                .Where(x => !x.MinPrice.HasValue || totalPrice >= x.MinPrice)
                .Where(x => !x.MaxPrice.HasValue || totalPrice <= x.MaxPrice)
                // принимает посылку таких габаритов
                .Where(x => !x.RestrictionHeight.HasValue || dimensions[2] <= x.RestrictionHeight)
                .Where(x => !x.RestrictionWidth.HasValue || dimensions[1] <= x.RestrictionWidth)
                .Where(x => !x.RestrictionLength.HasValue || dimensions[0] <= x.RestrictionLength)
                .OrderBy(x => x.Address)
                .Select(x => new OzonRocketShippingPoint()
                {
                    Id = x.Id.GetHashCode(),
                    Code = x.Id.ToString(),
                    ObjectTypeId = x.ObjectTypeId,
                    Address = x.Address,
                    Description = x.Description,
                    AddressComment = x.HowToGet,
                    Cash = !x.IsCashForbidden,
                    Card = x.CardPaymentAvailable,
                    PointX = x.Lat ?? 0f,
                    PointY = x.Long ?? 0f
                })
                .ToList();
        }

        public List<DeliveryVariant> GetPointsCity(string city) =>
            CacheManager.Get($"OzonRocket-Points-{city}", () => _ozonRocketClient.Delivery.GetVariants(
                new GetDeliveryVariantsParams() {CityName = city}));
    }
    
    public enum TypeViewPoints
    {
        [Localize("Списком")]
        List = 0,
        
        [Localize("Через Яндекс.Карты")]
        YaWidget = 1,
        
        [Localize("Через виджет Ozon Rocket")]
        OzonRocketWidget = 2,
    }

    // public enum TypeFrom
    // {
    //     [Localize("Со склада Ozon")]
    //     DropOff,
    //     
    //     [Localize("С забором заказов курьером Ozon")]
    //     PickUp
    // }
}