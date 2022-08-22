﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;

namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportFeedSettingsResellerModel : IValidatableObject
    {
        public ExportFeedSettingsResellerModel(ExportFeedResellerOptions exportFeedResellerOptions)
        {
            CsvEnconing = exportFeedResellerOptions.CsvEnconing;
            CsvSeparator = exportFeedResellerOptions.CsvSeparator;
            CsvColumSeparator = exportFeedResellerOptions.CsvColumSeparator;
            CsvPropertySeparator = exportFeedResellerOptions.CsvPropertySeparator;
            CsvExportNoInCategory = exportFeedResellerOptions.CsvExportNoInCategory;
            CsvCategorySort = exportFeedResellerOptions.CsvCategorySort;
            FieldMapping = exportFeedResellerOptions.FieldMapping;
            ModuleFieldMapping = exportFeedResellerOptions.ModuleFieldMapping;
            ResellerCode = exportFeedResellerOptions.ResellerCode;

            RecomendedPriceMargin = exportFeedResellerOptions.RecomendedPriceMargin;
            RecomendedPriceMarginType = exportFeedResellerOptions.RecomendedPriceMarginType;
            var recomendedPriceMarginTypeList = new Dictionary<EExportFeedResellerPriceMarginType, string>();
            foreach (EExportFeedResellerPriceMarginType recomendedPriceMarginType in Enum.GetValues(typeof(EExportFeedResellerPriceMarginType)))
            {
                recomendedPriceMarginTypeList.Add(recomendedPriceMarginType, recomendedPriceMarginType.Localize());
            }
            RecomendedPriceMarginTypeList = recomendedPriceMarginTypeList;

            ExportNotAvailable = exportFeedResellerOptions.ExportNotAvailable == null || exportFeedResellerOptions.ExportNotAvailable.Value;
            UploadOnlyMainCategory = exportFeedResellerOptions.UploadOnlyMainCategory.HasValue && exportFeedResellerOptions.UploadOnlyMainCategory.Value;
        }

        public string CsvEnconing { get; set; }
        public string CsvSeparator { get; set; }
        public string CsvColumSeparator { get; set; }
        public string CsvPropertySeparator { get; set; }
        public bool CsvExportNoInCategory { get; set; }
        public bool CsvCategorySort { get; set; }

        public string ResellerCode { get; set; }

        public float RecomendedPriceMargin { get; set; }
        public EExportFeedResellerPriceMarginType RecomendedPriceMarginType { get; set; }
        public Dictionary<EExportFeedResellerPriceMarginType, string> RecomendedPriceMarginTypeList { get; set; }

        public bool ExportNotAvailable { get; set; }

        public List<ProductFields> FieldMapping { get; set; }
        public List<CSVField> ModuleFieldMapping { get; set; }

        public bool UploadOnlyMainCategory { get; set; }

        public Dictionary<string, string> CsvSeparatorList
        {
            get
            {
                var csvSeparatorList = new Dictionary<string, string>();
                foreach (SeparatorsEnum csvSeparator in Enum.GetValues(typeof(SeparatorsEnum)))
                {
                    csvSeparatorList.Add(csvSeparator.StrName(), csvSeparator.Localize());
                }
                return csvSeparatorList;
            }
        }

        public Dictionary<string, string> CsvEnconingList
        {
            get
            {
                var csvEnconingList = new Dictionary<string, string>();
                foreach (EncodingsEnum csvEnconing in Enum.GetValues(typeof(EncodingsEnum)))
                {
                    csvEnconingList.Add(csvEnconing.StrName(), csvEnconing.StrName());
                }
                return csvEnconingList;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(CsvColumSeparator))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "CsvColumSeparator" });
            }
            if (string.IsNullOrEmpty(CsvPropertySeparator))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "CsvPropertySeparator" });
            }
        }
    }
}