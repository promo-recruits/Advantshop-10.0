﻿using System;
using System.IO;
using System.Net;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Diagnostics;
using AdvantShop.Saas;
using AdvantShop.Trial;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.DownloadableContent
{
    public class ShopActionsService
    {
        private const string ShopActionsUrl = "http://modules.advantshop.net/actionblocks/";
        private const string CachePrefix = "AdvantshopAdminAction_";

        public static string GetLast()
        {
            return CacheManager.Get(CachePrefix + SettingsLic.ClientCode + SettingsLic.LicKey, 10, () =>
            {
                if (!SaasDataService.IsSaasEnabled && !TrialService.IsTrialEnabled)
                    return "";

                string action = null;

                try
                {
                    var request = WebRequest.Create(ShopActionsUrl + "last?id=" + SettingsLic.LicKey);
                    request.Method = "GET";
                    request.Timeout = 2000;

                    using (var stream = request.GetResponse().GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            var response = reader.ReadToEnd();

                            if (!string.IsNullOrEmpty(response))
                                action = JsonConvert.DeserializeObject<string>(response);
                        }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }

                return action;
            });
        }

        public static bool HideAction(int actionId)
        {
            string result = null;
            try
            {
                var request = WebRequest.Create(ShopActionsUrl + "close?id=" + SettingsLic.LicKey + "&actionId=" + actionId);
                request.Method = "POST";
                request.Timeout = 2000;

                using (var stream = request.GetResponse().GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            CacheManager.RemoveByPattern(CachePrefix);

            return result != null;
        }
    }
}
