using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Taxes
{
    [TestFixture]
    public class SettingsTaxesSearchTest : BaseSeleniumTest
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
        public void SearchExistName()
        {
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            Driver.GetGridIdFilter("gridTaxes", "Tax 68");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual("Tax 68", Driver.GetGridCell(0, "Name", "Taxes").Text, "search exist tax");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchNotExist()
        {
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            Driver.GetGridIdFilter("gridTaxes", "Tax Test 123");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist tax");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchMuchSymbols()
        {
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            Driver.GetGridIdFilter("gridTaxes",
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchInvalidSymbols()
        {
            GoToAdmin("settingscheckout#?checkoutTab=taxes");

            Driver.GetGridIdFilter("gridTaxes", "########@@@@@@@@&&&&&&&******,,,,..");
            Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}