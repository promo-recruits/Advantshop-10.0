using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Brands
{
    [TestFixture]
    public class BrandTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Brands\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\Brands\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Brands\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Brands\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Brands\\Catalog.ProductCategories.csv"
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
        [Order(1)]
        public void BrandSort()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            //SortByName
            Driver.GetGridCell(-1, "BrandName").Click();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName12", Driver.GetGridCell(9, "BrandName").Text);
            Driver.GetGridCell(-1, "BrandName").Click();
            VerifyAreEqual("BrandName99", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName90", Driver.GetGridCell(9, "BrandName").Text);

            //SortByCountry
            Driver.GetGridCell(-1, "CountryName").Click();
            VerifyAreEqual("Реюньон", Driver.GetGridCell(0, "CountryName").Text);
            Driver.GetGridCell(-1, "CountryName").Click();
            VerifyAreEqual("Япония", Driver.GetGridCell(0, "CountryName").Text);

            //SortByActivity
            Driver.GetGridCell(-1, "Enabled").Click();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            Driver.GetGridCell(-1, "Enabled").Click();
            VerifyAreEqual("BrandName6", Driver.GetGridCell(0, "BrandName").Text);

            //SortByOrder
            Driver.GetGridCell(-1, "SortOrder").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder").Text);
            Driver.GetGridCell(-1, "SortOrder").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("BrandName105", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("105", Driver.GetGridCell(0, "SortOrder").Text);
            Driver.GetGridCell(-1, "SortOrder").Click();
            Thread.Sleep(1000);
        }

        [Test]
        [Order(2)]
        public void BrandPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(99, "BrandName").Text);

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        [Order(2)]
        public void BrandSearchAndDoToEdit()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            //search exist product
            Driver.GridFilterSendKeys("BrandName5");
            VerifyAreEqual("BrandName5", Driver.GetGridCell(0, "BrandName").Text);

            //search not exist product
            Driver.GridFilterSendKeys("Brand5");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search too much symbols
            Driver.GridFilterSendKeys(
                    "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", By.ClassName("ui-grid-custom-filter-total"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //go to edit
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridFilterSendKeys("BrandName1");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            Driver.GetGridCell(0, "BrandName").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Производитель \"BrandName1\"", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("BrandName1", Driver.FindElement(By.Id("BrandName")).GetAttribute("value"));
            VerifyIsTrue(Driver.Url.Contains("brands/edit/1"));
        }

        [Test]
        [Order(3)]
        public void BrandSelectAndDelete()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();

            //check delete cancel
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);

            //check delete
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
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("BrandName15", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName24", Driver.GetGridCell(9, "BrandName").Text);

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("91", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

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

            GoToAdmin("settingscatalog#?catalogTab=brand");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        [Order(0)]
        public void BrandCheckSort()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            VerifyAreEqual("BrandName7", Driver.GetGridCell(6, "BrandName").Text);
            VerifyAreEqual("7", Driver.GetGridCell(6, "SortOrder").Text);

            //check client
            GoToClient("manufacturers");

            var element = Driver.FindElements(By.CssSelector(".brand-item.col-xs-12.col-sm-6.col-md-4.col-lg-3"))[0];
            IJavaScriptExecutor jse = (IJavaScriptExecutor) Driver;
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);

            VerifyAreEqual("BrandName7",
                Driver.FindElements(By.CssSelector(".brand-item.col-xs-12.col-sm-6.col-md-4.col-lg-3"))[1]
                    .FindElement(By.ClassName("brand-name")).Text);

            //check inplace sort order
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Thread.Sleep(100);//костыль
            Driver.SendKeysGridCell("1", 6, "SortOrder");
            VerifyAreEqual("1", Driver.GetGridCell(6, "SortOrder").Text);

            //check client
            GoToClient("manufacturers");
            var element2 = Driver.FindElements(By.CssSelector(".brand-item.col-xs-12.col-sm-6.col-md-4.col-lg-3"))[0];
            IJavaScriptExecutor jse2 = (IJavaScriptExecutor) Driver;
            jse2.ExecuteScript("arguments[0].scrollIntoView(true)", element2);

            VerifyAreEqual("BrandName7",
                Driver.FindElements(By.CssSelector(".brand-item.col-xs-12.col-sm-6.col-md-4.col-lg-3"))[0]
                    .FindElement(By.ClassName("brand-name")).Text);

            //return 
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.SendKeysGridCell("7", 1, "SortOrder");
        }

        [Test]
        [Order(1)]
        public void BrandDoActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);

            //do selected items active 
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();
            VerifyAreEqual("BrandName2", Driver.GetGridCell(1, "BrandName").Text);
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyAreEqual("BrandName3", Driver.GetGridCell(2, "BrandName").Text);
            VerifyIsTrue(Driver.GetGridCell(2, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
        }

        [Test]
        [Order(1)]
        public void BrandDoNotActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GetGridCell(5, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.GetGridCell(5, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyAreEqual("BrandName6", Driver.GetGridCell(5, "BrandName").Text);

            //do selected items not active 
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Driver.GetGridCell(6, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(7, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"2\"]")).Click();
            VerifyAreEqual("BrandName7", Driver.GetGridCell(6, "BrandName").Text);
            VerifyIsFalse(Driver.GetGridCell(6, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyAreEqual("BrandName8", Driver.GetGridCell(7, "BrandName").Text);
            VerifyIsFalse(Driver.GetGridCell(7, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);

            //do all items on page not active

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"2\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
        }
    }
}