using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core.Scheduler;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Triggers
{
    public enum ETriggerActionType
    {
        None = 0,
        Email = 1,
        Sms = 2,
        Task = 3,
        Edit = 4,
        SendRequest = 6,
        Message = 7
    }

    public class TriggerAction
    {
        public int Id { get; set; }

        public int TriggerRuleId { get; set; }

        public ETriggerActionType ActionType { get; set; }
        
        public TimeInterval TimeDelay { get; set; }

        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }

        public string SmsText { get; set; }
        public string MessageText { get; set; }

        public EditField EditField { get; set; }

        private List<Coupon> _coupons;
        public List<Coupon> Coupons
        {
            get
            {
                if (_coupons != null || Id == 0)
                    return _coupons;

                return _coupons = CouponService.GetCouponsByTriggerAction(Id);
            }
        }
        public Guid EmailingId { get; set; }

        public TriggerActionSendRequestData SendRequestData {get; set; }

        public int SortOrder { get; set; }
    }

    public class EditField
    {
        [JsonProperty(PropertyName = "type")]
        public int? Type { get; set; }

        [JsonProperty(PropertyName = "objId")]
        public int? ObjId { get; set; }
        public int? DealStatusId { get; set; }
        public string EditFieldValue { get; set; }
    }

}
