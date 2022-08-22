using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.Country
{
    [TestFixture]
    public class CountriesAddEditTest : BaseSeleniumTest
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
        public void CountrieOpenEdit()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Driver.GetGridCell(0, "_serviceColumn", "Country").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Редактирование страны", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("TestCountry1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountryName\"]")).GetAttribute("value"), "pop up name");
            VerifyAreEqual("AA", Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso2\"]")).GetAttribute("value"),
                "pop up CountryIso2");
            VerifyAreEqual("AA1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso3\"]")).GetAttribute("value"),
                "pop up CountryIso3");

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"CountryDisplay\"] input")).Selected,
                "pop up CountryDisplay");
            VerifyAreEqual("111",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountryCode\"]")).GetAttribute("value"),
                "pop up CountryCode");
            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountrySortOrder\"]")).GetAttribute("value"),
                "pop up CountrySortOrder");
        }

        [Test]
        public void CountriesEdit()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Driver.GridFilterSendKeys("TestCountry100");

            Driver.GetGridCell(0, "_serviceColumn", "Country").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryName\"]")).SendKeys("NewName");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso2\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso2\"]")).SendKeys("QQ");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso3\"]")).SendKeys("QQQ");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryDisplay\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryCode\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryCode\"]")).SendKeys("999");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountrySortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountrySortOrder\"]")).SendKeys("888");

            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            Driver.GridFilterSendKeys("NewName");

            VerifyAreEqual("NewName", Driver.GetGridCell(0, "Name", "Country").Text, "grid name");
            VerifyAreEqual("QQ", Driver.GetGridCell(0, "Iso2", "Country").Text, "grid Iso2");
            VerifyAreEqual("QQQ", Driver.GetGridCell(0, "Iso3", "Country").Text, "grid Iso3");
            VerifyAreEqual("999", Driver.GetGridCell(0, "DialCode", "Country").Text, "grid DialCode");
            VerifyAreEqual("888", Driver.GetGridCell(0, "SortOrder", "Country").Text, "grid SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "DisplayInPopup", "Country").FindElement(By.TagName("input")).Selected,
                "grid DisplayInPopup");

            Driver.GetGridCell(0, "_serviceColumn", "Country").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Редактирование страны", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("NewName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountryName\"]")).GetAttribute("value"), "npop up ame");
            VerifyAreEqual("QQ", Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso2\"]")).GetAttribute("value"),
                "pop up CountryIso2");
            VerifyAreEqual("QQQ",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso3\"]")).GetAttribute("value"),
                "pop up CountryIso3");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"CountryDisplay\"] input")).Selected,
                "CountryDisplay");
            VerifyAreEqual("999",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountryCode\"]")).GetAttribute("value"),
                "pop up CountryCode");
            VerifyAreEqual("888",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountrySortOrder\"]")).GetAttribute("value"),
                "pop up CountrySortOrder");
        }

        [Test]
        public void CountriesOpenAdd()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCountry\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("Добавление страны", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"CountryName\"]")).GetAttribute("value"),
                "name");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso2\"]")).GetAttribute("value"),
                "CountryIso2");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso3\"]")).GetAttribute("value"),
                "CountryIso3");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"CountryDisplay\"] input")).Selected,
                "CountryDisplay");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"CountryCode\"]")).GetAttribute("value"),
                "CountryCode");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountrySortOrder\"]")).GetAttribute("value"),
                "CountrySortOrder");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryName\"]")).SendKeys("NewCountry");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso2\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso2\"]")).SendKeys("WW");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso3\"]")).SendKeys("WWW");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryDisplay\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryCode\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountryCode\"]")).SendKeys("987");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountrySortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CountrySortOrder\"]")).SendKeys("789");

            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Driver.GridFilterSendKeys("NewCountry");

            VerifyAreEqual("NewCountry", Driver.GetGridCell(0, "Name", "Country").Text, "grid name");
            VerifyAreEqual("WW", Driver.GetGridCell(0, "Iso2", "Country").Text, "grid Iso2");
            VerifyAreEqual("WWW", Driver.GetGridCell(0, "Iso3", "Country").Text, "grid Iso3");
            VerifyAreEqual("987", Driver.GetGridCell(0, "DialCode", "Country").Text, "grid DialCode");
            VerifyAreEqual("789", Driver.GetGridCell(0, "SortOrder", "Country").Text, "grid SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "DisplayInPopup", "Country").FindElement(By.TagName("input")).Selected,
                "grid DisplayInPopup");

            Driver.GetGridCell(0, "_serviceColumn", "Country").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Редактирование страны", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            VerifyAreEqual("NewCountry",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountryName\"]")).GetAttribute("value"), "pop up name");
            VerifyAreEqual("WW", Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso2\"]")).GetAttribute("value"),
                "pop up CountryIso2");
            VerifyAreEqual("WWW",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountryIso3\"]")).GetAttribute("value"),
                "pop up CountryIso3");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"CountryDisplay\"] input")).Selected,
                "pop up CountryDisplay");
            VerifyAreEqual("987",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountryCode\"]")).GetAttribute("value"),
                "pop up CountryCode");
            VerifyAreEqual("789",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CountrySortOrder\"]")).GetAttribute("value"),
                "pop up CountrySortOrder");
        }
    }
}