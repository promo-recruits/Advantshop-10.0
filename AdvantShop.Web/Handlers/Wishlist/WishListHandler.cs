﻿using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Handlers.Catalog
{
    public class WishListHandler
    {
        public ProductViewModel Get()
        {
            var products = new List<ProductModel>();

            foreach (var item in ShoppingCartService.CurrentWishlist)
            {
                var product = item.Offer.Product;
                var photos = product.ProductPhotos;

                var productModel = new ProductModel()
                {
                    ProductId = product.ProductId,
                    OfferId = item.OfferId,
                    UrlPath = product.UrlPath,
                    Name = product.Name,
                    ArtNo = item.Offer.ArtNo,
                    BriefDescription = product.BriefDescription,
                    Enabled = product.Enabled,
                    Bestseller = product.BestSeller,
                    Sales = product.OnSale,
                    Recomend = product.Recomended,
                    New = product.New,
                    AllowPreorder = product.AllowPreOrder,
                    Amount = item.Offer.Amount,
                    AmountOffer = item.Offer.Amount,
                    Multiplicity = product.Multiplicity,
                    BasePrice = item.Offer.BasePrice,
                    Discount = product.Discount.Percent,
                    DiscountAmount = product.Discount.Amount,
                    Photo = item.Offer.Photo,
                    CurrencyValue = product.Currency.Rate,
                    CountPhoto = item.Offer.ColorID != null ? photos.Count(x => x.ColorID == item.Offer.ColorID) : photos.Count,
                    Ratio = product.ManualRatio ?? product.Ratio,
                    Comments = ReviewService.GetReviewsCount(product.ProductId, EntityType.Product),
                    SelectedColorId = item.Offer.ColorID,
                    Colors = item.Offer.ColorID != null
                        ? JsonConvert.SerializeObject(new List<Color>()
                            {
                                ColorService.GetColor(item.Offer.ColorID)
                            })
                        : null,
                    SizeId = item.Offer.SizeID ?? 0,
                };

                products.Add(productModel);
            }

            var model = new ProductViewModel(products)
            {
                DisplayPhotoPreviews = false
            };

            return model;
        }
    }
}