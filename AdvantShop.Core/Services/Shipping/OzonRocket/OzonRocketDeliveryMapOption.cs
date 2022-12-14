using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.OzonRocket
{
    public class OzonRocketDeliveryMapOption : BaseShippingOption, Shipping.PointDelivery.IPointDeliveryMapOption
    {
        public OzonRocketDeliveryMapOption()
        {
        }

        public OzonRocketDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public BaseShippingPoint SelectedPoint { get; set; }
        public Shipping.PointDelivery.MapParams MapParams { get; set; }
        public Shipping.PointDelivery.PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }
        public string PickpointId { get; set; }
        [JsonIgnore]
        public List<OzonRocketShippingPoint> CurrentPoints { get; set; }
        public OzonRocketCalculateOption CalculateOption { get; set; }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as OzonRocketDeliveryMapOption;
            if (opt != null && opt.Id == this.Id)
            {
                if (this.CurrentPoints != null && this.CurrentPoints.Any(x => x.Code == opt.PickpointId))
                {
                    this.PickpointId = opt.PickpointId;
                    this.YaSelectedPoint = opt.YaSelectedPoint;
                    this.SelectedPoint = this.CurrentPoints.FirstOrDefault(x => x.Code == opt.PickpointId);
                    if (this.SelectedPoint != null)
                        this.IsAvailablePaymentCashOnDelivery &=
                            ((OzonRocketShippingPoint)this.SelectedPoint).Cash || ((OzonRocketShippingPoint)this.SelectedPoint).Card;
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
    }
}