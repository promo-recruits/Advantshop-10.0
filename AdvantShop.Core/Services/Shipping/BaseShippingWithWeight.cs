using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Payment;

namespace AdvantShop.Shipping
{
    public abstract class BaseShippingWithWeight : BaseShipping
    {
        protected float _defaultWeight;
        protected ExtrachargeType _extrachargeTypeWeight { get; set; }
        protected float _extrachargeWeight { get; set; }


        protected BaseShippingWithWeight(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items) : base(method, preOrder, items)
        {
            _defaultWeight = _method.Params.ElementOrDefault(DefaultWeightParams.DefaultWeight).TryParseFloat();
            _extrachargeTypeWeight = (ExtrachargeType)_method.Params.ElementOrDefault(DefaultWeightParams.ExtrachargeTypeWeight).TryParseInt();
            _extrachargeWeight = _method.Params.ElementOrDefault(DefaultWeightParams.ExtrachargeWeight).TryParseFloat();
        }


        private bool CallDefaultIfNotSet;
        protected virtual void DefaultIfNotSet()
        {
            CallDefaultIfNotSet = true;

            var model = _items;
            foreach (var item in model)
            {
                item.Weight = item.Weight == 0 ? _defaultWeight : item.Weight;
            }

            if (_preOrder.TotalWeight == 0)
                _preOrder.TotalWeight = _defaultWeight;

            _items = model;
        }

        public float GetTotalWeight(int rate = 1)
        {
            if (!CallDefaultIfNotSet)
                DefaultIfNotSet();

            var weight =
                _preOrder.TotalWeight != null
                    ? _preOrder.TotalWeight.Value
                    : _items.Sum(x => x.Weight*x.Amount);

            return (weight + GetExtracharge(_extrachargeTypeWeight, _extrachargeWeight, weight)) * rate;
        }

        protected float GetExtracharge(ExtrachargeType extrachargeType, float extracharge, float value)
        {
            if (extrachargeType == ExtrachargeType.Fixed)
                return extracharge;

            return extracharge * value / 100;
        }


        public override IEnumerable<BaseShippingOption> GetOptions()
        {
            DefaultIfNotSet();
            ReplaceGeo();
            return CalcOptions();
        }
    }
}