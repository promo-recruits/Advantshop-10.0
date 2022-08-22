//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace AdvantShop.Orders
{
    public enum ShoppingCartType
    {
        /// <summary>
        /// Shopping cart
        /// </summary>
        ShoppingCart = 1,

        /// <summary>
        /// Wishlist
        /// </summary>
        Wishlist = 2,

        /// <summary>
        /// Compare product
        /// </summary>
        Compare = 3
    }

    public static class ShoppingCartService
    {
        private const string ShoppingCartContextKey = "ShoppingCartContext";

        public static ShoppingCart CurrentShoppingCart
        {
            get { return GetShoppingCart(ShoppingCartType.ShoppingCart); }
        }

        public static ShoppingCart CurrentCompare
        {
            get { return GetShoppingCart(ShoppingCartType.Compare); }
        }

        public static ShoppingCart CurrentWishlist
        {
            get { return GetShoppingCart(ShoppingCartType.Wishlist); }
        }


        public static ShoppingCart GetShoppingCart(ShoppingCartType shoppingCartType)
        {
            if (HttpContext.Current != null)
            {
                var cachedCart = HttpContext.Current.Items[ShoppingCartContextKey + shoppingCartType] as ShoppingCart;
                if (cachedCart != null) return cachedCart;
            }

            var cart = GetShoppingCart(shoppingCartType, CustomerContext.CustomerId);

            if (cart != null && cart.Count > 0 && shoppingCartType == ShoppingCartType.ShoppingCart)
            {
                #region gifts
                // remove the gifts that have no products in cart
                foreach (var gift in cart.Where(x => x.IsGift && string.IsNullOrEmpty(x.ModuleKey)).ToList())
                {
                    var giftProducts = OfferService.GetGiftProducts(gift.OfferId); // products with this gift
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
                    //var cartItemsWithGift = cart.Where(x => giftProducts.Any(gp => gp.Key == x.Offer.ProductId) && !x.IsGift).ToList();
                    // all cart items with this gift
                    if (giftCount == 0 || gift.Offer.Amount <= 0 || !gift.Offer.Product.Enabled)
                    {
                        DeleteShoppingCartItem(gift);
                        cart.Remove(gift);
                    }
                    else if (giftCount > 0 && gift.Amount != giftCount)
                    {
                        gift.Amount = giftCount;
                        UpdateShoppingCartItem(gift);
                    }
                }
                #endregion

                PartnerService.ApplyPartnerCoupon();

                foreach (var cartModule in AttachedModules.GetModules<IShoppingCart>())
                {
                    var instance = (IShoppingCart)Activator.CreateInstance(cartModule, null);
                    instance.UpdateCart(cart);
                }
            }

            if (cart != null && HttpContext.Current != null)
            {
                HttpContext.Current.Items[ShoppingCartContextKey + shoppingCartType] = cart;
            }

            return cart;
        }

        public static ShoppingCart GetShoppingCart(ShoppingCartType shoppingCartType, Guid customerId)
        {
            return GetShoppingCart(shoppingCartType, customerId, true);
        }

        public static ShoppingCart GetShoppingCart(ShoppingCartType shoppingCartType, Guid customerId, bool useCurrentCustomer)
        {
            var templist =
                SQLDataAccess.ExecuteReadList(
                    "SELECT * FROM Catalog.ShoppingCart WHERE ShoppingCartType = @ShoppingCartType and CustomerId = @CustomerId",
                    CommandType.Text, GetFromReader,
                    new SqlParameter("@ShoppingCartType", (int) shoppingCartType),
                    new SqlParameter("@CustomerId", customerId));

            var shoppingCart = useCurrentCustomer ? new ShoppingCart() : new ShoppingCart(customerId);
            shoppingCart.AddRange(templist);
            return shoppingCart;
        }

        public static ShoppingCart GetAllShoppingCarts(Guid customerId)
        {
            var shoppingCart = new ShoppingCart();

            foreach (ShoppingCartType shoppingCartType in Enum.GetValues(typeof(ShoppingCartType)))
            {
                shoppingCart.AddRange(GetShoppingCart(shoppingCartType, customerId));
            }

            return shoppingCart;
        }

        public static int AddShoppingCartItem(ShoppingCartItem item, bool useModule = true)
        {
            return AddShoppingCartItem(item, CustomerContext.CustomerId, useModule);
        }

        public static int AddShoppingCartItem(ShoppingCartItem item, Guid customerId, bool useModule = true)
        {
            var shoppingcartItemId = 0;
            item.CustomerId = customerId;

            var shoppingCartItem = GetExistsShoppingCartItem(customerId, item.OfferId, item.AttributesXml, item.ShoppingCartType, item.IsGift, item.ModuleKey);
            if (shoppingCartItem != null)
            {
                if (!shoppingCartItem.IsGift)
                {
                    shoppingCartItem.Amount += item.Amount;
                    UpdateShoppingCartItem(shoppingCartItem, useModule);
                    shoppingcartItemId = shoppingCartItem.ShoppingCartItemId;
                    useModule = false;
                }
            }
            else
            {
                InsertShoppingCartItem(item);
                shoppingcartItemId = item.ShoppingCartItemId;
            }

            if (HttpContext.Current != null)
                HttpContext.Current.Items[ShoppingCartContextKey + item.ShoppingCartType] = null;

            if (useModule)
            {
                foreach (var moduleShoppingCart in AttachedModules.GetModules<IShoppingCart>())
                {
                    var instance = (IShoppingCart) Activator.CreateInstance(moduleShoppingCart, null);
                    instance.AddToCart(item);
                }
            }

            return shoppingcartItemId;
        }

        public static void AddShoppingCartItems(string products)
        {
            if (string.IsNullOrWhiteSpace(products))
                return;

            foreach (var item in products.Split(";"))
            {
                int offerId;
                var newItem = new ShoppingCartItem() { CustomerId = CustomerContext.CustomerId };

                var parts = item.Split("-");
                if (parts.Length > 0 && (offerId = parts[0].TryParseInt(0)) != 0 && OfferService.GetOffer(offerId) != null)
                {
                    newItem.OfferId = offerId;
                }
                else
                {
                    continue;
                }

                newItem.Amount = parts.Length > 1 ? parts[1].TryParseFloat() : 1;

                var currentItem = ShoppingCartService.CurrentShoppingCart.FirstOrDefault(shpCartitem => shpCartitem.OfferId == newItem.OfferId);
                if (currentItem != null)
                {
                    currentItem.Amount = newItem.Amount;
                    UpdateShoppingCartItem(currentItem);
                }
                else
                {
                    AddShoppingCartItem(newItem);
                }
            }
        }

        public static ShoppingCartItem GetExistsShoppingCartItem(Guid customerId, int offerId, string attributesXml, ShoppingCartType shoppingCartType, bool isGift, string moduleKey)
        {
            return
                SQLDataAccess.ExecuteReadOne(
                    "SELECT * FROM [Catalog].[ShoppingCart] WHERE [CustomerId] = @CustomerId  AND [OfferId] = @OfferId AND [ShoppingCartType] = @ShoppingCartType AND [AttributesXml] = @AttributesXml AND IsGift = @IsGift " +
                    (moduleKey.IsNotEmpty() ? "AND ModuleKey = @ModuleKey" : "AND (ModuleKey = '' OR ModuleKey IS NULL)"),
                    CommandType.Text, GetFromReader,
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@OfferId", offerId),
                    new SqlParameter("@AttributesXml", attributesXml ?? String.Empty),
                    new SqlParameter("@IsGift", isGift),
                    new SqlParameter("@ModuleKey", moduleKey ?? (object)DBNull.Value),
                    new SqlParameter("@ShoppingCartType", (int) shoppingCartType));
        }

        private static void InsertShoppingCartItem(ShoppingCartItem item)
        {
            item.ShoppingCartItemId = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO Catalog.ShoppingCart (ShoppingCartType, CustomerId, OfferId, AttributesXml, Amount, CreatedOn, UpdatedOn, IsGift, ModuleKey, AddedByRequest, CustomPrice) " +
                "VALUES (@ShoppingCartType, @CustomerId, @OfferId, @AttributesXml, @Amount, GetDate(), GetDate(), @IsGift, @ModuleKey, @AddedByRequest, @CustomPrice); Select SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@ShoppingCartType", (int) item.ShoppingCartType),
                new SqlParameter("@CustomerId", item.CustomerId),
                new SqlParameter("@OfferId", item.OfferId),
                new SqlParameter("@AttributesXml", item.AttributesXml ?? String.Empty),
                new SqlParameter("@Amount", item.Amount),
                new SqlParameter("@IsGift", item.IsGift),
                new SqlParameter("@ModuleKey", item.ModuleKey ?? (object)DBNull.Value),
                new SqlParameter("@AddedByRequest", item.AddedByRequest),
                new SqlParameter("@CustomPrice", item.CustomPrice ?? (object)DBNull.Value));
        }

        public static void UpdateShoppingCartItem(ShoppingCartItem item, bool useModule = true)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[ShoppingCart] SET [ShoppingCartType] = @ShoppingCartType, [CustomerId] = @CustomerId, [OfferId] = @OfferId, [AttributesXml] = @AttributesXml, [UpdatedOn] = GetDate(), " +
                "[Amount] = @Amount, IsGift=@IsGift, ModuleKey=@ModuleKey, AddedByRequest=@AddedByRequest, CustomPrice=@CustomPrice " +
                "WHERE [ShoppingCartItemId] = @ShoppingCartItemId",
                CommandType.Text,
                new SqlParameter("@ShoppingCartType", (int) item.ShoppingCartType),
                new SqlParameter("@ShoppingCartItemId", item.ShoppingCartItemId),
                new SqlParameter("@CustomerId", item.CustomerId),
                new SqlParameter("@OfferId", item.OfferId),
                new SqlParameter("@AttributesXml", item.AttributesXml ?? String.Empty),
                new SqlParameter("@Amount", (decimal)item.Amount),
                new SqlParameter("@IsGift", item.IsGift),
                new SqlParameter("@ModuleKey", item.ModuleKey ?? (object)DBNull.Value),
                new SqlParameter("@AddedByRequest", item.AddedByRequest),
                new SqlParameter("@CustomPrice", item.CustomPrice ?? (object)DBNull.Value));

            if (useModule)
            {
                if (item.ShoppingCartType == ShoppingCartType.ShoppingCart)
                {
                    foreach (var cartModule in AttachedModules.GetModules<IShoppingCart>())
                    {
                        var instance = (IShoppingCart)Activator.CreateInstance(cartModule, null);
                        instance.UpdateCart(item);
                    }
                }
            }
        }

        public static void ClearShoppingCart(ShoppingCartType shoppingCartType)
        {
            ClearShoppingCart(shoppingCartType, CustomerContext.CustomerId);
        }

        public static void ClearShoppingCart(ShoppingCartType shoppingCartType, Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Catalog.ShoppingCart WHERE ShoppingCartType = @ShoppingCartType and CustomerId = @CustomerId",
                CommandType.Text,
                new SqlParameter("@ShoppingCartType", (int) shoppingCartType),
                new SqlParameter("@CustomerId", customerId));

            if (HttpContext.Current != null)
                HttpContext.Current.Items[ShoppingCartContextKey + shoppingCartType] = null;
        }

        public static void DeleteExpiredShoppingCartItems(DateTime olderThan)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.ShoppingCart WHERE CreatedOn<@olderThan",
                CommandType.Text, new SqlParameter("@olderThan", olderThan));
        }

        public static void DeleteShoppingCartItem(ShoppingCartItem cartItem, bool useModule = true)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Catalog.ShoppingCart WHERE ShoppingCartItemId = @ShoppingCartItemId", CommandType.Text,
                new SqlParameter("@ShoppingCartItemId", cartItem.ShoppingCartItemId));

            if (HttpContext.Current != null)
                HttpContext.Current.Items[ShoppingCartContextKey + cartItem.ShoppingCartType] = null;

            if (useModule)
            {
                if (cartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
                {
                    foreach (var cartModule in AttachedModules.GetModules<IShoppingCart>())
                    {
                        var instance = (IShoppingCart) Activator.CreateInstance(cartModule, null);
                        instance.RemoveFromCart(cartItem);
                    }
                }
            }
        }

        public static void MergeShoppingCarts(Guid oldCustomerId, Guid currentCustomerId)
        {
            if (oldCustomerId == currentCustomerId) 
                return;

            foreach (var item in GetAllShoppingCarts(oldCustomerId))
            {
                AddShoppingCartItem(item, currentCustomerId);
            }
        }

        public static Discount GetShoppingCartItemDiscount(int shoppingCartItemId)
        {
            foreach (var moduleDiscount in AttachedModules.GetModules<IDiscount>().Where(x => x != null))
            {
                var discount = ((IDiscount)Activator.CreateInstance(moduleDiscount)).GetCartItemDiscount(shoppingCartItemId);
                if (discount != null && discount.HasValue)
                    return discount;
            }
            return null;
        }

        private static ShoppingCartItem GetFromReader(SqlDataReader reader)
        {
            return new ShoppingCartItem
            {
                ShoppingCartItemId = SQLDataHelper.GetInt(reader, "ShoppingCartItemId"),
                ShoppingCartType = (ShoppingCartType)SQLDataHelper.GetInt(reader, "ShoppingCartType"),
                OfferId = SQLDataHelper.GetInt(reader, "OfferID"),
                CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                AttributesXml = SQLDataHelper.GetString(reader, "AttributesXml"),
                Amount = SQLDataHelper.GetFloat(reader, "Amount"),
                CreatedOn = SQLDataHelper.GetDateTime(reader, "CreatedOn"),
                UpdatedOn = SQLDataHelper.GetDateTime(reader, "UpdatedOn"),
                IsGift = SQLDataHelper.GetBoolean(reader, "IsGift"),
                ModuleKey = SQLDataHelper.GetString(reader, "ModuleKey"),
                AddedByRequest = SQLDataHelper.GetBoolean(reader, "AddedByRequest"),
                CustomPrice = SQLDataHelper.GetNullableFloat(reader, "CustomPrice"),
            };
        }

        public static void ResetHttpContentCard(ShoppingCartType shoppingCartType)
        {
            HttpContext.Current.Items.Remove(ShoppingCartContextKey + shoppingCartType);
        }

        public static bool ShoppingCartHasProductsWithGifts()
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT TOP(1) 1 from Catalog.ShoppingCart " +
                "INNER JOIN Catalog.Offer ON Offer.OfferId = ShoppingCart.OfferId " +
                "INNER JOIN Catalog.ProductGifts ON ProductGifts.ProductId = Offer.ProductId " +
                "WHERE ShoppingCart.CustomerId = @CustomerId AND ShoppingCartType = @ShoppingCartType AND IsGift = 0",
                CommandType.Text,
                new SqlParameter("CustomerId", CustomerContext.CustomerId),
                new SqlParameter("ShoppingCartType", ShoppingCartType.ShoppingCart)) > 0;
        }
    }
}