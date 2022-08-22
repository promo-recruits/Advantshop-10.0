﻿using AdvantShop.Catalog;
using AdvantShop.Web.Admin.Models.Catalog.Brands;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Brands
{
    public class DeleteBrandPicture
    {
        private readonly int _brandId;

        public DeleteBrandPicture(int? brandId)
        {
            _brandId = brandId != null ? brandId.Value : -1;
        }

        public UploadBrandPictureResult Execute()
        {
            BrandService.DeleteBrandLogo(_brandId);
            return new UploadBrandPictureResult() { Result = true, Picture = "../images/nophoto_small.jpg" };
        }
    }
}
