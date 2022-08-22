using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.City
{
    [TestFixture]
    public class SettingsCitySearchTest : BaseSeleniumTest
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
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
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
        public void CitySearchNotexist()
        {
            //search not exist product
            Driver.GridFilterSendKeys("TestCity000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CitySearchMuch()
        {
            //search too much symbols
            Driver.GridFilterSendKeys("1111111111222222222223333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CitySearchInvalid()
        {
            //search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CitySearchExist()
        {
            Driver.GridFilterSendKeys("TestCity10");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(0, "Name", "City").Text, "find value");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}