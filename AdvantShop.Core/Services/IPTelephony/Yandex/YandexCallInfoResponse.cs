using System;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.IPTelephony.Yandex
{
    public enum EYandexCallDirection
    {
        None,
        Incoming,
        Outgoing,
        Callback,
    }

    public enum EYandexCallStatus
    {
        None,
        Missed,
        Connected,
        VoiceMail,
        Dropped
    }

    public class YandexCallInfoResponse : YandexResponse
    {
        [JsonProperty("data")]
        public YandexCallInfo Call { get; set; }
    }

    public class YandexCallInfo
    {
        public string Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public int Duration { get; set; }
        public EYandexCallDirection Direction { get; set; }
        public EYandexCallStatus CallStatus { get; set; }
        public YandexCallRecord CallRecord { get; set; }
    }

    public class YandexCallRecord
    {
        public string FileName { get; set; }
        public string Uri { get; set; }
    }
}
