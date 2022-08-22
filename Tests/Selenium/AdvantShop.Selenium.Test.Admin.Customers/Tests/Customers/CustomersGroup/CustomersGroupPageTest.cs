using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.CustomersGroup
{
    [TestFixture]
    public class CustomersGroupPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Customers\\CustomersGroup\\CustomersGroupPage\\Catalog.Product.csv",
                "data\\Admin\\Customers\\CustomersGroup\\CustomersGroupPage\\Catalog.Offer.csv",
                "data\\Admin\\Customers\\CustomersGroup\\CustomersGroupPage\\Catalog.Category.csv",
                "data\\Admin\\Customers\\CustomersGroup\\CustomersGroupPage\\Catalog.ProductCategories.csv",
                "data\\Admin\\Customers\\CustomersGroup\\CustomersGroupPage\\Customers.CustomerGroup.csv",
                "data\\Admin\\Customers\\CustomersGroup\\CustomersGroupPage\\Customers.Customer.csv",
                "data\\Admin\\Customers\\CustomersGroup\\CustomersGroupPage\\Customers.Departments.csv",
                "data\\Admin\\Customers\\CustomersGroup\\CustomersGroupPage\\Customers.Managers.csv",
                "data\\Admin\\Customers\\CustomersGroup\\CustomersGroupPage\\Customers.ManagerTask.csv"
            );

            Init();
            GoToAdmin("settingscustomers#?tab=customerGroups");
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
        public void CustomersGroupPage()
        {
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 1 line 1");
            VerifyAreEqual("CustomerGroup109", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 1 line 10");
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Driver.MouseFocus(By.CssSelector(".adv-panel-info"));
            VerifyAreEqual("CustomerGroup110", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 2 line 1");
            VerifyAreEqual("CustomerGroup119", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Driver.MouseFocus(By.CssSelector(".adv-panel-info"));
            VerifyAreEqual("CustomerGroup120", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 3 line 1");
            VerifyAreEqual("CustomerGroup199", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Driver.MouseFocus(By.CssSelector(".adv-panel-info"));
            VerifyAreEqual("CustomerGroup200", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 4 line 1");
            VerifyAreEqual("CustomerGroup209", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            VerifyAreEqual("CustomerGroup210", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 5 line 1");
            VerifyAreEqual("CustomerGroup219", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            VerifyAreEqual("CustomerGroup220", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 6 line 1");
            VerifyAreEqual("CustomerGroup229", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 1 line 1");
            VerifyAreEqual("CustomerGroup109", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 1 line 10");
        }

        [Test]
        public void CustomersGroupPageToPrevious()
        {
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 1 line 1");
            VerifyAreEqual("CustomerGroup109", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            VerifyAreEqual("CustomerGroup110", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 2 line 1");
            VerifyAreEqual("CustomerGroup119", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            VerifyAreEqual("CustomerGroup120", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 3 line 1");
            VerifyAreEqual("CustomerGroup199", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            VerifyAreEqual("CustomerGroup110", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 2 line 1");
            VerifyAreEqual("CustomerGroup119", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "page 1 line 1");
            VerifyAreEqual("CustomerGroup109", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            VerifyAreEqual("CustomerGroup181", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text,
                "last page line 1");
            VerifyAreEqual("CustomerGroup190", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text,
                "last page line 10");
        }
    }
}