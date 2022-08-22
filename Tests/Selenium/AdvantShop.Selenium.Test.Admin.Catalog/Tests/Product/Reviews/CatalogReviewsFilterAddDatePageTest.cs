using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsFilterAddDatePageTest : BaseSeleniumTest
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

            Functions.GridFilterSet(Driver, BaseUrl, name: "AddDateFormatted");
            Driver.SetGridFilterRange("AddDateFormatted", "01.01.2013 00:00", "31.12.2013 00:00");
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
            VerifyAreEqual("Текст отзыва 258", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 249", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Текст отзыва 248", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 239", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Текст отзыва 238", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 229", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("Текст отзыва 228", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 219", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Текст отзыва 218", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 209", Driver.GetGridCell(9, "Text").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Текст отзыва 258", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 249", Driver.GetGridCell(9, "Text").Text);
        }

        [Test]
        public void PageToPrevious()
        {
            VerifyAreEqual("Текст отзыва 258", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 249", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 248", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 239", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 238", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 229", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 248", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 239", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 258", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 249", Driver.GetGridCell(9, "Text").Text);

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Текст отзыва 181", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 190", Driver.GetGridCell(9, "Text").Text);
        }
    }
}