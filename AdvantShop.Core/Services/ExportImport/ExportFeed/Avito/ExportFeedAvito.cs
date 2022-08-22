//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace AdvantShop.ExportImport
{
    [ExportFeedKey("Avito")]
    public class ExportFeedAvito : BaseExportFeed
    {
        public ExportFeedAvito() : base()
        {
        }

        public ExportFeedAvito(bool useCommonStatistic) : base(useCommonStatistic)
        {
        }

        public static List<string> AvailableCurrencies
        {
            get { return new List<string> { "RUB", "RUR" }; }
        }

        public static List<string> AvailableFileExtentions
        {
            get { return new List<string> { "xml"/*, "yml"*/ }; }
        }

        private void ProcessProductRow(ExportFeedAvitoProduct row, XmlWriter writer, ExportFeedSettings commonSettings, ExportFeedAvitoOptions advancedSettings, Currency currency)
        {
            var avitoProductProperties = ExportFeedAvitoService.GetProductProperties(row.ProductId) ?? new List<ExportFeedAvitoProductProperty>();

            var avitoCommonTags = Enum.GetValues(typeof(EAvitoCommonTegs)).Cast<EAvitoCommonTegs>().ToList();

            writer.WriteStartElement("Ad");

            // common tags
            if (!avitoProductProperties.Any(item => item.PropertyName == "Id"))
            {
                writer.WriteStartElement("Id");
                writer.WriteRaw(row.ArtNo);
                writer.WriteEndElement();
            }

            var date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id, "Russian Standard Time");

            var productDateBegin = avitoProductProperties.FirstOrDefault(item => item.PropertyName == EAvitoCommonTegs.DateBegin.StrName());
            writer.WriteStartElement("DateBegin");
            writer.WriteRaw(productDateBegin != null ? productDateBegin.PropertyValue : date.AddDays(advancedSettings.PublicationDateOffset).ToString("yyyy-MM-dd"));
            writer.WriteEndElement();

            var productDateEnd = avitoProductProperties.FirstOrDefault(item => item.PropertyName == EAvitoCommonTegs.DateEnd.StrName());
            writer.WriteStartElement("DateEnd");
            writer.WriteRaw(productDateEnd != null ? productDateEnd.PropertyValue : date.AddDays(advancedSettings.PublicationDateOffset + advancedSettings.DurationOfPublicationInDays).ToString("yyyy-MM-dd"));
            writer.WriteEndElement();

            var productListingFee = avitoProductProperties.FirstOrDefault(item => item.PropertyName == EAvitoCommonTegs.ListingFee.StrName());
            writer.WriteStartElement("ListingFee");
            writer.WriteRaw(productListingFee != null ? productListingFee.PropertyValue : advancedSettings.PaidPublicationOption.StrName());
            writer.WriteEndElement();

            var productAdStatus = avitoProductProperties.FirstOrDefault(item => item.PropertyName == EAvitoCommonTegs.AdStatus.StrName());
            writer.WriteStartElement("AdStatus");
            writer.WriteRaw(productAdStatus != null ? productAdStatus.PropertyValue : advancedSettings.PaidServices.StrName());
            writer.WriteEndElement();

            var avitoId = avitoProductProperties.FirstOrDefault(item => item.PropertyName == EAvitoCommonTegs.AvitoId.StrName());
            if (avitoId != null && !string.IsNullOrEmpty(avitoId.PropertyValue))
            {
                writer.WriteStartElement("AvitoId");
                writer.WriteRaw(avitoId.PropertyValue);
                writer.WriteEndElement();
            }

            // contact information
            var productAllowEmail = avitoProductProperties.FirstOrDefault(item => item.PropertyName == EAvitoCommonTegs.AllowEmail.StrName());
            writer.WriteStartElement("AllowEmail");
            writer.WriteRaw(productAllowEmail != null
                ? productAllowEmail.PropertyValue
                : (advancedSettings.EmailMessages ? "Да" : "Нет"));
            writer.WriteEndElement();

            var productManagerName = avitoProductProperties.FirstOrDefault(item => item.PropertyName == EAvitoCommonTegs.ManagerName.StrName());
            writer.WriteStartElement("ManagerName");
            writer.WriteRaw(productManagerName != null ? productManagerName.PropertyValue : advancedSettings.ManagerName);
            writer.WriteEndElement();

            var productContactPhone = avitoProductProperties.FirstOrDefault(item => item.PropertyName == EAvitoCommonTegs.ContactPhone.StrName());
            writer.WriteStartElement("ContactPhone");
            writer.WriteRaw(productContactPhone != null ? productContactPhone.PropertyValue : advancedSettings.Phone);
            writer.WriteEndElement();

            //location
            var productAddress = avitoProductProperties.FirstOrDefault(item => item.PropertyName == EAvitoCommonTegs.Address.StrName());
            writer.WriteStartElement("Address");
            writer.WriteRaw(((productAddress != null ? productAddress.PropertyValue : advancedSettings.Address) ?? "").Reduce(256));
            writer.WriteEndElement();

            //Product descripton Будет отличаться в зависимости от выбранной категории Авито
            ////////////////////////////////////////////////////////////////////////////////////

            foreach (var productProperty in avitoProductProperties.Where(item => !avitoCommonTags.Any(tag => tag.StrName() == item.PropertyName)))
            {
                try
                {
                    writer.WriteStartElement(XmlConvert.EncodeName(productProperty.PropertyName.XmlEncode().RemoveInvalidXmlChars()));
                    writer.WriteRaw(productProperty.PropertyValue.XmlEncode().RemoveInvalidXmlChars());
                    writer.WriteEndElement();
                }
                catch (Exception ex)
                {
                    CsRowError(ex.Message);
                    Debug.Log.Error(ex);
                }
            }

            if (!avitoProductProperties.Any(item => item.PropertyName == "Category")
                    && !string.IsNullOrEmpty(advancedSettings.DefaultAvitoCategory))
            {
                writer.WriteStartElement("Category");
                writer.WriteRaw(advancedSettings.DefaultAvitoCategory);
                writer.WriteEndElement();
            }

            ////////////////////////////////////////////////////////////////////////////////////

            if (!avitoProductProperties.Any(item => item.PropertyName == "Title"))
            {
                writer.WriteStartElement("Title");
                writer.WriteRaw(row.Name.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (!avitoProductProperties.Any(item => item.PropertyName == "Description"))
            {
                writer.WriteStartElement("Description");
                var description = string.Empty;
                if (!string.IsNullOrEmpty(row.Colors))
                {
                    description += "Доступные " + SettingsCatalog.ColorsHeader + " : " + row.Colors + "<br/>";
                }
                if (!string.IsNullOrEmpty(row.Sizes))
                {
                    description += "Доступные " + SettingsCatalog.SizesHeader + " : " + row.Sizes + "<br/>";
                }
                description += advancedSettings.ProductDescriptionType == "full" ? row.Description : row.BriefDescription;

                if (advancedSettings.UnloadProperties && !string.IsNullOrEmpty(row.Properties))
                {
                    foreach (var item in row.Properties.Split(","))
                        description += "\n\t\t" + item;
                }

                writer.WriteCData(description.RemoveInvalidXmlChars());
                //writer.WriteRaw(row.Description.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            //price
            if (!avitoProductProperties.Any(item => item.PropertyName == "Price"))
            {
                float discount = 0;
                if (ProductDiscountModels != null)
                {
                    var prodDiscount = ProductDiscountModels.Find(d => d.ProductId == row.ProductId);
                    if (prodDiscount != null)
                    {
                        discount = prodDiscount.Discount;
                    }
                }

                var productCurrency = CurrencyService.Currency(row.Currency);

                var priceDiscount = discount > 0 && discount > row.Discount
                    ? new Discount(discount, 0)
                    : new Discount(row.Discount, row.DiscountAmount);

                var markup = row.Price * commonSettings.PriceMarginInPercents / 100 +
                             commonSettings.PriceMarginInNumbers;

                var newPrice = PriceService.GetFinalPrice(row.Price + markup, priceDiscount, productCurrency.Rate, currency);

                //выгрузка поля с ценой не обязательна - не выводим цену для товаров, чья итоговая цена 0
                if (newPrice > 0)
                {
                    writer.WriteStartElement("Price");
                    writer.WriteRaw(Convert.ToInt32(newPrice).ToString());
                    writer.WriteEndElement();
                }
            }
            //end price

            if (!string.IsNullOrEmpty(row.BarCode))
            {
                writer.WriteStartElement("Barcode");
                writer.WriteRaw(row.BarCode);
                writer.WriteEndElement();
            }

            //if (!string.IsNullOrEmpty(row.Photos))
            //{
            //    writer.WriteStartElement("Images");
            //    var temp = row.Photos.Split(',').Take(10);
            //    foreach (var item in temp.Where(item => !string.IsNullOrWhiteSpace(item)))
            //    {
            //        writer.WriteStartElement("Image");
            //        writer.WriteAttributeString("url", GetImageProductPath(item));
            //        writer.WriteEndElement();
            //    }
            //    writer.WriteEndElement();
            //}

            if (!string.IsNullOrEmpty(row.PhotosIds))
            {
                writer.WriteStartElement("Images");
                var temp = row.PhotosIds.Split(',').Take(10);
                foreach (var item in temp.Where(item => !string.IsNullOrWhiteSpace(item)))
                {
                    writer.WriteStartElement("Image");
                    writer.WriteAttributeString("url", SettingsMain.SiteUrl.TrimEnd('/') + "/avito/avitophoto?photoid=" + item);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            //VideoURL

            writer.WriteEndElement();
        }

        public override string Export(int exportFeedId)
        {
            try
            {
                var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
                var advancedSettings = JsonConvert.DeserializeObject<ExportFeedAvitoOptions>(commonSettings.AdvancedSettings);

                var categories = ExportFeedAvitoService.GetCategories(exportFeedId, advancedSettings.ExportNotAvailable);
                var products = ExportFeedAvitoService.GetProducts(exportFeedId, commonSettings, advancedSettings);
                var categoriesCount = ExportFeedAvitoService.GetCategoriesCount(exportFeedId, advancedSettings.ExportNotAvailable);
                var productsCount = ExportFeedAvitoService.GetProductsCount(exportFeedId, commonSettings, advancedSettings);

                return Export(categories, products, commonSettings, categoriesCount, productsCount);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return null;
        }

        public override string Export(IEnumerable<ExportFeedCategories> categories, IEnumerable<ExportFeedProductModel> products, ExportFeedSettings options, int categoriesCount, int productsCount)
        {
            try
            {
                var advancedSettings = ExportFeedSettingsProvider.ConvertAdvancedSettings<ExportFeedAvitoOptions>(options.AdvancedSettings);

                var exportFile = new FileInfo(options.FileFullPath);
                if (!string.IsNullOrEmpty(exportFile.Directory.FullName))
                {
                    FileHelpers.CreateDirectory(exportFile.Directory.FullName);
                }
                //FileHelpers.DeleteFile(exportFile.FullName);

                CsSetFileName("../" + options.FileFullName);

                FileHelpers.DeleteFile(exportFile.FullName + tempPrefix);

                using (var outputFile = new FileStream(exportFile.FullName + tempPrefix, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
                    using (var writer = XmlWriter.Create(outputFile, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Ads");
                        writer.WriteAttributeString("formatVersion", "3");
                        writer.WriteAttributeString("target", "Avito.ru");

                        CsSetTotalRow(productsCount);

                        var currency = CurrencyService.GetCurrencyByIso3(advancedSettings.Currency);

                        foreach (ExportFeedAvitoProduct offerRow in products)
                        {
                            ProcessProductRow(offerRow, writer, options, advancedSettings, currency);
                            CsNextRow();
                        }

                        writer.WriteEndElement();
                        writer.WriteEndDocument();

                        writer.Flush();
                        writer.Close();
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

        public override void SetDefaultSettings(int exportFeedId)
        {
            ExportFeedSettingsProvider.SetSettings(exportFeedId, new ExportFeedSettings
            {
                FileName = File.Exists(SettingsGeneral.AbsolutePath + "/export/avito.xml") ? "export/avito" + exportFeedId : "export/avito",
                FileExtention = AvailableFileExtentions[0],

                AdditionalUrlTags = string.Empty,

                Interval = 1,
                IntervalType = Core.Scheduler.TimeIntervalType.Days,
                JobStartTime = new DateTime(2017, 1, 1, 1, 0, 0),
                Active = false,

                AdvancedSettings = JsonConvert.SerializeObject(new ExportFeedAvitoOptions
                {
                    Currency = ExportFeedAvito.AvailableCurrencies[0],
                    PublicationDateOffset = 0,
                    DurationOfPublicationInDays = 0,
                    PaidPublicationOption = EPaidPublicationOption.Package,
                    PaidServices = EPaidServices.Free,

                    EmailMessages = false,
                    ManagerName = string.Empty,
                    Address = string.Empty,
                    Phone = HttpUtility.HtmlEncode(SettingsMain.Phone)
                }),
                ExportAdult = true
            });
        }

        public override List<string> GetAvailableVariables()
        {
            return new List<string> { "#STORE_NAME#", "#STORE_URL#", "#PRODUCT_NAME#", "#PRODUCT_ID#", "#PRODUCT_ARTNO#" };
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
            var advancedSettings = JsonConvert.DeserializeObject<ExportFeedAvitoOptions>(commonSettings.AdvancedSettings);

            return ExportFeedAvitoService.GetProductsCount(exportFeedId, commonSettings, advancedSettings);
        }

        public override int GetCategoriesCount(int exportFeedId)
        {
            var commonSettings = ExportFeedSettingsProvider.GetSettings(exportFeedId);
            var advancedSettings = JsonConvert.DeserializeObject<ExportFeedAvitoOptions>(commonSettings.AdvancedSettings);

            return ExportFeedAvitoService.GetCategoriesCount(exportFeedId, advancedSettings.ExportNotAvailable);
        }

        public override List<string> GetAvailableFileExtentions()
        {
            return AvailableFileExtentions;
        }
    }
}