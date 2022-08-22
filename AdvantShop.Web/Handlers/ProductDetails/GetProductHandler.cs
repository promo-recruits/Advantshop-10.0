using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Extensions;
using AdvantShop.Payment;
using AdvantShop.ViewModel.ProductDetails;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Handlers.ProductDetails
{
    public class GetProductHandler
    {
        private readonly Product _product;
        private readonly int? _color;
        private readonly int? _size;
        private readonly string _view;
        private readonly Discount _customDiscount;
        private readonly bool _hidePriceDiscount;

        public GetProductHandler(Product product, int? color, int? size, string view)
        {
            _product = product;
            _color = color;
            _size = size;
            _view = view;
        }

        public GetProductHandler(Product product, int? color, int? size, string view, Discount customDiscount) : this(product, color, size, view)
        {
            _customDiscount = customDiscount;
        }

        public GetProductHandler(Product product, int? color, int? size, string view, Discount customDiscount, bool hidePriceDiscount) : this(product, color, size, view, customDiscount)
        {
            _hidePriceDiscount = hidePriceDiscount;
        }

        public ProductDetailsViewModel Get()
        {
            var offer = OfferService.GetMainOffer(_product.Offers, _product.AllowPreOrder, _color, _size);
            var customer = CustomerContext.CurrentCustomer;

            var model = new ProductDetailsViewModel
            {
                IsAdmin = customer.IsAdmin,
                Product = _product,
                Offer = offer,
                ColorId = _color,
                SizeId = _size
            };

            var isAvailable = model.IsAvailable = offer != null && offer.Amount > 0;

            model.Availble = string.Format("{0}{1}",
                isAvailable ? LocalizationService.GetResource("Product.Available") : LocalizationService.GetResource("Product.NotAvailable"),
                isAvailable && SettingsCatalog.ShowStockAvailability
                    ? string.Format(
                        " (<div class=\"details-avalable-text inplace-offset inplace-rich-simple inplace-obj\" {1}>{0}</div><div class=\"details-avalable-unit inplace-offset inplace-rich-simple inplace-obj\" {3}>{2}</div>)",
                        offer.Amount,
                        InplaceExtensions.InplaceOfferAmount(offer.OfferId),
                        _product.Unit.IsNotEmpty() ? " " + _product.Unit : "",
                        _product.Unit.IsNotEmpty()
                            ? InplaceExtensions.InplaceProductUnit(offer.ProductId, ProductInplaceField.Unit).ToString()
                            : string.Empty)
                    : string.Empty);

            if (offer != null)
            {
                model.ShowAddButton = true;
                model.ShowPreOrderButton = _product.AllowPreOrder;
                model.ShowBuyOneClick = SettingsCheckout.BuyInOneClick;

                var optionsHandler = new GetProductCustomOptionsHandler(_product.ProductId, string.Empty);
                model.HasCustomOptions = optionsHandler.HasOptions;
                var customOptionsPrice = CustomOptionsService.GetCustomOptionPrice(offer.RoundedPrice, optionsHandler.GetXml(), _product.Currency.Rate);

                var price = (offer.RoundedPrice + customOptionsPrice).RoundPrice(CurrencyService.CurrentCurrency.Rate);

                model.FinalDiscount = PriceService.GetFinalDiscount(price, _customDiscount ?? _product.Discount, _product.Currency.Rate, customer.CustomerGroup, _product.ProductId);

                model.FinalPrice = PriceService.GetFinalPrice(price, model.FinalDiscount);
                model.PreparedPrice = _hidePriceDiscount
                    ? PriceFormatService.FormatPrice(model.FinalPrice, true, true)
                    : PriceFormatService.FormatPrice(price, model.FinalPrice, model.FinalDiscount, true, true);
                model.BonusPrice = GetBonusCardPrice(_product, model.FinalPrice);

                var currencyIso3 = CurrencyService.CurrentCurrency.Iso3;
                model.MicrodataOffers = new List<MicrodataOffer>();
                float? highPrice = null;
                float? lowPrice = null;

                foreach (var itemOffer in _product.Offers.OrderByDescending(x => x.OfferId == offer.OfferId))
                {
                    var offerPrice = PriceService.GetFinalPrice(itemOffer.RoundedPrice + customOptionsPrice, model.FinalDiscount);

                    if (highPrice == null || offerPrice > highPrice)
                    {
                        highPrice = offerPrice;
                    }
                    if (lowPrice == null || offerPrice < lowPrice)
                    {
                        lowPrice = offerPrice;
                    }

                    model.MicrodataOffers.Add(new MicrodataOffer()
                    {
                        Name = itemOffer.ArtNo,
                        Price = offerPrice.ToInvariantString(),
                        Available = (offerPrice > 0 || (offerPrice == 0 && model.FinalDiscount.HasValue)) && itemOffer.Amount > 0,
                        ColorId = itemOffer.ColorID,
                        SizeId = itemOffer.SizeID,
                        Currency = currencyIso3
                    });
                }

                model.MicrodataAggregateOffer = new MicrodataAggregateOffer
                {
                    HighPrice = highPrice.ToInvariantString(),
                    LowPrice = lowPrice.ToInvariantString(),
                    Currency = currencyIso3
                };

                if (SettingsDesign.ShowShippingsMethodsInDetails != SettingsDesign.eShowShippingsInDetails.Never)
                {
                    model.RenderShippings = true;
                    model.ShowShippingsMethods = SettingsDesign.ShowShippingsMethodsInDetails;
                }

                foreach (var creditPayment in PaymentService.GetCreditPaymentMethods())
                {
                    if (!creditPayment.ShowCreditButtonInProductCard)
                        continue;

                    var paymentMethod = creditPayment as PaymentMethod;
                    var finalPriceInPaymentCurrency =
                        model.FinalPrice
                            .ConvertCurrency(CurrencyService.CurrentCurrency,
                                paymentMethod.PaymentCurrency ?? CurrencyService.CurrentCurrency);

                    if (finalPriceInPaymentCurrency < creditPayment.MinimumPrice
                        || finalPriceInPaymentCurrency > creditPayment.MaximumPrice)
                        continue;

                    var firstPayment = creditPayment.GetFirstPayment(finalPriceInPaymentCurrency);
                    if (firstPayment.HasValue)
                    {
                        var firstPaymentInCurrentCurrency =
                            firstPayment.Value
                                .ConvertCurrency(
                                    paymentMethod.PaymentCurrency ?? CurrencyService.CurrentCurrency,
                                    CurrencyService.CurrentCurrency)
                                .RoundPrice();

                        model.ShowCreditButton = true;
                        model.CreditButtonText = creditPayment.CreditButtonTextInProductCard ?? LocalizationService.GetResource("Product.ProductInfo.BuyOnCredit");
                        model.FirstPaymentId = creditPayment.PaymentMethodId;
                        model.FirstPaymentMinPrice =
                            creditPayment.MinimumPrice.ConvertCurrency(
                                    paymentMethod.PaymentCurrency ?? CurrencyService.CurrentCurrency,
                                    CurrencyService.CurrentCurrency);
                        model.FirstPaymentMaxPrice = creditPayment.MaximumPrice.HasValue
                            ? creditPayment.MaximumPrice.Value.ConvertCurrency(
                                    paymentMethod.PaymentCurrency ?? CurrencyService.CurrentCurrency,
                                    CurrencyService.CurrentCurrency)
                            : (float?)null;
                        model.FirstPaymentPrice = firstPaymentInCurrentCurrency > 0
                            ? firstPaymentInCurrentCurrency.FormatPrice(true, false) + "*"
                            : LocalizationService.GetResource("Product.WithoutFirstPayment");

                        break;
                    }
                }
            }

            model.ProductProperties = PropertyService.GetPropertyValuesByProductId(_product.ProductId);
            model.BriefProperties = model.ProductProperties.Where(prop => prop.Property.UseInBrief)
                                        .GroupBy(x => new { x.PropertyId })
                                        .Select(x => new PropertyValue
                                        {
                                            PropertyId = x.Key.PropertyId,
                                            Property = x.First(y => y.PropertyId == x.Key.PropertyId).Property,
                                            PropertyValueId = x.First(y => y.PropertyId == x.Key.PropertyId).PropertyValueId,
                                            SortOrder = x.First(y => y.PropertyId == x.Key.PropertyId).SortOrder,
                                            Value = String.Join(", ", x.Where(y => y.PropertyId == x.Key.PropertyId).Select(v => v.Value))
                                        }).ToList();

            model.Gifts = OfferService.GetProductGifts(_product.ProductId);

            model.MinimumOrderPrice = CustomerGroupService.GetMinimumOrderPrice();

            model.RatingReadOnly = RatingService.DoesUserVote(_product.ProductId, customer.Id);

            var reviewsCountString = string.Empty;
            var modules = AttachedModules.GetModules<IModuleReviews>();
            if (modules.Any())
            {
                foreach (var module in modules)
                {
                    var instance = (IModuleReviews)Activator.CreateInstance(module);
                    reviewsCountString += instance.GetReviewsCount(HttpContext.Current.Request.Url.AbsoluteUri);
                }
            }
            else
            {
                var reviewsCount = SettingsCatalog.ModerateReviews
                    ? ReviewService.GetCheckedReviewsCount(_product.ProductId, EntityType.Product)
                    : ReviewService.GetReviewsCount(_product.ProductId, EntityType.Product);
                reviewsCountString = string.Format("{0} {1}", reviewsCount,
                    Strings.Numerals(reviewsCount,
                        LocalizationService.GetResource("Product.Reviews0"),
                        LocalizationService.GetResource("Product.Reviews1"),
                        LocalizationService.GetResource("Product.Reviews2"),
                        LocalizationService.GetResource("Product.Reviews5")));
            }

            model.ReviewsCount = reviewsCountString;
            model.AllowReviews = SettingsCatalog.AllowReviews;
            model.ShowBriefDescription = false;

            model.CustomViewPath = _view;

            model.PreOrderButtonText = SettingsCatalog.PreOrderButtonText;

            model.HidePrice = SettingsCatalog.HidePrice;
            model.TextInsteadOfPrice = SettingsCatalog.TextInsteadOfPrice;

            model.IsAvailableForPurchase =
                offer != null
                && (offer.Amount > 0 || model.AllowBuyOutOfStockProducts)
                && (model.FinalPrice > 0
                    // под модуль Бесплатно+Доставка
                    || (model.FinalPrice == 0 && model.FinalDiscount != null && model.FinalDiscount.HasValue)
                    // ---
                    || model.AllowBuyOutOfStockProducts);

            model.IsAvailableForPurchaseOnCredit =
                model.IsAvailableForPurchase
                && model.FirstPaymentMinPrice <= model.FinalPrice
                && model.FirstPaymentMaxPrice >= model.FinalPrice;

            model.IsAvailableForPurchaseOnBuyOneClick =
                model.IsAvailableForPurchase
                && model.MinimumOrderPrice <= model.FinalPrice;

            return model;
        }

        private string GetBonusCardPrice(Product product, float productPrice)
        {
            if (!BonusSystem.IsActive || productPrice <= 0 || !product.AccrueBonuses)
                return null;

            var bonusCard = BonusSystemService.GetCard(CustomerContext.CurrentCustomer.Id);
            if (bonusCard != null && bonusCard.Blocked)
                return null;

            if (bonusCard != null)
                return PriceService.GetBonusPrice((float)bonusCard.Grade.BonusPercent, productPrice).BaseRound().FormatPrice();

            return (//BonusSystem.BonusesForNewCard + 
                    PriceService.GetBonusPrice((float)BonusSystem.BonusFirstPercent, productPrice)).BaseRound().FormatPrice();
        }
    }
}