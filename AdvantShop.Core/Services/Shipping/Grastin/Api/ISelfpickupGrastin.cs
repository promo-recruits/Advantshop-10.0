﻿using AdvantShop.Shipping.Grastin;

namespace AdvantShop.Core.Services.Shipping.Grastin.Api
{
    public interface ISelfpickupGrastin
    {
        string Id { get; set; }
        string Name { get; set; }
        string TimeTable { get; set; }
        bool RegionalPoint { get; set; }
        string DrivingDescription { get; set; }
        string Phone { get; set; }
        string LinkDrivingDescription { get; set; }
        string Metrostation { get; set; }
        bool AcceptsPaymentCards { get; set; }
        float PointX { get; set; }
        float PointY { get; set; }
        EnPartner TypePoint { get; }
    }
}
