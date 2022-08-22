using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class PropertiesValue : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Properties\\PropertiesValue\\Catalog.Category.csv",
                "Data\\Admin\\Properties\\PropertiesValue\\Catalog.Brand.csv",
                "Data\\Admin\\Properties\\PropertiesValue\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Properties\\PropertiesValue\\Catalog.Property.csv",
                "Data\\Admin\\Properties\\PropertiesValue\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Properties\\PropertiesValue\\Catalog.Product.csv",
                "Data\\Admin\\Properties\\PropertiesValue\\Catalog.Offer.csv",
                "Data\\Admin\\Properties\\PropertiesValue\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\Properties\\PropertiesValue\\Catalog.ProductCategories.csv"
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
        public void OpenValueWindows()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.ScrollToTop();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"PropertyValueSettingTitle\"]"));
            VerifyAreEqual("10", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("0", Driver.GetGridCell(0, "SortOrder", "PropertyValues").Text);
            VerifyAreEqual("2", Driver.GetGridCell(0, "ProductsCount", "PropertyValues").Text);
            VerifyAreEqual(10,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            Driver.FindElement(By.LinkText("Вернуться назад")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
        }

        [Test]
        public void SearchValue()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridPaginationSelectItems("20");
            Driver.ScrollToTop();

            Driver.GridFilterSendKeys("40");
            VerifyAreEqual(2,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            VerifyAreEqual("40", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
        }

        [Test]
        public void FilterValue()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridPaginationSelectItems("20");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownItem\"]")).Click();
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Value\"]"))
                .Displayed);
            Driver.SetGridFilterValue("Value", "3");

            VerifyAreEqual(2,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"PropertyValueSettingTitle\"]"));
            VerifyAreEqual("30", Driver.GetGridCell(0, "Value", "PropertyValues").Text);

            Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Value\"] [data-e2e=\"gridFilterItemClose\"]"))
                .Click();
            Thread.Sleep(1000);
            VerifyAreEqual(14,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
        }

        [Test]
        public void Present20Value()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual(10,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual(14,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
        }

        [Test]
        public void SelectItemValue()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        }

        [Test]
        public void SelectAllItemValue()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("14", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Selected);
        }

        [Test]
        public void SelectAllOnPageItemValue()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        }

        [Test]
        public void ValuesDel()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridPaginationSelectItems("20");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridPropertyValues[0][\'_serviceColumn\']\"] a"))
                .Click();
            Driver.SwalConfirm();
            VerifyAreEqual(13,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            VerifyAreEqual("20", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
        }

        [Test]
        public void ValueSelectitemDelcencel()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("20", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
        }

        [Test]
        public void ValueSelectitemDelok()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("30", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
        }

        [Test]
        public void ValueSelectzAllOnPageItemDelcancel()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("30", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
        }

        [Test]
        public void ValueSelectzAllOnPageItemDelok()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("130", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
        }

        [Test]
        public void ValueSelectzAllzItemDelcancel()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void ValueSelectzAllzItemDelok()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "PropertyValues")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void Present10Page()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Refresh();
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("10", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("100", Driver.GetGridCell(9, "Value", "PropertyValues").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("110", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("140", Driver.GetGridCell(3, "Value", "PropertyValues").Text);
        }

        [Test]
        public void Present10PageToNext()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("10", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("100", Driver.GetGridCell(9, "Value", "PropertyValues").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("110", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("140", Driver.GetGridCell(3, "Value", "PropertyValues").Text);
        }

        [Test]
        public void Present10PageToPrevious()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("10", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("100", Driver.GetGridCell(9, "Value", "PropertyValues").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("110", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("140", Driver.GetGridCell(3, "Value", "PropertyValues").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("10", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("100", Driver.GetGridCell(9, "Value", "PropertyValues").Text);
        }

        [Test]
        public void Present10PageToEnd()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("10", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("100", Driver.GetGridCell(9, "Value", "PropertyValues").Text);
            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("110", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("140", Driver.GetGridCell(3, "Value", "PropertyValues").Text);
        }

        [Test]
        public void Present10PageToBegin()
        {
            GoToAdmin("settingscatalog#?propertyId=2");
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("10", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("100", Driver.GetGridCell(9, "Value", "PropertyValues").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("110", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("140", Driver.GetGridCell(3, "Value", "PropertyValues").Text);
            Thread.Sleep(1000);

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("10", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("100", Driver.GetGridCell(9, "Value", "PropertyValues").Text);
        }

        [Test]
        public void AddPropertyValuesCanсel()
        {
            GoToAdmin("settingscatalog#?propertyId=1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyValueSettingAdd\"]")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            Driver.FindElement(By.CssSelector(".col-xs-9 input")).SendKeys("New_proprtyvalue");
            Driver.FindElements(By.CssSelector(".col-xs-9 input"))[1].Clear();
            Driver.FindElements(By.CssSelector(".col-xs-9 input"))[1].SendKeys("2");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"PropertyValueSettingTitle\"]"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void AddPropertyValuesOk()
        {
            GoToAdmin("settingscatalog#?propertyId=1");
            Refresh();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyValueSettingAdd\"]")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            Driver.FindElement(By.CssSelector(".col-xs-9 input")).SendKeys("New_proprtyvalue");
            Driver.FindElements(By.CssSelector(".col-xs-9 input"))[1].Clear();
            Driver.FindElements(By.CssSelector(".col-xs-9 input"))[1].SendKeys("2");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"PropertyValueSettingTitle\"]"));

            VerifyAreEqual("New_proprtyvalue", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
        }

        [Test]
        public void AddPropertyValuesoknew()
        {
            GoToAdmin("settingscatalog#?propertyId=1");
            Refresh();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PropertyValueSettingAdd\"]")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);
            Driver.FindElement(By.CssSelector(".col-xs-9 input")).SendKeys("New_proprtyvalue1");
            Driver.FindElements(By.CssSelector(".col-xs-9 input"))[1].Clear();
            Driver.FindElements(By.CssSelector(".col-xs-9 input"))[1].SendKeys("1");
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"PropertyValueSettingTitle\"]"));
            VerifyAreEqual("New_proprtyvalue1", Driver.GetGridCell(0, "Value", "PropertyValues").Text);
            VerifyAreEqual("New_proprtyvalue", Driver.GetGridCell(1, "Value", "PropertyValues").Text);
        }
    }
}