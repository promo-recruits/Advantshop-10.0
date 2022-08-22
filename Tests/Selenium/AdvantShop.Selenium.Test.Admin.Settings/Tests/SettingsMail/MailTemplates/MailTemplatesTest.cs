using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailTemplates
{
    [TestFixture]
    public class SettingsMailTemplatesTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.MailTemplate);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsMail\\MailTemplates\\Settings.MailTemplate.csv"
            );

            Init();
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

        [Order(0)]
        [Test]
        public void Grid()
        {
            GoToAdmin("settingsmail#?notifyTab=templates");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Text
                    .Contains("Шаблоны ответов"), "mail templates page h1");
            VerifyAreEqual("Найдено записей: 107",
                Driver.FindElement(
                        By.CssSelector(
                            "[grid-unique-id=\"gridTemplates\"] .input-group-addon.ui-grid-custom-filter-total"))
                    .Text, "mail templates page count");

            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text,
                "mail templates grid name");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Templates").Text, "mail templates grid sort");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Active", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "mail templates grid Enable");
        }

        [Order(1)]
        [Test]
        public void InplaceEnabled()
        {
            GoToAdmin("settingsmail#?notifyTab=templates");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Active", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected, "pre check Enable");

            Driver.GetGridCell(0, "Active", "Templates").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]"))
                .Click();
            VerifyIsFalse(
                Driver.GetGridCell(0, "Active", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected, "inplace Enable");

            GoToAdmin("settingsmail#?notifyTab=templates");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Active", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "inplace Enable after refreshing");
        }

        [Order(2)]
        [Test]
        public void InplaceSort()
        {
            GoToAdmin("settingsmail#?notifyTab=templates");
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text, "pre check name");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Templates").Text, "pre check sort");

            Driver.SendKeysGridCell("7", 0, "SortOrder", "Templates");

            VerifyAreEqual("7", Driver.GetGridCell(0, "SortOrder", "Templates").Text, "inplace sort");

            GoToAdmin("settingsmail#?notifyTab=templates");

            Driver.GetGridIdFilter("gridTemplates", "Template Name Test 1");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text, "name");
            VerifyAreEqual("7", Driver.GetGridCell(0, "SortOrder", "Templates").Text, "inplace sort after refreshing");

            //return default
            Driver.SendKeysGridCell("1", 0, "SortOrder", "Templates");

        }

        [Order(3)]
        [Test]
        public void GoToEdit()
        {
            GoToAdmin("settingsmail#?notifyTab=templates");
            Driver.GetGridCell(0, "_serviceColumn", "Templates")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Редактирование шаблона письма ответа"),
                "edit pop up");
            Driver.FindElement(By.CssSelector("[data-e2e=\"cancelMailTemplate\"]")).Click();
        }

        [Order(10)]
        [Test]
        public void SelectDelete()
        {
            GoToAdmin("settingsmail#?notifyTab=templates");

            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "Templates")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text,
                "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Templates")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Template Name Test 2", Driver.GetGridCell(0, "Name", "Templates").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Templates")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Templates")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Templates")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text, "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridTemplates");
            VerifyAreEqual("Template Name Test 5", Driver.GetGridCell(0, "Name", "Templates").Text,
                "selected 2 grid delete");
            VerifyAreEqual("Template Name Test 6", Driver.GetGridCell(1, "Name", "Templates").Text,
                "selected 3 grid delete");
            VerifyAreEqual("Template Name Test 7", Driver.GetGridCell(2, "Name", "Templates").Text,
                "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridTemplates");
            VerifyAreEqual("Template Name Test 15", Driver.GetGridCell(0, "Name", "Templates").Text,
                "selected all on page 2 grid delete");
            VerifyAreEqual("Template Name Test 24", Driver.GetGridCell(9, "Name", "Templates").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("93",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridTemplates");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "deleting all 1");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 1");

            GoToAdmin("settingsmail#?notifyTab=templates");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "deleting all 2");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 2");
        }
    }
}