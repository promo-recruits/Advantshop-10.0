namespace AdvantShop.Web.Admin.Models.Bonuses.Cards
{
    public class CardFilterModelAddBonus : CardFilterModel
    {
        public CardFilterModelAddBonus()
        {
            SendSms = true;
        }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
        public bool SendSms { get; set; }
    }
}