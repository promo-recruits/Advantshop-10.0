using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists
{
    [TestFixture]
    public class ProductListsPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.ProductList.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.Product_ProductList.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("product-lists-menu [data-e2e-product-list-id=\"1\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ProductListTitle"));
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void PageProductLists()
        {
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct20", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct30", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("TestProduct31", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct40", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct51", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct60", Driver.GetGridCell(9, "Name", "ListProducts").Text);
        }

        [Test]
        public void PageProductListsToBegin()
        {
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct20", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct30", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct31", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct40", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            //to begin
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text);
        }

        [Test]
        public void PageProductListsToEnd()
        {
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            //to end
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("TestProduct91", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(9, "Name", "ListProducts").Text);
        }

        [Test]
        public void PageProductListsToNext()
        {
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct20", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct30", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct31", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct40", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name", "ListProducts").Text);
        }

        [Test]
        public void PageProductListsToPrevious()
        {
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct20", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct30", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct20", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text);
        }
    }
}