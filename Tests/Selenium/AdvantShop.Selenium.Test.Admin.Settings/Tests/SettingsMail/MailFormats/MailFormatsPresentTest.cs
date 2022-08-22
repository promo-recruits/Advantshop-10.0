using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailFormats
{
    [TestFixture]
    public class SettingsMailFormatsPresentTest : BaseSeleniumTest
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
            GoToAdmin("settingsmail#?notifyTab=formats");
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
            Driver.GridPaginationSelectItems("10", "grid");
            VerifyAreEqual("Format Name Test 1", Driver.GetGridCell(0, "FormatName").Text, "line 1");
            VerifyAreEqual("Format Name Test 10", Driver.GetGridCell(9, "FormatName").Text, "line 10");
        }

        [Test]
        public void Present20()
        {
            Driver.GridPaginationSelectItems("20", "grid");
            VerifyAreEqual("Format Name Test 1", Driver.GetGridCell(0, "FormatName").Text, "line 1");
            VerifyAreEqual("Format Name Test 20", Driver.GetGridCell(19, "FormatName").Text, "line 20");
        }

        [Test]
        public void Present50()
        {
            Driver.GridPaginationSelectItems("50", "grid");
            VerifyAreEqual("Format Name Test 1", Driver.GetGridCell(0, "FormatName").Text, "line 1");
            VerifyAreEqual("Format Name Test 50", Driver.GetGridCell(49, "FormatName").Text, "line 50");
        }

        [Test]
        public void Present100()
        {
            Driver.GridPaginationSelectItems("100", "grid");
            VerifyAreEqual("Format Name Test 1", Driver.GetGridCell(0, "FormatName").Text, "line 1");
            VerifyAreEqual("Format Name Test 100", Driver.GetGridCell(99, "FormatName").Text, "line 100");
        }
    }
}