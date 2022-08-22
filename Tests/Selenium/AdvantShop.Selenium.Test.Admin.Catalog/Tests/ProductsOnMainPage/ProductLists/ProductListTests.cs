using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists
{
    [TestFixture]
    public class ProductListTests : BaseMultiSeleniumTest
    {
        [SetUp]
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
            countList = 4;
            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.ClassName("aside-menu")).FindElements(By.ClassName("aside-menu-row-with-move"))[1]
                .Click();
            Thread.Sleep(1000);
            Driver.CheckBoxUncheck("[data-e2e=\"DisplayLatestProductsInNewOnMainPage\"]", "CssSelector");
            Thread.Sleep(1000);

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        int countList = 4;

        [Test]
        public void ProductList()
        {
            GoToAdmin("mainpageproductsstore");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".fas.fa-times")).Count == 5, "count list admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"productListAdd\"]")).Count == 1,
                "count btn add admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("ProductList1"),
                "item 1 list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("ProductList5"),
                "item 5 list admin");

            //check client
            GoToClient();
            VerifyAreEqual("ProductList1", Driver.FindElement(By.CssSelector(".products-specials-list .h2")).Text,
                "item 1 list client");
            VerifyAreEqual("ProductList2", Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[1].Text,
                "item 2 list client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-specials-list")).Count == countList,
                "count list client");

            GoToClient("productlist/list/1");
            VerifyAreEqual("ProductList1", Driver.FindElement(By.TagName("h1")).Text, "h1 list client");

            VerifyAreEqual("ProductList1",
                Driver.FindElement(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Text,
                "menu item 1 list client");
            VerifyAreEqual("ProductList2",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[1].Text,
                "menu item 2 list client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Count == countList,
                "menu count list client");
        }

        [Test]
        public void ProductListActivity()
        {
            GoToAdmin("mainpageproductsstore?listId=2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "enabled list admin");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListEnabled\"]")).FindElement(By.TagName("span"))
                .Click();
            countList--;
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "change enabled list admin");

            //check client
            GoToClient();
            VerifyAreEqual("ProductList1", Driver.FindElement(By.CssSelector(".products-specials-list .h2")).Text,
                "enabled list 1 client");
            VerifyAreEqual("ProductList3", Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[1].Text,
                "enabled list 2 client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-specials-list")).Count == countList,
                "count list client");

            GoToClient("productlist/list/1");
            VerifyAreEqual("ProductList1", Driver.FindElement(By.TagName("h1")).Text, "h1 list");

            VerifyAreEqual("ProductList1",
                Driver.FindElement(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Text,
                "enabled menu list 1 client");
            VerifyAreEqual("ProductList3",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[1].Text,
                "enabled menu list 2 client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Count == countList,
                "enabled menu count list client");

            GoToAdmin("mainpageproductsstore?listId=2");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "old enabled list admin");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListEnabled\"]")).FindElement(By.TagName("span"))
                .Click();
            countList++;
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListEnabled\"]")).FindElement(By.TagName("input"))
                    .Selected, "new enabled list admin");

            //check client
            GoToClient();
            VerifyAreEqual("ProductList1", Driver.FindElement(By.CssSelector(".products-specials-list .h2")).Text,
                "new enabled list 1 client");
            VerifyAreEqual("ProductList2", Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[1].Text,
                "new enabled list 2 client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-specials-list")).Count == countList,
                "new cont enabled list client");

            GoToClient("productlist/list/1");
            VerifyAreEqual("ProductList1", Driver.FindElement(By.TagName("h1")).Text, "new h1 list");

            VerifyAreEqual("ProductList1",
                Driver.FindElement(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Text,
                "new enabled menu list 1 client");
            VerifyAreEqual("ProductList2",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[1].Text,
                "new enabled menu list 2 client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Count == countList,
                "new enabled menu count list client");
        }

        [Test]
        public void ProductListDel()
        {
            GoToAdmin("mainpageproductsstore");

            Del(1);
            Del(2, false);
            countList--;

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".fas.fa-times")).Count == 4, "count list admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"productListAdd\"]")).Count == 1,
                "count btn add admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("ProductList2"),
                "item 1 list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("ProductList3"),
                "item 2 list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("ProductList4"),
                "item 3 list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("ProductList5"),
                "item 5 list admin");

            //check client
            GoToClient();
            VerifyAreEqual("ProductList2", Driver.FindElement(By.CssSelector(".products-specials-list .h2")).Text,
                "item 1 list client");
            VerifyAreEqual("ProductList3", Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[1].Text,
                "item 2 list client");
            VerifyAreEqual("ProductList4", Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[2].Text,
                "item 3 list client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-specials-list")).Count == countList,
                "count list client");

            GoToClient("productlist/list/3");

            VerifyAreEqual("ProductList2",
                Driver.FindElement(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Text,
                "menu item 1 list client");
            VerifyAreEqual("ProductList3",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[1].Text,
                "menu item 2 list client");
            VerifyAreEqual("ProductList4",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[2].Text,
                "menu item 3 list client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Count == countList,
                "menu count list client");
        }

        [Test]
        public void ProductListMove()
        {
            GoToAdmin("mainpageproductsstore");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".fas.fa-times")).Count == 5, "count list admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"productListAdd\"]")).Count == 1,
                "count btn add admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".as-sortable-item"))[0].Text.Contains("ProductList1"),
                "item 1 list admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".as-sortable-item"))[1].Text.Contains("ProductList2"),
                "item 2 list admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".as-sortable-item"))[2].Text.Contains("ProductList3"),
                "item 3 list admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".as-sortable-item"))[3].Text.Contains("ProductList4"),
                "item 4 list admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".as-sortable-item"))[4].Text.Contains("ProductList5"),
                "item 5 list admin");

            //check client
            GoToClient();
            VerifyAreEqual("ProductList1", Driver.FindElement(By.CssSelector(".products-specials-list .h2")).Text,
                "item 1 list client");
            VerifyAreEqual("ProductList2", Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[1].Text,
                "item 2 list client");
            VerifyAreEqual("ProductList3", Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[2].Text,
                "item 3 list client");
            VerifyAreEqual("ProductList4", Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[3].Text,
                "item 4 list client");

            GoToClient("productlist/list/1");

            VerifyAreEqual("ProductList1",
                Driver.FindElement(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Text,
                "menu item 1 list client");
            VerifyAreEqual("ProductList2",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[1].Text,
                "menu item 2 list client");
            VerifyAreEqual("ProductList3",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[2].Text,
                "menu item 3 list client");
            VerifyAreEqual("ProductList4",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[3].Text,
                "menu item 4 list client");

            //change sort
            GoToAdmin("mainpageproductsstore");

            Sort(0, 2);
            Thread.Sleep(1000);
            Sort(3, 1);
            Thread.Sleep(1000);
            Sort(4, 0);
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".as-sortable-item"))[0].Text.Contains("ProductList2"),
                "item 1 list admin change");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".as-sortable-item"))[1].Text.Contains("ProductList5"),
                "item 2 list admin change");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".as-sortable-item"))[2].Text.Contains("ProductList1"),
                "item 3 list admin change");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".as-sortable-item"))[3].Text.Contains("ProductList4"),
                "item 4 list admin change");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".as-sortable-item"))[4].Text.Contains("ProductList3"),
                "item 5 list admin change");

            //check client
            GoToClient();
            VerifyAreEqual("ProductList2", Driver.FindElement(By.CssSelector(".products-specials-list .h2")).Text,
                "item 1 list client change");
            VerifyAreEqual("ProductList1", Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[1].Text,
                "item 2 list client change");
            VerifyAreEqual("ProductList4", Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[2].Text,
                "item 3 list client change");
            VerifyAreEqual("ProductList3", Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[3].Text,
                "item 4 list client change");

            GoToClient("productlist/list/1");

            VerifyAreEqual("ProductList2",
                Driver.FindElement(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Text,
                "menu item 1 list client change");
            VerifyAreEqual("ProductList1",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[1].Text,
                "menu item 2 list client change");
            VerifyAreEqual("ProductList4",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[2].Text,
                "menu item 3 list client change");
            VerifyAreEqual("ProductList3",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[3].Text,
                "menu item 4 list client change");
        }

        [Test]
        public void ProductListProductAdd()
        {
            GoToAdmin("mainpageproductsstore?listId=5");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".fas.fa-times")).Count == 5, "count list admin");
            Driver.FindElement(By.CssSelector("[data-e2e=\"product_ProductListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Thread.Sleep(1000);
            countList++;
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ListProducts").Text, "new product");

            //check client
            GoToClient();
            VerifyAreEqual("ProductList1", Driver.FindElement(By.CssSelector(".products-specials-list .h2")).Text,
                "list 1 client");
            VerifyAreEqual("ProductList5",
                Driver.FindElements(By.CssSelector(".products-specials-list .h2"))[countList - 1].Text,
                "list 5 client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-specials-list")).Count == countList,
                "count list client");

            GoToClient("productlist/list/5");
            VerifyAreEqual("ProductList5", Driver.FindElement(By.TagName("h1")).Text, "h1 list");

            VerifyAreEqual("ProductList1",
                Driver.FindElement(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Text,
                "menu list 1 client");
            VerifyAreEqual("ProductList5",
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item"))[countList - 1].Text,
                "menu list 5 client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".menu-dropdown-expanded .menu-dropdown-item")).Count == countList,
                "menu list count client");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-view-item")).Count == 1,
                "list 5 count product client");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.CssSelector(".products-view-name")).Text,
                "list 5 product client");
        }

        private void Sort(int startPosition, int endPosition)
        {
            IWebElement dragElement =
                Driver.FindElements(By.CssSelector(".as-sortable-item .icon-move"))[startPosition];
            IWebElement dropElement = Driver.FindElements(By.CssSelector(".as-sortable-item .icon-move"))[endPosition];
            Functions.DragDropElement(Driver, dragElement, dropElement);
        }

        private void Del(int idList, bool accept = true)
        {
            Driver.MouseFocus(By.CssSelector("[data-e2e-product-list-id=\"" + idList + "\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"" + idList + "\"] .fas.fa-times")).Click();
            if (accept)
                Driver.SwalConfirm();
            else
                Driver.SwalCancel();
        }
    }
}