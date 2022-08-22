//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Catalog
{
    public enum CouponType
    {
        [Localize("Core.Coupon.CouponType.Fixed")]
        Fixed = 1,

        [Localize("Core.Coupon.CouponType.Percent")]
        Percent = 2
    }

    public enum CouponMode
    {
        None = 0,
        Template = 1,
        Generated = 2,
        TriggerTemplate = 3,
        PartnersTemplate = 4,
        Partner = 5
    }

    [Serializable]
    public class Coupon
    {
        public int CouponID { get; set; }
        public string Code { get; set; }
        public CouponType Type { get; set; }
        public float Value { get; set; }
        public string CurrencyIso3 { get; set; }
        public DateTime AddingDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int PossibleUses { get; set; }
        public int ActualUses { get; set; }
        public bool Enabled { get; set; }
        public float MinimalOrderPrice { get; set; }
        public bool IsMinimalOrderPriceFromAllCart { get; set; }
        public bool ForFirstOrder { get; set; }

        public int? EntityId { get; set; }

        private List<int> _categoryIds;
        public List<int> CategoryIds
        {
            get { return _categoryIds ?? (_categoryIds = CouponService.GetCategoriesIDsByCoupon(CouponID)); }
        }

        private List<int> _productsIds;
        public List<int> ProductsIds
        {
            get { return _productsIds ?? (_productsIds = CouponService.GetProductsIDsByCoupon(CouponID)); }
        }

        private Currency _currency;
        public Currency Currency
        {
            get { return _currency ?? (_currency = CurrencyService.Currency(CurrencyIso3)); }
        }

        public float GetRate()
        {
            return Type == CouponType.Percent
                    ? Value
                    : PriceService.SimpleRoundPrice(Value * Currency.Rate / CurrencyService.CurrentCurrency.Rate);
        }

        public override int GetHashCode()
        {
            return CouponID.GetHashCode() ^
                   Code.GetHashCode() ^
                   PossibleUses.GetHashCode() ^
                   ActualUses.GetHashCode() ^
                   MinimalOrderPrice.GetHashCode() ^
                   IsMinimalOrderPriceFromAllCart.GetHashCode() ^
                   StartDate.GetHashCode() ^
                   ExpirationDate.GetHashCode() ^
                   Type.GetHashCode() ^
                   Value.GetHashCode() ^
                   Mode.GetHashCode() ^
                   (TriggerId ?? 0).GetHashCode() ^
                   (TriggerActionId ?? 0).GetHashCode() ^
                   (Days ?? 0).GetHashCode() ^
                   (CustomerId ?? Guid.Empty).GetHashCode() ^
                   ForFirstOrder.GetHashCode();
        }

        public int? TriggerActionId { get; set; }

        public int? TriggerId { get; set; }

        public CouponMode Mode { get; set; }

        public int? Days { get; set; }

        public Guid? CustomerId { get; set; }
    }
}