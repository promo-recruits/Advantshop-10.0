using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class CustomersSettingsModel
    {
        public string ApplicationId { get; set; }

        public ImportCustomersModel ImportCustomersModel { get; set; }
    }
}