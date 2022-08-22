using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.Order
{
    [TestFixture]
    public class OrdersGridTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Orders);
            InitializeService.LoadData(
                "Data\\Admin\\Orders\\OrderGrid\\Catalog.Product.csv",
                "Data\\Admin\\Orders\\OrderGrid\\Catalog.Offer.csv",
                "Data\\Admin\\Orders\\OrderGrid\\Catalog.Category.csv",
                "Data\\Admin\\Orders\\OrderGrid\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Orders\\OrderGrid\\Customers.CustomerGroup.csv",
                "data\\Admin\\Orders\\OrderGrid\\Customers.Customer.csv",
                "data\\Admin\\Orders\\OrderGrid\\Customers.Contact.csv",
                "data\\Admin\\Orders\\OrderGrid\\Customers.Managers.csv",
                "Data\\Admin\\Orders\\OrderGrid\\[Order].OrderSource.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderContact.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderStatus.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].[Order].csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderCurrency.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderItems.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderCustomer.csv"
            );

            Init();
            GoToAdmin("orders");
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
        public void OpenOrders()
        {
            VerifyAreEqual("Заказы", Driver.FindElement(By.TagName("h1")).Text, "page h1");

            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "order number");
            VerifyAreEqual("Отменен навсегда", Driver.GetGridCell(0, "StatusName").Text, "order status");
            VerifyAreEqual("FirstName1 LastName1", Driver.GetGridCell(0, "BuyerName").Text, "order customer");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-empty")).Count == 0,
                "order paid");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-not-empty")).Count == 1,
                "order paid no");

            VerifyAreEqual("96", Driver.GetGridCell(0, "SumFormatted").Text, "order sum");
            VerifyAreEqual("31.08.2016 23:00", Driver.GetGridCell(0, "OrderDateFormatted").Text, "order date");

            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void OpenTabNewOrders()
        {
            Driver.FindElement(By.LinkText("Новые")).Click();
            VerifyAreEqual("5", Driver.GetGridCell(0, "Number").Text, "number order status new");
            VerifyIsTrue(Driver.GetGridCell(0, "StatusName").Text.Contains("Новый"), "order status new");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void OpenTabPaidOrders()
        {
            Driver.FindElement(By.LinkText("Оплаченные")).Click();
            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "number order paid line 1");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-empty")).Count == 0,
                "order paid 1 ");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-not-empty")).Count == 1,
                "order paid no 1");
            VerifyAreEqual("92", Driver.GetGridCell(9, "Number").Text, "number order paid line 10");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-empty")).Count == 0,
                "order paid 10");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-not-empty")).Count == 1,
                "order paid no 10");
            VerifyAreEqual("Найдено записей: 85",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void OpenTabNotPaidOrders()
        {
            Driver.FindElement(By.LinkText("Неоплаченные")).Click();
            VerifyAreEqual("14", Driver.GetGridCell(0, "Number").Text, "number order not paid line 1");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-empty")).Count == 1,
                "order paid 1");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-not-empty")).Count == 0,
                "order paid no 1");
            VerifyAreEqual("5", Driver.GetGridCell(9, "Number").Text, "number order not paid line 10");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-empty")).Count == 1,
                "order paid 10");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-not-empty")).Count == 0,
                "order paid no 10");
            VerifyAreEqual("Найдено записей: 14",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void OpenTabDrafts()
        {
            Driver.FindElement(By.LinkText("Черновики")).Click();
            VerifyAreEqual("100", Driver.GetGridCell(0, "Number").Text, "number order draft line 1");
            VerifyAreEqual("101", Driver.GetGridCell(1, "Number").Text, "number order draft line 2");
            VerifyAreEqual("102", Driver.GetGridCell(2, "Number").Text, "number order draft line 3");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}