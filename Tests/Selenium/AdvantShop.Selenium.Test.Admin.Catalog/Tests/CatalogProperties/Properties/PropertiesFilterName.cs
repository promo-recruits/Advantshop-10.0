using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class PropertiesFilterName : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Properties\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Properties\\Catalog.Category.csv",
                "Data\\Admin\\Properties\\Catalog.Brand.csv",
                "Data\\Admin\\Properties\\Catalog.Property.csv",
                "Data\\Admin\\Properties\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Properties\\Catalog.Product.csv",
                "Data\\Admin\\Properties\\Catalog.Offer.csv",
                "Data\\Admin\\Properties\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\Properties\\Catalog.ProductCategories.csv"
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
        public void ByName()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Functions.GridFilterSet(Driver, BaseUrl, "Name");
            Driver.SetGridFilterValue("Name", "Property10");

            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property101", Driver.GetGridCell(2, "Name").Text);
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);


            Driver.GridPaginationSelectItems("50");
            Driver.ScrollToTop();

            Functions.GridFilterSet(Driver, BaseUrl, "Name");
            Driver.SetGridFilterValue("Name", "Property10");

            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property101", Driver.GetGridCell(2, "Name").Text);
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            Thread.Sleep(1000);
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property50", Driver.GetGridCell(49, "Name").Text);

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void ByNameAndFilter()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Functions.GridFilterSet(Driver, BaseUrl, "Name");
            Driver.SetGridFilterValue("Name", "Property2");

            VerifyAreEqual("Property2", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property28", Driver.GetGridCell(9, "Name").Text);

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "UseInFilter", filterItem: "Нет");

            VerifyAreEqual("Property2", Driver.GetGridCell(0, "Name").Text);

            //close name
            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property5", Driver.GetGridCell(4, "Name").Text);
            //close filter
            Functions.GridFilterClose(Driver, BaseUrl, "UseInFilter");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void ByNameAndSort()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Functions.GridFilterSet(Driver, BaseUrl, "Name");
            Driver.SetGridFilterValue("Name", "Property1");

            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property18", Driver.GetGridCell(9, "Name").Text);

            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "1", "10");

            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(1, "Name").Text);

            //close name
            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            //close sort
            Functions.GridFilterClose(Driver, BaseUrl, "SortOrder");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void ByFilterAndDetails()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "UseInFilter", filterItem: "Да");

            VerifyAreEqual("Property6", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property15", Driver.GetGridCell(9, "Name").Text);

            Functions.GridFilterSet(Driver, BaseUrl, "UseInDetails");
            Driver.DropFocusCss("[data-e2e=\"PropertySettingTitle\"]");
            Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"UseInDetails\"] [data-e2e=\"gridFilterItemSelect\"]"))
                .Click();
            Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"UseInDetails\"] [data-e2e=\"gridFilterItemSelect\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"Да\"]")).Click();
            Driver.DropFocusCss("[data-e2e=\"PropertySettingTitle\"]");

            VerifyAreEqual("Property6", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property20", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollToTop();
            //close details
            Functions.GridFilterClose(Driver, BaseUrl, "UseInDetails");
            VerifyAreEqual("Property6", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property15", Driver.GetGridCell(9, "Name").Text);
            //close brief
            Functions.GridFilterClose(Driver, BaseUrl, "UseInFilter");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void ByNameGoToEditAndBack()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Functions.GridFilterSet(Driver, BaseUrl, "Name");
            Driver.SetGridFilterValue("Name", "Property10");

            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property101", Driver.GetGridCell(2, "Name").Text);


            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'_serviceColumn\']\"] a")).Click();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property101", Driver.GetGridCell(2, "Name").Text);
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"]"))
                .Displayed);
        }

        /*
        [Test]
        public void ByNameGoToDetailsAndBack()
        {
             GoToAdmin("settingscatalog#?catalogTab=properties");
            Functions.GridFilterSet(driver, baseURL, "Name");
            Driver.SetGridFilterValue("Name", "Property10"); 
            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property101", Driver.GetGridCell(2, "Name").Text);

            driver.FindElement(By.CssSelector(".fa.fa-list")).Click();
            Thread.Sleep(1000);
            Refresh();
            VerifyAreEqual("Значения свойства - \"Property10\"", driver.FindElement(By.TagName("h1")).Text);
            GoBack();
            GoBack();
            Thread.Sleep(1000);
            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property101", Driver.GetGridCell(2, "Name").Text);
            VerifyIsTrue(driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"]")).Displayed);
        }*/
        [Test]
        public void ByNamezDelEl()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.ScrollToTop();
            Functions.GridFilterSet(Driver, BaseUrl, "Name");
            Driver.SetGridFilterValue("Name", "Property2");
            VerifyAreEqual("Property2", Driver.GetGridCell(0, "Name").Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Property29", Driver.GetGridCell(0, "Name").Text);

            Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"] [data-e2e=\"gridFilterItemClose\"]"))
                .Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property3", Driver.GetGridCell(1, "Name").Text);
            VerifyAreEqual("Property11", Driver.GetGridCell(9, "Name").Text);
            VerifyIsFalse(Driver
                              .FindElements(
                                  By.CssSelector(
                                      "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"]"))
                              .Count >
                          0);
        }
    }
}