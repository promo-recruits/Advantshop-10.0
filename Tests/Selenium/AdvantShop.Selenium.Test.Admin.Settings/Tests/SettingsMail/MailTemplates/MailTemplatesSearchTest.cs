using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailTemplates
{
    [TestFixture]
    public class SettingsMailTemplatesSearchTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.MailTemplate);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsMail\\MailTemplates\\Settings.MailTemplate.csv"
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
            GoToAdmin("settingsmail#?notifyTab=templates");

            Driver.GetGridIdFilter("gridTemplates", "Template Name Test 97");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("Template Name Test 97", Driver.GetGridCell(0, "Name", "Templates").Text, "search exist");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchNotExist()
        {
            GoToAdmin("settingsmail#?notifyTab=templates");

            Driver.GetGridIdFilter("gridTemplates", "Registration Format Name Test 777");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist tax");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchMuchSymbols()
        {
            GoToAdmin("settingsmail#?notifyTab=templates");

            Driver.GetGridIdFilter("gridTemplates",
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchInvalidSymbols()
        {
            GoToAdmin("settingsmail#?notifyTab=templates");

            Driver.GetGridIdFilter("gridTemplates", "########@@@@@@@@&&&&&&&******,,,,..");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h1")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}