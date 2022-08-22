using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.Customer
{
    [TestFixture]
    public class CustomersTest : BaseSeleniumTest
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
        [Order(0)]
        public void CustomerGrid()
        {
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Покупатели"), "h1 customers grid");

            VerifyAreEqual("LastName120 FirstName120",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text, "customers Name grid");
            VerifyAreEqual("120", Driver.GetGridCell(0, "Phone", "Customers").Text, "customers Phone grid");
            VerifyAreEqual("test@mail.ru120", Driver.GetGridCell(0, "Email", "Customers").Text, "customers Email grid");
            VerifyAreEqual("0", Driver.GetGridCell(0, "OrdersCount", "Customers").Text, "customers OrdersCount grid");
            VerifyAreEqual("", Driver.GetGridCell(0, "LastOrderNumber", "Customers").Text,
                "customers LastOrderNumber grid");
            VerifyAreEqual("0", Driver.GetGridCell(0, "OrdersSum", "Customers").Text, "customers OrdersSum grid");
            VerifyAreEqual("15.08.2017 15:37", Driver.GetGridCell(0, "RegistrationDateTimeFormatted", "Customers").Text,
                "customers reg date grid");
            VerifyAreEqual("ManagerName2 ManagerLastName2", Driver.GetGridCell(0, "ManagerName", "Customers").Text,
                "customers ManagerName grid");

            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all customers");
        }

        [Test]
        [Order(1)]
        public void CustomerGoToEditByName()
        {
            Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            Driver.FindElement(By.LinkText("Редактировать")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lead-info-inner")).FindElement(By.TagName("h1")).Text
                    .Contains("LastName120 FirstName120"), "customer edit");
            VerifyIsTrue(Driver.Url.Contains("customerIdInfo"), "url customer edit");
        }

        [Test]
        [Order(1)]
        public void CustomerGoToLastOrder()
        {
            Driver.GridFilterSendKeys("FirstName5 LastName5");
            Driver.Blur();

            VerifyAreEqual("LastName5 FirstName5",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer with last order");
            Driver.GetGridCell(0, "LastOrderNumber", "Customers").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Заказ № 30"), "customer last order edit");
            VerifyIsTrue(Driver.Url.Contains("edit/30"), "url customer last order edit");
        }

        [Test]
        [Order(1)]
        public void CustomerGoToEditByServiceCol()
        {
            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".lead-info-inner")).FindElement(By.TagName("h1")).Text
                    .Contains("LastName120 FirstName120"), "customer edit");
            VerifyIsTrue(Driver.Url.Contains("customerIdInfo"), "url customer edit");
        }

        [Test]
        [Order(10)]
        public void CustomerSelectDelete()
        {
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("LastName120 FirstName120",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text, "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("LastName120 FirstName120",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Customers")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Customers")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Customers")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Customers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Customers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Customers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("LastName116 FirstName116",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text, "selected 2 grid delete");
            VerifyAreEqual("LastName115 FirstName115",
                Driver.GetGridCell(1, "Name", "Customers").FindElement(By.TagName("a")).Text, "selected 3 grid delete");
            VerifyAreEqual("LastName114 FirstName114",
                Driver.GetGridCell(2, "Name", "Customers").FindElement(By.TagName("a")).Text, "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Customers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Customers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("LastName106 FirstName106",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "selected all on page 2 grid delete");
            VerifyAreEqual("LastName97 FirstName97",
                Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("106", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "Customers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "Customers")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            GoToAdmin("customers");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");
        }
    }
}