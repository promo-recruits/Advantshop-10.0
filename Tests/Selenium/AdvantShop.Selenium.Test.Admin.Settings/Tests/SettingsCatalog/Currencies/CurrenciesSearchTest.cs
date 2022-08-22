using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog.Currencies
{
    [TestFixture]
    public class SettingsCurrenciesSearchTest : BaseSeleniumTest
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
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies", "TestCurrencyName55");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyAreEqual("TestCurrencyName55", Driver.GetGridCell(0, "Name", "Currencies").Text, "search exist");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchNotExist()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies", "Market Currency");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]")).Text
                    .Contains("Ни одной записи не найдено"), "search not exist");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchMuchSymbols()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies",
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]")).Text
                    .Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchInvalidSymbols()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies", "########@@@@@@@@&&&&&&&******,,,,..");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]")).Text
                    .Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}