using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.Product_ProductLists
{
    [TestFixture]
    public class Product_ProductListsPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\PaginationProduct_ProductListsTest\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\PaginationProduct_ProductListsTest\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\PaginationProduct_ProductListsTest\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\PaginationProduct_ProductListsTest\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\PaginationProduct_ProductListsTest\\Catalog.ProductList.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\PaginationProduct_ProductListsTest\\Catalog.Product_ProductList.csv"
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
        public void Product_ProductListsPage()
        {
            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"1\"]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 1 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct20", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 3 line 1");
            VerifyAreEqual("TestProduct30", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("TestProduct31", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 4 line 1");
            VerifyAreEqual("TestProduct40", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 4 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 5 line 1");
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 5 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct51", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 6 line 1");
            VerifyAreEqual("TestProduct60", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 6 line 10");

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 1 line 10");
        }

        [Test]
        public void Product_ProductListsPageToPrev()
        {
            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"1\"]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 1 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct20", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 3 line 1");
            VerifyAreEqual("TestProduct30", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 2 line 1");
            VerifyAreEqual("TestProduct20", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text, "page 1 line 1");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text, "page 1 line 10");

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("TestProduct91", Driver.GetGridCell(0, "Name", "ListProducts").Text, "last page line 1");
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(9, "Name", "ListProducts").Text, "last page line 10");
        }
    }
}