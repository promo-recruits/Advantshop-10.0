using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Orders;
using AdvantShop.Areas.Api.Model.Orders;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    public class OrderController : BaseApiController
    {
        [LogRequest, AuthApi, HttpPost]
        public JsonResult Add(AddOrderModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new AddOrder(), model);
        }

        [LogRequest, AuthApi, HttpGet]
        public JsonResult Get(int id)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new GetOrder(), id);
        }

        [LogRequest, AuthApi, HttpPost]
        public JsonResult GetList(FilterOrdersModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new GetOrders(model));
        }

        [LogRequest, AuthApi, HttpPost]
        public JsonResult ChangeStatus(ChangeStatusModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new ChangeStatus(model));
        }

        [LogRequest, AuthApi, HttpPost]
        public JsonResult SetPaid(SetPaidModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new SetPaid(model));
        }
    }
}