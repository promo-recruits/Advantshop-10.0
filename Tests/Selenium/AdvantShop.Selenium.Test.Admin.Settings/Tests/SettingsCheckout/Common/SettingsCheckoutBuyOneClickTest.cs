using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Common
{
    [TestFixture]
    public class SettingsCheckoutBuyOneClickTest : BaseSeleniumTest
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
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderContact.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderStatus.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].[Order].csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].Certificate.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\[Order].OrderSource.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderCurrency.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderItems.csv"
            );
            Init();

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
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
        public void EnableBuyInOneClick()
        {
            GoToAdmin("settingscheckout");

            if (!Driver.FindElement(By.Id("BuyInOneClick")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BuyInOneClick\"]")).Click();
                Thread.Sleep(2000);
            }

            if (Driver.FindElement(By.Id("BuyInOneClickDisableInCheckout")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"OneClickDisableInCheckout\"]")).Click();
                Thread.Sleep(2000);
            }

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(2000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-buy-one-click")).Displayed, "btn buy inOneClick");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-buy-one-click")).Enabled,
                "Enabled btn buy inOneClick");
            Thread.Sleep(2000);
            GoToClient("cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action span")).Displayed,
                "btn buy cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action span")).Enabled,
                "Enabled btn buy cart");

            GoToClient("checkout");
            Driver.WaitForElem(By.CssSelector(".checkout-cart-oneclick"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-cart-oneclick")).Displayed, " btn buy  checkout");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-cart-oneclick a")).Enabled,
                "Enabled btn buy checkout");
        }

        [Test]
        public void NotEnableBuyInOneClick()
        {
            GoToAdmin("settingscheckout");

            if (!Driver.FindElement(By.Id("BuyInOneClick")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BuyInOneClick\"]")).Click();
                Thread.Sleep(2000);
            }

            if (!Driver.FindElement(By.Id("BuyInOneClickDisableInCheckout")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"OneClickDisableInCheckout\"]")).Click();
                Thread.Sleep(2000);
            }

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(2000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-buy-one-click")).Displayed, "btn buy inOneClick");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-buy-one-click")).Enabled,
                "Enabled btn buy inOneClick");
            Thread.Sleep(2000);
            GoToClient("cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action span")).Displayed,
                "btn buy cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action span")).Enabled,
                "Enabled btn buy cart");

            GoToClient("checkout");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".checkout-cart-oneclick")).Count == 0,
                " btn buy no checkout");
        }

        [Test]
        public void NotVisibleBuyInOneClick()
        {
            GoToAdmin("settingscheckout");

            if (Driver.FindElement(By.Id("BuyInOneClick")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BuyInOneClick\"]")).Click();
                Thread.Sleep(2000);
            }

            if (Driver.FindElement(By.Id("BuyInOneClickDisableInCheckout")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"OneClickDisableInCheckout\"]")).Click();
                Thread.Sleep(2000);
            }

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(2000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-buy-one-click")).Count == 0,
                "btn buy inOneClick");
            Thread.Sleep(2000);
            GoToClient("cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".btn.btn-middle.btn-action span")).Count == 0,
                "btn buy cart");

            GoToClient("checkout");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".checkout-cart-oneclick")).Count == 0,
                " btn buy no checkout");
        }

        [Test]
        public void NotVisibleEnableBuyInOneClick()
        {
            GoToAdmin("settingscheckout");

            if (Driver.FindElement(By.Id("BuyInOneClick")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BuyInOneClick\"]")).Click();
                Thread.Sleep(2000);
            }

            if (!Driver.FindElement(By.Id("BuyInOneClickDisableInCheckout")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"OneClickDisableInCheckout\"]")).Click();
                Thread.Sleep(2000);
            }

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(2000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-buy-one-click")).Count == 0,
                "btn buy inOneClick");
            Thread.Sleep(2000);
            GoToClient("cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".btn.btn-middle.btn-action span")).Count == 0,
                "btn buy cart");

            GoToClient("checkout");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".checkout-cart-oneclick")).Count == 0,
                " btn buy no checkout");
        }

        [Test]
        public void SelectLeadBuyInOneClick()
        {
            GoToAdmin("settingscheckout");

            if (!Driver.FindElement(By.Id("BuyInOneClick")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BuyInOneClick\"]")).Click();
                Thread.Sleep(2000);
            }

            if (Driver.FindElement(By.Id("BuyInOneClickDisableInCheckout")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"OneClickDisableInCheckout\"]")).Click();
                Thread.Sleep(2000);
            }

            Driver.ScrollTo(By.Id("BuyInOneClick"));
            (new SelectElement(Driver.FindElement(By.Id("BuyInOneClickAction")))).SelectByText("Создавать лид");
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(2000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");

            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.Id("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormName")).SendKeys("NewLead");
            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "h1");
            VerifyAreEqual("Ваш телефон внесен в очередь, скоро мы с Вами свяжемся.",
                Driver.FindElement(By.CssSelector(".checkout-success-content-top")).Text, "final text");
            VerifyIsTrue(Driver.Url.Contains("checkout/buyinoneclicksuccess"), "url");

            GoToAdmin("leads");

            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text, " Grid lead StatusName");
            VerifyAreEqual("500 руб.", Driver.GetGridCell(0, "SumFormatted").Text, " Grid lead sum");
        }

        [Test]
        public void SelectOrderBuyInOneClick()
        {
            GoToAdmin("settingscheckout");

            if (!Driver.FindElement(By.Id("BuyInOneClick")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BuyInOneClick\"]")).Click();
                Thread.Sleep(2000);
            }

            if (Driver.FindElement(By.Id("BuyInOneClickDisableInCheckout")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"OneClickDisableInCheckout\"]")).Click();
                Thread.Sleep(2000);
            }

            Driver.ScrollTo(By.Id("BuyInOneClick"));
            (new SelectElement(Driver.FindElement(By.Id("BuyInOneClickAction")))).SelectByText("Создавать заказ");
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(2000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");

            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.Id("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormName")).SendKeys("NewOrder");
            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "sucsess");

            GoToAdmin("orders");

            VerifyAreEqual("NewOrder", Driver.GetGridCell(0, "BuyerName").Text, " Grid orders StatusName");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");
            VerifyAreEqual("500 руб.", Driver.GetGridCell(0, "SumFormatted").Text, " Grid orders sum");
        }


        [Test]
        public void SelectPaymentBuyInOneClick()
        {
            GoToAdmin("settingscheckout");

            if (!Driver.FindElement(By.Id("BuyInOneClick")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BuyInOneClick\"]")).Click();
                Thread.Sleep(2000);
            }

            if (Driver.FindElement(By.Id("BuyInOneClickDisableInCheckout")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"OneClickDisableInCheckout\"]")).Click();
                Thread.Sleep(2000);
            }

            Driver.ScrollTo(By.Id("BuyInOneClick"));
            (new SelectElement(Driver.FindElement(By.Id("BuyInOneClickAction")))).SelectByText("Создавать заказ");
            (new SelectElement(Driver.FindElement(By.Id("BuyInOneClickDefaultPaymentMethod")))).SelectByText(
                "При получении (наличными или банковской картой)");
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(2000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");

            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.Id("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormName")).SendKeys("NewOrderPay");
            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "sucsess");

            GoToAdmin("orders");

            VerifyAreEqual("NewOrderPay", Driver.GetGridCell(0, "BuyerName").Text, " Grid orders StatusName");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");

            Driver.GetGridCell(0, "StatusName").Click();
            VerifyAreEqual("При получении (наличными или банковской картой)",
                Driver.FindElement(By.CssSelector("[ng-bind=\"$ctrl.Summary.PaymentName\"]")).Text, " method");

            GoToAdmin("settingscheckout");
            (new SelectElement(Driver.FindElement(By.Id("BuyInOneClickDefaultPaymentMethod")))).SelectByText("----");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            GoToClient("products/test-product5");

            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();
            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);

            GoToAdmin("orders");

            Driver.GetGridCell(0, "StatusName").Click();
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[ng-bind=\"$ctrl.Summary.ShippingName\"]")).Text,
                " no method");
        }

        [Test]
        public void SelectShippingBuyInOneClick()
        {
            GoToAdmin("settingscheckout");

            if (!Driver.FindElement(By.Id("BuyInOneClick")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BuyInOneClick\"]")).Click();
                Thread.Sleep(2000);
            }

            if (Driver.FindElement(By.Id("BuyInOneClickDisableInCheckout")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"OneClickDisableInCheckout\"]")).Click();
                Thread.Sleep(2000);
            }

            Driver.ScrollTo(By.Id("BuyInOneClick"));
            (new SelectElement(Driver.FindElement(By.Id("BuyInOneClickAction")))).SelectByText("Создавать заказ");
            (new SelectElement(Driver.FindElement(By.Id("BuyInOneClickDefaultShippingMethod"))))
                .SelectByText("Курьером");
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(2000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");

            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.Id("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Id("buyOneClickFormName")).SendKeys("NewOrderShipping");
            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "sucsess");

            GoToAdmin("orders");

            VerifyAreEqual("NewOrderShipping", Driver.GetGridCell(0, "BuyerName").Text, " Grid orders StatusName");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");

            Driver.GetGridCell(0, "StatusName").Click();
            VerifyAreEqual("Курьером",
                Driver.FindElement(By.CssSelector("[ng-bind=\"$ctrl.Summary.ShippingName\"]")).Text, " method");


            GoToAdmin("settingscheckout");
            (new SelectElement(Driver.FindElement(By.Id("BuyInOneClickDefaultShippingMethod")))).SelectByText("----");
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            GoToClient("products/test-product5");

            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();
            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Thread.Sleep(2000);

            GoToAdmin("orders");

            Driver.GetGridCell(0, "StatusName").Click();
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[ng-bind=\"$ctrl.Summary.ShippingName\"]")).Text,
                " no method");
        }

        [Test]
        public void TextBuyInOneClick()
        {
            GoToAdmin("settingscheckout");

            if (!Driver.FindElement(By.Id("BuyInOneClick")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BuyInOneClick\"]")).Click();
                Thread.Sleep(2000);
            }

            if (Driver.FindElement(By.Id("BuyInOneClickDisableInCheckout")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"OneClickDisableInCheckout\"]")).Click();
                Thread.Sleep(2000);
            }

            Driver.FindElement(By.Id("BuyInOneClickLinkText")).Clear();
            Driver.FindElement(By.Id("BuyInOneClickLinkText")).SendKeys("Test link text");

            Driver.FindElement(By.Id("BuyInOneClickFirstText")).Clear();
            Driver.FindElement(By.Id("BuyInOneClickFirstText")).SendKeys("Test big text in window");

            Driver.FindElement(By.Id("BuyInOneClickButtonText")).Clear();
            Driver.FindElement(By.Id("BuyInOneClickButtonText")).SendKeys("TestOrder");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(2000);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-buy-one-click")).Displayed, "btn buy inOneClick");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-buy-one-click")).Enabled,
                "Enabled btn buy inOneClick");
            VerifyAreEqual("Test link text", Driver.FindElement(By.CssSelector(".details-buy-one-click")).Text,
                "text btn buy inOneClick prod");
            Driver.FindElement(By.CssSelector(".details-buy-one-click")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Test big text in window", Driver.FindElement(By.CssSelector(".buy-one-click-text")).Text,
                " big text btn buy inOneClick");
            VerifyAreEqual("TestOrder",
                Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).GetAttribute("value"),
                "btn text btn buy inOneClick");

            GoToClient("cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action span")).Displayed,
                "btn buy cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action span")).Enabled,
                "Enabled btn buy cart");
            VerifyAreEqual("Test link text", Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action span")).Text,
                "text btn buy inOneClick cart");
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action span")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Test big text in window", Driver.FindElement(By.CssSelector(".buy-one-click-text")).Text,
                " big text btn buy inOneClick cart");
            VerifyAreEqual("TestOrder",
                Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).GetAttribute("value"),
                "btn text btn buy inOneClick cart");

            GoToClient("checkout");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-cart-oneclick")).Displayed, " btn buy  checkout");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-cart-oneclick a")).Enabled,
                "Enabled btn buy checkout");
            VerifyAreEqual("Test link text", Driver.FindElement(By.CssSelector(".checkout-cart-oneclick a")).Text,
                "text btn buy inOneClick checkout");
            Driver.FindElement(By.CssSelector(".checkout-cart-oneclick a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Test big text in window", Driver.FindElement(By.CssSelector(".buy-one-click-text")).Text,
                " big text btn buy inOneClick checkout");
            VerifyAreEqual("TestOrder",
                Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).GetAttribute("value"),
                "btn text btn buy inOneClick checkout");
        }
    }
}