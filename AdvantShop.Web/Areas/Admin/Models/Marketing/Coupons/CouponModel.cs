using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Localization;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Marketing.Coupons
{
    public class CouponsFilterModel : BaseFilterModel
    {
        public string Code { get; set; }
        public CouponType? Type { get; set; }
        public string Value { get; set; }
        public string AddingDateFrom { get; set; }
        public string AddingDateTo { get; set; }
        public string StartDateFrom { get; set; }
        public string StartDateTo { get; set; }
        public string ExpirationDateFrom { get; set; }
        public string ExpirationDateTo { get; set; }
        public bool? Enabled { get; set; }
        public string MinimalOrderPrice { get; set; }
        public bool? ForFirstOrder { get; set; }

        public bool? WithTrigger { get; set; }
        public bool? PartnerCoupons { get; set; }
    }

    public class CouponModel : IValidatableObject
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        public CouponType Type { get; set; }

        public string TypeFormatted
        {
            get { return Type.Localize(); }
        }

        public float Value { get; set; }
        public string CurrencyIso3 { get; set; }
        public DateTime AddingDate { get; set; }

        public string AddingDateFormatted
        {
            get { return Culture.ConvertDate(AddingDate); }
        }

        public DateTime? StartDate { get; set; }

        public string StartDateFormatted
        {
            get
            {
                return StartDate != null
                    ? Culture.ConvertDate(StartDate.Value)
                    : LocalizationService.GetResource("Admin.Coupons.NoStartDate");
            }
        }

        public DateTime? ExpirationDate { get; set; }
        public string ExpirationDateFormatted
        {
            get
            {
                return ExpirationDate != null
                    ? Culture.ConvertDate(ExpirationDate.Value)
                    : LocalizationService.GetResource("Admin.Coupons.NoDate");
            }
        }

        public int PossibleUses { get; set; }
        public int ActualUses { get; set; }
        public bool Enabled { get; set; }
        public float MinimalOrderPrice { get; set; }
        public bool ForFirstOrder { get; set; }

        public List<int> CategoryIds { get; set; }
        public List<int> ProductsIds { get; set; }
        public int? TriggerActionId { get; set; }

        public string TriggerName
        {
            get { return Trigger != null ? Trigger.Name : null; }
        }

        private int? _triggerId;
        public int? TriggerId
        {
            get { return _triggerId ?? (_triggerId = Trigger != null ? Trigger.Id : default(int?)); }
            set { _triggerId = value; }
        }

        private TriggerRule _trigger;
        private TriggerRule Trigger
        {
            get
            {
                if (_trigger != null)
                    return _trigger;

                if (TriggerActionId == null)
                    return null;

                var triggerAction = TriggerActionService.GetTriggerAction(TriggerActionId.Value);
                if (triggerAction == null)
                    return null;

                return _trigger = TriggerRuleService.GetTrigger(triggerAction.TriggerRuleId);
            }
        }

        public CouponMode Mode { get; set; }
        public int? Days { get; set; }

        public int? PartnerId { get; set; }
        public string PartnerName { get; set; }
        public bool IsMinimalOrderPriceFromAllCart { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Code))
                yield return new ValidationResult("Неверный код купона");

            if (CouponId <= 0)
            {
                var coupon = CouponService.GetCouponByCode(Code);
                if (coupon != null)
                    yield return new ValidationResult("Код купона уже занят");
            }

            if (Value <= 0)
                yield return new ValidationResult("Неверное значение купона");

            if (Type == CouponType.Percent && Value > 100)
                yield return new ValidationResult("Значение купона не может быть больше 100");

            if (StartDate != null && ExpirationDate != null && (ExpirationDate.Value - StartDate.Value).TotalDays <= 0)
                yield return new ValidationResult("Неверные даты начала и окончания действия купона");
        }
    }
}
