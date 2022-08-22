using System;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.IPTelephony.Yandex
{
    public class YandexTokenResponse : YandexResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        public DateTime Expires { get; set; }
    }
}
