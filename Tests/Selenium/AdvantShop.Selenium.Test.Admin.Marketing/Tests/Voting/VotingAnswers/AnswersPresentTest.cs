using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting.VotingAnswers
{
    [TestFixture]
    public class VotingAnswersPresentTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\VotingAnswers\\Voice.VoiceTheme.csv",
                "data\\Admin\\VotingAnswers\\Voice.Answer.csv"
            );

            Init();
            InstallModule(BaseUrl, "Voting", true);
            ChangeSidebarState(true);
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));
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
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "line 1");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(9, "Name", "Answers").Text, "line 10");
        }

        [Test]
        public void Present20()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.PageSelectItems("20");
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "line 1");
            VerifyAreEqual("Answer 21", Driver.GetGridCell(19, "Name", "Answers").Text, "line 20");
        }

        [Test]
        public void Present50()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.PageSelectItems("50");
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "line 1");
            VerifyAreEqual("Answer 49", Driver.GetGridCell(49, "Name", "Answers").Text, "line 50");
        }

        [Test]
        public void Present100()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.PageSelectItems("100");
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "line 1");
            VerifyAreEqual("Answer 94", Driver.GetGridCell(99, "Name", "Answers").Text, "line 100");
        }
    }
}