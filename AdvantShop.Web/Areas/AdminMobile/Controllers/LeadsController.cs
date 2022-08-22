using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Areas.AdminMobile.Models.Leads;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Saas;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Attributes;
using System.Collections.Generic;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.AdminMobile.Controllers
{
    [SaasFeature(ESaasProperty.HaveCrm)]
    [Auth(RoleAction.Crm)]
    public class LeadsController : BaseAdminMobileController
    {
        // GET: adminmobile/leads
        public ActionResult Index()
        {
            var model = new LeadsViewModel();
            model.Statuses.Add(new SelectListItem {Text = T("AdminMobile.Leads.AllLeads"), Value = ""});
            //foreach (var status in DealStatusService.GetList())
            //{
            //    model.Statuses.Add(new SelectListItem() {Text = status.Name, Value = status.Id.ToString()});
            //}
            
            SetMetaInformation(T("AdminMobile.Leads.Leads"));
            return View(model);
        }

        public JsonResult GetLeads(int page, string status)
        {
            if (page == 0)
                page = 1;

            var paging = new SqlPaging(page, 10);
            paging.Select(
                "[Lead].Id",
                //"Name",
                "[LeadCustomer].FirstName as CustomerFirstName",
                "[LeadCustomer].LastName as CustomerLastName",
                "[LeadCustomer].Patronymic as CustomerPatronymic",
                
                "[DealStatus].[Name]".AsSqlField("Status"),
                "[Lead].CreatedDate"
                );

            paging.From("[Order].[Lead]");
            paging.Left_Join("[Customers].[Customer] as LeadCustomer on [Lead].[CustomerId] = [LeadCustomer].[CustomerId]");
            paging.Left_Join("[CRM].[DealStatus] ON [DealStatus].[Id] = [Lead].[DealStatusId]");
            
            var customer = CustomerContext.CurrentCustomer;
            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.Assigned)
                    {
                        paging.Where("[Lead].ManagerId = {0}", manager.ManagerId);
                    }
                    else if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.AssignedAndFree)
                    {
                        paging.Where("([Lead].ManagerId = {0} or [Lead].ManagerId is null)", manager.ManagerId);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
                paging.Where("[DealStatus].Id = {0}", status.TryParseInt());

            paging.OrderByDesc("CreatedDate");

            var items = paging.PageItemsList<LeadModel>();

            return Json(items);
        }


        // GET: adminmobile/leads/{id}
        public ActionResult Lead(int id)
        {
            var lead = LeadService.GetLead(id);
            if (lead == null || LeadService.CheckAccess(lead) == false)
                return new EmptyResult();

            var model = new LeadViewModel
            {
                Lead = lead,
                Managers = new List<SelectListItem> { new SelectListItem() { Text = "-", Value = "" } }
            };

            Manager manager = null;
            var managers = ManagerService.GetManagers(RoleAction.Crm);
            if (lead.ManagerId.HasValue && (manager = ManagerService.GetManager(lead.ManagerId.Value)) != null)
            {
                model.Manager = CustomerService.GetCustomer(manager.CustomerId);
                if (!managers.Any(x => x.ManagerId == lead.ManagerId.Value))
                    managers.Add(manager);
            }
            model.Managers.AddRange(managers.Select(x => new SelectListItem { Text = x.FullName, Value = x.ManagerId.ToString(), Selected = x.ManagerId == lead.ManagerId }));

            if (lead.CustomerId != null)
                model.Customer = CustomerService.GetCustomer((Guid)lead.CustomerId);

            model.CurrentCustomer = CustomerContext.CurrentCustomer;
            if (lead.LeadCurrency != null)
            {
                model.Currency = (Currency)lead.LeadCurrency;
            }

            model.Statuses = DealStatusService.GetList(lead.SalesFunnelId).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == lead.DealStatusId
            }).ToList();

            SetMetaInformation(lead.FirstName);

            return View(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeStatus(int id, int status)
        {
            var lead = LeadService.GetLead(id);
            if (lead == null || LeadService.CheckAccess(lead) == false)
                return Json(null);

            var statuses = DealStatusService.GetList(lead.SalesFunnelId);
            if (statuses.Find(x => x.Id == status) != null)
            {
                lead.DealStatusId = status;
                LeadService.UpdateLead(lead);
                return Json(new { Result = "success" });
            }

            return Json(null);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeManager(int id, int managerId)
        {
            var lead = LeadService.GetLead(id);
            if (lead == null || LeadService.CheckAccess(lead) == false)
                return Json(null);

            var manager = ManagerService.GetManager(managerId);
            if (manager == null || !manager.Enabled)
                return Json(null);
            
            if (lead.ManagerId != manager.ManagerId)
            {
                lead.ManagerId = manager.ManagerId;
                LeadService.UpdateLead(lead);
                return Json(new { Result = "success" });
            }

            return Json(null);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateOrder(int id)
        {
            var lead = LeadService.GetLead(id);

            if (lead == null || LeadService.CheckAccess(lead) == false)
                return Json(new { Error = "Error" });

            var order = OrderService.CreateOrder(lead);
            if (order == null)
                return Json(new { Error = "Error" });

            return Json(new { Result = "success", OrderUrl = Url.RouteUrl("AdminMobile_Order", new { orderId = order.OrderID }) }); ;
        }
    }
}