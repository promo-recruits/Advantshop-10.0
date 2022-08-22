using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting
{
    [TestFixture]
    public class VotingPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Voting\\Voice.VoiceTheme.csv",
                "data\\Admin\\Voting\\Voice.Answer.csv"
            );

            Init();
            InstallModule(BaseUrl, "Voting", true);
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
        public void PageVoiting()
        {
            GoToModule("voting");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "page 1 line 1");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("VoiceTheme11", Driver.GetGridCell(0, "Name", "Voting").Text, "page 2 line 1");
            VerifyAreEqual("VoiceTheme20", Driver.GetGridCell(9, "Name", "Voting").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("VoiceTheme21", Driver.GetGridCell(0, "Name", "Voting").Text, "page 3 line 1");
            VerifyAreEqual("VoiceTheme30", Driver.GetGridCell(9, "Name", "Voting").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("VoiceTheme31", Driver.GetGridCell(0, "Name", "Voting").Text, "page 4 line 1");
            VerifyAreEqual("VoiceTheme40", Driver.GetGridCell(9, "Name", "Voting").Text, "page 4 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("VoiceTheme41", Driver.GetGridCell(0, "Name", "Voting").Text, "page 5 line 1");
            VerifyAreEqual("VoiceTheme50", Driver.GetGridCell(9, "Name", "Voting").Text, "page 5 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("VoiceTheme51", Driver.GetGridCell(0, "Name", "Voting").Text, "page 6 line 1");
            VerifyAreEqual("VoiceTheme60", Driver.GetGridCell(9, "Name", "Voting").Text, "page 6 line 10");
        }

        [Test]
        [Order(5)]
        public void PageVoitingToPrevious()
        {
            GoToModule("voting");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "page 1 line 1");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("VoiceTheme11", Driver.GetGridCell(0, "Name", "Voting").Text, "page 2 line 1");
            VerifyAreEqual("VoiceTheme20", Driver.GetGridCell(9, "Name", "Voting").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("VoiceTheme21", Driver.GetGridCell(0, "Name", "Voting").Text, "page 3 line 1");
            VerifyAreEqual("VoiceTheme30", Driver.GetGridCell(9, "Name", "Voting").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("VoiceTheme11", Driver.GetGridCell(0, "Name", "Voting").Text, "page 2 line 1");
            VerifyAreEqual("VoiceTheme20", Driver.GetGridCell(9, "Name", "Voting").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "page 1 line 1");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text, "page 1 line 10");
        }

        [Test]
        [Order(0)]
        public void VoitingPresent()
        {
            GoToModule("voting");
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "find elem 170");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "line 1");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text, "line 10");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "line 1");
            VerifyAreEqual("VoiceTheme100", Driver.GetGridCell(99, "Name", "Voting").Text, "line 20");

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        [Order(2)]
        public void PageVoitingToBegin()
        {
            GoToModule("voting");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "page 1 line 1");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("VoiceTheme11", Driver.GetGridCell(0, "Name", "Voting").Text, "page 2 line 1");
            VerifyAreEqual("VoiceTheme20", Driver.GetGridCell(9, "Name", "Voting").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("VoiceTheme21", Driver.GetGridCell(0, "Name", "Voting").Text, "page 3 line 1");
            VerifyAreEqual("VoiceTheme30", Driver.GetGridCell(9, "Name", "Voting").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("VoiceTheme31", Driver.GetGridCell(0, "Name", "Voting").Text, "page 4 line 1");
            VerifyAreEqual("VoiceTheme40", Driver.GetGridCell(9, "Name", "Voting").Text, "page 4 line 10");

            //to begin
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "page 1 line 1");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text, "page 1 line 10");
        }

        [Test]
        [Order(3)]
        public void PageVoitingToEnd()
        {
            GoToModule("voting");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "page 1 line 1");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text, "page 1 line 10");

            //to end
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("VoiceTheme161", Driver.GetGridCell(0, "Name", "Voting").Text, "last page line 1");
            VerifyAreEqual("VoiceTheme170", Driver.GetGridCell(9, "Name", "Voting").Text, "last page line 10");
        }

        [Test]
        [Order(4)]
        public void PageVoitingToNext()
        {
            GoToModule("voting");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "page 1 line 1");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("VoiceTheme11", Driver.GetGridCell(0, "Name", "Voting").Text, "page 2 line 1");
            VerifyAreEqual("VoiceTheme20", Driver.GetGridCell(9, "Name", "Voting").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("VoiceTheme21", Driver.GetGridCell(0, "Name", "Voting").Text, "page 3 line 1");
            VerifyAreEqual("VoiceTheme30", Driver.GetGridCell(9, "Name", "Voting").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("VoiceTheme31", Driver.GetGridCell(0, "Name", "Voting").Text, "page 4 line 1");
            VerifyAreEqual("VoiceTheme40", Driver.GetGridCell(9, "Name", "Voting").Text, "page 4 line 10");
        }

        [Test]
        [Order(6)]
        public void SelectAndDelete()
        {
            GoToModule("voting");
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "1 grid canсel delete");
            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Voting")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Voting")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Voting")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Voting")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Voting")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Voting")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("VoiceTheme5", Driver.GetGridCell(0, "Name", "Voting").Text, "selected 2 grid delete");
            VerifyAreEqual("VoiceTheme6", Driver.GetGridCell(1, "Name", "Voting").Text, "selected 3 grid delete");
            VerifyAreEqual("VoiceTheme7", Driver.GetGridCell(2, "Name", "Voting").Text, "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Voting")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Voting")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("VoiceTheme15", Driver.GetGridCell(0, "Name", "Voting").Text,
                "selected all on page 1 grid delete");
            VerifyAreEqual("VoiceTheme24", Driver.GetGridCell(9, "Name", "Voting").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("156", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Voting")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all 1 grid delete");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Voting")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all 10 grid delete");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Voting")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Voting")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "1 delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "1 count all after deleting");

            Refresh();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "2 delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "2 count all after deleting");
        }
    }
}