using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderSources
{
    [TestFixture]
    public class OrderSourceSearchFilter : BaseSeleniumTest
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
        public void SearchCorrectSource()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("Source2");
            Driver.DropFocusCss("[data-e2e=\"OrderSourceTitle\"]");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCell\"][data-e2e-col-index=\"0\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Source2", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source28", Driver.GetGridCell(9, "Name", "OrderSources").Text);
        }

        [Test]
        public void SearchCorrectOneSource()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("Source100");
            Driver.DropFocusCss("[data-e2e=\"OrderSourceTitle\"]");
            VerifyAreEqual("Source100", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1);
        }

        [Test]
        public void SearchCorrectNoSource()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("Source1000");
            Driver.DropFocusCss("[data-e2e=\"OrderSourceTitle\"]");
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 0);
        }

        [Test]
        public void SearchIncorrectSource()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys("123123@#$$^%&%&^&$%");
            Driver.DropFocusCss("[data-e2e=\"OrderSourceTitle\"]");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count > 0);
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text);
        }

        [Test]
        public void SearchLongSource()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterSearch\"]")).SendKeys(
                "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111");

            Driver.DropFocusCss("[data-e2e=\"OrderSourceTitle\"]");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 0);
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text);
        }
    }
}