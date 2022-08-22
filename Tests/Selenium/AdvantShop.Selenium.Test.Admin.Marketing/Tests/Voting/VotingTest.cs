using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting
{
    [TestFixture]
    public class VotingTest : BaseSeleniumTest
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
        public void Grid()
        {
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Голосование"), "h1 page");

            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "grid count all");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "grid name");
            VerifyIsFalse(Driver.GetGridCell(0, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "grid IsDefault");
            VerifyIsFalse(Driver.GetGridCell(0, "IsHaveNullVoice", "Voting").FindElement(By.TagName("input")).Selected,
                "grid IsHaveNullVoice");
            VerifyIsFalse(Driver.GetGridCell(0, "IsClose", "Voting").FindElement(By.TagName("input")).Selected,
                "grid IsClose");
            VerifyAreEqual("1", Driver.GetGridCell(0, "CountAnswers", "Voting").Text, "grid CountAnswers");
            VerifyAreEqual("05.09.2012 11:52", Driver.GetGridCell(0, "DateAdded", "Voting").Text, "grid DateAdded");
            VerifyAreEqual("15.01.2014 14:26", Driver.GetGridCell(0, "DateModify", "Voting").Text, "grid DateModify");

            VerifyIsTrue(Driver.GetGridCell(6, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "grid IsDefault true");
            VerifyAreEqual("VoiceTheme7", Driver.GetGridCell(6, "Name", "Voting").Text, "grid name IsDefault");
        }


        [Test]
        public void ToEdit()
        {
            Driver.GetGridCell(0, "_serviceColumn", "Voting")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Редактирование голосования", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.XPathContainsText("button", "Отмена");
        }

        [Test]
        public void ToAnswers()
        {
            Driver.GetGridCell(0, "ID", "Voting").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("VotingAnswerTitle"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Ответы"), "h1 answers page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnBack\"]")).Click();
            Thread.Sleep(500);

            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Голосование"), "h1 page return");
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "grid name return");
        }
    }
}