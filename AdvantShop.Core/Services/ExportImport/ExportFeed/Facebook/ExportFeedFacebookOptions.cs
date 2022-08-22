namespace AdvantShop.ExportImport
{
    public class ExportFeedFacebookOptions : ExportFeedSettings
    {
        public string Currency { get; set; }

        public bool RemoveHtml { get; set; }

        public string DatafeedTitle { get; set; }

        public string DatafeedDescription { get; set; }

        public string GoogleProductCategory { get; set; }

        public string ProductDescriptionType { get; set; }

        public string OfferIdType { get; set; }

        public bool AllowPreOrderProducts { get; set; }

        public bool ExportNotAvailable { get; set; }

        public bool ColorSizeToName { get; set; }

        public bool OnlyMainOfferToExport { get; set; }
    }
}