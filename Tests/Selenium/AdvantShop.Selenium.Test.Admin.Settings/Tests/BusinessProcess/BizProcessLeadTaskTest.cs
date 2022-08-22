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
    public class SettingsBizProcessLeadTaskTest : BaseMultiSeleniumTest
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
                "data\\Admin\\Settings\\BizProcessLead\\CRM.BizProcessRule.csv",
                "data\\Admin\\Settings\\BizProcessLead\\[Order].LeadEvent.csv"
            );

            Init();
            Functions.EnableCapcha(Driver, BaseUrl);

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        /* positive tests */
        [Test]
        public void LeadAddFromAdmin()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("lead from admin");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+723177712923");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory2");

            VerifyAreEqual("120 руб.", Driver.GetGridCell(0, "PriceFormatted", "OffersSelectvizr").Text,
                "sum according to biz rule");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");
            Thread.Sleep(500);

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Новый лид");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyAreEqual("Новый лид", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Новый лид",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Текст задачи", "editor1");

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

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct21"), "lead product");
            VerifyAreEqual("Новый лид", Driver.GetGridCell(0, "Name", "Tasks").Text, "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadAddFromClient()
        {
            //set create lead when buy in one click
            GoToAdmin("settingscheckout#?checkoutTab=common");

            IWebElement selectElem = Driver.FindElement(By.Name("BuyInOneClickAction"));
            SelectElement select = new SelectElement(selectElem);

            if (!select.SelectedOption.Text.Contains("Создавать лид"))

            {
                Driver.ScrollTo(By.Name("BuyInOneClick"));
                (new SelectElement(Driver.FindElement(By.Name("BuyInOneClickAction")))).SelectByText("Создавать лид");

                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector(".btn.btn-sm.btn-success.btn--margin-left")).Click();
                Driver.WaitForToastSuccess();
            }

            //client
            GoToClient("products/test-product22");

            Driver.ScrollTo(By.CssSelector("[data-product-id=\"22\"]"));
            Driver.FindElement(By.LinkText("Купить в один клик")).Click();
            Driver.WaitForElem(By.ClassName("buy-one-click-dialog"));
            Driver.FindElement(By.CssSelector("[value=\"Заказать\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Новый лид");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyAreEqual("Новый лид", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Новый лид",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Текст задачи", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("test testov"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            select = new SelectElement(selectElemPriority);
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

            IWebElement selectElemManager = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "lead manager");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct22"), "lead product");
            VerifyAreEqual("Новый лид", Driver.GetGridCell(0, "Name", "Tasks").Text, "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void LeadChangeDealStatus()
        {
            GoToAdmin("leads?#?leadIdInfo=100");

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Сделка заключена"), "pre check lead deal status");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]")))).SelectByText(
                "Ожидание решения клиента");
            Driver.WaitForToastSuccess();

            GoToAdmin("leads?#?leadIdInfo=100");

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ожидание решения клиента"), "lead new deal status saved");

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Смена этапа лида");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyAreEqual("Смена этапа лида", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Низкий", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup2"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Смена этапа лида",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Текст задачи", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("Elena El"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            select = new SelectElement(selectElemPriority);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Низкий"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            select = new SelectElement(selectElemGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("TaskGroup2"), "task details group");

            //check lead
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadLink\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("lead-info-content"));
            ReadOnlyCollection<String> windowHandles = Driver.WindowHandles;
            VerifyIsTrue(windowHandles.Count == 2, "2 tabs - tasks and lead");

            VerifyAreEqual("Лид №100", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead title");
            VerifyAreEqual("Desc100", Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value"),
                "lead description");

            IWebElement selectElemLeadStatus = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElemLeadStatus);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ожидание решения клиента"), "lead status");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct100"), "lead product");
            VerifyAreEqual("Смена этапа лида", Driver.GetGridCell(0, "Name", "Tasks").Text, "lead's task name");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted", "Tasks").Text, "lead's task status");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "Managers", "Tasks").Text, "lead's task assigned name");

            Functions.CloseTab(Driver, BaseUrl);
        }

        /* negative tests */
        [Test]
        public void LeadAddFilterOrganizationInappropRuleCond()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));

            if (!Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadCreatedRules\"]")).Text
                .Contains("Правила не заданы"))
            {
                Driver.GetGridCell(0, "_serviceColumn", "LeadCreatedRules")
                    .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
                Driver.SwalConfirm();
            }

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
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadOrganization\"]")).SendKeys("Organization");

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

            Driver.GridFilterSendKeys("Task name test Organization filter");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created");
        }


        [Test]
        public void LeadStatusFilterOrganizationInappropRuleCond()
        {
            GoToAdmin("settingstasks");
            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));

            if (!Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadStatusChangedRules\"]")).Text
                .Contains("Правила не заданы"))
            {
                Driver.GetGridCell(0, "_serviceColumn", "LeadStatusChangedRules")
                    .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
                Driver.SwalConfirm();
            }

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
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParamValue\"]")).SendKeys("Test");

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
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadOrganization\"]")).SendKeys("Organization");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadSalesFunnel\"]")))).SelectByText(
                "SalesFunnel3");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(4, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("lead-info-content"));

            GoToAdmin("leads#?leadIdInfo=121");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]")))).SelectByText(
                "Созвон с клиентом");
            Driver.WaitForToastSuccess();

            //check task not added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test Organization filter");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created");
        }

        [Test]
        public void LeadAddFromAdminInappropRuleCond()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]"))
                .SendKeys("lead from admin Inappropriate");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+723177745623");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            VerifyAreEqual("103 руб.", Driver.GetGridCell(3, "PriceFormatted", "OffersSelectvizr").Text,
                "sum not according to biz rule");
            Driver.GetGridCell(3, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");
            Thread.Sleep(500);

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check task not added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Новый лид");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created");
        }

        [Test]
        public void LeadAddFromClientInappropRuleCond()
        {
            //set create lead when buy in one click
            GoToAdmin("settingscheckout#?checkoutTab=common");

            IWebElement selectElem = Driver.FindElement(By.Name("BuyInOneClickAction"));
            SelectElement select = new SelectElement(selectElem);

            if (!select.SelectedOption.Text.Contains("Создавать лид"))

            {
                Driver.ScrollTo(By.Name("BuyInOneClick"));
                (new SelectElement(Driver.FindElement(By.Name("BuyInOneClickAction")))).SelectByText("Создавать лид");

                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector(".btn.btn-sm.btn-success.btn--margin-left")).Click();
                Driver.WaitForToastSuccess();
            }

            //client
            GoToClient("products/test-product5");

            Driver.ScrollTo(By.CssSelector("[data-product-id=\"5\"]"));
            Driver.FindElement(By.LinkText("Купить в один клик")).Click();
            Driver.WaitForElem(By.ClassName("buy-one-click-dialog"));
            Driver.FindElement(By.CssSelector("[value=\"Заказать\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));

            //check task not added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Новый лид");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created");
        }

        [Test]
        public void LeadChangeDealStatusInappropRuleCond()
        {
            GoToAdmin("leads?#?leadIdInfo=101");

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Сделка заключена"), "pre check lead deal status");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]")))).SelectByText(
                "Созвон с клиентом");
            Driver.WaitForToastSuccess();

            GoToAdmin("leads?#?leadIdInfo=101");

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Созвон с клиентом"), "lead new deal status saved");

            //check task not added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Смена этапа лида");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created");
        }

        [Test]
        public void LeadAddFunnelInappropRuleCond()
        {
            GoToAdmin("settings/common#?indexTab=feedback");
            (new SelectElement(Driver.FindElement(By.Id("FeedbackAction")))).SelectByText("Создавать лид");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"] input")).Click();

            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"OrderStatusChanged\"]"));

            if (!Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadCreatedRules\"]")).Text
                .Contains("Правила не заданы"))
            {
                Driver.GetGridCell(0, "_serviceColumn", "LeadCreatedRules")
                    .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
                Driver.SwalConfirm();
            }

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
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).SendKeys("New Lead Sales Funnel");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).SendKeys("New Lead Sales Funnel text");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            //create lead from client
            ReInitClient();
            GoToClient("feedback");
            Refresh();
            Driver.FindElement(By.Id("Message")).Click();
            Driver.FindElement(By.Id("Message")).Clear();
            Driver.FindElement(By.Id("Message")).SendKeys("feedback Message");
            Driver.ScrollTo(By.Id("Name"));
            Driver.FindElement(By.Id("Name")).Click();
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("Test Name");
            Driver.FindElement(By.Id("Email")).Click();
            Driver.FindElement(By.Id("Email")).Clear();
            Driver.FindElement(By.Id("Email")).SendKeys("TestEmail@gmail.com");
            Driver.FindElement(By.Id("Phone")).Click();
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89345263412");

            Driver.ScrollTo(By.CssSelector(".btn.btn-submit.btn-middle"));
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Driver.WaitForElem(By.ClassName("feedbackSuccess-block"));

            ReInit();

            //check task not added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("New Lead Sales Funnel");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created");

            //check correct lead's sales funnel
            GoToAdmin("leads?salesFunnelId=3");
            Driver.GetGridFilterTab(0, "Test Name");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead's sales funnel no 1");

            GoToAdmin("leads?salesFunnelId=5");
            Driver.GetGridFilterTab(0, "Test Name");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead's sales funnel no 2");

            GoToAdmin("leads?salesFunnelId=1");
            Driver.GetGridFilterTab(0, "Test Name");
            VerifyAreEqual("Test Name", Driver.GetGridCell(0, "FullName").Text, "lead's default sales funnel");
        }

        [Test]
        public void LeadStatusSalesFunnelInappropRuleCond()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));

            if (!Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadStatusChangedRules\"]")).Text
                .Contains("Правила не заданы"))
            {
                Driver.GetGridCell(0, "_serviceColumn", "LeadStatusChangedRules")
                    .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
                Driver.SwalConfirm();
            }

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
                .SendKeys("Task name test SalesFunnel change status filter");

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

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadManager\"]")))).SelectByText(
                "test testov");

            //change deal status
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("lead-info-content"));

            GoToAdmin("leads#?leadIdInfo=121");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]")))).SelectByText(
                "Ожидание решения клиента");
            Driver.WaitForToastSuccess();

            //check task not added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task name test SalesFunnel change status filter");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created");

            //check correct lead's sales funnel
            GoToAdmin("leads?salesFunnelId=4");
            Driver.GetGridFilterTab(0, "Lead from Admin with sales funnel filter task");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead's sales funnel no 1");

            GoToAdmin("leads?salesFunnelId=5");
            Driver.GetGridFilterTab(0, "Lead from Admin with sales funnel filter task");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead's sales funnel no 2");

            GoToAdmin("leads?salesFunnelId=1");
            Driver.GetGridFilterTab(0, "Lead from Admin with sales funnel filter task");
            VerifyAreEqual("Lead from Admin with sales funnel filter task", Driver.GetGridCell(0, "FullName").Text,
                "lead's default sales funnel");
        }

        [Test]
        public void LeadStatusSalesFunnelDealInappropRuleCond()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));

            if (!Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadStatusChangedRules\"]")).Text
                .Contains("Правила не заданы"))
            {
                Driver.GetGridCell(0, "_serviceColumn", "LeadStatusChangedRules")
                    .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
                Driver.SwalConfirm();
            }

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadStatusChangedRules\"]")).Text
                    .Contains("Правила не заданы"), "rule deleted");

            GoToAdmin("settingstasks");
            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"LeadCreated\"]"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadStatusChangedRules\"]")).Text
                    .Contains("Правила не заданы"), "rule deleted after refreshing");

            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"LeadStatusChanged\"]")).Click();
            Driver.WaitForModal();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleSalesFunnel\"]")))).SelectByText(
                "SalesFunnel1");
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
                "Наиболее свободный");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("Task The same funnel, deal status different");

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
                .SendKeys("Lead The same funnel, deal status different");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+79729272288");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForElem(By.TagName("offers-selectvizr"));

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(4, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadManager\"]")))).SelectByText(
                "test testov");

            //change deal status
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(By.ClassName("lead-info-content"));

            GoToAdmin("leads#?leadIdInfo=121");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]")))).SelectByText(
                "Выставление КП");
            Driver.WaitForToastSuccess();

            //check task not added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Task The same funnel, deal status different");
            Driver.MouseFocus(By.CssSelector(".sticky-page-name-inner"));
            //Driver.DropFocus("h1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no task created");

            //check correct lead's sales funnel
            GoToAdmin("leads?salesFunnelId=4");
            Driver.GetGridFilterTab(0, "Lead The same funnel, deal status different");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead's sales funnel no 1");

            GoToAdmin("leads?salesFunnelId=5");
            Driver.GetGridFilterTab(0, "Lead The same funnel, deal status different");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead's sales funnel no 2");

            GoToAdmin("leads?salesFunnelId=1");
            Driver.GetGridFilterTab(0, "Lead The same funnel, deal status different");
            VerifyAreEqual("Lead The same funnel, deal status different", Driver.GetGridCell(0, "FullName").Text,
                "lead's default sales funnel");
        }
    }
}