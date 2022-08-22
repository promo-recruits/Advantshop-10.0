using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderStatus
{
    [TestFixture]
    public class OrderStatusPresentTest : BaseSeleniumTest
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
        public void Present()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "present line 1");
            VerifyAreEqual("Order Status10",
                Driver.GetGridCell(9, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "present line 10");
       
            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("Order Status1",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "present line 1");
            VerifyAreEqual("Order Status20",
                Driver.GetGridCell(19, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "present line 20");
        }
    }
}