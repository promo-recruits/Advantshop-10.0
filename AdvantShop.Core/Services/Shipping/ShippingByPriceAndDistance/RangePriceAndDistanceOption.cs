using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Shipping.RangePriceAndDistanceOption
{
    public class RangePriceAndDistanceOption : BaseShippingOption
    {
        public RangePriceAndDistanceOption() { }
        public RangePriceAndDistanceOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
        }
        public float Distance { get; set; }
        public int MaxDistance { get; set; }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/RangePriceAndDistanceOption.html"; }
        }
    }
}