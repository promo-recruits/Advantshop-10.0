﻿using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Booking.ReservationResources
{
    public class AdditionalTimeFromDataModel
    {
        public List<string> Times { get; set; }
        public List<string> WorkTimes { get; set; }
        public bool ExistAdditionalTimes { get; set; }
    }
}
