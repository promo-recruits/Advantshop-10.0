using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.Order
{
    [TestFixture]
    public class OrdersPresentTest : BaseSeleniumTest
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
        public void OrdersPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "line 1");
            VerifyAreEqual("92", Driver.GetGridCell(9, "Number").Text, "line 10");
       
            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "line 1");
            VerifyAreEqual("82", Driver.GetGridCell(19, "Number").Text, "line 20");
        }
    }
}