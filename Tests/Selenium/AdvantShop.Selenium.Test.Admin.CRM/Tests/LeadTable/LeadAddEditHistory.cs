using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadAddEditHistoryTasksTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Catalog | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Contact.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadItem.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CMS.ChangeHistory.csv"
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
        [Order(0)]
        public void AddTask()
        {
            GoToAdmin("leads#?leadIdInfo=6");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTasks\"]")).Displayed,
                "lead no tasks at first");

            Driver.ScrollTo(By.Id("Lead_LastName"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"addTaskTab\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Новая задача",
                Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("h2")).Text,
                "add task pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).SendKeys("Lead Task Test");

            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskAssigned\"]")).Click();
            Driver.FindElement(By.XPath("//div[contains(text(), 'ManagerName2 ManagerLastName2')]")).Click();

            Driver.SetCkText("Lead Task Description Test", By.CssSelector("[name=formAddTask]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).SendKeys("25.02.2030 19:55");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]")))).SelectByText(
                "TaskGroup");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskPriority\"]"))))
                .SelectByText("Высокий");

            Driver.DropFocus("h2");
            Driver.XPathContainsText("span", "Добавить");
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.FindElement(By.TagName("tasks-grid")).Text.Contains("Найдено задач: 1"),
                "task added count");
            VerifyAreEqual("6", Driver.GetGridCell(0, "Id", "Tasks").Text, "task Id before refresh");

            //check lead history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]")).Text
                    .Contains("Создана задача №6 \"Lead Task Test\""), "task added saved");

            GoToAdmin("leads#?leadIdInfo=6");

            //check lead details
            VerifyAreEqual("6", Driver.GetGridCell(0, "Id", "Tasks").Text, "task Id");
            VerifyAreEqual("Lead Task Test", Driver.GetGridCell(0, "Name", "Tasks").Text, "task Name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "task Status");
            VerifyIsTrue(Driver.GetGridCell(0, "Managers", "Tasks").Text.Contains("ManagerName2 ManagerLastName2"),
                "task AssignedName");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "AppointedName", "Tasks").Text, "task AppointedName");

            VerifyIsTrue(Driver.FindElement(By.TagName("tasks-grid")).Text.Contains("Найдено задач: 1"),
                "task added count after refresh");

            //check lead history after refresh
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]")).Text
                    .Contains("Создана задача №6 \"Lead Task Test\""), "task added saved after refresh");

            //check tasks grid 
            GoToAdmin("tasks");

            VerifyIsTrue(Driver.PageSource.Contains("Lead Task Test"), "grid task");
        }


        [Test]
        [Order(1)]
        public void TaskComplete()
        {
            GoToAdmin("leads#?leadIdInfo=5");

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            SelectElement select = new SelectElement(selectElem);
            string dealStatusBegin = select.SelectedOption.Text;

            Driver.ScrollTo(By.XPath("//h1[contains(text(), 'Задачи')]"));
            VerifyAreEqual("Task5", Driver.GetGridCell(0, "Name", "Tasks").Text, "pre check task Name");

            Driver.GetGridCell(0, "Name", "Tasks").Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusCompleted\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"taskResult\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taskResult\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taskResult\"]")).SendKeys("test Result lead's task");

            Driver.XPathContainsText("span", "Завершить");
            Driver.WaitForToastSuccess();

            VerifyAreEqual("Завершена", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text,
                "lead task completed Status");

            //check lead history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]")).Text
                    .Contains("Задача \"Task5\" завершена. Результат выполнения задачи: test Result lead's task"),
                "lead task completed");

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElem);
            string dealStatusEnd = select.SelectedOption.Text;

            VerifyIsTrue(dealStatusBegin.Equals(dealStatusEnd), "lead deal status not changed");

            //check lead details after refresh
            GoToAdmin("leads#?leadIdInfo=5");

            VerifyAreEqual("Завершена", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text,
                "lead task completed Status after refresh");

            //check lead history after refresh
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]")).Text
                    .Contains("Задача \"Task5\" завершена. Результат выполнения задачи: test Result lead's task"),
                "lead task completed after refresh");

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElem);
            dealStatusEnd = select.SelectedOption.Text;

            VerifyIsTrue(dealStatusBegin.Equals(dealStatusEnd), "lead deal status not changed after refresh");

            Driver.ScrollTo(By.XPath("//h1[contains(text(), 'Задачи')]"));

            //check task details in lead
            Driver.GetGridCell(0, "_serviceColumn", "Tasks").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskRezult\"]")).Text
                    .Contains("test Result lead's task"), "Result lead's task saved");

            //check tasks grid 
            GoToAdmin("tasks?filterby=completed");
            VerifyAreEqual("Завершена", Driver.GetGridCell(0, "StatusFormatted").Text, "grid task completed Status");

            GoToAdmin("tasks");
            VerifyIsFalse(Driver.PageSource.Contains("Task5"), "grid task no");
        }

        [Test]
        [Order(1)]
        public void TaskCompleteChangeDealStatus()
        {
            GoToAdmin("leads#?leadIdInfo=4");

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Новый"), "lead deal status default");

            // Driver.ScrollTo(By.TagName("tasks-grid"));
            Driver.ScrollTo(By.XPath("//h1[contains(text(), 'Задачи')]"));

            Driver.GetGridCell(0, "Name", "Tasks").Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusCompleted\"]")).Click();

            Driver.FindElement(By.CssSelector(".modal-dialog")).FindElement(By.CssSelector(".ui-select-toggle"))
                .Click();
            Driver.XPathContainsText("span", "Ожидание решения клиента");

            Driver.XPathContainsText("span", "Завершить");

            VerifyAreEqual("Завершена", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text,
                "lead task completed Status");

            //check lead history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]")).Text
                    .Contains("Задача \"Task4\" завершена"), "lead task completed");

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ожидание решения клиента"), "lead deal status changed");

            //check lead details
            GoToAdmin("leads#?leadIdInfo=4");

            VerifyAreEqual("Завершена", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text,
                "lead task completed Status after refresh");

            //check lead history after refresh
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]")).Text
                    .Contains("Задача \"Task4\" завершена"), "lead task completed after refresh");

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ожидание решения клиента"),
                "lead deal status changed after refresh");
        }

        [Test]
        [Order(2)]
        public void TaskDelete()
        {
            GoToAdmin("leads#?leadIdInfo=3");

            Driver.ScrollTo(By.XPath("//h1[contains(text(), 'Задачи')]"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTasks\"]")).Displayed,
                "task enabled");

            Driver.GetGridCell(0, "_serviceColumn", "Tasks").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTasks\"]")).Displayed,
                "task task deleted");

            //check lead history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]")).Text
                    .Contains("Удалена задача №3 \"Task3\""), "lead task deleted history");

            //check lead history after refresh
            GoToAdmin("leads#?leadIdInfo=3");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTasks\"]")).Displayed,
                "task task deleted after refresh");
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock\"]")).Text
                    .Contains("Удалена задача №3 \"Task3\""), "lead task deleted history after refresh");
        }
    }

    [TestFixture]
    public class CRMLeadAddEditHistoryTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Catalog | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Contact.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadItem.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CMS.ChangeHistory.csv"
            );

            Init();
            GoToAdmin("leads#?leadIdInfo=80");
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
        public void LeadEditDealStatus()
        {
            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Выставление КП"), "pre check lead deal status");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]")))).SelectByText(
                "Ожидание решения клиента");
            Driver.WaitForToastSuccess();

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ожидание решения клиента"), "lead deal status edited");

            //check history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Этап сделки:"), "lead deal status edited history");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".crm-old-status")).Text.Contains("Выставление КП"),
                "lead deal status edited old history");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".crm-new-status")).Text.Contains("Ожидание решения клиента"),
                "lead deal status edited new history");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=80");
            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ожидание решения клиента"),
                "lead deal status edited after refresh");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Этап сделки:"), "lead deal status edited history after refresh");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("Выставление КП"),
                "lead deal status edited old history after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("Ожидание решения клиента"), "lead deal status edited new history after refresh");
        }

        [Test]
        public void LeadEditOrderSource()
        {
            IWebElement selectElem = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Корзина интернет магазина"),
                "pre check lead order source");

            (new SelectElement(Driver.FindElement(By.Id("Lead_OrderSourceId")))).SelectByText("Мобильная версия");
            Driver.WaitForToastSuccess();

            selectElem = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Мобильная версия"), "lead order source edited");

            //check history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Источник:"), "lead order source edited history");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".crm-old-status")).Text.Contains("Корзина интернет магазина"),
                "lead order source edited old history");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".crm-new-status")).Text.Contains("Мобильная версия"),
                "lead order source edited new history");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=80");
            selectElem = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Мобильная версия"),
                "lead order source edited after refresh");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Источник:"), "lead order source edited history after refresh");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("Корзина интернет магазина"),
                "lead order source edited old history after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("Мобильная версия"), "lead order source edited new history after refresh");
        }

        [Test]
        public void LeadEditDescription()
        {
            VerifyAreEqual("Desc80", Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value"),
                "pre check lead description");

            Driver.FindElement(By.Id("Lead_Description")).Click();
            Driver.FindElement(By.Id("Lead_Description")).Clear();
            Driver.FindElement(By.Id("Lead_Description")).SendKeys("New Description Test");
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("New Description Test", Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value"),
                "lead description edited");

            //check history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Описание:"), "lead description edited history");
            //    VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("Desc80"), "lead description edited old history");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("New Description Test"), "lead  description edited new history");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=80");
            VerifyAreEqual("New Description Test", Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value"),
                "lead  description edited after refresh");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Описание:"), "lead description edited after refresh");
            //    VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("Desc80"), "lead  description edited old history after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("New Description Test"), "lead description edited new history after refresh");
        }

        [Test]
        public void LeadEditTitle()
        {
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text.Contains("Lead80"),
                "pre check lead title");

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"leadInfoTitle\"]"), "New title", byToDropFocus: By.ClassName("control-label"));
            Driver.ClearToastMessages();

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"leadInfoTitle\"]"), "New title Test", byToDropFocus: By.ClassName("control-label"));
            Driver.ClearToastMessages();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text.Contains("New title Test"),
                "lead title edited");

            //check history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            Thread.Sleep(2000);//костыль, иначе не успевает подгрузиться.
            VerifyIsTrue(Driver.PageSource.Contains("Заголовок:"), "lead title edited history");
            // VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("New title"), "lead title edited old history");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]"))
                    .FindElement(AdvBy.DataE2E("LeadEventType")).Text.Contains("New title Test"),
                "lead title edited new history");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=80");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text.Contains("New title Test"),
                "lead title edited after refresh");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Заголовок:"), "lead title edited after refresh");
            //  VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("New title"), "lead title edited old history after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text.Contains("New title Test"),
                "lead title edited new history after refresh");
        }

        [Test]
        public void LeadEditCustomerLastName()
        {
            VerifyAreEqual("LastName80", Driver.FindElement(By.Id("Lead_LastName")).GetAttribute("value"),
                "pre check lead customer last name");

            Driver.FindElement(By.Id("Lead_LastName")).Click();
            Driver.FindElement(By.Id("Lead_LastName")).Clear();
            Driver.FindElement(By.Id("Lead_LastName")).SendKeys("Edited Last Name");
            Driver.XPathContainsText("h1", "Товары");
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Edited Last Name", Driver.FindElement(By.Id("Lead_LastName")).GetAttribute("value"),
                "lead customer last name edited");

            //check history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Фамилия:"), "lead customer last name edited history");
            //  VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("LastName80"), "lead customer last name edited old history");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("Edited Last Name"), "lead customer last name edited new history");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=80");
            VerifyAreEqual("Edited Last Name", Driver.FindElement(By.Id("Lead_LastName")).GetAttribute("value"),
                "lead customer last name edited after refresh");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Фамилия:"), "lead customer last name edited after refresh");
            // VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("LastName80"), "lead customer last name edited old history after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("Edited Last Name"), "lead customer last name edited new history after refresh");
        }

        [Test]
        public void LeadEditCustomerFirstName()
        {
            VerifyAreEqual("FirstName80", Driver.FindElement(By.Id("Lead_FirstName")).GetAttribute("value"),
                "pre check lead customer first name");

            Driver.FindElement(By.Id("Lead_FirstName")).Clear();
            Driver.FindElement(By.Id("Lead_FirstName")).SendKeys("Edited First Name");
            Driver.XPathContainsText("h1", "Товары");
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Edited First Name", Driver.FindElement(By.Id("Lead_FirstName")).GetAttribute("value"),
                "lead customer first name edited");

            //check history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Имя:"), "lead customer first name edited history");
            //  VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("FirstName80"), "lead customer first name edited old history");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("Edited First Name"), "lead customer first name edited new history");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=80");
            VerifyAreEqual("Edited First Name", Driver.FindElement(By.Id("Lead_FirstName")).GetAttribute("value"),
                "lead customer first name edited after refresh");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Имя:"), "lead customer first name edited after refresh");
            // VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("FirstName80"), "lead customer first name edited old history after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("Edited First Name"), "lead customer first name edited new history after refresh");
        }

        [Test]
        public void LeadEditCustomerPatronymic()
        {
            VerifyAreEqual("Patron80", Driver.FindElement(By.Id("Lead_Patronymic")).GetAttribute("value"),
                "pre check lead customer patronymic");

            Driver.FindElement(By.Id("Lead_Patronymic")).Click();
            Driver.FindElement(By.Id("Lead_Patronymic")).Clear();
            Driver.FindElement(By.Id("Lead_Patronymic")).SendKeys("Edited Patronymic Name");
            Driver.XPathContainsText("h1", "Товары");
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Edited Patronymic Name", Driver.FindElement(By.Id("Lead_Patronymic")).GetAttribute("value"),
                "lead customer patronymic edited");

            //check history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Имя:"), "lead customer patronymic edited history");
            //  VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("Patron80"), "lead customer patronymic edited old history");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("Edited Patronymic Name"), "lead customer patronymic edited new history");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=80");
            VerifyAreEqual("Edited Patronymic Name", Driver.FindElement(By.Id("Lead_Patronymic")).GetAttribute("value"),
                "lead customer patronymic edited after refresh");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Имя:"), "lead customer patronymic edited after refresh");
            //   VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("Patron80"), "lead customer patronymic edited old history after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("Edited Patronymic Name"), "lead customer patronymic edited new history after refresh");
        }


        [Test]
        public void LeadEditCustomerOrganization()
        {
            VerifyAreEqual("Organization80", Driver.FindElement(By.Id("Lead_Organization")).GetAttribute("value"),
                "pre check lead customer organization");

            Driver.FindElement(By.Id("Lead_Organization")).Click();
            Driver.FindElement(By.Id("Lead_Organization")).Clear();
            Driver.FindElement(By.Id("Lead_Organization")).SendKeys("Edited Organization Test");
            Driver.XPathContainsText("h1", "Товары");
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Edited Organization Test",
                Driver.FindElement(By.Id("Lead_Organization")).GetAttribute("value"),
                "lead customer organization edited");

            //check history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Организация:"), "lead customer organization edited history");
            // VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("Organization80"), "lead customer organization edited old history");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("Edited Organization Test"), "lead customer organization edited new history");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=80");
            VerifyAreEqual("Edited Organization Test",
                Driver.FindElement(By.Id("Lead_Organization")).GetAttribute("value"),
                "lead customer organization edited after refresh");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Организация:"), "lead customer organization edited after refresh");
            //   VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("Organization80"), "lead customer organization edited old history after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text
                    .Contains("Edited Organization Test"),
                "lead customer organization edited new history after refresh");
        }

        [Test]
        public void LeadEditCustomerMail()
        {
            VerifyAreEqual("testmail@mail.ru80", Driver.FindElement(By.Id("Lead_Email")).GetAttribute("value"),
                "pre check lead customer mail");

            Driver.FindElement(By.Id("Lead_Email")).Click();
            Driver.FindElement(By.Id("Lead_Email")).Clear();
            Driver.FindElement(By.Id("Lead_Email")).SendKeys("test@mail.test");
            Driver.XPathContainsText("h1", "Товары");
            Driver.WaitForToastSuccess();
            VerifyAreEqual("test@mail.test", Driver.FindElement(By.Id("Lead_Email")).GetAttribute("value"),
                "lead customer mail edited");

            //check history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Email:"), "lead customer mail edited history");
            //  VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("testmail@mail.ru80"), "lead customer mail edited old history");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text.Contains("test@mail.test"),
                "lead customer mail edited new history");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=80");
            VerifyAreEqual("test@mail.test", Driver.FindElement(By.Id("Lead_Email")).GetAttribute("value"),
                "lead customer mail edited after refresh");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Email:"), "lead customer mail edited after refresh");
            //    VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("testmail@mail.ru80"), "lead customer mail edited old history after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text.Contains("test@mail.test"),
                "lead customer mail edited new history after refresh");
        }

        [Test]
        public void LeadEditCustomerPhone()
        {
            VerifyAreEqual("+8(0__)___-__-__", Driver.FindElement(By.Id("Lead_Phone")).GetAttribute("value"),
                "pre check lead customer phone");

            Driver.FindElement(By.Id("Lead_Phone")).Click();
            Driver.ClearInput(By.Id("Lead_Phone"));
            Driver.FindElement(By.Id("Lead_Phone")).SendKeys("+79279272727");
            Driver.XPathContainsText("h3", "Контактные данные");
            Driver.WaitForToastSuccess();
            VerifyAreEqual("+7(927)927-27-27", Driver.FindElement(By.Id("Lead_Phone")).GetAttribute("value"),
                "lead customer phone edited");

            //check history
            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Телефон:"), "lead customer phone edited history");
            // VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("80"), "lead customer phone edited old history");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text.Contains("79279272727"),
                "lead customer phone edited new history");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=80");
            VerifyAreEqual("+7(927)927-27-27", Driver.FindElement(By.Id("Lead_Phone")).GetAttribute("value"),
                "lead customer phone edited after refresh");

            Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeHistory\"]")).Click();
            VerifyIsTrue(Driver.PageSource.Contains("Телефон:"), "lead customer phone edited after refresh");
            //     VerifyIsTrue(driver.FindElements(By.CssSelector(".crm-old-status"))[0].Text.Contains("80"), "lead customer phone edited old history after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-history\"]")).Text.Contains("79279272727"),
                "lead customer phone edited new history after refresh");
        }
    }

    [TestFixture]
    public class CRMLeadAddEditHistoryAttachFileTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Catalog | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Contact.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\[Order].LeadItem.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CMS.ChangeHistory.csv",
                "data\\Admin\\CRM\\Lead\\LeadHistory\\CMS.Attachment.csv"
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
        public void LeadEditAttachFile()
        {
            GoToAdmin("leads#?leadIdInfo=81");
            Driver.ScrollTo(By.Id("Lead_Organization"));
            AttachFile(By.XPath("(//input[@type='file'])"), GetPicturePath("big.png"));
            VerifyAreEqual("big.png", Driver.FindElement(By.CssSelector("[data-e2e=\"attachedFileName\"]")).Text,
                "lead file attached");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text
                    .Contains("Прикреплен файл: big.png"), "attach file");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=81");
            VerifyAreEqual("big.png", Driver.FindElement(By.CssSelector("[data-e2e=\"attachedFileName\"]")).Text,
                "lead file attached after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text
                    .Contains("Прикреплен файл: big.png"), "attach file after refresh");
        }

        [Test]
        public void LeadEditAttachedFileDelete()
        {
            GoToAdmin("leads#?leadIdInfo=82");

            VerifyAreEqual("big.png", Driver.FindElement(By.CssSelector("[data-e2e=\"attachedFileName\"]")).Text,
                "pre check lead file attached");
            VerifyIsFalse(Driver.PageSource.Contains("Нет файлов"), "pre check no file text");

            Driver.ScrollTo(By.Id("Lead_Organization"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"attachedFileDelete\"]")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.PageSource.Contains("Нет файлов"), "file deleted text");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"attachedFileName\"]")).Count == 0,
                "count attach");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text
                    .Contains("Файл откреплен: big.png"), "file attach");

            //check after refresh
            GoToAdmin("leads#?leadIdInfo=82");
            VerifyIsTrue(Driver.PageSource.Contains("Нет файлов"), "file deleted text after refresh");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"attachedFileName\"]")).Count == 0,
                "count attach after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text
                    .Contains("Файл откреплен: big.png"), "file attach after refresh");
        }
    }
}