using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog.Currencies
{
    [TestFixture]
    public class SettingsCurrenciesPresentTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Currencies);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\Catalog.Currency.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\Settings.Settings.csv"
            );

            Init();


            GoToAdmin("settingscatalog#?catalogTab=currency");
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
        public void Present10()
        {
            Driver.GridPaginationSelectItems("10", "gridCurrencies");
            VerifyAreEqual("TestCurrencyName1", Driver.GetGridCell(0, "Name", "Currencies").Text, "line 1");
            VerifyAreEqual("TestCurrencyName16", Driver.GetGridCell(9, "Name", "Currencies").Text, "line 10");
        }

        [Test]
        public void Present20()
        {
            Driver.GridPaginationSelectItems("20", "gridCurrencies");
            VerifyAreEqual("TestCurrencyName1", Driver.GetGridCell(0, "Name", "Currencies").Text, "line 1");
            VerifyAreEqual("TestCurrencyName25", Driver.GetGridCell(19, "Name", "Currencies").Text, "line 20");
        }

        [Test]
        public void Present50()
        {
            Driver.GridPaginationSelectItems("50", "gridCurrencies");
            VerifyAreEqual("TestCurrencyName1", Driver.GetGridCell(0, "Name", "Currencies").Text, "line 1");
            VerifyAreEqual("TestCurrencyName52", Driver.GetGridCell(49, "Name", "Currencies").Text, "line 50");
        }

        [Test]
        public void Present100()
        {
            Driver.GridPaginationSelectItems("100", "gridCurrencies");
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName1", Driver.GetGridCell(0, "Name", "Currencies").Text, "line 1");
            VerifyAreEqual("TestCurrencyName98", Driver.GetGridCell(99, "Name", "Currencies").Text, "line 100");
        }
    }
}