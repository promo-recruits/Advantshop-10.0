using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests
{
    public class SettingsLeadFunctions : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetUp()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        public void leadBuyOneClick(string productId, string customerName, string customerPhone,
            string customerEmail = "")
        {
            GoToClient("products/test-product" + productId);
            Refresh();

            Driver.ScrollTo(By.CssSelector("[data-product-id=\"" + productId + "\"]"));
            Driver.FindElement(By.LinkText("Купить в один клик")).Click();
            Driver.WaitForElem(By.ClassName("buy-one-click-dialog"));
            Driver.FindElement(By.Name("buyOneClickFormName")).Click();
            Driver.FindElement(By.Name("buyOneClickFormName")).Clear();
            Driver.FindElement(By.Name("buyOneClickFormName")).SendKeys(customerName);

            if (customerEmail != "")
            {
                Driver.FindElement(By.Name("buyOneClickFormEmail")).Click();
                Driver.FindElement(By.Name("buyOneClickFormEmail")).Clear();
                Driver.FindElement(By.Name("buyOneClickFormEmail")).SendKeys(customerEmail);
            }

            Driver.FindElement(By.Name("buyOneClickFormPhone")).Click();
            Driver.FindElement(By.Name("buyOneClickFormPhone")).SendKeys(customerPhone);

            Driver.FindElement(By.CssSelector("[value=\"Заказать\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
        }

        public void setLeadBuyInOneClick()
        {
            GoToAdmin("settingscheckout#?checkoutTab=common");

            IWebElement selectElem1 = Driver.FindElement(By.Name("BuyInOneClickAction"));
            SelectElement select1 = new SelectElement(selectElem1);

            if (!select1.SelectedOption.Text.Contains("Создавать лид"))
            {
                Driver.ScrollTo(By.Name("BuyInOneClickLinkText"));
                (new SelectElement(Driver.FindElement(By.Name("BuyInOneClickAction")))).SelectByText("Создавать лид");

                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector(".btn.btn-sm.btn-success.btn--margin-left")).Click();
                Driver.WaitForToastSuccess();
            }
        }
    }
}