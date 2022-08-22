using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Tag
{
    [TestFixture]
    public class TagSelect : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Catalog\\Tag\\Catalog.Category.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.Tag.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.TagMap.csv"
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
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"gridTags[0][\'selectionRowHeaderCol\']\"] [data-e2e=\"gridCheckboxWrapSelect\"]"))
                .Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
        }

        [Test]
        public void SelectAllItemPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("105", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(99, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("105", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void SelectAllOnPageItemPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        
            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(99, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("100", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        }

        [Test]
        public void DeSelectAllItemPresent10()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("105", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Selected);
        
            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Selected);
        }

        //delet
        [Test]
        public void SelectItemzDeletCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("New_Tag1",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridTags[0][\'Name\']\"]")).Text);
        }

        [Test]
        public void SelectItemzDeletOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("New_Tag2",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridTags[0][\'Name\']\"]")).Text);
        }

        [Test]
        public void SelectItemzDeletWithoutSelectCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.FindElement(
                    By.CssSelector("[data-e2e-grid-cell=\"gridTags[0][\'_serviceColumn\']\"] ui-grid-custom-delete"))
                .Click(); //!!!!!!!!!!!!!!!!!!!!!!
            Driver.SwalCancel();
            VerifyAreEqual("New_Tag2",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridTags[0][\'Name\']\"]")).Text);
        }

        [Test]
        public void SelectItemzDeletWithoutSelectOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.FindElement(
                    By.CssSelector("[data-e2e-grid-cell=\"gridTags[0][\'_serviceColumn\']\"] ui-grid-custom-delete"))
                .Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("New_Tag2",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridTags[0][\'Name\']\"]")).Text);
            VerifyAreEqual("New_Tag3",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridTags[0][\'Name\']\"]")).Text);
        }

        [Test]
        public void SelectItemzDelOnPageCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("New_Tag3",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridTags[0][\'Name\']\"]")).Text);
        }

        [Test]
        public void SelectItemzDelOnPageOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Tags")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("New_Tag13",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridTags[0][\'Name\']\"]")).Text);
        }

        [Test]
        public void SelectItemzDelzAllCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
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
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }
    }
}