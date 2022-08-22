using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.Region
{
    [TestFixture]
    public class SettingsRegionTest : BaseSeleniumTest
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
        public void RegionGrid()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);

            VerifyAreEqual("TestCountry1 - Список регионов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"h1-region\"]")).Text, "h1 region");
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "name region");
            Driver.XPathContainsText("h1", "TestCountry1 - Список регионов");
            VerifyAreEqual("11", Driver.GetGridCell(0, "RegionCode", "Region").Text, "cod region");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Region").Text, "sort region");

            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all region");
        }

        [Test]
        public void RegionInplaceSortCode()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);

            Driver.GridFilterSendKeys("TestRegion100");
            Driver.XPathContainsText("h1", "Список регионов");
            VerifyAreEqual("TestRegion100", Driver.GetGridCell(0, "Name", "Region").Text, "name");
            VerifyAreEqual("110", Driver.GetGridCell(0, "RegionCode", "Region").Text, "DialCode");
            VerifyAreEqual("100", Driver.GetGridCell(0, "SortOrder", "Region").Text, "SortOrder");

            Driver.SendKeysGridCell("999", 0, "RegionCode", "Region");
            Driver.SendKeysGridCell("1000", 0, "SortOrder", "Region");

            Driver.XPathContainsText("h1", "Список регионов");
            VerifyAreEqual("999", Driver.GetGridCell(0, "RegionCode", "Region").Text, "inplace DialCode1");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "SortOrder", "Region").Text, "inplace SortOrder1");

            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);

            Driver.GridFilterSendKeys("TestRegion100");
            Driver.XPathContainsText("h1", "Список регионов");
            VerifyAreEqual("TestRegion100", Driver.GetGridCell(0, "Name", "Region").Text, "name");
            VerifyAreEqual("999", Driver.GetGridCell(0, "RegionCode", "Region").Text, "inplace DialCode2");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "SortOrder", "Region").Text, "inplace SortOrder2");
        }

        [Test]
        public void RegionRedirect()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(1, "Name", "Country").Click();
            Thread.Sleep(2000);

            VerifyAreEqual("TestCountry2 - Список регионов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"h1-region\"]")).Text, "h1 region");
            VerifyAreEqual("TestRegion102", Driver.GetGridCell(0, "Name", "Region").Text, "name region");
            Driver.XPathContainsText("h1", "TestCountry2 - Список регионов");
            VerifyAreEqual("112", Driver.GetGridCell(0, "RegionCode", "Region").Text, "cod region");
            VerifyAreEqual("102", Driver.GetGridCell(0, "SortOrder", "Region").Text, "sort region");

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all region");

            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestRegion102 - Список городов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"h1-city\"]")).Text, "h1 city 1");
            VerifyAreEqual("TestCity16", Driver.GetGridCell(0, "Name", "City").Text, "name city1");

            VerifyAreEqual("Найдено записей: 30",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all city");

            Driver.FindElement(By.CssSelector("[data-e2e=\"GoToCountry\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("Список стран", Driver.FindElement(By.CssSelector("[data-e2e=\"h1-country\"]")).Text,
                "h1 country");
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "name country");

            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);

            VerifyAreEqual("TestCountry1 - Список регионов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"h1-region\"]")).Text, "h1 region 2 ");
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "name region 2");
            Driver.XPathContainsText("h1", "TestCountry1 - Список регионов");
            VerifyAreEqual("11", Driver.GetGridCell(0, "RegionCode", "Region").Text, "cod region 2");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Region").Text, "sort region 2");

            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all region 2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"GoToCity\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("TestCountry1 - Список городов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"h1-city\"]")).Text, "h1 city");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "name city");

            VerifyAreEqual("Найдено записей: 71",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all city");

            Driver.FindElement(By.CssSelector("[data-e2e=\"GoToRegion\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestCountry1 - Список регионов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"h1-region\"]")).Text, "h1 region 2");
        }
    }

    [TestFixture]
    public class SettingsRegionSelectDeleteTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Countries);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Countries\\Region\\RegionDelete\\Customers.Country.csv",
                "data\\Admin\\Settings\\Countries\\Region\\RegionDelete\\Customers.Region.csv",
                "data\\Admin\\Settings\\Countries\\Region\\RegionDelete\\Customers.City.csv",
                "data\\Admin\\Settings\\Countries\\Region\\RegionDelete\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\Countries\\Region\\RegionDelete\\Customers.Customer.csv",
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
        public void RegionzSelectDelete()
        {
            GoToAdmin("settings/common");
            (new SelectElement(Driver.FindElement(By.Id("CountryId")))).SelectByText("TestCountry1");
            Thread.Sleep(1500);
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSelect\"]")))).SelectByText(
                "TestRegion101");

            Driver.FindElement(By.Id("City")).Clear();
            Driver.FindElement(By.Id("City")).SendKeys("TestCity");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"] input")).Click();

            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);

            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "Region")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Region")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("TestRegion2", Driver.GetGridCell(0, "Name", "Region").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Region")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Region")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Region")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Region")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Region")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Region")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("TestRegion5", Driver.GetGridCell(0, "Name", "Region").Text, "selected 1 grid delete");
            VerifyAreEqual("TestRegion6", Driver.GetGridCell(1, "Name", "Region").Text, "selected 2 grid delete");
            VerifyAreEqual("TestRegion7", Driver.GetGridCell(2, "Name", "Region").Text, "selected 3 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Region")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Region")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("TestRegion15", Driver.GetGridCell(0, "Name", "Region").Text,
                "selected all on page 1 grid delete");
            VerifyAreEqual("TestRegion24", Driver.GetGridCell(9, "Name", "Region").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("87", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "Region")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "Region")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");
            VerifyAreEqual("TestRegion101", Driver.GetGridCell(0, "Name", "Region").Text, "name Region end");

            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 2");
            VerifyAreEqual("TestRegion101", Driver.GetGridCell(0, "Name", "Region").Text,
                "name Region end after refresh");
        }
    }
}