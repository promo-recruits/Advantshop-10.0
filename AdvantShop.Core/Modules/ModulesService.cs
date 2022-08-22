//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using Newtonsoft.Json;
using AdvantShop.Core.Services.Mails;
using System.Collections.Generic;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.SalesChannels;

namespace AdvantShop.Core.Modules
{
    public class ModulesService
    {
        private const string RequestUrlGetModules = "http://modules.advantshop.net/DownloadableContent/GetDlcs?id={0}&dlctype=Module&storeversion={1}";
        private const string RequestUrlGetModuleArchive = "http://modules.advantshop.net/DownloadableContent/GetDlc?lickey={0}&dlcId={1}&storeversion={2}&update={3}";
        private const string RequestUrlGetModuleObject = "http://modules.advantshop.net/DownloadableContent/GetDlcObject?id={0}&dlcStringId={1}&dlctype=Module";
        private const string RequestUrlDeleteModule = "http://modules.advantshop.net/DownloadableContent/DeleteDlc?lickey={0}&dlcStringId={1}";

        private const string ModulesCacheKey = "AdvantshopModules";

        #region Process modules from remote server

        public static ModuleBox GetModules()
        {
            var modules = GetModulesFromRemoteServer() ?? new ModuleBox();

            foreach (var type in AttachedModules.GetModules())
            {
                var moduleInst = (IModule)Activator.CreateInstance(type);
                var curModule = modules.Items.Count > 0
                                    ? modules.Items.FirstOrDefault(x => x.StringId.ToLower() == moduleInst.ModuleStringId.ToLower())
                                    : null;

                if (curModule != null)
                {
                    curModule.IsInstall = moduleInst.CheckAlive() &&
                                        ModulesRepository.IsInstallModule(moduleInst.ModuleStringId);
                    curModule.HasSettings = moduleInst.HasSettings;
                    curModule.Enabled = ModulesRepository.IsActiveModule(moduleInst.ModuleStringId);
                    curModule.IsCustomVersion = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/modules/" + moduleInst.ModuleStringId + "/custom.txt"));

                    if (ModulesRepository.GetModulesFromDb().All(x => x.StringId != moduleInst.ModuleStringId))
                    {
                        curModule.IsLocalVersion = true;
                        curModule.Version = curModule.CurrentVersion = LocalizationService.GetResource("Admin.Core.Modules.ModuleInDebug");
                    }
                }
                else
                {
                    modules.Items.Add(new Module()
                    {
                        Name = moduleInst.ModuleName,
                        StringId = moduleInst.ModuleStringId,
                        Version = LocalizationService.GetResource("Admin.Core.Modules.ModuleInDebug"),
                        IsInstall =
                            moduleInst.CheckAlive() && ModulesRepository.IsInstallModule(moduleInst.ModuleStringId),
                        Price = 0,
                        IsLocalVersion = true,
                        Active = true,
                        HasSettings = moduleInst.HasSettings,
                        Enabled = ModulesRepository.IsActiveModule(moduleInst.ModuleStringId),
                        Icon =
                            File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/modules/" + moduleInst.ModuleStringId + "/icon.jpg"))
                                ? UrlService.GetUrl("modules/" + moduleInst.ModuleStringId + "/icon.jpg")
                                : UrlService.GetUrl("images/nophoto_small.jpg"), //null
                        IsCustomVersion = File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/modules/" + moduleInst.ModuleStringId + "/custom.txt"))
                    });
                }
            }

            var existModules = ModulesRepository.GetModulesFromDb();
            if (existModules.Count > 0)
            {
                foreach (var module in modules.Items)
                {
                    var currentModule = existModules.FirstOrDefault(x => x.StringId.ToLower() == module.StringId.ToLower());
                    if (currentModule != null)
                    {
                        module.CurrentVersion = currentModule.Version;
                        module.IsInstall = currentModule.IsInstall;
                    }
                }
            }
            return modules;
        }

