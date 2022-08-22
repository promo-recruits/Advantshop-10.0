using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields
{
    [TestFixture]
    public class SettingsCustomerFieldsFilterTest : BaseMultiSeleniumTest
    {
        [SetUp]
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

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void FilterName()
        {
            //check filter full name
            Functions.GridFilterSet(Driver, BaseUrl, name: "Name");

            //search by not exist name
            Driver.SetGridFilterValue("Name", "123123123 field test 3");
            
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Name", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Name", "########@@@@@@@@&&&&&&&******,,,,..");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist name
            Driver.SetGridFilterValue("Name", "Customer Field 2");

            VerifyAreEqual("Customer Field 2", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "Name");

            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter name");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter name after deleting");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Name");
            VerifyAreEqual("Найдено записей: 139",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter name deleting 1");

            Refresh();

            VerifyAreEqual("Найдено записей: 139",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter name deleting 2");
        }

        [Test]
        public void FilterEnabled()
        {
            //check filter enabled
            Functions.GridFilterSet(Driver, BaseUrl, name: "Enabled");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Активные\"]")).Click();

            VerifyAreEqual("Найдено записей: 130",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter enabled");

            VerifyAreEqual("Customer Field 21", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "filter enabled 1");
            VerifyAreEqual("Customer Field 30", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "filter enabled 10");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter enabled select 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "Enabled", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter enabled select 10");

            //check filter disabled
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Неактивные\"]")).Click();

            VerifyAreEqual("Найдено записей: 20",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter disabled");

            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "filter disabled 1");
            VerifyAreEqual("Customer Field 10", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "filter disabled 10");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Enabled", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter disabled select 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "Enabled", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter disabled select 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Enabled");
            VerifyAreEqual("Найдено записей: 130",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter enabled after deleting 1");

            Refresh();

            VerifyAreEqual("Найдено записей: 130",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter enabled after deleting 2");
        }

        [Test]
        public void FilterRequired()
        {
            //check filter Required
            Functions.GridFilterSet(Driver, BaseUrl, name: "Required");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Да\"]")).Click();

            VerifyAreEqual("Найдено записей: 50",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Required");

            VerifyAreEqual("Customer Field 101", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "filter Required 1");
            VerifyAreEqual("Customer Field 110", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "filter Required 10");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Required", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter Required select 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "Required", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter Required select 10");

            //check filter no Required
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();

            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter no Required");

            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "filter no Required 1");
            VerifyAreEqual("Customer Field 10", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "filter no Required 10");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Required", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter no Required select 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "Required", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter no Required select 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Required");
            VerifyAreEqual("Найдено записей: 50",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Required after deleting 1");

            Refresh();

            VerifyAreEqual("Найдено записей: 50",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Required after deleting 2");
        }

        [Test]
        public void FilterType()
        {
            //check filter enabled
            Functions.GridFilterSet(Driver, BaseUrl, name: "FieldTypeFormatted");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Выбор\"]")).Click();

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Type1");

            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "filter Type1 line 1");
            VerifyAreEqual("Выбор", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text,
                "filter Type2 select");

            //check filter disabled
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Текстовое поле\"]")).Click();

            VerifyAreEqual("Найдено записей: 17",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Type2");

            VerifyAreEqual("Customer Field 2", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "filter Type2 line 1");
            VerifyAreEqual("Текстовое поле", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text,
                "filter Type2 select");

            //check filter type 3
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Числовое поле\"]")).Click();

            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Type3");

            VerifyAreEqual("Customer Field 3", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "filter Type3 line 1");
            VerifyAreEqual("Числовое поле", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text,
                "filter Type3 select");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "FieldTypeFormatted");
            VerifyAreEqual("Найдено записей: 50",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter enabled after deleting 1");

            Refresh();

            VerifyAreEqual("Найдено записей: 50",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter enabled after deleting 2");
        }

        [Test]
        public void FilterShowInRegistration()
        {
            //check filter ShowInRegistration
            Functions.GridFilterSet(Driver, BaseUrl, name: "ShowInRegistration");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();

            VerifyAreEqual("Найдено записей: 39",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ShowInRegistration");

            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "filter ShowInRegistration 1");
            VerifyAreEqual("Customer Field 10", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "filter ShowInRegistration 10");
            VerifyIsFalse(
                Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter ShowInRegistration select 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "ShowInRegistration", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter ShowInRegistration select 10");

            //check filter no ShowInRegistration
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Да\"]")).Click();

            VerifyAreEqual("Найдено записей: 111",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter no ShowInRegistration");

            VerifyAreEqual("Customer Field 40", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "filter no ShowInRegistration 1");
            VerifyAreEqual("Customer Field 49", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "filter no ShowInRegistration 10");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter no ShowInRegistration select 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "ShowInRegistration", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter no ShowInRegistration select 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "ShowInRegistration");
            VerifyAreEqual("Найдено записей: 39",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ShowInRegistration after deleting 1");

            Refresh();

            VerifyAreEqual("Найдено записей: 39",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ShowInRegistration after deleting 2");
        }

        [Test]
        public void FilterShowInCheckout()
        {
            //check filter ShowInCheckout
            Functions.GridFilterSet(Driver, BaseUrl, name: "ShowInCheckout");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();

            VerifyAreEqual("Найдено записей: 90",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ShowInCheckout");

            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "filter ShowInCheckout 1");
            VerifyAreEqual("Customer Field 10", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "filter ShowInCheckout 10");
            VerifyIsFalse(
                Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter ShowInCheckout select 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "ShowInCheckout", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter ShowInCheckout select 10");

            //check filter no ShowInCheckout
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Да\"]")).Click();

            VerifyAreEqual("Найдено записей: 60",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter no ShowInCheckout");

            VerifyAreEqual("Customer Field 40", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "filter no ShowInCheckout 1");
            VerifyAreEqual("Customer Field 49", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "filter no ShowInCheckout 10");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter no ShowInCheckout select 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "ShowInCheckout", "CustomerFields")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter no ShowInCheckout select 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "ShowInCheckout");
            VerifyAreEqual("Найдено записей: 90",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ShowInCheckout after deleting 1");

            Refresh();

            VerifyAreEqual("Найдено записей: 90",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ShowInClient after deleting 2");
        }
    }
}