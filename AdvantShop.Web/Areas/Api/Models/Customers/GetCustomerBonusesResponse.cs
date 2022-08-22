using AdvantShop.Core.Services.Api;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Customers
{
    public class BonusCardResponse : IApiResponse
    {
        public long CardId { get; set; }

        public decimal Amount { get; set; }

        public decimal Percent { get; set; }

        public bool IsBlocked { get; set; }

        public string GradeName { get; set; }

        public int GradeId { get; internal set; }
    }
}