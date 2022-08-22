using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Brands
{
    [TestFixture]
    public class BrandFilterSortTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.ProductCategories.csv"
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

        [Test]
        public void BrandFilterSortPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "60");

            VerifyAreEqual("BrandName10", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName19", Driver.GetGridCell(9, "BrandName").Text);

            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Driver.SetGridFilterRange("SortOrder", "100", "200");

            VerifyAreEqual("BrandName100", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName199", Driver.GetGridCell(99, "BrandName").Text);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "SortOrder");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(99, "BrandName").Text);
        }

        [Test]
        public void BrandFilterSortSelectAndDelete()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "50");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("BrandName10", Driver.GetGridCell(0, "BrandName").Text);

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
            VerifyAreEqual("3", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreNotEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreNotEqual("BrandName12", Driver.GetGridCell(1, "BrandName").Text);
            VerifyAreNotEqual("BrandName13", Driver.GetGridCell(2, "BrandName").Text);

            //check select all on page
            Refresh();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("BrandName24", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName33", Driver.GetGridCell(9, "BrandName").Text);
            VerifyAreEqual("Найдено записей: 27",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //check select all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("27", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

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
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //close filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "SortOrder");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName51", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterSortDoActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridPaginationSelectItems("20");
            Driver.ScrollToTop();
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "50");
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(0, "BrandName").Text);

            //do selected items active 
            Refresh();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(3, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("BrandName11", Driver.GetGridCell(1, "BrandName").Text);
            VerifyAreEqual("BrandName13", Driver.GetGridCell(3, "BrandName").Text);
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyIsTrue(Driver.GetGridCell(3, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "SortOrder");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(19, "BrandName").Text);
        }

        [Test]
        public void BrandFilterSortDoNotActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridPaginationSelectItems("20");
            Driver.ScrollToTop();
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "101", "150");
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);

            //do selected items not active 
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(3, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"2\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(3, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyAreEqual("BrandName102", Driver.GetGridCell(1, "BrandName").Text);
            VerifyAreEqual("BrandName104", Driver.GetGridCell(3, "BrandName").Text);
        }

        [Test]
        public void BrandFilterSortGoToEditAndBack()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "20");

            VerifyAreEqual("BrandName10", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName19", Driver.GetGridCell(9, "BrandName").Text);

            Driver.GetGridCell(0, "BrandName").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("Производитель \"BrandName10\"", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("BrandName10", Driver.FindElement(By.Id("BrandName")).GetAttribute("value"));
            VerifyIsTrue(Driver.Url.Contains("brands/edit/10"));

            GoBack();
            VerifyAreEqual("BrandName10", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName19", Driver.GetGridCell(9, "BrandName").Text);
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"SortOrder\"]"))
                .Displayed);
        }
    }
}