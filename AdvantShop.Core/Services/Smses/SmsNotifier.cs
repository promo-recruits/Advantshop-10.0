using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Loging;
using AdvantShop.Core.Services.Loging.Smses;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Task = System.Threading.Tasks.Task;

namespace AdvantShop.Core.Services.Smses
{
    public class SmsNotifier
    {
        private static ISmsLoger Loger { get { return LogingManager.GetSmsLoger(); } }
        private static readonly Regex PhoneValid = new Regex("^([0-9]{10,15})$", RegexOptions.Compiled | RegexOptions.Singleline);


        public static void SendSms(long phone, string text, Guid? customerId = null, Customer customer = null)
        {
            var preparedText = PrepareSmsText(text, customer);

            Task.Run(() => SendSmsNow(phone, preparedText, customerId));
        }

        public static void SendSms(long phone, string text, Guid? customerId, Customer customer, Order order)
        {
            var preparedText = PrepareSmsText(text, customer);
            preparedText = PrepareSmsText(preparedText, order);

            Task.Run(() => SendSmsNow(phone, preparedText, customerId));
        }

        public static void SendSms(long phone, string text, Guid? customerId, Customer customer, Lead lead)
        {
            var preparedText = PrepareSmsText(text, customer);
            preparedText = PrepareSmsText(preparedText, lead);

            Task.Run(() => SendSmsNow(phone, preparedText, customerId));
        }

        public static string SendSmsNowWithResult(long phone, string text, Guid? customerId = null, bool? throwException = false)
        {
            return SendSmsNow(phone, text, customerId, throwException);
        }

        public static void SendTestSms(long phone, string text)
        {
            SendSmsNow(phone, text, throwException: true);
        }

        public static void SendSmsOnLeadAdded(Lead lead)
        {
            var smsText = SettingsSms.SmsTextOnNewLead;
            
            if (SettingsSms.SendSmsToAdminOnNewLead)
            {
                var phones = (SettingsSms.AdminPhone ?? "").Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                var customerId = HttpContext.Current != null ? CustomerContext.CustomerId : default(Guid?);

                foreach (var phone in phones)
                {
                    SendSms(Convert.ToInt64(phone.Trim()), smsText, customerId, lead.Customer, lead);
                }
            }
        }

        public static void SendSmsOnOrderAdded(Order order)
        {
            var smsText = SettingsSms.SmsTextOnNewOrder;

            if (SettingsSms.SendSmsToCustomerOnNewOrder)
            {
                var customer = order.OrderCustomer;
                if (customer != null && customer.StandardPhone.HasValue)
                {
                    SendSms(customer.StandardPhone.Value, smsText, customer.CustomerID, (Customer)customer, order);
                }
            }
            
            if (SettingsSms.SendSmsToAdminOnNewOrder)
            {
                var phones = (SettingsSms.AdminPhone ?? "").Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                var customerId = HttpContext.Current != null ? CustomerContext.CustomerId : default(Guid?);

                foreach (var phone in phones)
                {
                    SendSms(Convert.ToInt64(phone.Trim()), smsText, customerId, (Customer)order.OrderCustomer, order);
                }
            }
        }

        public static void SendSmsOnOrderStatusChanging(Order order)
        {
            if (order == null || order.IsDraft || order.OrderStatus == null || order.OrderStatus.IsDefault)
                return;
            
            var smsTemplate = SmsOnOrderChangingService.GetByOrderStatusId(order.OrderStatus.StatusID);
            if (smsTemplate == null || !smsTemplate.Enabled)
                return;

            var smsText = smsTemplate.SmsText;

            if (SettingsSms.SendSmsToCustomerOnOrderStatusChanging)
            {
                var customer = order.OrderCustomer;
                if (customer != null && customer.StandardPhone.HasValue)
                {
                    SendSms(customer.StandardPhone.Value, smsText, customer.CustomerID, (Customer)customer, order);
                }
            }

            if (SettingsSms.SendSmsToAdminOnOrderStatusChanging)
            {
                var phones = (SettingsSms.AdminPhone ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var customerId = HttpContext.Current != null ? CustomerContext.CustomerId : default(Guid?);

                foreach (var phone in phones)
                {
                    SendSms(Convert.ToInt64(phone.Trim()), smsText, customerId, (Customer)order.OrderCustomer, order);
                }
            }
        }

        public static List<ISmsService> GetAllSmsModules()
        {
            var list = new List<ISmsService>();

            foreach (var moduleType in AttachedModules.GetModules<ISmsService>())
            {
                var classInstance = (ISmsService)Activator.CreateInstance(moduleType, null);
                list.Add(classInstance);
            }

            return list;
        }

        public static ISmsService GetActiveSmsModule()
        {
            if (SettingsSms.ActiveSmsModule == "-1")
                return null;

            var list = new List<ISmsService>();

            foreach (var moduleType in AttachedModules.GetModules<ISmsService>())
            {
                var module = (ISmsService)Activator.CreateInstance(moduleType, null);

                if (module.ModuleStringId == SettingsSms.ActiveSmsModule)
                    return module;

                list.Add(module);
            }

            return list.Count > 0 ? list[0] : null;
        }

        public static bool HasSmsTemplateOnOrderStatus(int orderStatusId)
        {
            var module = GetActiveSmsModule();
            if (module == null)
                return false;

            var smsTemplate = SmsOnOrderChangingService.GetByOrderStatusId(orderStatusId);
            
            return smsTemplate != null && smsTemplate.Enabled && !string.IsNullOrEmpty(smsTemplate.SmsText);
        }

        private static string SendSmsNow(long phone, string text, Guid? customerId = null, bool? throwException = false)
        {
            string result = null;
            var throwError = throwException != null && throwException.Value;

            if (string.IsNullOrWhiteSpace(text) || phone == 0 || !IsValidPhone(phone))
            {
                if (throwError)
                    throw new BlException("Укажите валидный телефон и текст");
                return result;
            }

            var status = SmsStatus.Error;
            try
            {
                var module = GetActiveSmsModule();
                if (module == null)
                {
                    if (throwError)
                        throw new BlException("Не подключен модуль sms");
                    return result;
                }

                result = module.SendSms(phone, text);

                status = SmsStatus.Sent;
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    Debug.Log.Error(error);

                                    if (throwError) throw;
                                }
                    }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                if (throwError) throw;
            }

