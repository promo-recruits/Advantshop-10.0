using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderSources
{
    [TestFixture]
    public class OrderSourcePaginationAndView : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "Data\\Admin\\Orders\\OrderSources\\[Order].OrderSource.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void OpenOrders()
        {
            VerifyAreEqual("Источники", Driver.FindElement(By.CssSelector("[data-e2e=\"OrderSourceTitle\"]")).Text);
        }

        [Test]
        public void Present10Page()
        {
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source10", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Source11", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source20", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Source21", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source30", Driver.GetGridCell(9, "Name", "OrderSources").Text);
        }

        [Test]
        public void Present10PageToNext()
        {
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source10", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Source11", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source20", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Source21", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source30", Driver.GetGridCell(9, "Name", "OrderSources").Text);
        }

        [Test]
        public void Present10PageToPrevious()
        {
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source10", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Source11", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source20", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source10", Driver.GetGridCell(9, "Name", "OrderSources").Text);
        }

        [Test]
        public void Present10PageToEnd()
        {
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source10", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            //to end
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Source101", Driver.GetGridCell(0, "Name", "OrderSources").Text);
        }

        [Test]
        public void Present10PageToBegin()
        {
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source10", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Source11", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source20", Driver.GetGridCell(9, "Name", "OrderSources").Text);
            Thread.Sleep(2000);
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Source21", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source30", Driver.GetGridCell(9, "Name", "OrderSources").Text);

            //to begin
            Driver.ScrollTo(By.CssSelector(".version"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source10", Driver.GetGridCell(9, "Name", "OrderSources").Text);
        }
    }
}