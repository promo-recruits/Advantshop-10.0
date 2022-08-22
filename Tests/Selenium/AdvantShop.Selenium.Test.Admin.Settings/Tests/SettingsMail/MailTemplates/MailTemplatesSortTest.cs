using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailTemplates
{
    [TestFixture]
    public class SettingsMailTemplatesSortTest : BaseSeleniumTest
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
            GoToAdmin("settingsmail#?notifyTab=templates");
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
        public void ByTemplateName()
        {
            Driver.GetGridCell(-1, "Name", "Templates").Click();
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text, "Name 1 asc");
            VerifyAreEqual("Template Name Test 107", Driver.GetGridCell(9, "Name", "Templates").Text, "Name 10 asc");

            Driver.GetGridCell(-1, "Name", "Templates").Click();
            VerifyAreEqual("Template Name Test 99", Driver.GetGridCell(0, "Name", "Templates").Text, "Name 1 desc");
            VerifyAreEqual("Template Name Test 90", Driver.GetGridCell(9, "Name", "Templates").Text, "Name 10 desc");
        }

        [Test]
        public void ByEnabled()
        {
            Driver.GetGridCell(-1, "Active", "Templates").Click();
            VerifyIsFalse(Driver.GetGridCell(0, "Active", "Templates").FindElement(By.TagName("input")).Selected,
                "Enabled 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "Active", "Templates").FindElement(By.TagName("input")).Selected,
                "Enabled 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "Name", "Templates").Text;
            string ascLine10 = Driver.GetGridCell(9, "Name", "Templates").Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different mail formats");

            Driver.GetGridCell(-1, "Active", "Templates").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Active", "Templates").FindElement(By.TagName("input")).Selected,
                "Enabled 1 desc");
            VerifyIsTrue(Driver.GetGridCell(9, "Active", "Templates").FindElement(By.TagName("input")).Selected,
                "Enabled 10 desc");

            string descLine1 = Driver.GetGridCell(0, "Name", "Templates").Text;
            string descLine10 = Driver.GetGridCell(9, "Name", "Templates").Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different mail formats");
        }


        [Test]
        public void BySortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder", "Templates").Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Templates").Text, "SortOrder 1 asc");
            VerifyAreEqual("10", Driver.GetGridCell(9, "SortOrder", "Templates").Text, "SortOrder 10 asc");

            Driver.GetGridCell(-1, "SortOrder", "Templates").Click();
            VerifyAreEqual("107", Driver.GetGridCell(0, "SortOrder", "Templates").Text, "SortOrder 1 desc");
            VerifyAreEqual("98", Driver.GetGridCell(9, "SortOrder", "Templates").Text, "SortOrder 10 desc");
        }
    }
}