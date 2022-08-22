using System.Collections.Generic;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Mails;
using AdvantShop.Saas;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.Controllers;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.CMS;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Customers;
using AdvantShop.ViewModel.Managers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class ManagersController : BaseClientController
    {
        [AccessByChannel(EProviderSetting.StoreActive)]
        public ActionResult Index(int? departmentId)
        {
            if (!SettingsCheckout.ShowManagersPage 
                && (!SaasDataService.IsSaasEnabled || (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.EmployeesCount == 0)))
            {
                return Error404();
            }

            var department = departmentId != null ? DepartmentService.GetDepartment((int)departmentId) : null;

            var model = new ManagersViewModel
            {
                Managers = ManagerService.GetManagersList(),
                Departments = new DepartmentsListViewModel
                {
                    Departments = DepartmentService.GetDepartmentsList(true),
                    Selected = departmentId ?? -1
                }
            };

            if (department != null)
            {
                model.BreadCrumbs = new List<BreadCrumbs>()
                {
                    new BreadCrumbs(T("MainPage"), Url.AbsoluteRouteUrl("Home")),
                    new BreadCrumbs(T("Managers.Index.MangersTitle"), Url.AbsoluteRouteUrl("Managers")),
                    new BreadCrumbs(department.Name, Url.AbsoluteRouteUrl("Managers", new { departmentId}))
                };
            }
            else
            {
                model.BreadCrumbs = new List<BreadCrumbs>()
                {
                    new BreadCrumbs(T("MainPage"), Url.AbsoluteRouteUrl("Home")),
                    new BreadCrumbs(T("Managers.Index.MangersTitle"), Url.AbsoluteRouteUrl("Managers"))
                };
            }

            SetNgController(NgControllers.NgControllersTypes.ManagersCtrl);

            var metaString = T("Managers.Index.MangersTitle") + (department != null ? " - " + department.Name : string.Empty);

            SetMetaInformation(new MetaInfo
            {
                Title = metaString,
                MetaKeywords = metaString,
                MetaDescription = metaString,
                H1 = metaString
            });

            return View(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetModalParams()
        {
            return Json(new
            {
                SettingsCheckout.IsShowUserAgreementText,
                SettingsCheckout.UserAgreementText,
                SettingsCheckout.AgreementDefaultChecked
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RequestCall(string clientName, string clientPhone, string comment, int managerId)
        {
            var manager = ManagerService.GetManager(managerId);
            if (manager == null || string.IsNullOrWhiteSpace(clientName) || string.IsNullOrWhiteSpace(clientPhone))
                return Json(false);

            var mailTemplate = new FeedbackMailTemplate(SettingsMain.SiteUrl, SettingsMain.ShopName,
                                                            HttpUtility.HtmlEncode(clientName),
                                                            "",
                                                            HttpUtility.HtmlEncode(clientPhone),
                                                            T("Managers.RequestCall.RequestCallSubject"),
                                                            T("Managers.RequestCall.Manager") + ": " + HttpUtility.HtmlEncode(manager.FirstName + " " + manager.LastName), 
                                                            string.Empty);
            
            if (!string.IsNullOrWhiteSpace(manager.Email))
                MailService.SendMailNow(manager.CustomerId, manager.Email, mailTemplate);

            MailService.SendMailNow(manager.CustomerId, SettingsMail.EmailForFeedback, mailTemplate);

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendEmail(string clientName, string email, string emailText, int managerId)
        {
            var manager = ManagerService.GetManager(managerId);
            if (manager == null || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(emailText))
                return Json(false);

            var message = string.Format("{0} ({1}) написал {2}", HttpUtility.HtmlEncode(clientName),
                HttpUtility.HtmlEncode(email), HttpUtility.HtmlEncode(emailText));

            MailService.SendMailNow(manager.CustomerId, manager.Email, "Письмо для менеджера " + SettingsMain.SiteUrl, message, true);

            return Json(true);
        }
    }
}