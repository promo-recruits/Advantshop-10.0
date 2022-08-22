using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields
{
    [TestFixture]
    public class SettingsCustomerFieldsSearchTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\CustomerFields\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\CustomerFields\\Customers.Customer.csv",
                "data\\Admin\\Settings\\CustomerFields\\Customers.CustomerField.csv",
                "data\\Admin\\Settings\\CustomerFields\\Customers.CustomerFieldValue.csv"
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
        public void SearchExist()
        {
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field 111");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyAreEqual("Customer Field 111", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "search exist settings");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchNotExist()
        {
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("555 Field");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist");
        }

        [Test]
        public void SearchMuchSymbols()
        {
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys(
                    "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
        }

        [Test]
        public void SearchInvalidSymbols()
        {
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
        }
    }
}