using AdvantShop.Configuration;
using AdvantShop.Core.Scheduler;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace AdvantShop.ExportImport
{
    [Serializable()]
    public class ExportFeedSettings
    {
        [JsonIgnore]
        public int ExportFeedId { get; set; }

        [JsonProperty(PropertyName = "FileName")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "FileExtention")]
        public string FileExtention { get; set; }

        //[JsonProperty(PropertyName = "PriceMargin")]
        //public float PriceMargin { get; set; }
        
        //[JsonProperty(PropertyName = "PriceMarginType")]
        //public PriceMarginType PriceMarginType { get; set; }


        [JsonProperty(PropertyName = "PriceMarginInPercents")]
        public float PriceMarginInPercents { get; set; }

        [JsonProperty(PropertyName = "PriceMarginInNumbers")]
        public float PriceMarginInNumbers { get; set; }



        [JsonProperty(PropertyName = "AdditionalUrlTags")]
        public string AdditionalUrlTags { get; set; }

        [JsonProperty(PropertyName = "Active")]
        public bool Active { get; set; }

        [JsonProperty(PropertyName = "IntervalType")]
        public TimeIntervalType IntervalType { get; set; }

        [JsonProperty(PropertyName = "Interval")]
        public int Interval { get; set; }

        [JsonProperty(PropertyName = "JobStartTime")]
        public DateTime JobStartTime { get; set; }

        private string _advancedSettings;

        [JsonProperty(PropertyName = "AdvancedSettings")]
        public string AdvancedSettings
        {
            get { return _advancedSettings; }
            set
            {
                _advancedSettings = value;
                _advancedSettingsObject = null;
            }
        }

        private ExportFeedYandexOptions _advancedSettingsObject;

        [JsonIgnore]
        public ExportFeedYandexOptions AdvancedSettingsObject
        {
            get { return _advancedSettingsObject ?? (_advancedSettingsObject = ExportFeedSettingsProvider.ConvertAdvancedSettings<ExportFeedYandexOptions>(AdvancedSettings)); }
        }

        [JsonProperty(PropertyName = "ExportAllProducts")]
        public bool ExportAllProducts { get; set; }

        [JsonIgnore]
        private string _fileName;

        [JsonIgnore]
        public string FileFullName
        {
            get
            {
                //if (string.IsNullOrEmpty(_fileName))
                // {
                _fileName = FileName + "." + FileExtention;
                _fileName = _fileName.Replace("#DATE#", DateTime.Now.ToString("yyyyMMddHHmmss"));
                _fileName = _fileName.Replace("#SALT#", BitConverter.ToString(new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(SettingsLic.LicKey ?? string.Empty)), 0, 10));
                return _fileName;
                //}
                //else
                //{
                //    return _fileName;
                //}
            }
        }

        [JsonIgnore]
        public string FileFullPath { get { return SettingsGeneral.AbsolutePath + FileFullName; } }

        [JsonIgnore]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "ExportAdult")]
        public bool ExportAdult { get; set; }
    }

    public enum PriceMarginType
    {
        Percent = 0,
        Fixed = 1
    }
}