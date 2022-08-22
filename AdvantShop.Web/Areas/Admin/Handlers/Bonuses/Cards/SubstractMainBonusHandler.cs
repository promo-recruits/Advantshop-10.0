using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Web.Admin.Models.Bonuses.Cards;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Bonuses.Cards
{
    public class SubstractMainBonusHandler : AbstractCommandHandler<bool>
    {
        private readonly AddMainBonusModel _model;
        private Card _card;

        public SubstractMainBonusHandler(AddMainBonusModel model)
        {
            _model = model;
        }

        protected override void Load()
        {
            _card = CardService.Get(_model.CardId);
        }
        protected override void Validate()
        {
            if (_card == null) throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardNotExist"));
            if (_card.Blocked) throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardIsBlock"));
            if (_card.BonusAmount < _model.Amount) throw new BlException(T("Admin.Cards.SubstractMainBonusHandler.Error.MoreSubstractThatHave"));
        }


        protected override bool Handle()
        {
            BonusSystemService.AcceptMainBonuses(false, _card, _model.Amount, _model.Reason, _model.SendSms);
            return true;
        }
    }
}
