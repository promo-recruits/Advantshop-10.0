using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderStatus
{
    [TestFixture]
    public class OrderStatusSortTest : BaseSeleniumTest
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
        public void SortByName()
        {
            Driver.GetGridCell(-1, "StatusName", "OrderStatuses").Click();
            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "sort name 1 asc");
            VerifyAreEqual("Order Status107",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "sort name 10 asc");

            Driver.GetGridCell(-1, "StatusName", "OrderStatuses").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Order Status99",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "sort name 1 desc");
            VerifyAreEqual("Order Status90",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "sort name 10 desc");
        }


        [Test]
        public void SortByDefault()
        {
            Driver.GetGridCell(-1, "IsDefault", "OrderStatuses").Click();
            Driver.WaitForAjax();
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsDefault", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected, "sort IsDefault 1 asc");
            VerifyIsFalse(
                Driver.GetGridCell(9, "IsDefault", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected, "sort IsDefault 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;
            string ascLine10 = Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different statuses");

            Driver.GetGridCell(-1, "IsDefault", "OrderStatuses").Click();
            Driver.WaitForAjax();
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsDefault", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected, "sort IsDefault 1 desc");
            VerifyIsFalse(
                Driver.GetGridCell(9, "IsDefault", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort IsDefault 10 desc");

            string descLine1 = Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;
            string descLine10 = Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different statuses");
        }


        [Test]
        public void SortByIsCanceled()
        {
            Driver.GetGridCell(-1, "IsCanceled", "OrderStatuses").Click();
            Driver.WaitForAjax();
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsCanceled", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected, "sort IsCanceled 1 asc");
            VerifyIsFalse(
                Driver.GetGridCell(9, "IsCanceled", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort IsCanceled 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;
            string ascLine10 = Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different statuses");

            Driver.GetGridCell(-1, "IsCanceled", "OrderStatuses").Click();
            Driver.WaitForAjax();
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsCanceled", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort IsCanceled 1 desc");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsCanceled", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort IsCanceled 10 desc");

            string descLine1 = Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;
            string descLine10 = Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different statuses");
        }


        [Test]
        public void SortByIsCompleted()
        {
            Driver.GetGridCell(-1, "IsCompleted", "OrderStatuses").Click();
            Driver.WaitForAjax();
            VerifyIsFalse(
                Driver.GetGridCell(0, "IsCompleted", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort IsCompleted 1 asc");
            VerifyIsFalse(
                Driver.GetGridCell(9, "IsCompleted", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort IsCompleted 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;
            string ascLine10 = Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different statuses");

            Driver.GetGridCell(-1, "IsCompleted", "OrderStatuses").Click();
            Driver.WaitForAjax();
            VerifyIsTrue(
                Driver.GetGridCell(0, "IsCompleted", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort IsCompleted 1 desc");
            VerifyIsTrue(
                Driver.GetGridCell(9, "IsCompleted", "OrderStatuses")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffSelect\"]")).Selected,
                "sort IsCompleted 10 desc");

            string descLine1 = Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;
            string descLine10 = Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different statuses");
        }

        [Test]
        public void SortBySortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder", "OrderStatuses").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "OrderStatuses").Text, "sort SortOrder 1 asc");
            VerifyAreEqual("10", Driver.GetGridCell(9, "SortOrder", "OrderStatuses").Text, "sort SortOrder 10 asc");

            Driver.GetGridCell(-1, "SortOrder", "OrderStatuses").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("125", Driver.GetGridCell(0, "SortOrder", "OrderStatuses").Text, "sort SortOrder 1 desc");
            VerifyAreEqual("116", Driver.GetGridCell(9, "SortOrder", "OrderStatuses").Text, "sort SortOrder 10 desc");
        }


        [Test]
        public void SortByCommand()
        {
            Driver.GetGridCell(-1, "CommandFormatted", "OrderStatuses").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Нет команды", Driver.GetGridCell(0, "CommandFormatted", "OrderStatuses").Text,
                "sort Command 1 asc");
            VerifyAreEqual("Нет команды", Driver.GetGridCell(9, "CommandFormatted", "OrderStatuses").Text,
                "sort Command 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;
            string ascLine10 = Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different statuses");

            Driver.GetGridCell(-1, "CommandFormatted", "OrderStatuses").Click();
            Driver.WaitForAjax();
            VerifyAreEqual("Списание товара со склада", Driver.GetGridCell(0, "CommandFormatted", "OrderStatuses").Text,
                "sort Command 1 desc");
            VerifyAreEqual("Списание товара со склада", Driver.GetGridCell(0, "CommandFormatted", "OrderStatuses").Text,
                "sort Command 10 desc");

            string descLine1 = Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;
            string descLine10 = Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different statuses");
        }
    }
}