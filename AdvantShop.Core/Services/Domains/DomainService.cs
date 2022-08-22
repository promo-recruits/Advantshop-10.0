using AdvantShop.Configuration;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Customers;
using AdvantShop.Saas;
using AdvantShop.Trial;

namespace AdvantShop.Core.Services.Domains
{
    public interface IDomainService
    {
        void Add(string domain);
        void Remove(string domain);
    }

    public class DomainService : IDomainService
    {
        private const string Host = "http://modules.advantshop.net/";

        public void Add(string domain)
        {
            if (!SaasDataService.IsSaasEnabled && !TrialService.IsTrialEnabled)
                return;

            var model = new DomainShopDto
            {
                LicKey = SettingsLic.LicKey,
                Domain = domain,
            };

            var result = RequestHelper.MakeRequest<string>(Host + "domain/add", model);

            if (result != null)
                result = result.Trim('"');

            if (result != "ok")
                throw new BlException(result);
        }

        public void Remove(string domain)
        {
            if (!SaasDataService.IsSaasEnabled && !TrialService.IsTrialEnabled)
                return;

            var model = new DomainShopDto
            {
                LicKey = SettingsLic.LicKey,
                Domain = domain,
            };

            var result = RequestHelper.MakeRequest<string>(Host + "domain/remove", model);

            if (result != null)
                result = result.Trim('"');

            if (result != "ok")
                throw new BlException(result);
        }

        public static bool IsAvalable()
        {
            return (SaasDataService.IsSaasEnabled || TrialService.IsTrialEnabled || Demo.IsDemoEnabled) && CustomerContext.CurrentCustomer.IsAdmin;
        }

        public class DomainShopDto
        {
            public string LicKey { get; set; }
            public string Domain { get; set; }
        }
    }
}
