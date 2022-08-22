using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;
using AdvantShop.Repository;
using AdvantShop.Shipping.Dpd.Api;
using AdvantShop.Shipping.Dpd.GeographyServices;

namespace AdvantShop.Shipping.Dpd
{
    [ShippingKey("Dpd")]
    public class Dpd : BaseShippingWithCargo, IShippingLazyData, IShippingWithBackgroundMaintenance, IShippingSupportingPaymentCashOnDelivery
    {
        private readonly long _clientNumber;
        private readonly string _clientKey;
        private readonly bool _testServers;
        private readonly string[] _serviceCodes;
        private readonly string _pickupCountryIso2;
        private readonly string _pickupRegionName;
        private readonly string _pickupCityName;
        private readonly long? _pickupCityId;
        private readonly string _pickupPointCode;
        private readonly bool _selfPickup;
        private readonly List<EnDeliveryType> _deliveryTypes;
        private readonly int _increaseDeliveryTime;
        private readonly TypeViewPoints _typeViewPoints;
        private readonly string _yaMapsApiKey;
        private readonly bool _withInsure;

        private readonly DpdApiService _dpdApi;
        public static readonly string[] EAEUCountriesIso2 = { "RU", "KZ", "BY", "AM", "KG" };

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public Dpd(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items) : base(method, preOrder, items)
        {
            _clientNumber = _method.Params.ElementOrDefault(DpdTemplate.ClientNumber).TryParseLong();
            _clientKey = _method.Params.ElementOrDefault(DpdTemplate.ClientKey);
            _testServers = method.Params.ElementOrDefault(DpdTemplate.TestServers).TryParseBool();
            _serviceCodes = (method.Params.ElementOrDefault(DpdTemplate.ServiceCodes) ?? string.Empty).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            _pickupCountryIso2 = _method.Params.ElementOrDefault(DpdTemplate.PickupCountryIso2);
            _pickupRegionName = _method.Params.ElementOrDefault(DpdTemplate.PickupRegionName);
            _pickupCityName = _method.Params.ElementOrDefault(DpdTemplate.PickupCityName);
            _pickupCityId = _method.Params.ElementOrDefault(DpdTemplate.PickupCityId).TryParseLong(true);
            _pickupPointCode = _method.Params.ElementOrDefault(DpdTemplate.PickupPointCode);
            _selfPickup = _method.Params.ElementOrDefault(DpdTemplate.SelfPickup).TryParseBool();
            _deliveryTypes = (method.Params.ElementOrDefault(DpdTemplate.DeliveryTypes) ?? string.Empty)
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (EnDeliveryType)x.TryParseInt())
                .ToList();
            _increaseDeliveryTime = _method.ExtraDeliveryTime;
            _yaMapsApiKey = _method.Params.ElementOrDefault(DpdTemplate.YaMapsApiKey);
            _typeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(DpdTemplate.TypeViewPoints).TryParseInt();
            _withInsure = method.Params.ElementOrDefault(DpdTemplate.WithInsure).TryParseBool();

            _dpdApi = new DpdApiService(_testServers, _clientNumber, _clientKey);
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var shippingOptions = new List<BaseShippingOption>();

            var city = _preOrder.CityDest;
            var region = _preOrder.RegionDest;
            var country = _preOrder.CountryDest;
            var index = _preOrder.ZipDest;
            var countryRepo = CountryService.GetCountryByName(country);
            var deliveryCountryIso2 = countryRepo != null ? countryRepo.Iso2 : null;

