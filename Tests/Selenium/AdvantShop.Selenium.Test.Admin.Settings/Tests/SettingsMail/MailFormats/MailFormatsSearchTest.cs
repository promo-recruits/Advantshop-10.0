using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailFormats
{
    [TestFixture]
    public class SettingsMailFormatsSearchTest : BaseSeleniumTest
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
        public void SearchExist()
        {
            GoToAdmin("settingsmail#?notifyTab=formats");

            Driver.GetGridIdFilter("grid", "Format Name Test 97");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("Format Name Test 97", Driver.GetGridCell(0, "FormatName").Text, "search exist");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchNotExist()
        {
            GoToAdmin("settingsmail#?notifyTab=formats");

            Driver.GetGridIdFilter("grid", "Registration Format Name Test 777");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist tax");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchMuchSymbols()
        {
            GoToAdmin("settingsmail#?notifyTab=formats");

            Driver.GetGridIdFilter("grid",
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchInvalidSymbols()
        {
            GoToAdmin("settingsmail#?notifyTab=formats");

            Driver.GetGridIdFilter("grid", "########@@@@@@@@&&&&&&&******,,,,..");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}