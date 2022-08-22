using System;
using System.Linq;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Customers;
using AdvantShop.Models.Checkout;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Handlers.Checkout
{
    public class GetCheckoutPage
    {
        public CheckoutModel Execute(ShoppingCart cart, int? lpId = null, int? lpUpId = null, CheckoutLpMode? mode = null)
        {
            var customer = CustomerContext.CurrentCustomer;
            var current = MyCheckout.Factory(customer.Id);

            UpdateLpCheckoutData(current, lpId, lpUpId, mode);
            
            if (customer.RegistredUser)
            {
                current.Data.User.Id = customer.Id;
                current.Data.User.Email = customer.EMail;
                current.Data.User.FirstName = customer.FirstName;
                current.Data.User.LastName = customer.LastName;
                current.Data.User.Patronymic = customer.Patronymic;
                current.Data.User.Phone = customer.Phone;
                current.Data.User.BirthDay = customer.BirthDay;
                current.Data.User.Organization = customer.Organization;

                var card = BonusSystemService.GetCard(customer.Id);
                current.Data.User.BonusCardId = card != null ? customer.Id : default(Guid?);

                current.Data.User.ManagerId = customer.ManagerId;
                current.Data.User.CustomerFields = CustomerFieldService.GetCustomerFieldsWithValue(customer.Id).Where(x => x.ShowInCheckout).ToList();
                current.Update();
            }

            var hashCode = ShoppingCartService.CurrentShoppingCart.GetHashCode();
            if (hashCode != current.Data.ShopCartHash)
            {
                current.Data.ShopCartHash = hashCode;
                current.Update();
            }

            var tagManager = GoogleTagManagerContext.Current;
            if (tagManager.Enabled)
            {
                tagManager.PageType = ePageType.order;
                tagManager.ProdIds = cart.Select(item => item.Offer.ArtNo).ToList();
                tagManager.Products = cart.Select(x => new TransactionProduct()
                {
                    Id = x.Offer.OfferId.ToString(),
                    SKU = x.Offer.ArtNo,
                    Category = x.Offer.Product.MainCategory != null ? x.Offer.Product.MainCategory.Name : string.Empty,
                    Name = x.Offer.Product.Name,
                    Price = x.Price,
                    Quantity = x.Amount
                }).ToList();
                tagManager.TotalValue = cart.TotalPrice;
            }

            var model = new CheckoutModel()
            {
                CheckoutData = current.Data,
                IsLanding = current.Data != null && current.Data.LpId != null
            };

            return model;
        }

        private void UpdateLpCheckoutData(MyCheckout current, int? lpId, int? lpUpId, CheckoutLpMode? mode)
        {
            var updateCurrent = false;

            if (lpId != null)
            {
                if (current.Data.LpId != lpId || current.Data.LpUpId != lpUpId)
                {
                    current.Data.LpId = lpId;
                    current.Data.LpUpId = lpUpId;
                    updateCurrent = true;
                }

                if (mode != null && mode == CheckoutLpMode.WithoutShipping)
                {
                    current.Data.HideShippig = true;
                    current.Data.SelectShipping = new BaseShippingOption()
                    {
                        Name = LocalizationService.GetResource("Checkout.GetCheckoutPage.ShippingName.WithoutShipping"),
                        IsCustom = true
                    };
                    updateCurrent = true;
                }
                else if (mode == null && current.Data.HideShippig)
                {
                    current.Data.HideShippig = false;
                    updateCurrent = true;
                }
            }
            else
            {
                if (current.Data.LpId != null || current.Data.LpUpId != null)
                {
                    current.Data.LpId = null;
                    current.Data.LpUpId = null;
                    updateCurrent = true;
                }
                if (current.Data.HideShippig)
                {
                    current.Data.HideShippig = false;
                    updateCurrent = true;
                }
            }

            if (updateCurrent)
                current.Update();
        }
    }
}