using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailTemplates
{
    [TestFixture]
    public class SettingsMailTemplatesFiltersTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.MailTemplate);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsMail\\MailTemplates\\Settings.MailTemplate.csv"
            );

            Init();

            GoToAdmin("settingsmail#?notifyTab=templates");

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
            //check filter name
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "Name", gridId: "gridTemplates");

            //search by not exist name
            Driver.SetGridFilterValue("Name", "Template Name Test 777");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Name", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Name", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist name
            Driver.SetGridFilterValue("Name", "Template Name Test 2");
            VerifyAreEqual("Template Name Test 2", Driver.GetGridCell(0, "Name", "Templates").Text, "Name");

            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "filter name");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridTemplates");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("Найдено записей: 96",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter name deleting 1");

            GoToAdmin("settingsmail#?notifyTab=templates");
            VerifyAreEqual("Найдено записей: 96",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter name deleting 2");
        }

        [Test]
        public void FilterEnable()
        {
            //check filter 
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Active", filterItem: "Активные",
                gridId: "Templates");

            VerifyIsTrue(
                Driver.GetGridCell(0, "Active", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected, "filter Enable");


            VerifyAreEqual("Найдено записей: 58",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enable  count");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Неактивные\"]")).Click();
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();


            VerifyIsFalse(
                Driver.GetGridCell(0, "Active", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected, "filter not Enable");
            VerifyAreEqual("Найдено записей: 49",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter not Enable count");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridTemplates");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Active");
            VerifyAreEqual("Найдено записей: 58",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enable deleting 1");

            GoToAdmin("settingsmail#?notifyTab=templates");
            VerifyAreEqual("Найдено записей: 58",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enable deleting 2");
        }
    }
}