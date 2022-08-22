using System.Net;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.Pages
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class GridTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\Pages\\Grid\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\Pages\\Grid\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\Pages\\Grid\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\Pages\\Grid\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\Pages\\Grid\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\Pages\\Grid\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\Pages\\Grid\\CMS.LandingSubBlock.csv"
            );

            Init(false);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin("funnels/site/1");
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
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(0, "IsMain").FindElement(By.TagName("input")).Selected);
            VerifyAreEqual("03.06.2021 18:14", Driver.GetGridCell(0, "CreatedDateFormatted").Text);
            VerifyAreEqual("Найдено записей: 52",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyAreEqual(0,
                Driver.GetGridCell(0, "_serviceColumn").FindElements(By.TagName("ui-grid-custom-delete")).Count);
            VerifyAreEqual("FunnelPage10", Driver.GetGridCell(9, "Name").Text);
            VerifyIsFalse(Driver.GetGridCell(9, "Enabled").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "IsMain").FindElement(By.TagName("input")).Selected);
            VerifyAreEqual("03.06.2021 18:14", Driver.GetGridCell(0, "CreatedDateFormatted").Text);

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.ClassName("lp-block"));
            VerifyAreEqual(BaseUrl + "lp/testfunnel?inplace=true", Driver.Url);
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("block-type-form")).Count);
            VerifyAreEqual(1,
                Driver.GetGridCell(9, "_serviceColumn").FindElements(By.TagName("ui-grid-custom-delete")).Count);

            //check client
            ReInitClient();
            GoToClient("lp/testfunnel");
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "default 1st page");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("block-type-form")).Count);

            VerifyIsTrue(Is404Page("lp/testfunnel/funnelpage10"));
            ReInit();
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
            VerifyAreEqual("52", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text, "select all");
            //снять выделение со всех - ссылка. 
            Driver.FindElement(AdvBy.DataE2E("gridSelectionDeselectAllFn")).Click();
            VerifyIsTrue(
                string.IsNullOrEmpty(Driver.FindElement(By.ClassName("ui-grid-custom-selection-in-header")).Text),
                "unselect one");

            //пройтись по нескольким страницам, выделить по паре страниц
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
            VerifyAreEqual("52", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text, "select all");
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
            VerifyAreEqual("52", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
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
            VerifyAreEqual("52", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select all before unselect");
            Driver.SetGridAction("Снять выделение с видимых");
            VerifyAreEqual("42", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "select all after unselect");

            //Выделить несколько, сделать активными
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("2")).Click();
            SelectGridRow(0);
            SelectGridRow(1);
            SelectGridRow(4);
            Driver.SetGridAction("Сделать активными");


            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(3, "Enabled").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(4, "Enabled").FindElement(By.TagName("input")).Selected);

            //Выделить несколько, сделать неактивными"
            SelectGridRow(0);
            SelectGridRow(1);
            SelectGridRow(4);
            Driver.SetGridAction("Сделать неактивными");

            VerifyIsFalse(Driver.GetGridCell(0, "Enabled").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(2, "Enabled").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(4, "Enabled").FindElement(By.TagName("input")).Selected);

            UnselectGridRow(1);
            UnselectGridRow(4);
            Driver.SetGridAction("Сделать активными");

            Driver.SetGridAction("Снять выделение");
        }

        [Test]
        [Order(1)]
        public void GridPagination()
        {
            //по цифре, троеточие, следующая, последняя, предыдущая, первая

            //1st is current page
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").Text, "first page (1)");
            VerifyAreEqual("FunnelPage10", Driver.GetGridCell(9, "Name").Text, "first page (10)");
            //to next page, expected 2
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage11", Driver.GetGridCell(0, "Name").Text, "next page (3-1)");
            VerifyAreEqual("FunnelPage20", Driver.GetGridCell(9, "Name").Text, "next page (3-10)");
            //to next page, expected 3
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage21", Driver.GetGridCell(0, "Name").Text, "next page (2-1)");
            VerifyAreEqual("FunnelPage30", Driver.GetGridCell(9, "Name").Text, "next page (2-10)");
            //to next page, expected 4
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage31", Driver.GetGridCell(0, "Name").Text, "next page (4-1)");
            VerifyAreEqual("FunnelPage40", Driver.GetGridCell(9, "Name").Text, "next page (4-10)");

            //to prev page, expected 3
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage21", Driver.GetGridCell(0, "Name").Text, "prev page (3-1)");
            VerifyAreEqual("FunnelPage30", Driver.GetGridCell(9, "Name").Text, "prev page (3-10)");

            //last page
            //selected 11, links "next", "last" disabled.
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage52", Driver.GetGridCell(1, "Name").Text, "last page (2)");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-last")).GetAttribute("class")
                .Contains("disabled"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-next")).GetAttribute("class")
                .Contains("disabled"));

            //first page
            //selected 1, links "prev", "first" disabled.
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").Text, "first page (1)");
            VerifyAreEqual("FunnelPage10", Driver.GetGridCell(9, "Name").Text, "first page (10)");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-first")).GetAttribute("class")
                .Contains("disabled"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".pagination-prev")).GetAttribute("class")
                .Contains("disabled"));

            //... - выделяется следующая после видимых страница (5).
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElements(By.LinkText("..."))[0].Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage41", Driver.GetGridCell(0, "Name").Text, "... page (5-1)");
            VerifyAreEqual("FunnelPage50", Driver.GetGridCell(9, "Name").Text, "... page (5-10)");
            //... - back (2)
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElements(By.LinkText("..."))[0].Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage11", Driver.GetGridCell(0, "Name").Text, "... page (2-1)");
            VerifyAreEqual("FunnelPage20", Driver.GetGridCell(9, "Name").Text, "... page (2-10)");

            //page 4
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("4")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage31", Driver.GetGridCell(0, "Name").Text, "number page (4-1)");
            VerifyAreEqual("FunnelPage40", Driver.GetGridCell(9, "Name").Text, "number page (4-10)");

            //last
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Driver.WaitForAjax();
            //3
            Driver.FindElement(AdvBy.DataE2E("gridPagination")).FindElement(By.LinkText("3")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage21", Driver.GetGridCell(0, "Name").Text, "number page (3-1)");
            VerifyAreEqual("FunnelPage30", Driver.GetGridCell(9, "Name").Text, "number page (3-10)");

            //first
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").Text, "number page (1-1)");
            VerifyAreEqual("FunnelPage10", Driver.GetGridCell(9, "Name").Text, "number page (1-10)");
        }

        [Test]
        [Order(1)]
        public void GridPaginationPageSize()
        {
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").Text, "first grid cell (10)");
            VerifyAreEqual("FunnelPage10", Driver.GetGridCell(9, "Name").Text, "last grid cell (10)");
            VerifyAreEqual(10, Driver.FindElements(AdvBy.DataE2E("gridRow")).Count, "grid cells count (10)");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").Text, "first grid cell (100)");
            VerifyAreEqual("FunnelPage52", Driver.GetGridCell(51, "Name").Text, "last grid cell (100)");
            VerifyAreEqual(52, Driver.FindElements(AdvBy.DataE2E("gridRow")).Count, "grid cells count (100)");

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        [Order(1)]
        public void GridSearch()
        {
            //часть заголовка, целый заголовок

            //search by part of exist header
            Driver.GridFilterSendKeys("ge4");

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("FunnelPage4"));
            VerifyIsTrue(Driver.GetGridCell(3, "Name").Text.Contains("FunnelPage42"));
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text);

            //search by of exist header
            Driver.GridFilterSendKeys("FunnelPage17");

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("FunnelPage17"));
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text);

            //search by not exist name
            Driver.GridFilterSendKeys("FunnelPage5000");
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
            Driver.GetGridCell(-1, "Name").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("FunnelPage18", Driver.GetGridCell(9, "Name").Text);

            Driver.GetGridCell(-1, "Name").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage9", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("FunnelPage48", Driver.GetGridCell(9, "Name").Text);

            //sort by enabled
            Driver.GetGridCell(-1, "Enabled").Click();
            Driver.WaitForAjax();
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "Enabled").FindElement(By.TagName("input")).Selected);

            Driver.GetGridCell(-1, "Enabled").Click();
            Driver.WaitForAjax();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "Enabled").FindElement(By.TagName("input")).Selected);

            //sort by CreatedDateFormatted date
            Driver.GetGridCell(-1, "CreatedDateFormatted").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("03.06.2021 18:14", Driver.GetGridCell(0, "CreatedDateFormatted").Text);
            VerifyAreEqual("03.06.2021 18:14", Driver.GetGridCell(9, "CreatedDateFormatted").Text);

            Driver.GetGridCell(-1, "CreatedDateFormatted").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("03.06.2021 18:16", Driver.GetGridCell(0, "CreatedDateFormatted").Text);
            VerifyAreEqual("03.06.2021 18:14", Driver.GetGridCell(9, "CreatedDateFormatted").Text);

            //sort by main
            Driver.GetGridCell(-1, "IsMain").Click();
            Driver.WaitForAjax();
            VerifyIsFalse(Driver.GetGridCell(0, "IsMain").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "IsMain").FindElement(By.TagName("input")).Selected);

            Driver.GetGridCell(-1, "IsMain").Click();
            Driver.WaitForAjax();
            VerifyIsTrue(Driver.GetGridCell(0, "IsMain").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(1, "IsMain").FindElement(By.TagName("input")).Selected);

            Driver.GetGridCell(-1, "IsMain").Click();
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
            Driver.GetGridCell(1, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalCancel();
            Driver.WaitForAjax();
            VerifyAreEqual("FunnelPage2", Driver.GetGridCell(1, "Name").Text);

            //check delete
            Driver.GetGridCell(1, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            Driver.WaitForAjax();
            VerifyAreNotEqual("FunnelPage2", Driver.GetGridCell(1, "Name").Text);

            //check delete selected items
            SelectGridRow(0);
            SelectGridRow(1);
            SelectGridRow(3);
            SelectGridRow(4);

            RemoveSelectedGridRows();
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("FunnelPage4", Driver.GetGridCell(1, "Name").Text);
            VerifyAreNotEqual("FunnelPage6", Driver.GetGridCell(2, "Name").Text);
            VerifyAreNotEqual("FunnelPage7", Driver.GetGridCell(3, "Name").Text);
            VerifyAreEqual("Найдено записей: 47", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "deleted some at page");

            //check delete all on page
            SelectGridRow(-1);
            RemoveSelectedGridRows();
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("FunnelPage24", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("Найдено записей: 39", Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Text,
                "deleted all page");

            //check delete all
            SelectGridRow(-1);
            Driver.FindElement(AdvBy.DataE2E("gridSelectionSelectAllFn")).Click();
            RemoveSelectedGridRows();
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").Text);

            Refresh();
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").Text);
        }

        protected void SelectGridRow(int rowIndex)
        {
            SelectGridRow(rowIndex, "");
        }

        protected void UnselectGridRow(int rowIndex)
        {
            UnselectGridRow(rowIndex, "");
        }
    }
}