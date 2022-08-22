using System;
using System.Drawing;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store.Mobile
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class ModernMobileTest : MySitesFunctions
    {
        string settingPage = "settings/mobileversion";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Store\\Mobile\\Catalog.Brand.csv",
                "data\\Admin\\Store\\Mobile\\Catalog.Color.csv",
                "data\\Admin\\Store\\Mobile\\Catalog.Size.csv",
                "data\\Admin\\Store\\Mobile\\Catalog.Product.csv",
                "data\\Admin\\Store\\Mobile\\Catalog.ProductList.csv",
                "data\\Admin\\Store\\Mobile\\Catalog.Product_ProductList.csv",
                "data\\Admin\\Store\\Mobile\\Catalog.Photo.csv",
                "data\\Admin\\Store\\Mobile\\Catalog.Offer.csv",
                "data\\Admin\\Store\\Mobile\\Catalog.Category.csv",
                "data\\Admin\\Store\\Mobile\\Catalog.ProductCategories.csv"
            );

            Init();
            //InitializeService.SetCustomLogoAndFavicon();
            //GoToStoreSettings("Карусель");
            //AddCarouselItem("http://bipbap.ru/wp-content/uploads/2017/04/72fqw2qq3kxh.jpg");
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            ReInit();
            GoToAdmin(settingPage);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        [Description("'Цвет панели браузера' и 'Цвет шапки'")]
        public void CheckHeaderColorAndBrowserColor()
        {
            SelectItem("BrowserColorVariantsSelected", "Не задано");
            SelectItem("HeaderColorVariantsSelected", "Белый");
            SaveMobileSettings();

            GoToMobile();
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector("meta[name=\"theme-color\"]")).Count,
                "no meta theme-color");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.ClassName("mobile-header")).GetCssValue("background-color"),
                "colorscheme header color");
            VerifyAreEqual("rgba(56, 56, 56, 1)",
                Driver.FindElement(By.ClassName("mobile-header")).GetCssValue("color"), "colorscheme header color");

            ResizeDesktop();
            GoToAdmin(settingPage);
            SelectItem("BrowserColorVariantsSelected", "Согласно цветовой схеме");
            SelectItem("HeaderColorVariantsSelected", "Согласно цветовой схеме");
            SaveMobileSettings();

            GoToMobile();
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector("meta[name=\"theme-color\"][content=\"#0662c1\"]")).Count,
                "colorscheme meta theme-color");
            VerifyAreEqual("rgba(6, 98, 193, 1)",
                Driver.FindElement(By.ClassName("mobile-header")).GetCssValue("background-color"),
                "colorscheme header color");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.ClassName("mobile-header")).GetCssValue("color"), "colorscheme header color");

            ResizeDesktop();
            GoToStoreSettings("Параметры");
            new SelectElement(Driver.FindElement(By.CssSelector("[ng-model=\"settingsTemplate.CurrentColorScheme\"]")))
                .SelectByText("Бирюзовая");
            SaveTemplateSettings();

            GoToMobile();
            VerifyAreEqual(0,
                Driver.FindElements(By.CssSelector("meta[name=\"theme-color\"][content=\"#0662c1\"]")).Count,
                "old colorscheme meta theme-color");
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector("meta[name=\"theme-color\"][content=\"#7bc8a4\"]")).Count,
                "new colorscheme meta theme-color");
            VerifyAreEqual("rgba(123, 200, 164, 1)",
                Driver.FindElement(By.ClassName("mobile-header")).GetCssValue("background-color"),
                "colorscheme header color");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.ClassName("mobile-header")).GetCssValue("color"), "colorscheme header color");

            ResizeDesktop();
            GoToAdmin(settingPage);
            SelectItem("BrowserColorVariantsSelected", "Свой цвет");
            Driver.ClearInput(By.ClassName("color-picker-input"));
            Driver.FindElement(By.ClassName("color-picker-input")).SendKeys("15c179");
            SaveMobileSettings();

            GoToMobile();
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector("meta[name=\"theme-color\"][content=\"#15c179\"]")).Count,
                "user meta theme-color");
        }

        [Test]
        [Description("'Варианты отображения логотипа', 'Показывать заголовок по умолчанию', " +
            "'Свой заголовок' и загрузка логотипа как фото")]
        public void CheckLogo()
        {
            SelectItem("LogoType", "Текст");
            Driver.CheckBoxCheck("DisplayHeaderTitle");
            SaveMobileSettings();

            GoToMobile();
            VerifyAreEqual("Мой магазин", Driver.FindElement(By.ClassName("mobile-header__logo-link")).Text,
                "logo by text default1");
            GoToMobile("categories/test-category1");
            VerifyAreEqual("Мой магазин", Driver.FindElement(By.ClassName("mobile-header__logo-link")).Text,
                "logo by text default2");

            ResizeDesktop();
            GoToAdmin(settingPage);
            SelectItem("LogoType", "Текст");
            Driver.CheckBoxUncheck("DisplayHeaderTitle");
            Driver.FindElement(By.Id("HeaderCustomTitle")).SendKeys("My header");
            SaveMobileSettings();

            GoToMobile();
            VerifyAreEqual("My header", Driver.FindElement(By.ClassName("mobile-header__logo-link")).Text,
                "logo by text custom1");
            GoToMobile("categories/test-category1");
            VerifyAreEqual("My header", Driver.FindElement(By.ClassName("mobile-header__logo-link")).Text,
                "logo by text custom2");

            ResizeDesktop();
            GoToAdmin(settingPage);
            SelectItem("LogoType", "Логотип десктопной версии");
            SaveMobileSettings();

            GoToMobile();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                    .IndexOf(BaseUrl + "pictures/logo_generated_") != -1 &&
                Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                    .IndexOf(".png") != -1, "logo from desktop1");
            GoToMobile("categories/test-category1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                    .IndexOf(BaseUrl + "pictures/logo_generated_") != -1 &&
                Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                    .IndexOf(".png") != -1, "logo from desktop2");

            ResizeDesktop();
            GoToAdmin(settingPage);
            SelectItem("LogoType", "Загрузить свой логотип");
            AttachFile(By.XPath("(//input[@type='file'])[1]"), GetPicturePath("avatar.jpg"));
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            SaveMobileSettings();

            GoToMobile();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                    .IndexOf(BaseUrl + "pictures/logo_mobile_") != -1 &&
                Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                    .IndexOf(".jpg") != -1, "logo from PC");
            GoToMobile("categories/test-category1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                    .IndexOf(BaseUrl + "pictures/logo_mobile_") != -1 &&
                Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                    .IndexOf(".jpg") != -1, "logo from PC 2");

            ResizeDesktop();
            GoToAdmin(settingPage);
            Driver.GetByE2E("imgDel").Click();
            Driver.SwalConfirm();
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            Driver.GetByE2E("imgByHref").Click();
            Thread.Sleep(500);
            Driver.GetByE2E("imgByHrefLinkText")
                .SendKeys("https://www.advantshop.net/Content/company/images/company-about-img.jpg");
            Driver.GetByE2E("imgByHrefBtnSave").Click();
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            SaveMobileSettings();

            GoToMobile();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                    .IndexOf(BaseUrl + "pictures/logo_mobile_") != -1 &&
                Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                    .IndexOf(".jpg") != -1, "logo by href");
            GoToMobile("categories/test-category1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                  .IndexOf(BaseUrl + "pictures/logo_mobile_") != -1 &&
              Driver.FindElement(By.CssSelector(".mobile-header__logo-link img")).GetAttribute("src")
                  .IndexOf(".jpg") != -1, "logo by href 2");
        }

        [Test]
        [Description("'Выводить город' и 'Выводить карусель'")]
        public void CheckCityAndSlider()
        {
            Driver.CheckBoxCheck("ShowCity");
            Driver.CheckBoxCheck("ShowSlider");
            SaveMobileSettings();

            GoToMobile();
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("carousel-mobile")).Count, "carousel show");

            Driver.FindElement(By.ClassName("mobile-header__menu-trgger-block")).Click();
            Driver.WaitForElem(By.ClassName("mobile-header__menu-triger--opened"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".menu_item--nav-site .menu__item--root"))[0].Text
                    .IndexOf("Ваш город") != -1, "show city");

            ResizeDesktop();
            GoToAdmin(settingPage);
            Driver.CheckBoxUncheck("ShowCity");
            Driver.CheckBoxUncheck("ShowSlider");
            SaveMobileSettings();

            GoToMobile();
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("carousel-mobile")).Count, "carousel hide");

            Driver.FindElement(By.ClassName("mobile-header__menu-trgger-block")).Click();
            Driver.WaitForElem(By.ClassName("mobile-header__menu-triger--opened"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".menu_item--nav-site .menu__item--root"))[0].Text
                    .IndexOf("Ваш город") == -1, "not show city");
        }

        [Test]
        [Description("'Количество товаров на главной' и 'Отображение категорий на главной странице'")]
        public void CheckProductsAndCategoriesAtMainPage()
        {
            SelectItem("ViewCategoriesOnMain", "Не выводить");
            Driver.FindElement(By.Id("MainPageProductCountMobile")).Clear();
            Driver.FindElement(By.Id("MainPageProductCountMobile")).SendKeys("0");
            SaveMobileSettings();

            VerifyAreEqual("0", GetInputValue("MainPageProductCountMobile"), "MainPageProductCountMobile = 0");

            GoToMobile();
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("categories-root-wrap")).Count,
                "view categories on main page - none");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".mainpage-products")).Count, "mainpage blocks");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".mainpage-products__content-item")).Count,
                "0 mainpage items");

            ResizeDesktop();
            GoToAdmin(settingPage);
            SelectItem("ViewCategoriesOnMain", "Без иконок");
            Driver.FindElement(By.Id("MainPageProductCountMobile")).Clear();
            Driver.FindElement(By.Id("MainPageProductCountMobile")).SendKeys("3");
            SaveMobileSettings();

            GoToMobile();
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("categories-root-wrap")).Count,
                "view categories on main page - without icons");
            VerifyAreEqual(4, Driver.FindElements(By.ClassName("categories-root__item")).Count,
                "categories on main without icons - items");
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("categories-root__item-icon")).Count,
                "categories on main without icons - icons");
            VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".mainpage-products")).Count,
                "3 mainpage products block");
            VerifyAreEqual(3,
                Driver.FindElements(By.CssSelector(".mainpage-products--best .mainpage-products__content-item")).Count,
                "3 mainpage best items");
            VerifyAreEqual(3,
                Driver.FindElements(By.CssSelector(".novelty-section .mainpage-products__content-item")).Count,
                "3 mainpage novelty items");
            VerifyAreEqual(3,
                Driver.FindElements(By.CssSelector(".sale-section .mainpage-products__content-item")).Count,
                "3 mainpage sale items");
            VerifyAreEqual(3,
                Driver.FindElements(By.ClassName("prodList-section"))[0]
                    .FindElements(By.CssSelector(".mainpage-products__content-item")).Count,
                "3 mainpage prodList items");

            ResizeDesktop();
            GoToAdmin(settingPage);
            SelectItem("ViewCategoriesOnMain", "С иконками");
            Driver.FindElement(By.Id("MainPageProductCountMobile")).Clear();
            Driver.FindElement(By.Id("MainPageProductCountMobile")).SendKeys("9");
            SaveMobileSettings();

            GoToMobile();
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("categories-root-wrap")).Count,
                "view categories on main page - with icons");
            VerifyAreEqual(4, Driver.FindElements(By.ClassName("categories-root__item")).Count,
                "categories on main with icons - items");
            VerifyAreEqual(4, Driver.FindElements(By.ClassName("categories-root__item-icon")).Count,
                "categories on main with icons - icons");
            VerifyAreEqual(5, Driver.FindElements(By.CssSelector(".mainpage-products")).Count,
                "9 mainpage products block");
            VerifyAreEqual(9,
                Driver.FindElements(By.CssSelector(".mainpage-products--best .mainpage-products__content-item")).Count,
                "9 mainpage best items");
            VerifyAreEqual(9,
                Driver.FindElements(By.CssSelector(".novelty-section .mainpage-products__content-item")).Count,
                "9 mainpage novelty items");
            VerifyAreEqual(9,
                Driver.FindElements(By.CssSelector(".sale-section .mainpage-products__content-item")).Count,
                "9 mainpage sale items");
            VerifyAreEqual(7,
                Driver.FindElements(By.ClassName("prodList-section"))[0]
                    .FindElements(By.CssSelector(".mainpage-products__content-item")).Count,
                "9 mainpage prodList items");
        }

        [Test]
        [Description("'Отображать ссылку все товары категории в меню' и 'Вид отображения категории'")]
        public void CheckCategoriesInMenu()
        {
            SelectItem("CatalogMenuViewMode", "Выводить корневые категории");
            Driver.CheckBoxCheck("ShowMenuLinkAll"); //
            SaveMobileSettings();

            GoToMobile();
            Driver.FindElement(By.ClassName("mobile-header__menu-trgger-block")).Click();
            Driver.WaitForElem(By.ClassName("mobile-header__menu-triger--opened"));
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("menu_item--catalog")).Count, "menu-catalog block");
            VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".menu_item--catalog .menu__item--root")).Count,
                "menu-catalog items");
            Driver.FindElements(By.CssSelector(".menu_item--catalog .menu__item--root"))[3].Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Посмотреть все товары",
                Driver.FindElements(By.CssSelector(".menu__submenu"))[0].FindElements(By.CssSelector(".menu__item"))[1]
                    .Text, "menu link all show1");
            Driver.FindElements(By.CssSelector(".menu__submenu"))[0].FindElements(By.CssSelector(".menu__item"))[2]
                .Click();
            Thread.Sleep(1000);
            VerifyAreEqual(6, Driver.FindElements(By.CssSelector(".menu__submenu .menu__item")).Count,
                "menu items without menu link all non");
            VerifyAreEqual("Посмотреть все товары",
                Driver.FindElements(By.CssSelector(".menu__submenu"))[1].FindElements(By.CssSelector(".menu__item"))[1]
                    .Text, "menu link all show2");
            Driver.FindElements(By.CssSelector(".menu__submenu"))[1].FindElements(By.CssSelector(".menu__item"))[1]
                .Click();
            Thread.Sleep(1000);
            VerifyAreEqual((BaseUrl + "/categories/test-category6").Replace("//", "/"), Driver.Url.Replace("//", "/"),
                "show all products path");

            GoToMobile("categories/test-category1");
            Driver.FindElement(By.ClassName("mobile-header__menu-trgger-block")).Click();
            Driver.WaitForElem(By.ClassName("mobile-header__menu-triger--opened"));
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("menu_item--catalog")).Count, "menu-catalog block");
            VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".menu_item--catalog .menu__item--root")).Count,
                "menu-catalog items");
            Driver.FindElement(By.ClassName("mobile-header__menu-trgger-block")).Click();

            ResizeDesktop();
            GoToAdmin(settingPage);
            Driver.CheckBoxUncheck("ShowMenuLinkAll"); //
            SaveMobileSettings();

            GoToMobile();
            Driver.FindElement(By.ClassName("mobile-header__menu-trgger-block")).Click();
            Driver.WaitForElem(By.ClassName("mobile-header__menu-triger--opened"));
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".menu__submenu .menu__item")).Count,
                "menu items without menu link all non");
            Driver.FindElements(By.CssSelector(".menu_item--catalog .menu__item--root"))[3].Click();
            Thread.Sleep(1000);
            Driver.FindElements(By.CssSelector(".menu__submenu"))[0].FindElements(By.CssSelector(".menu__item"))[1]
                .Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.PageSource.IndexOf("Каталог") != -1, "catalog href disabled");
            VerifyAreEqual(4, Driver.FindElements(By.CssSelector(".menu__submenu .menu__item")).Count,
                "menu items without menu link all non");
            VerifyAreEqual(0, Driver.FindElements(By.LinkText("Посмотреть все товары")).Count, "menu link all none");
            Driver.FindElement(By.ClassName("mobile-header__menu-trgger-block")).Click();

            ResizeDesktop();
            GoToAdmin(settingPage);
            SelectItem("CatalogMenuViewMode", "Отображать только ссылку"); //
            SaveMobileSettings();

            GoToMobile();
            Driver.FindElement(By.ClassName("mobile-header__menu-trgger-block")).Click();
            Driver.WaitForElem(By.ClassName("mobile-header__menu-triger--opened"));

            VerifyAreEqual(0, Driver.FindElements(By.ClassName("menu_item--catalog")).Count, "menu-catalog block none");
            VerifyIsTrue(Driver.PageSource.IndexOf("Каталог") != -1, "catalog href enabled");
            VerifyAreEqual("Каталог", Driver.FindElements(By.ClassName("menu__item-inner--root"))[1].Text,
                "catalog link text");
            Driver.FindElements(By.ClassName("menu__item-inner--root"))[1].Click();
            Thread.Sleep(500);
            VerifyAreEqual(7,
                Driver.FindElements(By.CssSelector(".menu__item--root > .menu__submenu .menu__item")).Count,
                "catalog childs");
            VerifyAreEqual(5,
                Driver.FindElements(By.CssSelector(".menu__item--root > .menu__submenu > .menu__item")).Count,
                "catalog childs first level");
            VerifyAreEqual("Каталог", Driver.FindElements(By.CssSelector(".menu__submenu .menu__item-inner"))[0].Text,
                "catalog back link text");
            VerifyIsTrue(Driver.PageSource.IndexOf("Посмотреть все товары") == -1, "menu link all show4");

            Driver.FindElement(By.CssSelector(".menu__submenu .menu__item-inner")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElements(By.ClassName("menu__item--root"))[0].Displayed == true);
            Driver.FindElements(By.ClassName("menu__item-inner--root"))[1].Click();
            Thread.Sleep(500);

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".menu__submenu"))[0].FindElements(By.CssSelector(".menu__item"))[6]
                    .Displayed, "test category 6 not displayed");
            Driver.FindElements(By.CssSelector(".menu__submenu"))[0].FindElements(By.CssSelector(".menu__item"))[4]
                .Click();
            Thread.Sleep(1000);
            Driver.FindElements(By.CssSelector(".menu__submenu"))[0].FindElements(By.CssSelector(".menu__item"))[6]
                .Click();
            Thread.Sleep(1000);
            VerifyAreEqual(9,
                Driver.FindElements(By.CssSelector(".menu__item--root > .menu__submenu .menu__item")).Count,
                "catalog childs");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".menu__submenu"))[0].FindElements(By.CssSelector(".menu__item"))[7]
                    .Displayed, "test category 6 displayed");
            VerifyAreEqual("TestCategory6",
                Driver.FindElements(By.CssSelector(".menu__submenu"))[0].FindElements(By.CssSelector(".menu__item"))[7]
                    .Text, "test category 6 text");


            GoToMobile("categories/test-category1");
            Driver.FindElement(By.ClassName("mobile-header__menu-trgger-block")).Click();
            Driver.WaitForElem(By.ClassName("mobile-header__menu-triger--opened"));

            VerifyAreEqual(0, Driver.FindElements(By.ClassName("menu_item--catalog")).Count, "menu-catalog block none");
            VerifyAreEqual("Каталог", Driver.FindElements(By.ClassName("menu__item-inner--root"))[1].Text,
                "catalog link text");
        }

        [Test]
        [Description("'Высота блока изображения товара', 'Отображать кнопку В корзину', " +
            "'Число строк в названии товара', 'Тип отображения списка товаров в каталоге', " +
            "'Разрешить покупателям менять тип отображения'")]
        public void CheckProductsInCatalog()
        {
            Driver.FindElement(By.Id("BlockProductPhotoHeight")).Clear();
            Driver.FindElement(By.Id("BlockProductPhotoHeight")).SendKeys("0");
            Driver.CheckBoxCheck("ShowAddButton");
            Driver.FindElement(By.Id("CountLinesProductName")).Clear();
            Driver.FindElement(By.Id("CountLinesProductName")).SendKeys("0");
            SelectItem("DefaultCatalogView", "Плитка");
            Driver.CheckBoxCheck("EnableCatalogViewChange");
            SaveMobileSettings();

            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            Driver.FindElement(By.Id("CountLinesProductName")).Clear();
            Driver.FindElement(By.Id("CountLinesProductName")).SendKeys("1");
            SaveMobileSettings();

            VerifyAreEqual("180", GetInputValue("BlockProductPhotoHeight"), "BlockProductPhotoHeight = 0");
            VerifyAreEqual("1", GetInputValue("CountLinesProductName"), "CountLinesProductName = 1");

            ReInitClient();
            GoToMobile();
            VerifyAreEqual("180px",
                Driver.FindElement(By.ClassName("mobile-product-view-item-image-wrap")).GetCssValue("height"),
                "block product phono height1");
            VerifyAreEqual("18px",
                Driver.FindElements(By.CssSelector(".mainpage-products__content-item .prod-name"))[0]
                    .GetCssValue("height"), "CountLinesProductName 1(1)");
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".mainpage-products__content-item"))[0]
                    .FindElements(By.ClassName("products-view-buy")).Count, "catalog items[0] button enabled");

            GoToMobile("categories/test-category1");
            VerifyAreEqual(18,
                Driver.FindElements(By.CssSelector(".products-view-mobile-modern-tile .catalog-product-item")).Count,
                "DefaultCatalogView tile");
            VerifyAreEqual(18, Driver.FindElements(By.CssSelector(".catalog-product-item .products-view-buy")).Count,
                "catalog button enabled count");
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".catalog-product-item"))[0]
                    .FindElements(By.ClassName("products-view-buy")).Count, "catalog button enabled");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("catalog-change-btn")).Count,
                "catalog change view enabled");
            VerifyAreEqual("180px",
                Driver.FindElement(By.ClassName("mobile-product-view-item-image-wrap")).GetCssValue("height"),
                "block product phono height2");
            VerifyAreEqual("18px",
                Driver.FindElements(By.CssSelector(".catalog-product-item .prod-name"))[0].GetCssValue("height"),
                "CountLinesProductName 1(2)");

            Driver.FindElement(By.ClassName("catalog-change-btn")).Click();
            Thread.Sleep(500);
            VerifyAreNotEqual("180px",
                Driver.FindElement(By.ClassName("mobile-product-view-item-image-wrap")).GetCssValue("height"),
                "block product phono height3");
            VerifyAreEqual(0,
                Driver.FindElements(By.CssSelector(".products-view-mobile-modern-tile .catalog-product-item")).Count,
                "DefaultCatalogView tile changed1");
            VerifyAreEqual(18,
                Driver.FindElements(By.CssSelector(".products-view-mobile-modern-list .catalog-product-item")).Count,
                "DefaultCatalogView tile changed2");

            Driver.FindElement(By.ClassName("catalog-change-btn")).Click();
            Thread.Sleep(500);
            VerifyAreNotEqual("180px",
                Driver.FindElement(By.ClassName("mobile-product-view-item-image-wrap")).GetCssValue("height"),
                "block product phono height4");
            VerifyAreEqual(0,
                Driver.FindElements(By.CssSelector(".products-view-mobile-modern-list .catalog-product-item")).Count,
                "DefaultCatalogView tile changed3");
            VerifyAreEqual(18,
                Driver.FindElements(By.CssSelector(".products-view-mobile-modern-single .catalog-product-item")).Count,
                "DefaultCatalogView tile changed4");

            ReInit();
            GoToAdmin(settingPage);
            Driver.FindElement(By.Id("BlockProductPhotoHeight")).Clear();
            Driver.FindElement(By.Id("BlockProductPhotoHeight")).SendKeys("60");
            Driver.CheckBoxUncheck("ShowAddButton");
            Driver.FindElement(By.Id("CountLinesProductName")).Clear();
            Driver.FindElement(By.Id("CountLinesProductName")).SendKeys("3");
            SelectItem("DefaultCatalogView", "Список");
            SaveMobileSettings();

            ReInitClient();
            GoToMobile();
            VerifyAreEqual("60px",
                Driver.FindElement(By.ClassName("mobile-product-view-item-image-wrap")).GetCssValue("height"),
                "block product phono height5");
            VerifyAreEqual("54px",
                Driver.FindElements(By.CssSelector(".mainpage-products__content-item .prod-name"))[0]
                    .GetCssValue("height"), "CountLinesProductName 3(1)");
            VerifyAreEqual(0,
                Driver.FindElements(By.CssSelector(".mainpage-products__content-item .prod-name"))[0]
                    .FindElements(By.ClassName("products-view-buy")).Count, "catalog items[0] button disabled");

            GoToMobile("categories/test-category1");
            VerifyAreNotEqual("60px",
                Driver.FindElement(By.ClassName("mobile-product-view-item-image-wrap")).GetCssValue("height"),
                "block product phono height6");
            VerifyAreEqual(18,
                Driver.FindElements(By.CssSelector(".products-view-mobile-modern-list .catalog-product-item")).Count,
                "DefaultCatalogView tile changed2");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".catalog-product-item .products-view-buy")).Count,
                "catalog button disabled count");
            Driver.FindElement(By.ClassName("catalog-change-btn")).Click();
            Thread.Sleep(500);
            VerifyAreNotEqual("60px",
                Driver.FindElement(By.ClassName("mobile-product-view-item-image-wrap")).GetCssValue("height"),
                "block product phono height7");
            Driver.FindElement(By.ClassName("catalog-change-btn")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("60px",
                Driver.FindElement(By.ClassName("mobile-product-view-item-image-wrap")).GetCssValue("height"),
                "block product phono height8");
            VerifyAreEqual("54px",
                Driver.FindElements(By.CssSelector(".catalog-product-item .prod-name"))[0].GetCssValue("height"),
                "CountLinesProductName 3(2)");

            ReInit();
            GoToAdmin(settingPage);
            SelectItem("DefaultCatalogView", "Блоки");
            Driver.CheckBoxUncheck("EnableCatalogViewChange");
            SaveMobileSettings();

            ReInitClient();
            GoToMobile("categories/test-category1");
            VerifyAreEqual(18,
                Driver.FindElements(By.CssSelector(".products-view-mobile-modern-single .catalog-product-item")).Count,
                "DefaultCatalogView tile changed4");
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("catalog-change-btn")).Count,
                "catalog change view enabled");
        }

        [Test]
        [Description("Раньше тест проверял настройку мобильной версии." +
            "Сейчас это просто проверка работы переходов при уменьшении/увеличении экрана и" +
            "нажатии кнопок 'Да, перейти'/'Остаться в ... версии' в соответствующих диалоговых окнах")]
        public void AutoRedirectSetting()
        {
            //клиент из админки идет сразу в мобильную клиентку - десктоп; уменьшает экран: подтверждение(подтвердить/отменить). 
            //потом увеличивает экран: подтверждение(подтвердить/отменить). 
            //затем уменьшение/увеличивание экрана - и сразу отмена. (перейти в мобилку, вернуться в десктоп, перейти в десктоп, вернуться в мобилку)
            //каждый раз проверять текст во всплывашке и на кнопках
            
            ResizeMobile();
            GoToClient();
            VerifyIsDesktop("default");
            ResizeDesktop();

            ResizeMobile();
            CheckPanelGoToMobile("1");
            CancelGoto("mobile");
            VerifyIsDesktop("canceled goto from desktop");

            ResizeDesktop();
            ResizeMobile();
            CheckPanelGoToMobile("2");
            ConfirmGoto("mobile");
            VerifyIsMobile("comfirmed goto from desktop");

            ResizeDesktop();
            CheckPanelGoToDesktop("3");
            CancelGoto("desktop");
            VerifyIsMobile("canceled goto from mobile");

            ResizeMobile();
            ResizeDesktop();
            CheckPanelGoToDesktop("4");
            ConfirmGoto("desktop");
            VerifyIsDesktop("confirmed goto from mobile");

            ResizeMobile();
            CheckPanelGoToMobile("5");
            ResizeDesktop();
            VerifyIsDesktop("canceled resize from desktop");

            ResizeMobile();
            ConfirmGoto("mobile");

            Refresh();
            ResizeDesktop();
            CheckPanelGoToDesktop("6");
            ResizeMobile();
            VerifyIsMobile("canceled resize from mobile");
        }

        /// <summary>
        /// Click button to confirm the redirect to desktop or mobile mode
        /// </summary>
        /// <param name="mode">Set "desktop" or "mobile"</param>
        public void ConfirmGoto(string mode)
        {
            Driver.FindElement(By.ClassName("device-panel__" + mode + "-direction"))
                .FindElements(By.ClassName("device-panel__btn"))[0].Click();
        }

        /// <summary>
        /// Click button to cancel the redirect to desktop or mobile mode
        /// </summary>
        /// <param name="mode">Set "desktop" or "mobile"</param>
        public void CancelGoto(string mode)
        {
            Driver.FindElement(By.ClassName("device-panel__" + mode + "-direction"))
                .FindElements(By.ClassName("device-panel__btn"))[1].Click();
        }

        public void ResizeMobile()
        {
            Driver.Manage().Window.Size = new Size(414, 700);
        }

        public void ResizeDesktop()
        {
            Driver.Manage().Window.Maximize();
        }

        public void VerifyIsMobile(string message)
        {
            VerifyIsTrue(Driver.FindElement(By.TagName("html")).GetAttribute("class").IndexOf("mobile-version") != -1,
                "mobile layout " + message);
        }

        public void VerifyIsDesktop(string message)
        {
            VerifyIsTrue(Driver.FindElements(By.ClassName("stretch-container")).Count == 1,
                "desktop layout " + message);
        }

        public void CheckPanelGoToDesktop(string message)
        {
            By devicePanel = By.ClassName("device-panel__desktop-direction");
            VerifyAreEqual("Хотите перейти на полную версию сайта?",
                Driver.FindElement(devicePanel).FindElement(By.ClassName("device-panel__instruction")).Text.Trim(),
                "header goto full (" + message + ")");
            VerifyAreEqual("Да, перейти",
                Driver.FindElement(devicePanel).FindElements(By.ClassName("device-panel__btn"))[0].Text.Trim(),
                "button1 goto full (" + message + ")");
            VerifyAreEqual("Нет, остаться на мобильной версии",
                Driver.FindElement(devicePanel).FindElements(By.ClassName("device-panel__btn"))[1].Text.Trim(),
                "button2 goto full (" + message + ")");
        }

        public void CheckPanelGoToMobile(string message)
        {
            By devicePanel = By.ClassName("device-panel__mobile-direction");
            VerifyAreEqual("Хотите перейти на мобильную версию сайта?",
                Driver.FindElement(devicePanel).FindElement(By.ClassName("device-panel__instruction")).Text.Trim(),
                "header goto mobile (" + message + ")");
            VerifyAreEqual("Да, перейти",
                Driver.FindElement(devicePanel).FindElements(By.ClassName("device-panel__btn"))[0].Text.Trim(),
                "button1 goto mobile (" + message + ")");
            VerifyAreEqual("Остаться на полной версии",
                Driver.FindElement(devicePanel).FindElements(By.ClassName("device-panel__btn"))[1].Text.Trim(),
                "button2 goto mobile (" + message + ")");
        }
    }
}