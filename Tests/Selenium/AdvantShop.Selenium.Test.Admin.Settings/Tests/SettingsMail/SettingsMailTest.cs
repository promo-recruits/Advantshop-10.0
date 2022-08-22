using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail
{
    [TestFixture]
    public class SettingsMailNotificationsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();

            Init();

            GoToAdmin("settingsmail#?notifyTab=notifications");
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
        public void Notifications()
        {
            Driver.FindElement(By.Id("EmailForOrders")).Click();
            Driver.FindElement(By.Id("EmailForOrders")).Clear();
            Driver.FindElement(By.Id("EmailForOrders")).SendKeys("EmailForOrders@test.test");

            Driver.FindElement(By.Id("EmailForLeads")).Click();
            Driver.FindElement(By.Id("EmailForLeads")).Clear();
            Driver.FindElement(By.Id("EmailForLeads")).SendKeys("EmailForLeads@test.test");

            Driver.FindElement(By.Id("EmailForProductDiscuss")).Click();
            Driver.FindElement(By.Id("EmailForProductDiscuss")).Clear();
            Driver.FindElement(By.Id("EmailForProductDiscuss")).SendKeys("EmailForProductDiscuss@test.test");

            Driver.FindElement(By.Id("EmailForRegReport")).Click();
            Driver.FindElement(By.Id("EmailForRegReport")).Clear();
            Driver.FindElement(By.Id("EmailForRegReport")).SendKeys("EmailForRegReport@test.test");

            Driver.FindElement(By.Id("EmailForFeedback")).Click();
            Driver.FindElement(By.Id("EmailForFeedback")).Clear();
            Driver.FindElement(By.Id("EmailForFeedback")).SendKeys("EmailForFeedback@test.test");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveMailSettings\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingsmail#?notifyTab=notifications");
            VerifyAreEqual("EmailForOrders@test.test",
                Driver.FindElement(By.Id("EmailForOrders")).GetAttribute("value"), "email for orders");
            VerifyAreEqual("EmailForLeads@test.test", Driver.FindElement(By.Id("EmailForLeads")).GetAttribute("value"),
                "email for leads");
            VerifyAreEqual("EmailForProductDiscuss@test.test",
                Driver.FindElement(By.Id("EmailForProductDiscuss")).GetAttribute("value"), "email for product reviews");
            VerifyAreEqual("EmailForRegReport@test.test",
                Driver.FindElement(By.Id("EmailForRegReport")).GetAttribute("value"), "email for user sign in reports");
            VerifyAreEqual("EmailForFeedback@test.test",
                Driver.FindElement(By.Id("EmailForFeedback")).GetAttribute("value"), "email for feedback");
        }

        [Test]
        public void NotificationsPanelInfo()
        {
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".tab-pane.active .adv-panel-info")).Text
                    .Contains("Вы можете указать 2 и более получателей через символ точки с запятой"),
                "info panel text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tab-pane.active .adv-panel-info")).Displayed,
                "info panel displayed");
        }

        [Test]
        public void NotificationsSeveralMails()
        {
            Driver.FindElement(By.Id("EmailForOrders")).Click();
            Driver.FindElement(By.Id("EmailForOrders")).Clear();
            Driver.FindElement(By.Id("EmailForOrders"))
                .SendKeys("EmailForOrders1@test.test; EmailForOrders2@test.test; EmailForOrders3@test.test");

            Driver.FindElement(By.Id("EmailForLeads")).Click();
            Driver.FindElement(By.Id("EmailForLeads")).Clear();
            Driver.FindElement(By.Id("EmailForLeads"))
                .SendKeys("EmailForLeads1@test.test; EmailForLeads2@test.test; EmailForLeads3@test.test");

            Driver.FindElement(By.Id("EmailForProductDiscuss")).Click();
            Driver.FindElement(By.Id("EmailForProductDiscuss")).Clear();
            Driver.FindElement(By.Id("EmailForProductDiscuss")).SendKeys(
                "EmailForProductDiscuss1@test.test; EmailForProductDiscuss2@test.test; EmailForProductDiscuss3@test.test");

            Driver.FindElement(By.Id("EmailForRegReport")).Click();
            Driver.FindElement(By.Id("EmailForRegReport")).Clear();
            Driver.FindElement(By.Id("EmailForRegReport"))
                .SendKeys("EmailForRegReport1@test.test; EmailForRegReport2@test.test; EmailForRegReport3@test.test");

            Driver.FindElement(By.Id("EmailForFeedback")).Click();
            Driver.FindElement(By.Id("EmailForFeedback")).Clear();
            Driver.FindElement(By.Id("EmailForFeedback"))
                .SendKeys("EmailForFeedback1@test.test; EmailForFeedback2@test.test; EmailForFeedback3@test.test");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveMailSettings\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingsmail#?notifyTab=notifications");
            VerifyAreEqual("EmailForOrders1@test.test; EmailForOrders2@test.test; EmailForOrders3@test.test",
                Driver.FindElement(By.Id("EmailForOrders")).GetAttribute("value"), "email for orders");
            VerifyAreEqual("EmailForLeads1@test.test; EmailForLeads2@test.test; EmailForLeads3@test.test",
                Driver.FindElement(By.Id("EmailForLeads")).GetAttribute("value"), "email for leads");
            VerifyAreEqual(
                "EmailForProductDiscuss1@test.test; EmailForProductDiscuss2@test.test; EmailForProductDiscuss3@test.test",
                Driver.FindElement(By.Id("EmailForProductDiscuss")).GetAttribute("value"), "email for product reviews");
            VerifyAreEqual("EmailForRegReport1@test.test; EmailForRegReport2@test.test; EmailForRegReport3@test.test",
                Driver.FindElement(By.Id("EmailForRegReport")).GetAttribute("value"), "email for user sign in reports");
            VerifyAreEqual("EmailForFeedback1@test.test; EmailForFeedback2@test.test; EmailForFeedback3@test.test",
                Driver.FindElement(By.Id("EmailForFeedback")).GetAttribute("value"), "email for feedback");
        }
    }

    [TestFixture]
    public class SettingsMailEmailSettingsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();

            Init();

            Driver.Navigate().GoToUrl("https://passport.yandex.ru/auth");

            Driver.WaitForElem(By.Name("login"));
            Driver.FindElement(By.Name("login")).SendKeys("tadvantshop@yandex.ru");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector(".passp-button.passp-sign-in-button button")).Click();

            Driver.WaitForElem(By.Name("passwd"));
            Driver.FindElement(By.Name("passwd")).SendKeys("ewqEWQ321#@!");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector(".passp-button.passp-sign-in-button button")).Click();
            Thread.Sleep(500);

            //delete all letters
            Driver.Navigate().GoToUrl("https://mail.yandex.ru");
            Driver.WaitForElem(By.ClassName("mail-Layout-Content"));
            try
            {
                Driver.FindElement(By.XPath("//span[@class=\"checkbox_view\"]")).Click();
                Thread.Sleep(100);
                Driver.FindElement(
                        By.CssSelector(".mail-Toolbar-Item-Text.js-toolbar-item-title.js-toolbar-item-title-delete"))
                    .Click();
                Thread.Sleep(500);
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
        public void EmailSettingsSmtpTest()
        {
            GoToAdmin("settingsmail#?notifyTab=emailsettings");

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"UseSmtpInput\"]")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"UseSmtpMail\"]")).Click();
            }

            Driver.FindElement(By.Name("SMTP")).Click();
            Driver.FindElement(By.Name("SMTP")).Clear();
            Driver.FindElement(By.Name("SMTP")).SendKeys("smtp.yandex.ru");

            Driver.FindElement(By.Name("Port")).Click();
            Driver.FindElement(By.Name("Port")).Clear();
            Driver.FindElement(By.Name("Port")).SendKeys("25");

            Driver.FindElement(By.Name("Login")).Click();
            Driver.FindElement(By.Name("Login")).Clear();
            Driver.FindElement(By.Name("Login")).SendKeys("testmailimap@yandex.ru");

            Driver.FindElement(By.Name("Password")).Click();
            Driver.FindElement(By.Name("Password")).Clear();
            Driver.FindElement(By.Name("Password")).SendKeys("ewqEWQ321#@!");

            if (!Driver.FindElement(By.Name("SSL")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SSL\"]")).Click();
            }

            Driver.FindElement(By.Name("From")).Click();
            Driver.FindElement(By.Name("From")).Clear();
            Driver.FindElement(By.Name("From")).SendKeys("testmailimap@yandex.ru");

            Driver.FindElement(By.Name("SenderName")).Click();
            Driver.FindElement(By.Name("SenderName")).Clear();
            Driver.FindElement(By.Name("SenderName")).SendKeys("Test Sender Name");

            Driver.FindElement(By.Name("ImapHost")).Click();
            Driver.FindElement(By.Name("ImapHost")).Clear();
            Driver.FindElement(By.Name("ImapHost")).SendKeys("imap.yandex.ru");

            Driver.FindElement(By.Name("ImapPort")).Click();
            Driver.FindElement(By.Name("ImapPort")).Clear();
            Driver.FindElement(By.Name("ImapPort")).SendKeys("993");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveMailSettings\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingsmail#?notifyTab=emailsettings");
            VerifyAreEqual("smtp.yandex.ru", Driver.FindElement(By.Id("SMTP")).GetAttribute("value"), "SMTP admin");
            VerifyAreEqual("25", Driver.FindElement(By.Id("Port")).GetAttribute("value"), "Port admin smtp");
            VerifyAreEqual("testmailimap@yandex.ru", Driver.FindElement(By.Id("Login")).GetAttribute("value"),
                "Login admin smtp");
            VerifyAreEqual("testmailimap@yandex.ru", Driver.FindElement(By.Id("From")).GetAttribute("value"),
                "From field admin smtp");
            VerifyAreEqual("Test Sender Name", Driver.FindElement(By.Id("SenderName")).GetAttribute("value"),
                "SenderName admin smtp");
            VerifyAreEqual("imap.yandex.ru", Driver.FindElement(By.Id("ImapHost")).GetAttribute("value"),
                "ImapHost admin smtp");
            VerifyAreEqual("993", Driver.FindElement(By.Id("ImapPort")).GetAttribute("value"), "ImapPort admin smtp");
            VerifyIsTrue(Driver.FindElement(By.Name("SSL")).Selected, "SSl admin smtp");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"UseSmtpInput\"]")).Selected, "smtp admin");

            //check imap settings check
            Driver.FindElement(By.CssSelector("[data-e2e=\"testImap\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Проверка IMAP настроек успешно пройдена"), "smtp imap check");

            GoToAdmin("settingsmail#?notifyTab=emailsettings");
            //check sending test letter
            Driver.ScrollTo(By.Name("SenderName"));
            Driver.FindElement(By.Name("To")).Click();
            Driver.FindElement(By.Name("To")).Clear();
            Driver.FindElement(By.Name("To")).SendKeys("tadvantshop@yandex.ru");

            Driver.FindElement(By.Name("Subject")).Click();
            Driver.FindElement(By.Name("Subject")).Clear();
            Driver.FindElement(By.Name("Subject")).SendKeys("SettingsMailAddTest - IMAP Subject");

            Driver.FindElement(By.Name("Body")).Click();
            Driver.FindElement(By.Name("Body")).Clear();
            Driver.FindElement(By.Name("Body")).SendKeys("Test Letter Body Here");
            Driver.XPathContainsText("span", "Отправить");
            Thread.Sleep(1000);

            //check letter

            Driver.Navigate().GoToUrl("https://mail.yandex.ru");
            VerifyIsTrue(Driver.PageSource.Contains("SettingsMailAddTest - IMAP Subject"), "letter subject smtp");
            VerifyIsTrue(Driver.PageSource.Contains("Test Sender Name"), "letter Sender smtp");
            VerifyIsTrue(Driver.PageSource.Contains("Test Letter Body Here"), "letter Body smtp");

            //delete all letters
            Driver.FindElement(By.XPath("//span[@class=\"checkbox_view\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(
                By.CssSelector(".mail-Toolbar-Item-Text.js-toolbar-item-title.js-toolbar-item-title-delete")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.PageSource.Contains("нет писем"), "letters deleted");
        }

        [Test]
        public void EmailSettingsUniOneTest()
        {
            GoToAdmin("settingsmail#?notifyTab=emailsettings");

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"UseAdvantshopMailInput\"]")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"UseAdvantshopMail\"]")).Click();
            }

            Driver.FindElement(By.CssSelector(".m-t-sm button")).Click();

            Driver.FindElement(By.CssSelector("[type=\"email\"]")).Clear();
            Driver.FindElement(By.CssSelector("[type=\"email\"]")).SendKeys("ksyushaker@email-advantshop.ru");

            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            VerifyAreEqual("ksyushaker@email-advantshop.ru", Driver.FindElement(By.Id("Login")).GetAttribute("value"),
                "Login admin UniOne 1");
            VerifyAreEqual("ksyushaker@email-advantshop.ru",
                Driver.FindElement(By.Id("FromEmail")).GetAttribute("value"), "From field admin UniOne 1");
            VerifyAreEqual("ksyushaker", Driver.FindElement(By.Id("FromName")).GetAttribute("value"),
                "SenderName admin UniOne 1");
            VerifyAreEqual("", Driver.FindElement(By.Name("Password")).GetAttribute("value"),
                "SenderName admin UniOne 1");


            Driver.FindElement(By.Name("FromName")).Click();
            Driver.FindElement(By.Name("FromName")).Clear();
            Driver.FindElement(By.Name("FromName")).SendKeys("Shop Name");

            Driver.FindElement(By.Name("ImapHost")).Click();
            Driver.FindElement(By.Name("ImapHost")).Clear();
            Driver.FindElement(By.Name("ImapHost")).SendKeys("imap.yandex.ru");

            Driver.FindElement(By.Name("ImapPort")).Click();
            Driver.FindElement(By.Name("ImapPort")).Clear();
            Driver.FindElement(By.Name("ImapPort")).SendKeys("993");

            Driver.FindElement(By.Name("Password")).Click();
            Driver.FindElement(By.Name("Password")).Clear();
            Driver.FindElement(By.Name("Password")).SendKeys("vbforever1");

            if (!Driver.FindElement(By.Name("SSL")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SSL\"]")).Click();
            }

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveMailSettings\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingsmail#?notifyTab=emailsettings");
            VerifyAreEqual("ksyushaker@email-advantshop.ru", Driver.FindElement(By.Id("Login")).GetAttribute("value"),
                "Login admin UniOne");
            VerifyAreEqual("ksyushaker@email-advantshop.ru",
                Driver.FindElement(By.Id("FromEmail")).GetAttribute("value"), "From field admin UniOne");
            VerifyAreEqual("Shop Name", Driver.FindElement(By.Id("FromName")).GetAttribute("value"),
                "SenderName admin UniOne");
            VerifyAreEqual("imap.yandex.ru", Driver.FindElement(By.Id("ImapHost")).GetAttribute("value"),
                "ImapHost admin UniOne");
            VerifyAreEqual("993", Driver.FindElement(By.Id("ImapPort")).GetAttribute("value"), "ImapPort admin UniOne");
            VerifyIsTrue(Driver.FindElement(By.Name("SSL")).Selected, "SSl admin UniOne");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"UseAdvantshopMailInput\"]")).Selected,
                "UniOne admin");
            VerifyIsTrue(Driver.PageSource.Contains("Статус") && Driver.PageSource.Contains("подтвержден"),
                "email status admin UniOne");

            //check imap settings check
            Driver.FindElement(By.CssSelector("[data-e2e=\"testImap\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Проверка IMAP настроек успешно пройдена"), "UniOne imap check");

            //check sending test letter
            Driver.ScrollTo(By.Name("Login"));
            Driver.FindElement(By.Name("To")).Click();
            Driver.FindElement(By.Name("To")).Clear();
            Driver.FindElement(By.Name("To")).SendKeys("tadvantshop@yandex.ru");

            Driver.FindElement(By.Name("Subject")).Click();
            Driver.FindElement(By.Name("Subject")).Clear();
            Driver.FindElement(By.Name("Subject")).SendKeys("UniOne testing");

            Driver.FindElement(By.Name("Body")).Click();
            Driver.FindElement(By.Name("Body")).Clear();
            Driver.FindElement(By.Name("Body")).SendKeys("Adv Text");
            Driver.XPathContainsText("span", "Отправить");
            Thread.Sleep(1000);

            //check letter

            Driver.Navigate().GoToUrl("https://mail.yandex.ru");
            Driver.WaitForElem(By.CssSelector(".mail-MessageSnippet-Content"));
            VerifyIsTrue(Driver.PageSource.Contains("UniOne testing"), "letter subject UniOne");
            VerifyIsTrue(Driver.PageSource.Contains("Shop Name"), "letter Sender UniOne");
            VerifyIsTrue(Driver.PageSource.Contains("Adv Text"), "letter Body UniOne");

            //delete all letters
            Driver.FindElement(By.XPath("//span[@class=\"checkbox_view\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(
                By.CssSelector(".mail-Toolbar-Item-Text.js-toolbar-item-title.js-toolbar-item-title-delete")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.PageSource.Contains("нет писем"), "letters deleted");
        }
    }
}