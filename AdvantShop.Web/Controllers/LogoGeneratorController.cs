using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Design;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Models.Common;
using AdvantShop.Trial;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Core.Services.Screenshot;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class LogoGeneratorController : Controller
    {
        [AdminAuth]
        public JsonResult GetData()
        {
            return Json(new
            {
                logo = new
                {
                    text = SettingsMain.ShopName,
                    style = new
                    {
                        color = DesignService.GetDesigns(eDesign.Color).FirstOrDefault(x => x.Name == SettingsDesign.ColorScheme).Color,
                        fontFamily = "Lobster",
                        fontSize = 48,
                        textAlign = "center"
                    },
                    font = new
                    {
                        link = "https://fonts.googleapis.com/css?family=Lobster&amp;subset=cyrillic",
                        fontFamily = "Lobster",
                        languages = new List<string>() {
                            "latin",
                            "cyrillic"
                        }
                    }
                },
                slogan = new
                {
                    text = "Слоган интернет-магазина",
                    style = new
                    {
                        color = DesignService.GetDesigns(eDesign.Color).FirstOrDefault(x => x.Name == SettingsDesign.ColorScheme).Color,
                        fontFamily = "Old Standard TT",
                        fontSize = 14,
                        textAlign = "center",
                        marginTop = 0,
                        marginBottom = 0
                    },
                    font = new
                    {
                        link = "https://fonts.googleapis.com/css?family=Old+Standard+TT&amp;subset=cyrillic",
                        fontFamily = "Old Standard TT",
                        languages = new List<string>() {
                            "latin",
                            "cyrillic"
                        }
                    },
                    marginValue = 0
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [AdminAuth]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveLogo(string dataUrl, string fileExtension, string fontFamilyLogo, string cameFrom)
        {
            try
            {
                if (FileHelpers.CheckFileExtension(fileExtension, EAdvantShopFileTypes.Image))
                {
                    var base64 = dataUrl.Split(new[] { "base64," }, StringSplitOptions.None)[1];
                    var bytes = Convert.FromBase64String(base64);

                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        using (Image image = Image.FromStream(ms, true))
                        {
                            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
                            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoImageName));

                            var newFile = "logo_generated".FileNamePlusDate() + FileHelpers.GetExtension(fileExtension);
                            var newFilePath = FoldersHelper.GetPathAbsolut(FolderType.Pictures, newFile);

                            SettingsMain.LogoImageName = newFile;
                            FileHelpers.SaveResizePhotoFile(newFilePath, 1200, 500, image);
                            var img = Image.FromFile(newFilePath, true);
                            SettingsMain.LogoImageWidth = img.Width;
                            SettingsMain.LogoImageHeight = img.Height;
                        }
                    }

                    TrialService.TrackEvent(TrialEvents.GenerateLogo, fontFamilyLogo);
                    // if (!SettingsCongratulationsDashboard.LogoDone)
                    // {
                    //     SettingsCongratulationsDashboard.LogoDone = true;
                         Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_LogoDone);
                    // }
                    if (cameFrom == "adminArea")
                        Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_GenerateLogo_AdminArea);

                    new ScreenshotService().UpdateStoreScreenShotInBackground();

                    return Json(new LogoModel());
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("SaveLogoGenerator", ex);
            }
            return null;
        }
    }
}