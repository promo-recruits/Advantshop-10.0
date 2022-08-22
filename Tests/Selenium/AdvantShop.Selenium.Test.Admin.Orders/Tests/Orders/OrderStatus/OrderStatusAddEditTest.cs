using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderStatus
{
    [TestFixture]
    public class OrderStatusAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Orders\\OrderStatusAddEdit\\Catalog.Product.csv",
                "data\\Admin\\Orders\\OrderStatusAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\Orders\\OrderStatusAddEdit\\Catalog.Category.csv",
                "data\\Admin\\Orders\\OrderStatusAddEdit\\Catalog.ProductCategories.csv",
                "data\\Admin\\Orders\\OrderStatusAddEdit\\[Order].OrderContact.csv",
                "data\\Admin\\Orders\\OrderStatusAddEdit\\[Order].OrderSource.csv",
                "data\\Admin\\Orders\\OrderStatusAddEdit\\[Order].OrderStatus.csv",
                "data\\Admin\\Orders\\OrderStatusAddEdit\\[Order].[Order].csv",
                "data\\Admin\\Orders\\OrderStatusAddEdit\\[Order].OrderCurrency.csv",
                "data\\Admin\\Orders\\OrderStatusAddEdit\\[Order].OrderItems.csv"
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
        public void OrderStatusAddDefault()
        {
            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Новый статус заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusColor\"] input")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"orderStatusColor\"] input"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusColor\"] input")).SendKeys("#80d5fa");

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusName\"]")).SendKeys("New Order Status Test");

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsDefault\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsCompleted\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusSortOrder\"]")).SendKeys("1");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusCommand\"]")))).SelectByText(
                "Списание товара со склада");

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusSave\"]")).Click();
            Thread.Sleep(2000);

            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");

            VerifyAreEqual("Найдено записей: 126",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "grid count all");

            Driver.GridFilterSendKeys("New Order Status Test");

            VerifyAreEqual("color: rgb(128, 213, 250);",
                Driver.GetGridCell(0, "Color", "OrderStatuses").FindElement(By.TagName("i")).GetAttribute("style"),
                "grid color");
            VerifyAreEqual("New Order Status Test",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text, "grid name");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsDefault", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected, "grid default");
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsCanceled", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected, "grid IsCanceled");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsCompleted", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected, "grid IsCompleted");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "OrderStatuses").Text, "grid sort order");
            VerifyAreEqual("Списание товара со склада", Driver.GetGridCell(0, "CommandFormatted", "OrderStatuses").Text,
                "grid status command");

            Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("New Order Status Test",
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusName\"]")).GetAttribute("value"),
                "pop up name");
            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusSortOrder\"]")).GetAttribute("value"),
                "pop up sort order");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsDefault\"]"))
                    .FindElement(By.TagName("input")).Selected, "pop up default");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsCompleted\"]"))
                    .FindElement(By.TagName("input")).Selected, "pop up completed");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsCanceled\"]"))
                    .FindElement(By.TagName("input")).Selected, "pop up canceled");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusHidden\"]")).FindElement(By.TagName("input"))
                    .Selected, "pop up hidden");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusCommand\"]"));
            SelectElement select1 = new SelectElement(selectElem1);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Списание товара со склада"), "pop up command");

            //check prev default status not default
            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");

            Driver.GridFilterSendKeys("Order Status5");

            VerifyAreEqual("Order Status5",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "grid prev default name");
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsDefault", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "grid prev default not checked");

            //check new status added to orders
            GoToAdmin("orders/edit/1");

            IWebElement selectElem2 = Driver.FindElement(By.Id("Order_OrderStatusId"));
            SelectElement select2 = new SelectElement(selectElem2);
            VerifyIsTrue(select2.Options.Count == 126, "new status added to orders count");
        }


        [Test]
        public void OrderStatusEdit()
        {
            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");

            Driver.GridFilterSendKeys("Order Status107");

            VerifyAreEqual("Order Status107",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "pre check grid name");
            VerifyAreEqual("color: rgb(255, 255, 224);",
                Driver.GetGridCell(0, "Color", "OrderStatuses").FindElement(By.TagName("i")).GetAttribute("style"),
                "pre check grid color");
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsDefault", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "pre check grid default");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsCanceled", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "pre check grid IsCanceled");
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsCompleted", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "pre check grid IsCompleted");
            VerifyAreEqual("107", Driver.GetGridCell(0, "SortOrder", "OrderStatuses").Text, "grid sort order");
            VerifyAreEqual("Нет команды", Driver.GetGridCell(0, "CommandFormatted", "OrderStatuses").Text,
                "pre check grid status command");

            Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Order Status107",
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusName\"]")).GetAttribute("value"),
                "pre check pop up name");
            VerifyAreEqual("107",
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusSortOrder\"]")).GetAttribute("value"),
                "pre check pop up sort order");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsDefault\"]"))
                    .FindElement(By.TagName("input")).Selected, "pre check pop up default");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsCompleted\"]"))
                    .FindElement(By.TagName("input")).Selected, "pre check pop up completed");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsCanceled\"]"))
                    .FindElement(By.TagName("input")).Selected, "pre check pop up canceled");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusHidden\"]")).FindElement(By.TagName("input"))
                    .Selected, "pre check pop up hidden");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusCommand\"]"));
            SelectElement select1 = new SelectElement(selectElem1);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Нет команды"), "pre check pop up command");

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusColor\"] input")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"orderStatusColor\"] input"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusColor\"] input")).SendKeys("#ff00ff");

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusName\"]")).SendKeys("Edited Status Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsCanceled\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsCompleted\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusHidden\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusSortOrder\"]")).SendKeys("2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusColor\"]")).FindElement(By.TagName("input"))
                .Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusCommand\"]")))).SelectByText(
                "Возврат товара на склад");

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusSave\"]")).Click();
            Thread.Sleep(2000);

            //check edited status
            GoToAdmin("settingscheckout#?checkoutTab=orderStatuses");

            Driver.GridFilterSendKeys("Order Status107");

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "prev name");

            Driver.GridFilterSendKeys("Edited Status Name");

            VerifyAreEqual("color: rgb(255, 0, 255);",
                Driver.GetGridCell(0, "Color", "OrderStatuses").FindElement(By.TagName("i")).GetAttribute("style"),
                "grid color");
            VerifyAreEqual("Edited Status Name",
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
            VerifyAreEqual("2", Driver.GetGridCell(0, "SortOrder", "OrderStatuses").Text, "grid sort order");
            VerifyAreEqual("Возврат товара на склад", Driver.GetGridCell(0, "CommandFormatted", "OrderStatuses").Text,
                "grid status command");

            Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Edited Status Name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusName\"]")).GetAttribute("value"),
                "pop up name");
            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusSortOrder\"]")).GetAttribute("value"),
                " pop up sort order");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsDefault\"]"))
                    .FindElement(By.TagName("input")).Selected, "pop up default");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsCompleted\"]"))
                    .FindElement(By.TagName("input")).Selected, "pop up completed");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusIsCanceled\"]"))
                    .FindElement(By.TagName("input")).Selected, "pop up canceled");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusHidden\"]")).FindElement(By.TagName("input"))
                    .Selected, "pop up hidden");

            IWebElement selectElem2 = Driver.FindElement(By.CssSelector("[data-e2e=\"orderStatusCommand\"]"));
            SelectElement select2 = new SelectElement(selectElem2);
            VerifyIsTrue(select2.SelectedOption.Text.Contains("Возврат товара на склад"), "pop up command");
        }
    }
}