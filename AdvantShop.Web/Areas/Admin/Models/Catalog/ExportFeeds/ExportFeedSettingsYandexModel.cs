﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Repository.Currencies;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportFeedSettingsYandexModel : IValidatableObject
    {
        public ExportFeedSettingsYandexModel(ExportFeedYandexOptions exportFeedYandexOptions)
        {
            Currency = exportFeedYandexOptions.Currency;
            RemoveHtml = exportFeedYandexOptions.RemoveHtml;
            Delivery = exportFeedYandexOptions.Delivery;
            Pickup = exportFeedYandexOptions.Pickup;
            DeliveryCost = exportFeedYandexOptions.DeliveryCost;
           //GlobalDeliveryCost = exportFeedYandexOptions.GlobalDeliveryCost;
            //LocalDeliveryOption = exportFeedYandexOptions.LocalDeliveryOption;
            ExportProductProperties = exportFeedYandexOptions.ExportProductProperties;
            JoinPropertyValues = exportFeedYandexOptions.JoinPropertyValues;
            ProductPriceType = exportFeedYandexOptions.ProductPriceType;
            SalesNotes = exportFeedYandexOptions.SalesNotes;
            ShopName = exportFeedYandexOptions.ShopName;
            CompanyName = exportFeedYandexOptions.CompanyName;
            ColorSizeToName = exportFeedYandexOptions.ColorSizeToName;
            ProductDescriptionType = exportFeedYandexOptions.ProductDescriptionType;
            OfferIdType = exportFeedYandexOptions.OfferIdType;
            VendorCodeType = exportFeedYandexOptions.VendorCodeType;
            ExportNotAvailable = exportFeedYandexOptions.ExportNotAvailable;
            AllowPreOrderProducts = exportFeedYandexOptions.AllowPreOrderProducts ?? true;
            //Available = exportFeedYandexOptions.Available;
            ExportPurchasePrice = exportFeedYandexOptions.ExportPurchasePrice;
            ExportCount = exportFeedYandexOptions.ExportCount;
            ExportShopSku = exportFeedYandexOptions.ExportShopSku;
            ExportManufacturer = exportFeedYandexOptions.ExportManufacturer;
            ExportRelatedProducts = exportFeedYandexOptions.ExportRelatedProducts;
            Store = exportFeedYandexOptions.Store;
            ExportBarCode = exportFeedYandexOptions.ExportBarCode;
            ExportAllPhotos = exportFeedYandexOptions.ExportAllPhotos;
            TypeExportYandex = exportFeedYandexOptions.TypeExportYandex;
            NeedZip = exportFeedYandexOptions.NeedZip;
            OnlyMainOfferToExport = exportFeedYandexOptions.OnlyMainOfferToExport;
            ExportDimensions = exportFeedYandexOptions.ExportDimensions;
            Promos = new List<ExportFeedYandexPromo>();
            DontExportCurrency = exportFeedYandexOptions.DontExportCurrency;
            NotExportAmountCount = exportFeedYandexOptions.NotExportAmountCount;

            try
            {
                if (!string.IsNullOrWhiteSpace(exportFeedYandexOptions.Promos))
                {
                    Promos =
                        JsonConvert.DeserializeObject<List<ExportFeedYandexPromo>>(exportFeedYandexOptions.Promos);

                    PromosJson = exportFeedYandexOptions.Promos;


                    PromoCodesJson = JsonConvert.SerializeObject(Promos.Where(x => x.Type == YandexPromoType.PromoCode));
                    FlashDiscountsJson = JsonConvert.SerializeObject(Promos.Where(x => x.Type == YandexPromoType.Flash));
                    PromoGiftsJson = JsonConvert.SerializeObject(Promos.Where(x => x.Type == YandexPromoType.Gift));
                    NPlusMJson = JsonConvert.SerializeObject(Promos.Where(x => x.Type == YandexPromoType.NPlusM));
                }
            }
            finally
            {
                if (Promos == null)
                    Promos = new List<ExportFeedYandexPromo>();
            }

            LocalDeliveryOption = new ExportFeedYandexDeliveryCostOption();
            try
            {
                LocalDeliveryOption =
                    JsonConvert.DeserializeObject<ExportFeedYandexDeliveryCostOption>(exportFeedYandexOptions.LocalDeliveryOption);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (LocalDeliveryOption == null)
                    LocalDeliveryOption = new ExportFeedYandexDeliveryCostOption();
            }

            GlobalDeliveryCost = new List<ExportFeedYandexDeliveryCostOption>();
            try
            {
                if (!string.IsNullOrWhiteSpace(exportFeedYandexOptions.GlobalDeliveryCost))
                {
                    GlobalDeliveryCost =
                        JsonConvert.DeserializeObject<List<ExportFeedYandexDeliveryCostOption>>(exportFeedYandexOptions.GlobalDeliveryCost);

                    GlobalDeliveryCostJson = exportFeedYandexOptions.GlobalDeliveryCost;
                }
            }
            finally
            {
                if (GlobalDeliveryCost == null)
                    GlobalDeliveryCost = new List<ExportFeedYandexDeliveryCostOption>();
            }
        }

        
        public int ExportFeedId { get; set; }

        public string Currency { get; set; }
        public bool RemoveHtml { get; set; }
        public bool Delivery { get; set; }
        public bool Pickup { get; set; }
        public ExportFeedYandexDeliveryCost DeliveryCost { get; set; }
        public bool ExportProductProperties { get; set; }
        public bool JoinPropertyValues { get; set; }
        [Obsolete]
        public bool ExportProductDiscount { get; set; }
        public EExportFeedYandexPriceType ProductPriceType { get; set; }
        public string SalesNotes { get; set; }
        public string ShopName { get; set; }
        public string CompanyName { get; set; }
        public bool ColorSizeToName { get; set; }
        public string ProductDescriptionType { get; set; }
        public string OfferIdType { get; set; }
        public string VendorCodeType { get; set; }
        public bool ExportNotAvailable { get; set; }
        public bool AllowPreOrderProducts { get; set; }        
        public bool Available { get; set; }
        public bool ExportPurchasePrice { get; set; }
        public bool ExportCount { get; set; }
        public bool ExportShopSku { get; set; }
        public bool ExportManufacturer { get; set; }

        public bool ExportRelatedProducts { get; set; }
        public bool ExportAllPhotos { get; set; }
        public bool Store { get; set; }
        public bool ExportBarCode { get; set; }

        public bool TypeExportYandex { get; set; }
        public bool NeedZip { get;  set; }
        public bool OnlyMainOfferToExport { get; set; }        

        public bool ExportDimensions { get; set; }

        public int? NotExportAmountCount { get; set; }

        public List<ExportFeedYandexPromo> Promos { get; set; }

        public string PromosJson { get; set; }

        public string PromoCodesJson { get; set; }
        public string FlashDiscountsJson { get; set; }
        public string PromoGiftsJson { get; set; }
        public string NPlusMJson { get; set; }

        public bool DontExportCurrency { get; set; }

        public ExportFeedYandexDeliveryCostOption LocalDeliveryOption { get; set; }

        public List<ExportFeedYandexDeliveryCostOption> GlobalDeliveryCost { get; set; }

        public string GlobalDeliveryCostJson { get; set; }

        public Dictionary<ExportFeedYandexDeliveryCost, string> DeliveryCostList
        {
            get
            {
                var deliveryCostList = new Dictionary<ExportFeedYandexDeliveryCost, string>();
                foreach (ExportFeedYandexDeliveryCost deliveryCost in Enum.GetValues(typeof(ExportFeedYandexDeliveryCost)))
                {
                    deliveryCostList.Add(deliveryCost, deliveryCost.Localize());
                }
                return deliveryCostList;
            }
        }

        public Dictionary<string, string> Currencies
        {
            get
            {
                var currencyList = new Dictionary<string, string>();
                foreach (var item in CurrencyService.GetAllCurrencies().Where(item => ExportFeedYandex.AvailableCurrencies.Contains(item.Iso3)).ToList())
                {
                    currencyList.Add(item.Iso3, item.Name);
                }
                return currencyList;
            }
        }

        public Dictionary<string, string> ProductDescriptionTypeList
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"short", LocalizationService.GetResource("Admin.ExportFeed.Settings.BriefDescription")},
                    {"full", LocalizationService.GetResource("Admin.ExportFeed.Settings.FullDescription")},
                    {"none", LocalizationService.GetResource("Admin.ExportFeed.Settings.DontUseDescription")}
                };
            }
        }

        public Dictionary<string, string> OfferTypes
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"id", LocalizationService.GetResource("Admin.ExportFeed.Settings.OfferId")},
                    {"artno", LocalizationService.GetResource("Admin.ExportFeed.Settings.OfferSku")}
                };
            }
        }

        public Dictionary<string, string> VendorCodeTypes
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"productArtno", LocalizationService.GetResource("Admin.ExportFeed.Settings.ProductArtNo")},
                    {"offerArtno", LocalizationService.GetResource("Admin.ExportFeed.Settings.OfferArtNo")},
                };
            }
        }

        public Dictionary<string, string> DeliveryCostTypes
        {
            get
            {
                return new Dictionary<string, string> {
                    {ExportFeedYandexDeliveryCost.None.ToString(),ExportFeedYandexDeliveryCost.None.Localize()},
                    {ExportFeedYandexDeliveryCost.GlobalDeliveryCost.ToString(),ExportFeedYandexDeliveryCost.GlobalDeliveryCost.Localize() },
                    {ExportFeedYandexDeliveryCost.LocalDeliveryCost.ToString(),ExportFeedYandexDeliveryCost.LocalDeliveryCost.Localize() },
                };
            }
        }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ShopName))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "ShopName" });
            }
            if (string.IsNullOrEmpty(CompanyName))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "CompanyName" });
            }
        }
        
        public Dictionary<EExportFeedYandexPriceType, string> ProductPriceTypeList =>
            Enum.GetValues(typeof(EExportFeedYandexPriceType)).Cast<EExportFeedYandexPriceType>().ToDictionary(priceType => priceType, priceType => priceType.Localize());
    }
    
    public class ExportFeedSettingsYandexPromoModel : IValidatableObject
    {
        public Guid? PromoID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PromoUrl { get; set; }
        public string Type { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public List<int> ProductIDs { get; set; }
        public int RequiredQuantity { get; set; }
        public int FreeQuantity { get; set; }
        public int GiftID { get; set; }
        public List<int> CategoryIDs { get; set; }

        #region PromoCode

        public int CouponId { get; set; }
        private Coupon _coupon { get; set; }
        public Coupon Coupon
        {
            get
            {
                if (_coupon != null)
                {
                    return _coupon;
                }
                else
                {
                    _coupon = CouponService.GetCoupon(CouponId);
                    return _coupon;
                }
            }
        }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var type = Type.TryParseEnum<YandexPromoType>();
            if (StartDate != null && ExpirationDate != null)
            {
                var delta = ExpirationDate - StartDate;
                if (type == YandexPromoType.Flash && delta.Value.Days > 7)
                {
                    yield return new ValidationResult("Указанная длительность акции больше 7 дней.");
                }
                if (delta.Value < new TimeSpan(0,1,0))
                {
                    yield return new ValidationResult("Указанная длительность акции недопустима.");
                }
            }
            else if ((StartDate != null && ExpirationDate == null) || (StartDate == null && ExpirationDate != null))
            {
                yield return new ValidationResult("Необходимо указать обе даты.");
            }

            if (type == YandexPromoType.PromoCode)
            {
                if (CouponId <= 0)
                    yield return new ValidationResult("Неверный код купона.");
            }
            else
            {
                if(ProductIDs == null || ProductIDs.Count == 0)
                {
                    if(type == YandexPromoType.NPlusM )
                    {
                        if (CategoryIDs == null || CategoryIDs.Count == 0)
                        {
                            yield return new ValidationResult("Выберите по крайней мере один продукт или категорию.");
                        }
                    }
                    else
                    {
                        yield return new ValidationResult("Выберите по крайней мере один продукт.");
                    }

                }
                if (type == YandexPromoType.Gift)
                {
                    if (RequiredQuantity <= 0 || RequiredQuantity > 24)
                    {
                        yield return new ValidationResult("Количество товаров, которое нужно приобрести должно быть от 1 до 24.");
                    }
                    if (GiftID == 0)
                    {
                        yield return new ValidationResult("Выберите продукт в качестве подарка.");
                    }
                }
                if (type == YandexPromoType.NPlusM)
                {
                    if(RequiredQuantity <= 0 || RequiredQuantity > 24 || FreeQuantity <= 0 || FreeQuantity > 24)
                    {
                        yield return new ValidationResult("Можно добавить от 1 до 24 товаров за полную цену/бонусных товаров.");
                    }
                }
            }
        }
    }
}
