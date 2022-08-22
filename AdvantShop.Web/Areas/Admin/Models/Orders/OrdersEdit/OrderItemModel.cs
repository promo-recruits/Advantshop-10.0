namespace AdvantShop.Web.Admin.Models.Orders.OrdersEdit
{
    public class OrderItemModel
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public string ImageSrc { get; set; }
        public string ArtNo { get; set; }
        public string BarCode { get; set; }
        public string Name { get; set; }
        public string ProductLink { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
        public string PriceString => string.Format(Price%1 == 0 ? "{0:### ### ##0.##}" : "{0:### ### ##0.00##}", Price).Trim();

        public float Amount { get; set; }
        public bool Available { get; set; }
        public string AvailableText { get; set; }
        public string Cost { get; set; }
        public string CustomOptions { get; set; }
        public bool ShowEditCustomOptions { get; set; }

        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public bool Enabled { get; set; }
        public int? ProductId { get; set; }
    }
}
