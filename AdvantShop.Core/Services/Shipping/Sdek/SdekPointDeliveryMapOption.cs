//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Sdek
{
    public class SdekPointDeliveryMapOption : BaseShippingOption, PointDelivery.IPointDeliveryMapOption
    {
        public SdekPointDeliveryMapOption()
        {
        }

        public SdekPointDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            IsAvailablePaymentCashOnDelivery = true;
        }
        
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public BaseShippingPoint SelectedPoint { get; set; }
        public PointDelivery.MapParams MapParams { get; set; }
        public PointDelivery.PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }
        public string PickpointId { get; set; }
        [JsonIgnore]
        public List<SdekShippingPoint> CurrentPoints { get; set; }
        public int CityTo { get; set; }
        public SdekCalculateOption CalculateOption { get; set; }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as SdekPointDeliveryMapOption;
            if (opt != null && opt.Id == this.Id)
            {
                if (this.CityTo == opt.CityTo && this.CurrentPoints != null && this.CurrentPoints.Any(x => x.Code == opt.PickpointId))
                {
                    this.PickpointId = opt.PickpointId;
                    this.YaSelectedPoint = opt.YaSelectedPoint;
                    this.SelectedPoint = this.CurrentPoints.FirstOrDefault(x => x.Code == opt.PickpointId);
                    if (this.SelectedPoint != null)
                        this.IsAvailablePaymentCashOnDelivery &=
                            this.CurrentPoints.Any(x => x.AllowedCod && x.Code == opt.PickpointId);
                }
                else
                {
                    this.PickpointId = null;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PickpointId)
                ? new OrderPickPoint
                {
                    PickPointId = PickpointId,
                    PickPointAddress = SelectedPoint != null ? SelectedPoint.Address : null,
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
