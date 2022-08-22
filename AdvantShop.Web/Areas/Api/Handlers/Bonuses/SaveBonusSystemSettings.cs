using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    public class SaveBonusSystemSettings : AbstractCommandHandler<ApiResponse>
    {
        private readonly BonusSystemSettings _model;

        public SaveBonusSystemSettings(BonusSystemSettings model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (_model.CardNumberTo <= _model.CardNumberFrom)
                throw new BlException("Проверьте диапазон карт");

            if (_model.MaxOrderPercent < 0)
                _model.MaxOrderPercent = 0;

            if (_model.MaxOrderPercent > 100)
                _model.MaxOrderPercent = 100;
        }

        protected override ApiResponse Handle()
        {
            BonusSystem.IsEnabled = _model.IsEnabled;
            BonusSystem.DefaultGrade = _model.BonusGradeId;
            BonusSystem.CardFrom = _model.CardNumberFrom;
            BonusSystem.CardTo = _model.CardNumberTo;
            BonusSystem.SmsEnabled = _model.SmsEnabled;
            BonusSystem.MaxOrderPercent = _model.MaxOrderPercent;
            BonusSystem.BonusType = _model.BonusType;
            BonusSystem.BonusTextBlock = _model.BonusTextBlock;
            BonusSystem.BonusRightTextBlock = _model.BonusRightTextBlock;
            BonusSystem.ForbidOnCoupon = _model.DisallowUseWithCoupon;

            return new ApiResponse();
        }
    }
}