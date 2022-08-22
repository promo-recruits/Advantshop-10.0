using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Common
{
    public class UploadFaviconPictureByLinkHandler
    {
        private readonly string _fileLink;

        public UploadFaviconPictureByLinkHandler(string fileLink)
        {
            _fileLink = fileLink;
        }

        public UploadPictureResult Execute()
        {
            if (string.IsNullOrEmpty(_fileLink))
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            return AddPhoto(_fileLink);
        }


        private UploadPictureResult AddPhoto(string fileLink)
        {
            string error = null;
            var fileName = fileLink.Substring(fileLink.LastIndexOf("/") + 1);
            var newFileName = fileName.FileNamePlusDate("favicon");

            if (!FileHelpers.CheckFileExtension(newFileName, EAdvantShopFileTypes.Favicon))
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.InvalidFaviconFormat") };

            var photoFullName = FoldersHelper.GetPathAbsolut(FolderType.Pictures, newFileName);

            if (FileHelpers.DownloadRemoteImageFile(fileLink, photoFullName, out error))
            {
                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.FaviconImageName));

                SettingsMain.FaviconImageName = newFileName;                    
                return new UploadPictureResult()
                {
                    Result = true,
                    Picture = FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.FaviconImageName, true)
                };
            }

            return new UploadPictureResult() { Error = error ?? LocalizationService.GetResource("Admin.Error.FileNotFound") };
        }
    }
}
