//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Repository;
using System.Collections.Generic;

namespace AdvantShop.Shipping.ShippingByWeight
{
    [ShippingKey("ShippingByWeight")]
    public class ShippingByWeight : BaseShippingWithWeight, IShippingNoUseExtraDeliveryTime
    {

        private readonly float _pricePerKg;
        //private readonly float _extracharge;
        private readonly string _deliveryTime;

        public override bool CurrencyAllAvailable { get { return true; } }

        public ShippingByWeight(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items)
            : base(method, preOrder, items)
        {
            _pricePerKg = _method.Params.ElementOrDefault(ShippingByWeightTemplate.PricePerKg).TryParseFloat();
            //_extracharge = _method.Params.ElementOrDefault(ShippingByWeightTemplate.Extracharge).TryParseFloat();
            _deliveryTime = _method.Params.ElementOrDefault(ShippingByWeightTemplate.DeliveryTime);
        }

        private float GetRate(float weight, MeasureUnits.WeightUnit unit)
        {
            return GetRate(MeasureUnits.ConvertWeight(weight, unit, MeasureUnits.WeightUnit.Kilogramm));
        }

        private float GetRate(float weightInKg)
        {
            return (weightInKg * _pricePerKg) /*+ _extracharge*/;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions()
        {
            this.DefaultIfNotSet();

            var option = new BaseShippingOption(_method, _totalPrice)
            {
                Rate = GetRate(GetTotalWeight()),
                DeliveryTime = _deliveryTime
            };
            return new List<BaseShippingOption>() { option };
        }
    }
}