using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderStatus
{
    [TestFixture]
    public class OrderStatusPageTest : BaseSeleniumTest
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
        public void Page()
        {
            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 1 line 1");
            VerifyAreEqual("Order Status10",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Order Status11",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 2 line 1");
            VerifyAreEqual("Order Status20",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Order Status21",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 3 line 1");
            VerifyAreEqual("Order Status30",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("Order Status31",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 4 line 1");
            VerifyAreEqual("Order Status40",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Order Status41",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 5 line 1");
            VerifyAreEqual("Order Status50",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Order Status51",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 6 line 1");
            VerifyAreEqual("Order Status60",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 1 line 1");
            VerifyAreEqual("Order Status10",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 1 line 10");
        }

        [Test]
        public void PageToPrevious()
        {
            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 1 line 1");
            VerifyAreEqual("Order Status10",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Order Status11",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 2 line 1");
            VerifyAreEqual("Order Status20",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Order Status21",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 3 line 1");
            VerifyAreEqual("Order Status30",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Order Status11",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 2 line 1");
            VerifyAreEqual("Order Status20",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 1 line 1");
            VerifyAreEqual("Order Status10",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Order Status121",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "last page line 1");
            VerifyAreEqual("Order Status125",
                Driver.GetGridCell(4, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "last page line 10");
        }
    }
}