using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.Order
{
    [TestFixture]
    public class OrdersSearchTest : BaseSeleniumTest
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
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderCustomer.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderCurrency.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderItems.csv"
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
        public void ByNumberExist()
        {
            Driver.GetGridFilterTab(0, "12");
            VerifyAreEqual("12", Driver.GetGridCell(0, "Number").Text, "order number");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void ByCustomer()
        {
            Driver.GetGridFilterTab(0, "FirstName4 LastName4");
            VerifyAreEqual("FirstName4 LastName4", Driver.GetGridCell(0, "BuyerName").Text, "order customer");
            VerifyAreEqual("99", Driver.GetGridCell(0, "Number").Text, "order number");
            VerifyAreEqual("Найдено записей: 14",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void ByNumberNotExist()
        {
            Driver.GetGridFilterTab(0, "552");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "order number not exist");
        }


        [Test]
        public void ByCustomerNotExist()
        {
            Driver.GetGridFilterTab(0, "FirstName2 LastName4");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "order customer not exist");
        }

        [Test]
        public void MuchSymbols()
        {
            Driver.GetGridFilterTab(0,
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void InvalidSymbols()
        {
            Driver.GetGridFilterTab(0, "########@@@@@@@@&&&&&&&******,,,,..");

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}