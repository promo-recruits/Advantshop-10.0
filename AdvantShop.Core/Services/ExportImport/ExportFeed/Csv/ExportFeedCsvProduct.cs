namespace AdvantShop.ExportImport
{
    public class ExportFeedCsvProduct : ExportFeedProductModel
    {
        public string Category { get; set; }
        public string Sorting { get; set; }

        public string Enabled { get; set; }
        public string Currency { get; set; }
        public string Price { get; set; }
        public string PurchasePrice { get; set; }
        public string Amount { get; set; }
        public string MultiOffer { get; set; }
        public string Unit { get; set; }
        public string ShippingPrice { get; set; }
        public string YandexDeliveryDays { get; set; }
        public string Discount { get; set; }
        public string DiscountAmount { get; set; }
        public string Weight { get; set; }
        public string Size { get; set; }

        public string Markers { get; set; }
        public string Photos { get; set; }
        public string Videos { get; set; }

        public string Properties { get; set; }
        public string OrderByRequest { get; set; }
        public string Producer { get; set; }

        public string Related { get; set; }
        public string Alternative { get; set; }
        public string CustomOption { get; set; }
        public string Gifts { get; set; }
        public string Tags { get; set; }

        public string SalesNote { get; set; }
        public string Gtin { get; set; }
        public string GoogleProductCategory { get; set; }
        public string YandexTypePrefix { get; set; }
        public string YandexModel { get; set; }
        public string YandexSizeUnit { get; set; }
        public string Adult { get; set; }
        public string ManufacturerWarranty { get; set; }
        public string YandexName { get; set; }
        public string YandexDiscounted { get; set; }
        public string YandexDiscountCondition { get; set; }
        public string YandexDiscountReason { get; set; }

        public string MinAmount { get; set; }
        public string MaxAmount { get; set; }
        public string Multiplicity { get; set; }

        public string Bid { get; set; }
        public string BarCode { get; set; }

        public string Tax { get; set; }
        public string PaymentSubjectType { get; set; }
        public string PaymentMethodType { get; set; }

        public string Store { get; set; }
        public string Funnel { get; set; }
        public string Vk { get; set; }
        public string Instagram { get; set; }
        public string Yandex { get; set; }
        public string Avito { get; set; }
        public string Google { get; set; }
        public string Facebook { get; set; }
        public string Bonus { get; set; }
        public string Referal { get; set; }

        public string ManualRatio { get; set; }
    }
}