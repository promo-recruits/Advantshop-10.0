using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog.Currencies
{
    [TestFixture]
    public class SettingsCurrenciesPageTest : BaseSeleniumTest
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
        public void Page()
        {
            VerifyAreEqual("TestCurrencyName1", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 1 line 1");
            VerifyAreEqual("TestCurrencyName16", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName17", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 2 line 1");
            VerifyAreEqual("TestCurrencyName25", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName26", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 3 line 1");
            VerifyAreEqual("TestCurrencyName34", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName35", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 4 line 1");
            VerifyAreEqual("TestCurrencyName43", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName44", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 5 line 1");
            VerifyAreEqual("TestCurrencyName52", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName53", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 6 line 1");
            VerifyAreEqual("TestCurrencyName61", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName1", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 1 line 1");
            VerifyAreEqual("TestCurrencyName16", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 1 line 10");
        }

        [Test]
        public void PageToPrevious()
        {
            VerifyAreEqual("TestCurrencyName1", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 1 line 1");
            VerifyAreEqual("TestCurrencyName16", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName17", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 2 line 1");
            VerifyAreEqual("TestCurrencyName25", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName26", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 3 line 1");
            VerifyAreEqual("TestCurrencyName34", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName17", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 2 line 1");
            VerifyAreEqual("TestCurrencyName25", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName1", Driver.GetGridCell(0, "Name", "Currencies").Text, "page 1 line 1");
            VerifyAreEqual("TestCurrencyName16", Driver.GetGridCell(9, "Name", "Currencies").Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]"))
                .FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCurrencyName99", Driver.GetGridCell(0, "Name", "Currencies").Text, "last page line 1");
        }
    }
}