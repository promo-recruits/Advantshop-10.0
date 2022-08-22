using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Repository;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Customers;
using AdvantShop.Web.Admin.Models.Customers;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Customers
{
    public partial class CustomersCrmController : CustomersController { }

    [Auth(RoleAction.Customers)]
    [AccessBySettings(ETypeRedirect.AdminPanel, EProviderSetting.CrmActive, EProviderSetting.StoreActive)]
    public partial class CustomersController : BaseAdminController
    {
        #region Customers List

        public ActionResult Index(CustomersFilterModel filter)
        {
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var customer = CustomerService.GetCustomerByEmail(filter.Search);
                if (customer != null)
                    return RedirectToAction("Edit", new { id = customer.Id });

                var customerByCode = ClientCodeService.GetCustomerByCode(filter.Search, Guid.Empty);
                if (customerByCode != null && customerByCode.Id != Guid.Empty)
                {
                    return RedirectToAction("Edit", new { id = customerByCode.Id, code = filter.Search });
                }
            }
            
            SetMetaInformation(T("Admin.Customers.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.CustomersCtrl);

            return View("~/Areas/Admin/Views/Customers/Index.cshtml");
        }


        public ActionResult GetCustomers(CustomersFilterModel model)
        {
            var exportToCsv = model.OutputDataType == FilterOutputDataType.Csv;

            var result = new GetCustomersPaging(model, exportToCsv).Execute();

            if (exportToCsv)
            {
                var fileName = "export_grid_customers.csv";
                var fullFilePath = new ExportCustomersHandler(result, fileName.FileNamePlusDate()).Execute();
                return FileDeleteOnUpload(fullFilePath, "application/octet-stream", fileName);
            }

            return Json(result);
        }


        public JsonResult GetCustomerIds(CustomersFilterModel command)
        {
            var customerIds = new List<Guid>();
            Command(command, (id, c) =>
            {
                customerIds.Add(id);
                return true;
            });
            return Json(new { customerIds });
        }

        #region Commands

        private bool Command(CustomersFilterModel command, Func<Guid, CustomersFilterModel, bool> func)
        {
            bool result = true;

            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    if (func(id, command) == false)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                var handler = new GetCustomersPaging(command);
                var ids = handler.GetItemsIds("[Customer].[CustomerID]");

                foreach (var id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                    {
                        if (func(id, command) == false)
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCustomers(CustomersFilterModel command)
        {
            bool result = Command(command, (id, c) => DeleteCustomerById(id));
            return result ? JsonOk(result) : JsonError(T("Admin.Customers.ErrorWhileDeletingUsers"));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddTagsToCustomers(CustomersFilterModel command, List<string> tags)
        {
            foreach (var tag in tags)
            {
                if (TagService.Get(tag) == null)
                {
                    TagService.Add(new Tag
                    {
                        Name = tag,
                        Enabled = true
                    });
                }
            }

            bool result = Command(command, (id, c) =>
            {
                try
                {
                    var customer = CustomerService.GetCustomer(id);
                    var prevTags = TagService.Gets(customer.Id).Select(x => x.Name).ToList();
                    var newTags = tags.Where(x => !prevTags.Contains(x)).ToList();

                    customer.Tags = prevTags.Concat(newTags).Select(x => new Tag
                    {
                        Name = x,
                        Enabled = true,
                    }).ToList();

                    CustomerService.UpdateCustomer(customer);
                }
                catch(Exception ex)
                {
                    Debug.Log.Error("AddTagsToCustomers", ex);
                    return false;
                }
                
                return true;
            });

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_AddTagToCustomer);

            return result ? JsonOk() : JsonError();
        }
        #endregion


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCustomer(Guid customerid)
        {
            var result = DeleteCustomerById(customerid);
            return Json(result);
        }

        private bool DeleteCustomerById(Guid customerid)
        {
            if (!CustomerService.CanDelete(customerid))
            {
                // add message this customer can be deleted
                return false;
            }

            try
            {
                CustomerService.DeleteCustomer(customerid);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("DeleteCustomerById", ex);
                return false;
            }

            return true;
        }

        #endregion

        #region Add | Edit customer

        // Редирект на PopupAdd
        public ActionResult Add(AddCustomerModel model)
        {
            return Redirect(
                UrlService.GetAdminUrl(model.OrderId != null
                    ? "customers?orderid=" + model.OrderId + "#?customerIdInfo="
                    : "customers#?customerIdInfo="));
        }

        // Редирект на Popup
        public ActionResult Edit(Guid id, string code)
        {
            return Redirect(UrlService.GetAdminUrl("customers#?customerIdInfo=" + id));
        }

        [Auth(EAuthErrorType.PartialView, RoleAction.Customers)]
        public ActionResult PopupAdd(AddCustomerModel addCustomerModel)
        {
            var model = new GetCustomer(addCustomerModel).Execute();
            if (model == null)
                return Error404(partial: true);

            return PartialView("~/Areas/Admin/Views/Customers/AddEditPopup.cshtml", model);
        }

        [Auth(EAuthErrorType.PartialView, RoleAction.Customers)]
        public ActionResult Popup(Guid id, string code)
        {
            var model = new GetCustomer(id, code).Execute();
            if (model == null)
                return Error404(partial: true);

            return PartialView("~/Areas/Admin/Views/Customers/AddEditPopup.cshtml", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult SavePopup(CustomersModel model)
        {
            if (ModelState.IsValid)
            {
                var result = new AddUpdateCustomer(model).Execute();
                if (result)
                    return JsonOk(model.CustomerId);
            }
            return JsonError();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddEdit(CustomersModel model)
        {
            if (ModelState.IsValid)
            {
                var result = new AddUpdateCustomer(model).Execute();
                if (result)
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                    return Redirect(UrlService.GetAdminUrl("customers#?customerIdInfo=" + model.CustomerId));
                }
            }

            ShowErrorMessages();

            SetMetaInformation(T("Admin.Customers.Edit.Title"));
            SetNgController(NgControllers.NgControllersTypes.CustomerCtrl);

            if (model.IsEditMode)
                Redirect(UrlService.GetAdminUrl("customers#?customerIdInfo=" + model.CustomerId));

            return RedirectToAction("Add");
        }

        //[HttpGet]
        //public JsonResult GetOrders(Guid customerId)
        //{
        //    var model = OrderService.GetCustomerOrderHistory(customerId).Select(x => new
        //    {
        //        OrderId = x.OrderID,
        //        x.OrderNumber,
        //        x.Status,
        //        x.Payed,
        //        x.ArchivedPaymentName,
        //        x.ShippingMethodName,
        //        Sum = PriceFormatService.FormatPrice(x.Sum),
        //        OrderDate = Culture.ConvertDate(x.OrderDate),
        //        x.ManagerId,
        //        x.ManagerName
        //    });
        //    return Json(new { DataItems = model });
        //}


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePassword(Guid customerId, string pass, string pass2)
        {
            if (string.IsNullOrWhiteSpace(pass) || pass != pass2 || pass.Length < 6)
                return Json(new { result = false, error = T("Admin.Customers.Password6Chars") });

            var customer = CustomerService.GetCustomer(customerId);
            if (customer == null)
                return Json(new { result = false, error = T("Admin.Customers.UserIsNotFound") });

            CustomerService.ChangePassword(customerId, pass, false);

            return Json(new { result = true });
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ProcessCustomerContact(SuggestAddressQueryModel address)
        {
            if (address == null || address.City.IsNullOrEmpty())
                return JsonError();

            IpZone zone;
            if ((address.ByCity || address.Region.IsNullOrEmpty()) && (zone = IpZoneService.GetZoneByCity(address.City, null)) != null)
            {
                address.District = zone.District;
                address.Region = zone.Region;
                address.Country = zone.CountryName;
                address.Zip = zone.Zip;
            }
            ModulesExecuter.ProcessAddress(address, true);

            return JsonOk(address);
        }

        #endregion

        #region View

        public ActionResult View(Guid id, string code)
        {
            var model = new GetCustomerView(id, code, true).Execute();
            if (model == null)
                return RedirectToAction("Index");

            SetMetaInformation(T("Admin.Customers.Edit.Title"));
            SetNgController(NgControllers.NgControllersTypes.CustomerViewCtrl);

            return View("~/Areas/Admin/Views/Customers/View.cshtml", model);
        }

        [HttpGet]
        public JsonResult GetView(Guid id, string code)
        {
            var model = new GetCustomerView(id, code, false).Execute();
            return Json(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateAdminComment(Guid id, string comment)
        {
            var customer = CustomerService.GetCustomer(id);
            if (customer == null)
                return JsonError();

            customer.AdminComment = comment;
            CustomerService.UpdateCustomer(customer);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateClientStatus(Guid id, CustomerClientStatus clientStatus)
        {
            var customer = CustomerService.GetCustomer(id);
            if (customer == null)
                return JsonError();

            customer.ClientStatus = clientStatus;
            CustomerService.UpdateCustomer(customer);

            if (clientStatus != CustomerClientStatus.None)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_StatusChanged, clientStatus.ToString());

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeCustomerManager(Guid customerId, int managerId)
        {
            CustomerService.ChangeCustomerManager(customerId, managerId);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DisableDesktopAppNotification(string appName)
        {
            if (string.Equals(appName, "viber", StringComparison.OrdinalIgnoreCase))
            {
                SettingsAdmin.ShowViberDesktopAppNotification = false;
            }
            if (string.Equals(appName, "whatsapp", StringComparison.OrdinalIgnoreCase))
            {
                SettingsAdmin.ShowWhatsAppDesktopAppNotification = false;
            }
            return JsonOk();
        }

        #endregion

        #region Get | Send letter

        public JsonResult GetLetterToCustomer(GetLetterToCustomerModel model)
        {
            if (!string.IsNullOrEmpty(model.CustomerId) && string.IsNullOrEmpty(model.FirstName) &&
                string.IsNullOrEmpty(model.LastName) && string.IsNullOrEmpty(model.Patronymic))
            {
                var customer = CustomerService.GetCustomer(model.CustomerId.TryParseGuid());
                if (customer != null)
                {
                    model.FirstName = customer.FirstName;
                    model.LastName = customer.LastName;
                    model.Patronymic = customer.Patronymic;
                }
            }

            var result = new GetLetterToCustomerResult()
            {
                Error = MailService.ValidateMailSettingsBeforeSending()
            };

            var manager = CustomerContext.CurrentCustomer.IsManager ? ManagerService.GetManager(CustomerContext.CurrentCustomer.Id) : null;
            var managerName = manager != null ? manager.FullName : string.Empty;
            var managerSign = manager != null ? manager.Sign : string.Empty;

            if (model.TemplateId != -1)
            {
                var notFormatted = model.CustomerId == null &&
                                   string.IsNullOrEmpty(model.FirstName) &&
                                   string.IsNullOrEmpty(model.LastName) &&
                                   string.IsNullOrEmpty(model.Patronymic);

                var service = new MailAnswerTemplateService();
                var template = service.GetLetterWithTemplate(model.TemplateId, model.FirstName,
                                                             model.LastName, model.Patronymic, SettingsMain.ShopName, "", "", managerName, managerSign,
                                                             !string.IsNullOrEmpty(model.CustomerId) ? model.CustomerId.TryParseGuid(true) : default(Guid?),
                                                             notFormatted);
                if (template != null)
                {
                    result.Subject = template.Subject;
                    result.Text = template.Body;
                }
            }
            else
            {
                var mail =
                    new SendToCustomerTemplate(model.FirstName ?? "", model.LastName ?? "", model.Patronymic ?? "", "", "", managerName).BuildMail();

                result.Text = mail.Body;

                if (!string.IsNullOrEmpty(model.ReId))
                {
                    var email = CustomerService.GetEmailImap(model.ReId, null);
                    if (email != null)
                    {
                        result.Text +=
                            " <br><br><br> <blockquote style=\"margin:0px 0px 0px 0.8ex;border-left:1px solid rgb(204,204,204);padding-left:1ex\"> " +
                            (email.HtmlBody ?? email.TextBody ?? "").Replace("<html>", "").Replace("<HTML>", "").Replace("<body>", "").Replace("<BODY>", "").Replace("</body>", "").Replace("</BODY>", "").Replace("</HTML>", "") +
                            "</blockquote>";
                    }
                }
            }

            return Json(result);
        }

        public JsonResult GetAnswerTemplates()
        {
            var templates = new List<MailAnswerTemplate>
            {
                new MailAnswerTemplate { TemplateId = -1, Name = T("Admin.Customers.Empty") }
            };

            templates.AddRange(new MailAnswerTemplateService().Gets(true));
            return JsonOk(templates);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendLetterToCustomer(SendLetterToCustomerModel model)
        {
            return ProcessJsonResult(new SendLetterToCustomersHandler(model));
        }

        #endregion

        [HttpGet]
        public JsonResult GetCustomerWithContact(Guid customerId, string code)
        {
            var customer = CustomerService.GetCustomer(customerId);
            if (customer == null && code.IsNotEmpty())
            {
                customer = ClientCodeService.GetCustomerByCode(code, customerId);
                customer.Code = code;
            }

            if (customer == null)
                return Json(null);

            return Json(new
            {
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.Patronymic,
                Email = customer.EMail,
                customer.Phone,
                customer.StandardPhone,
                customer.SubscribedForNews,
                customer.BonusCardNumber,
                customer.CustomerGroup,
                customer.Code,
                customer.RegistredUser,
                customer.Contacts,
                customer.Organization,
                customer.BirthDay
            });
        }

        [HttpGet]
        public JsonResult GetCustomersAutocomplete(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(null);

            var customers = CustomerService.GetCustomersForAutocomplete(q).Select(x => new
            {
                label = string.Format("{0} {1}{2}, {3} {4}", x.LastName, x.FirstName, x.Patronymic.IsNotEmpty() ? " " + x.Patronymic : string.Empty, x.EMail, x.Phone),
                value = x.Id,
                CustomerId = x.Id,
                x.FirstName,
                x.LastName,
                x.Patronymic,
                x.Organization,
                Email = x.EMail,
                x.Phone
            });

            return Json(customers);
        }

        [HttpGet]
        public JsonResult GetCustomerFields()
        {
            return Json(CustomerFieldService.GetCustomerFields());
        }

        [Auth(RoleAction.Customers, RoleAction.Crm, RoleAction.Booking)]
        public ActionResult CustomerFieldsForm(Guid? customerId)
        {
            ViewBag.CustomerFieldModelPrefix = "$ctrl";
            return PartialView("_CustomerFields", CustomerFieldService.GetCustomerFieldsWithValue(customerId ?? Guid.Empty));
        }

        [HttpGet]
        public JsonResult GetCustomerFieldValues(int id)
        {
            return Json(CustomerFieldService.GetCustomerFieldValues(id).Select(x => new { label = x.Value, value = x.Value }));
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetStandartPhone(string phone)
        {
            return JsonOk(StringHelper.ConvertToStandardPhone(phone, true, true));
        }

        #region Tags
        public JsonResult GetAutocompleteTags()
        {
            return Json(new
            {
                tags = TagService.GetAutocompleteTags().Select(x => new { value = x.Name })
            });
        }

        public JsonResult GetTags(Guid customerId)
        {
            return Json(new
            {
                tags = TagService.GetAutocompleteTags().Select(x => new { value = x.Name }),
                selectedTags = TagService.Gets(customerId, onlyEnabled: false).Select(x => new { value = x.Name })
            });
        }
        #endregion
    }
}
