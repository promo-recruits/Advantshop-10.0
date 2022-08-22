using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailTemplates
{
    [TestFixture]
    public class SettingsMailTemplatesAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.MailTemplate);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsMail\\MailTemplates\\Settings.MailTemplate.csv"
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
        public void MailTemplatesAdd()
        {
            GoToAdmin("settingsmail#?notifyTab=templates");

            Driver.FindElement(By.CssSelector(".tab-pane.active [data-e2e=\"btnAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"NameMailTemplate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameMailTemplate\"]")).SendKeys("New Template Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ActiveMailTemplate\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMailTemplate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMailTemplate\"]")).SendKeys("Subjet Name Added");

            Driver.SetCkText("Letter Name Template Added", "editor1");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"SortOrderMailTemplate\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailTemplate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailTemplate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailTemplate\"]")).SendKeys("777");

            Driver.FindElement(By.CssSelector("[data-e2e=\"saveMailTemplate\"]")).Click();
            Thread.Sleep(2000);

            //check admin grid
            GoToAdmin("settingsmail#?notifyTab=templates");

            VerifyAreEqual("Найдено записей: 108",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "mail Templates page count");

            Driver.GetGridIdFilter("gridTemplates", "New Template Name");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();

            VerifyAreEqual("New Template Name", Driver.GetGridCell(0, "Name", "Templates").Text,
                "mail Template added grid name");
            VerifyAreEqual("777", Driver.GetGridCell(0, "SortOrder", "Templates").Text,
                "mail Template added grid sort");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Active", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "mail Template added grid Enable");

            //check admin edit pop up
            Driver.GetGridCell(0, "_serviceColumn", "Templates")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("New Template Name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NameMailTemplate\"]")).GetAttribute("value"),
                "mail Template added pop up name");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ActiveMailTemplate\"]")).FindElement(By.TagName("input"))
                    .Selected, "mail Template added pop up enable");

            VerifyAreEqual("Subjet Name Added",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMailTemplate\"]")).GetAttribute("value"),
                "mail Template added pop up subject");
            Driver.AssertCkText("Letter Name Template Added", "editor1");
            VerifyAreEqual("777",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailTemplate\"]")).GetAttribute("value"),
                "mail Template added pop up sort");
        }

        [Test]
        public void MailTemplatesEdit()
        {
            //pre check admin grid
            GoToAdmin("settingsmail#?notifyTab=templates");

            Driver.GetGridIdFilter("gridTemplates", "Template Name Test 41");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();

            VerifyAreEqual("Template Name Test 41", Driver.GetGridCell(0, "Name", "Templates").Text,
                "pre check mail Template grid name");
            VerifyAreEqual("41", Driver.GetGridCell(0, "SortOrder", "Templates").Text,
                "pre check mail Template grid sort");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Active", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "pre check mail Template grid Enable");

            //pre check admin edit pop up
            Driver.GetGridCell(0, "_serviceColumn", "Templates")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Template Name Test 41",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NameMailTemplate\"]")).GetAttribute("value"),
                "pre check mail Template pop up name");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ActiveMailTemplate\"]")).FindElement(By.TagName("input"))
                    .Selected, "pre check mail Template pop up enable");

            VerifyAreEqual("Subject Test 41",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMailTemplate\"]")).GetAttribute("value"),
                "pre check mail Template pop up subject");
            Driver.AssertCkText("Template Text Test 41", "editor1");
            VerifyAreEqual("41",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailTemplate\"]")).GetAttribute("value"),
                "pre check mail Template pop up sort");

            //edit
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameMailTemplate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameMailTemplate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NameMailTemplate\"]")).SendKeys("Edited Template Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ActiveMailTemplate\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMailTemplate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMailTemplate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMailTemplate\"]"))
                .SendKeys("Subjet Template Name Edited");

            Driver.SetCkText("Letter Template Name Edited", "editor1");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"SortOrderMailTemplate\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailTemplate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailTemplate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailTemplate\"]")).SendKeys("999");

            Driver.FindElement(By.CssSelector("[data-e2e=\"saveMailTemplate\"]")).Click();
            Thread.Sleep(2000);

            //check admin grid
            GoToAdmin("settingsmail#?notifyTab=templates");

            Driver.GetGridIdFilter("gridTemplates", "Template Name Test 41");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "prev mail Template name count");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no prev mail Template name");

            Driver.GetGridIdFilter("gridTemplates", "Edited Template Name");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();

            VerifyAreEqual("Edited Template Name", Driver.GetGridCell(0, "Name", "Templates").Text,
                "mail Template edited grid name");
            VerifyAreEqual("999", Driver.GetGridCell(0, "SortOrder", "Templates").Text,
                "mail Template edited grid sort");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Active", "Templates")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "mail Template edited grid Enable");

            //check admin edit pop up
            Driver.GetGridCell(0, "_serviceColumn", "Templates")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Edited Template Name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NameMailTemplate\"]")).GetAttribute("value"),
                "mail Template edited pop up name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ActiveMailTemplate\"]")).FindElement(By.TagName("input"))
                    .Selected, "mail Template edited pop up enable");

            VerifyAreEqual("Subjet Template Name Edited",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMailTemplate\"]")).GetAttribute("value"),
                "mail Template edited pop up subject");
            Driver.AssertCkText("Letter Template Name Edited", "editor1");
            VerifyAreEqual("999",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailTemplate\"]")).GetAttribute("value"),
                "mail Template edited pop up sort");
        }
    }
}