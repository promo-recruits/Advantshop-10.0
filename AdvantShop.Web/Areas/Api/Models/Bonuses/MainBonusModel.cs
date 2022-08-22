﻿using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Bonuses
{
    public class MainBonusModel
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("sendSms")]
        public bool SendSms { get; set; }
    }
}