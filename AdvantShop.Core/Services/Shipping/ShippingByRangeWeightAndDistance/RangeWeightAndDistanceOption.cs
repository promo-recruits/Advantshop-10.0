using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Shipping.RangeWeightAndDistanceOption
{
    public class RangeWeightAndDistanceOption : BaseShippingOption
    {
        public RangeWeightAndDistanceOption() { }
        public RangeWeightAndDistanceOption(ShippingMethod method, float preCost, bool useDistance)
            : base(method, preCost)
        {
            UseDistance = useDistance;
        }
        public float Distance { get; set; }
        public bool UseDistance { get; set; }
        public int MaxDistance { get; set; }

        public override string Template
        {
            get { return UrlService.GetUrl() + "scripts/_partials/shipping/extendTemplate/RangeWeightAndDistanceOption.html"; }
        }
    }
}