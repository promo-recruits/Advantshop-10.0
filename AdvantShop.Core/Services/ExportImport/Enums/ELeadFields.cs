using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ExportImport
{
    public enum ELeadFields
    {
        [StringName("none")]
        [Localize("Core.ExportImport.LeadFields.NotSelected")]
        [CsvFieldsStatus(CsvFieldStatus.None)]
        None,

        [StringName("воронка продаж")]
        [Localize("Core.ExportImport.LeadFields.SalesFunnel")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        SalesFunnel,

        [StringName("этап сделки")]
        [Localize("Core.ExportImport.LeadFields.DealStatus")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        DealStatus,
        
        [StringName("имя менеджера")]
        [Localize("Core.ExportImport.LeadFields.ManagerName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        ManagerName,
        
        [StringName("заголовок")]
        [Localize("Core.ExportImport.LeadFields.Title")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Title,

        [StringName("описание")]
        [Localize("Core.ExportImport.LeadFields.Description")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Description,
        
        [StringName("id пользователя")]
        [Localize("Core.ExportImport.LeadFields.CustomerId")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CustomerId,

        [StringName("имя")]
        [Localize("Core.ExportImport.LeadFields.FirstName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        FirstName,

        [StringName("фамилия")]
        [Localize("Core.ExportImport.LeadFields.LastName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        LastName,

        [StringName("отчество")]
        [Localize("Core.ExportImport.LeadFields.Patronymic")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Patronymic,

        [StringName("организация")]
        [Localize("Core.ExportImport.LeadFields.Organization")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Organization,

        [StringName("email")]
        [Localize("Core.ExportImport.LeadFields.Email")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Email,

        [StringName("телефон")]
        [Localize("Core.ExportImport.LeadFields.Phone")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Phone,

        [StringName("страна")]
        [Localize("Core.ExportImport.LeadFields.Country")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Country,

        [StringName("регион")]
        [Localize("Core.ExportImport.LeadFields.Region")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Region,

        [StringName("город")]
        [Localize("Core.ExportImport.LeadFields.City")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        City,

        [StringName("день рождения")]
        [Localize("Core.ExportImport.LeadFields.BirthDay")]
        [CsvFieldsStatus(CsvFieldStatus.NullableDateTime)]
        BirthDay,

        [StringName("артикул:цена:количество")]
        [Localize("Core.ExportImport.LeadFields.MultiOffer")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        MultiOffer,
    }
}
