using System;
using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;

namespace AdvantShop.ExportImport
{
    [Serializable()]
    public class ExportFeedYandexOptions : ExportFeedSettings
    {
        [JsonProperty(PropertyName = "Currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "RemoveHtml")]
        public bool RemoveHtml { get; set; }

        [JsonProperty(PropertyName = "Delivery")]
        public bool Delivery { get; set; }

        [JsonProperty(PropertyName = "Pickup")]
        public bool Pickup { get; set; }


        //[JsonProperty(PropertyName = "LocalDeliveryCost")]
        //public bool LocalDeliveryCost { get; set; }

        [JsonProperty(PropertyName = "DeliveryCost")]
        public ExportFeedYandexDeliveryCost DeliveryCost { get; set; }

        [JsonProperty(PropertyName = "GlobalDeliveryCost")]
        public string GlobalDeliveryCost { get; set; }

        private string _localDeliveryOption;
        [JsonProperty(PropertyName = "LocalDeliveryOption")]
        public string LocalDeliveryOption
        {
            get { return _localDeliveryOption; }
            set
            {
                _localDeliveryOption = value;
                _localDeliveryOptionObject = null;
            }
        }

        private ExportFeedYandexDeliveryCostOption _localDeliveryOptionObject;
        [JsonIgnore]
        public ExportFeedYandexDeliveryCostOption LocalDeliveryOptionObject
        {
            get { return _localDeliveryOptionObject ?? (_localDeliveryOptionObject = GetLocalDeliveryOption(LocalDeliveryOption)); }
        }

        [JsonProperty(PropertyName = "ExportProductProperties")]
        public bool ExportProductProperties { get; set; }

        [JsonProperty(PropertyName = "JoinPropertyValues")]
        public bool JoinPropertyValues { get; set; }

        [Obsolete("use ProductPriceType")]
        [JsonProperty(PropertyName = "ExportProductDiscount")]
        public bool ExportProductDiscount
        {
            set => ProductPriceType = value
                    ? EExportFeedYandexPriceType.Both
                    : EExportFeedYandexPriceType.WithDiscount;
        }
        
        [JsonProperty(PropertyName = "ProductPriceType")]
        public EExportFeedYandexPriceType ProductPriceType { get; set; }
        
        [JsonProperty(PropertyName = "SalesNotes")]
        public string SalesNotes { get; set; }

        [JsonProperty(PropertyName = "ShopName")]
        public string ShopName { get; set; }

        [JsonProperty(PropertyName = "CompanyName")]
        public string CompanyName { get; set; }

        [JsonProperty(PropertyName = "ColorSizeToName")]
        public bool ColorSizeToName { get; set; }

        [JsonProperty(PropertyName = "ProductDescriptionType")]
        public string ProductDescriptionType { get; set; }

        [JsonProperty(PropertyName = "OfferIdType")]
        public string OfferIdType { get; set; }

        [JsonProperty(PropertyName = "VendorCodeType")]
        public string VendorCodeType { get; set; }

        [JsonProperty(PropertyName = "ExportPurchasePrice")]
        public bool ExportPurchasePrice { get; set; }

        [JsonProperty(PropertyName = "ExportCount")]
        public bool ExportCount { get; set; }

        [JsonProperty(PropertyName = "ExportShopSku")]
        public bool ExportShopSku { get; set; }

        [JsonProperty(PropertyName = "ExportManufacturer")]
        public bool ExportManufacturer { get; set; }


        [JsonProperty(PropertyName = "ExportBarCode")]
        public bool ExportBarCode { get; set; }

        [JsonProperty(PropertyName = "Store")]
        public bool Store { get; set; }

        [JsonProperty(PropertyName = "ExportRelatedProducts")]
        public bool ExportRelatedProducts { get; set; }

        [JsonProperty(PropertyName = "AllowPreOrderProducts")]
        public bool? AllowPreOrderProducts { get; set; }

        [JsonProperty(PropertyName = "ExportNotAvailable")]
        public bool ExportNotAvailable { get; set; }

        [JsonProperty(PropertyName = "ExportAllPhotos")]
        public bool ExportAllPhotos { get; set; }

        [JsonProperty(PropertyName = "TypeExportYandex")]
        public bool TypeExportYandex { get; set; }

        [JsonProperty(PropertyName = "NeedZip")]
        public bool NeedZip { get;  set; }

        [JsonProperty(PropertyName = "OnlyMainOfferToExport")]
        public bool OnlyMainOfferToExport { get; set; }

        [JsonProperty(PropertyName = "ExportDimensions")]
        public bool ExportDimensions { get; set; }
        //public ExportFeedYandexOptions() {
        //    //Available = true;
        //    Store = true;
        //}

        [JsonProperty(PropertyName = "DontExportCurrency")]
        public bool DontExportCurrency { get; set; }

        [JsonProperty(PropertyName = "Promos")]
        public string Promos { get; set; }
        private static ExportFeedYandexDeliveryCostOption GetLocalDeliveryOption(string localDeliveryOptionString)
        {
            var localDeliveryOption = new ExportFeedYandexDeliveryCostOption();

            try
            {
                localDeliveryOption =
                    JsonConvert.DeserializeObject<ExportFeedYandexDeliveryCostOption>(localDeliveryOptionString);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (localDeliveryOption == null)
                    localDeliveryOption = new ExportFeedYandexDeliveryCostOption();
            }
            return localDeliveryOption;
        }

        [JsonProperty(PropertyName = "NotExportAmountCount")]
        public int? NotExportAmountCount { get; set; }
    }

    public enum ExportFeedYandexDeliveryCost
    {
        [Localize("Core.ExportImport.ExportFeedYandexDeliveryCost.None")]
        None = 0,

        [Localize("Core.ExportImport.ExportFeedYandexDeliveryCost.LocalDeliveryCost")]
        LocalDeliveryCost = 1,

        [Localize("Core.ExportImport.ExportFeedYandexDeliveryCost.GlobalDeliveryCost")]
        GlobalDeliveryCost = 2
    }

    public class ExportFeedYandexDeliveryCostOption
    {
        public string Cost { get; set; }

        public string Days { get; set; }

        public string OrderBefore { get; set; }
    }

    public enum EExportFeedYandexPriceType
    {
        [Localize("Core.ExportImport.ExportFeedYandexPriceType.WithDiscount")]
        WithDiscount,
        [Localize("Core.ExportImport.ExportFeedYandexPriceType.WithoutDiscount")]
        WithoutDiscount,
        [Localize("Core.ExportImport.ExportFeedYandexPriceType.Both")]
        Both
    }
}
