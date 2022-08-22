using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.Card
{
    [TestFixture]
    public class BonusSystemCardPage : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Grid\\Bonus.Card.csv"
            );
            InitializeService.BonusSystemActive();
            Init();

            GoToAdmin("cards");
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
        public void CardPage()
        {
            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 1 line 1");
            VerifyAreEqual("530810", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("530811", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 2 line 1");
            VerifyAreEqual("530820", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("530821", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 3 line 1");
            VerifyAreEqual("530830", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("530831", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 4 line 1");
            VerifyAreEqual("530840", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("530841", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 5 line 1");
            VerifyAreEqual("530850", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("530851", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 6 line 1");
            VerifyAreEqual("530860", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 1 line 1");
            VerifyAreEqual("530810", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 1 line 10");
        }

        [Test]
        public void CardPageToPrevious()
        {
            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 1 line 1");
            VerifyAreEqual("530810", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("530811", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 2 line 1");
            VerifyAreEqual("530820", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("530821", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 3 line 1");
            VerifyAreEqual("530830", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("530811", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 2 line 1");
            VerifyAreEqual("530820", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 1 line 1");
            VerifyAreEqual("530810", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("530911", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "last page line 1");
            VerifyAreEqual("530920", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "last page line 10");
        }
    }
}