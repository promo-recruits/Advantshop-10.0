using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.ExportImport;

namespace AdvantShop.Core.Common.Extensions
{
    public static class EnumExtensions
    {
        [Obsolete]
        public static string ResourceKey(this Enum enumValue)
        {
            return AttributeHelper.GetAttributeValueField<LocalizeAttribute, string>(enumValue); // ResourceKeyAttribute
        }

        public static string Localize(this Enum enumValue)
        {
            return AttributeHelper.GetAttributeValueField<LocalizeAttribute, string>(enumValue);
        }

        public static string DescriptionKey(this Enum enumValue)
        {
            return AttributeHelper.GetAttributeValueField<DescriptionKeyAttribute, string>(enumValue);
        }

        public static string StrName(this Enum enumValue)
        {
            var attrValue = AttributeHelper.GetAttributeValueField<StringNameAttribute, string>(enumValue);
            return string.IsNullOrEmpty(attrValue) ? enumValue.ToString().ToLower() : attrValue;
        }

        public static CsvFieldStatus Status(this ProductFields enumValue)
        {
            var attrValue = AttributeHelper.GetAttributeValueField<CsvFieldsStatusAttribute, CsvFieldStatus>(enumValue);
            return attrValue;
        }

        public static CsvFieldStatus Status(this CategoryFields enumValue)
        {
            var attrValue = AttributeHelper.GetAttributeValueField<CsvFieldsStatusAttribute, CsvFieldStatus>(enumValue);
            return attrValue;
        }

        public static CsvFieldStatus Status(this ECustomerFields enumValue)
        {
            var attrValue = AttributeHelper.GetAttributeValueField<CsvFieldsStatusAttribute, CsvFieldStatus>(enumValue);
            return attrValue;
        }

        public static CsvFieldStatus Status(this ELeadFields enumValue)
        {
            var attrValue = AttributeHelper.GetAttributeValueField<CsvFieldsStatusAttribute, CsvFieldStatus>(enumValue);
            return attrValue;
        }

        public static CsvFieldStatus Status(this EBrandFields enumValue)
        {
            var attrValue = AttributeHelper.GetAttributeValueField<CsvFieldsStatusAttribute, CsvFieldStatus>(enumValue);
            return attrValue;
        }

        public static string ColorValue(this Enum enumValue)
        {
            return AttributeHelper.GetAttributeValueField<ColorAttribute, string>(enumValue);
        }

        public static EFieldType FieldType(this Enum enumValue)
        {
            return AttributeHelper.GetAttributeValueField<FieldTypeAttribute, EFieldType>(enumValue);
        }

        public static FieldTypeValue FieldTypeValue(this Enum enumValue)
        {
            return AttributeHelper.GetAttributeValueField<FieldTypeValueAttribute, FieldTypeValue>(enumValue);
        }

        public static bool Ignore(this Enum enumValue)
        {
            return AttributeHelper.GetAttributeValueField<EnumIgnoreAttribute, bool>(enumValue);
        }

        public static Dictionary<TEnum, string> ToDictionary<TEnum>()
        {
            var values = (from TEnum e in Enum.GetValues(typeof(TEnum))
                select new {Id = e, Name = e.ToString()}).ToDictionary(x => x.Id, x => x.Name);
            return values;
        }

        #region CSV V2

        private static CsvV2FieldAttribute GetCsvV2FieldAttribute(this EProductField enumValue)
        {
            var attr = enumValue.GetAttribute<CsvV2FieldAttribute>();
            if (attr == null)
                throw new NotImplementedException("CsvV2FieldAttribute not set to field type " + enumValue.ToString());
            return attr;
        }

        public static CsvFieldStatus Status(this EProductField enumValue)
        {
            return enumValue.GetCsvV2FieldAttribute().FieldStatus;
        }

        public static bool IsOfferField(this EProductField enumValue)
        {
            return enumValue.GetCsvV2FieldAttribute().IsOfferField;
        }

        public static bool IsPostProcessField(this EProductField enumValue)
        {
            return enumValue.GetCsvV2FieldAttribute().IsPostProcessField;
        }

        #endregion

        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue) where TAttribute : Attribute
        {
            var type = enumValue.GetType();
            var name = Enum.GetName(type, enumValue);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}
