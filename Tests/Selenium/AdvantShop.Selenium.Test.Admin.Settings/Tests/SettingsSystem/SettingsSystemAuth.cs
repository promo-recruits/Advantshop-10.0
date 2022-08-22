using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSystem
{
    [TestFixture]
    public class SettingsSystemAuth : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingSystem\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingSystem\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingSystem\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingSystem\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingSystem\\Catalog.ProductCategories.csv"
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
        public void SettingCheckLink()
        {
            GoToAdmin("settingssystem#?systemTab=auth");

            Driver.XPathContainsText("a", "Инструкция. Настройка кнопок авторизации OAuth Google");
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.Url.Contains("advantshop.net/help/pages/openid-google-text"), "check tag link google");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 google");
            Functions.CloseTab(Driver, BaseUrl);

            Driver.XPathContainsText("a", "Инструкция. Настройка кнопок авторизации OAuth Yandex");
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.Url.Contains("advantshop.net/help/pages/openid-yandex"), "check tag link Yandex");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 Yandex");
            Functions.CloseTab(Driver, BaseUrl);

            Driver.XPathContainsText("a", "Инструкция. Настройка кнопок авторизации OAuth Mail.ru");
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.Url.Contains("advantshop.net/help/pages/openid-mail-ru"), "check tag link Mail");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 Mail");
            Functions.CloseTab(Driver, BaseUrl);

            Driver.XPathContainsText("a", "Инструкция. Настройка кнопок авторизации OAuth ВКонтакте");
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.Url.Contains("advantshop.net/help/pages/openid-vk"), "check tag link vk");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 vk");
            Functions.CloseTab(Driver, BaseUrl);

            Driver.XPathContainsText("a", "Инструкция. Настройка кнопок авторизации OAuth Facebook");
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.Url.Contains("advantshop.net/help/pages/openid-fb-text"), "check tag link Facebook");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 Facebook");
            Functions.CloseTab(Driver, BaseUrl);

            Driver.XPathContainsText("a",
                "Инструкция. Настройка кнопок авторизации OAuth Odnoklassniki(Одноклассники)");
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.Url.Contains("advantshop.net/help/pages/openid-odk"), "check tag link Odnoklassniki");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 Odnoklassniki");
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void SettingCheckEnabled()
        {
            GoToAdmin("settingssystem#?systemTab=auth");

            Functions.CheckNotSelected("GoogleActive", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
                GoToAdmin("settingssystem#?systemTab=auth");
            }
            catch
            {
            }

            VerifyIsFalse(Driver.FindElement(By.Id("GoogleActive")).Selected, "Selected GoogleActive");


            Functions.CheckNotSelected("YandexActive", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Thread.Sleep(1000);
                GoToAdmin("settingssystem#?systemTab=auth");
            }
            catch
            {
            }

            VerifyIsFalse(Driver.FindElement(By.Id("YandexActive")).Selected, "Selected YandexActive");

            Functions.CheckNotSelected("MailActive", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Thread.Sleep(1000);
                GoToAdmin("settingssystem#?systemTab=auth");
            }
            catch
            {
            }

            VerifyIsFalse(Driver.FindElement(By.Id("MailActive")).Selected, "Selected MailActive");

            Functions.CheckNotSelected("VkontakteActive", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Thread.Sleep(1000);
                GoToAdmin("settingssystem#?systemTab=auth");
            }
            catch
            {
            }

            VerifyIsFalse(Driver.FindElement(By.Id("VkontakteActive")).Selected, "Selected VkontakteActive");

            Functions.CheckNotSelected("FacebookActive", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Thread.Sleep(1000);
                GoToAdmin("settingssystem#?systemTab=auth");
            }
            catch
            {
            }

            VerifyIsFalse(Driver.FindElement(By.Id("FacebookActive")).Selected, "Selected FacebookActive");

            Functions.CheckNotSelected("OdnoklassnikiActive", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Thread.Sleep(1000);
                GoToAdmin("settingssystem#?systemTab=auth");
            }
            catch
            {
            }

            VerifyIsFalse(Driver.FindElement(By.Id("OdnoklassnikiActive")).Selected, "Selected OdnoklassnikiActive");

            Driver.ScrollToTop();
            Functions.CheckSelected("GoogleActive", Driver);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
            Thread.Sleep(1000);
            GoToAdmin("settingssystem#?systemTab=auth");
            VerifyIsTrue(Driver.FindElement(By.Id("GoogleActive")).Selected, "Selected GoogleActive");

            Functions.CheckSelected("YandexActive", Driver);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
            Thread.Sleep(1000);
            GoToAdmin("settingssystem#?systemTab=auth");
            VerifyIsTrue(Driver.FindElement(By.Id("YandexActive")).Selected, "Selected YandexActive");

            Functions.CheckSelected("MailActive", Driver);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
            Thread.Sleep(1000);
            GoToAdmin("settingssystem#?systemTab=auth");
            VerifyIsTrue(Driver.FindElement(By.Id("MailActive")).Selected, "Selected MailActive");

            Functions.CheckSelected("VkontakteActive", Driver);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
            Thread.Sleep(1000);
            GoToAdmin("settingssystem#?systemTab=auth");
            VerifyIsTrue(Driver.FindElement(By.Id("VkontakteActive")).Selected, "Selected VkontakteActive");

            Functions.CheckSelected("FacebookActive", Driver);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
            Thread.Sleep(1000);
            GoToAdmin("settingssystem#?systemTab=auth");
            VerifyIsTrue(Driver.FindElement(By.Id("FacebookActive")).Selected, "Selected FacebookActive");

            Functions.CheckSelected("OdnoklassnikiActive", Driver);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
            Thread.Sleep(1000);
            GoToAdmin("settingssystem#?systemTab=auth");
            VerifyIsTrue(Driver.FindElement(By.Id("OdnoklassnikiActive")).Selected, "Selected OdnoklassnikiActive");
        }
    }
}