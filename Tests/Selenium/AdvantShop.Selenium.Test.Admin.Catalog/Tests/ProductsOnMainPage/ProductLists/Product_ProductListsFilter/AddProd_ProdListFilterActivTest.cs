using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.Product_ProductListsFilter
{
    [TestFixture]
    public class Prod_ProdListAddFilterActivTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterPage\\Catalog.Brand.csv",
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
        public void AddProduct_ProductListsFilterActivityOn()
        {
            Functions.AddProduct_ProductListsFilterSelect(Driver, BaseUrl, filter: "Enabled", @select: "Активные");
            VerifyAreEqual("Найдено записей: 248",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text);

            //close
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"));
            Functions.GridFilterClose(Driver, BaseUrl, name: "Enabled");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text);
        }

        [Test]
        public void AddProduct_ProductListsFilterActivityOff()
        {
            Functions.AddProduct_ProductListsFilterSelect(Driver, BaseUrl, filter: "Enabled", @select: "Неактивные");
            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyAreEqual("TestProduct2", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct3", Driver.GetGridCell(1, "Name", "ProductsSelectvizr").Text);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "Enabled");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(1, "Name", "ProductsSelectvizr").Text);
        }
    }
}