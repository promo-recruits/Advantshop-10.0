﻿using System.ComponentModel.DataAnnotations;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Bonuses.SmsTemplates
{
    public class SmsLogFilterModel : BaseFilterModel
    {
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Not a valid Phone number")]
        public string Phone { get; set; }
        public string Status { get; set; }
        public string Body { get; set; }
    }
}