using System.Collections.Generic;
using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Domain.Common;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;

namespace AdvantShop.App.Landing.Models
{
    public class IndexViewModel
    {
        public Lp LandingPage { get; set; }

        public List<LpBlock> Blocks { get; set; }

        /// <summary>
        /// Сквозные блоки между страницами
        /// </summary>
        public List<LpBlock> BlocksOnAllPages { get; set; }

        public ELpShoppingCartType ShoppingCartType { get; set; }

        public bool ShoppingCartHideShipping { get; set; }
    }
}
