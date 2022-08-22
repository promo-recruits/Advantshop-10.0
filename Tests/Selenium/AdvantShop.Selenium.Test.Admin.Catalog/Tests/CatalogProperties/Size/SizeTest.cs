using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Size
{
    [TestFixture]
    public class SizeCatalogAdminTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.ProductCategories.csv"
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
        public void SizeSearch()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");

            //check search exist size
            Driver.GridFilterSendKeys("SizeName26");
            VerifyAreEqual("SizeName26", Driver.GetGridCell(0, "SizeName", "Sizes").Text);

            //check search not exist size
            Driver.GridFilterSendKeys("SizeName365");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check search too much symbols
            Driver.GridFilterSendKeys("1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void SizeSelectAndDelete()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");

            //check delete size in use
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.GridFilterSendKeys("SizeName101");
            Driver.DropFocusCss("[data-e2e=\"SizeSettingTitle\"]");
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "_serviceColumn", "Sizes")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-disabled")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Удаление невозможно", Driver.FindElement(By.Id("swal2-title")).Text);
            Driver.SwalConfirm();

            GoToAdmin("settingscatalog#?catalogTab=sizes");
            Driver.GridFilterSendKeys("SizeName101");
            VerifyAreEqual("SizeName101", Driver.GetGridCell(0, "SizeName", "Sizes").Text);

            GoToAdmin("settingscatalog#?catalogTab=sizes");

            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "Sizes")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Sizes")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Sizes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Sizes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Sizes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Sizes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "selectionRowHeaderCol", "Sizes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "selectionRowHeaderCol", "Sizes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("3", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreNotEqual("SizeName2", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreNotEqual("SizeName3", Driver.GetGridCell(1, "SizeName", "Sizes").Text);
            VerifyAreNotEqual("SizeName4", Driver.GetGridCell(2, "SizeName", "Sizes").Text);

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "Sizes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "Sizes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("SizeName15", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName24", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("186", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol", "Sizes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "selectionRowHeaderCol", "Sizes")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            GoToAdmin("settingscatalog#?catalogTab=sizes");
            VerifyAreEqual("SizeName101", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
        }

        [Test]
        public void SizeInplaceEditAndFilter()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");

            //inplace
            Driver.GridFilterSendKeys("SizeName99");

            Driver.SendKeysGridCell("SizeX", 0, "SizeName", "Sizes");
            Driver.SendKeysGridCell("500", 0, "SortOrder", "Sizes");

            //check
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            Driver.GridFilterSendKeys("SizeX");
            VerifyAreEqual("SizeX", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("500", Driver.GetGridCell(0, "SortOrder", "Sizes").Text);

            //name filter 
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            Functions.GridFilterSet(Driver, BaseUrl, "SizeName");
            Driver.SetGridFilterValue("SizeName", "SizeName10");
            VerifyAreEqual("SizeName10", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName100", Driver.GetGridCell(1, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName108", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            //close filter
            Functions.GridFilterClose(Driver, BaseUrl, "SizeName");
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName10", Driver.GetGridCell(9, "SizeName", "Sizes").Text);
        }
    }
}