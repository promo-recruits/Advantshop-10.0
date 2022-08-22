using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog.Currencies
{
    [TestFixture]
    public class SettingsCurrenciesSortTest : BaseSeleniumTest
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
        public void SortByName()
        {
            Driver.GetGridCell(-1, "Name", "Currencies").Click();
            VerifyAreEqual("TestCurrencyName1", Driver.GetGridCell(0, "Name", "Currencies").Text,
                "sort by name asc line 1");
            VerifyAreEqual("TestCurrencyName16", Driver.GetGridCell(9, "Name", "Currencies").Text,
                "sort by name asc line 10");

            Driver.GetGridCell(-1, "Name", "Currencies").Click();
            VerifyAreEqual("TestCurrencyName99", Driver.GetGridCell(0, "Name", "Currencies").Text,
                "sort by name desc line 1");
            VerifyAreEqual("TestCurrencyName90", Driver.GetGridCell(9, "Name", "Currencies").Text,
                "sort by name desc line 10");
        }

        [Test]
        public void SortBySymbol()
        {
            Driver.GetGridCell(-1, "Symbol", "Currencies").Click();
            VerifyAreEqual("$1", Driver.GetGridCell(0, "Symbol", "Currencies").Text, "sort by Symbol asc line 1");
            VerifyAreEqual("$18", Driver.GetGridCell(9, "Symbol", "Currencies").Text, "sort by Symbol asc line 10");

            Driver.GetGridCell(-1, "Symbol", "Currencies").Click();
            VerifyAreEqual("руб.9", Driver.GetGridCell(0, "Symbol", "Currencies").Text, "sort by Symbol desc line 1");
            VerifyAreEqual("руб.31", Driver.GetGridCell(9, "Symbol", "Currencies").Text, "sort by Symbol desc line 10");
        }

        [Test]
        public void SortByRate()
        {
            Driver.GetGridCell(-1, "Rate", "Currencies").Click();
            VerifyAreEqual("13.4564", Driver.GetGridCell(0, "Rate", "Currencies").Text, "sort by Rate asc line 1");
            VerifyAreEqual("19.4567", Driver.GetGridCell(9, "Rate", "Currencies").Text, "sort by Rate asc line 10");

            Driver.GetGridCell(-1, "Rate", "Currencies").Click();
            VerifyAreEqual("110.4567", Driver.GetGridCell(0, "Rate", "Currencies").Text, "sort by Rate desc line 1");
            VerifyAreEqual("101.4567", Driver.GetGridCell(9, "Rate", "Currencies").Text, "sort by Rate desc line 10");
        }

        [Test]
        public void SortByIso3()
        {
            Driver.GetGridCell(-1, "Iso3", "Currencies").Click();
            VerifyAreEqual("R1", Driver.GetGridCell(0, "Iso3", "Currencies").Text, "sort by Iso3 asc line 1");
            VerifyAreEqual("R18", Driver.GetGridCell(9, "Iso3", "Currencies").Text, "sort by Iso3 asc line 10");

            Driver.GetGridCell(-1, "Iso3", "Currencies").Click();
            VerifyAreEqual("U2", Driver.GetGridCell(0, "Iso3", "Currencies").Text, "sort by Iso3 desc line 1");
            VerifyAreEqual("R92", Driver.GetGridCell(9, "Iso3", "Currencies").Text, "sort by Iso3 desc line 10");
        }


        [Test]
        public void SortByNumIso3()
        {
            Driver.GetGridCell(-1, "NumIso3", "Currencies").Click();
            Driver.XPathContainsText("h3", "Валюты");
            VerifyAreEqual("1", Driver.GetGridCell(0, "NumIso3", "Currencies").Text, "sort by NumIso3 asc line 1");
            VerifyAreEqual("10", Driver.GetGridCell(9, "NumIso3", "Currencies").Text, "sort by NumIso3 asc line 10");

            Driver.GetGridCell(-1, "NumIso3", "Currencies").Click();
            Driver.XPathContainsText("h3", "Валюты");
            VerifyAreEqual("101", Driver.GetGridCell(0, "NumIso3", "Currencies").Text, "sort by NumIso3 desc line 1");
            VerifyAreEqual("92", Driver.GetGridCell(9, "NumIso3", "Currencies").Text, "sort by NumIso3 desc line 10");
        }

        [Test]
        public void SortByIsCodeBefore()
        {
            Driver.GetGridCell(-1, "IsCodeBefore", "Currencies").Click();
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsCodeBefore", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort by IsCodeBefore asc line 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "IsCodeBefore", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort by IsCodeBefore asc line 10");

            string nameAsc1 = Driver.GetGridCell(0, "Name", "Currencies").Text;
            string nameAsc10 = Driver.GetGridCell(9, "Name", "Currencies").Text;

            VerifyIsFalse(nameAsc1.Equals(nameAsc10), "sort by IsCodeBefore asc diff name");

            Driver.GetGridCell(-1, "IsCodeBefore", "Currencies").Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsCodeBefore", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort by IsCodeBefore desc line 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsCodeBefore", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort by IsCodeBefore desc line 10");

            string nameDesc1 = Driver.GetGridCell(0, "Name", "Currencies").Text;
            string nameDesc10 = Driver.GetGridCell(9, "Name", "Currencies").Text;

            VerifyIsFalse(nameDesc1.Equals(nameDesc10), "sort by IsCodeBefore desc diff name");
        }


        [Test]
        public void SortByRoundNumbers()
        {
            Driver.GetGridCell(-1, "RoundNumbers", "Currencies").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "RoundNumbers", "Currencies").Text.Contains("Не округлять"),
                "sort by RoundNumbers asc line 1");
            VerifyIsTrue(Driver.GetGridCell(9, "RoundNumbers", "Currencies").Text.Contains("Не округлять"),
                "sort by RoundNumbers asc line 10");

            string nameAsc1 = Driver.GetGridCell(0, "Name", "Currencies").Text;
            string nameAsc10 = Driver.GetGridCell(9, "Name", "Currencies").Text;

            VerifyIsFalse(nameAsc1.Equals(nameAsc10), "sort by RoundNumbers asc diff name");

            Driver.GetGridCell(-1, "RoundNumbers", "Currencies").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "RoundNumbers", "Currencies").Text.Contains("Округлять до тысяч"),
                "sort by RoundNumbers desc line 1");
            VerifyIsTrue(Driver.GetGridCell(9, "RoundNumbers", "Currencies").Text.Contains("Округлять до тысяч"),
                "sort by RoundNumbers desc line 10");

            string nameDesc1 = Driver.GetGridCell(0, "Name", "Currencies").Text;
            string nameDesc10 = Driver.GetGridCell(9, "Name", "Currencies").Text;

            VerifyIsFalse(nameDesc1.Equals(nameDesc10), "sort by RoundNumbers desc diff name");
        }
    }
}