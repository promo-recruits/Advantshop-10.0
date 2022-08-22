//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;


namespace AdvantShop.Shipping
{
    //*********************************************
    [Serializable]
    public class ShippingMethod
    {
        public int ShippingMethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }
        public bool DisplayCustomFields { get; set; }
        public bool DisplayIndex { get; set; }
        public bool ShowInDetails { get; set; }
        public bool MoveToEnd { get; set; }
        public bool ShowIfNoOtherShippings { get; set; }

        public string ZeroPriceMessage { get; set; }
        
        public float ExtrachargeInNumbers { get; set; }
        public float ExtrachargeInPercents { get; set; }

        public bool ExtrachargeFromOrder { get; set; }
        public bool UseExtracharge => !ShippingMethodService.ShippingMethodsNoUseExtracharge.Contains(ShippingType);


        public int ExtraDeliveryTime { get; set; }
        public bool UseExtraDeliveryTime => !ShippingMethodService.ShippingMethodsNoUseExtraDeliveryTime.Contains(ShippingType);

        public int? TaxId { get; set; }
        private TaxType _taxType;
        public TaxType TaxType
        {
            get
            {
                var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
                _taxType = tax?.TaxType ?? TaxType.None;
                return _taxType;
            }
        }

        public ePaymentMethodType PaymentMethodType { get; set; } = ePaymentMethodType.full_prepayment;
        public ePaymentSubjectType PaymentSubjectType { get; set; } = ePaymentSubjectType.payment;

        private Photo _picture;
        public Photo IconFileName
        {
            get => _picture ?? (_picture = PhotoService.GetPhotoByObjId(ShippingMethodId, PhotoType.Shipping));
            set => _picture = value;
        }

        public string ShippingType { get; set; }

        private Dictionary<string, string> _params = new Dictionary<string, string>();
        public Dictionary<string, string> Params
        {
            get => _params;
            set => _params = value;
        }

        public int? CurrencyId { get; set; }

        private Currency _shippingCurrency;
        public Currency ShippingCurrency
        {
            get
            {
                if (_shippingCurrency != null)
                    return _shippingCurrency;

                return
                    (_shippingCurrency = CurrencyId.HasValue && UseCurrency
                        ? CurrencyService.GetAllCurrencies(true).FirstOrDefault(x => x.CurrencyId == CurrencyId.Value)
                        : null);
            }
        }

        public bool UseCurrency => !ShippingMethodService.ShippingMethodsNoUseCurrency.Contains(ShippingType);

        public string ModuleStringId { get; set; }
    }
}