using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSystem
{
    [TestFixture]
    public class SettingsSystemLicense : BaseSeleniumTest
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
        public void SettingCheckActiveLic()
        {
            GoToAdmin("settingssystem");
            VerifyAreEqual("8b40c4f4-322e-4926-ad39-d2f6d6cd10c1",
                Driver.FindElement(By.Id("LicKey")).GetAttribute("value"), "check LicKey");
            VerifyAreEqual("Активирована", Driver.FindElement(By.CssSelector("[data-e2e=\"ActiveLicense\"]")).Text,
                "check active LicKey");
        }

        [Test]
        public void SettingCheckLic()
        {
            GoToAdmin("settingssystem");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckLicKey\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".swal2-modal")).Displayed, "modal window ");

            Driver.SwalConfirm();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".toast.toast-success")).Displayed, "toast container ");

            VerifyAreEqual("Статус лицензии - активна", Driver.FindElement(By.CssSelector(".toast-title")).Text,
                "check LicKey by button");
        }
    }

    [TestFixture]
    public class SettingsSystemAnother : BaseSeleniumTest
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
        public void SettingCheckAddTag()
        {
            GoToAdmin("settingstemplate");
            VerifyAreEqual("", Driver.FindElement(By.Id("AdditionalHeadMetaTag")).Text, "check tag field");
        }

        [Test]
        public void SettingCheckTag()
        {
            GoToAdmin("settingstemplate");

            Driver.FindElement(By.Id("AdditionalHeadMetaTag")).Clear();
            Driver.FindElement(By.Id("AdditionalHeadMetaTag")).SendKeys("Test Additional Head MetaTag");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Thread.Sleep(1000);

            GoToClient();
            VerifyIsTrue(Driver.PageSource.Contains("Test Additional Head MetaTag"), "client tag ");
            VerifyIsTrue(Driver.FindElement(By.TagName("body")).Text.StartsWith("Test Additional Head MetaTag"),
                "check text");

            GoToClient("categories/test-category1");
            VerifyIsTrue(Driver.PageSource.Contains("Test Additional Head MetaTag"), "client tag in category 1");

            GoToAdmin("settingstemplate");

            Driver.FindElement(By.Id("AdditionalHeadMetaTag")).Clear();
            Driver.FindElement(By.Id("AdditionalHeadMetaTag"))
                .SendKeys("<meta name='testmeta' content='testcontent' />");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Thread.Sleep(1000);

            GoToClient();
            VerifyAreEqual("testcontent", Driver.FindElement(By.Name("testmeta")).GetAttribute("content"),
                "check text 3");

            GoToClient("categories/test-category1");
            VerifyAreEqual("testcontent", Driver.FindElement(By.Name("testmeta")).GetAttribute("content"),
                "check text 3");
            ;

            GoToAdmin("settingstemplate");

            Driver.FindElement(By.Id("AdditionalHeadMetaTag")).Clear();
            Driver.ScrollToTop();

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Thread.Sleep(1000);

            GoToClient();
            VerifyIsFalse(Driver.PageSource.Contains("Test Additional Head MetaTag"), "client no tag 2");
            VerifyIsTrue(Driver.FindElements(By.Name("testmeta")).Count == 0, "check meta ");

            GoToClient("categories/test-category1");
            VerifyIsFalse(Driver.PageSource.Contains("Test Additional Head MetaTag"), "client no tag in category 2");
            VerifyIsTrue(Driver.FindElements(By.Name("testmeta")).Count == 0, "check meta in category");
        }

        [Test]
        public void SettingCheckLink()
        {
            GoToAdmin("settingstemplate");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionalHeadTag\"]")).Enabled,
                "check tag link");
        }

        [Test]
        public void SettingCheckLinkGo()
        {
            GoToAdmin("settingstemplate");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AdditionalHeadTag\"]")).Click();
            Thread.Sleep(1000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.Url.Contains("advantshop.net/help/pages"), "check tag link");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404");
            Functions.CloseTab(Driver, BaseUrl);
        }
    }

    [TestFixture]
    public class SettingsSystemUserAgreement : BaseMultiSeleniumTest
    {
        [SetUp]
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

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void SettingShowUserAgreementCheckout()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("ShowUserAgreementText", Driver);
            Driver.SetCkText("Test User Agreement Text", "UserAgreementText");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            GoToClient();
            Refresh();
            Functions.ProductToCart(Driver, BaseUrl, "/products/test-product22");

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".checkout-agree")).Count == 1, "agree in checkout count");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-agree")).Displayed,
                "agree in checkout Displayed");
            VerifyAreEqual("Test User Agreement Text",
                Driver.FindElement(By.CssSelector(".checkout-agree .custom-input-text")).Text,
                "agree text in checkout ");

            Driver.FindElement(By.Id("Data_User_Email")).SendKeys("mail@mail.test");
            Driver.FindElement(By.Id("Data_User_FirstName")).SendKeys("Name");
            Driver.FindElement(By.Id("Data_User_LastName")).SendKeys("LastName");
            Driver.ClearInput(By.Id("Data_User_Phone"));
            Driver.FindElement(By.Id("Data_User_Phone")).SendKeys("9999999999");
            //driver.FindElement(By.Id("Data_Contact_Country")).SendKeys("Country");
            //driver.FindElement(By.Id("Data_Contact_Region")).SendKeys("Destrict");
            //driver.FindElement(By.Id("Data_Contact_City")).SendKeys("City");

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(100);
            VerifyIsFalse(Driver.Url.Contains("checkout/success"), " url checkout fail");
            Driver.FindElement(By.CssSelector(".custom-input-checkbox.custom-input-checkbox__abs")).Click();
            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout success");
        }

        [Test]
        public void SettingShowUserAgreementPreorder()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("ShowUserAgreementText", Driver);
            Driver.SetCkText("Test User Agreement Text", "UserAgreementText");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            GoToClient("preorder?offerId=100&amount=1");
            Refresh();
            Driver.FindElement(By.Id("firstName")).Clear();
            Driver.FindElement(By.Id("firstName")).SendKeys("TestName");
            Driver.FindElement(By.Id("lastName")).Clear();
            Driver.FindElement(By.Id("lastName")).SendKeys("lastName");
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test@mail.mail");
            Driver.FindElement(By.Id("phone")).SendKeys(Keys.Control + "a" + Keys.Delete);
            Driver.FindElement(By.Id("phone")).SendKeys("9999999999");
            Driver.ScrollTo(By.Id("comment"));
            VerifyAreEqual("Test User Agreement Text",
                Driver.FindElement(By.CssSelector(".form-field-input.vertical-interval-xsmall")).Text,
                "agree text in preorder ");

            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Thread.Sleep(100);
            VerifyIsFalse(Driver.Url.EndsWith("preorder"), " url checkout fail");
            Driver.FindElement(By.CssSelector(".custom-input-checkbox")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Покупка товара под заказ", Driver.FindElement(By.TagName("h1")).Text, " preorder success");
            VerifyIsTrue(Driver.Url.EndsWith("preorder"), " url checkout fail");
        }

        [Test]
        public void SettingShowUserAgreementReg()
        {
            Functions.EnableCapcha(Driver, BaseUrl);
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("ShowUserAgreementText", Driver);
            Driver.SetCkText("Test User Agreement Text", "UserAgreementText");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
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
            Driver.FindElement(By.Id("Phone")).SendKeys(Keys.Control + "a" + Keys.Delete);
            Driver.FindElement(By.Id("Phone")).SendKeys("9998887766");
            Driver.FindElement(By.Id("Password")).Clear();
            Driver.FindElement(By.Id("Password")).SendKeys("123123");
            Driver.FindElement(By.Id("PasswordConfirm")).Clear();
            Driver.FindElement(By.Id("PasswordConfirm")).SendKeys("123123");
            VerifyIsTrue(Driver.PageSource.Contains("Test User Agreement Text"), "agree text in registration ");

            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(100);

            VerifyIsFalse(Driver.Url.Contains("myaccount"), " url checkout fail");
            Driver.FindElements(By.CssSelector(".custom-input-checkbox"))[1].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains("myaccount"), " url checkout succec");

            ReInit();
        }

        [Test]
        public void SettingShowUserAgreementGift()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("ShowUserAgreementText", Driver);
            Driver.SetCkText("Test User Agreement Text", "UserAgreementText");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            GoToClient("giftcertificate");

            VerifyAreEqual("Test User Agreement Text",
                Driver.FindElement(By.CssSelector(".form-field-input.vertical-interval-xsmall")).Text,
                "agree text in giftcertificate ");

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
            Thread.Sleep(100);
            VerifyIsFalse(Driver.Url.Contains("checkout/success"), " url checkout fail");
            Driver.FindElement(By.CssSelector(".custom-input-checkbox")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout success");
        }

        [Test]
        public void SettingShowUserAgreementNone()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckNotSelected("ShowUserAgreementText", Driver);
            Driver.SetCkText("Test User Agreement Text", "UserAgreementText");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            GoToClient("giftcertificate");
            Refresh();
            VerifyIsFalse(Driver.PageSource.Contains("Test User Agreement Text"), "agree text in giftcertificate ");

            Functions.ProductToCart(Driver, BaseUrl, "/products/test-product22");
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".checkout-agree")).Count == 0, "agree in checkout count");

            GoToClient("preorder?offerId=100&amount=1");
            VerifyIsFalse(Driver.PageSource.Contains("Test User Agreement Text"), "agree text in preorder ");

            ReInitClient();
            GoToClient("registration");
            Refresh();
            VerifyIsFalse(Driver.PageSource.Contains("Test User Agreement Text"), "agree text in registration ");
        }

        [Test]
        public void SettingShowUserAgreementAuthorize()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("ShowUserAgreementText", Driver);
            Driver.SetCkText("Test User Agreement Text", "UserAgreementText");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("giftcertificate");
            Refresh();
            VerifyIsFalse(Driver.PageSource.Contains("Test User Agreement Text"), "agree text in giftcertificate ");

            Functions.ProductToCart(Driver, BaseUrl, "/products/test-product22");
            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".checkout-agree")).Count == 0, "agree in checkout count");

            GoToClient("preorder?offerId=100&amount=1");
            VerifyIsFalse(Driver.PageSource.Contains("Test User Agreement Text"), "agree text in preorder ");

            GoToClient("registration");
            Refresh();
            VerifyIsFalse(Driver.PageSource.Contains("Test User Agreement Text"), "agree text in registration ");
        }
    }

    [TestFixture]
    public class SettingsSystemCookies : BaseSeleniumTest
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
        public void SettingShowCookies()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("ShowCookiesPolicyMessage", Driver);
            Driver.SetCkText("Test Show Cookies Policy Message", "CookiesPolicyMessage");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            GoToClient("registration");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".cookies-policy-cell")).Displayed, "display messange");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".cookies-policy-cell")).Text
                    .Contains("Test Show Cookies Policy Message"), " text messange ");
            Refresh();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cookies-policy-cell")).Count == 0,
                " no display messange");
            ReInit();
        }

        [Test]
        public void SettingShowCookiesNone()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckNotSelected("ShowCookiesPolicyMessage", Driver);
            Driver.SetCkText("Test Show Cookies Policy Message", "CookiesPolicyMessage");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            GoToClient("registration");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cookies-policy-cell")).Count == 0,
                " no display messange");
            VerifyIsFalse(Driver.PageSource.Contains("Test Show Cookies Policy Message"), " no display text");
            ReInit();
        }
    }

    [TestFixture]
    public class SettingsSystemCityBubble : BaseSeleniumTest
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
        public void SettingShowCityBubble()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckSelected("DisplayCityBubble", Driver);
            Driver.SetCkText("Test Show Cookies Policy Message", "CookiesPolicyMessage");

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            GoToClient("registration");

            VerifyIsTrue(Driver.FindElement(By.Id("zonePopover")).Displayed, "display messange");
            VerifyIsTrue(Driver.PageSource.Contains("Ваш город - "), " display text");

            Driver.FindElement(By.CssSelector(".btn-buy")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElement(By.Id("zonePopover")).Displayed, " no display messange");
            ReInit();
        }

        [Test]
        public void SettingCityBubbleNone()
        {
            GoToAdmin("settingstemplate");

            Functions.CheckNotSelected("DisplayCityBubble", Driver);

            try
            {
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            ReInitClient();
            GoToClient("registration");

            VerifyIsTrue(Driver.FindElements(By.Id("zonePopover")).Count == 0, " no display messange");
            VerifyIsFalse(Driver.PageSource.Contains("Ваш город - "), " no display text");
            ReInit();
        }
    }
}