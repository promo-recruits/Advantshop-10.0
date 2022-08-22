using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting.VotingAnswers
{
    [TestFixture]
    public class VotingAnswersFilterTest : BaseMultiSeleniumTest
    {
        [SetUp]
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
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 77-percent\"]"));
            //Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));
        }


        [Test]
        public void FilterByName()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "Name");

            //search by not exist 
            Driver.SetGridFilterValue("Name", "Test Answer one");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Name", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //check invalid symbols
            Driver.SetGridFilterValue("Name", "########@@@@@@@@&&&&&&&******");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("Name", "Answer 6");

            VerifyAreEqual("Answer 6", Driver.GetGridCell(0, "Name", "Answers").Text, "filter Answer name line 1");
            VerifyAreEqual("Answer 68", Driver.GetGridCell(9, "Name", "Answers").Text, "filter Answer name line 10");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Answer name count");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Answers")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Редактирование варианта ответа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Answer name return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            // Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Name");
            VerifyAreEqual("Найдено записей: 94",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Answer name deleting 1");

            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));

            VerifyAreEqual("Найдено записей: 94",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Answer name deleting 2");
        }

        [Test]
        public void FilterIsVisible()
        {
            //check filter Visible
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "IsVisible", filterItem: "Да");
            VerifyAreEqual("Найдено записей: 80",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsVisible count");
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "filter IsVisible name line 1");
            VerifyIsTrue(Driver.GetGridCell(0, "IsVisible", "Answers").FindElement(By.CssSelector("input")).Selected,
                "filter IsVisible checkbox line 1");
            VerifyAreEqual("Answer 18", Driver.GetGridCell(9, "Name", "Answers").Text, "filter IsVisible name line 10");
            VerifyIsTrue(Driver.GetGridCell(9, "IsVisible", "Answers").FindElement(By.CssSelector("input")).Selected,
                "filter IsVisible checkbox line 10");

            //check filter Visible not
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Найдено записей: 25",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter no IsVisible count");

            VerifyAreEqual("Answer 100", Driver.GetGridCell(0, "Name", "Answers").Text,
                "filter no  IsVisible name line 1");
            VerifyIsFalse(Driver.GetGridCell(0, "IsVisible", "Answers").FindElement(By.CssSelector("input")).Selected,
                "filter no IsVisible checkbox line 1");
            VerifyAreEqual("Answer 84", Driver.GetGridCell(9, "Name", "Answers").Text,
                "filter no  IsVisible name line 10");
            VerifyIsFalse(Driver.GetGridCell(9, "IsVisible", "Answers").FindElement(By.CssSelector("input")).Selected,
                "filter no IsVisible checkbox line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "IsVisible");
            VerifyAreEqual("Найдено записей: 80",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsVisible after deleting 1");

            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            VerifyAreEqual("Найдено записей: 80",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsVisible after deleting 2");
        }

        [Test]
        public void FilterCountVoice()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "CountVoice");

            //check min too much symbols
            Driver.SetGridFilterRange("CountVoice", "111111111", ""); 
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter CountVoice min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("CountVoice", "", "111111111"); 
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountVoice max many symbols");

            //check min and max too much symbols
            Driver.SetGridFilterRange("CountVoice", "111111111", "111111111"); 
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter CountVoice min/max many symbols");

            //check invalid symbols
            //check min invalid symbols
            Driver.SetGridFilterRange("CountVoice", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter CountVoice min imvalid symbols");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountVoice count min many symbols");

            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 77-percent\"]"));
            Functions.GridFilterSet(Driver, BaseUrl, name: "CountVoice");

            //check max invalid symbols
            Driver.SetGridFilterRange("CountVoice", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter CountVoice max imvalid symbols");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountVoice count max many symbols");

            //check min and max invalid symbols
            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 77-percent\"]"));
            Functions.GridFilterSet(Driver, BaseUrl, name: "CountVoice");

            Driver.SetGridFilterRange("CountVoice", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter CountVoice both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter CountVoice both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountVoice count min/max many symbols");

            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 77-percent\"]"));
            Functions.GridFilterSet(Driver, BaseUrl, name: "CountVoice");

            //check filter min not exist
            Driver.SetGridFilterRange("CountVoice", "1000", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter CountVoice min not exist");

            //check max not exist
            Driver.SetGridFilterRange("CountVoice", "", "1000");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountVoice max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("CountVoice", "1000", "1000"); 
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter CountVoice min/max not exist");

            //check filter
            Driver.SetGridFilterRange("CountVoice", "20", "100"); 
            VerifyAreEqual("Найдено записей: 81",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountVoice count");

            VerifyAreEqual("Answer 100", Driver.GetGridCell(0, "Name", "Answers").Text,
                "filter CountVoice name line 1");
            VerifyAreEqual("100", Driver.GetGridCell(0, "CountVoice", "Answers").Text, "filter CountVoice name line 1");
            VerifyAreEqual("Answer 28", Driver.GetGridCell(9, "Name", "Answers").Text,
                "filter CountVoice checkbox line 10");
            VerifyAreEqual("28", Driver.GetGridCell(9, "CountVoice", "Answers").Text,
                "filter CountVoice checkbox line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "CountVoice");
            VerifyAreEqual("Найдено записей: 24",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountVoice after deleting 1");

            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            VerifyAreEqual("Найдено записей: 24",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountVoice after deleting 2");
        }


        [Test]
        public void FilterSortOrder()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");

            //check min too much symbols
            Driver.SetGridFilterRange("SortOrder", "111111111", ""); 
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter SortOrder min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("SortOrder", "", "111111111"); 
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder max many symbols");

            //check min and max too much symbols
            Driver.SetGridFilterRange("SortOrder", "111111111", "111111111");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter SortOrder min/max many symbols");

            //check invalid symbols
            //check min invalid symbols
            Driver.SetGridFilterRange("SortOrder", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter SortOrder min imvalid symbols");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder count min many symbols");

            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 77-percent\"]"));
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");

            //check max invalid symbols
            Driver.SetGridFilterRange("SortOrder", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter SortOrder max imvalid symbols");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder count max many symbols");

            //check min and max invalid symbols
            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 77-percent\"]"));
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");

            Driver.SetGridFilterRange("SortOrder", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter SortOrder both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter SortOrder both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder count min/max many symbols");

            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 77-percent\"]"));
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");

            //check filter min not exist
            Driver.SetGridFilterRange("SortOrder", "1000", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter SortOrder min not exist");

            //check max not exist
            Driver.SetGridFilterRange("SortOrder", "", "1000");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("SortOrder", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter SortOrder min/max not exist");

            //check filter
            Driver.SetGridFilterRange("SortOrder", "25", "45");
            VerifyAreEqual("Найдено записей: 21",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder count");

            VerifyAreEqual("Answer 25", Driver.GetGridCell(0, "Name", "Answers").Text, "filter SortOrder name line 1");
            VerifyAreEqual("25", Driver.GetGridCell(0, "SortOrder", "Answers").Text, "filter SortOrder name line 1");
            VerifyAreEqual("Answer 34", Driver.GetGridCell(9, "Name", "Answers").Text,
                "filter SortOrder checkbox line 10");
            VerifyAreEqual("34", Driver.GetGridCell(9, "SortOrder", "Answers").Text,
                "filter SortOrder checkbox line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            //  Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "SortOrder");
            VerifyAreEqual("Найдено записей: 84",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder after deleting 1");

            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            VerifyAreEqual("Найдено записей: 84",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder after deleting 2");
        }

        [Test]
        public void FilterByDateAdded()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "DateAdded");

            //check filter min not exist
            Driver.SetGridFilterRange("DateAdded", "31.12.2050 00:00", "",
                ".ui-grid-custom-filter-total");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter add date min not exist");

            //check max not exist
            Driver.SetGridFilterRange("DateAdded", "", "31.12.2050 00:00", "h1");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("DateAdded", "31.12.2050 00:00", "31.12.2050 00:00", "h1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter sum min/max not exist");

            //check filter add date
            Driver.SetGridFilterRange("DateAdded", "06.09.2012 00:00", "07.09.2012 23:55", "h1");
            VerifyAreEqual("Найдено записей: 48",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date");

            VerifyAreEqual("Answer 13", Driver.GetGridCell(0, "Name", "Answers").Text, "filter add date name line 1");
            VerifyAreEqual("06.09.2012 23:02", Driver.GetGridCell(0, "DateAdded", "Answers").Text,
                "filter add date line 1 date");
            VerifyAreEqual("Answer 22", Driver.GetGridCell(9, "Name", "Answers").Text, "filter add date name line 10");
            VerifyAreEqual("06.09.2012 19:07", Driver.GetGridCell(9, "DateAdded", "Answers").Text,
                "filter add date line 10 date");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "DateAdded");
            VerifyAreEqual("Найдено записей: 57",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date after deleting 1");

            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            VerifyAreEqual("Найдено записей: 57",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date after deleting 2");
        }

        [Test]
        public void FilterByDateModify()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "DateModify");

            //check filter min not exist
            Driver.SetGridFilterRange("DateModify", "31.12.2050 00:00", "",
                ".ui-grid-custom-filter-total");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter DateModify min not exist");

            //check max not exist
            Driver.SetGridFilterRange("DateModify", "", "31.12.2050 00:00", "h1");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DateModify max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("DateModify", "31.12.2050 00:00", "31.12.2050 00:00", "h1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter sum min/max not exist");

            //check filter DateModify
            Driver.SetGridFilterRange("DateModify", "01.03.2015 00:00", "30.03.2015 23:00", "h1");
            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DateModify count");

            VerifyAreEqual("Answer 11", Driver.GetGridCell(0, "Name", "Answers").Text, "filter DateModify name line 1");
            VerifyAreEqual("19.03.2015 22:20", Driver.GetGridCell(0, "DateModify", "Answers").Text,
                "filter DateModify name DateModify line 1");
            VerifyAreEqual("Answer 16", Driver.GetGridCell(5, "Name", "Answers").Text,
                "filter DateModify checkbox line 6");
            VerifyAreEqual("23.03.2015 23:44", Driver.GetGridCell(5, "DateModify", "Answers").Text,
                "filter DateModify checkbox line 6");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "DateModify");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DateModify after deleting 1");

            GoToModule("voting");
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DateModify after deleting 2");
        }
    }
}