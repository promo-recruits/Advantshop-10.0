using System;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Shipping
{
    [Serializable]
    public class PreOrder
    {
        public string CountryDest { get; set; }
        //public string CountryIso { get; set; }
        public string ZipDest { get; set; }
        public string CityDest { get; set; }
        public string DistrictDest { get; set; }
        public string RegionDest { get; set; }

        public BaseShippingOption ShippingOption { get; set; }
        public BasePaymentOption PaymentOption { get; set; }
        
        public Currency Currency { get; set; }

        public string AddressDest { get; set; }
        public Guid? BonusCardId { get; set; }
        public bool BonusUseIt { get; set; }
        public float TotalDiscount { get; set; }
        public bool IsFromAdminArea { get; set; }

        public float? TotalWeight { get; set; }
        public float? TotalLength { get; set; }
        public float? TotalWidth { get; set; }
        public float? TotalHeight { get; set; }

        // Не надо приведение к типу
        // При чтении кода это приведениее зачастую невидимо
        public static PreOrder CreateFromOrder(Order order)
        {
            return CreateFromOrder(order, false);
        }

        public static PreOrder CreateFromOrder(Order order, bool actualizeShippingAndPayment)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var preOrder = new PreOrder
            {
                CountryDest = order.OrderCustomer != null ? order.OrderCustomer.Country : null,
                ZipDest = order.OrderCustomer != null ? order.OrderCustomer.Zip : null,
                CityDest = order.OrderCustomer != null ? order.OrderCustomer.City : null,
                DistrictDest = order.OrderCustomer != null ? order.OrderCustomer.District : null,
                RegionDest = order.OrderCustomer != null ? order.OrderCustomer.Region : null,
                Currency = order.OrderCurrency,
                TotalDiscount = order.TotalDiscount,
                TotalWeight = order.TotalWeight,
                TotalLength = order.TotalLength,
                TotalWidth = order.TotalWidth,
                TotalHeight = order.TotalHeight
            };

            if (actualizeShippingAndPayment && order.ShippingMethod != null)
            {
                preOrder.ShippingOption = new BaseShippingOption(order.ShippingMethod, order.Sum - order.ShippingCostWithDiscount);

                preOrder.ShippingOption.Rate = order.ShippingCost;
                preOrder.ShippingOption.IsAvailablePaymentCashOnDelivery = order.AvailablePaymentCashOnDelivery;
                preOrder.ShippingOption.IsAvailablePaymentPickPoint = order.AvailablePaymentPickPoint;

                // сбрасываем настройки по наценке, т.к. order.ShippingCost 
                // содержит уже наценку и вызов FinalRate приведет к повтоной наценке
                preOrder.ShippingOption.UseExtracharge = default(bool);
                preOrder.ShippingOption.ExtrachargeInNumbers = default(float);
                preOrder.ShippingOption.ExtrachargeInPercents = default(float);
                preOrder.ShippingOption.ExtrachargeFromOrder = default(bool);

            }
            else
            {
                preOrder.ShippingOption = new BaseShippingOption();

                preOrder.ShippingOption.MethodId = order.ShippingMethodId;
                preOrder.ShippingOption.Name = order.ArchivedShippingName;
                preOrder.ShippingOption.Rate = order.ShippingCost;
                preOrder.ShippingOption.PreCost = order.Sum - order.ShippingCostWithDiscount;
                preOrder.ShippingOption.IsAvailablePaymentCashOnDelivery = order.AvailablePaymentCashOnDelivery;
                preOrder.ShippingOption.IsAvailablePaymentPickPoint = order.AvailablePaymentPickPoint;
            }

            if (actualizeShippingAndPayment && order.PaymentMethod != null)
            {
                preOrder.PaymentOption = order.PaymentMethod.GetOption(preOrder.ShippingOption, order.Sum - order.PaymentCost); // может вернуть null
            }

            if (preOrder.PaymentOption == null)
            {
                preOrder.PaymentOption = order.PaymentDetails == null || (!order.PaymentDetails.IsCashOnDeliveryPayment && !order.PaymentDetails.IsPickPointPayment)
                        ? new BasePaymentOption()
                        : order.PaymentDetails.IsCashOnDeliveryPayment
                            ? (BasePaymentOption)new CashOnDeliverytOption()
                            : new PickPointOption();

                preOrder.PaymentOption.Id = order.PaymentMethodId;
                preOrder.PaymentOption.Name = order.ArchivedPaymentName;
                preOrder.PaymentOption.Rate = order.PaymentCost;
            }

            return preOrder;
        }

        public override int GetHashCode()
        {
            ShippingOption = ShippingOption ?? new BaseShippingOption();
            PaymentOption = PaymentOption ?? new BasePaymentOption();

            return
                17
                ^ (CityDest ?? "").GetHashCode()
                ^ (DistrictDest ?? "").GetHashCode()
                ^ (CountryDest ?? "").GetHashCode()
                ^ (RegionDest ?? "").GetHashCode()
                ^ (AddressDest ?? "").GetHashCode()
                ^ (ZipDest ?? "").GetHashCode()
                ^ (Currency != null ? Currency.Iso3 : "").GetHashCode()
                ^ ShippingOption.Id.GetHashCode()
                ^ PaymentOption.Id.GetHashCode()
                ^ IsFromAdminArea.GetHashCode()
                ^ (TotalWeight ?? 0).GetHashCode()
                ^ (TotalLength ?? 0).GetHashCode()
                ^ (TotalWidth ?? 0).GetHashCode()
                ^ (TotalHeight ?? 0).GetHashCode();
        }
    }
}