using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSystem
{
    [TestFixture]
    public class SettingsSystemCommonTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Tasks);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingSystem\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingSystem\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingSystem\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingSystem\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingSystem\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingSystem\\Customers.TaskGroup.csv",
                "Data\\Admin\\Settings\\SettingSystem\\Customers.Task.csv"
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
        public void SettingAltTeg()
        {
            GoToAdmin("settings/common");

            Driver.FindElement(By.Id("LogoImgAlt")).Clear();
            Driver.FindElement(By.Id("LogoImgAlt")).SendKeys("Test Alt");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"] input")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyAreEqual("Test Alt", Driver.FindElement(By.Id("logo")).GetAttribute("alt"), "alt teg ");

            GoToAdmin("settings/common");

            Driver.FindElement(By.Id("LogoImgAlt")).Clear();
            Driver.FindElement(By.Id("LogoImgAlt")).SendKeys("New Test Alt");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"] input")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyAreEqual("New Test Alt", Driver.FindElement(By.Id("logo")).GetAttribute("alt"), " new alt teg ");
        }

        [Test]
        public void SettingDataTimeFull()
        {
            GoToAdmin("settingssystem");

            Driver.FindElement(By.Id("AdminDateFormat")).Clear();
            Driver.FindElement(By.Id("AdminDateFormat")).SendKeys("dd.MM.yy");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToAdmin("orders");
            VerifyAreEqual("10.04.17", Driver.GetGridCell(0, "OrderDateFormatted").Text, "created date orders 1");

            Functions.KanbanOff(Driver, BaseUrl, "tasks");
            VerifyAreEqual("12.11.16", Driver.GetGridCell(0, "DateAppointedFormatted").Text, "created date tasks 1");


            GoToAdmin("settingssystem");
            Driver.FindElement(By.Id("AdminDateFormat")).Clear();
            Driver.FindElement(By.Id("AdminDateFormat")).SendKeys("dd.MM.yyyy HH:mm:ss");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToAdmin("orders");
            VerifyAreEqual("10.04.2017 09:45:17", Driver.GetGridCell(0, "OrderDateFormatted").Text,
                "created date orders 3");
            GoToAdmin("tasks");
            VerifyAreEqual("12.11.2016 13:58:00", Driver.GetGridCell(0, "DateAppointedFormatted").Text,
                "created date tasks 3");

            GoToAdmin("settingssystem");
            Driver.FindElement(By.Id("AdminDateFormat")).Clear();
            Driver.FindElement(By.Id("AdminDateFormat")).SendKeys("dd MMMM");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToAdmin("orders");
            VerifyAreEqual("10 апреля", Driver.GetGridCell(0, "OrderDateFormatted").Text, "created date orders 4");
            GoToAdmin("tasks");
            VerifyAreEqual("12 ноября", Driver.GetGridCell(0, "DateAppointedFormatted").Text, "created date tasks 4");

            GoToAdmin("settingssystem");
            Driver.FindElement(By.Id("AdminDateFormat")).Clear();
            Driver.FindElement(By.Id("AdminDateFormat")).SendKeys("dd.MM.yyyy HH:mm");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToAdmin("orders");
            VerifyAreEqual("10.04.2017 09:45", Driver.GetGridCell(0, "OrderDateFormatted").Text,
                "created date orders 2");
            GoToAdmin("tasks");
            VerifyAreEqual("12.11.2016 13:58", Driver.GetGridCell(0, "DateAppointedFormatted").Text,
                "created date tasks 2");
        }

        [Test]
        public void SettingInplace()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("EnableInplace", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            Driver.MouseFocus(By.Id("logo"));
            Driver.MouseFocus(By.ClassName("email"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".inplace-buttons")).Count > 0, "inplace-buttons ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".static-block.cke_editable")).Count > 0, " cke-editable");

            GoToClient("products/test-product1?tab=tabOptions");
            Driver.WaitForElem(By.ClassName("details-block"));
            Driver.MouseFocus(By.CssSelector(".gallery-picture-obj"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".inplace-buttons")).Count > 0, " inplace-buttons in cart");
            VerifyIsTrue(Driver.PageSource.Contains("Редактировать товар через панель администрирования"),
                "product admin edit in cart ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".inplace-properties-new-wrap")).Count > 0,
                " cke-editable in cart");


            GoToAdmin("settingstemplate");

            Functions.CheckNotSelected("EnableInplace", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            Driver.MouseFocus(By.Id("logo"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".inplace-buttons")).Count == 0, "no inplace-buttons ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".static-block.cke_editable")).Count == 0,
                "no cke-editable");

            GoToClient("products/test-product1?tab=tabOptions");
            Driver.WaitForElem(By.ClassName("details-block"));
            Driver.MouseFocus(By.CssSelector(".gallery-picture-obj"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".inplace-buttons")).Count == 0,
                "no inplace-buttons in cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".inplace-properties-new-wrap")).Count == 0,
                " cke-editable in cart");
        }

        [Test]
        public void SettingFooter()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("DisplayToolBarBottom", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".toolbar-bottom")).Count == 0, " no footer ");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".col-xs-5.toolbar-bottom-links")).Count == 0,
                " site-footer");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".col-xs.toolbar-bottom-inplace")).Count == 0,
                " site-footer");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".col-xs.toolbar-bottom-cart")).Count == 0,
                " site-footer");

            GoToClient("products/test-product1");
            Driver.WaitForElem(By.ClassName("details-block"));
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".toolbar-bottom")).Count == 0, " no footer ");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".col-xs-5.toolbar-bottom-links")).Count == 0,
                " site-footer in cart");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".col-xs.toolbar-bottom-inplace")).Count == 0,
                " site-footer in cart");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".col-xs.toolbar-bottom-cart")).Count == 0,
                " site-footer in cart");


            GoToAdmin("settingstemplate");

            Functions.CheckNotSelected("DisplayToolBarBottom", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".toolbar-bottom")).Count == 0, " no footer ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs-5.toolbar-bottom-links")).Count == 0,
                " no site-footer");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs.toolbar-bottom-inplace")).Count == 0,
                " no site-footer");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs.toolbar-bottom-cart")).Count == 0,
                " no site-footer");

            GoToClient("products/test-product1");
            Driver.WaitForElem(By.ClassName("details-block"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".toolbar-bottom")).Count == 0, " no footer in cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs-5.toolbar-bottom-links")).Count == 0,
                " no site-footer in cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs.toolbar-bottom-inplace")).Count == 0,
                " no site-footer in cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs.toolbar-bottom-cart")).Count == 0,
                " no site-footer in cart");
        }

        [Test]
        public void SettingDisplayCityAuto()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("DisplayCityInTopPanel", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".toolbar-top")).Text.Contains("Ваш город:"),
                "display city ");

            GoToAdmin("settingstemplate");

            Functions.CheckNotSelected("DisplayCityInTopPanel", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".toolbar-top")).Text.Contains("Ваш город:"),
                " no display city ");
        }

        [Test]
        public void SettingCopyright()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("ShowCopyright", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".footer-bottom-level-inner")).Text
                    .Contains("Работает на advantshop.net"), "display Copyright ");

            GoToAdmin("settingstemplate");

            Functions.CheckNotSelected("ShowCopyright", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToAdmin(
                "staticblock#?grid=%7B%22paginationCurrentPage%22:1,%22paginationPageSize%22:10,%22search%22:%22%D0%BF%D0%BE%D0%B4%D0%B2%D0%B0%D0%BB%D0%B0%22%7D");
            Driver.GetGridCell(0, "Key").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            Driver.SetCkText("Test Copyright", "editor1");
            Driver.XPathContainsText("span", "Сохранить");
            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".footer-bottom-level-inner")).Text
                    .Contains("Работает на advantshop.net"), "display old Copyright 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".footer-bottom-level-inner")).Text.Contains("Test Copyright"),
                "no display Copyright 1");

            GoToAdmin(
                "staticblock#?grid=%7B%22paginationCurrentPage%22:1,%22paginationPageSize%22:10,%22search%22:%22%D0%BF%D0%BE%D0%B4%D0%B2%D0%B0%D0%BB%D0%B0%22%7D");
            Driver.GetGridCell(0, "Key").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            Driver.SetCkText(" ", "editor1");
            Driver.XPathContainsText("span", "Сохранить");
            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".footer-bottom-level-inner")).Text
                    .Contains("Работает на advantshop.net"), "display old Copyright 2");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".footer-bottom-level-inner")).Text.Contains("Test Copyright"),
                "no display Copyright 2");
        }

        [Test]
        public void SettingRecalc()
        {
            GoToAdmin("settingssystem");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"StorageRecalc\"]")).Click();
                Thread.Sleep(500);
                VerifyIsTrue(Driver.FindElement(By.Id("toast-container")).Displayed, "display modal ");
            }
            catch
            {
            }

            GoToAdmin("settingssystem");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"StorageRecalc\"]")).Displayed,
                "no display button ");
        }

        [Test]
        public void SettingStoreClosed()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("IsStoreClosed", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".storeclosed-text-align")).Displayed, "store close");
            ReInitClient();

            GoToClient();
            Driver.WaitForElem(By.ClassName("closed-store__title"));
            VerifyIsTrue(Driver.Url.Contains("closed"), "store close url");
            VerifyIsTrue(Driver.PageSource.Contains("Ведутся технические работы."), "store close client");
            ReInit();
            GoToAdmin("settingstemplate");

            Functions.CheckNotSelected("IsStoreClosed", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".storeclosed-text-align")).Count == 0, "store close no");
            ReInitClient();

            GoToClient();
            Driver.WaitForElem(By.ClassName("site-head-logo-block"));
            VerifyIsFalse(Driver.Url.Contains("closed"), "store close url");
            VerifyIsFalse(Driver.PageSource.Contains("Ведутся технические работы."), "store close client no");
            ReInit();
        }
    }

    [TestFixture]
    public class SettingsSystemCommonCaptchaTest : BaseSeleniumTest
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
            GoToAdmin("settingssystem");
            Functions.CheckNotSelected("EnableCaptchaInCheckout", Driver);
            Functions.CheckNotSelected("EnableCaptchaInRegistration", Driver);
            Functions.CheckNotSelected("EnableCaptchaInPreOrder", Driver);
            Functions.CheckNotSelected("EnableCaptchaInGiftCerticate", Driver);
            Functions.CheckNotSelected("EnableCaptchaInFeedback", Driver);
            Functions.CheckNotSelected("EnableCaptcha", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Functions.CheckNotSelected("IsRequiredZip", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }
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
        public void CaptchaInCheckout()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("DisplayCityInTopPanel", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToAdmin("settingssystem");

            Functions.CheckSelected("EnableCaptchaInCheckout", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            Functions.ProductToCart(Driver, BaseUrl, "/products/test-product22");
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaSource_CaptchaImage")).Displayed, "captcha in checkout ");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaCode")).Displayed, "captcha field in checkout ");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(500);
            VerifyIsFalse(Driver.Url.Contains("checkout/success"), " url checkout fail");
            Driver.FindElement(By.Id("CaptchaCode")).SendKeys("test");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("toast-error"));
            VerifyIsTrue(Driver.FindElement(By.Id("toast-container")).Displayed, "captcha error in checkout ");
            VerifyIsFalse(Driver.Url.Contains("checkout/success"), " url checkout fail 2 ");

            GoToClient("giftcertificate");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0, "no captcha in gift 1");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, "no captcha field in gift 1");

            GoToAdmin("settingssystem");

            Functions.CheckNotSelected("EnableCaptchaInCheckout", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            Functions.ProductToCart(Driver, BaseUrl, "/products/test-product22");
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0,
                "no captcha in checkout ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, "no captcha field in checkout ");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout fail 2 ");

            GoToClient("giftcertificate");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0, "no captcha in gift ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, "no captcha field in gift ");
        }

        [Test]
        public void CaptchaInFeedBack()
        {
            GoToAdmin("settingssystem");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"EnableCaptchaInCheckout\"]"));
            Functions.CheckSelected("EnableCaptchaInFeedback", Driver);
            Driver.ScrollToTop();
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient("feedback");
            VerifyAreEqual("Отправить сообщение", Driver.FindElement(By.TagName("h1")).Text, "h1 pre order");

            Driver.FindElement(By.Id("Message")).Clear();
            Driver.FindElement(By.Id("Message")).SendKeys("Test Message");
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("TestName");
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("9998887766");
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("test@mail.mail");
            Driver.ScrollTo(By.Id("Email"));
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaSource_CaptchaImage")).Displayed, "captcha in feedback ");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaCode")).Displayed, "captcha field in feedback ");

            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(500);

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".feedbackSuccess-block")).Count == 0, " feedback fail");
            Driver.FindElement(By.Id("CaptchaCode")).SendKeys("test");
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Driver.WaitForElem(By.ClassName("toast-error"));
            VerifyIsTrue(Driver.FindElement(By.Id("toast-container")).Displayed, "captcha error in м ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".feedbackSuccess-block")).Count == 0, " feedback fail 2 ");

            GoToAdmin("settingssystem");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"EnableCaptchaInCheckout\"]"));
            Functions.CheckNotSelected("EnableCaptchaInFeedback", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient("feedback");
            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0,
                "no captcha in feedback ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, "no captcha field in feedback ");
        }

        [Test]
        public void CaptchaInGift()
        {
            GoToAdmin("settingssystem");

            Functions.CheckSelected("EnableCaptchaInGiftCerticate", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient("giftcertificate");

            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaSource_CaptchaImage")).Displayed, "captcha in gift ");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaCode")).Displayed, "captcha field in gift ");

            Driver.FindElement(By.Id("NameTo")).SendKeys("TestTo");
            Driver.FindElement(By.Id("NameFrom")).SendKeys("TestFrom");
            Driver.FindElement(By.Id("Sum")).Clear();
            Driver.FindElement(By.Id("Sum")).SendKeys("1000");
            Driver.FindElement(By.Id("Message")).SendKeys("test");
            Driver.FindElement(By.Id("EmailTo")).SendKeys("test@test.test");
            Driver.FindElement(By.Id("EmailFrom")).Clear();
            Driver.FindElement(By.Id("EmailFrom")).SendKeys("test1@test.test");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Thread.Sleep(500);
            VerifyIsFalse(Driver.Url.Contains("checkout/success"), " url checkout fail");
            Driver.FindElement(By.Id("CaptchaCode")).SendKeys("test");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Driver.WaitForElem(By.ClassName("toast-error"));
            VerifyIsTrue(Driver.FindElement(By.Id("toast-container")).Displayed, "captcha error in gift ");
            VerifyIsFalse(Driver.Url.Contains("checkout/success"), " url checkout fail 2 ");

            GoToAdmin("settingssystem");

            Functions.CheckNotSelected("EnableCaptchaInGiftCerticate", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient("giftcertificate");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0, "no captcha in gift ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, "no captcha field in gift ");
        }

        [Test]
        public void CaptchaInPreOrder()
        {
            GoToAdmin("settingssystem");

            Functions.CheckSelected("EnableCaptchaInPreOrder", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient("products/test-product100");
            Driver.XPathContainsText("a", "Под заказ");
            VerifyIsTrue(Driver.Url.Contains("preorder/100"), "url pre order");
            VerifyAreEqual("Оформление под заказ - TestProduct100", Driver.FindElement(By.TagName("h1")).Text,
                "h1 pre order");
            VerifyAreEqual("TestProduct100", Driver.FindElement(By.CssSelector(".h1")).Text, "h1 pre order product");

            Driver.FindElement(By.Id("firstName")).Click();
            Driver.FindElement(By.Id("firstName")).Clear();
            Driver.FindElement(By.Id("firstName")).SendKeys("TestName");

            Driver.FindElement(By.Id("lastName")).Click();
            Driver.FindElement(By.Id("lastName")).Clear();
            Driver.FindElement(By.Id("lastName")).SendKeys("TestLastName");
            Driver.FindElement(By.Id("email")).Click();
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test@mail.mail");
            Driver.ScrollTo(By.Id("comment"));
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaSource_CaptchaImage")).Displayed, "captcha in preorder ");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaCode")).Displayed, "captcha field in preorder ");

            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Thread.Sleep(500);

            VerifyAreNotEqual("Покупка товара под заказ", Driver.FindElement(By.TagName("h1")).Text,
                " no h1 pre order");
            Driver.FindElement(By.Id("CaptchaCode")).SendKeys("test");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Driver.WaitForElem(By.ClassName("toast-error"));
            VerifyIsTrue(Driver.FindElement(By.Id("toast-container")).Displayed, "captcha error in preorder ");
            VerifyAreNotEqual("Покупка товара под заказ", Driver.FindElement(By.TagName("h1")).Text,
                " no h1 pre order1");

            GoToAdmin("settingssystem");

            Functions.CheckNotSelected("EnableCaptchaInPreOrder", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToClient("products/test-product100");
            Driver.XPathContainsText("a", "Под заказ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0,
                "no captcha in preorder ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, "no captcha field in preorder ");
        }

        //оставлен функционал на будущее
        /*
        [Test]
        public void CaptchaInPage()
        {
            GoToAdmin("settingssystem");

            Functions.checkSelected("EnableCaptcha", driver);
            try
            {
              Driver.ScrollToTop();
                driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch { }

            GoToClient("feedback");
            VerifyAreEqual("Отправить сообщение", driver.FindElement(By.TagName("h1")).Text, "h1 pre order");

            driver.FindElement(By.Id("Message")).Clear();
            driver.FindElement(By.Id("Message")).SendKeys("Test Message");
            driver.FindElement(By.Id("Name")).Clear();
            driver.FindElement(By.Id("Name")).SendKeys("TestName");
            driver.FindElement(By.Id("Phone")).Clear();
            driver.FindElement(By.Id("Phone")).SendKeys("999888");
            driver.FindElement(By.Id("Email")).Clear();
            driver.FindElement(By.Id("Email")).SendKeys("test@mail.mail");
              Driver.ScrollTo(By.Id("Email"));
            VerifyIsTrue(driver.FindElement(By.Id("CaptchaSource_CaptchaImage")).Displayed, "captcha in feedback ");
            VerifyIsTrue(driver.FindElement(By.Id("CaptchaCode")).Displayed, "captcha field in feedback ");

            driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(500);

            VerifyIsTrue(driver.FindElements(By.CssSelector(".feedbackSuccess-block")).Count == 0, " feedback fail");
            driver.FindElement(By.Id("CaptchaCode")).SendKeys("test");
            driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Driver.WaitForElem(By.ClassName("toast-error"));
            VerifyIsTrue(driver.FindElement(By.Id("toast-container")).Displayed, "captcha error in м ");
            VerifyIsTrue(driver.FindElements(By.CssSelector(".feedbackSuccess-block")).Count == 0, " feedback fail 2 ");

            GoToAdmin("settingssystem");

            Functions.checkNotSelected("EnableCaptcha", driver);
            try
            {
              Driver.ScrollToTop();
                driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch { }

            GoToClient("feedback");
            VerifyIsTrue(driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0, "no captcha in feedback ");
            VerifyIsTrue(driver.FindElements(By.Id("CaptchaCode")).Count == 0, "no captcha field in feedback ");
        }*/
        [Test]
        public void CaptchaInRegistration()
        {
            GoToAdmin("settingssystem");

            Functions.CheckSelected("EnableCaptchaInRegistration", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            ReInitClient();
            GoToClient("registration");
            Refresh();
            Driver.FindElement(By.Id("FirstName")).Clear();
            Driver.FindElement(By.Id("FirstName")).SendKeys("TestName");
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("test1@mail.mail");
            Driver.FindElement(By.Id("LastName")).Clear();
            Driver.FindElement(By.Id("LastName")).SendKeys("LastName");
            Driver.FindElement(By.Id("Phone")).Clear();
            Driver.FindElement(By.Id("Phone")).SendKeys("79009999999");
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("123123");
            Driver.FindElement(By.Id("PasswordConfirm")).Clear();
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("123123");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaSource_CaptchaImage")).Displayed, "captcha in registration ");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaCode")).Displayed, "captcha field in registration ");
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(500);

            VerifyIsFalse(Driver.Url.Contains("myaccount"), " url checkout fail");
            Driver.FindElement(By.Id("CaptchaCode")).SendKeys("test");
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(500);
            VerifyIsFalse(Driver.Url.Contains("myaccount"), " url checkout fail 2 ");

            ReInit();
            GoToAdmin("settingssystem");

            Functions.CheckNotSelected("EnableCaptchaInRegistration", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            ReInitClient();
            GoToClient("registration");
            Refresh();
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0,
                "no captcha in registration ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, "no captcha field in registration ");
            ReInit();
        }

        [Test]
        public void CaptchazAllPage()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("DisplayCityInTopPanel", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToAdmin("settingssystem");

            Functions.CheckSelected("EnableCaptchaInCheckout", Driver);
            Functions.CheckSelected("EnableCaptchaInRegistration", Driver);
            Functions.CheckSelected("EnableCaptchaInPreOrder", Driver);
            Functions.CheckSelected("EnableCaptchaInGiftCerticate", Driver);
            Functions.CheckSelected("EnableCaptchaInFeedback", Driver);
            Functions.CheckSelected("EnableCaptcha", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            Functions.ProductToCart(Driver, BaseUrl, "/products/test-product22");
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaSource_CaptchaImage")).Displayed, "captcha in checkout ");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaCode")).Displayed, "captcha field in checkout ");

            GoToClient("giftcertificate");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaSource_CaptchaImage")).Displayed,
                "captcha in giftcertificate ");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaCode")).Displayed, "captcha field in giftcertificate ");

            GoToClient("feedback");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaSource_CaptchaImage")).Displayed, "captcha in feedback ");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaCode")).Displayed, "captcha field in feedback ");

            GoToClient("products/test-product100");
            Driver.XPathContainsText("a", "Под заказ");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaSource_CaptchaImage")).Displayed, "captcha in preorder ");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaCode")).Displayed, "captcha field in preorder ");

            ReInitClient();
            GoToClient("registration");
            Refresh();
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaSource_CaptchaImage")).Displayed, "captcha in registration ");
            VerifyIsTrue(Driver.FindElement(By.Id("CaptchaCode")).Displayed, "captcha field in registration ");
            ReInit();
        }

        [Test]
        public void CaptchazNonePage()
        {
            ReInit();
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("DisplayCityInTopPanel", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            GoToAdmin("settingssystem");

            Functions.CheckNotSelected("EnableCaptchaInCheckout", Driver);
            Functions.CheckNotSelected("EnableCaptchaInRegistration", Driver);
            Functions.CheckNotSelected("EnableCaptchaInPreOrder", Driver);
            Functions.CheckNotSelected("EnableCaptchaInGiftCerticate", Driver);
            Functions.CheckNotSelected("EnableCaptchaInFeedback", Driver);
            Functions.CheckNotSelected("EnableCaptcha", Driver);
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            Functions.ProductToCart(Driver, BaseUrl, "/products/test-product22");
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0,
                " no captcha in checkout ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, " no captcha field in checkout ");

            GoToClient("giftcertificate");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0,
                " no captcha in giftcertificate ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, " no captcha field in giftcertificate ");

            GoToClient("feedback");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0,
                " no captcha in feedback ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, " no captcha field in feedback ");

            GoToClient("products/test-product100");
            Driver.XPathContainsText("a", "Под заказ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0,
                " no captcha in preorder ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, " no captcha field in preorder ");

            ReInitClient();
            GoToClient("registration");
            Refresh();
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaSource_CaptchaImage")).Count == 0,
                " no captcha in registration ");
            VerifyIsTrue(Driver.FindElements(By.Id("CaptchaCode")).Count == 0, " no captcha field in registration ");
            ReInit();
        }

        [Test]
        public void CaptchaMode()
        {
            GoToAdmin("settingssystem");

            (new SelectElement(Driver.FindElement(By.Id("CaptchaMode")))).SelectByText("Численный");
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            IWebElement selectElem1 = Driver.FindElement(By.Id("CaptchaMode"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Численный"), "check CaptchaMode");

            Refresh();
            selectElem1 = Driver.FindElement(By.Id("CaptchaMode"));
            select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Численный"), "check CaptchaMode after refresh");
        }


        [Test]
        public void CaptchaLength()
        {
            ReInit();
            GoToAdmin("settingssystem");

            Driver.FindElement(By.Id("CaptchaLength")).Clear();
            Driver.FindElement(By.Id("CaptchaLength")).SendKeys("10");
            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
                Driver.WaitForElem(By.ClassName("toast-success"));
            }
            catch
            {
            }

            VerifyAreEqual("10", Driver.FindElement(By.Id("CaptchaLength")).GetAttribute("value"), "check lenght");
            Refresh();
            VerifyAreEqual("10", Driver.FindElement(By.Id("CaptchaLength")).GetAttribute("value"), "check lenght 2");


            Driver.FindElement(By.Id("CaptchaLength")).Clear();
            Driver.FindElement(By.Id("CaptchaLength")).SendKeys("test!!!");
            Driver.ScrollToTop();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tab-warning-icon")).Displayed, "captcha error");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveSettings\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tab-warning-icon")).Displayed, "captcha error 1");

            Refresh();
            VerifyAreEqual("10", Driver.FindElement(By.Id("CaptchaLength")).GetAttribute("value"),
                "check error lenght 2");
        }
    }
}