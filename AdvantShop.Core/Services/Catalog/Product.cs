//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.SEO;
using AdvantShop.Repository.Currencies;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.ChangeHistories;

namespace AdvantShop.Catalog
{
    public enum RelatedType
    {
        Related = 0,
        Alternative = 1
    }

    public enum EYandexDiscountCondition
    {
        [Localize("Core.Catalog.EYandexDiscountCondition.None")]
        None = 0,
        [Localize("Core.Catalog.EYandexDiscountCondition.LikeNew")]
        [StringName("likenew")]
        LikeNew,
        [Localize("Core.Catalog.EYandexDiscountCondition.Used")]
        [StringName("used")]
        Used
    }

    public class Product
    {
        public Product()
        {
            Discount = new Discount();
            AccrueBonuses = true;
        }

        public int ProductId { get; set; }

        [Compare("Core.Catalog.Product.ArtNo")]
        public string ArtNo { get; set; }

        [Compare("Core.Catalog.Product.Name")]
        public string Name { get; set; }


        public string Photo { get; set; }
        public string PhotoDesc { get; set; } // убрать или сделать lazy

        [Compare("Core.Catalog.Product.Ratio")]
        public double Ratio { get; set; } // убрать или сделать lazy

        [Compare("Core.Catalog.Product.Ratio")]
        public double? ManualRatio { get; set; }

        [Compare("Core.Catalog.Product.Discount")]
        public Discount Discount { get; set; }

        [Compare("Core.Catalog.Product.BriefDescription", true)]
        public string BriefDescription { get; set; }

        [Compare("Core.Catalog.Product.Description", true)]
        public string Description { get; set; }

        [Compare("Core.Catalog.Product.Enabled")]
        public bool Enabled { get; set; }

        public bool Hidden { get; set; }

        [Compare("Core.Catalog.Product.Recomended")]
        public bool Recomended { get; set; }

        [Compare("Core.Catalog.Product.New")]
        public bool New { get; set; }

        [Compare("Core.Catalog.Product.BestSeller")]
        public bool BestSeller { get; set; }

        [Compare("Core.Catalog.Product.OnSale")]
        public bool OnSale { get; set; }

        [Compare("Core.Catalog.Product.AllowPreOrder")]
        public bool AllowPreOrder { get; set; }

        public bool CategoryEnabled { get; set; }

        [Compare("Core.Catalog.Product.Unit")]
        public string Unit { get; set; }

        [Compare("Core.Catalog.Product.ShippingPrice")]
        public float? ShippingPrice { get; set; }

        [Compare("Core.Catalog.Product.MinAmount")]
        public float? MinAmount { get; set; }

        [Compare("Core.Catalog.Product.MaxAmount")]
        public float? MaxAmount { get; set; }

        [Compare("Core.Catalog.Product.Multiplicity")]
        public float Multiplicity { get; set; }

        [Compare("Core.Catalog.Product.SalesNote")]
        public string SalesNote { get; set; }

        [Compare("Core.Catalog.Product.GoogleProductCategory")]
        public string GoogleProductCategory { get; set; }

        [Compare("Core.Catalog.Product.YandexMarketCategory")]
        public string YandexMarketCategory { get; set; }

        [Compare("Core.Catalog.Product.YandexTypePrefix")]
        public string YandexTypePrefix { get; set; }

        [Compare("Core.Catalog.Product.YandexModel")]
        public string YandexModel { get; set; }

        [Compare("Core.Catalog.Product.Gtin")]
        public string Gtin { get; set; }

        [Compare("Core.Catalog.Product.Adult")]
        public bool Adult { get; set; }

        [Compare("Core.Catalog.Product.ManufacturerWarranty")]
        public bool ManufacturerWarranty { get; set; }

        [Compare("Core.Catalog.Product.Bid")]
        public float Bid { get; set; }

        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }        

        public bool ActiveView360 { get; set; }

        private string _yandexSizeUnit { get; set; }

        [Compare("Core.Catalog.Product.YandexSizeUnit")]
        public string YandexSizeUnit
        {
            get { return _yandexSizeUnit; }
            set { _yandexSizeUnit = value != null && ProductService.YandexSizeUnitValidate.Contains(value) ? value : null; }
        }

        [Compare("Core.Catalog.Product.YandexName")]
        public string YandexName { get; set; }

        [Compare("Core.Catalog.Product.YandexDeliveryDays")]
        public string YandexDeliveryDays { get; set; }

        [Compare("Core.Catalog.Product.YandexProductDiscounted")]
        public bool YandexProductDiscounted { get; set; }

        [Compare("Core.Catalog.Product.YandexProductDiscountCondition")]
        public EYandexDiscountCondition YandexProductDiscountCondition { get; set; }

        [Compare("Core.Catalog.Product.YandexProductDiscountReason")]
        public string YandexProductDiscountReason { get; set; }

        /// <summary>
        /// Начислять бонусы за покупку этого товара
        /// </summary>
        [Compare("Core.Catalog.Product.AccrueBonuses")]
        public bool AccrueBonuses { get; set; }
        
        [Compare("Core.Catalog.Product.Tax", ChangeHistoryParameterType.Tax)]
        public int? TaxId { get; set; }

        private ePaymentSubjectType _paymentSubjectType;

