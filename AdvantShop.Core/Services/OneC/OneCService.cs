using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.ExportImport;
using AdvantShop.Repository.Currencies;
using AdvantShop.SEO;

namespace AdvantShop.Core.Services.OneC
{
    public class OneCService
    {
        public static IEnumerable<ExportFeedCsvProduct> GetProducts(DateTime fromDate, DateTime toDate, bool onlyFromOrders, string csvColumSeparator, string csvPropertySeparator)
        {
            var subQuery = new List<string>();
            var sqlParams = new List<SqlParameter>();

            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
            {
                subQuery.Add("DateModified >= @From and DateModified <= @To");

                sqlParams.Add(new SqlParameter("@From", fromDate));
                sqlParams.Add(new SqlParameter("@To", toDate));
            }

            if (onlyFromOrders)
                subQuery.Add("EXISTS(SELECT 1 FROM [Order].[OrderItems] WHERE [OrderItems].[ProductID] = [Product].[ProductID])");

            return SQLDataAccess.ExecuteReadIEnumerable(
                "SELECT * FROM [Catalog].[Product] LEFT JOIN [Catalog].[Photo] ON [Photo].[ObjId] = [Product].[ProductID] AND Type = 'Product' AND Photo.[Main] = 1" + 
                    (subQuery.Count > 0 ? " WHERE " + string.Join(" AND ", subQuery.Select(query => string.Format("({0})", query))) : string.Empty),
                CommandType.Text,
                 reader =>
                 {
                     var product = ProductService.GetProductFromReader(reader);
                     var offerForExport = OfferService.GetMainOfferForExport(product.ProductId);

                     var productCsv = new ExportFeedCsvProduct
                     {
                         ProductId = product.ProductId,
                         ArtNo = product.ArtNo,
                         Name = product.Name,
                         UrlPath = product.UrlPath,
                         Enabled = product.Enabled ? "+" : "-",
                         Unit = product.Unit,
                         ShippingPrice = product.ShippingPrice == null ? "" : product.ShippingPrice.Value.ToString("F2"),
                         Discount = product.Discount.Percent.ToString("F2"),
                         DiscountAmount = product.Discount.Amount.ToString("F2"),
                         Weight = (offerForExport?.GetWeight() ?? 0).ToFormatString(),
                         Size = (offerForExport != null ? offerForExport.GetDimensions() : "0x0x0"),

                         BriefDescription = product.BriefDescription,
                         Description = product.Description,
                         Producer = BrandService.BrandToString(product.BrandId),

                         Category =
                             CategoryService.GetCategoryStringByProductId(product.ProductId,
                                 csvColumSeparator),
                         Sorting =
                             CategoryService.GetCategoryStringByProductId(product.ProductId,
                                 csvColumSeparator, true),
                         Currency = CurrencyService.GetCurrency(product.CurrencyID).Iso3,
                         Markers = ProductService.MarkersToString(
                             product.BestSeller,
                             product.New,
                             product.Recomended,
                             product.OnSale,
                             csvColumSeparator),
                         Photos = PhotoService.PhotoToString(
                             PhotoService.GetPhotos<ProductPhoto>(product.ProductId,
                                 PhotoType.Product),
                             csvColumSeparator,
                             csvPropertySeparator),
                         Videos = ProductVideoService.VideoToString(
                             ProductVideoService.GetProductVideos(product.ProductId)),
                         Properties = PropertyService.PropertiesToString(
                             PropertyService.GetPropertyValuesByProductId(product.ProductId),
                             csvColumSeparator,
                             csvPropertySeparator),

                         OrderByRequest = product.AllowPreOrder ? "+" : "-",

                         Related =
                             ProductService.LinkedProductToString(product.ProductId,
                                 RelatedType.Related, csvColumSeparator),
                         Alternative =
                             ProductService.LinkedProductToString(product.ProductId,
                                 RelatedType.Alternative, csvColumSeparator),
                         CustomOption =
                             CustomOptionsService.CustomOptionsToString(
                                 CustomOptionsService.GetCustomOptionsByProductId(product.ProductId)),

                         SalesNote = product.SalesNote,
                         Gtin = product.Gtin,
                         GoogleProductCategory = product.GoogleProductCategory,
                         YandexTypePrefix = product.YandexTypePrefix,
                         YandexModel = product.YandexModel,
                         Adult = product.Adult ? "+" : "-",
                         ManufacturerWarranty = product.ManufacturerWarranty ? "+" : "-",

                         Gifts = OfferService.ProductGiftsToString(product.ProductId, csvColumSeparator),
                     };

                     var meta = MetaInfoService.GetMetaInfo(productCsv.ProductId, MetaType.Product) ??
                                    new MetaInfo(0, 0, MetaType.Product, string.Empty, string.Empty, string.Empty, string.Empty);

                     productCsv.Title = meta.Title;
                     productCsv.H1 = meta.H1;
                     productCsv.MetaKeywords = meta.MetaKeywords;
                     productCsv.MetaDescription = meta.MetaDescription;

                     if (!product.HasMultiOffer && product.Offers.All(offer => offer.ArtNo == product.ArtNo) )
                     {
                         var offer = product.Offers.FirstOrDefault() ?? new Offer();
                         productCsv.Price = offer.BasePrice.ToString("F2");
                         productCsv.PurchasePrice = offer.SupplyPrice.ToString("F2");
                         productCsv.Amount = offer.Amount.ToString(CultureInfo.InvariantCulture);
                         productCsv.MultiOffer = string.Empty;
                     }
                     else
                     {
                         productCsv.Price = string.Empty;
                         productCsv.PurchasePrice = string.Empty;
                         productCsv.Amount = string.Empty;
                         productCsv.MultiOffer = OfferService.OffersToString(product.Offers, csvColumSeparator, csvPropertySeparator);
                     }

                     return productCsv;
                 },
                 sqlParams.ToArray());
        }
    }
}
