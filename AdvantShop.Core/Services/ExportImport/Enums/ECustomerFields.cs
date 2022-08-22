//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ExportImport
{
    public enum ECustomerFields
    {
        [StringName("none")]
        [Localize("Core.ExportImport.CustomerFields.NotSelected")]
        [CsvFieldsStatus(CsvFieldStatus.None)]
        None,
        
        [StringName("customerid")]
        [Localize("Core.ExportImport.CustomerFields.CustomerId")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CustomerId,

        [StringName("firstname")]
        [Localize("Core.ExportImport.CustomerFields.FirstName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        FirstName,

        [StringName("lastname")]
        [Localize("Core.ExportImport.CustomerFields.LastName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        LastName,

        [StringName("patronymic")]
        [Localize("Core.ExportImport.CustomerFields.Patronymic")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Patronymic,

        [StringName("phone")]
        [Localize("Core.ExportImport.CustomerFields.Phone")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Phone,

        [StringName("email")]
        [Localize("Core.ExportImport.CustomerFields.Email")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Email,

        [StringName("customergroup")]
        [Localize("Core.ExportImport.CustomerFields.CustomerGroup")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CustomerGroup,

        [StringName("enabled")]
        [Localize("Core.ExportImport.CustomerFields.Enabled")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Enabled,
        
        [StringName("admincomment")]
        [Localize("Core.ExportImport.CustomerFields.AdminComment")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        AdminComment,




        [StringName("city")]
        [Localize("Core.ExportImport.CustomerFields.City")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        City,

        [StringName("region")]
        [Localize("Core.ExportImport.CustomerFields.Region")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Region,

        [StringName("country")]
        [Localize("Core.ExportImport.CustomerFields.Country")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Country,

        [StringName("zip")]
        [Localize("Core.ExportImport.CustomerFields.Zip")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Zip,

        [StringName("address")]
        [Localize("Core.ExportImport.CustomerFields.Address")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Address,

        [StringName("street")]
        [Localize("Core.ExportImport.CustomerFields.Street")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Street,

        [StringName("house")]
        [Localize("Core.ExportImport.CustomerFields.House")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        House,

        [StringName("apartment")]
        [Localize("Core.ExportImport.CustomerFields.Apartment")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Apartment,

        [StringName("birthday")]
        [Localize("Core.ExportImport.CustomerFields.BirthDay")]
        [CsvFieldsStatus(CsvFieldStatus.NullableDateTime)]
        BirthDay,

        [StringName("organization")]
        [Localize("Core.ExportImport.CustomerFields.Organization")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Organization,
        
        [StringName("managername")]
        [Localize("Core.ExportImport.CustomerFields.ManagerName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        ManagerName,

        [StringName("managerid")]
        [Localize("Core.ExportImport.CustomerFields.ManagerId")]
        [CsvFieldsStatus(CsvFieldStatus.Int)]
        ManagerId,
    }
}