using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using System.Web;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Catalog
{
    public partial class ProductModel
    {
        private string PhotoName { get; set; }
        private string PhotoNameSize1 { get; set; }
        private string PhotoNameSize2 { get; set; }
        private string PhotoDescription { get; set; }
        private string AdditionalPhoto { get; set; }

        public int ProductId { get; set; }

        public int OfferId { get; set; }

        public string OfferArtNo { get; set; }

        public float AmountOffer { get; set; }

        public string UrlPath { get; set; }

        public string Name { get; set; }

        public string BriefDescription { get; set; }

        public string ArtNo { get; set; }

        public float Multiplicity { get; set; }

        public float Amount { get; set; }

        public float MinAmount { get; set; }

        public float MaxAmount { get; set; }

        public bool AllowPreorder { get; set; }

        public bool Recomend { get; set; }

        public bool Sales { get; set; }

        public bool Bestseller { get; set; }

        public bool New { get; set; }

        public bool Gifts { get; set; }

        public bool Enabled { get; set; }

        public string Colors { get; set; }

        public List<ProductColorModel> ColorsList
        {
            get
            {
                return !string.IsNullOrEmpty(Colors)
                    ? JsonConvert.DeserializeObject<List<ProductColorModel>>(Colors)
                    : null;
            }
        }

        public int ColorId { get; set; }

        public int? SelectedColorId { get; set; }

        public int SizeId { get; set; }

        public double Ratio { get; set; }

        public double? ManualRatio { get; set; }

        public int? RatioId { get; set; }

        public float BasePrice { get; set; }

        private float? _roundedPrice;
        public float RoundedPrice
        {
            get { return _roundedPrice ?? (_roundedPrice = PriceService.RoundPrice(BasePrice, null, CurrencyValue)).Value; }
        }

        public float Discount { get; set; }
        public float DiscountAmount { get; set; }


        public int Comments { get; set; }


        public string CategoryName { get; set; }
        public string CategoryUrl { get; set; }

        public float CurrencyValue { get; set; }


        public List<ProductDiscount> ProductDiscounts { private get; set; }

        public float DiscountByDatetime { private get; set; }

        public CustomerGroup CustomerGroup { private get; set; }


        private Discount _totalDiscount;

        public Discount TotalDiscount
        {
            get
            {
                if (_totalDiscount != null)
                    return _totalDiscount;

                return
                    _totalDiscount =
                        PriceService.GetFinalDiscount(RoundedPrice, Discount, DiscountAmount, CurrencyValue, CustomerGroup, ProductId,
                                                      DiscountByDatetime, ProductDiscounts);
            }
        }

        private float _finalPrice = -1;

        public float PriceWithDiscount
        {
            get
            {
                if (_finalPrice == -1)
                    _finalPrice = PriceService.GetFinalPrice(RoundedPrice, TotalDiscount);

                return _finalPrice;
            }
        }

        private string _preparedPrice;

        public string PreparedPrice => _preparedPrice ??
                                       (_preparedPrice = PriceFormatService.FormatPrice(RoundedPrice, PriceWithDiscount, TotalDiscount, true, true, MultiPrices));

        private ProductPhoto _photo;

        public ProductPhoto Photo
        {
            get
            {
                if (_photo != null)
                    return _photo;

                var title = !string.IsNullOrEmpty(PhotoDescription)
                            ? HttpUtility.HtmlAttributeEncode(PhotoDescription)
                            : HttpUtility.HtmlAttributeEncode(Name);

                _photo = new ProductPhoto()
                {
                    PhotoName = AdditionalPhoto ?? PhotoName,
                    PhotoNameSize1 = PhotoNameSize1,
                    PhotoNameSize2 = PhotoNameSize2,
                    Title = title,
                    Alt = title,
                    PhotoId = PhotoId
                };

                return _photo;
            }
            set => _photo = value;
        }

        public string StartPhotoJson => System.String.Format("[{{PathSmall:'{0}',PathMiddle:'{1}',PathBig:'{2}'}}]", Photo.ImageSrcSmall(), Photo.ImageSrcMiddle(), Photo.ImageSrcBig());

        public string PhotoSmall => Photo.ImageSrcSmall();

        public string PhotoMiddle => Photo.ImageSrcMiddle();

        public string PhotoBig => Photo.ImageSrcBig();

        public int PhotoId { get; set; }

        public int CountPhoto { get; set; }

        public bool MultiPrices { get; set; }

        public string BarCode { get; set; }
    }

    public class ProductColorModel
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public string PhotoName { get; set; }
        public bool Main { get; set; }
    }
}
