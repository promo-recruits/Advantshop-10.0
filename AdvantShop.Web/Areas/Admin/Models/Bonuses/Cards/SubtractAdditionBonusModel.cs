using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Web.Admin.Models.Bonuses.Cards
{
    public class SubtractAdditionBonusModel : IValidatableObject
    {
        public int BonusId { get; set; }
        [Range(typeof(decimal), "0", "100000")]
        public decimal Amount { get; set; }
        public string Basis { get; set; }
        public Guid CardId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
