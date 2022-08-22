using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.City
{
    [TestFixture]
    public class SettingsCityAdEditTest : BaseSeleniumTest
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
        public void CitiOpenEdit()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Driver.GetGridCell(0, "Name", "Country").Click();
            Driver.GetGridCell(0, "Name", "Region").Click();

            Driver.GetGridCell(0, "_serviceColumn", "City").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Редактирование города", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("TestCity1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityName\"]")).GetAttribute("value"), "pop up name");
            VerifyAreEqual("111111",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityPhone\"]")).GetAttribute("value"),
                "pop up CityPhone");
            VerifyAreEqual("999999",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityMobilePhone\"]")).GetAttribute("value"),
                "pop up CityMobilePhone");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"CityMain\"] input")).Selected,
                "pop up CityDisplay");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector("[data-e2e=\"CitySort\"]")).GetAttribute("value"),
                "pop up CitySortOrder");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"CityRegion\"]"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("TestRegion1"));
        }

        [Test]
        public void CityEdit()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Driver.GetGridCell(0, "Name", "Region").Click();

            Driver.GridFilterSendKeys("TestCity100");

            Driver.GetGridCell(0, "_serviceColumn", "City").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"CityName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityName\"]")).SendKeys("NewName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityPhone\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityPhone\"]")).SendKeys("789789");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityMobilePhone\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityMobilePhone\"]")).SendKeys("987987");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CityMain\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CitySort\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CitySort\"]")).SendKeys("888");

            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            Driver.GridFilterSendKeys("NewName");

            VerifyAreEqual("NewName", Driver.GetGridCell(0, "Name", "City").Text, "grid name");
            VerifyAreEqual("789789", Driver.GetGridCell(0, "PhoneNumber", "City").Text, "grid CityPhone");
            VerifyAreEqual("987987", Driver.GetGridCell(0, "MobilePhoneNumber", "City").Text, "grid Iso3");
            VerifyAreEqual("888", Driver.GetGridCell(0, "CitySort", "City").Text, "grid SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "DisplayInPopup", "City").FindElement(By.TagName("input")).Selected,
                "grid DisplayInPopup");

            Driver.GetGridCell(0, "_serviceColumn", "City").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Редактирование города", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("NewName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityName\"]")).GetAttribute("value"), "npop up ame");
            VerifyAreEqual("789789",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityPhone\"]")).GetAttribute("value"),
                "pop up CityPhone");
            VerifyAreEqual("987987",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityMobilePhone\"]")).GetAttribute("value"),
                "pop up CityMobilePhone");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"CityMain\"] input")).Selected, "CityDisplay");
            VerifyAreEqual("888", Driver.FindElement(By.CssSelector("[data-e2e=\"CitySort\"]")).GetAttribute("value"),
                "pop up CitySortOrder");
        }

        [Test]
        public void CityOpenAdd()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Driver.GetGridCell(0, "Name", "Region").Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCity\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Добавление города", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"CityName\"]")).GetAttribute("value"),
                "name");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"CityPhone\"]")).GetAttribute("value"),
                "CityPhone");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityMobilePhone\"]")).GetAttribute("value"),
                "CityMobilePhone");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"CityMain\"] input")).Selected, "CityDisplay");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"CitySort\"]")).GetAttribute("value"),
                "CitySortOrder");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CityName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityName\"]")).SendKeys("NewCity");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityPhone\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityPhone\"]")).SendKeys("789789");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityMobilePhone\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityMobilePhone\"]")).SendKeys("987987");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CityMain\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"CitySort\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CitySort\"]")).SendKeys("789");

            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Driver.GridFilterSendKeys("NewCity");

            VerifyAreEqual("NewCity", Driver.GetGridCell(0, "Name", "City").Text, "grid name");
            VerifyAreEqual("789789", Driver.GetGridCell(0, "PhoneNumber", "City").Text, "grid CityPhone");
            VerifyAreEqual("987987", Driver.GetGridCell(0, "MobilePhoneNumber", "City").Text, "grid MobilePhoneNumber");
            VerifyAreEqual("789", Driver.GetGridCell(0, "CitySort", "City").Text, "grid SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "DisplayInPopup", "City").FindElement(By.TagName("input")).Selected,
                "grid DisplayInPopup");

            Driver.GetGridCell(0, "_serviceColumn", "City").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Редактирование города", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("NewCity",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityName\"]")).GetAttribute("value"), "pop up name");
            VerifyAreEqual("789789",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityPhone\"]")).GetAttribute("value"),
                "pop up CityPhone");
            VerifyAreEqual("987987",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityMobilePhone\"]")).GetAttribute("value"),
                "pop up CityMobilePhone");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"CityMain\"] input")).Selected,
                "pop up CityDisplay");
            VerifyAreEqual("789", Driver.FindElement(By.CssSelector("[data-e2e=\"CitySort\"]")).GetAttribute("value"),
                "pop up CitySortOrder");
        }

        [Test]
        public void CityOpenAddRegion()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Driver.GetGridCell(0, "Name", "Region").Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCity\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"CityName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityName\"]")).SendKeys("NewCityRegion");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityPhone\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityPhone\"]")).SendKeys("777777");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityMobilePhone\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CityMobilePhone\"]")).SendKeys("888888");


            Driver.FindElement(By.CssSelector("[data-e2e=\"CitySort\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CitySort\"]")).SendKeys("10");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"CityRegion\"]")))).SelectByText(
                "TestRegion100");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Driver.GridFilterSendKeys("NewCityRegion");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            Driver.FindElement(By.CssSelector("[data-e2e=\"GoToRegion\"]")).Click();
            Driver.GridFilterSendKeys("TestRegion100");

            VerifyAreEqual("TestRegion100", Driver.GetGridCell(0, "Name", "Region").Text, "grid name");
            Driver.GetGridCell(0, "Name", "Region").Click();

            Driver.GridFilterSendKeys("NewCityRegion");

            VerifyAreEqual("NewCityRegion", Driver.GetGridCell(0, "Name", "City").Text, "grid name");
            VerifyAreEqual("777777", Driver.GetGridCell(0, "PhoneNumber", "City").Text, "grid CityPhone");
            VerifyAreEqual("888888", Driver.GetGridCell(0, "MobilePhoneNumber", "City").Text, "grid MobilePhoneNumber");
            VerifyAreEqual("10", Driver.GetGridCell(0, "CitySort", "City").Text, "grid SortOrder");
            VerifyIsFalse(Driver.GetGridCell(0, "DisplayInPopup", "City").FindElement(By.TagName("input")).Selected,
                "grid DisplayInPopup");

            Driver.GetGridCell(0, "_serviceColumn", "City").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Редактирование города", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("NewCityRegion",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityName\"]")).GetAttribute("value"), "pop up name");
            VerifyAreEqual("777777",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityPhone\"]")).GetAttribute("value"),
                "pop up CityPhone");
            VerifyAreEqual("888888",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CityMobilePhone\"]")).GetAttribute("value"),
                "pop up CityMobilePhone");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"CityMain\"] input")).Selected,
                "pop up CityDisplay");
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"CitySort\"]")).GetAttribute("value"),
                "pop up CitySortOrder");
        }
    }
}