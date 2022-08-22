﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Mails.Analytics;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Web.Admin.Models.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Customers
{
    public class SendLetterToCustomersHandler : AbstractCommandHandler
    {
        private readonly SendLetterToCustomerModel _model;
        private List<SendLetterToCustomerItemModel> _customers;
        private readonly MailAnswerTemplateService _service;
        private readonly Manager _currentManager;

        public SendLetterToCustomersHandler(SendLetterToCustomerModel model)
        {
            _model = model;
            _service = new MailAnswerTemplateService();
            _currentManager = ManagerService.GetManager(CustomerContext.CustomerId);
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_model.Subject) || string.IsNullOrWhiteSpace(_model.Text))
                throw new BlException(T("Admin.Customers.EnterTitleAndText"));
        }

        protected override void Handle()
        {
            LoadCustomers();

            if (_customers.Count == 1)
            {
                try
                {
                    SendLetter(_customers[0]);
                }
                catch (Exception ex)
                {
                    Debug.Log.Warn(ex.Message + " " + _customers[0].Email, ex);

                    if (SettingsMail.UseAdvantshopMail)
                        AdminInformerService.Add(new AdminInformer { Body = "Ошибка при отправке писем, детали в логе ошибок", Type = AdminInformerType.Error });
                    throw new BlException(ex.Message);
                }
            }
            else
            {
                Task.Run(() =>
                {
                    Guid? emailingId = null;
                    if (SettingsMail.UseAdvantshopMail)
                    {
                        emailingId = ManualEmailingService.AddManualEmailing(new ManualEmailing
                        {
                            Subject = _model.Subject,
                            TotalCount = _customers.Count
                        });
                    }

                    var exceptions = new ConcurrentQueue<SendLetterException>();

                    Parallel.ForEach(_customers, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (item) =>
                    {
                        try
                        {
                            SendLetter(item, emailingId);
                        }
                        catch (Exception ex)
                        {
                            exceptions.Enqueue(new SendLetterException(item.Email, ex));
                        }
                    });

                    if (exceptions.Any())
                    {
                        Debug.Log.Warn(exceptions.Select(x => x.Email + " " + x.Exception).AggregateString("\r\n<br/>--------<br/>\r\n"));

                        if (SettingsMail.UseAdvantshopMail)
                            AdminInformerService.Add(new AdminInformer { Body = "Ошибка при отправке писем, детали в логе ошибок", Type = AdminInformerType.Error });
                    }
                });
            }

            switch (_model.PageType)
            {
                case "order":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_SendLetterToCustomer);
                    break;
                case "customer":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_SendLetterToCustomer);
                    break;
                case "customerSegment":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_BulkEmailSendingBySegment);
                    break;
                case "leads":
                case "customers":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_BulkEmailSending);
                    break;
            }
        }

        private void SendLetter(SendLetterToCustomerItemModel item, Guid? emailingId = null)
        {
            var managerName = _currentManager != null ? _currentManager.FullName : string.Empty;
            var managerSign = _currentManager != null ? _currentManager.Sign : string.Empty;

            var subject = _service.FormatLetter(_model.Subject,
                item.Customer != null ? item.Customer.FirstName : "",
                item.Customer != null ? item.Customer.LastName : "",
                item.Customer != null ? item.Customer.Patronymic : "",
                SettingsMain.ShopName, "", "", managerName, managerSign,
                item.CustomerId);

            var text = _service.FormatLetter(_model.Text,
                item.Customer != null ? item.Customer.FirstName : "",
                item.Customer != null ? item.Customer.LastName : "",
                item.Customer != null ? item.Customer.Patronymic : "",
                SettingsMain.ShopName, "", "", managerName, managerSign,
                item.CustomerId);

            MailService.SendMail(item.CustomerId ?? Guid.Empty, item.Email, subject, text, true, emailingId, null, needretry: false, lettercount: _customers.Count);
        }

        private void LoadCustomers()
        {
            _customers = new List<SendLetterToCustomerItemModel>();

            if (!string.IsNullOrWhiteSpace(_model.Email))
            {
                var c = CustomerService.GetCustomerByEmail(_model.Email);
                _customers.Add(c != null
                    ? new SendLetterToCustomerItemModel(c)
                    : new SendLetterToCustomerItemModel {Email = _model.Email, CustomerId = _model.CustomerId});
            }
            else if (_model.CustomerId.HasValue)
            {
                var c = CustomerService.GetCustomer(_model.CustomerId.Value);
                if (c != null)
                    _customers.Add(new SendLetterToCustomerItemModel(c));
            }
            else if (_model.CustomerIds != null && _model.CustomerIds.Any())
            {
                foreach (var customerId in _model.CustomerIds)
                {
                    var c = CustomerService.GetCustomer(customerId);
                    if (c != null)
                        _customers.Add(new SendLetterToCustomerItemModel(c));
                }
            }
            else if (_model.SubscriptionIds != null && _model.SubscriptionIds.Any())
            {
                foreach (var id in _model.SubscriptionIds)
                {
                    var s = SubscriptionService.GetSubscriptionExt(id);
                    if (s != null)
                        _customers.Add(new SendLetterToCustomerItemModel(s));
                }
            }

            _customers =
                _customers.Where(x => ValidationHelper.IsValidEmail(x.Email))
                    .Distinct(new SendLetterToCustomerComparer())
                    .ToList();
        }

        private class SendLetterException
        {
            public string Email { get; set; }
            public Exception Exception { get; set; }

            public SendLetterException()
            {
            }

            public SendLetterException(string email, Exception exception)
            {
                Email = email;
                Exception = exception;
            }
        }

        private class SendLetterToCustomerComparer : IEqualityComparer<SendLetterToCustomerItemModel>
        {
            public bool Equals(SendLetterToCustomerItemModel x, SendLetterToCustomerItemModel y)
            {
                return x.Email == y.Email;
            }

            public int GetHashCode(SendLetterToCustomerItemModel obj)
            {
                return obj.Email.GetHashCode();
            }
        }
    }
}