        [Compare("Core.Catalog.Product.PaymentSubjectType")]
        public ePaymentSubjectType PaymentSubjectType
        {
            get { return (int) _paymentSubjectType != 0 ? _paymentSubjectType : ePaymentSubjectType.commodity; }
            set { _paymentSubjectType = value; }
        }

        private ePaymentMethodType _paymentMethodType;

        [Compare("Core.Catalog.Product.PaymentMethodType")]
        public ePaymentMethodType PaymentMethodType
        {
            get { return _paymentMethodType != 0 ? _paymentMethodType : ePaymentMethodType.full_prepayment; }
            set { _paymentMethodType = value; }
        }

        [Compare("Core.Catalog.Product.Brand", ChangeHistoryParameterType.Brand)]
        public int BrandId { get; set; }

        private Brand _brand;
        public Brand Brand
        {
            get { return _brand ?? (_brand = BrandService.GetBrandById(BrandId)); }
        }

        private string _urlPath;

        [Compare("Core.Catalog.Product.UrlPath")]
        public string UrlPath
        {
            get { return _urlPath; }
            set { _urlPath = value.ToLower(); }
        }

        public bool HasMultiOffer { get; set; }

        private bool? _gifts;
        public bool HasGifts()
        {
            if (_gifts.HasValue)
                return _gifts.Value;
            _gifts = ProductService.HasGifts(ProductId);
            return _gifts.Value;
        }

        [Compare("Core.Catalog.Product.Currency", ChangeHistoryParameterType.Currency)]
        public int CurrencyID { get; set; }

        private Currency _currency;
        public Currency Currency
        {
            get { return _currency ?? (_currency = CurrencyService.GetCurrency(CurrencyID, true)); }
        }

        /// <summary>
        /// Get from DB collection of Offer and set collection
        /// </summary>
        private List<Offer> _offers;

        public List<Offer> Offers
        {
            get { return _offers ?? (_offers = OfferService.GetProductOffers(ProductId)); }
            set { _offers = value; }
        }

        public Discount CalculableDiscount()
        {
            foreach (var discountModule in AttachedModules.GetModules<IDiscount>().Where(x => x != null))
            {
                var discount = ((IDiscount)Activator.CreateInstance(discountModule)).GetDiscount(ProductId);
                if (discount != 0)
                    return new Discount(discount, 0);
            }

            if (Discount.HasValue)
                return Discount;

            return new Discount(DiscountByTimeService.GetDiscountByTime(), 0);
        }

        public MetaType MetaType
        {
            get { return MetaType.Product; }
        }

        private bool _isMetaLoaded;
        private MetaInfo _meta;

        public MetaInfo Meta
        {
            get
            {
                if (_isMetaLoaded)
                    return _meta;

                _isMetaLoaded = true;

                return _meta ??
                       (_meta =
                           MetaInfoService.GetMetaInfo(ProductId, MetaType) ??
                           MetaInfoService.GetDefaultMetaInfo(MetaType, string.Empty));
            }
            set
            {
                _meta = value;
                _isMetaLoaded = true;
            }
        }

        /// <summary>
        /// return collection of ProductPhoto
        /// </summary>
        private List<ProductPhoto> _productphotos;
        public List<ProductPhoto> ProductPhotos
        {
            get { return _productphotos ?? (_productphotos = PhotoService.GetPhotos<ProductPhoto>(ProductId, PhotoType.Product)); }
        }

        /// <summary>
        /// return collection of ProductPhoto, 360 view
        /// </summary>
        private List<ProductPhoto> _productphotos360;
        public List<ProductPhoto> ProductPhotos360
        {
            get { return _productphotos360 ?? (_productphotos360 = PhotoService.GetPhotos<ProductPhoto>(ProductId, PhotoType.Product360)); }
        }


        private List<ProductVideo> _productVideos;
        public List<ProductVideo> ProductVideos
        {
            get { return _productVideos ?? (_productVideos = ProductVideoService.GetProductVideos(ProductId)); }
        }

        private List<PropertyValue> _productPropertyValues;
        public List<PropertyValue> ProductPropertyValues
        {
            get
            {
                return _productPropertyValues ??
                       (_productPropertyValues = PropertyService.GetPropertyValuesByProductId(ProductId));
            }
        }

        private int _categoryId;
        public int CategoryId
        {
            get { return _categoryId == 0 || _categoryId == CategoryService.DefaultNonCategoryId ? _categoryId = ProductService.GetFirstCategoryIdByProductId(ProductId) : _categoryId; }
        }

        private Category _mainCategory;
        public Category MainCategory
        {
            get { return _mainCategory ?? (_mainCategory = CategoryService.GetCategory(CategoryId)); }
        }

        private List<Category> _productCategories;
        [SoapIgnore]
        [XmlIgnore]
        public List<Category> ProductCategories
        {
            get { return _productCategories ?? (_productCategories = ProductService.GetCategoriesByProductId(ProductId)); }
        }


        private bool _tagsLoaded;
        private IList<Tag> _tags;
        public IList<Tag> Tags
        {
            get
            {
                if (_tagsLoaded)
                    return _tags;
                _tagsLoaded = true;
                return _tags ?? (_tags = TagService.Gets(ProductId, ETagType.Product, true));
            }
            set
            {
                _tags = value;
                _tagsLoaded = true;
            }
        }

        private int? _ratioCount;
        public int? RatioCount
        {
            get { return _ratioCount ?? (_ratioCount = RatingService.GetProductRatioCount(ProductId)); }
        }
    }
}