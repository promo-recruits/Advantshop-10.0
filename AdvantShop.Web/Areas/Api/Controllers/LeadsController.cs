﻿using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Leads;
using AdvantShop.Areas.Api.Models.Leads;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApi]
    public class LeadsController : BaseApiController
    {        
        public JsonResult Add(AddLeadModel model) => JsonApi(new AddLead(model));
    }
}