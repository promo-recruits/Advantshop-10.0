using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.City
{
    [TestFixture]
    public class SettingsCityFilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Countries);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Countries\\City\\Customers.Country.csv",
                "data\\Admin\\Settings\\Countries\\City\\Customers.Region.csv",
                "data\\Admin\\Settings\\Countries\\City\\Customers.City.csv",
                "data\\Admin\\Settings\\Countries\\City\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\Countries\\City\\Customers.Customer.csv",
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
        public void CityFilterDisplay()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Functions.GridFilterSet(Driver, BaseUrl, "DisplayInPopup");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Да\"]")).Click();

            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DisplayInPopup count");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 5,
                "filter DisplayInPopup row");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(0, "Name", "City").Text, "filter DisplayInPopup value1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();

            VerifyAreEqual("Найдено записей: 96",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter DisplayInPopup count 2");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 10,
                "filter DisplayInPopup row 2");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "filter DisplayInPopup value 2");

            Functions.GridFilterClose(Driver, BaseUrl, "DisplayInPopup");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "filter DisplayInPopup exit 1");
            VerifyAreEqual("TestCity6", Driver.GetGridCell(5, "Name", "City").Text, "filter DisplayInPopup exit 5");
        }


        [Test]
        public void CityFilterPhone()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Functions.GridFilterSet(Driver, BaseUrl, "PhoneNumber");
            //search by not exist 
            Driver.SetGridFilterValue("PhoneNumber", "qwe");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("PhoneNumber", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("PhoneNumber", "111113");
            Driver.XPathContainsText("h1", "Список городов");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter PhoneNumber count");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter PhoneNumber row");
            VerifyAreEqual("TestCity3", Driver.GetGridCell(0, "Name", "City").Text, "filter PhoneNumber value");
            Functions.GridFilterClose(Driver, BaseUrl, "PhoneNumber");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "filter PhoneNumber exit 1");
            VerifyAreEqual("TestCity6", Driver.GetGridCell(5, "Name", "City").Text, "filter PhoneNumber exit 5");
        }

        [Test]
        public void CityFilterMobile()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Functions.GridFilterSet(Driver, BaseUrl, "MobilePhoneNumber");

            //search by not exist 
            Driver.SetGridFilterValue("MobilePhoneNumber", "qwe");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("MobilePhoneNumber", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("MobilePhoneNumber", "999998");
            Driver.XPathContainsText("h1", "Список городов");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter MobilePhoneNumber count");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter MobilePhoneNumber row");
            VerifyAreEqual("TestCity2", Driver.GetGridCell(0, "Name", "City").Text, "filter MobilePhoneNumber value");
            Functions.GridFilterClose(Driver, BaseUrl, "MobilePhoneNumber");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "filter MobilePhoneNumber exit 1");
            VerifyAreEqual("TestCity6", Driver.GetGridCell(5, "Name", "City").Text, "filter MobilePhoneNumber exit 5");
        }

        [Test]
        public void CityFilterName()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Functions.GridFilterSet(Driver, BaseUrl, "Name");

            //search by not exist 
            Driver.SetGridFilterValue("Name", "qwew");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Name", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("Name", "TestCity10");
            Driver.XPathContainsText("h1", "Список городов");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 3,
                "filter Name row");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name count");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(0, "Name", "City").Text, "filter Name value");

            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Name exit");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "filter Name exit");
            VerifyAreEqual("TestCity6", Driver.GetGridCell(5, "Name", "City").Text, "filter Name exit");
        }

        [Test]
        public void CityFilterSort()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Functions.GridFilterSet(Driver, BaseUrl, "CitySort");

            //search by not exist 
            Driver.SetGridFilterRange("CitySort", "50000", "60000");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterRange("CitySort", "111111111122222222222222222222222222222", "111111111122222222222222222222222222233");
            Driver.XPathContainsText("h1", "Список городов");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter too much symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "filter too much symbols");

            //search by exist
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Functions.GridFilterSet(Driver, BaseUrl, "CitySort");
            Driver.SetGridFilterRange("CitySort", "10", "15");
            Driver.XPathContainsText("h1", "Список городов");
            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 6,
                "filter SortOrder 2 row");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(0, "Name", "City").Text, "filter SortOrder 2 value");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "City").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Редактирование города", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder return");
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"CitySort\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");


            Functions.GridFilterClose(Driver, BaseUrl, "CitySort");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "filter SortOrder exit 1 ");
            VerifyAreEqual("TestCity6", Driver.GetGridCell(5, "Name", "City").Text, "filter SortOrder exit 5");
            VerifyAreEqual("Найдено записей: 95",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SortOrder return");
        }
    }
}