using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.Customer
{
    [TestFixture]
    public class CustomersFilterTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Orders | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Customers\\CustomersFilter\\Customers.CustomerGroup.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Customers.Customer.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Customers.Contact.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Customers.Departments.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Customers.Managers.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Customers.CustomerField.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Customers.VkUser.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Customers.InstagramUser.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Customers.FacebookUser.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Catalog.Product.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Catalog.Offer.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Catalog.Category.csv",
                "data\\Admin\\Customers\\CustomersFilter\\Catalog.ProductCategories.csv",
                "data\\Admin\\Customers\\CustomersFilter\\[Order].OrderContact.csv",
                "Data\\Admin\\Customers\\CustomersFilter\\[Order].OrderSource.csv",
                "data\\Admin\\Customers\\CustomersFilter\\[Order].OrderStatus.csv",
                "data\\Admin\\Customers\\CustomersFilter\\[Order].[Order].csv",
                "data\\Admin\\Customers\\CustomersFilter\\[Order].OrderCurrency.csv",
                "data\\Admin\\Customers\\CustomersFilter\\[Order].OrderItems.csv",
                "data\\Admin\\Customers\\CustomersFilter\\[Order].OrderCustomer.csv"
            );

            Init();
            GoToAdmin("customers");

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void FilterByName()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "Name");

            //search by not exist 
            Driver.SetGridFilterValue("Name", "123123123 name customer 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Name", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Name", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist
            Driver.SetGridFilterValue("Name", "LastName2");

            VerifyAreEqual("LastName29 FirstName29",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer Name line 1 filter Name");
            VerifyAreEqual("LastName20 FirstName20",
                Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer Name line 10 filter Name");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));
            VerifyIsTrue(Driver.Url.Contains("customerIdInfo"), "customer edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerInfoClose\"]")).Click();

            VerifyAreEqual("Найдено записей: 11",
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
            VerifyAreEqual("Найдено записей: 109",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 109",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name deleting 2");
        }


        [Test]
        public void FilterByPhone()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "Phone");

            //search by not exist 
            Driver.SetGridFilterValue("Phone", "123123123");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Phone", "11111111112222222222222222223333333333333344444444445555555555555555555555555555555");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Phone", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist 
            Driver.SetGridFilterValue("Phone", "55");

            VerifyAreEqual("LastName55 FirstName55",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer Name filter phone");
            VerifyAreEqual("55", Driver.GetGridCell(0, "Phone", "Customers").Text, "customer phone filter phone");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter phone");
            //check go to edit and back 
            Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            VerifyIsTrue(Driver.Url.Contains("customers/view"), "customer cart");

            GoBack(3);

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter phone return");
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Phone\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Phone");
            VerifyAreEqual("Найдено записей: 119",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter phone deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 119",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter phone deleting 2");
        }

        [Test]
        public void FilterByEmail()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "Email");

            //search by not exist 
            Driver.SetGridFilterValue("Email", "inbox@mail.ru");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Email", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Email", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist
            Driver.SetGridFilterValue("Email", "test@mail.ru1");

            VerifyAreEqual("LastName120 FirstName120",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer Name line 1 filter Email");
            VerifyAreEqual("test@mail.ru120", Driver.GetGridCell(0, "Email", "Customers").Text,
                "customer Email line 1 filter Email");
            VerifyAreEqual("LastName111 FirstName111",
                Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer Name line 10 filter Email");
            VerifyAreEqual("test@mail.ru111", Driver.GetGridCell(9, "Email", "Customers").Text,
                "customer Email line 10 filter Email");
            VerifyAreEqual("Найдено записей: 32",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Email");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));
            VerifyIsTrue(Driver.Url.Contains("customerIdInfo"), "customer edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerInfoClose\"]")).Click();

            VerifyAreEqual("Найдено записей: 32",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Email return");
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Email\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Email");
            VerifyAreEqual("Найдено записей: 88",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Email deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 88",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Email deleting 2");
        }

        [Test]
        public void FilterByOrdersCount()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "OrdersCount");

            //check min too much symbols
            Driver.SetGridFilterRange("OrdersCount", "1111111111111111111111111111111", "");
            //VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"), "filter OrdersCount min many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("OrdersCount", "", "1111111111111111111111111111111");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "filter max many symbols border color");

            //check min and max too much symbols
            Driver.SetGridFilterRange("OrdersCount", "1111111111111111111111111111111", "1111111111111111111111111111111");
            //VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"), "filter OrdersCount min/max many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter  min many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "filter  max many symbols");

            //check invalid symbols
            //  GoToAdmin("customers");
            // Functions.GridFilterSet(driver, baseURL, name: "OrdersCount");
            //check min invalid symbols
            Driver.SetGridFilterRange("OrdersCount", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter OrdersCount min imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersCount count customers min many symbols");

            //    GoToAdmin("customers");
            //   Functions.GridFilterSet(driver, baseURL, name: "OrdersCount");

            //check max invalid symbols
            Driver.SetGridFilterRange("OrdersCount", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter OrdersCount max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersCount count customers max many symbols");

            //check min and max invalid symbols

            //  GoToAdmin("customers");
            //  Functions.GridFilterSet(driver, baseURL, name: "OrdersCount");
            Driver.SetGridFilterRange("OrdersCount", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter OrdersCount both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter OrdersCount both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersCount count customers min/max many symbols");

            // GoToAdmin("customers");
            //  Functions.GridFilterSet(driver, baseURL, name: "OrdersCount");

            //check filter min not exist
            Driver.SetGridFilterRange("OrdersCount", "1000", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter OrdersCount min not exist");

            //check max not exist
            Driver.SetGridFilterRange("OrdersCount", "", "1000");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersCount max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("OrdersCount", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter OrdersCount min/max not exist");

            //check filter
            Driver.SetGridFilterRange("OrdersCount", "1", "5");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersCount");

            VerifyAreEqual("LastName4 FirstName4",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name OrdersCount line 1 filter OrdersCount");
            VerifyAreEqual("1", Driver.GetGridCell(0, "OrdersCount", "Customers").Text,
                "OrdersCount line 1 filter OrdersCount");
            VerifyAreEqual("LastName1 FirstName1",
                Driver.GetGridCell(3, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name OrdersCount line 4 filter OrdersCount");
            VerifyAreEqual("1", Driver.GetGridCell(3, "OrdersCount", "Customers").Text,
                "OrdersCount line 4 filter OrdersCount");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "OrdersCount");
            VerifyAreEqual("Найдено записей: 116",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersCount after deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 116",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersCount after deleting 2");
        }

        [Test]
        public void FilterByLastOrder()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "LastOrderNumber");

            //search by not exist 
            Driver.SetGridFilterValue("LastOrderNumber", "29");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("LastOrderNumber", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("LastOrderNumber", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist
            Driver.SetGridFilterValue("LastOrderNumber", "19");

            VerifyAreEqual("LastName4 FirstName4",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer Name filter LastOrder");
            VerifyAreEqual("# 19",
                Driver.GetGridCell(0, "LastOrderNumber", "Customers").FindElement(By.TagName("a")).Text,
                "customer LastOrder filter LastOrder");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter LastOrder");

            //check go to last order and back 
            Driver.GetGridCell(0, "LastOrderNumber", "Customers").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.Id("Order_ManagerId"));
            VerifyIsTrue(Driver.Url.Contains("edit"), "LastOrder edit");

            GoBack(2);

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter LastOrder return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"LastOrderNumber\"]")).Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "LastOrderNumber");
            VerifyAreEqual("Найдено записей: 119",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter LastOrder deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 119",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter LastOrder deleting 2");
        }

        [Test]
        public void FilterByOrdersSum()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "OrdersSum");

            //check min too much symbols
            Driver.SetGridFilterRange("OrdersSum", "1111111111111111111111111111111", "");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter min many symbols");
            //VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"), "filter OrdersSum min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("OrdersSum", "", "1111111111111111111111111111111");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "filter max many symbols border color");

            //check min and max too much symbols
            Driver.SetGridFilterRange("OrdersSum", "1111111111111111111111111111111", "1111111111111111111111111111111");
            // VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"), "filter OrdersSum min/max many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter  min many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "filter  max many symbols");

            //check invalid symbols
            //GoToAdmin("customers");
            //  Functions.GridFilterSet(driver, baseURL, name: "OrdersSum");
            //check min invalid symbols
            Driver.SetGridFilterRange("OrdersSum", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter OrdersSum min imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersSum count customers min many symbols");

            //  GoToAdmin("customers");
            // Functions.GridFilterSet(driver, baseURL, name: "OrdersSum");

            //check max invalid symbols
            Driver.SetGridFilterRange("OrdersSum", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter OrdersSum max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersSum count customers max many symbols");

            //check min and max invalid symbols

            //  GoToAdmin("customers");
            //  Functions.GridFilterSet(driver, baseURL, name: "OrdersSum");
            Driver.SetGridFilterRange("OrdersSum", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter OrdersSum both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter OrdersSum both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersSum count customers min/max many symbols");

            // GoToAdmin("customers");
            // Functions.GridFilterSet(driver, baseURL, name: "OrdersSum");

            //check filter min not exist
            Driver.SetGridFilterRange("OrdersSum", "1000", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter OrdersSum min not exist");

            //check max not exist
            Driver.SetGridFilterRange("OrdersSum", "", "1000");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersSum max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("OrdersSum", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter OrdersSum min/max not exist");

            //check filter
            Driver.SetGridFilterRange("OrdersSum", "1", "18");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersSum");

            VerifyAreEqual("LastName3 FirstName3",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name OrdersSum line 1 filter OrdersSum");
            VerifyAreEqual("18", Driver.GetGridCell(0, "OrdersSum", "Customers").Text,
                "OrdersSum line 1 filter OrdersSum");
            VerifyAreEqual("LastName1 FirstName1",
                Driver.GetGridCell(2, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name OrdersSum line 3 filter OrdersSum");
            VerifyAreEqual("16", Driver.GetGridCell(2, "OrdersSum", "Customers").Text,
                "OrdersSum line 3 filter OrdersSum");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "OrdersSum");
            VerifyAreEqual("Найдено записей: 117",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersSum after deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 117",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrdersSum after deleting 2");
        }

        [Test]
        public void FilterByRegisterDate()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "RegistrationDateTimeFormatted");

            //check filter min not exist
            Driver.SetGridFilterRange("RegistrationDateTimeFormatted", "31.12.2050 00:00", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter reg date min not exist");

            //check max not exist
            Driver.SetGridFilterRange("RegistrationDateTimeFormatted", "", "31.12.2050 00:00");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter reg date max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("RegistrationDateTimeFormatted", "31.12.2050 00:00", "31.12.2050 00:00"); 
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter reg date min/max not exist");

            //check filter reg date 
            Functions.DataTimePickerFilter(Driver, BaseUrl, monthFrom: "Июль", yearFrom: "2017",
                dataFrom: "Июль 1, 2017", hourFrom: "12", minFrom: "00", monthTo: "Август", yearTo: "2017",
                dataTo: "Август 15, 2017", hourTo: "21", minTo: "00", dropFocusElem: "h1",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]", fieldTo: "[data-e2e=\"datetimeFilterTo\"]");
            VerifyAreEqual("Найдено записей: 46",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter reg date");

            VerifyAreEqual("LastName120 FirstName120",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name filter reg date line 1");
            VerifyAreEqual("LastName111 FirstName111",
                Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name filter reg date line 10");
            VerifyAreEqual("15.08.2017 15:37", Driver.GetGridCell(0, "RegistrationDateTimeFormatted", "Customers").Text,
                "reg date filter reg date line 1");
            VerifyAreEqual("06.08.2017 15:37", Driver.GetGridCell(9, "RegistrationDateTimeFormatted", "Customers").Text,
                "reg date filter reg date line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "RegistrationDateTimeFormatted");
            VerifyAreEqual("Найдено записей: 74",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter reg date after deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 74",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter reg date after deleting 2");
        }

        [Test]
        public void FilterByManager()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "ManagerName",
                filterItem: "ManagerName1 ManagerLastName1");

            VerifyAreEqual("LastName109 FirstName109",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer Name line 1 filter ManagerName");
            VerifyAreEqual("LastName100 FirstName100",
                Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer Name line 10 filter ManagerName");
            VerifyAreEqual("ManagerName1 ManagerLastName1", Driver.GetGridCell(0, "ManagerName", "Customers").Text,
                "customer ManagerName line 1 filter ManagerName");
            VerifyAreEqual("ManagerName1 ManagerLastName1", Driver.GetGridCell(9, "ManagerName", "Customers").Text,
                "customer ManagerName line 10 filter ManagerName");
            VerifyAreEqual("Найдено записей: 75",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ManagerName");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));
            VerifyIsTrue(Driver.Url.Contains("customerIdInfo"), "customer edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerInfoClose\"]")).Click();

            VerifyAreEqual("Найдено записей: 75",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ManagerName return");
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"ManagerName\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "ManagerName");
            VerifyAreEqual("Найдено записей: 45",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ManagerName deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 45",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ManagerName deleting 2");
        }


        [Test]
        public void FilterByLocation()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnLocation");

            //search by not exist 
            Driver.SetGridFilterValue("_noopColumnLocation", "Казань");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnLocation", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnLocation", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist
            Driver.SetGridFilterValue("_noopColumnLocation", "Москва");

            VerifyAreEqual("LastName47 FirstName47",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer Name line 1 filter Location");
            VerifyAreEqual("LastName38 FirstName38",
                Driver.GetGridCell(9, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer Name line 10 filter Location");
            VerifyAreEqual("Найдено записей: 47",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Location");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));
            VerifyIsTrue(Driver.Url.Contains("customerIdInfo"), "customer edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerInfoClose\"]")).Click();

            VerifyAreEqual("Найдено записей: 47",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Location return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnLocation\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnLocation");
            VerifyAreEqual("Найдено записей: 73",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Location deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 73",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Location deleting 2");
        }

        [Test]
        public void FilterByAvgCheck()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnAvgCheck");

            //check min too much symbols
            Driver.SetGridFilterRange("_noopColumnAvgCheck", "1111111111111111111111111111111", "");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("_noopColumnAvgCheck", "", "1111111111111111111111111111111");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "filter max many symbols border color");

            //check min and max too much symbols
            Driver.SetGridFilterRange("_noopColumnAvgCheck", "1111111111111111111111111111111", "1111111111111111111111111111111");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter  min many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "filter  max many symbols");

            //check invalid symbols
            //   GoToAdmin("customers");
            //  Functions.GridFilterSet(driver, baseURL, name: "_noopColumnAvgCheck");
            //check min invalid symbols
            Driver.SetGridFilterRange("_noopColumnAvgCheck", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter AvgCheck min imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter AvgCheck count customers min many symbols");

            //   GoToAdmin("customers");
            //   Functions.GridFilterSet(driver, baseURL, name: "_noopColumnAvgCheck");

            //check max invalid symbols
            Driver.SetGridFilterRange("_noopColumnAvgCheck", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter AvgCheck max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter AvgCheck count customers max many symbols");

            //check min and max invalid symbols
            //   GoToAdmin("customers");
            //  Functions.GridFilterSet(driver, baseURL, name: "_noopColumnAvgCheck");

            Driver.SetGridFilterRange("_noopColumnAvgCheck", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter AvgCheck both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter AvgCheck both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter AvgCheck count customers min/max many symbols");

            //   GoToAdmin("customers");
            //   Functions.GridFilterSet(driver, baseURL, name: "_noopColumnAvgCheck");

            //check filter min not exist
            Driver.SetGridFilterRange("_noopColumnAvgCheck", "1000", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter AvgCheck min not exist");

            //check max not exist
            Driver.SetGridFilterRange("_noopColumnAvgCheck", "", "1000");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter AvgCheck max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("_noopColumnAvgCheck", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter AvgCheck min/max not exist");

            //check filter
            Driver.SetGridFilterRange("_noopColumnAvgCheck", "19", "25");
            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter AvgCheck");

            VerifyAreEqual("LastName5 FirstName5",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name AvgCheck line 1 filter AvgCheck");
            VerifyAreEqual("290", Driver.GetGridCell(0, "OrdersSum", "Customers").Text,
                "AvgCheck line 1 filter AvgCheck");
            VerifyAreEqual("LastName4 FirstName4",
                Driver.GetGridCell(1, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name AvgCheck line 2 filter AvgCheck");
            VerifyAreEqual("19", Driver.GetGridCell(1, "OrdersSum", "Customers").Text,
                "AvgCheck line 2 filter AvgCheck");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnAvgCheck");
            VerifyAreEqual("Найдено записей: 118",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter AvgCheck after deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 118",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter AvgCheck after deleting 2");
        }

        [Test]
        public void FilterBySocial()
        {
            //check filter vk
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnSocial",
                filterItem: "ВКонтакте");

            VerifyAreEqual("Найдено записей: 20",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Social Vk count");

            VerifyAreEqual("LastName119 FirstName119", Driver.GetGridCell(0, "Name", "Customers").Text,
                "filter Social Vk 1");
            VerifyAreEqual("LastName110 FirstName110", Driver.GetGridCell(9, "Name", "Customers").Text,
                "filter Social Vk 10");

            //check filter inst
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Instagram\"]")).Click();
            Driver.DropFocus("h1");

            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Social Inst count");

            VerifyAreEqual("LastName101 FirstName101", Driver.GetGridCell(0, "Name", "Customers").Text,
                "filter Social Inst 1");
            VerifyAreEqual("LastName14 FirstName14", Driver.GetGridCell(3, "Name", "Customers").Text,
                "filter Social Inst 4");

            //check filter any social
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Любой соц сети\"]")).Click();
            Driver.DropFocus("h1");

            VerifyAreEqual("Найдено записей: 33",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Social any count");

            VerifyAreEqual("LastName119 FirstName119", Driver.GetGridCell(0, "Name", "Customers").Text,
                "filter Social any 1");
            VerifyAreEqual("LastName110 FirstName110", Driver.GetGridCell(9, "Name", "Customers").Text,
                "filter Social any 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered Social items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnSocial");
            VerifyAreEqual("Найдено записей: 87",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Social after deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 87",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Social after deleting 2");
        }
    }
}