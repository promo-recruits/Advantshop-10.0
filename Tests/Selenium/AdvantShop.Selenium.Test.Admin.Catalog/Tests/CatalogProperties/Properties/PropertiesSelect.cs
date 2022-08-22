using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class PropertiesSelect : BaseSeleniumTest
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
        public void SelectItemPresentDefault()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"grid[0][\'selectionRowHeaderCol\']\"] [data-e2e=\"gridCheckboxWrapSelect\"]"))
                .Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
        }

        [Test]
        public void SelectAllItemPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("101", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        
            Driver.GridPaginationSelectItems("100");
            Driver.WaitForElem(By.CssSelector("[data-e2e-grid-cell=\"grid[99][\'Name\']\"]"));
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(99, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("101", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        }

        [Test]
        public void SelectAllOnPageItemPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        
            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("Property100",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[99][\'Name\']\"]")).Text);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(99, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("100", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        }

        [Test]
        public void DeSelectAllItemPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("101", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Selected);
        
            Driver.GridPaginationSelectItems("100");
            // Driver.WaitForElem(By.CssSelector("[data-e2e-grid-cell=\"grid[99][\'Name\']\"]"));
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Selected);
        }

        //delet
        [Test]
        public void SelectItemzDeletCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
        }

        [Test]
        public void SelectItemzDeletOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Property2", Driver.GetGridCell(0, "Name").Text);
        }

        [Test]
        public void SelectItemzDeletWithoutSelectCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Property2", Driver.GetGridCell(0, "Name").Text);
        }

        [Test]
        public void SelectItemzDeletWithoutSelectOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("Property2", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property3", Driver.GetGridCell(0, "Name").Text);
        }

        [Test]
        public void SelectItemzDelOnPageCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Property3", Driver.GetGridCell(0, "Name").Text);
        }

        [Test]
        public void SelectItemzDelOnPageOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Property13", Driver.GetGridCell(0, "Name").Text);
        }

        [Test]
        public void SelectItemzDelzAllCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void SelectItemzDelzAllOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void SelectItemChangeGroup()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"grid[0][\'selectionRowHeaderCol\']\"] [data-e2e=\"gridCheckboxWrapSelect\"]"))
                .Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-9 select")))).SelectByText("Group11");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            VerifyAreEqual("Group11", Driver.GetGridCell(0, "GroupName").Text);
            VerifyAreNotEqual("Group11", Driver.GetGridCell(1, "GroupName").Text);
            GoToAdmin("settingscatalog?groupId=11");
            VerifyIsTrue(1 == Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]"))
                .Count);
        }

        [Test]
        public void SelectItemChangeGroupOnPage()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-9 select")))).SelectByText("Group3");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            VerifyAreEqual("Group3", Driver.GetGridCell(0, "GroupName").Text);
            VerifyAreEqual("Group3", Driver.GetGridCell(9, "GroupName").Text);
            GoToAdmin("settingscatalog?groupId=3");
            VerifyIsTrue(10 == Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]"))
                .Count);
        }

        [Test]
        public void SelectItemChangeGroupAll()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();
            (new SelectElement(Driver.FindElement(By.CssSelector(".col-xs-9 select")))).SelectByText("Group2");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            VerifyAreEqual("Group2", Driver.GetGridCell(0, "GroupName").Text);
            VerifyAreEqual("Group2", Driver.GetGridCell(9, "GroupName").Text);
            GoToAdmin("settingscatalog?groupId=2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("101", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        }
    }
}