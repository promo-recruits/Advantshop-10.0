using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.MethodShippingPayment
{
    [TestFixture]
    public class MethodTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Shipping | ClearType.Payment | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Shipping\\[Order].PaymentMethod.csv",
                "data\\Admin\\Settings\\Shipping\\[Order].ShippingMethod.csv",
                "data\\Admin\\Settings\\Shipping\\Catalog.Category.csv",
                "data\\Admin\\Settings\\Shipping\\Catalog.Product.csv",
                "data\\Admin\\Settings\\Shipping\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\Shipping\\Catalog.ProductCategories.csv"
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
        public void SettingsMethod()
        {
            GoToAdmin("settings/shippingmethods");
            VerifyAreEqual("FixedRateMailMen Фиксированная стоимость доставки", Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingName\"]")).Text, " Shiping Name Method in Setting");
            VerifyAreEqual("true", Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnabled\"] input")).GetAttribute("value"), " Shiping Enabled Method in Setting");
            VerifyAreEqual("FreeShipping Бесплатная доставка", Driver.FindElements(By.CssSelector("[data-e2e=\"ShippingName\"]"))[1].Text, " Shiping Name Method in Setting");
            VerifyAreEqual("false", Driver.FindElements(By.CssSelector("[data-e2e=\"ShippingEnabled\"] input"))[1].GetAttribute("value"), " Shiping Enabled Method in Setting");
            GoToAdmin("settings/paymentmethods");
            VerifyAreEqual("Cash Наличные", Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentName\"]")).Text, " payment Name Method in Setting");
            VerifyAreEqual("true", Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentEnabled\"] input")).GetAttribute("value"), " payment Enabled Method in Setting");
            VerifyAreEqual("WalletOne Единая Касса (Wallet One)", Driver.FindElements(By.CssSelector("[data-e2e=\"PaymentName\"]"))[1].Text, " payment Name Method in Setting");
            VerifyAreEqual("false", Driver.FindElements(By.CssSelector("[data-e2e=\"PaymentEnabled\"] input"))[1].GetAttribute("value"), " payment Enabled Method in Setting");

            ProductToCard();

            //отображение методов в корзине
            GoToClient("checkout");
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".checkout-block"))[1].FindElements(By.CssSelector(".custom-input-radio")).Count, " Count shipping Method in cart");
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".checkout-block"))[3].FindElements(By.CssSelector(".payment-item-radio")).Count, " Count payment Method in cart");

            VerifyAreEqual("FixedRateMailMen", Driver.FindElement(By.CssSelector(".shipping-item-title")).Text, " Name shipping Method in cart");
            VerifyAreEqual("Cash", Driver.FindElement(By.CssSelector(".payment-item-title")).Text, " Name payment Method in cart");
        }

        [Test]
        public void SettingsMethodEditInplace()
        {
            GoToAdmin("settings/shippingmethods");
            VerifyAreEqual("FixedRateMailMen Фиксированная стоимость доставки", Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingName\"]")).Text, " Shiping Name 1 Method in Setting");
            VerifyAreEqual("true", Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnabled\"] input")).GetAttribute("value"), " Shiping Enabled 1 Method in Setting");
            VerifyAreEqual("FreeShipping Бесплатная доставка", Driver.FindElements(By.CssSelector("[data-e2e=\"ShippingName\"]"))[1].Text, " Shiping Name 2 Method in Setting");
            VerifyAreEqual("false", Driver.FindElements(By.CssSelector("[data-e2e=\"ShippingEnabled\"] input"))[1].GetAttribute("value"), " Shiping Enabled 2 Method in Setting");

            Driver.FindElements(By.CssSelector(".switcher-state-trigger"))[2].Click();
            VerifyAreEqual("true", Driver.FindElements(By.CssSelector(".switcher-state-label input"))[1].GetAttribute("value"), " change Shiping Enabled Method in Setting");

            GoToAdmin("settings/paymentmethods");
            VerifyAreEqual("Cash Наличные", Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentName\"]")).Text, " payment Name 1 Method in Setting");
            VerifyAreEqual("true", Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentEnabled\"] input")).GetAttribute("value"), " payment Enabled 1 Method in Setting");
            VerifyAreEqual("WalletOne Единая Касса (Wallet One)", Driver.FindElements(By.CssSelector("[data-e2e=\"PaymentName\"]"))[1].Text, " payment Name 2 Method in Setting");
            VerifyAreEqual("false", Driver.FindElements(By.CssSelector("[data-e2e=\"PaymentEnabled\"] input"))[1].GetAttribute("value"), " payment Enabled 2 Method in Setting");

            Driver.FindElements(By.CssSelector(".switcher-state-trigger"))[2].Click();
            VerifyAreEqual("true", Driver.FindElements(By.CssSelector(".switcher-state-label input"))[1].GetAttribute("value"), " change payment Enabled Method in Setting");

            ProductToCard();

            //отображение методов в корзине
            GoToClient("checkout");
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".checkout-block"))[1].FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".checkout-block"))[3].FindElements(By.CssSelector(".payment-item-radio")).Count, "change payment Method in cart");
            VerifyAreEqual("FixedRateMailMen", Driver.FindElement(By.CssSelector(".shipping-item-title")).Text, " Name shipping 1 Method in cart");
            VerifyAreEqual("Cash", Driver.FindElement(By.CssSelector(".payment-item-title")).Text, " Name payment 1 Method in cart");
            VerifyAreEqual("FreeShipping", Driver.FindElements(By.CssSelector(".shipping-item-title"))[1].Text, " Name shipping 2 Method in cart");
            VerifyAreEqual("WalletOne", Driver.FindElements(By.CssSelector(".payment-item-info"))[1].
                FindElement(By.CssSelector(".payment-item-title")).Text, " Name payment 2 Method in cart");
        }

        [Test]
        public void SettingsMethodsDel()
        {
            GoToAdmin("settings/shippingmethods");
            Driver.FindElement(By.CssSelector(".fa.fa-times")).Click();
            Driver.SwalConfirm();
            Driver.FindElement(By.CssSelector(".fa.fa-times")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".shipping-text")).Count == 0, "del ship method in setting");

            GoToAdmin("settings/paymentmethods");
            Driver.FindElement(By.CssSelector(".fa.fa-times")).Click();
            Driver.SwalConfirm();
            Driver.FindElement(By.CssSelector(".fa.fa-times")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".payment-text")).Count == 0, "del pay method in setting");

            ProductToCard();

            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text, " no method shipping for city in cart");
            VerifyAreEqual("Нет доступных методов оплаты", Driver.FindElement(By.Id("checkoutpayment")).Text, " no method payment in cart");
        }
        public void ProductToCard()
        {
            GoToClient();
            if (Driver.FindElement(By.CssSelector(".cart-mini a")).Text.Contains("пусто"))
            {
                Driver.ScrollTo(By.CssSelector(".products-specials-more"));
                Driver.FindElement(By.CssSelector(".products-view-buttons")).Click();
                Thread.Sleep(2000);
            }
        }
    }
}