using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Payment.Alfabank;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Shared.Smses;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    public partial class SmsController : BaseAdminController
    {
        public JsonResult GetSmsModuleEnabled()
        {
            var smsModule = SmsNotifier.GetActiveSmsModule();

            return Json(new {result = smsModule != null});
        }

        public JsonResult SendSms(SendSmsModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
                return JsonError("");

            var smsModule = SmsNotifier.GetActiveSmsModule();

            if (smsModule == null)
                return JsonError("Нет активного sms модуля");

            var customers = new List<SendSmsModelItem>();

            if (model.CustomerId.HasValue)
            {
                var c = CustomerService.GetCustomer(model.CustomerId.Value);
                customers.Add(new SendSmsModelItem
                {
                    Phone = StringHelper.ConvertToStandardPhone(model.Phone, true, true) ?? (c != null ? c.StandardPhone : null),
                    Customer = c
                });
            }
            else if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                customers.Add(new SendSmsModelItem
                {
                    Phone = StringHelper.ConvertToStandardPhone(model.Phone, true, true)
                });
            }
            else if (model.CustomerIds != null)
            {
                foreach (var customerId in model.CustomerIds)
                {
                    var c = CustomerService.GetCustomer(customerId);
                    if (c != null)
                        customers.Add(new SendSmsModelItem(c));
                }
            }
            else if (model.SubscriptionIds != null)
            {
                foreach (var id in model.SubscriptionIds)
                {
                    var s = SubscriptionService.GetSubscriptionExt(id);
                    if (s != null)
                        customers.Add(new SendSmsModelItem(s));
                }
            }

            var order = model.OrderId != null ? OrderService.GetOrder(model.OrderId.Value) : null;
            var lead = model.LeadId != null ? LeadService.GetLead(model.LeadId.Value) : null;

            var customersList = customers.Where(x => x.Phone != null).Distinct(new SendSmsModelItemComparer());

            foreach (var item in customersList)
            {
                if (lead == null)
                {
                    SmsNotifier.SendSms(item.Phone.Value, model.Text,
                        item.Customer != null ? item.Customer.Id : Guid.Empty,
                        item.Customer,
                        order);
                }
                else
                {
                    SmsNotifier.SendSms(item.Phone.Value, model.Text,
                        item.Customer != null ? item.Customer.Id : Guid.Empty,
                        item.Customer,
                        lead);
                }
            }

            switch (model.PageType)
            {
                case "order":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_SendSmsToCustomer);
                    break;
                case "customer":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_SendSmsToCustomer);
                    break;
                case "customerSegment":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_BulkSmsSendingBySegment);
                    break;
                case "leads":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_BulkSmsSending);
                    break;
            }

            return JsonOk();
        }

        public JsonResult GetSmsToCustomer(GetSmsToCustomerModel model)
        {
            if (model.TemplateId != -1)
            {
                var template = new SmsAnswerTemplateService().Get(model.TemplateId, false);

                if (template != null)
                {
                    var text = template.Text;
                    if (model.CustomerId.HasValue)
                        text = SmsNotifier.PrepareSmsText(text, CustomerService.GetCustomer(model.CustomerId.Value));
                    if (model.OrderId.HasValue)
                        text = SmsNotifier.PrepareSmsText(text, OrderService.GetOrder(model.OrderId.Value));
                    if (model.LeadId.HasValue)
                        text = SmsNotifier.PrepareSmsText(text, LeadService.GetLead(model.LeadId.Value));

                    return Json(new { text = text });
                }
            }

            return Json(new {text = string.Empty});
        }

        public JsonResult GetAnswerTemplates()
        {
            var templates = new List<SmsAnswerTemplate>
            {
                new SmsAnswerTemplate { TemplateId = -1, Name = T("Admin.Customers.Empty") }
            };

            templates.AddRange(new SmsAnswerTemplateService().Gets(true));
            return JsonOk(templates);
        }

        private string PrepareSmsText(string text, Customer customer, int? orderId)
        {
            if (text == null)
                text = "";

            if (orderId.HasValue)
            {
                var order = OrderService.GetOrder(orderId.Value);
                if (order != null)
                {
                    text =
                        text.Replace("#ORDERNUMBER#", order.Number)
                            .Replace("#ORDER_SUM#", order.Sum.ToString())
                            .Replace("#ORDER_STATUS#", order.OrderStatus.StatusName)
                            .Replace("#TRACKNUMBER#", order.TrackNumber)
                            .Replace("#STORE_NAME#", SettingsMain.ShopName)
                            .Replace("#PAY_STATUS#",  
                                order.Payed
                                    ? LocalizationService.GetResource("Core.Orders.Order.PaySpend").ToLower()
                                    : LocalizationService.GetResource("Core.Orders.Order.PayCancel").ToLower());

                    if (order.OrderCustomer != null)
                    {
                        text = text.Replace("#FIRST_NAME#", order.OrderCustomer.FirstName)
                            .Replace("#LAST_NAME#", order.OrderCustomer.LastName)
                            .Replace("#FULL_NAME#", StringHelper.AggregateStrings(" ", order.OrderCustomer.FirstName, order.OrderCustomer.LastName));
                    }
                }
            }

            if (text.Contains("#FIRST_NAME#") || text.Contains("#LAST_NAME#") || text.Contains("#FULL_NAME#"))
            {
                text =
                    text.Replace("#FIRST_NAME#", customer != null ? customer.FirstName : "")
                        .Replace("#LAST_NAME#", customer != null ? customer.LastName : "")
                        .Replace("#FULL_NAME#", customer != null ? StringHelper.AggregateStrings(" ", customer.FirstName, customer.LastName) : "");
            }

            return text;
        }
    }
}

