using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailFormats
{
    [TestFixture]
    public class SettingsMailFormatsSortTest : BaseSeleniumTest
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
        public void ByFormatName()
        {
            Driver.GetGridCell(-1, "FormatName").Click();
            VerifyAreEqual("Format Name Test 1", Driver.GetGridCell(0, "FormatName").Text, "Name 1 asc");
            VerifyAreEqual("Format Name Test 107", Driver.GetGridCell(9, "FormatName").Text, "Name 10 asc");

            Driver.GetGridCell(-1, "FormatName").Click();
            VerifyAreEqual("Format Name Test 99", Driver.GetGridCell(0, "FormatName").Text, "Name 1 desc");
            VerifyAreEqual("Format Name Test 90", Driver.GetGridCell(9, "FormatName").Text, "Name 10 desc");
        }

        [Test]
        public void ByEnabled()
        {
            Driver.GetGridCell(-1, "Enable").Click();
            VerifyIsFalse(Driver.GetGridCell(0, "Enable").FindElement(By.TagName("input")).Selected, "Enabled 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "Enable").FindElement(By.TagName("input")).Selected, "Enabled 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "FormatName").Text;
            string ascLine10 = Driver.GetGridCell(9, "FormatName").Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different mail formats");

            Driver.GetGridCell(-1, "Enable").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Enable").FindElement(By.TagName("input")).Selected, "Enabled 1 desc");
            VerifyIsTrue(Driver.GetGridCell(9, "Enable").FindElement(By.TagName("input")).Selected, "Enabled 10 desc");

            string descLine1 = Driver.GetGridCell(0, "FormatName").Text;
            string descLine10 = Driver.GetGridCell(9, "FormatName").Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different mail formats");
        }

        [Test]
        public void ByFormatType()
        {
            Driver.GetGridCell(-1, "TypeName").Click();
            VerifyAreEqual("Восстановление пароля", Driver.GetGridCell(0, "TypeName").Text, "MailFormatType 1 asc");
            VerifyAreEqual("Восстановление пароля", Driver.GetGridCell(1, "TypeName").Text, "MailFormatType 2 asc");
            VerifyAreEqual("Заказ в один клик", Driver.GetGridCell(8, "TypeName").Text, "MailFormatType 9 asc");
            VerifyAreEqual("Заказ в один клик", Driver.GetGridCell(9, "TypeName").Text, "MailFormatType 10 asc");

            Driver.GetGridCell(-1, "TypeName").Click();
            VerifyAreEqual("Удаление задачи", Driver.GetGridCell(0, "TypeName").Text, "MailFormatType 1 desc");
            VerifyAreEqual("Удаление задачи", Driver.GetGridCell(1, "TypeName").Text, "MailFormatType 2 desc");
            VerifyAreEqual("Уведомление о новом отзыве", Driver.GetGridCell(8, "TypeName").Text,
                "MailFormatType 9 desc");
            VerifyAreEqual("Уведомление о новом отзыве", Driver.GetGridCell(9, "TypeName").Text,
                "MailFormatType 10 desc");
        }

        [Test]
        public void BySortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder").Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder").Text, "SortOrder 1 asc");
            VerifyAreEqual("10", Driver.GetGridCell(9, "SortOrder").Text, "SortOrder 10 asc");

            Driver.GetGridCell(-1, "SortOrder").Click();
            VerifyAreEqual("107", Driver.GetGridCell(0, "SortOrder").Text, "SortOrder 1 desc");
            VerifyAreEqual("98", Driver.GetGridCell(9, "SortOrder").Text, "SortOrder 10 desc");
        }
    }
}