using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Taxes
{
    [TestFixture]
    public class SettingsTaxesFilterEnabledTest : BaseMultiSeleniumTest
    {
        [SetUp]
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

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void FilterEnabled()
        {
            if (!Driver.GetGridCell(0, "IsDefault", "Taxes").FindElement(By.TagName("input")).Selected)
            {
                Driver.GetGridCell(0, "IsDefault", "Taxes").Click();
                Thread.Sleep(1000);
            }

            //check filter disabled
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "Enabled", gridId: "gridTaxes");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Неактивные\"]")).Click();
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();
            VerifyAreEqual("Найдено записей: 65",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter disabled");

            VerifyAreEqual("Tax 100", Driver.GetGridCell(0, "Name", "Taxes").Text, "Name filter disabled 1");
            VerifyAreEqual("Tax 44", Driver.GetGridCell(9, "Name", "Taxes").Text, "Name filter disabled 10");

            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "select filter disabled 1");
            VerifyIsFalse(Driver.GetGridCell(9, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "select filter disabled 10");

            //check filter enabled
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Активные\"]")).Click();
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();
            VerifyAreEqual("Найдено записей: 42",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter enabled");

            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "Name filter enabled 1");
            VerifyAreEqual("Tax 18", Driver.GetGridCell(9, "Name", "Taxes").Text, "Name filter enabled 10");

            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "select filter enabled 1");
            VerifyIsTrue(Driver.GetGridCell(9, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "select filter enabled 10");

            //check delete with filter

            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridTaxes");
            Driver.ScrollToTop();
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "delete all except default");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");
            VerifyAreEqual("Найдено записей: 66",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter enabled deleting");

            GoToAdmin("settingscheckout#?checkoutTab=taxes");
            VerifyAreEqual("Найдено записей: 66",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter enabled deleting after refreshing");
        }
    }
}