using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.Product_ProductListsFilter
{
    [TestFixture]
    public class Prod_ProdListAddFilterPriceTest : BaseSeleniumTest
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
        public void AddProduct_ProductListFilterPriceMinMax()
        {
            Functions.AddProduct_ProductListsFilterFromTo(Driver, BaseUrl, filterName: "Price");
            Driver.SetGridFilterRange("Price", "200", "250");
            VerifyAreEqual("TestProduct200", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("TestProduct250", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "Price");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
        }

        [Test]
        public void AddProduct_ProductListFilterPriceMinMaxNotExist()
        {
            Functions.AddProduct_ProductListsFilterFromTo(Driver, BaseUrl, filterName: "Price");

            /*
            //check min not exist
            Driver.SetGridFilterRange("Price", "1000", "");
            VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"));

            //check max not exist
            Driver.SetGridFilterRange("Price", "", "1000");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);

            
            Driver.GridPaginationSelectItems("baseURL, 100");

            VerifyAreEqual("TestProduct189", Driver.GetGridCell(99, "Name", "ProductsSelectvizr").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct19", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);

            
            VerifyAreEqual("TestProduct53", Driver.GetGridCell(99, "Name", "ProductsSelectvizr").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct54", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);

            
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(49, "Name", "ProductsSelectvizr").Text);

            VerifyIsTrue(driver.FindElements(By.CssSelector(".pagination-next.disabled")).Count == 1);*/

            //check min and max not exist
            Driver.SetGridFilterRange("Price", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void AddProduct_ProductListFilterPriceMinMaxInvalidSymbols()
        {
            Functions.AddProduct_ProductListsFilterFromTo(Driver, BaseUrl, filterName: "Price");

            //check min not exist
            Driver.SetGridFilterRange("Price", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text);

            //check max not exist
            Driver.SetGridFilterRange("Price", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text);

            //check min and max not exist
            Driver.SetGridFilterRange("Price", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text);
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text);
        }

        [Test]
        public void AddProduct_ProductListFilterPriceMinMaxTooMuchSymbols()
        {
            Functions.AddProduct_ProductListsFilterFromTo(Driver, BaseUrl, filterName: "Price");

            /*
            //check min not exist
            Driver.SetGridFilterRange("Price", "1111111111", "");
            VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"));

            //check max not exist
            Driver.SetGridFilterRange("Price", "", "1111111111");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);

            
            Driver.GridPaginationSelectItems("baseURL, 100");

            VerifyAreEqual("TestProduct189", Driver.GetGridCell(99, "Name", "ProductsSelectvizr").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct19", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);

            
            VerifyAreEqual("TestProduct53", Driver.GetGridCell(99, "Name", "ProductsSelectvizr").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct54", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);

            
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(49, "Name", "ProductsSelectvizr").Text);

            VerifyIsTrue(driver.FindElements(By.CssSelector(".pagination-next.disabled")).Count == 1);*/

            //check min and max not exist
            Driver.WaitForModal();
            Driver.SetGridFilterRange("Price", "1111111111", "1111111111");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }
    }
}