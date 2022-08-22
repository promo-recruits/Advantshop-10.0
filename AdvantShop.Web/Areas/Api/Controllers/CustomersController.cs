using System;
using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Customers;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApi]
    public class CustomersController : BaseApiController
    {
        // GET api/customers
        [HttpGet]
        public JsonResult Filter(CustomersFilterModel filter) => JsonApi(new GetCustomers(filter));

        // GET api/customers/{id}
        [HttpGet]
        public JsonResult Get(Guid id) => JsonApi(new GetCustomer(id));

        // POST api/customers/add (Put not work in asp.net mvc)
        [HttpPost]
        public JsonResult Add(AddUpdateCustomerModel model) => JsonApi(new AddUpdateCustomer(model));

        // POST api/customers/{id}
        [HttpPost]
        public JsonResult Update(Guid id, AddUpdateCustomerModel model) => JsonApi(new AddUpdateCustomer(id, model));

        // POST api/customers/smsphoneconfirmation {"phone": "79000000000"}
        [HttpPost]
        public JsonResult SmsPhoneConfirmation(string phone) => JsonApi(new SmsPhoneConfirmation(phone));

        // GET api/customers/{id}/bonuses
        [HttpGet, BonusSystem]
        public JsonResult Bonuses(Guid id) => JsonApi(new GetCustomerBonuses(id));
    }
}