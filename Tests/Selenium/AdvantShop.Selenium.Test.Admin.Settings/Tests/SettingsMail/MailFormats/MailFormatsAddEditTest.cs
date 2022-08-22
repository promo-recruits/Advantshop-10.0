using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailFormats
{
    [TestFixture]
    public class SettingsMailFormatsAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.MailFormat);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsMail\\Settings.MailFormatType.csv",
                "data\\Admin\\Settings\\SettingsMail\\Settings.MailFormat.csv"
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
        public void MailFormatsAdd()
        {
            GoToAdmin("settingsmail#?notifyTab=formats");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormatNameMailFormat\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormatNameMailFormat\"]")).SendKeys("New Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"EnableMailFormat\"]")).FindElement(By.TagName("span"))
                .Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"MailFormatTypeIdMailFormat\"]"))))
                .SelectByText("Письмо покупателю");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormatSubjectMailFormat\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormatSubjectMailFormat\"]")).SendKeys("Subjet Name Added");

            Driver.SetCkText("Theme Name Added", "editor1");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"SortOrderMailFormat\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailFormat\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailFormat\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailFormat\"]")).SendKeys("777");

            Driver.FindElement(By.CssSelector("[data-e2e=\"saveMailFormat\"]")).Click();
            Thread.Sleep(2000);

            //check admin grid
            GoToAdmin("settingsmail#?notifyTab=formats");

            VerifyAreEqual("Найдено записей: 108",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "mail formats page count");

            Driver.GetGridIdFilter("grid", "New Name");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();

            VerifyAreEqual("New Name", Driver.GetGridCell(0, "FormatName").Text, "mail format added grid name");
            VerifyAreEqual("Письмо покупателю", Driver.GetGridCell(0, "TypeName").Text, "mail format added grid type");
            VerifyAreEqual("777", Driver.GetGridCell(0, "SortOrder").Text, "mail format added grid sort");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "mail format added grid Enable");

            //check admin edit pop up
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("New Name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormatNameMailFormat\"]")).GetAttribute("value"),
                "mail format added pop up name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnableMailFormat\"]")).FindElement(By.TagName("input"))
                    .Selected, "mail format added pop up enable");

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"MailFormatTypeIdMailFormat\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Письмо покупателю"), "mail format added pop up type");

            VerifyAreEqual("Subjet Name Added",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormatSubjectMailFormat\"]")).GetAttribute("value"),
                "mail format added pop up subject");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"MailFormatTypeIdMailFormat\"]")).GetAttribute("disabled")
                    .Equals("true"), "mail format added pop up subject readonly");
            Driver.AssertCkText("Theme Name Added", "editor1");
            VerifyAreEqual("777",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailFormat\"]")).GetAttribute("value"),
                "mail format added pop up sort");
        }

        [Test]
        public void MailFormatsEdit()
        {
            //pre check admin grid
            GoToAdmin("settingsmail#?notifyTab=formats");

            Driver.GetGridIdFilter("grid", "Format Name Test 41");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();

            VerifyAreEqual("Format Name Test 41", Driver.GetGridCell(0, "FormatName").Text,
                "pre check mail format grid name");
            VerifyAreEqual("Менеджеру назначен заказ", Driver.GetGridCell(0, "TypeName").Text,
                "pre check mail format grid type");
            VerifyAreEqual("41", Driver.GetGridCell(0, "SortOrder").Text, "pre check mail format grid sort");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "pre check mail format grid Enable");

            //pre check admin edit pop up
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Format Name Test 41",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormatNameMailFormat\"]")).GetAttribute("value"),
                "pre check mail format pop up name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnableMailFormat\"]")).FindElement(By.TagName("input"))
                    .Selected, "pre check mail format pop up enable");

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"MailFormatTypeIdMailFormat\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Менеджеру назначен заказ"),
                "pre check mail format pop up type");

            VerifyAreEqual("Format Subject Test 41",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormatSubjectMailFormat\"]")).GetAttribute("value"),
                "pre check mail format pop up subject");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"MailFormatTypeIdMailFormat\"]")).GetAttribute("disabled")
                    .Equals("true"), "pre check mail format pop up subject readonly");
            Driver.AssertCkText("Format Text Test 41", "editor1");
            VerifyAreEqual("41",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailFormat\"]")).GetAttribute("value"),
                "pre check mail format pop up sort");

            //edit
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormatNameMailFormat\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormatNameMailFormat\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormatNameMailFormat\"]")).SendKeys("Edited Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"EnableMailFormat\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormatSubjectMailFormat\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormatSubjectMailFormat\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormatSubjectMailFormat\"]")).SendKeys("Subjet Name Edited");

            Driver.SetCkText("Theme Name Edited", "editor1");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"SortOrderMailFormat\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailFormat\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailFormat\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailFormat\"]")).SendKeys("999");

            Driver.FindElement(By.CssSelector("[data-e2e=\"saveMailFormat\"]")).Click();
            Thread.Sleep(2000);

            //check admin grid
            GoToAdmin("settingsmail#?notifyTab=formats");

            Driver.GetGridIdFilter("grid", "Format Name Test 41");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "prev mail format name count");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no prev mail format name");

            Driver.GetGridIdFilter("grid", "Edited Name");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();

            VerifyAreEqual("Edited Name", Driver.GetGridCell(0, "FormatName").Text, "mail format edited grid name");
            VerifyAreEqual("Менеджеру назначен заказ", Driver.GetGridCell(0, "TypeName").Text,
                "mail format edited grid type");
            VerifyAreEqual("999", Driver.GetGridCell(0, "SortOrder").Text, "mail format edited grid sort");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected,
                "mail format edited grid Enable");

            //check admin edit pop up
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Edited Name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormatNameMailFormat\"]")).GetAttribute("value"),
                "mail format edited pop up name");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EnableMailFormat\"]")).FindElement(By.TagName("input"))
                    .Selected, "mail format edited pop up enable");

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"MailFormatTypeIdMailFormat\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Менеджеру назначен заказ"),
                "mail format edited pop up type");

            VerifyAreEqual("Subjet Name Edited",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormatSubjectMailFormat\"]")).GetAttribute("value"),
                "mail format edited pop up subject");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"MailFormatTypeIdMailFormat\"]")).GetAttribute("disabled")
                    .Equals("true"), "mail format edited pop up subject readonly");
            Driver.AssertCkText("Theme Name Edited", "editor1");
            VerifyAreEqual("999",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SortOrderMailFormat\"]")).GetAttribute("value"),
                "mail format edited pop up sort");
        }
    }
}