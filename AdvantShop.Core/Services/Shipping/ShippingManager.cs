//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AdvantShop.Shipping
{  
    public class ShippingManager
    {
        private readonly PreOrder _preOrder;
        private readonly List<PreOrderItem> _items;
        private readonly bool _showOnlyInDetails;
        private readonly float _totalPrice;
        public int? TimeLimitMilliseconds { get; set; }

        public ShippingManager(PreOrder preOrder, List<PreOrderItem> items, float totalPrice)
        {
            _preOrder = preOrder;
            _items = items;
            _totalPrice = totalPrice.RoundPrice(CurrencyService.CurrentCurrency.Rate, CurrencyService.CurrentCurrency);
#if !DEBUG
            TimeLimitMilliseconds = 10_000; // 10 seconds
#endif
        }

        public ShippingManager(PreOrder preOrder, List<PreOrderItem> items, bool showOnlyInDetails, float totalPrice) : this(preOrder, items, totalPrice)
        {
            _showOnlyInDetails = showOnlyInDetails;
        }

        public List<BaseShippingOption> GetOptions(bool getAll = true)
        {
            var listMethods = ShippingMethodService.GetAllShippingMethods(true);
            var availableMethods = GetShippingMethodsByGeoMappingAndCatalog(listMethods);
            var context = HttpContext.Current;

            if (_showOnlyInDetails)
                availableMethods = availableMethods.Where(x => x.ShowInDetails);

            if (!getAll && _preOrder.ShippingOption != null)
                availableMethods = availableMethods.Where(x => x.ShippingMethodId == _preOrder.ShippingOption.MethodId);

            var header = context.Request.Headers["not_delete_fix"];

            var activeShippingModules = 
                availableMethods.Any(x => x.ModuleStringId.IsNotEmpty()) // for performance
                    ? AttachedModules.GetModules<IShippingMethod>()
                        .Where(module => module != null)
                        .Select(module => (IShippingMethod)Activator.CreateInstance(module))
                        .Select(module => module.ModuleStringId)
                        .ToList()
                    : null;

            var cancellationToken = new CancellationTokenSource();

            var tasks = availableMethods
                .Where(x => x.ModuleStringId.IsNullOrEmpty() ||
                    activeShippingModules.Contains(x.ModuleStringId))
                .Select(shippingMethod => Task.Run(() =>
                    {
                        try
                        {
                            using (cancellationToken.Token.Register(Thread.CurrentThread.Abort))
                            {
                                HttpContext.Current = context;
                                var type = ReflectionExt.GetTypeByAttributeValue<ShippingKeyAttribute>(typeof(BaseShipping), atr => atr.Value, shippingMethod.ShippingType);
                                //todo Пересмотреть передачу параметров, чтобы не требовалось DeepCloneJson
                        
                                var shipping = (BaseShipping)Activator.CreateInstance(type, shippingMethod, _preOrder.DeepCloneJson(Newtonsoft.Json.TypeNameHandling.All), _items.DeepCloneJson());
                                return shipping.GetOptions();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log.Error(ex);
                            return null;
                        }
                    },
                    cancellationToken.Token))
                .ToList();

            List<BaseShippingOption> items = null;

            if (TimeLimitMilliseconds.HasValue)
            {
                var delay = Task.Delay(TimeLimitMilliseconds.Value);
                Task.WhenAny(delay, Task.WhenAll(tasks)).Wait();
                cancellationToken.Cancel(true);
            }
            else
                Task.WhenAll(tasks).Wait();

            items = tasks
                .Where(x => x.IsCompleted)
                .Select(x => x.Result)
                .Where(x => x != null)
                .SelectMany(x => x)
                .Where(x => x != null)
                .ToList();
            

            var modules = AttachedModules.GetModules<IShippingCalculator>();
            foreach (var module in modules)
            {
                if (module != null)
                {
                    var classInstance = (IShippingCalculator)Activator.CreateInstance(module);
                    classInstance.ProcessOptions(items, _items, _totalPrice);
                }
            }

            ShippingMethod method;
            var orderedItems = items.OrderBy(option =>
                    (method = availableMethods.FirstOrDefault(ship => ship.ShippingMethodId == option.MethodId)) != null && method.MoveToEnd ? 1 : 0);

            if (SettingsCheckout.TypeSortOrderShippings == TypeSortOrderShippings.AscByRate)
                orderedItems = orderedItems.ThenBy(option => option.FinalRate);
            else if (SettingsCheckout.TypeSortOrderShippings == TypeSortOrderShippings.DescByRate)
                orderedItems = orderedItems.ThenByDescending(option => option.FinalRate);

            items = orderedItems.ToList();

            return items.All(o => (method = availableMethods.FirstOrDefault(ship => ship.ShippingMethodId == o.MethodId)) != null && method.ShowIfNoOtherShippings)
                // в списке только одни "Показывать только при отсутствии других доставок"
                ? items
                // в списке присутствуют доставки без флага "Показывать только при отсутствии других доставок"
                : items.Where(o => (method = availableMethods.FirstOrDefault(ship => ship.ShippingMethodId == o.MethodId)) == null || method.ShowIfNoOtherShippings == false).ToList();
        }


        private IEnumerable<ShippingMethod> GetShippingMethodsByGeoMappingAndCatalog(IEnumerable<ShippingMethod> listMethods)
        {
            var items = new List<ShippingMethod>();
            foreach (var shippingMethod in listMethods)
            {
                var validGeo = false;
                if (ShippingPaymentGeoMaping.IsExistGeoShipping(shippingMethod.ShippingMethodId))
                {
                    if (ShippingPaymentGeoMaping.CheckShippingEnabledGeo(shippingMethod.ShippingMethodId, _preOrder.CountryDest, _preOrder.RegionDest, _preOrder.CityDest, _preOrder.DistrictDest))
                        validGeo = true;
                }
                else
                    validGeo = true;

                var validCatalog = false;
                if (ShippingCatalogMaping.IsExistLinksToShipping(shippingMethod.ShippingMethodId))
                {
                    var productIds = _items
                        .Where(x => x.ProductId.HasValue)
                        .Select(x => x.ProductId.Value)
                        .Distinct()
                        .ToArray();

                    if (ShippingCatalogMaping.CheckShippingEnabled(shippingMethod.ShippingMethodId, productIds))
                        validCatalog = true;
                }
                else
                    validCatalog = true;

                if (validGeo && validCatalog)
                    items.Add(shippingMethod);
            }
            return items;
        }

        public override int GetHashCode()
        {
            return (_preOrder.CityDest ?? "").GetHashCode()
                   ^ (_preOrder.DistrictDest ?? "").GetHashCode()
                   ^ (_preOrder.CountryDest ?? "").GetHashCode()
                   ^ (_preOrder.RegionDest ?? "").GetHashCode()
                   ^ (_preOrder.AddressDest ?? "").GetHashCode()
                   ^ (_preOrder.ZipDest ?? "").GetHashCode();
        }
    }
}