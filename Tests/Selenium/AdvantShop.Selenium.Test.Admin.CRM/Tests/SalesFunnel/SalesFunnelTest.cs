using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.SalesFunnel
{
    [TestFixture]
    public class CRMSalesFunnelTest : BaseSeleniumTest
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
                "data\\Admin\\CRM\\SalesFunnel\\Customers.Departments.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.CustomerFieldValuesMap.csv",
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
                "data\\Admin\\CRM\\SalesFunnel\\[Order].LeadItem.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Settings.Settings.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("settingscrm#?crmTab=salesfunnels");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void SalesFunnel1()
        {
            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            VerifyAreEqual("Sales Funnel 1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).GetAttribute("value"),
                "sales funnel 1 name");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).Enabled,
                "sales funnel 1 name enabled by dafault");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"1\"")).Text
                    .Contains("Funnel 1 Deal Status 1"), "sales funnel 1 deal status 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"2\"")).Text
                    .Contains("Funnel 1 Deal Status 2"), "sales funnel 1 deal status 2");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"16\"")).Text
                    .Contains("Сделка заключена"), "sales funnel 1 system deal status 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"17\"")).Text
                    .Contains("Сделка отклонена"), "sales funnel 1 system deal status 2");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-crm=\"DealStatus\"]")).Count == 8,
                "funnel 1 deal statuses count"); //6 funnel deal statuses + 2 system statuses
        }

        [Test]
        public void SalesFunnel2()
        {
            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Sales Funnel 2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).GetAttribute("value"),
                "sales funnel 2 name");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).Enabled,
                "sales funnel 2 name enabled");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"7\"")).Text
                    .Contains("Funnel 2 Deal Status 1"), "sales funnel 2 deal status 7");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"8\"")).Text
                    .Contains("Funnel 2 Deal Status 2"), "sales funnel 2 deal status 8");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"18\"")).Text
                    .Contains("Сделка заключена"), "sales funnel 2 system deal status 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"19\"")).Text
                    .Contains("Сделка отклонена"), "sales funnel 2 system deal status 2");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-crm=\"DealStatus\"]")).Count == 8,
                "funnel 2 deal statuses count"); //6 funnel deal statuses + 2 system statuses
        }

        [Test]
        public void SalesFunnel3()
        {
            Driver.GetGridCell(2, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Sales Funnel 3",
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).GetAttribute("value"),
                "sales funnel 3 name");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).Enabled,
                "sales funnel 3 name enabled");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"13\"")).Text
                    .Contains("Funnel 3 Deal Status 1"), "sales funnel 3 deal status 13");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"14\"")).Text
                    .Contains("Funnel 3 Deal Status 2"), "sales funnel 3 deal status 14");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"20\"")).Text
                    .Contains("Сделка заключена"), "sales funnel 2 system deal status 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"21\"")).Text
                    .Contains("Сделка отклонена"), "sales funnel 2 system deal status 2");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-crm=\"DealStatus\"]")).Count == 5,
                "funnel 2 deal statuses count"); //3 funnel deal statuses + 2 system statuses
        }

        [Test]
        public void SalesFunnel4()
        {
            Driver.GetGridCell(3, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Sales Funnel 4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).GetAttribute("value"),
                "sales funnel 4 name");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).Enabled,
                "sales funnel 4 name enabled");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"22\"")).Text
                    .Contains("Сделка заключена"), "sales funnel 4 system deal status 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"23\"")).Text
                    .Contains("Сделка отклонена"), "sales funnel 4 system deal status 2");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-crm=\"DealStatus\"]")).Count == 2,
                "funnel 2 deal statuses count"); //0 funnel deal statuses + 2 system statuses
        }
    }
}