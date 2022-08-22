using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("Cash")]
    public class CashPaymentModel : PaymentMethodAdminModel
    {
        public override string PaymentViewPath { get { return null; } }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/cash-payment", "Инструкция. Способ оплаты Наличными"); }
        }
    }
}
