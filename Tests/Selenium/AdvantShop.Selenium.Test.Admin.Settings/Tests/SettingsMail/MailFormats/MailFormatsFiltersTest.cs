using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailFormats
{
    [TestFixture]
    public class SettingsMailFormatsFiltersTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.MailFormat);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsMail\\Settings.MailFormatType.csv",
                "data\\Admin\\Settings\\SettingsMail\\Settings.MailFormat.csv"
            );

            Init();

            GoToAdmin("settingsmail#?notifyTab=formats");

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
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "FormatName", gridId: "grid");

            //search by not exist name
            Driver.SetGridFilterValue("FormatName", "Format Name Test 777");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("FormatName", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("FormatName", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist name
            Driver.SetGridFilterValue("FormatName", "Format Name Test 2");

            VerifyAreEqual("Format Name Test 2", Driver.GetGridCell(0, "FormatName").Text, "Name");

            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "filter name");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "grid");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "FormatName");
            VerifyAreEqual("Найдено записей: 96",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter name deleting 1");

            GoToAdmin("settingsmail#?notifyTab=formats");
            VerifyAreEqual("Найдено записей: 96",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter name deleting 2");
        }

        [Test]
        public void FilterTypeName()
        {
            //check filter 
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "TypeName",
                filterItem: "Уведомление об ответе на отзыв");

            VerifyAreEqual("Уведомление об ответе на отзыв", Driver.GetGridCell(0, "TypeName").Text, "TypeName");


            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter TypeName");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "grid");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "TypeName");
            VerifyAreEqual("Найдено записей: 103",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter TypeName deleting 1");

            GoToAdmin("settingsmail#?notifyTab=formats");
            VerifyAreEqual("Найдено записей: 103",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter TypeName deleting 2");
        }

        [Test]
        public void FilterEnable()
        {
            //check filter 
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enable", filterItem: "Активные");

            VerifyIsTrue(
                Driver.GetGridCell(0, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter Enable");


            VerifyAreEqual("Найдено записей: 54",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enable  count");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Неактивные\"]")).Click();
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();


            VerifyIsFalse(
                Driver.GetGridCell(0, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "filter not Enable");
            VerifyAreEqual("Найдено записей: 53",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter not Enable count");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "grid");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Enable");
            VerifyAreEqual("Найдено записей: 54",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enable deleting 1");

            GoToAdmin("settingsmail#?notifyTab=formats");
            VerifyAreEqual("Найдено записей: 54",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enable deleting 2");
        }
    }
}