using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Search.Tests.Search
{
    [TestFixture]
    public class SettingsSearchSortTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.SettingsSearch);
            InitializeService.LoadData("data\\Admin\\SettingsSearch\\Settings.SettingsSearch.csv");

            Init();

            GoToAdmin("settingssearch");
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
        public void SettingsSearchSortTitle()
        {
            Driver.GetGridCell(-1, "Title").Click();

            VerifyAreEqual("test title 1", Driver.GetGridCellText(0, "Title"), "sort title 1 asc");
            VerifyAreEqual("test title 107", Driver.GetGridCellText(9, "Title"), "sort title 10 asc");

            Driver.GetGridCell(-1, "Title").Click();

            VerifyAreEqual("test title 99", Driver.GetGridCellText(0, "Title"), "sort title 1 desc");
            VerifyAreEqual("test title 90", Driver.GetGridCellText(9, "Title"), "sort title 10 desc");
        }

        [Test]
        public void SettingsSearchSortLink()
        {
            Driver.GetGridCell(-1, "Link").Click();

            VerifyAreEqual("link1", Driver.GetGridCellText(0, "Link"), "sort link 1 asc");
            VerifyAreEqual("link107", Driver.GetGridCellText(9, "Link"), "sort link 10 asc");

            Driver.GetGridCell(-1, "Link").Click();

            VerifyAreEqual("settingsseo#?seoTab=seo301", Driver.GetGridCellText(0, "Link"), "sort link 1 desc");
            VerifyAreEqual("link91", Driver.GetGridCellText(9, "Link"), "sort link 10 desc");
        }

        [Test]
        public void SettingsSearchSortKeywords()
        {
            Driver.GetGridCell(-1, "KeyWords").Click();

            VerifyAreEqual("301 редирект", Driver.GetGridCellText(0, "KeyWords"), "sort KeyWords 1 desc");
            VerifyAreEqual("keywords 106", Driver.GetGridCellText(9, "KeyWords"), "sort KeyWords 10 desc");

            Driver.GetGridCell(-1, "KeyWords").Click();

            VerifyAreEqual("keywords 99", Driver.GetGridCellText(0, "KeyWords"), "sort KeyWords 1 asc");
            VerifyAreEqual("keywords 90", Driver.GetGridCellText(9, "KeyWords"), "sort KeyWords 10 asc");
        }

        [Test]
        public void SettingsSearchSortSortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder").Click();

            VerifyAreEqual("10", Driver.GetGridCellText(0, "SortOrder"), "sort SortOrder 1 asc");
            VerifyAreEqual("100", Driver.GetGridCellText(9, "SortOrder"), "sort SortOrder 10 asc");

            Driver.GetGridCell(-1, "SortOrder").Click();

            VerifyAreEqual("1500", Driver.GetGridCellText(0, "SortOrder"), "sort SortOrder 1 desc");
            VerifyAreEqual("1410", Driver.GetGridCellText(9, "SortOrder"), "sort SortOrder 10 desc");
        }
    }
}