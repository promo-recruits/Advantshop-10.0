using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.SmsHistory
{
    [TestFixture]
    public class BonusSmsLogFilter : BaseSeleniumTest
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
        public void FilterPhone()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "Phone");

            //search by not exist card
            Driver.SetGridFilterValue("Phone", "53624351");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Phone", "111111111122222222");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Phone", "####@@@@@@&&&&&&");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist card
            Driver.SetGridFilterValue("Phone", "111111");
            Driver.Blur();

            VerifyAreEqual("111111", Driver.GetGridCell(0, "Phone", "log").Text, "sms number line 1");
            VerifyAreEqual("Test Message46", Driver.GetGridCell(0, "Body", "log").Text, "sms Body line 1");
            VerifyAreEqual("Get", Driver.GetGridCell(0, "State", "log").Text, "sms State line 1");
            VerifyAreEqual("06.03.2017 03:40:00", Driver.GetGridCell(0, "Created_Str", "log").Text,
                "sms CreatedStr line 1");

            Functions.GridFilterClose(Driver, BaseUrl, name: "Phone");
            VerifyAreEqual("Найдено записей: 51",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter card num deleting 1");
            Refresh();
            VerifyAreEqual("Найдено записей: 51",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter card num deleting 1");
        }

        [Test]
        public void FilterText()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "Body");

            //search by not exist Contact
            Driver.SetGridFilterValue("Body", "123123123 name contact 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Body", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Body", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist FIO
            Driver.SetGridFilterValue("Body", "Test Message10");

            VerifyAreEqual("888888", Driver.GetGridCell(0, "Phone", "log").Text, "sms number line 1");
            VerifyAreEqual("Test Message10", Driver.GetGridCell(0, "Body", "log").Text, "sms Body line 1");
            VerifyAreEqual("Send", Driver.GetGridCell(0, "State", "log").Text, "sms State line 1");
            VerifyAreEqual("11.04.2017 03:40:00", Driver.GetGridCell(0, "Created_Str", "log").Text,
                "sms CreatedStr line 1");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Body");
            VerifyAreEqual("Найдено записей: 51",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter FIO deleting 1");
        }
    }
}