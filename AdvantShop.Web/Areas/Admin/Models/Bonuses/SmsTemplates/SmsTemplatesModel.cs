﻿namespace AdvantShop.Web.Admin.Models.Bonuses.SmsTemplates
{
    public partial class SmsTemplatesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public decimal BonusPercent { get; set; }
        public decimal PurchaseBarrier { get; set; }
    }
}
