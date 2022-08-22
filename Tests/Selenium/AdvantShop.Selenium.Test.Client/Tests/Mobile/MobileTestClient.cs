using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Client.Tests.Mobile
{
    [TestFixture]
    public class MobileTestClient : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\MobileTest\\Catalog.Category.csv",
                "Data\\MobileTest\\Catalog.Product.csv",
                "Data\\MobileTest\\Catalog.ProductCategories.csv",
                "Data\\MobileTest\\Catalog.Offer.csv",
                "Data\\MobileTest\\CMS.Carousel.csv",
                "Data\\MobileTest\\Catalog.Photo.csv"
            );

            Init();
            Functions.AdminMobileOn(Driver, BaseUrl);
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"mobileShowSlider\"]")).FindElement(By.TagName("input"))
                .Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"mobileShowSlider\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"mobileShowCity\"]")).FindElement(By.TagName("input"))
                .Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"mobileShowCity\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            if (!Driver.FindElement(By.Id("EnableCatalogViewChange")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnableCatalogViewChange\"]")).Click();
            }

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"mobileDisplayHeader\"]"))
                .FindElement(By.TagName("input")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"mobileDisplayHeader\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.ScrollToTop();
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Enabled)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            GoToAdmin("modules");
            try
            {
                Driver.GridFilterSendKeys("Всплывающая корзина", By.TagName("h1"));
                if (Driver.FindElement(By.CssSelector(".item-module .adv-checkbox-input")).Selected)
                {
                    Driver.FindElement(By.CssSelector(".item-module .adv-checkbox-label span")).Click();
                }
            }
            catch
            {
            }
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
        public void Carousel()
        {
            GoToMobile();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".carousel-main-item.js-carousel-item")).Displayed,
                "carousel");
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".carousel-main-item.js-carousel-item")).Count,
                "count carousel");

            ReInit();
            GoToAdmin("settings/mobileversion");
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileShowSlider\"]")).FindElement(By.TagName("span"))
                .Click();
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
            Driver.WaitForToastSuccess();

            ReInitClient();
            GoToMobile();
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".carousel-main-item.js-carousel-item")).Count,
                "no carousel");
        }

        [Test]
        public void ChangeCity()
        {
            GoToMobile();
            Driver.FindElement(By.ClassName("mobile-header__menu-triger")).Click();
            Driver.FindElement(By.XPath("//button[@class='menu__city']")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".zone-dialog")).Count, "modal window");
            Driver.FindElement(By.Name("zoneCity")).Clear();
            Driver.FindElement(By.Name("zoneCity")).SendKeys("Ульяновск");
            Driver.FindElement(By.Name("zoneDialogForm")).FindElement(By.CssSelector(".btn.btn-submit.btn-big"))
                .Click();
            VerifyIsTrue(Driver.FindElement(By.XPath("//span[@data-ng-bind='zone.City']")).Text.Contains("Ульяновск"),
                "city in menu");
            //GoToMobile();

            ReInit();
            GoToAdmin("settings/mobileversion");
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileShowCity\"]")).FindElement(By.TagName("span")).Click();
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToMobile();
            Driver.FindElement(By.ClassName("mobile-header__menu-triger")).Click();
            VerifyIsFalse(Driver.PageSource.Contains("Ульяновск"), "no city in menu");
        }

        [Test]
        public void ChangeView()
        {
            GoToMobile("/categories/first");
            Driver.FindElement(By.CssSelector(".catalog-change-btn")).Click();
            VerifyAreEqual("catalog-view products-view-mobile-modern-list",
                Driver.FindElement(By.CssSelector(".catalog-view")).GetAttribute("class"), "List");

            Driver.FindElement(By.CssSelector(".catalog-change-btn")).Click();
            VerifyAreEqual("catalog-view products-view-mobile-modern-single",
                Driver.FindElement(By.CssSelector(".catalog-view")).GetAttribute("class"), "Single");

            Driver.FindElement(By.CssSelector(".catalog-change-btn")).Click();
            VerifyAreEqual("catalog-view products-view-mobile-modern-tile",
                Driver.FindElement(By.CssSelector(".catalog-view")).GetAttribute("class"), "Tile");
        }

        [Test]
        public void FilterProduct()
        {
            GoToMobile();
            Driver.FindElement(By.ClassName("mobile-header__menu-triger")).Click();
            Driver.FindElement(By.XPath("//div[@data-mobile-menu-item='1']")).Click();
            //Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("categories/first"), "url");

            //при переходе через выбор в селекте не срабатывает WaitAngular и фильтры не прогружаются
            // GoToClient("categories/first");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile.icon-right-open-big-after")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".col-xs-5 input")).GetAttribute("value"),
                "filter value min");
            VerifyAreEqual("19", Driver.FindElements(By.CssSelector(".col-xs-5 input"))[1].GetAttribute("value"),
                "filter value max");
            Driver.FindElement(By.Name("filteredModelMin")).Click();
            Driver.FindElement(By.Name("filteredModelMin")).SendKeys(Keys.Backspace);
            Driver.FindElement(By.Name("filteredModelMin")).SendKeys("10");
            Driver.FindElement(By.Name("filteredModelMax")).Click();
            Driver.FindElement(By.Name("filteredModelMax")).SendKeys(Keys.Backspace);
            Driver.FindElement(By.Name("filteredModelMax")).SendKeys(Keys.Backspace);
            Driver.FindElement(By.Name("filteredModelMax")).SendKeys("15");
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(6, Driver.FindElements(By.CssSelector(".catalog-product-item")).Count, "count find elem");
            VerifyAreEqual("TestProduct8", Driver.FindElement(By.CssSelector(".prod-name.text-floating")).Text,
                "find elem 1");
            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile.icon-right-open-big-after")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(19, Driver.FindElements(By.CssSelector(".catalog-product-item")).Count, "close filter");
        }

        [Test]
        public void GoToCategory()
        {
            GoToMobile();
            Driver.FindElement(By.ClassName("mobile-header__menu-triger")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//div[@data-mobile-menu-item='1']")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".catalog-product-item")).Count == 19, "count item 1");
            VerifyAreEqual("first pr", Driver.FindElement(By.CssSelector(".prod-name.text-floating")).Text,
                "name product");
        }

        [Test]
        public void GoToEmptyCart()
        {
            GoToMobile();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".mobile-header__item.mobile-header__cart-block")).Displayed,
                "icon");
            Driver.FindElement(By.CssSelector(".mobile-header__item.mobile-header__cart-block")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("КОРЗИНА", Driver.FindElement(By.CssSelector(".sidebar__header")).Text, "cart");
            Thread.Sleep(2000);
            VerifyAreEqual("Ваш заказ не содержит товаров", Driver.FindElement(By.CssSelector(".cart-mini-empty")).Text,
                "empty cart");
            Driver.FindElement(By.CssSelector(".sidebar__close")).Click();
        }

        [Test]
        public void GoToProduct()
        {
            GoToMobile();
            Driver.FindElement(By.ClassName("mobile-header__menu-triger")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//div[@data-mobile-menu-item='1']")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector(".prod-name.text-floating")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("first pr", Driver.FindElement(By.TagName("h1")).Text, "");
            /*driver.FindElement(By.CssSelector("")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(driver.Url.Contains("/categories/first"), "");
            //Возможно нужно указывать имя категории
            VerifyAreEqual("Каталог", driver.FindElement(By.CssSelector("")).Text, "");*/
        }

        [Test]
        public void MobilePhone()
        {
            ReInit();
            GoToAdmin("settings/common");
            Driver.FindElement(By.Id("MobilePhone")).Click();
            Driver.FindElement(By.Id("MobilePhone")).Clear();
            Driver.FindElement(By.Id("MobilePhone")).SendKeys("89022222222");
            Thread.Sleep(2000);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"] input")).Click();
            Thread.Sleep(2000);

            GoToMobile();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-ng-href=\"tel:89022222222\"]")).Displayed,
                "icon tel");

            ReInit();
            GoToAdmin("settings/common");
            Driver.FindElement(By.Id("MobilePhone")).Click();
            Driver.FindElement(By.Id("MobilePhone")).Clear();
            Thread.Sleep(2000);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"] input")).Click();
            Thread.Sleep(2000);
            GoToMobile();
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-ng-href=\"tel:\"]")).Displayed, "no icon");
        }

        [Test]
        public void OpenMobile()
        {
            ReInit();
            GoToAdmin("settings/mobileversion");
            Driver.FindElement(By.Id("MainPageProductCountMobile")).Clear();
            Driver.FindElement(By.Id("MainPageProductCountMobile")).SendKeys("5");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();

            GoToMobile();
            VerifyAreEqual("Мой магазин",
                Driver.FindElement(By.CssSelector(".mobile-header__item.mobile-header__logo-block")).Text, "header");
            VerifyIsTrue(Driver.FindElement(By.LinkText("Полная версия сайта")).Displayed, "full version");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".mainpage-products--best")).Displayed, "bestsellers");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".mainpage-products__content .mainpage-products__content-item"))
                    .Count == 5, "count bestsellers");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".novelty-section.mainpage-products")).Displayed, "news");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".novelty-section__content .mainpage-products__content-item"))
                    .Count == 5, "count news");
            VerifyAreEqual("TestProduct11",
                Driver.FindElements(
                    By.CssSelector(".mainpage-products__content .mainpage-products__content-item .prod-name"))[0].Text,
                "bestsellers product name 1");
            VerifyAreEqual("TestProduct13",
                Driver.FindElements(
                    By.CssSelector(".mainpage-products__content .mainpage-products__content-item .prod-name"))[2].Text,
                "bestsellers product name 3");
            VerifyAreEqual("TestProduct6",
                Driver.FindElements(
                    By.CssSelector(".novelty-section__content .mainpage-products__content-item .prod-name"))[0].Text,
                "news product name 1");
            VerifyAreEqual("TestProduct8",
                Driver.FindElements(
                    By.CssSelector(".novelty-section__content .mainpage-products__content-item .prod-name"))[2].Text,
                "news product name 3");

            Driver.FindElement(By.LinkText("Все хиты продаж")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Хиты продаж", Driver.FindElement(By.CssSelector("h1")).Text, "h1 bestsellers");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".catalog-view .product-view-mobile-list__item")).Count == 5,
                "count all bestsellers ");
            VerifyAreEqual("TestProduct11",
                Driver.FindElements(By.CssSelector(".catalog-view .product-view-mobile-list__item .prod-name"))[0].Text,
                "bestsellers prod");
            //Назад по кнопке
            Driver.FindElement(By.CssSelector(".mobile-header__item.mobile-header__logo-block")).Click();
            Thread.Sleep(2000);
            Driver.ScrollTo(By.CssSelector(".mainpage-products__content .mainpage-products__content-item"));
            Driver.FindElement(By.LinkText("Все новинки")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Новинки", Driver.FindElement(By.CssSelector("h1")).Text, "h1 news");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".catalog-view .product-view-mobile-list__item")).Count == 5,
                "count news");
            VerifyAreEqual("TestProduct6",
                Driver.FindElements(By.CssSelector(".catalog-view .product-view-mobile-list__item .prod-name"))[0].Text,
                "product news");

            ReInit();
            GoToAdmin("settings/mobileversion");
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileDisplayHeader\"]")).Click();
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
        }

        [Test]
        public void OpenProductPage()
        {
            GoToMobile("/products/first-pr");
            VerifyAreEqual("first pr", Driver.FindElement(By.TagName("h1")).Text, "h1 product");
            VerifyAreEqual("449", Driver.FindElement(By.CssSelector(".details-param-value.inplace-offset")).Text,
                "atr num");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".price-number")).Text, "price");
            VerifyAreEqual(" руб.", Driver.FindElement(By.CssSelector(".price-currency")).Text,
                "currency"); //availability  available
            VerifyAreEqual("Есть в наличии", Driver.FindElement(By.CssSelector(".availability.available")).Text);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".btn-middle.btn-confirm.btn-block")).Count, "icon");
            VerifyAreEqual("Описание",
                Driver.FindElements(By.CssSelector(".product-data .product-data__header"))[0].Text, "description");
            VerifyAreEqual("Description1",
                Driver.FindElements(By.CssSelector(".product-data .product-data__body"))[0].Text, "description1");
        }

        [Test]
        public void OpenMobileHeader()
        {
            ReInit();
            GoToAdmin("settings/mobileversion");
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileDisplayHeader\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();

            GoToMobile();
            VerifyAreEqual("Мой магазин",
                Driver.FindElement(By.CssSelector(".mobile-header__item.mobile-header__logo-block")).Text, "header");
            ReInit();
            GoToAdmin("settings/mobileversion");
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileDisplayHeader\"]")).FindElement(By.TagName("span"))
                .Click();
            Driver.FindElement(By.Id("HeaderCustomTitle")).Clear();
            Driver.FindElement(By.Id("HeaderCustomTitle")).SendKeys("NewHeader");
            Thread.Sleep(2000);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
            Thread.Sleep(2000);

            GoToMobile();
            VerifyAreEqual("NewHeader",
                Driver.FindElement(By.CssSelector(".mobile-header__item.mobile-header__logo-block")).Text,
                "new header");
        }

        [Test]
        public void ProductsOnMainPage()
        {
            ReInit();
            GoToAdmin("settings/mobileversion");
            Driver.FindElement(By.Id("MainPageProductCountMobile")).Clear();
            Driver.FindElement(By.Id("MainPageProductCountMobile")).SendKeys("5");
            Thread.Sleep(2000);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
            Thread.Sleep(2000);

            GoToMobile();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".mainpage-products--best")).Displayed, "bestsellers");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".novelty-section.mainpage-products")).Displayed, "news");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".novelty-section .mainpage-products__content-item")).Count == 5,
                "count news");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".mainpage-products__content .mainpage-products__content-item"))
                    .Count == 5, "count bestsellers");

            ReInit();
            GoToAdmin("settings/mobileversion");
            Driver.FindElement(By.Id("MainPageProductCountMobile")).Clear();
            Driver.FindElement(By.Id("MainPageProductCountMobile")).SendKeys("3");
            Thread.Sleep(2000);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
            Thread.Sleep(2000);

            GoToMobile();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".mainpage-products--best")).Displayed, "bestsellers");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".novelty-section.mainpage-products")).Displayed, "news");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".novelty-section .mainpage-products__content-item")).Count == 3,
                "count news");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".mainpage-products__content .mainpage-products__content-item"))
                    .Count == 3, "count bestsellers");
        }

        [Test]
        public void ProductToCart()
        {
            ReInit();
            DelItemFromCart();
            GoToMobile("/products/first-pr");
            VerifyAreEqual("first pr", Driver.FindElement(By.TagName("h1")).Text, "h1");
            Driver.FindElement(By.CssSelector(".btn-middle.btn-confirm.btn-block")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".swal2-popup")).Displayed, "display pop up add to cart ");
            Driver.FindElement(By.CssSelector(".btn-small.btn-expander.btn-action")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("1", Driver.FindElements(By.CssSelector(".mobile-header__cart-count"))[0].Text,
                "count prods");
            VerifyAreEqual("first pr",
                Driver.FindElement(By.CssSelector(".cart-full-mobile-name-link")).Text,
                "name prod");
            VerifyAreEqual("1 руб.", Driver.FindElement(By.CssSelector(".cart-full-mobile-item-cost")).Text,
                "price ");
            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).GetAttribute("value"), "count");
            Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).SendKeys("00");
            Thread.Sleep(1000);

            Refresh();
            VerifyAreEqual("100",
                Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).GetAttribute("value"),
                "change count");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".cart-full-mobile-result-price")).Text,
                "rezult price"); //cart-full-error panel cs-br-1
            Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).Clear();
            Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).SendKeys("1");
            Thread.Sleep(1000);
            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).GetAttribute("value"),
                "change 1 count");
            Driver.FindElement(By.CssSelector(".cart-full-mobile-remove")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.PageSource.Contains("Ваш заказ не содержит товаров"), "empty cart");
        }

        [Test]
        public void ProductToCartOrderFullVersion()
        {
            ReInit();
            Functions.AdminMobileCheckoutOff(Driver, BaseUrl);
            DelItemFromCart();
            GoToMobile("/products/test-product20");
            VerifyAreEqual("TestProduct20", Driver.FindElement(By.TagName("h1")).Text, "h1");
            Driver.FindElement(By.CssSelector(".btn-middle.btn-confirm.btn-block")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".swal2-popup")).Displayed, "display pop up add to cart ");
            Driver.FindElement(By.CssSelector(".btn-small.btn-expander.btn-action")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".mobile-header__cart-count")).Text, "count prods");
            VerifyAreEqual("TestProduct20", Driver.FindElement(By.CssSelector(".cart-full-mobile-name-link")).Text,
                "name prod");
            VerifyAreEqual("22 руб.", Driver.FindElement(By.CssSelector(".cart-full-mobile-item-cost")).Text, "coast");
            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).GetAttribute("value"), "count");
            Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).Clear();
            Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).SendKeys("10");
            Thread.Sleep(1000);

            Refresh();
            VerifyAreEqual("220 руб.", Driver.FindElement(By.CssSelector(".cart-full-mobile-result-price")).Text,
                "new coast");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".btn-middle.btn-submit.btn-disabled")).Count == 0,
                "disabled btn");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cart-full-error.panel.cs-br-1")).Count == 0, "error");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".btn-middle.btn-submit")).Count > 0, "sucsess");
            Driver.FindElement(By.CssSelector(".btn-middle.btn-submit")).Click();
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "checkout h1");
            //Add adress
            /*  driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action")).Click();
              Thread.Sleep(2000);
              VerifyIsTrue(driver.FindElements(By.CssSelector(".modal-inner.address-dialog.ng-pristine.ng-valid.ng-valid-required")).Count > 0);
              driver.FindElement(By.CssSelector(".col-xs-9")).SendKeys("Customer1");
              //  (new SelectElement(driver.FindElements(By.CssSelector(".col-xs-9"))[1])).SelectByText("Австрия");
              driver.FindElements(By.CssSelector(".col-xs-9"))[2].SendKeys("Ульяновская область");
              driver.FindElements(By.CssSelector(".col-xs-9"))[3].SendKeys("Ульяновск");
              driver.FindElements(By.CssSelector(".col-xs-9"))[4].SendKeys("NewAdress1");
              driver.FindElements(By.CssSelector(".col-xs-9"))[5].SendKeys("111222");
              driver.FindElement(By.CssSelector(".col-xs-8.col-xs-offset-4")).Click();
              Thread.Sleep(2000);

              //Edit Adress
              driver.FindElement(By.CssSelector(".address-controls-item")).Click();
              Thread.Sleep(2000);
              VerifyIsTrue(driver.FindElements(By.CssSelector(".modal-inner.address-dialog.ng-pristine.ng-valid.ng-valid-required")).Count > 0);
              VerifyAreEqual("Customer1", driver.FindElement(By.CssSelector(".col-xs-9")).GetAttribute("value"));
              VerifyAreEqual("Россия", driver.FindElements(By.CssSelector(".col-xs-9"))[1].Text);
              VerifyAreEqual("Ульяновская область", driver.FindElements(By.CssSelector(".col-xs-9"))[2].GetAttribute("value"));
              VerifyAreEqual("Ульяновск", driver.FindElements(By.CssSelector(".col-xs-9"))[3].GetAttribute("value"));
              VerifyAreEqual("NewAdress1", driver.FindElements(By.CssSelector(".col-xs-9"))[4].GetAttribute("value"));
              VerifyAreEqual("111222", driver.FindElements(By.CssSelector(".col-xs-9"))[5].GetAttribute("value"));
              driver.FindElements(By.CssSelector(".col-xs-8.col-xs-offset-4"))[1].Click();
              Thread.Sleep(2000);
              VerifyAreEqual("Россия", driver.FindElement(By.CssSelector("[data-ng-bind=\"item.Country\"]")).Text);
              VerifyAreEqual("Ульяновская область", driver.FindElement(By.CssSelector("[data-ng-if=\"item.Region\"]")).Text);
              VerifyAreEqual("Ульяновск", driver.FindElement(By.CssSelector("[data-ng-if=\"item.City\"]")).Text);
              VerifyAreEqual("NewAdress1", driver.FindElement(By.CssSelector("[data-ng-if=\"item.Address\"]")).Text);
              //Add adress
              driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action")).Click();
              Thread.Sleep(2000);
              driver.FindElement(By.CssSelector(".col-xs-9")).SendKeys("Customer10");
              (new SelectElement(driver.FindElements(By.CssSelector(".col-xs-9"))[1])).SelectByText("Австрия");
              driver.FindElements(By.CssSelector(".col-xs-9"))[2].SendKeys("Reg1");
              driver.FindElements(By.CssSelector(".col-xs-9"))[3].SendKeys("City1");
              driver.FindElements(By.CssSelector(".col-xs-9"))[4].SendKeys("Adress1");
              driver.FindElements(By.CssSelector(".col-xs-9"))[5].SendKeys("111111");
              driver.FindElement(By.CssSelector(".col-xs-8.col-xs-offset-4")).Click();
              Thread.Sleep(2000);
              VerifyIsTrue(driver.FindElements(By.CssSelector("address-list-item")).Count == 2);
              driver.FindElements(By.CssSelector(".address-controls-item.cs-l-5'"))[1].Click();
              VerifyIsTrue(driver.FindElements(By.CssSelector("address-list-item")).Count == 1);
             
            //shipping method
            driver.FindElements(By.CssSelector(".shipping-item"))[1].Click();
            driver.FindElements(By.CssSelector(".shipping-item"))[2].Click();
            driver.FindElements(By.CssSelector(".shipping-item"))[3].Click();
            //payment method
            driver.FindElements(By.CssSelector(".payment-item"))[1].Click();
            driver.FindElements(By.CssSelector(".payment-item"))[2].Click();
            driver.FindElements(By.CssSelector(".payment-item"))[3].Click();
            */

            Driver.XPathContainsText("span", "Самовывоз");
            //  Driver.ScrollTo(By.CssSelector("[data-payment-list]"));
            Driver.XPathContainsText("span", "наличными");
            //Comment CustomerComment
            Driver.FindElement(By.Id("CustomerComment")).SendKeys("Customer Comment 1");
            Thread.Sleep(1000);
            VerifyAreEqual("220 руб.", Driver.FindElement(By.CssSelector(".checkout-result-price")).Text,
                "checkout price");

            // VerifyAreEqual(" 6,6  руб.", driver.FindElements(By.CssSelector(".checkout-result-price"))[1].Text);
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector(".btn-big.btn-submit")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 sucsess order");

            ReInit();
            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");
            // VerifyAreEqual("Customer1", Driver.GetGridCell(0, "BuyerName").Text, " Grid orders buyer");
            VerifyAreEqual("220 руб.", Driver.GetGridCell(0, "SumFormatted").Text, " Grid orders Sum");
            VerifyIsTrue(
                Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "grid orders CreationDates");
            //check order
            Driver.GetGridCell(0, "Number").Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));
            VerifyAreEqual("TestProduct20\r\nАртикул: 520", Driver.GetGridCell(0, "Name", "OrderItems").Text,
                " Grid in order CustomName");
            VerifyAreEqual("22", Driver.GetGridCell(0, "PriceString", "OrderItems").Text, " Grid in order Price");
            VerifyAreEqual("10", Driver.GetGridCell(0, "Amount", "OrderItems").Text, " Grid in order Amount");
            VerifyAreEqual("220 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Grid in order Cost");

            IWebElement selectElem1 = Driver.FindElement(By.Id("Order_OrderSourceId"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Мобильная версия"), "select item source");
            VerifyAreEqual("220 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSum\"]")).Text,
                "price rezultt");
        }

        [Test]
        public void ProductToCartOrderMobileVersion()
        {
            ReInit();
            Functions.AdminMobileCheckoutOn(Driver, BaseUrl);
            DelItemFromCart();
            GoToMobile("/products/test-product20");
            VerifyAreEqual("TestProduct20", Driver.FindElement(By.TagName("h1")).Text, "h1");
            Driver.FindElement(By.CssSelector(".btn-middle.btn-confirm.btn-block")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".swal2-popup")).Displayed, "display pop up add to cart ");
            Driver.FindElement(By.CssSelector(".btn-small.btn-expander.btn-action")).Click();
            Thread.Sleep(1000);

            GoToMobile("cart");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".mobile-header__cart-count")).Text, "count prods");
            VerifyAreEqual("TestProduct20",
                Driver.FindElement(By.CssSelector(".cart-full-mobile-name-link")).Text, "prod name");
            VerifyAreEqual("22 руб.", Driver.FindElement(By.CssSelector(".cart-full-mobile-item-cost")).Text, "coast");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).GetAttribute("value"), "count");
            Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).Clear();
            Driver.FindElement(By.CssSelector(".spinbox-input-wrap input")).SendKeys("10");
            Thread.Sleep(1000);
            Refresh();
            VerifyAreEqual("220 руб.", Driver.FindElement(By.CssSelector(".cart-full-mobile-result-price")).Text,
                "rezult price");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".btn-middle.btn-submit.btn-disabled")).Count == 0,
                "disabled btn");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cart-full-error.panel")).Count == 0,
                "error panel");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".btn-middle.btn-submit")).Count > 0, "sucess btn");
            Driver.FindElement(By.CssSelector(".btn-middle.btn-submit")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "checkout rezult");
            Driver.FindElement(By.Name("Name")).SendKeys("Customer1");
            Driver.FindElement(By.Name("Phone")).Click();
            Driver.FindElement(By.Name("Phone")).SendKeys("9999999999");
            Driver.FindElement(By.Name("Message")).SendKeys("new test order");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".btn-confirm.btn-big")).Click();
            Driver.WaitForElem(By.CssSelector(".btn-confirm.btn-small"));
            Driver.WaitForElem(By.CssSelector(".checkout-confirm-txt"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-confirm-txt")).Text
                    .Contains("Спасибо, ваш заказ оформлен."), "checkout sucess");

            ReInit();
            GoToAdmin("orders");

            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");
            VerifyAreEqual("Customer1", Driver.GetGridCell(0, "BuyerName").Text, " Grid orders buyer");
            VerifyAreEqual("220 руб.", Driver.GetGridCell(0, "SumFormatted").Text, " Grid orders Sum");
            VerifyIsTrue(
                Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "grid orders CreationDates");
            //check order
            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestProduct20",
                Driver.GetGridCell(0, "Name", "OrderItems").FindElement(By.TagName("a")).Text,
                " Grid in order CustomName");
            VerifyAreEqual("22", Driver.GetGridCell(0, "PriceString", "OrderItems").Text, " Grid in order Price");
            VerifyAreEqual("10", Driver.GetGridCell(0, "Amount", "OrderItems").Text, " Grid in order Amount");
            VerifyAreEqual("220 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Grid in order Cost");

            IWebElement selectElem1 = Driver.FindElement(By.Id("Order_OrderSourceId"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Мобильная версия"), "select item source");
            VerifyAreEqual("220 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSum\"]")).Text,
                "price rezultt");
        }

        [Test]
        public void SearchProduct()
        {
            ReInit();
            GoToAdmin("settings/mobileversion");
            if (!Driver.FindElement(By.Id("DisplayHeaderTitle")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"mobileDisplayHeader\"]")).FindElement(By.TagName("span"))
                    .Click();
                Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
                Thread.Sleep(2000);
            }

            GoToMobile();
            Driver.FindElement(By.CssSelector(".mobile-header__item .mobile-header__search-btn")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input.input-small"))
                .SendKeys("TestProduct1000");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            //VerifyAreEqual("Найдено 0 по запросу "TestProduct1000"", driver.FindElement(By.Id("content")).FindElement(By.CssSelector(".catalog-title")).Text, "header 2");
            VerifyIsTrue(Driver.PageSource.Contains("К сожалению, по вашему запросу ничего не найдено"),
                "no exist by button");

            GoToMobile();
            Driver.FindElement(By.CssSelector(".mobile-header__item .mobile-header__search-btn")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input.input-small"))
                .SendKeys("TestProduct11");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.LinkText("TestProduct11")).Displayed, "TestProduct11 найден");
        }

        [Test]
        public void SortProduct()
        {
            ReInit();
            GoToMobile("/categories/first");

            VerifyAreEqual("nosorting", Driver.FindElement(By.TagName("select")).GetAttribute("value"),
                "select nosorting");
            VerifyAreEqual("first pr",
                Driver.FindElements(By.ClassName("catalog-product-item"))[0]
                    .FindElement(By.CssSelector(".prod-name.text-floating")).Text, "value nosorting");

            (new SelectElement(Driver.FindElement(By.TagName("select")))).SelectByText("Популярные");
            Thread.Sleep(2000);
            VerifyAreEqual("descbypopular", Driver.FindElement(By.TagName("select")).GetAttribute("value"),
                "select popular");
            VerifyAreEqual("first pr", Driver.FindElement(By.CssSelector(".prod-name.text-floating")).Text,
                "popular value");
            VerifyAreEqual("TestProduct8", Driver.FindElements(By.CssSelector(".prod-name.text-floating"))[9].Text,
                "popular 10");

            (new SelectElement(Driver.FindElement(By.TagName("select")))).SelectByText("Новинки");
            Thread.Sleep(2000);
            VerifyAreEqual("descbyaddingdate", Driver.FindElement(By.TagName("select")).GetAttribute("value"),
                "select addingdate");
            VerifyAreEqual("TestProduct17", Driver.FindElement(By.CssSelector(".prod-name.text-floating")).Text,
                "addingdate value");
            /*
            (new SelectElement(driver.FindElement(By.TagName("select")))).SelectByText("Названию, по возрастанию");
            Thread.Sleep(2000);
            VerifyAreEqual("ascbyname", driver.FindElement(By.TagName("select")).GetAttribute("value"), "select ascbyname");
            VerifyAreEqual("first pr", driver.FindElement(By.CssSelector(".prod-name.text-floating")).Text, "ascbyname value1");
            VerifyAreEqual("TestProduct16", driver.FindElements(By.CssSelector(".prod-name.text-floating"))[9].Text, "ascbyname value 2");

            (new SelectElement(driver.FindElement(By.TagName("select")))).SelectByText("Названию, по убыванию");
            Thread.Sleep(2000);
            VerifyAreEqual("descbyname", driver.FindElement(By.TagName("select")).GetAttribute("value"), "select descbyname");
            VerifyAreEqual("TestProduct9", driver.FindElement(By.CssSelector(".prod-name.text-floating")).Text, "descbyname 1");
            VerifyAreEqual("TestProduct8", driver.FindElements(By.CssSelector(".prod-name.text-floating"))[1].Text, "descbyname 2");
            VerifyAreEqual("TestProduct16", driver.FindElements(By.CssSelector(".prod-name.text-floating"))[9].Text, "descbyname 10");
            */
            (new SelectElement(Driver.FindElement(By.TagName("select")))).SelectByText("Сначала дешевле");
            Thread.Sleep(2000);
            VerifyAreEqual("ascbyprice", Driver.FindElement(By.TagName("select")).GetAttribute("value"),
                "select ascbyprice");
            VerifyAreEqual("first pr", Driver.FindElement(By.CssSelector(".prod-name.text-floating")).Text,
                "ascbyprice 1");
            VerifyAreEqual("TestProduct8", Driver.FindElements(By.CssSelector(".prod-name.text-floating"))[9].Text,
                "ascbyprice 10");

            (new SelectElement(Driver.FindElement(By.TagName("select")))).SelectByText("Сначала дороже");
            Thread.Sleep(2000);
            VerifyAreEqual("descbyprice", Driver.FindElement(By.TagName("select")).GetAttribute("value"),
                "select DescByPrice");
            VerifyAreEqual("TestProduct17", Driver.FindElement(By.CssSelector(".prod-name.text-floating")).Text,
                "DescByPrice 1");
            VerifyAreEqual("TestProduct8", Driver.FindElements(By.CssSelector(".prod-name.text-floating"))[9].Text,
                "DescByPrice 10");

            (new SelectElement(Driver.FindElement(By.TagName("select")))).SelectByText("По размеру скидки");
            Thread.Sleep(2000);
            VerifyAreEqual("descbydiscount", Driver.FindElement(By.TagName("select")).GetAttribute("value"),
                "select DescByDiscount");
            VerifyAreEqual("first pr", Driver.FindElement(By.CssSelector(".prod-name.text-floating")).Text,
                "DescByDiscount 1");
            VerifyAreEqual("TestProduct8", Driver.FindElements(By.CssSelector(".prod-name.text-floating"))[9].Text,
                "DescByDiscount 10");

            (new SelectElement(Driver.FindElement(By.TagName("select")))).SelectByText("Высокий рейтинг");
            Thread.Sleep(2000);
            VerifyAreEqual("descbyratio", Driver.FindElement(By.TagName("select")).GetAttribute("value"),
                "select DescByRatio");
            VerifyAreEqual("first pr", Driver.FindElement(By.CssSelector(".prod-name.text-floating")).Text,
                "DescByRatio 1");
            VerifyAreEqual("TestProduct8", Driver.FindElements(By.CssSelector(".prod-name.text-floating"))[9].Text,
                "DescByRatio 10");
        }

        [Test]
        public void ProductAddToWishlist()
        {
            ReInit();
            GoToMobile("myaccount?tab=wishlist", true);
            Driver.FindElement(By.Id("wishlist")).Click();
            Driver.FindElement(By.CssSelector(".h2.myaccount-subtitle")).Text.Contains("Избранное");
            VerifyAreEqual("wishlist-empty  ",
                Driver.FindElement(By.CssSelector(".wishlist-empty")).GetAttribute("class"));
            Driver.Manage().Window.Maximize();
            Driver.FindElements(By.CssSelector(".device-panel__desktop-direction .device-panel__btn"))[0].Click();
            GoToClient("/products/first-pr");
            Driver.FindElement(By.CssSelector(".wishlist-control.cs-l-2")).Click();

            GoToMobile("myaccount", true);
            Driver.FindElement(By.Id("wishlist")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-container")).Text.Contains("first pr"));
        }

        protected void DelItemFromCart()
        {
            GoToClient("/cart");
            if (Driver.FindElements(By.CssSelector(".cart-full-mobile-remove")).Count > 0)
                Driver.FindElement(By.CssSelector(".cart-full-mobile-remove")).Click();
        }
    }
}