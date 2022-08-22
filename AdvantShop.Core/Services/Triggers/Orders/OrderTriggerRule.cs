using AdvantShop.Catalog;
using AdvantShop.Mails;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Triggers.Orders
{
    public abstract class OrderTriggerRule : TriggerRule
    {
        public override ETriggerObjectType ObjectType
        {
            get { return ETriggerObjectType.Order; }
        }
    }

    public class OrderCreatedTriggerRule : OrderTriggerRule
    {
        public override ETriggerEventType EventType
        {
            get { return ETriggerEventType.OrderCreated; }
        }

        public override string[] AvailableVariables
        {
            get
            {
                return new[]
                {
                    "#ORDER_ID#", "#NUMBER#", "#BILLING_LINK#", "#BILLING_SHORTLINK#", "#EMAIL#", "#CUSTOMERCONTACTS#", "#SHIPPINGMETHOD#",
                    "#PAYMENTTYPE#", "#ORDERTABLE#", "#CURRENTCURRENCYCODE#", "#TOTALPRICE#", "#COMMENTS#",
                    "#MANAGER_NAME#", "#ADDITIONALCUSTOMERFIELDS#", "#INN#", "#COMPANYNAME#",
                    "#FIRSTNAME#", "#LASTNAME#", "#CITY#", "#ADDRESS#", "#GENERATED_COUPON_CODE#"
                };
            }
        }

        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var order = (Order)triggerObject;

            _mail = OrderService.GetMailTemplate(order);
            
            return  _mail.FormatValue(value, coupon, triggerCouponCode);
        }

        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnNewOrder.ToString());
            if (mail != null)
                return new TriggerMailFormat(mail);

            return null;
        }
    }

    public class OrderStatusChangedTriggerRule : OrderTriggerRule
    {
        public override ETriggerEventType EventType
        {
            get { return ETriggerEventType.OrderStatusChanged; }
        }

        public override string[] AvailableVariables
        {
            get
            {
                return new[]
                {
                    "#NUMBER#", "#ORDERSTATUS#", "#STATUSCOMMENT#", "#ORDERTABLE#", "#TRACKNUMBER#", "#BILLING_LINK#", "#BILLING_SHORTLINK#",
                    "#MANAGER_NAME#", "#SHIPPINGMETHOD#", "#PAYMENTTYPE#", "#TOTALPRICE#",
                    "#FIRSTNAME#", "#LASTNAME#", "#CITY#", "#ADDRESS#", "#GENERATED_COUPON_CODE#"
                };
            }
        }


        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var order = (Order)triggerObject;

            _mail = new OrderStatusMailTemplate(order);

            return _mail.FormatValue(value, coupon, triggerCouponCode);
        }


        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnChangeOrderStatus.ToString());
            if (mail != null)
                return new TriggerMailFormat(mail);

            return null;
        }
    }

    public class OrderPayTriggerRule : OrderTriggerRule
    {
        public override ETriggerEventType EventType
        {
            get { return ETriggerEventType.OrderPaied; }
        }

        public override string[] AvailableVariables
        {
            get
            {
                return new[]
                {
                    "#ORDER_ID#", "#NUMBER#", "#STORE_NAME#", "#SUM#", "#MANAGER_NAME#",
                    "#SHIPPINGMETHOD#", "#PAYMENTTYPE#", "#FIRSTNAME#", "#LASTNAME#",
                    "#CITY#", "#ADDRESS#", "#GENERATED_COUPON_CODE#"
                };
            }
        }


        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var order = (Order)triggerObject;

            _mail = new PayOrderTemplate(order, true);

            return _mail.FormatValue(value, coupon, triggerCouponCode);
        }


        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnPayOrder.ToString());
            if (mail != null)
                return new TriggerMailFormat(mail);

            return null;
        }
    }
}
