using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Webhook.Models.Api
{
    public class OrderItemModel
    {
        public static OrderItemModel FromOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                return null;

            return new OrderItemModel
            {
                ArtNo = orderItem.ArtNo,
                Name = orderItem.Name,
                Color = orderItem.Color,
                Size = orderItem.Size,
                Price = orderItem.Price,
                Amount = orderItem.Amount
            };
        }

        public string ArtNo { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
        public float Amount { get; set; }
    }
}
