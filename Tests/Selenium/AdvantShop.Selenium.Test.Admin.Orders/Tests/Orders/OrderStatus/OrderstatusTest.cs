using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderStatus
{
    [TestFixture]
    public class OrderStatusTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\Orders\\OrderStatus\\[Order].OrderStatus.csv"
            );

            Init();
            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");
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
        public void Grid()
        {
            VerifyAreEqual("Найдено записей: 125",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "grid count all");
            VerifyAreEqual("color: rgb(240, 128, 128);",
                Driver.GetGridCell(0, "Color", "OrderStatuses").FindElement(By.TagName("i")).GetAttribute("style"),
                "grid color");
            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text, "grid name");
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsDefault", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected, "grid default");
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsCanceled", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected, "grid IsCanceled");
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsCompleted", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected, "grid IsCompleted");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "OrderStatuses").Text, "grid sort order");
            VerifyAreEqual("Нет команды", Driver.GetGridCell(0, "CommandFormatted", "OrderStatuses").Text,
                "grid status command");
        }


        [Test]
        public void GoToEditByName()
        {
            Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Редактирование статуса заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.XPathContainsText("button", "Отмена");
        }


        [Test]
        public void GoToEditByServiceCol()
        {
            Driver.GetGridCell(0, "_serviceColumn", "OrderStatuses")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Редактирование статуса заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.XPathContainsText("button", "Отмена");
        }

        [Test]
        public void OrderStatisezSelectDelete()
        {
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "OrderStatuses")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "1 grid canсel delete");
            VerifyAreEqual("Найдено записей: 125",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "OrderStatuses")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderStatuses")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OrderStatuses")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "OrderStatuses")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Order Status5",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "selected 2 grid delete");
            VerifyAreEqual("Order Status6",
                Driver.GetGridCell(1, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "selected 3 grid delete");
            VerifyAreEqual("Order Status7",
                Driver.GetGridCell(2, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Order Status5",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "selected all on page 2 grid delete"); //default status
            VerifyAreEqual("Order Status23",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("112", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Order Status5",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "1 delete all except default");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "1 count all after deleting");

            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");
            VerifyAreEqual("Order Status5",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "2 delete all except default");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "2 count all after deleting");
        }
    }
}