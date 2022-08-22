using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Areas.Mobile.Models.Catalog;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Configuration.Settings;

namespace AdvantShop.Areas.Mobile.Handlers.Catalog
{
    public class GetCatalogRoots
    {
        private List<Category> _categories;
        private readonly UrlHelper _urlHelper;
        private string _currentUrl = string.Empty;
        private SettingsMobile.eViewCategoriesOnMain _viewMode;

        public GetCatalogRoots(string currentUrl, SettingsMobile.eViewCategoriesOnMain viewMode)
        {
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            _currentUrl = currentUrl;
            _viewMode = viewMode;
        }

        public CatalogMenuModel Execute()
        {
            _categories = CategoryService.GetChildCategoriesByCategoryId(0, false);

            var model = new CatalogMenuModel
            {
                Items = CacheManager.Get(CacheNames.MenuCatalog + "_mobile_root_categories", () => GetCategoryItems(0, 0)),
                ViewMode = _viewMode,
                PhotoWidth = SettingsPictureSize.IconCategoryImageWidth,
                PhotoHeight = SettingsPictureSize.IconCategoryImageHeight
            };

            return model;
        }

        private List<CatalogMenuItem> GetCategoryItems(int parentCategoryId, int level)
        {
            if (level == 4)
                return null;

            var list = new List<CatalogMenuItem>();
            CatalogMenuItem item;
            foreach (var category in _categories.Where(x => x.ParentCategoryId == parentCategoryId && x.Enabled && !x.Hidden).OrderBy(x => x.SortOrder))
            {

                item = new CatalogMenuItem()
                {
                    Name = category.Name,
                    Url = _urlHelper.RouteUrl("Category", new { url = category.UrlPath }),
                    SubItems = GetCategoryItems(category.CategoryId, level + 1),
                    Icon = category.Icon
                };

                list.Add(item);
            }

            return list;
        }
    }
}