using System;

namespace AdvantShop.Web.Admin.Models.Bonuses.Cards
{
    public class AddMainBonusModel
    {
        public AddMainBonusModel()
        {
            SendSms = true;
        }
        public Guid CardId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public bool SendSms { get; set; }
    }
}
