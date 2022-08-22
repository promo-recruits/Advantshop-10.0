using AdvantShop.Payment;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class ChangePaymentHandler
    {
        private readonly ChangePaymentModel _model;

        public ChangePaymentHandler(ChangePaymentModel model)
        {
            _model = model;
            if (_model.Summary == null)
                _model.Summary = new BookingSummaryModel();
        }

        public BookingSummaryModel Execute()
        {
            if (_model.Payment != null)
            {
                _model.Summary.PaymentMethodId = _model.Payment.Id;
                _model.Summary.PaymentName = _model.Payment.Name ?? "";
                _model.Summary.PaymentCost = _model.Payment.Rate;
                _model.Summary.PaymentDetails = _model.Payment.GetDetails();

                var payment = PaymentService.GetPaymentMethod(_model.Payment.Id);
                _model.Summary.PaymentKey = payment != null ? payment.PaymentKey.ToLower() : null;
            }

            return _model.Summary;
        }
    }
}
