﻿//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using Newtonsoft.Json;

namespace AdvantShop.Payment
{
    public class PayAnyWayTemplate
    {
        public const string MerchantId = "PayAnyWay_MerchantId";
        public const string Signature = "PayAnyWay_Signature";
        public const string TestMode = "PayAnyWay_TestMode";
        public const string UnitId = "PayAnyWay_UnitId";
        public const string LimitIds = "PayAnyWay_LimitIds";

        public const string UseKassa = "PayAnyWay_UseKassa";
    }

    public class PayAnyWayItem
    {
        public string n { get; set; }
        public string p { get; set; }
        public string q { get; set; }
        public string t { get; set; }
    }

    public class PayAnyWayItemInXml
    {
        public string name { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }
        public string vatTag { get; set; }
        [JsonProperty("pm")]
        public string paymentMethod { get; set; }
        [JsonProperty("po")]
        public string paymentObject { get; set; }
    }
}