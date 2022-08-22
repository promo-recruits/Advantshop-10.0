﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;

namespace AdvantShop.Web.Admin.Models.Catalog.ExportFeeds
{
    public class ExportFeedSettingsModel : IValidatableObject
    {
        public int ExportFeedId { get; set; }

        public EExportFeedType ExportFeedType { get; set; }
        public string FileName { get; set; }
        public string FileExtention { get; set; }
        public float PriceMarginInPercents { get; set; }
        public float PriceMarginInNumbers { get; set; }
        public string AdditionalUrlTags { get; set; }

        public bool NotAvailableJob { get; set; }
        public bool Active { get; set; }
        public TimeIntervalType IntervalType { get; set; }
        public Dictionary<TimeIntervalType, string> IntervalTypeList { get; set; }
        public int Interval { get; set; }
        //public DateTime JobStartTime { get; set; }

        public int JobStartHour { get; set; }
        public int JobStartMinute { get; set; }

        [Obsolete]
        public bool ExportAllProducts { get; set; }

        public EExportFeedCatalogType ExportCatalogType { get; set; }

        //частные настройки для каждого типа экспорта, храняться  в json
        public string AdvancedSettings { get; set; }

        public Dictionary<string, string> FileExtentions { get; set; }

        public bool DoNotExportAdult { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Name"), new[] { "FileName" });
            }

            if (Interval < 1 && Active)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Category.AdminCategoryModel.Error.Interval"), new[] { "Interval" });
            }
        }
    }
}
