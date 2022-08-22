using System.Collections.Generic;

namespace AdvantShop.ExportImport
{
    public class ExportFeedCsvV2Offer
    {
        public int OfferId { get; set; }
        public string ArtNo { get; set; }
        public string Price { get; set; }
        public string PurchasePrice { get; set; }
        public string Amount { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string OfferPhotos { get; set; }
        public string Weight { get; set; }
        public string Dimensions { get; set; }
        public string BarCode { get; set; }
    }

    public class ExportFeedCsvV2Category
    {
        public string Path { get; set; }
        public string Sort { get; set; }
    }

    public class ExportFeedCsvV2Product : ExportFeedProductModel
    {
        //public string ArtNo { get; set; }
        public List<ExportFeedCsvV2Offer> Offers { get; set; }
        //public string UrlPath { get; set; }
        public List<ExportFeedCsvV2Category> Categories { get; set; }
        public string Sorting { get; set; }
        public string Enabled { get; set; }
        public string Currency { get; set; }
        public string Photos { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public string Unit { get; set; }
        public string Discount { get; set; }
        public string DiscountAmount { get; set; }
        public string ShippingPrice { get; set; }
        //public string BriefDescription { get; set; }
        //public string Description { get; set; }
        //public string Title { get; set; }
        //public string MetaKeywords { get; set; }
        //public string MetaDescription { get; set; }
        //public string H1 { get; set; }
        public string Related { get; set; }
        public string Alternative { get; set; }
        public string Videos { get; set; }
        public string MarkerNew { get; set; }
        public string MarkerBestseller { get; set; }
        public string MarkerRecomended { get; set; }
        public string MarkerOnSale { get; set; }
        public string ManualRatio { get; set; }
        public string Producer { get; set; }
        public string OrderByRequest { get; set; }
        public string CustomOptions { get; set; }
        public string YandexSalesNotes { get; set; }
        public string YandexDeliveryDays { get; set; }
        public string YandexTypePrefix { get; set; }
        public string YandexName { get; set; }
        public string YandexModel { get; set; }
        public string YandexSizeUnit { get; set; }
        public string YandexDiscounted { get; set; }
        public string YandexDiscountCondition { get; set; }
        public string YandexDiscountReason { get; set; }
        public string YandexBid { get; set; }
        public string GoogleGtin { get; set; }
        public string GoogleProductCategory { get; set; }
        public string Adult { get; set; }
        public string ManufacturerWarranty { get; set; }
        public string AvitoProductProperties { get; set; }
        public string Tags { get; set; }
        public string Gifts { get; set; }
        public string MinAmount { get; set; }
        public string MaxAmount { get; set; }
        public string Multiplicity { get; set; }
        //public string BarCode { get; set; }
        public string Tax { get; set; }
        public string PaymentSubjectType { get; set; }
        public string PaymentMethodType { get; set; }
        public string ModifiedDate { get; set; }
    }
}