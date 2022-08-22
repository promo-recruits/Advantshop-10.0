using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.Order
{
    [TestFixture]
    public class OrdersFilter : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\Orders\\OrderGrid\\Customers.CustomerGroup.csv",
                "data\\Admin\\Orders\\OrderGrid\\Customers.Customer.csv",
                "data\\Admin\\Orders\\OrderGrid\\Customers.Managers.csv",
                "data\\Admin\\Orders\\OrderGrid\\Catalog.Product.csv",
                "data\\Admin\\Orders\\OrderGrid\\Catalog.Offer.csv",
                "data\\Admin\\Orders\\OrderGrid\\Catalog.Category.csv",
                "data\\Admin\\Orders\\OrderGrid\\Catalog.ProductCategories.csv",
                "data\\Admin\\Orders\\OrderGrid\\Customers.Contact.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderStatus.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].[Order].csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderCustomer.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderContact.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderCurrency.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderItems.csv",
                "data\\Admin\\Orders\\OrderGrid\\[Order].OrderSource.csv"
            );

            Init();

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("orders");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void FilterStatus()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnStatuses",
                filterItem: "Новый");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all new filtered");
            VerifyAreEqual("5", Driver.GetGridCell(0, "Number").Text, "line 1 status new filtered");
            VerifyAreEqual("1", Driver.GetGridCell(4, "Number").Text, "line 5 status new filtered");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"В обработке\"]")).Click();
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all in progress filtered");
            VerifyAreEqual("10", Driver.GetGridCell(0, "Number").Text, "line 1 status in progress filtered");
            VerifyAreEqual("6", Driver.GetGridCell(4, "Number").Text, "line 5 status in progress filtered");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Отправлен\"]")).Click();
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all sent filtered");
            VerifyAreEqual("15", Driver.GetGridCell(0, "Number").Text, "line 1 status sent filtered");
            VerifyAreEqual("11", Driver.GetGridCell(4, "Number").Text, "line 5 status sent filtered");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Доставлен\"]")).Click();
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all delivered filtered");
            VerifyAreEqual("25", Driver.GetGridCell(0, "Number").Text, "line 1 status delivered filtered");
            VerifyAreEqual("16", Driver.GetGridCell(9, "Number").Text, "line 10 status delivered filtered");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Отменён\"]")).Click();
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all canceled filtered");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "no orders with canceled status filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Отменен навсегда\"]")).Click();
            VerifyAreEqual("Найдено записей: 74",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all canceled forever filtered");
            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "line 1 status canceled forever filtered");
            VerifyAreEqual("92", Driver.GetGridCell(9, "Number").Text, "line 10 status canceled forever filtered");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnStatuses");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "line 1 filter closed");
            VerifyAreEqual("92", Driver.GetGridCell(9, "Number").Text, "line 10 filter closed");
        }


        [Test]
        public void FilterStatusDelete()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnStatuses",
                filterItem: "Отменен навсегда");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete all filtered");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all filtered after deleting");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnStatuses");
            VerifyAreEqual("Найдено записей: 25",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all after deleting");
            VerifyAreEqual("25", Driver.GetGridCell(0, "Number").Text, "line 1 after deleting");
            VerifyAreEqual("16", Driver.GetGridCell(9, "Number").Text, "line 10 after deleting");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 25",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterNumber()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnNumber");

            //search by not exist 
            Driver.SetGridFilterValue("_noopColumnNumber", "6754");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnNumber", "111111111122222222222222222222222222222");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("_noopColumnNumber", "2");

            VerifyAreEqual("92", Driver.GetGridCell(0, "Number").Text, "line 1 number filtered");
            VerifyAreEqual("27", Driver.GetGridCell(9, "Number").Text, "line 10 number filtered");
            VerifyAreEqual("Найдено записей: 19",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "number filter count");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Order_ManagerId"));

            VerifyIsTrue(Driver.Url.Contains("edit"), "filtered order edit");
            GoBack();

            VerifyAreEqual("Найдено записей: 19",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "number filter return");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnNumber\"]")).Displayed,
                "filter displayed");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all filtered after deleting");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnNumber");
            VerifyAreEqual("Найдено записей: 80",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 80",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterSum()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnSum");

            //check min too much symbols
            Driver.SetGridFilterRange("_noopColumnSum", "111111111", "");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "filter min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("_noopColumnSum", "", "111111111");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter max many symbols");

            //check min and max too much symbols
            Driver.SetGridFilterRange("_noopColumnSum", "111111111", "111111111");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "filter min/max many symbols");

            //check invalid symbols
            //check min invalid symbols
            Driver.SetGridFilterRange("_noopColumnSum", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter min imvalid symbols");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter count min many symbols");

            //check max invalid symbols
            Driver.SetGridFilterRange("_noopColumnSum", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter max imvalid symbols");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter count max many symbols");

            //check min and max invalid symbols
            Driver.SetGridFilterRange("_noopColumnSum", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter both max imvalid symbols");
            Driver.DropFocus("h1");
            // VerifyAreEqual("rgb(241, 89, 89)", driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).GetCssValue("border-color"), "filter sum min many symbols border color");
            // VerifyAreEqual("rgb(241, 89, 89)", driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"), "filter sum max many symbols border color");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter count min/max many symbols");

            //check filter min not exist
            Driver.SetGridFilterRange("_noopColumnSum", "90000", "");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "filter min not exist");

            //check max not exist
            Driver.SetGridFilterRange("_noopColumnSum", "", "90000");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("_noopColumnSum", "90000", "90000");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "filter min/max not exist");

            //check filter
            Driver.SetGridFilterRange("_noopColumnSum", "30", "35");
            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter count");

            VerifyAreEqual("33", Driver.GetGridCell(0, "Number").Text, "filter number line 1");
            VerifyAreEqual("33", Driver.GetGridCell(0, "SumFormatted").Text, "filter number line 1");
            VerifyAreEqual("32", Driver.GetGridCell(5, "Number").Text, "filter checkbox line 6");
            VerifyAreEqual("32", Driver.GetGridCell(5, "SumFormatted").Text, "filter checkbox line 6");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnSum");
            VerifyAreEqual("Найдено записей: 93",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 93",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterPaid()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnIsPaid", filterItem: "Да");

            VerifyAreEqual("Найдено записей: 85",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter yes count");
            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "filter number line 1");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-empty")).Count == 0,
                "order paid 1");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-not-empty")).Count == 1,
                "order paid no 1");
            VerifyAreEqual("92", Driver.GetGridCell(9, "Number").Text, "filter number line 10");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-empty")).Count == 0,
                "order paid 10");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-not-empty")).Count == 1,
                "order paid no 10");

            //check filter not paid
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();
            VerifyAreEqual("Найдено записей: 14",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter no count");
            VerifyAreEqual("14", Driver.GetGridCell(0, "Number").Text, "filter no number line 1");
            VerifyAreEqual("5", Driver.GetGridCell(9, "Number").Text, "filter no number line 10");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-empty")).Count == 1,
                "order paid 1");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-not-empty")).Count == 0,
                "order paid no 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-empty")).Count == 1,
                "order paid 10");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsPaid")
                    .FindElements(By.CssSelector("[data-e2e=\"switchOnOffSelect\"].ng-not-empty")).Count == 0,
                "order paid no 10");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnIsPaid");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "close filter count all");
        }


        [Test]
        public void FilterPaidDelete()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnIsPaid", filterItem: "Да");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete all filtered");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all filtered after deleting");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnIsPaid");
            VerifyAreEqual("Найдено записей: 14",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all after deleting");
            VerifyAreEqual("14", Driver.GetGridCell(0, "Number").Text, "line 1 after deleting");
            VerifyAreEqual("5", Driver.GetGridCell(9, "Number").Text, "line 10 after deleting");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 14",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterFIO()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnName");

            //search by not exist 
            Driver.SetGridFilterValue("_noopColumnName", "FirstName3 LastName5");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnName", "111111111122222222222222222222222222222");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("_noopColumnName", "FirstName3");

            VerifyAreEqual("98", Driver.GetGridCell(0, "Number").Text, "line 1 number FIO filtered items");
            VerifyAreEqual("33", Driver.GetGridCell(9, "Number").Text, "line 10 number FIO filtered items");
            VerifyAreEqual("FirstName3 LastName3", Driver.GetGridCell(0, "BuyerName").Text,
                "line 1 customer FIO filtered items");
            VerifyAreEqual("FirstName3 LastName3", Driver.GetGridCell(9, "BuyerName").Text,
                "line 10 customer FIO filtered items");
            VerifyAreEqual("Найдено записей: 14",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "FIO filter count");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all filtered after deleting");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnName");
            VerifyAreEqual("Найдено записей: 85",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 85",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterPhone()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnPhone");

            //search by not exist 
            Driver.SetGridFilterValue("_noopColumnPhone", "+7 495 900 900 99");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnPhone", "111111111122222222222222222222222222222");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("_noopColumnPhone", "+7 495 800 200 05");

            VerifyAreEqual("95", Driver.GetGridCell(0, "Number").Text, "line 1 number phone filtered items");
            VerifyAreEqual("80", Driver.GetGridCell(9, "Number").Text, "line 10 number phone filtered items");
            VerifyAreEqual("FirstName5 LastName5", Driver.GetGridCell(0, "BuyerName").Text,
                "line 1 customer phone filtered items");
            VerifyAreEqual("FirstName5 LastName5", Driver.GetGridCell(9, "BuyerName").Text,
                "line 10 customer phone filtered items");
            VerifyAreEqual("Найдено записей: 43",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "phone filter count");

            //check close filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnPhone");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "close filter count all");
        }

        [Test]
        public void FilterPhoneDelete()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnPhone");

            //search by not exist 
            Driver.SetGridFilterValue("_noopColumnPhone", "+7 495 800 200 02");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all filtered after deleting");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnPhone");
            VerifyAreEqual("Найдено записей: 85",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 85",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterEmail()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnEmail");

            //search by not exist 
            Driver.SetGridFilterValue("_noopColumnEmail", "test555@mail.ru4");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnEmail", "111111111122222222222222222222222222222");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("_noopColumnEmail", "test@mail.ru3");

            VerifyAreEqual("98", Driver.GetGridCell(0, "Number").Text, "line 1 number email filtered items");
            VerifyAreEqual("33", Driver.GetGridCell(9, "Number").Text, "line 10 number email filtered items");
            VerifyAreEqual("FirstName3 LastName3", Driver.GetGridCell(0, "BuyerName").Text,
                "line 1 customer email filtered items");
            VerifyAreEqual("FirstName3 LastName3", Driver.GetGridCell(9, "BuyerName").Text,
                "line 10 customer email filtered items");
            VerifyAreEqual("Найдено записей: 14",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "email filter count");

            //check close filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnEmail");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "close filter count all");
        }

        [Test]
        public void FilterEmailDelete()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnEmail");

            //search by not exist 
            Driver.SetGridFilterValue("_noopColumnEmail", "test@mail.ru4");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all filtered after deleting");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnEmail");
            VerifyAreEqual("Найдено записей: 85",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 85",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterCity()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnCity");

            //search by not exist 
            Driver.SetGridFilterValue("_noopColumnCity", "Самара");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnCity", "111111111122222222222222222222222222222");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("_noopColumnCity", "Москва");

            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "line 1 number city filtered items");
            VerifyAreEqual("73", Driver.GetGridCell(9, "Number").Text, "line 10 number city filtered items");
            VerifyAreEqual("FirstName1 LastName1", Driver.GetGridCell(0, "BuyerName").Text,
                "line 1 customer city filtered items");
            VerifyAreEqual("FirstName3 LastName3", Driver.GetGridCell(9, "BuyerName").Text,
                "line 10 customer city filtered items");
            VerifyAreEqual("Найдено записей: 42",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "city filter count");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all filtered after deleting");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCity");
            VerifyAreEqual("Найдено записей: 57",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 57",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterManager()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnManager",
                filterItem: "Manager Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            var optionsCount = Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]"))
                .FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count;

            VerifyIsTrue(optionsCount == 4, "count all managers in filter"); //2 managers 
            Driver.DropFocus("h1");

            VerifyAreEqual("Найдено записей: 68",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all manager 1 filtered");
            VerifyAreEqual("83", Driver.GetGridCell(0, "Number").Text, "line 1 number manager 1 filtered");
            VerifyAreEqual("79", Driver.GetGridCell(9, "Number").Text, "line 10 number manager 1 filtered");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Manager2 Name2\"]")).Click();
            VerifyAreEqual("Найдено записей: 28",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all manager 2 filtered"); //exept 3 drafts among all
            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "line 1 number manager 2 filtered");
            VerifyAreEqual("92", Driver.GetGridCell(9, "Number").Text, "line 10 number manager 1 filtered");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnManager");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "line 1 filter closed");
            VerifyAreEqual("92", Driver.GetGridCell(9, "Number").Text, "line 10 filter closed");
        }

        [Test]
        public void FilterManagerDelete()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnManager",
                filterItem: "Manager Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete all filtered");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all filtered after deleting");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnManager");
            VerifyAreEqual("Найдено записей: 31",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all after deleting");
            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "line 1 number after deleting");
            VerifyAreEqual("92", Driver.GetGridCell(9, "Number").Text, "line 10 number after deleting");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 31",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterShipping()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnShippings");

            //search by not exist 
            Driver.SetGridFilterValue("_noopColumnShippings", "Пункт выдачи в постаматах PickPoint");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnShippings", "111111111122222222222222222222222222222");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("_noopColumnShippings", "Курьером");

            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "line 1 number shipping filtered items");
            VerifyAreEqual("98", Driver.GetGridCell(2, "Number").Text, "line 3 number shipping filtered items");
            VerifyAreEqual("Найдено записей: 87",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "shipping filter count");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all filtered after deleting");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnShippings");
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterPayment()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnPayments",
                filterItem: "Банковский перевод для юр. лиц");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            var optionsCount = Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]"))
                .FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count;

            VerifyIsTrue(optionsCount == 2, "count all payment methods in filter"); //2 payment methods
            Driver.DropFocus("h1");

            VerifyAreEqual("Найдено записей: 77",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all certificate payment filtered");
            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "line 1 certificate payment filtered");
            VerifyAreEqual("97", Driver.GetGridCell(1, "Number").Text, "line 2 certificate payment filtered");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"При получении (наличными или банковской картой)\"]"))
                .Click();
            VerifyAreEqual("Найдено записей: 8",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all cash payment filtered");
            VerifyAreEqual("22", Driver.GetGridCell(0, "Number").Text, "line 1 cash payment filtered");
            VerifyAreEqual("15", Driver.GetGridCell(7, "Number").Text, "line 10 wallet payment filtered");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all filtered after deleting");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnPayments");
            VerifyAreEqual("Найдено записей: 91",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 91",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterOrderSource()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnSources",
                filterItem: "В один клик");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            var optionsCount = Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]"))
                .FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count;

            VerifyIsTrue(optionsCount == 12, "count all order sources in filter"); //12 order sources
            Driver.DropFocus("h1");

            VerifyAreEqual("Найдено записей: 8",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all one click order source filtered");
            VerifyAreEqual("96", Driver.GetGridCell(0, "Number").Text, "line 1 one click order source filtered");
            VerifyAreEqual("97", Driver.GetGridCell(1, "Number").Text, "line 2 one click order source filtered");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нашли дешевле\"]")).Click();
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all find cheaper order source filtered");
            VerifyAreEqual("22", Driver.GetGridCell(0, "Number").Text, "line 1 find cheaper order source filtered");
            VerifyAreEqual("21", Driver.GetGridCell(1, "Number").Text, "line 2 find cheaper order source filtered");
            VerifyAreEqual("20", Driver.GetGridCell(2, "Number").Text, "line 3 find cheaper order source filtered");

            Driver.ScrollToUISelect("Обратный звонок");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Обратный звонок\"]")).Click();
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all callback source filtered");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "callback order source no orders filtered");

            Driver.ScrollToUISelect("Корзина интернет магазина");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Корзина интернет магазина\"]")).Click();
            VerifyAreEqual("Найдено записей: 44",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all shopping cart order source filtered");
            VerifyAreEqual("63", Driver.GetGridCell(0, "Number").Text, "line 1 shopping cart order source filtered");
            VerifyAreEqual("54", Driver.GetGridCell(9, "Number").Text, "line 10 shopping cart order source filtered");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count all filtered after deleting");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnSources");
            VerifyAreEqual("Найдено записей: 55",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 55",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }

        [Test]
        public void FilterDate()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "OrderDateFormatted");

            //check filter min not exist
            Driver.SetGridFilterRange("OrderDateFormatted", "31.12.2050 00:00", "");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "filter min not exist");

            //check max not exist
            Driver.SetGridFilterRange("OrderDateFormatted", "", "31.12.2050 00:00"); 
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("OrderDateFormatted", "31.12.2050 00:00", "31.12.2050 00:00"); 
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "filter sum min/max not exist");

            //check filter
            Driver.SetGridFilterRange("OrderDateFormatted", "04.08.2016 00:00", "14.08.2016 23:00");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filtered items count");

            VerifyAreEqual("11", Driver.GetGridCell(0, "Number").Text, "filter order number line 1");
            VerifyAreEqual("14.08.2016 00:00", Driver.GetGridCell(0, "OrderDateFormatted").Text,
                "filter order date line 1");
            VerifyAreEqual("2", Driver.GetGridCell(9, "Number").Text, "filter order number line 10");
            VerifyAreEqual("05.08.2016 00:00", Driver.GetGridCell(9, "OrderDateFormatted").Text,
                "filter order date line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "OrderDateFormatted");
            VerifyAreEqual("Найдено записей: 88",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all");

            GoToAdmin("orders");
            VerifyAreEqual("Найдено записей: 88",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "deleting filtered items count all after refreshing");
        }
    }
}