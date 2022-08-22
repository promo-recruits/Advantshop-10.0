using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Customers;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Handlers.Inplace
{
    public class InplaceOfferHandler
    {
        public object Execute(int id, string content, OfferInplaceField field)
        {
            var offer = OfferService.GetOffer(id);
            if (offer == null)
                return null;

            var productCurrency = offer.Product.Currency.Rate;
            Offer _offerMutation;

            switch (field)
            {
                case OfferInplaceField.Price:
                    var price = content.Replace("&nbsp;", "").Replace(" ", "").TryParseFloat();

                    offer.BasePrice = price / productCurrency * CurrencyService.CurrentCurrency.Rate;
                    OfferService.UpdateOffer(offer);
                    break;

                case OfferInplaceField.DiscountPercent:
                    
                    var percent = content.Replace("&nbsp;", "").Replace(" ", "").TryParseFloat();

                    offer.Product.Discount = new Discount(percent, 0);
   
                    ProductService.UpdateProductDiscount(offer.ProductId, offer.Product.Discount.Percent);

                    break;

                case OfferInplaceField.DiscountAbs:

                    var percentAbs = content.Replace("&nbsp;", "").Replace(" ", "").TryParseFloat() / productCurrency * CurrencyService.CurrentCurrency.Rate;

                    offer.Product.Discount = new Discount(percentAbs / offer.BasePrice * 100, 0);

                    ProductService.UpdateProductDiscount(offer.ProductId, offer.Product.Discount.Percent);

                    break;
                case OfferInplaceField.Amount:
                    offer.Amount = content.TryParseFloat();
                    OfferService.UpdateOffer(offer);
                    break;

                case OfferInplaceField.ArtNo:
                    offer.ArtNo = content;
                    OfferService.UpdateOffer(offer);

                    if (offer.Product.Offers.Count == 1)
                    {
                        var p = offer.Product;
                        p.ArtNo = content;
                        p.ModifiedBy = CustomerContext.CustomerId.ToString();
                        ProductService.UpdateProduct(p, true, true);
                    }
                    break;

                case OfferInplaceField.Weight:
                    _offerMutation = FeaturesService.IsEnabled(EFeature.OfferWeightAndDimensions) ? offer : (offer.Main ? offer : OfferService.GetMainOfferForExport(offer.ProductId));

                    _offerMutation.Weight = content.TryParseFloat();
                    OfferService.UpdateOffer(_offerMutation);

                    break;

                case OfferInplaceField.Length:
                    _offerMutation = FeaturesService.IsEnabled(EFeature.OfferWeightAndDimensions) ? offer : (offer.Main ? offer : OfferService.GetMainOfferForExport(offer.ProductId));

                    _offerMutation.Length = content.TryParseFloat();
                    OfferService.UpdateOffer(_offerMutation);

                    break;

                case OfferInplaceField.Width:
                    _offerMutation = FeaturesService.IsEnabled(EFeature.OfferWeightAndDimensions) ? offer : (offer.Main ? offer : OfferService.GetMainOfferForExport(offer.ProductId));

                    _offerMutation.Width = content.TryParseFloat();
                    OfferService.UpdateOffer(_offerMutation);

                    break;

                case OfferInplaceField.Height:
                    _offerMutation = FeaturesService.IsEnabled(EFeature.OfferWeightAndDimensions) ? offer : (offer.Main ? offer : OfferService.GetMainOfferForExport(offer.ProductId));

                    _offerMutation.Height = content.TryParseFloat();
                    OfferService.UpdateOffer(_offerMutation);

                    break;

                default:
                    return false;
            }

            ProductService.PreCalcProductParams(offer.ProductId);
            return true;
        }
    }
}