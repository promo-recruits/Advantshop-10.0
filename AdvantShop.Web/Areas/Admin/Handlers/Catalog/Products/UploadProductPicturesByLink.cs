using System;
using System.Drawing;
using System.IO;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Catalog.Products;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class UploadProductPicturesByLink
    {
        private readonly int _productId;
        private readonly string _fileLink;

        public UploadProductPicturesByLink(int productId, string fileLink)
        {
            _productId = productId;
            _fileLink = fileLink;
        }

        public UploadPhotoResultModel Execute()
        {
            if (SaasDataService.IsSaasEnabled)
            {
                var maxPhotoCount = SaasDataService.CurrentSaasData.PhotosCount;

                if (PhotoService.GetCountPhotos(_productId, PhotoType.Product) >= maxPhotoCount)
                {
                    return new UploadPhotoResultModel()
                    {
                        Error = LocalizationService.GetResource("Admin.UploadPhoto.MaxReached") + maxPhotoCount
                    };
                }
            }

            FileHelpers.UpdateDirectories();

            if (!FileHelpers.CheckFileExtension(_fileLink, EAdvantShopFileTypes.Image))
                return new UploadPhotoResultModel()
                {
                    Result = false,
                    Error = string.Format("Файл {0} имеет неправильное разрешение", _fileLink)
                };

            string error;
            AddPhoto(_fileLink, PhotoType.Product, out error);
            
            ProductService.PreCalcProductParams(_productId);

            return new UploadPhotoResultModel() {Result = error.IsNullOrEmpty(), Error = error};
        }


        private bool AddPhoto(string fileLink, PhotoType type, out string error)
        {
            error = "";
            try
            {
                fileLink = fileLink.Split('?')[0];
                var photo = new Photo(0, _productId, type) { OriginName = fileLink };
                var tempPhotoName = Path.GetFileName(fileLink);
                var tempPhotoFullName = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, tempPhotoName);

                if (tempPhotoName.IsNotEmpty() && FileHelpers.DownloadRemoteImageFile(fileLink, tempPhotoFullName, out error))
                {
                    using (var image = Image.FromFile(tempPhotoFullName))
                    {
                        PhotoService.AddPhoto(photo);
                        if (photo.PhotoName.IsNotEmpty())
                            FileHelpers.SaveProductImageUseCompress(photo.PhotoName, image);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("UploadProductPicturesByLink", ex);
                error = "Ошибка при добавлении файла " + fileLink + " " + ex.Message;
            }

            return false;
        }
    }
}
