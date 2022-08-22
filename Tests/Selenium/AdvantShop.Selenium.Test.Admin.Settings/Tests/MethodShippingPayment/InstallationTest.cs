using System.Net;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.MethodShippingPayment
{
    [TestFixture]
    public class InstallationTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Shipping | ClearType.Payment);
            InitializeService.LoadData();

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
        public void PaymentMethodsInstallation()
        {
            By selector = AdvBy.DataE2E("PaymentAddSelect");
            string methodName = "";
            int currentIndex = 0, optionsCount = 0;

            GoToAdmin("settings/paymentmethods");
            Driver.FindElement(AdvBy.DataE2E("PaymentAdd")).Click();
            optionsCount = GetSelect(selector).Options.Count;
            Driver.FindElement(By.CssSelector(".modal-dialog .close")).Click();
            while (currentIndex < optionsCount)
            {
                Driver.FindElement(AdvBy.DataE2E("PaymentAdd")).Click();
                Driver.WaitForElem(By.ClassName("modal-header-title"));

                GetSelect(selector).SelectByIndex(currentIndex++);
                methodName = GetSelect(selector).SelectedOption.Text;
                Driver.SendKeysInput(AdvBy.DataE2E("PaymentAddName"), methodName);
                Thread.Sleep(500);
                Driver.FindElement(AdvBy.DataE2E("PaymentAdd")).Click();
                try
                {
                    Driver.WaitForElem(By.Id("PaymentMethodId"));
                }
                catch(Exception ex)
                {
                    VerifyIsNull(ex, "error in payment " + methodName + ", message: " + ex.Message);
                }
                Thread.Sleep(1000);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "page status for payment method " + methodName);
                VerifyIsNull(CheckConsoleLog(true), "console log for payment method " + methodName);

                GoToAdmin("settings/paymentmethods");
                VerifyIsNull(CheckConsoleLog(true), "console log for added payment method " + methodName);
            }
        }

        [Test]
        public void ShippingMethodsInstallation()
        {
            By selector = AdvBy.DataE2E("ShippingAddSelect");
            string methodName = "";
            int currentIndex = 0, optionsCount = 0;

            GoToAdmin("settings/shippingmethods");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            optionsCount = GetSelect(selector).Options.Count;
            Driver.FindElement(By.CssSelector(".modal-dialog .close")).Click();
            while (currentIndex < optionsCount)
            {
                Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
                Driver.WaitForElem(By.ClassName("modal-header-title"));

                GetSelect(selector).SelectByIndex(currentIndex++);
                methodName = GetSelect(selector).SelectedOption.Text;
                Driver.SendKeysInput(AdvBy.DataE2E("ShippingAddName"), methodName);
                Thread.Sleep(500);
                Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
                try
                {
                    Driver.WaitForElem(By.Id("ShippingMethodId"));
                }
                catch (Exception ex)
                {
                    VerifyIsNull(ex, "error in shipping " + methodName + ", message: " + ex.Message);
                }
                Thread.Sleep(1000);

                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "page status for shipping method " + methodName);
                VerifyIsNull(CheckConsoleLog(true), "console log for shipping method " + methodName);

                GoToAdmin("settings/shippingmethods");
                VerifyIsNull(CheckConsoleLog(true), "console log for added shipping method " + methodName);
            }
        }

        public SelectElement GetSelect(By cssSelector)
        {
            return new SelectElement(Driver.FindElement(cssSelector));
        }
    }
}