using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadAddEditEventLetterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.MailTemplate);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\LeadLetter\\Settings.MailTemplate.csv"
            );

            Init();

            //preconditions

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
            Driver.FindElement(By.Name("SenderName")).SendKeys("Test Name");

            Driver.FindElement(By.Name("ImapHost")).Click();
            Driver.FindElement(By.Name("ImapHost")).Clear();
            Driver.FindElement(By.Name("ImapHost")).SendKeys("imap.yandex.ru");

            Driver.FindElement(By.Name("ImapPort")).Click();
            Driver.FindElement(By.Name("ImapPort")).Clear();
            Driver.FindElement(By.Name("ImapPort")).SendKeys("993");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveMailSettings\"]")).Click();
            Driver.WaitForToastSuccess();
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
        public void LetterToCustomerEvent()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("test mail event");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71231212923");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).SendKeys("testmailimap@yandex.ru");


            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            GoToAdmin("leads?salesFunnelId=-1");
            Driver.GetGridFilterTab(0, "test mail event");
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            Driver.ScrollTo(By.Id("Lead_FirstName"));
            Driver.FindElement(By.LinkText("Отправить письмо")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMail\"]")).SendKeys("Test Letter Subject");

            Driver.SetCkText("Test Letter To Customer from lead", "editor2");

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            string leadUrl = Driver.Url;

            Driver.Navigate().GoToUrl(leadUrl);

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("lead-events")).Text.Contains("Исходящее письмо"),
                "lead letter sent event");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"LeadEventType\"]"))[0].Text
                    .Contains("Test Letter Subject"), "lead letter sent subject");
            var curDate = DateTime.Now.Date.ToString("dd.MM.yyyy");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lead-time"))[0].GetAttribute("title").Contains(curDate),
                "lead letter sent date");
        }

        [Test]
        public void LetterToCustomerTemplate()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("test mail template");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+712315662923");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).SendKeys("tadvantshop@yandex.ru");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            GoToAdmin("leads?salesFunnelId=-1");
            Driver.GetGridFilterTab(0, "test mail template");
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            Driver.ScrollTo(By.Id("Lead_FirstName"));
            Driver.FindElement(By.LinkText("Отправить письмо")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMail\"]")).SendKeys("Template");

            Driver.SetCkText("Template", "editor2");
            Thread.Sleep(100);

            //check template edit link
            Driver.FindElement(By.CssSelector("[data-e2e=\"EditTemplate\"]")).Click();
            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.CssSelector(".tab-pane.active"));
            VerifyIsTrue(Driver.Url.Contains("templates"));
            VerifyAreEqual("Шаблоны ответов", Driver.FindElement(By.CssSelector(".tab-pane.active h1")).Text,
                "template tab h1");
            Functions.CloseTab(Driver, BaseUrl);

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"AnswerTemplate\"]"));
            SelectElement select = new SelectElement(selectElem);
            IList<IWebElement> allOptions = select.Options;
            VerifyIsTrue(allOptions.Count == 5, "count templates in select"); // 4 active templates + empty

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AnswerTemplate\"]")))).SelectByText(
                "Template Name Test 3");

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"AnswerTemplate\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Template Name Test 3"), "template selectec");

            //check template in letter
            Driver.AssertCkText("Template Text Test 3", "editor3");
            VerifyAreEqual("Subject Test 3",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMail\"]")).GetAttribute("value"),
                "template subject");

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            string leadUrl = Driver.Url;

            Driver.Navigate().GoToUrl(leadUrl);

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("lead-events")).Text.Contains("Исходящее письмо"),
                "lead letter sent event");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"LeadEventType\"]"))[0].Text.Contains("Subject Test 3"),
                "lead letter sent subject");
            var curDate = DateTime.Now.Date.ToString("dd.MM.yyyy");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lead-time"))[0].GetAttribute("title").Contains(curDate),
                "lead letter sent date");
        }
    }
}