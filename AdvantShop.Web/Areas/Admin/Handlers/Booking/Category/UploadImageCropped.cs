using System;
using System.Drawing;
using System.IO;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Handlers.Booking.Category
{
    public class UploadImageCropped
    {
        private readonly Core.Services.Booking.Category _category;
        private readonly string _fileName;
        private readonly string _base64String;

        public UploadImageCropped(Core.Services.Booking.Category category, string fileName, string base64String)
        {
            _category = category;
            _fileName = fileName;
            _base64String = base64String;
        }

        public string Execute()
        {
            if (!FileHelpers.CheckFileExtension(_fileName, EAdvantShopFileTypes.Image)) 
                return null;
            
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.BookingCategory, _category.Image));

            var ext = Path.GetExtension(_fileName.Split('?')[0]);
            var newFileName = _category.Id + "_" + DateTime.Now.ToString("yyMMddhhmmss") + ext;
            var newFilePath = FoldersHelper.GetPathAbsolut(FolderType.BookingCategory, newFileName);

            try
            {
                var base64 = _base64String.Split(new[] { "base64," }, StringSplitOptions.None)[1];
                var bytes = Convert.FromBase64String(base64);

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (Image image = Image.FromStream(ms, true))
                    {
                        FileHelpers.SaveResizePhotoFile(newFilePath, SettingsPictureSize.BookingCategoryImageWidth, SettingsPictureSize.BookingCategoryImageHeight, image);
                    }
                }

                _category.Image = newFileName;
                CategoryService.Update(_category);

                // delete temp fileif exists
                var filePath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, _fileName.Split('/').LastOrDefault());
                FileHelpers.DeleteFile(filePath);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("UploadImageCropped", ex);
                return null;
            }

            return FoldersHelper.GetPath(FolderType.BookingCategory, _category.Image, false);
        }
    }
}
