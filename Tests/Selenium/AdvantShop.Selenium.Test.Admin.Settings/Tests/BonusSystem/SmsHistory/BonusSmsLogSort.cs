using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.SmsHistory
{
    [TestFixture]
    public class BonusSmsLogSort : BaseSeleniumTest
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
        public void ByStatus()
        {
            Driver.GetGridCell(-1, "State", "log").Click();
            VerifyAreEqual("Get", Driver.GetGridCell(0, "State", "log").Text, "sort by State asc 1");
            VerifyAreEqual("Get", Driver.GetGridCell(9, "State", "log").Text, "sort by State asc 10");

            Driver.GetGridCell(-1, "State", "log").Click();
            VerifyAreEqual("Send", Driver.GetGridCell(0, "State", "log").Text, "sort by State desc 1");
            VerifyAreEqual("Send", Driver.GetGridCell(9, "State", "log").Text, "sort by State desc 10");
        }


        [Test]
        public void ByCreatedDate()
        {
            Driver.GetGridCell(-1, "Created_Str", "log").Click();
            VerifyAreEqual("01.03.2017 03:40:00", Driver.GetGridCell(0, "Created_Str", "log").Text,
                "sort by created date asc 1");
            VerifyAreEqual("10.03.2017 03:40:00", Driver.GetGridCell(9, "Created_Str", "log").Text,
                "sort by created date asc 10");

            Driver.GetGridCell(-1, "Created_Str", "log").Click();
            VerifyAreEqual("20.04.2017 03:40:00", Driver.GetGridCell(0, "Created_Str", "log").Text,
                "sort by created date desc 1");
            VerifyAreEqual("11.04.2017 03:40:00", Driver.GetGridCell(9, "Created_Str", "log").Text,
                "sort by created date desc 10");
        }
    }
}