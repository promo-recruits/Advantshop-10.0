using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Taxes
{
    [TestFixture]
    public class SettingsTaxesSortTest : BaseSeleniumTest
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
        public void ByName()
        {
            Driver.GetGridCell(-1, "Name", "Taxes").Click();
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "Name 1 asc");
            VerifyAreEqual("Tax 107", Driver.GetGridCell(9, "Name", "Taxes").Text, "Name 10 asc");

            Driver.GetGridCell(-1, "Name", "Taxes").Click();
            VerifyAreEqual("Tax 99", Driver.GetGridCell(0, "Name", "Taxes").Text, "Name 1 desc");
            VerifyAreEqual("Tax 90", Driver.GetGridCell(9, "Name", "Taxes").Text, "Name 10 desc");
        }

        [Test]
        public void ByEnabled()
        {
            Driver.GetGridCell(-1, "Enabled", "Taxes").Click();
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "Enabled 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "Enabled 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "Name", "Taxes").Text;
            string ascLine10 = Driver.GetGridCell(9, "Name", "Taxes").Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different taxes");

            Driver.GetGridCell(-1, "Enabled", "Taxes").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "Enabled 1 desc");
            VerifyIsTrue(Driver.GetGridCell(9, "Enabled", "Taxes").FindElement(By.TagName("input")).Selected,
                "Enabled 10 desc");

            string descLine1 = Driver.GetGridCell(0, "Name", "Taxes").Text;
            string descLine10 = Driver.GetGridCell(9, "Name", "Taxes").Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different taxes");
        }

        [Test]
        public void ByTaxType()
        {
            Driver.GetGridCell(-1, "TaxTypeFormatted", "Taxes").Click();
            VerifyAreEqual("Не указано", Driver.GetGridCell(0, "TaxTypeFormatted", "Taxes").Text, "TaxType 1 asc");
            VerifyAreEqual("Не указано", Driver.GetGridCell(9, "TaxTypeFormatted", "Taxes").Text, "TaxType 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "Name", "Taxes").Text;
            string ascLine10 = Driver.GetGridCell(9, "Name", "Taxes").Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different taxes");

            Driver.GetGridCell(-1, "TaxTypeFormatted", "Taxes").Click();
            VerifyAreEqual("НДС по ставке 18%", Driver.GetGridCell(0, "TaxTypeFormatted", "Taxes").Text,
                "TaxType 1 desc");
            VerifyAreEqual("НДС по ставке 18%", Driver.GetGridCell(9, "TaxTypeFormatted", "Taxes").Text,
                "TaxType 10 desc");

            string descLine1 = Driver.GetGridCell(0, "Name", "Taxes").Text;
            string descLine10 = Driver.GetGridCell(9, "Name", "Taxes").Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different taxes");
        }

        [Test]
        public void ByRate()
        {
            Driver.GetGridCell(-1, "Rate", "Taxes").Click();
            VerifyAreEqual("0", Driver.GetGridCell(0, "Rate", "Taxes").Text, "Rate 1 asc");
            VerifyAreEqual("9", Driver.GetGridCell(9, "Rate", "Taxes").Text, "Rate 10 asc");

            Driver.GetGridCell(-1, "Rate", "Taxes").Click();
            VerifyAreEqual("106", Driver.GetGridCell(0, "Rate", "Taxes").Text, "Rate 1 desc");
            VerifyAreEqual("97", Driver.GetGridCell(9, "Rate", "Taxes").Text, "Rate 10 desc");
        }
    }
}