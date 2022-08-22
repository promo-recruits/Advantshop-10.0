using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store.Mobile
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class MobileVersionTest : MySitesFunctions
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
            InitializeService.SetCustomLogoAndFavicon();

            Init();
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
        [Description("Включить и выключить мобильную версию, проверить разницу в клиентской части")]
        public void Enabled()
        {
            VerifyIsTrue(Driver.FindElement(By.Id("Enabled")).GetAttribute("class").IndexOf("ng-not-empty") != -1,
                "enabled default");

            GoToMobile();
            VerifyIsTrue(Driver.FindElement(By.TagName("html")).GetAttribute("class").IndexOf("mobile-version") != -1,
                "client mobile");
            VerifyAreEqual(1, Driver.FindElements(By.Id("layout")).Count, "mobile layout");

            ReInit();
            GoToAdmin(settingPage);
            Driver.CheckBoxUncheck("Enabled");
            SaveMobileSettings();

            GoToMobile();
            VerifyIsTrue(Driver.FindElement(By.TagName("html")).GetAttribute("class").IndexOf("mobile-version") == -1,
                "client desktop");
            VerifyAreEqual(0, Driver.FindElements(By.Id("layout")).Count, "mobile layout in desktop");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("stretch-container")).Count, "mobile layout");

            ReInit();
            GoToAdmin(settingPage);
            Driver.CheckBoxCheck("Enabled");
            SaveMobileSettings();
        }

        [Test]
        [Description("Проверяет работу движка с включенной Полной формой оформления заказа")]
        public void FullCheckoutOn()
        {
            VerifyIsTrue(
                Driver.FindElement(By.Id("IsFullCheckout")).GetAttribute("class").IndexOf("ng-not-empty") != -1,
                "IsFullCheckout default");

            GoToMobile("products/test-product106");
            Driver.ScrollTo(By.ClassName("mobile-product-main-info"));
            Driver.FindElement(By.CssSelector("a.btn-confirm")).Click();
            GoToMobile("cart");
            Driver.FindElement(By.ClassName("cart-full-mobile-btn")).Click();
            Driver.WaitForElem(By.ClassName("checkout-title"));
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".mobileCheckoutForm")).Count, "checkout header");
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".checkout-title-row")).Count, "checkout header");
            VerifyAreEqual(5, Driver.FindElements(By.CssSelector("article.checkout-block")).Count, "checkout header");
            Driver.ScrollTo(By.Id("rightCell"));
            Driver.FindElement(By.ClassName("checkout__button-summary")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            VerifyIsTrue(
                Driver.FindElement(By.ClassName("checkout-success-content")).Text
                    .IndexOf("Спасибо, ваш заказ оформлен") != -1, "checkout success");

            ReInit();
        }

        [Test]
        [Description("Проверяет работу движка с выключенной Полной формой оформления заказа")]
        public void FullCheckoutOff()
        {
            GoToAdmin(settingPage);
            Driver.CheckBoxUncheck("IsFullCheckout");
            SaveMobileSettings();

            GoToMobile("products/test-product106");
            Driver.ScrollTo(By.ClassName("mobile-product-main-info"));
            Driver.FindElement(By.CssSelector("a.btn-confirm")).Click();
            GoToMobile("cart");
            Driver.FindElement(By.ClassName("cart-full-mobile-btn")).Click();
            Driver.WaitForElem(By.Name("checkoutForm"));
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".checkout-title-row")).Count, "checkout header");
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".mobileCheckoutForm")).Count, "checkout header");
            Driver.FindElement(By.Name("Name")).SendKeys("Name");
            Driver.FindElement(By.Name("Phone")).Click();
            Driver.FindElement(By.Name("Phone")).SendKeys("79001729372");
            Driver.FindElement(By.ClassName("btn-confirm")).Click();
            Driver.WaitForElem(By.ClassName("checkout-confirm-txt"));
            VerifyIsTrue(
                Driver.FindElement(By.ClassName("checkout-confirm-txt")).Text.IndexOf("Спасибо, ваш заказ оформлен") !=
                -1, "checkout success");

            ReInit();
            GoToAdmin(settingPage);
            Driver.CheckBoxCheck("IsFullCheckout");
            SaveMobileSettings();
        }

        [Test]
        [Description("Тест нужен для возможности отследить кол-во настроек " +
            "и базовые классы, которые используются в других тестах." +
            "Если он упадет, все менять.")]
        public void MobileTemplate()
        {
            VerifyAreEqual(17, Driver.FindElements(By.CssSelector(".tab-pane.active .form-group.row")).Count,
                "mobile settings common count");

            GoToMobile();
            VerifyIsTrue(Driver.FindElement(By.TagName("html")).GetAttribute("class").IndexOf("mobile-version") != -1,
                "modern mobile");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("mobile-header")).Count, "modern mobile menu");
            VerifyIsTrue(Driver.FindElement(By.Id("footer")).GetAttribute("class").IndexOf("cs-bg-3") != -1,
                "modern mobile footer");
        }
    }
}