using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Handlers.Cart;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;

namespace AdvantShop.Areas.Mobile.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class CartController : BaseMobileController
    {
        public ActionResult Index(string products)
        {
            if (!String.IsNullOrWhiteSpace(products))
            {
                foreach(var item in products.Split(';'))
                {
                    int offerId;
                    var newItem = new ShoppingCartItem() { CustomerId = CustomerContext.CustomerId };

                    var parts = item.Split('-');
                    if(parts.Length > 0 && (offerId = parts[0].TryParseInt(0)) != 0 && OfferService.GetOffer(offerId) != null)
                    {
                        newItem.OfferId = offerId;
                    }
                    else
                    {
                        continue;
                    }

                    newItem.Amount = parts.Length > 1 ? parts[1].TryParseFloat() : 1;

                    var currentItem = ShoppingCartService.CurrentShoppingCart.FirstOrDefault(shpCartitem => shpCartitem.OfferId == newItem.OfferId);
                    if(currentItem != null)
                    {
                        currentItem.Amount = newItem.Amount;
                        ShoppingCartService.UpdateShoppingCartItem(currentItem);
                    }
                    else
                    {
                        ShoppingCartService.AddShoppingCartItem(newItem);
                    }
                }
            }

            SetMobileTitle(T("Mobile.Cart.Index.Title"));
            SetMetaInformation(T("Mobile.Cart.Index.Title"));
            SetNoFollowNoIndex();

            ViewBag.CartHasItems = ShoppingCartService.CurrentShoppingCart.HasItems;

            return View();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult GetCart()
        {
            return PartialView("_SidebarCart", new GetCartHandler().Get());
        }
    }
}