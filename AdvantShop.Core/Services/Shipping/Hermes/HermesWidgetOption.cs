using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Hermes
{
    public class HermesWidgetOption : BaseShippingOption
    {
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public string PickpointId { get; set; }
        public string PickpointAddress { get; set; }
        [JsonIgnore]
        public List<HermesPoint> CurrentPoints { get; set; }
        public HermesCalculateOption CalculateOption { get; set; }
        public Dictionary<string, object> WidgetConfigParams { get; set; }

        public HermesWidgetOption()
        {
        }

        public HermesWidgetOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/HermesWidgetOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as HermesWidgetOption;

            if (opt != null && opt.Id == this.Id)
            {
                if (this.CurrentPoints == null || this.CurrentPoints.Any(x => x.Code == opt.PickpointId))
                {
                    this.PickpointId = opt.PickpointId;
                    this.PickpointAddress = opt.PickpointAddress;
                }
                else
                {
                    this.PickpointId = null;
                    this.PickpointAddress = null;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PickpointId)
                ? new OrderPickPoint
                {
                    PickPointId = PickpointId,
                    PickPointAddress = PickpointAddress ?? string.Empty,
                    AdditionalData = JsonConvert.SerializeObject(CalculateOption)
                }
                : null;
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
