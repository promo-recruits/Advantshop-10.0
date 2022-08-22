using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.Product_ProductLists
{
    [TestFixture]
    public class Product_ProductListsTest : BaseSeleniumTest
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
        public void Product_ProductListPresent()
        {
            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"1\"]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(99, "Name", "ListProducts").Text);

            //check no products in list
            Driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"5\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Driver.GridPaginationSelectItems("10");

            //client
            GoToClient();
            VerifyAreEqual("TestProduct1",
                Driver.FindElements(By.CssSelector(".products-specials-list .products-view.products-view-tile.row"))[0]
                    .FindElements(By.CssSelector(".products-view-name.products-view-name-default a"))[0].Text);
        }

        [Test]
        public void Product_ProductListaSort()
        {
            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"1\"]")).Click();

            //check sort by name
            Driver.GetGridCell(-1, "Name", "ListProducts").Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct17", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            Driver.GetGridCell(-1, "Name", "ListProducts").Click();
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            //check sort by name ArtNo
            Driver.GetGridCell(-1, "ProductArtNo", "ListProducts").Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct17", Driver.GetGridCell(9, "Name", "ListProducts").Text);
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductArtNo", "ListProducts").Text);
            VerifyAreEqual("17", Driver.GetGridCell(9, "ProductArtNo", "ListProducts").Text);

            Driver.GetGridCell(-1, "ProductArtNo", "ListProducts").Click();
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name", "ListProducts").Text);
            VerifyAreEqual("99", Driver.GetGridCell(0, "ProductArtNo", "ListProducts").Text);
            VerifyAreEqual("90", Driver.GetGridCell(9, "ProductArtNo", "ListProducts").Text);

            //check sort by name sort order
            Driver.GetGridCell(-1, "SortOrder", "ListProducts").Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name", "ListProducts").Text);
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "ListProducts").Text);
            VerifyAreEqual("10", Driver.GetGridCell(9, "SortOrder", "ListProducts").Text);

            Driver.GetGridCell(-1, "SortOrder", "ListProducts").Click();
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct91", Driver.GetGridCell(9, "Name", "ListProducts").Text);
            VerifyAreEqual("100", Driver.GetGridCell(0, "SortOrder", "ListProducts").Text);
            VerifyAreEqual("91", Driver.GetGridCell(9, "SortOrder", "ListProducts").Text);

            Driver.GetGridCell(-1, "SortOrder", "ListProducts").Click();
        }

        [Test]
        public void Product_ProductListSearchAndInplace()
        {
            GoToAdmin("mainpageproductsstore");

            //check search exist product
            Driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"1\"]")).Click();
            Driver.GridFilterSendKeys("TestProduct70");
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(0, "Name", "ListProducts").Text);

            //check sort order inplace edit 
            Driver.SendKeysGridCell("200", 0, "SortOrder", "ListProducts");

            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"1\"]")).Click();
            Driver.GridFilterSendKeys("TestProduct70");
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("200", Driver.GetGridCell(0, "SortOrder", "ListProducts").Text);

            //check search not exist product
            Driver.GridFilterSendKeys("TestProduct110");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check search too much symbols
            Driver.GridFilterSendKeys(
                    "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void Product_ProductListSelectAndDelete()
        {
            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"1\"]")).Click();
            Driver.GridReturnDefaultView10(BaseUrl);

            //check delete cancel
            Driver.GetGridCell(0, "_serviceColumn", "ListProducts").FindElement(By.TagName("ui-grid-custom-delete"))
                .Click();
            Driver.SwalCancel();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "ListProducts").FindElement(By.TagName("ui-grid-custom-delete"))
                .Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text);

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ListProducts")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "ListProducts")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "ListProducts")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "ListProducts")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "selectionRowHeaderCol", "ListProducts")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "selectionRowHeaderCol", "ListProducts")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("3", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreNotEqual("TestProduct2", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreNotEqual("TestProduct3", Driver.GetGridCell(1, "Name", "ListProducts").Text);
            VerifyAreNotEqual("TestProduct4", Driver.GetGridCell(2, "Name", "ListProducts").Text);

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "ListProducts")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "ListProducts")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("TestProduct15", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            VerifyAreEqual("TestProduct24", Driver.GetGridCell(9, "Name", "ListProducts").Text);

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("86", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol", "ListProducts")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "selectionRowHeaderCol", "ListProducts")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            Refresh();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }
    }
}