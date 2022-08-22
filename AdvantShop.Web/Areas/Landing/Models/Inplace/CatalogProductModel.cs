﻿using AdvantShop.Core.Services.Catalog;
using AdvantShop.FilePath;

namespace AdvantShop.App.Landing.Models.Inplace
{
    public class CatalogProductModel
    {
        public int ProductId { get; set; }

        public string ArtNo { get; set; }

        public string Name { get; set; }

        public string ProductArtNo { get; set; }

        public string PhotoName { get; set; }

        public string PhotoSrc
        {
            get { return FoldersHelper.GetImageProductPath(ProductImageType.Small, PhotoName, true); }
        }

        public float Price { get; set; }

        public string PriceString
        {
            get
            {
                return PriceFormatService.FormatPrice(Price, CurrencyValue, CurrencyCode, CurrencyIso3, CurrencyIsCodeBefore);
            }
        }

        public float Amount { get; set; }

        public int OffersCount { get; set; }

        public bool Enabled { get; set; }

        public string CurrencyCode { get; set; }
        public string CurrencyIso3 { get; set; }
        public float CurrencyValue { get; set; }
        public bool CurrencyIsCodeBefore { get; set; }

        public int SortOrder { get; set; }

        public int ColorId { get; set; }
        public int SizeId { get; set; }
    }
}
