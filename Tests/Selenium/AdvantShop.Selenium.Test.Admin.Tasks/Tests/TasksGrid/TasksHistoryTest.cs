using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TasksGrid
{
    [TestFixture]
    public class TasksHistoryTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\HistoryTask\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\HistoryTask\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\HistoryTask\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\HistoryTask\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\HistoryTask\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\HistoryTask\\Customers.Task.csv",
                "data\\Admin\\Tasks\\HistoryTask\\Customers.TaskManager.csv",
                "data\\Admin\\Tasks\\HistoryTask\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\Tasks\\HistoryTask\\CMS.ChangeHistory.csv"
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
        public void OpenTasksCart()
        {
            GoToAdmin("tasks#?viewed=true&modal=2");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text.Contains("22.01.2018"),
                "date");
            VerifyAreEqual("Исполнитель", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event");
            VerifyAreEqual("", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text), "NewValue");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[grid-unique-id=\"gridChangeHistory\"]")).Count == 0,
                "hide ui-grid");
        }

        [Test]
        public void ChangeName()
        {
            GoToAdmin("tasks");
            Driver.GetGridCellElement(0, "Name", by: By.TagName("a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elements");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskName\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).SendKeys("NewTestTask");

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskButtonSave\"]")).Click();

            Refresh();
            Driver.GridFilterSendKeys("NewTestTask");

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("NewTestTask"));

            Driver.GetGridCellElement(0, "Name", by: By.TagName("a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 1, "count elem");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date");
            VerifyAreEqual("Название", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event");
            VerifyAreEqual("test1", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue");
            VerifyAreEqual("NewTestTask", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text), "NewValue");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed");
        }

        [Test]
        public void ChangeDescription()
        {
            GoToAdmin("tasks");
            Driver.GetGridCellElement(2, "Name", by: By.TagName("a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elements");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskName\"]"));
            Driver.SetCkText("Edit Description NewTestTask", "editor1");


            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskButtonSave\"]")).Click();

            Refresh();

            Driver.GetGridCellElement(2, "Name", by: By.TagName("a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 1, "count elem");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date");
            VerifyAreEqual("Описание", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event");
            VerifyAreEqual("new test3", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue");
            VerifyAreEqual("Edit Description NewTestTask", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text),
                "NewValue");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed");
        }


        [Test]
        public void ChangeAppointed()
        {
            GoToAdmin("tasks");
            Driver.GetGridCellElement(3, "Name", by: By.TagName("a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elements");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskName\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAppointed\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"))
                .FindElement(
                    By.CssSelector(".ui-select-choices.ui-select-choices-content.ui-select-dropdown.dropdown-menu"))
                .FindElement(By.XPath("//div[contains(text(), 'test testov')]")).Click();


            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskButtonSave\"]")).Click();

            Refresh();

            Driver.GetGridCellElement(3, "Name", by: By.TagName("a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 1, "count elem");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date");
            VerifyAreEqual("Постановщик", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue");
            VerifyAreEqual("test testov", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text), "NewValue");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed");
        }

        [Test]
        public void ChangeAssigned()
        {
            GoToAdmin("tasks");
            Driver.GetGridCellElement(4, "Name", by: By.TagName("a")).Click();


            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"] input")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]"))
                .FindElement(
                    By.CssSelector(".ui-select-choices.ui-select-choices-content.ui-select-dropdown.dropdown-menu"))
                .FindElement(By.XPath("//div[contains(text(), 'Elena El')]")).Click();


            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 1, "count elem");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date");
            VerifyAreEqual("Исполнитель", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue");
            VerifyAreEqual("Admin Ad, Elena El", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text), "NewValue");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed");

            //   driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"] .close")).Click();
        }

        [Test]
        public void ChangePriority()
        {
            GoToAdmin("tasks#?viewed=true&modal=6");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elements");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskName\"]"));
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"))))
                .SelectByText("Низкий");

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskButtonSave\"]")).Click();

            Refresh();
            GoToAdmin("tasks#?viewed=true&modal=6");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 1, "count elem");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date");
            VerifyAreEqual("Приоритет", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event");
            VerifyAreEqual("Высокий", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue");
            VerifyAreEqual("Низкий", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text), "NewValue");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed");
        }

        [Test]
        public void ChangeDuedate()
        {
            GoToAdmin("tasks#?viewed=true&modal=8");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elements");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskName\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskDuedate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskDuedate\"]")).SendKeys("12.12.2020 13:40");
            //remove focus from date field
            Driver.DropFocus("h2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskButtonSave\"]")).Click();

            Refresh();
            GoToAdmin("tasks#?viewed=true&modal=8");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 1, "count elem");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date");
            VerifyAreEqual("Срок исполнения", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event");
            VerifyAreEqual("05.11.2016 13:58:00", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text),
                "OldValue");
            VerifyAreEqual("12.12.2020 13:40:00", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text),
                "NewValue");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed");
        }

        [Test]
        public void ChangeProject()
        {
            GoToAdmin("tasks#?viewed=true&modal=7");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elements");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskName\"]"));
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]")))).SelectByText(
                "test group 3");


            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskButtonSave\"]")).Click();

            GoToAdmin("projects/3#?viewed=true&modal=7");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 1, "count elem");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date");
            VerifyAreEqual("Проект", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event");
            VerifyAreEqual("test group 1", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue");
            VerifyAreEqual("test group 3", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text), "NewValue");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed");
        }

        [Test]
        public void ChangeStatus()
        {
            GoToAdmin("tasks#?viewed=true&modal=22");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elements");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskStatusInprogress\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusInprogress\"]")).Click();

            Refresh();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 1, "count elem 1");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date 1");
            VerifyAreEqual("Статус", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event 1");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue 1");
            VerifyAreEqual("В работе (выполняется)", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text),
                "NewValue 1");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed 1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskStatusStop\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusStop\"]")).Click();

            Refresh();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 2, "count elem 2");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date 2");
            VerifyAreEqual("Статус", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event 2");
            VerifyAreEqual("В работе (выполняется)", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text),
                "OldValue 2");
            VerifyAreEqual("В работе", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text), "NewValue 2");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed 2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskStatusCompleted\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusCompleted\"]")).Click();
            Driver.FindElement(By.CssSelector(".form-group input")).SendKeys("Rezult");
            Driver.FindElement(By.CssSelector(".ladda-label")).Click();


            GoToAdmin("tasks#?viewed=true&modal=22");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 4, "count elem 3");

            //когда отправляем несколько запросов сразу, как при изменении статуса с записью о результате,
            //контроллер может обработать их в разном порядке
            //т.е. 3 и 4 записи могут меняться местами
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date 3");
            if (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text == "Статус")
            {
                VerifyAreEqual("Статус", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event3");
                VerifyAreEqual("В работе", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue 3");
                VerifyAreEqual("Завершена", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text), "NewValue 3");
                VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed 3");

                VerifyAreEqual("Результат", (Driver.GetGridCell(1, "ParameterName", "ChangeHistory").Text), "event 4");
                VerifyAreEqual("", (Driver.GetGridCell(1, "OldValue", "ChangeHistory").Text), "OldValue 4");
                VerifyAreEqual("Rezult", (Driver.GetGridCell(1, "NewValue", "ChangeHistory").Text), "NewValue 4");
                VerifyAreEqual("Admin Ad", (Driver.GetGridCell(1, "ChangedByName", "ChangeHistory").Text), "Changed 4");
            }
            else
            {
                VerifyAreEqual("Статус", (Driver.GetGridCell(1, "ParameterName", "ChangeHistory").Text), "event3");
                VerifyAreEqual("В работе", (Driver.GetGridCell(1, "OldValue", "ChangeHistory").Text), "OldValue 3");
                VerifyAreEqual("Завершена", (Driver.GetGridCell(1, "NewValue", "ChangeHistory").Text), "NewValue 3");
                VerifyAreEqual("Admin Ad", (Driver.GetGridCell(1, "ChangedByName", "ChangeHistory").Text), "Changed 3");

                VerifyAreEqual("Результат", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event 4");
                VerifyAreEqual("", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue 4");
                VerifyAreEqual("Rezult", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text), "NewValue 4");
                VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed 4");
            }

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskStatusAccepted\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusAccepted\"]")).Click();


            GoToAdmin("tasks#?viewed=true&modal=22");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 5, "count elem 5");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date 5");
            VerifyAreEqual("Принята", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event 5");
            VerifyAreEqual("Нет", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue 5");
            VerifyAreEqual("Да", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text), "NewValue 5");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed 5");
        }

        [Test]
        public void ChangeDescriptionMany()
        {
            GoToAdmin("tasks#?modal=20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elements");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskName\"]"));
            Driver.SetCkText("Edit Description NewTestTask", "editor1");


            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskButtonSave\"]")).Click();

            Refresh();

            GoToAdmin("tasks#?modal=20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 1, "count elem");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date");
            VerifyAreEqual("Описание", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event");
            VerifyAreEqual("new test20", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue");
            VerifyAreEqual("Edit Description NewTestTask", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text),
                "NewValue");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskName\"]"));
            Driver.SetCkText("New Description NewTestTask", "editor1");


            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskButtonSave\"]")).Click();


            GoToAdmin("tasks#?modal=20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]"))
                    .Count == 2, "count elem 2");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "date 2");
            VerifyAreEqual("Описание", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event 2");
            VerifyAreEqual("Edit Description NewTestTask", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text),
                "OldValue 2");
            VerifyAreEqual("New Description NewTestTask", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text),
                "NewValue 2");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed 2");
        }
        /*
        [Test]
        public void HistoryAddComment()
        {
            GoToAdmin("tasks");
            Driver.GetGridCellElement(8, "_serviceColumn", by: By.TagName("a")).Click();
            
            Driver.WaitForElem(By.Name("editTaskForm"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();
            
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"), "no elements");

            driver.FindElement(By.Name("addAdminCommentForm")).FindElement(By.TagName("textarea")).SendKeys("TestComment1");
            driver.FindElement(By.CssSelector("[data-e2e=\"commentAdd\"]")).Click();
            
            driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();
            

            GoToAdmin("tasks");
            Driver.GetGridCellElement(8, "_serviceColumn", by: By.TagName("a")).Click();
            Driver.WaitForElem(By.Name("editTaskForm"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"));
            VerifyIsTrue(driver.FindElement(By.CssSelector("[data-e2e=\"commentName\"]")).Text.Contains("Admin Ad"));
            VerifyIsTrue(driver.FindElement(By.CssSelector("[data-e2e=\"commentText\"]")).Text.Contains("TestComment1"));
            driver.FindElement(By.CssSelector("[data-e2e=\"ShowHistory\"] a")).Click();
            
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"ShowHistory\"]"));

            VerifyIsTrue(driver.FindElements(By.CssSelector(".modal-content [data-e2e=\"gridCell\"][data-e2e-col-index=\"0\"]")).Count == 1, "count elem");
            VerifyIsTrue(Driver.GetGridCell(0, "ModificationTimeFormatted", "ChangeHistory").Text.Contains(System.DateTime.Today.ToString("dd.MM.yyyy")), "date");
            VerifyAreEqual("Проект", (Driver.GetGridCell(0, "ParameterName", "ChangeHistory").Text), "event");
            VerifyAreEqual("test group 1", (Driver.GetGridCell(0, "OldValue", "ChangeHistory").Text), "OldValue");
            VerifyAreEqual("test group 3", (Driver.GetGridCell(0, "NewValue", "ChangeHistory").Text), "NewValue");
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "ChangedByName", "ChangeHistory").Text), "Changed");
        }*/
    }
}