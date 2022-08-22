//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Repository.Currencies;
using Newtonsoft.Json;

namespace AdvantShop.Orders
{
    public class ShoppingCartItem : ICloneable
    {
        [JsonIgnore]
        public int ShoppingCartItemId { get; set; }

        [JsonIgnore]
        public ShoppingCartType ShoppingCartType { get; set; }

        [JsonIgnore]
        public Guid CustomerId { get; set; }

        [JsonIgnore]
        public string AttributesXml { get; set; }

        private bool _loadAmount;
        private float _amount;
        public float Amount
        {
            get
            {
                if (!_loadAmount)
                {
                    var multiplicity = (Offer.Product.Multiplicity > 0 ? Offer.Product.Multiplicity : 1f);
                    var minAmount = Offer.Product.MinAmount.HasValue ? Offer.Product.MinAmount.Value : multiplicity;

                    _amount = (float)(Math.Ceiling((decimal)_amount / (decimal)multiplicity) * (decimal)multiplicity);
                    if (_amount < minAmount)
                        _amount = minAmount;

                    _loadAmount = true;
                }
                return _amount;
            }
            set
            {
                _amount = value;
                _loadAmount = false;
            }
        }

        [JsonIgnore]
        public DateTime CreatedOn { get; set; }

        [JsonIgnore]
        public DateTime UpdatedOn { get; set; }

        [JsonIgnore]
        public bool AddedByRequest { get; set; }

        public int OfferId { get; set; }

        public string ArtNo
        {
            get
            {
                return this.Offer.ArtNo;
            }
        }

        public bool IsGift { get; set; }

        public string ModuleKey { get; set; }

        public bool FrozenAmount
        {
            get { return AddedByRequest || IsGift; }
        }

        private Coupon _coupon;
        private bool? _isCouponApplied;

        [JsonIgnore]
        public bool IsCouponApplied
        {
            get
            {
                if (_isCouponApplied.HasValue)
                    return _isCouponApplied.Value;

                if (_coupon == null)
                    _coupon = CouponService.GetCustomerCoupon(CustomerId);

                //_isCouponApplied = _coupon != null && CouponService.IsCouponAppliedToProduct(_coupon.CouponID, Offer.ProductId)
                //    && ((_coupon.Type == CouponType.Percent && _coupon.Value > ProductDiscount.Percent) || _coupon.Type == CouponType.Fixed);

                if (_coupon != null && CouponService.IsCouponAppliedToProduct(_coupon.CouponID, Offer.ProductId))
                {
                    if (_coupon.Type == CouponType.Percent)
                    {
                        if ((ProductsDiscount.Type == DiscountType.Percent && _coupon.Value > ProductsDiscount.Percent) ||
                            (ProductsDiscount.Type == DiscountType.Amount && Price * _coupon.Value / 100 > ProductsDiscount.Amount))
                        {
                            return (_isCouponApplied = true).Value;
                        }
                    }
                    else if (_coupon.Type == CouponType.Fixed)
                    {
                        if ((ProductsDiscount.Type == DiscountType.Percent && _coupon.Value > Price * ProductsDiscount.Percent / 100) ||
                            (ProductsDiscount.Type == DiscountType.Amount && _coupon.Value > ProductsDiscount.Amount))
                        {
                            return (_isCouponApplied = true).Value;
                        }
                    }
                }

                return (_isCouponApplied = false).Value;
            }
        }

        private Offer _offer;
        [JsonIgnore]
        public Offer Offer
        {
            get
            {
                return _offer ?? (_offer = OfferService.GetOffer(OfferId));
            }
        }

        public override int GetHashCode()
        {
            return OfferId ^ Amount.GetHashCode() ^ IsGift.GetHashCode() ^ (AttributesXml ?? "").GetHashCode() ^ (CustomPrice ?? 0).GetHashCode();
        }

        private CustomerGroup _customerGroup;
        private CustomerGroup CustomerGroup
        {
            get
            {
                if (_customerGroup != null)
                    return _customerGroup;

                if (CustomerContext.CurrentCustomer != null)
                    return _customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

                var customer = CustomerService.GetCustomer(CustomerId);

                _customerGroup = customer != null
                    ? customer.CustomerGroup
                    : CustomerGroupService.GetCustomerGroup(CustomerGroupService.DefaultCustomerGroup);
                
                return _customerGroup;
            }
        }

