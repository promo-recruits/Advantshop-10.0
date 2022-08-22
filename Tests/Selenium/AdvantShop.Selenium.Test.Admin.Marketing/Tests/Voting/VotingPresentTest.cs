using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting
{
    [TestFixture]
    public class VotingPresentTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Voting\\Voice.VoiceTheme.csv",
                "data\\Admin\\Voting\\Voice.Answer.csv"
            );

            Init();
            InstallModule(BaseUrl, "Voting", true);
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
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.PageSelectItems("10");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "line 1");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text, "line 10");
        }

        [Test]
        public void Present20()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.PageSelectItems("20");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "line 1");
            VerifyAreEqual("VoiceTheme20", Driver.GetGridCell(19, "Name", "Voting").Text, "line 20");
        }

        [Test]
        public void Present50()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.PageSelectItems("50");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "line 1");
            VerifyAreEqual("VoiceTheme50", Driver.GetGridCell(49, "Name", "Voting").Text, "line 50");
        }

        [Test]
        public void Present100()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.PageSelectItems("100");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "line 1");
            VerifyAreEqual("VoiceTheme100", Driver.GetGridCell(99, "Name", "Voting").Text, "line 100");
        }
    }
}