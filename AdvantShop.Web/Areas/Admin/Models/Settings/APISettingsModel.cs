using System.Collections.Generic;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class APISettingsModel
    {
        public string Key { get; set; }
        public bool IsRus { get; set; }
        public bool _1CEnabled { get; set; }
        public bool _1CDisableProductsDecremention { get; set; }
        public bool _1CUpdateStatuses { get; set; }
        public bool ExportOrdersType { get; set; }
        public List<SelectListItem> ExportOrders { get; set; }
        public bool _1CUpdateProducts { get; set; }
        public List<SelectListItem> UpdateProducts { get; set; }
        public bool _1CSendProducts { get; set; }
        public List<SelectListItem> SendProducts { get; set; }
        public string ImportPhotosUrl { get; set; }
        public string ImportProductsUrl { get; set; }
        public string ExportProductsUrl { get; set; }
        public string DeletedProducts { get; set; }
        public string ExportOrdersUrl { get; set; }
        public string ChangeOrderStatusUrl { get; set; }
        public string DeletedOrdersUrl { get; set; }

        public string LeadAddUrl { get; set; }
        public string VkUrl { get; set; }

        public bool ShowOneC { get; set; }

        public string OrderAddUrl { get; set; }
        public string OrderGetUrl { get; set; }
        public string OrderGetListUrl { get; set; }
        public string OrderChangeStatusUrl { get; set; }
        public string OrderSetPaidUrl { get; set; }

        public string OrderStatusGetListUrl { get; set; }

        public string WebhookAddOrderUrl { get; set; }
        public string WebhookChangeOrderStatusUrl { get; set; }
        public string WebhookUpdateOrderUrl { get; set; }
        public string WebhookDeleteOrderUrl { get; set; }
        
        public string WebhookAddLeadUrl { get; set; }
        public string WebhookChangeLeadStatusUrl { get; set; }
        public string WebhookUpdateLeadUrl { get; set; }
        public string WebhookDeleteLeadUrl { get; set; }


        public string CustomerGetUrl { get; set; }
        public string CustomerAddUrl { get; set; }
        public string CustomerUpdateUrl { get; set; }
        public string CustomerSmsPhoneConfirmationUrl { get; set; }
        public string ManagersUrl { get; set; }
        public string CustomerGroupsUrl { get; set; }
        public string CustomerBonusesUrl { get; set; }
        public string CustomerListUrl { get; set; }


        public string BonusesGetUrl { get; set; }
        public string BonusesAddUrl { get; set; }
        public string BonusesMainBonusesAcceptUrl { get; set; }
        public string BonusesMainBonusesSubstractUrl { get; set; }
        public string BonusesAdditionalBonusesUrl { get; set; }
        public string BonusesAdditionalBonusesAcceptUrl { get; set; }
        public string BonusesAdditionalBonusesSubstractUrl { get; set; }
        public string BonusesSettingsUrl { get; set; }
        public string BonusesTransactionsUrl { get; set; }

        public string GradesUrl { get; set; }
    }
}