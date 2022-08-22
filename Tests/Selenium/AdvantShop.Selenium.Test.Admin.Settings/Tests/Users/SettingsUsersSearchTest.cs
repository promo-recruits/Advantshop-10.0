using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Users
{
    [TestFixture]
    public class SettingsUsersSearchTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Users\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\Users\\Customers.Customer.csv",
                "data\\Admin\\Settings\\Users\\Customers.Departments.csv",
                "data\\Admin\\Settings\\Users\\Customers.Managers.csv",
                "data\\Admin\\Settings\\Users\\Customers.ManagerRole.csv",
                "data\\Admin\\Settings\\Users\\Customers.ManagerRolesMap.csv"
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
        public void SettingsUsersSearchExistName()
        {
            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers", "testfirstname111");
            Driver.XPathContainsText("h1", "Сотрудники");

            VerifyAreEqual("testfirstname111 testlastname111", Driver.GetGridCell(0, "FullName", "Users").Text,
                "search exist name");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SettingsUsersSearchExistEmail()
        {
            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers", "testmail@mail.ru135");
            Driver.XPathContainsText("h1", "Сотрудники");

            VerifyAreEqual("testmail@mail.ru135", Driver.GetGridCell(0, "Email", "Users").Text, "search exist email");
            VerifyAreEqual("testfirstname135 testlastname135", Driver.GetGridCell(0, "FullName", "Users").Text,
                "search exist email's name");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SettingsUsersSearchNotExist()
        {
            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers", "testfirstname111222222222222");
            Driver.XPathContainsText("h1", "Сотрудники");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist name");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SettingsUsersSearchMuchSymbols()
        {
            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers",
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Driver.XPathContainsText("h1", "Сотрудники");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SettingsUsersSearchInvalidSymbols()
        {
            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers", "########@@@@@@@@&&&&&&&******,,,,..");
            Driver.XPathContainsText("h1", "Сотрудники");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}