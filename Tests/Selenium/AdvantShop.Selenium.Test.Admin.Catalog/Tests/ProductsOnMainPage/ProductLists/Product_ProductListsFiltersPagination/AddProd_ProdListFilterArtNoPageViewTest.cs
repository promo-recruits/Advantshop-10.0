using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.
    Product_ProductListsFiltersPagination
{
    [TestFixture]
    public class Prod_ProdListAddFilterArtNoPageViewTest : BaseSeleniumTest
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
        public void AddProduct_ProductListsFilterArtNoPage()
        {
            Functions.AddProduct_ProductListsFilter(Driver, BaseUrl, filterName: "ProductArtNo");
            Driver.SetGridFilterValue("ProductArtNo", "1");

            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 1 line 1 art no");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 1 line 1 product name");
            VerifyAreEqual("107", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 1 line 10 art no");
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10 product name");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("108", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 2 line 1 art no");
            VerifyAreEqual("TestProduct108", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 2 line 1 product name");
            VerifyAreEqual("116", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 2 line 10 art no");
            VerifyAreEqual("TestProduct116", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10 product name");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("117", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 3 line 1 art no");
            VerifyAreEqual("TestProduct117", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 3 line 1 product name");
            VerifyAreEqual("125", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 3 line 10 art no");
            VerifyAreEqual("TestProduct125", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 3 line 10 product name");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("126", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 4 line 1 art no");
            VerifyAreEqual("TestProduct126", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 4 line 1 product name");
            VerifyAreEqual("134", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 4 line 10 art no");
            VerifyAreEqual("TestProduct134", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 4 line 10 product name");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("135", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 5 line 1 art no");
            VerifyAreEqual("TestProduct135", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 5 line 1 product name");
            VerifyAreEqual("143", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 5 line 10 art no");
            VerifyAreEqual("TestProduct143", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 5 line 10 product name");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("144", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 6 line 1 art no");
            VerifyAreEqual("TestProduct144", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 6 line 1 product name");
            VerifyAreEqual("152", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 6 line 10 art no");
            VerifyAreEqual("TestProduct152", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 6 line 10 product name");

            //to begin             
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 1 line 1 art no");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 1 line 1 product name");
            VerifyAreEqual("107", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 1 line 10 art no");
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10 product name");
        }

        [Test]
        public void AddProduct_ProductListsFilterArtNoPageToPrev()
        {
            Functions.AddProduct_ProductListsFilter(Driver, BaseUrl, filterName: "ProductArtNo");
            Driver.SetGridFilterValue("ProductArtNo", "1");

            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 1 line 1 art no");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 1 line 1 product name");
            VerifyAreEqual("107", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 1 line 10 art no");
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10 product name");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("108", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 2 line 1 art no");
            VerifyAreEqual("TestProduct108", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 2 line 1 product name");
            VerifyAreEqual("116", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 2 line 10 art no");
            VerifyAreEqual("TestProduct116", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10 product name");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("117", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 3 line 1 art no");
            VerifyAreEqual("TestProduct117", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 3 line 1 product name");
            VerifyAreEqual("125", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 3 line 10 art no");
            VerifyAreEqual("TestProduct125", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 3 line 10 product name");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("108", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 2 line 1 art no");
            VerifyAreEqual("TestProduct108", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 2 line 1 product name");
            VerifyAreEqual("116", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 2 line 10 art no");
            VerifyAreEqual("TestProduct116", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10 product name");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 1 line 1 art no");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "page 1 line 1 product name");
            VerifyAreEqual("107", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text,
                "page 1 line 10 art no");
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10 product name");

            //to end             
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("71", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text,
                "last page line 1 art no");
            VerifyAreEqual("TestProduct71", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "last page line 1 product name");
            VerifyAreEqual("91", Driver.GetGridCell(2, "ProductArtNo", "ProductsSelectvizr").Text,
                "last page line 3 art no");
            VerifyAreEqual("TestProduct91", Driver.GetGridCell(2, "Name", "ProductsSelectvizr").Text,
                "last page line 3 product name");
        }

        [Test]
        public void AddProduct_ProductListsFilterArtNoView()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Functions.AddProduct_ProductListsFilter(Driver, BaseUrl, filterName: "ProductArtNo");
            Driver.SetGridFilterValue("ProductArtNo", "1");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text, "line 1 art no");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "line 1 product name");
            VerifyAreEqual("107", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text, "line 10 art no");
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "line 10 product name");

            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text, "line 1 art no");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "line 1 product name");
            VerifyAreEqual("116", Driver.GetGridCell(19, "ProductArtNo", "ProductsSelectvizr").Text, "line 20 art no");
            VerifyAreEqual("TestProduct116", Driver.GetGridCell(19, "Name", "ProductsSelectvizr").Text,
                "line 20 product name");
        }
    }
}