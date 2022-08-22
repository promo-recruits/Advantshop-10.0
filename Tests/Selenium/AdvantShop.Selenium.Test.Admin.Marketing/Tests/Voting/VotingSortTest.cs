using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Voting
{
    [TestFixture]
    public class VotingSortTest : BaseSeleniumTest
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
        public void ByName()
        {
            Driver.GetGridCell(-1, "Name", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "Name 1 asc");
            VerifyAreEqual("VoiceTheme107", Driver.GetGridCell(9, "Name", "Voting").Text, "Name 10 asc");

            Driver.GetGridCell(-1, "Name", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("VoiceTheme99", Driver.GetGridCell(0, "Name", "Voting").Text, "Name 1 desc");
            VerifyAreEqual("VoiceTheme90", Driver.GetGridCell(9, "Name", "Voting").Text, "Name 10 desc");

            Driver.GetGridCell(-1, "Name", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("VoiceTheme1", Driver.GetGridCell(0, "Name", "Voting").Text, "Name 1 asc");
            VerifyAreEqual("VoiceTheme10", Driver.GetGridCell(9, "Name", "Voting").Text, "Name 10 asc");
        }


        [Test]
        public void ByIsDefault()
        {
            Driver.GetGridCell(-1, "IsDefault", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsFalse(Driver.GetGridCell(0, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "IsDefault 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "IsDefault 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "Name", "Voting").Text;
            string ascLine10 = Driver.GetGridCell(9, "Name", "Voting").Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different themes");

            Driver.GetGridCell(-1, "IsDefault", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsTrue(Driver.GetGridCell(0, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "IsDefault 1 desc");
            VerifyAreEqual("VoiceTheme7", Driver.GetGridCell(0, "Name", "Voting").Text, "item IsDefault");
            VerifyIsFalse(Driver.GetGridCell(9, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "IsDefault 10 desc");

            string descLine1 = Driver.GetGridCell(0, "Name", "Voting").Text;
            string descLine10 = Driver.GetGridCell(9, "Name", "Voting").Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different themes");

            Driver.GetGridCell(-1, "IsDefault", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsFalse(Driver.GetGridCell(0, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "IsDefault 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "IsDefault", "Voting").FindElement(By.TagName("input")).Selected,
                "IsDefault 10 asc");
        }

        [Test]
        public void ByIsHaveNullVoice()
        {
            Driver.GetGridCell(-1, "IsHaveNullVoice", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsFalse(Driver.GetGridCell(0, "IsHaveNullVoice", "Voting").FindElement(By.TagName("input")).Selected,
                "IsHaveNullVoice 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "IsHaveNullVoice", "Voting").FindElement(By.TagName("input")).Selected,
                "IsHaveNullVoice 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "Name", "Voting").Text;
            string ascLine10 = Driver.GetGridCell(9, "Name", "Voting").Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different themes");

            Driver.GetGridCell(-1, "IsHaveNullVoice", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsTrue(Driver.GetGridCell(0, "IsHaveNullVoice", "Voting").FindElement(By.TagName("input")).Selected,
                "IsHaveNullVoice 1 desc");
            VerifyIsTrue(Driver.GetGridCell(9, "IsHaveNullVoice", "Voting").FindElement(By.TagName("input")).Selected,
                "IsHaveNullVoice 10 desc");

            string descLine1 = Driver.GetGridCell(0, "Name", "Voting").Text;
            string descLine10 = Driver.GetGridCell(9, "Name", "Voting").Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different themes");

            Driver.GetGridCell(-1, "IsHaveNullVoice", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsFalse(Driver.GetGridCell(0, "IsHaveNullVoice", "Voting").FindElement(By.TagName("input")).Selected,
                "IsHaveNullVoice 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "IsHaveNullVoice", "Voting").FindElement(By.TagName("input")).Selected,
                "IsHaveNullVoice 10 asc");
        }

        [Test]
        public void ByIsClose()
        {
            Driver.GetGridCell(-1, "IsClose", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsFalse(Driver.GetGridCell(0, "IsClose", "Voting").FindElement(By.TagName("input")).Selected,
                "IsClose 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "IsClose", "Voting").FindElement(By.TagName("input")).Selected,
                "IsClose 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "Name", "Voting").Text;
            string ascLine10 = Driver.GetGridCell(9, "Name", "Voting").Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different themes");

            Driver.GetGridCell(-1, "IsClose", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsTrue(Driver.GetGridCell(0, "IsClose", "Voting").FindElement(By.TagName("input")).Selected,
                "IsClose 1 desc");
            VerifyIsTrue(Driver.GetGridCell(9, "IsClose", "Voting").FindElement(By.TagName("input")).Selected,
                "IsClose 10 desc");

            string descLine1 = Driver.GetGridCell(0, "Name", "Voting").Text;
            string descLine10 = Driver.GetGridCell(9, "Name", "Voting").Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different themes");

            Driver.GetGridCell(-1, "IsClose", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyIsFalse(Driver.GetGridCell(0, "IsClose", "Voting").FindElement(By.TagName("input")).Selected,
                "IsClose 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "IsClose", "Voting").FindElement(By.TagName("input")).Selected,
                "IsClose 10 asc");
        }


        [Test]
        public void ByDateAdded()
        {
            Driver.GetGridCell(-1, "DateAdded", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("05.09.2012 11:52", Driver.GetGridCell(0, "DateAdded", "Voting").Text, "DateAdded 1 asc");
            VerifyAreEqual("14.09.2012 20:52", Driver.GetGridCell(9, "DateAdded", "Voting").Text, "DateAdded 10 asc");

            Driver.GetGridCell(-1, "DateAdded", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("28.02.2013 12:52", Driver.GetGridCell(0, "DateAdded", "Voting").Text, "DateAdded 1 desc");
            VerifyAreEqual("19.02.2013 23:52", Driver.GetGridCell(9, "DateAdded", "Voting").Text, "DateAdded 10 desc");

            Driver.GetGridCell(-1, "DateAdded", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("05.09.2012 11:52", Driver.GetGridCell(0, "DateAdded", "Voting").Text, "DateAdded 1 asc");
            VerifyAreEqual("14.09.2012 20:52", Driver.GetGridCell(9, "DateAdded", "Voting").Text, "DateAdded 10 asc");
        }

        [Test]
        public void ByDateModify()
        {
            Driver.GetGridCell(-1, "DateModify", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("15.01.2014 14:26", Driver.GetGridCell(0, "DateModify", "Voting").Text, "DateModify 1 asc");
            VerifyAreEqual("24.01.2014 23:26", Driver.GetGridCell(9, "DateModify", "Voting").Text, "DateModify 10 asc");

            Driver.GetGridCell(-1, "DateModify", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("10.07.2014 15:26", Driver.GetGridCell(0, "DateModify", "Voting").Text, "DateModify 1 desc");
            VerifyAreEqual("01.07.2014 16:26", Driver.GetGridCell(9, "DateModify", "Voting").Text,
                "DateModify 10 desc");

            Driver.GetGridCell(-1, "DateModify", "Voting").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("15.01.2014 14:26", Driver.GetGridCell(0, "DateModify", "Voting").Text, "DateModify 1 asc");
            VerifyAreEqual("24.01.2014 23:26", Driver.GetGridCell(9, "DateModify", "Voting").Text, "DateModify 10 asc");
        }
    }
}