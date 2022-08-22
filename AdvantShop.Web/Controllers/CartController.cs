using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Customers;
using AdvantShop.Handlers.Cart;
using AdvantShop.Orders;
using AdvantShop.ViewModel.Cart;
using AdvantShop.ViewModel.Common;
using AdvantShop.Web.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Models.Cart;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class CartController : BaseClientController
    {
        #region Cart

        public ActionResult Index(string products, string coupon)
        {
            if (!string.IsNullOrWhiteSpace(products))
                ShoppingCartService.AddShoppingCartItems(products);

            if (!string.IsNullOrWhiteSpace(coupon))
            {
                var customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

                if (customerGroup.CustomerGroupId == CustomerGroupService.DefaultCustomerGroup)
                {
                    var userCoupon = CouponService.GetCouponByCode(coupon);
                    if (userCoupon != null && CouponService.CanApplyCustomerCoupon(userCoupon) && GiftCertificateService.GetCustomerCertificate() == null)
                        CouponService.AddCustomerCoupon(userCoupon.CouponID);
                }
            }

            var shpCart = ShoppingCartService.CurrentShoppingCart;

            var model = new CartViewModel()
            {
                Cart = shpCart,
                ShowConfirmButton = true,
                PhotoWidth = SettingsPictureSize.XSmallProductImageWidth
            };

            foreach (var module in AttachedModules.GetModules<IShoppingCart>())
            {
                var moduleObject = (IShoppingCart)Activator.CreateInstance(module, null);
                model.ShowConfirmButton &= moduleObject.ShowConfirmButtons;
            }

            model.ShowBuyOneClick = model.ShowConfirmButton && SettingsCheckout.BuyInOneClick;

            SetMetaInformation(T("Cart.Index.ShoppingCart"));
            SetNoFollowNoIndex();

            var tagManager = GoogleTagManagerContext.Current;
            if (tagManager.Enabled)
            {
                tagManager.PageType = ePageType.cart;
                tagManager.ProdIds = shpCart.Select(item => item.Offer.ArtNo).ToList();
                tagManager.Products = shpCart.Select(x => new TransactionProduct()
                {
                    Id = x.Offer.OfferId.ToString(),
                    SKU = x.Offer.ArtNo,
                    Category = x.Offer.Product.MainCategory != null ? x.Offer.Product.MainCategory.Name : string.Empty,
                    Name = x.Offer.Product.Name,
                    Price = x.Price,
                    Quantity = x.Amount
                }).ToList();
                tagManager.TotalValue = shpCart.TotalPrice;
            }

            WriteLog("", Url.AbsoluteRouteUrl("Cart"), ePageType.cart);

            SetNgController(NgControllers.NgControllersTypes.CartPageCtrl);

            return View(model);
        }


        [ChildActionOnly]
        public ActionResult ShoppingCart()
        {
            var itemsAmount = ShoppingCartService.CurrentShoppingCart.TotalItems;
            var amount = string.Format("{0} {1}", itemsAmount == 0 ? "" : itemsAmount.ToString(),
                                  Strings.Numerals(itemsAmount,
                                    T("Cart.Product0"), T("Cart.Product1"), T("Cart.Product2"), T("Cart.Product5")));

            return PartialView("ShoppingCart", new ShoppingCartViewModel() { Amount = amount, TotalItems = itemsAmount });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetCart()
        {
            return Json(new GetCartHandler().Get());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddToCart(CartItemAddingModel item)
        {
            return Json(new AddToCart(item, this).Execute());
        }

        public JsonResult AddCartItems(List<CartItemAddingModel> items, int payment = 0)
        {
            if (!items.Any())
                return Json(new { status = "fail" });

            foreach (var item in items)
            {
                AddToCart(item);
            }
            var cart = ShoppingCartService.CurrentShoppingCart;
            var mainCartItem = cart.FirstOrDefault(x => x.OfferId == items[0].OfferId) ?? cart.FirstOrDefault();
            return Json(new { cart.TotalItems, status = "success", cartId = mainCartItem != null ? mainCartItem.ShoppingCartItemId : 0 });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateCart(Dictionary<int, float> items)
        {
            if (items == null)
                return Json(new { status = "fail" });

            var cart = ShoppingCartService.CurrentShoppingCart;

            foreach (var pair in items)
            {
                var cartItem = cart.Find(x => x.ShoppingCartItemId == pair.Key);

                if (cartItem == null || pair.Value <= 0)
                {
                    return Json(new { status = "fail" });
                }
                if (cartItem.FrozenAmount)
                    continue;

                #region gifts
                if (ShoppingCartService.ShoppingCartHasProductsWithGifts())
                {
                    foreach (var gift in OfferService.GetProductGifts(cartItem.Offer.ProductId))
                    {
                        var giftProducts = OfferService.GetGiftProducts(gift.OfferId);    // products with this gift
                        var giftCount = cart.Where(x => giftProducts.ContainsKey(x.Offer.ProductId) && !x.IsGift)
                            .GroupBy(x => x.Offer.ProductId) // по модификациям одного товара
                            .Select(x => new
                            {
                                FinalAmount = x.Sum(y => items.ContainsKey(y.ShoppingCartItemId) ? items[y.ShoppingCartItemId] : y.Amount),
                                ProductId = x.Key
                            })
                            .Sum(x => x.FinalAmount >= giftProducts[x.ProductId]
                                ? SettingsCheckout.MultiplyGiftsCount ? (int)Math.Floor(x.FinalAmount / giftProducts[x.ProductId]) : 1
                                : 0);

                        var giftItem = cart.Find(x => x.OfferId == gift.OfferId && x.IsGift);
                        if (giftItem != null)
                        {
                            if (giftCount == 0)
                                ShoppingCartService.DeleteShoppingCartItem(giftItem);
                            else if (giftCount != giftItem.Amount)
                            {
                                giftItem.Amount = giftCount;
                                ShoppingCartService.UpdateShoppingCartItem(giftItem);
                            }
                        }
                        else if (giftCount > 0)
                        {
                            ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem()
                            {
                                OfferId = gift.OfferId,
                                Amount = giftCount,
                                IsGift = true
                            });
                        }
                    }
                }
                #endregion

                cartItem.Amount = pair.Value;
                ShoppingCartService.UpdateShoppingCartItem(cartItem);
            }

            return Json(new { ShoppingCartService.CurrentShoppingCart.TotalItems, status = "success" });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RemoveFromCart(int itemId)
        {
            if (itemId == 0)
                return Json(new { status = "fail" });

            var cart = ShoppingCartService.CurrentShoppingCart;

            var cartItem = cart.Find(item => item.ShoppingCartItemId == itemId);
            if (cartItem != null)
            {
                #region gifts
                if (cart.Any(x => x.IsGift))
                {
                    var gifts = OfferService.GetProductGifts(cartItem.Offer.ProductId);
                    foreach (var gift in gifts)
                    {
                        var giftItem = cart.Find(x => x.OfferId == gift.OfferId && x.IsGift);
                        if (giftItem == null)
                            continue;

                        var giftProducts = OfferService.GetGiftProducts(gift.OfferId);
                        var giftCount = cart.Where(x => giftProducts.ContainsKey(x.Offer.ProductId) && !x.IsGift && x.ShoppingCartItemId != itemId)
                            .GroupBy(x => x.Offer.ProductId) // по модификациям одного товара
                            .Select(x => new
                            {
                                Amount = x.Sum(y => y.Amount),
                                ProductId = x.Key
                            })
                            .Sum(x => x.Amount >= giftProducts[x.ProductId] 
                                ? SettingsCheckout.MultiplyGiftsCount ? (int)Math.Floor(x.Amount / giftProducts[x.ProductId]) : 1
                                : 0);

                        if (giftCount > 0 && giftItem.Amount != giftCount)
                        {
                            giftItem.Amount = giftCount;
                            ShoppingCartService.UpdateShoppingCartItem(giftItem);
                        }
                        else if (giftCount <= 0)
                        {
                            ShoppingCartService.DeleteShoppingCartItem(giftItem);
                        }
                    }
                }
                #endregion

                ShoppingCartService.DeleteShoppingCartItem(cartItem);
            }

            return Json(new
            {
                ShoppingCartService.CurrentShoppingCart.TotalItems,
                status = "success",
                offerId = cartItem != null ? cartItem.OfferId : 0
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ClearCart()
        {
            ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart);

            // при полной очистке корзины удаляем примененый купон 04 12 18
            var coupon = CouponService.GetCustomerCoupon();
            if (coupon != null)
                CouponService.DeleteCustomerCoupon(coupon.CouponID);

            return Json(new { ShoppingCartService.CurrentShoppingCart.TotalItems, status = "success" });
        }

        #endregion

        // moved region Certificate and coupon. Не нашел ссылки в движке, модулях и шаблонах
    }
}