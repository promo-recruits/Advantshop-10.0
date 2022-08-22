using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Grid.Tests.Grid
{
    [TestFixture]
    public class GridProductsPaginationTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Grid\\ProductsPageTest\\Catalog.Product.csv",
                "data\\Admin\\Grid\\ProductsPageTest\\Catalog.Offer.csv",
                "data\\Admin\\Grid\\ProductsPageTest\\Catalog.Category.csv",
                "data\\Admin\\Grid\\ProductsPageTest\\Catalog.ProductCategories.csv"
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
        public void PageGridProducts()
        {
            GoToAdmin("catalog?categoryid=1");
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct20", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("TestProduct31", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct40", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct50", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("TestProduct51", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct60", Driver.GetGridCellText(9, "Name"));
        }

        [Test]
        public void PageGridProductsToBegin()
        {
            GoToAdmin("catalog?categoryid=1");
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct20", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct31", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct40", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct50", Driver.GetGridCellText(9, "Name"));

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));
        }

        [Test]
        public void PageGridProductsToEnd()
        {
            GoToAdmin("catalog?categoryid=1");
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            //to end
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("TestProduct91", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct100", Driver.GetGridCellText(9, "Name"));
        }

        [Test]
        public void PageGridProductsToNext()
        {
            GoToAdmin("catalog?categoryid=1");
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct20", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct31", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct40", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct50", Driver.GetGridCellText(9, "Name"));
        }

        [Test]
        public void PageGridProductsToPrevious()
        {
            GoToAdmin("catalog?categoryid=1");
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct20", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct11", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct20", Driver.GetGridCellText(9, "Name"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));
        }
    }
}