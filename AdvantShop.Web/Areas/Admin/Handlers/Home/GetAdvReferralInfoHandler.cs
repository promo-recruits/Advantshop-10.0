using System;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Models.Home;

namespace AdvantShop.Web.Admin.Handlers.Home
{
    public class GetAdvReferralInfoHandler
    {
        private const string Url = "http://modules.advantshop.net/";

        public AdvReferralModel Execute()
        {
            try
            {
                var referrer = HttpContext.Current.Request.GetUrlReferrer();

                var referralModel = RequestHelper.MakeRequest<AdvReferralModel>(
                    $"{Url}partner/GetReferralData?licKey={SettingsLic.LicKey}&urlReferrer={(referrer != null ? HttpUtility.UrlEncode(referrer.AbsoluteUri) : null)}",
                    method: ERequestMethod.GET);

                if (referralModel.Auth != null && SettingsDesign.CopyrightText.Contains(
                    "<a href=\"https://www.advantshop.net?utm_source=advantshop&utm_medium=site&utm_campaign=footer\" target=\"_blank\">advantshop.net</a>"))
                {
                    SettingsDesign.ShowCopyright = true;
                    SettingsDesign.CopyrightText = SettingsDesign.CopyrightText.Replace(
                        "<a href=\"https://www.advantshop.net?utm_source=advantshop&utm_medium=site&utm_campaign=footer\" target=\"_blank\">advantshop.net</a>",
                        $"<a href=\"{referralModel.ReferralLink}\" target=\"_blank\">advantshop.net</a>");
                }

                return referralModel;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }
    }
}