            Loger.LogSms(new TextMessage
            {
                CreateOn = DateTime.Now,
                CustomerId = customerId ?? Guid.Empty,
                Body = text,
                Status = status,
                Phone = phone
            });

            return result;
        }

        private static bool IsValidPhone(long phone)
        {
            return PhoneValid.IsMatch(phone.ToString());
        }

        public static string PrepareSmsText(string text, Customer customer = null)
        {
            if (text == null)
                return string.Empty;

            if (text.Contains("#FIRST_NAME#") || text.Contains("#LAST_NAME#") || text.Contains("#FULL_NAME#"))
            {
                return
                    text.Replace("#FIRST_NAME#", customer != null ? customer.FirstName : "")
                        .Replace("#LAST_NAME#", customer != null ? customer.LastName : "")
                        .Replace("#FULL_NAME#", customer != null ? StringHelper.AggregateStrings(" ", customer.FirstName, customer.LastName) : "");
            }

            return text;
        }

        public static string PrepareSmsText(string text, Order order = null)
        {
            if (text == null)
                return string.Empty;

            if (order != null)
            {
                text = 
                    text.Replace("#ORDER_NUMBER#", order.Number)
                        .Replace("#ORDER_SUM#", PriceFormatService.FormatPrice(order.Sum, order.OrderCurrency))
                        .Replace("#ORDER_STATUS#", order.OrderStatus.StatusName)
                        .Replace("#STATUS_COMMENT#", order.StatusComment)
                        .Replace("#TRACKNUMBER#", order.TrackNumber)
                        .Replace("#SHIPPING_NAME#", order.ArchivedShippingName)
                        .Replace("#PICKPOINT_ADDRESS#", order.OrderPickPoint != null ? order.OrderPickPoint.PickPointAddress : string.Empty)
                        .Replace("#PAYMENT_NAME#", order.ArchivedPaymentName)
                        .Replace("#STORE_NAME#", SettingsMain.ShopName)
                        .Replace("#PAY_STATUS#", LocalizationService.GetResource(order.Payed ? "Core.Orders.Order.PaySpend" : "Core.Orders.Order.PayCancel").ToLower());

                if (text.Contains("#BILLING_SHORTLINK#"))
                {
                    if (order.PayCode.IsNullOrEmpty())
                        order.PayCode = OrderService.GeneratePayCode(order.OrderID);
                    text = text.Replace("#BILLING_SHORTLINK#", SettingsMain.SiteUrl.Trim('/') + "/pay/" + order.PayCode);
                }
            }

            return text;
        }

        public static string PrepareSmsText(string text, Lead lead = null)
        {
            if (text == null)
                return string.Empty;

            if (lead != null)
            {
                return
                    text.Replace("#TITLE#", lead.Title)
                        .Replace("#LEAD_SUM#", PriceFormatService.FormatPrice(lead.Sum, lead.LeadCurrency))
                        .Replace("#DEAL_STATUS#", lead.DealStatus.Name)
                        .Replace("#SALES_FUNNEL#", SalesFunnelService.Get(lead.SalesFunnelId).Name)
                        .Replace("#SHIPPING_NAME#", lead.ShippingName)
                        .Replace("#PICKPOINT_ADDRESS#", lead.ShippingPickPoint ?? string.Empty)
                        .Replace("#STORE_NAME#", SettingsMain.ShopName);
            }

            return text;
        }
    }
}
