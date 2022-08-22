using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting
{
    [TestFixture]
    public class VotingFilterTest : BaseMultiSeleniumTest
    {
        [SetUp]
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
            ChangeSidebarState(true);
        }

        [Test]
        public void FilterByTheme()
        {
            GoToModule("voting");

            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "Name");

            //search by not exist 
            Driver.SetGridFilterValue("Name", "Test Voice theme");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Name", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Name", "########@@@@@@@@&&&&&&&******");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("Name", "VoiceTheme4");

            VerifyAreEqual("VoiceTheme4", Driver.GetGridCell(0, "Name", "Voting").Text, "filter Theme line 1");
            VerifyAreEqual("VoiceTheme48", Driver.GetGridCell(9, "Name", "Voting").Text, "filter Theme line 10");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Theme count");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Редактирование голосования", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Theme return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Name");
            VerifyAreEqual("Найдено записей: 159",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Theme deleting 1");

            Refresh();
            VerifyAreEqual("Найдено записей: 159",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Theme deleting 2");
        }

        [Test]
        public void FilterIsDefault()
        {
            GoToModule("voting");

            //check filter default
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "IsDefault", filterItem: "Текущие");
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsDefault count");
            VerifyAreEqual("VoiceTheme7", Driver.GetGridCell(0, "Name", "Voting").Text, "filter IsDefault name");
            VerifyIsTrue(Driver.GetGridCell(0, "IsDefault", "Voting").FindElement(By.CssSelector("input")).Selected,
                "filter IsDefault checkbox");

            //check filter not default
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Не текущие\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));

            VerifyAreEqual("Найдено записей: 169",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter no IsDefault count");

            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "filter no IsDefault line 1");
            VerifyAreEqual("VoiceTheme11", Driver.GetGridCell(9, "Name", "Voting").Text, "filter no IsDefault line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "IsDefault");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsDefault after deleting 1");

            Refresh();
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsDefault after deleting 2");
        }


        [Test]
        public void FilterIsHaveNullVoice()
        {
            GoToModule("voting");

            //check filter IsHaveNullVoice
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "IsHaveNullVoice",
                filterItem: "Имеют пустые голоса");
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsHaveNullVoice count");
            VerifyAreEqual("VoiceTheme161", Driver.GetGridCell(0, "Name", "Voting").Text,
                "filter IsHaveNullVoice name line 1");
            VerifyAreEqual("VoiceTheme170", Driver.GetGridCell(9, "Name", "Voting").Text,
                "filter IsHaveNullVoice name line 10");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsHaveNullVoice", "Voting").FindElement(By.CssSelector("input")).Selected,
                "filter IsHaveNullVoice checkbox line 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsHaveNullVoice", "Voting").FindElement(By.CssSelector("input")).Selected,
                "filter IsHaveNullVoice checkbox line 10");

            //check filter not IsHaveNullVoice
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Не имеют пустые голоса\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Найдено записей: 160",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter no IsHaveNullVoice count");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text,
                "filter no IsHaveNullVoice name line 1");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text,
                "filter no IsHaveNullVoice name line 10");
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsHaveNullVoice", "Voting").FindElement(By.CssSelector("input")).Selected,
                "filter no IsHaveNullVoice checkbox line 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "IsHaveNullVoice", "Voting").FindElement(By.CssSelector("input")).Selected,
                "filter no IsHaveNullVoice checkbox line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "IsHaveNullVoice");
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsHaveNullVoice after deleting 1");

            Refresh();
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsHaveNullVoice after deleting 2");
        }

        [Test]
        public void FilterIsClose()
        {
            GoToModule("voting");

            //check filter IsClose
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "IsClose", filterItem: "Закрытые");
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Найдено записей: 37",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsClose count");
            VerifyAreEqual("VoiceTheme64", Driver.GetGridCell(0, "Name", "Voting").Text, "filter IsClose name line 1");
            VerifyAreEqual("VoiceTheme73", Driver.GetGridCell(9, "Name", "Voting").Text, "filter IsClose name line 10");
            VerifyIsTrue(Driver.GetGridCell(0, "IsClose", "Voting").FindElement(By.CssSelector("input")).Selected,
                "filter IsClose checkbox line 1");
            VerifyIsTrue(Driver.GetGridCell(9, "IsClose", "Voting").FindElement(By.CssSelector("input")).Selected,
                "filter IsClose checkbox line 10");

            //check filter not IsClose
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Не закрытые\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Найдено записей: 133",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter no IsClose count");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text,
                "filter no IsClose name line 1");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text,
                "filter no IsClose name line 10");
            VerifyIsFalse(Driver.GetGridCell(0, "IsClose", "Voting").FindElement(By.CssSelector("input")).Selected,
                "filter no IsClose checkbox line 1");
            VerifyIsFalse(Driver.GetGridCell(9, "IsClose", "Voting").FindElement(By.CssSelector("input")).Selected,
                "filter no IsClose checkbox line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "IsClose");
            VerifyAreEqual("Найдено записей: 37",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsClose after deleting 1");

            Refresh();
            VerifyAreEqual("Найдено записей: 37",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter IsClose after deleting 2");
        }

        [Test]
        public void FilterByDateAdded()
        {
            GoToModule("voting");

            Functions.GridFilterSet(Driver, BaseUrl, name: "DateAdded");

            //check filter min not exist
            Driver.SetGridFilterRange("DateAdded", "31.12.2050 00:00", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter add date min not exist");

            //check max not exist
            Driver.SetGridFilterRange("DateAdded", "", "31.12.2050 00:00");

            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("DateAdded", "31.12.2050 00:00", "31.12.2050 00:00");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter sum min/max not exist");

            //check filter add date
            Driver.SetGridFilterRange("DateAdded", "01.01.2013 00:00", "26.02.2013 23:00");
            VerifyAreEqual("Найдено записей: 55",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date");

            VerifyAreEqual("VoiceTheme114", Driver.GetGridCell(0, "Name", "Voting").Text,
                "filter add date name line 1");
            VerifyAreEqual("01.01.2013 14:52", Driver.GetGridCell(0, "DateAdded", "Voting").Text,
                "filter add date name add date line 1");
            VerifyAreEqual("VoiceTheme123", Driver.GetGridCell(9, "Name", "Voting").Text,
                "filter add date checkbox line 10");
            VerifyAreEqual("10.01.2013 13:52", Driver.GetGridCell(9, "DateAdded", "Voting").Text,
                "filter add date checkbox line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "DateAdded");
            VerifyAreEqual("Найдено записей: 115",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date after deleting 1");

            Refresh();
            VerifyAreEqual("Найдено записей: 115",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date after deleting 2");
        }

        [Test]
        public void FilterByDateModify()
        {
            GoToModule("voting");

            Functions.GridFilterSet(Driver, BaseUrl, name: "DateModify");

            //check filter min not exist
            Driver.SetGridFilterRange("DateModify", "31.12.2050 00:00", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter DateModify min not exist");

            //check max not exist
            Driver.SetGridFilterRange("DateModify", "", "31.12.2050 00:00");
            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DateModify max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("DateModify", "31.12.2050 00:00", "31.12.2050 00:00");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter sum min/max not exist");

            //check filter DateModify
            Driver.SetGridFilterRange("DateModify", "01.04.2014 00:00", "28.04.2014 23:00");
            VerifyAreEqual("Найдено записей: 27",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DateModify count");

            VerifyAreEqual("VoiceTheme74", Driver.GetGridCell(0, "Name", "Voting").Text,
                "filter DateModify name line 1");
            VerifyAreEqual("01.04.2014 15:26", Driver.GetGridCell(0, "DateModify", "Voting").Text,
                "filter DateModify name DateModify line 1");
            VerifyAreEqual("VoiceTheme83", Driver.GetGridCell(9, "Name", "Voting").Text,
                "filter DateModify checkbox line 10");
            VerifyAreEqual("11.04.2014 23:26", Driver.GetGridCell(9, "DateModify", "Voting").Text,
                "filter DateModify checkbox line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "DateModify");
            VerifyAreEqual("Найдено записей: 143",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DateModify after deleting 1");

            Refresh();
            VerifyAreEqual("Найдено записей: 143",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DateModify after deleting 2");
        }

        [Test]
        public void FilterCountAnswers()
        {
            GoToModule("voting");

            Functions.GridFilterSet(Driver, BaseUrl, name: "CountAnswers");

            //check min too much symbols
            Driver.SetGridFilterRange("CountAnswers", "111111111", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter CountAnswers min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("CountAnswers", "", "111111111");
            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountAnswers max many symbols");

            //check min and max too much symbols
            Driver.SetGridFilterRange("CountAnswers", "111111111", "111111111");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter CountAnswers min/max many symbols");

            //check invalid symbols
            //check min invalid symbols
            Driver.SetGridFilterRange("CountAnswers", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter CountAnswers min imvalid symbols");
            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountAnswers count min many symbols");

            //check max invalid symbols
            Refresh();
            
            Driver.SetGridFilterRange("CountAnswers", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter CountAnswers max imvalid symbols");
            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountAnswers count max many symbols");

            //check min and max invalid symbols
            Refresh();

            Driver.SetGridFilterRange("CountAnswers", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter CountAnswers both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter CountAnswers both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountAnswers count min/max many symbols");

            //check filter min not exist
            Refresh();

            Driver.SetGridFilterRange("CountAnswers", "1000", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter CountAnswers min not exist");

            //check max not exist
            Driver.SetGridFilterRange("CountAnswers", "", "1000");
            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountAnswers max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("CountAnswers", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter CountAnswers min/max not exist");

            //check filter
            Driver.SetGridFilterRange("CountAnswers", "3", "5");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountAnswers count");

            VerifyAreEqual("VoiceTheme3", Driver.GetGridCell(0, "Name", "Voting").Text,
                "filter CountAnswers name line 1");
            VerifyAreEqual("3", Driver.GetGridCell(0, "CountAnswers", "Voting").Text,
                "filter CountAnswers name line 1");
            VerifyAreEqual("VoiceTheme4", Driver.GetGridCell(1, "Name", "Voting").Text,
                "filter CountAnswers checkbox line 2");
            VerifyAreEqual("4", Driver.GetGridCell(1, "CountAnswers", "Voting").Text,
                "filter CountAnswers checkbox line 2");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "CountAnswers");
            VerifyAreEqual("Найдено записей: 167",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountAnswers after deleting 1");

            Refresh();
            VerifyAreEqual("Найдено записей: 167",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CountAnswers after deleting 2");
        }
    }
}