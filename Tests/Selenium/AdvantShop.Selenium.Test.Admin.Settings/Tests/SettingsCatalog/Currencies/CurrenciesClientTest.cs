using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog.Currencies
{
    [TestFixture]
    public class SettingsCurrenciesVariables : BaseSeleniumTest
    {
        public string currencyMainPageBegin,
            currencyMainPagePriceBegin,
            currencyCategoryBegin,
            currencyCategoryPriceBegin,
            currencyFilterBegin,
            currencyFilterFromBegin,
            currencyFilterToBegin,
            currencyProductBegin,
            currencyProductPriceBegin,
            currencyMainPageMobileBegin,
            currencyMainPagePriceMobileBegin,
            currencyCategoryMobileBegin,
            currencyCategoryPriceMobileBegin,
            currencyFilterMobileBegin,
            currencyFilterFromMobileBegin,
            currencyFilterToMobileBegin,
            currencyProductMobileBegin,
            currencyProductPriceMobileBegin;

        public string currencyMainPageEnd,
            currencyMainPagePriceEnd,
            currencyCategoryEnd,
            currencyCategoryPriceEnd,
            currencyFilterEnd,
            currencyFilterFromEnd,
            currencyFilterToEnd,
            currencyProductEnd,
            currencyProductPriceEnd,
            currencyMainPageMobileEnd,
            currencyMainPagePriceMobileEnd,
            currencyCategoryMobileEnd,
            currencyCategoryPriceMobileEnd,
            currencyFilterMobileEnd,
            currencyFilterFromMobileEnd,
            currencyFilterToMobileEnd,
            currencyProductMobileEnd,
            currencyProductPriceMobileEnd;
    }

    public class SettingsCurrenciesClientAdminTest : SettingsCurrenciesVariables
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Currencies | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Catalog.Currency.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Catalog.Product.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Catalog.Category.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Settings.Settings.csv"
            );
            Init();
            EnableInplaceOff();
            GoToAdmin("settingscatalog#?catalogTab=currency");
            IWebElement selectElem = Driver.FindElement(By.Name("DefaultCurrencyIso3"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.AllSelectedOptions[0].Text.Contains("TestCurrencyName99"), "currencies default");

            /* preconditions */
            //pre check client main page
            GoToClient();

            currencyMainPageBegin = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyMainPagePriceBegin = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            //pre check client category
            GoToClient("categories/test-category1");

            currencyCategoryBegin = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyCategoryPriceBegin = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            currencyFilterBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-question-description")).Text;
            currencyFilterFromBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-min")).Text;
            currencyFilterToBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-max")).Text;

            //pre check client product cart
            GoToClient("products/test-product6");

            currencyProductBegin = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyProductPriceBegin = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-number")).Text;

            ReInit();
            //pre check client main page mobile
            GoToMobile();
            Driver.ScrollTo(By.CssSelector(".mainpage-products--best"));

            currencyMainPageMobileBegin = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyMainPagePriceMobileBegin = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            //pre check client category mobile
            GoToMobile("categories/test-category1");

            currencyCategoryMobileBegin = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyCategoryPriceMobileBegin = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            Driver.FindElement(By.ClassName("catalog-filter-trigger--mobile")).Click();
            Thread.Sleep(2000);

            currencyFilterMobileBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-question-description")).Text;
            currencyFilterFromMobileBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-min")).Text;
            currencyFilterToMobileBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-max")).Text;

            //pre check client product cart mobile
            GoToMobile("products/test-product6");

            currencyProductMobileBegin = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyProductPriceMobileBegin = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-number")).Text;

            ReInit();
            /* test */
            //change default currency
            GoToAdmin("settingscatalog#?catalogTab=currency");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));

            (new SelectElement(Driver.FindElement(By.Name("DefaultCurrencyIso3")))).SelectByText("TestCurrencyName81");
            Driver.CheckNotSelected("AllowToChangeCurrency", "SettingsCatalogSave");

            Driver.ScrollToTop();
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Enabled)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCatalogSave\"]")).Click();
                Thread.Sleep(2000);
            }
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
        public void AdminMainPage()
        {
            GoToClient("");

            currencyMainPageEnd = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyMainPagePriceEnd = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            VerifyIsFalse(currencyMainPageBegin.Equals(currencyMainPageEnd), "currency main page");
            VerifyIsFalse(currencyMainPagePriceBegin.Equals(currencyMainPagePriceEnd), "currency price main page");
        }

        [Test]
        public void AdminCategory()
        {
            GoToClient("categories/test-category1");

            currencyCategoryEnd = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyCategoryPriceEnd = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            currencyFilterEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-question-description")).Text;
            currencyFilterFromEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-min")).Text;
            currencyFilterToEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-max")).Text;

            VerifyIsFalse(currencyCategoryBegin.Equals(currencyCategoryEnd), "currency category");
            VerifyIsFalse(currencyCategoryPriceBegin.Equals(currencyCategoryPriceEnd), "currency price category");

            VerifyIsFalse(currencyFilterBegin.Equals(currencyFilterEnd), "currency category filter");
            VerifyIsFalse(currencyFilterFromBegin.Equals(currencyFilterFromEnd), "currency category filter from");
            VerifyIsFalse(currencyFilterToBegin.Equals(currencyFilterToEnd), "currency category filter to");
        }

        [Test]
        public void AdminProductCart()
        {
            GoToClient("products/test-product6");

            currencyProductEnd = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyProductPriceEnd = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-number")).Text;

            VerifyIsFalse(currencyProductBegin.Equals(currencyProductEnd), "currency product cart");
            VerifyIsFalse(currencyProductPriceBegin.Equals(currencyProductPriceEnd), "currency price product cart");
        }

        [Test]
        public void AdminMobileMainPage()
        {
            GoToMobile();
            Driver.ScrollTo(By.CssSelector(".mainpage-products--best"));

            currencyMainPageMobileEnd = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyMainPagePriceMobileEnd = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            VerifyIsFalse(currencyMainPageMobileBegin.Equals(currencyMainPageMobileEnd), "currency main page");
            VerifyIsFalse(currencyMainPagePriceMobileBegin.Equals(currencyMainPagePriceMobileEnd),
                "currency price main page");

            ReInit();
        }

        [Test]
        public void AdminMobileCategory()
        {
            GoToMobile("categories/test-category1");

            currencyCategoryMobileEnd = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyCategoryPriceMobileEnd = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            Driver.XPathContainsText("button", "Фильтры");

            currencyFilterMobileEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-question-description")).Text;
            currencyFilterFromMobileEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-min")).Text;
            currencyFilterToMobileEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-max")).Text;

            VerifyIsFalse(currencyCategoryMobileBegin.Equals(currencyCategoryMobileEnd), "currency category");
            VerifyIsFalse(currencyCategoryPriceMobileBegin.Equals(currencyCategoryPriceMobileEnd),
                "currency price category");

            VerifyIsFalse(currencyFilterMobileBegin.Equals(currencyFilterMobileEnd), "currency category filter");
            VerifyIsFalse(currencyFilterFromMobileBegin.Equals(currencyFilterFromMobileEnd),
                "currency category filter from");
            VerifyIsFalse(currencyFilterToMobileBegin.Equals(currencyFilterToMobileEnd), "currency category filter to");

            ReInit();
        }

        [Test]
        public void AdminMobileProductCart()
        {
            GoToMobile("products/test-product6");

            currencyProductMobileEnd = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyProductPriceMobileEnd = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-number")).Text;

            VerifyIsFalse(currencyProductMobileBegin.Equals(currencyProductMobileEnd), "currency product cart");
            VerifyIsFalse(currencyProductPriceMobileBegin.Equals(currencyProductPriceMobileEnd),
                "currency price product cart");

            ReInit();
        }
    }

    [TestFixture]
    public class SettingsCurrenciesClientClientTest : SettingsCurrenciesVariables
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Currencies);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Catalog.Currency.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Catalog.Product.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Catalog.Category.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\SettingsCatalog\\Currencies\\CurrencyClient\\Settings.Settings.csv"
            );
            Init();
            EnableInplaceOff();

            /* preconditions */
            //pre check client main page

            GoToClient("");

            currencyMainPageBegin = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyMainPagePriceBegin = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            //pre check client category
            GoToClient("categories/test-category1");

            currencyCategoryBegin = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyCategoryPriceBegin = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            currencyFilterBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-question-description")).Text;
            currencyFilterFromBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-min")).Text;
            currencyFilterToBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-max")).Text;

            //pre check client product cart
            GoToClient("products/test-product6");

            currencyProductBegin = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyProductPriceBegin = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-number")).Text;

            //pre check client main page mobile
            GoToMobile("");
            Driver.ScrollTo(By.CssSelector(".mainpage-products--best"));

            currencyMainPageMobileBegin = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyMainPagePriceMobileBegin = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            //pre check client category mobile
            GoToMobile("categories/test-category1");

            currencyCategoryMobileBegin = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyCategoryPriceMobileBegin = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            Driver.XPathContainsText("button", "Фильтры");

            currencyFilterMobileBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-question-description")).Text;
            currencyFilterFromMobileBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-min")).Text;
            currencyFilterToMobileBegin = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-max")).Text;

            //pre check client product cart mobile
            GoToMobile("products/test-product6");

            currencyProductMobileBegin = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyProductPriceMobileBegin = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-number")).Text;

            ReInit();
            /* test */
            //change admin default currency
            GoToAdmin("settingscatalog#?catalogTab=currency");
            Driver.CheckSelected("AllowToChangeCurrency", "SettingsCatalogSave");

            //check client default currency
            ReInitClient();
            GoToClient("");
            Driver.WaitForElem(By.Name("ddlCurrency"));
            (new SelectElement(Driver.FindElement(By.Name("ddlCurrency")))).SelectByText("TestCurrencyName80");
            Thread.Sleep(1000);
            Driver.FindElement(By.Name("q")).Click();
            Refresh();
        }

        [Test]
        public void ClientMainPage()
        {
            GoToClient("");
            GoToClient("");

            currencyMainPageEnd = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyMainPagePriceEnd = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            VerifyIsFalse(currencyMainPageBegin.Equals(currencyMainPageEnd), "currency main page");
            VerifyIsFalse(currencyMainPagePriceBegin.Equals(currencyMainPagePriceEnd), "currency price main page");
        }

        [Test]
        public void ClientCategory()
        {
            GoToClient("categories/test-category1");
            GoToClient("categories/test-category1");

            currencyCategoryEnd = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyCategoryPriceEnd = Driver.FindElements(By.CssSelector(".products-view-price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            currencyFilterEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-question-description")).Text;
            currencyFilterFromEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-min")).Text;
            currencyFilterToEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-max")).Text;

            VerifyIsFalse(currencyCategoryBegin.Equals(currencyCategoryEnd), "currency category");
            VerifyIsFalse(currencyCategoryPriceBegin.Equals(currencyCategoryPriceEnd), "currency price category");

            VerifyIsFalse(currencyFilterBegin.Equals(currencyFilterEnd), "currency category filter");
            VerifyIsFalse(currencyFilterFromBegin.Equals(currencyFilterFromEnd),
                "currency category filter from: currencyFilterFromEnd= " + currencyFilterFromEnd + currencyFilterEnd +
                ", currencyFilterFromBegin= " + currencyFilterFromBegin + currencyFilterBegin);
            VerifyIsFalse(currencyFilterToBegin.Equals(currencyFilterToEnd), "currency category filter to");
        }

        [Test]
        public void ClientProductCart()
        {
            GoToClient("products/test-product6");
            GoToClient("products/test-product6");

            currencyProductEnd = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyProductPriceEnd = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-number")).Text;

            VerifyIsFalse(currencyProductBegin.Equals(currencyProductEnd), "currency product cart");
            VerifyIsFalse(currencyProductPriceBegin.Equals(currencyProductPriceEnd), "currency price product cart");
        }

        [Test]
        public void ClientMobileMainPage()
        {
            GoToMobile();
            Driver.ScrollTo(By.CssSelector(".mainpage-products--best"));

            currencyMainPageMobileEnd = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyMainPagePriceMobileEnd = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            VerifyIsFalse(currencyMainPageMobileBegin.Equals(currencyMainPageMobileEnd), "currency main page");
            VerifyIsFalse(currencyMainPagePriceMobileBegin.Equals(currencyMainPagePriceMobileEnd),
                "currency price main page");
        }

        [Test]
        public void ClientMobileCategory()
        {
            GoToMobile("categories/test-category1");

            currencyCategoryMobileEnd = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyCategoryPriceMobileEnd = Driver.FindElements(By.CssSelector(".price"))[0]
                .FindElement(By.CssSelector(".price-number")).Text;

            Driver.XPathContainsText("button", "Фильтры");

            currencyFilterMobileEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-question-description")).Text;
            currencyFilterFromMobileEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-min")).Text;
            currencyFilterToMobileEnd = Driver.FindElement(By.CssSelector(".catalog-filter-block"))
                .FindElement(By.CssSelector(".catalog-filter-block .ngrs-value.ngrs-value-max")).Text;

            VerifyIsFalse(currencyCategoryMobileBegin.Equals(currencyCategoryMobileEnd), "currency category");
            VerifyIsFalse(currencyCategoryPriceMobileBegin.Equals(currencyCategoryPriceMobileEnd),
                "currency price category");

            VerifyIsFalse(currencyFilterMobileBegin.Equals(currencyFilterMobileEnd), "currency category filter");
            VerifyIsFalse(currencyFilterFromBegin.Equals(currencyFilterFromEnd),
                "currency category filter from: currencyFilterFromEnd= " + currencyFilterFromMobileEnd +
                currencyFilterMobileEnd + ", currencyFilterFromBegin= " + currencyFilterFromMobileBegin +
                currencyFilterMobileBegin);
            VerifyIsFalse(currencyFilterFromMobileBegin.Equals(currencyFilterFromMobileEnd),
                "currency category filter from");
            VerifyIsFalse(currencyFilterToMobileBegin.Equals(currencyFilterToMobileEnd), "currency category filter to");
        }

        [Test]
        public void ClientMobileProductCart()
        {
            GoToMobile("products/test-product6");

            currencyProductMobileEnd = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-currency")).Text;
            currencyProductPriceMobileEnd = Driver.FindElement(By.CssSelector(".price"))
                .FindElement(By.CssSelector(".price-number")).Text;

            VerifyIsFalse(currencyProductMobileBegin.Equals(currencyProductMobileEnd), "currency product cart");
            VerifyIsFalse(currencyProductPriceMobileBegin.Equals(currencyProductPriceMobileEnd),
                "currency price product cart");
        }
    }
}