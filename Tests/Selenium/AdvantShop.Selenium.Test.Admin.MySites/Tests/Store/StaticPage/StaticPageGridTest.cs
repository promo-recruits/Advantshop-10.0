using System.Net;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store.StaticPage
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class StaticPageGridTest : MySitesFunctions
    {
        private string spPageUrl = "staticpages#?staticPageTab=pages";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Store\\StaticPage\\CMS.StaticPage.csv"
            );
            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin(spPageUrl);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        [Order(1)]
        public void CheckGrid()
        {
            //check admin
            VerifyAreEqual("Page1", Driver.GetGridCell(0, "PageName", "Pages").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Pages").FindElement(By.TagName("input")).Selected);
            VerifyAreEqual("15.06.2012 14:22", Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text);
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Pages").Text);
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            VerifyAreEqual("Page10", Driver.GetGridCell(9, "PageName", "Pages").Text);
            VerifyAreEqual("10", Driver.GetGridCell(9, "SortOrder", "Pages").Text);
            VerifyAreEqual("06.06.2012 14:22", Driver.GetGridCell(9, "ModifyDateFormatted", "Pages").Text);
            VerifyAreEqual("false", Driver.FindElement(By.Name("switchOnOff_9")).GetAttribute("value"));

            Driver.GetGridCell(0, "PageName", "Pages").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Статическая страница \"Page1\"", Driver.FindElement(By.TagName("h2")).Text);
            VerifyAreEqual("Page1", Driver.FindElement(By.Name("PageName")).GetAttribute("value"));
            VerifyAreEqual("1", Driver.FindElement(By.Name("SortOrder")).GetAttribute("value"));
            VerifyAreEqual("page1", Driver.FindElement(By.Name("UrlPath")).GetAttribute("value"));

            Thread.Sleep(1000);
            Driver.SwitchTo().Frame(0);
            Thread.Sleep(1000);
            VerifyAreEqual("text1", Driver.FindElement(By.TagName("body")).Text);
            Driver.SwitchTo().DefaultContent();

            //check client
            GoToClient("pages/page1");
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "default 1st page");
            VerifyAreEqual("Page1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("text1", Driver.FindElement(By.CssSelector(".staticpage-content")).Text);

            VerifyIsTrue(Is404Page("pages/page10"));
        }


        [Test]
        [Order(1)]
        public void GridAction()
        {
            //Выделить одну, отменить выделение одной
            SelectGridRow(0);
            VerifyAreEqual("1", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text, "select one");
            UnselectGridRow(0);
            VerifyIsTrue(
                string.IsNullOrEmpty(Driver.FindElement(By.ClassName("ui-grid-custom-selection-in-header")).Text),
                "unselect one");

            //выделить несколько, отменить выделение нескольких
            SelectGridRow(1);
            SelectGridRow(3);
            SelectGridRow(9);
            VerifyAreEqual("3", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text, "select some");
            UnselectGridRow(1);
            UnselectGridRow(3);
            VerifyAreEqual("1", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text, "select some");
            UnselectGridRow(9);
            VerifyIsTrue(
                string.IsNullOrEmpty(Driver.FindElement(By.ClassName("ui-grid-custom-selection-in-header")).Text),
                "unselect one");

            //выделить всю страницу, отменить выделение страницы 
            SelectGridRow(-1);
            VerifyAreEqual("10", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select page");
            UnselectGridRow(-1);
            VerifyIsTrue(
                string.IsNullOrEmpty(Driver.FindElement(By.ClassName("ui-grid-custom-selection-in-header")).Text),
                "unselect one");

            //выделить все
            SelectGridRow(0);
            Driver.FindElement(AdvBy.DataE2E("gridSelectionSelectAllFn")).Click();
            VerifyAreEqual("101", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select all");
            //снять выделение со всех - ссылка. 
            Driver.FindElement(AdvBy.DataE2E("gridSelectionDeselectAllFn")).Click();
            VerifyIsTrue(
                string.IsNullOrEmpty(Driver.FindElement(By.ClassName("ui-grid-custom-selection-in-header")).Text),
                "unselect one");

            //пройтись по нескольким страницам, выделить по паре товаров
            SelectGridRow(2);
            SelectGridRow(3);
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            SelectGridRow(0);
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("4")).Click();
            Driver.WaitForAjax();
            SelectGridRow(7);
            SelectGridRow(8);
            SelectGridRow(9);
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.WaitForAjax();
            SelectGridRow(-1);
            VerifyAreEqual("16", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select some on multiple pages");
            Driver.FindElement(AdvBy.DataE2E("gridSelectionSelectAllFn")).Click();
            VerifyAreEqual("101", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select all");
            Driver.FindElement(AdvBy.DataE2E("gridSelectionDeselectAllFn")).Click();

            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Driver.WaitForAjax();

            //ACTIONS
            //снять выделение
            SelectGridRow(-1);
            VerifyAreEqual("10", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select page before unselect");
            Driver.SetGridAction("Снять выделение");
            VerifyIsTrue(
                string.IsNullOrEmpty(Driver.FindElement(By.ClassName("ui-grid-custom-selection-in-header")).Text),
                "remove selection(1)");

            SelectGridRow(0);
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            SelectGridRow(8);
            SelectGridRow(9);
            VerifyAreEqual("3", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select some before unselect");
            Driver.SetGridAction("Снять выделение");
            VerifyIsTrue(
                string.IsNullOrEmpty(Driver.FindElement(By.ClassName("ui-grid-custom-selection-in-header")).Text),
                "remove selection(3)");

            SelectGridRow(-1);
            Driver.FindElement(AdvBy.DataE2E("gridSelectionSelectAllFn")).Click();
            VerifyAreEqual("101", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select all before unselect");
            Driver.SetGridAction("Снять выделение");
            VerifyIsTrue(
                string.IsNullOrEmpty(Driver.FindElement(By.ClassName("ui-grid-custom-selection-in-header")).Text),
                "remove selection(2)");

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            //Снять выделение с видимых, 
            SelectGridRow(-1);
            VerifyAreEqual("10", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select page before unselect");
            Driver.SetGridAction("Снять выделение с видимых");
            VerifyIsTrue(
                string.IsNullOrEmpty(Driver.FindElement(By.ClassName("ui-grid-custom-selection-in-header")).Text),
                "remove selection(1)");

            SelectGridRow(0);
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            SelectGridRow(8);
            SelectGridRow(9);
            VerifyAreEqual("3", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select some before unselect");
            Driver.SetGridAction("Снять выделение с видимых");
            VerifyAreEqual("1", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select some after unselect");

            SelectGridRow(-1);
            Driver.FindElement(AdvBy.DataE2E("gridSelectionSelectAllFn")).Click();
            VerifyAreEqual("101", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select all before unselect");
            Driver.SetGridAction("Снять выделение с видимых");
            VerifyAreEqual("91", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select all after unselect");

            Driver.SetGridAction("Снять выделение");
        }

        [Test]
        [Order(1)]
        public void GridFilter()
        {
            //check only 3 filters
            Driver.FindElement(AdvBy.DataE2E("gridFilterDropdownButton")).Click();
            VerifyAreEqual(3, Driver.FindElements(AdvBy.DataE2E("gridFilterDropdownItem")).Count,
                "grid filter dropdown items count");
            Driver.FindElement(By.TagName("h1")).Click();

            //filter by part of key
            Driver.SetGridFilter(BaseUrl, "PageName", "ge9");
            VerifyAreEqual("Page9", Driver.GetGridCell(0, "PageName", "Pages").Text,
                "filter by part of key first cell");
            VerifyAreEqual("Найдено записей: 11", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by part of key cells count");

            //filter by key
            Driver.SetGridFilterValue("PageName", "Page81");
            VerifyAreEqual("Page81", Driver.GetGridCell(0, "PageName", "Pages").Text, "filter by key first cell");
            VerifyAreEqual("Найдено записей: 1", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by key cells count");

            //filter by not enabled key
            Driver.SetGridFilterValue("PageName", "Page248");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled key(1)");
            Driver.SetGridFilterValue("PageName", "custom page name");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled key(2)");
            Driver.SetGridFilterValue("PageName",
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled key(3)");
            Driver.SetGridFilterValue("PageName", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled key(4)");

            //remove filter
            Functions.GridFilterClose(Driver, BaseUrl, "PageName");

            //filter by data
            Functions.GridFilterSet(Driver, BaseUrl, "ModifyDateFormatted");
            Functions.DataTimePickerFilter(Driver, BaseUrl, monthFrom: "Апрель", yearFrom: "2012",
                dataFrom: "Апрель 12, 2012", monthTo: "Май", yearTo: "2012", dataTo: "Май 23, 2012",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]");
            VerifyAreEqual("23.05.2012 14:22", Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text,
                "filter by data first cell");
            VerifyAreEqual("Найдено записей: 41", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by part data cells count");

            Functions.DataTimePickerFilter(Driver, BaseUrl, monthFrom: "Январь", yearFrom: "2010",
                dataFrom: "Январь 10, 2010", monthTo: "Декабрь", yearTo: "2020", dataTo: "Декабрь 31, 2020",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]");
            VerifyAreEqual("15.06.2012 14:22", Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text,
                "filter by data first cell2");
            VerifyAreEqual("Найдено записей: 101", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by part data cells count2");

            //filter by not enabled data
            Functions.DataTimePickerFilter(Driver, BaseUrl, monthFrom: "Январь", yearFrom: "2006",
                dataFrom: "Январь 10, 2006", monthTo: "Декабрь", yearTo: "2010", dataTo: "Декабрь 31, 2010",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled data(1)");
            Functions.DataTimePickerFilter(Driver, BaseUrl, monthFrom: "Январь", yearFrom: "2018",
                dataFrom: "Январь 10, 2018", monthTo: "Декабрь", yearTo: "2020", dataTo: "Декабрь 31, 2020",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled data(2)");

            //remove filter
            Functions.GridFilterClose(Driver, BaseUrl, "ModifyDateFormatted");

            //filter by enabled
            Driver.SetGridFilter(BaseUrl, "Enabled", "Активные", true);
            VerifyAreEqual("Page1", Driver.GetGridCell(0, "PageName", "Pages").Text, "filter by enabled first cell");
            VerifyAreEqual("Page15", Driver.GetGridCell(9, "PageName", "Pages").Text, "filter by enabled last cell");
            VerifyAreEqual("Найдено записей: 96", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by enabled cells count");

            //filter by not enabled 
            Driver.SetGridFilterSelectValue("Enabled", "Неактивные");
            VerifyAreEqual("Page6", Driver.GetGridCell(0, "PageName", "Pages").Text,
                "filter by not enabled first cell");
            VerifyAreEqual("Page10", Driver.GetGridCell(4, "PageName", "Pages").Text,
                "filter by not enabled last cell");
            VerifyAreEqual("Найдено записей: 5", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by not enabled cells count");

            //remove filter
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");

            //filter by key, name, enabled (2 or 3 variant)
            Functions.GridFilterSet(Driver, BaseUrl, "ModifyDateFormatted");
            Functions.DataTimePickerFilter(Driver, BaseUrl, monthFrom: "Май", yearFrom: "2012", dataFrom: "Май 1, 2012",
                monthTo: "Июнь", yearTo: "2012", dataTo: "Июнь 30, 2012",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]");
            VerifyAreEqual("Найдено записей: 45", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by combine cells count (1)");
            Driver.SetGridFilter(BaseUrl, "Enabled", "Активные", true);
            VerifyAreEqual("Найдено записей: 40", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by combine cells count (2)");
            Driver.SetGridFilter(BaseUrl, "PageName", "1");
            VerifyAreEqual("Найдено записей: 13", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by combine cells count (3)");

            //remove filter
            Functions.GridFilterClose(Driver, BaseUrl, "PageName");
            Functions.GridFilterClose(Driver, BaseUrl, "ModifyDateFormatted");
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");
        }

        [Test]
        [Order(1)]
        public void GridPagination()
        {
            //по цифре, троеточие, следующая, последняя, предыдущая, первая

            //1st is current page
            VerifyAreEqual("Page1", Driver.GetGridCell(0, "PageName", "Pages").Text, "first page (1)");
            VerifyAreEqual("Page10", Driver.GetGridCell(9, "PageName", "Pages").Text, "first page (10)");
            //to next page, expected 2
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page11", Driver.GetGridCell(0, "PageName", "Pages").Text, "next page (3-1)");
            VerifyAreEqual("Page20", Driver.GetGridCell(9, "PageName", "Pages").Text, "next page (3-10)");
            //to next page, expected 3
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page21", Driver.GetGridCell(0, "PageName", "Pages").Text, "next page (2-1)");
            VerifyAreEqual("Page30", Driver.GetGridCell(9, "PageName", "Pages").Text, "next page (2-10)");
            //to next page, expected 4
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page31", Driver.GetGridCell(0, "PageName", "Pages").Text, "next page (4-1)");
            VerifyAreEqual("Page40", Driver.GetGridCell(9, "PageName", "Pages").Text, "next page (4-10)");

            //to prev page, expected 3
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page21", Driver.GetGridCell(0, "PageName", "Pages").Text, "prev page (3-1)");
            VerifyAreEqual("Page30", Driver.GetGridCell(9, "PageName", "Pages").Text, "prev page (3-10)");
            //to prev page, expected 2
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page11", Driver.GetGridCell(0, "PageName", "Pages").Text, "prev page (2-1)");
            VerifyAreEqual("Page20", Driver.GetGridCell(9, "PageName", "Pages").Text, "prev page (2-10)");

            //last page
            //selected 11, links "next", "last" disabled.
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page101", Driver.GetGridCell(0, "PageName", "Pages").Text, "last page (1)");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-last")).GetAttribute("class")
                .Contains("disabled"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-next")).GetAttribute("class")
                .Contains("disabled"));

            //first page
            //selected 1, links "prev", "first" disabled.
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page1", Driver.GetGridCell(0, "PageName", "Pages").Text, "first page (1)");
            VerifyAreEqual("Page10", Driver.GetGridCell(9, "PageName", "Pages").Text, "first page (10)");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-first")).GetAttribute("class")
                .Contains("disabled"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-prev")).GetAttribute("class")
                .Contains("disabled"));

            //... - выделяется следующая после видимых страница (5).
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElements(By.LinkText("..."))[0].Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page41", Driver.GetGridCell(0, "PageName", "Pages").Text, "... page (5-1)");
            VerifyAreEqual("Page50", Driver.GetGridCell(9, "PageName", "Pages").Text, "... page (5-10)");
            //... - еще вперед (7)
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElements(By.LinkText("..."))[1].Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page61", Driver.GetGridCell(0, "PageName", "Pages").Text, "... page (7-1)");
            VerifyAreEqual("Page70", Driver.GetGridCell(9, "PageName", "Pages").Text, "... page (7-10)");
            //... - back (4)
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElements(By.LinkText("..."))[0].Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page31", Driver.GetGridCell(0, "PageName", "Pages").Text, "... page (4-1)");
            VerifyAreEqual("Page40", Driver.GetGridCell(9, "PageName", "Pages").Text, "... page (4-10)");

            //page 5, 6, 7
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("5")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page41", Driver.GetGridCell(0, "PageName", "Pages").Text, "number page (5-1)");
            VerifyAreEqual("Page50", Driver.GetGridCell(9, "PageName", "Pages").Text, "number page (5-10)");
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("6")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page51", Driver.GetGridCell(0, "PageName", "Pages").Text, "number page (6-1)");
            VerifyAreEqual("Page60", Driver.GetGridCell(9, "PageName", "Pages").Text, "number page (6-10)");
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("7")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page61", Driver.GetGridCell(0, "PageName", "Pages").Text, "number page (7-1)");
            VerifyAreEqual("Page70", Driver.GetGridCell(9, "PageName", "Pages").Text, "number page (7-10)");

            //last
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Driver.WaitForAjax();
            //10, 8, 7
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("10")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page91", Driver.GetGridCell(0, "PageName", "Pages").Text, "number page (10-1)");
            VerifyAreEqual("Page100", Driver.GetGridCell(9, "PageName", "Pages").Text, "number page (10-10)");
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("8")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page71", Driver.GetGridCell(0, "PageName", "Pages").Text, "number page (8-1)");
            VerifyAreEqual("Page80", Driver.GetGridCell(9, "PageName", "Pages").Text, "number page (8-10)");
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("7")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page61", Driver.GetGridCell(0, "PageName", "Pages").Text, "number page (7-1)");
            VerifyAreEqual("Page70", Driver.GetGridCell(9, "PageName", "Pages").Text, "number page (7-10)");

            //first
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            //4.
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("4")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page31", Driver.GetGridCell(0, "PageName", "Pages").Text, "number page (4-1)");
            VerifyAreEqual("Page40", Driver.GetGridCell(9, "PageName", "Pages").Text, "number page (4-10)");
        }

        [Test]
        [Order(1)]
        public void GridPaginationPageSize()
        {
            VerifyAreEqual("Page1", Driver.GetGridCell(0, "PageName", "Pages").Text, "first grid cell (10)");
            VerifyAreEqual("Page10", Driver.GetGridCell(9, "PageName", "Pages").Text, "last grid cell (10)");
            VerifyAreEqual(10, Driver.FindElements(AdvBy.DataE2E("gridRow")).Count, "grid cells count (10)");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("Page1", Driver.GetGridCell(0, "PageName", "Pages").Text, "first grid cell (100)");
            VerifyAreEqual("Page100", Driver.GetGridCell(99, "PageName", "Pages").Text, "last grid cell (100)");
            VerifyAreEqual(100, Driver.FindElements(AdvBy.DataE2E("gridRow")).Count, "grid cells count (100)");

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        [Order(1)]
        public void GridSearch()
        {
            //часть заголовка, целый заголовок

            //search by part of exist header
            Driver.GridFilterSendKeys("ge8");

            VerifyIsTrue(Driver.GetGridCell(0, "PageName", "Pages").Text.Contains("Page8"));
            VerifyIsTrue(Driver.GetGridCell(3, "PageName", "Pages").Text.Contains("Page82"));
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text);

            //search by of exist header
            Driver.GridFilterSendKeys("Page17");

            VerifyIsTrue(Driver.GetGridCell(0, "PageName", "Pages").Text.Contains("Page17"));
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text);

            //search by not exist name
            Driver.GridFilterSendKeys("Page5000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search too much symbols
            Driver.GridFilterSendKeys(
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", By.ClassName("ui-grid-custom-filter-total"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        [Order(1)]
        public void GridSort()
        {
            //page, enabled, modifyDate, sortOrder

            //check sort by page
            Driver.GetGridCell(-1, "PageName", "Pages").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page1", Driver.GetGridCell(0, "PageName", "Pages").Text);
            VerifyAreEqual("Page16", Driver.GetGridCell(9, "PageName", "Pages").Text);

            Driver.GetGridCell(-1, "PageName", "Pages").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Page99", Driver.GetGridCell(0, "PageName", "Pages").Text);
            VerifyAreEqual("Page90", Driver.GetGridCell(9, "PageName", "Pages").Text);

            //sort by enabled
            Driver.GetGridCell(-1, "Enabled", "Pages").Click();
            Driver.WaitForAjax();
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Pages").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled", "Pages").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(4, "Enabled", "Pages").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(5, "Enabled", "Pages").FindElement(By.TagName("input")).Selected);

            Driver.GetGridCell(-1, "Enabled", "Pages").Click();
            Driver.WaitForAjax();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Pages").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled", "Pages").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "Enabled", "Pages").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "Enabled", "Pages").FindElement(By.TagName("input")).Selected);

            //sort by ModifyDateFormatted date
            Driver.GetGridCell(-1, "ModifyDateFormatted", "Pages").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("07.03.2012 14:22", Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text);
            VerifyAreEqual("16.03.2012 14:22", Driver.GetGridCell(9, "ModifyDateFormatted", "Pages").Text);

            Driver.GetGridCell(-1, "ModifyDateFormatted", "Pages").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("15.06.2012 14:22", Driver.GetGridCell(0, "ModifyDateFormatted", "Pages").Text);
            VerifyAreEqual("06.06.2012 14:22", Driver.GetGridCell(9, "ModifyDateFormatted", "Pages").Text);

            //sort by sort
            Driver.GetGridCell(-1, "SortOrder", "Pages").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Pages").Text);
            VerifyAreEqual("10", Driver.GetGridCell(9, "SortOrder", "Pages").Text);

            Driver.GetGridCell(-1, "SortOrder", "Pages").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("101", Driver.GetGridCell(0, "SortOrder", "Pages").Text);
            VerifyAreEqual("92", Driver.GetGridCell(9, "SortOrder", "Pages").Text);


            Driver.GetGridCell(-1, "SortOrder", "Pages").Click();
            Driver.WaitForAjax();
        }

        [Test]
        [Order(10)]
        public void GridActionRemove()
        {
            //ACTIONS
            //выделить 1 товар, удалить, отменить; выделить 1 товар, удалить, подтвердить
            //удалить всю страницу; удалить выборочно; удалить все.

            //check delete cancel
            Driver.GetGridCell(0, "_serviceColumn", "Pages").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalCancel();
            Thread.Sleep(1000);
            VerifyAreEqual("Page1", Driver.GetGridCell(0, "PageName", "Pages").Text);

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Pages").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            Thread.Sleep(1000);
            VerifyAreNotEqual("Page1", Driver.GetGridCell(0, "PageName", "Pages").Text);

            //check delete selected items
            SelectGridRow(0);
            SelectGridRow(2);
            SelectGridRow(3);

            RemoveSelectedGridRows();
            VerifyAreNotEqual("Page2", Driver.GetGridCell(0, "PageName", "Pages").Text);
            VerifyAreEqual("Page3", Driver.GetGridCell(0, "PageName", "Pages").Text);
            VerifyAreNotEqual("Page4", Driver.GetGridCell(2, "PageName", "Pages").Text);
            VerifyAreNotEqual("Page5", Driver.GetGridCell(3, "PageName", "Pages").Text);
            VerifyAreEqual("Найдено записей: 97", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "deleted some at page");

            //check delete all on page
            //удаляет и вложенные страницы (15 вложена в 14)
            SelectGridRow(-1);
            RemoveSelectedGridRows();
            VerifyAreEqual("Page16", Driver.GetGridCell(0, "PageName", "Pages").Text);
            VerifyAreEqual("Page25", Driver.GetGridCell(9, "PageName", "Pages").Text);
            VerifyAreEqual("Найдено записей: 86", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "deleted all page");

            //check delete all
            SelectGridRow(-1);
            Driver.FindElement(AdvBy.DataE2E("gridSelectionSelectAllFn")).Click();
            RemoveSelectedGridRows();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Refresh();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        protected void SelectGridRow(int rowIndex)
        {
            SelectGridRow(rowIndex, "Pages");
        }

        protected void UnselectGridRow(int rowIndex)
        {
            UnselectGridRow(rowIndex, "Pages");
        }
    }
}