using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog.Currencies
{
    [TestFixture]
    public class SettingsCurrenciesFiltersTest : BaseMultiSeleniumTest
    {
        [SetUp]
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
            //check filter 
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "Name", gridId: "gridCurrencies");

            //search by not exist 
            Driver.SetGridFilterValue("Name", "currency name test 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Name", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Name", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist 
            Driver.SetGridFilterValue("Name", "TestCurrencyName1");
            VerifyAreEqual("TestCurrencyName1", Driver.GetGridCell(0, "Name", "Currencies").Text, "currency Name");

            VerifyAreEqual("Найдено записей: 13",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridCurrencies");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("Найдено записей: 88",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 1");

            GoToAdmin("settingscatalog#?catalogTab=currency");
            VerifyAreEqual("Найдено записей: 88",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 2");
        }

        [Test]
        public void FilterIso3()
        {
            //check filter 
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "Iso3", gridId: "gridCurrencies");

            //search by not exist 
            Driver.SetGridFilterValue("Iso3", "currency Iso3 test 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Iso3", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Iso3", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist 
            Driver.SetGridFilterValue("Iso3", "U");
            VerifyAreEqual("U1", Driver.GetGridCell(0, "Iso3", "Currencies").Text, "currency 1 Iso3");
            VerifyAreEqual("U2", Driver.GetGridCell(1, "Iso3", "Currencies").Text, "currency 2 Iso3");

            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridCurrencies");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Iso3");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 1");

            GoToAdmin("settingscatalog#?catalogTab=currency");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 2");
        }
    }
}