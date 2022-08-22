using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BusinessProcess
{
    [TestFixture]
    public class SettingsBizProcessOrderStatus : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Orders | ClearType.Catalog |
                                        ClearType.Payment | ClearType.Shipping);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\Customers.Contact.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\Customers.Departments.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\Customers.Managers.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].OrderContact.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].OrderSource.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].OrderStatus.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].PaymentMethod.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].ShippingMethod.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].[Order].csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].OrderCurrency.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].OrderItems.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].OrderCustomer.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\CRM.DealStatus.csv",
                "data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\CRM.SalesFunnel.csv",
                "data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\CRM.SalesFunnel_DealStatus.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].Lead.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].LeadCurrency.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\[Order].LeadItem.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\Customers.CustomerRoleAction.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\OrderStatus\\Customers.TaskGroup.csv"
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
        public void OpenPageSettingTask()
        {
            GoToAdmin("settingstasks");
            VerifyAreEqual("Общие", Driver.FindElement(By.TagName("h3")).Text, " open page h3 - common");
            VerifyAreEqual("Бизнес-процессы", Driver.FindElements(By.TagName("h3"))[1].Text,
                " open page h3 -business process");

            IWebElement selectElem1 = Driver.FindElement(By.Id("DefaultTaskGroupId"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("All"), "select default task group");
            VerifyIsTrue(Driver.FindElements(By.TagName("h4")).Count > 0, "count business process");
        }

        [Test]
        public void SettingTaskNewStatusNew()
        {
            GoToAdmin("settingstasks");
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Смена статуса заказа",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text, " event name");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]"))))
                .SelectByText("Новый");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("15");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Фамилия");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Ad");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Фамилия = Ad", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text,
                "filter rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText("Имя");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Admin");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(1000);


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).SendKeys("New Order FIO #ORDER_ID#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_ID#, #NAME#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида
            /*
            VerifyAreEqual("Смена статуса заказа на Новый", Driver.GetGridCell(0, "EventName", "OrderStatusChangedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderStatusChangedRules").Text, "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderStatusChangedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderStatusChangedRules").Text, "create");
            VerifyAreEqual("15", Driver.GetGridCell(0, "Priority", "OrderStatusChangedRules").Text, "priority");
          */
            /*negative*/
            GoToAdmin("orders");
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.ClassName("order__main"));
            (new SelectElement(Driver.FindElement(By.Id("Order_OrderStatusId")))).SelectByText("Отменён");
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("tasks");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "new task in grid");

            /*positive*/
            GoToAdmin("orders");
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.ClassName("order__main"));
            (new SelectElement(Driver.FindElement(By.Id("Order_OrderStatusId")))).SelectByText("Новый");
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("tasks");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order FIO 30"), "name new task in grid");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-col-index=\"1\"]")).Count == 2,
                "count new tasks");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            Driver.AssertCkText("New description 30, Ad Admin, admin", "editor1");
            VerifyAreEqual("New Order FIO 30",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"), "name task");
            VerifyAreEqual("Заказ №30", Driver.FindElement(By.CssSelector("[data-e2e=\"orderLink\"]")).Text, "order");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "select assigned");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select2 = new SelectElement(selectElem1);
            VerifyIsTrue(select2.AllSelectedOptions[0].Text.Contains("Низкий"), "select priority");

            selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select2 = new SelectElement(selectElem1);
            VerifyIsTrue(select2.AllSelectedOptions[0].Text.Contains("All"), "select project");
        }

        [Test]
        public void SettingTaskStatusObr()
        {
            GoToAdmin("settingstasks");
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Смена статуса заказа",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text, " event name");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "В обработке");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("14");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Сумма заказа");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("30");


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Сумма заказа = 30", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text,
                "filter rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Admin Ad");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Admin Ad", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).SendKeys("New Order Sum #ORDER_ID#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_ID#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Высокий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateEmul\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).SendKeys("5");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"))))
                .SelectByText("В днях");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида
            /*
            VerifyAreEqual("Смена статуса заказа на В обработке", Driver.GetGridCell(0, "EventName", "OrderStatusChangedRules").Text, "name");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderStatusChangedRules").Text, "manager name");
            VerifyAreEqual("5 дней", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderStatusChangedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderStatusChangedRules").Text, "create");
            VerifyAreEqual("14", Driver.GetGridCell(0, "Priority", "OrderStatusChangedRules").Text, "priority");
            */
            /*positive*/
            GoToAdmin("orders");
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.ClassName("order__main"));
            (new SelectElement(Driver.FindElement(By.Id("Order_OrderStatusId")))).SelectByText("В обработке");
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("tasks");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Sum"), "name new task in grid");
            VerifyAreEqual("Высокий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
        }

        [Test]
        public void SettingTaskStatusShipping()
        {
            GoToAdmin("settingstasks");
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Смена статуса заказа",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text, " event name");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]"))))
                .SelectByText("Отправлен");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("13");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Группа покупателя");
            Thread.Sleep(1000);
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("Обычный покупатель");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Группа покупателя = Обычный покупатель",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text, "filter rule new order");


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).SendKeys("New Order Group #ORDER_ID#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_ID#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида
            /*
            VerifyAreEqual("Смена статуса заказа на Отправлен", Driver.GetGridCell(0, "EventName", "OrderStatusChangedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderStatusChangedRules").Text, "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderStatusChangedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderStatusChangedRules").Text, "create");
            VerifyAreEqual("13", Driver.GetGridCell(0, "Priority", "OrderStatusChangedRules").Text, "priority");
            */
            /*positive*/
            GoToAdmin("orders");
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.ClassName("order__main"));
            (new SelectElement(Driver.FindElement(By.Id("Order_OrderStatusId")))).SelectByText("Отправлен");
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("tasks");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Group"), "name new task in grid");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
        }

        [Test]
        public void SettingTaskStatuszDeliver()
        {
            GoToAdmin("settingstasks");
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Смена статуса заказа",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text, " event name");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]"))))
                .SelectByText("Доставлен");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("9");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Email");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("admin");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Email = admin", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text,
                "filter rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Источник заказа");
            Thread.Sleep(1000);
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("Мобильная версия");


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("New Order Source #ORDER_ID#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_ID#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateEmul\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).SendKeys("24");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"))))
                .SelectByText("В часах");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида
            /*
            VerifyAreEqual("Смена статуса заказа на Доставлен", Driver.GetGridCell(0, "EventName", "OrderStatusChangedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderStatusChangedRules").Text, "manager name");
            VerifyAreEqual("24 часа", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderStatusChangedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderStatusChangedRules").Text, "create");
            VerifyAreEqual("9", Driver.GetGridCell(0, "Priority", "OrderStatusChangedRules").Text, "priority");
          */
            /*negative*/
            GoToAdmin("orders");
            Driver.GetGridCell(1, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.ClassName("order__main"));
            (new SelectElement(Driver.FindElement(By.Id("Order_OrderStatusId")))).SelectByText("Доставлен");
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("tasks");
            VerifyIsFalse(Driver.GetGridCell(0, "Name").Text.Contains("New Order Source"), "name new task in grid");

            /*positive*/
            GoToAdmin("orders");
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.ClassName("order__main"));
            (new SelectElement(Driver.FindElement(By.Id("Order_OrderStatusId")))).SelectByText("Доставлен");
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("tasks");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Source"), "name new task in grid");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
        }

        /*
        [Test]
        public void SettingTaskStatuzCancel()
        {
            GoToAdmin("settingstasks");
            driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Смена статуса заказа", driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text, " event name");

            (new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText("Отменён");
            driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("8");

            driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText("Телефон");
            Thread.Sleep(1000);
            driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("+74958002001");

            driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Телефон = +74958002001", driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text, "filter rule new order");


            driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]")))).SelectByText("Elena El");

            driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text, "name assigned user rule new order");

            driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).SendKeys("New Order Phone #ORDER_ID#");
            driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).SendKeys("New description #ORDER_ID#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]")))).SelectByText("Низкий");
            (new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]")))).SelectByText("All");

            driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида
            
            VerifyAreEqual("Смена статуса заказа на Отменён", Driver.GetGridCell(0, "EventName", "OrderStatusChangedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderStatusChangedRules").Text, "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderStatusChangedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderStatusChangedRules").Text, "create");
            VerifyAreEqual("8", Driver.GetGridCell(0, "Priority", "OrderStatusChangedRules").Text, "priority");
            
            //positive
            Functions.NewOrderClient_450(driver, baseURL);
            Thread.Sleep(1000);
            GoToAdmin("tasks");
            VerifyIsFalse(driver.PageSource.Contains("Ни одной записи не найдено"), "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Phone"), "name new task in grid");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text), "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text), "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text), "assigned new task in grid");
        }
    */
        [Test]
        public void SettingTaskStatuzCancelAll()
        {
            GoToAdmin("settingstasks");
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Смена статуса заказа",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text, " event name");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]"))))
                .SelectByText("Отменён");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("6");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Создан из лида");
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Создан из лида = Да",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text, "filter rule new order");


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).SendKeys("New Order Lead #ORDER_ID#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_ID#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида
/*
            VerifyAreEqual("Смена статуса заказа на Отменен навсегда", Driver.GetGridCell(0, "EventName", "OrderStatusChangedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderStatusChangedRules").Text, "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderStatusChangedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderStatusChangedRules").Text, "create");
            VerifyAreEqual("6", Driver.GetGridCell(0, "Priority", "OrderStatusChangedRules").Text, "priority");
            */
            /*negative*/
            GoToAdmin("orders");
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.ClassName("order__main"));
            (new SelectElement(Driver.FindElement(By.Id("Order_OrderStatusId")))).SelectByText("Отменён");
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("tasks");
            VerifyIsFalse(Driver.GetGridCell(0, "Name").Text.Contains("New Order Lead"), "name new task in grid");

            /*positive*/
            GoToAdmin("leads#?leadIdInfo=119");
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadCreateOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"completeLead\"]")).Click();
            Thread.Sleep(2000);

            (new SelectElement(Driver.FindElement(By.Id("Order_OrderStatusId")))).SelectByText("Отменён");
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            GoToAdmin("tasks");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Lead"), "name new task in grid");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
        }
    }
}