using System;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Customers
{
    public class GetCustomerBonuses : AbstractCommandHandler<BonusCardResponse>
    {
        private readonly Guid _id;
        private readonly long _cardNumber;
        private Card _bonusCard;

        public GetCustomerBonuses(Guid id)
        {
            _id = id;
        }

        public GetCustomerBonuses(long cardNumber)
        {
            _cardNumber = cardNumber;
        }

        protected override void Validate()
        {
            if (_cardNumber != 0)
            {
                _bonusCard = BonusSystemService.GetCard(_cardNumber);
            }
            else
            {
                var customer = CustomerService.GetCustomer(_id);
                if (customer == null)
                    throw new BlException("Пользователь не найден");               

                if (customer.BonusCardNumber == null)
                    throw new BlException("У покупателя нет бонусной карты");

                _bonusCard = BonusSystemService.GetCard(customer.BonusCardNumber);
            }

            if (_bonusCard == null)
                throw new BlException("Бонусная карта не найдена");
        }

        protected override BonusCardResponse Handle()
        {
            return new BonusCardResponse()
            {
                CardId = _bonusCard.CardNumber,
                Amount = _bonusCard.BonusesTotalAmount,
                Percent = _bonusCard.Grade.BonusPercent,                
                GradeName = _bonusCard.Grade.Name,
                GradeId = _bonusCard.Grade.Id,
                IsBlocked = _bonusCard.Blocked
            };
        }
    }
}