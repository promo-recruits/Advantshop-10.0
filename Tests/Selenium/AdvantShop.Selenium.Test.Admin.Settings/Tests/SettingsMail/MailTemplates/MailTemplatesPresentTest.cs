using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailTemplates
{
    [TestFixture]
    public class SettingsMailTemplatesPresentTest : BaseSeleniumTest
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
        public void Present10()
        {
            Driver.GridPaginationSelectItems("10", "gridTemplates");
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text, "line 1");
            VerifyAreEqual("Template Name Test 10", Driver.GetGridCell(9, "Name", "Templates").Text, "line 10");
        }

        [Test]
        public void Present20()
        {
            Driver.GridPaginationSelectItems("20", "gridTemplates");
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text, "line 1");
            VerifyAreEqual("Template Name Test 20", Driver.GetGridCell(19, "Name", "Templates").Text, "line 20");
        }

        [Test]
        public void Present50()
        {
            Driver.GridPaginationSelectItems("50", "gridTemplates");
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text, "line 1");
            VerifyAreEqual("Template Name Test 50", Driver.GetGridCell(49, "Name", "Templates").Text, "line 50");
        }

        [Test]
        public void Present100()
        {
            Driver.GridPaginationSelectItems("100", "gridTemplates");
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text, "line 1");
            VerifyAreEqual("Template Name Test 100", Driver.GetGridCell(99, "Name", "Templates").Text, "line 100");
        }
    }
}