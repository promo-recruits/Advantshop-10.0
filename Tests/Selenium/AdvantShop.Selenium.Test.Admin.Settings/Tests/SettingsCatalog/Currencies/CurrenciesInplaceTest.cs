using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog.Currencies
{
    [TestFixture]
    public class SettingsCurrenciesInplaceTest : BaseSeleniumTest
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
        [Order(10)]
        public void InplaceName()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");
            Driver.GetGridIdFilter("gridCurrencies", "TestCurrencyName71");
            Driver.XPathContainsText("h3", "Валюты");

            Driver.SendKeysGridCell("edited currency name", 0, "Name", "Currencies");

            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies", "edited currency name");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyAreEqual("edited currency name", Driver.GetGridCell(0, "Name", "Currencies").Text,
                "new name inplace edited after refreshing");

            Driver.GetGridIdFilter("gridCurrencies", "TestCurrencyName71");
            Driver.XPathContainsText("h3", "Валюты");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCurrencies\"]")).Text
                    .Contains("Ни одной записи не найдено"), "prev name inplace edited");
        }

        [Test]
        [Order(1)]
        public void InplaceSymbol()
        {
            VerifyAreEqual("руб.4", Driver.GetGridCell(1, "Symbol", "Currencies").Text, "pre check currencies symbol");

            Driver.SendKeysGridCell("TEST", 1, "Symbol", "Currencies");

            VerifyAreEqual("TEST", Driver.GetGridCell(1, "Symbol", "Currencies").Text, "symbol inplace edited");

            GoToAdmin("settingscatalog#?catalogTab=currency");

            VerifyAreEqual("TEST", Driver.GetGridCell(1, "Symbol", "Currencies").Text,
                "new symbol inplace edited after refreshing");
        }


        [Test]
        [Order(1)]
        public void InplaceRate()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            VerifyAreEqual("109.4567", Driver.GetGridCell(2, "Rate", "Currencies").Text, "pre check currencies Rate");

            Driver.SendKeysGridCell("89", 2, "Rate", "Currencies");

            VerifyAreEqual("89", Driver.GetGridCell(2, "Rate", "Currencies").Text, "Rate inplace edited");

            GoToAdmin("settingscatalog#?catalogTab=currency");

            VerifyAreEqual("89", Driver.GetGridCell(2, "Rate", "Currencies").Text,
                "new Rate inplace edited after refreshing");
        }


        [Test]
        [Order(1)]
        public void InplaceIso3()
        {
            VerifyAreEqual("U2", Driver.GetGridCell(3, "Iso3", "Currencies").Text, "pre check currencies Iso3");

            Driver.SendKeysGridCell("AAA", 3, "Iso3", "Currencies");

            VerifyAreEqual("AAA", Driver.GetGridCell(3, "Iso3", "Currencies").Text, "Iso3 inplace edited");

            GoToAdmin("settingscatalog#?catalogTab=currency");

            VerifyAreEqual("AAA", Driver.GetGridCell(3, "Iso3", "Currencies").Text,
                "new Iso3 inplace edited after refreshing");
        }

        [Test]
        [Order(1)]
        public void InplaceNumIso3()
        {
            VerifyAreEqual("11", Driver.GetGridCell(4, "NumIso3", "Currencies").Text, "pre check currencies NumIso3");

            Driver.SendKeysGridCell("777", 4, "NumIso3", "Currencies");

            VerifyAreEqual("777", Driver.GetGridCell(4, "NumIso3", "Currencies").Text, "NumIso3 inplace edited");

            GoToAdmin("settingscatalog#?catalogTab=currency");

            VerifyAreEqual("777", Driver.GetGridCell(4, "NumIso3", "Currencies").Text,
                "new NumIso3 inplace edited after refreshing");
        }

        [Test]
        [Order(1)]
        public void InplaceIsCodeBefore()
        {
            VerifyIsFalse(
                Driver.GetGridCell(5, "IsCodeBefore", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "pre check IsCodeBefore");

            Driver.GetGridCellInputForm(5, "IsCodeBefore", "Currencies")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(5, "IsCodeBefore", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "IsCodeBefore inplace edited");
            Thread.Sleep(1500);

            GoToAdmin("settingscatalog#?catalogTab=currency"); //refresh();

            VerifyIsTrue(
                Driver.GetGridCell(5, "IsCodeBefore", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "IsCodeBefore inplace edited after refreshing");
        }

        [Test]
        [Order(1)]
        public void InplaceRoundNumbers()
        {
            VerifyIsTrue(Driver.GetGridCell(2, "RoundNumbers", "Currencies").Text.Contains("Округлять до десятков"),
                "pre check RoundNumbers");

            Driver.MouseFocus(Driver.GetGridCell(2, "RoundNumbers", "Currencies"));
            Driver.GetGridCell(2, "RoundNumbers", "Currencies").FindElement(By.CssSelector("[data-e2e=\"select\"]"))
                .Click();
            Thread.Sleep(2000);
            Driver.FindElements(By.CssSelector(".ui-select-choices-row"))[4]
                .Click(); //Если падает на этом месте, значит добавили новый элемент

            Thread.Sleep(2000);
            Driver.XPathContainsText("h3", "Валюты");

            VerifyIsTrue(Driver.GetGridCell(2, "RoundNumbers", "Currencies").Text.Contains("Округлять до сотен"),
                "RoundNumbers inplace edited");

            GoToAdmin("settingscatalog#?catalogTab=currency");

            VerifyIsTrue(Driver.GetGridCell(2, "RoundNumbers", "Currencies").Text.Contains("Округлять до сотен"),
                "RoundNumbers inplace edited after refreshing");
        }
    }
}