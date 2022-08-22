using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.CustomerSegments
{
    [TestFixture]
    public class CustomerSegmentsAddTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Orders | ClearType.Catalog |
                                        ClearType.Shipping | ClearType.Payment);
            InitializeService.LoadData(
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerGroup.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Customer.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Contact.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Departments.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Managers.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerField.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.Product.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.Category.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.ProductCategories.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderContact.csv",
                "Data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderSource.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderStatus.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].PaymentMethod.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].ShippingMethod.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].[Order].csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderCurrency.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderItems.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderCustomer.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("customersegments/add");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        [Order(1)]
        public void FilterOrderSumAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Orders Sum");

            Driver.FindElement(By.Id("SegmentFilter_OrdersSumFrom")).Click();
            Driver.FindElement(By.Id("SegmentFilter_OrdersSumFrom")).Clear();
            Driver.FindElement(By.Id("SegmentFilter_OrdersSumFrom")).SendKeys("150");

            Driver.FindElement(By.Id("SegmentFilter_OrdersSumTo")).Click();
            Driver.FindElement(By.Id("SegmentFilter_OrdersSumTo")).Clear();
            Driver.FindElement(By.Id("SegmentFilter_OrdersSumTo")).SendKeys("200");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Orders Sum", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("7", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Сегмент \"New Segment Orders Sum\""),
                "segment added h1");

            VerifyAreEqual("150", Driver.FindElement(By.Id("SegmentFilter_OrdersSumFrom")).GetAttribute("value"),
                "segment filter orders sum from");
            VerifyAreEqual("200", Driver.FindElement(By.Id("SegmentFilter_OrdersSumTo")).GetAttribute("value"),
                "segment filter orders sum to");

            VerifyAreEqual("Найдено записей: 7",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter orders sum customers count");
            VerifyAreEqual("LastName37 FirstName37 Patronymic37", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f337");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Orders Sum"), "segment filter in customer edit");
        }

        [Test]
        [Order(1)]
        public void FilterOrderPaidSumAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Orders Paid Sum");

            Driver.FindElement(By.Id("SegmentFilter_OrdersPaidSumFrom")).Click();
            Driver.FindElement(By.Id("SegmentFilter_OrdersPaidSumFrom")).Clear();
            Driver.FindElement(By.Id("SegmentFilter_OrdersPaidSumFrom")).SendKeys("70");

            Driver.FindElement(By.Id("SegmentFilter_OrdersPaidSumTo")).Click();
            Driver.FindElement(By.Id("SegmentFilter_OrdersPaidSumTo")).Clear();
            Driver.FindElement(By.Id("SegmentFilter_OrdersPaidSumTo")).SendKeys("200");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Orders Paid Sum", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("15", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyAreEqual("70", Driver.FindElement(By.Id("SegmentFilter_OrdersPaidSumFrom")).GetAttribute("value"),
                "segment filter orders paid sum from");
            VerifyAreEqual("200", Driver.FindElement(By.Id("SegmentFilter_OrdersPaidSumTo")).GetAttribute("value"),
                "segment filter orders paid sum to");

            VerifyAreEqual("Найдено записей: 15",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter orders paid sum customers count");
            VerifyAreEqual("LastName50 FirstName50 Patronymic50", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f344");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Orders Paid Sum"), "segment filter in customer edit");
        }

        [Test]
        [Order(1)]
        public void FilterOrdersCountAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Orders Count");

            Driver.FindElement(By.Id("SegmentFilter_OrdersCountFrom")).Click();
            Driver.FindElement(By.Id("SegmentFilter_OrdersCountFrom")).Clear();
            Driver.FindElement(By.Id("SegmentFilter_OrdersCountFrom")).SendKeys("1");

            Driver.FindElement(By.Id("SegmentFilter_OrdersCountTo")).Click();
            Driver.FindElement(By.Id("SegmentFilter_OrdersCountTo")).Clear();
            Driver.FindElement(By.Id("SegmentFilter_OrdersCountTo")).SendKeys("2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Orders Count", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("37", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyAreEqual("1", Driver.FindElement(By.Id("SegmentFilter_OrdersCountFrom")).GetAttribute("value"),
                "segment filter orders count from");
            VerifyAreEqual("2", Driver.FindElement(By.Id("SegmentFilter_OrdersCountTo")).GetAttribute("value"),
                "segment filter orders count to");

            VerifyAreEqual("Найдено записей: 37",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter orders count customers count");
            VerifyAreEqual("LastName50 FirstName50 Patronymic50", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f340");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Orders Count"), "segment filter in customer edit");
        }

        [Test]
        [Order(1)]
        public void FilterCityAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment City");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCities\"]")).Click();
            Driver.WaitForElem(By.CssSelector("span.ui-select-choices-row-inner"));
            Driver.XPathContainsText("span", "Москва");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment City", Driver.GetGridCell(0, "Name", "Segments").Text, "segment grid name");
            VerifyAreEqual("28", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCities\"]")).Text.Contains("Москва"),
                "segment filter city");

            VerifyAreEqual("Найдено записей: 28",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter city");
            VerifyAreEqual("LastName47 FirstName47 Patronymic47", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f344");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment City"), "segment filter in customer edit");
        }


        [Test]
        [Order(1)]
        public void FilterCountryAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Country");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCountries\"]")).Click();
            Driver.WaitForElem(By.CssSelector("span.ui-select-choices-row-inner"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCountries\"] input")).SendKeys("Укр");
            Driver.XPathContainsText("span", "Украина");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Country", Driver.GetGridCell(0, "Name", "Segments").Text, "segment grid name");
            VerifyAreEqual("6", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCountries\"]")).Text.Contains("Украина"),
                "segment filter country");

            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter country");
            VerifyAreEqual("LastName41 FirstName41 Patronymic41", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f338");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Country"), "segment filter in customer edit");
        }

        [Test]
        [Order(1)]
        public void FilterLastOrderDateAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Last Order Date");

            Driver.FindElement(By.Id("SegmentFilter_LastOrderDateFrom")).Click();
            Driver.FindElement(By.Id("SegmentFilter_LastOrderDateFrom")).Clear();
            Driver.FindElement(By.Id("SegmentFilter_LastOrderDateFrom")).SendKeys("01.09.2016");

            Driver.FindElement(By.Id("SegmentFilter_LastOrderDateTo")).Click();
            Driver.FindElement(By.Id("SegmentFilter_LastOrderDateTo")).Clear();
            Driver.FindElement(By.Id("SegmentFilter_LastOrderDateTo")).SendKeys("10.09.2016");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Last Order Date", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("13", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyAreEqual("01.09.2016",
                Driver.FindElement(By.Id("SegmentFilter_LastOrderDateFrom")).GetAttribute("value"),
                "segment filter last order date from");
            VerifyAreEqual("10.09.2016",
                Driver.FindElement(By.Id("SegmentFilter_LastOrderDateTo")).GetAttribute("value"),
                "segment filter last order date to");

            VerifyAreEqual("Найдено записей: 13",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter last order date customers count");
            VerifyAreEqual("LastName50 FirstName50 Patronymic50", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f340");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Last Order Date"), "segment filter in customer edit");
        }

        [Test]
        [Order(1)]
        public void FilterAverageCheckAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Average Check");

            Driver.FindElement(By.Id("SegmentFilter_AverageCheckFrom")).Click();
            Driver.FindElement(By.Id("SegmentFilter_AverageCheckFrom")).Clear();
            Driver.FindElement(By.Id("SegmentFilter_AverageCheckFrom")).SendKeys("50");

            Driver.FindElement(By.Id("SegmentFilter_AverageCheckTo")).Click();
            Driver.FindElement(By.Id("SegmentFilter_AverageCheckTo")).Clear();
            Driver.FindElement(By.Id("SegmentFilter_AverageCheckTo")).SendKeys("60");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Average Check", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("13", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyAreEqual("50", Driver.FindElement(By.Id("SegmentFilter_AverageCheckFrom")).GetAttribute("value"),
                "segment filter average check from");
            VerifyAreEqual("60", Driver.FindElement(By.Id("SegmentFilter_AverageCheckTo")).GetAttribute("value"),
                "segment filter average check to");

            VerifyAreEqual("Найдено записей: 13",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter average check customers count");
            VerifyAreEqual("LastName34 FirstName34 Patronymic34", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f315");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Average Check"), "segment filter in customer edit");
        }

        [Test]
        [Order(1)]
        public void FilterCategoriesAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Categories");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCategories\"]")).Click();
            //driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCategories\"]")).SendKeys("TestCategory5"); 
            Driver.WaitForElem(By.CssSelector("span.ui-select-choices-row-inner"));
            //driver.FindElement(By.CssSelector("span.ui-select-choices-row-inner")).Click();
            Driver.XPathContainsText("span", "TestCategory5");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Categories", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("20", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCategories\"]")).Text
                    .Contains("TestCategory5"), "segment filter categories");

            VerifyAreEqual("Найдено записей: 20",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter categories");
            VerifyAreEqual("LastName37 FirstName37 Patronymic37", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f318");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Categories"), "segment filter in customer edit");
        }

        [Test]
        [Order(1)]
        public void FilterNoCustomerMatchAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment No Customers Match");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCities\"]")).Click();
            Driver.WaitForElem(By.CssSelector("span.ui-select-choices-row-inner"));
            Driver.XPathContainsText("span", "Санкт-Петербург");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment No Customers Match", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("0", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCities\"]")).Text
                    .Contains("Санкт-Петербург"), "segment filter");

            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter city");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]")).Text
                    .Contains("Ни одной записи не найдено"), "segment no customers match");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f344");
            VerifyIsFalse(Driver.PageSource.Contains("New Segment No Customers Match"),
                "segment filter in customer edit 1");

            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c4");
            VerifyIsFalse(Driver.PageSource.Contains("New Segment No Customers Match"),
                "segment filter in customer edit 2");

            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c8");
            VerifyIsFalse(Driver.PageSource.Contains("New Segment No Customers Match"),
                "segment filter in customer edit 3");
        }

        [Test]
        [Order(1)]
        public void FilterNoAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment No Filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment No Filter", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("50", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyAreEqual("Найдено записей: 50",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter city");
            VerifyAreEqual("LastName50 FirstName50 Patronymic50", Driver.GetGridCell(0, "Name").Text,
                "segment customer name 1");
            VerifyAreEqual("LastName41 FirstName41 Patronymic41", Driver.GetGridCell(9, "Name").Text,
                "segment customer name 10");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f344");
            VerifyIsTrue(Driver.PageSource.Contains("New Segment No Filter"), "segment filter in customer edit 1");

            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c4");
            VerifyIsTrue(Driver.PageSource.Contains("New Segment No Filter"), "segment filter in customer edit 2");

            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c8");
            VerifyIsTrue(Driver.PageSource.Contains("New Segment No Filter"), "segment filter in customer edit 3");

            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f350");
            VerifyIsTrue(Driver.PageSource.Contains("New Segment No Filter"), "segment filter in customer edit 3");
        }
    }

    [TestFixture]
    public class CustomerSegmentsEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Orders | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerGroup.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Customer.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Contact.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Departments.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Managers.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerField.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.Product.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.Category.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.ProductCategories.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderContact.csv",
                "Data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderSource.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderStatus.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].PaymentMethod.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].ShippingMethod.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].[Order].csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderCustomer.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderCurrency.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderItems.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerSegment.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerSegment_Customer.csv"
            );

            Init();
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
        [Order(1)]
        public void AddAnotherFilter()
        {
            //pre check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("Из Москвы", Driver.GetGridCell(3, "Name", "Segments").Text, "pre check segment grid name");
            VerifyAreEqual("28", Driver.GetGridCell(3, "CustomersCount", "Segments").Text,
                "pre check segment grid customers count");

            //pre check check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c2");

            VerifyIsTrue(Driver.PageSource.Contains("Из Москвы"), "pre check segment filter in customer edit");

            //pre check segments edit
            GoToAdmin("customersegments/edit/3");

            VerifyAreEqual("Из Москвы", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "pre check segment's name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCities\"]")).Text.Contains("Москва"),
                "pre check segment default filter");
            VerifyAreEqual("Найдено записей: 28",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "pre check segment filter customers count");

            //edit
            Driver.ScrollToTop();
            Driver.FindElement(By.Id("SegmentFilter_OrdersCountTo")).Click();
            Driver.FindElement(By.Id("SegmentFilter_OrdersCountTo")).Clear();
            Driver.FindElement(By.Id("SegmentFilter_OrdersCountTo")).SendKeys("2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            //segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("Из Москвы", Driver.GetGridCell(3, "Name", "Segments").Text, "segment grid name");
            VerifyAreEqual("15", Driver.GetGridCell(3, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segments edit
            GoToAdmin("customersegments/edit/3");

            VerifyAreEqual("Найдено записей: 15",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter orders sum customers count");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCities\"]")).Text.Contains("Москва"),
                "segment default filter");
            VerifyAreEqual("2", Driver.FindElement(By.Id("SegmentFilter_OrdersCountTo")).GetAttribute("value"),
                "segment added filter");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c2");

            VerifyIsFalse(Driver.PageSource.Contains("Из Москвы"), "segment filter in customer edit");
        }

        [Test]
        [Order(1)]
        public void AddSameFilter()
        {
            //pre check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("1 категория", Driver.GetGridCell(4, "Name", "Segments").Text,
                "pre check segment grid name");
            VerifyAreEqual("14", Driver.GetGridCell(4, "CustomersCount", "Segments").Text,
                "pre check segment grid customers count");

            //pre check check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f334");

            VerifyIsFalse(Driver.PageSource.Contains("1 категория"), "pre check segment filter in customer edit");

            //pre check segments edit
            GoToAdmin("customersegments/edit/4");

            VerifyAreEqual("1 категория", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "pre check segment's name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCategories\"]")).Text
                    .Contains("TestCategory2"), "pre check segment default filter");
            VerifyAreEqual("Найдено записей: 14",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "pre check segment filter customers count");

            //edit
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCategories\"]"))
                .FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCategories\"]"))
                .FindElement(By.TagName("input")).SendKeys("TestCategory3");
            Driver.WaitForElem(By.CssSelector("span.ui-select-choices-row-inner"));
            Driver.FindElement(By.CssSelector("span.ui-select-choices-row-inner")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            //segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("1 категория", Driver.GetGridCell(4, "Name", "Segments").Text, "segment grid name");
            VerifyAreEqual("34", Driver.GetGridCell(4, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segments edit
            GoToAdmin("customersegments/edit/4");

            VerifyAreEqual("Найдено записей: 34",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter orders sum customers count");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCategories\"]")).Text
                    .Contains("TestCategory2"), "segment default filter");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCategories\"]")).Text
                    .Contains("TestCategory3"), "segment added filter");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f334");

            VerifyIsTrue(Driver.PageSource.Contains("1 категория"), "segment filter in customer edit");
        }


        [Test]
        [Order(1)]
        public void DeleteOneOfSeveralFilter()
        {
            //pre check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("из Москвы и до 2 заказов", Driver.GetGridCell(5, "Name", "Segments").Text,
                "pre check segment grid name");
            VerifyAreEqual("15", Driver.GetGridCell(5, "CustomersCount", "Segments").Text,
                "pre check segment grid customers count");

            //pre check check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f315");

            VerifyIsFalse(Driver.PageSource.Contains("из Москвы и до 2 заказов"),
                "pre check segment filter in customer edit");

            //pre check segments edit
            GoToAdmin("customersegments/edit/1");

            VerifyAreEqual("из Москвы и до 2 заказов", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "pre check segment's name");
            VerifyAreEqual("2", Driver.FindElement(By.Id("SegmentFilter_OrdersCountTo")).GetAttribute("value"),
                "pre check segment default filter 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCities\"]")).Text.Contains("Москва"),
                "pre check segment default filter 2");
            VerifyAreEqual("Найдено записей: 15",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "pre check segment filter customers count");

            //edit delete filter
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCities\"]"))
                .FindElement(By.CssSelector(".close.ui-select-match-close")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            //segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("из Москвы и до 2 заказов", Driver.GetGridCell(5, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("37", Driver.GetGridCell(5, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segments edit
            GoToAdmin("customersegments/edit/1");

            VerifyAreEqual("Найдено записей: 37",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter orders sum customers count");
            VerifyAreEqual("2", Driver.FindElement(By.Id("SegmentFilter_OrdersCountTo")).GetAttribute("value"),
                "segment left filter");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCities\"]")).Text.Contains("Москва"),
                "segment deleted filter");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f315");

            VerifyIsTrue(Driver.PageSource.Contains("из Москвы и до 2 заказов"), "segment filter in customer edit");
        }

        [Test]
        [Order(1)]
        public void ChangeNameFilter()
        {
            //pre check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("2 категории", Driver.GetGridCell(6, "Name", "Segments").Text,
                "pre check segment grid name");

            //pre check check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f327");

            VerifyIsTrue(Driver.PageSource.Contains("2 категории"), "pre check segment filter in customer edit");

            //pre check segments edit
            GoToAdmin("customersegments/edit/2");

            VerifyAreEqual("2 категории", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "pre check segment's name");

            //edit 
            Driver.ScrollToTop();
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("edited segment's name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            //segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");

            //check prev segment's name
            Driver.GridFilterSendKeys("2 категории");
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "segment grid prev name");

            //check new segment's nam
            Driver.GridFilterSendKeys("edited segment's name");
            Driver.Blur();

            VerifyAreEqual("edited segment's name", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid new name");

            //check segments edit
            GoToAdmin("customersegments/edit/2");

            VerifyAreEqual("edited segment's name", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "segment's name edited");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f327");

            VerifyIsFalse(Driver.PageSource.Contains("2 категории"), "segment filter in customer edit prev name");
            VerifyIsTrue(Driver.PageSource.Contains("edited segment's name"),
                "segment filter in customer edit new name");
        }

        [Test]
        [Order(1)]
        public void CustomFieldFilterAddCustomFieldFilter()
        {
            //pre check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("Доп поле Текст", Driver.GetGridCell(1, "Name", "Segments").Text,
                "pre check segment grid name");
            VerifyAreEqual("4", Driver.GetGridCell(1, "CustomersCount", "Segments").Text,
                "pre check segment grid customers count");

            //pre check check customer's edit 1
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c1");

            VerifyIsTrue(Driver.PageSource.Contains("Доп поле Текст"), "pre check segment filter in customer edit 1");

            //pre check check customer's edit 2
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c5");

            VerifyIsTrue(Driver.PageSource.Contains("Доп поле Текст"), "pre check segment filter in customer edit 2");

            //pre check segments edit
            GoToAdmin("customersegments/edit/6");

            VerifyAreEqual("Доп поле Текст", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "pre check segment's name");
            VerifyAreEqual("text field", Driver.FindElement(By.Id("customerfields_1__value")).GetAttribute("value"),
                "pre check filter custom field in segment edit");

            //edit 
            Driver.ScrollToTop();
            (new SelectElement(Driver.FindElement(By.Id("customerfields_0__value")))).SelectByText(
                "Customer Field 1 Value 3");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("Доп поле Текст", Driver.GetGridCell(1, "Name", "Segments").Text,
                "segment edited grid name");
            VerifyAreEqual("1", Driver.GetGridCell(1, "CustomersCount", "Segments").Text,
                "segment edited grid customers count");

            //check segment's edit
            GoToAdmin("customersegments/edit/6");

            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Сегмент \"Доп поле Текст\""),
                "segment edit h1");
            VerifyAreEqual("Доп поле Текст", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "segment edit name");

            VerifyAreEqual("text field", Driver.FindElement(By.Id("customerfields_1__value")).GetAttribute("value"),
                "segment edit default custom field filter");

            IWebElement selectCustomerField = Driver.FindElement(By.Id("customerfields_0__value"));
            SelectElement select1 = new SelectElement(selectCustomerField);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Customer Field 1 Value 3"),
                "segment edit added custom field filter");

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter custom field text customers count");
            VerifyAreEqual("LastName1 FirstName1 Patronymic1", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit 1
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c1");

            VerifyIsTrue(Driver.PageSource.Contains("Доп поле Текст"), "segment filter in customer edit 1");

            //check customer's edit 2
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c5");

            VerifyIsFalse(Driver.PageSource.Contains("Доп поле Текст"), "segment filter in customer edit 2");
        }

        [Test]
        [Order(1)]
        public void DefaultFilterAddCustomFieldFilter()
        {
            //pre check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("для доп поля, Москва", Driver.GetGridCell(2, "Name", "Segments").Text,
                "pre check segment grid name");
            VerifyAreEqual("28", Driver.GetGridCell(2, "CustomersCount", "Segments").Text,
                "pre check segment grid customers count");

            //pre check check customer's edit 1
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c1");

            VerifyIsTrue(Driver.PageSource.Contains("для доп поля, Москва"),
                "pre check segment filter in customer edit 1");

            //pre check check customer's edit 2
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f313");

            VerifyIsTrue(Driver.PageSource.Contains("для доп поля, Москва"),
                "pre check segment filter in customer edit 2");

            //pre check segments edit
            GoToAdmin("customersegments/edit/7");

            VerifyAreEqual("для доп поля, Москва", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "pre check segment's name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCities\"]")).Text.Contains("Москва"),
                "pre check default filter in segment edit");

            //edit 
            Driver.ScrollToTop();
            (new SelectElement(Driver.FindElement(By.Id("customerfields_0__value")))).SelectByText(
                "Customer Field 1 Value 3");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("для доп поля, Москва", Driver.GetGridCell(2, "Name", "Segments").Text,
                "segment edited grid name");
            VerifyAreEqual("13", Driver.GetGridCell(2, "CustomersCount", "Segments").Text,
                "segment edited grid customers count");

            //check segment's edit
            GoToAdmin("customersegments/edit/7");

            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Сегмент \"для доп поля, Москва\""),
                "segment edit h1");
            VerifyAreEqual("для доп поля, Москва", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "segment edit name");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerSegmentCities\"]")).Text.Contains("Москва"),
                "segment edit default filter");

            IWebElement selectCustomerField = Driver.FindElement(By.Id("customerfields_0__value"));
            SelectElement select1 = new SelectElement(selectCustomerField);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Customer Field 1 Value 3"),
                "segment edit added custom field filter");

            VerifyAreEqual("Найдено записей: 13",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter custom field text customers count");
            VerifyAreEqual("LastName47 FirstName47 Patronymic47", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit 1
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c1");

            VerifyIsTrue(Driver.PageSource.Contains("для доп поля, Москва"), "segment filter in customer edit 1");

            //check customer's edit 2
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f313");

            VerifyIsFalse(Driver.PageSource.Contains("для доп поля, Москва"), "segment filter in customer edit 2");
        }

        [Test]
        [Order(10)]
        public void DeleteSegmentFromEdit()
        {
            //pre check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("сегмент удалить", Driver.GetGridCell(0, "Name", "Segments").Text,
                "pre check segment grid name");

            //pre check check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f313");

            VerifyIsTrue(Driver.PageSource.Contains("сегмент удалить"), "pre check segment filter in customer edit");

            //pre check segments edit
            GoToAdmin("customersegments/edit/5");

            Driver.ScrollToTop();
            VerifyAreEqual("сегмент удалить", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "pre check segment's name");

            //edit 
            Driver.FindElement(By.LinkText("Удалить")).Click();
            Driver.SwalConfirm();

            //segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");

            Driver.GridFilterSendKeys("сегмент удалить");
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "deleted segment");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f313");

            VerifyIsFalse(Driver.PageSource.Contains("сегмент удалить"), "deleted segment filter in customer edit");
        }
    }

    [TestFixture]
    public class CustomerSegmentsCustomFieldsAddTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Orders | ClearType.Catalog |
                                        ClearType.Shipping | ClearType.Payment);
            InitializeService.LoadData(
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerGroup.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Customer.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Contact.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Departments.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.Managers.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerField.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.Product.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.Category.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\Catalog.ProductCategories.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderContact.csv",
                "Data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderSource.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderStatus.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].PaymentMethod.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].ShippingMethod.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].[Order].csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderCurrency.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderItems.csv",
                "data\\Admin\\CustomerSegments\\CustomerSegmentsAddEdit\\[Order].OrderCustomer.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("customersegments/add");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void FilterCustomFieldSelectAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Custom Field Select");

            (new SelectElement(Driver.FindElement(By.Id("customerfields_0__value")))).SelectByText(
                "Customer Field 1 Value 3");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Custom Field Select", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("19", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyIsTrue(
                Driver.FindElement(By.TagName("h1")).Text.Contains("Сегмент \"New Segment Custom Field Select\""),
                "segment added h1");
            VerifyAreEqual("New Segment Custom Field Select", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "segment added name");

            IWebElement selectSegmentCustomerField = Driver.FindElement(By.Id("customerfields_0__value"));
            SelectElement select = new SelectElement(selectSegmentCustomerField);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Customer Field 1 Value 3"),
                "segment edit filter custom field select");

            VerifyAreEqual("Найдено записей: 19",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter custom field select customers count");
            VerifyAreEqual("LastName47 FirstName47 Patronymic47", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f341");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Custom Field Select"), "segment filter in customer");

            GoToAdmin("customers#?customerIdInfo=2c8fb106-8f07-499b-b06f-51b43076f341");

            IWebElement selectCustomerField = Driver.FindElement(By.Id("customerfields_0__value"));
            select = new SelectElement(selectCustomerField);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Customer Field 1 Value 3"),
                "filter custom field select in customer edit");
        }

        [Test]
        public void FilterCustomFieldNumberAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Custom Field Number");

            Driver.FindElement(By.Id("customerfields_2__value")).Click();
            Driver.FindElement(By.Id("customerfields_2__value")).Clear();
            Driver.FindElement(By.Id("customerfields_2__value")).SendKeys("50");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Custom Field Number", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("10", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyIsTrue(
                Driver.FindElement(By.TagName("h1")).Text.Contains("Сегмент \"New Segment Custom Field Number\""),
                "segment added h1");
            VerifyAreEqual("New Segment Custom Field Number", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "segment added name");

            VerifyAreEqual("50", Driver.FindElement(By.Id("customerfields_2__value")).GetAttribute("value"),
                "segment edit filter custom field number");

            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter custom field number customers count");
            VerifyAreEqual("LastName30 FirstName30 Patronymic30", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f321");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Custom Field Number"), "segment filter in customer");

            GoToAdmin("customers#?customerIdInfo=2c8fb106-8f07-499b-b06f-51b43076f321");

            VerifyAreEqual("50", Driver.FindElement(By.Id("customerfields_2__value")).GetAttribute("value"),
                "filter custom field number in customer edit");
        }

        [Test]
        public void FilterCustomFieldTextAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Custom Field Text");

            Driver.FindElement(By.Id("customerfields_1__value")).Click();
            Driver.FindElement(By.Id("customerfields_1__value")).Clear();
            Driver.FindElement(By.Id("customerfields_1__value")).SendKeys("text field");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Custom Field Text", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("4", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyIsTrue(
                Driver.FindElement(By.TagName("h1")).Text.Contains("Сегмент \"New Segment Custom Field Text\""),
                "segment added h1");
            VerifyAreEqual("New Segment Custom Field Text", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "segment added name");

            VerifyAreEqual("text field", Driver.FindElement(By.Id("customerfields_1__value")).GetAttribute("value"),
                "segment edit filter custom field text");

            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter custom field text customers count");
            VerifyAreEqual("LastName5 FirstName5 Patronymic5", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f3c1");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Custom Field Text"), "segment filter in customer");

            GoToAdmin("customers#?customerIdInfo=2c8fb106-8f07-499b-b06f-51b43076f3c1");

            VerifyAreEqual("text field", Driver.FindElement(By.Id("customerfields_1__value")).GetAttribute("value"),
                "filter custom field text in customer edit");
        }

        [Test]
        public void FilterCustomFieldMultilineTextAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Custom Field Multiline Text");

            Driver.FindElement(By.Id("customerfields_3__value")).Click();
            Driver.FindElement(By.Id("customerfields_3__value")).Clear();
            Driver.FindElement(By.Id("customerfields_3__value"))
                .SendKeys("line 1" + Keys.Enter + "line 2" + Keys.Enter + "line 3");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Custom Field Multiline Text", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("6", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyIsTrue(
                Driver.FindElement(By.TagName("h1")).Text
                    .Contains("Сегмент \"New Segment Custom Field Multiline Text\""), "segment added h1");
            VerifyAreEqual("New Segment Custom Field Multiline Text",
                Driver.FindElement(By.Id("Name")).GetAttribute("value"), "segment added name");

            VerifyAreEqual("line 1\r\nline 2\r\nline 3",
                Driver.FindElement(By.Id("customerfields_3__value")).GetAttribute("value"),
                "segment edit filter custom field multiline text");

            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter custom field multiline text customers count");
            VerifyAreEqual("LastName50 FirstName50 Patronymic50", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f348");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Custom Field Multiline Text"),
                "segment filter in customer");

            GoToAdmin("customers#?customerIdInfo=2c8fb106-8f07-499b-b06f-51b43076f348");

            VerifyAreEqual("line 1\r\nline 2\r\nline 3",
                Driver.FindElement(By.Id("customerfields_3__value")).GetAttribute("value"),
                "filter custom field multiline text in customer edit");
        }

        [Test]
        public void FilterCustomFieldDateAdd()
        {
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New Segment Custom Field Date");

            Driver.FindElement(By.Id("customerfields_4_value")).Click();
            Driver.FindElement(By.Id("customerfields_4_value")).Clear();
            Driver.FindElement(By.Id("customerfields_4_value")).SendKeys("04.09.2016");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();

            string url = Driver.Url;

            //check segments grid
            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("New Segment Custom Field Date", Driver.GetGridCell(0, "Name", "Segments").Text,
                "segment grid name");
            VerifyAreEqual("9", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment grid customers count");

            //check segment's edit
            Driver.Navigate().GoToUrl(url);

            VerifyIsTrue(
                Driver.FindElement(By.TagName("h1")).Text.Contains("Сегмент \"New Segment Custom Field Date\""),
                "segment added h1");
            VerifyAreEqual("New Segment Custom Field Date", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                "segment added name");

            VerifyAreEqual("04.09.2016", Driver.FindElement(By.Id("customerfields_4_value")).GetAttribute("value"),
                "segment edit filter custom field date");

            VerifyAreEqual("Найдено записей: 9",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment filter custom field date customers count");
            VerifyAreEqual("LastName20 FirstName20 Patronymic20", Driver.GetGridCell(0, "Name").Text,
                "segment customer name");

            //check customer's edit
            GoToAdmin("customers/view/2c8fb106-8f07-499b-b06f-51b43076f319");

            VerifyIsTrue(Driver.PageSource.Contains("New Segment Custom Field Date"), "segment filter in customer");

            GoToAdmin("customers#?customerIdInfo=2c8fb106-8f07-499b-b06f-51b43076f319");

            VerifyAreEqual("04.09.2016", Driver.FindElement(By.Id("customerfields_4_value")).GetAttribute("value"),
                "filter custom field date in customer edit");
        }
    }
}