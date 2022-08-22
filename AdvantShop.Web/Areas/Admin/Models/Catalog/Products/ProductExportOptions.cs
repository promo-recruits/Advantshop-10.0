using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Web.Admin.Models.Catalog.Products
{
    public class ProductExportOptions
    {
        public ProductExportOptions()
        {
            YandexDiscountConditions = Enum.GetValues(typeof(EYandexDiscountCondition)).Cast<EYandexDiscountCondition>()
                .Select(x => new SelectItemModel(x.Localize(), (int)x)).ToList();
        }

        public int ProductId { get; set; }
        public string SalesNote { get; set; }
        public string Gtin { get; set; }
        public string GoogleProductCategory { get; set; }
        public string YandexMarketCategory { get; set; }
        public string YandexTypePrefix { get; set; }
        public string YandexModel { get; set; }
        public string YandexSizeUnit { get; set; }
        public string YandexName { get; set; }
        public string YandexDeliveryDays { get; set; }
        public bool YandexProductDiscounted { get; set; }
        public EYandexDiscountCondition YandexProductDiscountCondition { get; set; }
        public string YandexProductDiscountReason { get; set; }

        public bool Adult { get; set; }
        public bool ManufacturerWarranty { get; set; }
        
        public float Bid { get; set; }

        public List<SelectItemModel> YandexDiscountConditions { get; private set; }
    }
}
