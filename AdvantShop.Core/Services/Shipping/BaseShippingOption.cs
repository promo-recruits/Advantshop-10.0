using AdvantShop.Orders;

namespace AdvantShop.Shipping
{
    public class BaseShippingOption : AbstractShippingOption
    {
        public BaseShippingOption()
        {
        }

        public BaseShippingOption(ShippingMethod method)
        {
            MethodId = method.ShippingMethodId;
            Name = method.Name;
            Desc = method.Description;
            DisplayCustomFields = method.DisplayCustomFields;
            DisplayIndex = method.DisplayIndex;
            IconName = method.IconFileName != null
                ? ShippingIcons.GetShippingIcon(method.ShippingType, method.IconFileName.PhotoName, method.Name)
                : null;
            ShowInDetails = method.ShowInDetails;
            ZeroPriceMessage = method.ZeroPriceMessage;
            TaxId = method.TaxId;
            PaymentMethodType = method.PaymentMethodType;
            PaymentSubjectType = method.PaymentSubjectType;
            ShippingType = method.ShippingType;
            ShippingCurrency = method.ShippingCurrency;

            if (method.UseExtracharge)
            {
                UseExtracharge = method.UseExtracharge;
                ExtrachargeInNumbers = method.ExtrachargeInNumbers;
                ExtrachargeInPercents = method.ExtrachargeInPercents;
                ExtrachargeFromOrder = method.ExtrachargeFromOrder;
            }

            if (method.UseExtraDeliveryTime)
            {
                ExtraDeliveryTime = method.ExtraDeliveryTime;
            }
        }

        public BaseShippingOption(ShippingMethod method, float preCost) : this (method)
        {
            PreCost = preCost;
        }

        public virtual void Update(BaseShippingOption option)
        {
           
        }

        public virtual OrderPickPoint GetOrderPickPoint()
        {
            return null;
        }

        public bool IsCustom { get; set; }
    }
}