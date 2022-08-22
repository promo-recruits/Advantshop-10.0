using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.
    Product_ProductListsFiltersPagination
{
    [TestFixture]
    public class Prod_ProdListAddFilterPricePageViewTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterPage\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterPage\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterPage\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterPage\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterPage\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterPage\\Catalog.ProductList.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterPage\\Catalog.Product_ProductList.csv"
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
        public void AddProduct_ProductListsFilterPricePage()
        {
            Functions.AddProduct_ProductListsFilterFromTo(Driver, BaseUrl, filterName: "Price");
            Functions.FilterPageFromTo(Driver, BaseUrl, "h2");

            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct17", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 1 line 10");


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("TestProduct18", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct26", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 2 line 10");


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("TestProduct27", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 3 line 1");
            VerifyAreEqual("TestProduct35", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 3 line 10");


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("TestProduct36", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 4 line 1");
            VerifyAreEqual("TestProduct44", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 4 line 10");


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct45", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 5 line 1");
            VerifyAreEqual("TestProduct53", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 5 line 10");


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct54", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 6 line 1");
            VerifyAreEqual("TestProduct62", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 6 line 10");

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct17", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 1 line 10");
        }

        [Test]
        public void AddProduct_ProductListsFilterPricePageToPrev()
        {
            Functions.AddProduct_ProductListsFilterFromTo(Driver, BaseUrl, filterName: "Price");
            Functions.FilterPageFromTo(Driver, BaseUrl, "h2");

            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct17", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 1 line 10");


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct18", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct26", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 2 line 10");


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct27", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 3 line 1");
            VerifyAreEqual("TestProduct35", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 3 line 10");


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct18", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct26", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 2 line 10");


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct17", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "page 1 line 10");

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "last page line 1");
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "last page line 10");
        }

        [Test]
        public void AddProduct_ProductListsFilterPriceView()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Functions.AddProduct_ProductListsFilterFromTo(Driver, BaseUrl, filterName: "Price");
            Functions.FilterPageFromTo(Driver, BaseUrl, "h2");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "line 1");
            VerifyAreEqual("TestProduct17", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "line 10");

            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "line 1");
            VerifyAreEqual("TestProduct26", Driver.GetGridCell(19, "Name", "ProductsSelectvizr").Text, "line 20");
        }
    }
}