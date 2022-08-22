using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderStatus
{
    [TestFixture]
    public class OrderStatusSearchTest : BaseSeleniumTest
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
        }

        [SetUp]
        public void SetUpTest()
        {
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
        public void SearchExist()
        {
            Driver.GridFilterSendKeys("Order Status114");

            VerifyAreEqual("Order Status114",
                Driver.GetGridCell(0, "StatusName", "OrderStatuses").FindElement(By.TagName("a")).Text,
                "search exist status");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchNotExist()
        {
            Driver.GridFilterSendKeys("Order Status5543");

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "search not exist status");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchMuchSymbols()
        {
            Driver.GridFilterSendKeys(
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", 
                By.ClassName("ui-grid-custom-filter-total"));

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchInvalidSymbols()
        {
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");

            VerifyAreEqual("Ни одной записи не найдено", Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text,
                "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}