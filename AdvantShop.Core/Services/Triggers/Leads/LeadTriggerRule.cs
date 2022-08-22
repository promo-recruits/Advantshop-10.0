using AdvantShop.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Triggers.Leads
{
    public abstract class LeadTriggerRule : TriggerRule
    {
        public override ETriggerObjectType ObjectType
        {
            get { return ETriggerObjectType.Lead; }
        }
    }

    public class LeadCreatedTriggerRule : LeadTriggerRule
    {
        public override ETriggerEventType EventType
        {
            get { return ETriggerEventType.LeadCreated; }
        }

        public override string[] AvailableVariables
        {
            get
            {
                return new[] {"#LEAD_ID#", "#NAME#", "#COMMENTS#", "#PHONE#", "#EMAIL#", "#ORDERTABLE#", "#STORE_NAME#", "#MANAGER_NAME#", "#GENERATED_COUPON_CODE#" };
            }
        }

        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var lead = (Lead)triggerObject;

            _mail = new LeadMailTemplate(lead);

            return _mail.FormatValue(value, coupon, triggerCouponCode);
        }

        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnLead.ToString());
            if (mail != null)
                return new TriggerMailFormat(mail);

            return null;
        }
    }


    public class LeadStatusChangedTriggerRule : LeadTriggerRule
    {
        public override ETriggerEventType EventType
        {
            get { return ETriggerEventType.LeadStatusChanged; }
        }

        public override string[] AvailableVariables
        {
            get
            {
                return new[]
                {
                    "#LEAD_ID#", "#NAME#", "#COMMENTS#", "#PHONE#", "#EMAIL#", "#ORDERTABLE#",
                    "#STORE_NAME#", "#MANAGER_NAME#", "#GENERATED_COUPON_CODE#"
                };
            }
        }

        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var lead = (Lead)triggerObject;

            _mail = new LeadMailTemplate(lead);     // нет шаблона на изменение статуса лида

            return _mail.FormatValue(value, coupon, triggerCouponCode);
        }

        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnLead.ToString());
            if (mail != null)
                return new TriggerMailFormat(mail);

            return null;
        }
    }

}
