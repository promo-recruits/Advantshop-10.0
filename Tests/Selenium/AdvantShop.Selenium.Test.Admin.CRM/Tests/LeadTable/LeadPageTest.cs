using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadItem.csv"
            );

            Init();
            GoToAdmin("leads?salesFunnelId=-1");
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
        public void LeadPage()
        {
            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 1 line 1");
            VerifyAreEqual("111", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("110", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 2 line 1");
            VerifyAreEqual("101", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("100", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 3 line 1");
            VerifyAreEqual("91", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("90", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 4 line 1");
            VerifyAreEqual("81", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("80", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 5 line 1");
            VerifyAreEqual("71", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("70", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 6 line 1");
            VerifyAreEqual("61", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 1 line 1");
            VerifyAreEqual("111", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 1 line 10");
        }

        [Test]
        public void LeadPageToPrevious()
        {
            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 1 line 1");
            VerifyAreEqual("111", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("110", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 2 line 1");
            VerifyAreEqual("101", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("100", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 3 line 1");
            VerifyAreEqual("91", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("110", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 2 line 1");
            VerifyAreEqual("101", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "page 1 line 1");
            VerifyAreEqual("111", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("10", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "last page line 1");
            VerifyAreEqual("1", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "last page line 10");
        }
    }
}