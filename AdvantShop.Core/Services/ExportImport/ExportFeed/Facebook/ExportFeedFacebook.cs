using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
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

namespace AdvantShop.ExportImport
{
    [ExportFeedKey("Facebook")]
    public class ExportFeedFacebook : BaseExportFeed
    {
        private const string GoogleBaseNamespace = "http://base.google.com/ns/1.0";

        public ExportFeedFacebook() : base() { }

        public ExportFeedFacebook(bool useCommonStatistic) : base(useCommonStatistic) { }

        public override string Export(int exportFeedId)
        {
            try
            {
                var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
                var advancedSettings = JsonConvert.DeserializeObject<ExportFeedFacebookOptions>(commonSettings.AdvancedSettings);

                var products = ExportFeedFacebookService.GetProducts(exportFeedId, commonSettings, advancedSettings);
                var productsCount = ExportFeedFacebookService.GetProductsCount(exportFeedId, commonSettings, advancedSettings);

                return Export(null, products, commonSettings, 0, productsCount);
            }
            catch (Exception ex)
            {
                CsRowError(ex.Message);
                Debug.Log.Error(ex);
            }
            return null;
        }

        public override string Export(IEnumerable<ExportFeedCategories> categories, IEnumerable<ExportFeedProductModel> products, ExportFeedSettings options, int categoriesCount,
            int productsCount)
        {
            try
            {
                var advancedSettings = JsonConvert.DeserializeObject<ExportFeedFacebookOptions>(options.AdvancedSettings);
                
                var currency = CurrencyService.GetCurrencyByIso3(advancedSettings.Currency);

                var exportFile = new FileInfo(options.FileFullPath);
                if (!string.IsNullOrEmpty(exportFile.Directory.FullName))
                {
                    FileHelpers.CreateDirectory(exportFile.Directory.FullName);
                }
                //FileHelpers.DeleteFile(exportFile.FullName);

                CsSetFileName("../" + options.FileFullName);

                FileHelpers.DeleteFile(exportFile.FullName + tempPrefix);

                using (var stream = new FileStream(exportFile.FullName + tempPrefix, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    var settings = new XmlWriterSettings {Encoding = Encoding.UTF8, Indent = true};
                    using (var writer = XmlWriter.Create(stream, settings))
                    {
                        // source https://developers.facebook.com/docs/marketing-api/catalog/reference#supported-fields
                        writer.WriteStartDocument();

                        writer.WriteStartElement("rss");
                        writer.WriteAttributeString("version", "2.0");
                        writer.WriteAttributeString("xmlns", "g", null, GoogleBaseNamespace);
                        writer.WriteStartElement("channel");
                        writer.WriteElementString("title", advancedSettings.DatafeedTitle.Replace("#STORE_NAME#", SettingsMain.ShopName));
                        writer.WriteElementString("link", SettingsMain.SiteUrl);
                        writer.WriteElementString("description", advancedSettings.DatafeedDescription.Replace("#STORE_NAME#", SettingsMain.ShopName));

                        CsSetTotalRow(productsCount);
                        foreach (ExportFeedFacebookProduct productRow in products)
                        {
                            ProcessProductRow(productRow, writer, options, currency);
                            CsNextRow();
                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                }

                FileHelpers.ReplaceFile(exportFile.FullName + tempPrefix, exportFile.FullName);
            }
            catch (Exception ex)
            {
                CsRowError(ex.Message);
                Debug.Log.Error(ex);
            }

            return options.FileFullName;
        }

        private void ProcessProductRow(ExportFeedFacebookProduct row, XmlWriter writer, ExportFeedSettings commonOptions, Currency currency)
        {
            var advancedSettings = JsonConvert.DeserializeObject<ExportFeedGoogleMerchantCenterOptions>(commonOptions.AdvancedSettings);

            writer.WriteStartElement("item");

            #region Основные сведения о товарах

            switch (advancedSettings.OfferIdType)
            {
                case "id":
                    writer.WriteElementString("g", "id", GoogleBaseNamespace, row.OfferId.ToString(CultureInfo.InvariantCulture));
                    break;

                case "artno":
                    writer.WriteElementString("g", "id", GoogleBaseNamespace, row.OfferArtNo.ToString(CultureInfo.InvariantCulture));
                    break;

                default:
                    writer.WriteElementString("g", "id", GoogleBaseNamespace, row.OfferId.ToString(CultureInfo.InvariantCulture));
                    break;
            }

            //title [title]
            var title = row.Name +
                        (advancedSettings.ColorSizeToName && !row.SizeName.IsNullOrEmpty() ? " " + row.SizeName : string.Empty) +
                        (advancedSettings.ColorSizeToName && !row.ColorName.IsNullOrEmpty() ? " " + row.ColorName : string.Empty);
            // writer.WriteStartElement("title");
            // //title should be not longer than 150 characters
            // writer.WriteCData(title.Reduce(150));
            // writer.WriteEndElement();
            writer.WriteElementString("g", "title", GoogleBaseNamespace, title.Reduce(150));

            //description

            //if (!string.Equals(advancedSettings.ProductDescriptionType, "none"))
            //{
            // Sckeef: Согласно инструкции, описание обязательно для товара 05/12/18
            var desc = advancedSettings.ProductDescriptionType == "full" && advancedSettings.ProductDescriptionType != "none" ? row.Description : row.BriefDescription;
            if (advancedSettings.RemoveHtml)
                desc = StringHelper.RemoveHTML(desc);

            if (desc.IsNotEmpty())
            {
                writer.WriteStartElement("g", "description", GoogleBaseNamespace);
                writer.WriteCData(desc.RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }
            //}
            //else
            //{
            //    writer.WriteStartElement("g", "description", GoogleBaseNamespace);
            //    writer.WriteEndElement();
            //}

            //google_product_category http://www.google.com/support/merchants/bin/answer.py?answer=160081
            var googleProductCategory = row.GoogleProductCategory;
            if (string.IsNullOrEmpty(googleProductCategory))
                googleProductCategory = advancedSettings.GoogleProductCategory;
            if (!googleProductCategory.IsNullOrEmpty())
            {
                writer.WriteStartElement("g", "google_product_category", GoogleBaseNamespace);
                writer.WriteCData(googleProductCategory);
                writer.WriteEndElement();
            }

            //product_type
            var localPath = string.Empty;
            var cats =
                CategoryService.GetParentCategories(row.ParentCategory)
                    .Where(x => x.CategoryId != 0)
                    .Reverse()
                    .Select(cat => new { Name = cat.Name, Url = UrlService.GetLink(ParamType.Category, cat.UrlPath, cat.CategoryId) })
                    .ToList();

            for (var i = 0; i < cats.Count; i++)
            {
                var cat = cats[i];
                localPath = localPath + cat.Name;
                if (i == cats.Count - 1) continue;
                localPath = localPath + " > ";
            }
            writer.WriteStartElement("g", "product_type", GoogleBaseNamespace);
            writer.WriteCData(localPath);
            writer.WriteEndElement();

            if (row.Adult)
                writer.WriteElementString("g", "adult", GoogleBaseNamespace, row.Adult.ToString());

            //link
            writer.WriteElementString("g", "link", GoogleBaseNamespace, CreateLink(row, commonOptions.AdditionalUrlTags));

            //image link
            if (!string.IsNullOrEmpty(row.Photos))
            {
                var temp = row.Photos.Split(',');
                for (var i = 0; i < temp.Length && i < 21; i++)
                    writer.WriteElementString("g", i == 0 ? "image_link" : "additional_image_link", GoogleBaseNamespace, GetImageProductPath(temp[i]));
            }

            //condition
            writer.WriteElementString("g", "condition", GoogleBaseNamespace, "new");

            #endregion Основные сведения о товарах

            #region наличие и цена

            //availability
            string availability = "in stock";
            if (row.Amount == 0)
                if (advancedSettings.AllowPreOrderProducts && row.AllowPreorder) availability = "available for order";
                else availability = "out of stock";

            writer.WriteElementString("g", "availability", GoogleBaseNamespace, availability);

            float discount = 0;
            if (ProductDiscountModels != null)
            {
                var prodDiscount = ProductDiscountModels.Find(d => d.ProductId == row.ProductId);
                if (prodDiscount != null)
                {
                    discount = prodDiscount.Discount;
                }
            }

            var markup = row.Price * commonOptions.PriceMarginInPercents / 100 + commonOptions.PriceMarginInNumbers;

            var price = PriceService.GetFinalPrice(row.Price + markup, new Discount(), row.CurrencyValue, currency);

            writer.WriteElementString("g", "price", GoogleBaseNamespace, $"{price.ToInvariantString()} {advancedSettings.Currency}");

            var discountPrice = discount > 0 && discount > row.Discount ? new Discount(discount, 0) : new Discount(row.Discount, row.DiscountAmount);
            if (discountPrice.HasValue)
            {
                var priceWithDiscount =
                    PriceService.GetFinalPrice(row.Price + markup, discountPrice, row.CurrencyValue, currency);

                writer.WriteElementString("g", "sale_price", GoogleBaseNamespace, $"{priceWithDiscount.ToInvariantString()} {advancedSettings.Currency}");
            }

            #endregion наличие и цена

            #region Уникальные идентификаторы товаров

            //GTIN
            var gtin = row.Gtin;
            if (!string.IsNullOrEmpty(gtin))
            {
                writer.WriteStartElement("g", "gtin", GoogleBaseNamespace);
                writer.WriteCData(gtin);
                writer.WriteFullEndElement(); // g:gtin
            }

            //brand
            if (!string.IsNullOrEmpty(row.BrandName))
            {
                writer.WriteStartElement("g", "brand", GoogleBaseNamespace);
                writer.WriteCData(row.BrandName);
                writer.WriteFullEndElement(); // g:brand
            }

            //mpn [mpn]
            if (!string.IsNullOrEmpty(row.ArtNo))
            {
                writer.WriteStartElement("g", "mpn", GoogleBaseNamespace);
                writer.WriteCData(row.ArtNo);
                writer.WriteFullEndElement(); // g:mpn
            }

            #endregion Уникальные идентификаторы товаров

            #region Варианты товара

            if (row.ColorName.IsNotEmpty() || row.SizeName.IsNotEmpty())
            {
                //item_group_id
                writer.WriteElementString("g", "item_group_id", GoogleBaseNamespace, row.ProductId.ToString());
                //color
                if (row.ColorName.IsNotEmpty())
                {
                    writer.WriteElementString("g", "color", GoogleBaseNamespace, row.ColorName);
                }
                //size
                if (row.SizeName.IsNotEmpty())
                {
                    writer.WriteElementString("g", "size", GoogleBaseNamespace, row.SizeName);
                }
            }

            #endregion Варианты товара

            writer.WriteEndElement();
        }

        private string CreateLink(ExportFeedFacebookProduct row, string additionalUrlTags)
        {
            var sufix = string.Empty;
            if (!row.Main)
            {
                if (row.ColorId != 0)
                {
                    sufix = "color=" + row.ColorId;
                }
                if (row.SizeId != 0)
                {
                    if (string.IsNullOrEmpty(sufix))
                        sufix = "size=" + row.SizeId;
                    else
                        sufix += "&size=" + row.SizeId;
                }
            }

            var urlTags = GetAdditionalUrlTags(row, additionalUrlTags);
            if (!string.IsNullOrEmpty(urlTags))
            {
                sufix += (!string.IsNullOrEmpty(sufix) ? "&" + urlTags : urlTags);
            }
            return SettingsMain.SiteUrl.TrimEnd('/') + "/" + UrlService.GetLink(ParamType.Product, row.UrlPath, row.ProductId, sufix);
        }

        public override void SetDefaultSettings(int exportFeedId)
        {
            ExportFeedSettingsProvider.SetSettings(exportFeedId, new ExportFeedSettings
            {
                FileName = File.Exists(SettingsGeneral.AbsolutePath + "/export/facebook.xml")
                    ? "export/facebook" + exportFeedId
                    : "export/facebook",
                FileExtention = GetAvailableFileExtentions()[0],

                AdditionalUrlTags = string.Empty,

                Interval = 1,
                IntervalType = Core.Scheduler.TimeIntervalType.Days,
                Active = false,

                AdvancedSettings = JsonConvert.SerializeObject(new ExportFeedFacebookOptions
                {
                    ProductDescriptionType = "short",
                    DatafeedTitle = "#STORE_NAME#",
                    DatafeedDescription = "#STORE_NAME#",
                    Currency = CurrencyService.BaseCurrency.Iso3,
                    RemoveHtml = true
                }),
                ExportAdult = true
            });
        }

        public override List<string> GetAvailableVariables()
        {
            return new List<string> { "#STORE_NAME#", "#STORE_URL#", "#PRODUCT_NAME#", "#PRODUCT_ID#", "#PRODUCT_ARTNO#" };
        }

        public override List<string> GetAvailableFileExtentions()
        {
            return new List<string> { "xml" };
        }

        public override List<ExportFeedCategories> GetCategories(int exportFeedId)
        {
            throw new System.NotImplementedException();
        }

        public override List<ExportFeedProductModel> GetProducts(int exportFeedId)
        {
            throw new System.NotImplementedException();
        }

        public override int GetProductsCount(int exportFeedId)
        {
            var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
            var advancedSettings = JsonConvert.DeserializeObject<ExportFeedFacebookOptions>(commonSettings.AdvancedSettings);

            return ExportFeedFacebookService.GetProductsCount(exportFeedId, commonSettings, advancedSettings);
        }

        public override int GetCategoriesCount(int exportFeedId)
        {
            return 0;
        }
    }
}