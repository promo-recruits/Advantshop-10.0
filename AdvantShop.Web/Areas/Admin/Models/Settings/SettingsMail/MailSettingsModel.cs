using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Models.Settings.SettingsMail
{
    public class MailSettingsModel
    {
        public string EmailForOrders { get; set; }
        public string EmailForProductDiscuss { get; set; }
        public string EmailForRegReport { get; set; }
        public string EmailForFeedback { get; set; }
        public string EmailForLeads { get; set; }
        public string EmailForBookings { get; set; }
        public string EmailForPartners { get; set; }
        public string EmailForMissedCall { get; set; }

        public string SMTP { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Пароль с которым сравнивается Password, чтобы узнать что он изменился
        /// </summary>
        public string PasswordCompare { get; set; }

        [EmailAddress]
        public string From { get; set; }
        public string SenderName { get; set; }
        public bool SSL { get; set; }
        public int? Port { get; set; }


        public string ImapHost { get; set; }
        public int? ImapPort { get; set; }

        public bool UseAdvantshopMail { get; set; }

        [EmailAddress]        
        public string FromEmail { get; set; }
        
        public string FromName { get; set; }
        public DateTime? ConfirmDateEmail { get; set; }
        public string HtmlMessage { get; set; }


        public List<SelectListItem> SmsModules { get; set; }
        public string ActiveSmsModule { get; set; }
        public string AdminPhone { get; set; }
        //public bool SendSmsOnOrderPhone { get; set; }
        public bool SendSmsToCustomerOnNewOrder { get; set; }
        public bool SendSmsToAdminOnNewOrder { get; set; }
        public string SmsTextOnNewOrder { get; set; }
        public bool SendSmsToCustomerOnOrderStatusChanging { get; set; }
        public bool SendSmsToAdminOnOrderStatusChanging { get; set; }

        public bool SendSmsToAdminOnNewLead { get; set; }
        public string SmsTextOnNewLead { get; set; }
    }
}


