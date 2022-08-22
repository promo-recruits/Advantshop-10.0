using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.CustomerSegments
{
    [TestFixture]
    public class CustomerSegmentsTest : BaseSeleniumTest
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
        public void SegmentsGrid()
        {
            GoToAdmin("settingscustomers#?tab=customerSegments");

            VerifyAreEqual("Сегменты покупателей",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerSegmentTitle\"]")).Text, "segment page h1");
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "segment page count");

            VerifyAreEqual("CustomerSegment105", Driver.GetGridCell(0, "Name", "Segments").Text, "segment's grid name");
            VerifyAreEqual("101", Driver.GetGridCell(0, "CustomersCount", "Segments").Text,
                "segment's grid CustomersCount");
            VerifyAreEqual("20.01.2018 16:11", Driver.GetGridCell(0, "CreatedDateFormatted", "Segments").Text,
                "segment's grid created date");
        }

        [Test]
        public void SegmentsGoToEditByName()
        {
            GoToAdmin("settingscustomers#?tab=customerSegments");

            Driver.GetGridCell(0, "Name", "Segments").Click();
            Driver.WaitForElem(By.Id("Name"));

            VerifyIsTrue(Driver.Url.Contains("edit"), "segment edit url");
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Сегмент \"CustomerSegment105\""),
                "segment edit h1");

            GoToAdmin("settingscustomers#?tab=customerSegments");
        }

        [Test]
        public void SegmentsGoToEditByServiceCol()
        {
            GoToAdmin("settingscustomers#?tab=customerSegments");

            Driver.GetGridCell(0, "_serviceColumn", "Segments")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Name"));

            VerifyIsTrue(Driver.Url.Contains("edit"), "segment edit url");
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Сегмент \"CustomerSegment105\""),
                "segment edit h1");

            GoToAdmin("settingscustomers#?tab=customerSegments");
        }


        [Test]
        public void SegmentzSelectDelete()
        {
            GoToAdmin("settingscustomers#?tab=customerSegments");

            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "Segments")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("CustomerSegment105", Driver.GetGridCell(0, "Name", "Segments").Text,
                "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Segments")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("CustomerSegment104", Driver.GetGridCell(0, "Name", "Segments").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Segments")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Segments")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Segments")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Segments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Segments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Segments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("CustomerSegment101", Driver.GetGridCell(0, "Name", "Segments").Text,
                "selected 1 grid delete");
            VerifyAreEqual("CustomerSegment100", Driver.GetGridCell(1, "Name", "Segments").Text,
                "selected 2 grid delete");
            VerifyAreEqual("CustomerSegment99", Driver.GetGridCell(2, "Name", "Segments").Text,
                "selected 3 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Segments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Segments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("CustomerSegment91", Driver.GetGridCell(0, "Name", "Segments").Text,
                "selected all on page 1 grid delete");
            VerifyAreEqual("CustomerSegment82", Driver.GetGridCell(9, "Name", "Segments").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("91", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "Segments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "Segments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");

            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all 2");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 2");
        }
    }

    [TestFixture]
    public class CustomerSegmentsFilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Orders | ClearType.Catalog);
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
        public void SegmentsFilterName()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "Name");

            //search by not exist 
            Driver.SetGridFilterValue("Name", "123123123 customer segments 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Name", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Name", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist
            Driver.SetGridFilterValue("Name", "CustomerSegment10");

            VerifyAreEqual("CustomerSegment105", Driver.GetGridCell(0, "Name", "Segments").Text,
                "customer segment line 1 filter Name");
            VerifyAreEqual("CustomerSegment10", Driver.GetGridCell(6, "Name", "Segments").Text,
                "customer segment line 7 filter Name");
            VerifyAreEqual("Найдено записей: 7",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Segments")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Name"));
            VerifyIsTrue(Driver.Url.Contains("edit"), "customer segment edit");
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Сегмент \"CustomerSegment105\""),
                "customer segment edit h1");

            GoBack();

            VerifyAreEqual("Найдено записей: 7",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Name");
            VerifyAreEqual("Найдено записей: 98",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name deleting 1");

            GoToAdmin("settingscustomers#?tab=customerSegments");
            VerifyAreEqual("Найдено записей: 98",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name deleting 2");
        }
    }
}