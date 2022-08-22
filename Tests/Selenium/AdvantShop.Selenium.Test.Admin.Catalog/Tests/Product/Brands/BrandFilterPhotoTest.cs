using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Brands
{
    [TestFixture]
    public class BrandFilterPhotoTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Photo.csv",
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
        public void BrandFilterPhoto()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            VerifyAreEqual("Найдено записей: 200",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //check with photo
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "С фотографией");
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsFalse(Driver.GetGridCell(9, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //check without photo
            GoToAdmin("settingscatalog#?catalogTab=brand");

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "Без фотографии");
            VerifyIsTrue(Driver.GetGridCell(0, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.GetGridCell(9, "PhotoSrc").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Test]
        public void BrandFilterPhotoPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "С фотографией");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName130", Driver.GetGridCell(99, "BrandName").Text);

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void BrandFilterPhotoSelectAndDelete()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "С фотографией");

            //check delete
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);

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
            VerifyAreNotEqual("BrandName2", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreNotEqual("BrandName3", Driver.GetGridCell(1, "BrandName").Text);
            VerifyAreNotEqual("BrandName4", Driver.GetGridCell(2, "BrandName").Text);

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
            VerifyAreEqual("BrandName55", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName130", Driver.GetGridCell(45, "BrandName").Text);
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
            Functions.GridFilterClose(Driver, BaseUrl, name: "PhotoSrc");
            VerifyAreEqual("BrandName71", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Test]
        public void BrandFilterPhotoDoNotActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "Без фотографии");
            Driver.GridPaginationSelectItems("100");
            VerifyIsTrue(Driver.GetGridCell(99, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.GetGridCell(99, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Refresh();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyAreEqual("BrandName200", Driver.GetGridCell(99, "BrandName").Text);

            //do selected items not active 
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.GetGridCell(98, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(97, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"2\"]")).Click();
            Thread.Sleep(1000);
            Refresh();
            VerifyAreEqual("BrandName199", Driver.GetGridCell(98, "BrandName").Text);
            VerifyAreEqual("BrandName198", Driver.GetGridCell(97, "BrandName").Text);
            VerifyIsFalse(Driver.GetGridCell(98, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(97, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
        }

        [Test]
        public void BrandFilterPhotoDoActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "С фотографией");
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Refresh();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);

            //do selected items active 
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();
            Thread.Sleep(1000);
            Refresh();
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
        }

        [Test]
        public void BrandFilterPhotoGoToEditAndBack()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "С фотографией");

            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.GetGridCell(0, "BrandName").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Производитель \"BrandName1\"", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("BrandName1", Driver.FindElement(By.Id("BrandName")).GetAttribute("value"));
            VerifyIsTrue(Driver.Url.Contains("brands/edit/1"));

            GoBack();
            Thread.Sleep(1000);
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"PhotoSrc\"]"))
                .Displayed);
        }
    }
}