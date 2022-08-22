using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    public class GetBonusSystemSettings : AbstractCommandHandler<BonusSystemSettings>
    {
        public GetBonusSystemSettings()
        {
        }

        protected override void Validate()
        {
        }

        protected override BonusSystemSettings Handle()
        {
            var settings = new BonusSystemSettings()
            {
                IsEnabled = BonusSystem.IsEnabled,
                BonusGradeId = BonusSystem.DefaultGrade,
                CardNumberFrom = BonusSystem.CardFrom,
                CardNumberTo = BonusSystem.CardTo,
                SmsEnabled = BonusSystem.SmsEnabled,
                MaxOrderPercent = BonusSystem.MaxOrderPercent,
                BonusType = BonusSystem.BonusType,
                BonusTextBlock = BonusSystem.BonusTextBlock,
                BonusRightTextBlock = BonusSystem.BonusRightTextBlock,
                DisallowUseWithCoupon = BonusSystem.ForbidOnCoupon
            };

            var grade = settings.Grades.Find(x => x.Value == settings.BonusGradeId.ToString());
            if (grade != null)
                grade.Selected = true;

            var bonusType = settings.BonusTypes.Find(x => x.Value == settings.BonusType.ToString());
            if (bonusType != null)
                bonusType.Selected = true;

            return settings;
        }
    }
}