using AdvantShop.Core.Services.Api;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Customers
{
    
    public class SmsPhoneConfirmationResponse : ApiResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}