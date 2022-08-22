using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.Country
{
    [TestFixture]
    public class CountriesTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Countries);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Countries\\Country\\Customers.Country.csv",
                "data\\Admin\\Settings\\Countries\\Country\\Customers.Region.csv",
                "data\\Admin\\Settings\\Countries\\Country\\Customers.City.csv",
                "data\\Admin\\Settings\\Countries\\Country\\Settings.Settings.csv"
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
        public void CountriesGrid()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "name");
            VerifyAreEqual("AA", Driver.GetGridCell(0, "Iso2", "Country").Text, "Iso2");
            VerifyAreEqual("AA1", Driver.GetGridCell(0, "Iso3", "Country").Text, "Iso3");
            VerifyAreEqual("111", Driver.GetGridCell(0, "DialCode", "Country").Text, "DialCode");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Country").Text, "SortOrder");
            VerifyIsFalse(Driver.GetGridCell(0, "DisplayInPopup", "Country").FindElement(By.TagName("input")).Selected,
                "DisplayInPopup");

            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CountriesInplaceDisplay()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "name");
            VerifyIsFalse(Driver.GetGridCell(0, "DisplayInPopup", "Country").FindElement(By.TagName("input")).Selected,
                "DisplayInPopup");

            Driver.GetGridCellInputForm(0, "DisplayInPopup", "Country")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.GetGridCell(0, "DisplayInPopup", "Country").FindElement(By.TagName("input")).Selected,
                "inplace DisplayInPopup 1");

            GoToAdmin("settingssystem#?systemTab=countries");

            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "name");
            VerifyIsTrue(Driver.GetGridCell(0, "DisplayInPopup", "Country").FindElement(By.TagName("input")).Selected,
                "inplace DisplayInPopup 2");
        }

        [Test]
        public void CountriesInplaceIso()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "name");
            VerifyAreEqual("AA", Driver.GetGridCell(0, "Iso2", "Country").Text, "Iso2");
            VerifyAreEqual("AA1", Driver.GetGridCell(0, "Iso3", "Country").Text, "Iso3");

            Driver.SendKeysGridCell("ZZ", 0, "Iso2", "Country");
            Driver.SendKeysGridCell("ZZZ", 0, "Iso3", "Country");

            Driver.XPathContainsText("h2", "Список стран");
            VerifyAreEqual("ZZ", Driver.GetGridCell(0, "Iso2", "Country").Text, "inplace Iso2 1");
            VerifyAreEqual("ZZZ", Driver.GetGridCell(0, "Iso3", "Country").Text, "inplace Iso3 1");

            GoToAdmin("settingssystem#?systemTab=countries");

            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "name");
            VerifyAreEqual("ZZ", Driver.GetGridCell(0, "Iso2", "Country").Text, "inplace Iso2 2");
            VerifyAreEqual("ZZZ", Driver.GetGridCell(0, "Iso3", "Country").Text, "inplace Iso3 2");
        }

        [Test]
        public void CountriesInplaceSortCod()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GridFilterSendKeys("TestCountry100");
            Driver.XPathContainsText("h2", "Список стран");
            VerifyAreEqual("TestCountry100", Driver.GetGridCell(0, "Name", "Country").Text, "name");
            VerifyAreEqual("210", Driver.GetGridCell(0, "DialCode", "Country").Text, "DialCode");
            VerifyAreEqual("100", Driver.GetGridCell(0, "SortOrder", "Country").Text, "SortOrder");

            Driver.SendKeysGridCell("999", 0, "DialCode", "Country");
            Driver.SendKeysGridCell("1000", 0, "SortOrder", "Country");

            Driver.XPathContainsText("h2", "Список стран");
            VerifyAreEqual("999", Driver.GetGridCell(0, "DialCode", "Country").Text, "inplace DialCode1");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "SortOrder", "Country").Text, "inplace SortOrder1");

            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GridFilterSendKeys("TestCountry100");
            Driver.XPathContainsText("h2", "Список стран");
            VerifyAreEqual("TestCountry100", Driver.GetGridCell(0, "Name", "Country").Text, "name");
            VerifyAreEqual("999", Driver.GetGridCell(0, "DialCode", "Country").Text, "inplace DialCode2");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "SortOrder", "Country").Text, "inplace SortOrder2");
        }

        [Test]
        public void CountriesRedirect()
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

            VerifyAreEqual("Найдено записей: 29",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all region");

            Driver.FindElement(By.CssSelector("[data-e2e=\"GoToCountry\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("Список стран", Driver.FindElement(By.CssSelector("[data-e2e=\"h1-country\"]")).Text,
                "h1 country");
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "name country");

            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"GoToCity\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("TestCountry1 - Список городов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"h1-city\"]")).Text, "h1 city");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "name city");

            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all city");

            Driver.FindElement(By.CssSelector("[data-e2e=\"GoToRegion\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestCountry1 - Список регионов",
                Driver.FindElement(By.CssSelector("[data-e2e=\"h1-region\"]")).Text, "h1 region 2");
        }

        [Test]
        public void CountrieszSelectDelete()
        {
            GoToAdmin("settings/common");
            (new SelectElement(Driver.FindElement(By.Id("CountryId")))).SelectByText("TestCountry100");
            Thread.Sleep(1500);
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"RegionSelect\"]")))).SelectByText(
                "TestRegion101");

            Driver.FindElement(By.Id("City")).Clear();
            Driver.FindElement(By.Id("City")).SendKeys("TestCity");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"] input")).Click();

            GoToAdmin("settingssystem#?systemTab=countries");
            //check delete by search 
            Driver.GridFilterSendKeys("TestCountry21");
            VerifyAreEqual("TestCountry21", Driver.GetGridCell(0, "Name", "Country").Text, "find value");
            Driver.GetGridCell(0, "_serviceColumn", "Country")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "find no value ");

            Driver.GridFilterSendKeys("");
            GoToAdmin("settingssystem#?systemTab=countries");
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "Country")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Country")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("TestCountry2", Driver.GetGridCell(0, "Name", "Country").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Country")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Country")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Country")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Country")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Country")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Country")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("TestCountry5", Driver.GetGridCell(0, "Name", "Country").Text, "selected 1 grid delete");
            VerifyAreEqual("TestCountry6", Driver.GetGridCell(1, "Name", "Country").Text, "selected 2 grid delete");
            VerifyAreEqual("TestCountry7", Driver.GetGridCell(2, "Name", "Country").Text, "selected 3 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Country")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Country")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("TestCountry15", Driver.GetGridCell(0, "Name", "Country").Text,
                "selected all on page 1 grid delete");
            VerifyAreEqual("TestCountry25", Driver.GetGridCell(9, "Name", "Country").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("86", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "Country")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "Country")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");
            VerifyAreEqual("TestCountry100", Driver.GetGridCell(0, "Name", "Country").Text, "name country");

            GoToAdmin("settingssystem#?systemTab=countries");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 2");
            VerifyAreEqual("TestCountry100", Driver.GetGridCell(0, "Name", "Country").Text, "name country");
        }
    }
}