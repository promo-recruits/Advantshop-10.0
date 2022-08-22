using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Taxes
{
    [TestFixture]
    public class SettingsTaxesDefaultOrderTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Shipping);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Taxes\\[Order].PaymentMethod.csv",
                "data\\Admin\\Settings\\Taxes\\[Order].ShippingMethod.csv",
                "data\\Admin\\Settings\\Taxes\\TaxDefault\\Catalog.Product.csv",
                "data\\Admin\\Settings\\Taxes\\TaxDefault\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\Taxes\\TaxDefault\\Catalog.Category.csv",
                "data\\Admin\\Settings\\Taxes\\TaxDefault\\Catalog.ProductCategories.csv"
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
        public void TaxDefaultOrderClient()
        {
            //pre check admin default tax
            GoToAdmin("product/edit/1");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(1000);

            IWebElement selectProd = Driver.FindElement(By.Id("TaxId"));
            SelectElement select = new SelectElement(selectProd);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Без НДС"), "pre check tax default");

            //order
            GoToClient("products/test-product1");

            Driver.ScrollTo(By.CssSelector(".details-row.details-rating"));
            Driver.FindElement(By.CssSelector("[data-product-id=\"1\"]")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");

            Driver.ScrollTo(By.CssSelector(".cart-full-result-name"));
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.WaitForElem(By.CssSelector(".breads"));
            Driver.WaitForElem(By.Id("rightCell"));

            Driver.ScrollTo(By.CssSelector(".btn.btn-small.btn-action.btn-expander"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".checkout-success-title.page-title"));

            //check admin order
            GoToAdmin("orders/edit/3");

            VerifyIsTrue(Driver.FindElement(By.TagName("order-items-summary")).Text.Contains("В том числе Без НДС"),
                "default tax in order"); //раньше при "Без НДС" не выводилась запись об НДС, сейчас выводится всегда 
        }
    }
}