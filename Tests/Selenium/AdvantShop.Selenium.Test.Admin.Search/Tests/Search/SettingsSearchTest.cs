using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Search.Tests.Search
{
    [TestFixture]
    public class SettingsSearchTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.SettingsSearch);
            InitializeService.LoadData("data\\Admin\\SettingsSearch\\Settings.SettingsSearch.csv");

            Init();

            GoToAdmin();

            Driver.FindElement(By.CssSelector(".search-input")).Click();
            Driver.FindElement(By.CssSelector(".search-input")).SendKeys("301 редирект");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'test title 1')]"));
            Driver.MouseFocus(By.XPath("//span[contains(text(), 'test title 1')]"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'test title 1')]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("EnableRedirect301"));
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin("settingssearch");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void SettingsSearchGridTest()
        {
            VerifyAreEqual("Поиск настроек", Driver.FindElement(By.TagName("h1")).Text, "h1 settings search page");

            VerifyAreEqual("test title 1", Driver.GetGridCellText(0, "Title"), "Title");
            VerifyAreEqual("settingsseo#?seoTab=seo301", Driver.GetGridCellText(0, "Link"), "Link");
            VerifyAreEqual("301 редирект", Driver.GetGridCellText(0, "KeyWords"), "KeyWords");
            VerifyAreEqual("10", Driver.GetGridCellText(0, "SortOrder"), "SortOrder");

            VerifyAreEqual("Найдено записей: 150",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }


        [Test]
        public void SettingsSearchInplaceTitle()
        {
            VerifyAreEqual("test title 1", Driver.GetGridCellText(0, "Title"), "inplace Title before editing");

            Driver.GetGridCellElement(0, "Title", by: By.Name("inputForm")).Click();
            Driver.ClearGridCell(0, "Title");
            Driver.GetGridCellElement(0, "Title", by: By.CssSelector(".ui-grid-custom-edit-field.form-control"))
                .SendKeys("Edited title 1");
            Driver.Blur();
            Driver.DropFocus("h1");

            //check admin
            GoToAdmin("settingssearch");

            VerifyAreEqual("Edited title 1", Driver.GetGridCellText(0, "Title"), "inplace Title");

            Driver.GetGridCellElement(0, "Title", by: By.Name("inputForm")).Click();
            Driver.ClearGridCell(0, "Title");
            Driver.GetGridCellElement(0, "Title", by: By.CssSelector(".ui-grid-custom-edit-field.form-control"))
                .SendKeys("test title 1");
            Driver.Blur();
            Driver.DropFocus("h1");
        }

        [Test]
        public void SettingsSearchInplaceKeywords()
        {
            VerifyAreEqual("301 редирект", Driver.GetGridCellText(0, "KeyWords"), "inplace KeyWords before editing");

            Driver.GetGridCellElement(0, "KeyWords", by: By.Name("inputForm")).Click();
            Driver.ClearGridCell(0, "KeyWords");
            Driver.GetGridCellElement(0, "KeyWords", by: By.CssSelector(".ui-grid-custom-edit-field.form-control"))
                .SendKeys("Edited KeyWords 1");
            Driver.Blur();
            Driver.DropFocus("h1");

            //check admin
            GoToAdmin("settingssearch");

            VerifyAreEqual("Edited KeyWords 1", Driver.GetGridCellText(0, "KeyWords"), "inplace KeyWords");

            //check search
            GoToAdmin();

            Driver.FindElement(By.CssSelector(".search-input")).Click();
            Driver.FindElement(By.CssSelector(".search-input")).SendKeys("Edited KeyWords 1");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'test title 1')]"));
            Driver.MouseFocus(By.XPath("//span[contains(text(), 'test title 1')]"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'test title 1')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Настройки')]"));

            VerifyIsTrue(Driver.Url.Contains("seoTab=seo301"), "check url from search");
            VerifyIsTrue(Driver.PageSource.Contains("301 Редиректы"), "page from search");

            GoToAdmin("settingssearch");

            Driver.GetGridCellElement(0, "KeyWords", by: By.Name("inputForm")).Click();
            Driver.ClearGridCell(0, "KeyWords");
            Driver.GetGridCellElement(0, "KeyWords", by: By.CssSelector(".ui-grid-custom-edit-field.form-control"))
                .SendKeys("301 редирект");
            Driver.Blur();
            Driver.DropFocus("h1");
        }

        [Test]
        public void SettingsSearchInplaceSort()
        {
            VerifyAreEqual("10", Driver.GetGridCellText(0, "SortOrder"), "inplace Sort before editing");

            Driver.GetGridCellElement(0, "SortOrder", by: By.Name("inputForm")).Click();
            Driver.ClearGridCell(0, "SortOrder");
            Driver.GetGridCellElement(0, "SortOrder", by: By.CssSelector(".ui-grid-custom-edit-field.form-control"))
                .SendKeys("200");
            Driver.DropFocus("h1");
            Driver.Blur();

            //check admin
            GoToAdmin("settingssearch");

            VerifyAreEqual("200", Driver.GetGridCellText(0, "SortOrder"), "inplace Sort");

            Driver.GetGridCellElement(0, "SortOrder", by: By.Name("inputForm")).Click();
            Driver.ClearGridCell(0, "SortOrder");
            Driver.GetGridCellElement(0, "SortOrder", by: By.CssSelector(".ui-grid-custom-edit-field.form-control"))
                .SendKeys("10");
            Driver.DropFocus("h1");
            Driver.Blur();
        }


        [Test]
        public void SettingsSearch()
        {
            GoToAdmin();

            Driver.FindElement(By.CssSelector(".search-input")).Click();
            Driver.FindElement(By.CssSelector(".search-input")).SendKeys("301 редирект");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'test title 1')]"));
            Driver.MouseFocus(By.XPath("//span[contains(text(), 'test title 1')]"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'test title 1')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Настройки')]"));

            VerifyIsTrue(Driver.Url.Contains("seoTab=seo301"), "check url from search");
            VerifyIsTrue(Driver.PageSource.Contains("301 Редиректы"), "page from search");
        }

        [Test]
        public void SettingsSearchGoToLink()
        {
            VerifyAreEqual("settingsseo#?seoTab=seo301", Driver.GetGridCellText(0, "Link"), "Link");

            Driver.GetGridCell(0, "Link").Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.WindowHandles.Count.Equals(2), "count tabs");

            VerifyIsTrue(Driver.Url.Contains("seoTab=seo301"), "check url from settings search grid");
            VerifyIsTrue(Driver.PageSource.Contains("301 Редиректы"), "page from settings search grid");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void SettingsSearchzSelectDelete()
        {
            //check delete cancel 
            Driver.GetGridCellElement(0, "_serviceColumn",
                by: By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("test title 1", Driver.GetGridCellText(0, "Title"), "1 grid canсel delete");

            //check delete
            Driver.GetGridCellElement(0, "_serviceColumn",
                by: By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("test title 10", Driver.GetGridCellText(0, "Title"), "1 grid delete");

            //check select 
            Driver.GetGridCellElement(0, "selectionRowHeaderCol",
                by: By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCellElement(1, "selectionRowHeaderCol",
                by: By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCellElement(2, "selectionRowHeaderCol",
                by: By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCellElement(0, "selectionRowHeaderCol",
                    by: By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 1 grid");
            VerifyIsTrue(
                Driver.GetGridCellElement(1, "selectionRowHeaderCol",
                    by: By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCellElement(2, "selectionRowHeaderCol",
                    by: By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("test title 102", Driver.GetGridCellText(0, "Title"), "selected 1 grid delete");
            VerifyAreEqual("test title 103", Driver.GetGridCellText(1, "Title"), "selected 2 grid delete");
            VerifyAreEqual("test title 104", Driver.GetGridCellText(2, "Title"), "selected 3 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            VerifyIsTrue(
                Driver.GetGridCellElement(0, "selectionRowHeaderCol",
                    by: By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCellElement(9, "selectionRowHeaderCol",
                    by: By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("test title 111", Driver.GetGridCellText(0, "Title"), "selected all on page 1 grid delete");
            VerifyAreEqual("test title 12", Driver.GetGridCellText(9, "Title"), "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("136", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();

            VerifyIsTrue(
                !Driver.GetGridCellElement(0, "selectionRowHeaderCol",
                    by: By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCellElement(9, "selectionRowHeaderCol",
                    by: By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            GoToAdmin("settingssearch");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");
        }
    }
}