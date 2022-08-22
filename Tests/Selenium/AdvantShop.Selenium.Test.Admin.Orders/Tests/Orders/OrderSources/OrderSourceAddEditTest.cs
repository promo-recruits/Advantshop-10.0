using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderSources
{
    [TestFixture]
    public class OrderSourceAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\Orders\\OrderSources\\OrderSourceAdd\\Catalog.Product.csv",
                "data\\Admin\\Orders\\OrderSources\\OrderSourceAdd\\Catalog.Offer.csv",
                "data\\Admin\\Orders\\OrderSources\\OrderSourceAdd\\Catalog.Category.csv",
                "data\\Admin\\Orders\\OrderSources\\OrderSourceAdd\\Catalog.ProductCategories.csv",
                "data\\Admin\\Orders\\OrderSources\\OrderSourceAdd\\[Order].OrderContact.csv",
                "data\\Admin\\Orders\\OrderSources\\OrderSourceAdd\\[Order].OrderSource.csv",
                "data\\Admin\\Orders\\OrderSources\\OrderSourceAdd\\[Order].OrderStatus.csv",
                "data\\Admin\\Orders\\OrderSources\\OrderSourceAdd\\[Order].[Order].csv",
                "data\\Admin\\Orders\\OrderSources\\OrderSourceAdd\\[Order].OrderCurrency.csv",
                "data\\Admin\\Orders\\OrderSources\\OrderSourceAdd\\[Order].OrderItems.csv"
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
        public void OrderSourceAdd()
        {
            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceAdd\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Источник заказа/лида", Driver.FindElement(By.TagName("h2")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceName\"]")).SendKeys("NewOrderSourceName");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceType\"]"))))
                .SelectByValue("number:1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceTypeSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceSort\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceSort\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceSort\"]")).SendKeys("1");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Thread.Sleep(2000);

            //check
            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
            Thread.Sleep(1000);
            Driver.GridFilterSendKeys("NewOrderSourceName");
            Driver.DropFocusCss("[data-e2e=\"OrderSourceTitle\"]");
            VerifyAreEqual("NewOrderSourceName", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "OrderSources").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "TypeFormatted", "OrderSources").Text
                .Contains("Корзина интернет магазина"));
            VerifyIsTrue(Driver.GetGridCell(0, "Main", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
        }

        [Test]
        public void OrderSourceEdit()
        {
            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
            Driver.GridFilterSendKeys("NewOrderSourceName");
            Driver.DropFocusCss("[data-e2e=\"OrderSourceTitle\"]");
            Driver.GetGridCell(0, "_serviceColumn", "OrderSources")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Источник заказа/лида", Driver.FindElement(By.TagName("h2")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceName\"]")).SendKeys("Changed");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceType\"]"))))
                .SelectByValue("number:5");

            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceTypeSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceSort\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceSort\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceSort\"]")).SendKeys("5");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Thread.Sleep(2000);

            //check
            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
            Thread.Sleep(1000);
            Driver.GridFilterSendKeys("Changed");
            Driver.DropFocusCss("[data-e2e=\"OrderSourceTitle\"]");
            VerifyAreEqual("Changed", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("5", Driver.GetGridCell(0, "SortOrder", "OrderSources").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "TypeFormatted", "OrderSources").Text.Contains("Мобильная версия"));
            VerifyIsFalse(Driver.GetGridCell(0, "Main", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
        }

        [Test]
        public void OrderSourceAddSaveNotMain()
        {
            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceAdd\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceName\"]")).SendKeys("NewOrderSourceNotMain");
            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Thread.Sleep(2000);

            GoToAdmin("settingscheckout#?checkoutTab=orderSources");

            Driver.GridFilterSendKeys("NewOrderSourceNotMain");
            Driver.DropFocusCss("[data-e2e=\"OrderSourceTitle\"]");
            VerifyAreEqual("NewOrderSourceNotMain",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"gridOrderSources[0][\'Name\']\"]")).Text);
            VerifyIsFalse(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"gridOrderSources[0][\'Main\']\"] [data-e2e=\"switchOnOffInput\"]"))
                .Selected);
        }
    }
}