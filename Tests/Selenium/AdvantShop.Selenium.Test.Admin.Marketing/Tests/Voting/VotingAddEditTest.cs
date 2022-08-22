using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting
{
    [TestFixture]
    public class VotingAddEditTest : BaseSeleniumTest
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
            InitializeService.VoitingActive();
            Init();
            InstallModule(BaseUrl, "Voting", true);
            ChangeSidebarState(true);
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

        [Order(0)]
        [Test]
        public void VotingAddIsDefault()
        {
            GoToModule("voting");

            //pre check
            GoToClient();
            VerifyIsTrue(Driver.PageSource.Contains("VoiceTheme7"), "pre check default voting on page");

            //test
            GoToModule("voting");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteName\"]")).SendKeys("New Voting");
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin grid
            Refresh();
            VerifyAreEqual("Найдено записей: 171",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "voting added count");

            Driver.GridFilterSendKeys("New Voting");
            VerifyAreEqual("New Voting", Driver.GetGridCell(0, "Name", "Voting").Text, "added name grid");
            VerifyIsTrue(Driver.GetGridCell(0, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "added IsDefault grid");
            VerifyIsFalse(Driver.GetGridCell(0, "IsHaveNullVoice", "Voting").FindElement(By.TagName("input")).Selected,
                "added IsHaveNullVoice grid");
            VerifyIsFalse(Driver.GetGridCell(0, "IsClose", "Voting").FindElement(By.TagName("input")).Selected,
                "added IsClose grid");
            VerifyAreEqual("0", Driver.GetGridCell(0, "CountAnswers", "Voting").Text, "added CountAnswers grid");

            //check admin pop up
            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("New Voting",
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteName\"]")).GetAttribute("value"),
                "added name pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                    .Selected, "added IsDefault pop up");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsHaveNullVoice\"]"))
                    .FindElement(By.TagName("input")).Selected, "added IsHaveNullVoice pop up");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsClose\"]")).FindElement(By.TagName("input"))
                    .Selected, "added IsClose pop up");

            //add answers
            Driver.FindElement(By.CssSelector("[data-e2e=\"answerNameAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"answerNameAdd\"]")).SendKeys("New Answer 1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"answerAdd\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"answerNameAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"answerNameAdd\"]")).SendKeys("New Answer 2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"answerAdd\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Refresh();

            //check previous default voting
            Driver.GridFilterSendKeys("VoiceTheme7");
            VerifyAreEqual("VoiceTheme7", Driver.GetGridCell(0, "Name", "Voting").Text, "previous default name grid");
            VerifyIsFalse(Driver.GetGridCell(0, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "previous default IsDefault no");

            //check client
            GoToClient();
            VerifyIsFalse(Driver.PageSource.Contains("VoiceTheme7"), "previous default name client no on page");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-question")).Text.Contains("New Voting"),
                "added name client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("New Answer 1"),
                "added answer 1 client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("New Answer 2"),
                "added answer 2 client");
            VerifyIsTrue(
                Driver.FindElement(By.Name("votingForm")).FindElements(By.CssSelector(".btn.btn-small.btn-action"))
                    .Count == 1, "added has null voice not");
        }

        [Order(1)]
        [Test]
        public void VotingAdd()
        {
            GoToModule("voting");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteName\"]")).SendKeys("New Voting All Options");

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsHaveNullVoice\"]")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsClose\"]")).FindElement(By.TagName("span")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Refresh();
            Driver.GridFilterSendKeys("New Voting All Options");

            VerifyAreEqual("New Voting All Options", Driver.GetGridCell(0, "Name", "Voting").Text, "added name grid");
            VerifyIsTrue(Driver.GetGridCell(0, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "added IsDefault grid");
            VerifyIsTrue(Driver.GetGridCell(0, "IsHaveNullVoice", "Voting").FindElement(By.TagName("input")).Selected,
                "added IsHaveNullVoice grid");
            VerifyIsTrue(Driver.GetGridCell(0, "IsClose", "Voting").FindElement(By.TagName("input")).Selected,
                "added IsClose grid");
            VerifyAreEqual("0", Driver.GetGridCell(0, "CountAnswers", "Voting").Text, "added CountAnswers grid");
        }

        [Order(3)]
        [Test]
        public void VotingEditIsHaveNullVoice()
        {
            GoToModule("voting");

            Driver.GridFilterSendKeys("VoiceTheme10");

            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(0, "Name", "Voting").Text,
                "pre check voting name grid IsHaveNullVoice");
            VerifyIsFalse(Driver.GetGridCell(0, "IsHaveNullVoice", "Voting").FindElement(By.TagName("input")).Selected,
                "pre check voting grid IsHaveNullVoice");
            VerifyIsFalse(Driver.GetGridCell(0, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "pre check voting grid IsDefault");

            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("VoiceTheme10",
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteName\"]")).GetAttribute("value"),
                "pre check voting name grid pop up");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsHaveNullVoice\"]"))
                    .FindElement(By.TagName("input")).Selected, "pre check voting IsHaveNullVoice pop up");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                    .Selected, "pre check voting IsDefault pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswersList\"]"))
                    .FindElements(By.CssSelector(".row.as-sortable-item")).Count == 11,
                "pre check voting answers count pop up"); //10 answers + add button field
            VerifyAreEqual("Answer 46",
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswersList\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"answer-Answer 46\"]")).Text,
                "pre check voting answers name pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsHaveNullVoice\"]")).FindElement(By.TagName("span"))
                .Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteName\"]")).SendKeys("Edited Voting Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin grid
            Refresh();
            //Driver.GridFilterSendKeys("VoiceTheme10");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridVoting\"]")).Text.Contains("VoiceTheme10 "),
                "previous edited name");

            Driver.GridFilterSendKeys("Edited Voting Name");

            VerifyAreEqual("Edited Voting Name", Driver.GetGridCell(0, "Name", "Voting").Text, "edited name grid");
            VerifyIsTrue(Driver.GetGridCell(0, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "edited IsDefault grid");
            VerifyIsTrue(Driver.GetGridCell(0, "IsHaveNullVoice", "Voting").FindElement(By.TagName("input")).Selected,
                "edited IsHaveNullVoice grid");

            //check admin pop up
            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Edited Voting Name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteName\"]")).GetAttribute("value"),
                "edited voting name grid pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsHaveNullVoice\"]"))
                    .FindElement(By.TagName("input")).Selected, "edited voting IsHaveNullVoice pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                    .Selected, "edited voting IsDefault pop up");

            //check client
            GoToClient();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-question")).Text.Contains("Edited Voting Name"),
                "edited name client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 46"),
                "edited answer 1 client");
            VerifyIsTrue(
                Driver.FindElement(By.Name("votingForm")).FindElements(By.CssSelector(".btn.btn-small.btn-action"))
                    .Count == 2, "edited has null voice");

            //check IsHaveNullVoice change no
            GoToModule("voting");

            Driver.GridFilterSendKeys("Edited Voting Name");

            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsHaveNullVoice\"]")).FindElement(By.TagName("span"))
                .Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check client no IsHaveNullVoice
            GoToClient();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-question")).Text.Contains("Edited Voting Name"),
                "edited name client no IsHaveNullVoice");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 46"),
                "edited answer 1 client no IsHaveNullVoice");
            VerifyIsTrue(
                Driver.FindElement(By.Name("votingForm")).FindElements(By.CssSelector(".btn.btn-small.btn-action"))
                    .Count == 1, "edited has null voice no IsHaveNullVoice");
        }

        [Order(2)]
        [Test]
        public void VotingEditIsClose()
        {
            GoToModule("voting");

            Driver.GridFilterSendKeys("VoiceTheme11");

            VerifyAreEqual("VoiceTheme11", Driver.GetGridCell(0, "Name", "Voting").Text,
                "pre check voting name grid no IsClose");

            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsClose\"]")).FindElement(By.TagName("input"))
                    .Selected, "pre check voting IsClose pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsClose\"]")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin grid
            Refresh();

            Driver.GridFilterSendKeys("VoiceTheme11");

            VerifyAreEqual("VoiceTheme11", Driver.GetGridCell(0, "Name", "Voting").Text, "name grid");
            VerifyIsTrue(Driver.GetGridCell(0, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "edited IsDefault grid");
            VerifyIsTrue(Driver.GetGridCell(0, "IsClose", "Voting").FindElement(By.TagName("input")).Selected,
                "edited IsClose grid");

            //check admin pop up
            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsClose\"]")).FindElement(By.TagName("input"))
                    .Selected, "edited voting IsClose pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                    .Selected, "edited voting IsDefault pop up");

            //check client
            GoToClient();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-question")).Text.Contains("VoiceTheme11"),
                "edited name IsClose client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-results")).Text.Contains("Answer 56"),
                "edited answer 1 IsClose client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-results-total")).Displayed,
                "edited IsClose result client");

            //check IsClose change no
            GoToModule("voting");

            Driver.GridFilterSendKeys("VoiceTheme11");

            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsClose\"]")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check client no IsClose
            GoToClient();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-question")).Text.Contains("VoiceTheme11"),
                "edited name client no IsClose");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 56"),
                "edited answer 1 client no IsClose");
            VerifyIsTrue(
                Driver.FindElement(By.Name("votingForm")).FindElements(By.CssSelector(".btn.btn-small.btn-action"))
                    .Count == 1, "edited no IsClose client");
        }
    }
}