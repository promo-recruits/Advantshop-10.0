﻿using System;

namespace AdvantShop.Web.Admin.Models.Bonuses.Cards
{
    public class CardFilterModelAddAdditionBonus : CardFilterModel
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool SendSms { get; set; }
    }
}