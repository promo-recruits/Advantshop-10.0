using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.PickPoint
{
    public class PickPointOption : BaseShippingOption
    {
        public PickPointShippingPoint SelectedPoint { get; set; }

        public List<PickPointShippingPoint> ShippingPoints { get; set; }

        public PickPointOption()
        {
        }

        public PickPointOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            IsAvailablePaymentCashOnDelivery = true;
        }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/PickPointOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as PickPointOption;

            if (opt != null && opt.Id == this.Id && opt.ShippingPoints != null)
            {
                this.SelectedPoint = opt.SelectedPoint != null && this.ShippingPoints != null ? this.ShippingPoints.FirstOrDefault(x => x.Code == opt.SelectedPoint.Code) : null;
                this.SelectedPoint = this.SelectedPoint ?? opt.ShippingPoints.FirstOrDefault();
                if (this.SelectedPoint != null)
                    this.IsAvailablePaymentCashOnDelivery &=
                        this.SelectedPoint.Cash || this.SelectedPoint.Card;
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            if (SelectedPoint == null) return null;
            var temp = new OrderPickPoint
            {
                PickPointId = SelectedPoint.Code,
                PickPointAddress = SelectedPoint.Address,
                AdditionalData = JsonConvert.SerializeObject(SelectedPoint)
            };
            return temp;
        }
    }
}
