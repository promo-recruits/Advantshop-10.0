using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.ViewModel.Checkout;
using AdvantShop.Repository.Currencies;
using AdvantShop.Configuration;

namespace AdvantShop.Handlers.Checkout
{
    public class CheckoutUserHandler
    {
        private readonly bool? _isLanding;

        public CheckoutUserHandler(bool? isLanding)
        {
            _isLanding = isLanding;
        }

        public CheckoutUserViewModel Execute()
        {
            var current = MyCheckout.Factory(CustomerContext.CustomerId);

            var model = new CheckoutUserViewModel()
            {
                Customer = CustomerContext.CurrentCustomer,
                Data = current.Data,
                Currency = CurrencyService.CurrentCurrency,
                IsLanding = _isLanding != null && _isLanding.Value,
            };

            if (BonusSystem.IsActive)
            {
                model.IsBonusSystemActive = true;
                model.BonusPlus = BonusSystem.BonusesForNewCard != 0
                    ? BonusSystem.BonusesForNewCard
                    : BonusSystemService.GetBonusCost(ShoppingCartService.CurrentShoppingCart).BonusPlus;
            }

            return model;
        }

    }
}