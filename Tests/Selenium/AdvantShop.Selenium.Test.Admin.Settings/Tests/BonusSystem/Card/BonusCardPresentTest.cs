using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.Card
{
    [TestFixture]
    public class BonusSystemCardPresent : BaseSeleniumTest
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
        public void CardPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "present line 1");
            VerifyAreEqual("530810", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "present line 10");

            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "present line 1");
            VerifyAreEqual("530820", Driver.GetGridCell(19, "CardNumber").FindElement(By.TagName("a")).Text,
                "present line 20");
        }
    }
}