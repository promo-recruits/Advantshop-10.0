using System;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class MobileAppTest : MySitesFunctions
    {
        string settingName = "Моб. приложение";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToStoreSettings(settingName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void OpenPageMobileApp()
        {
            VerifyIsFalse(GetConsoleLog(settingName), "tab log");
            VerifyIsTrue(Driver.FindElements(By.ClassName("form-group")).Count == 7 &&
                         Driver.FindElements(By.CssSelector(".ng-hide .form-group")).Count == 6, "default items count");
        }

        [Test]
        public void EditMobileApp()
        {
            Driver.SwitchOn("Active", "SettingsTemplateSave");
            VerifyIsTrue(Driver.FindElements(By.ClassName("form-group")).Count == 7 &&
                         Driver.FindElements(By.CssSelector(".ng-hide .form-group")).Count == 0, "default items count");
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            VerifyIsFalse(GetConsoleLog(settingName), "tab log");
            VerifyIsTrue(String.IsNullOrEmpty(Driver.FindElement(By.Id("AppName")).GetAttribute("value")),
                "empty appName");
            VerifyIsTrue(String.IsNullOrEmpty(Driver.FindElement(By.Id("ShortName")).GetAttribute("value")),
                "empty ShortName");
            VerifyIsTrue(String.IsNullOrEmpty(Driver.FindElement(By.Id("AppleAppStoreLink")).GetAttribute("value")),
                "empty AppleAppStoreLink");
            VerifyIsTrue(String.IsNullOrEmpty(Driver.FindElement(By.Id("GooglePlayMarket")).GetAttribute("value")),
                "empty GooglePlayMarket");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-id=\"ShowBadges\"] input")).GetAttribute("value") == "false",
                "empty ShowBadges");

            Driver.FindElement(By.Id("AppName")).SendKeys("AppName");
            Driver.FindElement(By.Id("ShortName")).SendKeys("ShortName");
            Driver.FindElement(By.Id("AppleAppStoreLink")).SendKeys("https://www.apple.com/ru/app-store/");
            Driver.FindElement(By.Id("GooglePlayMarket")).SendKeys("https://play.google.com/store/apps");
            Driver.SwitchOn("ShowBadges");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("AppName", Driver.FindElement(By.Id("AppName")).GetAttribute("value"), "filled appName");
            VerifyAreEqual("ShortName", Driver.FindElement(By.Id("ShortName")).GetAttribute("value"),
                "filled ShortName");
            VerifyAreEqual("https://www.apple.com/ru/app-store/",
                Driver.FindElement(By.Id("AppleAppStoreLink")).GetAttribute("value"), "filled AppleAppStoreLink");
            VerifyAreEqual("https://play.google.com/store/apps",
                Driver.FindElement(By.Id("GooglePlayMarket")).GetAttribute("value"), "filled GooglePlayMarket");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-id=\"ShowBadges\"] input")).GetAttribute("value") == "true",
                "filled ShowBadges");

            //заполнить картинку
            AttachFile(By.XPath("(//input[@type='file'])[1]"), GetPicturePath("brandpic.png"));
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(By.ClassName("toast-error")).Text.IndexOf("Изображение должно быть квадратным") !=
                -1, "save without name and shortname");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            AttachFile(By.XPath("(//input[@type='file'])[1]"), GetPicturePath("avatar.jpg"));
            Thread.Sleep(500);
            string imgSbstr = (BaseUrl + "pictures/mobileapp").Replace("//", "/");

            string firstImg = Driver.FindElement(By.ClassName("picture-uploader-img")).GetAttribute("src")
                .Replace("//", "/");
            VerifyIsTrue(firstImg.IndexOf(imgSbstr) != -1, "first img");
            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.ClassName("picture-uploader-img")).GetAttribute("src").Replace("//", "/")
                    .IndexOf(imgSbstr) != -1, "first img");

            Driver.GetByE2E("imgByHref").Click();
            Thread.Sleep(500);
            Driver.GetByE2E("imgByHrefLinkText")
                .SendKeys("https://www.advantshop.net/Content/company/images/company-about-img.jpg");
            Driver.GetByE2E("imgByHrefBtnSave").Click();
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(By.ClassName("toast-error")).Text.IndexOf("Изображение должно быть квадратным") !=
                -1, "save without name and shortname");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            VerifyAreEqual(firstImg,
                Driver.FindElement(By.ClassName("picture-uploader-img")).GetAttribute("src").Replace("//", "/"),
                "image without second load");
            Driver.GetByE2E("imgByHref").Click();
            Thread.Sleep(500);
            Driver.GetByE2E("imgByHrefLinkText")
                .SendKeys("https://www.advantshop.net/res/partnerImages/b98191cd-1eb2-4b01-b731-0a840e7a65ee.png");
            Driver.GetByE2E("imgByHrefBtnSave").Click();
            Thread.Sleep(500);
            VerifyAreNotEqual(firstImg,
                Driver.FindElement(By.ClassName("picture-uploader-img")).GetAttribute("src").Replace("//", "/"),
                "image without second load");
            VerifyIsTrue(
                Driver.FindElement(By.ClassName("picture-uploader-img")).GetAttribute("src").Replace("//", "/")
                    .IndexOf(imgSbstr) != -1, "first img");

            Driver.GetByE2E("imgDel").Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.FindElements(By.ClassName("picture-uploader-img")).Count == 0, "deleted image");

            Driver.SwitchOff("Active", "SettingsTemplateSave");
            VerifyIsTrue(Driver.FindElements(By.ClassName("form-group")).Count == 7 &&
                         Driver.FindElements(By.CssSelector(".ng-hide .form-group")).Count == 6, "default items count");
        }
    }
}