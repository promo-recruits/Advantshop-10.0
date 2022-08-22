using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Booking.Tests.Affiliate
{
    [TestFixture]
    public class BookingAffiliateAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.BookingActive();
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
        [Order(1)]
        public void BookingAffiliateOpenAndEdit()
        {
            GoToAdmin("booking");

            VerifyAreEqual("Основной филиал",
                Driver.FindElement(By.CssSelector(".dropdown .page-name-block-text")).Text, "affiliate header before");
            GoToAdmin("bookingaffiliate/settings");

            Driver.FindElement(By.Id("Affiliate_Name")).Click();
            Driver.FindElement(By.Id("Affiliate_Name")).Clear();
            Driver.FindElement(By.Id("Affiliate_Name")).SendKeys("Main affiliate");

            Driver.FindElement(By.Id("Affiliate_Address")).Click();
            Driver.FindElement(By.Id("Affiliate_Address")).Clear();
            Driver.FindElement(By.Id("Affiliate_Address")).SendKeys("Address");

            Driver.FindElement(By.Id("Affiliate_Phone")).Click();
            Driver.ClearInput(By.Id("Affiliate_Phone"));
            Driver.FindElement(By.Id("Affiliate_Phone")).SendKeys("79998522558");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();

            Refresh();

            VerifyAreEqual("Main affiliate", Driver.FindElement(By.Id("Affiliate_Name")).GetAttribute("value"),
                "name of affiliate");
            VerifyAreEqual("Address", Driver.FindElement(By.Id("Affiliate_Address")).GetAttribute("value"),
                "address of affiliate");
            VerifyAreEqual("+7(999)852-25-58", Driver.FindElement(By.Id("Affiliate_Phone")).GetAttribute("value"),
                "phone of affiliate, mask is work");

            GoToAdmin("booking");

            VerifyAreEqual("Main affiliate", Driver.FindElement(By.CssSelector(".dropdown .page-name-block-text")).Text,
                "affiliate header after");
        }

        [Test]
        [Order(2)]
        public void BookingAffiliateAdd()
        {
            GoToAdmin("booking");

            Driver.MouseFocus(By.CssSelector(".header-bottom-menu-link"));
            Driver.XPathContainsText("a", "Добавить филиал");
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"affiliateName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"affiliateName\"]")).SendKeys("New Affiliate");
            Driver.FindElement(By.CssSelector(".btn-save")).Click();

            VerifyAreEqual("New Affiliate", Driver.FindElement(By.Id("Affiliate_Name")).GetAttribute("value"),
                "name of affiliate");

            GoToAdmin("booking");

            Driver.MouseFocus(By.CssSelector(".header-bottom-menu-link"));
            VerifyAreEqual("New Affiliate", Driver.FindElement(By.CssSelector(".dropdown .page-name-block-text")).Text,
                "header contain new affiliate");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".dropdown-menu--limited li:not(.divider)")).Count == 2,
                "count items in dropdown-menu");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".dropdown-menu--limited li > a")).Text.Contains("Main affiliate"),
                "select contain old affiliate");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".dropdown-menu--limited li a.dropdown-menu-link")).Text
                    .Contains("Добавить филиал"), "select contain add-link");
        }

        [Test]
        [Order(3)]
        public void BookingAffiliateDelete()
        {
            GoToAdmin("booking/index/2");
            GoToAdmin("bookingaffiliate/settings");

            Driver.FindElement(By.CssSelector(".btn.btn-sm.btn-red-white")).Click();

            Driver.SwalConfirm();

            GoToAdmin("bookingaffiliate/settings");
            Driver.FindElement(By.CssSelector(".btn.btn-sm.btn-red-white")).Click();

            Driver.SwalConfirm();

            VerifyIsTrue(Driver.PageSource.Contains("Добавить филиал"), "no affiliate");
        }
    }
}