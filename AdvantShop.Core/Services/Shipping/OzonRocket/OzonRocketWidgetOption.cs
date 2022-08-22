using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.OzonRocket
{
    public class OzonRocketWidgetOption : BaseShippingOption
    {
        public OzonRocketWidgetOption()
        {
            HideAddressBlock = true;
        }

        public OzonRocketWidgetOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public Dictionary<string, string> WidgetConfigData { get; set; }
        public string PointId { get; set; }
        public string PickpointAddress { get; set; }
        public string PickpointAdditionalData { get; set; }
        
        public OzonRocketCalculateOption CalculateOption { get; set; }
        
        [JsonIgnore]
        public List<OzonRocketShippingPoint> CurrentPoints { get; set; }

        public override string Template
            => UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/OzonRocketWidgetOption.html";

        public override void Update(BaseShippingOption option)
        {
            if (option is OzonRocketWidgetOption opt && opt.Id == this.Id)
            {
                if (this.CurrentPoints == null || this.CurrentPoints.Any(x => x.Code == opt.PointId))
                {
                    this.PointId = opt.PointId;
                    this.PickpointAddress = opt.PickpointAddress;
                    this.PickpointAdditionalData = opt.PickpointAdditionalData;
                }
                else
                {
                    this.PointId = null;
                    this.PickpointAddress = null;
                    this.PickpointAdditionalData = null;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PointId)
                ? new OrderPickPoint
                {
                    PickPointId = PointId,
                    PickPointAddress = PickpointAddress ?? string.Empty,
                    AdditionalData = JsonConvert.SerializeObject(CalculateOption)
                }
                : null;
        }
    }
}