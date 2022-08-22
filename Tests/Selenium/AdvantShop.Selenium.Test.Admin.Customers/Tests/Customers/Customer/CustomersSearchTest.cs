using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.Customer
{
    [TestFixture]
    public class CustomersSearchTest : BaseSeleniumTest
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
                "data\\Admin\\Customers\\CustomerGrid\\[Order].OrderStatus.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].PaymentMethod.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].ShippingMethod.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].[Order].csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].OrderCurrency.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].OrderItems.csv",
                "data\\Admin\\Customers\\CustomerGrid\\[Order].OrderCustomer.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("customers");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void SearchExist()
        {
            Driver.GridFilterSendKeys("FirstName111 LastName111");

            VerifyAreEqual("LastName111 FirstName111",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text, "search exist customer");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }


        [Test]
        public void SearchNotExist()
        {
            Driver.GridFilterSendKeys("Unknown Unknown");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchMuchSymbols()
        {
            Driver.GridFilterSendKeys(
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", By.ClassName("ui-grid-custom-filter-total"));

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchInvalidSymbols()
        {
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}