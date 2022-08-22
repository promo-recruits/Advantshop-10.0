using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderStatus
{
    [TestFixture]
    public class OrderStatusFilterTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\Orders\\OrderStatus\\[Order].OrderStatus.csv"
            );

            Init();

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void FilterName()
        {
            //check filter Contact
            Functions.GridFilterSet(Driver, BaseUrl, name: "StatusName");

            //search by not exist Contact
            Driver.SetGridFilterValue("StatusName", "not existing Order Status 3");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("StatusName", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("StatusName", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "invalid symbols");

            //search by exist contact
            Driver.SetGridFilterValue("StatusName", "Order Status1");
            Driver.Blur();

            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "lead id fullname line 1");
            VerifyAreEqual("Order Status18",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "lead id fullname line 10");
            VerifyAreEqual("Найдено записей: 37",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter StatusName");

            //check go to edit and back
            Driver.GetGridCell(0, "_serviceColumn", "OrderStatuses")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            VerifyAreEqual("Редактирование статуса заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 37",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter StatusName return");
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"StatusName\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "StatusName");
            VerifyAreEqual("Найдено записей: 88",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter StatusName deleting 1");

            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");
            VerifyAreEqual("Найдено записей: 88",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter StatusName deleting 2");
        }

        [Test]
        public void FilterIsDefault()
        {
            //check filter is default yes
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "IsDefault", filterItem: "Да");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count filter is default yes");
            VerifyAreEqual("Order Status5",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name filter is default yes");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsDefault", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "default filter is default yes");

            //check filter is default no
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();
            VerifyAreEqual("Найдено записей: 124",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count filter is default no");

            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 1 filter is default no");
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsDefault", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "default line 1 filter is default no");
            VerifyAreEqual("Order Status11",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 10 filter is default no");
            VerifyIsFalse(
                Driver.GetGridCell(9, "IsDefault", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "default line 10 filter is default no");

            //check go to edit and back
            Driver.GetGridCell(0, "_serviceColumn", "OrderStatuses")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            VerifyAreEqual("Редактирование статуса заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 124",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter is default return");
            VerifyIsTrue(
                Driver.FindElement(
                        By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"IsDefault\"]"))
                    .Displayed, "filter is default Displayed");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "IsDefault");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter is default after deleting 1");

            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter is default after deleting 2");
            VerifyAreEqual("Order Status5",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name filter is default after deleting 2");
        }

        [Test]
        public void FilterIsCanceled()
        {
            //check filter IsCanceled yes
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "IsCanceled", filterItem: "Да");
            VerifyAreEqual("Найдено записей: 21",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count filter is Canceled yes");
            VerifyAreEqual("Order Status105",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 1 filter is Canceled yes");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsCanceled", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "Canceled line 1 filter is Canceled yes");
            VerifyAreEqual("Order Status114",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 10 filter is Canceled yes");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsCanceled", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "Canceled line 10 filter is Canceled yes");

            //check filter IsCanceled no
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();
            VerifyAreEqual("Найдено записей: 104",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count filter is Canceled no");

            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 1 filter is Canceled no");
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsCanceled", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "Canceled line 1 filter is Canceled no");
            VerifyAreEqual("Order Status10",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 10 filter is Canceled no");
            VerifyIsFalse(
                Driver.GetGridCell(9, "IsCanceled", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "Canceled line 10 filter is Canceled no");

            //check go to edit and back
            Driver.GetGridCell(0, "_serviceColumn", "OrderStatuses")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            VerifyAreEqual("Редактирование статуса заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 104",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter is Canceled return");
            VerifyIsTrue(
                Driver.FindElement(
                        By.CssSelector(
                            "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"IsCanceled\"]"))
                    .Displayed, "filter is Canceled Displayed");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "delete filtered items except default");
            VerifyAreEqual("Order Status5",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name delete filtered items except default");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "IsCanceled");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter is Canceled after deleting 1 except default");

            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter is Canceled after deleting 2 except default");
        }

        [Test]
        public void FilterIsCompleted()
        {
            //check filter Completed yes
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "IsCompleted", filterItem: "Да");
            VerifyAreEqual("Найдено записей: 20",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count filter is Completed yes");
            VerifyAreEqual("Order Status4",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 1 filter is Completed yes");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsCompleted", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "Completed line 1 filter is Completed yes");
            VerifyAreEqual("Order Status115",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 10 filter is Completed yes");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsCompleted", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "Completed line 10 filter is Completed yes");

            //check filter Completed no
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count filter is Completed no");

            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 1 filter is Completed no");
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsCompleted", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "Completed line 1 filter is Completed no");
            VerifyAreEqual("Order Status11",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 10 filter is Completed no");
            VerifyIsFalse(
                Driver.GetGridCell(9, "IsCompleted", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "Completed line 10 filter is Completed no");

            //check go to edit and back
            Driver.GetGridCell(0, "_serviceColumn", "OrderStatuses")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            VerifyAreEqual("Редактирование статуса заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter is Completed return");
            VerifyIsTrue(
                Driver.FindElement(
                        By.CssSelector(
                            "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"IsCompleted\"]"))
                    .Displayed, "filter is Completed Displayed");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "delete filtered items except default");
            VerifyAreEqual("Order Status5",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name delete filtered items except default");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "IsCompleted");
            VerifyAreEqual("Найдено записей: 21",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter is Completed after deleting 1 except default");

            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");
            VerifyAreEqual("Найдено записей: 21",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter is Completed after deleting 2 except default");
        }


        [Test]
        public void FilterCommand()
        {
            //check filter Command to stock
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CommandFormatted",
                filterItem: "Возврат товара на склад");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count filter Command to stock");
            VerifyAreEqual("Order Status15",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 1 filter Command to stock");
            VerifyAreEqual("Возврат товара на склад", Driver.GetGridCell(0, "CommandFormatted", "OrderStatuses").Text,
                "Command line 1 filter Command to stock");
            VerifyAreEqual("Order Status24",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 10 filter Command to stock");
            VerifyAreEqual("Возврат товара на склад", Driver.GetGridCell(9, "CommandFormatted", "OrderStatuses").Text,
                "Command line 10 filter Command to stock");

            //check filter Command from stock
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Списание товара со склада\"]")).Click();
            VerifyAreEqual("Найдено записей: 29",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "count filter Command from stock");
            VerifyAreEqual("Order Status34",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 1 filter Command from stock");
            VerifyAreEqual("Списание товара со склада", Driver.GetGridCell(0, "CommandFormatted", "OrderStatuses").Text,
                "Command line 1 filter Command from stock");
            VerifyAreEqual("Order Status43",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 10 filter Command from stock");
            VerifyAreEqual("Списание товара со склада", Driver.GetGridCell(9, "CommandFormatted", "OrderStatuses").Text,
                "Command line 10 filter Command from stock");

            //check filter Command no
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет команды\"]")).Click();
            VerifyAreEqual("Найдено записей: 74",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count filter Command no");
            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 1 filter Command no");
            VerifyAreEqual("Нет команды", Driver.GetGridCell(0, "CommandFormatted", "OrderStatuses").Text,
                "Completed line 1 filter Command no");
            VerifyAreEqual("Order Status10",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name line 10 filter Command no");
            VerifyAreEqual("Нет команды", Driver.GetGridCell(9, "CommandFormatted", "OrderStatuses").Text,
                "Completed line 10 filter Command no");

            //check go to edit and back
            Driver.GetGridCell(0, "_serviceColumn", "OrderStatuses")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            VerifyAreEqual("Редактирование статуса заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 74",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "filter Command return");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"CommandFormatted\"]")).Displayed,
                "filter Command Displayed");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "delete filtered items except default");
            VerifyAreEqual("Order Status5",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "name delete filtered items except default");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "CommandFormatted");
            VerifyAreEqual("Найдено записей: 52",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter Command after deleting 1 except default");

            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");
            VerifyAreEqual("Найдено записей: 52",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text,
                "filter Command after deleting 2 except default");
        }
    }
}