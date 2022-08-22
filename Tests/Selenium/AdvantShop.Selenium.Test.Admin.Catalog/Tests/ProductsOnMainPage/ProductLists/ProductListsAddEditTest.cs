using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists
{
    [TestFixture]
    public class ProductListsAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProductList\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProductList\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProductList\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProductList\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProductList\\Catalog.ProductList.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\AddProductList\\Catalog.Product_ProductList.csv"
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
        public void ProductListAddActive()
        {
            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("[data-e2e=\"productListAdd\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Добавление списка", Driver.FindElement(By.TagName("h2")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).SendKeys("NewProductList");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortProductList\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortProductList\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortProductList\"]")).SendKeys("5");
            Driver.CheckBoxCheck("[data-e2e=\"ProductListMeta\"]", "CssSelector");
            Driver.SetCkText("Product List Description Test", "editor1");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("NewProductList"));
            VerifyAreEqual("Список товаров \"NewProductList\"",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListTitle\"]")).Text);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridListProducts\"]")).Text
                .Contains("Ни одной записи не найдено"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListEnabled\"]"))
                .FindElement(By.TagName("input")).Selected);

            //check details
            Driver.FindElement(By.CssSelector("[data-e2e=\"editProductList\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));
            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Редактирование списка"));
            VerifyAreEqual("NewProductList",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).GetAttribute("value"));
            VerifyAreEqual("5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SortProductList\"]")).GetAttribute("value"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"SwitchOnOffProductList\"]"))
                .FindElement(By.TagName("input")).Selected);
            Driver.AssertCkText("Product List Description Test", "editor2");
        }

        [Test]
        public void ProductListEdit()
        {
            GoToAdmin("mainpageproductsstore?listId=2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"editProductList\"]")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).SendKeys("ChangedName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortProductList\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortProductList\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortProductList\"]")).SendKeys("20");
            Driver.SetCkText("Product List Description Edited", "editor1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SwitchOnOffProductList\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("Список товаров \"ChangedName\"",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListTitle\"]")).Text);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListEnabled\"]"))
                .FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("ChangedName"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("ProductList2"));

            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("[data-e2e-product-list-id=\"2\"]")).Click();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("ChangedName"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("ProductList2"));

            VerifyAreEqual("Список товаров \"ChangedName\"",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListTitle\"]")).Text);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListEnabled\"]"))
                .FindElement(By.TagName("input")).Selected);

            //check details
            Driver.FindElement(By.CssSelector("[data-e2e=\"editProductList\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));
            VerifyAreEqual("ChangedName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).GetAttribute("value"));
            VerifyAreEqual("20",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SortProductList\"]")).GetAttribute("value"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"SwitchOnOffProductList\"]"))
                .FindElement(By.TagName("input")).Selected);
            Driver.AssertCkText("Product List Description Edited", "editor1");

            GoToClient("productlist/list/2");
            VerifyIsTrue(Is404Page("productlist/list/2"));
            VerifyAreEqual("Страница была удалена или перемещена",
                Driver.FindElement(By.CssSelector(".err-reasons-list")).Text);

            GoToAdmin("mainpageproductsstore?listId=2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"editProductList\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"SwitchOnOffProductList\"]")).FindElement(By.TagName("span"))
                .Click();
            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListEnabled\"]"))
                .FindElement(By.TagName("input")).Selected);

            GoToClient("productlist/list/2");
            VerifyAreEqual("ChangedName", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("Product List Description Edited",
                Driver.FindElement(By.CssSelector(".category-description")).Text);
        }

        [Test]
        public void ProductListEditMeta()
        {
            GoToAdmin("mainpageproductsstore?listId=2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"editProductList\"]")).Click();
            Thread.Sleep(1000);

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListMeta\"] input")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListMeta\"] span")).Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListH1\"]")).SendKeys("ProductList_H1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListTitle\"]")).SendKeys("ProductList_Title");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListMetaKey\"]")).SendKeys("ProductList_SeoKeywords");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListMetaDesc\"]"))
                .SendKeys("ProductList_SeoDescription");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListSave\"]")).Click();
            Thread.Sleep(1000);

            GoToClient("productlist/list/2");
            VerifyAreEqual("ProductList_Title", Driver.Title);
            VerifyAreEqual("ProductList_H1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("ProductList_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"));
            VerifyAreEqual("ProductList_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"));

            GoToAdmin("mainpageproductsstore?listId=2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"editProductList\"]")).Click();
            Thread.Sleep(1000);

            string list_name = Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]"))
                .GetAttribute("value");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListMeta\"] span")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListSave\"]")).Click();
            Thread.Sleep(1000);

            GoToClient("productlist/list/2");
            VerifyAreEqual("Мой магазин - " + list_name, Driver.Title);
            VerifyAreEqual(list_name, Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("Мой магазин - " + list_name,
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"));
            VerifyAreEqual("Мой магазин - " + list_name,
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"));
        }

        [Test]
        public void ProductListAddNotActive()
        {
            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("[data-e2e=\"productListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).SendKeys("NewProductListNotActive");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SwitchOnOffProductList\"]")).FindElement(By.TagName("span"))
                .Click();
            Driver.CheckBoxCheck("[data-e2e=\"ProductListMeta\"]", "CssSelector");
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text.Contains("NewProductList"));
            VerifyAreEqual("Список товаров \"NewProductListNotActive\"",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListTitle\"]")).Text);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridListProducts\"]")).Text
                .Contains("Ни одной записи не найдено"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"ProductListEnabled\"]"))
                .FindElement(By.TagName("input")).Selected);

            //check details
            Driver.FindElement(By.CssSelector("[data-e2e=\"editProductList\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"SwitchOnOffProductList\"]"))
                .FindElement(By.TagName("input")).Selected);
        }


        [Test]
        public void ProductListAddCancel()
        {
            GoToAdmin("mainpageproductsstore");
            Driver.FindElement(By.CssSelector("[data-e2e=\"productListAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameProductList\"]")).SendKeys("NewProductListCancel");
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//button[contains(text(), 'Отмена')]")).Click();
            Thread.Sleep(1000);

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text
                .Contains("NewProductListCancel"));

            Refresh();
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".product-lists-menu")).Text
                .Contains("NewProductListCancel"));
        }
    }
}