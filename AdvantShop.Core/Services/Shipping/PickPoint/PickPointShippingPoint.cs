namespace AdvantShop.Shipping.PickPoint
{
    public class PickPointShippingPoint : BaseShippingPoint
    {
        public float PointX { get; set; }
        public float PointY { get; set; }
        public bool Cash { get; set; }
        public bool Card { get; set; }
        //public bool PayPassAvailable { get; set; }
        public string AddressComment { get; set; }
    }
}
