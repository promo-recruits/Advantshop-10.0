using AdvantShop.Orders;

namespace AdvantShop.Models.Checkout
{
    public class CheckoutModel
    {
        public CheckoutData CheckoutData { get; set; }
        public bool IsLanding { get; set; }
    }
}