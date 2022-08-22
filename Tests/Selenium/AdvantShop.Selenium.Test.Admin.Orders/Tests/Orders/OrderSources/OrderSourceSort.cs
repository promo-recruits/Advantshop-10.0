using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderSources
{
    [TestFixture]
    public class OrderSourceSort : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\Orders\\OrderSources\\[Order].OrderSource.csv",
                "data\\Admin\\Orders\\Catalog.Product.csv",
                "data\\Admin\\Orders\\Catalog.Offer.csv",
                "data\\Admin\\Orders\\Catalog.Category.csv",
                "data\\Admin\\Orders\\Catalog.ProductCategories.csv",
                "data\\Admin\\Orders\\[Order].OrderStatus.csv",
                //"data\\Admin\\Orders\\[Order].ShippingMethod.csv",
                //"data\\Admin\\Orders\\[Order].PaymentMethod.csv",
                "data\\Admin\\Orders\\[Order].[Order].csv",
                "data\\Admin\\Orders\\[Order].OrderItems.csv",
                "data\\Admin\\Orders\\[Order].OrderContact.csv",
                "data\\Admin\\Orders\\[Order].OrderCurrency.csv"
                //"data\\Admin\\Orders\\[Order].LeadItem.csv",
                //"data\\Admin\\Orders\\[Order].LeadCurrency.csv",
                //"data\\Admin\\Orders\\[Order].Lead.csv"
            );

            Init();
            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
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
            Driver.GetGridCell(-1, "Name", "OrderSources").Click();
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source16", Driver.GetGridCell(9, "Name", "OrderSources").Text);

            Driver.GetGridCell(-1, "Name", "OrderSources").Click();
            VerifyAreEqual("Source99", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source90", Driver.GetGridCell(9, "Name", "OrderSources").Text);
        }

        [Test]
        public void SortByGroup()
        {
            Driver.GetGridCell(-1, "TypeFormatted", "OrderSources").Click();
            VerifyAreEqual("Source10", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source100", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            VerifyAreEqual("Забытая корзина", Driver.GetGridCell(9, "TypeFormatted", "OrderSources").Text);

            Driver.GetGridCell(-1, "TypeFormatted", "OrderSources").Click();
            VerifyAreEqual("Source8", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source98", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            VerifyAreEqual("Социальные сети", Driver.GetGridCell(1, "TypeFormatted", "OrderSources").Text);
        }

        [Test]
        public void SortByMain()
        {
            Driver.GetGridCell(-1, "Main", "OrderSources").Click();
            VerifyAreEqual("Source2", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source11", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "Main", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "Main", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "Main", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);

            Driver.GetGridCell(-1, "Main", "OrderSources").Click();
            VerifyAreEqual("Source3", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source12", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Main", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "Main", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
        }

        [Test]
        public void SortBySortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder", "OrderSources").Click();
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source10", Driver.GetGridCell(9, "Name", "OrderSources").Text);

            Driver.GetGridCell(-1, "SortOrder", "OrderSources").Click();
            VerifyAreEqual("Source101", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source92", Driver.GetGridCell(9, "Name", "OrderSources").Text);
        }

        [Test]
        public void SortByCountOrders()
        {
            Driver.GetGridCell(-1, "OrdersCount", "OrderSources").Click();
            VerifyAreEqual("Source10", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source19", Driver.GetGridCell(9, "Name", "OrderSources").Text);

            Driver.GetGridCell(-1, "OrdersCount", "OrderSources").Click();
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source14", Driver.GetGridCell(9, "Name", "OrderSources").Text);
        }
    }
}