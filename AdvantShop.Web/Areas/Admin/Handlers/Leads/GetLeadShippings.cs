using System.Linq;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models.Orders;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class GetLeadShippings
    {
        private readonly int _leadId;
        private readonly string _country;
        private readonly string _region;
        private readonly string _district;
        private readonly string _city;
        private readonly string _zip;
        private readonly BaseShippingOption _shipping;
        private readonly bool _getAll;

        public GetLeadShippings(int leadId, string country, string city, string district, string region, string zip,
                                BaseShippingOption shipping = null, bool getAll = true)
        {
            _leadId = leadId;
            _country = country;
            _region = region;
            _district = district;
            _city = city;
            _zip = zip;
            _shipping = shipping;
            _getAll = getAll;
        }

        public OrderShippingsModel Execute()
        {
            var lead = LeadService.GetLead(_leadId);
            if (lead == null)
                return null;

            var model = new OrderShippingsModel();

            if (lead.LeadItems == null || lead.LeadItems.Count == 0)
                return model;
            
            var preOrder = new PreOrder
            {
                CountryDest = _country ?? "",
                RegionDest = _region ?? "",
                DistrictDest = _district ?? "",
                CityDest = _city ?? "",
                ZipDest = _zip ?? "",
                Currency = lead.LeadCurrency ?? CurrencyService.CurrentCurrency,
                ShippingOption = _shipping,
                TotalDiscount = lead.GetTotalDiscount(lead.LeadCurrency),
                IsFromAdminArea = true
            };
            var items = lead.LeadItems.Select(x => new PreOrderItem(x)).ToList();

            var manager = new ShippingManager(preOrder, items, lead.Sum - lead.ShippingCost);
#if !DEBUG
            manager.TimeLimitMilliseconds = 20_000; // 20 seconds
#endif
            model.Shippings = manager.GetOptions(_getAll);

            if (model.Shippings != null)
                model.Shippings.ForEach(item => item.ManualRate = item.FinalRate);

            model.CustomShipping = new BaseShippingOption()
            {
                Name = "",
                Rate = 0,
                IsCustom = true
            };

            return model;
        }
    }
}
