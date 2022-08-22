using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Taxes
{
    [TestFixture]
    public class SettingsTaxesOrderTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Taxes);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Taxes\\TaxesClient\\Catalog.Product.csv",
                "data\\Admin\\Settings\\Taxes\\TaxesClient\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\Taxes\\TaxesClient\\Catalog.Category.csv",
                "data\\Admin\\Settings\\Taxes\\TaxesClient\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\Taxes\\TaxesClient\\Catalog.Tax.csv",
                "data\\Admin\\Settings\\Taxes\\TaxesClient\\Settings.Settings.csv"
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
        public void TaxProductOrderClient()
        {
            //pre check admin default tax
            GoToAdmin("product/edit/10");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(1000);

            IWebElement selectProd = Driver.FindElement(By.Id("TaxId"));
            SelectElement select = new SelectElement(selectProd);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Tax 10"), "pre check product tax");

            //order
            GoToClient("products/test-product10");

            Driver.ScrollTo(By.CssSelector(".details-row.details-rating"));
            Driver.FindElement(By.CssSelector("[data-product-id=\"10\"]")).Click();
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
            GoToAdmin("orders");
            Driver.GetGridCell(0, "Number").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.CssSelector("[grid-unique-id=\"gridOrderItems\"]"));

            VerifyIsTrue(Driver.FindElement(By.TagName("order-items-summary")).Text.Contains("В том числе Tax 10"),
                "product tax in order");
        }

        [Test]
        public void TaxProductOrderAdmin()
        {
            //pre check admin default tax
            GoToAdmin("product/edit/11");

            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(1000);

            IWebElement selectProd = Driver.FindElement(By.Id("TaxId"));
            SelectElement select = new SelectElement(selectProd);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Tax 11"), "pre check product tax");

            //order
            GoToAdmin("orders/add");

            Driver.FindElement(By.Id("Order_OrderCustomer_LastName")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_LastName")).Clear();
            Driver.FindElement(By.Id("Order_OrderCustomer_LastName")).SendKeys("Customer");
            //driver.FindElement(By.CssSelector(".inline")).Click();

            Driver.ScrollTo(By.Id("Order_OrderCustomer_Phone"));
            Driver.FindElement(By.LinkText("Добавить товар")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.GridFilterSendKeys("TestProduct11");
            Driver.XPathContainsText("h2", "Выбор товара");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr").Click();
            Driver.XPathContainsText("button", "Выбрать");
            Thread.Sleep(100);

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();
            //check admin order
            GoToAdmin("orders");
            Driver.GetGridCell(0, "Number").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.CssSelector("[grid-unique-id=\"gridOrderItems\"]"));

            VerifyIsTrue(Driver.FindElement(By.TagName("order-items-summary")).Text.Contains("В том числе Tax 11"),
                "product tax in order");
        }
    }
}