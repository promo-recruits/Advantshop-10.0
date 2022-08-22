using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting.VotingAnswers
{
    [TestFixture]
    public class VotingAnswersSortTest : BaseSeleniumTest
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
            Thread.Sleep(500);
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));
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
        public void ByName()
        {
            Driver.GetGridCell(-1, "Name", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "Name 1 asc");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(9, "Name", "Answers").Text, "Name 10 asc");

            Driver.GetGridCell(-1, "Name", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Answer 99", Driver.GetGridCell(0, "Name", "Answers").Text, "Name 1 desc");
            VerifyAreEqual("Answer 90", Driver.GetGridCell(9, "Name", "Answers").Text, "Name 10 desc");

            Driver.GetGridCell(-1, "Name", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Answer 1", Driver.GetGridCell(0, "Name", "Answers").Text, "Name 1 asc");
            VerifyAreEqual("Answer 12", Driver.GetGridCell(9, "Name", "Answers").Text, "Name 10 asc");
        }


        [Test]
        public void ByIsVisible()
        {
            Driver.GetGridCell(-1, "IsVisible", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsFalse(Driver.GetGridCell(0, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "IsVisible 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "IsVisible 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "Name", "Answers").Text;
            string ascLine10 = Driver.GetGridCell(9, "Name", "Answers").Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different answers");

            Driver.GetGridCell(-1, "IsVisible", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsTrue(Driver.GetGridCell(0, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "IsVisible 1 desc");
            VerifyIsTrue(Driver.GetGridCell(9, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "IsVisible 10 desc");

            string descLine1 = Driver.GetGridCell(0, "Name", "Answers").Text;
            string descLine10 = Driver.GetGridCell(9, "Name", "Answers").Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different answers");

            Driver.GetGridCell(-1, "IsVisible", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsTrue(Driver.GetGridCell(0, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "IsVisible 1 asc");
            VerifyIsFalse(Driver.GetGridCell(4, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "IsVisible 5 asc");
            VerifyIsTrue(Driver.GetGridCell(9, "IsVisible", "Answers").FindElement(By.TagName("input")).Selected,
                "IsVisible 10 asc");
        }


        [Test]
        public void ByCountVoice()
        {
            Driver.GetGridCell(-1, "CountVoice", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("1", Driver.GetGridCell(0, "CountVoice", "Answers").Text, "CountVoice 1 asc");
            VerifyAreEqual("10", Driver.GetGridCell(9, "CountVoice", "Answers").Text, "CountVoice 10 asc");

            Driver.GetGridCell(-1, "CountVoice", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("105", Driver.GetGridCell(0, "CountVoice", "Answers").Text, "CountVoice 1 desc");
            VerifyAreEqual("96", Driver.GetGridCell(9, "CountVoice", "Answers").Text, "CountVoice 10 desc");

            Driver.GetGridCell(-1, "CountVoice", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("1", Driver.GetGridCell(0, "CountVoice", "Answers").Text, "CountVoice 1 asc");
            VerifyAreEqual("12", Driver.GetGridCell(9, "CountVoice", "Answers").Text, "CountVoice 10 asc");
        }

        [Test]
        public void BySortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Answers").Text, "SortOrder 1 asc");
            VerifyAreEqual("10", Driver.GetGridCell(9, "SortOrder", "Answers").Text, "SortOrder 10 asc");

            Driver.GetGridCell(-1, "SortOrder", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("105", Driver.GetGridCell(0, "SortOrder", "Answers").Text, "SortOrder 1 desc");
            VerifyAreEqual("96", Driver.GetGridCell(9, "SortOrder", "Answers").Text, "SortOrder 10 desc");

            Driver.GetGridCell(-1, "SortOrder", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Answers").Text, "SortOrder 1 asc");
            VerifyAreEqual("12", Driver.GetGridCell(9, "SortOrder", "Answers").Text, "SortOrder 10 asc");
        }

        [Test]
        public void ByDateAdded()
        {
            Driver.GetGridCell(-1, "DateAdded", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("05.09.2012 11:55", Driver.GetGridCell(0, "DateAdded", "Answers").Text, "DateAdded 1 asc");
            VerifyAreEqual("05.09.2012 21:00", Driver.GetGridCell(9, "DateAdded", "Answers").Text, "DateAdded 10 asc");

            Driver.GetGridCell(-1, "DateAdded", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("09.09.2012 23:45", Driver.GetGridCell(0, "DateAdded", "Answers").Text, "DateAdded 1 desc");
            VerifyAreEqual("09.09.2012 17:54", Driver.GetGridCell(9, "DateAdded", "Answers").Text, "DateAdded 10 desc");

            Driver.GetGridCell(-1, "DateAdded", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("05.09.2012 11:55", Driver.GetGridCell(0, "DateAdded", "Answers").Text, "DateAdded 1 asc");
            VerifyAreEqual("05.09.2012 23:01", Driver.GetGridCell(9, "DateAdded", "Answers").Text, "DateAdded 10 asc");
        }

        [Test]
        public void ByDateModify()
        {
            Driver.GetGridCell(-1, "DateModify", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("04.10.2014 12:23", Driver.GetGridCell(0, "DateModify", "Answers").Text, "DateModify 1 asc");
            VerifyAreEqual("18.02.2015 23:56", Driver.GetGridCell(9, "DateModify", "Answers").Text,
                "DateModify 10 asc");

            Driver.GetGridCell(-1, "DateModify", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("17.01.2019 13:30", Driver.GetGridCell(0, "DateModify", "Answers").Text,
                "DateModify 1 desc");
            VerifyAreEqual("04.09.2018 23:56", Driver.GetGridCell(9, "DateModify", "Answers").Text,
                "DateModify 10 desc");

            Driver.GetGridCell(-1, "DateModify", "Answers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("04.10.2014 12:23", Driver.GetGridCell(0, "DateModify", "Answers").Text, "DateModify 1 asc");
            VerifyAreEqual("19.03.2015 23:44", Driver.GetGridCell(9, "DateModify", "Answers").Text,
                "DateModify 10 asc");
        }
    }
}