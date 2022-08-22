﻿using AdvantShop.Core.Services.Orders;

namespace AdvantShop.Models.Checkout
{
    public class CheckoutPostModel
    {
        public string CaptchaCode { get; set; }
        public string CaptchaSource { get; set; }
        public string CustomData { get; set; }
        public OrderType OrderType { get; set; }

        public bool IsLanding { get; set; }
    }
}