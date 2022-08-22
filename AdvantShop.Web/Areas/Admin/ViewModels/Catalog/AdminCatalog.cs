using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Catalog;

namespace AdvantShop.Web.Admin.ViewModels.Catalog
{
    public class AdminCatalog
    {
        public AdminCatalog()
        {
            BreadCrumbs = new List<BreadCrumbs>();
        }

        public int CategoryId { get; set; }
        public string Title { get; set; }
        public Category Category { get; set; }
        public string Search { get; set; }
        public string CategorySearch { get; set; }
        public bool HasChildCategories { get; set; }

        public ECatalogShowMethod ShowMethod { get; set; }
        public List<BreadCrumbs> BreadCrumbs { get; set; }

        public bool IsTagsVisible
        {
            get { return !SaasDataService.IsSaasEnabled || (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveTags); }
        }
    }
}