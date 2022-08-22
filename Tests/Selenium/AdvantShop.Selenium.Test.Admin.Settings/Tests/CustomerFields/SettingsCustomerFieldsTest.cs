using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields
{
    [TestFixture]
    public class SettingsCustomerFieldsTest : BaseSeleniumTest
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
        public void CustomerFieldsGridTest()
        {
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "name");
            VerifyAreEqual("Выбор", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text, "type");
            VerifyAreEqual("Список значений", Driver.GetGridCell(0, "HasValues", "CustomerFields").Text, "values");
            VerifyIsFalse(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "Required");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "SortOrder");
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "enabled");
            VerifyIsFalse(
                Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "show in Checkout");
            VerifyIsFalse(
                Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "show in Registration");

            VerifyAreEqual("Найдено записей: 150",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CustomerFieldsInplaceEnabledTest()
        {
            Driver.GridFilterSendKeys("Customer Field 1");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "pre check name");

            Driver.GetGridCell(0, "Enabled", "CustomerFields").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "inplace enabled 1");

            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field 1");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "name");

            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "inplace enabled 2");
        }

        [Test]
        public void CustomerFieldsInplaceShowInCheckoutTest()
        {
            Driver.GridFilterSendKeys("Customer Field 1");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "pre check name");

            Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields").Click();
            VerifyIsFalse(
                Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "inplace ShowInCheckout 1");

            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field 50");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 50", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "name");

            VerifyIsTrue(
                Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "inplace ShowInCheckout 50");
        }

        [Test]
        public void CustomerFieldsInplaceShowInRegistationTest()
        {
            Driver.GridFilterSendKeys("Customer Field 1");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "pre check name");
            VerifyIsFalse(
                Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "before inplace ShowInRegistation 1");

            Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields").Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "inplace ShowInRegistation 1");

            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field 50");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 50", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "name");

            VerifyIsTrue(
                Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "inplace ShowInClient 50");
        }

        [Test]
        public void CustomerFieldsInplaceRequiredTest()
        {
            Driver.GridFilterSendKeys("Customer Field 3");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 3", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "pre check name");

            Driver.GetGridCell(0, "Required", "CustomerFields").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "inplace Required 1");

            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field 3");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 3", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "name");

            VerifyIsTrue(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "inplace Required 2");
        }


        [Test]
        public void CustomerFieldsInplaceDisabledTest()
        {
            Driver.GridFilterSendKeys("Customer Field 21");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 21", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "pre check name");

            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "pre check inplace enabled");

            Driver.GetGridCell(0, "Enabled", "CustomerFields").Click();
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "inplace enabled 1");

            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field 21");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 21", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "name");

            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "inplace enabled 2");
        }

        [Test]
        public void CustomerFieldsInplaceSortTest()
        {
            Driver.GridFilterSendKeys("Customer Field 100");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 100", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "pre check name");
            VerifyAreEqual("100", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "pre check SortOrder");

            Driver.SendKeysGridCell("1", 0, "SortOrder", "CustomerFields");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field 100");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyAreEqual("Customer Field 100", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "name");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "SortOrder");

            //return default
            Driver.SendKeysGridCell("100", 0, "SortOrder", "CustomerFields");

            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
        }

        [Test]
        public void CustomerFieldzSelectDelete()
        {
            GoToAdmin("settingscustomers#?tab=customerFields");

            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "CustomerFields")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "CustomerFields")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Customer Field 2", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "CustomerFields")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "CustomerFields")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "CustomerFields")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Customer Field 5", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "selected 1 grid delete");
            VerifyAreEqual("Customer Field 6", Driver.GetGridCell(1, "Name", "CustomerFields").Text,
                "selected 2 grid delete");
            VerifyAreEqual("Customer Field 7", Driver.GetGridCell(2, "Name", "CustomerFields").Text,
                "selected 3 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Customer Field 15", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "selected all on page 1 grid delete");
            VerifyAreEqual("Customer Field 24", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("136", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");

            GoToAdmin("settingscustomers#?tab=customerFields");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all 2");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 2");
        }
    }
}