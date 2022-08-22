using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.MethodShippingPayment
{
    [TestFixture]
    public class MethodPaymentAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Shipping | ClearType.Payment| ClearType.Catalog);
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

        [Order(1)]
        [Test]
        public void SettingsMethodPaymentEditCash()
        {
            GoToAdmin("settings/paymentmethods");
            VerifyAreEqual("Cash Наличные", Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentName\"]")).Text, " payment Name 1 Method in Setting");
            VerifyAreEqual("true", Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentEnabled\"] input")).GetAttribute("value"), " payment Enabled 1 Method in Setting");
            
            GoToAdmin("paymentmethods/edit/1");
            Driver.WaitForElem(AdvBy.DataE2E("PaymentReturn"));
            VerifyAreEqual("Метод оплаты - \"Cash\"", Driver.FindElement(By.TagName("h1")).Text, " h1 teg edit payment");
            VerifyAreEqual("Cash", Driver.FindElement(By.Name("Name")).GetAttribute("value"), " Shiping Name 1 Method in edit ");
            VerifyIsTrue(Driver.FindElement(By.Name("Enabled")).Selected, " payment Enabled 1 Method in edit");
           // VerifyAreEqual("Наличные", driver.FindElements(By.Name(".row.middle-xs .col-xs-6.relative"))[2].Text, " payment type 1 Method in edit");
           
            Driver.FindElement(By.Name("Name")).Clear();
            Driver.FindElement(By.Name("Name")).SendKeys("TestNameCash");
            //country
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCountry\"]")).SendKeys("Россия");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCountryAdd\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCountry\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCountry\"]")).SendKeys("Беларусь");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCountryAdd\"]")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("Беларусь", Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentNameCountry\"]")).Text, " payment 1 country in edit ");
            VerifyAreEqual(", Россия", Driver.FindElements(By.CssSelector("[data-e2e=\"PaymentNameCountry\"]"))[1].Text, " payment 2 country in edit ");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCountryDel\"]")).Click();
            Thread.Sleep(100);
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"PaymentNameCountry\"]")).Count==2, " Shiping country 1 Method in edit ");
            
            //city
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCity\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCity\"]")).SendKeys("Москва");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCityAdd\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCity\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCity\"]")).SendKeys("Самара");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentCityAdd\"]")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("Москва", Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentNameCity\"]")).Text, " payment 1 city in edit ");
            VerifyAreEqual("Самара", Driver.FindElements(By.CssSelector("[data-e2e=\"PaymentNameCity\"]"))[1].Text, " payment 2  city in edit ");
            Thread.Sleep(100);
            Driver.FindElements(By.CssSelector("[data-e2e=\"PaymentCityDel\"]"))[1].Click();

            //description
            Driver.FindElement(By.Id("Description")).Clear();
            Driver.FindElement(By.Id("Description")).SendKeys("New Description here");
            Driver.FindElement(By.Name("SortOrder")).Clear();
            Driver.FindElement(By.Name("SortOrder")).SendKeys("0");

            //attachFile(driver, By.CssSelector("input[type=\"file\"]"), GetPicturePath("brand_logo.jpg"));
            Driver.ScrollTo(By.Name("ExtrachargeInNumbers"));
            Driver.FindElement(By.Name("ExtrachargeInNumbers")).Clear();
            Driver.FindElement(By.Name("ExtrachargeInNumbers")).SendKeys("300");
            Thread.Sleep(100);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentAdd\"]")).Click();
            Driver.WaitForToastSuccess();

            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentReturn\"] a")).Click();
            Driver.WaitForElem(By.TagName("payment-methods-list"));
            VerifyIsTrue(Driver.Url.Contains("settings/paymentmethods"), " return from edit paymentm");
            VerifyAreEqual("TestNameCash Наличные", Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentName\"]")).Text, " payment Name 1 Method in Setting");
            VerifyAreEqual("true", Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentEnabled\"] input")).GetAttribute("value"), " payment Enabled 1 Method in Setting");

            ProductToCard();

            //отображение методов в корзине
            GoToClient("checkout");

            VerifyAreEqual("TestNameCash", Driver.FindElement(By.CssSelector(".payment-item-title")).Text, " Name edited payment Method in cart");
            VerifyAreEqual("New Description here", Driver.FindElement(By.CssSelector(".payment-item-description.cs-t-3")).Text, "Description edited payment Method in cart");
            VerifyAreEqual("300 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Payment.Value\"]")).Text, " cost edited payment Method in cart");
        }

        [Order(0)]
        [Test]
        public void SettingsMethodPaymentAdd()
        {
            GoToAdmin("settings/paymentmethods");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentAdd\"]")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Метод оплаты", Driver.FindElement(By.TagName("h2")).Text, " h2 add payment method");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentAddName\"]")).SendKeys("New payment Method");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentAddSelect\"]")))).SelectByText("Единая Касса (Wallet One)");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentAddDesc\"]")).SendKeys("New Description payment Method");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PaymentAdd\"]")).Click();
            Thread.Sleep(100);

            GoToAdmin("settings/paymentmethods");
            VerifyIsTrue(Driver.PageSource.Contains("New payment Method"), "Show on page method in list");
            Driver.FindElements(By.CssSelector("[data-e2e=\"PaymentEnabled\"] span"))[4].Click();
            Thread.Sleep(100);
            ProductToCard();
            //отображение методов в корзине
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-page"));
            VerifyIsTrue(Driver.PageSource.Contains("New payment Method"), "Show on page method in cart");
        }
        //.products-view-buttons
        public void ProductToCard()
        {
            GoToClient();
            if (Driver.FindElement(By.CssSelector(".cart-mini a")).Text.Contains("пусто"))
            {
                Driver.ScrollTo(By.CssSelector(".products-specials-more"));
                Driver.FindElement(By.CssSelector(".products-view-buttons")).Click();
                Driver.WaitForElem(By.ClassName("cart-mini-block"));
            }
        }
    }
}
