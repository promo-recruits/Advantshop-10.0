using System.Collections.ObjectModel;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog.Currencies
{
    [TestFixture]
    public class SettingsCurrenciesSettingsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Currencies);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\Catalog.Currency.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\Catalog.Product.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\Catalog.Category.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\Catalog.ProductCategories.csv",
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
        public void CurrenciesGoToInstruction()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.XPathContainsText("span", "Инструкция. Настройка параметров валюты в магазине");

            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.Url.Contains("help") && Driver.Url.Contains("currency"), "check url");
            VerifyIsTrue(Driver.Url.Contains("help") && Driver.Url.Contains("currency"), "check url");

            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "tabs count");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void CurrenciesAllowToChangeOn()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.CheckSelected("AllowToChangeCurrency", "SettingsCatalogSave");

            //check admin
            GoToAdmin("settingscatalog#?catalogTab=currency");
            VerifyIsTrue(Driver.FindElement(By.Id("AllowToChangeCurrency")).Selected, "allow to change currency admin");

            //check client
            GoToClient("categories/test-category1");
            VerifyIsTrue(Driver.FindElement(By.Id("ddlCurrency")).Displayed, "allow to change currency client");

            //check currency change in client
            string currencyBegin = Driver.FindElement(By.CssSelector("[data-product-id=\"6\"]"))
                .FindElement(By.CssSelector(".price-currency")).Text;
            (new SelectElement(Driver.FindElement(By.Id("ddlCurrency")))).SelectByText("TestCurrencyName15");
            Thread.Sleep(2000);

            GoToClient("categories/test-category1");
            string currencyEnd = Driver.FindElement(By.CssSelector("[data-product-id=\"6\"]"))
                .FindElement(By.CssSelector(".price-currency")).Text;

            VerifyIsFalse(currencyBegin.Equals(currencyEnd), "currency changed");
        }

        [Test]
        public void CurrenciesAllowToChangeOff()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.CheckNotSelected("AllowToChangeCurrency", "SettingsCatalogSave");

            //check admin
            GoToAdmin("settingscatalog#?catalogTab=currency");
            VerifyIsFalse(Driver.FindElement(By.Id("AllowToChangeCurrency")).Selected,
                "allow to change currency admin");

            //check client
            GoToClient("categories/test-category1");
            VerifyIsFalse(Driver.FindElements(By.Id("ddlCurrency")).Count > 0, "allow to change currency client");
        }

        [Test]
        public void CurrenciesAutoUpdateOn()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.CheckSelected("AutoUpdateCurrencies", "SettingsCatalogSave");

            GoToAdmin("settingscatalog#?catalogTab=currency");
            VerifyIsTrue(Driver.FindElement(By.Id("AutoUpdateCurrencies")).Selected, "currency auto update admin");
        }

        [Test]
        public void CurrenciesAutoUpdateOff()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");
            Driver.CheckNotSelected("AutoUpdateCurrencies", "SettingsCatalogSave");

            GoToAdmin("settingscatalog#?catalogTab=currency");
            VerifyIsFalse(Driver.FindElement(By.Id("AutoUpdateCurrencies")).Selected, "currency auto update admin");
        }
    }
}