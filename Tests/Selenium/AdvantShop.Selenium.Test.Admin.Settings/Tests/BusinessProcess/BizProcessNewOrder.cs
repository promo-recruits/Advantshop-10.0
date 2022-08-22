using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BusinessProcess
{
    [TestFixture]
    public class SettingsBizProcessNewOrder : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Orders | ClearType.Catalog |
                                        ClearType.Shipping | ClearType.Payment);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BizProcessOrder\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Customers.Contact.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Customers.Departments.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Customers.Managers.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Customers.TaskGroup.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].OrderContact.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].OrderSource.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].OrderStatus.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].PaymentMethod.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].ShippingMethod.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].[Order].csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].OrderCurrency.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].OrderItems.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].OrderCustomer.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\CRM.DealStatus.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\CRM.SalesFunnel.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\CRM.SalesFunnel_DealStatus.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].Lead.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].LeadCurrency.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\[Order].LeadItem.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Customers.CustomerRoleAction.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Settings.Settings.csv",
                "Data\\Admin\\Settings\\BizProcessOrder\\Customers.ManagerTask.csv"
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
        public void SettingTaskNewOrderFio()
        {
            GoToAdmin("settingstasks");
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderCreated\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Новый заказ", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text,
                " event name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("15");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Фамилия");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Ad");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();

            VerifyAreEqual("Фамилия = Ad", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text,
                "filter rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText("Имя");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Admin");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Отчество");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Patronymic");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("New Order FIO #ORDER_NUMBER#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_NUMBER#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида

            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "EventName", "OrderCreatedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderCreatedRules").Text,
                "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderCreatedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderCreatedRules").Text,
                "create");
            VerifyAreEqual("15", Driver.GetGridCell(0, "Priority", "OrderCreatedRules").Text, "priority");

            /*positive*/
            Functions.NewFullOrderClient_9000(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            GoToAdmin("tasks");
            VerifyIsFalse(Driver.FindElements(By.ClassName("ui-grid-empty-text")).Count == 1, "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order FIO 31"), "name new task in grid");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-col-index=\"1\"]")).Count == 2,
                "count new tasks");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();
            Driver.AssertCkText("New description 31, Ad Admin Patronymic, +7 495 800 20 02 , admin", "editor1");
            VerifyAreEqual("New Order FIO 31",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"), "name task");
            VerifyAreEqual("Заказ №31", Driver.FindElement(By.CssSelector("[data-e2e=\"orderLink\"]")).Text, "order");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "select assigned");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select2 = new SelectElement(selectElem1);
            VerifyIsTrue(select2.AllSelectedOptions[0].Text.Contains("Низкий"), "select priority");

            selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select2 = new SelectElement(selectElem1);
            VerifyIsTrue(select2.AllSelectedOptions[0].Text.Contains("All"), "select project");

            Driver.FindElement(By.CssSelector("[data-e2e=\"orderLink\"]")).Click();
            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("order-header-item"));
            VerifyIsTrue(Driver.Url.EndsWith("orders/edit/31"), "order url");
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Заказ № 31"), "name order h1");
            GoToAdmin("orders/edit/31#?orderTabs=3");
            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(
                Driver.GetGridCell(0, "Name", "Tasks").FindElement(By.TagName("span")).Text
                    .Contains("New Order FIO 31"), "name new task in order grid");
            VerifyAreEqual("В работе",
                (Driver.GetGridCell(0, "StatusFormatted", "Tasks").FindElement(By.TagName("div")).Text),
                "statys new task in order grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers", "Tasks").FindElement(By.TagName("div")).Text),
                "assigned new task in order grid");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"leadTaskAdd\"]"));
            Driver.GetGridCell(0, "Name", "Tasks").FindElement(By.TagName("span")).Click();
            //Driver.GetGridCell(0, "Name", "Tasks").Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed, "open modal window");
            VerifyAreEqual("New Order FIO 31",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "name task modal");
            VerifyAreEqual("Заказ №31", Driver.FindElement(By.CssSelector("[data-e2e=\"orderLink\"]")).Text,
                "order modal");
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void SettingTaskNewOrderFrom100To10000()
        {
            GoToAdmin("settingscheckout#?checkoutTab=common");
            (new SelectElement(Driver.FindElement(By.Id("BuyInOneClickAction")))).SelectByText("Создавать заказ");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();


            GoToAdmin("settingstasks");

            try
            {
                Driver.GetGridCell(0, "_serviceColumn", "OrderCreatedRules")
                    .FindElement(By.CssSelector(".fa.fa-times.link-invert")).Click();
                Driver.FindElement(By.CssSelector(".swal2-popup.swal2-modal .btn.btn-success")).Click();
                Thread.Sleep(1000);
            }
            catch { }

            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderCreated\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Новый заказ", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text,
                " event name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("14");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Сумма заказа");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleRange\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueFrom\"]")).SendKeys("100");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueTo\"]")).SendKeys("1000");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();

            VerifyAreEqual("Сумма заказа от 100 до 1000",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text, "filter rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Admin Ad");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Admin Ad", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("New Order Sum #ORDER_NUMBER#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_NUMBER#, #NAME#, #PHONE#, #EMAIL#");

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

            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "EventName", "OrderCreatedRules").Text, "name");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderCreatedRules").Text,
                "manager name");
            VerifyAreEqual("5 дней", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderCreatedRules").Text,
                "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderCreatedRules").Text,
                "create");
            VerifyAreEqual("14", Driver.GetGridCell(0, "Priority", "OrderCreatedRules").Text, "priority");

            /*positive*/
            Functions.NewOrderClient_450(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            GoToAdmin("tasks");
            Driver.GridFilterSendKeys("New Order Sum");
            VerifyIsFalse(Driver.FindElements(By.ClassName("ui-grid-empty-text")).Count == 1, "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Sum"), "name new task in grid");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-col-index=\"1\"]")).Count == 2,
                "1 count new tasks");
            VerifyAreEqual("Высокий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
            /*negative*/
            Functions.NewOrderClient_1350(Driver, BaseUrl);
            GoToAdmin("tasks");
            Driver.GridFilterSendKeys("New Order Sum");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Sum"), "repeat name new task in grid");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-col-index=\"1\"]")).Count == 2,
                "count new tasks");
        }

        [Test]
        public void SettingTaskNewOrderGroup()
        {
            GoToAdmin("settingscheckout#?checkoutTab=common");
            (new SelectElement(Driver.FindElement(By.Id("BuyInOneClickAction")))).SelectByText("Создавать заказ");
            Driver.ScrollToTop();
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToAdmin("settingstasks");
            
            
            try{
                Driver.GetGridCell(0, "_serviceColumn", "OrderCreatedRules")
                    .FindElement(By.CssSelector(".fa.fa-times.link-invert")).Click();
                Driver.FindElement(By.CssSelector(".swal2-popup.swal2-modal .btn.btn-success")).Click();
                Thread.Sleep(1000);
            }
            catch { }

            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderCreated\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Новый заказ", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text,
                " event name");

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

            VerifyAreEqual("Группа покупателя = Обычный покупатель",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text, "filter rule new order");


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("New Order Group #ORDER_NUMBER#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_NUMBER#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида

            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "EventName", "OrderCreatedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderCreatedRules").Text,
                "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderCreatedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderCreatedRules").Text,
                "create");
            VerifyAreEqual("13", Driver.GetGridCell(0, "Priority", "OrderCreatedRules").Text, "priority");

            /*positive*/
            Functions.NewOrderClient_450(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            GoToAdmin("tasks");
            VerifyIsFalse(Driver.FindElements(By.ClassName("ui-grid-empty-text")).Count == 1, "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Group"), "name new task in grid");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
        }

        [Test]
        public void SettingTaskNewOrderPayment()
        {
            GoToAdmin("settingstasks");

            try { 
            Driver.GetGridCell(0, "_serviceColumn", "OrderCreatedRules")
                .FindElement(By.CssSelector(".fa.fa-times.link-invert")).Click();
            Driver.FindElement(By.CssSelector(".swal2-popup.swal2-modal .btn.btn-success")).Click();
            Thread.Sleep(1000);
            }
            catch { }

            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderCreated\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Новый заказ", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text,
                " event name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("12");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Метод оплаты");
            Thread.Sleep(100);
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("Наличными курьеру");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();

            VerifyAreEqual("Метод оплаты = Наличными курьеру",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text, "filter rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("New Order Payment #ORDER_NUMBER#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_NUMBER#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида

            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "EventName", "OrderCreatedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderCreatedRules").Text,
                "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderCreatedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderCreatedRules").Text,
                "create");
            VerifyAreEqual("12", Driver.GetGridCell(0, "Priority", "OrderCreatedRules").Text, "priority");

            //positive
            Functions.NewFullOrderClient_9000(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            GoToAdmin("tasks");
            VerifyIsFalse(Driver.FindElements(By.ClassName("ui-grid-empty-text")).Count == 1, "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Payment"), "name new task in grid");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
        }

        [Test]
        public void SettingTaskNewOrderRegion()
        {
            GoToAdmin("settingstasks");

            try { 
                Driver.GetGridCell(0, "_serviceColumn", "OrderCreatedRules")
                    .FindElement(By.CssSelector(".fa.fa-times.link-invert")).Click();
                Driver.FindElement(By.CssSelector(".swal2-popup.swal2-modal .btn.btn-success")).Click();
                Thread.Sleep(1000);
            }
            catch { }

            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderCreated\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Новый заказ", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text,
                " event name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("11");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Страна");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Россия");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();

            VerifyAreEqual("Страна = Россия", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text,
                "filter rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Регион");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Московская область");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Город");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Москва");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("New Order Adress #ORDER_NUMBER#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_NUMBER#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида

            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "EventName", "OrderCreatedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderCreatedRules").Text,
                "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderCreatedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderCreatedRules").Text,
                "create");
            VerifyAreEqual("11", Driver.GetGridCell(0, "Priority", "OrderCreatedRules").Text, "priority");

            /*positive*/
            Functions.NewFullOrderClient_9000(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            GoToAdmin("tasks");
            VerifyIsFalse(Driver.FindElements(By.ClassName("ui-grid-empty-text")).Count == 1, "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Adress"), "name new task in grid");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
        }

        [Test]
        public void SettingTaskNewOrderShipping()
        {
            GoToAdmin("settingstasks");
            
            try{
                Driver.GetGridCell(0, "_serviceColumn", "OrderCreatedRules")
                    .FindElement(By.CssSelector(".fa.fa-times.link-invert")).Click();
                Driver.FindElement(By.CssSelector(".swal2-popup.swal2-modal .btn.btn-success")).Click();
                Thread.Sleep(1000);
                }
            catch { }


            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderCreated\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Новый заказ", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text,
                " event name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("10");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Метод доставки");
            Thread.Sleep(100);
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("Самовывоз");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();

            VerifyAreEqual("Метод доставки = Самовывоз",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text, "filter rule new order");


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("New Order Shipping #ORDER_NUMBER#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_NUMBER#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида

            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "EventName", "OrderCreatedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderCreatedRules").Text,
                "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderCreatedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderCreatedRules").Text,
                "create");
            VerifyAreEqual("10", Driver.GetGridCell(0, "Priority", "OrderCreatedRules").Text, "priority");

            /*positive*/

            Driver.Navigate().GoToUrl(BaseUrl + "/products/test-product100");
            Driver.WaitForElem(By.ClassName("details-block"));
            var element = Driver.FindElement(By.CssSelector(".link-dotted-invert.cs-l-d-1"));
            IJavaScriptExecutor jse = (IJavaScriptExecutor) Driver;
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(1000);

            Driver.Navigate().GoToUrl(BaseUrl + "/checkout");
            Driver.WaitForElem(By.ClassName("checkout-page"));
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Самовывоз')]"));
            Driver.XPathContainsText("span", "Самовывоз");
            Thread.Sleep(1000);
            element = Driver.FindElement(By.CssSelector(".checkout-result"));
            jse = (IJavaScriptExecutor) Driver;
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            GoToAdmin("tasks");
            VerifyIsFalse(Driver.FindElements(By.ClassName("ui-grid-empty-text")).Count == 1, "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Shipping"), "name new task in grid");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
        }

        [Test]
        public void SettingTaskNewOrderSource()
        {
            GoToAdmin("settingstasks");

            try
            {
                Driver.GetGridCell(0, "_serviceColumn", "OrderCreatedRules")
                    .FindElement(By.CssSelector(".fa.fa-times.link-invert")).Click();
                Driver.FindElement(By.CssSelector(".swal2-popup.swal2-modal .btn.btn-success")).Click();
                Thread.Sleep(1000);
            }
            catch { }

            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderCreated\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Новый заказ", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text,
                " event name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("9");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Email");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("admin");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();

            VerifyAreEqual("Email = admin", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text,
                "filter rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Источник заказа");
            Thread.Sleep(100);
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("В один клик");


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("New Order Source #ORDER_NUMBER#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_NUMBER#, #NAME#, #PHONE#, #EMAIL#");

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

            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "EventName", "OrderCreatedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderCreatedRules").Text,
                "manager name");
            VerifyAreEqual("24 часа", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderCreatedRules").Text,
                "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderCreatedRules").Text,
                "create");
            VerifyAreEqual("9", Driver.GetGridCell(0, "Priority", "OrderCreatedRules").Text, "priority");

            /*negative*/
            Functions.NewFullOrderClient_9000(Driver, BaseUrl);
            GoToAdmin("tasks");
            VerifyIsFalse(Driver.GetGridCell(0, "Name").Text.Contains("New Order Source"),
                "repeat name new task in grid");

            /*positive*/
            Functions.NewOrderClient_450(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            GoToAdmin("tasks");
            VerifyIsFalse(Driver.FindElements(By.ClassName("ui-grid-empty-text")).Count == 1, "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Source"), "name new task in grid");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
        }

        [Test]
        public void SettingTaskNewOrderTel()
        {
            GoToAdmin("settingstasks");
            try { 
                Driver.GetGridCell(0, "_serviceColumn", "OrderCreatedRules")
                    .FindElement(By.CssSelector(".fa.fa-times.link-invert")).Click();
                Driver.FindElement(By.CssSelector(".swal2-popup.swal2-modal .btn.btn-success")).Click();
                Thread.Sleep(1000);
            }
            catch { }

            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderCreated\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Новый заказ", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text,
                " event name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("8");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Телефон");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("74958002002");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();

            VerifyAreEqual("Телефон = 74958002002",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text, "filter rule new order");


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("New Order Phone #ORDER_NUMBER#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_NUMBER#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида

            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "EventName", "OrderCreatedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderCreatedRules").Text,
                "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderCreatedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderCreatedRules").Text,
                "create");
            VerifyAreEqual("8", Driver.GetGridCell(0, "Priority", "OrderCreatedRules").Text, "priority");

            /*positive*/
            GoToClient("/products/test-product5");

            Driver.FindElement(By.CssSelector(".link-dotted-invert.cs-l-d-1")).Click();
            Driver.WaitForElem(By.ClassName("buy-one-click-dialog"));
            Driver.ClearInput(By.Id("buyOneClickFormPhone"));
            Driver.FindElement(By.Id("buyOneClickFormPhone")).SendKeys("74958002002");
            Driver.FindElement(By.CssSelector(".buy-one-click-buttons input")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            GoToAdmin("tasks");
            VerifyIsFalse(Driver.FindElements(By.ClassName("ui-grid-empty-text")).Count == 1, "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Phone"), "name new task in grid");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("Elena El", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
        }

        [Test]
        public void SettingTaskNewOrderzFromAdmin()
        {
            GoToAdmin("settingstasks");

            try { 
                Driver.GetGridCell(0, "_serviceColumn", "OrderCreatedRules")
                    .FindElement(By.CssSelector(".fa.fa-times.link-invert")).Click();
                Driver.FindElement(By.CssSelector(".swal2-popup.swal2-modal .btn.btn-success")).Click();
                Thread.Sleep(1000);
            }
            catch { }

            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderCreated\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Новый заказ", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text,
                " event name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("7");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Создан в части администрирования");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();

            VerifyAreEqual("Создан в части администрирования = Да",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text, "filter rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]")))).SelectByText(
                "Менеджер заказа");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Менеджер заказа",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("New Order Admin #ORDER_NUMBER#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description Admin  #ORDER_NUMBER#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Средний");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида

            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "EventName", "OrderCreatedRules").Text, "name");
            VerifyAreEqual("Менеджер заказа", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderCreatedRules").Text,
                "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderCreatedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderCreatedRules").Text,
                "create");
            VerifyAreEqual("7", Driver.GetGridCell(0, "Priority", "OrderCreatedRules").Text, "priority");

            /*negative*/
            Functions.NewFullOrderClient_9000(Driver, BaseUrl);
            GoToAdmin("tasks");
            VerifyIsFalse(Driver.GetGridCell(0, "Name").Text.Contains("New Order Admin"),
                "repeat name new task in grid");

            /*positive*/
            GoToAdmin("orders/add");
            (new SelectElement(Driver.FindElement(By.Id("Order_ManagerId")))).SelectByText("test testov");

            Driver.FindElement(By.CssSelector(".edit.link-decoration-none")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".ui-grid-cell-contents a")).Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.LinkText("Добавить товар")).Click();

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr").Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr").Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Thread.Sleep(1000);

            GoToAdmin("tasks");
            VerifyIsFalse(Driver.FindElements(By.ClassName("ui-grid-empty-text")).Count == 1, "new task in grid");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("New Order Admin"), "name new task in grid");
            VerifyAreEqual("Средний", (Driver.GetGridCell(0, "PriorityFormatted").FindElement(By.TagName("div")).Text),
                "priority new task in grid");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "StatusFormatted").FindElement(By.TagName("div")).Text),
                "statys new task in grid");
            VerifyAreEqual("test testov", (Driver.GetGridCell(0, "Managers").FindElement(By.TagName("div")).Text),
                "assigned new task in grid");
        }

        [Test]
        public void SettingTaskNewOrderzFromLead()
        {
            GoToAdmin("settingstasks");

            try { 
                Driver.GetGridCell(0, "_serviceColumn", "OrderCreatedRules")
                    .FindElement(By.CssSelector(".fa.fa-times.link-invert")).Click();
                Driver.FindElement(By.CssSelector(".swal2-popup.swal2-modal .btn.btn-success")).Click();
                Thread.Sleep(1000);
            }
            catch { }

            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"OrderCreated\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Новое правило", Driver.FindElement(By.TagName("h2")).Text, " new rule title");
            VerifyAreEqual("Новый заказ", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text,
                " event name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("6");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForModal();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Создан из лида");
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();

            VerifyAreEqual("Создан из лида = Да",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text, "filter rule new order");


            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();

            VerifyAreEqual("Elena El", Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text,
                "name assigned user rule new order");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("New Order Lead #ORDER_NUMBER#");
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("New description #ORDER_NUMBER#, #NAME#, #PHONE#, #EMAIL#");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"))))
                .SelectByText("All");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();
            //проверка грида

            VerifyAreEqual("Новый заказ", Driver.GetGridCell(0, "EventName", "OrderCreatedRules").Text, "name");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "OrderCreatedRules").Text,
                "manager name");
            VerifyAreEqual("", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "OrderCreatedRules").Text, "date");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "OrderCreatedRules").Text,
                "create");
            VerifyAreEqual("6", Driver.GetGridCell(0, "Priority", "OrderCreatedRules").Text, "priority");

            /*negative*/
            Functions.NewOrderClient_450(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            GoToAdmin("tasks");
            Driver.GridFilterSendKeys("New Order Lead");

            /*positive*/
            GoToAdmin("leads#?leadIdInfo=120");
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadCreateOrder\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"completeLead\"]")).Click();
            Thread.Sleep(1000);
            Driver.WaitForElem(By.Id("Order_OrderStatusId"));
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Заказ №"), "new order h1");

            GoToAdmin("tasks");
            VerifyIsFalse(Driver.FindElements(By.ClassName("ui-grid-empty-text")).Count == 1, "new task in grid");
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