using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Taxes
{
    [TestFixture]
    public class SettingsTaxesPresentTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Taxes);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Taxes\\Catalog.Tax.csv",
                "data\\Admin\\Settings\\Taxes\\Settings.Settings.csv"
            );

            Init();

            GoToAdmin("settingscheckout#?checkoutTab=taxes");
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
            Driver.GridPaginationSelectItems("10", "gridTaxes");
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "line 1");
            VerifyAreEqual("Tax 107", Driver.GetGridCell(9, "Name", "Taxes").Text, "line 10");
        }

        [Test]
        public void Present20()
        {
            Driver.GridPaginationSelectItems("20", "gridTaxes");
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "line 1");
            VerifyAreEqual("Tax 2", Driver.GetGridCell(19, "Name", "Taxes").Text, "line 20");
        }

        [Test]
        public void Present50()
        {
            Driver.GridPaginationSelectItems("50", "gridTaxes");
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "line 1");
            VerifyAreEqual("Tax 47", Driver.GetGridCell(49, "Name", "Taxes").Text, "line 50");
        }

        [Test]
        public void Present100()
        {
            Driver.GridPaginationSelectItems("100", "gridTaxes");
            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "line 1");
            VerifyAreEqual("Tax 92", Driver.GetGridCell(99, "Name", "Taxes").Text, "line 100");
        }
    }
}