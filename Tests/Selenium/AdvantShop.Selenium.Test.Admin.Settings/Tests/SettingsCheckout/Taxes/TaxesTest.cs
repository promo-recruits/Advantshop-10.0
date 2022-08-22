using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Taxes
{
    [TestFixture]
    public class SettingsTaxesTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Taxes);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Taxes\\Catalog.Tax.csv",
                "data\\Admin\\Settings\\Taxes\\Settings.Settings.csv"
            );

            Init();

            GoToAdmin("settingscheckout#?checkoutTab=taxes");
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
        [Order(1)]
        public void GoToEditByName()
        {
            Driver.GetGridCell(0, "Name", "Taxes").FindElement(By.TagName("span")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Редактирование налога", Driver.FindElement(By.TagName("h2")).Text, "open pop edit up");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Отмена')]")).Click();
        }


        [Test]
        [Order(1)]
        public void GoToEditByServiceCol()
        {
            Driver.GetGridCell(0, "_serviceColumn", "Taxes")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Редактирование налога", Driver.FindElement(By.TagName("h2")).Text, "open pop edit up");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Отмена')]")).Click();
        }

        [Test]
        [Order(1)]
        public void InplaceEnabled()
        {
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "pre check Enabled");

            Driver.GetGridCell(1, "Enabled", "Taxes").Click();
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "Enabled inplace");

            Refresh();

            VerifyIsFalse(Driver.GetGridCell(1, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "Enabled inplace after refreshing");
        }

        [Test]
        [Order(1)]
        public void InplaceIsDefault()
        {
            VerifyIsFalse(Driver.GetGridCell(2, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected,
                "pre check IsDefault no");

            Driver.GetGridCell(2, "IsDefault", "Taxes").Click();
            //VerifyIsTrue(Driver.GetGridCell(2, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected, "IsDefault inplace");

            Refresh();

            VerifyIsTrue(Driver.GetGridCell(2, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected,
                "IsDefault inplace after refreshing");
            VerifyIsFalse(Driver.GetGridCell(0, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected,
                "prev IsDefault no");

            //back default
            Driver.GetGridCell(0, "IsDefault", "Taxes").Click();
            Refresh();
        }

        [Test]
        [Order(10)]
        public void SelectDelete()
        {
            //check delete cancel 
            Driver.GetGridCell(1, "_serviceColumn", "Taxes")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Tax 10", Driver.GetGridCell(1, "Name", "Taxes").Text, "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(1, "_serviceColumn", "Taxes")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Tax 100", Driver.GetGridCell(1, "Name", "Taxes").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Taxes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Taxes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(3, "selectionRowHeaderCol", "Taxes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Taxes")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected 2 grid"); //1 default
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Taxes")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(3, "selectionRowHeaderCol", "Taxes")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text, "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridTaxes");
            VerifyAreEqual("Tax 103", Driver.GetGridCell(1, "Name", "Taxes").Text, "selected 2 grid delete");
            VerifyAreEqual("Tax 104", Driver.GetGridCell(2, "Name", "Taxes").Text, "selected 3 grid delete");
            VerifyAreEqual("Tax 105", Driver.GetGridCell(3, "Name", "Taxes").Text, "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Taxes")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Taxes")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridTaxes");
            VerifyAreEqual("Tax 15", Driver.GetGridCell(1, "Name", "Taxes").Text, "selected all on page 2 grid delete");
            VerifyAreEqual("Tax 22", Driver.GetGridCell(9, "Name", "Taxes").Text,
                "selected all on page 10 grid delete");
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "delete all on page except default");

            //check select all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("94",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "Taxes")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "Taxes")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridTaxes");

            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "delete all except default");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");

            GoToAdmin("settingscheckout#?checkoutTab=taxes");
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text,
                "delete all except default after refreshing");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting after refreshing");
        }
    }
}