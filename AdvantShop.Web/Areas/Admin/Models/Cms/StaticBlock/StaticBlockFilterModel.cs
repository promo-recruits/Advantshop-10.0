﻿using System;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Cms.StaticBlock
{
    public class StaticBlockFilterModel : BaseFilterModel
    {
        public int? StaticBlockId { get; set; }

        public string Key { get; set; }

        public string InnerName { get; set; }

        public string Content { get; set; }

        public DateTime? Added { get; set; }

        public DateTime? Modified { get; set; }

        public bool? Enabled { get; set; }
    }
}
