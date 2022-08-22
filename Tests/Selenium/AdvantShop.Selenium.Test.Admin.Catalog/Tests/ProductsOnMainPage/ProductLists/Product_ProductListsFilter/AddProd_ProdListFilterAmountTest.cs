using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.Product_ProductListsFilter
{
    [TestFixture]
    public class Prod_ProdListAddFilterAmountTest : BaseSeleniumTest
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
        public void AddProduct_ProductListFilterAmountMinMax()
        {
            Functions.AddProduct_ProductListsFilterFromTo(Driver, BaseUrl, filterName: "Amount");
            Driver.SetGridFilterRange("Amount", "200", "250");
            VerifyAreEqual("Найдено записей: 51",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyAreEqual("TestProduct200", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("TestProduct250", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "Amount");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
        }

        [Test]
        public void AddProduct_ProductListFilterAmountMinMaxNotExist()
        {
            Functions.AddProduct_ProductListsFilterFromTo(Driver, BaseUrl, filterName: "Amount");

            //check min not exist
            Driver.SetGridFilterRange("Amount", "500", "");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check max not exist
            Driver.SetGridFilterRange("Amount", "", "2000"); 
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("TestProduct189", Driver.GetGridCell(99, "Name", "ProductsSelectvizr").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct19", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct53", Driver.GetGridCell(99, "Name", "ProductsSelectvizr").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct54", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(49, "Name", "ProductsSelectvizr").Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".pagination-next.disabled")).Count == 1);

            //check min and max not exist
            Driver.SetGridFilterRange("Amount", "500", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void AddProduct_ProductListFilterAmountMinMaxInvalidSymbols()
        {
            Functions.AddProduct_ProductListsFilterFromTo(Driver, BaseUrl, filterName: "Amount");

            //check min invalid symbols
            Driver.SetGridFilterRange("Amount", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text);

            //check max invalid symbols
            Driver.SetGridFilterRange("Amount", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text);

            //check min and max invalid symbols
            Driver.SetGridFilterRange("Amount", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text);
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text);
        }

        [Test]
        public void AddProduct_ProductListFilterAmountMinMaxTooMuchSymbols()
        {
            Functions.AddProduct_ProductListsFilterFromTo(Driver, BaseUrl, filterName: "Amount");

            //check min too much symbols
            Driver.SetGridFilterRange("Amount", "1111111111", ""); 
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check max too much symbols
            Driver.SetGridFilterRange("Amount", "", "1111111111");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("TestProduct189", Driver.GetGridCell(99, "Name", "ProductsSelectvizr").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct19", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct53", Driver.GetGridCell(99, "Name", "ProductsSelectvizr").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct54", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(49, "Name", "ProductsSelectvizr").Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".pagination-next.disabled")).Count == 1);

            //check min and max too much symbols
            Driver.SetGridFilterRange("Amount", "1111111111", "1111111111");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }
    }
}