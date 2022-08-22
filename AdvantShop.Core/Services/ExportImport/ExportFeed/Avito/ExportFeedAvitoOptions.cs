
using System;
using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;

namespace AdvantShop.ExportImport
{

    public enum EPaidPublicationOption
    {
        //размещение объявления осуществляется только при наличии подходящего пакета размещения;
        [StringName("Package")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidPublicationOption.Package")]
        Package,

        //при наличии подходящего пакета оплата размещения объявления произойдет с него; если нет подходящего пакета, но достаточно денег на кошельке Avito, то произойдет разовое размещение;
        [StringName("PackageSingle")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidPublicationOption.PackageSingle")]
        PackageSingle,

        //только разовое размещение, произойдет при наличии достаточной суммы на кошельке Avito; если есть подходящий пакет размещения, он будет проигнорирован.
        [StringName("Single")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidPublicationOption.Single")]
        Single
    }


    public enum EPaidServices
    {
        // обычное объявление;
        [StringName("Free")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.Free")]
        Free,

        //премиум-объявление;
        [StringName("Premium")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.Premium")]
        Premium,

        //VIP-объявление;
        [StringName("VIP")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.VIP")]
        VIP,

        //поднятие объявления в поиске;
        [StringName("PushUp")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.PushUp")]
        PushUp,

        //выделение объявления;
        [StringName("Highlight")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.Highlight")]
        Highlight,

        //применение пакета «Турбо-продажа»;
        [StringName("TurboSale")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.TurboSale")]
        TurboSale,

        //применение пакета «Быстрая продажа».
        [StringName("QuickSale")]
        [Localize("Core.ExportImport.ExportFeedAvito.PaidServices.QuickSale")]
        QuickSale
    }


    [Serializable()]
    public class ExportFeedAvitoOptions : ExportFeedSettings
    {
        [JsonProperty(PropertyName = "Currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "PublicationDateOffset")]
        public int PublicationDateOffset { get; set; }

        [JsonProperty(PropertyName = "DurationOfPublicationInDays")]
        public int DurationOfPublicationInDays { get; set; }

        [JsonProperty(PropertyName = "PaidPublicationOption")]
        public EPaidPublicationOption PaidPublicationOption { get; set; }

        [JsonProperty(PropertyName = "PaidServices")]
        public EPaidServices PaidServices { get; set; }

        [JsonProperty(PropertyName = "EmailMessages")]
        public bool EmailMessages { get; set; }

        [JsonProperty(PropertyName = "ManagerName")]
        public string ManagerName { get; set; }

        [JsonProperty(PropertyName = "Phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "Address")]
        public string Address { get; set; }
        
        [JsonProperty(PropertyName = "ExportNotAvailable")]
        public bool ExportNotAvailable { get; set; }

        [JsonProperty(PropertyName = "ProductDescriptionType")]
        public string ProductDescriptionType { get; set; }

        [JsonProperty(PropertyName = "DefaultAvitoCategory")]
        public string DefaultAvitoCategory { get; set; }

        [JsonProperty(PropertyName = "UnloadProperties")]
        public bool UnloadProperties { get; set; }

    }
}
