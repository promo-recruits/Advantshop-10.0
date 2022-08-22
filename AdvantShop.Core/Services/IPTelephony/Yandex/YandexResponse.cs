using Newtonsoft.Json;

namespace AdvantShop.Core.Services.IPTelephony.Yandex
{
    public class YandexResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("isSuccess")]
        public bool Success { get; set; }
    }
}
