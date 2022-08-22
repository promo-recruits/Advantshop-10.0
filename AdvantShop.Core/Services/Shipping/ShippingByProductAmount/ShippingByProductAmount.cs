using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Shipping.ShippingByProductAmount
{
    [ShippingKey("ShippingByProductAmount")]
    public class ShippingByProductAmount : BaseShipping, IShippingNoUseExtraDeliveryTime
    {
        private readonly List<ShippingAmountRange> _priceRanges;

        public override bool CurrencyAllAvailable { get { return true; } }

        public ShippingByProductAmount(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items)
            : base(method, preOrder, items)
        {
            _priceRanges = GetRange();
            _priceRanges = _priceRanges.OrderBy(item => item.Amount).ToList();
        }

        private List<ShippingAmountRange> GetRange()
        {
            var priceRanges = new List<ShippingAmountRange>();

            var ranges = _method.Params.ElementOrDefault(ShippingByProductAmountTemplate.PriceRanges);
            if (ranges.IsNullOrEmpty())
                return priceRanges;
            
            foreach (var item in ranges.Split(';'))
            {
                var arr = item.Split('=');

                if (arr.Length != 2) continue;
                priceRanges.Add(new ShippingAmountRange()
                {
                    Amount = arr[0].TryParseFloat(),
                    ShippingPrice = arr[1].TryParseFloat()
                });
            }
            return priceRanges;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var rate = GetRate(_items.Sum(item => item.Amount));
            if (rate >= 0)
            {
                var option = new BaseShippingOption(_method, _totalPrice)
                {
                    Rate = rate,
                    DeliveryTime = _method.Params.ElementOrDefault(ShippingByProductAmountTemplate.DeliveryTime)
                };
                return new List<BaseShippingOption> { option };
            }

            return new List<BaseShippingOption>();
        }

        private float GetRate(float amount)
        {
            float shippingPrice = -1;
            foreach (var range in _priceRanges)
            {
                if (amount >= range.Amount)
                {
                    shippingPrice = range.ShippingPrice;
                }
            }
            return shippingPrice;
        }
    }
}