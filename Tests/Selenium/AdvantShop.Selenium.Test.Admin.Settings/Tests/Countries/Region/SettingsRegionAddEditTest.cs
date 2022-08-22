using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.Region
{
    [TestFixture]
    public class SettingsRegionAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Countries);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Countries\\Region\\Customers.Country.csv",
                "data\\Admin\\Settings\\Countries\\Region\\Customers.Region.csv",
                "data\\Admin\\Settings\\Countries\\Region\\Customers.City.csv",
                "data\\Admin\\Settings\\Countries\\Region\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\Countries\\Region\\Customers.Customer.csv",
                "data\\Admin\\Settings\\Countries\\Settings.Settings.csv"
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
        public void RegionOpenEdit()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();

            Driver.GetGridCell(0, "_serviceColumn", "Region").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Редактирование региона", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("TestRegion1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RegionName\"]")).GetAttribute("value"), "pop up name");
            VerifyAreEqual("11", Driver.FindElement(By.CssSelector("[data-e2e=\"RegionCode\"]")).GetAttribute("value"),
                "pop up RegionCode");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSort\"]")).GetAttribute("value"),
                "pop up RegionSort");
        }

        [Test]
        public void RegionEdit()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Driver.GridFilterSendKeys("TestRegion100");

            Driver.GetGridCell(0, "_serviceColumn", "Region").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionName\"]")).SendKeys("NewName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionCode\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionCode\"]")).SendKeys("999");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSort\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSort\"]")).SendKeys("888");

            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            Driver.GridFilterSendKeys("NewName");

            VerifyAreEqual("NewName", Driver.GetGridCell(0, "Name", "Region").Text, "grid name");
            VerifyAreEqual("999", Driver.GetGridCell(0, "RegionCode", "Region").Text, "grid RegionCode");
            VerifyAreEqual("888", Driver.GetGridCell(0, "SortOrder", "Region").Text, "grid SortOrder");

            Driver.GetGridCell(0, "_serviceColumn", "Region").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Редактирование региона", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("NewName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RegionName\"]")).GetAttribute("value"), "npop up ame");
            VerifyAreEqual("999", Driver.FindElement(By.CssSelector("[data-e2e=\"RegionCode\"]")).GetAttribute("value"),
                "pop up RegionCode");
            VerifyAreEqual("888", Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSort\"]")).GetAttribute("value"),
                "pop up RegionSort");
        }

        [Test]
        public void RegionOpenAdd()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddRegion\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Добавление региона", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"RegionName\"]")).GetAttribute("value"),
                "name");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"RegionCode\"]")).GetAttribute("value"),
                "RegionCode");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSort\"]")).GetAttribute("value"),
                "RegionSort");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionName\"]")).SendKeys("NewRegion");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionCode\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionCode\"]")).SendKeys("987");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSort\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSort\"]")).SendKeys("789");

            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Driver.GridFilterSendKeys("NewRegion");

            VerifyAreEqual("NewRegion", Driver.GetGridCell(0, "Name", "Region").Text, "grid name");
            VerifyAreEqual("987", Driver.GetGridCell(0, "RegionCode", "Region").Text, "grid RegionCode");
            VerifyAreEqual("789", Driver.GetGridCell(0, "SortOrder", "Region").Text, "grid SortOrder");

            Driver.GetGridCell(0, "_serviceColumn", "Region").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Редактирование региона", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("NewRegion",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RegionName\"]")).GetAttribute("value"), "pop up name");
            VerifyAreEqual("987", Driver.FindElement(By.CssSelector("[data-e2e=\"RegionCode\"]")).GetAttribute("value"),
                "pop up RegionCode");
            VerifyAreEqual("789", Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSort\"]")).GetAttribute("value"),
                "pop up RegionSort");
        }
    }
}