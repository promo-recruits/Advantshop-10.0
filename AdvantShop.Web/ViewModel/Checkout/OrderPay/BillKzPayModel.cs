using AdvantShop.Core.Common.Attributes;
using AdvantShop.Payment;

namespace AdvantShop.ViewModel.Checkout.OrderPay
{
    [PaymentOrderPayModel("BillKz")]
    public class BillKzPayModel: OrderPayModel
    {
        public string StampImageName => (PaymentMethod as BillKz)?.StampImageName;
        public override string ViewPath => "~/Views/Checkout/OrderPay/_BillKz.cshtml";
    }
}