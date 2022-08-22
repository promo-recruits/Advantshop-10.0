using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.Customer
{
    [TestFixture]
    public class CustomersSortTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Orders | ClearType.Catalog |
                                        ClearType.Shipping | ClearType.Payment);
            InitializeService.LoadData(
                "data\\Admin\\Customers\\CustomerGrid\\Customers.CustomerGroup.csv",
                "data\\Admin\\Customers\\CustomerGrid\\Customers.Customer.csv",
                "data\\Admin\\Customers\\CustomerGrid\\Customers.Contact.csv",
                "data\\Admin\\Customers\\CustomerGrid\\Customers.Departments.csv",
                "data\\Admin\\Customers\\CustomerGrid\\Customers.Managers.csv",
                "data\\Admin\\Customers\\CustomerGrid\\Customers.CustomerField.csv",
                "data\\Admin\\Customers\\CustomerGrid\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\Customers\\CustomerGrid\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\Customers\\CustomerGrid\\Catalog.Product.csv",
                "data\\Admin\\Customers\\CustomerGrid\\Catalog.Offer.csv",
                "data\\Admin\\Customers\\CustomerGrid\\Catalog.Category.csv",
                "data\\Admin\\Customers\\CustomerGrid\\Catalog.ProductCategories.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].OrderContact.csv",
                "Data\\Admin\\Customers\\CustomerGrid\\[Order].OrderSource.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].OrderStatus.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].PaymentMethod.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].ShippingMethod.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].[Order].csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].OrderCurrency.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].OrderItems.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].OrderCustomer.csv"
            );

            Init();
            GoToAdmin("customers");
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
        public void SortByCustomer()
        {
            Driver.GetGridCell(-1, "Name", "Customers").Click();
            VerifyAreEqual("LastName1 FirstName1",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "sort by customer asc line 1");
            VerifyAreEqual("LastName107 FirstName107",
                Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "sort by customer asc line 10");

            Driver.GetGridCell(-1, "Name", "Customers").Click();
            VerifyAreEqual("LastName99 FirstName99",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "sort by customer desc line 1");
            VerifyAreEqual("LastName90 FirstName90",
                Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "sort by customer desc line 10");
        }

        [Test]
        public void SortByPhone()
        {
            Driver.GetGridCell(-1, "Phone", "Customers").Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "Phone", "Customers").Text, "sort by Phone asc line 1");
            VerifyAreEqual("107", Driver.GetGridCell(9, "Phone", "Customers").Text, "sort by Phone asc line 10");

            Driver.GetGridCell(-1, "Phone", "Customers").Click();
            VerifyAreEqual("99", Driver.GetGridCell(0, "Phone", "Customers").Text, "sort by Phone desc line 1");
            VerifyAreEqual("90", Driver.GetGridCell(9, "Phone", "Customers").Text, "sort by Phone desc line 10");
        }

        [Test]
        public void SortByMail()
        {
            Driver.GetGridCell(-1, "Email", "Customers").Click();
            VerifyAreEqual("test@mail.ru1", Driver.GetGridCell(0, "Email", "Customers").Text,
                "sort by Email asc line 1");
            VerifyAreEqual("test@mail.ru107", Driver.GetGridCell(9, "Email", "Customers").Text,
                "sort by Email asc line 10");

            Driver.GetGridCell(-1, "Email", "Customers").Click();
            VerifyAreEqual("test@mail.ru99", Driver.GetGridCell(0, "Email", "Customers").Text,
                "sort by Email desc line 1");
            VerifyAreEqual("test@mail.ru90", Driver.GetGridCell(9, "Email", "Customers").Text,
                "sort by Email desc line 10");
        }

        [Test]
        public void SortByOrdersCount()
        {
            Driver.GetGridCell(-1, "OrdersCount", "Customers").Click();
            VerifyAreEqual("0", Driver.GetGridCell(0, "OrdersCount", "Customers").Text,
                "sort by OrdersCount asc line 1");
            VerifyAreEqual("0", Driver.GetGridCell(9, "OrdersCount", "Customers").Text,
                "sort by OrdersCount asc line 10");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text
                    .Equals(Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text),
                "diff customers asc");

            Driver.GetGridCell(-1, "OrdersCount", "Customers").Click();
            VerifyAreEqual("12", Driver.GetGridCell(0, "OrdersCount", "Customers").Text,
                "sort by OrdersCount desc line 1");
            VerifyAreEqual("LastName5 FirstName5",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "sort by OrdersCount customer name");
            VerifyAreEqual("1", Driver.GetGridCell(1, "OrdersCount", "Customers").Text,
                "sort by OrdersCount desc line 2");
            VerifyAreEqual("1", Driver.GetGridCell(2, "OrdersCount", "Customers").Text,
                "sort by OrdersCount desc line 3");
            VerifyAreEqual("1", Driver.GetGridCell(3, "OrdersCount", "Customers").Text,
                "sort by OrdersCount desc line 4");
            VerifyAreEqual("1", Driver.GetGridCell(4, "OrdersCount", "Customers").Text,
                "sort by OrdersCount desc line 5");
            VerifyAreEqual("0", Driver.GetGridCell(9, "OrdersCount", "Customers").Text,
                "sort by OrdersCount desc line 10");
        }

        [Test]
        public void SortByLastOrderNumber()
        {
            Driver.GetGridCell(-1, "LastOrderNumber", "Customers").Click();
            VerifyAreEqual("", Driver.GetGridCell(0, "LastOrderNumber", "Customers").Text,
                "sort by LastOrderNumber asc line 1");
            VerifyAreEqual("", Driver.GetGridCell(9, "LastOrderNumber", "Customers").Text,
                "sort by LastOrderNumber asc line 10");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text
                    .Equals(Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text),
                "diff customers asc");

            Driver.GetGridCell(-1, "LastOrderNumber", "Customers").Click();
            VerifyAreEqual("# 30",
                Driver.GetGridCell(0, "LastOrderNumber", "Customers").FindElement(By.TagName("a")).Text,
                "sort by LastOrderNumber desc line 1");
            VerifyAreEqual("LastName5 FirstName5",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "sort by LastOrderNumber customer name");
            VerifyAreEqual("# 19",
                Driver.GetGridCell(1, "LastOrderNumber", "Customers").FindElement(By.TagName("a")).Text,
                "sort by LastOrderNumber desc line 2");
            VerifyAreEqual("# 18",
                Driver.GetGridCell(2, "LastOrderNumber", "Customers").FindElement(By.TagName("a")).Text,
                "sort by LastOrderNumber desc line 3");
            VerifyAreEqual("# 17",
                Driver.GetGridCell(3, "LastOrderNumber", "Customers").FindElement(By.TagName("a")).Text,
                "sort by LastOrderNumber desc line 4");
            VerifyAreEqual("# 16",
                Driver.GetGridCell(4, "LastOrderNumber", "Customers").FindElement(By.TagName("a")).Text,
                "sort by LastOrderNumber desc line 5");
            VerifyAreEqual("", Driver.GetGridCell(9, "LastOrderNumber", "Customers").Text,
                "sort by LastOrderNumber desc line 10");
        }

        [Test]
        public void SortByOrdersSum()
        {
            Driver.GetGridCell(-1, "OrdersSum", "Customers").Click();
            VerifyAreEqual("0", Driver.GetGridCell(0, "OrdersSum", "Customers").Text, "sort by OrdersCount asc line 1");
            VerifyAreEqual("0", Driver.GetGridCell(9, "OrdersSum", "Customers").Text,
                "sort by OrdersCount asc line 10");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text
                    .Equals(Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text),
                "diff customers asc");

            Driver.GetGridCell(-1, "OrdersSum", "Customers").Click();
            VerifyAreEqual("290", Driver.GetGridCell(0, "OrdersSum", "Customers").Text,
                "sort by OrdersCount desc line 1");
            VerifyAreEqual("LastName5 FirstName5",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "sort by OrdersCount customer name");
            VerifyAreEqual("19", Driver.GetGridCell(1, "OrdersSum", "Customers").Text,
                "sort by OrdersCount desc line 2");
            VerifyAreEqual("18", Driver.GetGridCell(2, "OrdersSum", "Customers").Text,
                "sort by OrdersCount desc line 3");
            VerifyAreEqual("17", Driver.GetGridCell(3, "OrdersSum", "Customers").Text,
                "sort by OrdersCount desc line 4");
            VerifyAreEqual("16", Driver.GetGridCell(4, "OrdersSum", "Customers").Text,
                "sort by OrdersCount desc line 5");
            VerifyAreEqual("0", Driver.GetGridCell(9, "OrdersSum", "Customers").Text,
                "sort by OrdersCount desc line 10");
        }

        [Test]
        public void SortByRegDate()
        {
            Driver.GetGridCell(-1, "RegistrationDateTimeFormatted", "Customers").Click();
            VerifyAreEqual("19.04.2017 15:37", Driver.GetGridCell(0, "RegistrationDateTimeFormatted", "Customers").Text,
                "sort by RegDate asc line 1");
            VerifyAreEqual("28.04.2017 15:37", Driver.GetGridCell(9, "RegistrationDateTimeFormatted", "Customers").Text,
                "sort by RegDate asc line 10");

            Driver.GetGridCell(-1, "RegistrationDateTimeFormatted", "Customers").Click();
            VerifyAreEqual("15.08.2017 15:37", Driver.GetGridCell(0, "RegistrationDateTimeFormatted", "Customers").Text,
                "sort by RegDate desc line 1");
            VerifyAreEqual("06.08.2017 15:37", Driver.GetGridCell(9, "RegistrationDateTimeFormatted", "Customers").Text,
                "sort by RegDate desc line 10");
        }

        [Test]
        public void SortByManager()
        {
            Driver.GetGridCell(-1, "ManagerName", "Customers").Click();
            VerifyAreEqual("", Driver.GetGridCell(0, "ManagerName", "Customers").Text,
                "sort by ManagerName asc line 1");
            VerifyAreEqual("", Driver.GetGridCell(9, "ManagerName", "Customers").Text,
                "sort by ManagerName asc line 10");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text
                    .Equals(Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text),
                "diff customers asc");

            Driver.GetGridCell(-1, "ManagerName", "Customers").Click();
            VerifyAreEqual("ManagerName2 ManagerLastName2", Driver.GetGridCell(0, "ManagerName", "Customers").Text,
                "sort by ManagerName desc line 1");
            VerifyAreEqual("ManagerName2 ManagerLastName2", Driver.GetGridCell(9, "ManagerName", "Customers").Text,
                "sort by ManagerName desc line 10");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text
                    .Equals(Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text),
                "diff customers desc");
        }
    }
}