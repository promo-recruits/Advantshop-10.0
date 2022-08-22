using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TasksGrid
{
    [TestFixture]
    public class TasksAdd : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\Tasks\\Customers.TaskGroup.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("tasks");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void AddCorrectTask()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();
            Driver.WaitForModal();
            //name
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).SendKeys("NewTestTask");

            //Assigned  
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskAssigned\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskAssigned\"]"))
                .FindElement(
                    By.CssSelector(".ui-select-choices.ui-select-choices-content.ui-select-dropdown.dropdown-menu"))
                .FindElement(By.XPath("//div[contains(text(), 'test testov')]")).Click();

            //Despription
            Driver.SetCkText("Description NewTestTask", "editor1");
            //Group
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]")))).SelectByText("All");

            //Priority
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskPriority\"]"))))
                .SelectByText("Высокий");


            //duedate dueDate
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).SendKeys("08.12.2016 13:40");

            Driver.FindElement(By.CssSelector("[data-e2e=\"TaskAdd\"]")).Click();

            Refresh();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("NewTestTask"));
            VerifyAreEqual("Высокий", (Driver.GetGridCellElement(0, "PriorityFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("08.12.2016 13:40",
                (Driver.GetGridCellElement(0, "DueDateFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("В работе", (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("test testov", (Driver.GetGridCellElement(0, "Managers", by: By.TagName("div")).Text));
            VerifyAreEqual("Admin Ad", (Driver.GetGridCellElement(0, "AppointedName", by: By.TagName("div")).Text));

            Driver.GetGridCell(0, "Name").Click();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskName\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();


            VerifyIsTrue(Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                .Contains(DateTime.Today.ToString("dd.MM.yyyy")));
            VerifyAreEqual("Создана задача 2", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text));
            VerifyAreEqual("", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text));
            VerifyAreEqual("", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text));
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text));
        }


        [Test]
        public void EditTask()
        {
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();
            Driver.WaitForModal();
            //name
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).SendKeys("EditNewTestTask");
            //Assigned
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"] .close")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]"))
                .FindElement(
                    By.CssSelector(".ui-select-choices.ui-select-choices-content.ui-select-dropdown.dropdown-menu"))
                .FindElement(By.XPath("//div[contains(text(), 'Elena El')]")).Click();
            //Appointed
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAppointed\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"))
                .FindElement(
                    By.CssSelector(".ui-select-choices.ui-select-choices-content.ui-select-dropdown.dropdown-menu"))
                .FindElement(By.XPath("//div[contains(text(), 'test testov')]")).Click();

            //Group
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]")))).SelectByText("All");
            //Priority
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"))))
                .SelectByText("Средний");
            //Despription
            Driver.SetCkText("Edit Description NewTestTask", "editor1");
            //duedate dueDate
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskDuedate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskDuedate\"]")).SendKeys("12.12.2020 13:40");

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskButtonSave\"]")).Click();
            Driver.WaitForToastSuccess();
            Refresh();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("EditNewTestTask"));
            VerifyAreEqual("Средний", (Driver.GetGridCellElement(0, "PriorityFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("12.12.2020 13:40",
                (Driver.GetGridCellElement(0, "DueDateFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("В работе", (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("Elena El", (Driver.GetGridCellElement(0, "Managers", by: By.TagName("div")).Text));
            VerifyAreEqual("test testov", (Driver.GetGridCellElement(0, "AppointedName", by: By.TagName("div")).Text));

            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AssignedToMe\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn-cancel")).Click();

            Refresh();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("EditNewTestTask"));
            VerifyAreEqual("Admin Ad", (Driver.GetGridCellElement(0, "Managers", by: By.TagName("div")).Text));
        }

        [Test]
        public void EditsTaskCandel()
        {
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();

            //name

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).SendKeys("EditCancelNewTestTask");


            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            Driver.SwalCancel();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed);

            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            Driver.SwalConfirm();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog")).Count == 0);

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("NewTestTask"));
            VerifyIsFalse(Driver.PageSource.Contains("EditCancelNewTestTask"));
        }

        [Test]
        public void NewTaskAssignedToMe()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();
            Driver.WaitForModal();
            //name
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).SendKeys("NewTaskAssignedToMe");
            //duedate dueDate
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).SendKeys("08.12.2020 13:40");

            //Assigned  
            Driver.FindElement(By.CssSelector("[data-e2e=\"AssignedToMe\"]")).Click();
            //Despription
            Driver.SetCkText("Description NewTaskAssignedToMe", "editor1");
            //Group
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]")))).SelectByText("All");

            //Priority
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskPriority\"]"))))
                .SelectByText("Низкий");


            Driver.FindElement(By.CssSelector("[data-e2e=\"TaskAdd\"]")).Click();

            Driver.WaitForModalClose();

            Refresh();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("NewTaskAssignedToMe"));
            VerifyAreEqual("Низкий", (Driver.GetGridCellElement(0, "PriorityFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("08.12.2020 13:40",
                (Driver.GetGridCellElement(0, "DueDateFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("В работе", (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("Admin Ad", (Driver.GetGridCellElement(0, "Managers", by: By.TagName("div")).Text));
            VerifyAreEqual("Admin Ad", (Driver.GetGridCellElement(0, "AppointedName", by: By.TagName("div")).Text));
        }
    }
}