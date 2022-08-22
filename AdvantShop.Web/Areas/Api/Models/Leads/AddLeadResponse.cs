﻿using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Leads
{
    public class AddLeadResponse : ApiResponse
    {
        public AddLeadResponse() { }
        public AddLeadResponse(ApiStatus status, string errors):base(status, errors) { }
        public int leadId { get; set; }
    }
}