            // расчет доставки возможен из России или в Россию
            if (IsRussiaCountry(_pickupCountryIso2) || IsRussiaCountry(deliveryCountryIso2))
            {
                if (deliveryCountryIso2.IsNotEmpty() && _clientKey.IsNotEmpty() && city.IsNotEmpty() 
                    && _serviceCodes.Length > 0 && _deliveryTypes.Count > 0)
                {
                    var orderCost = _withInsure ? _totalPrice : (float?)null;
                    var weight = Math.Round(GetTotalWeight(), 2); // double для совместимости с api, чтобы не было коллизий
                    var dimensions = GetDimensions(rate: 10).Select(x => Math.Round(x, 2)).ToArray(); // double для совместимости с api, чтобы не было коллизий

                    string selectedPoint = null;
                    if (_preOrder.ShippingOption != null &&
                        _preOrder.ShippingOption.ShippingType == ((ShippingKeyAttribute)typeof(Dpd).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        //if (_preOrder.ShippingOption.GetType() == typeof(PecPointDeliveryMapOption))
                        //    selectedPoint = ((PecPointDeliveryMapOption)_preOrder.ShippingOption).PickpointId;

                        if (_preOrder.ShippingOption.GetType() == typeof(DpdOption) && ((DpdOption)_preOrder.ShippingOption).SelectedPoint != null)
                            selectedPoint = ((DpdOption)_preOrder.ShippingOption).SelectedPoint.Code;
                    }
                    //var deliveryPoint = deliveryPoints != null
                    //    ? deliveryPoints.FirstOrDefault(p => p.Code == selectedPoint)
                    //    : null;

                    //Api.Geography.address pickupPointAdress = FindPickupPointAdress();

                    if (IsEAEUCountry(deliveryCountryIso2) && IsEAEUCountry(_pickupCountryIso2))
                    {
                        // доставка внутри таможенного союза
                        //_dpdApi.GetServiceCostByParcels

                        var EAEUServiceCodes = _serviceCodes
                            .Where(x => !x.Equals("DPI", StringComparison.OrdinalIgnoreCase) &&
                                !x.Equals("DPE", StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (EAEUServiceCodes.Count > 0)
                        {
                            var pickup = GetEAEUPickup();
                            var delivery = GetEAEUDelivery(city, region, index, deliveryCountryIso2);

                            if (_deliveryTypes.Contains(EnDeliveryType.Pickpoint))
                            {
                                var resultsCalc = CalcInEAEUCountries(true, region, orderCost, weight, dimensions, EAEUServiceCodes, pickup, delivery);

                                if (resultsCalc != null)
                                {
                                    var deliveryPoints = GetPointsCity(deliveryCountryIso2, region, city, delivery.cityIdSpecified ? delivery.cityId : (long?)null, weight, dimensions);

                                    if (deliveryPoints.Count > 0)
                                    {
                                        foreach (var costService in resultsCalc.Where(x => x.cost > 0).OrderBy(x => x.cost))
                                        {
                                            var pointsCurrentService = deliveryPoints
                                                .Where(p => p.Services.Contains(costService.serviceCode))
                                                .ToList();

                                            if (pointsCurrentService.Count > 0)
                                            {
                                                var deliveryPoint = pointsCurrentService.FirstOrDefault(x => x.Code.Equals(selectedPoint, StringComparison.Ordinal));
                                                var rate = (float)costService.cost;

                                                if (_typeViewPoints == TypeViewPoints.List ||
                                                    (_typeViewPoints == TypeViewPoints.YaWidget && _yaMapsApiKey.IsNullOrEmpty()))
                                                {

                                                    var option = CreateOption(
                                                        name: string.Format("{0} {1} ({2})", _method.Name, EnDeliveryType.Pickpoint.Localize().ToLower(), costService.serviceName),
                                                        rate: rate,
                                                        priceCash: rate,
                                                        deliveryTime: costService.daysSpecified ? costService.days + _increaseDeliveryTime + " д." : null,
                                                        codAvailable: deliveryPoint != null && deliveryPoint.ExtraServices != null
                                                            ? deliveryPoint.ExtraServices.Contains("НПП")
                                                            : false,
                                                        points: pointsCurrentService,
                                                        selectedPoint: deliveryPoint,
                                                        calculateOption: new DpdCalculateOption { SelfPickup = _selfPickup, SelfDelivery = true, ServiceCode = costService.serviceCode },
                                                        hideAddressBlock: true,
                                                        deliveryId: costService.serviceCode.GetHashCode());

                                                    shippingOptions.Add(option);
                                                }
                                                else if (_typeViewPoints == TypeViewPoints.YaWidget)
                                                {
                                                    var option = CreatePointDeliveryMapOption(
                                                        name: string.Format("{0} {1} ({2})", _method.Name, EnDeliveryType.Pickpoint.Localize().ToLower(), costService.serviceName),
                                                        rate: rate,
                                                        priceCash: rate,
                                                        deliveryTime: costService.daysSpecified ? costService.days + _increaseDeliveryTime + " д." : null,
                                                        codAvailable: deliveryPoint != null && deliveryPoint.ExtraServices != null
                                                            ? deliveryPoint.ExtraServices.Contains("НПП")
                                                            : false,
                                                        points: pointsCurrentService,
                                                        selectedPoint: deliveryPoint,
                                                        calculateOption: new DpdCalculateOption { SelfPickup = _selfPickup, SelfDelivery = true, ServiceCode = costService.serviceCode },
                                                        hideAddressBlock: true,
                                                        deliveryId: costService.serviceCode.GetHashCode());

                                                    SetMapData(option, deliveryCountryIso2, country, region, city, delivery.cityIdSpecified ? delivery.cityId : (long?)null, weight, dimensions);

                                                    shippingOptions.Add(option);

                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (_deliveryTypes.Contains(EnDeliveryType.Courier))
                            {
                                var resultsCalc = CalcInEAEUCountries(false, region, orderCost, weight, dimensions, EAEUServiceCodes, pickup, delivery);

                                if (resultsCalc != null)
                                {
                                    var cashCities = CashCityService.Find(deliveryCountryIso2, region, city, delivery.cityIdSpecified ? delivery.cityId : (long?)null);

                                    foreach (var costService in resultsCalc.Where(x => x.cost > 0).OrderBy(x => x.cost))
                                    {
                                        var rate = (float)costService.cost;

                                        var option = CreateOption(
                                            name: string.Format("{0} {1} ({2})", _method.Name, EnDeliveryType.Courier.Localize().ToLower(), costService.serviceName),
                                            rate: rate,
                                            priceCash: rate,
                                            deliveryTime: costService.daysSpecified ? costService.days + _increaseDeliveryTime + " д." : null,
                                            codAvailable: cashCities.Count > 0,
                                            points: null,
                                            selectedPoint: null,
                                            calculateOption: new DpdCalculateOption { SelfPickup = _selfPickup, SelfDelivery = false, ServiceCode = costService.serviceCode },
                                            hideAddressBlock: false,
                                            deliveryId: costService.serviceCode.GetHashCode());

                                        shippingOptions.Add(option);
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        // международна доставка
                        // DPI: DPD CLASSIC international IMPORT - в Россию
                        // DPE	DPD CLASSIC international EXPORT - из России

                        //_dpdApi.GetServiceCostInternational

                        //selfPickup: IsRussiaCountry(_pickupCountryIso2),
                        //selfDelivery: IsRussiaCountry(countryIso2),

                        if (_serviceCodes.Contains("DPI", StringComparer.OrdinalIgnoreCase) ||
                        _serviceCodes.Contains("DPE", StringComparer.OrdinalIgnoreCase))
                        {
                            var pickupCountry = CountryService.GetCountryByIso2(_pickupCountryIso2);
                            if (pickupCountry != null)
                            {
                                var pickup = GetInternationalPickup(pickupCountry.Name);
                                var delivery = GetInternationalDelivery(city, region, country);

                                if (_deliveryTypes.Contains(EnDeliveryType.Pickpoint)
                                    // Самовывоз/Самопривоз можно ставить true только если страна отправления/назначения равна Россия.
                                    && IsRussiaCountry(deliveryCountryIso2))
                                {
                                    var resultsCalc = CalcInternational(true, region, orderCost, weight, dimensions, pickup, delivery);

                                    if (resultsCalc != null)
                                    {
                                        var deliveryPoints = GetPointsCity(deliveryCountryIso2, region, city, delivery.cityIdSpecified ? delivery.cityId : (long?)null, weight, dimensions);

                                        if (deliveryPoints.Count > 0)
                                        {
                                            foreach (var costService in resultsCalc.Where(x => x.cost > 0).OrderBy(x => x.cost))
                                            {
                                                var pointsCurrentService = deliveryPoints
                                                    .Where(p => p.Services.Contains(costService.serviceCode))
                                                    .ToList();

                                                if (pointsCurrentService.Count > 0)
                                                {
                                                    var deliveryPoint = pointsCurrentService.FirstOrDefault(x => x.Code.Equals(selectedPoint, StringComparison.Ordinal));
                                                    var rate = (float)costService.cost;

                                                    if (_typeViewPoints == TypeViewPoints.List ||
                                                        (_typeViewPoints == TypeViewPoints.YaWidget && _yaMapsApiKey.IsNullOrEmpty()))
                                                    {
                                                        var option = CreateOption(
                                                            name: string.Format("{0} {1} ({2})", _method.Name, EnDeliveryType.Pickpoint.Localize().ToLower(), costService.serviceName),
                                                            rate: rate,
                                                            priceCash: rate,
                                                            deliveryTime: costService.days.IsNotEmpty() ? costService.days + " д." : null,
                                                            codAvailable: deliveryPoint != null && deliveryPoint.ExtraServices != null
                                                                ? deliveryPoint.ExtraServices.Contains("НПП")
                                                                : false,
                                                            points: pointsCurrentService,
                                                            selectedPoint: deliveryPoint,
                                                            calculateOption: new DpdCalculateOption { SelfPickup = IsRussiaCountry(_pickupCountryIso2) && _selfPickup, SelfDelivery = true, ServiceCode = costService.serviceCode },
                                                            hideAddressBlock: true,
                                                            deliveryId: costService.serviceCode.GetHashCode());

                                                        shippingOptions.Add(option);
                                                    }
                                                    else if (_typeViewPoints == TypeViewPoints.YaWidget)
                                                    {
                                                        var option = CreatePointDeliveryMapOption(
                                                            name: string.Format("{0} {1} ({2})", _method.Name, EnDeliveryType.Pickpoint.Localize().ToLower(), costService.serviceName),
                                                            rate: rate,
                                                            priceCash: rate,
                                                            deliveryTime: costService.days.IsNotEmpty() ? costService.days + " д." : null,
                                                            codAvailable: deliveryPoint != null && deliveryPoint.ExtraServices != null
                                                                ? deliveryPoint.ExtraServices.Contains("НПП")
                                                                : false,
                                                            points: pointsCurrentService,
                                                            selectedPoint: deliveryPoint,
                                                            calculateOption: new DpdCalculateOption { SelfPickup = IsRussiaCountry(_pickupCountryIso2) && _selfPickup, SelfDelivery = true, ServiceCode = costService.serviceCode },
                                                            hideAddressBlock: true,
                                                            deliveryId: costService.serviceCode.GetHashCode());

                                                        SetMapData(option, deliveryCountryIso2, country, region, city, delivery.cityIdSpecified ? delivery.cityId : (long?)null, weight, dimensions);

                                                        shippingOptions.Add(option);

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (_deliveryTypes.Contains(EnDeliveryType.Courier))
                                {
                                    var resultsCalc = CalcInternational(false, region, orderCost, weight, dimensions, pickup, delivery);

                                    if (resultsCalc != null)
                                    {
                                        var cashCities = CashCityService.Find(deliveryCountryIso2, region, city, delivery.cityIdSpecified ? delivery.cityId : (long?)null);

                                        foreach (var costService in resultsCalc.Where(x => x.cost > 0).OrderBy(x => x.cost))
                                        {
                                            var rate = (float)costService.cost;

                                            var option = CreateOption(
                                                name: string.Format("{0} {1} ({2})", _method.Name, EnDeliveryType.Courier.Localize().ToLower(), costService.serviceName),
                                                rate: rate,
                                                priceCash: rate,
                                                deliveryTime: costService.days.IsNotEmpty() ? costService.days + " д." : null,
                                                codAvailable: cashCities.Count > 0,
                                                points: null,
                                                selectedPoint: null,
                                                calculateOption: new DpdCalculateOption { SelfPickup = IsRussiaCountry(_pickupCountryIso2) && _selfPickup, SelfDelivery = false, ServiceCode = costService.serviceCode },
                                                hideAddressBlock: false,
                                                deliveryId: costService.serviceCode.GetHashCode());

                                            shippingOptions.Add(option);
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }

            return shippingOptions;
        }

        private Api.Calculator.cityInternationalRequest GetInternationalDelivery(string city, string region, string country)
        {
            var delivery = new Api.Calculator.cityInternationalRequest { };
            var pickupCityId = GetInternationalCityIdFromCache(region, city);
            if (pickupCityId != null)
            {
                delivery.cityId = pickupCityId.Value;
                delivery.cityIdSpecified = true;
            }
            else
            {
                delivery.countryName = country;
                delivery.cityName = city;
            }

            return delivery;
        }

        private Api.Calculator.cityInternationalRequest GetInternationalPickup(string country)
        {
            var pickup = new Api.Calculator.cityInternationalRequest
            {
                countryName = country,
            };

            if (_pickupCityId.HasValue)
            {
                pickup.cityId = _pickupCityId.Value;
                pickup.cityIdSpecified = true;
            }
            else
            {
                var pickupCityId = GetInternationalCityIdFromCache(_pickupRegionName, _pickupCityName);
                if (pickupCityId != null)
                {
                    pickup.cityId = pickupCityId.Value;
                    pickup.cityIdSpecified = true;
                }
                else
                {
                    pickup.cityName = _pickupCityName;
                }
            }

            return pickup;
        }

        private Api.Calculator.cityRequest GetEAEUDelivery(string city, string region, string index, string deliveryCountryIso2)
        {
            var delivery = new Api.Calculator.cityRequest
            {
                countryCode = deliveryCountryIso2,
                index = index
            };
            var delivertyCityId = GetEAEUCityIdFromCache(region, city);
            if (delivertyCityId != null)
            {
                delivery.cityId = delivertyCityId.Value;
                delivery.cityIdSpecified = true;
            }
            else
            {
                delivery.cityName = city;
            }

            return delivery;
        }

        private Api.Calculator.cityRequest GetEAEUPickup()
        {
            var pickup = new Api.Calculator.cityRequest
            {
                countryCode = _pickupCountryIso2,
            };

            if (_pickupCityId.HasValue)
            {
                pickup.cityId = _pickupCityId.Value;
                pickup.cityIdSpecified = true;
            }
            else
            {
                var pickupCityId = GetEAEUCityIdFromCache(_pickupRegionName, _pickupCityName);
                if (pickupCityId != null)
                {
                    pickup.cityId = pickupCityId.Value;
                    pickup.cityIdSpecified = true;
                }
                else
                {
                    pickup.cityName = _pickupCityName;
                }
            }

            return pickup;
        }

        private Api.Calculator.serviceCostInternational[] CalcInternational(bool selfDelivery, string region, float? orderCost, double weight, double[] dimensions, 
            Api.Calculator.cityInternationalRequest pickup, Api.Calculator.cityInternationalRequest delivery, bool replay = false)
        {
            Api.Calculator.ServiceCostFault serviceCostFault;

            var resultCalc = _dpdApi.GetServiceCostInternational(
                pickup,
                delivery,
                selfPickup: IsRussiaCountry(_pickupCountryIso2) && _selfPickup,// Самовывоз/Самопривоз можно ставить true только если страна отправления/назначения равна Россия.
                selfDelivery: selfDelivery,
                weight: weight,
                length: (long)Math.Ceiling(dimensions[0]),
                width: (long)Math.Ceiling(dimensions[1]),
                height: (long)Math.Ceiling(dimensions[2]),
                declaredValue: orderCost,
                insurance: false,
                serviceCostFault: out serviceCostFault
                );

            if (serviceCostFault != null)
            {
                if (!replay //заходит только если это не повторная попытка расчета, иначе логируем
                    && string.Equals(serviceCostFault.code, "too-many-rows", StringComparison.OrdinalIgnoreCase))
                {
                    var pickupCityId = GetInternationalCityId(serviceCostFault.pickupDups, region, pickup.cityName);
                    if (pickupCityId != null)
                    {
                        pickup.cityId = pickupCityId.Value;
                        pickup.cityIdSpecified = true;
                    }

                    var deliveryCityId = GetInternationalCityId(serviceCostFault.deliveryDups, region, delivery.cityName);
                    if (deliveryCityId != null)
                    {
                        delivery.cityId = deliveryCityId.Value;
                        delivery.cityIdSpecified = true;
                    }

                    if (pickupCityId != null || deliveryCityId != null)
                    {
                        return CalcInternational(selfDelivery, region, orderCost, weight, dimensions, pickup, delivery, replay: true);
                    }
                }
                else
                    AdvantShop.Diagnostics.Debug.Log.Warn(string.Format("Dpd code: {0}, message: {1}", serviceCostFault.code, serviceCostFault.message));
            }

            return resultCalc;
        }

        private long? GetInternationalCityId(Api.Calculator.city[] dups, string region, string city)
        {
            if (dups != null)
            {
                var regionFind = region.RemoveTypeFromRegion();

                var dup = dups
                    .FirstOrDefault(x => x.regionName.Equals(regionFind, StringComparison.OrdinalIgnoreCase));

                if (dup != null)
                {
                    InsertInternationalCityId(dup.cityId, region, city);
                    return dup.cityId;
                }

            }

            return null;
        }

        private Api.Calculator.serviceCost[] CalcInEAEUCountries(bool selfDelivery, string region, float? orderCost, double weight, double[] dimensions, 
            List<string> EAEUServiceCodes, Api.Calculator.cityRequest pickup, Api.Calculator.cityRequest delivery, bool replay = false)
        {
            Api.Calculator.ServiceCostFault2 serviceCostFault2;

            var resultCalc = _dpdApi.GetServiceCostByParcels(
                pickup,
                delivery,
                selfPickup: _selfPickup,
                selfDelivery: selfDelivery,
                declaredValue: orderCost,
                serviceCodes: EAEUServiceCodes.ToArray(),
                parcels: new[] {
                    new Api.Calculator.parcelRequest
                    {
                        quantity = 1,
                        weight = weight,
                        height = dimensions[2],
                        width = dimensions[1],
                        length = dimensions[0]
                    }
                },
                serviceCostFault: out serviceCostFault2
                );

            if (serviceCostFault2 != null)
            {
                if (!replay //заходит только если это не повторная попытка расчета, иначе логируем
                    && string.Equals(serviceCostFault2.code, "too-many-rows", StringComparison.OrdinalIgnoreCase))
                {
                    var pickupCityId = GetEAEUCityId(serviceCostFault2.pickupDups, region, pickup.cityName);
                    if (pickupCityId != null)
                    {
                        pickup.cityId = pickupCityId.Value;
                        pickup.cityIdSpecified = true;
                    }

                    var deliveryCityId = GetEAEUCityId(serviceCostFault2.deliveryDups, region, delivery.cityName);
                    if (deliveryCityId != null)
                    {
                        delivery.cityId = deliveryCityId.Value;
                        delivery.cityIdSpecified = true;
                    }

                    if (pickupCityId != null || deliveryCityId != null)
                    {
                        return CalcInEAEUCountries(selfDelivery, region, orderCost, weight, dimensions, EAEUServiceCodes, pickup, delivery, replay: true);
                    }
                }
                else
                    AdvantShop.Diagnostics.Debug.Log.Warn(string.Format("Dpd code: {0}, message: {1}", serviceCostFault2.code, serviceCostFault2.message));
            }

            return resultCalc;
        }

        private long? GetEAEUCityId(Api.Calculator.cityIndex[] dups, string region, string city)
        {
            if (dups != null)
            {
                var regionFind = region.RemoveTypeFromRegion();

                var dup = dups
                    .OrderBy(x => string.Equals("г", x.abbreviation, StringComparison.OrdinalIgnoreCase) ? 0 : 1)// города в приоритете
                    .FirstOrDefault(x => x.regionName.Equals(regionFind, StringComparison.OrdinalIgnoreCase));

                if (dup != null)
                {
                    InsertEAEUCityId(dup.cityId, region, city);
                    return dup.cityId;
                }

            }

            return null;
        }

        private DpdOption CreateOption(string name, float rate, float priceCash, string deliveryTime, bool codAvailable,
            List<DpdPoint> points, DpdPoint selectedPoint, DpdCalculateOption calculateOption, bool hideAddressBlock,
            int deliveryId)
        {
            return new DpdOption(_method, _totalPrice)
            {
                Name = name,
                DeliveryId = deliveryId,
                Rate = rate,
                BasePrice = rate,
                PriceCash = priceCash,
                DeliveryTime = deliveryTime,
                IsAvailablePaymentCashOnDelivery = codAvailable,
                ShippingPoints = points,
                SelectedPoint = selectedPoint,
                CalculateOption = calculateOption,
                HideAddressBlock = hideAddressBlock
            };
        }

        private DpdPointDeliveryMapOption CreatePointDeliveryMapOption(string name, float rate, float priceCash, string deliveryTime, bool codAvailable,
            List<DpdPoint> points, DpdPoint selectedPoint, DpdCalculateOption calculateOption, bool hideAddressBlock,
            int deliveryId)
        {
            return new DpdPointDeliveryMapOption(_method, _totalPrice)
            {
                Name = name,
                DeliveryId = deliveryId,
                Rate = rate,
                BasePrice = rate,
                PriceCash = priceCash,
                DeliveryTime = deliveryTime,
                IsAvailablePaymentCashOnDelivery = codAvailable,
                CurrentPoints = points,
                SelectedPoint = selectedPoint,
                CalculateOption = calculateOption,
                HideAddressBlock = hideAddressBlock
            };
        }

        private void SetMapData(DpdPointDeliveryMapOption option, string countryIso2, string country, string region, string city, long? cityId, double weight, double[] dimensions)
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
                    { "countryIso2", countryIso2 },
                    { "region", region },
                    { "city", city },
                    { "cityId", cityId },
                    { "serviceCode", option.CalculateOption.ServiceCode },
                    { "weight", weight },
                    { "dimensionsH", dimensions[2] },
                    { "dimensionsW", dimensions[1] },
                    { "dimensionsL", dimensions[0] },
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("countryIso2") || 
                !data.ContainsKey("region") || !data.ContainsKey("city") || !data.ContainsKey("cityId") || 
                !data.ContainsKey("serviceCode") ||
                !data.ContainsKey("weight") || !data.ContainsKey("dimensionsH") ||
                !data.ContainsKey("dimensionsW") || !data.ContainsKey("dimensionsL"))
                return null;

            string countryIso2 = (string)data["countryIso2"];
            string region = (string)data["region"];
            string city = (string)data["city"];
            long? cityId = data["cityId"] == null ? null : data["cityId"].ToString().TryParseLong(true);
            string serviceCode = (string)data["serviceCode"];
            var weight = data["weight"].ToString().TryParseDouble();
            var dimensions = new double[]
            {
                data["dimensionsL"].ToString().TryParseDouble(),
                data["dimensionsW"].ToString().TryParseDouble(),
                data["dimensionsH"].ToString().TryParseDouble(),
            };

            var deliveryPoints = GetPointsCity(countryIso2, region, city, cityId, weight, dimensions);
            var pointsCurrentService = deliveryPoints
                .Where(p => p.Services.Contains(serviceCode))
                .ToList();

            return GetFeatureCollection(pointsCurrentService);
        }

        public PointDelivery.FeatureCollection GetFeatureCollection(List<DpdPoint> points)
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

        private List<DpdPoint> GetPointsCity(string countryIso2, string region, string city, long? cityId, double weight, double[] dimensions)
        {
            if (city.IsNullOrEmpty())
                return null;

            var terminals = TerminalsService.Find(countryIso2, region, city, cityId)
                // с координатами
                .Where(x => x.Latitude > 0d && x.Longitude > 0d)
                // выдает посылки
                .Where(x => x.IsSelfDelivery);

            var dimensionSum = dimensions.Sum();

            var parcelShops = ParcelShopsService.Find(countryIso2, region, city, cityId)
                // с координатами
                .Where(x => x.Latitude > 0d && x.Longitude > 0d)
                // выдает посылки
                .Where(x => x.IsSelfDelivery)
                // принимает такой вес посылки
                .Where(x => x.MaxWeight == null || weight <= x.MaxWeight)
                // принимает посылку таких габаритов
                .Where(x => x.DimensionSum == null || dimensionSum <= x.DimensionSum)
                .Where(x => x.MaxHeight == null || dimensions[2] <= x.MaxHeight)
                .Where(x => x.MaxWidth == null || dimensions[1] <= x.MaxWidth)
                .Where(x => x.MaxLength == null || dimensions[0] <= x.MaxLength);

            var pointsCity = new List<DpdPoint>();

            pointsCity.AddRange(
                terminals.Select(t => new DpdPoint
                {
                    Id = t.Code.GetHashCode(),
                    Code = t.Code,
                    Description = string.Format("{0} {1}",
                                "Терминал",
                                t.SelfDeliveryTimes),
                    PointX = (float)t.Latitude,
                    PointY = (float)t.Longitude,
                    Address = t.Address,
                    AddressComment = t.AddressDescription,
                    Services = t.Services,
                    ExtraServices = t.ExtraServices
                }));
            pointsCity.AddRange(
                parcelShops.Select(p => new DpdPoint
                {
                    Id = p.Code.GetHashCode(),
                    Code = p.Code,
                    Description = string.Format("{0} {1}",
                                p.Type,
                                p.SelfDeliveryTimes),
                    PointX = (float)p.Latitude,
                    PointY = (float)p.Longitude,
                    Address = p.Address,
                    AddressComment = p.AddressDescription,
                    Services = p.Services,
                    ExtraServices = p.ExtraServices
                }));

            return pointsCity
                .OrderBy(x => x.Address)
                .ToList();
        }

        #region IShippingWithBackgroundMaintenance

        public void ExecuteJob()
        {
            if (_clientNumber != 0 && _clientKey.IsNotEmpty())
            {
                SyncGeographyServices(_dpdApi);
            }
        }

        public static void SyncGeographyServices(DpdApiService dpdApiClient)
        {
            // общая настройка, т.к. справочники общие, не зависят от настроек
            var lattDateSync = Configuration.SettingProvider.Items["DpdLastDateServiceSync"].TryParseDateTime(true);
            try
            {
                var currentDateTime = DateTime.UtcNow;

                if (!lattDateSync.HasValue || (currentDateTime - lattDateSync.Value.ToUniversalTime() > TimeSpan.FromHours(23)))
                {
                    // пишем в начале импорта, чтобы, если запустят в паралель еще
                    // то не прошло по условию времени последнего запуска
                    Configuration.SettingProvider.Items["DpdLastDateServiceSync"] = currentDateTime.ToString("O");

                    TerminalsService.Sync(dpdApiClient);
                    CashCityService.Sync(dpdApiClient);
                    ParcelShopsService.Sync(dpdApiClient);

                    //ToDo: очистить кэш
                    CacheManager.RemoveByPattern("DpdTerminals-");
                    CacheManager.RemoveByPattern("DpdCashCities-");
                    CacheManager.RemoveByPattern("DpdParcelShops-");
                }
            }
            catch (Exception ex)
            {
                // возвращаем предыдущее заначение, чтобы при следующем запуске снова сработало
                Configuration.SettingProvider.Items["DpdLastDateServiceSync"] = lattDateSync.HasValue ? lattDateSync.Value.ToString("O") : null;
                Diagnostics.Debug.Log.Warn(ex);
            }
        }

        #endregion

        #region Help

        private bool IsEAEUCountry(string countryIso2)
        {
            return EAEUCountriesIso2.Contains(countryIso2, StringComparer.OrdinalIgnoreCase);
        }

        private bool IsRussiaCountry(string countryIso2)
        {
            return string.Equals("RU", countryIso2, StringComparison.OrdinalIgnoreCase);
        }

        private string formatCacheKeyEAEUCityId = "Dpd-EAEUCityId-{0}-{1}";
        private void InsertEAEUCityId(long cityId, string region, string city)
        {
            if (region.IsNotEmpty() && city.IsNotEmpty() && cityId > 0)
                CacheManager.Insert(string.Format(formatCacheKeyEAEUCityId, region.ToLower().GetHashCode(), city.ToLower().GetHashCode()), cityId, 60 * 24);
        }

        private long? GetEAEUCityIdFromCache(string region, string city)
        {
            return CacheManager.Get<long?>(string.Format(formatCacheKeyEAEUCityId, region.ToLower().GetHashCode(), city.ToLower().GetHashCode()));
        }

        private string formatCacheKeyInternationalCityId = "Dpd-InternationalCityId-{0}-{1}";
        private void InsertInternationalCityId(long cityId, string region, string city)
        {
            if (region.IsNotEmpty() && city.IsNotEmpty() && cityId > 0)
                CacheManager.Insert(string.Format(formatCacheKeyInternationalCityId, region.ToLower().GetHashCode(), city.ToLower().GetHashCode()), cityId, 60 * 24);
        }

        private long? GetInternationalCityIdFromCache(string region, string city)
        {
            return CacheManager.Get<long?>(string.Format(formatCacheKeyInternationalCityId, region.ToLower().GetHashCode(), city.ToLower().GetHashCode()));
        }

        #endregion
    }

    public enum EnDeliveryType
    {

        [Localize("До пункта выдачи")]
        Pickpoint = 1,

        [Localize("Курьером")]
        Courier = 2
    }

    public enum TypeViewPoints
    {
        [Localize("Через Яндекс.Карты")]
        YaWidget = 0,

        [Localize("Списком")]
        List = 1
    }
}
