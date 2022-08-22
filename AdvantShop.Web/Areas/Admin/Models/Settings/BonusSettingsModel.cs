using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Service;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class BonusSettingsModel : IValidatableObject
    {
        public bool SmsEnabled { get; set; }
        public int BonusGradeId { get; set; }
        public long CardNumFrom { get; set; }
        public long CardNumTo { get; set; }

        public List<SelectListItem> Grades { get; set; }
        public bool IsEnabled { get; set; }
        public float MaxOrderPercent { get; set; }

        public EBonusType BonusType { get; set; }
        public List<SelectListItem> BonusTypes { get; set; }

        public string BonusTextBlock { get; set; }
        public string BonusRightTextBlock { get; set; }
        

        public bool ForbidOnCoupon { get; set; }

        public BonusSettingsModel()
        {
            Grades = GradeService.GetAll()
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToList();

            BonusTypes = new List<SelectListItem>();
            foreach (EBonusType type in Enum.GetValues(typeof(EBonusType)))
            {
                BonusTypes.Add(new SelectListItem() { Text = type.Localize(), Value = ((int)type).ToString() });
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CardNumTo <= CardNumFrom)
                yield return new ValidationResult("Проверьте диапазон карт");

            if (MaxOrderPercent < 0)
                MaxOrderPercent = 0;

            if (MaxOrderPercent > 100)
                MaxOrderPercent = 100;
        }
    }
}