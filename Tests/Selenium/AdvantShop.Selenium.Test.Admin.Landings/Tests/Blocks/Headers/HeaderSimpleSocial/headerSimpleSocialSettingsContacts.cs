using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Headers.HeaderSimpleSocial
{
    [TestFixture]
    public class headerSimpleSocialSettingsContacts : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerSimpleSocial\\Settings.Settings.csv"
            );
            Init();
        }

        private readonly string blockName = "headerSimpleSocial";
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

            //Social Networks
            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderContact");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"ShowPhone\"] input")).Selected,
                "initial phone");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"ShowEmail\"] input")).Selected,
                "initial email is hidden");

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

            VerifyAreEqual("http://www.facebook.com/AdVantShop.NET",
                Driver.FindElement(By.CssSelector(".icon-lp-facebook")).GetAttribute("href"), "fb href in admin");
            VerifyAreEqual("http://vk.com/advantshop",
                Driver.FindElement(By.CssSelector(".icon-lp-vkontakte")).GetAttribute("href"), "vk href in admin");
            VerifyAreEqual("https://www.instagram.com/advantshop/",
                Driver.FindElement(By.CssSelector(".icon-lp-instagram")).GetAttribute("href"), "insta href in admin");
            VerifyAreEqual("https://www.youtube.com/channel/UCR9o2YJ2sOJwuV1sR1eqaSw",
                Driver.FindElement(By.CssSelector(".icon-lp-youtube")).GetAttribute("href"), "YouTube href in admin");
            VerifyAreEqual("https://twitter.com/AdVantShop",
                Driver.FindElement(By.CssSelector(".icon-lp-twitter")).GetAttribute("href"), "twitter href in admin");
            VerifyAreEqual("https://telegram.org/",
                Driver.FindElement(By.CssSelector(".icon-lp-telegram")).GetAttribute("href"), "telega href in admin");
            VerifyAreEqual("https://ok.ru/AdVantShop",
                Driver.FindElement(By.CssSelector(".icon-lp-odnoklassniki")).GetAttribute("href"), "ok href in admin");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("http://www.facebook.com/AdVantShop.NET",
                Driver.FindElement(By.CssSelector(".icon-lp-facebook")).GetAttribute("href"), "fb href in client");
            VerifyAreEqual("http://vk.com/advantshop",
                Driver.FindElement(By.CssSelector(".icon-lp-vkontakte")).GetAttribute("href"), "vk href in client");
            VerifyAreEqual("https://www.instagram.com/advantshop/",
                Driver.FindElement(By.CssSelector(".icon-lp-instagram")).GetAttribute("href"), "insta href in client");
            VerifyAreEqual("https://www.youtube.com/channel/UCR9o2YJ2sOJwuV1sR1eqaSw",
                Driver.FindElement(By.CssSelector(".icon-lp-youtube")).GetAttribute("href"), "YouTube href in client");
            VerifyAreEqual("https://twitter.com/AdVantShop",
                Driver.FindElement(By.CssSelector(".icon-lp-twitter")).GetAttribute("href"), "twitter href in client");
            VerifyAreEqual("https://telegram.org/",
                Driver.FindElement(By.CssSelector(".icon-lp-telegram")).GetAttribute("href"), "telega href in client");
            VerifyAreEqual("https://ok.ru/AdVantShop",
                Driver.FindElement(By.CssSelector(".icon-lp-odnoklassniki")).GetAttribute("href"), "ok href in client");

            GoToMobile("lp/test1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);

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

            //Email & Phone
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderContact");
            FunctionsHeaders.HiddenAllContacts(Driver);
            FunctionsHeaders.ShowPhone(Driver);
            Driver.AssertCkText("+7(000)000-00-00", "editor1");
            VerifyAreEqual("89021236547",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PhoneNumber\"]")).GetAttribute("value"),
                "mobile phone number");
            FunctionsHeaders.ShowEmail(Driver);
            Thread.Sleep(500);
            Driver.AssertCkText("mail@email.mail", "editor2");
            Thread.Sleep(500);
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"headerSimpleSocialEmail\"]")).Displayed,
                "email displayed in admin");
            VerifyAreEqual("mail@email.mail",
                Driver.FindElement(By.CssSelector("[data-e2e=\"headerSimpleSocialEmail\"]")).Text,
                "correct email in admin");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"headerSimpleSocialPhone\"]")).Displayed,
                "phone displayed in admin");
            VerifyAreEqual("+7(000)000-00-00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"headerSimpleSocialPhone\"]")).Text,
                "correct phone in admin");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"headerSimpleSocialEmail\"]")).Displayed,
                "email displayed in client");
            VerifyAreEqual("mail@email.mail",
                Driver.FindElement(By.CssSelector("[data-e2e=\"headerSimpleSocialEmail\"]")).Text,
                "correct email in client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"headerSimpleSocialPhone\"]")).Displayed,
                "phone displayed in client");
            VerifyAreEqual("+7(000)000-00-00",
                Driver.FindElement(By.CssSelector("[data-e2e=\"headerSimpleSocialPhone\"]")).Text,
                "correct phone in client");

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

            VerifyFinally(TestName);
        }

        [Test]
        public void ContactsChangeAll()
        {
            TestName = "ContactsChangeAll";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            //Change Email & Phone
            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderContact");
            Driver.SetCkText("+1(222)333-44-55", "editor1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PhoneNumber\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PhoneNumber\"]")).SendKeys("89001112233");
            Driver.SetCkText("Email", "editor2");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Email", Driver.FindElement(By.CssSelector("[data-e2e=\"headerSimpleSocialEmail\"]")).Text,
                "correct email in client");
            VerifyAreEqual("+1(222)333-44-55",
                Driver.FindElement(By.CssSelector("[data-e2e=\"headerSimpleSocialPhone\"]")).Text,
                "correct phone in client");

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

            //Change Social Networks
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabHeaderContact");
            FunctionsHeaders.HiddenAllContacts(Driver);
            FunctionsHeaders.ShowVK(Driver);
            Driver.FindElement(By.CssSelector("input[data-e2e=\"vk_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"vk_enabled\"]")).SendKeys("vk");
            FunctionsHeaders.ShowFB(Driver);
            Driver.FindElement(By.CssSelector("input[data-e2e=\"fb_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"fb_enabled\"]")).SendKeys("fb");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"odnoklassniki_enabled\"]"));
            FunctionsHeaders.ShowInstagram(Driver);
            Driver.FindElement(By.CssSelector("input[data-e2e=\"instagram_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"instagram_enabled\"]")).SendKeys("insta");
            FunctionsHeaders.ShowYouTube(Driver);
            Driver.FindElement(By.CssSelector("input[data-e2e=\"youtube_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"youtube_enabled\"]")).SendKeys("YouTube");
            FunctionsHeaders.ShowTwitter(Driver);
            Driver.FindElement(By.CssSelector("input[data-e2e=\"twitter_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"twitter_enabled\"]")).SendKeys("twitter");
            FunctionsHeaders.ShowTelegram(Driver);
            Driver.FindElement(By.CssSelector("input[data-e2e=\"telegram_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"telegram_enabled\"]")).SendKeys("telega");
            FunctionsHeaders.ShowOdnoklassniki(Driver);
            Driver.FindElement(By.CssSelector("input[data-e2e=\"odnoklassniki_enabled\"]")).Clear();
            Driver.FindElement(By.CssSelector("input[data-e2e=\"odnoklassniki_enabled\"]")).SendKeys("ok");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".icon-lp-facebook")).GetAttribute("href").Contains("fb"),
                "fb href in client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".icon-lp-vkontakte")).GetAttribute("href").Contains("vk"),
                "vk href in client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-instagram")).GetAttribute("href").Contains("insta"),
                "insta href in client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-youtube")).GetAttribute("href").Contains("YouTube"),
                "YouTube href in client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-twitter")).GetAttribute("href").Contains("twitter"),
                "twitter href in client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-telegram")).GetAttribute("href").Contains("telega"),
                "telega href in client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-odnoklassniki")).GetAttribute("href").Contains("ok"),
                "ok href in client");

            GoToMobile("lp/test1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);

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
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"headerSimpleSocialEmail\"]")).Count == 0,
                "email not displayed in admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"headerSimpleSocialPhone\"]")).Count == 0,
                "phone not displayed in admin");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-facebook")).Count == 0, "no fb in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-vkontakte")).Count == 0, "no vk in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-instagram")).Count == 0, "no insta in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-youtube")).Count == 0, "no YouTube in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-twitter")).Count == 0, "no twitter in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-telegram")).Count == 0, "no telega in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-odnoklassniki")).Count == 0, "no ok in client");

            GoToMobile("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-header__phone--mobile")).Count == 0,
                "no mobile_phone button");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-facebook")).Count == 0, "no fb in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-vkontakte")).Count == 0, "no vk in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-instagram")).Count == 0, "no insta in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-youtube")).Count == 0, "no YouTube in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-twitter")).Count == 0, "no twitter in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-telegram")).Count == 0, "no telega in mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-odnoklassniki")).Count == 0, "no ok in mobile");

            VerifyFinally(TestName);
        }
    }
}