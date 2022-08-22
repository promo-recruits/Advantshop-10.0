using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Search.Tests.Search
{
    [TestFixture]
    public class SettingsSearchPresentTest : BaseSeleniumTest
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
        public void SettingsSearchPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("test title 1", Driver.GetGridCellText(0, "Title"), "present line 1");
            VerifyAreEqual("test title 107", Driver.GetGridCellText(9, "Title"), "present line 10");
       
            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("test title 1", Driver.GetGridCellText(0, "Title"), "present line 1");
            VerifyAreEqual("test title 116", Driver.GetGridCellText(19, "Title"), "present line 20");
        }
    }
}