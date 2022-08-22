using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.
    Product_ProductListsFiltersPagination
{
    [TestFixture]
    public class Prod_ProdListAddFilterBrandPageViewTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\FilterPage\\Catalog.Photo.csv",
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
        public void AddProduct_ProductListsFilterBrandPage()
        {
            Functions.AddProduct_ProductListsFilterSelect(Driver, BaseUrl, filter: "BrandId", @select: "BrandName1");

            VerifyAreEqual("TestProduct100", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct109", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("TestProduct110", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct119", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("TestProduct120", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 3 line 1");
            VerifyAreEqual("TestProduct129", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("TestProduct130", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 4 line 1");
            VerifyAreEqual("TestProduct139", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 4 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct140", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 5 line 1");
            VerifyAreEqual("TestProduct149", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 5 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct150", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 6 line 1");
            VerifyAreEqual("TestProduct159", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 6 line 10");

            //to begin             
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct109", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");
        }

        [Test]
        public void AddProduct_ProductListsFilterBrandPageToPrev()
        {
            Functions.AddProduct_ProductListsFilterSelect(Driver, BaseUrl, filter: "BrandId", @select: "BrandName1");

            VerifyAreEqual("TestProduct100", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct109", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct110", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct119", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct120", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 3 line 1");
            VerifyAreEqual("TestProduct129", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct110", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct119", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct109", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "page 1 line 10");

            //to end             
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "last page line 1");
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "last page line 10");
        }

        [Test]
        public void AddProduct_ProductListsFilterBrandView()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Functions.AddProduct_ProductListsFilterSelect(Driver, BaseUrl, filter: "BrandId", @select: "BrandName1");
            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "line 1");
            VerifyAreEqual("TestProduct109", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text, "line 10");

            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text, "line 1");
            VerifyAreEqual("TestProduct119", Driver.GetGridCell(19, "Name", "ProductsSelectvizr").Text, "line 20");
        }
    }
}