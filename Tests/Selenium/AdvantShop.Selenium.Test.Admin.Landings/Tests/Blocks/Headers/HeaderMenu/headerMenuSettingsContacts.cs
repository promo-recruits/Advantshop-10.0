using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Headers.HeaderMenu
{
    [TestFixture]
    public class headerMenuSettingsContacts : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu \\Settings.Settings.csv"
            );
            Init();
        }

        private readonly string blockName = "headerMenu";
        private readonly string blockType = "Headers";
        private readonly int numberBlock = 1;

        public void ChangeAdminSettings()
        {
            GoToAdmin("settings/mobileversion");
            Thread.Sleep(2000);
            Driver.FindElement(By.Id("MobilePhone")).Click();
            Driver.FindElement(By.Id("MobilePhone")).Clear();
            Driver.FindElement(By.Id("MobilePhone")).SendKeys("89021236547");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();

            GoToAdmin("settingsmail"); //EmailForLeads
            Thread.Sleep(2000);
        }

        [Test]
        public void ContactsAllShow()
        {
            TestName = "ContactsAllShow";
            VerifyBegin(TestName);
            ChangeAdminSettings();

            GoToClient("lp/test1");

            AddBlockByBtnBig(blockType, blockName);

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderContact");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ShowPhone\"] input")).Selected,
                "initial phone");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ShowEmail\"] input")).Selected,
                "initial email");

            Driver.AssertCkText("+7(000)000-00-00", "editor1");
            VerifyAreEqual("89021236547",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PhoneNumber\"]")).GetAttribute("value"),
                "mobile phone number");
            Driver.AssertCkText("mail@email.mail", "editor2");
            Thread.Sleep(500);

            VerifyAreEqual("http://vk.com/advantshop",
                Driver.FindElement(By.CssSelector("input[data-e2e=\"vk_enabled\"]")).GetAttribute("value"), "vk href");
            VerifyAreEqual("http://www.facebook.com/AdVantShop.NET",
                Driver.FindElement(By.CssSelector("input[data-e2e=\"fb_enabled\"]")).GetAttribute("value"), "fb href");
            VerifyAreEqual("https://www.instagram.com/advantshop/",
                Driver.FindElement(By.CssSelector("input[data-e2e=\"instagram_enabled\"]")).GetAttribute("value"),
                "insta href");
            VerifyAreEqual("https://www.youtube.com/channel/UCR9o2YJ2sOJwuV1sR1eqaSw",
                Driver.FindElement(By.CssSelector("input[data-e2e=\"youtube_enabled\"]")).GetAttribute("value"),
                "YouTube href");
            VerifyAreEqual("https://twitter.com/AdVantShop",
                Driver.FindElement(By.CssSelector("input[data-e2e=\"twitter_enabled\"]")).GetAttribute("value"),
                "twitter href");
            VerifyAreEqual("https://telegram.org/",
                Driver.FindElement(By.CssSelector("input[data-e2e=\"telegram_enabled\"]")).GetAttribute("value"),
                "telega href");
            VerifyAreEqual("https://ok.ru/AdVantShop",
                Driver.FindElement(By.CssSelector("input[data-e2e=\"odnoklassniki_enabled\"]")).GetAttribute("value"),
                "ok href");

            BlockSettingsSave();

            ReInitClient();
            GoToMobile("lp/test1");

            VerifyAreEqual("tel:89021236547",
                Driver.FindElement(By.CssSelector(".lp-header__phone--mobile")).GetAttribute("href"),
                "mobile_phone button");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("mail@email.mail",
                Driver.FindElement(By.CssSelector(".lp-menu-header-container--open .lp-header__email")).Text,
                "email in burger");
            VerifyAreEqual("+7(000)000-00-00",
                Driver.FindElement(By.CssSelector(".lp-menu-header-container--open .lp-header__phone")).Text,
                "phone in burger");

            VerifyAreEqual("http://www.facebook.com/AdVantShop.NET",
                Driver.FindElement(By.CssSelector(".icon-lp-facebook")).GetAttribute("href"), "fb href in burger");
            VerifyAreEqual("http://vk.com/advantshop",
                Driver.FindElement(By.CssSelector(".icon-lp-vkontakte")).GetAttribute("href"), "vk href in burger");
            VerifyAreEqual("https://www.instagram.com/advantshop/",
                Driver.FindElement(By.CssSelector(".icon-lp-instagram")).GetAttribute("href"), "insta href in burger");
            VerifyAreEqual("https://www.youtube.com/channel/UCR9o2YJ2sOJwuV1sR1eqaSw",
                Driver.FindElement(By.CssSelector(".icon-lp-youtube")).GetAttribute("href"), "YouTube href in burger");
            VerifyAreEqual("https://twitter.com/AdVantShop",
                Driver.FindElement(By.CssSelector(".icon-lp-twitter")).GetAttribute("href"), "twitter href in burger");
            VerifyAreEqual("https://telegram.org/",
                Driver.FindElement(By.CssSelector(".icon-lp-telegram")).GetAttribute("href"), "telega href in burger");
            VerifyAreEqual("https://ok.ru/AdVantShop",
                Driver.FindElement(By.CssSelector(".icon-lp-odnoklassniki")).GetAttribute("href"), "ok href in burger");

            VerifyFinally(TestName);
        }

        [Test]
        public void ContactsChangeAll()
        {
            TestName = "ContactsChangeAll";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderContact");
            Driver.SetCkText("+1(222)333-44-55", "editor1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"PhoneNumber\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PhoneNumber\"]")).SendKeys("89001112233");

            Driver.SetCkText("Email", "editor2");

            Driver.FindElement(By.CssSelector("input[data-e2e=\"vk_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"vk_enabled\"]")).SendKeys("vk");

            Driver.FindElement(By.CssSelector("input[data-e2e=\"fb_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"fb_enabled\"]")).SendKeys("fb");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"odnoklassniki_enabled\"]"));

            Driver.FindElement(By.CssSelector("input[data-e2e=\"instagram_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"instagram_enabled\"]")).SendKeys("insta");

            Driver.FindElement(By.CssSelector("input[data-e2e=\"youtube_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"youtube_enabled\"]")).SendKeys("YouTube");

            Driver.FindElement(By.CssSelector("input[data-e2e=\"twitter_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"twitter_enabled\"]")).SendKeys("twitter");

            Driver.FindElement(By.CssSelector("input[data-e2e=\"telegram_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"telegram_enabled\"]")).SendKeys("telega");

            Driver.FindElement(By.CssSelector("input[data-e2e=\"odnoklassniki_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"odnoklassniki_enabled\"]")).SendKeys("ok");

            BlockSettingsSave();

            ReInitClient();
            GoToMobile("lp/test1");

            VerifyAreEqual("tel:89001112233",
                Driver.FindElement(By.CssSelector(".lp-header__phone--mobile")).GetAttribute("href"),
                "mobile_phone button after");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("Email",
                Driver.FindElement(By.CssSelector(".lp-menu-header-container--open .lp-header__email")).Text,
                "email in burger after");
            VerifyAreEqual("+1(222)333-44-55",
                Driver.FindElement(By.CssSelector(".lp-menu-header-container--open .lp-header__phone")).Text,
                "phone in burger after");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".icon-lp-facebook")).GetAttribute("href").Contains("fb"),
                "fb href in burger after");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".icon-lp-vkontakte")).GetAttribute("href").Contains("vk"),
                "vk href in burger after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-instagram")).GetAttribute("href").Contains("insta"),
                "insta href in burger after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-youtube")).GetAttribute("href").Contains("YouTube"),
                "YouTube href in burger after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-twitter")).GetAttribute("href").Contains("twitter"),
                "twitter href in burger after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-telegram")).GetAttribute("href").Contains("telega"),
                "telega href in burger after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-odnoklassniki")).GetAttribute("href").Contains("ok"),
                "ok href in burger after");

            VerifyFinally(TestName);
        }

        [Test]
        public void ContactsHideAll()
        {
            TestName = "ContactsHideAll";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderContact");
            FunctionsHeaders.HiddenAllContacts(Driver);
            BlockSettingsSave();

            ReInitClient();
            GoToMobile("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-header__phone--mobile")).Count == 0,
                "no mobile_phone button");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-facebook")).Count == 0, "no fb");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-vkontakte")).Count == 0, "no vk");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-instagram")).Count == 0, "no insta");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-youtube")).Count == 0, "no YouTube");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-twitter")).Count == 0, "no twitter");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-telegram")).Count == 0, "no telega");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-odnoklassniki")).Count == 0, "no ok");

            VerifyFinally(TestName);
        }
    }
}