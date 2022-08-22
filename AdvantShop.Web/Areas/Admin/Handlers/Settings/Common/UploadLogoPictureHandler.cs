using System.Web;
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
    public class UploadLogoPictureHandler
    {

        private readonly bool _isLogoMobile;

        public UploadLogoPictureHandler(bool isLogoMobile = false)
        {
            _isLogoMobile = isLogoMobile;
        }

        public UploadPictureResult Execute()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            var img = HttpContext.Current.Request.Files["file"];

            if (img != null && img.ContentLength > 0)
            {
                return AddPhoto(img);
            }

            return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
        }

        private UploadPictureResult AddPhoto(HttpPostedFile file)
        {
            if (!FileHelpers.CheckFileExtension(file.FileName, EAdvantShopFileTypes.Image))
                return new UploadPictureResult() { Error = LocalizationService.GetResource("Admin.Error.InvalidLogoFormat") };

            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, _isLogoMobile ? SettingsMobile.LogoImageName : SettingsMain.LogoImageName));

            var newFile = file.FileName.FileNamePlusDate("logo" + (_isLogoMobile ? "_mobile" : ""));
            var img = Image.FromStream(file.InputStream, true, true);

            if (_isLogoMobile)
            {
                SettingsMobile.LogoImageName = newFile;
                SettingsMobile.LogoImageWidth = img.Width;
                SettingsMobile.LogoImageHeight = img.Height;
            }
            else
            {
                SettingsMain.LogoImageName = newFile;
                SettingsMain.LogoImageWidth = img.Width;
                SettingsMain.LogoImageHeight = img.Height;
            }

            file.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, newFile));



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

    }
}
