using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    public class AcceptMainBonuses : AbstractCommandHandler<ApiResponse>
    {
        private readonly long _cardId;
        private readonly MainBonusModel _model;
        private readonly bool _adding;
        private Card _card;

        public AcceptMainBonuses(long cardId, MainBonusModel model, bool adding)
        {
            _cardId = cardId;
            _model = model;
            _adding = adding;
        }

        protected override void Validate()
        {
            _card = CardService.Get(_cardId);

            if (_card == null) 
                throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardNotExist"));

            if (_card.Blocked) 
                throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardIsBlock"));

            if (!_adding)
            {
                if (_card.BonusAmount < _model.Amount)
                    throw new BlException(T("Admin.Cards.SubstractMainBonusHandler.Error.MoreSubstractThatHave"));
            }
        }

        protected override ApiResponse Handle()
        {
            BonusSystemService.AcceptMainBonuses(_adding, _card, _model.Amount, _model.Reason, _model.SendSms);
            return new ApiResponse();
        }
    }
}