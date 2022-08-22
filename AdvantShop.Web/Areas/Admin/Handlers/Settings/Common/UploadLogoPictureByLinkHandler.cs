using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Models.Settings;
using Image = System.Drawing.Image;

namespace AdvantShop.Web.Admin.Handlers.Settings.Common
{
    public class UploadLogoPictureByLinkHandler
    {
        private readonly string _fileLink;
        private readonly bool _isLogoMobile;

        public UploadLogoPictureByLinkHandler(string fileLink, bool isLogoMobile = false)
        {
            _fileLink = fileLink;
            _isLogoMobile = isLogoMobile;
        }

        public UploadPictureResult Execute()
        {
            if (string.IsNullOrEmpty(_fileLink))
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            return AddPhoto(_fileLink);
        }


        private UploadPictureResult AddPhoto(string fileLink)
        {
            string error;

            var fileName = fileLink.Substring(fileLink.LastIndexOf("/") + 1);
            var newFileName = fileName.FileNamePlusDate("logo" + (_isLogoMobile ? "_mobile" : ""));

            if (!FileHelpers.CheckFileExtension(newFileName, EAdvantShopFileTypes.Image))
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.InvalidLogoFormat") };

            var photoFullName = FoldersHelper.GetPathAbsolut(FolderType.Pictures, newFileName);

            if (FileHelpers.DownloadRemoteImageFile(fileLink, photoFullName, out error))
            {
                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, _isLogoMobile ? SettingsMobile.LogoImageName : SettingsMain.LogoImageName));


                var img = Image.FromFile(photoFullName, true);

                if (_isLogoMobile)
                {
                    SettingsMobile.LogoImageName = newFileName;
                    SettingsMobile.LogoImageWidth = img.Width;
                    SettingsMobile.LogoImageHeight = img.Height;
                }
                else
                {
                    SettingsMain.LogoImageName = newFileName;
                    SettingsMain.LogoImageWidth = img.Width;
                    SettingsMain.LogoImageHeight = img.Height;
                }



                TrialService.TrackEvent(TrialEvents.ChangeLogo, "");

                // if (!SettingsCongratulationsDashboard.LogoDone)
                // {
                //     SettingsCongratulationsDashboard.LogoDone = true;
                //     Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_LogoDone);
                // }

                return new UploadPictureResult()
                {
                    Result = true,
                    Picture = FoldersHelper.GetPath(FolderType.Pictures, _isLogoMobile ? SettingsMobile.LogoImageName : SettingsMain.LogoImageName, true)
                };
            }

            return new UploadPictureResult() { Error = error ?? LocalizationService.GetResource("Admin.Error.FileNotFound") };
        }
    }
}
