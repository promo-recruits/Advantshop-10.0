//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using System;

namespace AdvantShop.Shipping.SelfDelivery
{
    [ShippingKey("SelfDelivery")]
    public class SelfDelivery : BaseShipping, IShippingNoUseExtracharge, IShippingNoUseExtraDeliveryTime
    {
        private readonly float _shippingPrice;

        public override bool CurrencyAllAvailable { get { return true; } }

        public SelfDelivery(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items)
            : base(method, preOrder, items)
        {
            _shippingPrice = _method.Params.ElementOrDefault(SelfDeliveryTemplate.ShippingPrice,"").TryParseFloat();

        }

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var option = new BaseShippingOption(_method, _totalPrice)
            {
                DeliveryTime = _method.Params.ElementOrDefault(SelfDeliveryTemplate.DeliveryTime),
                Rate = _shippingPrice,
                HideAddressBlock = true
            };
            return new List<BaseShippingOption> { option };
        }     
    }
}