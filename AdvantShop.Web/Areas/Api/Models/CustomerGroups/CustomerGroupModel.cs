using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.CustomerGroups
{
    public class CustomerGroupModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}