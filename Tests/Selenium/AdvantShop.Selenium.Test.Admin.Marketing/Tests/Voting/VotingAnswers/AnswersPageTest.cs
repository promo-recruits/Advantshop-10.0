using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting.VotingAnswers
{
    [TestFixture]
    public class VotingAnswersPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\VotingAnswers\\Voice.VoiceTheme.csv",
                "data\\Admin\\VotingAnswers\\Voice.Answer.csv"
            );

            Init();
            InstallModule(BaseUrl, "Voting", true);
            ChangeSidebarState(true);
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));
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
        public void PageAnswers()
        {
            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "page 1 line 1");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(9, "Name", "Answers").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Answer 13", Driver.GetGridCell(0, "Name", "Answers").Text, "page 2 line 1");
            VerifyAreEqual("Answer 21", Driver.GetGridCell(9, "Name", "Answers").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Answer 22", Driver.GetGridCell(0, "Name", "Answers").Text, "page 3 line 1");
            VerifyAreEqual("Answer 30", Driver.GetGridCell(9, "Name", "Answers").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("Answer 31", Driver.GetGridCell(0, "Name", "Answers").Text, "page 4 line 1");
            VerifyAreEqual("Answer 4", Driver.GetGridCell(9, "Name", "Answers").Text, "page 4 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Answer 40", Driver.GetGridCell(0, "Name", "Answers").Text, "page 5 line 1");
            VerifyAreEqual("Answer 49", Driver.GetGridCell(9, "Name", "Answers").Text, "page 5 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Answer 5", Driver.GetGridCell(0, "Name", "Answers").Text, "page 6 line 1");
            VerifyAreEqual("Answer 58", Driver.GetGridCell(9, "Name", "Answers").Text, "page 6 line 10");
        }

        [Test]
        public void PageAnswersToPrevious()
        {
            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "page 1 line 1");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(9, "Name", "Answers").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Answer 13", Driver.GetGridCell(0, "Name", "Answers").Text, "page 2 line 1");
            VerifyAreEqual("Answer 21", Driver.GetGridCell(9, "Name", "Answers").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Answer 22", Driver.GetGridCell(0, "Name", "Answers").Text, "page 3 line 1");
            VerifyAreEqual("Answer 30", Driver.GetGridCell(9, "Name", "Answers").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Answer 13", Driver.GetGridCell(0, "Name", "Answers").Text, "page 2 line 1");
            VerifyAreEqual("Answer 21", Driver.GetGridCell(9, "Name", "Answers").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "page 1 line 1");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(9, "Name", "Answers").Text, "page 1 line 10");
        }

        [Test]
        public void PageAnswersToBegin()
        {
            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "page 1 line 1");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(9, "Name", "Answers").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Answer 13", Driver.GetGridCell(0, "Name", "Answers").Text, "page 2 line 1");
            VerifyAreEqual("Answer 21", Driver.GetGridCell(9, "Name", "Answers").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Answer 22", Driver.GetGridCell(0, "Name", "Answers").Text, "page 3 line 1");
            VerifyAreEqual("Answer 30", Driver.GetGridCell(9, "Name", "Answers").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Answer 31", Driver.GetGridCell(0, "Name", "Answers").Text, "page 4 line 1");
            VerifyAreEqual("Answer 4", Driver.GetGridCell(9, "Name", "Answers").Text, "page 4 line 10");

            //to begin
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "page 1 line 1");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(9, "Name", "Answers").Text, "page 1 line 10");
        }

        [Test]
        public void PageAnswersToEnd()
        {
            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "page 1 line 1");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(9, "Name", "Answers").Text, "page 1 line 10");

            //to end
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Answer 95", Driver.GetGridCell(0, "Name", "Answers").Text, "last page line 1");
            VerifyAreEqual("Answer 99", Driver.GetGridCell(4, "Name", "Answers").Text, "last page line 5");
        }

        [Test]
        public void PageAnswersToNext()
        {
            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "page 1 line 1");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(9, "Name", "Answers").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Answer 13", Driver.GetGridCell(0, "Name", "Answers").Text, "page 2 line 1");
            VerifyAreEqual("Answer 21", Driver.GetGridCell(9, "Name", "Answers").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Answer 22", Driver.GetGridCell(0, "Name", "Answers").Text, "page 3 line 1");
            VerifyAreEqual("Answer 30", Driver.GetGridCell(9, "Name", "Answers").Text, "page 3 line 10");
        }

        [Test]
        public void AnswersPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "find elem 105");
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "line 1");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(9, "Name", "Answers").Text, "line 10");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "line 1");
            VerifyAreEqual("Answer 94", Driver.GetGridCell(99, "Name", "Answers").Text, "line 99");

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void SelectAndDelete()
        {
            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "Answers").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "1 grid canсel delete");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Answers").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Answers")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Answers")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Answers")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Answers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Answers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Answers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Answer 102", Driver.GetGridCell(0, "Name", "Answers").Text, "selected 2 grid delete");
            VerifyAreEqual("Answer 103", Driver.GetGridCell(1, "Name", "Answers").Text, "selected 3 grid delete");
            VerifyAreEqual("Answer 104", Driver.GetGridCell(2, "Name", "Answers").Text, "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Answers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Answers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Answer 17", Driver.GetGridCell(0, "Name", "Answers").Text,
                "selected all on page 1 grid delete");
            VerifyAreEqual("Answer 25", Driver.GetGridCell(9, "Name", "Answers").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("91", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Answers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all 1 grid delete");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Answers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all 10 grid delete");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Answers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Answers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "1 delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "1 count all after deleting");

            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "2 delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "2 count all after deleting");
        }
    }
}