using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog.Currencies
{
    [TestFixture]
    public class SettingsCurrencyAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Currencies);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrenciesAddEdit\\Catalog.Currency.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrenciesAddEdit\\Catalog.Product.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrenciesAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrenciesAddEdit\\Catalog.Category.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrenciesAddEdit\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrenciesAddEdit\\Settings.Settings.csv"
            );

            Init();
            EnableInplaceOff();
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

        public void setAddedCurrencyToProduct(string productId, string currencyName)
        {
            GoToAdmin("product/edit/" + productId);
            Driver.ScrollTo(By.Id("BarCode"));
            (new SelectElement(Driver.FindElement(By.Id("CurrencyId")))).SelectByText(currencyName);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(2000);
        }

        [Test]
        [Order(0)]
        public void CurrenciesAddIsCodeBefore()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");
            //data-e2e="CurrenciesSettingAdd"
            Driver.FindElement(By.CssSelector("[data-e2e=\"CurrenciesSettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Новая валюта"), "pop up h1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).SendKeys("Added Currency Code Before");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).SendKeys("TEST");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).SendKeys("20.456");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).SendKeys("TST");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).SendKeys("111");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIsCodeBefore\"]")).FindElement(By.TagName("span"))
                .Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]")))).SelectByText(
                "Не округлять");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //check to default currency added
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.ScrollTo(By.TagName("footer"));
            IWebElement selectElemDefaultCurrency = Driver.FindElement(By.Name("DefaultCurrencyIso3"));
            SelectElement select = new SelectElement(selectElemDefaultCurrency);

            IList<IWebElement> allOptionsDefaultCurrency = select.Options;

            VerifyIsTrue(allOptionsDefaultCurrency.Count == 2, "currency added to default list");

            (new SelectElement(Driver.FindElement(By.Name("DefaultCurrencyIso3")))).SelectByText(
                "Added Currency Code Before");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin currency grid
            GoToAdmin("settingscatalog#?catalogTab=currency");

            selectElemDefaultCurrency = Driver.FindElement(By.Name("DefaultCurrencyIso3"));
            SelectElement select1 = new SelectElement(selectElemDefaultCurrency);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Added Currency Code Before"),
                "currencies added set default admin");

            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "currencies added count");

            Driver.GetGridIdFilter("gridCurrencies", "Added Currency Code Before");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyAreEqual("Added Currency Code Before", Driver.GetGridCell(0, "Name", "Currencies").Text,
                "currencies added grid name");
            VerifyAreEqual("TEST", Driver.GetGridCell(0, "Symbol", "Currencies").Text, "currencies added grid symbol");
            VerifyAreEqual("20.456", Driver.GetGridCell(0, "Rate", "Currencies").Text, "currencies added grid Rate");
            VerifyAreEqual("TST", Driver.GetGridCell(0, "Iso3", "Currencies").Text, "currencies added grid Iso3");
            VerifyAreEqual("111", Driver.GetGridCell(0, "NumIso3", "Currencies").Text, "currencies added grid NumIso3");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsCodeBefore", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "currencies added grid IsCodeBefore");
            VerifyIsTrue(Driver.GetGridCell(0, "RoundNumbers", "Currencies").Text.Contains("Не округлять"),
                "currencies added grid RoundNumbers");

            //check admin currency edit pop up 
            Driver.GetGridCell(0, "_serviceColumn", "Currencies")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Added Currency Code Before",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).GetAttribute("value"),
                "currencies added name pop up");
            VerifyAreEqual("TEST",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).GetAttribute("value"),
                "currencies added symbol pop up");
            VerifyAreEqual("20.456",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).GetAttribute("value"),
                "currencies added rate pop up");
            VerifyAreEqual("TST",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).GetAttribute("value"),
                "currencies added iso3 pop up");
            VerifyAreEqual("111",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).GetAttribute("value"),
                "currencies added num iso3 pop up");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIsCodeBefore\"]"))
                    .FindElement(By.TagName("input")).Selected, "currencies added IsCodeBefore pop up");

            IWebElement selectElemRoundNumbers =
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]"));
            SelectElement select2 = new SelectElement(selectElemRoundNumbers);
            VerifyIsTrue(select2.SelectedOption.Text.Contains("Не округлять"), "currencies added Round Numbers pop up");

            //check admin product cart
            GoToAdmin("product/edit/39");

            Driver.ScrollTo(By.Id("BarCode"));
            IWebElement selectElemProductCurrency = Driver.FindElement(By.Id("CurrencyId"));
            SelectElement select3 = new SelectElement(selectElemProductCurrency);

            IList<IWebElement> allOptionsProductCurrency = select3.Options;

            VerifyIsTrue(allOptionsProductCurrency.Count == 2, "admin product cart currency added");

            //check client is code before
            GoToClient("products/test-product39");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-payment-block"))
                    .FindElement(By.CssSelector(".price-current.cs-t-1")).Text.Contains("TEST126,7422"),
                "client currency added code before");
        }

        [Test]
        [Order(1)]
        public void CurrenciesAddIsCodeAfter()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CurrenciesSettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).SendKeys("Added Currency Code After");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).SendKeys("After");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).SendKeys("21.456");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).SendKeys("AFT");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).SendKeys("222");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]")))).SelectByText(
                "Не округлять");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //check to default currency added
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.ScrollTo(By.TagName("footer"));
            (new SelectElement(Driver.FindElement(By.Name("DefaultCurrencyIso3")))).SelectByText(
                "Added Currency Code After");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin currency grid
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies", "Added Currency Code After");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyAreEqual("Added Currency Code After", Driver.GetGridCell(0, "Name", "Currencies").Text,
                "currencies added grid name");
            VerifyAreEqual("After", Driver.GetGridCell(0, "Symbol", "Currencies").Text, "currencies added grid symbol");
            VerifyAreEqual("21.456", Driver.GetGridCell(0, "Rate", "Currencies").Text, "currencies added grid Rate");
            VerifyAreEqual("AFT", Driver.GetGridCell(0, "Iso3", "Currencies").Text, "currencies added grid Iso3");
            VerifyAreEqual("222", Driver.GetGridCell(0, "NumIso3", "Currencies").Text, "currencies added grid NumIso3");
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsCodeBefore", "Currencies")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "currencies added grid IsCodeBefore");
            VerifyIsTrue(Driver.GetGridCell(0, "RoundNumbers", "Currencies").Text.Contains("Не округлять"),
                "currencies added grid RoundNumbers");

            //check admin currency edit pop up 
            Driver.GetGridCell(0, "_serviceColumn", "Currencies")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Added Currency Code After",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).GetAttribute("value"),
                "currencies added name pop up");
            VerifyAreEqual("After",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).GetAttribute("value"),
                "currencies added symbol pop up");
            VerifyAreEqual("21.456",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).GetAttribute("value"),
                "currencies added rate pop up");
            VerifyAreEqual("AFT",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).GetAttribute("value"),
                "currencies added iso3 pop up");
            VerifyAreEqual("222",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).GetAttribute("value"),
                "currencies added num iso3 pop up");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIsCodeBefore\"]"))
                    .FindElement(By.TagName("input")).Selected, "currencies added IsCodeBefore pop up");

            IWebElement selectElemRoundNumbers =
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]"));
            SelectElement select1 = new SelectElement(selectElemRoundNumbers);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Не округлять"), "currencies added Round Numbers pop up");

            //check client is code After
            GoToClient("products/test-product40");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-payment-block"))
                    .FindElement(By.CssSelector(".price-current.cs-t-1")).Text.Contains("123,9334After"),
                "client currency added code After");
        }

        [Test]
        [Order(1)]
        public void CurrenciesAddRoundNumCop()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CurrenciesSettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).SendKeys("Round Number Cop");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).SendKeys("R");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).SendKeys("66.4779");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).SendKeys("COP");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).SendKeys("333");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]")))).SelectByText(
                "Округлять до копеек");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIsCodeBefore\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //set added currency as default
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.ScrollTo(By.TagName("footer"));
            (new SelectElement(Driver.FindElement(By.Name("DefaultCurrencyIso3")))).SelectByText("Round Number Cop");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin currency grid
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies", "Round Number Cop");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyAreEqual("Round Number Cop", Driver.GetGridCell(0, "Name", "Currencies").Text,
                "currencies added grid name");
            VerifyIsTrue(Driver.GetGridCell(0, "RoundNumbers", "Currencies").Text.Contains("Округлять до копеек"),
                "currencies added grid RoundNumbers");

            //check admin currency edit pop up 
            Driver.GetGridCell(0, "_serviceColumn", "Currencies")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Round Number Cop",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).GetAttribute("value"),
                "currencies added name pop up");

            IWebElement selectElemRoundNumbers =
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]"));
            SelectElement select1 = new SelectElement(selectElemRoundNumbers);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Округлять до копеек"),
                "currencies added Round Numbers pop up");

            setAddedCurrencyToProduct("41", "Round Number Cop");

            //check client 
            GoToClient("products/test-product41");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-block")).Text.Contains("41,46"),
                "client currency added round num");
        }

        [Test]
        [Order(1)]
        public void CurrenciesAddRoundNumNo()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CurrenciesSettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).SendKeys("Round Number No");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).SendKeys("R1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).SendKeys("66.4779");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).SendKeys("Noo");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).SendKeys("666");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]")))).SelectByText(
                "Не округлять");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIsCodeBefore\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //set added currency as default
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.ScrollTo(By.TagName("footer"));
            (new SelectElement(Driver.FindElement(By.Name("DefaultCurrencyIso3")))).SelectByText("Round Number No");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin currency grid
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies", "Round Number No");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyAreEqual("Round Number No", Driver.GetGridCell(0, "Name", "Currencies").Text,
                "currencies added grid name");
            VerifyIsTrue(Driver.GetGridCell(0, "RoundNumbers", "Currencies").Text.Contains("Не округлять"),
                "currencies added grid RoundNumbers");

            //check admin currency edit pop up 
            Driver.GetGridCell(0, "_serviceColumn", "Currencies")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Round Number No",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).GetAttribute("value"),
                "currencies added name pop up");

            IWebElement selectElemRoundNumbers =
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]"));
            SelectElement select1 = new SelectElement(selectElemRoundNumbers);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Не округлять"), "currencies added Round Numbers pop up");

            setAddedCurrencyToProduct("42", "Round Number No");

            //check client 
            GoToClient("products/test-product42");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-payment-block"))
                    .FindElement(By.CssSelector(".price-current.cs-t-1")).Text.Contains("42,4544"),
                "client currency added round num");
        }

        [Test]
        [Order(1)]
        public void CurrenciesAddRoundNumInt()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CurrenciesSettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).SendKeys("Round Number Int");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).SendKeys("R2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).SendKeys("66.4779");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).SendKeys("Int");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).SendKeys("444");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]")))).SelectByText(
                "Округлять до целого");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIsCodeBefore\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //set added currency as default
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.ScrollTo(By.TagName("footer"));
            (new SelectElement(Driver.FindElement(By.Name("DefaultCurrencyIso3")))).SelectByText("Round Number Int");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin currency grid
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies", "Round Number Int");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyAreEqual("Round Number Int", Driver.GetGridCell(0, "Name", "Currencies").Text,
                "currencies added grid name");
            VerifyIsTrue(Driver.GetGridCell(0, "RoundNumbers", "Currencies").Text.Contains("Округлять до целого"),
                "currencies added grid RoundNumbers");

            //check admin currency edit pop up 
            Driver.GetGridCell(0, "_serviceColumn", "Currencies")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Round Number Int",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).GetAttribute("value"),
                "currencies added name pop up");

            IWebElement selectElemRoundNumbers =
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]"));
            SelectElement select1 = new SelectElement(selectElemRoundNumbers);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Округлять до целого"),
                "currencies added Round Numbers pop up");

            setAddedCurrencyToProduct("43", "Round Number Int");

            //check client 
            GoToClient("products/test-product43");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-payment-block"))
                    .FindElement(By.CssSelector(".price-current.cs-t-1")).Text.Contains("43"),
                "client currency added round num");
        }

        [Test]
        [Order(1)]
        public void CurrenciesAddRoundNumDozens()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CurrenciesSettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).SendKeys("Round Number Dozens");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).SendKeys("R3");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).SendKeys("66.4779");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).SendKeys("DOZ");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).SendKeys("555");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]")))).SelectByText(
                "Округлять до десятков");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIsCodeBefore\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //set added currency as default
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.ScrollTo(By.TagName("footer"));
            (new SelectElement(Driver.FindElement(By.Name("DefaultCurrencyIso3")))).SelectByText("Round Number Dozens");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin currency grid
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies", "Round Number Dozens");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyAreEqual("Round Number Dozens", Driver.GetGridCell(0, "Name", "Currencies").Text,
                "currencies added grid name");
            VerifyIsTrue(Driver.GetGridCell(0, "RoundNumbers", "Currencies").Text.Contains("Округлять до десятков"),
                "currencies added grid RoundNumbers");

            //check admin currency edit pop up 
            Driver.GetGridCell(0, "_serviceColumn", "Currencies")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Round Number Dozens",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).GetAttribute("value"),
                "currencies added name pop up");

            IWebElement selectElemRoundNumbers =
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]"));
            SelectElement select1 = new SelectElement(selectElemRoundNumbers);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Округлять до десятков"),
                "currencies added Round Numbers pop up");

            setAddedCurrencyToProduct("44", "Round Number Dozens");

            //check client 
            GoToClient("products/test-product44");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-payment-block"))
                    .FindElement(By.CssSelector(".price-current.cs-t-1")).Text.Contains("450"),
                "client currency added round num");
        }

        [Test]
        [Order(1)]
        public void CurrenciesAddRoundNumHundreds()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CurrenciesSettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).SendKeys("Round Number Hundreds");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).SendKeys("R4");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).SendKeys("66.4779");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).SendKeys("HUN");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).SendKeys("777");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]")))).SelectByText(
                "Округлять до сотен");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIsCodeBefore\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //set added currency as default
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.ScrollTo(By.TagName("footer"));
            (new SelectElement(Driver.FindElement(By.Name("DefaultCurrencyIso3")))).SelectByText(
                "Round Number Hundreds");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin currency grid
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies", "Round Number Hundreds");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyAreEqual("Round Number Hundreds", Driver.GetGridCell(0, "Name", "Currencies").Text,
                "currencies added grid name");
            VerifyIsTrue(Driver.GetGridCell(0, "RoundNumbers", "Currencies").Text.Contains("Округлять до сотен"),
                "currencies added grid RoundNumbers");

            //check admin currency edit pop up 
            Driver.GetGridCell(0, "_serviceColumn", "Currencies")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Round Number Hundreds",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).GetAttribute("value"),
                "currencies added name pop up");

            IWebElement selectElemRoundNumbers =
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]"));
            SelectElement select1 = new SelectElement(selectElemRoundNumbers);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Округлять до сотен"),
                "currencies added Round Numbers pop up");

            setAddedCurrencyToProduct("45", "Round Number Hundreds");

            //check client 
            GoToClient("products/test-product45");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-block")).Text.Contains("4 500"),
                "client currency added round num");
        }

        [Test]
        [Order(1)]
        public void CurrenciesAddRoundNumThousands()
        {
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CurrenciesSettingAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).SendKeys("Round Number Thousands");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencySymbol\"]")).SendKeys("R5");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRate\"]")).SendKeys("66.4779");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIso3\"]")).SendKeys("THU");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyNumIso3\"]")).SendKeys("888");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]")))).SelectByText(
                "Округлять до тысяч");

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyIsCodeBefore\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"currencyButtonSave\"]")).Click();
            Thread.Sleep(2000);

            //set added currency as default
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.ScrollTo(By.TagName("footer"));
            (new SelectElement(Driver.FindElement(By.Name("DefaultCurrencyIso3")))).SelectByText(
                "Round Number Thousands");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
            Thread.Sleep(2000);

            //check admin currency grid
            GoToAdmin("settingscatalog#?catalogTab=currency");

            Driver.GetGridIdFilter("gridCurrencies", "Round Number Thousands");
            Driver.XPathContainsText("h3", "Валюты");

            VerifyAreEqual("Round Number Thousands", Driver.GetGridCell(0, "Name", "Currencies").Text,
                "currencies added grid name");
            VerifyIsTrue(Driver.GetGridCell(0, "RoundNumbers", "Currencies").Text.Contains("Округлять до тысяч"),
                "currencies added grid RoundNumbers");

            //check admin currency edit pop up 
            Driver.GetGridCell(0, "_serviceColumn", "Currencies")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Round Number Thousands",
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyName\"]")).GetAttribute("value"),
                "currencies added name pop up");

            IWebElement selectElemRoundNumbers =
                Driver.FindElement(By.CssSelector("[data-e2e=\"currencyRoundNumbers\"]"));
            SelectElement select1 = new SelectElement(selectElemRoundNumbers);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Округлять до тысяч"),
                "currencies added Round Numbers pop up");

            setAddedCurrencyToProduct("46", "Round Number Thousands");

            //check client 
            GoToClient("products/test-product46");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-payment-block")).Text.Contains("5 000"),
                "client currency added round num");
        }
    }
}