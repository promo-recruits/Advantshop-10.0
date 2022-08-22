using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.Region
{
    [TestFixture]
    public class SettingsRegionFilterTest : BaseSeleniumTest
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
        public void RegionFilterCode()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);

            Functions.GridFilterSet(Driver, BaseUrl, "RegionCode");

            //search by not exist 
            Driver.SetGridFilterValue("RegionCode", "qwe");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("RegionCode", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("RegionCode", "70");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter RegionCode 2 count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter RegionCode 2 row");
            VerifyAreEqual("TestRegion60", Driver.GetGridCell(0, "Name", "Region").Text, "filter RegionCode 2 value");
            Functions.GridFilterClose(Driver, BaseUrl, "RegionCode");
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "filter RegionCode exit 1");
            VerifyAreEqual("TestRegion6", Driver.GetGridCell(5, "Name", "Region").Text, "filter RegionCode exit 5");
        }

        [Test]
        public void RegionFilterName()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);

            Functions.GridFilterSet(Driver, BaseUrl, "Name");

            //search by not exist 
            Driver.SetGridFilterValue("Name", "qwew");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Name", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("Name", "TestRegion10");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 3,
                "filter Name row");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name count");
            VerifyAreEqual("TestRegion10", Driver.GetGridCell(0, "Name", "Region").Text, "filter Name value");

            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name exit");
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "filter Name exit");
            VerifyAreEqual("TestRegion6", Driver.GetGridCell(5, "Name", "Region").Text, "filter Name exit");
        }

        [Test]
        public void RegionFilterSort()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);

            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");

            //search by not exist 
            Driver.SetGridFilterRange("SortOrder", "50000", "60000");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterRange("SortOrder", "111111111122222222222222222222222222233", "1111111111222222222222222222222222222");
            Driver.XPathContainsText("h1", "Список регионов");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "too much symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "too much symbols");

            //search by exist
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "15");
            Driver.XPathContainsText("h1", "Список регионов");
            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 6,
                "filter SortOrder 2 row");
            VerifyAreEqual("TestRegion10", Driver.GetGridCell(0, "Name", "Region").Text, "filter SortOrder 2 value");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Region").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Редактирование региона", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder return");
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"SortOrder\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");


            Functions.GridFilterClose(Driver, BaseUrl, "SortOrder");
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "filter SortOrder exit 1 ");
            VerifyAreEqual("TestRegion6", Driver.GetGridCell(5, "Name", "Region").Text, "filter SortOrder exit 5");
            VerifyAreEqual("Найдено записей: 95",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder return");
        }
    }
}