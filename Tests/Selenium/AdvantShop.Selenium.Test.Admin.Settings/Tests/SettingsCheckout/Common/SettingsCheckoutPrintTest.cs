using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Common
{
    [TestFixture]
    public class SettingsCheckoutPrintTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
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
        public void CheckField()
        {
            GoToAdmin("settingscheckout");
            if (Driver.FindElement(By.Id("PrintOrder_ShowStatusInfo")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowStatusInfo\"]")).Click();
                Thread.Sleep(2000);
            }

            if (!Driver.FindElement(By.Id("PrintOrder_ShowMap")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowMap\"]")).Click();
                Thread.Sleep(2000);
            }

            Driver.ScrollToTop();
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Thread.Sleep(2000);
            }
            catch
            {
            }

            Refresh();
            VerifyIsFalse(Driver.FindElement(By.Id("PrintOrder_ShowStatusInfo")).Selected, "check status info");
            VerifyIsTrue(Driver.FindElement(By.Id("PrintOrder_ShowMap")).Selected, "check show map");
            IWebElement selectElem1 = Driver.FindElement(By.Id("PrintOrder_MapType"));
            SelectElement select3 = new SelectElement(selectElem1);
            (new SelectElement(Driver.FindElement(By.Id("PrintOrder_MapType")))).SelectByText("Яндекс карты");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowStatusInfo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowMap\"]")).Click();
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            Thread.Sleep(2000);
            Refresh();
            VerifyIsTrue(Driver.FindElement(By.Id("PrintOrder_ShowStatusInfo")).Selected, "check status info 2");
            VerifyIsFalse(Driver.FindElement(By.Id("PrintOrder_ShowMap")).Selected, "check show map 2");
            selectElem1 = Driver.FindElement(By.Id("PrintOrder_MapType"));
            SelectElement select4 = new SelectElement(selectElem1);
            VerifyIsTrue(select4.AllSelectedOptions[0].Text.Contains("Яндекс карты"), "check type map 2");
        }
    }

    [TestFixture]
    public class SettingsCheckoutGiftTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingCheckout\\Catalog.Product.csv",
                "data\\Admin\\Settings\\SettingCheckout\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductGifts.csv",
                "data\\Admin\\Settings\\SettingCheckout\\Catalog.Category.csv",
                "data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderContact.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderSource.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderStatus.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].[Order].csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].Certificate.csv",
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
        public void CheckMultiplyGifts()
        {
            GoToAdmin("settingscheckout");
            Driver.ScrollTo(By.Id("BuyInOneClickDefaultPaymentMethod"));

            if (!Driver.FindElement(By.Id("MultiplyGiftsCount")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"MultiplyGiftsCount\"]")).Click();
                Thread.Sleep(2000);
            }

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            }
            catch
            {
            }

            GoToClient("products/test-product21");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-gift-image")).Count == 1, "count gift");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs-3.col-p-v.gift-img")).Count == 1,
                "count product gift");

            Driver.MouseFocus(By.CssSelector(".product-gift-image"));
            VerifyAreEqual("TestProduct10", Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[0].Text,
                "name gift");
            Driver.FindElement(By.CssSelector(".input-small")).SendKeys("0");
            Thread.Sleep(1000);
            Driver.ScrollTo(By.CssSelector(".rating-item"));
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();

            GoToClient("cart");
            VerifyAreEqual("TestProduct21", Driver.FindElement(By.CssSelector(".cart-full-name-link")).Text,
                "name cart");
            VerifyAreEqual("TestProduct10", Driver.FindElements(By.CssSelector(".cart-full-name-link"))[1].Text,
                "name 2 cart");

            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).GetAttribute("value"),
                "Amount cart");
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-ng-bind=\"item.Amount\"]")).Text,
                "Amount 2 cart");

            Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).SendKeys("5");
            Driver.DropFocus("h1");

            VerifyAreEqual("5",
                Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).GetAttribute("value"),
                "Amount change cart");
            VerifyAreEqual("5", Driver.FindElement(By.CssSelector("[data-ng-bind=\"item.Amount\"]")).Text,
                "Amount change 2 cart");

            Functions.CleanCart(Driver, BaseUrl);
        }

        [Test]
        public void CheckSingleGifts()
        {
            Functions.CleanCart(Driver, BaseUrl);
            GoToAdmin("settingscheckout");
            Driver.ScrollTo(By.Id("BuyInOneClickDefaultPaymentMethod"));

            if (Driver.FindElement(By.Id("MultiplyGiftsCount")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"MultiplyGiftsCount\"]")).Click();
                Thread.Sleep(2000);
            }

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            }
            catch
            {
            }

            GoToClient("products/test-product21");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-gift-image")).Count == 1, "count gift");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs-3.col-p-v.gift-img")).Count == 1,
                "count product gift");

            Driver.MouseFocus(By.CssSelector(".product-gift-image"));
            VerifyAreEqual("TestProduct10", Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[0].Text,
                "name gift");
            Driver.FindElement(By.CssSelector(".input-small")).SendKeys("0");
            Thread.Sleep(1000);
            Driver.ScrollTo(By.CssSelector(".rating-item"));
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();

            GoToClient("cart");
            VerifyAreEqual("TestProduct21", Driver.FindElement(By.CssSelector(".cart-full-name-link")).Text,
                "name cart");
            VerifyAreEqual("TestProduct10", Driver.FindElements(By.CssSelector(".cart-full-name-link"))[1].Text,
                "name 2 cart");

            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).GetAttribute("value"),
                "Amount cart");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector("[data-ng-bind=\"item.Amount\"]")).Text,
                "Amount 2 cart");

            Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).SendKeys("5");
            Driver.DropFocus("h1");

            VerifyAreEqual("5",
                Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).GetAttribute("value"),
                "Amount change cart");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector("[data-ng-bind=\"item.Amount\"]")).Text,
                "Amount change 2 cart");

            Functions.CleanCart(Driver, BaseUrl);
        }
    }
}