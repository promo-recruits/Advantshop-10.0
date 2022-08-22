using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Webhook.Models.Api
{
    public class OrderStatusModel
    {
        public static OrderStatusModel FromOrderStatus(OrderStatus orderStatus)
        {
            if (orderStatus == null)
                return null;

            return new OrderStatusModel
            {
                Id = orderStatus.StatusID,
                Name = orderStatus.StatusName,
                IsCanceled = orderStatus.IsCanceled,
                IsCompleted = orderStatus.IsCompleted,
                Hidden = orderStatus.Hidden
            };
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsCompleted { get; set; }
        public bool Hidden { get; set; }
    }
}
