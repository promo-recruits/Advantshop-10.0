using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class AddOrderItems
    {
        private readonly Order _order;
        private readonly List<int> _offerIds;

        public AddOrderItems(Order order, List<int> offerIds)
        {
            _order = order;
            _offerIds = offerIds;
        }

        public bool Execute()
        {
            if (CurrencyService.CurrentCurrency.Iso3 != _order.OrderCurrency.CurrencyCode)
                CurrencyService.CurrentCurrency = _order.OrderCurrency;

            var saveChanges = false;
            var couponCode = _order.Coupon != null ? _order.Coupon.Code : null;

            foreach (var offerId in _offerIds)
            {
                var offer = OfferService.GetOffer(offerId);
                if (offer == null)
                    continue;

                var product = offer.Product;

                var prodMinAmount = product.MinAmount == null
                            ? product.Multiplicity
                            : product.Multiplicity > product.MinAmount
                                ? product.Multiplicity
                                : product.MinAmount.Value;

                var price = PriceService.GetFinalPrice(offer, new CustomerGroup() { GroupDiscount = _order.GroupDiscount });
                var productsDiscount = PriceService.GetFinalDiscount(price, product.Discount, product.Currency.Rate, new CustomerGroup(), offer.ProductId);

                var item = GetOrderItem(product, offer, price, prodMinAmount, couponCode, productsDiscount, false);

                var oItem = _order.OrderItems.Find(x => x == item);
                if (oItem != null)
                {
                    oItem.Amount += 1;
                    saveChanges = true;
                }
                else
                {
                    _order.OrderItems.Add(item);
                    saveChanges = true;
                }

                #region Gift

                foreach (var gift in OfferService.GetProductGifts(offer.ProductId))
                {
                    var giftProducts = OfferService.GetGiftProducts(gift.OfferId);    // products with this gift
                    var giftCount = _order.OrderItems.Where(x => x.ProductID != null && giftProducts.ContainsKey(x.ProductID.Value) && !x.IsGift)
                        .GroupBy(x => x.ProductID) // по модификациям одного товара
                        .Select(x => new
                        {
                            Amount = x.Sum(y => y.Amount),
                            ProductId = x.Key
                        })
                        .Sum(x => x.Amount >= giftProducts[x.ProductId.Value]
                                  ? SettingsCheckout.MultiplyGiftsCount ? (int)Math.Floor(x.Amount / giftProducts[x.ProductId.Value]) : 1
                                  : 0);

                    if (giftCount <= 0)
                        continue;

                    var giftItem = _order.OrderItems.Find(x => x.ArtNo == gift.ArtNo && x.IsGift);
                    if (giftItem != null)
                    {
                        giftItem.Amount = giftCount;
                    }
                    else
                    {
                        var giftOffer = OfferService.GetOffer(gift.OfferId);

                        var giftOrderItem = GetOrderItem(gift.Product, giftOffer, 0, giftCount, null, null, true);

                        _order.OrderItems.Add(giftOrderItem);
                    }
                }

                #endregion
            }

            return saveChanges;
        }

        private OrderItem GetOrderItem(Product product, Offer offer, float price, float prodMinAmount, string couponCode, Discount productsDiscount, bool isGift)
        {
            var item = new OrderItem
            {
                ProductID = product.ProductId,
                Name = product.Name,
                ArtNo = offer.ArtNo,
                BarCode = offer.BarCode,
                Price = price,
                Amount = prodMinAmount,
                SupplyPrice = offer.SupplyPrice,
                SelectedOptions = new List<EvaluatedCustomOptions>(),
                Weight = offer.GetWeight(),
                IsCouponApplied = IsCouponApplied(couponCode, product.ProductId, price, productsDiscount),
                Color = offer.Color != null ? offer.Color.ColorName : null,
                Size = offer.Size != null ? offer.Size.SizeName : null,
                PhotoID = offer.Photo != null ? offer.Photo.PhotoId : default(int),
                AccrueBonuses = offer.Product.AccrueBonuses,
                Width = offer.GetWidth(),
                Length = offer.GetLength(),
                Height = offer.GetHeight(),
                PaymentMethodType = offer.Product.PaymentMethodType,
                PaymentSubjectType = offer.Product.PaymentSubjectType,
                IsGift = isGift
            };

            var tax = product.TaxId != null ? TaxService.GetTax(product.TaxId.Value) : null;
            if (tax != null)
            {
                item.TaxId = tax.TaxId;
                item.TaxName = tax.Name;
                item.TaxType = tax.TaxType;
                item.TaxRate = tax.Rate;
                item.TaxShowInPrice = tax.ShowInPrice;
            }

            return item;
        }

        private bool IsCouponApplied(string couponCode, int productId, float price, Discount productsDiscount)
        {
            if (couponCode.IsNullOrEmpty())
                return false;

            var coupon = CouponService.GetCouponByCode(couponCode);

            if (coupon != null && CouponService.IsCouponAppliedToProduct(coupon.CouponID, productId))
            {
                if (coupon.Type == CouponType.Percent)
                {
                    if ((productsDiscount.Type == DiscountType.Percent && coupon.Value > productsDiscount.Percent) ||
                        (productsDiscount.Type == DiscountType.Amount && price * coupon.Value / 100 > productsDiscount.Amount))
                    {
                        return true;
                    }
                }
                else if (coupon.Type == CouponType.Fixed)
                {
                    if ((productsDiscount.Type == DiscountType.Percent && coupon.Value > price * productsDiscount.Percent / 100) ||
                        (productsDiscount.Type == DiscountType.Amount && coupon.Value > productsDiscount.Amount))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
