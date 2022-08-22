using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Common
{
    [TestFixture]
    public class SettingsCheckoutzOrdersNumberTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Offer.csv",
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
        public void ChangeNumOrder()
        {
            GoToAdmin("settingscheckout");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"NextOrderNumber\"]"));

            Driver.FindElement(By.Id("NextOrderNumber")).Clear();
            Driver.FindElement(By.Id("NextOrderNumber")).SendKeys("1111");
            Driver.FindElement(By.Id("OrderNumberFormat")).Clear();
            Driver.FindElement(By.Id("OrderNumberFormat")).SendKeys("#NUMBER#");

            Driver.FindElement(By.CssSelector("[data-e2e=\"NextOrderNumber\"]")).Click();
            Thread.Sleep(2000);
            Driver.ScrollToTop();
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            }
            catch
            {
            }

            Functions.NewOrderClient_450(Driver, BaseUrl);
            VerifyAreEqual("Ваш заказ принят под номером 1111", Driver.FindElement(By.CssSelector(".congrat-num")).Text,
                "num 1 order");
            Functions.NewOrderClient_450(Driver, BaseUrl);
            VerifyAreEqual("Ваш заказ принят под номером 1112", Driver.FindElement(By.CssSelector(".congrat-num")).Text,
                "num 2 order");

            GoToAdmin("settingscheckout");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"NextOrderNumber\"]"));
            VerifyAreEqual("1113", Driver.FindElement(By.Id("NextOrderNumber")).GetAttribute("value"),
                "num next order");
        }

        [Test]
        public void ChangeNumOrderUseFormat()
        {
            GoToAdmin("settingscheckout");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"NextOrderNumber\"]"));

            Driver.FindElement(By.Id("NextOrderNumber")).Clear();
            Driver.FindElement(By.Id("NextOrderNumber")).SendKeys("2222");
            Driver.FindElement(By.CssSelector("[data-e2e=\"NextOrderNumber\"]")).Click();
            Thread.Sleep(2000);

            Driver.FindElement(By.Id("OrderNumberFormat")).Clear();
            Driver.FindElement(By.Id("OrderNumberFormat")).SendKeys("Order:#NUMBER#.#YEAR#.#MONTH#.#DAY#.");
            Driver.ScrollToTop();
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            }
            catch
            {
            }

            Functions.NewOrderClient_450(Driver, BaseUrl);
            string s = String.Format("Ваш заказ принят под номером Order:2222.{0}.{1}.{2}.",
                DateTime.Now.ToString("yy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"));
            VerifyAreEqual(s, Driver.FindElement(By.CssSelector(".congrat-num")).Text, "num 1 order");
            Functions.NewOrderClient_450(Driver, BaseUrl);
            s = String.Format("Ваш заказ принят под номером Order:2223.{0}.{1}.{2}.", DateTime.Now.ToString("yy"),
                DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"));
            VerifyAreEqual(s, Driver.FindElement(By.CssSelector(".congrat-num")).Text, "num 2 order");

            GoToAdmin("settingscheckout");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"NextOrderNumber\"]"));
            Driver.FindElement(By.Id("OrderNumberFormat")).Clear();
            Driver.FindElement(By.Id("OrderNumberFormat")).SendKeys("#NUMBER#");
            Driver.ScrollToTop();
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            }
            catch
            {
            }
        }
    }
}