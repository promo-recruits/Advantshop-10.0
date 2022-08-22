using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting.VotingAnswers
{
    [TestFixture]
    public class VotingAnswersTest : BaseSeleniumTest
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
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
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
        public void Grid()
        {
            VerifyAreEqual("Ответы", Driver.FindElement(By.CssSelector("[data-e2e=\"VotingAnswerTitle\"]")).Text,
                "h1 page");

            VerifyAreEqual(1, Driver.FindElements(AdvBy.DataE2E("voteAnswersStat")).Count, "Answers count all");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "grid count all");
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "grid name");
            VerifyAreEqual("1", Driver.GetGridCell(0, "CountVoice", "Answers").Text, "grid CountVoice");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Answers").Text, "grid SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "grid IsVisible");

            VerifyAreEqual("05.09.2012 11:55", Driver.GetGridCell(0, "DateAdded", "Answers").Text, "grid DateAdded");
            VerifyAreEqual("04.10.2014 12:23", Driver.GetGridCell(0, "DateModify", "Answers").Text, "grid DateModify");
        }

        [Test]
        public void ToEdit()
        {
            Driver.GetGridCell(0, "_serviceColumn", "Answers")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Редактирование варианта ответа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.XPathContainsText("button", "Отмена");
        }
    }
}