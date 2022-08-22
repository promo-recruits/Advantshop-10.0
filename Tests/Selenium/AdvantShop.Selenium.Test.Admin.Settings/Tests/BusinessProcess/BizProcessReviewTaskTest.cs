using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BusinessProcess
{
    [TestFixture]
    public class SettingsBizProcessReviewTaskTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Catalog | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\BizProcessReview\\Catalog.Product.csv",
                "data\\Admin\\Settings\\BizProcessReview\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\BizProcessReview\\Catalog.Category.csv",
                "data\\Admin\\Settings\\BizProcessReview\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\BizProcessReview\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\BizProcessReview\\Customers.Customer.csv",
                "data\\Admin\\Settings\\BizProcessReview\\Customers.Departments.csv",
                "data\\Admin\\Settings\\BizProcessReview\\Customers.Managers.csv",
                "data\\Admin\\Settings\\BizProcessReview\\CRM.DealStatus.csv",
                "data\\Admin\\Settings\\BizProcessReview\\[Order].OrderSource.csv",
                "data\\Admin\\Settings\\BizProcessReview\\[Order].Lead.csv",
                "data\\Admin\\Settings\\BizProcessReview\\[Order].LeadCurrency.csv",
                "data\\Admin\\Settings\\BizProcessReview\\[Order].LeadItem.csv",
                "data\\Admin\\Settings\\BizProcessReview\\Customers.TaskGroup.csv",
                "data\\Admin\\Settings\\BizProcessReview\\Customers.Task.csv",
                "data\\Admin\\Settings\\BizProcessReview\\[Order].LeadEvent.csv",
                "data\\Admin\\Settings\\BizProcessReview\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\Settings\\BizProcessReview\\CRM.BizProcessRule.csv"
            );
            Init();
            Functions.AdminSettingsReviewsOn(Driver, BaseUrl);

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void ReviewAddFromClient()
        {
            GoToClient("products/test-product41");

            Driver.ScrollTo(By.Id("tabReviews"));
            Driver.FindElement(By.Id("tabReviews")).FindElement(By.TagName("a")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.Name("reviewsFormName")).Click();
            Driver.FindElement(By.Name("reviewsFormName")).Clear();
            Driver.FindElement(By.Name("reviewsFormName")).SendKeys("ReviewAuthor");

            Driver.FindElement(By.Name("reviewsFormEmail")).Click();
            Driver.FindElement(By.Name("reviewsFormEmail")).Clear();
            Driver.FindElement(By.Name("reviewsFormEmail")).SendKeys("ReviewAuthor@mail.ru");

            Driver.FindElement(By.Name("reviewFormText")).Click();
            Driver.FindElement(By.Name("reviewFormText")).Clear();
            Driver.FindElement(By.Name("reviewFormText")).SendKeys("Review Test Test");

            Driver.ScrollTo(By.Name("reviewsFormName"));
            Driver.FindElement(By.Name("reviewSubmit")).Click();
            Driver.WaitForToastSuccess();

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Новый отзыв");
            //Driver.DropFocus("h1");
            Driver.MouseFocus(By.CssSelector(".header-bottom-menu-link"));
            Driver.Blur();

            VerifyAreEqual("Новый отзыв", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Новый отзыв",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Текст задачи", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("test testov"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select2 = new SelectElement(selectElemPriority);
            VerifyIsTrue(select2.AllSelectedOptions[0].Text.Contains("Средний"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            SelectElement select3 = new SelectElement(selectElemGroup);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("TaskGroup1"), "task details group");

            //check review
            Driver.FindElement(By.CssSelector("[data-e2e=\"OpenReview\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".modal-header")).FindElement(By.TagName("h2")).Text
                    .Contains("Отзыв"), "review h2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-body")).Text.Contains("TestProduct41"),
                "review's product");
            VerifyAreEqual("ReviewAuthor",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).GetAttribute("value"),
                "review's author");
            VerifyAreEqual("ReviewAuthor@mail.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).GetAttribute("value"),
                "review's mail");
            Driver.AssertCkText("Review Test Test", "ReviewText");
        }

        [Test]
        public void ReviewAddFromAdmin()
        {
            GoToAdmin("reviews");

            Driver.FindElement(By.CssSelector("[data-e2e=\"reviewAdd\"]")).Click();
            Thread.Sleep(2000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewArtNo\"]")).SendKeys("42");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).SendKeys("ReviewAuthor from Admin");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).SendKeys("ReviewAuthorFromAdmin@mail.ru");
            Driver.SetCkText("текст отзыва", "ReviewText");


            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Thread.Sleep(2000);

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("Новый отзыв");
            //Driver.DropFocus("h1");
            Driver.MouseFocus(By.CssSelector(".header-bottom-menu-link"));
            Driver.Blur();

            VerifyAreEqual("Новый отзыв", Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Средний", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup1"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("Новый отзыв",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("Текст задачи", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("test testov"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select2 = new SelectElement(selectElemPriority);
            VerifyIsTrue(select2.AllSelectedOptions[0].Text.Contains("Средний"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            SelectElement select3 = new SelectElement(selectElemGroup);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("TaskGroup1"), "task details group");

            //check review
            Driver.FindElement(By.CssSelector("[data-e2e=\"OpenReview\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".modal-header")).FindElement(By.TagName("h2")).Text
                    .Contains("Отзыв"), "review h2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-body")).Text.Contains("TestProduct42"),
                "review's product");
            VerifyAreEqual("ReviewAuthor from Admin",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).GetAttribute("value"),
                "review's author");
            VerifyAreEqual("ReviewAuthorFromAdmin@mail.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).GetAttribute("value"),
                "review's mail");
            Driver.AssertCkText("текст отзыва", "ReviewText");
        }

        [Test]
        public void ReviewAddVariables()
        {
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"CallMissed\"]"));
            if (!Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridReviewAddedRules\"]")).Text
                .Contains("Правила не заданы"))

            {
                Driver.GetGridCell(0, "_serviceColumn", "ReviewAddedRules")
                    .FindElement(By.TagName("ui-grid-custom-delete")).Click();
                Driver.SwalConfirm();
            }

            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"ReviewAdded\"]")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).GetAttribute("innerText")
                    .Contains("Новый отзыв"), "biz rule type");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("BizRuleManager"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManager\"]")))).SelectByText(
                "test testov");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerOk\"]")).Click();
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]"))
                .SendKeys("название задачи - #NAME#, #EMAIL#");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]"))
                .SendKeys("текст задачи - #NAME#, #EMAIL#");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateEmul\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).SendKeys("10");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).SendKeys("2");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"))))
                .SelectByText("В днях");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"))))
                .SelectByText("Высокий");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]")))).SelectByText(
                "TaskGroup2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleOk\"]")).Click();
            Driver.WaitForToastSuccess();

            //check rule grid
            GoToAdmin("settingstasks");

            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"CallMissed\"]"));

            VerifyAreEqual("Новый отзыв", Driver.GetGridCell(0, "EventName", "ReviewAddedRules").Text,
                "biz rule grid type");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerFilterHTML", "ReviewAddedRules").Text,
                "biz rule grid manager");
            VerifyAreEqual("10 дней", Driver.GetGridCell(0, "TaskDueDateIntervalFormatted", "ReviewAddedRules").Text,
                "biz rule grid task due time");
            VerifyAreEqual("сразу", Driver.GetGridCell(0, "TaskCreateIntervalFormatted", "ReviewAddedRules").Text,
                "biz rule grid task interval");
            VerifyAreEqual("2", Driver.GetGridCell(0, "Priority", "ReviewAddedRules").Text, "biz rule grid priority");

            Driver.GetGridCell(0, "_serviceColumn", "ReviewAddedRules")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-body"));

            //check rule pop up
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleType\"]")).Text.Contains("Новый отзыв"),
                "rule pop up type");
            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRulePriority\"]")).GetAttribute("value"),
                "rule pop up task priority");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleManagerName\"]")).Text.Contains("test testov"),
                "rule pop up manager");
            VerifyAreEqual("название задачи - #NAME#, #EMAIL#",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskName\"]")).GetAttribute("value"),
                "rule pop up task name");
            VerifyAreEqual("текст задачи - #NAME#, #EMAIL#",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskText\"]")).GetAttribute("value"),
                "rule pop up task text");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateInput\"]")).Selected,
                "rule pop up task due time in use");
            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDate\"]")).GetAttribute("value"),
                "rule pop up task due time");

            IWebElement selectElemDueDateSelect =
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskDueDateSelect\"]"));
            SelectElement select = new SelectElement(selectElemDueDateSelect);
            VerifyIsTrue(select.SelectedOption.Text.Contains("В днях"), "rule pop up task due time value");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskCreateDateInput\"]")).Selected,
                "rule pop up task create time not in use");

            IWebElement selectElemTaskPriority =
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskPriority\"]"));
            SelectElement select1 = new SelectElement(selectElemTaskPriority);
            VerifyIsTrue(select1.SelectedOption.Text.Contains("Высокий"), "rule pop up task priority");

            IWebElement selectElemTaskProj = Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleTaskGroup\"]"));
            SelectElement select2 = new SelectElement(selectElemTaskProj);
            VerifyIsTrue(select2.SelectedOption.Text.Contains("TaskGroup2"), "rule pop up task group");

            //add review
            GoToClient("products/test-product43");

            Driver.ScrollTo(By.Id("tabReviews"));
            Driver.FindElement(By.Id("tabReviews")).FindElement(By.TagName("a")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.Name("reviewsFormName")).Click();
            Driver.FindElement(By.Name("reviewsFormName")).Clear();
            Driver.FindElement(By.Name("reviewsFormName")).SendKeys("TestReviewAuthorName");

            Driver.FindElement(By.Name("reviewsFormEmail")).Click();
            Driver.FindElement(By.Name("reviewsFormEmail")).Clear();
            Driver.FindElement(By.Name("reviewsFormEmail")).SendKeys("TestReviewAuthor@mail.ru");

            Driver.FindElement(By.Name("reviewFormText")).Click();
            Driver.FindElement(By.Name("reviewFormText")).Clear();
            Driver.FindElement(By.Name("reviewFormText")).SendKeys("Test Review Text");

            Driver.ScrollTo(By.Name("reviewsFormName"));
            Driver.FindElement(By.Name("reviewSubmit")).Click();
            Driver.WaitForToastSuccess();

            //check task added
            GoToAdmin("tasks");

            Driver.GridFilterSendKeys("название задачи - TestReviewAuthorName, TestReviewAuthor@mail.ru");
            Driver.MouseFocus(By.CssSelector(".header-bottom-menu-link"));
            Driver.Blur();

            VerifyAreEqual("название задачи - TestReviewAuthorName, TestReviewAuthor@mail.ru",
                Driver.GetGridCell(0, "Name").Text, "task added name");
            VerifyAreEqual("Высокий", Driver.GetGridCell(0, "PriorityFormatted").Text, "task added priority");
            VerifyAreEqual("В работе", Driver.GetGridCell(0, "StatusFormatted").Text, "task added status");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "Managers").Text, "task added assigned name");

            VerifyIsTrue(Driver.PageSource.Contains("TaskGroup2"), "task added group");

            //check task details
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("cke_editor1"));

            VerifyAreEqual("название задачи - TestReviewAuthorName, TestReviewAuthor@mail.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).GetAttribute("value"),
                "task details name");
            Driver.AssertCkText("текст задачи - TestReviewAuthorName, TestReviewAuthor@mail.ru", "editor1");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Text.Contains("test testov"),
                "task details user assigned to");

            IWebElement selectElemPriority = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"));
            SelectElement select4 = new SelectElement(selectElemPriority);
            VerifyIsTrue(select4.SelectedOption.Text.Contains("Высокий"), "task details priority");

            IWebElement selectElemGroup = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            SelectElement select5 = new SelectElement(selectElemGroup);
            VerifyIsTrue(select5.SelectedOption.Text.Contains("TaskGroup2"), "task details group");

            //check review
            Driver.FindElement(By.CssSelector("[data-e2e=\"OpenReview\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".modal-header")).FindElement(By.TagName("h2")).Text
                    .Contains("Отзыв"), "review h2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-body")).Text.Contains("TestProduct43"),
                "review's product");
            VerifyAreEqual("TestReviewAuthorName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).GetAttribute("value"),
                "review's author");
            VerifyAreEqual("TestReviewAuthor@mail.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).GetAttribute("value"),
                "review's mail");
            Driver.AssertCkText("Test Review Text", "ReviewText");
        }
    }
}