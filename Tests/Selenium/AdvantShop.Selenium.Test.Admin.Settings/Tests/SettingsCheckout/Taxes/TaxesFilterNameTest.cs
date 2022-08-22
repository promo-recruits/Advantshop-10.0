using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Taxes
{
    [TestFixture]
    public class SettingsTaxesFilterNameTest : BaseMultiSeleniumTest
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
        public void FilterName()
        {
            //check filter tax name
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "Name", gridId: "gridTaxes");

            //search by not exist name
            Driver.SetGridFilterValue("Name", "Tax name test 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Name", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist name
            Driver.SetGridFilterValue("Name", "Tax 5");

            VerifyAreEqual("Tax 5", Driver.GetGridCell(0, "Name", "Taxes").Text, "Tax Name");

            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter tax name");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridTaxes");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("Найдено записей: 96",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter tax name deleting 1");

            GoToAdmin("settingscheckout#?checkoutTab=taxes");
            VerifyAreEqual("Найдено записей: 96",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter tax name deleting 2");
        }
    }
}