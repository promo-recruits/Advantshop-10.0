using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store.StaticBlock
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class StaticBlockGridTest : MySitesFunctions
    {
        private string sbPageUrl = "staticpages#?staticPageTab=blocks";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Store\\StaticBlock\\CMS.StaticBlock.csv"
            );
            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin(sbPageUrl);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CheckGrid()
        {
            //check admin
            VerifyAreEqual("bannerDetails", Driver.GetGridCell(0, "Key", "Blocks").Text);
            VerifyAreEqual("Баннер, единый для всех товаров", Driver.GetGridCell(0, "InnerName", "Blocks").Text);
            VerifyAreEqual("13.08.2012 10:59", Driver.GetGridCell(0, "AddedFormatted", "Blocks").Text);
            VerifyAreEqual("13.08.2016 11:09", Driver.GetGridCell(0, "ModifiedFormatted", "Blocks").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected);
            VerifyAreEqual("Найдено записей: 150",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //check client
            GoToClient("products/vash-tovar");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-aside"))
                .FindElement(By.CssSelector(".static-block")).Displayed);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-aside"))
                .FindElement(By.CssSelector(".static-block")).Text
                .Contains("баннер, единый для всех товаров содержание тест"));
        }


        [Test]
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
            VerifyAreEqual("150", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
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
            VerifyAreEqual("150", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
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
            VerifyAreEqual("150", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
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
            VerifyAreEqual("150", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select all before unselect");
            Driver.SetGridAction("Снять выделение с видимых");
            VerifyAreEqual("140", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select all after unselect");

            Driver.SetGridAction("Снять выделение");

            //Выделить несколько, сделать активными
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            SelectGridRow(-1);
            Driver.SetGridAction("Сделать активными");
            SelectGridRow(-1);

            Driver.SetGridFilter(BaseUrl, "Enabled", "Активные", true);
            VerifyAreEqual("Найдено записей: 148", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "change enabled (true)");
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");

            //Выделить несколько, сделать неактивными"
            SelectGridRow(-1);
            Driver.SetGridAction("Сделать неактивными");
            SelectGridRow(-1);

            Driver.SetGridFilter(BaseUrl, "Enabled", "Активные", true);
            VerifyAreEqual("Найдено записей: 138", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "change enabled (false)");
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");

            SelectGridRow(-1);
            UnselectGridRow(1);
            Driver.SetGridAction("Сделать активными");
            SelectGridRow(-1);
        }

        [Test]
        public void GridFilter()
        {
            //check only 3 filters
            Driver.FindElement(AdvBy.DataE2E("gridFilterDropdownButton")).Click();
            VerifyAreEqual(3, Driver.FindElements(AdvBy.DataE2E("gridFilterDropdownItem")).Count,
                "grid filter dropdown items count");
            Driver.FindElement(By.TagName("h1")).Click();

            //filter by part of key
            Driver.SetGridFilter(BaseUrl, "Key", "key12");
            VerifyAreEqual("staticblockkey12", Driver.GetGridCell(0, "Key", "Blocks").Text,
                "filter by part of key first cell");
            VerifyAreEqual("Найдено записей: 11", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by part of key cells count");

            //filter by key
            Driver.SetGridFilterValue("Key", "staticblockkey76");
            VerifyAreEqual("staticblockkey76", Driver.GetGridCell(0, "Key", "Blocks").Text, "filter by key first cell");
            VerifyAreEqual("Найдено записей: 1", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by key cells count");

            //filter by not enabled key
            Driver.SetGridFilterValue("Key", "staticblockkey875");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled key(1)");
            Driver.SetGridFilterValue("Key", "custom block key");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled key(2)");
            Driver.SetGridFilterValue("Key",
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled key(3)");
            Driver.SetGridFilterValue("Key", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled key(4)");

            //remove filter
            Functions.GridFilterClose(Driver, BaseUrl, "Key");

            //filter by part of name
            Driver.SetGridFilter(BaseUrl, "InnerName", "name 3");
            VerifyAreEqual("inner name 3", Driver.GetGridCell(0, "InnerName", "Blocks").Text,
                "filter by part of name first cell");
            VerifyAreEqual("Найдено записей: 11", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by part of name cells count");

            //filter by name
            Driver.SetGridFilterValue("InnerName", "inner name 46");
            VerifyAreEqual("inner name 46", Driver.GetGridCell(0, "InnerName", "Blocks").Text,
                "filter by name first cell");
            VerifyAreEqual("Найдено записей: 1", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by name cells count");

            //filter by not enabled name
            Driver.SetGridFilterValue("InnerName", "inner name 7441");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled name(1)");
            Driver.SetGridFilterValue("InnerName", "custom block name");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled name(2)");
            Driver.SetGridFilterValue("InnerName",
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled name(3)");
            Driver.SetGridFilterValue("InnerName", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "not enabled name(4)");

            //remove filter
            Functions.GridFilterClose(Driver, BaseUrl, "InnerName");

            //filter by enabled
            Driver.SetGridFilter(BaseUrl, "Enabled", "Активные", true);
            VerifyAreEqual("bannerDetails", Driver.GetGridCell(0, "Key", "Blocks").Text,
                "filter by enabled first cell");
            VerifyAreEqual("staticblockkey107", Driver.GetGridCell(9, "Key", "Blocks").Text,
                "filter by enabled last cell");
            VerifyAreEqual("Найдено записей: 147", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by enabled cells count");

            //filter by not enabled 
            Driver.SetGridFilterSelectValue("Enabled", "Неактивные");
            VerifyAreEqual("staticblockkey1", Driver.GetGridCell(0, "Key", "Blocks").Text,
                "filter by not enabled first cell");
            VerifyAreEqual("staticblockkey3", Driver.GetGridCell(2, "Key", "Blocks").Text,
                "filter by not enabled last cell");
            VerifyAreEqual("Найдено записей: 3", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by not enabled cells count");

            //remove filter
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");

            //filter by key, name, enabled (2 or 3 variant)
            Driver.SetGridFilter(BaseUrl, "InnerName", "name 1");
            VerifyAreEqual("Найдено записей: 61", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by combine cells count (1)");
            Driver.SetGridFilter(BaseUrl, "Enabled", "Активные", true);
            VerifyAreEqual("Найдено записей: 60", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by combine cells count (2)");
            Driver.SetGridFilter(BaseUrl, "Key", "0");
            VerifyAreEqual("Найдено записей: 15", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "filter by combine cells count (3)");

            //remove filter
            Functions.GridFilterClose(Driver, BaseUrl, "Key");
            Functions.GridFilterClose(Driver, BaseUrl, "InnerName");
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");
        }

        [Test]
        public void GridPagination()
        {
            //по цифре, троеточие, следующая, последняя, предыдущая, первая

            //1st is current page
            VerifyAreEqual("bannerDetails", Driver.GetGridCell(0, "Key", "Blocks").Text, "first page (1)");
            VerifyAreEqual("staticblockkey106", Driver.GetGridCell(9, "Key", "Blocks").Text, "first page (10)");
            //to next page, expected 2
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey107", Driver.GetGridCell(0, "Key", "Blocks").Text, "next page (3-1)");
            VerifyAreEqual("staticblockkey115", Driver.GetGridCell(9, "Key", "Blocks").Text, "next page (3-10)");
            //to next page, expected 3
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey116", Driver.GetGridCell(0, "Key", "Blocks").Text, "next page (2-1)");
            VerifyAreEqual("staticblockkey124", Driver.GetGridCell(9, "Key", "Blocks").Text, "next page (2-10)");
            //to next page, expected 4
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey125", Driver.GetGridCell(0, "Key", "Blocks").Text, "next page (4-1)");
            VerifyAreEqual("staticblockkey133", Driver.GetGridCell(9, "Key", "Blocks").Text, "next page (4-10)");

            //to prev page, expected 3
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey116", Driver.GetGridCell(0, "Key", "Blocks").Text, "prev page (3-1)");
            VerifyAreEqual("staticblockkey124", Driver.GetGridCell(9, "Key", "Blocks").Text, "prev page (3-10)");
            //to prev page, expected 2
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey107", Driver.GetGridCell(0, "Key", "Blocks").Text, "prev page (2-1)");
            VerifyAreEqual("staticblockkey115", Driver.GetGridCell(9, "Key", "Blocks").Text, "prev page (2-10)");

            //last page
            //selected 15, links "next", "last" disabled.
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey90", Driver.GetGridCell(0, "Key", "Blocks").Text, "last page (1)");
            VerifyAreEqual("staticblockkey99", Driver.GetGridCell(9, "Key", "Blocks").Text, "last page (10)");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-last")).GetAttribute("class")
                .Contains("disabled"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-next")).GetAttribute("class")
                .Contains("disabled"));

            //first page
            //selected 1, links "prev", "first" disabled.
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("bannerDetails", Driver.GetGridCell(0, "Key", "Blocks").Text, "first page (1)");
            VerifyAreEqual("staticblockkey106", Driver.GetGridCell(9, "Key", "Blocks").Text, "first page (10)");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-first")).GetAttribute("class")
                .Contains("disabled"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-prev")).GetAttribute("class")
                .Contains("disabled"));

            //... - выделяется следующая после видимых страница (5).
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElements(By.LinkText("..."))[0].Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey134", Driver.GetGridCell(0, "Key", "Blocks").Text, "... page (5-1)");
            VerifyAreEqual("staticblockkey142", Driver.GetGridCell(9, "Key", "Blocks").Text, "... page (5-10)");
            //... - еще вперед (7)
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElements(By.LinkText("..."))[1].Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey18", Driver.GetGridCell(0, "Key", "Blocks").Text, "... page (7-1)");
            VerifyAreEqual("staticblockkey26", Driver.GetGridCell(9, "Key", "Blocks").Text, "... page (7-10)");
            //... - back (4)
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElements(By.LinkText("..."))[0].Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey125", Driver.GetGridCell(0, "Key", "Blocks").Text, "... page (4-1)");
            VerifyAreEqual("staticblockkey133", Driver.GetGridCell(9, "Key", "Blocks").Text, "... page (4-10)");

            //page 5, 6, 7
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("5")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey134", Driver.GetGridCell(0, "Key", "Blocks").Text, "number page (5-1)");
            VerifyAreEqual("staticblockkey142", Driver.GetGridCell(9, "Key", "Blocks").Text, "number page (5-10)");
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("6")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey143", Driver.GetGridCell(0, "Key", "Blocks").Text, "number page (6-1)");
            VerifyAreEqual("staticblockkey17", Driver.GetGridCell(9, "Key", "Blocks").Text, "number page (6-10)");
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("7")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey18", Driver.GetGridCell(0, "Key", "Blocks").Text, "number page (7-1)");
            VerifyAreEqual("staticblockkey26", Driver.GetGridCell(9, "Key", "Blocks").Text, "number page (7-10)");

            //last
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Driver.WaitForAjax();
            //13, 11, 10
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("13")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey72", Driver.GetGridCell(0, "Key", "Blocks").Text, "number page (13-1)");
            VerifyAreEqual("staticblockkey80", Driver.GetGridCell(9, "Key", "Blocks").Text, "number page (13-10)");
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("11")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey54", Driver.GetGridCell(0, "Key", "Blocks").Text, "number page (11-1)");
            VerifyAreEqual("staticblockkey62", Driver.GetGridCell(9, "Key", "Blocks").Text, "number page (11-10)");
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("10")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey45", Driver.GetGridCell(0, "Key", "Blocks").Text, "number page (10-1)");
            VerifyAreEqual("staticblockkey53", Driver.GetGridCell(9, "Key", "Blocks").Text, "number page (10-10)");

            //first
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            //4.
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("4")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey125", Driver.GetGridCell(0, "Key", "Blocks").Text, "number page (4-1)");
            VerifyAreEqual("staticblockkey133", Driver.GetGridCell(9, "Key", "Blocks").Text, "number page (4-10)");
        }

        [Test]
        public void GridPaginationPageSize()
        {
            VerifyAreEqual("bannerDetails", Driver.GetGridCell(0, "Key", "Blocks").Text, "first grid cell (10)");
            VerifyAreEqual("staticblockkey106", Driver.GetGridCell(9, "Key", "Blocks").Text, "last grid cell (10)");
            VerifyAreEqual(10, Driver.FindElements(AdvBy.DataE2E("gridRow")).Count, "grid cells count (10)");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("bannerDetails", Driver.GetGridCell(0, "Key", "Blocks").Text, "first grid cell (100)");
            VerifyAreEqual("staticblockkey53", Driver.GetGridCell(99, "Key", "Blocks").Text, "last grid cell (100)");
            VerifyAreEqual(100, Driver.FindElements(AdvBy.DataE2E("gridRow")).Count, "grid cells count (100)");

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void GridSearch()
        {
            //часть ключа, целый ключ, часть имени, целое имя

            //search by part of exist inner name
            Driver.GridFilterSendKeys("name 7");

            VerifyIsTrue(Driver.GetGridCell(0, "InnerName", "Blocks").Text.Contains("inner name 7"));
            VerifyIsTrue(Driver.GetGridCell(3, "InnerName", "Blocks").Text.Contains("inner name 72"));
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text);

            //search by part of exist key
            Driver.GridFilterSendKeys("blockkey8");

            VerifyIsTrue(Driver.GetGridCell(0, "Key", "Blocks").Text.Contains("staticblockkey8"));
            VerifyIsTrue(Driver.GetGridCell(3, "Key", "Blocks").Text.Contains("staticblockkey82"));
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text);

            //search by exist inner name
            Driver.GridFilterSendKeys("inner name 81");

            VerifyIsTrue(Driver.GetGridCell(0, "InnerName", "Blocks").Text.Contains("inner name 81"));
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text);

            //search by exist key
            Driver.GridFilterSendKeys("staticblockkey112");

            VerifyIsTrue(Driver.GetGridCell(0, "Key", "Blocks").Text.Contains("staticblockkey112"));
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text);

            //search by not exist name
            Driver.GridFilterSendKeys("inner name 5000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search by not exist key
            Driver.GridFilterSendKeys("staticblockkey954");
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
        public void GridSort()
        {
            //ключ, название, активность, дата добавления, дата модификации

            //check sort by key
            Driver.GetGridCell(-1, "Key", "Blocks").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("bannerDetails", Driver.GetGridCell(0, "Key", "Blocks").Text);
            VerifyAreEqual("staticblockkey106", Driver.GetGridCell(9, "Key", "Blocks").Text);

            Driver.GetGridCell(-1, "Key", "Blocks").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("staticblockkey99", Driver.GetGridCell(0, "Key", "Blocks").Text);
            VerifyAreEqual("staticblockkey90", Driver.GetGridCell(9, "Key", "Blocks").Text);

            //sort by inner name
            Driver.GetGridCell(-1, "InnerName", "Blocks").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("inner name 1", Driver.GetGridCell(0, "InnerName", "Blocks").Text);
            VerifyAreEqual("inner name 107", Driver.GetGridCell(9, "InnerName", "Blocks").Text);

            Driver.GetGridCell(-1, "InnerName", "Blocks").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Баннер, единый для всех товаров", Driver.GetGridCell(0, "InnerName", "Blocks").Text);
            VerifyAreEqual("inner name 91", Driver.GetGridCell(9, "InnerName", "Blocks").Text);

            //sort by enabled
            Driver.GetGridCell(-1, "Enabled", "Blocks").Click();
            Driver.WaitForAjax();
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(2, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(3, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected);

            Driver.GetGridCell(-1, "Enabled", "Blocks").Click();
            Driver.WaitForAjax();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "Enabled", "Blocks").FindElement(By.TagName("input")).Selected);

            //sort by add date
            Driver.GetGridCell(-1, "AddedFormatted", "Blocks").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("13.08.2012 10:59", Driver.GetGridCell(0, "AddedFormatted", "Blocks").Text);
            VerifyAreEqual("21.09.2013 11:11", Driver.GetGridCell(9, "AddedFormatted", "Blocks").Text);

            Driver.GetGridCell(-1, "AddedFormatted", "Blocks").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("08.02.2014 11:11", Driver.GetGridCell(0, "AddedFormatted", "Blocks").Text);
            VerifyAreEqual("30.01.2014 11:11", Driver.GetGridCell(9, "AddedFormatted", "Blocks").Text);

            //sort by ModifiedFormatted date
            Driver.GetGridCell(-1, "ModifiedFormatted", "Blocks").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("13.08.2016 11:09", Driver.GetGridCell(0, "ModifiedFormatted", "Blocks").Text);
            VerifyAreEqual("22.08.2016 11:09", Driver.GetGridCell(9, "ModifiedFormatted", "Blocks").Text);

            Driver.GetGridCell(-1, "ModifiedFormatted", "Blocks").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("09.01.2017 11:09", Driver.GetGridCell(0, "ModifiedFormatted", "Blocks").Text);
            VerifyAreEqual("31.12.2016 11:09", Driver.GetGridCell(9, "ModifiedFormatted", "Blocks").Text);


            Driver.GetGridCell(-1, "Key", "Blocks").Click();
            Driver.WaitForAjax();
        }

        protected void SelectGridRow(int rowIndex)
        {
            SelectGridRow(rowIndex, "Blocks");
        }

        protected void UnselectGridRow(int rowIndex)
        {
            UnselectGridRow(rowIndex, "Blocks");
        }
    }
}