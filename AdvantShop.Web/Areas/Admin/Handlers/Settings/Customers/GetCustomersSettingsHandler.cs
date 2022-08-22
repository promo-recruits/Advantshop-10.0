using AdvantShop.Configuration;
using AdvantShop.Web.Admin.Handlers.Catalog.Import;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Customers
{
    public class GetCustomersSettingsHandler
    {
        public CustomersSettingsModel Execute()
        {
            return new CustomersSettingsModel
            {
                ApplicationId = SettingsVk.ApplicationId,
                ImportCustomersModel = new GetImportCustomersModel().Execute()
            };
        }
    }
}
