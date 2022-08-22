using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.CustomerSegments
{
    [TestFixture]
    public class CustomerSegmentsCustomersTest : BaseSeleniumTest
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

            GoToAdmin("customersegments/edit/1");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        [Order(1)]
        public void SegmentsCustomersGrid()
        {
            VerifyIsTrue(Driver.FindElement(By.TagName("h3")).Text.Contains("Покупатели"),
                "segment's customers page h1");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment's customers count");

            VerifyAreEqual("LastName101 FirstName101 Patronymic101", Driver.GetGridCell(0, "Name").Text,
                "segment's customer grid name");
            VerifyAreEqual("101", Driver.GetGridCell(0, "Phone").Text, "segment's customers grid Phone");
            VerifyAreEqual("test@mail.ru101", Driver.GetGridCell(0, "Email").Text, "segment's customers grid Email");
            VerifyAreEqual("2", Driver.GetGridCell(0, "OrdersCount").Text, "segment's customers grid OrdersCount");
            VerifyAreEqual("07.10.2017 15:37", Driver.GetGridCell(0, "RegistrationDateTimeFormatted").Text,
                "segment's customers grid reg date");
        }

        [Test]
        [Order(1)]
        public void SegmentsCustomersGoToCustomer()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterSearch\"]"));

            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"clientName\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"EditClientRight\"]")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));

            VerifyIsTrue(Driver.Url.Contains("customerIdInfo"), "customer edit url");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lead-info-inner")).FindElement(By.TagName("h1")).Text
                    .Contains("LastName100 FirstName100 Patronymic100"), "customer edit h1");
            VerifyAreEqual("LastName100", Driver.FindElement(By.Id("Customer_LastName")).GetAttribute("value"),
                "customer edit last name");
            VerifyAreEqual("FirstName100", Driver.FindElement(By.Id("Customer_FirstName")).GetAttribute("value"),
                "customer edit first name");
            VerifyAreEqual("Patronymic100", Driver.FindElement(By.Id("Customer_Patronymic")).GetAttribute("value"),
                "customer edit patronymic");
        }

        [Test]
        [Order(3)]
        public void SegmentsCustomersPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("LastName101 FirstName101 Patronymic101", Driver.GetGridCell(0, "Name").Text, "line 1");
            VerifyAreEqual("LastName92 FirstName92 Patronymic92", Driver.GetGridCell(9, "Name").Text, "line 10");

            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("LastName101 FirstName101 Patronymic101", Driver.GetGridCell(0, "Name").Text, "line 1");
            VerifyAreEqual("LastName82 FirstName82 Patronymic82", Driver.GetGridCell(19, "Name").Text, "line 20");
        }

        [Test]
        [Order(4)]
        public void SearchExist()
        {
            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GridFilterSendKeys("FirstName99 LastName99");
            Driver.Blur();

            VerifyAreEqual("LastName99 FirstName99 Patronymic99", Driver.GetGridCell(0, "Name").Text,
                "search exist segment's customer");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all customers");
        }


        [Test]
        [Order(4)]
        public void SearchNotExist()
        {
            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GridFilterSendKeys("First Name 789");
            Driver.Blur();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]")).Text
                    .Contains("Ни одной записи не найдено"), "search not exist");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        [Order(4)]
        public void SearchMuchSymbols()
        {
            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GridFilterSendKeys(
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", By.ClassName("ui-grid-custom-filter-total"));
            Driver.Blur();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]")).Text
                    .Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        [Order(4)]
        public void SearchInvalidSymbols()
        {
            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            Driver.Blur();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]")).Text
                    .Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        [Order(5)]
        public void SortByCustomerName()
        {
            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GetGridCell(-1, "Name").Click();
            VerifyAreEqual("LastName1 FirstName1 Patronymic1", Driver.GetGridCell(0, "Name").Text,
                "sort by name asc line 1");
            VerifyAreEqual("LastName16 FirstName16 Patronymic16", Driver.GetGridCell(9, "Name").Text,
                "sort by name asc line 10");

            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GetGridCell(-1, "Name").Click();
            VerifyAreEqual("LastName99 FirstName99 Patronymic99", Driver.GetGridCell(0, "Name").Text,
                "sort by name desc line 1");
            VerifyAreEqual("LastName90 FirstName90 Patronymic90", Driver.GetGridCell(9, "Name").Text,
                "sort by name desc line 10");
        }

        [Test]
        [Order(5)]
        public void SortByCustomerPhone()
        {
            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GetGridCell(-1, "Phone").Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "Phone").Text, "sort by Phone asc line 1");
            VerifyAreEqual("16", Driver.GetGridCell(9, "Phone").Text, "sort by Phone asc line 10");

            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GetGridCell(-1, "Phone").Click();
            VerifyAreEqual("99", Driver.GetGridCell(0, "Phone").Text, "sort by Phone desc line 1");
            VerifyAreEqual("90", Driver.GetGridCell(9, "Phone").Text, "sort by Phone desc line 10");
        }

        [Test]
        [Order(5)]
        public void SortByCustomerEmail()
        {
            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GetGridCell(-1, "Email").Click();
            VerifyAreEqual("test@mail.ru1", Driver.GetGridCell(0, "Email").Text, "sort by Email asc line 1");
            VerifyAreEqual("test@mail.ru16", Driver.GetGridCell(9, "Email").Text, "sort by Email asc line 10");

            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GetGridCell(-1, "Email").Click();
            VerifyAreEqual("test@mail.ru99", Driver.GetGridCell(0, "Email").Text, "sort by Email desc line 1");
            VerifyAreEqual("test@mail.ru90", Driver.GetGridCell(9, "Email").Text, "sort by Email desc line 10");
        }

        [Test]
        [Order(5)]
        public void SortByOrdersCount()
        {
            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GetGridCell(-1, "OrdersCount").Click();
            VerifyAreEqual("0", Driver.GetGridCell(0, "OrdersCount").Text, "sort by OrdersCount asc line 1");
            VerifyAreEqual("0", Driver.GetGridCell(9, "OrdersCount").Text, "sort by OrdersCount asc line 10");

            string ordersCountAsc1 = Driver.GetGridCell(0, "Name").Text;
            string ordersCountAsc10 = Driver.GetGridCell(9, "Name").Text;

            VerifyIsFalse(ordersCountAsc1.Equals(ordersCountAsc10), "sort by OrdersCount asc diff name");

            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GetGridCell(-1, "OrdersCount").Click();
            VerifyAreEqual("2", Driver.GetGridCell(0, "OrdersCount").Text, "sort by OrdersCount desc line 1");
            VerifyAreEqual("2", Driver.GetGridCell(9, "OrdersCount").Text, "sort by OrdersCount desc line 10");

            string ordersCountDesc1 = Driver.GetGridCell(0, "Name").Text;
            string ordersCountDesc10 = Driver.GetGridCell(9, "Name").Text;

            VerifyIsFalse(ordersCountDesc1.Equals(ordersCountDesc10), "sort by OrdersCount desc diff name");
        }


        [Test]
        [Order(5)]
        public void SortByCustomerRegDate()
        {
            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GetGridCell(-1, "RegistrationDateTimeFormatted").Click();
            VerifyAreEqual("19.04.2017 15:37", Driver.GetGridCell(0, "RegistrationDateTimeFormatted").Text,
                "sort by Customer RegDate asc line 1");
            VerifyAreEqual("28.04.2017 15:37", Driver.GetGridCell(9, "RegistrationDateTimeFormatted").Text,
                "sort by Customer RegDate asc line 10");

            Driver.ScrollTo(By.Id("communicationDropdown"));
            Driver.GetGridCell(-1, "RegistrationDateTimeFormatted").Click();
            VerifyAreEqual("07.10.2017 15:37", Driver.GetGridCell(0, "RegistrationDateTimeFormatted").Text,
                "sort by Customer RegDate desc line 1");
            VerifyAreEqual("28.09.2017 15:37", Driver.GetGridCell(9, "RegistrationDateTimeFormatted").Text,
                "sort by Customer RegDate desc line 10");
        }

        [Test]
        [Order(10)]
        public void SegmentsCustomerPage()
        {
            //reset order
            Driver.FindElement(AdvBy.DataE2E("gridHeaderCell")).Click();
            while (Driver.FindElement(AdvBy.DataE2E("gridHeaderCell")).
                FindElement(By.ClassName("sortable")).GetAttribute("aria-sort") != "none")
            {
                Driver.FindElement(AdvBy.DataE2E("gridHeaderCell")).Click();
            }

            VerifyAreEqual("LastName101 FirstName101 Patronymic101", Driver.GetGridCell(0, "Name").Text,
                "page 1 line 1");
            VerifyAreEqual("LastName92 FirstName92 Patronymic92", Driver.GetGridCell(9, "Name").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("LastName91 FirstName91 Patronymic91", Driver.GetGridCell(0, "Name").Text, "page 2 line 1");
            VerifyAreEqual("LastName82 FirstName82 Patronymic82", Driver.GetGridCell(9, "Name").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("LastName81 FirstName81 Patronymic81", Driver.GetGridCell(0, "Name").Text, "page 3 line 1");
            VerifyAreEqual("LastName72 FirstName72 Patronymic72", Driver.GetGridCell(9, "Name").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("LastName71 FirstName71 Patronymic71", Driver.GetGridCell(0, "Name").Text, "page 4 line 1");
            VerifyAreEqual("LastName62 FirstName62 Patronymic62", Driver.GetGridCell(9, "Name").Text, "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("LastName61 FirstName61 Patronymic61", Driver.GetGridCell(0, "Name").Text, "page 5 line 1");
            VerifyAreEqual("LastName52 FirstName52 Patronymic52", Driver.GetGridCell(9, "Name").Text, "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("LastName51 FirstName51 Patronymic51", Driver.GetGridCell(0, "Name").Text, "page 6 line 1");
            VerifyAreEqual("LastName42 FirstName42 Patronymic42", Driver.GetGridCell(9, "Name").Text, "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("LastName101 FirstName101 Patronymic101", Driver.GetGridCell(0, "Name").Text,
                "page 1 line 1");
            VerifyAreEqual("LastName92 FirstName92 Patronymic92", Driver.GetGridCell(9, "Name").Text, "page 1 line 10");
        }

        [Order(11)]
        [Test]
        public void SegmentsCustomerPageToPrevious()
        {
            //reset order
            Driver.FindElement(AdvBy.DataE2E("gridHeaderCell")).Click();
            while (Driver.FindElement(AdvBy.DataE2E("gridHeaderCell")).
                FindElement(By.ClassName("sortable")).GetAttribute("aria-sort") != "none")
            {
                Driver.FindElement(AdvBy.DataE2E("gridHeaderCell")).Click();
            }

            VerifyAreEqual("LastName101 FirstName101 Patronymic101", Driver.GetGridCell(0, "Name").Text,
                "page 1 line 1");
            VerifyAreEqual("LastName92 FirstName92 Patronymic92", Driver.GetGridCell(9, "Name").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("LastName91 FirstName91 Patronymic91", Driver.GetGridCell(0, "Name").Text, "page 2 line 1");
            VerifyAreEqual("LastName82 FirstName82 Patronymic82", Driver.GetGridCell(9, "Name").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("LastName81 FirstName81 Patronymic81", Driver.GetGridCell(0, "Name").Text, "page 3 line 1");
            VerifyAreEqual("LastName72 FirstName72 Patronymic72", Driver.GetGridCell(9, "Name").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("LastName91 FirstName91 Patronymic91", Driver.GetGridCell(0, "Name").Text, "page 2 line 1");
            VerifyAreEqual("LastName82 FirstName82 Patronymic82", Driver.GetGridCell(9, "Name").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("LastName101 FirstName101 Patronymic101", Driver.GetGridCell(0, "Name").Text,
                "page 1 line 1");
            VerifyAreEqual("LastName92 FirstName92 Patronymic92", Driver.GetGridCell(9, "Name").Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("LastName1 FirstName1 Patronymic1", Driver.GetGridCell(0, "Name").Text, "last page line 1");
        }
    }
}