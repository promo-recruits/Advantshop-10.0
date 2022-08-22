using System.Drawing;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Catalog.Categories;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Categories
{
    public class UploadCategoryPictures
    {
        private readonly int _categoryId;
        private readonly bool _isEditMode;
        private readonly PhotoType _type;
        private readonly HttpPostedFileBase _file;

        public UploadCategoryPictures(HttpPostedFileBase file, PhotoType type, int? categoryId)
        {
            _categoryId = categoryId != null ? categoryId.Value : -1;
            _isEditMode = _categoryId != -1;
            _type = type;
            _file = file;
        }

        public UploadPictureResult Execute()
        {
            UploadPictureResult result;

            if (_file == null || string.IsNullOrEmpty(_file.FileName))
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            if (!FileHelpers.CheckFileExtension(_file.FileName, EAdvantShopFileTypes.Image))
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Core.Errors.UnsupportedImageFormat") };

            FileHelpers.UpdateDirectories();

            if (_isEditMode)
            {
                var cacheKey = CacheNames.GetCategoryCacheObjectName(_categoryId);
                CacheManager.Remove(cacheKey);
            }

            switch (_type)
            {
                case PhotoType.CategoryBig:
                    result = AddPhoto(_file, PhotoType.CategoryBig, CategoryImageType.Big,
                                    SettingsPictureSize.BigCategoryImageWidth,
                                    SettingsPictureSize.BigCategoryImageHeight);
                    break;
                case PhotoType.CategorySmall:
                    result = AddPhoto(_file, PhotoType.CategorySmall, CategoryImageType.Small,
                                    SettingsPictureSize.SmallCategoryImageWidth,
                                    SettingsPictureSize.SmallCategoryImageHeight);
                    break;
                case PhotoType.CategoryIcon:
                    result = AddPhoto(_file, PhotoType.CategoryIcon, CategoryImageType.Icon,
                                    SettingsPictureSize.IconCategoryImageWidth,
                                    SettingsPictureSize.IconCategoryImageHeight);
                    break;
                default:
                    result = new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
                    break;
            }

            CategoryService.ClearCategoryCache(_categoryId);

            return result;
        }

        private UploadPictureResult AddPhoto(HttpPostedFileBase file, PhotoType type, CategoryImageType imgType, int width, int height)
        {
            if (_isEditMode)
                PhotoService.DeletePhotos(_categoryId, type);

            var photo = new Photo(0, _categoryId, type) { OriginName = file.FileName };
            var photoName = PhotoService.AddPhoto(photo);

            if (!string.IsNullOrWhiteSpace(photoName))
            {
                using (Image image = Image.FromStream(file.InputStream))
                    FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageCategoryPathAbsolut(imgType, photoName), width, height, image);

                return new UploadPictureResult()
                {
                    Result = true,
                    PictureId = photo.PhotoId,
                    Picture = FoldersHelper.GetImageCategoryPath(imgType, photoName, false)
                };
            }

            return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
        }

    }
}
