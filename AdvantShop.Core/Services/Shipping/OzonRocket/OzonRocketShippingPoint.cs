namespace AdvantShop.Shipping.OzonRocket
{
    public class OzonRocketShippingPoint : BaseShippingPoint
    {
        public long ObjectTypeId { get; set; }
        public float PointX { get; set; }
        public float PointY { get; set; }
        public bool Cash { get; set; }
        public bool Card { get; set; }
        public string AddressComment { get; set; }
    }
}