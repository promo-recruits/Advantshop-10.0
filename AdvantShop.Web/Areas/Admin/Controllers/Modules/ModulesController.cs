using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Customers;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Handlers.Modules;
using AdvantShop.Web.Admin.Models.Modules;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using System.Collections.Generic;
using AdvantShop.Web.Admin.Attributes;
using System.IO;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Diagnostics;

namespace AdvantShop.Web.Admin.Controllers.Modules
{
    [Auth(RoleAction.Modules)]
    public class ModulesController : BaseAdminController
    {
        public ActionResult Index(ModulesFilterModel filter, bool force = false)
        {
            if (!force && !ModulesHandler.IsExistInstallModules())
                return Redirect(Url.RouteUrl(new { Controller = "Modules", action = "Market" }));

            if (!string.IsNullOrEmpty(filter.Name))
            {
                var model = new ModulesHandler().GetLocalModules(filter);
                if (model.Count == 0)
                    return Redirect(Url.RouteUrl(new { controller = "Modules", action = "Market" }) + "?name=" + filter.Name);

                if (model.Count == 1)
                    return Redirect(Url.RouteUrl(new { controller = "Modules", action = "Details" }) + "?id=" + model[0].StringId);
            }

            SetMetaInformation(T("Admin.Modules.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.ModulesCtrl);

            return View();
        }

        public JsonResult GetLocalModules(ModulesFilterModel filter)
        {
            return Json(new ModulesHandler().GetLocalModules(filter));
        }

        public JsonResult GetMarketModules(ModulesFilterModel filter)
        {
            return Json(new ModulesHandler().GetMarketModules(filter));
        }

        public ActionResult Market(ModulesFilterModel filter = null)
        {
            SetMetaInformation(T("Admin.Modules.Market.Title"));
            SetNgController(NgControllers.NgControllersTypes.ModulesCtrl);

            return View();
        }

        public ActionResult Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var module = new ModulesHandler().GetModule(id);
            if (module == null)
                return RedirectToAction("Index");

            var model = new DetailsModel { Module = module };

            var moduleType = AttachedModules.GetModuleById(module.StringId);
            var m = moduleType != null ? Activator.CreateInstance(moduleType) : null;
            if (m != null)
            {
                var settings = m as IAdminModuleSettings;
                if (settings != null && settings.AdminSettings != null && settings.AdminSettings.Count > 0)
                    model.Settings = settings.AdminSettings;

                var moduleFromServer = ModulesService.GetModuleObjectFromRemoteServer(module.StringId);
                if (moduleFromServer != null && !string.IsNullOrEmpty(moduleFromServer.InstructionLink))
                {
                    model.InstructionTitle = string.IsNullOrEmpty(moduleFromServer.InstructionTitle)
                        ? "Инструкция"
                        : moduleFromServer.InstructionTitle;
                    model.InstructionUrl = string.Format("{0}?v={1}&moduleversion={2}",
                        moduleFromServer.InstructionLink, SettingsGeneral.SiteVersionDev, moduleFromServer.Version);
                }
            }

            SetMetaInformation(T("Admin.Modules.Details.Title", module.Name));
            SetNgController(NgControllers.NgControllersTypes.ModuleCtrl);

            return View(model);
        }

        public ActionResult Preview(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            if (AttachedModules.GetModuleById(id) != null)
                return RedirectToAction("Details", new {id});

            var channel = SalesChannelService.GetByType(ESalesChannelType.Module, id);
            if (channel == null)
                return RedirectToAction("Index");

            SetMetaInformation(T("Admin.Modules.Details.Title", channel.Name));
            SetNgController(NgControllers.NgControllersTypes.ModulesCtrl);

            return View(channel);
        }

        #region Install, update, uninstall, enable module

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InstallModule(string stringId, string id, string version, bool? active = null)
        {
            if (string.IsNullOrWhiteSpace(stringId) || string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(version))
            {
                return Json(new { result = false });
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Modules_ModuleInstalled, stringId);

            var moduleInst = AttachedModules.GetModuleById(stringId);
            if (moduleInst != null)
            {
                ModulesService.InstallModule(stringId, version);

                if (active != null && active.Value)
                    ModulesRepository.SetActiveModule(stringId, true);

                return Json(new { result = true, url = Url.AbsoluteActionUrl("Details", "Modules", new { id = stringId }) });
            }

            ModulesRepository.SetModuleNeedUpdate(stringId, true);

            var message = ModulesService.GetModuleArchiveFromRemoteServer(id);
            if (message.IsNullOrEmpty())
            {
                HttpRuntime.UnloadAppDomain();

                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();

                return Json(new
                {
                    result = true,
                    url = Url.AbsoluteActionUrl("InstallModuleInDb", "Modules", new {stringId, id, version, active})
                });
            }

            return Json(new { result = false });
        }

        public ActionResult InstallModuleInDb(string stringId, string id, string version, bool? active = null)
        {
            var moduleInst = AttachedModules.GetModuleById(stringId);
            if (moduleInst != null)
            {
                ModulesService.InstallModule(stringId, version);

                if (active != null && active.Value)
                    ModulesRepository.SetActiveModule(stringId, true);
            }
            return RedirectToAction("Details", "Modules", new { id = stringId });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateModule(string stringId, string id, string version)
        {
            ModulesRepository.SetModuleNeedUpdate(stringId, true);

            var message = ModulesService.GetModuleArchiveFromRemoteServer(id, update: true);
            if (message.IsNullOrEmpty())
            {
                ModulesService.InstallModule(stringId.ToLower(), version);
                //ModulesRepository.SetModuleNeedUpdate(stringId, false);
            }

            HttpRuntime.UnloadAppDomain();

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateAllModules(List<Module> modules)
        {
            string tempPath = Server.MapPath("~/App_Data/TempModules");
            Helpers.FileHelpers.CreateDirectory(tempPath);

            var modulesList = ModulesService.GetModules().Items.Where(module => module.IsInstall);

            foreach (var module in modulesList)
            {
                if (module.IsLocalVersion || module.IsCustomVersion || module.Version == module.CurrentVersion)
                {
                    continue;
                }

                ModulesRepository.SetModuleNeedUpdate(module.StringId, true);

                var message = ModulesService.GetModuleArchiveFromRemoteServer(module.Id.ToString(), tempPath, true);
                if (!message.IsNullOrEmpty())
                {
                    module.IsInstall = false;
                }
            }

            var path = "";
            try
            {
                foreach (var file in Directory.GetFiles(tempPath, "*", SearchOption.AllDirectories).OrderByDescending(x => x))
                {
                    var newFile = file.Replace("App_Data\\TempModules\\", "");
                    Helpers.FileHelpers.DeleteFile(newFile);

                    FileInfo fi = new FileInfo(newFile);
                    Helpers.FileHelpers.CreateDirectory(fi.DirectoryName);

                    System.IO.File.Move(file, newFile);
                    
                }

                Directory.Delete(tempPath, true);

                foreach (var module in modulesList.Where(m=> m.IsInstall))
                {
                    ModulesService.InstallModule(module.StringId.ToLower(), module.Version);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(path, ex);
            }

            HttpRuntime.UnloadAppDomain();

            return JsonOk();
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeEnabled(string stringId, bool enabled)
        {
            if (string.IsNullOrWhiteSpace(stringId))
                return Json(new { result = false });

            ModulesRepository.SetActiveModule(stringId, enabled);

            TrialService.TrackEvent(enabled ? TrialEvents.ActivateModule : TrialEvents.DeactivateModule, stringId);

            if (stringId.ToLower() == "yametrika" && enabled)
                TrialService.TrackEvent(TrialEvents.SetUpYandexMentrika, string.Empty);

            Module module = null;

            if (Saas.SaasDataService.IsSaasEnabled)
            {
                module =
                    ModulesService.GetModules().Items.FirstOrDefault(x => x.StringId.ToLower() == stringId.ToLower());
            }

            return JsonOk(new { SaasAndPaid = module != null && module.Price > 0f });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UninstallModule(string stringId)
        {
            var channel = SalesChannelService.GetByType(ESalesChannelType.Module, stringId);

            if (channel != null && channel.ShowInstalledAndPreview)
                SalesChannelService.SetNotShowInstalled(stringId, true);
            
            var result = ModulesService.UninstallModule(stringId);
            
            return result.IsNullOrEmpty() ? JsonOk() : JsonError(result);
        }

        #endregion
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult IsInstallModule(string stringId)
        {
            var result = ModulesRepository.IsInstallModule(stringId);
            return Json(result);
        }
    }
}
