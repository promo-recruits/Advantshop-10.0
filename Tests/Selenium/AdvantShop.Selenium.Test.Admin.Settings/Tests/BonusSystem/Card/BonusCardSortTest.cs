using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.Card
{
    [TestFixture]
    public class BonusSystemCardSort : BaseSeleniumTest
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
        public void ByNum()
        {
            Driver.GetGridCell(-1, "CardNumber").Click();
            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "sort card num asc 1");
            VerifyAreEqual("530810", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "sort card num asc 10");

            Driver.GetGridCell(-1, "CardNumber").Click();
            VerifyAreEqual("530920", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "sort card num desc 1");
            VerifyAreEqual("530911", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "sort card num desc 10");
        }

        [Test]
        public void ByFIO()
        {
            Driver.GetGridCell(-1, "FIO").Click();
            VerifyAreEqual("LastName1 FirstName1", Driver.GetGridCell(0, "FIO").Text, "sort card fio asc 1");
            VerifyAreEqual("LastName107 FirstName107", Driver.GetGridCell(9, "FIO").Text, "sort card fio asc 10");

            Driver.GetGridCell(-1, "FIO").Click();
            VerifyAreEqual("LastName99 FirstName99", Driver.GetGridCell(0, "FIO").Text, "sort card fio desc 1");
            VerifyAreEqual("LastName90 FirstName90", Driver.GetGridCell(9, "FIO").Text, "sort card fio desc 10");
        }

        [Test]
        public void ByGrade()
        {
            Driver.GetGridCell(-1, "GradeName").Click();
            VerifyAreEqual("Бронзовый", Driver.GetGridCell(0, "GradeName").Text, "sort card grade asc 1");
            VerifyAreEqual("Бронзовый", Driver.GetGridCell(9, "GradeName").Text, "sort card grade asc 10");
            VerifyIsFalse(Driver.GetGridCell(0, "CardNumber").Text.Equals(Driver.GetGridCell(9, "CardNumber").Text),
                "sort card grade asc diff fields");

            Driver.GetGridCell(-1, "GradeName").Click();
            VerifyAreEqual("Серебряный", Driver.GetGridCell(0, "GradeName").Text, "sort card grade desc 1");
            VerifyAreEqual("Серебряный", Driver.GetGridCell(9, "GradeName").Text, "sort card grade desc 10");
            VerifyIsFalse(Driver.GetGridCell(0, "CardNumber").Text.Equals(Driver.GetGridCell(9, "CardNumber").Text),
                "sort card grade desc diff fields");
        }

        [Test]
        public void ByPercent()
        {
            Driver.GetGridCell(-1, "GradePersent").Click();
            VerifyAreEqual("3", Driver.GetGridCell(0, "GradePersent").Text, "sort card percent asc 1");
            VerifyAreEqual("3", Driver.GetGridCell(9, "GradePersent").Text, "sort card percent asc 10");
            VerifyIsFalse(Driver.GetGridCell(0, "CardNumber").Text.Equals(Driver.GetGridCell(9, "CardNumber").Text),
                "sort card percent asc diff fields");

            Driver.GetGridCell(-1, "GradePersent").Click();
            VerifyAreEqual("30", Driver.GetGridCell(0, "GradePersent").Text, "sort card percent desc 1");
            VerifyAreEqual("30", Driver.GetGridCell(9, "GradePersent").Text, "sort card percent desc 10");
            VerifyIsFalse(Driver.GetGridCell(0, "CardNumber").Text.Equals(Driver.GetGridCell(9, "CardNumber").Text),
                "sort card percent desc diff fields");
        }

        [Test]
        public void ByCreatedDate()
        {
            Driver.GetGridCell(-1, "CreatedFormatted").Click();
            VerifyAreEqual("22.12.2016 15:40", Driver.GetGridCell(0, "CreatedFormatted").Text,
                "sort card created date asc 1");
            VerifyAreEqual("31.12.2016 15:40", Driver.GetGridCell(9, "CreatedFormatted").Text,
                "sort card created date asc 10");

            Driver.GetGridCell(-1, "CreatedFormatted").Click();
            VerifyAreEqual("20.04.2017 15:40", Driver.GetGridCell(0, "CreatedFormatted").Text,
                "sort card created date desc 1");
            VerifyAreEqual("11.04.2017 15:40", Driver.GetGridCell(9, "CreatedFormatted").Text,
                "sort card created date desc 10");
        }
    }
}