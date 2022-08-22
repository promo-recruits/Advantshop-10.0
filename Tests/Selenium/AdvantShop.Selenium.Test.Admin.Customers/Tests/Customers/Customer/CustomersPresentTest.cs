using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.Customer
{
    [TestFixture]
    public class CustomersPresentTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Orders | ClearType.Catalog | ClearType.Payment |
                                        ClearType.Shipping);
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
                "data\\Admin\\Customers\\CustomerGrid\\[Order].PaymentMethod.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].ShippingMethod.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].OrderStatus.csv",
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
        public void CustomerPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("LastName120 FirstName120",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text, "line 1");
            VerifyAreEqual("LastName111 FirstName111",
                Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text, "line 10");
       
            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("LastName120 FirstName120",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text, "line 1");
            VerifyAreEqual("LastName101 FirstName101",
                Driver.GetGridCell(19, "Name", "Customers").FindElement(By.TagName("a")).Text, "line 20");
        }
    }
}