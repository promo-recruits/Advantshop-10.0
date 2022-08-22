//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Catalog
{
    public class OfferService
    {
        public static int AddOffer(Offer offer, bool trackChanges = false, ChangedBy changedBy = null)
        {
            if (trackChanges)
                ProductHistoryService.NewOffer(offer, changedBy);

            offer.OfferId =
                SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_AddOffer]", CommandType.StoredProcedure,
                                          new SqlParameter("@ProductID", offer.ProductId),
                                          new SqlParameter("@ArtNo", offer.ArtNo),
                                          new SqlParameter("@Amount", offer.Amount),
                                          new SqlParameter("@Price", offer.BasePrice),
                                          new SqlParameter("@SupplyPrice", offer.SupplyPrice),
                                          new SqlParameter("@ColorID", offer.ColorID ?? (object)DBNull.Value),
                                          new SqlParameter("@SizeID", offer.SizeID ?? (object)DBNull.Value),
                                          new SqlParameter("@Main", offer.Main),
                                          new SqlParameter("@Weight", offer.Weight ?? (object)DBNull.Value),
                                          new SqlParameter("@Length", offer.Length ?? (object)DBNull.Value),
                                          new SqlParameter("@Width", offer.Width ?? (object)DBNull.Value),
                                          new SqlParameter("@Height", offer.Height ?? (object)DBNull.Value),
                                          new SqlParameter("@BarCode", offer.BarCode ?? (object)DBNull.Value)
                                          );
            return offer.OfferId;
        }

        public static void UpdateOffer(Offer offer, bool trackChanges = false, ChangedBy changedBy = null)
        {
            if (trackChanges)
                ProductHistoryService.TrackOfferChanges(offer, changedBy);

            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateOffer]", CommandType.StoredProcedure,
                                            new SqlParameter("@OfferId", offer.OfferId),
                                            new SqlParameter("@ProductID", offer.ProductId),
                                            new SqlParameter("@ArtNo", offer.ArtNo),
                                            new SqlParameter("@Amount", offer.Amount),
                                            new SqlParameter("@Price", offer.BasePrice),
                                            new SqlParameter("@SupplyPrice", offer.SupplyPrice),
                                            new SqlParameter("@ColorID", offer.ColorID ?? (object)DBNull.Value),
                                            new SqlParameter("@SizeID", offer.SizeID ?? (object)DBNull.Value),
                                            new SqlParameter("@Main", offer.Main),
                                            new SqlParameter("@Weight", offer.Weight ?? (object)DBNull.Value),
                                            new SqlParameter("@Length", offer.Length ?? (object)DBNull.Value),
                                            new SqlParameter("@Width", offer.Width ?? (object)DBNull.Value),
                                            new SqlParameter("@Height", offer.Height ?? (object)DBNull.Value),
                                            new SqlParameter("@BarCode", offer.BarCode ?? (object)DBNull.Value)
                                            );
        }

        public static void DeleteOffer(int offerId, bool trackChanges = false, ChangedBy changedBy = null)
        {
            if (trackChanges)
                ProductHistoryService.DeleteOffer(offerId, changedBy);

            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_RemoveOffer]",
                                          CommandType.StoredProcedure,
                                          new SqlParameter("@OfferID", offerId));
        }


        public static void DeleteOldOffers(int productId, List<Offer> newOffers, bool trackChanges = false, ChangedBy changedBy = null)
        {
            if (newOffers == null || !newOffers.Any())
            {
                GetProductOffers(productId).ForEach(offer => DeleteOffer(offer.OfferId, trackChanges, changedBy));
            }
            else
            {
                var currentOffers = GetProductOffers(productId);
                currentOffers
                    .Where(offer => !newOffers.Any(x => x.OfferId == offer.OfferId))
                    .ForEach(offer => DeleteOffer(offer.OfferId, trackChanges, changedBy));
            }
        }

        public static void SetMainOfferForProductsWithoutMainOffer()
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Catalog].[Offer]
                  SET [Main] = 1
                  WHERE NOT EXISTS(SELECT * FROM [Catalog].[Offer] AS OF2 WHERE OF2.[ProductID] = [Offer].[ProductId] AND OF2.[Main] = 1)
	                    AND [OfferID] IN (SELECT TOP (1) OF2.[OfferID] FROM [Catalog].[Offer] AS OF2 WHERE OF2.[ProductID] = [Offer].[ProductId])",
                CommandType.Text);
        }

        public static List<Offer> GetProductOffers(int productId)
        {
            return SQLDataAccess.ExecuteReadList<Offer>(
                     "SELECT Offer.*, CurrencyValue " +
                     "FROM Catalog.Offer " +
                     "Inner Join Catalog.Product on Offer.ProductID = Product.ProductID " +
                     "Inner Join Catalog.Currency on Currency.CurrencyID = Product.CurrencyID " +
                     "WHERE Offer.ProductID = @ProductID",
                     CommandType.Text,
                     GetOfferFromReader,
                     new SqlParameter("@ProductID", productId));
        }

        public static Offer GetOffer(int offerId)
        {
            return SQLDataAccess.ExecuteReadOne<Offer>(
                     "SELECT Offer.*, CurrencyValue " +
                     "FROM Catalog.Offer " +
                     "Inner Join Catalog.Product on Offer.ProductID = Product.ProductID " +
                     "Inner Join Catalog.Currency on Currency.CurrencyID = Product.CurrencyID " +
                     "WHERE [offerID] = @offerID",
                     CommandType.Text,
                     GetOfferFromReader,
                     new SqlParameter("@offerID", offerId));
        }

        public static Offer GetOffer(string artNo)
        {
            return SQLDataAccess.ExecuteReadOne<Offer>(
                     "SELECT Offer.*, CurrencyValue " +
                     "FROM Catalog.Offer " +
                     "Inner Join Catalog.Product on Offer.ProductID = Product.ProductID " +
                     "Inner Join Catalog.Currency on Currency.CurrencyID = Product.CurrencyID " +
                     "WHERE Offer.ArtNo = @ArtNo",
                     CommandType.Text,
                     GetOfferFromReader,
                     new SqlParameter("@ArtNo", artNo));
        }

        public static Offer GetMainOffer(List<Offer> offers, bool allowPreOrder, int? colorid = null, int? sizeId = null)
        {
            if (offers == null || !offers.Any())
                return null;

            Offer CurrentOffer = null;

            // сначала доступные к покупке
            if (colorid.HasValue && sizeId.HasValue)
            {
                CurrentOffer = GetMainOffer(offers, o => o.ColorID == colorid && o.SizeID == sizeId);
            }
            else if (colorid.HasValue)
            {
                CurrentOffer = GetMainOffer(offers, o => o.ColorID == colorid);
            }
            else if (sizeId.HasValue)
            {
                CurrentOffer = GetMainOffer(offers, o => o.SizeID == sizeId);
            }

            if (CurrentOffer == null)
            {
                CurrentOffer = GetMainOffer(offers, o => IsAvailableOffer(o, false))
                            ?? GetMainOffer(offers, o => IsAvailableOffer(o, allowPreOrder)); // сначала доступные к покупке, потом под заказ
            }

            return CurrentOffer ?? offers.FirstOrDefault();
        }

        private static Offer GetMainOffer(List<Offer> offers, Func<Offer, bool> condition)
        {
            return offers.OrderByDescending(o=> o.Amount > 0 && o.BasePrice > 0).ThenByDescending(o => o.Main)
                .ThenByDescending(o => o.ColorID == offers.FirstOrDefault(x=>x.Main)?.ColorID)
                .ThenBy(o => o.Color != null ? o.Color.SortOrder : 0)
                .ThenBy(o => o.Color != null ? o.Color.ColorName : "")
                .ThenBy(o => o.Size != null ? o.Size.SortOrder : 0)
                .ThenBy(o => o.Size != null ? o.Size.SizeName : "")
                .FirstOrDefault(condition);
        }

        public static Offer GetMainOfferForExport(int productId)
        {
            return SQLDataAccess.ExecuteReadOne<Offer>(
                     "SELECT top(1) Offer.*, CurrencyValue " +
                     "FROM Catalog.Offer " +
                     "Inner Join Catalog.Product on Offer.ProductID = Product.ProductID " +
                     "Inner Join Catalog.Currency on Currency.CurrencyID = Product.CurrencyID " +
                     "WHERE Offer.ProductID = @ProductID and Main=1",
                     CommandType.Text,
                     GetOfferFromReader,
                     new SqlParameter("@ProductID", productId));
        }

        private static bool IsAvailableOffer(Offer o, bool allowPreOrder)
        {
            return (o.RoundedPrice > 0 && o.Amount > 0) || SettingsCheckout.OutOfStockAction == eOutOfStockAction.Cart; // || allowPreOrder
        }

        private static Offer GetOfferFromReader(SqlDataReader reader)
        {
            var offer = new Offer
            {
                ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                BasePrice = SQLDataHelper.GetFloat(reader, "Price"),
                Amount = SQLDataHelper.GetFloat(reader, "Amount"),
                SupplyPrice = SQLDataHelper.GetFloat(reader, "SupplyPrice"),
                ProductId = SQLDataHelper.GetInt(reader, "ProductID"),
                OfferId = SQLDataHelper.GetInt(reader, "OfferID"),
                ColorID = SQLDataHelper.GetNullableInt(reader, "ColorID"),
                SizeID = SQLDataHelper.GetNullableInt(reader, "SizeID"),
                Main = SQLDataHelper.GetBoolean(reader, "Main"),
                Weight = SQLDataHelper.GetNullableFloat(reader, "Weight"),
                Length = SQLDataHelper.GetNullableFloat(reader, "Length"),
                Width = SQLDataHelper.GetNullableFloat(reader, "Width"),
                Height = SQLDataHelper.GetNullableFloat(reader, "Height"),
                BarCode = SQLDataHelper.GetString(reader, "BarCode"),
            };

            return offer;
        }

        public static bool IsArtNoExist(string artNo, int offerId)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "Select Count(OfferID) from Catalog.Offer Where ArtNo=@ArtNo and OfferID<>@OfferID",
                    CommandType.Text, new SqlParameter("@ArtNo", artNo),
                    new SqlParameter("@offerID", offerId)
                    ) > 0;
        }

        public static string OffersToString(List<Offer> offers, string columSeparator, string propertySeparator)
        {
            return offers.OrderByDescending(o => o.Main).Select(offer =>
                                 offer.ArtNo + propertySeparator + (offer.Size != null ? offer.Size.SizeName : "null") + propertySeparator +
                                 (offer.Color != null ? offer.Color.ColorName : "null") + propertySeparator + offer.BasePrice +
                                 propertySeparator + offer.SupplyPrice + propertySeparator + offer.Amount).AggregateString(columSeparator);
        }

        public static void OffersFromString(Product product, string offersString, string columSeparator, string propertySeparator, bool importRemains = false)
        {
            if (string.IsNullOrEmpty(columSeparator) || string.IsNullOrEmpty(propertySeparator))
                _OffersFromString(product, offersString);
            else
                _OffersFromString(product, offersString, columSeparator, propertySeparator, importRemains);
        }

        private static void _OffersFromString(Product product, string offersString)
        {
            product.HasMultiOffer = true;

            var oldOffers = new List<Offer>(product.Offers);
            product.Offers.Clear();

            var mainOffer = true;

            foreach (string[] fields in offersString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Replace("[", "").Replace("]", "").Split(':')))
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    fields[i] = fields[i].SupperTrim();
                }

                if (fields.Count() == 6)
                {
                    var multiOffer = oldOffers.FirstOrDefault(offer => offer.ArtNo == fields[0]) ?? new Offer();
                    multiOffer.ProductId = product.ProductId;
                    multiOffer.Main = mainOffer;

                    multiOffer.ArtNo = fields[0]; // ArtNo

                    if (fields[1] != "null") // Size
                    {
                        Size size = SizeService.GetSize(fields[1]);
                        if (size == null)
                        {
                            size = new Size { SizeName = fields[1] };
                            size.SizeId = SizeService.AddSize(size);
                        }

                        multiOffer.SizeID = size.SizeId;
                    }
                    else
                    {
                        multiOffer.SizeID = null;
                    }

                    if (fields[2] != "null") // Color
                    {
                        Color color = ColorService.GetColor(fields[2]);
                        if (color == null)
                        {
                            color = new Color { ColorName = fields[2], ColorCode = "#000000" };
                            color.ColorId = ColorService.AddColor(color);
                        }

                        multiOffer.ColorID = color.ColorId;
                    }
                    else
                    {
                        multiOffer.ColorID = null;
                    }


                    multiOffer.BasePrice = fields[3].TryParseFloat(); // Price
                    multiOffer.SupplyPrice = fields[4].TryParseFloat(); // SupplyPrice
                    multiOffer.Amount = fields[5].TryParseFloat(); //Amount

                    product.Offers.Add(multiOffer);
                    mainOffer = false;
                }
            }
        }

        private static void _OffersFromString(Product product, string offersString, string columSeparator, string propertySeparator, bool importRemains)
        {
            product.HasMultiOffer = true;

            var oldOffers = new List<Offer>(product.Offers);
            product.Offers.Clear();

            var mainOffer = true;

            foreach (string[] fields in offersString.Replace("[", "").Replace("]", "")
                .Split(new[] { columSeparator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(str => str.Split(new[] { propertySeparator }, StringSplitOptions.RemoveEmptyEntries)))
            {

                for (int i = 0; i < fields.Length; i++)
                {
                    fields[i] = fields[i].SupperTrim();
                }

                if (fields.Length == 6)
                {
                    var multiOffer = oldOffers.FirstOrDefault(offer => offer.ArtNo.ToLower() == fields[0].ToLower()) ?? new Offer();
                    multiOffer.ProductId = product.ProductId;
                    multiOffer.Main = mainOffer;

                    multiOffer.ArtNo = fields[0]; // ArtNo

                    if (fields[1] != "null") // Size
                    {
                        Size size = SizeService.GetSize(fields[1]);
                        if (size == null)
                        {
                            size = new Size { SizeName = fields[1] };
                            size.SizeId = SizeService.AddSize(size);
                        }

                        multiOffer.SizeID = size.SizeId;
                    }
                    else
                    {
                        multiOffer.SizeID = null;
                    }

                    if (fields[2] != "null") // Color
                    {
                        Color color = ColorService.GetColor(fields[2]);
                        if (color == null)
                        {
                            color = new Color { ColorName = fields[2], ColorCode = "#000000" };
                            color.ColorId = ColorService.AddColor(color);
                        }

                        multiOffer.ColorID = color.ColorId;
                    }
                    else
                    {
                        multiOffer.ColorID = null;
                    }

                    multiOffer.BasePrice = fields[3].TryParseFloat(); // Price
                    multiOffer.SupplyPrice = fields[4].TryParseFloat(); // SupplyPrice

                    if (importRemains)
                    {
                        multiOffer.Amount = multiOffer.Amount + fields[5].TryParseFloat() > 0
                            ? multiOffer.Amount + fields[5].TryParseFloat()
                            : 0;

                    }
                    else
                    {
                        multiOffer.Amount = fields[5].TryParseFloat();
                    }
                    //Amount

                    product.Offers.Add(multiOffer);
                    mainOffer = false;
                }
            }
        }

        public static void OfferFromFields(Product product, float? price, float? purchase, float? amount, bool importRemains = false)
        {

            if (price == null && purchase == null && amount == null)
                return;

            product.HasMultiOffer = false;

            var singleOffer = product.Offers.FirstOrDefault() ?? new Offer();
            product.Offers.Clear();

            singleOffer.ArtNo = product.ArtNo;
            singleOffer.Main = true;
            singleOffer.ProductId = product.ProductId;
            singleOffer.BasePrice = price ?? singleOffer.BasePrice;
            singleOffer.SupplyPrice = purchase ?? singleOffer.SupplyPrice;

            if (amount.HasValue)
            {
                if (importRemains)
                {
                    singleOffer.Amount = singleOffer.Amount + amount.Value > 0
                        ? singleOffer.Amount + amount.Value
                        : 0;
                }
                else
                {
                    singleOffer.Amount = amount.Value;
                }

                //проблема в том что некоторые добавлЯют 1 млрд и может не работать выгрузка в маркет  taskid 10451
                singleOffer.Amount = singleOffer.Amount > 1000000 ? 1000000 : singleOffer.Amount;
            }
            product.Offers.Add(singleOffer);
        }


        #region Product Gifts

        public static List<GiftModel> GetProductGifts(int productId, bool onlyAvailable = true)
        {
            return SQLDataAccess.Query<GiftModel>(
                     "SELECT OfferID, Offer.ProductID, Offer.ArtNo, Price, Amount, SupplyPrice, ColorID, SizeID, Main, CurrencyValue, ProductGifts.ProductCount " +
                     "FROM Catalog.Offer " +
                     "Inner Join Catalog.ProductGifts on ProductGifts.GiftOfferId = Offer.OfferId " +
                     "Inner Join Catalog.Product on Offer.ProductID = Product.ProductID " +
                     "Inner Join Catalog.Currency on Currency.CurrencyID = Product.CurrencyID " +
                     "WHERE ProductGifts.ProductId = @ProductId" +
                     (onlyAvailable ? " and Product.Enabled = 1 and Offer.Amount > 0" : string.Empty),
                     new { ProductId = productId }).ToList();
        }

        public static void AddProductGift(int productId, int giftOfferId, int productCount)
        {
            if (IsExistProductGift(productId, giftOfferId))
                return;
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO Catalog.ProductGifts (ProductId, GiftOfferId, ProductCount) VALUES (@ProductId, @GiftOfferId, @ProductCount)",
                CommandType.Text, 
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@GiftOfferId", giftOfferId),
                new SqlParameter("@ProductCount", productCount));
        }

        public static void UpdateProductGift(int productId, int giftOfferId, int productCount)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Catalog.ProductGifts SET ProductCount = @ProductCount WHERE ProductId = @ProductId AND GiftOfferId = @GiftOfferId",
                CommandType.Text,
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@GiftOfferId", giftOfferId),
                new SqlParameter("@ProductCount", productCount));
        }

        public static void DeleteProductGift(int productId, int giftOfferId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Catalog.ProductGifts WHERE ProductId = @ProductId AND GiftOfferId = @GiftOfferId",
                CommandType.Text, new SqlParameter("@ProductId", productId),
                new SqlParameter("@GiftOfferId", giftOfferId));
        }

        public static void ClearProductGifts(int productId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Catalog.ProductGifts WHERE ProductId = @ProductId",
                CommandType.Text, new SqlParameter("@ProductId", productId));
        }

        public static bool IsExistProductGift(int productId, int giftOfferId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM Catalog.ProductGifts WHERE ProductId = @ProductId AND GiftOfferId = @GiftOfferId",
                CommandType.Text, new SqlParameter("@ProductId", productId),
                new SqlParameter("@GiftOfferId", giftOfferId)) > 0;
        }

        public static Dictionary<int, int> GetGiftProducts(int giftOfferId)
        {
            return SQLDataAccess.ExecuteReadDictionary<int, int>(
                "SELECT ProductId, ProductCount FROM Catalog.ProductGifts WHERE GiftOfferId = @GiftOfferId",
                CommandType.Text, "ProductId", "ProductCount",
                new SqlParameter("@GiftOfferId", giftOfferId));
        }

        public static string ProductGiftsToString(int productId, string columnSeparator)
        {
            var items = SQLDataAccess.ExecuteReadColumn<string>(
                "Select ArtNo from Catalog.Offer inner join Catalog.ProductGifts on ProductGifts.GiftOfferId = Offer.OfferId where ProductGifts.ProductId = @productId",
                CommandType.Text, "ArtNo", new SqlParameter("@productId", productId));
            return items.AggregateString(columnSeparator);
        }

        public static bool ProductGiftsFromString(int productId, string value, string columnSeparator)
        {
            ClearProductGifts(productId);

            if (string.IsNullOrEmpty(value))
                return true;

            var arrArt = value.Split(new[] { columnSeparator }, StringSplitOptions.None);
            foreach (string t in arrArt)
            {
                var artNo = t.Trim();
                if (string.IsNullOrWhiteSpace(artNo))
                    continue;
                var offer = GetOffer(artNo);
                if (offer != null)
                    AddProductGift(productId, offer.OfferId, 1);
            }
            return true;
        }

        #endregion
    }
}