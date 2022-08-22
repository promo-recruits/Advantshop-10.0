﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.CustomerSegments;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Saas;

namespace AdvantShop.Web.Admin.Models.Customers
{
    public class CustomerViewModel
    {
        public Customer Customer { get; set; }
        public Manager Manager { get { return Customer != null ? Customer.Manager : null; } }

        public int? ManagerId { get; set; }

        public List<SelectListItem> ManagerList { get; set; }

        public string OrdersSum { get; set; }
        public int OrdersCount { get; set; }
        public int AllOrdersCount { get; set; }
        public int LeadsCount { get; set; }
        public int BookingsCount { get; set; }
        public string AverageCheck { get; set; }

        public VkUser VkUser { get; set; }
        public InstagramUser InstagramUser { get; set; }
        public FacebookUser FacebookUser { get; set; }
        public TelegramUser TelegramUser { get; set; }
        public OkUser OkUser { get; set; }
        public List<CustomerSegment> CustomerSegments { get; set; }

        public PartnerInfoModel PartnerInfo { get; set; }

        public Card BonusCard { get; set; }
        public List<CustomerFieldWithValue> CustomerFields { get; set; }

        public string DurationDate { get; set; }
        
        public List<ISmsService> SmsModules { get; private set; }

        public bool ShowVk { get; private set; }
        public bool ShowFacebook { get; private set; }
        public bool ShowInstagram { get; private set; }
        public bool ShowTelegram { get; private set; }
        public bool ShowOk { get; private set; }
        public List<string> CustomerInterestingCategories { get; set; }

        public ShoppingCart ShoppingCart { get; set; }
        public bool ShowActivity { get; private set; }
        public bool ShowCrm { get; private set; }
        public bool ShowBooking { get; private set; }
        public bool ShowTasks { get; private set; }
        public bool ShowBonuses { get; private set; }
        public bool ShowPartners { get; private set; }

        public double Elapsed { get; set; }


        public CustomerViewModel()
        {
            ShowVk = VkApiService.IsVkActive();
            ShowFacebook = new FacebookApiService().IsActive();
            ShowInstagram = Instagram.Instance.IsActive();
            ShowTelegram = new TelegramApiService().IsActive();
            ShowOk = OkApiService.IsActive();
            SmsModules = SmsNotifier.GetAllSmsModules();

            ShowActivity = !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCustomerLog;
            ShowBooking = SettingsMain.BookingActive;
            ShowCrm = Core.Services.Configuration.Settings.SettingsCrm.CrmActive &&
                      (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCrm);
            ShowTasks = SettingsTasks.TasksActive;
            ShowBonuses = SettingsMain.BonusAppActive;
            ShowPartners = SettingsMain.PartnersActive;
        }
        public bool ShowViberDesktopAppNotification
        {
            get { return SettingsAdmin.ShowViberDesktopAppNotification; }
        }
        public bool ShowWhatsAppDesktopAppNotification
        {
            get { return SettingsAdmin.ShowWhatsAppDesktopAppNotification; }
        }
    }

    public class PartnerInfoModel
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public bool PartnerEnabled { get; set; }
        public DateTime DateBinded { get; set; }
        public string DateBindedFormatted
        {
            get { return Culture.ConvertDate(DateBinded); }
        }
    }

}