        private static ModuleBox GetModulesFromRemoteServer()
        {
            return CacheManager.Get(ModulesCacheKey, 5, () =>
            {
                var modules = new ModuleBox();

                try
                {
                    var url = string.Format(RequestUrlGetModules,
                        SaasDataService.IsSaasEnabled ? SettingsGeneral.CurrentSaasId : SettingsLic.LicKey,
                        SettingsGeneral.SiteVersionDev);

                    var request = WebRequest.Create(url);
                    request.Method = "GET";

                    using (var dataStream = request.GetResponse().GetResponseStream())
                    {
                        using (var reader = new StreamReader(dataStream))
                        {
                            var responseFromServer = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(responseFromServer))
                            {
                                modules = JsonConvert.DeserializeObject<ModuleBox>(responseFromServer);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }

                return modules;
            });
        }

        private static bool DeleteModuleFromRemoteServer(string stringId)
        {
            bool result = false;
            try
            {
                var url = string.Format(RequestUrlDeleteModule, SettingsLic.LicKey, stringId);

                var request = WebRequest.Create(url);
                request.Method = "GET";

                using (var dataStream = request.GetResponse().GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        var responseFromServer = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(responseFromServer))
                        {
                            result = JsonConvert.DeserializeObject<bool>(responseFromServer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                result = false;
            }
            return result;
        }


        public static string GetModuleArchiveFromRemoteServer(string moduleId, string tempFolder = null, bool update = false)
        {
            var zipFileName = HttpContext.Current.Server.MapPath("~/App_Data/" + Guid.NewGuid() + ".Zip");
            try
            {
                new WebClient().DownloadFile(
                    string.Format(RequestUrlGetModuleArchive, SettingsLic.LicKey, moduleId, SettingsGeneral.SiteVersionDev, update),
                    zipFileName
                   );

                if (!FileHelpers.UnZipFile(zipFileName, tempFolder ?? HttpContext.Current.Server.MapPath("~/")))
                {
                    FileHelpers.DeleteFile(zipFileName);
                    return "error on UnZipFile";
                }
                FileHelpers.DeleteFile(zipFileName);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return "error on UnZipFile";
            }

            return string.Empty;
        }

        public static Module GetModuleObjectFromRemoteServer(string moduleStringId)
        {
            var url = string.Format(RequestUrlGetModuleObject,
                    SaasDataService.IsSaasEnabled ? SettingsGeneral.CurrentSaasId : SettingsLic.LicKey,
                    moduleStringId);

            var request = WebRequest.Create(url);
            request.Method = "GET";
            var module = new Module();
            using (var dataStream = request.GetResponse().GetResponseStream())
            {
                using (var reader = new StreamReader(dataStream))
                {
                    var responseFromServer = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(responseFromServer))
                    {
                        module = JsonConvert.DeserializeObject<Module>(responseFromServer);
                    }
                }
            }
            return module;
        }




        #endregion

        #region Public methods to process modules

        public static bool InstallModule(string moduleStringId)
        {
            return InstallModule(moduleStringId, "0");
        }

        public static bool InstallModule(string moduleStringId, string version)
        {
            var moduleInst = AttachedModules.GetModuleById(moduleStringId.ToLower());
            if (moduleInst != null)
            {
                var module = ((IModule)Activator.CreateInstance(moduleInst, null));

                var isInstallModule = ModulesRepository.IsInstallModule(module.ModuleStringId);
                var result = !isInstallModule
                                ? module.InstallModule()
                                : module.UpdateModule();

                if (result)
                {
                    ModulesRepository.InstallModuleToDb(
                        new Module
                        {
                            StringId = module.ModuleStringId,
                            Name = module.ModuleName,
                            DateModified = DateTime.Now,
                            DateAdded = DateTime.Now,
                            Version = version,
                            Active = false,
                            HasSettings = module.HasSettings,
                            NeedUpdate = isInstallModule ? true : false
                        });
                    return true;
                }
            }

            return false;
        }

        public static string UninstallModule(string moduleStringId)
        {
            if (string.IsNullOrWhiteSpace(moduleStringId))
                return "moduleStringId is empty";

            if (!DeleteModuleFromRemoteServer(moduleStringId))
            {
                return "Not deleted from remote server";
            }

            var moduleInst = AttachedModules.GetModuleById(moduleStringId);
            if (moduleInst != null)
            {
                var module = (IModule)Activator.CreateInstance(moduleInst);
                module.UninstallModule();

                FileHelpers.DeleteDirectory(HttpContext.Current.Server.MapPath("~/Modules/" + moduleStringId));
                FileHelpers.DeleteFile(HttpContext.Current.Server.MapPath("~/bin/" + moduleInst.Assembly.GetName().Name + ".dll")); // AdvantShop.Module." + moduleStringId + 
            }

            ModulesRepository.UninstallModuleFromDb(moduleStringId);

            return "";
        }

        public static void CallModulesUpdate()
        {
            var modulesInDb = ModulesRepository.GetModulesFromDb().Where(x => x.NeedUpdate).ToList();

            if (modulesInDb.Count == 0)
                return;

            foreach (var module in AttachedModules.GetModules<IModule>())
            {
                var classInstance = (IModule)Activator.CreateInstance(module, null);
                var moduleInDb = modulesInDb.FirstOrDefault(item => item.StringId == classInstance.ModuleStringId);
                if (moduleInDb != null)
                {
                    if (classInstance.UpdateModule())
                    {
                        ModulesRepository.SetModuleNeedUpdate(moduleInDb.StringId, false);
                    }
                }
            }
        }

        public static string GetModuleStringIdByUrlPath(string urlPath)
        {
            if (urlPath.IsNullOrEmpty())
                return string.Empty;

            var moduleInst = AttachedModules.GetModules<IClientPageModule>().FirstOrDefault(
                        item =>
                        ((IClientPageModule)Activator.CreateInstance(item, null)).UrlPath.ToLower() == SQLDataHelper.GetString(urlPath).ToLower());

            if (moduleInst != null)
            {
                var module = ((IModule)Activator.CreateInstance(moduleInst, null));
                return module.ModuleStringId;
            }

            return string.Empty;
        }
        #endregion

        #region Call core methods for modules
        public static void SendModuleMail(Guid customerIdTo, string subject, string message, string email, bool isBodyHtml)
        {
            MailService.SendMailNow(customerIdTo, email, subject, message, isBodyHtml);
        }

        public static Dictionary<string, string> GetFunnels()
        {
            var result = new Dictionary<string, string>();

            foreach (var salesFunnel in SalesFunnelService.GetList())
            {
                result.Add(salesFunnel.Id.ToString(), salesFunnel.Name);
            }
            return result;
        }
        #endregion

        public static List<SalesChannel> GetModuleSalesChannels()
        {
            return CacheManager.Get(ModulesCacheKey + "_SalesChannels", 25, () =>
            {
                var moduleBox = GetModulesFromRemoteServer();
                var modules = moduleBox != null && moduleBox.Items != null ? moduleBox.Items : new List<Module>();

                var salesChannels = modules.Where(x => x.ShowInSalesChannels).Select(GetSalesChannelFromModule).ToList();
                return salesChannels;
            });
        }

        private static SalesChannel GetSalesChannelFromModule(Module module)
        {
            return new SalesChannel(new SalesChannelConfigModel()
            {
                ModuleId = module.Id,
                ModuleStringId = module.StringId,
                ModuleVersion =  module.Version,
                Type = ESalesChannelType.Module,

                Name = module.TitleInSalesChannels,
                MenuName = module.MenuNameInSalesChannels,
                Url =
                    module.ShowInstalledAndPreviewInSalesChannel && !ModulesRepository.IsInstallModule(module.StringId)
                        ? "modules/preview/" + module.StringId.ToLower()
                        : "modules/details/" + module.StringId.ToLower(),
                MenuIcon = module.MenuIconInSalesChannels,
                Icon = module.IconInSalesChannels,
                Description = module.BriefDescriptionInSalesChannels,
                Details = new SalesChannelDetails()
                {
                    Title = module.TitleInSalesChannels,
                    Text = module.DescriptionInSalesChannels,
                    Images =
                        !string.IsNullOrEmpty(module.ImageInSalesChannels)
                            ? new List<SalesChannelPicture>()
                            {
                                new SalesChannelPicture()
                                {
                                    Src = module.ImageInSalesChannels,
                                    Alt = module.Name
                                }
                            }
                            : new List<SalesChannelPicture>(),
                    PriceString = module.PriceString,
                    Price = module.Price,
                },
                ShowInstalledAndPreview = module.ShowInstalledAndPreviewInSalesChannel,
                PreviewRightText = module.PreviewRightTextInSalesChannel,
                PreviewLeftText = module.PreviewLeftTextInSalesChannel,
                PreviewButtonText = module.PreviewButtonTextInSalesChannel
            });
        }

        //public static void SetDeleted()
        //{

        //}
    }
}