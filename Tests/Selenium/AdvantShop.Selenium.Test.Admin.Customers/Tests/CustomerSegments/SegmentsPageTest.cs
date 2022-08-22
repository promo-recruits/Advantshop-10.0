using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.CustomerSegments
{
    [TestFixture]
    public class CustomerSegmentsPageTest : BaseSeleniumTest
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
        public void SegmentsPage()
        {
            VerifyAreEqual("CustomerSegment105", Driver.GetGridCell(0, "Name", "Segments").Text, "page 1 line 1");
            VerifyAreEqual("CustomerSegment96", Driver.GetGridCell(9, "Name", "Segments").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("CustomerSegment95", Driver.GetGridCell(0, "Name", "Segments").Text, "page 2 line 1");
            VerifyAreEqual("CustomerSegment86", Driver.GetGridCell(9, "Name", "Segments").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("CustomerSegment85", Driver.GetGridCell(0, "Name", "Segments").Text, "page 3 line 1");
            VerifyAreEqual("CustomerSegment76", Driver.GetGridCell(9, "Name", "Segments").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("CustomerSegment75", Driver.GetGridCell(0, "Name", "Segments").Text, "page 4 line 1");
            VerifyAreEqual("CustomerSegment66", Driver.GetGridCell(9, "Name", "Segments").Text, "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("CustomerSegment65", Driver.GetGridCell(0, "Name", "Segments").Text, "page 5 line 1");
            VerifyAreEqual("CustomerSegment56", Driver.GetGridCell(9, "Name", "Segments").Text, "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("CustomerSegment55", Driver.GetGridCell(0, "Name", "Segments").Text, "page 6 line 1");
            VerifyAreEqual("CustomerSegment46", Driver.GetGridCell(9, "Name", "Segments").Text, "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("CustomerSegment105", Driver.GetGridCell(0, "Name", "Segments").Text, "page 1 line 1");
            VerifyAreEqual("CustomerSegment96", Driver.GetGridCell(9, "Name", "Segments").Text, "page 1 line 10");
        }

        [Test]
        public void SegmentsPageToPrevious()
        {
            VerifyAreEqual("CustomerSegment105", Driver.GetGridCell(0, "Name", "Segments").Text, "page 1 line 1");
            VerifyAreEqual("CustomerSegment96", Driver.GetGridCell(9, "Name", "Segments").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("CustomerSegment95", Driver.GetGridCell(0, "Name", "Segments").Text, "page 2 line 1");
            VerifyAreEqual("CustomerSegment86", Driver.GetGridCell(9, "Name", "Segments").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("CustomerSegment85", Driver.GetGridCell(0, "Name", "Segments").Text, "page 3 line 1");
            VerifyAreEqual("CustomerSegment76", Driver.GetGridCell(9, "Name", "Segments").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("CustomerSegment95", Driver.GetGridCell(0, "Name", "Segments").Text, "page 2 line 1");
            VerifyAreEqual("CustomerSegment86", Driver.GetGridCell(9, "Name", "Segments").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("CustomerSegment105", Driver.GetGridCell(0, "Name", "Segments").Text, "page 1 line 1");
            VerifyAreEqual("CustomerSegment96", Driver.GetGridCell(9, "Name", "Segments").Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("CustomerSegment27", Driver.GetGridCell(0, "Name", "Segments").Text, "last page line 1");
            VerifyAreEqual("CustomerSegment28", Driver.GetGridCell(4, "Name", "Segments").Text, "last page line 5");
        }
    }
}