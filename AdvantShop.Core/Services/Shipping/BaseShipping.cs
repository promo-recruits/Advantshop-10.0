using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Shipping
{
    public abstract class BaseShipping : IShipping
    {
        protected ShippingMethod _method;
        protected PreOrder _preOrder;
        protected List<PreOrderItem> _items;
        protected readonly float _totalPrice;

        protected BaseShipping()
        {
        }

        protected BaseShipping(ShippingMethod method, PreOrder preOrder)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
            _preOrder = preOrder;
        }

        protected BaseShipping(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items) : this(method, preOrder)
        {
            if (preOrder != null || items != null)
            {
                if (preOrder == null)
                    throw new ArgumentNullException("preOrder");
                if (items == null)
                    throw new ArgumentNullException("items");
            }
            _items = items ?? new List<PreOrderItem>();

            if (_preOrder != null)
            {
                if (_method.ShippingCurrency != null &&
                    _preOrder.Currency.Rate != _method.ShippingCurrency.Rate)
                {
                    _items.ForEach(item => item.Price = CurrencyService.ConvertCurrency(item.Price, _method.ShippingCurrency.Rate, _preOrder.Currency.Rate));
                }

                _totalPrice = _items.Sum(x => x.Price * x.Amount);
                _totalPrice -= _method.ShippingCurrency != null && _preOrder.Currency.Rate != _method.ShippingCurrency.Rate
                    ? CurrencyService.ConvertCurrency(_preOrder.TotalDiscount, _method.ShippingCurrency.Rate, _preOrder.Currency.Rate)
                    : _preOrder.TotalDiscount;
            }
        }

        public virtual bool CurrencyAllAvailable => false;

        public virtual string[] CurrencyIso3Available => null;

        protected void ReplaceGeo()
        {
            var cacheKey = "ShippingReplaceGeo-PreOrder_" + _method.ShippingType +
                "_" + (_preOrder.CountryDest ?? string.Empty).ToLower().GetHashCode() +
                "_" + (_preOrder.RegionDest ?? string.Empty).ToLower().GetHashCode() +
                "_" + (_preOrder.CityDest ?? string.Empty).ToLower().GetHashCode() +
                "_" + (_preOrder.DistrictDest ?? string.Empty).ToLower().GetHashCode();

            var replacedInfo = CacheManager.Get(cacheKey, 5, () =>
                {
                    var replacedPreOrder = new PreOrder();

                    string outCountry, outRegion, outDistrict, outCity, outZip;
                    var replaced = ShippingReplaceGeoService.ReplaceGeo(
                        _method.ShippingType,
                        _preOrder.CountryDest, _preOrder.RegionDest, _preOrder.DistrictDest, _preOrder.CityDest, _preOrder.ZipDest,
                        out outCountry, out outRegion, out outDistrict, out outCity, out outZip);

                    return replaced
                        ? new Tuple<PreOrder>(new PreOrder
                            {
                                CountryDest = outCountry,
                                RegionDest = outRegion,
                                DistrictDest = outDistrict,
                                CityDest = outCity,
                                ZipDest = outZip,
                            })
                        : new Tuple<PreOrder>(null);// Tuple, т.к. в кэш null не пишется, а нужно запоминать и отрицательный результат тоже
                });

            if (replacedInfo != null && replacedInfo.Item1 != null)
            {
                _preOrder.CountryDest = replacedInfo.Item1.CountryDest;
                _preOrder.RegionDest = replacedInfo.Item1.RegionDest;
                _preOrder.DistrictDest = replacedInfo.Item1.DistrictDest;
                _preOrder.CityDest = replacedInfo.Item1.CityDest;
                _preOrder.ZipDest = replacedInfo.Item1.ZipDest;
            }
        }

        protected virtual IEnumerable<BaseShippingOption> CalcOptions()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<BaseShippingOption> GetOptions()
        {
            ReplaceGeo();
            return CalcOptions();
        }
    }
}