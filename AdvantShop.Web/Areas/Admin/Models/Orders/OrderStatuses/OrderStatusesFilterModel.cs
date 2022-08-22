﻿using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Orders.OrderStatuses
{
    public class OrderStatusesFilterModel : BaseFilterModel
    {
        public string Name { get; set; }

        public int? CommandId { get; set; }

        public bool? IsDefault { get; set; }

        public bool? IsCanceled { get; set; }

        public bool? IsCompleted { get; set; }

        public bool? CancelForbidden { get; set; }
    }
}