        private float? _customOptionsPrice;

        private float CustomOptionPrice
        {
            get
            {
                return _customOptionsPrice ??
                       (_customOptionsPrice =
                           CustomOptionsService.GetCustomOptionPrice(Offer.RoundedPrice, AttributesXml, Offer.Product.Currency.Rate)).Value;
            }
        }

        private float? _price;
        public float Price
        {
            get
            {
                if (_price != null)
                    return _price.Value;

                var price = CustomPrice ?? Offer.RoundedPrice + CustomOptionPrice;

                return (_price = PriceService.RoundPrice(price, null, CurrencyService.CurrentCurrency.Rate)).Value;
            }
        }

        public float? CustomPrice { get; set; }


        private float? _priceWithDiscount;
        [JsonIgnore]
        public float PriceWithDiscount
        {
            get
            {
                if (IsGift) return 0;

                if (_priceWithDiscount.HasValue)
                    return _priceWithDiscount.Value;

                if (IsCouponApplied)
                    return Price;

                _priceWithDiscount = CustomPrice ?? PriceService.GetFinalPrice(Price, ProductsDiscount);

                return _priceWithDiscount.Value;
            }
        }

        private float? _priceWithDiscountWithoutCoupon;
        [JsonIgnore]
        public float PriceWithDiscountWithoutCoupon
        {
            get
            {
                if (IsGift) return 0;

                if (_priceWithDiscountWithoutCoupon.HasValue)
                    return _priceWithDiscountWithoutCoupon.Value;

                //if (IsCouponApplied)
                //    return Price;

                _priceWithDiscountWithoutCoupon = PriceService.GetFinalPrice(Price, ProductsDiscount);

                return _priceWithDiscountWithoutCoupon.Value;
            }
        }

        private Discount _productsDiscount;
        private Discount ProductsDiscount
        {
            get
            {
                if (_productsDiscount != null)
                    return _productsDiscount;

                var discountCartItem = ShoppingCartService.GetShoppingCartItemDiscount(ShoppingCartItemId);
                var productDiscount = Offer.Product.Discount;

                if (discountCartItem != null)
                {
                    Discount discount = null;
                    if (discountCartItem.Type == productDiscount.Type)
                        discount = discountCartItem.Value > productDiscount.Value ? discountCartItem : productDiscount;
                    else
                    {
                        var discountPriceCartItem = PriceService.GetFinalPrice(Price, discountCartItem, Offer.Product.Currency.Rate);
                        var discountPriceProduct = PriceService.GetFinalPrice(Price, productDiscount, Offer.Product.Currency.Rate);
                        discount = discountPriceCartItem > discountPriceProduct ? productDiscount : discountCartItem;
                    }

                    _productsDiscount = PriceService.GetFinalDiscount(Price, discount, Offer.Product.Currency.Rate,
                            CustomerGroup, Offer.ProductId);
                }
                else
                {
                    _productsDiscount = PriceService.GetFinalDiscount(Price, Offer.Product.Discount,
                        Offer.Product.Currency.Rate, CustomerGroup, Offer.ProductId);
                }

                return _productsDiscount;
            }
        }

        private Discount _discount;
        [JsonIgnore]
        public Discount Discount
        {
            get
            {
                if (_discount != null)
                    return _discount;

                if (IsCouponApplied)
                    return _discount = new Discount();
                
                return _discount = new Discount(ProductsDiscount.Percent, ProductsDiscount.Amount, ProductsDiscount.Type);
            }
        }

        private Discount _discountWithouCpupon;
        [JsonIgnore]
        public Discount DiscountWithouCpupon
        {
            get
            {
                if (_discountWithouCpupon != null)
                    return _discountWithouCpupon;

                //if (IsCouponApplied)
                //    return _discountWithouCpupon = new Discount();

                return _discountWithouCpupon = new Discount(ProductsDiscount.Percent, ProductsDiscount.Amount, ProductsDiscount.Type);
            }
        }

        private Customer _customer;
        public Customer Customer
        {
            get { return _customer ?? (_customer = CustomerService.GetCustomer(CustomerId) ?? new Customer(CustomerGroupService.DefaultCustomerGroup) { Id = CustomerId, CustomerRole = Role.Guest }); }
        }

        public ShoppingCartItem()
        {
            ShoppingCartType = ShoppingCartType.ShoppingCart;
            AttributesXml = "";
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}