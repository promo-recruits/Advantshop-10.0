using AdvantShop.Core;
using AdvantShop.Core.Services.Webhook.Models.Api;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Orders
{
    public class GetOrder : ICommandHandler<int, OrderModel>
    {
        public OrderModel Execute(int id)
        {
            var order = OrderService.GetOrder(id);

            if (order == null || order.IsDraft)
                throw new BlException("Заказ не найден");

            return OrderModel.FromOrder(order);
        }
    }
}