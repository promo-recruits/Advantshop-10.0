﻿using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;

namespace AdvantShop.Shipping.ShippingYandexNewDelivery
{
    public class YandexNewDeliveryWidgetOption : BaseShippingOption
    {
        public string WidgetApiKey { get; set; }
        public int SenderId { get; set; }
        public string WarehouseId { get; set; }
        public string Weight { get; set; }
        public float TotalHeight { get; set; }
        public float TotalWidth { get; set; }
        public float TotalLength { get; set; }
        public string Cost { get; set; }

        public long? PickpointId { get; set; }
        public string ShippingInfo { get; set; }
        public string AdditionalData { get; set; }
        public int TariffId { get; set; }

        private string _company;
        public string Company
        {
            get => _company;
            set => _company = value;
        }

        public string DeliveryType { get; set; }
        
        public YandexNewDeliveryWidgetOption() { }
        public YandexNewDeliveryWidgetOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            IsAvailablePaymentCashOnDelivery = true;
            DeliveryType = "COURIER";
        }
        
        private string PostfixName =>
            !string.IsNullOrEmpty(Company) && (string.IsNullOrEmpty(base.Name) || !base.Name.EndsWith($" ({Company})"))
                ? $" ({Company})"
                : string.Empty;

        public new string Name
        {
            get => base.Name + PostfixName;
            set => base.Name =
                !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(PostfixName) && value.EndsWith(PostfixName)
                    ? value.Replace(PostfixName, string.Empty)
                    : value;
        }

        public override string Template => UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/YandexNewDeliveryWidgetOption.html";

        public override void Update(BaseShippingOption option)
        {
            var opt = option as YandexNewDeliveryWidgetOption;

            if (opt != null && opt.Id == this.Id)
            {
                this.PickpointId = opt.PickpointId;
                this.ShippingInfo = opt.ShippingInfo;
                this.AdditionalData = opt.AdditionalData;
                this.TariffId = opt.TariffId;
                // this.IsAvailablePaymentCashOnDelivery = opt.IsAvailablePaymentCashOnDelivery;
                this.HideAddressBlock = opt.TariffId.IsDefault() || opt.PickpointId.HasValue ? true : false;
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
                {
                    PickPointId = PickpointId.ToString(),
                    PickPointAddress = 
                        string.Format("{0}{1}{2}",
                            Company,
                            Company.IsNotEmpty() && ShippingInfo.IsNotEmpty() ? " " : string.Empty,
                            ShippingInfo),
                    AdditionalData = AdditionalData
                };
        }
    }
}
