//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Xml.Serialization;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;

namespace AdvantShop.Catalog
{
    [Serializable]
    public class Offer
    {
        public int OfferId { get; set; }

        public int ProductId { get; set; }

        [Compare("Core.Catalog.Offer.Amount")]
        public float Amount { get; set; }

        /// <summary>
        /// Price from db
        /// </summary>
        [Compare("Core.Catalog.Offer.BasePrice")]
        public float BasePrice { get; set; }

        private float? _roundedPrice;

        public float RoundedPrice
        {
            get
            {
                return _roundedPrice ??
                       (float) (_roundedPrice = PriceService.RoundPrice(BasePrice, null, Product.Currency.Rate));
            }
        }

        [Compare("Core.Catalog.Offer.SupplyPrice")]
        public float SupplyPrice { get; set; }

        [Compare("Core.Catalog.Offer.Color", ChangeHistoryParameterType.Color)]
        public int? ColorID { get; set; }

        [Compare("Core.Catalog.Offer.Size", ChangeHistoryParameterType.Size)]
        public int? SizeID { get; set; }

        [Compare("Core.Catalog.Offer.Main")]
        public bool Main { get; set; }

        [Compare("Core.Catalog.Offer.ArtNo")]
        public string ArtNo { get; set; }

        [NonSerialized] private Color _color;

        [XmlIgnore]
        public Color Color
        {
            get { return _color ?? (_color = ColorService.GetColor(ColorID)); }
        }

        [NonSerialized] 
        private Size _size;

        [XmlIgnore]
        public Size Size
        {
            get { return _size ?? (_size = SizeService.GetSize(SizeID)); }
        }

        [NonSerialized] private Product _product;

        [XmlIgnore]
        public Product Product
        {
            get { return _product ?? (_product = ProductService.GetProduct(ProductId)); }
        }

        [NonSerialized] 
        private ProductPhoto _photo;

        [XmlIgnore]
        public ProductPhoto Photo
        {
            get { return _photo ?? (_photo = PhotoService.GetMainProductPhoto(ProductId, ColorID)); }
        }

        [XmlIgnore]
        public bool CanOrderByRequest
        {
            get { return Product != null && Product.AllowPreOrder && (Amount <= 0 || RoundedPrice == 0); }
        }

        [Compare("Core.Catalog.Offer.Weight")]
        public float? Weight { get; set; }

        [Compare("Core.Catalog.Offer.Length")]
        public float? Length { get; set; }

        [Compare("Core.Catalog.Offer.Width")]
        public float? Width { get; set; }

        [Compare("Core.Catalog.Offer.Height")]
        public float? Height { get; set; }

        [Compare("Core.Catalog.Offer.BarCode")]
        public string BarCode { get; set; }
    }
}
