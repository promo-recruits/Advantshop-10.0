using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields.CustomerFieldValues
{
    [TestFixture]
    public class SettingsCustomerFieldValueTest : BaseSeleniumTest
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
        public void CustomerFieldValuesGridTest()
        {
            VerifyIsTrue(Driver.GetGridCell(0, "Name", "CustomerFields").FindElements(By.TagName("span")).Count > 0,
                "Customer Fields grid");
            VerifyIsFalse(Driver.FindElements(By.XPath("//span[contains(text(), 'Value 1')]")).Count > 0,
                "no Customer Field Values grid");

            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();

            VerifyIsTrue(Driver.PageSource.Contains("Значения поля \"Customer Field 1\""), "h1 values page");

            VerifyIsFalse(Driver.FindElements(By.XPath("//span[contains(text(), 'Customer Field 1')]")).Count > 0,
                "no Customer Fields grid");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Value", "CustomerFieldValues").FindElements(By.TagName("span")).Count > 0,
                "Customer Field Values grid");

            VerifyAreEqual("Value 1", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "Value");
            Driver.XPathContainsText("h1", "Значения поля \"Customer Field 1\"");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "CustomerFieldValues").Text, "SortOrder");

            VerifyAreEqual("Найдено записей: 140",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            Driver.FindElement(By.LinkText("Вернуться к списку полей")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "CustomerFields").FindElements(By.TagName("span")).Count > 0,
                "Customer Fields grid back");
            VerifyIsFalse(Driver.FindElements(By.XPath("//span[contains(text(), 'Value 1')]")).Count > 0,
                "no Customer Field Values grid back");
        }

        [Test]
        public void CustomerFieldValuesInplaceTest()
        {
            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();

            Driver.GridFilterSendKeys("Value 50");
            Driver.XPathContainsText("h1", "Значения поля \"Customer Field 1\"");

            VerifyAreEqual("50", Driver.GetGridCell(0, "SortOrder", "CustomerFieldValues").Text, "before edit");

            Driver.SendKeysGridCell("200", 0, "SortOrder", "CustomerFieldValues");

            Driver.XPathContainsText("h1", "Значения поля \"Customer Field 1\"");

            Refresh();

            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();

            Driver.GridFilterSendKeys("Value 50");

            Driver.XPathContainsText("h1", "Значения поля \"Customer Field 1\"");

            VerifyAreEqual("200", Driver.GetGridCell(0, "SortOrder", "CustomerFieldValues").Text, "edited inplace");

            //back
            Driver.SendKeysGridCell("50", 0, "SortOrder", "CustomerFieldValues");

            Driver.XPathContainsText("h1", "Значения поля \"Customer Field 1\"");
        }

        [Test]
        public void CustomerFieldValuezSelectDelete()
        {
            Refresh();
            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();
            Driver.WaitForElem(By.CssSelector("[grid-unique-id=\"gridCustomerFieldValues\"]"));

            VerifyAreEqual("Value 1", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "open Values page");

            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "CustomerFieldValues")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Value 1", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text,
                "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "CustomerFieldValues")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Value 2", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "CustomerFieldValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "CustomerFieldValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "CustomerFieldValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "CustomerFieldValues")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "CustomerFieldValues")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "CustomerFieldValues")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Value 5", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text,
                "selected 1 grid delete");
            VerifyAreEqual("Value 6", Driver.GetGridCell(1, "Value", "CustomerFieldValues").Text,
                "selected 2 grid delete");
            VerifyAreEqual("Value 7", Driver.GetGridCell(2, "Value", "CustomerFieldValues").Text,
                "selected 3 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "CustomerFieldValues")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "CustomerFieldValues")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Value 15", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text,
                "selected all on page 1 grid delete");
            VerifyAreEqual("Value 24", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("126", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            Thread.Sleep(100);
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "CustomerFieldValues")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "CustomerFieldValues")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            Refresh();
            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();
            Driver.WaitForElem(By.CssSelector("[grid-unique-id=\"gridCustomerFieldValues\"]"));
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 1");

            GoToAdmin("settingscustomers#?tab=customerFields");
            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();
            Driver.WaitForElem(By.CssSelector("[grid-unique-id=\"gridCustomerFieldValues\"]"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all 2");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 2");
        }
    }
}