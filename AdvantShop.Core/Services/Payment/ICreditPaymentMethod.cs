namespace AdvantShop.Payment
{
    public interface ICreditPaymentMethod
    {
        float MinimumPrice { get;  }
        float? MaximumPrice { get;  }
        float? GetFirstPayment(float finalPrice);
        int PaymentMethodId { get; }
        bool ActiveCreditPayment { get; }
        string CreditButtonTextInProductCard { get; }
        bool ShowCreditButtonInProductCard { get; }
    }
}