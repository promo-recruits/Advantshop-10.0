namespace AdvantShop.Web.Admin.Models.Settings.Partners
{
    public class PartnersSettingsModel
    {
        public float DefaultRewardPercent { get; set; }
        public int PayoutMinCustomersCount { get; set; }
        public decimal PayoutMinBalance { get; set; }
        public decimal PayoutCommissionNaturalPerson { get; set; }
        public bool AutoApplyPartnerCoupon { get; set; }
        public bool EnableCaptchaInRegistrationPartners { get; set; }
    }
}
