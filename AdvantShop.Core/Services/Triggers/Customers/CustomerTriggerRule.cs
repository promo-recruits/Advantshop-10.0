using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Triggers.Customers
{
    public abstract class CustomerTriggerRule : TriggerRule
    {
        public override ETriggerObjectType ObjectType
        {
            get { return ETriggerObjectType.Customer; }
        }
    }

    public class CustomerCreatedTriggerRule : CustomerTriggerRule
    {
        public override ETriggerEventType EventType
        {
            get { return ETriggerEventType.CustomerCreated; }
        }

        public override string[] AvailableVariables
        {
            get
            {
                return new[] { "#EMAIL#", "#FIRSTNAME#", "#LASTNAME#", "#PATRONYMIC#", "#PHONE#", "#REGDATE#", "#PASSWORD#", "#SUBSRCIBE#", "#SHOPURL#", "#GENERATED_COUPON_CODE#" };
            }
        }

        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var customer = (Customer)triggerObject;

            _mail = new RegistrationMailTemplate(customer);

            return _mail.FormatValue(value, coupon, triggerCouponCode);
        }

        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnRegistration.ToString());
            if (mail != null)
                return new TriggerMailFormat(mail);

            return null;
        }
    }
}
