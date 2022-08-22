using System;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.Settings
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class MobileAppSettingsTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\Default\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSubBlock.csv"
            );

            Init(false);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin("funnels/site/1#?landingAdminTab=settings&landingSettingsTab=mobileApp");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }


        [Test]
        public void OpenPageMobileApp()
        {
            VerifyIsFalse(GetConsoleLog(Driver.Url), "tab log");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElements(By.ClassName("form-group"))
                             .Count == 6 &&
                         Driver.FindElement(By.CssSelector(".tab-pane.active"))
                             .FindElements(By.CssSelector(".ng-hide .form-group")).Count == 5, "default items count");
        }

        [Test]
        public void EditMobileApp()
        {
            Driver.SwitchOn("Active", "funnelSettingSave");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElements(By.ClassName("form-group"))
                             .Count == 6 &&
                         Driver.FindElement(By.CssSelector(".tab-pane.active"))
                             .FindElements(By.CssSelector(".ng-hide .form-group")).Count == 0, "default items count");
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            VerifyIsTrue(CheckConsoleLog() == null, "tab log");
            VerifyIsTrue(String.IsNullOrEmpty(Driver.FindElement(By.Id("MobileAppName")).GetAttribute("value")),
                "empty MobileAppName");
            VerifyIsTrue(String.IsNullOrEmpty(Driver.FindElement(By.Id("MobileAppShortName")).GetAttribute("value")),
                "empty MobileAppShortName");
            VerifyIsTrue(String.IsNullOrEmpty(Driver.FindElement(By.Id("AppleAppStoreLink")).GetAttribute("value")),
                "empty AppleAppStoreLink");
            VerifyIsTrue(String.IsNullOrEmpty(Driver.FindElement(By.Id("GooglePlayMarketLink")).GetAttribute("value")),
                "empty GooglePlayMarketLink");

            Driver.FindElement(By.Id("MobileAppName")).SendKeys("MobileAppName");
            Driver.FindElement(By.Id("MobileAppShortName")).SendKeys("MobileAppShortName");
            Driver.FindElement(By.Id("AppleAppStoreLink")).SendKeys("https://www.apple.com/ru/app-store/");
            Driver.FindElement(By.Id("GooglePlayMarketLink")).SendKeys("https://play.google.com/store/apps");
            Driver.FindElement(AdvBy.DataE2E("funnelSettingSave")).Click();
            Driver.WaitForAjax();
            Refresh();

            VerifyAreEqual("MobileAppName", Driver.FindElement(By.Id("MobileAppName")).GetAttribute("value"),
                "filled MobileAppName");
            VerifyAreEqual("MobileAppShortName", Driver.FindElement(By.Id("MobileAppShortName")).GetAttribute("value"),
                "filled MobileAppShortName");
            VerifyAreEqual("https://www.apple.com/ru/app-store/",
                Driver.FindElement(By.Id("AppleAppStoreLink")).GetAttribute("value"), "filled AppleAppStoreLink");
            VerifyAreEqual("https://play.google.com/store/apps",
                Driver.FindElement(By.Id("GooglePlayMarketLink")).GetAttribute("value"), "filled GooglePlayMarketLink");

            //заполнить картинку
            AttachFile(By.XPath("(//input[@type='file'])[1]"), GetPicturePath("brandpic.png"));
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElement(By.ClassName("toast-error")).Text.IndexOf("Изображение должно быть квадратным") !=
                -1, "save without name and shortname");
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            AttachFile(By.XPath("(//input[@type='file'])[1]"), GetPicturePath("avatar.jpg"));
            Thread.Sleep(500);
            string imgSbstr = (BaseUrl + "pictures/landing/1").Replace("//", "/");

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
                -1, "save without name and MobileAppShortName");
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

            Driver.SwitchOff("Active", "funnelSettingSave");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElements(By.ClassName("form-group"))
                             .Count == 6 &&
                         Driver.FindElement(By.CssSelector(".tab-pane.active"))
                             .FindElements(By.CssSelector(".ng-hide .form-group")).Count == 5, "default items count");
        }
    }
}