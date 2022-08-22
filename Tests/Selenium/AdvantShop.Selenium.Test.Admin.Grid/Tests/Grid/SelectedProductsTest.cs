using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Grid.Tests.Grid
{
    [TestFixture]
    public class GridSelectedProductsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Grid\\SelectProductsTest\\Catalog.Product.csv",
                "data\\Admin\\Grid\\SelectProductsTest\\Catalog.Offer.csv",
                "data\\Admin\\Grid\\SelectProductsTest\\Catalog.Category.csv",
                "data\\Admin\\Grid\\SelectProductsTest\\Catalog.ProductCategories.csv");

            Init();
            GoToAdmin("catalog?categoryid=1");
            if (Driver.FindElements(By.CssSelector(".sidebar sidebar--default")).Count == 0)
                Driver.FindElement(By.CssSelector(".burger")).Click();
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

        [Test, Order(1)]
        public void ProductInCategoryPresent()
        {
            GoToAdmin("catalog?categoryid=1");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Найдено записей: 20",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyAreEqual("98/100",
                Driver.FindElements(By.CssSelector(".aside-menu-inner"))[0]
                    .FindElement(By.CssSelector(".aside-menu-count")).Text);
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));

            GoToAdmin("catalog?showMethod=AllProducts");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct99", Driver.GetGridCellText(99, "Name"));

            Driver.GridPaginationSelectItems("10");
        }

        [Test, Order(1)]
        public void ProductAllOnPageSelect()
        {
            GoToAdmin("catalog?categoryid=1");
            Driver.GridReturnDefaultView10(BaseUrl);

            //выбрать все товары на странице
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            Refresh();

            Driver.GridPaginationSelectItems("20");
            Driver.ScrollTo(By.TagName("h2"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(19, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("20", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        }

        [Test, Order(1)]
        public void ProductSelectDeselect()
        {
            GoToAdmin("catalog?categoryid=1");
            Driver.GridReturnDefaultView10(BaseUrl);

            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "begin no select line 1 page 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "begin no select line 10 page 1");

            //select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();


            VerifyAreEqual("20", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "select all count");
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all line 1 page 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all line 10 page 1");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all line 1 page 2");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all line 10 page 2");

            //deselect only viewed
            Driver.ScrollTo(By.TagName("h2"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();

            Driver.XPathContainsText("span", "Снять выделение с видимых");


            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect only viewed line 1 page 2");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect only viewed line 10 page 2");
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "deselect only viewed count");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();

            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select (no viewed) line 1 page 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select (no viewed) line 10 page 1");
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "deselect only viewed count (no viewed page)");

            //deselect all
            Driver.ScrollTo(By.TagName("h2"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();

            Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"" +
                    (Driver.FindElements(By.CssSelector("[data-e2e=\"gridSelectionDropdownItem\"]")).Count - 1) +
                    "\"]"))
                .Click();


            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Displayed,
                "deselect all no count");
            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect all line 1 page 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect all line 10 page 1");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect all line 1 page 2");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect all line 10 page 2");
        }

        [Test, Order(1)]
        public void ProductDeselect()
        {
            GoToAdmin("catalog?categoryid=1");
            Driver.GridReturnDefaultView10(BaseUrl);

            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "begin no select line 1 page 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "begin no select line 10 page 1");

            //select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();


            VerifyAreEqual("20", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "select all count");
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all line 1 page 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all line 10 page 1");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all line 1 page 2");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all line 10 page 2");

            //deselect by header checkbox
            Driver.ScrollTo(By.TagName("h2"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"] span")).Click();


            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect by header checkbox line 1 page 2");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect by header checkbox line 10 page 2");
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "deselect by header checkbox count");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Displayed,
                "deselect by header checkbox no link");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();

            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select (no by header checkbox page) line 1 page 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select (no by header checkbox page) line 10 page 1");
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "deselect only viewed count (no by header checkbox page) page 1");

            //deselect all by link
            Driver.ScrollTo(By.TagName("h2"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();


            VerifyAreEqual("20", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "select all by link");
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all by link line 1 page 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all by link line 10 page 1");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();


            VerifyAreEqual("20", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "select all by link count");
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all by link line 1 page 2");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "select all by link line 10 page 2");

            Driver.ScrollTo(By.TagName("h2"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();


            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Displayed,
                "deselect all by link no count page 2");
            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect all by link line 1 page 2");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect all by link line 10 page 2");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();


            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Displayed,
                "deselect all by link no count page 1");
            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect all by link line 1 page 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "deselect all by link line 10 page 1");
        }

        /* 2, 3 товары неактивны в CSV */
        [Test, Order(1)]
        public void ProductChangeActive()
        {
            GoToAdmin("catalog?categoryid=1");

            //check do not active
            Driver.GetGridCell(4, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(3, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.XPathContainsText("span", "Сделать неактивными");
            Refresh();

            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(3, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(4, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);

            //check do active
            Refresh();

            Driver.GetGridCell(4, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.XPathContainsText("span", "Сделать активными");

            Refresh();

            VerifyIsTrue(Driver.GetGridCell(4, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
        }

        [Test, Order(3)]
        public void ProductSelectAndDelete()
        {
            GoToAdmin("catalog?categoryid=4");
            Driver.GridReturnDefaultView10(BaseUrl);
            //check delete from category
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();
            Driver.SwalConfirm();
            //  Refresh();
            VerifyAreNotEqual("TestProduct61", Driver.GetGridCellText(0, "Name"));
            VerifyAreNotEqual("TestProduct62", Driver.GetGridCellText(1, "Name"));

            GoToAdmin("catalog?showMethod=OnlyWithoutCategories");

            VerifyAreEqual("TestProduct61", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct62", Driver.GetGridCellText(1, "Name"));

            //check select item
            GoToAdmin("catalog?categoryid=5");
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete cancel
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(
                By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'_serviceColumn\']\"] ui-grid-custom-delete")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("TestProduct81", Driver.GetGridCellText(0, "Name"));

            //check delete
            Driver.FindElement(
                By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'_serviceColumn\']\"] ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("TestProduct81", Driver.GetGridCellText(0, "Name"));

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreNotEqual("TestProduct82", Driver.GetGridCellText(0, "Name"));
            VerifyAreNotEqual("TestProduct83", Driver.GetGridCellText(1, "Name"));
            VerifyAreNotEqual("TestProduct84", Driver.GetGridCellText(2, "Name"));

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("TestProduct95", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct100", Driver.GetGridCellText(5, "Name"));

            //check select all
            GoToAdmin("catalog?showMethod=AllProducts");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("86", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            GoToAdmin("catalog?showMethod=AllProducts");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test, Order(2)]
        public void ProductSelectedToAnotherCategory()
        {
            GoToAdmin("catalog?categoryid=1");

            //to another category
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"3\"]")).Click();

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Добавить товары')]")).Click();

            Refresh();
            //move to another category
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(3, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"2\"]")).Click();

            Driver.FindElement(By.Id("2")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Перенести товары')]")).Click();

            //check first category
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct2", Driver.GetGridCellText(1, "Name"));

            VerifyAreNotEqual("TestProduct3", Driver.GetGridCellText(2, "Name"));
            VerifyAreNotEqual("TestProduct4", Driver.GetGridCellText(3, "Name"));

            //check others categories
            GoToAdmin("catalog?categoryid=3");
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct2", Driver.GetGridCellText(1, "Name"));

            GoToAdmin("catalog?categoryid=2");
            VerifyAreEqual("TestProduct3", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct4", Driver.GetGridCellText(1, "Name"));
        }
    }
}