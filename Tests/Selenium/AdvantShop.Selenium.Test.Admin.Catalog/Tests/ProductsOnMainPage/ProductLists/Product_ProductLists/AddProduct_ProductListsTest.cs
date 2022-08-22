using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists.Product_ProductLists
{
    [TestFixture]
    public class Product_ProductListAddTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProd_ProdLists\\Catalog.ProductList.csv"
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
        public void AddProduct_ProductListviaSearch()
        {
            GoToAdmin("mainpageproductsstore?listId=1");

            //check whether a product is in product list
            Driver.GridFilterSendKeys("TestProduct110");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
            GoToAdmin("mainpageproductsstore?listId=1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.Id("4")).FindElement(By.CssSelector(".jstree-icon.jstree-ocl")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'TestCategory5')]"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.GridFilterSendKeys("TestProduct110");
            // 
            Driver.XPathContainsText("h2", "Выбор товара");
            Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Click();
            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();

            GoToAdmin("mainpageproductsstore?listId=1");
            Driver.GridFilterSendKeys("TestProduct110");
            VerifyAreEqual("TestProduct110", Driver.GetGridCell(0, "Name", "ListProducts").Text);
        }

        [Test]
        public void AddProduct_ProductListviaCategory()
        {
            GoToAdmin("mainpageproductsstore?listId=2");

            //check whether a product is in product list
            Driver.GridFilterSendKeys("TestProduct155");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
            GoToAdmin("mainpageproductsstore?listId=2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory6')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.GetGridCell(4, "Name", "ProductsSelectvizr").Click();

            Driver.ScrollTo(By.XPath("//button[contains(text(), 'Выбрать')]"));

            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();

            GoToAdmin("mainpageproductsstore?listId=2");
            Driver.GridFilterSendKeys("TestProduct155");
            VerifyAreEqual("TestProduct155", Driver.GetGridCell(0, "Name", "ListProducts").Text);
        }

        [Test]
        public void AddProduct_ProductListviaSubCategory()
        {
            GoToAdmin("mainpageproductsstore?listId=3");

            //check whether a product is in product list
            Driver.GridFilterSendKeys("TestProduct101");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
            GoToAdmin("mainpageproductsstore?listId=3");

            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("4")).FindElement(By.CssSelector(".jstree-icon.jstree-ocl")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'TestCategory5')]"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Click();

            Driver.ScrollTo(By.XPath("//button[contains(text(), 'Выбрать')]"));

            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();

            GoToAdmin("mainpageproductsstore?listId=3");
            Driver.GridFilterSendKeys("TestProduct101");
            VerifyAreEqual("TestProduct101", Driver.GetGridCell(0, "Name", "ListProducts").Text);
        }

        [Test]
        public void AddNotExistProduct_ProductListviaSearch()
        {
            GoToAdmin("mainpageproductsstore?listId=4");

            //check whether a product is in product list
            Driver.GridFilterSendKeys("TestProduct504");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
            GoToAdmin("mainpageproductsstore?listId=4");

            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory6')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.GridFilterSendKeys("TestProduct504");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();

            GoToAdmin("mainpageproductsstore?listId=4");
            Driver.GridFilterSendKeys("TestProduct504");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void AddProduct_ProductListUsingPagination()
        {
            GoToAdmin("mainpageproductsstore?listId=5");

            //check whether a product is in product list
            Driver.GridFilterSendKeys("TestProduct179");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
            GoToAdmin("mainpageproductsstore?listId=5");

            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.GridPaginationSelectItems("10");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory6')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            //  Driver.ScrollTo(By.XPath("//button[contains(text(), 'Выбрать')]"));

            Driver.GetGridCell(8, "Name", "ProductsSelectvizr").Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();

            GoToAdmin("mainpageproductsstore?listId=5");
            VerifyAreEqual("TestProduct179", Driver.GetGridCell(0, "Name", "ListProducts").Text);
        }

        [Test]
        public void AddAProduct_ProductListView()
        {
            GoToAdmin("mainpageproductsstore?listId=8");
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory6')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("TestProduct151", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct160", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text);

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("TestProduct151", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct250", Driver.GetGridCell(99, "Name", "ProductsSelectvizr").Text);

            //return default
            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void AddProduct_ProductListSelect()
        {
            GoToAdmin("mainpageproductsstore?listId=9");

            //check whether a product is in product list
            Driver.GridFilterSendKeys("TestProduct151");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
            GoToAdmin("mainpageproductsstore?listId=9");

            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory6')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(1, "Name", "ProductsSelectvizr").Click();

            //check items selected 
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            Driver.ScrollTo(By.XPath("//button[contains(text(), 'Выбрать')]"));

            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();

            //check items selected add
            GoToAdmin("mainpageproductsstore?listId=9");
            Driver.GridFilterSendKeys("TestProduct151");
            VerifyAreEqual("TestProduct151", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            Driver.GridFilterSendKeys("TestProduct152");
            VerifyAreEqual("TestProduct152", Driver.GetGridCell(0, "Name", "ListProducts").Text);
        }

        [Test]
        public void AddProduct_ProductListSelectAllOnPage()
        {
            GoToAdmin("mainpageproductsstore?listId=6");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory6')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.GridPaginationSelectItems("50");

            //check items selected all on page
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));
            Driver.GetGridCell(-1, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(49, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            Driver.ScrollTo(By.XPath("//button[contains(text(), 'Выбрать')]"));

            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();

            //check items selected add
            GoToAdmin("mainpageproductsstore?listId=6");
            Driver.GridFilterSendKeys("TestProduct169");
            VerifyAreEqual("TestProduct169", Driver.GetGridCell(0, "Name", "ListProducts").Text);
            Driver.GridFilterSendKeys("TestProduct152");
            VerifyAreEqual("TestProduct152", Driver.GetGridCell(0, "Name", "ListProducts").Text);
        }

        [Test]
        public void AddProduct_ProductListSelectAll()
        {
            GoToAdmin("mainpageproductsstore?listId=7");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory6')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));

            //check items selected all 
            Driver.GetGridCell(-1, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check items deselected all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            Driver.GetGridCell(-1, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "ProductsSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            Driver.ScrollTo(By.XPath("//button[contains(text(), 'Выбрать')]"));

            Driver.FindElement(By.XPath("//button[contains(text(), 'Выбрать')]")).Click();

            //check all items add
            GoToAdmin("mainpageproductsstore?listId=7");
            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Test]
        public void AddProduct_ProductListSort()
        {
            GoToAdmin("mainpageproductsstore?listId=10");
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();

            //check sort by name
            Driver.GetGridCell(-1, "Name", "ProductsSelectvizr").Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text);

            Driver.GetGridCell(-1, "Name", "ProductsSelectvizr").Click();
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text);

            //check sort by product art no
            Driver.GetGridCell(-1, "ProductArtNo", "ProductsSelectvizr").Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text);
            VerifyAreEqual("107", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text);

            Driver.GetGridCell(-1, "ProductArtNo", "ProductsSelectvizr").Click();
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text);
            VerifyAreEqual("99", Driver.GetGridCell(0, "ProductArtNo", "ProductsSelectvizr").Text);
            VerifyAreEqual("90", Driver.GetGridCell(9, "ProductArtNo", "ProductsSelectvizr").Text);

            Driver.GetGridCell(-1, "ProductArtNo", "ProductsSelectvizr").Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
        }
    }
}