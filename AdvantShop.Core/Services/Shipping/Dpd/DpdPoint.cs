using System.Runtime.Serialization;

namespace AdvantShop.Shipping.Dpd
{
    public class DpdPoint : BaseShippingPoint
    {
        public string Type;
        public string AddressComment { get; set; }
        public float PointX { get; set; }
        public float PointY { get; set; }
        [IgnoreDataMember]
        public string Services { get; set; }
        [IgnoreDataMember]
        public string ExtraServices;
    }
}
