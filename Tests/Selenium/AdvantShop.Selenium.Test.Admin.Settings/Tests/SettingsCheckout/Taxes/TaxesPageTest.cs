using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Taxes
{
    [TestFixture]
    public class SettingsTaxesPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Taxes);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Taxes\\Catalog.Tax.csv",
                "data\\Admin\\Settings\\Taxes\\Settings.Settings.csv"
            );

            Init();

            GoToAdmin("settingscheckout#?checkoutTab=taxes");
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
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 1 line 1");
            VerifyAreEqual("Tax 107", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Tax 11", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 2 line 1");
            VerifyAreEqual("Tax 2", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Tax 20", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 3 line 1");
            VerifyAreEqual("Tax 29", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Tax 3", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 4 line 1");
            VerifyAreEqual("Tax 38", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Tax 39", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 5 line 1");
            VerifyAreEqual("Tax 47", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Tax 48", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 6 line 1");
            VerifyAreEqual("Tax 56", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 1 line 1");
            VerifyAreEqual("Tax 107", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 1 line 10");
        }

        [Test]
        public void PageToPrevious()
        {
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 1 line 1");
            VerifyAreEqual("Tax 107", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Tax 11", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 2 line 1");
            VerifyAreEqual("Tax 2", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Tax 20", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 3 line 1");
            VerifyAreEqual("Tax 29", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Tax 11", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 2 line 1");
            VerifyAreEqual("Tax 2", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Tax 1", Driver.GetGridCell(0, "Name", "Taxes").Text, "page 1 line 1");
            VerifyAreEqual("Tax 107", Driver.GetGridCell(9, "Name", "Taxes").Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]"))
                .FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Tax 93", Driver.GetGridCell(0, "Name", "Taxes").Text, "last page line 1");
            VerifyAreEqual("Tax 99", Driver.GetGridCell(6, "Name", "Taxes").Text, "last page line 7");
        }
    }
}