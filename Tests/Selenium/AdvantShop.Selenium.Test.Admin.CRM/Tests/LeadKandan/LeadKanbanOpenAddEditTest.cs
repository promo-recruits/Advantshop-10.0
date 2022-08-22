using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadKandan
{
    [TestFixture]
    public class CRMLeadKanbanOpenAddTest : BaseSeleniumTest
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
        public void LeadKanbanOpenLead()
        {
            Driver.GetKanbanCard(1, 0, "FullName").Click();
            Driver.WaitForElem(By.CssSelector("[data-e2e=\"leadInfoTitle\"]"));

            VerifyIsTrue(Driver.Url.Contains("leadIdInfo=40"), "lead pop up url");
            VerifyAreEqual("Lead Title 40", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead pop up title");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoClose\"]")).Click();
        }

        [Test]
        public void LeadKanbanAddLeadFromTopBtn()
        {
            int leadsCount = Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                .FindElements(By.CssSelector(".kanban-task")).Count;

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("New Lead From Top Button");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71231212923");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).SendKeys("New Lead Description");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            int leadsCountWithAdded = Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                .FindElements(By.CssSelector(".kanban-task")).Count;
            VerifyIsTrue(leadsCountWithAdded - leadsCount == 1, "new lead added to kanban");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("46"),
                "new lead added to kanban count");

            GoToAdmin("leads?salesFunnelId=1&usekanban=true");

            leadsCountWithAdded = Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                .FindElements(By.CssSelector(".kanban-task")).Count;
            VerifyIsTrue(leadsCountWithAdded - leadsCount == 1, "new lead added to kanban check after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("46"),
                "new lead added to kanban count check after refresh");
        }

        [Test]
        public void LeadzKanbanAddLeadFromBtnNoLeads()
        {
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=3");

            int leadsCount = Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                .FindElements(By.CssSelector(".kanban-task")).Count;

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]"))
                .SendKeys("New Lead From Button No Leads");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71231672923");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).SendKeys("New Lead Description 2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            GoToAdmin("leads?salesFunnelId=3");

            int leadsCountWithAdded = Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                .FindElements(By.CssSelector(".kanban-task")).Count;
            VerifyIsTrue(leadsCountWithAdded - leadsCount == 1, "new lead added to kanban");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("1"),
                "new lead added to kanban count");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElements(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Count > 0,
                "kanban with leads no add button");

            GoToAdmin("leads?salesFunnelId=3&usekanban=true");

            leadsCountWithAdded = Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                .FindElements(By.CssSelector(".kanban-task")).Count;
            VerifyIsTrue(leadsCountWithAdded - leadsCount == 1, "new lead added to kanban check after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("1"),
                "new lead added to kanban count check after refresh");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElements(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Count > 0,
                "kanban with leads no add button check after refresh");
        }
    }

    [TestFixture]
    public class CRMLeadKanbanEditTest : BaseSeleniumTest
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
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].LeadItem.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].LeadEvent.csv"
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
        public void LeadKanbanEditStatusDragDrop()
        {
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=2");

            //pre check
            VerifyIsTrue(Driver.GetKanbanCard(1, 0, "Description").Text.Contains("Desc118"),
                "pre check lead description");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Созвон с клиентом"),
                "pre check lead deal status column 2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("1"),
                "pre check header leads count column 2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count == 1, "pre check kanban cards in column 2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text
                    .Contains("Ожидание решения клиента"), "pre check lead deal status column 4");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("1"),
                "pre check header leads count column 4");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count == 1, "pre check kanban cards in column 4");

            Functions.DragDropElement(Driver, Driver.GetKanbanCard(1, 0, "FullName"),
                Driver.GetKanbanCard(3, 0, "FullName"));

            //check
            //VerifyIsTrue(Driver.GetKanbanCard(3, 0, "Description").Text.Contains("Desc118"), "lead description in 4 column edited");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]")).Text.Contains("Desc118"),
                "lead description in 4 column edited");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Созвон с клиентом"),
                "lead deal status column 2 edited");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("0"),
                "header leads count column 2 edited");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count == 0, "kanban cards in column 2 edited");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text
                    .Contains("Ожидание решения клиента"), "lead deal status column 4 edited");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("2"),
                "header leads count column 4 edited");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count == 2, "kanban cards in column 4 edited");

            //check after refreshing page
            GoToAdmin("leads?salesFunnelId=2&usekanban=true");
            // VerifyIsTrue(Driver.GetKanbanCard(3, 0, "Description").Text.Contains("Desc118"), "lead description edited after refresh");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]")).Text.Contains("Desc118"),
                "lead description edited after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Созвон с клиентом"),
                "lead deal status column 2 edited after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("0"),
                "header leads count column 2 edited after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count == 0,
                "kanban cards in column 2 edited after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text
                    .Contains("Ожидание решения клиента"), "lead deal status column 4 edited after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"CardsPerColumn\"]")).Text.Contains("2"),
                "header leads count column 4 edited after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"3\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count == 2,
                "kanban cards in column 4 edited after refresh");
        }

        [Test]
        public void LeadKanbanCompleteSuccessDragDrop()
        {
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=4");

            //pre check
            int cardsCount = Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                .FindElements(By.CssSelector(".kanban-task")).Count;
            VerifyIsTrue(Driver.GetKanbanCard(0, 0, "Description").Text.Contains("Desc121"),
                "pre check lead description");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Новый"),
                "pre check lead deal status column 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Завершить лид"),
                "pre check system lead deal status system column");

            Functions.DragDropElement(Driver, Driver.GetKanbanCard(0, 0, "FullName"),
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]")));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("h2")).Text
                    .Contains("Завершение лида №121"), "complete lead pop up");
            Driver.FindElement(By.CssSelector("[data-e2e=\"completeLead\"]")).Click();

            //check order from lead
            Driver.WaitForElem(By.Id("Order_OrderSourceId"));
            VerifyIsTrue(Driver.Url.Contains("orders/edit"), "order opened");
            VerifyIsFalse(Is404Page(Driver.Url), "order not 404 page");

            //check leads kanban
            GoToAdmin("leads?salesFunnelId=4&usekanban=true");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Новый"),
                "lead deal status column 1 edited after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Завершить лид"),
                "lead system deal status system column after refresh");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count > 0, "no completed lead after refresh");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count > 0, "no leads in system system status");

            //check not kanban
            Functions.KanbanOff(Driver, BaseUrl, "leads?salesFunnelId=4");
            Driver.FindElement(By.CssSelector(".tasks-navbar"))
                .FindElement(By.CssSelector("[data-e2e=\"Сделка заключена\"]")).Click();
            VerifyAreEqual("121", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "lead in grid saved");
        }


        [Test]
        public void LeadKanbanCompleteFailDragDrop()
        {
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=5");

            //pre check
            int cardsCount = Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                .FindElements(By.CssSelector(".kanban-task")).Count;
            VerifyIsTrue(Driver.GetKanbanCard(0, 0, "Description").Text.Contains("Desc122"),
                "pre check lead description");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Новый"),
                "pre check lead deal status column 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Завершить лид"),
                "pre check system lead deal status system column");

            Functions.DragDropElement(Driver, Driver.GetKanbanCard(0, 0, "FullName"),
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]")));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("h2")).Text
                    .Contains("Завершение лида №122"), "complete lead pop up");
            Driver.FindElement(By.CssSelector("[data-e2e=\"cancelLead\"]")).Click();
            Driver.WaitForToastSuccess();

            //check order from lead
            GoToAdmin("orders");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no orders created");

            //check leads kanban
            GoToAdmin("leads?salesFunnelId=5&usekanban=true");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Новый"),
                "lead deal status column 1 edited after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"columnHeader\"]")).Text.Contains("Завершить лид"),
                "lead system deal status system column after refresh");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"0\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count > 0, "no completed lead after refresh");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-columnindex=\"1\"]"))
                    .FindElements(By.CssSelector(".kanban-task")).Count > 0, "no leads in system system status");

            //check not kanban
            Functions.KanbanOff(Driver, BaseUrl, "leads?salesFunnelId=5");
            Driver.FindElement(By.CssSelector(".tasks-navbar"))
                .FindElement(By.CssSelector("[data-e2e=\"Сделка отклонена\"]")).Click();
            VerifyAreEqual("122", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "lead in grid saved");
        }
    }
}