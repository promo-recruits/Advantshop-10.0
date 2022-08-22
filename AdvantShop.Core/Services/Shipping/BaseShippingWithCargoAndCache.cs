using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping
{
    public abstract class BaseShippingWithCargoAndCache : BaseShippingWithCargo
    {
        protected BaseShippingWithCargoAndCache(ShippingMethod method, PreOrder preOrder, List<PreOrderItem> items)
            : base(method, preOrder, items)
        {
        }

        protected virtual int GetHashForCache()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<BaseShippingOption> GetOptions()
        {
            ShippingCacheRepositiry.Delete();
            var hash = GetHashForCache();

            var cached = ShippingCacheRepositiry.Get(_method.ShippingMethodId, hash);
            if (cached != null)
                return cached.Options;

            DefaultIfNotSet();
            ReplaceGeo();
            var calcOptions = CalcOptions();
            IList<BaseShippingOption> options = calcOptions as IList<BaseShippingOption> ?? calcOptions?.ToList();

            var model = new ShippingCache
            {
                Options = options,
                ShippingMethodId = _method.ShippingMethodId,
                ParamHash = hash
            };


            if (ShippingCacheRepositiry.Exist(_method.ShippingMethodId, hash))
                ShippingCacheRepositiry.Update(model);
            else
                ShippingCacheRepositiry.Add(model);

            return options;
        }
    }
}