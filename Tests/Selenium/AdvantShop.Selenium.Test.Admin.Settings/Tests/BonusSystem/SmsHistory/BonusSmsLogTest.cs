using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.SmsHistory
{
    [TestFixture]
    public class BonusSmsLogTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Bonus.Card.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Bonus.SmsTemplate.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Bonus.SmsLog.csv"
            );
            InitializeService.BonusSystemActive();
            Init();

            GoToAdmin("smstemplates/smslog");
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
        public void SmsLogGrid()
        {
            VerifyAreEqual("999999", Driver.GetGridCell(0, "Phone", "log").Text, "sms number line 1");
            VerifyAreEqual("Test Message1", Driver.GetGridCell(0, "Body", "log").Text, "sms Body line 1");
            VerifyAreEqual("Send", Driver.GetGridCell(0, "State", "log").Text, "sms State line 1");
            VerifyAreEqual("20.04.2017 03:40:00", Driver.GetGridCell(0, "Created_Str", "log").Text,
                "sms CreatedStr line 1");

            VerifyAreEqual("999999", Driver.GetGridCell(1, "Phone", "log").Text, "sms number line 2");
            VerifyAreEqual("Test Message2", Driver.GetGridCell(1, "Body", "log").Text, "sms Body line 2");
            VerifyAreEqual("Send", Driver.GetGridCell(1, "State", "log").Text, "sms State line 1");
            VerifyAreEqual("19.04.2017 03:40:00", Driver.GetGridCell(1, "Created_Str", "log").Text,
                "sms CreatedStr line 2");
        }


        [Test]
        public void SmsSearchExistByText()
        {
            GoToAdmin("smstemplates/smslog");

            Driver.GridFilterSendKeys("Test Message10");

            VerifyAreEqual("888888", Driver.GetGridCell(0, "Phone", "log").Text, "sms number line 1");
            VerifyAreEqual("Test Message10", Driver.GetGridCell(0, "Body", "log").Text, "sms Body line 1");
            VerifyAreEqual("Send", Driver.GetGridCell(0, "State", "log").Text, "sms State line 1");
            VerifyAreEqual("11.04.2017 03:40:00", Driver.GetGridCell(0, "Created_Str", "log").Text,
                "sms CreatedStr line 1");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SmsSearchMuchSymbols()
        {
            GoToAdmin("smstemplates/smslog");

            Driver.GridFilterSendKeys(
                    "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SmsSearchInvalidSymbols()
        {
            GoToAdmin("smstemplates/smslog");

            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}