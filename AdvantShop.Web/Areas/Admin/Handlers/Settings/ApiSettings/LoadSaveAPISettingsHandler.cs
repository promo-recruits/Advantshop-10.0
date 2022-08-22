using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Localization;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.ApiSettings
{
    public class LoadSaveApiSettingsHandler
    {
        private readonly APISettingsModel _model;
        private readonly bool _isRu = Culture.Language == Culture.SupportLanguage.Russian ||
                              Culture.Language == Culture.SupportLanguage.Ukrainian;

        public LoadSaveApiSettingsHandler(APISettingsModel model)
        {
            _model = model;
        }

        public APISettingsModel Load()
        {
            var model = new APISettingsModel();

            var siteUrl = SettingsMain.SiteUrl.TrimEnd('/');
            var key = SettingsApi.ApiKey;

            model.Key = key;
            model.IsRus = _isRu;
            model._1CEnabled = Settings1C.Enabled;
            model._1CDisableProductsDecremention = Settings1C.DisableProductsDecremention;
            model._1CUpdateStatuses = Settings1C.UpdateStatuses;

            model.ExportOrdersType = Settings1C.OnlyUseIn1COrders;
            model.ExportOrders = new List<SelectListItem>();
            model.ExportOrders.Add(new SelectListItem() { Text = LocalizationService.GetResource("Admin.Settings.API.ExportOrdersType.SelectValue0"), Value = "True", Selected = model.ExportOrdersType });
            model.ExportOrders.Add(new SelectListItem() { Text = LocalizationService.GetResource("Admin.Settings.API.ExportOrdersType.SelectValue1"), Value = "False", Selected = model.ExportOrdersType });

            model._1CUpdateProducts = Settings1C.UpdateProducts;
            model.UpdateProducts = new List<SelectListItem>();
            model.UpdateProducts.Add(new SelectListItem() { Text = LocalizationService.GetResource("Admin.Settings.API.1CUpdateProducts.SelectValue0"), Value = "True", Selected = model._1CUpdateProducts });
            model.UpdateProducts.Add(new SelectListItem() { Text = LocalizationService.GetResource("Admin.Settings.API.1CUpdateProducts.SelectValue1"), Value = "False", Selected = model._1CUpdateProducts });

            model._1CSendProducts = Settings1C.SendAllProducts;
            model.SendProducts = new List<SelectListItem>();
            model.SendProducts.Add(new SelectListItem() { Text = LocalizationService.GetResource("Admin.Settings.API.1CSendProducts.SelectValue0"), Value = "True", Selected = model._1CSendProducts });
            model.SendProducts.Add(new SelectListItem() { Text = LocalizationService.GetResource("Admin.Settings.API.1CSendProducts.SelectValue1"), Value = "False", Selected = model._1CSendProducts });

            model.ImportPhotosUrl = siteUrl + "/api/1c/importphotos?apikey=" + key;
            model.ImportProductsUrl = siteUrl + "/api/1c/importproducts?apikey=" + key;
            model.ExportProductsUrl = siteUrl + "/api/1c/exportproducts?apikey=" + key;
            model.DeletedProducts = siteUrl + "/api/1c/deletedproducts?apikey=" + key;
            model.ExportOrdersUrl = siteUrl + "/api/1c/exportorders?apikey=" + key;
            model.ChangeOrderStatusUrl = siteUrl + "/api/1c/changeorderstatus?apikey=" + key;
            model.DeletedOrdersUrl = siteUrl + "/api/1c/deletedorders?apikey=" + key;

            model.LeadAddUrl = siteUrl + "/api/leads/add?apikey=" + key;
            model.VkUrl = siteUrl + "/api/vk?apikey=" + key;

            model.OrderAddUrl = siteUrl+ "/api/order/add?apikey=" + key;
            model.OrderGetUrl = siteUrl+ "/api/order/get/{id}?apikey=" + key;
            model.OrderGetListUrl = siteUrl+ "/api/order/getlist?apikey=" + key;
            model.OrderChangeStatusUrl = siteUrl+ "/api/order/changestatus?apikey=" + key;
            model.OrderSetPaidUrl = siteUrl+ "/api/order/setpaid?apikey=" + key;

            model.OrderStatusGetListUrl = siteUrl+ "/api/orderstatus/getlist?apikey=" + key;

            model.ShowOneC = !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.Have1C;

            model.CustomerGetUrl = siteUrl + "/api/customers/{id}?apikey=" + key;
            model.CustomerAddUrl = siteUrl + "/api/customers/add?apikey=" + key;
            model.CustomerUpdateUrl = siteUrl + "/api/customers/{id}?apikey=" + key;
            model.CustomerSmsPhoneConfirmationUrl = siteUrl + "/api/customers/smsphoneconfirmation?apikey=" + key;
            model.ManagersUrl = siteUrl + "/api/managers?apikey=" + key;
            model.CustomerGroupsUrl = siteUrl + "/api/customergroups?apikey=" + key;
            model.CustomerBonusesUrl = siteUrl + "/api/customers/{id}/bonuses?apikey=" + key;
            model.CustomerListUrl = siteUrl + "/api/customers?apikey=" + key;


            model.BonusesGetUrl = siteUrl + "/api/bonus-cards/{id}?apikey=" + key;
            model.BonusesAddUrl = siteUrl + "/api/bonus-cards/add?apikey=" + key;

            model.BonusesMainBonusesAcceptUrl = siteUrl + "/api/bonus-cards/{id}/main-bonuses/accept?apikey=" + key;
            model.BonusesMainBonusesSubstractUrl = siteUrl + "/api/bonus-cards/{id}/main-bonuses/substract?apikey=" + key;

            model.BonusesAdditionalBonusesUrl = siteUrl + "/api/bonus-cards/{id}/additional-bonuses?apikey=" + key;
            model.BonusesAdditionalBonusesAcceptUrl = siteUrl + "/api/bonus-cards/{id}/additional-bonuses/accept?apikey=" + key;
            model.BonusesAdditionalBonusesSubstractUrl = siteUrl + "/api/bonus-cards/{id}/additional-bonuses/substract?apikey=" + key;

            model.BonusesSettingsUrl = siteUrl + "/api/bonus-cards/settings?apikey=" + key;
            model.BonusesTransactionsUrl = siteUrl + "/api/bonus-cards/{id}/transactions?apikey=" + key;

            model.GradesUrl = siteUrl + "/api/bonus-grades?apikey=" + key;

            return model;
        }

        public void Save()
        {
            SettingsApi.ApiKey = _model.Key;
            Settings1C.Enabled = _model._1CEnabled;
            Settings1C.DisableProductsDecremention = _model._1CDisableProductsDecremention;
            Settings1C.UpdateStatuses = _model._1CUpdateStatuses;
            Settings1C.OnlyUseIn1COrders = _model.ExportOrdersType;
            Settings1C.UpdateProducts = _model._1CUpdateProducts;
            Settings1C.SendAllProducts = _model._1CSendProducts;
        }
    }
}
