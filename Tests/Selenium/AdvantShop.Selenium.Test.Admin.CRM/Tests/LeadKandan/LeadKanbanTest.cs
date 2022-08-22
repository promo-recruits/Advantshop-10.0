using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadKandan
{
    [TestFixture]
    public class CRMLeadKanbanTest : BaseSeleniumTest
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
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=1");
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
        public void LeadKanbanOn()
        {
            VerifyIsTrue(Driver.FindElement(By.Id("kanban")).Displayed, "lead kanban opened");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[grid-unique-id=\"grid\"]")).Count > 0,
                "lead grid not opened");
        }

        [Test]
        public void LeadKanbanNoSystemStatuses()
        {
            VerifyIsFalse(Driver.PageSource.Contains("Сделка заключена"), "no system status 1");
            VerifyIsFalse(Driver.PageSource.Contains("Сделка отклонена"), "no system status 2");
            VerifyIsTrue(Driver.PageSource.Contains("Завершить лид"), "system deal status finish lead");
        }

        [Test]
        public void LeadKanbanCart()
        {
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Новый"),
                "kanban lead status 1 header status name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("45"),
                "kanban lead status 1 header leads count");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnTotal\"]")).Text.Contains("1 461 руб."),
                "kanban lead status 1 header leads sum");
            VerifyAreEqual("rgba(103, 106, 108, 1)",
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]"))
                    .FindElement(By.ClassName("kanban-column-header__inner")).GetCssValue("color"),
                "kanban lead status 1 color");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count == 45, "kanban leads status 1 count");

            VerifyAreEqual("LastName1 FirstName1", Driver.GetKanbanCard(0, 0, "FullName").Text,
                "kanban lead status 1 customer name");
            VerifyAreEqual("Desc1", Driver.GetKanbanCard(0, 0, "Description").Text, "kanban lead status 1 description");
            VerifyAreEqual("1 руб.", Driver.GetKanbanCard(0, 0, "Sum").Text, "kanban lead status 1 sum");
            VerifyAreEqual("Корзина интернет магазина", Driver.GetKanbanCard(0, 0, "OrderSourceName").Text,
                "kanban lead status 1 order source");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Созвон с клиентом"),
                "kanban lead status 2 header status name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("21"),
                "kanban lead status 2 header leads count");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnTotal\"]")).Text.Contains("1 050 руб."),
                "kanban lead status 2 header leads sum");
            VerifyAreEqual("rgba(103, 106, 108, 1)",
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]"))
                    .FindElement(By.ClassName("kanban-column-header__inner")).GetCssValue("color"),
                "kanban lead status 1 color");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count == 21, "kanban leads status 2 count");

            VerifyAreEqual("LastName40 FirstName40", Driver.GetKanbanCard(1, 0, "FullName").Text,
                "kanban lead status 2 customer name");
            VerifyAreEqual("Desc40", Driver.GetKanbanCard(1, 0, "Description").Text,
                "kanban lead status 2 description");
            VerifyAreEqual("40 руб.", Driver.GetKanbanCard(1, 0, "Sum").Text, "kanban lead status 2 sum");
            VerifyAreEqual("Брошенные корзины", Driver.GetKanbanCard(1, 0, "OrderSourceName").Text,
                "kanban lead status 2 order source");
        }

        [Test]
        public void LeadKanbanSalesFunnel()
        {
            VerifyIsTrue(Driver.PageSource.Contains("Desc58"), "lead from funnel 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116, "leads count from funnel 1");

            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=2");
            VerifyIsTrue(Driver.PageSource.Contains("Desc117"), "lead from funnel 2");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 4, "leads count from funnel 2");
        }

        [Test]
        public void LeadKanbanNoLeadsInDealStatus()
        {
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text
                    .Contains("Ожидание решения клиента"), "kanban no lead status 1 header status name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("0"),
                "kanban no lead status 1 header leads count");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnTotal\"]")).Text.Contains("0 руб."),
                "kanban no lead status 1 header leads sum");
            VerifyAreEqual("rgba(103, 106, 108, 1)",
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]"))
                    .FindElement(By.ClassName("kanban-column-header__inner")).GetCssValue("color"),
                "kanban no lead status 1 color");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count == 0, "kanban no leads status 1 count");
        }

        [Test]
        public void LeadzKanbanNoLeadsInFunnel()
        {
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=3");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Новый"),
                "kanban no lead status 1 header status name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("0"),
                "kanban no lead status 1 header leads count");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnTotal\"]")).Text.Contains("0 руб."),
                "kanban no lead status 1 header leads sum");
            VerifyAreEqual("rgba(103, 106, 108, 1)",
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]"))
                    .FindElement(By.ClassName("kanban-column-header__inner")).GetCssValue("color"),
                "kanban no lead status 1 color");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count == 0, "kanban no leads status 1 count");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Созвон с клиентом"),
                "kanban no lead status 2 header status name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("0"),
                "kanban no lead status 2 header leads count");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnTotal\"]")).Text.Contains("0 руб."),
                "kanban no lead status 2 header leads sum");
            VerifyAreEqual("rgba(103, 106, 108, 1)",
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]"))
                    .FindElement(By.ClassName("kanban-column-header__inner")).GetCssValue("color"),
                "kanban no lead status 2 color");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count == 0, "kanban no leads status 2 count");
        }

        [Test]
        public void LeadsKanbanOff()
        {
            Functions.KanbanOff(Driver, BaseUrl, url: "leads?salesFunnelId=1");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]")).Displayed, "lead grid opened");
            VerifyIsFalse(Driver.FindElements(By.Id("kanban")).Count > 0, "lead kanban not opened");
        }
    }
}