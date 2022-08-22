using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Web.Admin.Models.Bonuses.Cards;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Bonuses.Cards
{
    public class SubtractAditionBonusHandler : AbstractCommandHandler<bool>
    {
        private readonly SubctractAdditionalBonusModel _model;
        private Card _card;
        private AdditionBonus _bonus;
        public SubtractAditionBonusHandler(SubctractAdditionalBonusModel model)
        {
            _model = model;
        }

        protected override void Load()
        {
            _card = CardService.Get(_model.CardId);
            _bonus = AdditionBonusService.Get(_model.AdditionId);
        }

        protected override void Validate()
        {
            if (_card == null) throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardNotExist"));
            if (_card.Blocked) throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardIsBlock"));

            if (_bonus == null) throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.BonusNotExist"));
            if (_bonus.Amount < _model.Amount) throw new BlException(T("Admin.Cards.SubstractMainBonusHandler.Error.MoreSubstractThatHave"));
        }

        protected override bool Handle()
        {
            BonusSystemService.SubtractAddAditionalBonuses(_bonus, _card.CardId, _model.Amount, _model.Reason, _model.SendSms);
            return true;
        }
    }
}
