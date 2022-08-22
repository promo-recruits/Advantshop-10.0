using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.City
{
    [TestFixture]
    public class SettingsCityTest : BaseSeleniumTest
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
        public void CityGrid()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);

            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "name");
            Driver.XPathContainsText("h1", "TestRegion1 - Список городов");
            VerifyAreEqual("111111", Driver.GetGridCell(0, "PhoneNumber", "City").Text, "phone");
            VerifyAreEqual("999999", Driver.GetGridCell(0, "MobilePhoneNumber", "City").Text, "mobile phone");
            VerifyAreEqual("1", Driver.GetGridCell(0, "CitySort", "City").Text, "SortOrder");
            VerifyIsFalse(Driver.GetGridCell(0, "DisplayInPopup", "City").FindElement(By.TagName("input")).Selected,
                "DisplayInPopup");

            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CityInplaceDisplay()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);

            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "name");
            VerifyIsFalse(Driver.GetGridCell(0, "DisplayInPopup", "City").FindElement(By.TagName("input")).Selected,
                "DisplayInPopup");

            Driver.GetGridCell(0, "DisplayInPopup", "City")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.GetGridCell(0, "DisplayInPopup", "City").FindElement(By.TagName("input")).Selected,
                "inplace DisplayInPopup 1");

            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);

            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "name");
            VerifyIsTrue(Driver.GetGridCell(0, "DisplayInPopup", "City").FindElement(By.TagName("input")).Selected,
                "inplace DisplayInPopup 2");
        }

        [Test]
        public void CityInplacePhone()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);

            Driver.XPathContainsText("h1", "TestRegion1 - Список городов");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "name");
            VerifyAreEqual("111111", Driver.GetGridCell(0, "PhoneNumber", "City").Text, "phone");
            VerifyAreEqual("999999", Driver.GetGridCell(0, "MobilePhoneNumber", "City").Text, "mobile phone");

            Driver.SendKeysGridCell("123123", 0, "PhoneNumber", "City");
            Driver.SendKeysGridCell("321321", 0, "MobilePhoneNumber", "City");

            Driver.XPathContainsText("h1", "TestRegion1 - Список городов");
            VerifyAreEqual("123123", Driver.GetGridCell(0, "PhoneNumber", "City").Text, "inplace phone");
            VerifyAreEqual("321321", Driver.GetGridCell(0, "MobilePhoneNumber", "City").Text, "inplace mobile phone");

            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);

            Driver.XPathContainsText("h1", "TestRegion1 - Список городов");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "name");
            VerifyAreEqual("123123", Driver.GetGridCell(0, "PhoneNumber", "City").Text, "inplace phone 2");
            VerifyAreEqual("321321", Driver.GetGridCell(0, "MobilePhoneNumber", "City").Text, "inplace mobile phone 2");
        }

        [Test]
        public void CityInplacesCod()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Driver.GridFilterSendKeys("TestCity100");
            Driver.XPathContainsText("h1", "Список городов");
            VerifyAreEqual("TestCity100", Driver.GetGridCell(0, "Name", "City").Text, "name");
            VerifyAreEqual("100", Driver.GetGridCell(0, "CitySort", "City").Text, "SortOrder");

            Driver.SendKeysGridCell("1000", 0, "CitySort", "City");

            Driver.XPathContainsText("h1", "Список городов");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "CitySort", "City").Text, "inplace SortOrder1");

            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Driver.GridFilterSendKeys("TestCity100");
            Driver.XPathContainsText("h1", "Список городов");
            VerifyAreEqual("TestCity100", Driver.GetGridCell(0, "Name", "City").Text, "name");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "CitySort", "City").Text, "inplace SortOrder2");
        }


        [Test]
        public void CityzSelectDelete()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);

            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "City")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "City")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("TestCity2", Driver.GetGridCell(0, "Name", "City").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "City")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "City")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "City")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "City")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "City")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "City")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("TestCity5", Driver.GetGridCell(0, "Name", "City").Text, "selected 1 grid delete");
            VerifyAreEqual("TestCity6", Driver.GetGridCell(1, "Name", "City").Text, "selected 2 grid delete");
            VerifyAreEqual("TestCity7", Driver.GetGridCell(2, "Name", "City").Text, "selected 3 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "City")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "City")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("TestCity15", Driver.GetGridCell(0, "Name", "City").Text,
                "selected all on page 1 grid delete");
            VerifyAreEqual("TestCity24", Driver.GetGridCell(9, "Name", "City").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("87", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "City")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "City")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");

            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all 2");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 2");
        }
    }
}