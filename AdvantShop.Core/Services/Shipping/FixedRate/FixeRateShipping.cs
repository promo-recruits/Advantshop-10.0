//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.FixedRate
{
    [ShippingKey("FixedRate")]
    public class FixeRateShipping : BaseShipping, IShippingNoUseExtraDeliveryTime
    {
        private readonly float _shippingPrice;

        public override bool CurrencyAllAvailable { get { return true;} }

        public FixeRateShipping(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items)
            : base(method, preOrder, items)
        {
            _shippingPrice = method.Params.ElementOrDefault(FixeRateShippingTemplate.ShippingPrice).TryParseFloat();
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var option = new BaseShippingOption(_method, _totalPrice)
            {
                Rate = _shippingPrice,
                DeliveryTime = _method.Params.ElementOrDefault(FixeRateShippingTemplate.DeliveryTime)
            };
            return new List<BaseShippingOption> { option };
        }
    }
}