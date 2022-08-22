using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsTasks
{
    [TestFixture]
    public class SettingsTasksTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Tasks);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsTasks\\Customers.TaskGroup.csv"
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

        /*
        [Test]
        public void SetDefaultProject()
        {
            GoToAdmin("settingstasks");

            (new SelectElement(driver.FindElement(By.Id("DefaultTaskGroupId")))).SelectByText("Project 3");
            driver.FindElement(By.CssSelector("[data-e2e=\"settingsTasksSave\"]")).Click();
            Thread.Sleep(2000);

            GoToAdmin("settingstasks");

            IWebElement selectElem = driver.FindElement(By.Id("DefaultTaskGroupId"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Project 3"), "default task group admin");

            GoToAdmin("tasks");

            driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            selectElem = driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Project 3"), "default task group add task");
        }
        */

        [Test]
        public void PushNotificationSameTab()
        {
            GoToAdmin("settingstasks");

            Driver.FindElement(By.CssSelector("[data-e2e=\"pushNotificationSameTab\"]")).Click();
            Thread.Sleep(1000);

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"settingsTasksSave\"]")).Enabled)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"settingsTasksSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            GoToAdmin("settingstasks");
            VerifyIsTrue(
                Driver.FindElement(By.Id("WebNotificationInNewTab")).GetAttribute("value").Contains("False") &&
                Driver.FindElement(By.Id("WebNotificationInNewTab")).Selected, "notifications in same tab");
        }

        [Test]
        public void PushNotificationNewTab()
        {
            GoToAdmin("settingstasks");

            Driver.FindElement(By.CssSelector("[data-e2e=\"pushNotificationNewTab\"]")).Click();
            Thread.Sleep(100);

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"settingsTasksSave\"]")).Enabled)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"settingsTasksSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            GoToAdmin("settingstasks");
            VerifyIsTrue(
                Driver.FindElements(By.Id("WebNotificationInNewTab"))[1].GetAttribute("value").Contains("True") &&
                Driver.FindElements(By.Id("WebNotificationInNewTab"))[1].Selected, "notifications in new tab");
        }
    }
}