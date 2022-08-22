using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.CustomerSegments
{
    [TestFixture]
    public class CustomerSegmentsSearchTest : BaseSeleniumTest
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
                "data\\Admin\\CustomerSegments\\Customers.CustomerSegment.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("settingscustomers#?tab=customerSegments");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void SearchExist()
        {
            Driver.GridFilterSendKeys("CustomerSegment98");
            Driver.Blur();

            VerifyAreEqual("CustomerSegment98", Driver.GetGridCell(0, "Name", "Segments").Text,
                "search exist customer segment");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }


        [Test]
        public void SearchNotExist()
        {
            Driver.GridFilterSendKeys("Customer Segment 206");
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchMuchSymbols()
        {
            Driver.GridFilterSendKeys(
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", By.ClassName("ui-grid-custom-filter-total"));
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchInvalidSymbols()
        {
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}