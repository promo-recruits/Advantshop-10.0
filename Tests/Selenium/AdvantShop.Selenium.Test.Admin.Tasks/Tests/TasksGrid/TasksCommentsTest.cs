using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TasksGrid
{
    [TestFixture]
    public class TasksCommentsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\TasksCommentTest\\Customers.Departments.csv",
                "data\\Admin\\TasksCommentTest\\Customers.Customer.csv",
                "data\\Admin\\TasksCommentTest\\Customers.Managers.csv",
                "data\\Admin\\TasksCommentTest\\Customers.TaskGroup.csv",
                "data\\Admin\\TasksCommentTest\\Customers.Task.csv",
                "data\\Admin\\TasksCommentTest\\Customers.TaskManager.csv"
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
        public void AddCommentTasksADD()
        {
            GoToAdmin("tasks");

            Driver.GetGridFilter().Click();
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.WaitForElem(By.Name("editTaskForm"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAttachment\"]"));

            Driver.FindElement(By.Name("addAdminCommentForm")).FindElement(By.TagName("textarea"))
                .SendKeys("TestComment1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"commentAdd\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            GoToAdmin("tasks");

            Driver.GetGridFilter().Click();
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();
            Driver.WaitForElem(By.Name("editTaskForm"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"commentName\"]")).Text.Contains("Admin Ad"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"commentText\"]")).Text
                .Contains("TestComment1"));
        }

        [Test]
        public void AddCommentTasksANSWER()
        {
            GoToAdmin("tasks");
            Driver.GetGridCellElement(1, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.WaitForElem(By.Name("editTaskForm"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAttachment\"]"));

            Driver.FindElement(By.Name("addAdminCommentForm")).FindElement(By.TagName("textarea"))
                .SendKeys("TestComment2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"commentAdd\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            GoToAdmin("tasks");
            Driver.GetGridCellElement(1, "_serviceColumn", by: By.TagName("a")).Click();
            Driver.WaitForElem(By.Name("editTaskForm"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAttachment\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"commentAnswer\"]")).Click();
            Driver.FindElement(By.Id("adminCommentsFormText")).SendKeys(" TestComment2 Answer");
            Driver.FindElement(By.CssSelector("[data-e2e=\"commentAdd\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            GoToAdmin("tasks");
            Driver.GetGridCellElement(1, "_serviceColumn", by: By.TagName("a")).Click();
            Driver.WaitForElem(By.Name("editTaskForm"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"commentName\"]"))[1].Text
                .Contains("Admin Ad"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"commentText\"]"))[1].Text
                .Contains("TestComment2 Answer"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"commentParentName\"]")).Text
                .Contains("Admin Ad"));
        }

        [Test]
        public void AddCommentTasksDELETE()
        {
            GoToAdmin("tasks");
            Driver.GetGridCellElement(2, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.WaitForElem(By.Name("editTaskForm"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAttachment\"]"));

            Driver.FindElement(By.Name("addAdminCommentForm")).FindElement(By.TagName("textarea"))
                .SendKeys("TestComment3");
            Driver.FindElement(By.CssSelector("[data-e2e=\"commentAdd\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            GoToAdmin("tasks");
            Driver.GetGridCellElement(2, "_serviceColumn", by: By.TagName("a")).Click();
            Driver.WaitForElem(By.Name("editTaskForm"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"commentText\"]")).Text
                .Contains("TestComment3"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAttachment\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"commentDelete\"]")).Click();
            Driver.SwalConfirm();
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            GoToAdmin("tasks");
            Driver.GetGridCellElement(2, "_serviceColumn", by: By.TagName("a")).Click();
            Driver.WaitForElem(By.Name("editTaskForm"));

            VerifyIsFalse(Driver.PageSource.Contains("TestComment3"));
        }

        [Test]
        public void AddCommentTasksEDIT()
        {
            GoToAdmin("tasks");
            Driver.GetGridCellElement(3, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.WaitForElem(By.Name("editTaskForm"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAttachment\"]"));
            Driver.FindElement(By.Name("addAdminCommentForm")).FindElement(By.TagName("textarea"))
                .SendKeys("TestComment4");
            Driver.FindElement(By.CssSelector("[data-e2e=\"commentAdd\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            GoToAdmin("tasks");
            Driver.GetGridCellElement(3, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.WaitForElem(By.Name("editTaskForm"));
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"edittaskAttachment\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"commentEdit\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"commentEditText\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"commentEditText\"]")).SendKeys("changed");

            Driver.FindElement(By.CssSelector("[data-e2e=\"commentSave\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            GoToAdmin("tasks");
            Driver.GetGridCellElement(3, "_serviceColumn", by: By.TagName("a")).Click();
            Driver.WaitForElem(By.Name("editTaskForm"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"commentText\"]")).Text.Contains("changed"));
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"commentText\"]")).Text.Contains("TestComment4"));
        }
    }
}