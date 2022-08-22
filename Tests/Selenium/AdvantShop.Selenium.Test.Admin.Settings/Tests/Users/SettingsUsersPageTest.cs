using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Users
{
    [TestFixture]
    public class SettingsUsersPageTest : BaseSeleniumTest
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

            GoToAdmin("settings/userssettings");
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
        public void SettingsUsersPage()
        {
            VerifyAreEqual("testfirstname1 testlastname1", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 1 line 1");
            VerifyAreEqual("testfirstname10 testlastname10", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("testfirstname11 testlastname11", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 2 line 1");
            VerifyAreEqual("testfirstname20 testlastname20", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("testfirstname21 testlastname21", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 3 line 1");
            VerifyAreEqual("testfirstname30 testlastname30", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("testfirstname31 testlastname31", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 4 line 1");
            VerifyAreEqual("testfirstname40 testlastname40", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("testfirstname41 testlastname41", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 5 line 1");
            VerifyAreEqual("testfirstname50 testlastname50", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("testfirstname51 testlastname51", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 6 line 1");
            VerifyAreEqual("testfirstname60 testlastname60", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("testfirstname1 testlastname1", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 1 line 1");
            VerifyAreEqual("testfirstname10 testlastname10", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 1 line 10");
        }

        [Test]
        public void SettingsUsersPageToPrevious()
        {
            VerifyAreEqual("testfirstname1 testlastname1", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 1 line 1");
            VerifyAreEqual("testfirstname10 testlastname10", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("testfirstname11 testlastname11", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 2 line 1");
            VerifyAreEqual("testfirstname20 testlastname20", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("testfirstname21 testlastname21", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 3 line 1");
            VerifyAreEqual("testfirstname30 testlastname30", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("testfirstname11 testlastname11", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 2 line 1");
            VerifyAreEqual("testfirstname20 testlastname20", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("testfirstname1 testlastname1", Driver.GetGridCell(0, "FullName", "Users").Text,
                "page 1 line 1");
            VerifyAreEqual("testfirstname10 testlastname10", Driver.GetGridCell(9, "FullName", "Users").Text,
                "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("admin testlastname221", Driver.GetGridCell(0, "FullName", "Users").Text,
                "last page line 1");
        }
    }
}