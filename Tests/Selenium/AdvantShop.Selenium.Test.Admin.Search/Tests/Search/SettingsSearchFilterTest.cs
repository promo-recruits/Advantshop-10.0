using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Search.Tests.Search
{
    [TestFixture]
    public class SettingsSearchFilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.SettingsSearch);
            InitializeService.LoadData("data\\Admin\\SettingsSearch\\Settings.SettingsSearch.csv");

            Init();

            GoToAdmin("settingssearch");
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
        public void SettingsSearchFilterTitle()
        {
            //search by exist name 
            Functions.GridFilterSet(Driver, BaseUrl, name: "Title");
            Driver.ElementSendKeys(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]"), "test title 2", "h1");

            VerifyAreEqual("test title 2", Driver.GetGridCellText(0, "Title"), "filtered items");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all with filter");

            //search by not exist name 
            Driver.ElementSendKeys(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]"), "123123123 title 3", "h1");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter not exist settings");

            //search too much symbols
            Driver.ElementSendKeys(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]"),
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", "h1");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter settings too much symbols");

            //search invalid symbols
            Driver.ElementSendKeys(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]"),
                "########@@@@@@@@&&&&&&&******,,,,..", "h1");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter  invalid symbols");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "Title");
            VerifyAreEqual("Найдено записей: 150",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after closing filter");
        }

        [Test]
        public void SettingsSearchzFilterTitleDelete()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "Title");
            Driver.ElementSendKeys(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]"), "test title 2", "h1");

            VerifyAreEqual("test title 2", Driver.GetGridCellText(0, "Title"), "filtered items");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all with filter");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter delete items");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "Title");
            Refresh();
            VerifyAreEqual("Найдено записей: 139",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after closing filter");

            GoToAdmin("settingssearch");
            VerifyAreEqual("Найдено записей: 139",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count 2 all after closing filter");
        }
    }
}