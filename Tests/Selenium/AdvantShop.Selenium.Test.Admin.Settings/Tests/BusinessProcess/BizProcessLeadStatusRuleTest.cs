using System.Collections.ObjectModel;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BusinessProcess
{
    [TestFixture]
    public class SettingsBizProcessLeadStatusRuleTest : SettingsLeadFunctions
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Catalog | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\BizProcessLead\\Catalog.Product.csv",
                "data\\Admin\\Settings\\BizProcessLead\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\BizProcessLead\\Catalog.Category.csv",
                "data\\Admin\\Settings\\BizProcessLead\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\BizProcessLead\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\BizProcessLead\\Customers.Customer.csv",
                "data\\Admin\\Settings\\BizProcessLead\\Customers.Contact.csv",
                "data\\Admin\\Settings\\BizProcessLead\\Customers.Departments.csv",
                "data\\Admin\\Settings\\BizProcessLead\\Customers.Managers.csv",
                "data\\Admin\\Settings\\BizProcessLead\\CRM.DealStatus.csv",
                "data\\Admin\\Settings\\BizProcessLead\\CRM.SalesFunnel.csv",
                "data\\Admin\\Settings\\BizProcessLead\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\Settings\\BizProcessLead\\[Order].OrderSource.csv",
                "data\\Admin\\Settings\\BizProcessLead\\[Order].Lead.csv",
                "data\\Admin\\Settings\\BizProcessLead\\[Order].LeadCurrency.csv",
                "data\\Admin\\Settings\\BizProcessLead\\[Order].LeadItem.csv",
                "data\\Admin\\Settings\\BizProcessLead\\Customers.TaskGroup.csv",
                "data\\Admin\\Settings\\BizProcessLead\\Customers.Task.csv",
                "data\\Admin\\Settings\\BizProcessLead\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\Settings\\BizProcessLead\\[Order].LeadEvent.csv"
            );

            Init();

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void LeadStatusFilterSum()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).GetAttribute("innerText")
                    .Contains("Смена этапа лида"), "biz rule type");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Созвон с клиентом");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Бюджет");

            Driver.FindElement(By.LinkText("Указать диапазон")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueFrom\"]")).SendKeys("150");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueTo\"]")).SendKeys("200");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).SendKeys("Task name sum");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).SendKeys("Task text sum");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateEmul\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).SendKeys("6");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"))))
                .SelectByText("В днях");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Высокий");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            //check rules grid
            GoToAdmin("settingstasks");

            VerifyAreEqual("Смена этапа лида на Созвон с клиентом (SalesFunnel1)",
                Driver.GetGridCell(0, "EventName", "LeadStatusChangedRules").Text, "biz rule grid type");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "LeadStatusChangedRules").Text,
                "biz rule grid manager");
            VerifyAreEqual("6 дней",
                Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "LeadStatusChangedRules").Text,
                "biz rule grid task due time");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "LeadStatusChangedRules").Text,
                "biz rule grid task interval");
            VerifyAreEqual("0", Driver.GetGridCell(0, "Priority", "LeadStatusChangedRules").Text,
                "biz rule grid priority");

            setLeadBuyInOneClick();

            //client
            ReInitClient();
            leadBuyOneClick("100", "Task filter sum", "71231231122");

            ReInit();

            //check task not added yet 
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name sum");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created yet");

            changeLeadStatus("121", "Созвон с клиентом");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name sum");

            VerifyAreEqual("Task name sum", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Высокий", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name sum",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text sum", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Высокий"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(""), "lead desc");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct100"), "lead product");
            VerifyAreEqual("Task name sum", Driver.GetGridCell(0, "Name", "Tasks").Text, "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            IWebElement selectElemDealStatus = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElemDealStatus);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Созвон с клиентом"), "lead status");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadStatusFilterFIO()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Созвон с клиентом");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText("Имя");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("FirstName");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Фамилия");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("LastName");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Отчество");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Patronymic");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).SendKeys("Task name test FIO filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).SendKeys("Task text test FIO filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateEmul\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).SendKeys("20");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"))))
                .SelectByText("В минутах");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            changeLeadStatus("77", "Созвон с клиентом");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test FIO filter");

            VerifyAreEqual("Task name test FIO filter", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name test FIO filter",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text test FIO filter", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Средний"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №77", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains("Desc77"), "lead desc");

            IWebElement selectElemDealStatus = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElemDealStatus);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Созвон с клиентом"), "lead status");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct77"), "lead product");
            VerifyAreEqual("Task name test FIO filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadStatusFilterEmail()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Выставление КП");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Email");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("mail@mail.com");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("Task name test Email filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("Task text test Email filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            changeLeadStatus("25", "Выставление КП");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test Email filter");

            VerifyAreEqual("Task name test Email filter", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name test Email filter",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text test Email filter", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Средний"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №25", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains("Desc25"), "lead desc");

            IWebElement selectElemDealStatus = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElemDealStatus);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Выставление КП"), "lead status");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct25"), "lead product");
            VerifyAreEqual("Task name test Email filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadStatusFilterPhone()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Выставление КП");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Телефон");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("79999999999");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("Task name test Phone filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("Task text test Phone filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            /* create lead from client */
            setLeadBuyInOneClick();

            //client
            ReInitClient();
            leadBuyOneClick("101", "Task filter Phone", "79999999999");

            //check task not added yet 
            ReInit();
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test Phone filter");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created yet");

            changeLeadStatus("121", "Выставление КП");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test Phone filter");

            VerifyAreEqual("Task name test Phone filter", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name test Phone filter",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text test Phone filter", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Средний"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(""), "lead desc");

            IWebElement selectElemDealStatus = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElemDealStatus);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Выставление КП"), "lead status");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct101"), "lead product");
            VerifyAreEqual("Task name test Phone filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadStatusFilterCustomerGroup()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Выставление КП");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Группа покупателя");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("CustomerGroup2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("Task name test CustomerGroup filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("Task text test CustomerGroup filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            changeLeadStatus("24", "Выставление КП");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test CustomerGroup filter");

            VerifyAreEqual("Task name test CustomerGroup filter", Driver.GetGridCell(0, "Name").Text,
                "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name test CustomerGroup filter",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text test CustomerGroup filter", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Средний"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №24", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains("Desc24"), "lead desc");

            IWebElement selectElemDealStatus = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElemDealStatus);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Выставление КП"), "lead status");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct24"), "lead product");
            VerifyAreEqual("Task name test CustomerGroup filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadStatusFilterSource()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Выставление КП");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Источник");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("Оффлайн");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("Task name test Source filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("Task text test Source filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            changeLeadStatus("100", "Выставление КП");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test Source filter");
            VerifyAreEqual("Task name test Source filter", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name test Source filter",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text test Source filter", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Средний"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №100", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains("Desc100"), "lead desc");

            IWebElement selectElemDealStatus = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElemDealStatus);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Выставление КП"), "lead status");

            IWebElement selectElemLeadSource = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            select = new SelectElement(selectElemLeadSource);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Оффлайн"), "lead source");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct100"), "lead product");
            VerifyAreEqual("Task name test Source filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadStatusFilterCountryRegionCity()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Ожидание решения клиента");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Страна");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Россия");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Регион");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Московская область");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Город");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Москва");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("Task name test CountryRegionCity filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("Task text test CountryRegionCity filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            changeLeadStatus("23", "Ожидание решения клиента");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test CountryRegionCity filter");
            VerifyAreEqual("Task name test CountryRegionCity filter", Driver.GetGridCell(0, "Name").Text,
                "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name test CountryRegionCity filter",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text test CountryRegionCity filter", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Средний"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №23", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains("Desc23"), "lead desc");

            IWebElement selectElemDealStatus = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElemDealStatus);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ожидание решения клиента"), "lead status");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct23"), "lead product");
            VerifyAreEqual("Task name test CountryRegionCity filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }


        [Test]
        public void LeadStatusFilterCreateFromAdmin()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Ожидание решения клиента");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Создан в части администрирования");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]")))).SelectByText(
                "test testov");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).SendKeys("Task name lead from admin");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).SendKeys("Task text lead from admin");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateEmul\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).SendKeys("1");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"))))
                .SelectByText("В часах");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            //check rules grid
            GoToAdmin("settingstasks");

            VerifyAreEqual("Смена этапа лида на Ожидание решения клиента (SalesFunnel1)",
                Driver.GetGridCell(0, "EventName", "LeadStatusChangedRules").Text, "biz rule grid type");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerFilterHTML", "LeadStatusChangedRules").Text,
                "biz rule grid manager");
            VerifyAreEqual("1 час",
                Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "LeadStatusChangedRules").Text,
                "biz rule grid task due time");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "LeadStatusChangedRules").Text,
                "biz rule grid task interval");
            VerifyAreEqual("0", Driver.GetGridCell(0, "Priority", "LeadStatusChangedRules").Text,
                "biz rule grid priority");

            //create lead from admin
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]"))
                .SendKeys("лид из администрированной части");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+79729272728");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(4, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check task not added yet 
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name lead from admin");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created yet");

            changeLeadStatus("121", "Ожидание решения клиента");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name lead from admin");

            VerifyAreEqual("Task name lead from admin", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Низкий", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name lead from admin",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text lead from admin", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("test testov"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Низкий"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(""), "lead desc");

            IWebElement selectElemDealStatus = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElemDealStatus);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ожидание решения клиента"), "lead status");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct5"), "lead product");
            VerifyAreEqual("Task name lead from admin", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }


        [Test]
        public void LeadStatusFilterDesc()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Ожидание решения клиента");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Описание");

            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("input")).Clear();
            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("input")).SendKeys("Desc33");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]")))).SelectByText(
                "test testov");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("Task name lead with description");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("Task text lead with description");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateEmul\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).SendKeys("1");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"))))
                .SelectByText("В часах");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            //check rules grid
            GoToAdmin("settingstasks");

            VerifyAreEqual("Смена этапа лида на Ожидание решения клиента (SalesFunnel1)",
                Driver.GetGridCell(0, "EventName", "LeadStatusChangedRules").Text, "biz rule grid type");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerFilterHTML", "LeadStatusChangedRules").Text,
                "biz rule grid manager");
            VerifyAreEqual("1 час",
                Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "LeadStatusChangedRules").Text,
                "biz rule grid task due time");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "LeadStatusChangedRules").Text,
                "biz rule grid task interval");
            VerifyAreEqual("0", Driver.GetGridCell(0, "Priority", "LeadStatusChangedRules").Text,
                "biz rule grid priority");

            changeLeadStatus("33", "Ожидание решения клиента");

            //check task added 
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name lead with description");

            VerifyAreEqual("Task name lead with description", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Низкий", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name lead with description",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text lead with description", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("test testov"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Низкий"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №33", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains("Desc33"), "lead desc");

            IWebElement selectElemDealStatus = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElemDealStatus);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ожидание решения клиента"), "lead status");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct33"), "lead product");
            VerifyAreEqual("Task name lead with description", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadStatusFilterCreateFromAdminValueNot()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Ожидание решения клиента");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Создан в части администрирования");

            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.CssSelector(".adv-checkbox-label"))
                .FindElement(By.TagName("span")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]")))).SelectByText(
                "test testov");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("Task name lead from admin - not");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("Task text lead from admin - not");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateEmul\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).SendKeys("1");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"))))
                .SelectByText("В часах");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            //check rules grid
            GoToAdmin("settingstasks");

            VerifyAreEqual("Смена этапа лида на Ожидание решения клиента (SalesFunnel1)",
                Driver.GetGridCell(0, "EventName", "LeadStatusChangedRules").Text, "biz rule grid type");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerFilterHTML", "LeadStatusChangedRules").Text,
                "biz rule grid manager");
            VerifyAreEqual("1 час",
                Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "LeadStatusChangedRules").Text,
                "biz rule grid task due time");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "LeadStatusChangedRules").Text,
                "biz rule grid task interval");
            VerifyAreEqual("0", Driver.GetGridCell(0, "Priority", "LeadStatusChangedRules").Text,
                "biz rule grid priority");

            //create lead
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]"))
                .SendKeys("лид из администрированной части");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+79729272729");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(4, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check task not added yet 
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name lead from admin - not");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created yet");

            changeLeadStatus("121", "Ожидание решения клиента");

            //check task not added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name lead from admin - not");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created");
        }


        [Test]
        public void LeadStatusFilterVariables()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Ожидание решения клиента");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Группа покупателя");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("CustomerGroup2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]")))).SelectByText(
                "test testov");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("название задачи - #LEAD_ID#, #NAME#, #PHONE#, #EMAIL#");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("текст задачи - #LEAD_ID#, #NAME#, #PHONE#, #EMAIL#");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateEmul\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).SendKeys("1");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"))))
                .SelectByText("В часах");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Низкий");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            //check rule pop up
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.GetGridCell(0, "_serviceColumn", "LeadStatusChangedRules")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text
                    .Contains("Смена этапа лида на Ожидание решения клиента"), "rule pop up type");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text
                    .Contains("Группа покупателя = CustomerGroup2"), "rule pop up filter");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text.Contains("test testov"),
                "rule pop up manager");
            VerifyAreEqual("название задачи - #LEAD_ID#, #NAME#, #PHONE#, #EMAIL#",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).GetAttribute("value"),
                "rule pop up task name");
            VerifyAreEqual("текст задачи - #LEAD_ID#, #NAME#, #PHONE#, #EMAIL#",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).GetAttribute("value"),
                "rule pop up task text");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateInput\"]")).Selected,
                "rule pop up task due time in use");
            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).GetAttribute("value"),
                "rule pop up task due time");

            IWebElement selectElemDueDateSelect =
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"));
            SelectElement select = new SelectElement(selectElemDueDateSelect);
            VerifyIsTrue(select.SelectedOption.Text.Contains("В часах"), "rule pop up task due time value");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskCreateDateInput\"]")).Selected,
                "rule pop up task create time not in use");

            IWebElement selectElemTaskPriority =
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"));
            select = new SelectElement(selectElemTaskPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Низкий"), "rule pop up task priority");

            IWebElement selectElemTaskProj = Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"));
            select = new SelectElement(selectElemTaskProj);
            VerifyIsTrue(select.SelectedOption.Text.Contains("По умолчанию"), "rule pop up task group");

            changeLeadStatus("26", "Ожидание решения клиента");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("название задачи - 26, LastName FirstName Patronymic, +7 495 800 200 01, mail@mail.com");

            VerifyAreEqual("название задачи - 26, LastName FirstName Patronymic, +7 495 800 200 01, mail@mail.com",
                Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Низкий", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("название задачи - 26, LastName FirstName Patronymic, +7 495 800 200 01, mail@mail.com",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("текст задачи - 26, LastName FirstName Patronymic, +7 495 800 200 01, mail@mail.com",
                "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("test testov"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Низкий"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №26", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains("Desc26"), "lead desc");

            IWebElement selectElemDealStatus = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElemDealStatus);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ожидание решения клиента"), "lead status");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct26"), "lead product");
            VerifyAreEqual("название задачи - 26, LastName FirstName Patronymic, +7 495 800 200 01, mail@mail.com",
                Driver.GetGridCell(0, "Name", "Tasks").Text, "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadStatusFilterSalesFunnel()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleSalesFunnel\"]")))).SelectByText(
                "SalesFunnel4");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Ожидание решения клиента");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Создан в части администрирования");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]")))).SelectByText(
                "Менеджер лида");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("Task name test SalesFunnel filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("Task text test SalesFunnel filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            //create lead from admin
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]"))
                .SendKeys("Lead from Admin with sales funnel filter task");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+79729272238");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(4, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadSalesFunnel\"]")))).SelectByText(
                "SalesFunnel4");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadManager\"]")))).SelectByText(
                "test testov");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            changeLeadStatus("121", "Ожидание решения клиента");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test SalesFunnel filter");

            VerifyAreEqual("Task name test SalesFunnel filter", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name test SalesFunnel filter",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text test SalesFunnel filter", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("test testov"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Средний"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(""), "lead desc");

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct5"), "lead product");
            VerifyAreEqual("Task name test SalesFunnel filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadStatusFilterOrganization()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleSalesFunnel\"]")))).SelectByText(
                "SalesFunnel3");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTypeStatus\"]")))).SelectByText(
                "Созвон с клиентом");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Организация");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Test Organization");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("Task name test Organization filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("Task text test Organization filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            /* create lead from admin */
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("lead with organization");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+79729299928");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadOrganization\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadOrganization\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadOrganization\"]")).SendKeys("Test Organization");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadSalesFunnel\"]")))).SelectByText(
                "SalesFunnel3");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(4, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            changeLeadStatus("121", "Созвон с клиентом");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test Organization filter");

            VerifyAreEqual("Task name test Organization filter", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name test Organization filter",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text test Organization filter", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Средний"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup1"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(""), "lead desc");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct5"), "lead product");
            VerifyAreEqual("Task name test Organization filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            VerifyAreEqual("Test Organization", Driver.FindElement(By.Id("Lead_Organization")).GetAttribute("value"),
                "lead's organization");

            Functions.CloseTab(Driver, BaseUrl);
        }

        public void changeLeadStatus(string leadId, string dealStatus)
        {
            GoToAdmin("leads#?leadIdInfo=" + leadId);
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"))))
                .SelectByText(dealStatus);
            Driver.WaitForToastSuccess();
        }
    }
}