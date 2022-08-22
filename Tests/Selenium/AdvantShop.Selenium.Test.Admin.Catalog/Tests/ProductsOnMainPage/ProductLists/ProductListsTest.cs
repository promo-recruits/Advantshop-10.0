using NUnit.Framework;
using OpenQA.Selenium;
using System.Threading;
using AdvantShop.SeleniumTest.Core;

namespace AdvantShop.SeleniumTest.Admin.ProductLists
{
    [TestFixture]
    public class ProductListsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
           "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.Product.csv",
           "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.Offer.csv",
           "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.Category.csv",
           "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.ProductCategories.csv",
           "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.ProductList.csv",
           "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\Catalog.Product_ProductList.csv"
           );
             
            Init();
        }
        
        [Test]
        public void ProductListPresent()
        {
            GoToAdmin("mainpageproductsstore");
            VerifyAreEqual("100", driver.FindElements(By.CssSelector(".link-invert.aside-menu-row"))[5].FindElement(By.CssSelector(".aside-menu-count-inner")).Text);
            VerifyIsTrue(driver.FindElement(By.TagName("h2")).Text.Contains("Списки товаров на главной"));
            Functions.GridPaginationSelect10(driver, baseURL);
            VerifyAreEqual("ProductList1", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList10", GetGridCell(9, "Name", "ProductLists").Text);

            Functions.GridPaginationSelect20(driver, baseURL);
            VerifyAreEqual("ProductList1", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList20", GetGridCell(19, "Name", "ProductLists").Text);

            Functions.GridPaginationSelect50(driver, baseURL);
            VerifyAreEqual("ProductList1", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList50", GetGridCell(49, "Name", "ProductLists").Text);

            Functions.GridPaginationSelect100(driver, baseURL);
            VerifyAreEqual("ProductList1", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList100", GetGridCell(99, "Name", "ProductLists").Text);

            //check client
            GoToClient();
            VerifyAreEqual("ProductList1", driver.FindElements(By.ClassName("products-specials-header"))[0].FindElement(By.CssSelector(".h2.h-inline")).Text);
        }

        [Test]
        public void ProductListSearch()
        {
            GoToAdmin("mainpageproductsstore");

            //check search exist list
            GetGridFilter().SendKeys("ProductList7");
            DropFocus("h2");
            WaitForAjax();
            VerifyAreEqual("ProductList7", GetGridCell(0, "Name", "ProductLists").Text);

            //check search not exist list
            GetGridFilter().Click();
            GetGridFilter().Clear();
            GetGridFilter().SendKeys("ProductList203");
            DropFocus("h2");
            WaitForAjax();
            VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"));

            //check search too much symbols
            GetGridFilter().Click();
            GetGridFilter().Clear();
            GetGridFilter().SendKeys("1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            DropFocus("h2");
            WaitForAjax();
            VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"));

            //check search invalid symbols
            GetGridFilter().Click();
            GetGridFilter().Clear();
            GetGridFilter().SendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            DropFocus("h2");
            WaitForAjax();
            VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"));
        }
        
        [Test]
        public void ProductListzSelectAndDelete()
        {
            GoToAdmin("mainpageproductsstore");
            gridReturnDefaultView10();

            //check delete cancel
            GetGridCell(0, "_serviceColumn", "ProductLists").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.ClassName("swal2-cancel")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("ProductList1", GetGridCell(0, "Name", "ProductLists").Text);

            //check delete
            GetGridCell(0, "_serviceColumn", "ProductLists").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.ClassName("swal2-confirm")).Click();
            Thread.Sleep(2000);
            Assert.AreNotEqual("ProductList1", GetGridCell(0, "Name", "ProductLists").Text);

            //check select 
            GetGridCell(0, "selectionRowHeaderCol", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            GetGridCell(1, "selectionRowHeaderCol", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            GetGridCell(2, "selectionRowHeaderCol", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(GetGridCell(0, "selectionRowHeaderCol", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(GetGridCell(1, "selectionRowHeaderCol", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(GetGridCell(2, "selectionRowHeaderCol", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("3", driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

            //check delete selected items
            Functions.GridDropdownDelete(driver, baseURL);
            Assert.AreNotEqual("ProductList2", GetGridCell(0, "Name", "ProductLists").Text);
            Assert.AreNotEqual("ProductList3", GetGridCell(1, "Name", "ProductLists").Text);
            Assert.AreNotEqual("ProductList4", GetGridCell(2, "Name", "ProductLists").Text);

            //check select all on page
            driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(GetGridCell(0, "selectionRowHeaderCol", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(GetGridCell(9, "selectionRowHeaderCol", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all on page
            Functions.GridDropdownDelete(driver, baseURL);
            VerifyAreEqual("ProductList15", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList24", GetGridCell(9, "Name", "ProductLists").Text);

            //check select all
            driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("86", driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

            //check deselect all 
            driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(GetGridCell(0, "selectionRowHeaderCol", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(GetGridCell(9, "selectionRowHeaderCol", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all
            driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(driver, baseURL);
            VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"));

            Refresh();
            VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void ProductListFilterAndGoToEdit()
        {
            GoToAdmin("mainpageproductsstore");

            //check name filter
            driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterDropdownItem\"][data-e2e-filter-dropdown-name=\"Name\"]")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]")).Click();
            driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]")).Clear();
            driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]")).SendKeys("ProductList10");
            DropFocus("h2");
            WaitForAjax();
            VerifyAreEqual("ProductList10", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList100", GetGridCell(1, "Name", "ProductLists").Text);

            //check go to list and back with filter
            GetGridCell(0, "Name", "ProductLists").FindElement(By.CssSelector("[href=\"mainpageproductsstore?listId=10\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Список товаров \"ProductList10\"", driver.FindElement(By.TagName("h2")).Text);
            //driver.FindElement(By.LinkText("Вернуться назад")).Click();
            //Thread.Sleep(2000);
            GoBack();
            VerifyIsTrue(driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"]")).Displayed);
            VerifyAreEqual("ProductList10", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList100", GetGridCell(1, "Name", "ProductLists").Text);

            //close
            driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"] [data-e2e=\"gridFilterItemClose\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("ProductList1", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList10", GetGridCell(9, "Name", "ProductLists").Text);
            VerifyIsFalse(driver.FindElements(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"]")).Count > 0);

            Refresh();

            //check go to edit
            GetGridCell(0, "_serviceColumn", "ProductLists").FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Редактирование списка", driver.FindElement(By.TagName("h2")).Text);
        }

        [Test]
        public void ProductListSort()
        {
            GoToAdmin("mainpageproductsstore");

            //check sort by name
            GetGridCell(-1, "Name", "ProductLists").Click();
            WaitForAjax();
            Thread.Sleep(1000);
            VerifyAreEqual("ProductList1", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList17", GetGridCell(9, "Name", "ProductLists").Text);

            GetGridCell(-1, "Name", "ProductLists").Click();
            WaitForAjax();
            Thread.Sleep(1000);
            VerifyAreEqual("ProductList99", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList90", GetGridCell(9, "Name", "ProductLists").Text);

            //check sort by sort order
            GetGridCell(-1, "SortOrder", "ProductLists").Click();
            WaitForAjax();
            Thread.Sleep(1000);
            VerifyAreEqual("ProductList1", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList10", GetGridCell(9, "Name", "ProductLists").Text);
            VerifyAreEqual("1", GetGridCell(0, "SortOrder", "ProductLists").Text);
            VerifyAreEqual("10", GetGridCell(9, "SortOrder", "ProductLists").Text);

            GetGridCell(-1, "SortOrder", "ProductLists").Click();
            WaitForAjax();
            Thread.Sleep(1000);
            VerifyAreEqual("ProductList100", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList91", GetGridCell(9, "Name", "ProductLists").Text);
            VerifyAreEqual("100", GetGridCell(0, "SortOrder", "ProductLists").Text);
            VerifyAreEqual("91", GetGridCell(9, "SortOrder", "ProductLists").Text);

            //check sort by activity
            GetGridCell(-1, "Enabled", "ProductLists").Click();
            WaitForAjax();
            Thread.Sleep(1000);
            VerifyAreEqual("ProductList91", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList100", GetGridCell(9, "Name", "ProductLists").Text);
            VerifyIsFalse(GetGridCell(0, "Enabled", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsFalse(GetGridCell(9, "Enabled", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);

            GetGridCell(-1, "Enabled", "ProductLists").Click();
            WaitForAjax();
            Thread.Sleep(1000);
            VerifyAreEqual("ProductList1", GetGridCell(0, "Name", "ProductLists").Text);
            VerifyAreEqual("ProductList10", GetGridCell(9, "Name", "ProductLists").Text);
            VerifyIsTrue(GetGridCell(0, "Enabled", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
            VerifyIsTrue(GetGridCell(9, "Enabled", "ProductLists").FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected);
        }

    }
}