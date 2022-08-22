using System.Collections.Generic;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Settings
{
    public class SettingsResponse : List<SettingItemResponse>, IApiResponse
    {
        public SettingsResponse(List<SettingItemResponse> items)
        {
            this.AddRange(items);
        }
    }
    
    public class SettingItemResponse
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public SettingItemResponse(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}