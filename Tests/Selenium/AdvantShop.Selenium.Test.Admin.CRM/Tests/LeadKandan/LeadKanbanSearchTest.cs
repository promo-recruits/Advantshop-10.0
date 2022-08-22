using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadKandan
{
    [TestFixture]
    public class CRMLeadKanbanSearchTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead_Kanban\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].LeadItem.csv"
            );

            Init();
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
        public void LeadKanbanSearchExistById()
        {
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=1");

            Driver.GridFilterSendKeys("111", AdvBy.DataE2E("UseKanban"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 1,
                "kanban search exist lead count");
            VerifyIsTrue(Driver.PageSource.Contains("Desc111"), "kanban search exist lead card");
        }

        [Test]
        public void LeadKanbanSearchExistByContact()
        {
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=1");

            Driver.GridFilterSendKeys("Patronymic", AdvBy.DataE2E("UseKanban"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 17,
                "kanban search exist lead count");
            VerifyIsTrue(Driver.PageSource.Contains("FirstName"), "kanban search exist lead card");
        }

        [Test]
        public void LeadKanbanSearchNotExistId()
        {
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=1");

            Driver.GridFilterSendKeys("3333333333", AdvBy.DataE2E("UseKanban"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 0,
                "kanban search not exist lead count");
        }

        [Test]
        public void LeadKanbanSearchMuchSymbols()
        {
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=1");

            Driver.GridFilterSendKeys(
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", AdvBy.DataE2E("UseKanban"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 0,
                "kanban search too much symbols lead count");
        }

        [Test]
        public void LeadKanbanSearchInvalidSymbols()
        {
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=1");

            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..", AdvBy.DataE2E("UseKanban"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 0,
                "kanban search invalid symbols lead count");
        }
    }
}