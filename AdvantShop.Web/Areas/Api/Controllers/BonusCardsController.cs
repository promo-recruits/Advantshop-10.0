using System;
using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Bonuses;
using AdvantShop.Areas.Api.Handlers.Customers;
using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApi, BonusSystem]
    public class BonusCardsController : BaseApiController
    {
        // Получение бонусной карты по номеру
        // GET api/bonus-cards/{id}
        [HttpGet]
        public JsonResult Card(long id) => JsonApi(new GetCustomerBonuses(id));

        // Создание бонусной карты для покупателя
        // POST api/bonus-cards/add
        [HttpPost]
        public JsonResult Create(Guid customerId) => JsonApi(new CreateBonusCard(customerId));



        // Список транзакций по карте с пагинацией
        // GET api/bonus-cards/{id}/transactions
        [HttpGet]
        public JsonResult Transactions(long id, TransactionsFilterModel filter) => JsonApi(new GetTransactions(id, filter));



        // Начисление основных бонусов
        // POST api/bonus-cards/{id}/main-bonuses/accept
        [HttpPost]
        public JsonResult AcceptMainBonuses(long id, MainBonusModel model) => JsonApi(new AcceptMainBonuses(id, model, true));

        // Списание основных бонусов
        // POST api/bonus-cards/{id}/main-bonuses/substract
        [HttpPost]
        public JsonResult SubstractMainBonuses(long id, MainBonusModel model) => JsonApi(new AcceptMainBonuses(id, model, false));



        // Получение дополнительных бонусов
        // GET api/bonus-cards/{id}/additional-bonuses
        [HttpGet]
        public JsonResult GetAdditionalBonuses(long id) => JsonApi(new GetAdditionalBonuses(id));

        // Начисление дополнительных бонусов
        // POST api/bonus-cards/{id}/additional-bonuses/accept
        [HttpPost]
        public JsonResult AcceptAdditionalBonuses(long id, AddAdditionalBonusModel model) => JsonApi(new AcceptAdditionalBonuses(id, model));

        // Списание дополнительных бонусов
        // POST api/bonus-cards/{id}/additionalbonuses/substract
        [HttpPost]
        public JsonResult SubstractAdditionalBonuses(long id, SubctractAdditionalBonusModel model) => JsonApi(new SubtractAddAditionalBonuses(id, model));



        // Настройки бонусной системы
        // GET api/bonus-cards/settings
        [HttpGet]
        public JsonResult GetSettings() => JsonApi(new GetBonusSystemSettings());

        // Сохранение настроек бонусной системы
        // POST api/bonus-cards/settings
        [HttpPost]
        public JsonResult SaveSettings(BonusSystemSettings model) => JsonApi(new SaveBonusSystemSettings(model));
    }
}