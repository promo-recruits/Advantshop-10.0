using System;
using AdvantShop.Core.Services.Api;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Customers
{
    public class AddUpdateCustomerModel : CustomerModel
    {
        [JsonProperty("partnerId")]
        public int? PartnerId { get; set; }
    }
    
    public class AddUpdateCustomerResponse : ApiResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}