﻿using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.CheckoutSettings
{
    public class TaxesFilterModel : BaseFilterModel<int>
    {                
        public string Name { get; set; }
        public bool? Enabled { get; set; }
        public float? Rate { get; set; }
        public bool? ShowInPrice { get; set; }
    }
}
