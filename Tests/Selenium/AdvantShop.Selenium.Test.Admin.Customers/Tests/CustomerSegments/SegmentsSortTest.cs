using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.CustomerSegments
{
    [TestFixture]
    public class CustomerSegmentsSortTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Orders | ClearType.Catalog |
                                        ClearType.Shipping | ClearType.Payment);
            InitializeService.LoadData(
                "data\\Admin\\CustomerSegments\\Customers.CustomerGroup.csv",
                "data\\Admin\\CustomerSegments\\Customers.Customer.csv",
                "data\\Admin\\CustomerSegments\\Customers.Contact.csv",
                "data\\Admin\\CustomerSegments\\Customers.Departments.csv",
                "data\\Admin\\CustomerSegments\\Customers.Managers.csv",
                "data\\Admin\\CustomerSegments\\Customers.CustomerField.csv",
                "data\\Admin\\CustomerSegments\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CustomerSegments\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CustomerSegments\\Catalog.Product.csv",
                "data\\Admin\\CustomerSegments\\Catalog.Offer.csv",
                "data\\Admin\\CustomerSegments\\Catalog.Category.csv",
                "data\\Admin\\CustomerSegments\\Catalog.ProductCategories.csv",
                "data\\Admin\\CustomerSegments\\[Order].OrderContact.csv",
                "Data\\Admin\\CustomerSegments\\[Order].OrderSource.csv",
                "data\\Admin\\CustomerSegments\\[Order].OrderStatus.csv",
                "data\\Admin\\CustomerSegments\\[Order].PaymentMethod.csv",
                "data\\Admin\\CustomerSegments\\[Order].ShippingMethod.csv",
                "data\\Admin\\CustomerSegments\\[Order].[Order].csv",
                "data\\Admin\\CustomerSegments\\[Order].OrderCurrency.csv",
                "data\\Admin\\CustomerSegments\\[Order].OrderItems.csv",
                "data\\Admin\\CustomerSegments\\[Order].OrderCustomer.csv",
                "data\\Admin\\CustomerSegments\\Customers.CustomerSegment.csv",
                "data\\Admin\\CustomerSegments\\Customers.CustomerSegment_Customer.csv"
            );

            Init();
            GoToAdmin("settingscustomers#?tab=customerSegments");
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
        public void SortBySegmentName()
        {
            Driver.GetGridCell(-1, "Name", "Segments").Click();
            VerifyAreEqual("CustomerSegment1", Driver.GetGridCell(0, "Name", "Segments").Text,
                "sort by name asc line 1");
            VerifyAreEqual("CustomerSegment12", Driver.GetGridCell(9, "Name", "Segments").Text,
                "sort by name asc line 10");

            Driver.GetGridCell(-1, "Name", "Segments").Click();
            VerifyAreEqual("CustomerSegment99", Driver.GetGridCell(0, "Name", "Segments").Text,
                "sort by name desc line 1");
            VerifyAreEqual("CustomerSegment90", Driver.GetGridCell(9, "Name", "Segments").Text,
                "sort by name desc line 10");
        }

        [Test]
        public void SortByCustomersCount()
        {
            Driver.GetGridCell(-1, "CustomersCount", "Segments").Click();
            VerifyAreEqual("88", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "sort by CustomersCount asc line 1");
            VerifyAreEqual("101", Driver.GetGridCell(9, "CustomersCount", "Segments").Text,
                "sort by CustomersCount asc line 10");

            Driver.GetGridCell(-1, "CustomersCount", "Segments").Click();

            string segmentLine1 = Driver.GetGridCell(0, "Name", "Segments").Text;
            string segmentLine10 = Driver.GetGridCell(9, "Name", "Segments").Text;

            VerifyAreEqual("101", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "sort by CustomersCount desc line 1");
            VerifyAreEqual("101", Driver.GetGridCell(9, "CustomersCount", "Segments").Text,
                "sort by CustomersCount desc line 10");

            VerifyIsFalse(segmentLine1.Equals(segmentLine10), "sort by CustomersCount desc diff name");
        }

        [Test]
        public void SortByCreateDate()
        {
            Driver.GetGridCell(-1, "CreatedDateFormatted", "Segments").Click();
            VerifyAreEqual("05.10.2017 16:11", Driver.GetGridCell(0, "CreatedDateFormatted", "Segments").Text,
                "sort by CreateDate asc line 1");
            VerifyAreEqual("06.10.2017 16:11", Driver.GetGridCell(9, "CreatedDateFormatted", "Segments").Text,
                "sort by CreateDate asc line 10");

            Driver.GetGridCell(-1, "CreatedDateFormatted", "Segments").Click();
            VerifyAreEqual("20.01.2018 16:11", Driver.GetGridCell(0, "CreatedDateFormatted", "Segments").Text,
                "sort by CreateDate desc line 1");
            VerifyAreEqual("11.01.2018 16:11", Driver.GetGridCell(9, "CreatedDateFormatted", "Segments").Text,
                "sort by CreateDate desc line 10");
        }
    }
}