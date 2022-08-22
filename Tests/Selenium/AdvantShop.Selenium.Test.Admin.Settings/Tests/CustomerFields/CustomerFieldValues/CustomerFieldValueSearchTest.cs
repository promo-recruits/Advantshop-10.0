using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields.CustomerFieldValues
{
    [TestFixture]
    public class SettingsCustomerFieldValueSearchTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\CustomerFieldValues\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\CustomerFieldValues\\Customers.Customer.csv",
                "data\\Admin\\Settings\\CustomerFieldValues\\Customers.CustomerField.csv",
                "data\\Admin\\Settings\\CustomerFieldValues\\Customers.CustomerFieldValue.csv"
            );

            Init();

            GoToAdmin("settingscustomers#?tab=customerFields");
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
        public void CustomerFieldValueSearchExistName()
        {
            Refresh();

            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();

            Driver.GridFilterSendKeys("Value 111");

            Driver.XPathContainsText("h1", "Значения поля \"Customer Field 1\"");

            VerifyAreEqual("Value 111", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text,
                "search exist name");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CustomerFieldValueSearchNotExist()
        {
            Refresh();

            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();

            Driver.GridFilterSendKeys("test value 5000");

            Driver.XPathContainsText("h1", "Значения поля \"Customer Field 1\"");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist name");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CustomerFieldValueSearchMuchSymbols()
        {
            Refresh();

            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();

            Driver.GridFilterSendKeys(
                    "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");

            Driver.XPathContainsText("h1", "Значения поля \"Customer Field 1\"");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CustomerFieldValueSearchInvalidSymbols()
        {
            Refresh();

            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();

            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");

            Driver.XPathContainsText("h1", "Значения поля \"Customer Field 1\"");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CustomerFieldValueFilterName()
        {
            Refresh();

            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();

            //check filter name
            Functions.GridFilterSet(Driver, BaseUrl, name: "Value");

            //search by not exist name
            Driver.SetGridFilterValue("Value", "123123123 name test 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Value", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Value", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist name
            Driver.SetGridFilterValue("Value", "Value 2");

            VerifyAreEqual("Value 2", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "Value");

            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Value");
            VerifyAreEqual("Найдено записей: 129",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter name deleting 1");

            Refresh();
            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();

            VerifyAreEqual("Найдено записей: 129",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter name deleting 2");
        }
    }
}