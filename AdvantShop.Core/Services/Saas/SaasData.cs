//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using System;

namespace AdvantShop.Saas
{
    public class SaasData
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int ProductsCount { get; set; }
        public int PhotosCount { get; set; }
        /// <summary>
        /// Объем файлового хранилища, ГБ. 0 - неограниченно
        /// </summary>
        public int FileStorageVolume { get; set; }
        //public int ManagersCount { get; set; }
        public bool HaveExcel { get; set; }
        public bool Have1C { get; set; }
        public bool HaveExportFeeds { get; set; }
        public bool ExportFeedsAutoUpdate { get; set; }
        public bool HavePriceRegulating { get; set; }
        public bool HavePriceVisibility { get; set; }
        //public bool HaveBankIntegration { get; set; }

        public bool HaveCrm { get; set; }
        //public bool HaveCrmSalesFunnels { get; set; }
        public int LeadsListsCount { get; set; }
        public bool HaveTelephony { get; set; }
        public bool HaveMobileAdmin { get; set; }
        public bool HaveTags { get; set; }
        public bool HaveCustomerLog { get; set; }

        public bool HaveCustom { get; set; }
        public bool HaveVIPsupport { get; set; }

        public bool MobileVersion { get; set; }
        public bool GoogleYandexMetriks { get; set; }
        public bool LandingPage { get; set; }
        public bool RoleActions { get; set; }
        public bool OrderStatuses { get; set; }
        public bool OrderAdditionFields { get; set; }
        public bool BonusSystem { get; set; }
        public bool DeepAnalytics { get; set; }

        public bool BizProcess { get; set; }

        public bool CustomerAdditionFields { get; set; }
        public int EmployeesCount { get; set; }

        public int CRMIntegrationsCount { get; set; }
        public bool HaveCustomerSegmets { get; set; }

        public string SupportChat { get; set; }

        public bool IsWork { get; set; }

        public string ClientNumber { get; set; }
        public int LeftDay { get; set; }
        public DateTime ValidTill { get; set; }

        public decimal Money { get; set; }
        public decimal Bonus { get; set; }
        public string Error { get; set; }

        public bool IsCorrect
        {
            get { return Error == "fine" || string.IsNullOrEmpty(Error); }
        }

        public string BalanceFormating { get; set; }

        public bool HaveDomains { get; set; }

        public int DomainMaxAmount { get; set; }

        public int LandingFunnelCount { get; set; }
        public int LandingFunnelPageCount { get; set; }

        public bool HaveTriggers { get; set; }
        public int TriggersCount { get; set; }
        public bool HaveLandingFunnel { get; set; }

        public bool HaveBooking { get; set; }
        public bool HaveTasks { get; set; }

        public string LandingPage_AvailableFrom { get; set; }
        public string Triggers_AvailableFrom { get; set; }
        public string ExportFeeds_AvailableFrom { get; set; }
        public string Vk_AvailableFrom { get; set; }
        public string BonusSystem_AvailableFrom { get; set; }
        public string Partners_AvailableFrom { get; set; }
        public string Reseller_AvailableFrom { get; set; }
        public string Instagram_AvailableFrom { get; set; }
        public string Facebook_AvailableFrom { get; set; }
        public string Ok_AvailableFrom { get; set; }
        public string Telegram_AvailableFrom { get; set; }

        public bool Vk { get; set; }
        public bool Reseller { get; set; }
        public bool Instagram { get; set; }
        public bool Facebook { get; set; }
        public bool Ok { get; set; }
        public bool Telegram { get; set; }
        public bool Partners { get; set; }
        public bool HaveMobileApp { get; set; }


        public decimal Balance
        {
            get
            {
                return Money + Bonus;
            }
        }

        public string LeftDayString
        {
            get
            {
                return LeftDay + " " + Strings.Numerals(LeftDay,
                   LocalizationService.GetResource("AdvantShop.Trial.TrialService.LeftDay0"),
                   LocalizationService.GetResource("AdvantShop.TrialTrialService.LeftDay1"),
                   LocalizationService.GetResource("AdvantShop.TrialTrialService.LeftDay2"),
                   LocalizationService.GetResource("AdvantShop.TrialTrialService.LeftDay5"));
            }
        }

        public decimal MoneyService { get; set; }
        public decimal BonusService { get; set; }
        public string BalanceServiceFormating { get; set; }

        public string AllServicesRemainsFormated { get; set; }
        public long EmailsReains { get; set; }
        public long SearchRemains { get; set; }

        public decimal BalanceService
        {
            get
            {
                return MoneyService + BonusService;
            }
        }

        public string CustomCSS { get; set; }


        public string CheckMailingSendTo { get; set; }
        
        public SaasData()
        {
            Name = string.Empty;
            ValidTill = DateTime.Now.AddDays(-7);
            Money = 0;
            Bonus = 0;
            Error = "fine";
        }
    }
}
