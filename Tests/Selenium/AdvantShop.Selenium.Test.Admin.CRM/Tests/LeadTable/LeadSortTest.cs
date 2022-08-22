using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadSortTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\SalesFunnel\\Catalog.Product.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Catalog.Category.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.Customer.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.Departments.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.Managers.csv",
                "data\\Admin\\CRM\\SalesFunnel\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\SalesFunnel\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\SalesFunnel\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\SalesFunnel\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\SalesFunnel\\[Order].Lead.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.Task.csv",
                "data\\Admin\\CRM\\SalesFunnel\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\SalesFunnel\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\SalesFunnel\\[Order].LeadItem.csv"
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
        public void LeadSortId()
        {
            Driver.GetGridCell(-1, "Id").Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "sort lead Id 1 asc");
            VerifyAreEqual("10", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "sort lead Id 10 asc");

            Driver.GetGridCell(-1, "Id").Click();
            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "sort lead Id 1 desc");
            VerifyAreEqual("111", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "sort lead Id 10 desc");
        }

        [Test]
        public void LeadSortDealStatusName()
        {
            Driver.GetGridCell(-1, "DealStatusName").Click();
            VerifyAreEqual("Funnel 1 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text,
                "sort DealStatusName 1 asc");
            VerifyAreEqual("Funnel 1 Deal Status 1", Driver.GetGridCell(9, "DealStatusName").Text,
                "sort DealStatusName 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text;
            string ascLine10 = Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different leads");

            Driver.GetGridCell(-1, "DealStatusName").Click();
            VerifyAreEqual("Funnel 2 Deal Status 3", Driver.GetGridCell(0, "DealStatusName").Text,
                "sort DealStatusName 1 desc");
            VerifyAreEqual("Funnel 1 Deal Status 6", Driver.GetGridCell(9, "DealStatusName").Text,
                "sort DealStatusName 10 desc");
        }

        [Test]
        public void LeadSortContact()
        {
            Driver.GetGridCell(-1, "FullName").Click();
            VerifyAreEqual("FirstName", Driver.GetGridCell(0, "FullName").Text, "sort FullName 1 desc");
            VerifyAreEqual("FirstName", Driver.GetGridCell(9, "FullName").Text, "sort FullName 10 asc");

            Driver.GetGridCell(-1, "FullName").Click();
            VerifyAreEqual("LastName9 FirstName9 Patron9", Driver.GetGridCell(0, "FullName").Text,
                "sort FullName 1 asc");
            VerifyAreEqual("LastName91 FirstName91 Patron91", Driver.GetGridCell(9, "FullName").Text,
                "sort FullName 10 asc");

            Driver.GetGridCell(-1, "Id").Click();
            VerifyAreEqual("LastName1 FirstName1 Patron1", Driver.GetGridCell(0, "FullName").Text,
                "sort FullName 1 asc");
            VerifyAreEqual("LastName10 FirstName10 Patron10", Driver.GetGridCell(9, "FullName").Text,
                "sort FullName 10 asc");

            string descLine1 = Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text;
            string descLine10 = Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different leads");
        }


        [Test]
        public void LeadSortManager()
        {
            Driver.GetGridCell(-1, "ManagerName").Click();
            VerifyAreEqual("", Driver.GetGridCell(0, "ManagerName").Text, "sort ManagerName 1 asc");
            VerifyAreEqual("", Driver.GetGridCell(9, "ManagerName").Text, "sort ManagerName 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text;
            string ascLine10 = Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different leads");

            Driver.GetGridCell(-1, "ManagerName").Click();
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text, "sort ManagerName 1 desc");
            VerifyAreEqual("test testov", Driver.GetGridCell(9, "ManagerName").Text, "sort ManagerName 10 asc");

            string descLine1 = Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text;
            string descLine10 = Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(descLine1.Equals(descLine10), "desc different leads");
        }

        [Test]
        public void LeadSortProductsCount()
        {
            Driver.GetGridCell(-1, "ProductsCount").Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductsCount").Text, "sort ProductsCount 1 asc");
            VerifyAreEqual("5", Driver.GetGridCell(9, "ProductsCount").Text, "sort ProductsCount 10 asc");

            Driver.GetGridCell(-1, "ProductsCount").Click();
            VerifyAreEqual("100", Driver.GetGridCell(0, "ProductsCount").Text, "sort ProductsCount 1 desc");
            VerifyAreEqual("91", Driver.GetGridCell(9, "ProductsCount").Text, "sort ProductsCount 10 desc");
        }

        [Test]
        public void LeadSortSum()
        {
            Driver.GetGridCell(-1, "SumFormatted").Click();
            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "sort Sum 1 asc");
            VerifyAreEqual("10 руб.", Driver.GetGridCell(9, "SumFormatted").Text, "sort Sum 10 asc");

            Driver.GetGridCell(-1, "SumFormatted").Click();
            VerifyAreEqual("120 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "sort Sum 1 desc");
            VerifyAreEqual("111 руб.", Driver.GetGridCell(9, "SumFormatted").Text, "sort Sum 10 desc");
        }

        [Test]
        public void LeadSortSalesFunnel()
        {
            Driver.GetGridCell(-1, "SalesFunnelName").Click();
            VerifyAreEqual("Sales Funnel 1", Driver.GetGridCell(0, "SalesFunnelName").Text,
                "sort SalesFunnelName 1 asc");
            VerifyAreEqual("Sales Funnel 1", Driver.GetGridCell(9, "SalesFunnelName").Text,
                "sort SalesFunnelName 10 asc");

            string ascLine1 = Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text;
            string ascLine10 = Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text;

            VerifyIsFalse(ascLine1.Equals(ascLine10), "asc different leads");

            Driver.GetGridCell(-1, "SalesFunnelName").Click();
            VerifyAreEqual("Sales Funnel 2", Driver.GetGridCell(0, "SalesFunnelName").Text,
                "sort SalesFunnelName 1 desc");
            VerifyAreEqual("Sales Funnel 2", Driver.GetGridCell(1, "SalesFunnelName").Text,
                "sort SalesFunnelName 2 desc");
            VerifyAreEqual("Sales Funnel 2", Driver.GetGridCell(2, "SalesFunnelName").Text,
                "sort SalesFunnelName 3 desc");
            VerifyAreEqual("Sales Funnel 1", Driver.GetGridCell(3, "SalesFunnelName").Text,
                "sort SalesFunnelName 4 desc");
        }
    }
}