using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog.Currencies
{
    [TestFixture]
    public class SettingsCurrenciesTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Currencies);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\Catalog.Currency.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\Settings.Settings.csv"
            );
            Init();
            GoToAdmin("settingscatalog#?catalogTab=currency");
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
        public void CurrenciesGrid()
        {
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h3")).Text
                    .Contains("Валюты"), "currencies page h1");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "currencies page count");
            Driver.XPathContainsText("h3", "Валюты");
            VerifyAreEqual("TestCurrencyName1", Driver.GetGridCellText(0, "Name", "Currencies"),
                "currencies grid name");
            VerifyAreEqual("руб.1", Driver.GetGridCellText(0, "Symbol", "Currencies"), "currencies grid symbol");
            VerifyAreEqual("13.4564", Driver.GetGridCellText(0, "Rate", "Currencies"), "currencies grid Rate");
            VerifyAreEqual("R1", Driver.GetGridCellText(0, "Iso3", "Currencies"), "currencies grid Iso3");
            VerifyAreEqual("1", Driver.GetGridCellText(0, "NumIso3", "Currencies"), "currencies grid NumIso3");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsCodeBefore", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "currencies grid IsCodeBefore");
            VerifyIsTrue(Driver.GetGridCell(0, "RoundNumbers", "Currencies").Text.Contains("Не округлять"),
                "currencies grid RoundNumbers");
        }

        [Test]
        [Order(1)]
        public void GoToEdit()
        {
            Driver.GetGridCell(0, "_serviceColumn", "Currencies")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Редактирование валюты", Driver.FindElement(By.TagName("h2")).Text, "open pop edit up");

            Driver.XPathContainsText("button", "Отмена");
        }

        [Test]
        [Order(10)]
        public void SelectDelete()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.ScrollTo(By.TagName("footer"));

            IWebElement selectElem = Driver.FindElement(By.Name("DefaultCurrencyIso3"));
            SelectElement select = new SelectElement(selectElem);
            if (!select.SelectedOption.Text.Contains("TestCurrencyName99"))

            {
                select.SelectByText("TestCurrencyName99");
                Thread.Sleep(2000);
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
                Thread.Sleep(2000);
            }

            GoToAdmin("settingscatalog#?catalogTab=currency");
            //check delete cancel 
            Driver.GetGridCell(1, "_serviceColumn", "Currencies")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            //Driver.MouseFocus(driver, By.CssSelector(".original-header-page"));
            Driver.MouseFocus(By.CssSelector(".breadcrumb__link--admin"));
            VerifyAreEqual("TestCurrencyName10", Driver.GetGridCellText(1, "Name", "Currencies"),
                "1 grid cancel delete");

            //check delete
            Driver.GetGridCell(1, "_serviceColumn", "Currencies")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            //Driver.MouseFocus(driver, By.CssSelector(".original-header-page"));
            Driver.MouseFocus(By.CssSelector(".breadcrumb__link--admin"));
            VerifyAreEqual("TestCurrencyName100", Driver.GetGridCellText(1, "Name", "Currencies"), "1 grid delete");

            //check select 
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Currencies")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Currencies")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(3, "selectionRowHeaderCol", "Currencies")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(3, "selectionRowHeaderCol", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text, "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridCurrencies");
            Driver.XPathContainsText("h3", "Валюты");
            VerifyAreEqual("TestCurrencyName12", Driver.GetGridCellText(1, "Name", "Currencies"),
                "selected 2 grid delete");
            VerifyAreEqual("TestCurrencyName13", Driver.GetGridCellText(2, "Name", "Currencies"),
                "selected 3 grid delete");
            VerifyAreEqual("TestCurrencyName14", Driver.GetGridCellText(3, "Name", "Currencies"),
                "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridCurrencies");
            //to restoring cell contents after focus
            //Driver.MouseFocus(driver, By.CssSelector(".breadcrumb__link--admin")); //для данного грида не сбрасывает фокус ячейки
            //Driver.MouseFocus(driver, Driver.GetGridCell(1, "Name", "Currencies"));//сбрасывает фокус с нулевой ячейки, но перетягивает на себя. 
            Driver.FindElement(By.CssSelector(".tab-pane.active h3"))
                .Click(); //рабочий вариант, но хочется избежать кликов лишних
            VerifyAreEqual("TestCurrencyName20", Driver.GetGridCellText(0, "Name", "Currencies"),
                "selected all on page 2 grid delete");
            VerifyAreEqual("TestCurrencyName29", Driver.GetGridCellText(9, "Name", "Currencies"),
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("87",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridCurrencies");
            //to restoring cell contents after focus
            //Driver.MouseFocus(driver, By.CssSelector(".breadcrumb__link--admin"));
            Driver.FindElement(By.CssSelector(".tab-pane.active h3"))
                .Click(); //рабочий вариант, но хочется избежать кликов лишних
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]")).Text
                    .Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("TestCurrencyName99", Driver.GetGridCellText(0, "Name", "Currencies"), "grid after delete");
            GoToAdmin("settingscatalog#?catalogTab=currency");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]")).Text
                    .Contains("Ни одной записи не найдено"), "delete all refresh");
            VerifyAreEqual("TestCurrencyName99", Driver.GetGridCellText(0, "Name", "Currencies"),
                "grid after delete refresh");
        }
    }
}