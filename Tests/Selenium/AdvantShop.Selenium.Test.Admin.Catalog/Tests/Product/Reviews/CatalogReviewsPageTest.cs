using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Reviews\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\Reviews\\Customers.CustomerGroup.csv",
                "data\\Admin\\Catalog\\Reviews\\Customers.Customer.csv",
                "data\\Admin\\Catalog\\Reviews\\CMS.Review.csv"
            );

            Init();

            GoToAdmin("reviews");
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
        public void Page()
        {
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 20", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Текст отзыва 21", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 10", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Текст отзыва 11", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 45", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("Текст отзыва 40", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 35", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Текст отзыва 32", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 250", Driver.GetGridCell(9, "Text").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 20", Driver.GetGridCell(9, "Text").Text);
        }

        [Test]
        public void PageToPrevious()
        {
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 20", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 21", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 10", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 11", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 45", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 21", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 10", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 20", Driver.GetGridCell(9, "Text").Text);

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Текст отзыва 291", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 300", Driver.GetGridCell(9, "Text").Text);
        }
    }
}