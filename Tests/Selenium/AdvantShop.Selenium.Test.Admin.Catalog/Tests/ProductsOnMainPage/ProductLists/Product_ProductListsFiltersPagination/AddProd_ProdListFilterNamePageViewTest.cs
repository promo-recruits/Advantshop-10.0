using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.
    Product_ProductListsFiltersPagination
{
    [TestFixture]
    public class Prod_ProdListAddFilterNamePageViewTest : BaseSeleniumTest
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
        public void AddProduct_ProductListsFilterNamePage()
        {
            Functions.AddProduct_ProductListsFilter(Driver, BaseUrl, filterName: "Name");
            Driver.SetGridFilterValue("Name", "TestProduct");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("TestProduct108", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct116", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("TestProduct117", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 3 line 1");
            VerifyAreEqual("TestProduct125", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("TestProduct126", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 4 line 1");
            VerifyAreEqual("TestProduct134", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 4 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct135", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 5 line 1");
            VerifyAreEqual("TestProduct143", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 5 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct144", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 6 line 1");
            VerifyAreEqual("TestProduct152", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 6 line 10");

            //to begin             
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");
        }

        [Test]
        public void AddProduct_ProductListsFilterNamePageToPrev()
        {
            Functions.AddProduct_ProductListsFilter(Driver, BaseUrl, filterName: "Name");
            Driver.SetGridFilterValue("Name", "TestProduct");

            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct108", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct116", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct117", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 3 line 1");
            VerifyAreEqual("TestProduct125", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct108", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct116", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");

            //to end             
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "last page line 1");
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "last page line 10");
        }

        [Test]
        public void AddProduct_ProductListsFilterNameView()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Functions.AddProduct_ProductListsFilter(Driver, BaseUrl, filterName: "Name");
            Driver.SetGridFilterValue("Name", "TestProduct");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "line 1");
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "line 10");

            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "line 1");
            VerifyAreEqual("TestProduct116", Driver.GetGridCell(19, "Name", "ProductsSelectvizr").Text, "line 20");
        }
    }
}