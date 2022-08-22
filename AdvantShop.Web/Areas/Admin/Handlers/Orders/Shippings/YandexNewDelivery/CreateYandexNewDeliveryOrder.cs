using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.ActionResults;
using System;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Services.Orders;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.YandexNewDelivery
{
    public class CreateYandexNewDeliveryOrder
    {
        private readonly int _orderId;

        public CreateYandexNewDeliveryOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() { Error = "Order is null" };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != "YandexNewDelivery")
                return new CommandResult() { Error = "Order shipping method is not 'YandexNewDelivery' type" };

            try
            {
                var preOrder = PreOrder.CreateFromOrder(order);
                preOrder.IsFromAdminArea = true;
                
                var recalculatedOrderItems = new RecalculateOrderItemsToSum(order.OrderItems.CeilingAmountToInteger())
                {
                    AcceptableDifference = 0.1f
                }.ToSum(order.Sum - order.ShippingCost).ToList();
                var recalculatedPreOrderItems = recalculatedOrderItems.Select(x => new PreOrderItem(x)).ToList();
                
                var yandexNewDelivery = new Shipping.ShippingYandexNewDelivery.YandexNewDelivery(shippingMethod, preOrder, recalculatedPreOrderItems);
                var result = yandexNewDelivery.CreateOrder(order, recalculatedOrderItems);

                if (result)
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, shippingMethod.ShippingType);

                return result
                    ? new CommandResult() { Result = true, Message = "Черновик заказа успешно создан" }
                    : new CommandResult() { Error = "Не удалось создать черновик заказа" };
            }
            catch (BlException ex)
            {
                return new CommandResult() { Error = "Не удалось создать черновик заказа: " + ex.Message };
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = "Не удалось создать черновик заказа: " + ex.Message };
            }
        }
    }
}
