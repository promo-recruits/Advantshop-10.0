using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogRewiewsSort : BaseSeleniumTest
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
        public void Sort()
        {
            GoToAdmin("reviews");

            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();

            //check sort by name
            Driver.GetGridCell(-1, "Name").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName1"));
            VerifyIsTrue(Driver.GetGridCell(99, "Name").Text.Contains("CustomerName2"));

            Driver.GetGridCell(-1, "Name").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName3"));
            VerifyIsTrue(Driver.GetGridCell(99, "Name").Text.Contains("CustomerName3"));

            //sort by text
            Driver.GetGridCell(-1, "Text").Click();
            VerifyAreEqual("Текст отзыва 1", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 189", Driver.GetGridCell(99, "Text").Text);

            Driver.GetGridCell(-1, "Text").Click();
            VerifyAreEqual("Текст отзыва 99", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 28", Driver.GetGridCell(99, "Text").Text);

            //sort by add date
            Driver.GetGridCell(-1, "AddDateFormatted").Click();
            VerifyAreEqual("26.07.2012 14:22", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyAreEqual("26.07.2013 14:22", Driver.GetGridCell(99, "AddDateFormatted").Text);

            Driver.GetGridCell(-1, "AddDateFormatted").Click();
            VerifyAreEqual("26.07.2015 14:22", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyAreEqual("04.09.2013 14:22", Driver.GetGridCell(99, "AddDateFormatted").Text);

            //sort by checked
            Driver.GetGridCell(-1, "Checked").Click();
            VerifyIsFalse(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(99, "Checked").FindElement(By.TagName("input")).Selected);

            Driver.GetGridCell(-1, "Checked").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(99, "Checked").FindElement(By.TagName("input")).Selected);
        }
    }
}