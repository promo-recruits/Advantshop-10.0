using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Handlers.ProductDetails;
using AdvantShop.Payment;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class ProductExtController : BaseClientProductController
    {
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetShippings(int offerId, float amount, string customOptions, string zip)
        {
            var model = new GetShippingsHandler(offerId, amount, customOptions, zip).Get();
            return Json(model);
        }

        public JsonResult GetOffers(int productId, int? colorId, int? sizeId)
        {
            var product = ProductService.GetProduct(productId);
            if (product == null)
                return Json(null);

            var offers = product.Offers;
            if (offers == null || offers.Count == 0)
                return Json(null);

            var offerSelected = OfferService.GetMainOffer(offers, product.AllowPreOrder, colorId, sizeId);
            var amountBuy = product.MinAmount == null
                                ? product.Multiplicity
                                : product.Multiplicity > product.MinAmount
                                    ? product.Multiplicity
                                    : product.MinAmount.Value;

            var obj = new
            {
                Offers = offers.Select(
                    offer => new
                    {
                        offer.OfferId,
                        offer.ProductId,
                        offer.ArtNo,
                        Color = offer.Color,
                        Size = offer.Size,
                        offer.RoundedPrice,
                        offer.Product.Discount,
                        offer.Amount,
                        AmountBuy = amountBuy,
                        offer.Main,
                        IsAvailable = RoundsMin(offer.Amount, product.Multiplicity) > 0.0f,
                        Available = string.Format("{0}{1}",
                        offer.Amount > 0 ? T("Product.Available") : T("Product.NotAvailable"),
                        offer.Amount > 0 && SettingsCatalog.ShowStockAvailability
                            ? string.Format(" (<div class=\"details-avalable-text\">{0}</div><div class=\"details-avalable-unit\">{1}</div>)",
                            RoundsMin(offer.Amount, product.Multiplicity).ToString(),
                            product.Unit.IsNotEmpty() ? " " + product.Unit: string.Empty)
                            : string.Empty),
                        Weight = offer.GetWeight(),
                        Width = offer.GetWidth(),
                        Length = offer.GetLength(),
                        Height = offer.GetHeight(),
                    }),
                StartOfferIdSelected = offerSelected.OfferId,
                product.Unit,
                ShowStockAvailability = SettingsCatalog.ShowStockAvailability,
                AllowPreOrder = product.AllowPreOrder
            };
            return Json(obj);
        }

        private float RoundsMin(float Amount, float Multiplicity)
        {
            int t = Multiplicity.ToString().Remove(0, Multiplicity.ToString().IndexOf(",") + 1).Length;
            var round = (float)Math.Round(Amount, t);
            var multiplicity = (float)Math.Round(Multiplicity, t);

            round = round > Amount ? round - multiplicity : round;

            return round;
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetOfferPrice(int offerId, string attributesXml, int? lpBlockId)
        {
            var offer = OfferService.GetOffer(offerId);
            if (offer == null)
                return Json(new { PriceString = 0F, PriceNumber = "", Bonuses = ""});

            var customer = CustomerContext.CurrentCustomer;
            var bonusPrice = string.Empty;

            Discount customDiscount = null;

            if (lpBlockId != null)
            {
                var block = new LpBlockService().Get(lpBlockId.Value);
                var button = block != null ? block.TryGetSetting<LpButton>("button") : null;

                if (button != null && button.Discount != null)
                    customDiscount = button.Discount;
            }

            var customOptionsPrice = CustomOptionsService.GetCustomOptionPrice(offer.RoundedPrice, HttpUtility.UrlDecode(attributesXml), offer.Product.Currency.Rate);

            var price = offer.RoundedPrice + customOptionsPrice;
            var totalDiscount = PriceService.GetFinalDiscount(price, customDiscount ?? offer.Product.Discount, offer.Product.Currency.Rate, customer.CustomerGroup, offer.ProductId);

            var startPrice = PriceService.GetFinalPrice(price);
            var finalPrice = PriceService.GetFinalPrice(price, totalDiscount);

            var priceHtml = PriceFormatService.FormatPrice(startPrice, finalPrice, totalDiscount, true, true);

            if (BonusSystem.IsActive && offer.RoundedPrice > 0 && offer.Product.AccrueBonuses)
            {
                var bonusCard = BonusSystemService.GetCard(customer.Id);
                if (bonusCard != null && bonusCard.Blocked)
                {
                    bonusPrice = null;
                }
                else if (bonusCard != null)
                {
                    bonusPrice = PriceFormatService.RenderBonusPrice((float)bonusCard.Grade.BonusPercent, finalPrice, totalDiscount);
                }
                else if (BonusSystem.BonusFirstPercent != 0)
                {
                    bonusPrice =
                        //BonusSystem.BonusesForNewCard +
                        PriceFormatService.RenderBonusPrice((float)BonusSystem.BonusFirstPercent, finalPrice, totalDiscount);
                }
            }

            return Json(new
            {
                PriceString = priceHtml, //for details page 
                PriceNumber = finalPrice,
                PriceOldNumber = startPrice,
                Bonuses = bonusPrice
            });
        }

        public JsonResult GetFirstPaymentPrice(float price, float discount, float discountAmount)
        {
            var finalPrice = PriceService.GetFinalPrice(price, new Discount(discount, discountAmount));
            foreach (var creditPayment in PaymentService.GetCreditPaymentMethods())
            {
                if (!creditPayment.ShowCreditButtonInProductCard)
                    continue;
                    
                var paymentMethod = creditPayment as PaymentMethod;
                var finalPriceInPaymentCurrency =
                    finalPrice
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
                    var result = firstPaymentInCurrentCurrency > 0
                        ? firstPaymentInCurrentCurrency.FormatPrice(true, false) + "*"
                        : T("Product.WithoutFirstPayment");
                    return Json(result);
                }
            }

            return null;
        }

        public JsonResult GetVideos(int productId)
        {
            return Json(ProductVideoService.GetProductVideos(productId));
        }

        public JsonResult GetPhotos(int productId)
        {
            var photos = PhotoService.GetPhotos<ProductPhoto>(productId, PhotoType.Product).Select(photo => new
            {
                PathXSmall = photo.ImageSrcXSmall(),
                PathSmall = photo.ImageSrcSmall(),
                PathMiddle = photo.ImageSrcMiddle(),
                PathBig = photo.ImageSrcBig(),
                photo.ColorID,
                photo.PhotoId,
                photo.Description,
                photo.Main,
                SettingsPictureSize.XSmallProductImageHeight,
                SettingsPictureSize.XSmallProductImageWidth,
                SettingsPictureSize.SmallProductImageHeight,
                SettingsPictureSize.SmallProductImageWidth,
                SettingsPictureSize.MiddleProductImageWidth,
                SettingsPictureSize.MiddleProductImageHeight,
                SettingsPictureSize.BigProductImageWidth,
                SettingsPictureSize.BigProductImageHeight,
                photo.Alt,
                photo.Title
            }).ToList();

            return Json(photos, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomOptions(int productId)
        {
            var customOptions = CustomOptionsService.GetCustomOptionsByProductId(productId);
            return Json(customOptions);
        }

        public JsonResult CustomOptionsXml(int productId, string selectedOptions)
        {
            var attributesXml = new GetProductCustomOptionsHandler(productId, selectedOptions).GetXml();
            return Json(HttpUtility.UrlEncode(attributesXml));
        }

        public JsonResult CustomOptions(int productId, string selectedOptions)
        {
            var handler = new GetProductCustomOptionsHandler(productId, selectedOptions);
            return Json(new
            {
                xml = HttpUtility.UrlEncode(handler.GetXml()),
                jsonHash = HttpUtility.UrlEncode(handler.GetJsonHash())
            });
        }

        public JsonResult AddRating(int objid, int rating)
        {
            float newRating = 0;

            if (objid != 0 && rating != 0)
                newRating = RatingService.Vote(objid, rating);

            return Json(newRating);
        }

        [HttpGet]
        public JsonResult GetPropertiesNames(string q)
        {
            return Json(PropertyService.GetPropertiesByName(q).ToList());
        }

        [HttpGet]
        public JsonResult GetPropertiesValues(string q, int productId, int propertyId = 0)
        {
            return Json(PropertyService.GetPropertiesValuesByNameEndProductId(q, productId, propertyId).ToList());
        }

        [HttpGet]
        public JsonResult GetReviewsCount(int productId)
        {
            var reviewsCount = ReviewService.GetReviews(productId, EntityType.Product).Count(x => x.Checked || !SettingsCatalog.ModerateReviews);
            return Json(new { reviewsCount });
        }
    }
}