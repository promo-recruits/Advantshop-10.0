using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.Card
{
    [TestFixture]
    public class BonusSystemCardSearch : BaseSeleniumTest
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
        public void CardSearchExistByCardNum()
        {
            GoToAdmin("Cards");

            Driver.GridFilterSendKeys("530874");

            VerifyAreEqual("530874", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "search exist Card id");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }


        [Test]
        public void CardSearchExistByCustomer()
        {
            GoToAdmin("Cards");

            Driver.GridFilterSendKeys("FirstName3");

            VerifyAreEqual("LastName3 FirstName3", Driver.GetGridCell(0, "FIO").Text, "search exist fullname");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CardSearchMuchSymbols()
        {
            GoToAdmin("Cards");

            Driver.GridFilterSendKeys(
                    "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CardSearchInvalidSymbols()
        {
            GoToAdmin("Cards");

            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}