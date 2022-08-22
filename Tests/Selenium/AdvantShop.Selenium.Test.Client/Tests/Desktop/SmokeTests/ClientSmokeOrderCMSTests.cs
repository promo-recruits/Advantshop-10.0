using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AdvantShop.Selenium.Test.Client.Tests.Desktop.SmokeTests
{
    [TestFixture]
    public class ClientSmokeOrderCMSTests : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS | ClearType.Catalog | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Client\\SmokeTests\\CMS.StaticBlock.csv",
                "data\\Client\\SmokeTests\\CMS.StaticPage.csv",
                "data\\Client\\SmokeTests\\Catalog.Photo.csv",
                "data\\Client\\SmokeTests\\Catalog.Category.csv",
                "data\\Client\\SmokeTests\\Catalog.Brand.csv",
                "data\\Client\\SmokeTests\\Catalog.Product.csv",
                "data\\Client\\SmokeTests\\Catalog.ProductCategories.csv",
                "data\\Client\\SmokeTests\\Catalog.Tag.csv",
                "data\\Client\\SmokeTests\\Catalog.Property.csv",
                "data\\Client\\SmokeTests\\Catalog.PropertyValue.csv",
                "data\\Client\\SmokeTests\\Catalog.ProductPropertyValue.csv",
                "data\\Client\\SmokeTests\\Catalog.PropertyGroup.csv",
                "data\\Client\\SmokeTests\\Catalog.Color.csv",
                "data\\Client\\SmokeTests\\Catalog.Size.csv",
                "data\\Client\\SmokeTests\\Catalog.Offer.csv",
                "data\\Client\\SmokeTests\\CMS.Menu.csv",
                "data\\Client\\SmokeTests\\Settings.NewsCategory.csv",
                "data\\Client\\SmokeTests\\Settings.News.csv",
                "data\\Client\\SmokeTests\\Customers.Customer.csv",
                "data\\Client\\SmokeTests\\Customers.Contact.csv",
                "data\\Client\\SmokeTests\\Customers.CustomerGroup.csv"
            );

            Init();
            EnableInplaceOff();

            Functions.EnableCapcha(Driver, BaseUrl);
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
        public void ClientProductToCart()
        {
            GoToClient("products/test-product106");

            Driver.FindElement(By.CssSelector(".details-spinbox-block")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.CssSelector(".details-spinbox-block")).FindElement(By.TagName("input")).Clear();
            Driver.FindElement(By.CssSelector(".details-spinbox-block")).FindElement(By.TagName("input")).SendKeys("2");
            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить')]")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");
            Driver.WaitForElem(By.CssSelector(".btn-middle.btn-submit"));
            VerifyAreEqual("Корзина", Driver.FindElement(By.TagName("h1")).Text);
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct106"));
            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector(".cart-full-amount-control input")).GetAttribute("value"));
            Driver.FindElement(By.CssSelector(".cart-full-amount-control input")).Click();
            Driver.FindElement(By.CssSelector(".cart-full-amount-control input")).Clear();
            Driver.FindElement(By.CssSelector(".cart-full-amount-control input")).SendKeys("3");
            Thread.Sleep(500);
            Driver.DropFocus("h1");
            VerifyAreEqual("318 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text);
        }

        [Test]
        public void ClientProductDeleteFromCart()
        {
            GoToClient("products/test-product107");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить')]")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".cart-full-name-link")).Text.Contains("TestProduct107"));
            VerifyIsTrue(0 == Driver.FindElements(By.CssSelector(".cart-full-empty")).Count);
            Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-remove")).Click();
            Thread.Sleep(3000);
            GoToClient("cart");
            VerifyIsTrue(0 == Driver.FindElements(By.CssSelector(".cart-full-name-link")).Count);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".cart-full-empty")).Displayed);
        }


        [Test]
        public void ClientProductDeleteAllFromCart()
        {
            GoToClient("products/test-product108");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить')]")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("products/test-product109");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить')]")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");

            VerifyIsTrue(Driver.PageSource.Contains("TestProduct108"));
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct109"));
            Driver.FindElement(By.CssSelector(".cart-full-header-item.cart-full-remove")).Click();
            Thread.Sleep(3000);
            GoToClient("cart");
            VerifyIsTrue(Driver.PageSource.Contains("Ваш заказ не содержит товаров"));
        }

        [Test]
        public void ClientStaticPageOpen()
        {
            GoToClient("pages/about");

            VerifyAreEqual("О магазине", Driver.FindElement(By.TagName("h1")).Text);
        }

        [Test]
        public void ClientNewsOpen()
        {
            GoToClient();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".block.news-block")).Text.Contains("Test News 1 title"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".news-block-row")).Count == 1);

            Driver.FindElement(By.LinkText("Все новости")).Click();
            Thread.Sleep(4000);

            VerifyAreEqual("Новости", Driver.FindElement(By.TagName("h1")).Text);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".h3")).Count == 2);

            Driver.FindElement(By.LinkText("NewsCategory2")).Click();
            Thread.Sleep(4000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".h3")).Text.Contains("Test News 2 title"));
        }

        [Test]
        public void ClientBrandOpen()
        {
            GoToClient("manufacturers");

            VerifyAreEqual("Бренды", Driver.FindElement(By.TagName("h1")).Text);
            VerifyIsFalse(Driver.PageSource.Contains("DescripBrand1"));
            VerifyIsTrue(Driver.PageSource.Contains("BriefBrand1"));

            Driver.FindElement(By.LinkText("BrandName1")).Click();
            Thread.Sleep(4000);

            VerifyAreEqual("BrandName1", Driver.FindElement(By.CssSelector(".site-body-main h1")).Text);
            VerifyIsTrue(Driver.PageSource.Contains("DescripBrand1"));
            VerifyIsFalse(Driver.PageSource.Contains("BriefBrand1"));
        }

        [Test]
        public void ClientSearch()
        {
            ReindexSearch();
            GoToClient();

            //check search exist item
            Driver.FindElement(By.Name("q")).Click();
            Driver.FindElement(By.Name("q")).Clear();
            Driver.FindElement(By.Name("q")).SendKeys("TestProduct21");
            Driver.FindElement(By.XPath("//span[contains(text(), 'Найти')]")).Click();
            Thread.Sleep(4000);
            VerifyIsTrue(Driver
                             .FindElements(
                                 By.CssSelector(".products-view-item.text-static.cs-br-1.js-products-view-item"))
                             .Count ==
                         1);
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(".products-view-item.text-static.cs-br-1.js-products-view-item")).Text
                .Contains("TestProduct21"));

            //check search not exist item
            Driver.FindElement(By.Name("q")).Click();
            Driver.FindElement(By.Name("q")).Clear();
            Driver.FindElement(By.Name("q")).SendKeys("TestProduct2000");
            Driver.FindElement(By.XPath("//span[contains(text(), 'Найти')]")).Click();
            Thread.Sleep(4000);

            VerifyIsTrue(Driver.PageSource.Contains("Найдено 0 по запросу \"TestProduct2000\""));
        }


        [Test]
        public void ClientFeedbackMessage()
        {
            GoToClient("feedback");

            VerifyAreEqual("Отправить сообщение", Driver.FindElement(By.TagName("h1")).Text);

            Driver.FindElement(By.LinkText("Вопрос")).Click();
            Thread.Sleep(4000);

            Driver.FindElement(By.Id("Message")).Click();
            Driver.FindElement(By.Id("Message")).Clear();
            Driver.FindElement(By.Id("Message")).SendKeys("Test Message");
            Driver.ScrollTo(By.Id("Name"));
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("Test Name");
            Driver.FindElement(By.Id("Email")).Click();
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("TestEmail@gmail.com");
            Driver.FindElement(By.Id("Phone")).Click();
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89345263412");

            Driver.ScrollTo(By.CssSelector(".btn.btn-submit.btn-middle"));
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(4000);

            VerifyIsTrue(Driver.PageSource.Contains("Спасибо за сообщение!"));
        }

        [Test]
        public void ClientOrder()
        {
            GoToClient("products/test-product103");

            Driver.ScrollTo(By.CssSelector("[title=\"SizeName1\"]"));
            Driver.FindElement(By.CssSelector("[data-product-id=\"103\"]")).Click();
            Thread.Sleep(4000);

            GoToClient("cart");

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Оформить')]")).Click();
            Thread.Sleep(5000);

            Driver.ScrollTo(By.Id("CustomerComment"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(5000);
            VerifyIsTrue(Driver.Url.Contains("success"));
            Thread.Sleep(1000);
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text);
        }

        [Test]
        public void ClientOrderOneClick()
        {
            GoToClient("products/test-product103");

            Driver.ScrollTo(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before"));
            Driver.FindElement(By.LinkText("Купить в один клик")).Click();

            Driver.FindElement(By.Id("buyOneClickFormName")).Click();
            Driver.FindElement(By.Id("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormName")).SendKeys("OneClickName");

            Driver.FindElement(By.Id("buyOneClickFormPhone")).Click();
            Driver.ClearInput(By.Id("buyOneClickFormPhone"));
            Driver.FindElement(By.Id("buyOneClickFormPhone")).SendKeys("55555555555");

            Driver.FindElement(By.CssSelector("[value=\"Заказать\"]")).Click();
            Thread.Sleep(3000);

            VerifyIsTrue(Driver.PageSource.Contains("Спасибо, ваш заказ оформлен!"));
        }

        [Test]
        public void ClientCheckOrder()
        {
            GoToClient();
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-action")).Click();
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector(".input-xsmall")).GetCssValue("border-color"),
                "invalid field border");
            Driver.FindElement(By.CssSelector(".input-xsmall")).Click();
            Driver.FindElement(By.CssSelector(".input-xsmall")).SendKeys("3");
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-action")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".adv-modal-inner")).Displayed);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-header")).Text.Contains("Статус заказа"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-content")).Text.Contains("Заказ не найден"));
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Driver.FindElement(By.CssSelector(".input-xsmall")).Click();
            Driver.FindElement(By.CssSelector(".input-xsmall")).Clear();
            Driver.FindElement(By.CssSelector(".input-xsmall")).SendKeys("1");
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-action")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".adv-modal-inner")).Displayed);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-header")).Text.Contains("Статус заказа"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-content")).Text.Contains("№1 Доставлен"));
            Thread.Sleep(1000);
            Actions action = new Actions(Driver);
            action.MoveToElement(Driver.FindElement(By.CssSelector(".adv-modal-close")), 50, 50).Click();
            action.Perform();
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".adv-modal-inner")).Count > 0);
        }
    }
}