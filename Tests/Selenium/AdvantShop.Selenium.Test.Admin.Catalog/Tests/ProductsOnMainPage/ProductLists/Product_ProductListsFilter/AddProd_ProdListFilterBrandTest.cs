using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.Product_ProductListsFilter
{
    [TestFixture]
    public class Prod_ProdListAddFilterBrandTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterTest\\Catalog.Photo.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterTest\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterTest\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterTest\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterTest\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterTest\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterTest\\Catalog.ProductList.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterTest\\Catalog.Product_ProductList.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void AddProduct_ProductListsFilterBrand()
        {
            Functions.AddProduct_ProductListsFilterSelect(Driver, BaseUrl, filter: "BrandId", @select: "BrandName10");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            var optionsCount = Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]"))
                .FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count;
            Driver.FindElement(By.CssSelector(".modal-header-title")).Click();

            VerifyIsTrue(optionsCount == 105); //all brands  
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct115", Driver.GetGridCell(1, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct220", Driver.GetGridCell(2, "Name", "ProductsSelectvizr").Text);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "BrandId");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text);
        }
    }
}