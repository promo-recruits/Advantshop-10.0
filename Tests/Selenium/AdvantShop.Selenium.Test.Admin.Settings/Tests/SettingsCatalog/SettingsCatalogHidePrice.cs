using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog
{
    [TestFixture]
    public class SettingsCatalogHidePrice : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsCatalog\\HidePrice\\Catalog.Product.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\HidePrice\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\HidePrice\\Catalog.Category.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\HidePrice\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\HidePrice\\Catalog.ProductList.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\HidePrice\\Catalog.Product_ProductList.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\HidePrice\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\HidePrice\\Customers.Customer.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\HidePrice\\Customers.Departments.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\HidePrice\\Customers.Managers.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\HidePrice\\Customers.ManagerTask.csv"
            );

            Init();
            ReindexSearch();
            Functions.EnableCapcha(Driver, BaseUrl);

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void HidePricaSetting()
        {
            GoToAdmin("settingscatalog#?catalogTab=prices");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".tab-pane.active")).Text.Contains("Видимость цен для пользователей"),
                "label type customers");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".tab-pane.active")).Text
                    .Contains("Отображение цены для групп пользователей"), "group customers");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".tab-pane.active")).Text
                    .Contains("Текст, отображаемый при скрытой цене"), "text for price");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PricesList\"] select")).Displayed,
                "display select");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"] input")).Displayed,
                "display ui-select");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"DisplayText\"]")).Displayed,
                "display DisplayText");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"] input")).Enabled,
                "Enabled ui-select");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"DisplayText\"] textarea")).GetAttribute("readonly") ==
                "true", "Enabled DisplayText");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"PricesList\"] select"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Все пользователи"), "checked select");
        }

        [Test]
        public void HidePriceForAll()
        {
            GoToAdmin("settingscatalog#?catalogTab=prices");
            (new SelectElement(Driver.FindElement(By.Id("DisplayModeOfPrices")))).SelectByText("Все пользователи");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("100 руб.",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value.cs-t-1")).Text,
                "price in search block");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in search catalog");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in cart product");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category");

            GoToMobile();

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in search catalog");

            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsTrue(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in cart product");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category mobile");

            ReInitClient();
            GoToClient();
            Refresh();
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in catalog ReInitClient");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block ReInitClient");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog ReInitClient");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search ReInitClient");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product ReInitClient");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in category ReInitClient");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category ReInitClient");

            GoToMobile();

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in catalog ReInitClient");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block ReInitClient");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog ReInitClient");


            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsTrue(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search ReInitClient 1");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product ReInitClient");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in category ReInitClient");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category mobile ReInitClient");

            ReInitClient();
            GoToClient("login");
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test1@advantshop1.net9");
            Driver.FindElement(By.Id("password")).Clear();
            Driver.FindElement(By.Id("password")).SendKeys("123123");
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(2000);

            GoToClient();
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog login");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("100 руб.",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value.cs-t-1")).Text,
                "price in search block login");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog login");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search login");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product login");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category login");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category login");

            GoToMobile();

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog login");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block login");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog login");

            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsTrue(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product login");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category login");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category mobile login");

            ReInitClient();
            GoToClient("registration");
            Refresh();
            Driver.FindElement(By.Id("FirstName")).SendKeys("FirstName");
            Driver.FindElement(By.Id("LastName")).SendKeys("LastName");
            Driver.FindElement(By.Id("Email")).SendKeys("email@email.ru");
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89991234567");
            Driver.FindElement(By.Id("Password")).SendKeys("123123");
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("123123");
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.Url.Contains("myaccount"), "url reg");
            GoToClient();
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog reg");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("100 руб.",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value.cs-t-1")).Text,
                "price in search block reg");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog reg");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search reg");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in cart product reg");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category login");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category reg");

            GoToMobile();

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog reg");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block reg");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog reg");


            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsTrue(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search reg");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in cart product reg");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category reg");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category mobile reg");
        }

        [Test]
        public void HidePriceForCustomer()
        {
            GoToAdmin("settingscatalog#?catalogTab=prices");
            (new SelectElement(Driver.FindElement(By.Id("DisplayModeOfPrices")))).SelectByText(
                "Только зарегистрированные");
            Driver.FindElement(By.Id("TextInsteadOfPrice")).Clear();
            Driver.FindElement(By.Id("TextInsteadOfPrice")).SendKeys("please login");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("100 руб.",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value.cs-t-1")).Text,
                "price in search block");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in search catalog");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in cart product");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category");

            GoToMobile();

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in search catalog");


            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsTrue(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in cart product");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category mobile");

            ReInitClient();
            GoToClient();
            Refresh();
            VerifyAreEqual("please login", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in catalog ReInitClient");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("please login",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block ReInitClient");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("please login", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in search catalog ReInitClient");
            VerifyIsFalse(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search ReInitClient");

            GoToClient("products/test-product2");
            VerifyAreEqual("please login", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product ReInitClient");

            GoToClient("categories/test-category1");
            VerifyAreEqual("please login", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in category ReInitClient");
            VerifyIsTrue(Driver.FindElements(By.Name("catalogFilterForm")).Count == 0,
                "filter in category ReInitClient");

            GoToMobile();

            VerifyAreEqual("please login", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in catalog ReInitClient");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("please login",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block ReInitClient");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("please login", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in search catalog ReInitClient");


            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsFalse(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search ReInitClient 1");

            GoToClient("products/test-product2");
            VerifyAreEqual("please login", Driver.FindElement(By.CssSelector(".instead__text-price")).Text,
                "price in cart product ReInitClient");
            VerifyIsFalse(Driver.PageSource.Contains("100 руб."), "no price on page");

            GoToClient("categories/test-category1");
            VerifyAreEqual("please login", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in category ReInitClient");
            VerifyIsFalse(Driver.PageSource.Contains("100 руб."), "no price on category page");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElements(By.Name("catalogFilterForm")).Count == 0,
                "filter in category mobile ReInitClient");

            ReInitClient();
            GoToClient("login");
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test1@advantshop1.net9");
            Driver.FindElement(By.Id("password")).Clear();
            Driver.FindElement(By.Id("password")).SendKeys("123123");
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(2000);
            GoToClient();
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog login");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("100 руб.",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value.cs-t-1")).Text,
                "price in search block login");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog login");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search login");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product login");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category login");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category login");

            GoToMobile();

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog login");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block login");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog login");


            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsTrue(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product login");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category login");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category mobile login");

            ReInitClient();
            GoToClient("registration");
            Refresh();
            Driver.FindElement(By.Id("FirstName")).SendKeys("FirstName");
            Driver.FindElement(By.Id("LastName")).SendKeys("LastName");
            Driver.FindElement(By.Id("Email")).SendKeys("email@email.ru");
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89991234567");
            Driver.FindElement(By.Id("Password")).SendKeys("123123");
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("123123");
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.Contains("myaccount"), "url reg");

            GoToClient();
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog reg");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("100 руб.",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value.cs-t-1")).Text,
                "price in search block reg");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog reg");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search reg");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in cart product reg");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category login");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category reg");

            GoToMobile();

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in catalog reg");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block reg");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog reg");


            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsTrue(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search reg");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in cart product reg");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text, "price in category reg");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category mobile reg");
        }

        [Test]
        public void HidePriceForGroupCustomer()
        {
            GoToAdmin("settingscatalog#?catalogTab=prices");
            (new SelectElement(Driver.FindElement(By.Id("DisplayModeOfPrices")))).SelectByText(
                "Только пользователям из перечисленных групп");
            Driver.FindElement(By.Id("TextInsteadOfPrice")).Clear();
            Driver.FindElement(By.Id("TextInsteadOfPrice")).SendKeys("pleaseLogin");

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
            }
            catch
            {
            }

            GoToAdmin("settingscatalog#?catalogTab=prices");
            Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]"))
                .FindElement(
                    By.CssSelector(".ui-select-choices.ui-select-choices-content.ui-select-dropdown.dropdown-menu"))
                .FindElement(By.XPath("//span[contains(text(), 'Обычный покупатель')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]"))
                .FindElement(
                    By.CssSelector(".ui-select-choices.ui-select-choices-content.ui-select-dropdown.dropdown-menu"))
                .FindElement(By.XPath("//span[contains(text(), 'Group3')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]"))
                .FindElement(
                    By.CssSelector(".ui-select-choices.ui-select-choices-content.ui-select-dropdown.dropdown-menu"))
                .FindElement(By.XPath("//span[contains(text(), 'Group4')]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductSettingTitle\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]")).Text.Contains("Group4"),
                "group 1 ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]")).Text.Contains("Group3"),
                "group 2 ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]")).Text.Contains("Обычный покупатель"),
                "group 3 ");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProductSettingTitle\"]")).Click();

            Driver.FindElement(By.CssSelector(".close.ui-select-match-close")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]")).Text.Contains("Group4"),
                "group 1 del");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]")).Text.Contains("Group3"),
                "group 2 del");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]")).Text.Contains("Обычный покупатель"),
                "group 3 del");

            GoToAdmin("settingscatalog#?catalogTab=prices");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]")).Text.Contains("Group4"),
                "group 1 del refresh");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]")).Text.Contains("Group3"),
                "group 2 del refresh");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCust\"]")).Text.Contains("Обычный покупатель"),
                "group 3 del refresh");
            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in catalog");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("pleaseLogin",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value.cs-t-1")).Text,
                "price in search block");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in search catalog");
            VerifyIsFalse(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search");

            GoToClient("products/test-product2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price")).Text, "price in cart product");

            GoToClient("categories/test-category1");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in category");
            VerifyIsTrue(Driver.FindElements(By.Name("catalogFilterForm")).Count == 0, "filter in category ");

            GoToMobile();

            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in catalog mobile");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block mobile");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in search catalog mobile");


            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsFalse(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search");

            GoToClient("products/test-product2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".instead__text-price")).Text,
                "price in cart product mobile");

            GoToClient("categories/test-category1");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in category mobile");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElements(By.Name("catalogFilterForm")).Count == 0, "filter in category mobile");

            ReInitClient();
            GoToClient();
            Refresh();
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in catalog ReInitClient");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block ReInitClient");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in search catalog ReInitClient");
            VerifyIsFalse(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search ReInitClient");

            GoToClient("products/test-product2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product ReInitClient");

            GoToClient("categories/test-category1");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in category ReInitClient");
            VerifyIsTrue(Driver.FindElements(By.Name("catalogFilterForm")).Count == 0,
                "filter in category ReInitClient");

            GoToMobile();

            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in catalog ReInitClient mobile");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block ReInitClient mobile");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in search catalog ReInitClient mobile");


            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsFalse(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search ReInitClient 1");

            GoToClient("products/test-product2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".instead__text-price")).Text,
                "price in cart product ReInitClient 1 mobile");
            VerifyIsFalse(Driver.PageSource.Contains("100 руб."), "no price on page");
            GoToClient("categories/test-category1");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in category ReInitClient 1 mobile");
            VerifyIsFalse(Driver.PageSource.Contains("100 руб."), "no price on category page mobile");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElements(By.Name("catalogFilterForm")).Count == 0,
                "filter in category mobile ReInitClient mobile");

            ReInitClient();
            GoToClient("login");
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test1@advantshop1.net9");
            Driver.FindElement(By.Id("password")).Clear();
            Driver.FindElement(By.Id("password")).SendKeys("123123");
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(2000);
            GoToClient();
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in catalog login");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("pleaseLogin",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value.cs-t-1")).Text,
                "price in search block login");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in search catalog login");
            VerifyIsFalse(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search login");

            GoToClient("products/test-product2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product login");

            GoToClient("categories/test-category1");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in category login");
            VerifyIsTrue(Driver.FindElements(By.Name("catalogFilterForm")).Count == 0, "filter in category login");

            GoToMobile();

            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in catalog login");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block login");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in search catalog login");


            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsFalse(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search");

            GoToClient("products/test-product2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".instead__text-price")).Text,
                "price in cart product 1 login");

            GoToClient("categories/test-category1");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in category 1 login");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElements(By.Name("catalogFilterForm")).Count == 0, "filter in category 1 login");

            ReInitClient();
            GoToClient("registration");
            Refresh();
            Driver.FindElement(By.Id("FirstName")).SendKeys("FirstName");
            Driver.FindElement(By.Id("LastName")).SendKeys("LastName");
            Driver.FindElement(By.Id("Email")).SendKeys("email@email.ru");
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89991234567");
            Driver.FindElement(By.Id("Password")).SendKeys("123123");
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("123123");
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.Contains("myaccount"), "url reg");

            GoToClient();
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in catalog reg");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("pleaseLogin",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value.cs-t-1")).Text,
                "price in search block reg");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in search catalog reg");
            VerifyIsFalse(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search reg");

            GoToClient("products/test-product2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product reg");

            GoToClient("categories/test-category1");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".price-current")).Text,
                "price in category reg");
            VerifyIsTrue(Driver.FindElements(By.Name("catalogFilterForm")).Count == 0, "filter in category reg");

            GoToMobile();

            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in catalog reg mobile");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block reg mobile");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in search catalog reg mobile");


            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsFalse(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search reg mobile");

            GoToClient("products/test-product2");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".instead__text-price")).Text,
                "price in cart product reg mobile 1");

            GoToClient("categories/test-category1");
            VerifyAreEqual("pleaseLogin", Driver.FindElement(By.CssSelector(".text-instead-of-price")).Text,
                "price in category reg mobile 1");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElements(By.Name("catalogFilterForm")).Count == 0,
                "filter in category mobile reg mobile 1");

            ReInitClient();
            GoToClient("login");
            Refresh();
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test1@advantshop1.net4");
            Driver.FindElement(By.Id("password")).Clear();
            Driver.FindElement(By.Id("password")).SendKeys("123123");
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(2000);
            GoToClient();
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in catalog login group");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("100 руб.",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value.cs-t-1")).Text,
                "price in search block login group");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog login group");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search login group");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product login group");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in category login group");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category group");

            GoToMobile();
            Refresh();
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in catalog login group 1");

            Driver.FindElement(By.CssSelector(".mobile-header__search-block")).Click();
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("testProduct");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-input")).SendKeys("2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".autocompleter-product-price-value")).Text,
                "price in search block login group");
            Driver.FindElement(By.CssSelector(".mobile-header__search-form-item .mobile-header__search-btn")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog login group");


            //driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            //Thread.Sleep(2000);
            //VerifyIsTrue(driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"), "filter in search group");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product login group");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in category login group");

            Driver.FindElement(By.CssSelector(".catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category mobile login group");

            ReInitClient();
            GoToClient("login");
            Refresh();
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test1@advantshop1.net3");
            Driver.FindElement(By.Id("password")).Clear();
            Driver.FindElement(By.Id("password")).SendKeys("123123");
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(2000);
            GoToClient();
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in catalog login group 2");

            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("testProduct");
            Driver.FindElement(By.Name("searchHeaderForm")).FindElement(By.TagName("input")).SendKeys("2");
            VerifyAreEqual("100 руб.",
                Driver.FindElement(By.CssSelector(".autocompleter-product-price-value.cs-t-1")).Text,
                "price in search block login group2");
            Driver.FindElement(By.CssSelector(".site-head-search-btn-text")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in search catalog login group2");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in search login group2");

            GoToClient("products/test-product2");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in cart product login group2");

            GoToClient("categories/test-category1");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".price")).Text,
                "price in category login group2");
            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена ( руб.)"),
                "filter in category group2");
        }
    }
}