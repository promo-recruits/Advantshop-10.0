using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Shipping.PointDelivery
{
    public class PointDeliveryOption : BaseShippingOption
    {
        public DeliveryPointShipping SelectedPoint { get; set; }
        public List<DeliveryPointShipping> ShippingPoints { get; set; }

        public PointDeliveryOption()
        {
        }

        public PointDeliveryOption(ShippingMethod method, float preCost) : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/PointDeliverySelectOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as PointDeliveryOption;
            if (opt != null && this.Id == opt.Id)
            {
                this.SelectedPoint = opt.SelectedPoint != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                this.SelectedPoint = this.SelectedPoint ?? opt.ShippingPoints.FirstOrDefault();
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return SelectedPoint != null
                ? new OrderPickPoint
                {
                    PickPointId = SelectedPoint.Id.ToString(),
                    PickPointAddress = SelectedPoint.Address,
                    AdditionalData = JsonConvert.SerializeObject(SelectedPoint)
                }
                : null;
        }

    }
}
