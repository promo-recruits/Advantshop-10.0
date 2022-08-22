//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace AdvantShop.ExportImport
{
    [ExportFeedKey("YandexMarket")]
    public class ExportFeedYandex : BaseExportFeed
    {
        private HashSet<int> _offerIds;
        private const string ZipPrefix = ".zip";

        //private readonly ExportFeedYandexDeliveryCostOption _localDeliveryOption;

        public ExportFeedYandex() : base()
        {
        }

        public ExportFeedYandex(bool useCommonStatistic) : base(useCommonStatistic)
        {
        }

        public static List<string> AvailableCurrencies
        {
            get { return new List<string> {"RUB", "RUR", "USD", "BYN", "KZT", "EUR", "UAH"}; }
        }

        public static List<string> AvailableEtalonCurrencies
        {
            get { return new List<string> {"RUB", "RUR", "BYN", "KZT", "UAH"}; }
        }

        public static List<string> AvailableFileExtentions
        {
            get { return new List<string> {"xml", "yml"}; }
        }

        private static void ProcessCurrency(List<Currency> currencies, string currency, XmlWriter writer)
        {
            if (currencies == null) return;
            var defaultCurrency =
                currencies.FirstOrDefault(item => item.Iso3 == currency && AvailableEtalonCurrencies.Contains(currency))
                ?? currencies.FirstOrDefault(item => AvailableEtalonCurrencies.Contains(item.Iso3));
            if (defaultCurrency == null) return;
            ProcessCurrencyRow(new Currency
            {
                CurrencyId = defaultCurrency.CurrencyId,
                Rate = 1,
                Iso3 = defaultCurrency.Iso3
            }, writer);

            foreach (var curRow in currencies.Where(item => item.Iso3 != defaultCurrency.Iso3))
            {
                curRow.Rate = Convert.ToSingle(curRow.Rate / defaultCurrency.Rate, CultureInfo.InvariantCulture);
                ProcessCurrencyRow(curRow, writer);
            }
        }

        private static void ProcessCurrencyRow(Currency currency, XmlWriter writer)
        {
            writer.WriteStartElement("currency");
            writer.WriteAttributeString("id", currency.Iso3);

            var nfi = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberDecimalSeparator = ".";
            writer.WriteAttributeString("rate", Math.Round(currency.Rate, 4).ToString(nfi));
            writer.WriteEndElement();
        }

        private static void ProcessCategoryRow(ExportFeedCategories row, XmlWriter writer)
        {
            writer.WriteStartElement("category");
            writer.WriteAttributeString("id", row.Id.ToString(CultureInfo.InvariantCulture));
            if (row.ParentCategory != 0)
            {
                writer.WriteAttributeString("parentId", row.ParentCategory.ToString(CultureInfo.InvariantCulture));
            }

            writer.WriteRaw(row.Name.XmlEncode().RemoveInvalidXmlChars());
            writer.WriteEndElement();
        }

        private void ProcessProductRow(ExportFeedYandexProduct row, XmlWriter writer, ExportFeedSettings commonSettings,
            Currency currency, bool productExistInPromos = false)
        {
            var advancedSettings = commonSettings.AdvancedSettingsObject;

            if (advancedSettings.NotExportAmountCount != null && advancedSettings.NotExportAmountCount.Value > 0 &&
                row.Amount < advancedSettings.NotExportAmountCount.Value)
            {
                return;
            }

            var showVendorModel = !advancedSettings.TypeExportYandex && !string.IsNullOrEmpty(row.BrandName);

            writer.WriteStartElement("offer");

            switch (advancedSettings.OfferIdType)
            {
                case "id":
                    writer.WriteAttributeString("id", row.OfferId.ToString(CultureInfo.InvariantCulture));
                    break;

                case "artno":
                    writer.WriteAttributeString("id", row.OfferArtNo.ToString(CultureInfo.InvariantCulture));
                    break;

                default:
                    writer.WriteAttributeString("id", row.OfferId.ToString(CultureInfo.InvariantCulture));
                    break;
            }

            if (row.Amount > 0 || row.AllowPreorder && (advancedSettings.AllowPreOrderProducts ?? true))
                writer.WriteAttributeString("available", (row.Amount > 0).ToString().ToLower());

            if (row.ColorId != 0 || row.SizeId != 0)
            {
                writer.WriteAttributeString("group_id", row.ProductId.ToString(CultureInfo.InvariantCulture));
            }

            if (showVendorModel)
            {
                writer.WriteAttributeString("type", "vendor.model");
            }

            if (row.Bid > 0)
            {
                writer.WriteAttributeString("bid", row.Bid.ToString(CultureInfo.InvariantCulture));
            }

            writer.WriteStartElement("url");
            writer.WriteRaw(CreateLink(row, commonSettings.AdditionalUrlTags));
            writer.WriteEndElement();

            float discount = 0;
            if (ProductDiscountModels != null)
            {
                var prodDiscount = ProductDiscountModels.Find(d => d.ProductId == row.ProductId);
                if (prodDiscount != null)
                {
                    discount = prodDiscount.Discount;
                }
            }

            var priceDiscount =
                discount > 0 && discount > row.Discount
                    ? new Discount(discount, 0)
                    : new Discount(row.Discount, row.DiscountAmount);

            var discountPrice = priceDiscount.Type == DiscountType.Percent
                ? row.Price * priceDiscount.Percent / 100
                : priceDiscount.Amount;

            var resultPrice =
                PriceService.RoundPrice(row.Price, currency, currency.Rate) -
                PriceService.RoundPrice(discountPrice, currency, currency.Rate);

            var markup = GetMarkup(resultPrice, commonSettings, row.CurrencyValue);

            var newPrice = (decimal)PriceService.RoundPrice(resultPrice + markup, currency, row.CurrencyValue);

            var markupOldPrice = GetMarkup(row.Price, commonSettings, row.CurrencyValue);

            var oldPrice = (decimal)PriceService.GetFinalPrice(row.Price + markupOldPrice, new Discount(), row.CurrencyValue, currency);

            var nfi = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberDecimalSeparator = ".";

            writer.WriteStartElement("price");
            if (productExistInPromos)
            {
                writer.WriteRaw(oldPrice.ToString(nfi));
            }
            else
            {
                switch (advancedSettings.ProductPriceType)
                {
                    case EExportFeedYandexPriceType.Both:
                    case EExportFeedYandexPriceType.WithDiscount:
                        writer.WriteRaw(newPrice.ToString(nfi));
                        break;
                    case EExportFeedYandexPriceType.WithoutDiscount:
                        writer.WriteRaw(oldPrice.ToString(nfi));
                        break;
                }
            }
            writer.WriteEndElement();

            if (!productExistInPromos && advancedSettings.ProductPriceType == EExportFeedYandexPriceType.Both && oldPrice - newPrice > 1)
            {
                
                writer.WriteStartElement("oldprice");
                writer.WriteRaw(oldPrice.ToString(nfi));
                writer.WriteEndElement();
            }
            
            if (advancedSettings.ExportPurchasePrice)
            {
                var purchasePrice = PriceService.RoundPrice(row.SupplyPrice, currency, row.CurrencyValue);
                writer.WriteStartElement("purchase_price");
                writer.WriteRaw(Convert.ToInt32(purchasePrice).ToString(nfi));
                writer.WriteEndElement();
            }

            writer.WriteStartElement("currencyId");
            writer.WriteRaw(advancedSettings.Currency);
            writer.WriteEndElement();

            writer.WriteStartElement("categoryId");
            writer.WriteRaw(row.ParentCategory.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            if (!string.IsNullOrEmpty(row.Photos))
            {
                if (advancedSettings.ExportAllPhotos)
                {
                    var temp = row.Photos.Split(',').Take(10);
                    foreach (var item in temp.Where(item => !string.IsNullOrWhiteSpace(item)))
                    {
                        writer.WriteStartElement("picture");
                        writer.WriteRaw(GetImageProductPath(item));
                        writer.WriteEndElement();
                    }
                }
                else
                {
                    writer.WriteStartElement("picture");
                    writer.WriteRaw(GetImageProductPath(row.Photos.Split(',')[0]));
                    writer.WriteEndElement();
                }
            }

            writer.WriteStartElement("store");
            writer.WriteRaw(advancedSettings.Store.ToString().ToLower());
            writer.WriteEndElement();

            writer.WriteStartElement("pickup");
            writer.WriteRaw(advancedSettings.Pickup.ToString().ToLower());
            writer.WriteEndElement();

            if (advancedSettings.ExportBarCode && !string.IsNullOrEmpty(row.BarCode) &&
                long.TryParse(row.BarCode, out _) &&
                (row.BarCode.Length == 8 || row.BarCode.Length == 12 || row.BarCode.Length == 13))
            {
                writer.WriteStartElement("barcode");
                writer.WriteRaw(row.BarCode.ToLower());
                writer.WriteEndElement();
            }

            writer.WriteStartElement("delivery");
            writer.WriteRaw(advancedSettings.Delivery.ToString().ToLower());
            writer.WriteEndElement();

            if (showVendorModel)
            {
                if (!string.IsNullOrEmpty(row.YandexTypePrefix))
                {
                    writer.WriteStartElement("typePrefix");
                    writer.WriteRaw(row.YandexTypePrefix);
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("vendor");
                writer.WriteRaw(row.BrandName.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();

                if (!string.IsNullOrEmpty(row.BrandCountryManufacture) || !string.IsNullOrEmpty(row.BrandCountry))
                {
                    var countryOfOrigin = !string.IsNullOrEmpty(row.BrandCountryManufacture) ? row.BrandCountryManufacture : row.BrandCountry;

                    writer.WriteStartElement("country_of_origin");
                    writer.WriteRaw(countryOfOrigin.XmlEncode().RemoveInvalidXmlChars());
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("model");
                writer.WriteRaw(GetProductName(row, advancedSettings.ColorSizeToName, true));
                writer.WriteEndElement();
            }
            else
            {
                writer.WriteStartElement("name");
                writer.WriteRaw(GetProductName(row, advancedSettings.ColorSizeToName, false));
                writer.WriteEndElement();
            }

            if (advancedSettings.ExportShopSku)
            {
                writer.WriteStartElement("shop-sku");
                writer.WriteRaw(
                    advancedSettings.OfferIdType == "artno"
                        ? row.OfferArtNo.ToString(CultureInfo.InvariantCulture)
                        : row.OfferId.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }

            if (advancedSettings.ExportManufacturer && !string.IsNullOrEmpty(row.BrandName))
            {
                writer.WriteStartElement("manufacturer");
                writer.WriteRaw(row.BrandName.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (advancedSettings.ExportCount)
            {
                writer.WriteStartElement("count");
                writer.WriteRaw(row.Amount.ToString(nfi));
                writer.WriteEndElement();
            }

            if (!string.IsNullOrEmpty(advancedSettings.ProductDescriptionType) &&
                !string.Equals(advancedSettings.ProductDescriptionType, "none"))
            {
                var desc = SQLDataHelper.GetString(advancedSettings.ProductDescriptionType == "full"
                    ? row.Description
                    : row.BriefDescription);
                if (!desc.IsNullOrEmpty())
                {
                    if (desc.Contains("userfiles/"))
                    {
                        desc = desc.Replace("\"userfiles/", "\"" + SettingsMain.SiteUrl + "/userfiles/")
                            .Replace("'userfiles/", "\'" + SettingsMain.SiteUrl + "/userfiles/")
                            .Replace("\"/userfiles/", "\"" + SettingsMain.SiteUrl + "/userfiles/")
                            .Replace("\'/userfiles/", "'" + SettingsMain.SiteUrl + "/userfiles/");
                    }

                    writer.WriteStartElement("description");
                    if (advancedSettings.RemoveHtml)
                    {
                        desc = StringHelper.RemoveHTML(desc);

                        writer.WriteRaw(desc.XmlEncode().RemoveInvalidXmlChars());
                    }
                    else
                    {
                        writer.WriteCData(desc.RemoveInvalidXmlChars());
                    }

                    writer.WriteEndElement();
                }
            }

            var vendorCode =
                advancedSettings.VendorCodeType == "offerArtno"
                    ? row.OfferArtNo.ToString(CultureInfo.InvariantCulture)
                    : row.ArtNo;

            writer.WriteStartElement("vendorCode");
            writer.WriteRaw(vendorCode.XmlEncode().RemoveInvalidXmlChars());
            writer.WriteEndElement();

            if (!string.IsNullOrWhiteSpace(row.SalesNote))
            {
                writer.WriteStartElement("sales_notes");
                writer.WriteRaw(row.SalesNote.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }
            else if (!string.IsNullOrWhiteSpace(advancedSettings.SalesNotes))
            {
                writer.WriteStartElement("sales_notes");
                writer.WriteRaw(advancedSettings.SalesNotes.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            var localDeliveryOption = advancedSettings.LocalDeliveryOptionObject;
            if (advancedSettings.Delivery &&
                advancedSettings.DeliveryCost == ExportFeedYandexDeliveryCost.LocalDeliveryCost &&
                row.ShippingPrice >= 0 && localDeliveryOption != null)
            {
                writer.WriteStartElement("delivery-options");
                writer.WriteStartElement("option");
                writer.WriteAttributeString("cost", Math.Round(row.ShippingPrice.Value).ToString(nfi));
                //condition same that in available
                if (row.Amount > 0)
                {
                    writer.WriteAttributeString("days",
                        !string.IsNullOrEmpty(row.YandexDeliveryDays)
                            ? row.YandexDeliveryDays
                            : localDeliveryOption.Days);

                    if (!string.IsNullOrEmpty(localDeliveryOption.OrderBefore))
                    {
                        writer.WriteAttributeString("order-before", localDeliveryOption.OrderBefore);
                    }
                }
                else
                    writer.WriteAttributeString("days", "");

                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            if (row.ManufacturerWarranty)
            {
                writer.WriteStartElement("manufacturer_warranty");
                writer.WriteRaw(row.ManufacturerWarranty.ToString().ToLower());
                writer.WriteEndElement();
            }

            if (row.Adult)
            {
                writer.WriteStartElement("adult");
                writer.WriteRaw(row.Adult.ToString().ToLower());
                writer.WriteEndElement();
            }

            if (row.Weight > 0)
            {
                writer.WriteStartElement("weight");
                var weight = Math.Round(row.Weight, 3, MidpointRounding.AwayFromZero);
                writer.WriteRaw((weight >= 0.001f ? weight : 0.001f).ToString("F3").Replace(",", ".").ToLower());
                writer.WriteEndElement();
            }

            if (advancedSettings.ExportRelatedProducts)
            {
                var recProducts = ProductService.GetRelatedProducts(row.ProductId, RelatedType.Related)
                    .Where(x => _offerIds.Any(y => y == x.OfferId)).ToList();
                var result = string.Empty;
                for (int index = 0; index < recProducts.Count && index < 30; ++index)
                {
                    result += (index > 0 && result != string.Empty ? "," : string.Empty) +
                              (advancedSettings.OfferIdType == "id"
                                  ? recProducts[index].OfferId.ToString(CultureInfo.InvariantCulture)
                                  : recProducts[index].ArtNo.ToString(CultureInfo.InvariantCulture));
                }

                if (!string.IsNullOrEmpty(result))
                {
                    writer.WriteStartElement("rec");
                    writer.WriteRaw(result);
                    writer.WriteEndElement();
                }
            }

            if (!string.IsNullOrWhiteSpace(row.ColorName))
            {
                writer.WriteStartElement("param");
                writer.WriteAttributeString("name", SettingsCatalog.ColorsHeader.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteRaw(row.ColorName.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (!string.IsNullOrWhiteSpace(row.SizeName))
            {
                writer.WriteStartElement("param");
                writer.WriteAttributeString("name", SettingsCatalog.SizesHeader.XmlEncode().RemoveInvalidXmlChars());
                if (!string.IsNullOrEmpty(row.YandexSizeUnit))
                {
                    writer.WriteAttributeString("unit", row.YandexSizeUnit);
                }

                writer.WriteRaw(row.SizeName.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (advancedSettings.ExportProductProperties)
            {
                var propertyValues = PropertyService.GetPropertyValuesByProductId(row.ProductId);

                if (advancedSettings.JoinPropertyValues)
                {
                    var newPropertyValues = new List<PropertyValue>();

                    foreach (var propertyValue in propertyValues)
                    {
                        var propertyId = propertyValue.PropertyId;

                        if (newPropertyValues.Any(x => x.PropertyId == propertyId))
                            continue;

                        propertyValue.Value = String.Join(", ",
                            propertyValues.Where(x => x.PropertyId == propertyId).Select(x => x.Value));

                        newPropertyValues.Add(propertyValue);
                    }

                    propertyValues = newPropertyValues;
                }

                foreach (var prop in propertyValues)
                {
                    if (prop.Property.Name.IsNotEmpty() && prop.Value.IsNotEmpty())
                    {
                        writer.WriteStartElement("param");
                        writer.WriteAttributeString("name", prop.Property.Name.XmlEncode().RemoveInvalidXmlChars());
                        if (!string.IsNullOrEmpty(prop.Property.Unit))
                        {
                            writer.WriteAttributeString("unit", prop.Property.Unit.XmlEncode().RemoveInvalidXmlChars());
                        }

                        writer.WriteRaw(prop.Value.XmlEncode().RemoveInvalidXmlChars());
                        writer.WriteEndElement();
                    }
                }
            }

            if (advancedSettings.ExportDimensions &&
                row.Length.HasValue && !row.Length.Value.IsDefault()
                && row.Height.HasValue && !row.Height.Value.IsDefault()
                && row.Width.HasValue && !row.Width.Value.IsDefault())
            {
                writer.WriteStartElement("dimensions");
                writer.WriteRaw(Math.Round(row.Length.Value / 10, 3).ToInvariantString() + "/"
                    + Math.Round(row.Width.Value / 10, 3).ToInvariantString() + "/"
                    + Math.Round(row.Height.Value / 10, 3).ToInvariantString());
                writer.WriteEndElement();
            }

            if (row.YandexProductDiscounted && row.YandexProductDiscountCondition != EYandexDiscountCondition.None)
            {
                writer.WriteStartElement("condition");
                writer.WriteAttributeString("type",
                    row.YandexProductDiscountCondition.StrName().XmlEncode().RemoveInvalidXmlChars());
                writer.WriteStartElement("reason");
                writer.WriteRaw(row.YandexProductDiscountReason.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void ProcessYandexPromoRow(ExportFeedYandexPromo promo, XmlWriter writer,
            ExportFeedSettings commonSettings, Currency currency, HashSet<int> categoryIds)
        {
            var advancedSettings = commonSettings.AdvancedSettingsObject;

            var coupon = new Coupon();

            if (promo.Type == YandexPromoType.PromoCode)
            {
                coupon = CouponService.GetCoupon(promo.CouponId);
            }

            writer.WriteStartElement("promo");
            writer.WriteAttributeString("id", BitConverter.ToInt64(promo.PromoID.ToByteArray(), 0).ToString());
            var type = promo.Type == YandexPromoType.PromoCode ? "promo code"
                : promo.Type == YandexPromoType.Flash ? "flash discount"
                : promo.Type == YandexPromoType.Gift ? "gift with purchase"
                : "n plus m";
            writer.WriteAttributeString("type", type);

            #region Dates

            if (promo.Type == YandexPromoType.PromoCode && coupon.ExpirationDate != null)
            {
                writer.WriteStartElement("start-date");
                writer.WriteRaw((coupon.StartDate ?? coupon.AddingDate).ToString("yyyy-MM-dd HH:mm:ss").XmlEncode()
                    .RemoveInvalidXmlChars());
                writer.WriteEndElement();
                writer.WriteStartElement("end-date");
                writer.WriteRaw(((DateTime) coupon.ExpirationDate).ToString("yyyy-MM-dd HH:mm:ss").XmlEncode()
                    .RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (promo.Type != YandexPromoType.PromoCode)
            {
                if (promo.StartDate != null && promo.ExpirationDate != null)
                {
                    writer.WriteStartElement("start-date");
                    writer.WriteRaw(((DateTime) promo.StartDate).ToString("yyyy-MM-dd HH:mm:ss").XmlEncode()
                        .RemoveInvalidXmlChars());
                    writer.WriteEndElement();
                    writer.WriteStartElement("end-date");
                    writer.WriteRaw(((DateTime) promo.ExpirationDate).ToString("yyyy-MM-dd HH:mm:ss").XmlEncode()
                        .RemoveInvalidXmlChars());
                    writer.WriteEndElement();
                }
            }

            #endregion Dates

            if (promo.Description != null)
            {
                writer.WriteStartElement("description");
                writer.WriteRaw(promo.Description.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (promo.PromoUrl != null)
            {
                writer.WriteStartElement("url");
                writer.WriteRaw(promo.PromoUrl.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (promo.Type == YandexPromoType.PromoCode)
            {
                writer.WriteStartElement("promo-code");
                writer.WriteRaw(coupon.Code.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
                writer.WriteStartElement("discount");
                var couponType = coupon.Type == CouponType.Fixed ? "currency" : "percent";
                writer.WriteAttributeString("unit", couponType);
                if (coupon.Type == CouponType.Fixed)
                    writer.WriteAttributeString("currency", coupon.CurrencyIso3);
                writer.WriteRaw(coupon.Value.ToInvariantString().XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            writer.WriteStartElement("purchase");
            if (promo.Type == YandexPromoType.PromoCode)
            {
                if (coupon.ProductsIds.Count > 0)
                {
                    foreach (var id in coupon.ProductsIds)
                    {
                        var offers = OfferService.GetProductOffers(id);
                        foreach (var offer in offers.Where(x => _offerIds.Any(y => y == x.OfferId)))
                        {
                            writer.WriteStartElement("product");
                            switch (advancedSettings.OfferIdType)
                            {
                                case "id":
                                    writer.WriteAttributeString("offer-id",
                                        offer.OfferId.ToString(CultureInfo.InvariantCulture));
                                    break;

                                case "artno":
                                    writer.WriteAttributeString("offer-id",
                                        offer.ArtNo.ToString(CultureInfo.InvariantCulture));
                                    break;

                                default:
                                    writer.WriteAttributeString("offer-id",
                                        offer.OfferId.ToString(CultureInfo.InvariantCulture));
                                    break;
                            }

                            writer.WriteEndElement();
                        }
                    }
                }

                if (coupon.CategoryIds.Count > 0)
                {
                    foreach (var id in coupon.CategoryIds.Intersect(categoryIds))
                    {
                        writer.WriteStartElement("product");
                        writer.WriteAttributeString("category-id", id.ToString());
                        writer.WriteEndElement();
                    }
                }

                if (coupon.ProductsIds.Count <= 0 && coupon.CategoryIds.Count <= 0)
                {
                    foreach (var categoryId in categoryIds)
                    {
                        writer.WriteStartElement("product");
                        writer.WriteAttributeString("category-id", categoryId.ToString());
                        writer.WriteEndElement();
                    }
                }
            }
            else
            {
                if (promo.Type == YandexPromoType.Gift || promo.Type == YandexPromoType.NPlusM)
                {
                    writer.WriteStartElement("required-quantity");
                    writer.WriteRaw(promo.RequiredQuantity.ToString().XmlEncode().RemoveInvalidXmlChars());
                    writer.WriteEndElement();
                }

                if (promo.Type == YandexPromoType.NPlusM)
                {
                    writer.WriteStartElement("free-quantity");
                    writer.WriteRaw(promo.FreeQuantity.ToString().XmlEncode().RemoveInvalidXmlChars());
                    writer.WriteEndElement();
                    foreach (var categoryID in promo.CategoryIDs)
                    {
                        writer.WriteStartElement("product");
                        writer.WriteAttributeString("category-id", categoryID.ToString(CultureInfo.InvariantCulture));
                        writer.WriteEndElement();
                    }
                }

                foreach (var productID in promo.ProductIDs)
                {
                    var product = ProductService.GetProduct(productID);
                    var offers = OfferService.GetProductOffers(productID);
                    foreach (var offer in offers.Where(x => _offerIds.Any(y => y == x.OfferId)))
                    {
                        writer.WriteStartElement("product");
                        switch (advancedSettings.OfferIdType)
                        {
                            case "id":
                                writer.WriteAttributeString("offer-id",
                                    offer.OfferId.ToString(CultureInfo.InvariantCulture));
                                break;

                            case "artno":
                                writer.WriteAttributeString("offer-id",
                                    offer.ArtNo.ToString(CultureInfo.InvariantCulture));
                                break;

                            default:
                                writer.WriteAttributeString("offer-id",
                                    offer.OfferId.ToString(CultureInfo.InvariantCulture));
                                break;
                        }

                        if (promo.Type == YandexPromoType.Flash)
                        {
                            writer.WriteStartElement("discount-price");
                            writer.WriteAttributeString("currency", advancedSettings.Currency);

                            float discount = 0;
                            if (ProductDiscountModels != null)
                            {
                                var prodDiscount = ProductDiscountModels.Find(d => d.ProductId == offer.ProductId);
                                if (prodDiscount != null)
                                {
                                    discount = prodDiscount.Discount;
                                }
                            }

                            var priceDiscount =
                                discount > 0 && discount > product.Discount.Amount
                                    ? new Discount(discount, 0)
                                    : product.Discount;
                            
                            var markup = offer.BasePrice * commonSettings.PriceMarginInPercents / 100 + commonSettings.PriceMarginInNumbers;

                            var newPrice = (decimal) PriceService.GetFinalPrice(offer.BasePrice + markup, priceDiscount, product.Currency.Rate, currency);
                            var nfi = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone();
                            nfi.NumberDecimalSeparator = ".";
                            writer.WriteRaw(newPrice.ToString(nfi));
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                }
            }

            writer.WriteEndElement();
            if (promo.Type == YandexPromoType.Gift)
            {
                writer.WriteStartElement("promo-gifts");
                writer.WriteStartElement("promo-gift");
                var offer = OfferService.GetOffer(promo.GiftID);
                if (_offerIds.All(x => x != offer.OfferId))
                {
                    writer.WriteAttributeString("gift-id", offer.OfferId.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    switch (advancedSettings.OfferIdType)
                    {
                        case "id":
                            writer.WriteAttributeString("offer-id",
                                offer.OfferId.ToString(CultureInfo.InvariantCulture));
                            break;

                        case "artno":
                            writer.WriteAttributeString("offer-id", offer.ArtNo.ToString(CultureInfo.InvariantCulture));
                            break;

                        default:
                            writer.WriteAttributeString("offer-id",
                                offer.OfferId.ToString(CultureInfo.InvariantCulture));
                            break;
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void RemoveNotAvailableYandexPromos(List<ExportFeedYandexPromo> promos, HashSet<int> categoryIds)
        {
            var promosToRemove = new List<Guid>();

            foreach (var promo in promos)
            {
                if (promo.Type == YandexPromoType.PromoCode)
                {
                    var coupon = CouponService.GetCoupon(promo.CouponId);
                    if (coupon == null || !coupon.Enabled || coupon.Code == null || coupon.Code.Length > 20 ||
                        coupon.ExpirationDate.HasValue && coupon.ExpirationDate < DateTime.Now ||
                        coupon.StartDate.HasValue && coupon.StartDate > DateTime.Now && !coupon.ExpirationDate.HasValue)
                    {
                        promosToRemove.Add(promo.PromoID);
                        continue;
                    }

                    var removePromo = true;
                    if (coupon.ProductsIds != null && coupon.ProductsIds.Count > 0)
                    {
                        foreach (var productId in coupon.ProductsIds)
                        {
                            var offers = OfferService.GetProductOffers(productId);
                            if (offers.Any(x => _offerIds.Any(y => y == x.OfferId)))
                            {
                                removePromo = false;
                                break;
                            }
                        }
                    }

                    if (coupon.CategoryIds != null && coupon.CategoryIds.Count > 0)
                    {
                        if (coupon.CategoryIds.Any(x => categoryIds.Any(y => y == x)))
                            removePromo = false;
                    }

                    if ((coupon.ProductsIds == null || coupon.ProductsIds.Count == 0) &&
                        (coupon.CategoryIds == null || coupon.CategoryIds.Count == 0))
                    {
                        removePromo = false;
                    }

                    if (removePromo)
                    {
                        promosToRemove.Add(promo.PromoID);
                    }
                }
                else
                {
                    var productsTemp = new List<int>();
                    if (promo.ProductIDs != null)
                    {
                        foreach (var productId in promo.ProductIDs)
                        {
                            var productOfferIds = OfferService.GetProductOffers(productId).Select(x => x.OfferId);
                            if (_offerIds.Any(x => productOfferIds.Any(y => y == x)))
                            {
                                if (promo.Type == YandexPromoType.Flash)
                                {
                                    var product = ProductService.GetProduct(productId);
                                    if (product.Discount.HasValue)
                                        productsTemp.Add(productId);
                                }
                                else
                                {
                                    productsTemp.Add(productId);
                                }
                            }
                        }
                    }

                    var categoriesTemp = new List<int>();
                    if (promo.CategoryIDs != null)
                    {
                        foreach (var categoryID in promo.CategoryIDs)
                        {
                            if (categoryIds.Any(x => x == categoryID))
                                categoriesTemp.Add(categoryID);
                        }
                    }

                    if (promo.Type == YandexPromoType.NPlusM)
                    {
                        if (productsTemp.Count == 0 && categoriesTemp.Count == 0)
                        {
                            promosToRemove.Add(promo.PromoID);
                            continue;
                        }
                    }
                    else if (productsTemp.Count == 0)
                    {
                        promosToRemove.Add(promo.PromoID);
                        continue;
                    }

                    if (promo.Type == YandexPromoType.Gift)
                    {
                        if (OfferService.GetOffer(promo.GiftID) == null)
                        {
                            promosToRemove.Add(promo.PromoID);
                            continue;
                        }
                    }

                    promo.CategoryIDs = categoriesTemp;
                    promo.ProductIDs = productsTemp;
                }
            }

            if (promosToRemove.Count > 0)
            {
                promos.RemoveAll(x => promosToRemove.IndexOf(x.PromoID) > -1);
            }
        }

        private void ProcessGiftRow(Offer offer, XmlWriter writer)
        {
            writer.WriteStartElement("gift");
            writer.WriteAttributeString("id", offer.OfferId.ToString(CultureInfo.InvariantCulture));
            writer.WriteStartElement("name");
            writer.WriteRaw(offer.Product.Name.XmlEncode().RemoveInvalidXmlChars());
            writer.WriteEndElement();
            writer.WriteStartElement("picture");
            writer.WriteRaw(GetImageProductPath(offer.Product.Photo));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void RemoveNotAvailableYandexGifts(HashSet<int> giftIds)
        {
            var idsToRemove = new List<int>();
            foreach (var id in giftIds)
            {
                var offer = OfferService.GetOffer(id);
                if (offer == null || _offerIds.Any(x => x == id))
                {
                    idsToRemove.Add(id);
                }
            }

            if (idsToRemove.Count > 0)
            {
                giftIds.RemoveWhere(x => idsToRemove.Any(y => y == x));
            }
        }

        private string CreateLink(ExportFeedYandexProduct row, string additionalUrlTags)
        {
            var suffix = string.Empty;
            if (!row.Main)
            {
                if (row.ColorId != 0)
                {
                    suffix = "color=" + row.ColorId;
                }

                if (row.SizeId != 0)
                {
                    if (string.IsNullOrEmpty(suffix))
                        suffix = "size=" + row.SizeId;
                    else
                        suffix += "&size=" + row.SizeId;
                }
            }

            var urlTags = GetAdditionalUrlTags(row, additionalUrlTags);
            if (!string.IsNullOrEmpty(urlTags))
            {
                suffix += !string.IsNullOrEmpty(suffix) ? "&" + urlTags : urlTags;
            }

            return HttpUtility.HtmlEncode(SettingsMain.SiteUrl.TrimEnd('/') + "/" +
                                          UrlService.GetLink(ParamType.Product, row.UrlPath, row.ProductId, suffix));
        }

        private string GetProductName(ExportFeedYandexProduct row, bool colorSizeToName, bool vendorModel)
        {
            var result = string.Empty;
            if (vendorModel && !row.YandexModel.IsNullOrEmpty())
            {
                result = row.YandexModel;
            }
            else
            {
                result = row.YandexName.IsNotEmpty() ? row.YandexName : row.Name;
            }

            if (colorSizeToName)
            {
                result +=
                    (!string.IsNullOrWhiteSpace(row.SizeName) ? " " + row.SizeName : string.Empty) +
                    (!string.IsNullOrWhiteSpace(row.ColorName) ? " " + row.ColorName : string.Empty);
            }

            return result.XmlEncode().RemoveInvalidXmlChars();
        }

        public override string Export(int exportFeedId)
        {
            try
            {
                var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
                var advancedSettings = commonSettings.AdvancedSettingsObject;

                var categories =
                    ExportFeedYandexService.GetCategories(exportFeedId, advancedSettings.ExportNotAvailable);
                var products = ExportFeedYandexService.GetProducts(exportFeedId, commonSettings, advancedSettings);
                _offerIds = ExportFeedYandexService.GetOfferIds(exportFeedId, commonSettings, advancedSettings);
                var categoriesCount =
                    ExportFeedYandexService.GetCategoriesCount(exportFeedId, advancedSettings.ExportNotAvailable);
                var productsCount =
                    ExportFeedYandexService.GetProductsCount(exportFeedId, commonSettings, advancedSettings);

                return Export(categories, products, commonSettings, categoriesCount, productsCount);
            }
            catch (Exception ex)
            {
                CsRowError(ex.Message);
                Debug.Log.Error(ex);
            }

            return null;
        }

        public override string Export(IEnumerable<ExportFeedCategories> categories,
            IEnumerable<ExportFeedProductModel> products, ExportFeedSettings options, int categoriesCount,
            int productsCount)
        {
            try
            {
                var advancedSettings = options.AdvancedSettingsObject;

                var currencies = CurrencyService.GetAllCurrencies()
                    .Where(item => AvailableCurrencies.Contains(item.Iso3)).ToList();

                var exportFile = new FileInfo(options.FileFullPath);
                if (!string.IsNullOrEmpty(exportFile.Directory?.FullName))
                {
                    FileHelpers.CreateDirectory(exportFile.Directory.FullName);
                }

                CsSetFileName("../" + options.FileFullName);

                FileHelpers.DeleteFile(exportFile.FullName + tempPrefix);

                using (var outputFile = new FileStream(exportFile.FullName + tempPrefix, FileMode.OpenOrCreate,
                    FileAccess.Write, FileShare.ReadWrite))
                {
                    var settings = new XmlWriterSettings {Encoding = Encoding.UTF8, Indent = true};
                    using (var writer = XmlWriter.Create(outputFile, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteDocType("yml_catalog", null, "shops.dtd", null);
                        writer.WriteStartElement("yml_catalog");
                        writer.WriteAttributeString("date", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        writer.WriteStartElement("shop");

                        writer.WriteStartElement("name");
                        writer.WriteRaw(advancedSettings.ShopName.Replace("#STORE_NAME#", SettingsMain.ShopName)
                            .XmlEncode());
                        writer.WriteEndElement();

                        writer.WriteStartElement("company");
                        writer.WriteRaw(advancedSettings.CompanyName.Replace("#STORE_NAME#", SettingsMain.ShopName)
                            .XmlEncode());
                        writer.WriteEndElement();

                        writer.WriteStartElement("url");
                        writer.WriteRaw(SettingsMain.SiteUrl);
                        writer.WriteEndElement();

                        if (!advancedSettings.DontExportCurrency)
                        {
                            writer.WriteStartElement("currencies");
                            ProcessCurrency(currencies, advancedSettings.Currency, writer);
                            writer.WriteEndElement();
                        }

                        CsSetTotalRow(categoriesCount + productsCount);

                        var categoryIds = new HashSet<int>();
                        writer.WriteStartElement("categories");
                        foreach (var categoryRow in categories)
                        {
                            ProcessCategoryRow(categoryRow, writer);
                            categoryIds.Add(categoryRow.Id);
                            CsNextRow();
                        }

                        writer.WriteEndElement();

                        if (advancedSettings.Delivery &&
                            advancedSettings.DeliveryCost != ExportFeedYandexDeliveryCost.None &&
                            !string.IsNullOrWhiteSpace(advancedSettings.GlobalDeliveryCost))
                        {
                            try
                            {
                                var deliveryOptions =
                                    JsonConvert.DeserializeObject<List<ExportFeedYandexDeliveryCostOption>>(
                                        advancedSettings.GlobalDeliveryCost);
                                if (deliveryOptions.Count != 0)
                                {
                                    writer.WriteStartElement("delivery-options");

                                    foreach (var deliveryOption in deliveryOptions)
                                    {
                                        writer.WriteStartElement("option");
                                        writer.WriteAttributeString("cost", deliveryOption.Cost);
                                        writer.WriteAttributeString("days", deliveryOption.Days);
                                        if (!string.IsNullOrEmpty(deliveryOption.OrderBefore))
                                        {
                                            writer.WriteAttributeString("order-before", deliveryOption.OrderBefore);
                                        }

                                        writer.WriteEndElement();
                                    }

                                    writer.WriteEndElement();
                                }
                            }
                            catch (Exception ex)
                            {
                                CsRowError(ex.Message);
                                Debug.Log.Error(ex);
                            }
                        }

                        var flashDiscountProducts = new HashSet<int>();
                        var giftProducts = new HashSet<int>();
                        var promos = new List<ExportFeedYandexPromo>();
                        if (!string.IsNullOrEmpty(advancedSettings.Promos))
                        {
                            promos = JsonConvert
                                .DeserializeObject<List<ExportFeedYandexPromo>>(advancedSettings.Promos);
                            RemoveNotAvailableYandexPromos(promos, categoryIds);
                        }

                        foreach (var promo in promos)
                        {
                            if (promo.Type == YandexPromoType.Flash)
                            {
                                foreach (var productId in promo.ProductIDs)
                                {
                                    flashDiscountProducts.Add(productId);
                                }
                            }

                            if (promo.Type == YandexPromoType.Gift)
                            {
                                giftProducts.Add(promo.GiftID);
                            }
                        }

                        writer.WriteStartElement("offers");
                        var currency = CurrencyService.GetCurrencyByIso3(advancedSettings.Currency);
                        foreach (ExportFeedYandexProduct offerRow in products)
                        {
                            if (flashDiscountProducts.Any(x => x == offerRow.ProductId))
                            {
                                ProcessProductRow(offerRow, writer, options, currency, true);
                            }
                            else
                            {
                                ProcessProductRow(offerRow, writer, options, currency);
                            }

                            CsNextRow();
                        }

                        writer.WriteEndElement();

                        RemoveNotAvailableYandexGifts(giftProducts);
                        if (giftProducts.Count > 0)
                        {
                            writer.WriteStartElement("gifts");
                            foreach (var id in giftProducts)
                            {
                                var offer = OfferService.GetOffer(id);
                                ProcessGiftRow(offer, writer);
                            }

                            writer.WriteEndElement();
                        }

                        if (promos.Count > 0)
                        {
                            writer.WriteStartElement("promos");
                            foreach (var promo in promos)
                            {
                                ProcessYandexPromoRow(promo, writer, options, currency, categoryIds);
                            }

                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();

                        writer.Flush();
                        writer.Close();
                    }
                }

                FileHelpers.ReplaceFile(exportFile.FullName + tempPrefix, exportFile.FullName);

                if (advancedSettings.NeedZip)
                {
                    FileHelpers.ZipFiles(exportFile.FullName, exportFile.FullName + ZipPrefix + tempPrefix);
                    FileHelpers.ReplaceFile(exportFile.FullName + ZipPrefix + tempPrefix,
                        exportFile.FullName + ZipPrefix);
                }
            }
            catch (Exception ex)
            {
                CsRowError(ex.Message);
                Debug.Log.Error(ex);
            }

            return options.FileFullName;
        }

        public override void SetDefaultSettings(int exportFeedId)
        {
            ExportFeedSettingsProvider.SetSettings(exportFeedId, new ExportFeedSettings
            {
                FileName = File.Exists(SettingsGeneral.AbsolutePath + "/export/yamarket.xml")
                    ? "export/yamarket" + exportFeedId
                    : "export/yamarket",
                FileExtention = AvailableFileExtentions[0],

                AdditionalUrlTags = string.Empty,

                Interval = 1,
                IntervalType = Core.Scheduler.TimeIntervalType.Days,
                JobStartTime = new DateTime(2017, 1, 1, 1, 0, 0),
                Active = false,

                AdvancedSettings = JsonConvert.SerializeObject(new ExportFeedYandexOptions
                {
                    CompanyName = "#STORE_NAME#",
                    ShopName = "#STORE_NAME#",
                    ProductDescriptionType = "short",
                    Currency = AvailableCurrencies[0],
                    GlobalDeliveryCost = "[]",
                    LocalDeliveryOption = "{\"Cost\":null,\"Days\":\"\",\"OrderBefore\":\"\"}",
                    RemoveHtml = true,
                    AllowPreOrderProducts = true,
                    Delivery = true,
                    TypeExportYandex = true,
                    ColorSizeToName = true,
                    ExportAllPhotos = true,
                    OfferIdType = "id",
                    VendorCodeType = "offerArtno"
                }),
                ExportAdult = true
            });
        }

        public override List<string> GetAvailableVariables()
        {
            return new List<string>
                {"#STORE_NAME#", "#STORE_URL#", "#PRODUCT_NAME#", "#PRODUCT_ID#", "#PRODUCT_ARTNO#"};
        }

        public override List<ExportFeedCategories> GetCategories(int exportFeedId)
        {
            throw new NotImplementedException();
        }

        public override List<ExportFeedProductModel> GetProducts(int exportFeedId)
        {
            throw new NotImplementedException();
        }

        public override int GetProductsCount(int exportFeedId)
        {
            var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
            var advancedSettings = commonSettings.AdvancedSettingsObject;

            return ExportFeedYandexService.GetProductsCount(exportFeedId, commonSettings, advancedSettings);
        }

        public override int GetCategoriesCount(int exportFeedId)
        {
            var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
            var advancedSettings = commonSettings.AdvancedSettingsObject;

            return ExportFeedYandexService.GetCategoriesCount(exportFeedId, advancedSettings.ExportNotAvailable);
        }

        public override List<string> GetAvailableFileExtentions()
        {
            return AvailableFileExtentions;
        }

        private Currency _markupCurrency;

        private Currency MarkupCurrency => _markupCurrency ??
                                           (_markupCurrency =
                                               CurrencyService.BaseCurrency ??
                                               CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3));

        /// <summary>
        /// Получить наценку
        /// </summary>
        private float GetMarkup(float price, ExportFeedSettings commonSettings, float renderCurrencyRate)
        {
            return price * commonSettings.PriceMarginInPercents / 100 +
                   (commonSettings.PriceMarginInNumbers > 0
                       ? commonSettings.PriceMarginInNumbers / renderCurrencyRate * MarkupCurrency.Rate
                       : 0);
        }
    }
}
