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
    public class SettingsBizProcessLeadAddRuleTest : SettingsLeadFunctions
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
        public void LeadAddRuleFilterSum()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).GetAttribute("innerText")
                    .Contains("Новый лид"), "biz rule type");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Бюджет");

            Driver.FindElement(By.LinkText("Указать диапазон")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueFrom\"]")).SendKeys("170");

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
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).SendKeys("Task name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).SendKeys("Task text");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateEmul\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).SendKeys("6");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"))))
                .SelectByText("В днях");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskCreateDateEmul\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskCreateDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskCreateDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskCreateDate\"]")).SendKeys("5");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskCreateDateSelect\"]"))))
                .SelectByText("В минутах");

            IWebElement selectElemCreateDate =
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskCreateDateSelect\"]"));
            SelectElement select = new SelectElement(selectElemCreateDate);
            VerifyIsTrue(select.SelectedOption.Text.Contains("В минутах"), "task add create date select");
            VerifyAreEqual("5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskCreateDate\"]")).GetAttribute("value"),
                "task add create date");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Высокий");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskCreateDateEmul\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            //check rules grid
            GoToAdmin("settingstasks");

            VerifyAreEqual("Новый лид", Driver.GetGridCell(0, "EventName", "LeadCreatedRules").Text,
                "biz rule grid type");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerFilterHTML", "LeadCreatedRules").Text,
                "biz rule grid manager");
            VerifyAreEqual("6 дней", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "LeadCreatedRules").Text,
                "biz rule grid task due time");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "LeadCreatedRules").Text,
                "biz rule grid task interval");
            VerifyAreEqual("0", Driver.GetGridCell(0, "Priority", "LeadCreatedRules").Text, "biz rule grid priority");

            setLeadBuyInOneClick();

            //client
            ReInitClient();
            leadBuyOneClick("100", "Task filter sum", "71231231122");

            //check task added
            ReInit();
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyAreEqual("Task name", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Высокий", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Task name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Task text", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            select = new SelectElement(selectElemPriority);
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

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Elena El"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct100"), "lead product");
            VerifyAreEqual("Task name", Driver.GetGridCell(0, "Name", "Tasks").Text, "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadAddFilterFIO()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText("Имя");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Имя");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Фамилия");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Фамилия");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Отчество");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Отчество");

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

            //create lead
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("Имя");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).SendKeys("Фамилия");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).SendKeys("Отчество");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+79729272727");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("lead-info-content"));

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test FIO filter");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

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

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(""), "lead desc");

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Elena El"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct1"), "lead product");
            VerifyAreEqual("Task name test FIO filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadAddFilterEmail()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Email");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Email@Email.Email");

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

            /* create lead from client */
            setLeadBuyInOneClick();
            setBuyOneClickFieldEmail();

            //client
            ReInitClient();
            leadBuyOneClick("100", "Task filter email", "71111111111", "Email@Email.Email");

            //check task added
            ReInit();
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test Email filter");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

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

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(""), "lead desc");

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Elena El"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct100"), "lead product");
            VerifyAreEqual("Task name test Email filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadAddFilterPhone()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

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
            leadBuyOneClick("100", "Task filter Phone", "79999999999");

            //check task added
            ReInit();
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test Phone filter");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

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

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Elena El"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct100"), "lead product");
            VerifyAreEqual("Task name test Phone filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadAddFilterCustomerGroup()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Группа покупателя");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("Обычный покупатель");

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

            /* create lead from client */
            setLeadBuyInOneClick();

            //client
            ReInitClient();
            leadBuyOneClick("100", "Task filter CustomerGroup", "75555555555");

            //check task added
            ReInit();
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test CustomerGroup filter");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

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

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(""), "lead desc");

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Elena El"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct100"), "lead product");
            VerifyAreEqual("Task name test CustomerGroup filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadAddFilterSource()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Источник");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("Мобильная версия");

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

            /* create lead from client */
            setLeadBuyInOneClick();

            GoToAdmin("settings/mobileversion");
            Driver.CheckBoxUncheck("IsFullCheckout");
            Driver.ScrollToTop();
            if (Driver.FindElements(By.CssSelector("[data-e2e=\"mobileSave\"][disabled]")).Count == 0)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"mobileSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            //client
            ReInitClient();
            GoToMobile("products/test-product80");
            Refresh();

            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm")).Click();
            Driver.WaitForElem(By.ClassName("swal2-title"));
            GoToMobile("cart");
            Driver.FindElement(By.CssSelector(".cart-full-mobile-btn")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.Name("Name")).Click();
            Driver.FindElement(By.Name("Name")).Clear();
            Driver.FindElement(By.Name("Name")).SendKeys("Task filter Source");

            Driver.FindElement(By.Name("Phone")).Click();
            Driver.FindElement(By.Name("Phone")).SendKeys("77777777777");

            Driver.FindElement(By.CssSelector(".btn.btn-confirm")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));

            //check task added
            ReInit();
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test Source filter");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

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

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(""), "lead desc");

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Elena El"), "lead manager");

            IWebElement selectElemLeadSource = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            select = new SelectElement(selectElemLeadSource);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Мобильная версия"), "lead source");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct80"), "lead product");
            VerifyAreEqual("Task name test Source filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadAddFilterCountryRegionCity()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

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
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Ульяновская область");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Город");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Ульяновск");

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

            // create lead from client 
            setLeadBuyInOneClick();

            //client
            ReInitClient();
            Functions.LogCustomer(Driver, BaseUrl, customerId: "cfc2c33b-1e84-415e-8482-e98156341604",
                auth:
                "6DE952141E601E94D0F1A4C8FCBF2090DAB02038F9EE9BF3773A9A5BBB33F25A6ED56D08C644CA6F7937CF1D5667EB57FF60A9AE4190124F4A2A549DC7A2038A82CD3C6BC25D4E0D31BF579E778C39BBE29F536FCDA73FC9BAF09F1EAAB04F7CCEBD423500D1E23724E5078F0EACD30DEA39C78EDA1DA82078776CB59E080D6C66E0ACD1D5B3C0A1742C2C2EC58BDDC85FDC33A0");

            GoToClient("products/test-product28");
            Functions.SetCity(Driver, "Ульяновск");

            Driver.ScrollTo(By.CssSelector("[data-product-id=\"28\"]"));
            Driver.FindElement(By.LinkText("Купить в один клик")).Click();
            Driver.WaitForElem(By.ClassName("buy-one-click-dialog"));

            Driver.FindElement(By.CssSelector("[value=\"Заказать\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));

            //check task added
            ReInit();
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test CountryRegionCity filter");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

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

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(""), "lead desc");

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Elena El"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct28"), "lead product");
            VerifyAreEqual("Task name test CountryRegionCity filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }


        [Test]
        public void LeadAddFilterCreateFromAdmin()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

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

            VerifyAreEqual("Новый лид", Driver.GetGridCell(0, "EventName", "LeadCreatedRules").Text,
                "biz rule grid type");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerFilterHTML", "LeadCreatedRules").Text,
                "biz rule grid manager");
            VerifyAreEqual("1 час", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "LeadCreatedRules").Text,
                "biz rule grid task due time");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "LeadCreatedRules").Text,
                "biz rule grid task interval");
            VerifyAreEqual("0", Driver.GetGridCell(0, "Priority", "LeadCreatedRules").Text, "biz rule grid priority");

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
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(4, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("lead-info-content"));

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name lead from admin");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

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

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct5"), "lead product");
            VerifyAreEqual("Task name lead from admin", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }


        [Test]
        public void LeadAddFilterDesc()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]"))))
                .SelectByText("Описание");

            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("input")).Click();
            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("input")).Clear();
            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("input")).SendKeys("Описание");

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

            VerifyAreEqual("Новый лид", Driver.GetGridCell(0, "EventName", "LeadCreatedRules").Text,
                "biz rule grid type");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerFilterHTML", "LeadCreatedRules").Text,
                "biz rule grid manager");
            VerifyAreEqual("1 час", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "LeadCreatedRules").Text,
                "biz rule grid task due time");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "LeadCreatedRules").Text,
                "biz rule grid task interval");
            VerifyAreEqual("0", Driver.GetGridCell(0, "Priority", "LeadCreatedRules").Text, "biz rule grid priority");

            //create lead from admin
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("test lead with description");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+79729272729");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).SendKeys("Описание");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("lead-info-content"));

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name lead with description");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

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

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains("Описание"), "lead desc");

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct1"), "lead product");
            VerifyAreEqual("Task name lead with description", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadAddFilterCreateFromAdminValueNot()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

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

            VerifyAreEqual("Новый лид", Driver.GetGridCell(0, "EventName", "LeadCreatedRules").Text,
                "biz rule grid type");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerFilterHTML", "LeadCreatedRules").Text,
                "biz rule grid manager");
            VerifyAreEqual("1 час", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "LeadCreatedRules").Text,
                "biz rule grid task due time");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "LeadCreatedRules").Text,
                "biz rule grid task interval");
            VerifyAreEqual("0", Driver.GetGridCell(0, "Priority", "LeadCreatedRules").Text, "biz rule grid priority");

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
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+79729272729");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(4, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("lead-info-content"));

            //check task not added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name lead from admin - not");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created");
        }

        [Test]
        public void LeadAddFilterVariables()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Группа покупателя");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("Обычный покупатель");

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
            Driver.GetGridCell(0, "_serviceColumn", "LeadCreatedRules")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text.Contains("Новый лид"),
                "rule pop up type");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilter\"]")).Text
                    .Contains("Группа покупателя = Обычный покупатель"), "rule pop up filter");
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

            /* create lead from client */
            setLeadBuyInOneClick();
            setBuyOneClickFieldEmail();

            //client
            ReInitClient();
            leadBuyOneClick("101", "CustomerName", "71234567890", "Customer@Email.test");

            //check task added
            ReInit();
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("название задачи - 121, CustomerName, 71234567890, Customer@Email.test");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyAreEqual("название задачи - 121, CustomerName, 71234567890, Customer@Email.test",
                Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Низкий", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("название задачи - 121, CustomerName, 71234567890, Customer@Email.test",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("текст задачи - 121, CustomerName, 71234567890, Customer@Email.test", "editor1");

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

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyIsTrue(Driver.FindElement(By.Id("Lead_Description")).Text.Contains(""), "lead desc");

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct101"), "lead product");
            VerifyAreEqual("название задачи - 121, CustomerName, 71234567890, Customer@Email.test",
                Driver.GetGridCell(0, "Name", "Tasks").Text, "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadAddFilterSalesFunnel()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleParamValueOk"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")))).SelectByText(
                "Список лидов");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValueSelect\"]"))))
                .SelectByText("SalesFunnel5");

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
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(4, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadSalesFunnel\"]")))).SelectByText(
                "SalesFunnel5");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("lead-info-content"));

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test SalesFunnel filter");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyAreEqual("Task name test SalesFunnel filter", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

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

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Elena El"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct5"), "lead product");
            VerifyAreEqual("Task name test SalesFunnel filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);

            //check correct lead's sales funnel
            GoToAdmin("leads?salesFunnelId=3");
            Driver.GetGridFilterTab(0, "sales funnel");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead's sales funnel no 1");

            GoToAdmin("leads?salesFunnelId=1");
            Driver.GetGridFilterTab(0, "sales funnel");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead's sales funnel no 2");

            GoToAdmin("leads?salesFunnelId=5");
            Driver.GetGridFilterTab(0, "sales funnel");
            VerifyAreEqual("Lead from Admin with sales funnel filter task", Driver.GetGridCell(0, "FullName").Text,
                "lead's sales funnel");
        }

        [Test]
        public void LeadAddFilterOrganization()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]")).Click();
            Driver.WaitForModal();

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

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(4, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("lead-info-content"));

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test Organization filter");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

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

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Elena El"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct5"), "lead product");
            VerifyAreEqual("Task name test Organization filter", Driver.GetGridCell(0, "Name", "Tasks").Text,
                "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            VerifyAreEqual("Test Organization", Driver.FindElement(By.Id("Lead_Organization")).GetAttribute("value"),
                "lead's organization");

            Functions.CloseTab(Driver, BaseUrl);
        }


        public void setBuyOneClickFieldEmail()
        {
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");

            if (!Driver.FindElement(By.Name("IsShowBuyInOneClickEmail")).Selected)

            {
                Driver.ScrollTo(By.Name("CustomShippingField3"));
                Driver.FindElement(By.CssSelector("[data-e2e=\"IsShowBuyInOneClickEmail\"]")).Click();

                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
        }
    }
}