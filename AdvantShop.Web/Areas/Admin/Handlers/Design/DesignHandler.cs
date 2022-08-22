using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Screenshot;
using AdvantShop.Design;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.ViewModels.Design;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace AdvantShop.Web.Admin.Handlers.Design
{
    public class DesignHandler
    {
        private const string _none = "_none";
        private string stringId;

        public DesignHandler(string filterStringId = null)
        {
            stringId = filterStringId;
        }

        public DesignModel Execute()
        {
            var templatesList = TemplateService.GetTemplates();

            if (stringId.IsNotEmpty())
            {
                templatesList.Items = templatesList.Items.Where(x => string.Equals(x.StringId.ToLower(), stringId.ToLower()) || string.Equals(x.StringId, SettingsDesign.TemplateInDb)).ToList();
            }

            var avaliableTemplates = templatesList.Items.Where(x => x.StringId != SettingsDesign.TemplateInDb && x.Active && (x.IsInstall || x.StringId == TemplateService.DefaultTemplateId)).ToList();
            var marketTemplates = templatesList.Items.Where(x => x.StringId != SettingsDesign.TemplateInDb && !avaliableTemplates.Any(ax => ax.StringId == x.StringId)).ToList();

            return new DesignModel()
            {
                AvaliableTemplates = avaliableTemplates,
                MarketTemplates = marketTemplates,
                CurrentTemplate = templatesList.Items.FirstOrDefault(x => x.StringId == SettingsDesign.TemplateInDb),
                CurrentPreviewTemplate = templatesList.Items.FirstOrDefault(x => x.StringId == SettingsDesign.PreviewTemplate),
                Themes = DesignService.GetDesigns(eDesign.Theme, false),
                BackGrounds = DesignService.GetDesigns(eDesign.Background, false),
                ColorSchemes = DesignService.GetDesigns(eDesign.Color, false),

                CurrentTheme = SettingsDesign.ThemeInDb,
                CurrentBackGround = SettingsDesign.BackgroundInDb,
                CurrentColorScheme = SettingsDesign.ColorSchemeInDb,
            };
        }

        public bool UploadDesignFile(eDesign designType, HttpPostedFileBase file)
        {
            if (!FileHelpers.CheckFileExtension(file.FileName, EAdvantShopFileTypes.Zip))
            {
                return false;
            }

            var fileName = Path.GetFileNameWithoutExtension(file.FileName);

            var themes = DesignService.GetDesigns(designType, false);
            var existingItem = themes.Find(x => x.Name.ToLower() == fileName.ToLower());
            if (existingItem != null && !existingItem.Custom)
                return false;

            string designFolderPath = SettingsDesign.Template != TemplateService.DefaultTemplateId
                                          ? HttpContext.Current.Server.MapPath("~/Templates/" + SettingsDesign.Template + "/")
                                          : HttpContext.Current.Server.MapPath("~/");

            string filename = string.Format("{0}/design/{1}s/{2}", designFolderPath, designType.ToString(), file.FileName);

            bool result;
            try
            {
                file.SaveAs(filename);
                result = FileHelpers.UnZipFile(filename);
                FileHelpers.DeleteFile(filename);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                result = false;
            }

            CacheManager.RemoveByPattern(CacheNames.GetDesignCacheObjectName(""));

            if (!result)
                return false;

            //если удачно распаковали - обновляем в кофиге custom тег
            try
            {
                var configPath = string.Format("{0}/design/{1}s/{2}/{1}.config", designFolderPath,
                    designType.ToString(), fileName);
                if (File.Exists(configPath))
                {
                    Theme theme = null;
                    var serializer = new XmlSerializer(typeof(Theme));
                    using (var streamReader = new StreamReader(configPath))
                    {
                        theme = (Theme)serializer.Deserialize(streamReader);
                        streamReader.Close();
                    }

                    theme.Custom = true;

                    using (var streamWriter = new StreamWriter(configPath, false, Encoding.UTF8))
                    {
                        var xmlNamespaces = new XmlSerializerNamespaces();
                        xmlNamespaces.Add("", "");
                        serializer.Serialize(streamWriter, theme, xmlNamespaces);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return true;
        }

        public bool DeleteDesign(eDesign designType, string name)
        {
            if (name == _none)
                return false;

            string designFolderPath = SettingsDesign.TemplateInDb != TemplateService.DefaultTemplateId
                                          ? HttpContext.Current.Server.MapPath("~/Templates/" + SettingsDesign.Template + "/")
                                          : HttpContext.Current.Server.MapPath("~/");

            try
            {
                var _currentDesigns = DesignService.GetDesigns(designType, false);
                string dirname = string.Format("{0}design\\{1}s\\{2}", designFolderPath, designType.ToString(), name);

                switch (designType)
                {
                    case eDesign.Theme:
                        if (SettingsDesign.ThemeInDb == name)
                        {
                            var theme = _currentDesigns.FirstOrDefault(t => t.Name != name);
                            SettingsDesign.ThemeInDb = theme != null ? theme.Name : _none;
                        }
                        break;

                    case eDesign.Background:
                        if (SettingsDesign.BackgroundInDb == name)
                        {
                            var theme = _currentDesigns.FirstOrDefault(t => t.Name != name);
                            SettingsDesign.BackgroundInDb = theme != null ? theme.Name : _none;
                        }
                        break;

                    case eDesign.Color:
                        if (SettingsDesign.ColorSchemeInDb == name)
                        {
                            var theme = _currentDesigns.FirstOrDefault(t => t.Name != name);
                            SettingsDesign.ColorSchemeInDb = theme != null ? theme.Name : _none;
                        }
                        break;
                }

                FileHelpers.DeleteDirectory(dirname);

                CacheManager.RemoveByPattern(CacheNames.GetDesignCacheObjectName(""));

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }
        }

        public bool SaveDesign(eDesign designType, string name)
        {
            if (name.IsNullOrEmpty())
            {
                return false;
            }

            switch (designType)
            {
                case eDesign.Theme:
                    SettingsDesign.ThemeInDb = name;
                    SettingsDesign.BackgroundInDb = _none;
                    break;

                case eDesign.Background:
                    SettingsDesign.BackgroundInDb = name;
                    SettingsDesign.ThemeInDb = _none;
                    break;

                case eDesign.Color:
                    SettingsDesign.ColorSchemeInDb = name;
                    var mobileBrowserColorVariantsSelected = SettingsMobile.BrowserColorVariantsSelected.TryParseEnum(SettingsMobile.eBrowserColorVariants.ColorScheme);
                    if (mobileBrowserColorVariantsSelected == SettingsMobile.eBrowserColorVariants.ColorScheme)
                    {
                        var curColorScheme = DesignService.GetCurrenDesign(eDesign.Color);
                        SettingsMobile.BrowserColor = curColorScheme.Color;
                    }
                    break;
            }

            CacheManager.Clean();

            new ScreenshotService().UpdateStoreScreenShotInBackground();

            return true;
        }
    }
}