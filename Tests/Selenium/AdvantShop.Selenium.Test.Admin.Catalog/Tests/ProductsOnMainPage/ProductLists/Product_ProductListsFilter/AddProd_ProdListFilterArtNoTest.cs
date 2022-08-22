using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.Product_ProductListsFilter
{
    [TestFixture]
    public class Prod_ProdListAddFilterArtNoTest : BaseSeleniumTest
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
        public void AddProduct_ProductListFilterArtNoExist()
        {
            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Добавить фильтр",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]")).Text);
            VerifyAreEqual("Выбор товара", Driver.FindElement(By.TagName("h2")).Text);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Functions.GridFilterSet(Driver, BaseUrl, name: "ProductArtNo");
            Driver.SetGridFilterValue("ProductArtNo", "14");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyAreEqual("14", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct14", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "ProductArtNo");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
        }

        [Test]
        public void AddProduct_ProductListFilterArtNoNotExist()
        {
            Functions.AddProduct_ProductListsFilter(Driver, BaseUrl, filterName: "ProductArtNo");

            //check not exist product
            Driver.SetGridFilterValue("ProductArtNo", "574"); 
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check too much symbols 
            Driver.SetGridFilterValue("ProductArtNo", "1111111111222222222222222222333333333333334444444444555555555555555555555555");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check invalid symbols 
            Driver.SetGridFilterValue("ProductArtNo", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }
    }
}