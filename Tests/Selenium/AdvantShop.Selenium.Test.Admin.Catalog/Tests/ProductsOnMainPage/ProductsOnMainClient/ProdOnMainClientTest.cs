using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductsOnMainClient
{
    [TestFixture]
    public class ProductOnMainClientTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductOnMainClient\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductOnMainClient\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductOnMainClient\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductOnMainClient\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductOnMainClient\\Catalog.ProductList.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductOnMainClient\\Catalog.Product_ProductList.csv"
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
        public void BestSellerClient()
        {
            GoToAdmin("mainpageproductsstore?type=best");

            //check admin            
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"MainPageTitle\"]")).Text.Contains("Хиты продаж"),
                "h1 page admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct4"),
                "product 1 in list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct5"),
                "product 2 in list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct6"),
                "product 3 in list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct7"),
                "product 4 in list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct8"),
                "product 5 in list admin");

            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all in grid");

            //check client
            GoToClient();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-best")).Text
                    .Contains("Хиты продаж"), "list h2 client main");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-best")).Text
                    .Contains("TestProduct7"), "product 1 in list client main");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-best")).Text
                    .Contains("TestProduct8"), "product 2 in list client main");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-best")).Text
                    .Contains("TestProduct5"), "product 3 in list client main");

            GoToClient("productlist/best");

            VerifyAreEqual("Хиты продаж", Driver.FindElement(By.TagName("h1")).Text, "h1 page client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-view-sort-result")).Text.Contains("Всего найдено: 4"),
                "count list products client");
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct4"), "product 1 in list client");
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct5"), "product 2 in list client");
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct7"), "product 3 in list client");
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct8"), "product 4 in list client");
            VerifyIsFalse(Driver.PageSource.Contains("TestProduct6"), "product 5 disabled in list client");
        }

        [Test]
        public void NewClient()
        {
            GoToAdmin("mainpageproductsstore?type=new");

            //check admin

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"MainPageTitle\"]")).Text.Contains("Новинки"),
                "h1 page admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct1"),
                "product 1 in list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct2"),
                "product 2 in list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct3"),
                "product 3 in list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct4"),
                "product 4 in list admin");

            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all in grid");

            //check client
            GoToClient();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-new")).Text
                    .Contains("Новинки"), "list h2 client main");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-new")).Text
                    .Contains("TestProduct3"), "product 1 in list client main");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-new")).Text
                    .Contains("TestProduct4"), "product 2 in list client main");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-new")).Text
                    .Contains("TestProduct2"), "product 3 in list client main");

            GoToClient("productlist/new");

            VerifyAreEqual("Новинки", Driver.FindElement(By.TagName("h1")).Text, "h1 page client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-view-sort-result")).Text.Contains("Всего найдено: 3"),
                "count list products client");
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct2"), "product 1 in list client");
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct3"), "product 2 in list client");
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct4"), "product 3 in list client");
            VerifyIsFalse(Driver.PageSource.Contains("TestProduct1"), "product 5 disabled in list client");
        }

        [Test]
        public void SaleClient()
        {
            GoToAdmin("mainpageproductsstore?type=sale");

            //check admin
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"MainPageTitle\"]")).Text.Contains("Товары со скидкой"),
                "h1 page admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct13"),
                "product 1 in list admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-contents-wrapper")).Text.Contains("TestProduct14"),
                "product 2 in list admin");

            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all in grid");

            //check client
            GoToClient();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-discount")).Text
                    .Contains("Скидка!"), "list h2 client main");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-discount")).Text
                    .Contains("TestProduct13"), "product 1 in list client main");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-discount")).Text
                    .Contains("TestProduct14"), "product 2 in list client main");

            GoToClient("productlist/sale");

            VerifyAreEqual("Товары со скидкой", Driver.FindElement(By.TagName("h1")).Text, "h1 page client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-view-sort-result")).Text.Contains("Всего найдено: 2"),
                "count list products client");
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct13"), "product 1 in list client");
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct14"), "product 2 in list client");
        }

        [Test]
        public void BestSellerClientAddDescription()
        {
            GoToAdmin("mainpageproductsstore?type=best");

            Driver.FindElement(By.CssSelector("[data-e2e=\"editMainPageList\"]")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Редактирование списка"), "pop up header");

            Driver.SetCkText("Description Added Best Sellers", "editor1");
            Driver.XPathContainsText("button", "Сохранить");

            //check admin
            GoToAdmin("mainpageproductsstore?type=best");

            Driver.FindElement(By.CssSelector("[data-e2e=\"editMainPageList\"]")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            Driver.AssertCkText("Description Added Best Sellers", "editor1");

            //check client
            GoToClient();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-best")).Text
                    .Contains("Description Added Best Sellers"), "list's description added client no on main");

            GoToClient("productlist/best");

            VerifyIsTrue(Driver.PageSource.Contains("Description Added Best Sellers"),
                "list's description added client");
        }

        [Test]
        public void NewClientAddDescription()
        {
            GoToAdmin("mainpageproductsstore?type=new");

            Driver.FindElement(By.CssSelector("[data-e2e=\"editMainPageList\"]")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Редактирование списка"), "pop up header");

            Driver.SetCkText("Description Added New Sellers", "editor1");
            Driver.XPathContainsText("button", "Сохранить");

            //check admin
            GoToAdmin("mainpageproductsstore?type=new");

            Driver.FindElement(By.CssSelector("[data-e2e=\"editMainPageList\"]")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            Driver.AssertCkText("Description Added New Sellers", "editor1");

            //check client
            GoToClient();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-new")).Text
                    .Contains("Description Added New Sellers"), "list's description added client no on main");

            GoToClient("productlist/new");

            VerifyIsTrue(Driver.PageSource.Contains("Description Added New Sellers"),
                "list's description added client");
        }

        [Test]
        public void SaleClientAddDescription()
        {
            GoToAdmin("mainpageproductsstore?type=sale");

            Driver.FindElement(By.CssSelector("[data-e2e=\"editMainPageList\"]")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Редактирование списка"), "pop up header");

            Driver.SetCkText("Discount description", "editor1");
            Driver.XPathContainsText("button", "Сохранить");

            //check admin
            GoToAdmin("mainpageproductsstore?type=sale");

            Driver.FindElement(By.CssSelector("[data-e2e=\"editMainPageList\"]")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            Driver.AssertCkText("Discount description", "editor1");

            //check client
            GoToClient();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-discount")).Text
                    .Contains("Discount description"), "list's description added client no on main");

            GoToClient("productlist/sale");

            VerifyIsTrue(Driver.PageSource.Contains("Discount description"), "list's description added client");
        }

        [Test]
        public void ProdListClientEditDescription()
        {
            //pre check client
            GoToClient("productlist/list/1");

            VerifyIsTrue(Driver.PageSource.Contains("Description1"), "list's description pre check client");

            GoToAdmin("mainpageproductsstore?listId=1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"editProductList\"]")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Редактирование списка"), "pop up header");

            Driver.AssertCkText("Description1", "editor1");
            Driver.SetCkText("edited text test", "editor1");
            Driver.XPathContainsText("button", "Сохранить");

            //check admin
            GoToAdmin("mainpageproductsstore?listId=1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"editProductList\"]")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            Driver.AssertCkText("edited text test", "editor1");

            //check client
            GoToClient();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-list")).Text
                    .Contains("edited text test"), "list's description added client no on main");

            GoToClient("productlist/list/1");

            VerifyIsTrue(Driver.PageSource.Contains("edited text test"), "list's description added client");
        }

        [Test]
        public void SaleClientAddActivity()
        {
            //not active
            GoToAdmin("mainpageproductsstore?type=sale");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"MainPageListEnabled\"]"))
                    .FindElement(By.TagName("input")).Selected, "enabled discount admin");
            Driver.FindElement(By.CssSelector("[data-e2e=\"MainPageListEnabled\"]")).FindElement(By.TagName("span"))
                .Click();
            Thread.Sleep(1000);
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"MainPageListEnabled\"]"))
                    .FindElement(By.TagName("input")).Selected, "change enabled discount admin");
            //check client
            GoToClient();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-specials-discount")).Count == 0,
                "no discount list client");

            GoToClient("productlist/list/1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".menu-dropdown-expanded")).Text.Contains("Товары со скидкой"),
                "no discount menu client");

            //active
            GoToAdmin("mainpageproductsstore?type=sale");
            Driver.FindElement(By.CssSelector("[data-e2e=\"MainPageListEnabled\"]")).FindElement(By.TagName("span"))
                .Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"MainPageListEnabled\"]"))
                    .FindElement(By.TagName("input")).Selected, "new change enabled discount admin");

            //check client
            GoToClient();
            VerifyAreEqual("Скидка!", Driver.FindElement(By.CssSelector(".products-specials-discount .h2")).Text,
                "new enabled discount client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-specials-discount")).Count == 1,
                "new count enabled discount client");
            Driver.ScrollTo(By.CssSelector(".products-specials-new"));
            Driver.FindElement(By.CssSelector(".products-specials-discount a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual(BaseUrl + "productlist/sale", Driver.Url, "url discount client");
            VerifyAreEqual("Товары со скидкой", Driver.FindElement(By.TagName("h1")).Text, "new h1 discount");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".menu-dropdown-expanded")).Text.Contains("Товары со скидкой"),
                "new enabled menu discount client");
        }
    }
}