using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.ViewModel.Home;

namespace AdvantShop.Handlers.Home
{
    public class MainPageProductsHandler
    {
        private static ProductViewModel PrepareProductModel(EProductOnMain type, int itemsCount)
        {
            if (type == EProductOnMain.Best && !SettingsCatalog.ShowBestOnMainPage ||
                type == EProductOnMain.New && !SettingsCatalog.ShowNewOnMainPage ||
                type == EProductOnMain.NewArrivals && (!SettingsCatalog.ShowNewOnMainPage || !SettingsCatalog.DisplayLatestProductsInNewOnMainPage) ||
                type == EProductOnMain.Sale && !SettingsCatalog.ShowSalesOnMainPage)
            {
                return new ProductViewModel(null);
            }

            var products = ProductOnMain.GetProductsByType(type, itemsCount);

            var model = new ProductViewModel(products)
            {
                DisplayCategory = true,
                CountProductsInLine = SettingsDesign.CountMainPageProductInLine,
                DisplayQuickView = false
            };
            return model;
        }

        public MainPageProductsViewModel Get()
        {
            var model = new MainPageProductsViewModel();

            var countNew = SettingsCatalog.ShowNewOnMainPage
                ? ProductOnMain.GetProductCountByType(EProductOnMain.New)
                : 0;

            if (countNew == 0 && SettingsCatalog.ShowNewOnMainPage && SettingsCatalog.DisplayLatestProductsInNewOnMainPage)
            {
                model.HideNewProductsLink = true;
                model.NewArrivals = true;
            }

            var itemsCount = SettingsDesign.CountMainPageProductInSection;

            model.BestSellers = PrepareProductModel(EProductOnMain.Best, itemsCount);
            model.NewProducts = countNew > 0
                ? PrepareProductModel(EProductOnMain.New, itemsCount)
                : PrepareProductModel(EProductOnMain.NewArrivals, itemsCount);
            model.Sales = PrepareProductModel(EProductOnMain.Sale, itemsCount);

            var productLists = ProductListService.GetMainPageList();
            foreach (var productList in productLists)
            {
                var products = ProductListService.GetProducts(productList.Id, itemsCount);
                if (products.Count > 0)
                {
                    var productListModel = new ProductViewModel(products)
                    {
                        Id = productList.Id,
                        Title = productList.Name,
                        DisplayCategory = true,
                        CountProductsInLine = SettingsDesign.CountMainPageProductInLine,
                        DisplayQuickView = false
                    };
                    model.ProductLists.Add(productListModel);
                }
            }

            return model;
        }

    }
}
