using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting.VotingAnswers
{
    [TestFixture]
    public class VotingAnswersAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\VotingAnswers\\VotingAnswersAddEdit\\Voice.VoiceTheme.csv",
                "data\\Admin\\VotingAnswers\\VotingAnswersAddEdit\\Voice.Answer.csv"
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

        [Test]
        public void AnswersAddFromPage()
        {
            //set default
            GoToModule("voting");

            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Редактирование голосования", Driver.FindElement(By.TagName("h2")).Text, "modal add h2");
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                .Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Refresh();
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnAddAnswers\"]")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Добавление варианта ответа", Driver.FindElement(By.TagName("h2")).Text, "modal add h2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerName\"]")).SendKeys("New Answer Test");

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerSortOrder\"]")).SendKeys("2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerIsVisible\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin grid
            Refresh();
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "grid count all");
            VerifyAreEqual("New Answer Test", Driver.GetGridCell(1, "Name", "Answers").Text, "grid name");
            VerifyAreEqual("0", Driver.GetGridCell(1, "CountVoice", "Answers").Text, "grid CountVoice");
            VerifyAreEqual("2", Driver.GetGridCell(1, "SortOrder", "Answers").Text, "grid SortOrder");
            VerifyIsTrue(Driver.GetGridCell(1, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "grid IsVisible");

            //check admin edit pop up
            Driver.GetGridCell(1, "_serviceColumn", "Answers")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Редактирование варианта ответа", Driver.FindElement(By.TagName("h2")).Text, "modal add h2");
            VerifyAreEqual("VoiceTheme1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteTheme\"]")).GetAttribute("value"),
                "voting name edit pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteTheme\"]")).GetAttribute("readonly").Equals("true"),
                "voting name readonly edit pop up");
            VerifyAreEqual("New Answer Test",
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerName\"]")).GetAttribute("value"),
                "answer name edit pop up");
            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerSortOrder\"]")).GetAttribute("value"),
                "answer sort order edit pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerIsVisible\"]"))
                    .FindElement(By.TagName("input")).Selected, "answer is visible edi pop up");

            //check client
            GoToClient();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-question")).Text.Contains("VoiceTheme1"),
                "voting name client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 1"),
                "voting answer 1 client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("New Answer Test"),
                "added voting answer 2 client");
        }

        [Test]
        public void AnswersVote()
        {
            //set default
            GoToModule("voting");

            Driver.GetGridCell(1, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                .Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Refresh();
            Driver.GetGridCell(1, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 2\"]")).Text
                    .Contains("Answer 2"), "answers statistics 1 answer");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 2-percent\"]")).Text
                    .Contains("2 голосов(40%)"), "answers statistics 1 answer count voices");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 3\"]")).Text
                    .Contains("Answer 3"), "answers statistics 2 answer");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 3-percent\"]")).Text
                    .Contains("3 голосов(60%)"), "answers statistics 2 answer count voices");

            VerifyAreEqual("2", Driver.GetGridCell(0, "CountVoice", "Answers").Text, "grid CountVoice 1 answer");
            VerifyAreEqual("3", Driver.GetGridCell(1, "CountVoice", "Answers").Text, "grid CountVoice 2 answer");

            //check client voting
            GoToClient();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-question")).Text.Contains("VoiceTheme2"),
                "voting name client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 2"),
                "voting answer 1 client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 3"),
                "voting answer 2 client");
            Driver.FindElement(By.CssSelector(".voting-answers")).FindElements(By.CssSelector(".voting-answers-row"))[1]
                .FindElement(By.CssSelector(".custom-input-radio")).Click();
            Thread.Sleep(500);

            Driver.FindElement(By.Name("votingForm")).FindElement(By.CssSelector(".btn.btn-small.btn-action")).Click();
            Thread.Sleep(500);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-question")).Text.Contains("VoiceTheme2"),
                "voting name client after vote");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-results")).Text.Contains("Answer 2"),
                "answer 1 client after vote");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-results")).Text.Contains("Answer 3"),
                "answer 2 client after vote");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-results-total")).Text.Contains("6"),
                "all result count client after vote");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".voting-results"))
                    .FindElements(By.CssSelector(".voting-results-progress"))[0].Text.Contains("33%"),
                "result answer 1 count client after vote");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".voting-results"))
                    .FindElements(By.CssSelector(".voting-results-progress"))[1].Text.Contains("67%"),
                "result answer 2 count client after vote");

            //check admin after vote
            GoToModule("voting");

            Driver.GetGridCell(1, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 2\"]")).Text
                    .Contains("Answer 2"), "answers statistics 1 admin after vote");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 2\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 2-percent\"]")).Text
                    .Contains("2 голосов(33%)"), "answers statistics 1 answer count voices after vote");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 3\"]")).Text
                    .Contains("Answer 3"), "answers statistics 2 admin after vote");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"voteAnswersStat-answer-Answer 3-percent\"]")).Text
                    .Contains("4 голосов(67%)"), "answers statistics 2 answer count voices after vote");

            VerifyAreEqual("2", Driver.GetGridCell(0, "CountVoice", "Answers").Text,
                "grid CountVoice 1 answer after vote");
            VerifyAreEqual("4", Driver.GetGridCell(1, "CountVoice", "Answers").Text,
                "grid CountVoice 2 answer after vote");
        }

        [Test]
        public void AnswersAddFromVoteEdit()
        {
            GoToModule("voting");
            Driver.GetGridCell(2, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Answer 4", Driver.FindElement(By.CssSelector("[data-e2e=\"answer-Answer 4\"]")).Text,
                "pop up voting answer 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".payment-text")).Count == 1, "pop up count answers");

            Driver.FindElement(By.CssSelector("[data-e2e=\"answerNameAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"answerNameAdd\"]"))
                .SendKeys("New Answer Test From Voting Edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"answerAdd\"]")).Click();

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                .Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            Refresh();
            Driver.GetGridCell(2, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("New Answer Test From Voting Edit",
                Driver.FindElement(By.CssSelector("[data-e2e=\"answer-New Answer Test From Voting Edit\"]")).Text,
                "pop up voting answer added");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".payment-text")).Count == 2,
                "pop up count answers with added");

            Driver.XPathContainsText("button", "Отмена");

            Driver.GetGridCell(2, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "grid count all");
            VerifyAreEqual("New Answer Test From Voting Edit", Driver.GetGridCell(1, "Name", "Answers").Text,
                "grid name added");
            VerifyAreEqual("0", Driver.GetGridCell(1, "CountVoice", "Answers").Text, "grid CountVoice added");
            VerifyAreEqual("0", Driver.GetGridCell(1, "SortOrder", "Answers").Text, "grid SortOrder added");
            VerifyIsTrue(Driver.GetGridCell(1, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "grid IsVisible added");

            //check client
            GoToClient();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-question")).Text.Contains("VoiceTheme3"),
                "voting name client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 4"),
                "voting answer 1 client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("New Answer Test From Voting Edit"),
                "added voting answer 2 client");
        }

        [Test]
        public void AnswersEditSort()
        {
            //set default
            GoToModule("voting");
            Driver.GetGridCell(3, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                .Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //pre check sort
            GoToClient();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".voting-answers"))
                    .FindElements(By.CssSelector(".voting-answers-row"))[0].Text.Contains("Answer 5"),
                "pre check sort client 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".voting-answers"))
                    .FindElements(By.CssSelector(".voting-answers-row"))[1].Text.Contains("Answer 6"),
                "pre check sort client 2");

            //test
            GoToModule("voting");

            Driver.GetGridCell(3, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Answer 5", Driver.GetGridCell(0, "Name", "Answers").Text, "grid name");
            VerifyAreEqual("5", Driver.GetGridCell(0, "CountVoice", "Answers").Text, "grid CountVoice");
            VerifyAreEqual("5", Driver.GetGridCell(0, "SortOrder", "Answers").Text, "grid SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "grid IsVisible");

            Driver.GetGridCell(0, "_serviceColumn", "Answers")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerSortOrder\"]")).SendKeys("10");

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToModule("voting");
            Driver.GetGridCell(3, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));

            VerifyAreEqual("10", Driver.GetGridCell(0, "SortOrder", "Answers").Text, "grid SortOrder edited");

            //check client
            GoToClient();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".voting-answers"))
                    .FindElements(By.CssSelector(".voting-answers-row"))[0].Text.Contains("Answer 6"),
                "sort client 1 edited");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".voting-answers"))
                    .FindElements(By.CssSelector(".voting-answers-row"))[1].Text.Contains("Answer 5"),
                "sort client 2 edited");
        }

        [Test]
        public void AnswersEditName()
        {
            //set default
            GoToModule("voting");
            Driver.GetGridCell(4, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                .Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //test
            Refresh();
            Driver.GetGridCell(4, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Answer 7", Driver.GetGridCell(0, "Name", "Answers").Text, "grid name");

            Driver.GetGridCell(0, "_serviceColumn", "Answers")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerName\"]")).SendKeys("Edited Answer Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToModule("voting");

            Driver.GetGridCell(4, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));

            VerifyAreEqual("Edited Answer Name", Driver.GetGridCell(0, "Name", "Answers").Text, "grid name edited");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridAnswers\"]")).Text.Contains("Answer 7"),
                "previous edited name");

            //check client
            GoToClient();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Edited Answer Name"),
                "edited answer name client");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 7"),
                "previous edited answer name client");
        }

        [Test]
        public void AnswersEditVisible()
        {
            //set default
            GoToModule("voting");
            Driver.GetGridCell(5, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                .Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //pre check client
            GoToClient();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 8"),
                "pre check client 1 answer is visible");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".voting-answers-row")).Count == 1,
                "pre check client 2 answer not visible count");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 9"),
                "pre check client 2 answer not visible name");

            //test
            GoToModule("voting");

            Driver.GetGridCell(5, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Answer 8", Driver.GetGridCell(0, "Name", "Answers").Text, "grid name 1");
            VerifyIsTrue(Driver.GetGridCell(0, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "grid IsVisible 1");
            VerifyAreEqual("Answer 9", Driver.GetGridCell(1, "Name", "Answers").Text, "grid name 2");
            VerifyIsFalse(Driver.GetGridCell(1, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "grid IsVisible 2");

            Driver.GetGridCell(1, "_serviceColumn", "Answers")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerIsVisible\"]"))
                    .FindElement(By.TagName("input")).Selected, "pop up IsVisible 2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerIsVisible\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToModule("voting");

            Driver.GetGridCell(5, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));

            Driver.GetGridCell(1, "_serviceColumn", "Answers")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteAnswerIsVisible\"]"))
                    .FindElement(By.TagName("input")).Selected, "pop up IsVisible 2 edited");

            VerifyAreEqual("Answer 9", Driver.GetGridCell(1, "Name", "Answers").Text, "grid name 2");
            VerifyIsTrue(Driver.GetGridCell(1, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "grid IsVisible 2 edited");

            //check client
            GoToClient();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 8"),
                "client 1 answer is visible");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".voting-answers-row")).Count == 2,
                "client 2 answer edited visible count");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 9"),
                "client 2 answer edited visible name");
        }

        [Test]
        public void AnswersEditDeleteFromVoteEdit()
        {
            GoToModule("voting");
            Driver.GetGridCell(6, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Answer 10", Driver.FindElement(By.CssSelector("[data-e2e=\"answer-Answer 10\"]")).Text,
                "pop up voting answer 1");
            VerifyAreEqual("Answer 11", Driver.FindElement(By.CssSelector("[data-e2e=\"answer-Answer 11\"]")).Text,
                "pop up voting answer 2");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".payment-text")).Count == 2, "pop up count answers");

            Driver.FindElement(By.CssSelector("[data-e2e=\"answer-id-11-delete\"]")).Click();
            Driver.SwalConfirm();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".payment-text")).Count == 1,
                "pop up count answers after deleting");

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                .Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            Refresh();
            Driver.GetGridCell(6, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Answer 10", Driver.FindElement(By.CssSelector("[data-e2e=\"answer-Answer 10\"]")).Text,
                "edit pop up voting answer 1 left after deleting");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".payment-text")).Count == 1,
                "edit pop up count answers after deleting");

            Driver.XPathContainsText("button", "Отмена");

            Driver.GetGridCell(6, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "grid count all");
            VerifyAreEqual("Answer 10", Driver.GetGridCell(0, "Name", "Answers").Text, "grid name left after deleting");

            //check client
            GoToClient();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-question")).Text.Contains("VoiceTheme7"),
                "voting name client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 10"),
                "voting answer 1 client");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".voting-answers")).Text.Contains("Answer 11"),
                "voting answer 2 client deleted");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".voting-answers-row")).Count == 1,
                "voting answer 2 answer deleted count");
        }

        [Test]
        public void AnswersEditDeleteFromPage()
        {
            //set default
            GoToModule("voting");

            Driver.GetGridCell(7, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("input"))
                .Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"voteIsDefault\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.FindElement(By.CssSelector("[data-e2e=\"voteSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Refresh();
            Driver.GetGridCell(7, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "grid count all before deleting");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(0, "Name", "Answers").Text, "grid name before deleting");

            Driver.GetGridCell(0, "_serviceColumn", "Answers").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            Driver.WaitForToastSuccess();

            //check admin 
            GoToModule("voting");
            Driver.GetGridCell(7, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "grid count all after deleting");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "grid after deleting");

            //check client
            GoToClient();
            VerifyIsFalse(Driver.PageSource.Contains("VoiceTheme8"), "voting name client deleted");
            VerifyIsFalse(Driver.PageSource.Contains("Answer 12"), "voting answer 1 client deleted");
        }
    }
}