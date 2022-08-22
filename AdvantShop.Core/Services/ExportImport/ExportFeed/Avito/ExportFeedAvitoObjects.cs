

using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ExportImport
{
    public enum EAvitoCommonTegs
    {
        [StringName("AvitoId")]
        AvitoId,
        [StringName("DateBegin")]
        DateBegin,
        [StringName("DateEnd")]
        DateEnd,
        [StringName("ListingFee")]
        ListingFee,
        [StringName("AdStatus")]
        AdStatus,
        [StringName("AllowEmail")]
        AllowEmail,
        [StringName("ManagerName")]
        ManagerName,
        [StringName("ContactPhone")]
        ContactPhone,
        [StringName("Address")]
        Address
    }

    public class ExportFeedAvitoLocationObject
    {
        public int ParentId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ExportFeedAvitoProductProperty
    {
        public int ProductId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
    }
}
