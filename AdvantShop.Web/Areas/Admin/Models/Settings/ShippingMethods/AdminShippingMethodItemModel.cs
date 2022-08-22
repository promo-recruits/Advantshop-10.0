﻿namespace AdvantShop.Web.Admin.Models.Settings.ShippingMethods
{
    public class AdminShippingMethodItemModel
    {
        public int ShippingMethodId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public string Icon { get; set; }
        public string ShippingType { get; set; }
        public string WarningMessage { get; set; }
    }
}
