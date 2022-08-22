using System;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.AdminPanelActions.Settings
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class IconSettingsTest : MySitesFunctions
    {
        string siteUrl = "lp/testfunnel_1?inplace=false";
        By faviconSelector = By.CssSelector("link[rel=\"shortcut icon\"]");
        By faviconImgSelector = By.CssSelector(".tab-content-active img");

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSubBlock.csv"
            );

            Init(false);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToClient(siteUrl);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void LoadFaviconNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work, cause task 23718");

            GoToClient(siteUrl);
            VerifyAreEqual("image/ico", Driver.FindElement(faviconSelector).GetAttribute("type"), "default type");
            VerifyAreEqual(BaseUrl + "pictures/favicon.ico", Driver.FindElement(faviconSelector).GetAttribute("href"),
                "default href");

            GoToIconSettings();
            VerifyAreEqual(0, Driver.FindElements(faviconImgSelector).Count, "default favicon is not exist");
            SetFavicon("icon.svg", false);
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".ico") != -1,
                "upload: invalid format1");
            VerifyAreEqual(BaseUrl + "pictures/favicon.ico", Driver.FindElement(faviconSelector).GetAttribute("href"),
                "default href");

            GoToIconSettings();
            VerifyAreEqual(0, Driver.FindElements(faviconImgSelector).Count, "upload: invalid format1 in admin");
            SetFavicon("icon.jpg", false);
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".ico") != -1,
                "upload: invalid format2");
            VerifyAreEqual(BaseUrl + "pictures/favicon.ico", Driver.FindElement(faviconSelector).GetAttribute("href"),
                "default href");

            GoToIconSettings();
            VerifyAreEqual(0, Driver.FindElements(faviconImgSelector).Count, "upload: invalid format2 in admin");
            SetFavicon("icon.png");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".png") != -1,
                "upload: valid format png");
            VerifyAreEqual("image/png", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type png");
            //check other page
            GoToClient("lp/testfunnel_1/funnelpage2?inplace=false");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".png") != -1,
                "upload: valid format png");
            VerifyAreEqual("image/png", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type png");


            GoToIconSettings();
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".png") != -1,
                "upload: valid format png in admin");
            SetFavicon("icon.ico");
            VerifyAreEqual("image/x-icon", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type ico");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".ico") != -1,
                "upload: valid format ico");

            GoToIconSettings();
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".ico") != -1,
                "upload: valid format ico in admin");
            SetFavicon("icon.gif");
            VerifyAreEqual("image/gif", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type gif");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".gif") != -1,
                "upload: valid format gif");

            GoToIconSettings();
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "upload: valid format gif in admin");

            //удалить
            DeleteFavicon();
        }

        [Test]
        public void UploadFaviconByUrlNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work, cause task 23718");

            GoToClient(siteUrl);
            VerifyAreEqual("image/ico", Driver.FindElement(faviconSelector).GetAttribute("type"), "default type");
            VerifyAreEqual(BaseUrl + "pictures/favicon.ico", Driver.FindElement(faviconSelector).GetAttribute("href"),
                "default href");

            GoToIconSettings();
            VerifyAreEqual(0, Driver.FindElements(faviconImgSelector).Count, "default favicon is not exist");
            SetFaviconByHref("https://www.advantshop.net/content/favicon.svg", false);
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".ico") != -1,
                "upload: invalid format1");
            VerifyAreEqual(BaseUrl + "pictures/favicon.ico", Driver.FindElement(faviconSelector).GetAttribute("href"),
                "default href");

            GoToIconSettings();
            VerifyAreEqual(0, Driver.FindElements(faviconImgSelector).Count, "upload: invalid format1 in admin");
            SetFaviconByHref("https://www.advantshop.net/content/opinion/images/antufev.jpg", false);
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".ico") != -1,
                "upload: invalid format2");
            VerifyAreEqual(BaseUrl + "pictures/favicon.ico", Driver.FindElement(faviconSelector).GetAttribute("href"),
                "default href");

            GoToIconSettings();
            VerifyAreEqual(0, Driver.FindElements(faviconImgSelector).Count, "upload: invalid format2 in admin");
            SetFaviconByHref("https://www.advantshop.net/xsmall.png");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".png") != -1,
                "upload: valid format png");
            VerifyAreEqual("image/png", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type png");
            //check other page
            GoToClient("lp/testfunnel_1/funnelpage2?inplace=false");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".png") != -1,
                "upload: valid format png");
            VerifyAreEqual("image/png", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type png");

            GoToIconSettings();
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".png") != -1,
                "upload: valid format png in admin");
            SetFaviconByHref("https://ru.wikipedia.org/static/favicon/wikipedia.ico");
            VerifyAreEqual("image/x-icon", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type ico");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".ico") != -1,
                "upload: valid format ico");

            GoToIconSettings();
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".ico") != -1,
                "upload: valid format ico in admin");
            SetFaviconByHref(
                "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Rotating_earth_%28large%29.gif/274px-Rotating_earth_%28large%29.gif");
            VerifyAreEqual("image/gif", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type gif");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".gif") != -1,
                "upload: valid format gif");

            GoToIconSettings();
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".gif") != -1,
                "upload: valid format gif in admin");

            //удалить
            DeleteFavicon();
        }

        [Test]
        public void UploadFaviconFromGalleryNOTCOMPLETE()
        {
            VerifyIsTrue(false, "file format is Jpeg, but jpeg is not valid format for favicon - cause task 23718");
            By faviconSelector = By.CssSelector("link[rel=\"shortcut icon\"]");

            GoToIconSettings();
            Driver.FindElement(AdvBy.DataE2E("GalleryCloud")).Click();
            Driver.WaitForElem(By.ClassName("gallery-cloud-modal"));

            //try search by keys
            string previousSrc = Driver.FindElements(By.ClassName("gallery-cloud__list-item-img"))[0]
                .GetAttribute("src");
            Driver.SendKeysInput(By.ClassName("gallery-cloud-search__input"), "worker");
            Thread.Sleep(500);
            VerifyAreNotEqual(previousSrc,
                Driver.FindElements(By.ClassName("gallery-cloud__list-item-img"))[0].GetAttribute("src"),
                "picture after search");

            //try search by category
            previousSrc = Driver.FindElements(By.ClassName("gallery-cloud__list-item-img"))[0].GetAttribute("src");
            Driver.FindElement(By.ClassName("gallery-cloud-search__list-item")).Click();
            Thread.Sleep(500);
            VerifyAreNotEqual(previousSrc,
                Driver.FindElements(By.ClassName("gallery-cloud__list-item-img"))[0].GetAttribute("src"),
                "picture after category click");

            Driver.FindElements(By.ClassName("gallery-cloud__list-item-img"))[0].Click();
            Driver.WaitForElem(By.Id("cropImg"));
            Driver.FindElement(By.ClassName("blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(100);

            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".jpeg") != -1,
                "upload: valid format png");
            VerifyAreEqual("image/jpeg", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "favicon from gallery");
            //check other page
            GoToClient("lp/testfunnel_1/funnelpage2?inplace=false");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".png") != -1,
                "upload: valid format png");
            VerifyAreEqual("image/png", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type png");

            GoToIconSettings();
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".jpeg") != -1,
                "upload: valid format gif in admin");

            //удалить
            DeleteFavicon();
        }

        [Test]
        public void FaviconLazyLoadEnabledNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work cause task 23718, p6");
        }

        [Test]
        public void DeleteFaviconNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work cause not exist confirm modal - task 23718, p4or5");

            //cancel and confirm
            GoToIconSettings();
            SetFavicon("icon.ico");
            VerifyAreEqual("image/x-icon", Driver.FindElement(faviconSelector).GetAttribute("type"),
                "upload: valid type ico");
            VerifyIsTrue(Driver.FindElement(faviconSelector).GetAttribute("href").IndexOf(".ico") != -1,
                "upload: valid format ico");

            GoToIconSettings();
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".ico") != -1,
                "upload: valid format ico in admin");

            Driver.FindElement(AdvBy.DataE2E("deletePicture")).Click();
            Driver.SwalCancel();
            Thread.Sleep(100);
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".ico") != -1,
                "upload: valid format ico in admin");
            Refresh();

            GoToIconSettings();
            VerifyIsTrue(Driver.FindElement(faviconImgSelector).GetAttribute("src").IndexOf(".ico") != -1,
                "upload: valid format ico in admin");
            Driver.FindElement(AdvBy.DataE2E("deletePicture")).Click();
            Driver.SwalConfirm();
            Thread.Sleep(500);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("link[rel=\"shortcut icon\"]")).GetAttribute("href")
                    .IndexOf("/favicon.ico") != -1, "upload: removed favicon");
            GoToIconSettings();
            VerifyAreEqual(0, Driver.FindElements(faviconImgSelector).Count, "default favicon is not exist");
        }

        public void GoToIconSettings()
        {
            GoToClient(siteUrl);
            APItemSettingsClick();
            Driver.FindElement(By.Id("tabHeaderFav")).Click();
            Driver.WaitForElem(By.CssSelector(".tab-content-active [data-e2e=\"LoadPicture\"]"));
        }

        public void DeleteFavicon()
        {
            Driver.FindElement(AdvBy.DataE2E("deletePicture")).Click();
            Driver.SwalConfirm();
            Thread.Sleep(500);
        }

        public void SetFavicon(string imgName, bool expectedSuccess = true)
        {
            try
            {
                AttachFile(By.XPath($"(//input[@type='file'][@data-e2e='LoadPicture'])"), GetPicturePath(imgName));
                if (expectedSuccess)
                {
                    Driver.WaitForElem(By.Id("cropImg"));
                    Driver.FindElement(By.ClassName("blocks-constructor-btn-confirm")).Click();
                }
                else
                {
                    VerifyIsTrue(false, "TEST NOT COMPLETE https://task.advant.me/adminv3/tasks#?modal=23718");
                    Driver.WaitForElem(By.ClassName("toast-error"));
                    VerifyIsTrue(
                        Driver.FindElement(By.ClassName("toast-message")).Text.IndexOf("Неверный формат") != -1,
                        "error message");
                    Driver.FindElement(By.CssSelector(".toast-error .toast-close-button")).Click();
                }

                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                VerifyAddErrors(ex.Message);
                Refresh();
            }
        }

        public void SetFaviconByHref(string urlPath, bool expectedSuccess = true)
        {
            try
            {
                Driver.FindElement(AdvBy.DataE2E("uploadByUrl")).Click();
                Driver.WaitForElem(AdvBy.DataE2E("UrlUpload"));
                Driver.SendKeysInput(AdvBy.DataE2E("UrlUpload"), urlPath);
                Driver.FindElement(AdvBy.DataE2E("uploadUrlBtn")).Click();
                if (expectedSuccess)
                {
                    Driver.WaitForElem(By.Id("cropImg"));
                    Driver.FindElement(By.ClassName("blocks-constructor-btn-confirm")).Click();
                }
                else
                {
                    VerifyIsTrue(false, "TEST NOT COMPLETE https://task.advant.me/adminv3/tasks#?modal=23718");
                    Driver.WaitForElem(By.ClassName("toast-error"));
                    VerifyIsTrue(
                        Driver.FindElement(By.ClassName("toast-message")).Text
                            .IndexOf("Некорректный формат изображения") != -1, "error message");
                    Driver.FindElement(By.CssSelector(".toast-error .toast-close-button")).Click();
                }

                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                VerifyIsTrue(false, ex.Message);
                Refresh();
            }
        }
    }
}