using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailFormats
{
    [TestFixture]
    public class SettingsMailFormatsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
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
        [Order(0)]
        public void Grid()
        {
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Text
                    .Contains("Форматы писем"), "mail formatrs page h1");
            VerifyAreEqual("Найдено записей: 107",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "mail formatrs page count");

            VerifyAreEqual("Format Name Test 1", Driver.GetGridCell(0, "FormatName").Text, "mail formatrs grid name");
            VerifyAreEqual("При регистрации", Driver.GetGridCell(0, "TypeName").Text, "mail formatrs grid type");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder").Text, "mail formatrs grid sort");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "mail formatrs grid Enable");
        }

        [Test]
        [Order(1)]
        public void InplaceEnabled()
        {
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "pre check Enable");

            Driver.GetGridCell(0, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            VerifyIsFalse(
                Driver.GetGridCell(0, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "inplace Enable");

            GoToAdmin("settingsmail#?notifyTab=formats");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "inplace Enable after refreshing");
        }

        [Test]
        [Order(2)]
        public void InplaceSort()
        {
            VerifyAreEqual("Format Name Test 1", Driver.GetGridCell(0, "FormatName").Text, "pre check name");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder").Text, "pre check sort");

            Driver.SendKeysGridCell("7", 0, "SortOrder");

            VerifyAreEqual("7", Driver.GetGridCell(0, "SortOrder").Text, "inplace sort");

            GoToAdmin("settingsmail#?notifyTab=formats");

            Driver.GetGridIdFilter("grid", "Format Name Test 1");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            VerifyAreEqual("Format Name Test 1", Driver.GetGridCell(0, "FormatName").Text, "name");
            VerifyAreEqual("7", Driver.GetGridCell(0, "SortOrder").Text, "inplace sort after refreshing");

            //return default
            Driver.SendKeysGridCell("1", 0, "SortOrder");
        }

        [Test]
        [Order(3)]
        public void GoToEdit()
        {
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Редактирование формата писем"),
                "edit pop up");
            Driver.FindElement(By.CssSelector("[data-e2e=\"cancelMailFormat\"]")).Click();
        }

        [Test]
        [Order(10)]
        public void SelectDelete()
        {
            GoToAdmin("settingsmail#?notifyTab=formats");

            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Format Name Test 1", Driver.GetGridCell(0, "FormatName").Text, "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Format Name Test 2", Driver.GetGridCell(0, "FormatName").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text, "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "grid");
            VerifyAreEqual("Format Name Test 5", Driver.GetGridCell(0, "FormatName").Text, "selected 2 grid delete");
            VerifyAreEqual("Format Name Test 6", Driver.GetGridCell(1, "FormatName").Text, "selected 3 grid delete");
            VerifyAreEqual("Format Name Test 7", Driver.GetGridCell(2, "FormatName").Text, "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "grid");
            VerifyAreEqual("Format Name Test 15", Driver.GetGridCell(0, "FormatName").Text,
                "selected all on page 2 grid delete");
            VerifyAreEqual("Format Name Test 24", Driver.GetGridCell(9, "FormatName").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("93",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "grid");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "deleting all 1");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 1");

            GoToAdmin("settingsmail#?notifyTab=formats");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "deleting all 2");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 2");
        }
    }
}