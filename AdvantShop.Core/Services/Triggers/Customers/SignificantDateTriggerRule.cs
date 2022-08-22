using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Triggers.Customers
{
    public class SignificantDateTriggerRule : TriggerRule
    {
        public override ETriggerObjectType ObjectType
        {
            get { return ETriggerObjectType.Customer; }
        }

        public override ETriggerEventType EventType
        {
            get { return ETriggerEventType.SignificantDate; }
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
            var triggerParams = (TriggerParamsDate) TriggerParams;

            var days = (triggerParams.Since == TriggerParamsDateSince.After ? 1 : -1) * (triggerParams.Days ?? 0);
            var triggerDate = new DateTime(triggerParams.DateTime.Year, triggerParams.DateTime.Month, triggerParams.DateTime.Day).AddDays(days);
            var now = DateTime.Now;
            var nowDate = new DateTime(now.Year, now.Month, now.Day);

            var customers = new List<Customer>();

            if (!triggerParams.IgnoreYear)
            {
                if (triggerDate == nowDate)
                    customers = CustomerService.GetCustomersByRoles(Role.User);
            }
            else
            {
                if (triggerDate.Day == nowDate.Day && triggerDate.Month == nowDate.Month)
                    customers = CustomerService.GetCustomersByRoles(Role.User);
            }

            return customers.Select(x => (ITriggerObject)x).ToList();
        }
    }
}
