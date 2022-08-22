using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem
{
    [TestFixture]
    public class BonusSystemSettingsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(
                ClearType.Catalog | ClearType.Customers | ClearType.Bonuses | ClearType.Shipping);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\Settings\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Settings\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Settings\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Settings\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Settings\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Settings\\Customers.Customer.csv",
                "data\\Admin\\Settings\\BonusSystem\\Settings\\[Order].ShippingMethod.csv",
                "data\\Admin\\Settings\\BonusSystem\\Settings\\[Order].ShippingParam.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Settings\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Settings\\Bonus.Card.csv"
            );
            InitializeService.BonusSystemActive();
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
        public void CheckBonusType()
        {
            GoToAdmin("settingsbonus");
            (new SelectElement(Driver.FindElement(By.Id("BonusType")))).SelectByText("Стоимость товаров и доставки");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();
            IWebElement selectElem1 = Driver.FindElement(By.Id("BonusType"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Стоимость товаров и доставки"),
                "select item shipping");

            ProductToCard("+5 руб. на бонусную карту");
            GoToClient("checkout");
            Driver.ScrollTo(By.CssSelector(".footer-menu-head"));
            VerifyAreEqual("200 руб.", Driver.FindElement(By.CssSelector(".checkout-result-price")).Text,
                " checkout rezult with shipping");
            VerifyAreEqual("10 руб.",
                Driver.FindElement(By.CssSelector(".checkout-bonus-result"))
                    .FindElement(By.CssSelector(".checkout-result-price")).Text,
                "bonus count in checkout with shipping");

            GoToAdmin("settingsbonus");

            (new SelectElement(Driver.FindElement(By.Id("BonusType")))).SelectByText("Стоимость товаров");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("checkout");
            Driver.ScrollTo(By.CssSelector(".footer-menu-head"));
            VerifyAreEqual("200 руб.", Driver.FindElement(By.CssSelector(".checkout-result-price")).Text,
                " checkout rezult without shipping");
            VerifyAreEqual("5 руб.",
                Driver.FindElement(By.CssSelector(".checkout-bonus-result"))
                    .FindElement(By.CssSelector(".checkout-result-price")).Text,
                "bonus count in checkout rezult without shipping");
        }

        [Test]
        public void CheckDescriptionBonus()
        {
            GoToAdmin("settingsbonus");
            Driver.SetCkText("Description BonusTextBlock", "BonusTextBlock");
            Driver.SetCkText("Description BonusRightTextBlock", "BonusRightTextBlock");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();
            Driver.AssertCkText("Description BonusTextBlock", "BonusTextBlock");
            Driver.AssertCkText("Description BonusRightTextBlock", "BonusRightTextBlock");

            GoToClient("getbonuscard");
            VerifyAreEqual("Description BonusTextBlock", Driver.FindElement(By.CssSelector(".loyalty-txt")).Text,
                "main description ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".site-body-cell-no-right")).Text
                    .Contains("Description BonusRightTextBlock"), "right block description");
        }

        [Test]
        public void CheckPaymentBonus()
        {
            GoToAdmin("settingsbonus");
            Driver.FindElement(By.Id("MaxOrderPercent")).Clear();
            Driver.FindElement(By.Id("MaxOrderPercent")).SendKeys("50");
            (new SelectElement(Driver.FindElement(By.Id("BonusType")))).SelectByText("Стоимость товаров");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();

            ProductToCard("+5 руб. на бонусную карту");
            GoToClient("checkout");

            Driver.ScrollTo(By.Id("isBonusApply"));
            VerifyAreEqual("Бонусами по карте (у вас 1000,0 бонусов)",
                Driver.FindElement(By.CssSelector(".custom-input-text")).Text, "bonus count in checkout checkbox");
            Driver.FindElement(By.CssSelector(".bonus-form-label span")).Click();
            Thread.Sleep(1000);

            Driver.ScrollTo(By.CssSelector(".footer-menu-head"));
            VerifyAreEqual("150 руб.", Driver.FindElement(By.CssSelector(".checkout-result-price")).Text,
                " checkout rezult isBonusApply");
            VerifyAreEqual("2 руб.",
                Driver.FindElement(By.CssSelector(".checkout-bonus-result"))
                    .FindElement(By.CssSelector(".checkout-result-price")).Text,
                "bonus count in checkout rezult isBonusApply");

            GoToAdmin("settingsbonus");
            (new SelectElement(Driver.FindElement(By.Id("BonusType")))).SelectByText("Стоимость товаров и доставки");
            Driver.FindElement(By.Id("MaxOrderPercent")).Clear();
            Driver.FindElement(By.Id("MaxOrderPercent")).SendKeys("100");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("checkout");
            Driver.ScrollTo(By.CssSelector(".footer-menu-head"));
            VerifyAreEqual("0 руб.", Driver.FindElement(By.CssSelector(".checkout-result-price")).Text,
                " checkout zero rezult");
        }

        public void ProductToCard(string bonus)
        {
            GoToClient("products/test-product1");
            if (Driver.FindElement(By.CssSelector(".cart-mini")).Text.Contains("пусто"))
            {
                Driver.ScrollTo(By.CssSelector(".rating"));
                VerifyAreEqual(bonus, Driver.FindElement(By.CssSelector(".bonus-string-sum")).Text,
                    "Count bonus in product cart");
                Driver.FindElement(By.CssSelector(".details-payment-inline a")).Click();
                Driver.WaitForElem(By.ClassName("cart-mini-block"));
            }
        }
    }
}