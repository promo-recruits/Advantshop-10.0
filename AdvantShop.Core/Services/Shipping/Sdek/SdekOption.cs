using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Sdek
{
    public class SdekOption : BaseShippingOption
    {
        public string TariffId { get; set; }
        public BaseShippingPoint SelectedPoint { get; set; }

        public List<BaseShippingPoint> ShippingPoints { get; set; }
        public SdekCalculateOption CalculateOption { get; set; }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public SdekOption()
        {
        }

        public SdekOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            IsAvailablePaymentCashOnDelivery = true;
        }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/SdekOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as SdekOption;

            if (opt != null && opt.Id == this.Id && opt.ShippingPoints != null)
            {
                this.SelectedPoint = opt.SelectedPoint != null && this.ShippingPoints != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                this.SelectedPoint = this.SelectedPoint ?? opt.ShippingPoints.FirstOrDefault();
            }
        }
        public override OrderPickPoint GetOrderPickPoint()
        {
           var temp= new OrderPickPoint
            {
                PickPointId = SelectedPoint == null ? string.Empty : SelectedPoint.Code,
                PickPointAddress = SelectedPoint == null ? string.Empty : SelectedPoint.Address,
                AdditionalData = JsonConvert.SerializeObject(CalculateOption)
            };
            return temp;
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption != null && payOption.GetType() == typeof(CashOnDeliverytOption))
                Rate = PriceCash;
            else
                Rate = BasePrice;
            return true;
        }

        public override string GetDescriptionForPayment()
        {
            var diff = PriceCash - BasePrice;
            if (diff <= 0)
                return string.Empty;

            return string.Format("Стоимость доставки увеличится на {0}", diff.RoundPrice().FormatPrice());
        }
    }
}