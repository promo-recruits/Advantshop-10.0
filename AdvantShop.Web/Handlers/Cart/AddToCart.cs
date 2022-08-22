using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Models.Cart;
using AdvantShop.Models.Checkout;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvantShop.Handlers.Cart
{
    public class AddToCart
    {
        #region Ctor

        private readonly CartItemAddingModel _item;
        private readonly Controller _controller;

        public AddToCart(CartItemAddingModel item, Controller controller)
        {
            _item = item;
            _controller = controller;
        }

        #endregion Ctor

        public AddToCartResult Execute()
        {
            var cartId = 0;
            ShoppingCartItem cartItem = null;

            if (_item.Mode == "lp" && _item.OfferIds != null)
            {
                var offers = _item.OfferIds.Select(OfferService.GetOffer).Where(o => o != null).ToList();
                if (offers.Count == 0)
                    return new AddToCartResult("fail");

                var button = GetLpButton(_item.LpBlockId);

                if (_item.LpEntityId != null && _item.LpEntityId != 0)
                {
                    if (_item.LpEntityType == "order")
                    {
                        var order = OrderService.GetOrder(_item.LpEntityId.Value);
                        if (order != null)
                        {
                            AddToCartResult result = null;
                            foreach (var offer in offers)
                                result = AddItemToOrder(GetCartItem(offer, button: button), order);

                            return result;
                        }
                    }
                    else if (_item.LpEntityType == "lead")
                    {
                        var lead = LeadService.GetLead(_item.LpEntityId.Value);
                        if (lead != null)
                        {
                            AddToCartResult result = null;
                            foreach (var offer in offers)
                                result = AddItemToLead(lead, offer);

                            return result;
                        }
                    }
                }
                else
                {
                    ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart);

                    foreach (var offer in offers)
                        cartId = ShoppingCartService.AddShoppingCartItem(GetCartItem(offer, button: button));
                }
            }
            else
            {
                Offer offer;

                if (_item.OfferId == 0 && _item.ProductId != 0)
                {
                    var product = ProductService.GetProduct(_item.ProductId);

                    if (product == null || product.Offers.Count == 0)
                        return new AddToCartResult("fail");

                    if (product.Offers.Count != 1)
                        return new AddToCartResult("redirect");

                    offer = product.Offers.First();
                }
                else
                {
                    offer = OfferService.GetOffer(_item.OfferId);
                }

                if (offer == null)
                    return new AddToCartResult("fail");

                if ((!offer.Product.Enabled || !offer.Product.CategoryEnabled) && _item.Mode != "lp")
                    return new AddToCartResult("fail");

                List<EvaluatedCustomOptions> listOptions = null;
                var selectedOptions = !String.IsNullOrWhiteSpace(_item.AttributesXml) && _item.AttributesXml != "null"
                                        ? HttpUtility.UrlDecode(_item.AttributesXml)
                                        : null;

                if (selectedOptions != null)
                {
                    try
                    {
                        listOptions = CustomOptionsService.DeserializeFromXml(selectedOptions, offer.Product.Currency.Rate);
                    }
                    catch (Exception)
                    {
                        listOptions = null;
                    }
                }

                if (CustomOptionsService.DoesProductHaveRequiredCustomOptions(offer.ProductId) && listOptions == null)
                    return new AddToCartResult("redirect");

                cartItem = GetCartItem(offer, listOptions, selectedOptions, GetLpButton(_item.LpBlockId));

                if (_item.Mode == "lp")
                {
                    if (_item.LpEntityId != null && _item.LpEntityId != 0)
                    {
                        if (_item.LpEntityType == "order")
                        {
                            var order = OrderService.GetOrder(_item.LpEntityId.Value);
                            if (order != null)
                                return AddItemToOrder(cartItem, order);
                        }
                        else if (_item.LpEntityType == "lead")
                        {
                            var lead = LeadService.GetLead(_item.LpEntityId.Value);
                            if (lead != null)
                                return AddItemToLead(lead, offer);
                        }
                    }

                    if (_item.LpId != null && LPageSettings.ShowShoppingCart(_item.LpId.Value))
                        _item.Mode = "";
                    else
                        ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart);
                }

                cartId = ShoppingCartService.AddShoppingCartItem(cartItem);

                #region gifts

                var cart = ShoppingCartService.CurrentShoppingCart;
                foreach (var gift in OfferService.GetProductGifts(offer.ProductId))
                {
                    var giftProducts = OfferService.GetGiftProducts(gift.OfferId);    // products with this gift
                    var giftCount = cart.Where(x => giftProducts.ContainsKey(x.Offer.ProductId) && !x.IsGift)
                        .GroupBy(x => x.Offer.ProductId) // по модификациям одного товара
                        .Select(x => new
                        {
                            Amount = x.Sum(y => y.Amount),
                            ProductId = x.Key
                        })
                        .Sum(x => x.Amount >= giftProducts[x.ProductId]
                                  ? SettingsCheckout.MultiplyGiftsCount ? (int)Math.Floor(x.Amount / giftProducts[x.ProductId]) : 1
                                  : 0);

                    if (giftCount <= 0)
                        continue;

                    var giftItem = cart.Find(x => x.OfferId == gift.OfferId && x.IsGift);
                    if (giftItem != null)
                    {
                        giftItem.Amount = giftCount;
                        ShoppingCartService.UpdateShoppingCartItem(giftItem);
                    }
                    else
                    {
                        ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem()
                        {
                            OfferId = gift.OfferId,
                            Amount = giftCount,
                            IsGift = true
                        });
                    }
                }

                #endregion gifts

                if (_item.Payment != 0)
                    CommonHelper.SetCookie("payment", _item.Payment.ToString());

                ModulesExecuter.AddToCart(cartItem, _controller.Url.AbsoluteRouteUrl("Product", new { url = offer.Product.UrlPath }));
            }

            var url = "";

            if (_item.Mode == "lp")
            {
                var queryString = _item.LpId != null ? "?lpid=" + _item.LpId : "";

                if (_item.LpUpId != null)
                    queryString += (string.IsNullOrEmpty(queryString) ? "?" : "&") + "lpupid=" + _item.LpUpId;

                if (_item.HideShipping.HasValue && _item.HideShipping.Value)
                    queryString += (string.IsNullOrEmpty(queryString) ? "?" : "&") + "mode=" + (int)CheckoutLpMode.WithoutShipping;

                url = "checkout/lp" + queryString;
            }
            
            return new AddToCartResult(_item.Mode == "lp" ? "redirect" : "success")
            {
                Url = url,
                CartId = cartId,
                TotalItems = ShoppingCartService.CurrentShoppingCart.TotalItems,
                CartItem = cartItem
            };
        }

        private ShoppingCartItem GetCartItem(Offer offer, List<EvaluatedCustomOptions> listOptions = null, string selectedOptions = null, LpButton button = null)
        {
            if (Single.IsNaN(_item.Amount) || _item.Amount == 0)
            {
                var prodMinAmount = offer.Product.MinAmount == null
                            ? offer.Product.Multiplicity
                            : offer.Product.Multiplicity > offer.Product.MinAmount
                                ? offer.Product.Multiplicity
                                : offer.Product.MinAmount.Value;

                _item.Amount = prodMinAmount > 0 ? prodMinAmount : 1;
            }

            var item = new ShoppingCartItem()
            {
                OfferId = offer.OfferId,
                Amount = _item.Amount,
                AttributesXml = listOptions != null ? selectedOptions : string.Empty,
            };

            if (button != null)
            {
                if (button.Discount != null)
                {
                    var customOptionsPrice = CustomOptionsService.GetCustomOptionPrice(offer.RoundedPrice, selectedOptions, offer.Product.Currency.Rate);

                    item.CustomPrice = PriceService.GetFinalPrice(offer.RoundedPrice + customOptionsPrice, button.Discount);
                }

                var actionOfferItem = GetActionOfferItem(button, offer);
                if (actionOfferItem != null)
                {
                    if (!string.IsNullOrEmpty(actionOfferItem.OfferPrice))
                        item.CustomPrice = actionOfferItem.OfferPrice.TryParseFloat();

                    if (!string.IsNullOrEmpty(actionOfferItem.OfferAmount))
                        item.Amount = actionOfferItem.OfferAmount.TryParseFloat(1);
                }
            }

            return item;
        }

        private AddToCartResult AddItemToOrder(ShoppingCartItem cartItem, Order order)
        {
            var orderItems = order.OrderItems;
            var orderItem = (OrderItem)cartItem;

            var button = GetLpButton(_item.LpBlockId);
            if (button != null && !string.IsNullOrEmpty(button.ActionOfferPrice) && (button.UseManyOffers == null || !button.UseManyOffers.Value))
                orderItem.Price = button.ActionOfferPrice.TryParseFloat();

            orderItems.Add(orderItem);

            OrderService.AddUpdateOrderItems(orderItems, OrderService.GetOrderItems(order.OrderID), order, null, false, true);

            var lpUrl = "";
            if (_item.LpUpId != null)
            {
                lpUrl = new LpService().GetLpLink(_controller.Request.Url.Host, _item.LpUpId.Value) + "?code=" + order.Code;
            }
            else
            {
                var lpSite = _item.LpBlockId != null ? new LpSiteService().GetByLandingBlockId(_item.LpBlockId.Value) : null;
                var showMode = lpSite == null ||
                               lpSite.Template != LpFunnelType.ProductCrossSellDownSell.ToString() ||
                               (lpSite.Template == LpFunnelType.ProductCrossSellDownSell.ToString() && _item.ModeFrom == "lp");

                _controller.TempData["orderid"] = order.OrderID;

                lpUrl = UrlService.GetUrl("checkout/success?code=" + order.Code + (showMode ? "&mode=lp" : ""));
            }

            return new AddToCartResult("redirect") { Url = lpUrl };
        }

        private AddToCartResult AddItemToLead(Lead lead, Offer offer)
        {
            float? actionOfferPrice = null;

            var button = GetLpButton(_item.LpBlockId);
            if (button != null)
            {
                if (!string.IsNullOrEmpty(button.ActionOfferPrice) &&
                    (button.UseManyOffers == null || !button.UseManyOffers.Value))
                {
                    actionOfferPrice = button.ActionOfferPrice.TryParseFloat();
                }
                else
                {
                    var actionOfferItem = GetActionOfferItem(button, offer);
                    if (actionOfferItem != null)
                    {
                        if (!string.IsNullOrEmpty(actionOfferItem.OfferPrice))
                            actionOfferPrice = actionOfferItem.OfferPrice.TryParseFloat();

                        if (!string.IsNullOrEmpty(actionOfferItem.OfferAmount))
                            _item.Amount = actionOfferItem.OfferAmount.TryParseFloat(1);
                    }
                }

                var shippingPrice = button.ActionShippingPrice.TryParseFloat(true);
                if (shippingPrice != null)
                {
                    lead.ShippingName = LocalizationService.GetResource("Lead.LaningShippingName");
                    lead.ShippingCost = shippingPrice.Value;
                }
            }

            lead.LeadItems.Add(new LeadItem(offer, _item.Amount, actionOfferPrice));
            LeadService.UpdateLead(lead, false, trackChanges: false);

            var lpUrl = _item.LpUpId != null
                ? new LpService().GetLpLink(_controller.Request.Url.Host, _item.LpUpId.Value) + "?lid=" + lead.Id
                : UrlService.GetUrl("checkout/buyinoneclicksuccess/" + lead.Id + "?mode=lp");

            return new AddToCartResult("redirect") { Url = lpUrl };
        }

        private LpButton GetLpButton(int? blockId)
        {
            if (blockId == null)
                return null;

            var block = new LpBlockService().Get(blockId.Value);
            var buttonName = !string.IsNullOrEmpty(_item.LpButtonName) ? _item.LpButtonName : "button";

            return block != null ? block.TryGetSetting<LpButton>(buttonName) : null;
        }

        private LpButtonOfferItem GetActionOfferItem(LpButton button, Offer offer)
        {
            if (button == null)
                return null;

            if (button.ActionOfferItems != null && button.ActionOfferItems.Count > 0)
                return button.ActionOfferItems.Find(x => x.OfferId == offer.OfferId.ToString());

            return null;
        }
    }
}