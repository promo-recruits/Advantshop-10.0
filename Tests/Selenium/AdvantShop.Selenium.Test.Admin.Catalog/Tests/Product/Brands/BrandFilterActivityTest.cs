using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Brands
{
    [TestFixture]
    public class BrandFilterActivityTest : BaseSeleniumTest
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
        public void BrandFilterActivityAOnPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enabled", filterItem: "Активные");
            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName110", Driver.GetGridCell(9, "BrandName").Text);

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName200", Driver.GetGridCell(99, "BrandName").Text);

            //close
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Enabled");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(99, "BrandName").Text);
        }

        [Test]
        public void BrandFilterActivityBOffPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enabled", filterItem: "Неактивные");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(99, "BrandName").Text);
        }

        [Test]
        public void BrandFilterActivityOnSelectAndDelete()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enabled", filterItem: "Активные");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);

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
            VerifyAreNotEqual("BrandName102", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreNotEqual("BrandName103", Driver.GetGridCell(1, "BrandName").Text);
            VerifyAreNotEqual("BrandName104", Driver.GetGridCell(2, "BrandName").Text);

            //check select all on page
            Driver.GridPaginationSelectItems("50");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(49, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("BrandName155", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName200", Driver.GetGridCell(45, "BrandName").Text);
            VerifyAreEqual("Найдено записей: 46",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //check select all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("46", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(45, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //close filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "Enabled");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterActivityOnDoNotActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enabled", filterItem: "Активные");
            Driver.GridPaginationSelectItems("20");
            Driver.ScrollToTop();
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected, "");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);

            //do selected items not active 
            Refresh();
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"2\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreNotEqual("BrandName2", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreNotEqual("BrandName6", Driver.GetGridCell(1, "BrandName").Text);

            //close
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Enabled");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(19, "BrandName").Text);
        }

        [Test]
        public void BrandFilterActivityOffDoActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enabled", filterItem: "Неактивные");
            Driver.GridPaginationSelectItems("20");
            Driver.ScrollToTop();
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);

            //do selected items active 
            Refresh();
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(4, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreNotEqual("BrandName2", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreNotEqual("BrandName6", Driver.GetGridCell(4, "BrandName").Text);
        }

        [Test]
        public void BrandFilterActivityAOnGoToEditAndBack()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enabled", filterItem: "Активные");
            Driver.GridPaginationSelectItems("10");

            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName110", Driver.GetGridCell(9, "BrandName").Text);

            Driver.GetGridCell(0, "BrandName").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Производитель \"BrandName101\"", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("BrandName101", Driver.FindElement(By.Id("BrandName")).GetAttribute("value"));
            VerifyIsTrue(Driver.Url.Contains("brands/edit/101"));

            GoBack();
            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName110", Driver.GetGridCell(9, "BrandName").Text);
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Enabled\"]"))
                .Displayed);
        }
    }
}