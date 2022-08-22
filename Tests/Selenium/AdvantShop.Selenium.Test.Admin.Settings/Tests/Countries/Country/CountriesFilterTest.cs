using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.Country
{
    [TestFixture]
    public class CountriesFilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Countries);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Countries\\Customers.Country.csv",
                "data\\Admin\\Settings\\Countries\\Customers.Region.csv",
                "data\\Admin\\Settings\\Countries\\Customers.City.csv",
                "data\\Admin\\Settings\\Countries\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\Countries\\Customers.Customer.csv",
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
        public void CountriesFilterCode()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Functions.GridFilterSet(Driver, BaseUrl, "DialCode");

            //search by not exist 
            Driver.SetGridFilterValue("DialCode", "qqqqwd");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]")).Text,
                "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("DialCode", "111111111122222222222222222222222222222");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]")).Text,
                "too much symbols");

            //search by exist
            GoToAdmin("settingssystem#?systemTab=countries");
            Functions.GridFilterSet(Driver, BaseUrl, "DialCode");
            Driver.SetGridFilterValue("DialCode", "112");

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DialCode 2 count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter DialCode 2 row");
            VerifyAreEqual("TestCountry2", Driver.GetGridCell(0, "Name", "Country").Text, "filter DialCode 2 value");
            Functions.GridFilterClose(Driver, BaseUrl, "DialCode");
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "filter DialCode exit 1");
            VerifyAreEqual("TestCountry6", Driver.GetGridCell(5, "Name", "Country").Text, "filter DialCode exit 5");
        }

        [Test]
        public void CountriesFilterDisplay()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Functions.GridFilterSet(Driver, BaseUrl, "DisplayInPopup");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Да\"]")).Click();


            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DisplayInPopup count");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 5,
                "filter DisplayInPopup row");
            VerifyAreEqual("TestCountry2", Driver.GetGridCell(0, "Name", "Country").Text,
                "filter DisplayInPopup value1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();


            VerifyAreEqual("Найдено записей: 96",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DisplayInPopup count 2");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 10,
                "filter DisplayInPopup row 2");
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text,
                "filter DisplayInPopup value 2");

            Functions.GridFilterClose(Driver, BaseUrl, "DisplayInPopup");
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text,
                "filter DisplayInPopup exit 1");
            VerifyAreEqual("TestCountry6", Driver.GetGridCell(5, "Name", "Country").Text,
                "filter DisplayInPopup exit 5");
        }


        [Test]
        public void CountriesFilterIso2()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Functions.GridFilterSet(Driver, BaseUrl, "Iso2");
            //search by not exist 
            Driver.SetGridFilterValue("Iso2", "qwe");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Iso2", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            GoToAdmin("settingssystem#?systemTab=countries");
            Functions.GridFilterSet(Driver, BaseUrl, "Iso2");
            Driver.SetGridFilterValue("Iso2", "Z");
            Driver.XPathContainsText("h2", "Список стран");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Iso2 count");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 3,
                "filter Iso2 row");
            VerifyAreEqual("TestCountry26", Driver.GetGridCell(0, "Name", "Country").Text, "filter Iso2 value");
            Functions.GridFilterClose(Driver, BaseUrl, "Iso2");
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "filter Iso2 exit 1");
            VerifyAreEqual("TestCountry6", Driver.GetGridCell(5, "Name", "Country").Text, "filter Iso2 exit 5");
        }

        [Test]
        public void CountriesFilterIso3()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Functions.GridFilterSet(Driver, BaseUrl, "Iso3");

            //search by not exist 
            Driver.SetGridFilterValue("Iso3", "qwe");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Iso3", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            GoToAdmin("settingssystem#?systemTab=countries");
            Functions.GridFilterSet(Driver, BaseUrl, "Iso3");
            Driver.SetGridFilterValue("Iso3", "z1");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Iso3 count");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 3,
                "filter Iso3 row");
            VerifyAreEqual("TestCountry26", Driver.GetGridCell(0, "Name", "Country").Text, "filter Iso3 value");
            Functions.GridFilterClose(Driver, BaseUrl, "Iso3");
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "filter Iso3 exit 1");
            VerifyAreEqual("TestCountry6", Driver.GetGridCell(5, "Name", "Country").Text, "filter Iso3 exit 5");
        }

        [Test]
        public void CountriesFilterName()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Functions.GridFilterSet(Driver, BaseUrl, "Name");

            //search by not exist 
            Driver.SetGridFilterValue("Name", "qwew");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Name", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            GoToAdmin("settingssystem#?systemTab=countries");
            Functions.GridFilterSet(Driver, BaseUrl, "Name");
            Driver.SetGridFilterValue("Name", "TestCountry10");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 3,
                "filter Name row");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name count");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(0, "Name", "Country").Text, "filter Name value");

            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name exit");
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "filter Name exit");
            VerifyAreEqual("TestCountry6", Driver.GetGridCell(5, "Name", "Country").Text, "filter Name exit");
        }

        [Test]
        public void CountriesFilterSort()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");

            //search by not exist 
            Driver.SetGridFilterRange("SortOrder", "50000", "60000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterRange("SortOrder", "111111111122222222222222222222222222222", "111111111122222222222222222222222222233");
            Driver.XPathContainsText("h2", "Список стран");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "too much symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "too much symbols");

            //search by exist
            GoToAdmin("settingssystem#?systemTab=countries");
            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "15");
            Driver.XPathContainsText("h2", "Список стран");
            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 6,
                "filter SortOrder 2 row");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(0, "Name", "Country").Text, "filter SortOrder 2 value");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Country").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Редактирование страны", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
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
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "filter SortOrder exit 1 ");
            VerifyAreEqual("TestCountry6", Driver.GetGridCell(5, "Name", "Country").Text, "filter SortOrder exit 5");
            VerifyAreEqual("Найдено записей: 95",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder return");
        }
    }
}