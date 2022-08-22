namespace AdvantShop.Shipping.Sdek
{
    public class SdekShippingPoint : BaseShippingPoint
    {
        public string AddressComment { get; set; }
        public float PointX { get; set; }
        public float PointY { get; set; }
        public bool AllowedCod { get; set; }
        public string Type { get; set; }
    }
}
