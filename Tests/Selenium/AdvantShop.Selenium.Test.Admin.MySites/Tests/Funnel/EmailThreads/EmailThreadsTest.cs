using System;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.EmailThreads
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class EmailThreadsTest : MySitesFunctions
    {
        string defaultTriggerName = "TestFunnelTrigger";
        string defaultTheme = "TestFunnelTriggerTheme";
        string defaultText = "The text of the email for the lead from the TestFunnelWithEmail";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\EmailThread\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\EmailThread\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\EmailThread\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\EmailThread\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\EmailThread\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\EmailThread\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\EmailThread\\CMS.LandingSubBlock.csv"
            );

            Init(false);

            GoToAdmin("triggers");
            Driver.FindElement(By.LinkText("Включить канал")).Click();
            Driver.WaitForElem(By.TagName("ui-modal-trigger"));

            Functions.MailSmtp(Driver, BaseUrl);

            AddEmailTrigger(2, defaultTriggerName, defaultTheme, defaultText);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            ReInit();
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test, Description("Создать триггер на добавление лида, задать имя, действие," +
            " сверить неизменяемые настройки; проверить отображение в воронке")]
        public void AddEmailThread()
        {
            GoToFunnelTab(1, "Email цепочки", AdvBy.DataE2E("AddSiteTrigger"));
            Driver.FindElement(AdvBy.DataE2E("AddSiteTrigger")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.TagName("funnel-email-sequences")).Text.Contains("Нет настроенных цепочек"),
                "default empty sequences");
            AddEmailTrigger(1, defaultTriggerName, defaultTheme, defaultText);
            VerifyIsTrue(
                Driver.FindElement(By.XPath("//div[contains(text(), 'Время срабатывания')]/following-sibling::div"))
                    .Text.Contains("Сразу"), "time delay");

            GoToFunnelTab(1, "Email цепочки", AdvBy.DataE2E("AddSiteTrigger"));
            VerifyAreEqual("TestFunnelTrigger",
                Driver.FindElement(By.CssSelector("funnel-email-sequences h3")).Text.Trim(), "trigger name in funnel");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("funnel-email-sequences table")).Text.Contains("Тема письма"),
                "header in funnel");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("funnel-email-sequences table")).Text
                    .Contains("TestFunnelTriggerTheme"), "email name in funnel");
        }

        
        [Test, Description("Изменить имя триггера, тему и текст письма, " +
            "проверить отображение в воронке; вернуть к дефолтным.")]
        public void EditEmailThread()
        {
            GoToFunnelTab(2, "Email цепочки", AdvBy.DataE2E("AddSiteTrigger"));
            VerifyAreEqual("TestFunnelTrigger",
                Driver.FindElement(By.CssSelector("funnel-email-sequences h3")).Text.Trim(), "trigger name in funnel");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("funnel-email-sequences table")).Text.Contains("Тема письма"),
                "header in funnel");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("funnel-email-sequences table")).Text
                    .Contains("TestFunnelTriggerTheme"), "email name in funnel");
            Driver.FindElement(By.CssSelector("funnel-email-sequences .fa-pencil-alt")).Click();
            Driver.WaitForElem(By.TagName("trigger-edit"));

            SetEmailTriggerProperties("NewTestTrigger", "New trigger theme", "New test letter");

            GoToFunnelTab(2, "Email цепочки", AdvBy.DataE2E("AddSiteTrigger"));
            VerifyAreEqual("NewTestTrigger",
                Driver.FindElement(By.CssSelector("funnel-email-sequences h3")).Text.Trim(), "trigger name in funnel");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("funnel-email-sequences table")).Text.Contains("Тема письма"),
                "header in funnel");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("funnel-email-sequences table")).Text.Contains("New trigger theme"),
                "email name in funnel");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("funnel-email-sequences table")).Text
                    .Contains("Подключить статистику"), "statistic in funnel");

            Driver.FindElement(By.CssSelector("funnel-email-sequences .fa-pencil-alt")).Click();
            Driver.WaitForElem(By.TagName("trigger-edit"));
            SetEmailTriggerProperties(defaultTriggerName, defaultTheme, defaultText);
        }

        [Test, Description("Заполнить в кач-ве клиента форму, проверить админку воронки и почтовый ящик")]
        public void SendEmailThread()
        {
            ReInitClient();
            GoToClient("lp/sendemailthreadfunnel");
            Driver.SendKeysInput(By.CssSelector(".lp-form__field input[type=\"email\"]"), Functions.YandexEmail);
            Driver.FindElement(By.CssSelector(".lp-form__agreement label")).Click();
            Driver.FindElement(By.CssSelector(".lp-form__submit-block .lp-btn")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("FormSuccessText"));

            Functions.SingInYandex(Driver);
            Driver.Navigate().GoToUrl("https://mail.yandex.ru/");
            try
            {
                Functions.CloseYandexMailPopups(Driver);
                Driver.FindElements(By.ClassName("mail-MessageSnippet-Item_subjectWrapper"))[0]
                    .FindElement(By.ClassName("mail-MessageSnippet-Item_threadExpand")).Click();
                Thread.Sleep(500);
                Driver.FindElement(By.CssSelector(".mail-MessageSnippet-Thread .mail-MessageSnippet-Item_body"))
                    .Click();
            }
            catch (Exception ex)
            {
                Driver.FindElement(By.ClassName("mail-MessageSnippet-Item_subjectWrapper")).Click();
            }

            Driver.WaitForElem(By.ClassName("mail-Message-Body-Content"));
            VerifyAreEqual(defaultTheme, Driver.FindElement(By.CssSelector(".mail-Message-Toolbar-Subject")).Text,
                "mail theme in email");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".mail-Message-Body-Content")).Text.Contains(defaultText),
                "mail text in email");

            Driver.FindElement(By.ClassName("ns-view-toolbar-button-delete")).Click();
            Thread.Sleep(500);
        }

        public void AddEmailTrigger(int funnelId, string triggerName, string emailTheme, string emailText)
        {
            GoToFunnelTab(funnelId, "Email цепочки", AdvBy.DataE2E("AddSiteTrigger"));
            Driver.FindElement(AdvBy.DataE2E("AddSiteTrigger")).Click();
            Driver.WaitForModal();
            SelectItem(By.CssSelector(".modal-content select"), "Новый лид");
            Driver.FindElement(By.CssSelector(".modal-content .btn-primary")).Click();
            Driver.WaitForElem(By.TagName("trigger-edit"));

            SetEmailTriggerProperties(triggerName, emailTheme, emailText);
        }

        public void SetEmailTriggerProperties(string triggerName, string emailTheme, string emailText)
        {
            Driver.FindElement(By.CssSelector("h1.simple-edit__input")).Click();
            Driver.SendKeysInput(By.CssSelector("h1.simple-edit__input"), triggerName);
            Thread.Sleep(100);
            Driver.FindElement(By.ClassName("page-name-block-text--bigger")).Click();

            Driver.ScrollTo(By.ClassName("action-item"));
            Driver.SendKeysInput(By.CssSelector(".textcomplete-wrapper input"), emailTheme);
            Driver.SetCkText(emailText, By.ClassName("action-item"));
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("trigger-edit .btn-success")).Click();
            Thread.Sleep(100);
        }
    }
}