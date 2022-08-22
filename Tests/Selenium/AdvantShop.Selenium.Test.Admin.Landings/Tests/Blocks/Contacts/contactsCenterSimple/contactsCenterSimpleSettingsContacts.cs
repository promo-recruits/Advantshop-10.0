using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Headers;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Contacts.contactsCenterSimple
{
    [TestFixture]
    public class contactsCenterSimpleSettingsContacts : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Contacts\\contactsCenterSimple\\Settings.Settings.csv"
            );
            Init();
        }

        private readonly string blockName = "contactsCenterSimple";
        private readonly string blockType = "Contacts";
        private readonly int numberBlock = 1;

        [Test]
        public void ContactsAllShow()
        {
            TestName = "ContactsAllShow";
            VerifyBegin(TestName);

            GoToAdmin("settingssocial");

            GoToClient("lp/test1");

            AddBlockByBtnBig(blockType, blockName);

            BlockSettingsBtn(numberBlock);
            TabSelect("tabFooterContact");
            Thread.Sleep(2000);
            FunctionsHeaders.ShowOdnoklassniki(Driver);
            FunctionsHeaders.ShowTelegram(Driver);
            FunctionsHeaders.ShowTwitter(Driver);
            FunctionsHeaders.ShowYouTube(Driver);
            FunctionsHeaders.ShowInstagram(Driver);
            FunctionsHeaders.ShowFB(Driver);
            FunctionsHeaders.ShowVK(Driver);

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
            VerifyAreEqual("http://www.facebook.com/AdVantShop.NET",
                Driver.FindElement(By.CssSelector(".icon-lp-facebook")).GetAttribute("href"), "fb href in mobile");
            VerifyAreEqual("http://vk.com/advantshop",
                Driver.FindElement(By.CssSelector(".icon-lp-vkontakte")).GetAttribute("href"), "vk href in mobile");
            VerifyAreEqual("https://www.instagram.com/advantshop/",
                Driver.FindElement(By.CssSelector(".icon-lp-instagram")).GetAttribute("href"), "insta href in mobile");
            VerifyAreEqual("https://www.youtube.com/channel/UCR9o2YJ2sOJwuV1sR1eqaSw",
                Driver.FindElement(By.CssSelector(".icon-lp-youtube")).GetAttribute("href"), "YouTube href in mobile");
            VerifyAreEqual("https://twitter.com/AdVantShop",
                Driver.FindElement(By.CssSelector(".icon-lp-twitter")).GetAttribute("href"), "twitter href in mobile");
            VerifyAreEqual("https://telegram.org/",
                Driver.FindElement(By.CssSelector(".icon-lp-telegram")).GetAttribute("href"), "telega href in mobile");
            VerifyAreEqual("https://ok.ru/AdVantShop",
                Driver.FindElement(By.CssSelector(".icon-lp-odnoklassniki")).GetAttribute("href"), "ok href in mobile");

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
            TabSelect("tabFooterContact");

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
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".icon-lp-facebook")).GetAttribute("href").Contains("fb"),
                "fb href in client after");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".icon-lp-vkontakte")).GetAttribute("href").Contains("vk"),
                "vk href in client after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-instagram")).GetAttribute("href").Contains("insta"),
                "insta href in client after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-youtube")).GetAttribute("href").Contains("YouTube"),
                "YouTube href in client after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-twitter")).GetAttribute("href").Contains("twitter"),
                "twitter href in client after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-telegram")).GetAttribute("href").Contains("telega"),
                "telega href in client after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-odnoklassniki")).GetAttribute("href").Contains("ok"),
                "ok href in client after");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".icon-lp-facebook")).GetAttribute("href").Contains("fb"),
                "fb href in mobile after");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".icon-lp-vkontakte")).GetAttribute("href").Contains("vk"),
                "vk href in mobile after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-instagram")).GetAttribute("href").Contains("insta"),
                "insta href in mobile after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-youtube")).GetAttribute("href").Contains("YouTube"),
                "YouTube href in mobile after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-twitter")).GetAttribute("href").Contains("twitter"),
                "twitter href in mobile after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-telegram")).GetAttribute("href").Contains("telega"),
                "telega href in mobile after");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".icon-lp-odnoklassniki")).GetAttribute("href").Contains("ok"),
                "ok href in mobile after");

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
            TabSelect("tabFooterContact");
            FunctionsHeaders.HiddenVK(Driver);
            FunctionsHeaders.HiddenFB(Driver);
            FunctionsHeaders.HiddenInstagram(Driver);
            FunctionsHeaders.HiddenYouTube(Driver);
            FunctionsHeaders.HiddenTwitter(Driver);
            FunctionsHeaders.HiddenTelegram(Driver);
            FunctionsHeaders.HiddenOdnoklassniki(Driver);
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-facebook")).Count == 0, "no fb in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-vkontakte")).Count == 0, "no vk in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-instagram")).Count == 0, "no insta in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-youtube")).Count == 0, "no YouTube in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-twitter")).Count == 0, "no twitter in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-telegram")).Count == 0, "no telega in client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".icon-lp-odnoklassniki")).Count == 0, "no ok in client");

            GoToMobile("lp/test1");

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