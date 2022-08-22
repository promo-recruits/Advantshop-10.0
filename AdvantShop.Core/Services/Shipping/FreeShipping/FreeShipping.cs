//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.FreeShipping
{
    public struct FreeShippingTemplate
    {
        public const string DeliveryTime = "DeliveryTime";
    }

    [ShippingKey("FreeShipping")]
    public class FreeShipping : BaseShipping, IShippingNoUseExtracharge, IShippingNoUseCurrency, IShippingNoUseExtraDeliveryTime
    {
        public FreeShipping(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items)
            : base(method, preOrder, items)
        {
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            var option = new BaseShippingOption(_method, _totalPrice)
            {
                Rate = 0F,
                DeliveryTime = _method.Params.ElementOrDefault(FreeShippingTemplate.DeliveryTime)
            };
            return new List<BaseShippingOption> { option };
        }
    }
}