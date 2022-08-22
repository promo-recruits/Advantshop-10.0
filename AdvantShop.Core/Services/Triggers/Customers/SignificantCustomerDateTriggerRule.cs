using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Triggers.Customers
{
    public class SignificantCustomerDateTriggerRule : TriggerRule
    {
        public override ETriggerObjectType ObjectType
        {
            get { return ETriggerObjectType.Customer; }
        }

        public override ETriggerEventType EventType
        {
            get { return ETriggerEventType.SignificantCustomerDate; }
        }

        public override ETriggerProcessType ProcessType
        {
            get { return ETriggerProcessType.Datetime; }
        }

        public override string[] AvailableVariables
        {
            get
            {
                return new[] { "#EMAIL#", "#FIRSTNAME#", "#LASTNAME#", "#PATRONYMIC#", "#PHONE#", "#SUBSRCIBE#", "#SHOPURL#", "#GENERATED_COUPON_CODE#" };
            }
        }

        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var customer = (Customer)triggerObject;

            _mail = new RegistrationMailTemplate(customer);

            return  _mail.FormatValue(value, coupon, triggerCouponCode);
        }

        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            //var mail = MailFormatService.GetByType(MailType.OnNewOrder.ToString());
            //if (mail != null)
            //    return new TriggerMailFormat(mail);

            return null;
        }

        public override List<ITriggerObject> GeTriggerObjects()
        {
            var triggerParams = (TriggerParamsDaysBeforeDate)TriggerParams;

            var days = (triggerParams.Since == TriggerParamsDateSince.After ? -1 : 1) * triggerParams.Days;
            var date = DateTime.Now.AddDays(days);

            var customers = CustomerService.GetCustomersByTriggersDateParams(date, triggerParams.IgnoreYear, !triggerParams.IsCustomField, triggerParams.CustomFieldId.TryParseInt());

            return customers.Select(x => (ITriggerObject)x).ToList();
        }
    }
}
