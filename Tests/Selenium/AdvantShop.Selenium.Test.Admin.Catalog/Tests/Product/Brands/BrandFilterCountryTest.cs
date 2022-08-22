using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Brands
{
    [TestFixture]
    public class BrandFilterCountryTest : BaseSeleniumTest
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
        public void BrandAFilterCountryPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Южный Судан");
            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("Южный Судан", Driver.GetGridCell(0, "CountryName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);
            VerifyAreEqual("Южный Судан", Driver.GetGridCell(9, "CountryName").Text);

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName190", Driver.GetGridCell(99, "BrandName").Text);

            //close
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "CountryName");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(99, "BrandName").Text);
        }

        [Test]
        public void BrandBFilterCountryNotExistPresent10()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Германия");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "CountryName");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterCountrySelectAndDelete()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Южный Судан");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);

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
            VerifyAreNotEqual("BrandName92", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreNotEqual("BrandName93", Driver.GetGridCell(1, "BrandName").Text);
            VerifyAreNotEqual("BrandName94", Driver.GetGridCell(2, "BrandName").Text);

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("BrandName105", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName114", Driver.GetGridCell(9, "BrandName").Text);

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("89", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

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

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "CountryName");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterCountryDoNotActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Южный Судан");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.ScrollToTop();
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);

            //do selected items not active 
            Refresh();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"2\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("BrandName102", Driver.GetGridCell(1, "BrandName").Text);
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyAreEqual("BrandName103", Driver.GetGridCell(2, "BrandName").Text);
            VerifyIsFalse(Driver.GetGridCell(2, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);

            //close
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "CountryName");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterCountryDoActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridPaginationSelectItems("20");
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Южный Судан");
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);

            //do selected items active 
            Refresh();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("BrandName92", Driver.GetGridCell(1, "BrandName").Text);
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyAreEqual("BrandName93", Driver.GetGridCell(2, "BrandName").Text);
            VerifyIsTrue(Driver.GetGridCell(2, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
        }

        [Test]
        public void BrandCFilterCountryGoToEditAndBack()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Россия");

            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName9", Driver.GetGridCell(8, "BrandName").Text);

            Driver.ScrollToTop();
            Driver.GetGridCell(0, "BrandName").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("Производитель \"BrandName1\"", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("BrandName1", Driver.FindElement(By.Id("BrandName")).GetAttribute("value"));
            VerifyIsTrue(Driver.Url.Contains("brands/edit/1"));
            GoBack();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName9", Driver.GetGridCell(8, "BrandName").Text);
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"CountryName\"]"))
                .Displayed);
        }

        /* more than 1 filters */
        [Test]
        public void ABrandTwoFiltersActivityAndCountry()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            Driver.GridPaginationSelectItems("50");
            Driver.ScrollToTop();

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Южный Судан");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterDropdownItem\"][data-e2e-filter-dropdown-name=\"Enabled\"]"))
                .Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Enabled\"] [data-e2e=\"gridFilterItemSelect\"]"))
                .Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Неактивные\"]")).Click();

            Driver.DropFocusCss("[data-e2e=\"BrandSettingTitle\"]");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);
            Driver.ScrollToTop();
            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "Enabled");
            Functions.GridFilterClose(Driver, BaseUrl, name: "CountryName");
            VerifyAreEqual("Найдено записей: 200",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }
    }
}