using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Colors
{
    [TestFixture]
    public class ColorSelect : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Color\\Catalog.Color.csv"
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
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"gridColors[0][\'selectionRowHeaderCol\']\"] [data-e2e=\"gridCheckboxWrapSelect\"]"))
                .Click();
            
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
        }

        [Test]
        public void SelectAllItemPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("111", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);


            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(99, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("111", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        }

        [Test]
        public void SelectAllOnPageItemPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(99, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("100", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        }

        [Test]
        public void DeSelectAllItemPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("111", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();

            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Selected);

            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();

            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Selected);
        }

        [Test]
        public void SelectItemzDeletCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"gridColors[0][\'selectionRowHeaderCol\']\"] [data-e2e=\"gridCheckboxWrapSelect\"]"))
                .Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
        }

        [Test]
        public void SelectItemzDeletOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"gridColors[0][\'selectionRowHeaderCol\']\"] [data-e2e=\"gridCheckboxWrapSelect\"]"))
                .Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("Color1", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color2", Driver.GetGridCell(0, "ColorName", "Colors").Text);
        }

        [Test]
        public void SelectItemzDeletWithoutSelectCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.GetGridCell(0, "_serviceColumn", "Colors")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Color2", Driver.GetGridCell(0, "ColorName", "Colors").Text);
        }

        [Test]
        public void SelectItemzDeletWithoutSelectOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.GetGridCell(0, "_serviceColumn", "Colors")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("Color2", Driver.GetGridCell(0, "ColorName", "Colors").Text);
            VerifyAreEqual("Color3", Driver.GetGridCell(0, "ColorName", "Colors").Text);
        }

        [Test]
        public void SelectItemzDelOnPageCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Color3", Driver.GetGridCell(0, "ColorName", "Colors").Text);
        }

        [Test]
        public void SelectItemzDelOnPageOk()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Colors")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Color13", Driver.GetGridCell(0, "ColorName", "Colors").Text);
        }

        [Test]
        public void SelectItemzDelzAllCancel()
        {
            GoToAdmin("settingscatalog#?catalogTab=colors");
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
            GoToAdmin("settingscatalog#?catalogTab=colors");
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