using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.CheckoutFields
{
    [TestFixture]
    public class SettingsCheckoutOneClickFields : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductCategories.csv"
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
        public void OneClickVisibleName()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickComment", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormName")).Displayed, "name field");
            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormEmail")).Count == 0, "lastname field");
            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormPhone")).Count == 0, "patronymic field");
            VerifyIsTrue(Driver.FindElements(By.Name("buyOneClickFormComment")).Count == 0, "phone field");
        }

        [Test]
        public void OneClickVisibleMail()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickComment", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormName")).Count == 0, "name field");
            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormEmail")).Displayed, "lastname field");
            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormPhone")).Count == 0, "patronymic field");
            VerifyIsTrue(Driver.FindElements(By.Name("buyOneClickFormComment")).Count == 0, "phone field");
        }

        [Test]
        public void OneClickVisiblePhone()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickComment", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormName")).Count == 0, "name field");
            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormEmail")).Count == 0, "lastname field");
            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormPhone")).Displayed, "patronymic field");
            VerifyIsTrue(Driver.FindElements(By.Name("buyOneClickFormComment")).Count == 0, "phone field");
        }

        [Test]
        public void OneClickVisibleComment()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickComment", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormName")).Count == 0, "name field");
            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormEmail")).Count == 0, "lastname field");
            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormPhone")).Count == 0, "patronymic field");
            VerifyIsTrue(Driver.FindElement(By.Name("buyOneClickFormComment")).Displayed, "phone field");
        }

        [Test]
        public void OneClickVisibleAll()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickComment", Driver);

            Driver.FindElement(By.Id("BuyInOneClickName")).Clear();
            Driver.FindElement(By.Id("BuyInOneClickName")).SendKeys("One click name");
            Driver.FindElement(By.Id("BuyInOneClickEmail")).Clear();
            Driver.FindElement(By.Id("BuyInOneClickEmail")).SendKeys("One click mail");
            Driver.FindElement(By.Id("BuyInOneClickPhone")).Clear();
            Driver.FindElement(By.Id("BuyInOneClickPhone")).SendKeys("One click phone");
            Driver.FindElement(By.Id("BuyInOneClickComment")).Clear();
            Driver.FindElement(By.Id("BuyInOneClickComment")).SendKeys("One click comment");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormName")).Displayed, "name field");
            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormEmail")).Displayed, "mail field");
            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormPhone")).Displayed, "phone field");
            VerifyIsTrue(Driver.FindElement(By.Name("buyOneClickFormComment")).Displayed, "comment field");


            VerifyIsTrue(Driver.FindElement(By.Name("buyoneclickForm")).Text.Contains("One click name"), "name label");
            VerifyIsTrue(Driver.FindElement(By.Name("buyoneclickForm")).Text.Contains("One click mail"), "mail label");
            VerifyIsTrue(Driver.FindElement(By.Name("buyoneclickForm")).Text.Contains("One click phone"),
                "phone label");
            VerifyIsTrue(Driver.FindElement(By.Name("buyoneclickForm")).Text.Contains("One click comment"),
                "comment label");
        }

        [Test]
        public void OneClickVisibleNoOne()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckNotSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckNotSelected("IsShowBuyInOneClickComment", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormName")).Count == 0, "name field");
            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormEmail")).Count == 0, "lastname field");
            VerifyIsTrue(Driver.FindElements(By.Id("buyOneClickFormPhone")).Count == 0, "patronymic field");
            VerifyIsTrue(Driver.FindElements(By.Name("buyOneClickFormComment")).Count == 0, "phone field");

            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void OneClickRequireName()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickComment", Driver);


            Functions.CheckSelected("IsRequiredBuyInOneClickName", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickEmail", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickPhone", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickComment", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormName")).Displayed, "name field");
            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormEmail")).Displayed, "lastname field");
            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormPhone")).Displayed, "patronymic field");
            VerifyIsTrue(Driver.FindElement(By.Name("buyOneClickFormComment")).Displayed, "phone field");

            Driver.FindElement(By.Id("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormEmail")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormPhone")).Clear();
            Driver.FindElement(By.Name("buyOneClickFormComment")).Clear();

            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("/products/test-product5"), "error checkout 1");

            Driver.FindElement(By.Id("buyOneClickFormEmail")).SendKeys("Name@name.as");
            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("/products/test-product5"), "error checkout 2");

            Driver.FindElement(By.Id("buyOneClickFormEmail")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormName")).SendKeys("TestName");

            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void OneClickRequireMail()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickComment", Driver);


            Functions.CheckNotSelected("IsRequiredBuyInOneClickName", Driver);
            Functions.CheckSelected("IsRequiredBuyInOneClickEmail", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickPhone", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickComment", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            Driver.FindElement(By.Id("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormEmail")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormPhone")).Clear();
            Driver.FindElement(By.Name("buyOneClickFormComment")).Clear();

            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("/products/test-product5"), "error checkout 1");

            Driver.FindElement(By.Id("buyOneClickFormName")).SendKeys("Name");
            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("/products/test-product5"), "error checkout 2");

            Driver.FindElement(By.Id("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormEmail")).SendKeys("Test@mail.as");

            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void OneClickRequirePhone()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickComment", Driver);

            Functions.CheckNotSelected("IsRequiredBuyInOneClickName", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickEmail", Driver);
            Functions.CheckSelected("IsRequiredBuyInOneClickPhone", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickComment", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            Driver.FindElement(By.Id("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormEmail")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormPhone")).Clear();
            Driver.FindElement(By.Name("buyOneClickFormComment")).Clear();

            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("/products/test-product5"), "error checkout 1");

            Driver.FindElement(By.Name("buyOneClickFormComment")).SendKeys("Test Comment");
            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("/products/test-product5"), "error checkout 2");

            Driver.FindElement(By.Name("buyOneClickFormComment")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormPhone")).Click();
            Driver.FindElement(By.Id("buyOneClickFormPhone")).SendKeys("9012345678");

            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void OneClickRequireComment()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickComment", Driver);

            Functions.CheckNotSelected("IsRequiredBuyInOneClickName", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickEmail", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickPhone", Driver);
            Functions.CheckSelected("IsRequiredBuyInOneClickComment", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            Driver.FindElement(By.Id("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormEmail")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormPhone")).Clear();
            Driver.FindElement(By.Name("buyOneClickFormComment")).Clear();

            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("/products/test-product5"), "error checkout 1");

            Driver.FindElement(By.Id("buyOneClickFormPhone")).SendKeys("9999999999");
            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.EndsWith("/products/test-product5"), "error checkout 2");

            Driver.FindElement(By.Id("buyOneClickFormPhone")).Clear();
            Driver.FindElement(By.Name("buyOneClickFormComment")).SendKeys("TestComment");

            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void OneClickRequireAll()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Driver.ScrollTo(By.TagName("footer"));
            Functions.CheckSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickComment", Driver);

            Functions.CheckSelected("IsRequiredBuyInOneClickName", Driver);
            Functions.CheckSelected("IsRequiredBuyInOneClickEmail", Driver);
            Functions.CheckSelected("IsRequiredBuyInOneClickPhone", Driver);
            Functions.CheckSelected("IsRequiredBuyInOneClickComment", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormName")).Displayed, "name field");
            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormEmail")).Displayed, "lastname field");
            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormPhone")).Displayed, "patronymic field");
            VerifyIsTrue(Driver.FindElement(By.Name("buyOneClickFormComment")).Displayed, "phone field");
        }

        [Test]
        public void OneClickRequireNoOne()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            Functions.CheckSelected("IsShowBuyInOneClickName", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickEmail", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickPhone", Driver);
            Functions.CheckSelected("IsShowBuyInOneClickComment", Driver);

            Functions.CheckNotSelected("IsRequiredBuyInOneClickName", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickEmail", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickPhone", Driver);
            Functions.CheckNotSelected("IsRequiredBuyInOneClickComment", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();

            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormName")).Displayed, "name field");
            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormEmail")).Displayed, "lastname field");
            VerifyIsTrue(Driver.FindElement(By.Id("buyOneClickFormPhone")).Displayed, "patronymic field");
            VerifyIsTrue(Driver.FindElement(By.Name("buyOneClickFormComment")).Displayed, "phone field");

            Driver.FindElement(By.Id("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormEmail")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormPhone")).Clear();
            Driver.FindElement(By.Name("buyOneClickFormComment")).Clear();

            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }
    }
}