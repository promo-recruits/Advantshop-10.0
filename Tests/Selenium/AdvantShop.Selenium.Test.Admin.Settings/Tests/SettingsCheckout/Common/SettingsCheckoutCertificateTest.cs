using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Common
{
    [TestFixture]
    public class SettingsCheckoutCertificateTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\SettingCheckout\\Catalog.Coupon.csv",
                "data\\Admin\\Settings\\SettingCheckout\\Catalog.CouponCategories.csv",
                "data\\Admin\\Settings\\SettingCheckout\\Catalog.CouponProducts.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderContact.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderStatus.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].[Order].csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].Certificate.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\[Order].OrderSource.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderCurrency.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderItems.csv"
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
        public void EnableCertificate()
        {
            GoToAdmin("settingscheckout");

            Driver.ScrollTo(By.Id("BuyInOneClickDisableInCheckout"));
            if (!Driver.FindElement(By.Id("EnableGiftCertificateService")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnableCertificate\"]")).Click();
            }

            if (!Driver.FindElement(By.Id("DisplayPromoTextbox")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DisplayPromoTextbox\"]")).Click();
            }

            Driver.FindElement(By.Id("MinimalPriceCertificate")).Clear();
            Driver.FindElement(By.Id("MinimalPriceCertificate")).SendKeys("100");

            Driver.FindElement(By.Id("MaximalPriceCertificate")).Clear();
            Driver.FindElement(By.Id("MaximalPriceCertificate")).SendKeys("1000");
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));
            GoToClient("cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".cart-full-coupon")).Displayed, "certificate block cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-gift-button")).Enabled, "certificate btn cart");
            Driver.FindElement(By.CssSelector(".cart-full-coupon input")).SendKeys("Certificate2");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Thread.Sleep(500);

            VerifyAreEqual("Сертификат:", Driver.FindElements(By.CssSelector(".cart-full-summary-name"))[1].Text,
                "client coupon");
            VerifyAreEqual("2 000 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[0].Text,
                "client sum coupon");
            Driver.FindElement(By.CssSelector(".cart-full-summary-price a")).Click();
            Thread.Sleep(100);

            GoToClient("checkout");
            Driver.ScrollTo(By.Name("cardsFormBlock"));
            VerifyIsTrue(Driver.FindElement(By.Name("cardsFormBlock")).Displayed, "certificate block checkout");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-gift-button")).Enabled,
                "certificate btn checkout");

            Driver.FindElement(By.CssSelector(".checkout-block-content .col-sm-8 input")).SendKeys("Certificate2");
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-action.btn-expander")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-result")).Text.Contains("0 руб."),
                "checkout afred cert rezult");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(100);
        }

        [Test]
        public void NotEnableCertificate()
        {
            GoToAdmin("settingscheckout");

            Driver.ScrollTo(By.Id("BuyInOneClickDisableInCheckout"));
            if (Driver.FindElement(By.Id("EnableGiftCertificateService")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnableCertificate\"]")).Click();
            }

            if (Driver.FindElement(By.Id("DisplayPromoTextbox")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DisplayPromoTextbox\"]")).Click();
            }

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));
            GoToClient("cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cart-full-coupon")).Count == 0, "certificate block cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".checkout-gift-button")).Count == 0,
                "certificate btn cart");
            GoToClient("checkout");
            VerifyIsTrue(Driver.FindElements(By.Name("cardsFormBlock")).Count == 0, "certificate block checkout");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".checkout-gift-button")).Count == 0,
                "certificate btn checkout");
        }

        [Test]
        public void NotVisibleCertificate()
        {
            GoToAdmin("settingscheckout");

            Driver.ScrollTo(By.Id("BuyInOneClickDisableInCheckout"));
            if (!Driver.FindElement(By.Id("EnableGiftCertificateService")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnableCertificate\"]")).Click();
            }

            if (Driver.FindElement(By.Id("DisplayPromoTextbox")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DisplayPromoTextbox\"]")).Click();
            }

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));
            GoToClient("cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cart-full-coupon")).Count == 0, "certificate block cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".checkout-gift-button")).Count == 0,
                "certificate btn cart");
            GoToClient("checkout");
            VerifyIsTrue(Driver.FindElements(By.Name("cardsFormBlock")).Count == 0, "certificate block checkout");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".checkout-gift-button")).Count == 0,
                "certificate btn checkout");

            GoToAdmin("settingscheckout");

            Driver.ScrollTo(By.Id("BuyInOneClickDisableInCheckout"));
            if (Driver.FindElement(By.Id("EnableGiftCertificateService")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnableCertificate\"]")).Click();
            }

            if (!Driver.FindElement(By.Id("DisplayPromoTextbox")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DisplayPromoTextbox\"]")).Click();
            }

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".cart-full-coupon")).Displayed, "certificate block cart 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-gift-button")).Enabled, "certificate btn cart 1");

            Driver.FindElement(By.CssSelector(".cart-full-coupon input")).SendKeys("Certificate3");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.Id("toast-container")).Displayed, "certificate disable block cart");

            GoToClient("checkout");
            Driver.WaitForElem(By.Name("cardsFormBlock"));
            Driver.ScrollTo(By.Name("cardsFormBlock"));
            VerifyIsTrue(Driver.FindElement(By.Name("cardsFormBlock")).Displayed, "certificate block checkout 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-gift-button")).Enabled,
                "certificate btn checkout 1");

            Driver.FindElement(By.CssSelector(".checkout-block-content .col-sm-8 input")).SendKeys("Certificate3");
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-action.btn-expander")).Click();
            Thread.Sleep(100);

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".checkout-cart-content")).Text.Contains("Сертификат"),
                "certificate disable block checkout");
            VerifyAreNotEqual("0 руб.", Driver.FindElement(By.CssSelector(".checkout-result span")).Text,
                "checkout afred cert rezult");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(100);
        }

        [Test]
        public void PriceCertificate()
        {
            /*
                        GoToAdmin("design");
                        driver.FindElement(By.CssSelector(".btn.btn-sm.btn-action.other-btn")).Click();
                        Thread.Sleep(1000);

                          Driver.ScrollTo(By.CssSelector("[data-e2e=\"Вы уже смотрели\"]"));
                        driver.FindElement(By.CssSelector("[data-e2e=\"Блок подарочных сертификатов\"] span")).Click();
                        Thread.Sleep(2000);
                        if (!driver.FindElement(By.CssSelector("[data-e2e=\"Блок подарочных сертификатов\"] input")).Selected)
                        {
                            driver.FindElement(By.CssSelector("[data-e2e=\"Блок подарочных сертификатов\"] span")).Click();
                        }
                        driver.FindElement(By.CssSelector(".modal-footer button")).Click();
                        Thread.Sleep(1000);
                        */
            GoToAdmin("settingscheckout");

            Driver.ScrollTo(By.Id("BuyInOneClickDisableInCheckout"));

            if (!Driver.FindElement(By.Id("EnableGiftCertificateService")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnableCertificate\"]")).Click();
            }

            Driver.FindElement(By.Id("MinimalPriceCertificate")).Clear();
            Driver.FindElement(By.Id("MinimalPriceCertificate")).SendKeys("100");

            Driver.FindElement(By.Id("MaximalPriceCertificate")).Clear();
            Driver.FindElement(By.Id("MaximalPriceCertificate")).SendKeys("1000");
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("giftcertificate");
            Driver.WaitForElem(By.Id("NameTo"));
            Driver.FindElement(By.Id("NameTo")).SendKeys("TestTo");
            Driver.FindElement(By.Id("NameFrom")).SendKeys("TestFrom");
            Driver.FindElement(By.Id("Sum")).Clear();
            Driver.FindElement(By.Id("Sum")).SendKeys("100000");
            Driver.FindElement(By.Id("Message")).SendKeys("test");
            Driver.FindElement(By.Id("EmailTo")).SendKeys("test@test.test");
            Driver.FindElement(By.Id("EmailFrom")).Clear();
            Driver.FindElement(By.Id("EmailFrom")).SendKeys("test1@test.test");
            Thread.Sleep(100);
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.Id("toast-container")).Displayed, "max sum");
            Driver.FindElement(By.Id("Sum")).Clear();
            Driver.FindElement(By.Id("Sum")).SendKeys("10");

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.Id("toast-container")).Displayed, "min sum");
            Driver.FindElement(By.Id("Sum")).Clear();
            Driver.FindElement(By.Id("Sum")).SendKeys("1000");

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "sucsess");

            GoToAdmin("orders");

            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");
            VerifyAreEqual("1 000 руб.", Driver.GetGridCell(0, "SumFormatted").Text, " Grid orders sum");
            Driver.GetGridCell(0, "StatusName").Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Сертификат", Driver.GetGridCell(0, "CustomName", "OrderCertificates").Text,
                " Name product at order");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Sum", "OrderCertificates").Text, " Sum at order");
        }
    }
}