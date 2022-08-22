using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.City
{
    [TestFixture]
    public class SettingsCityPageTest : BaseSeleniumTest
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
        public void PageCity()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "line 1");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(9, "Name", "City").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity11", Driver.GetGridCell(0, "Name", "City").Text, "line 11");
            VerifyAreEqual("TestCity20", Driver.GetGridCell(9, "Name", "City").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity21", Driver.GetGridCell(0, "Name", "City").Text, "line 21");
            VerifyAreEqual("TestCity30", Driver.GetGridCell(9, "Name", "City").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity31", Driver.GetGridCell(0, "Name", "City").Text, "line 31");
            VerifyAreEqual("TestCity40", Driver.GetGridCell(9, "Name", "City").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity41", Driver.GetGridCell(0, "Name", "City").Text, "line 41");
            VerifyAreEqual("TestCity50", Driver.GetGridCell(9, "Name", "City").Text, "line 50");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity51", Driver.GetGridCell(0, "Name", "City").Text, "line 51");
            VerifyAreEqual("TestCity60", Driver.GetGridCell(9, "Name", "City").Text, "line 60");
        }

        [Test]
        public void PageCityToBegin()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "line 1");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(9, "Name", "City").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity11", Driver.GetGridCell(0, "Name", "City").Text, "line 11");
            VerifyAreEqual("TestCity20", Driver.GetGridCell(9, "Name", "City").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity21", Driver.GetGridCell(0, "Name", "City").Text, "line 21");
            VerifyAreEqual("TestCity30", Driver.GetGridCell(9, "Name", "City").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity31", Driver.GetGridCell(0, "Name", "City").Text, "line 31");
            VerifyAreEqual("TestCity40", Driver.GetGridCell(9, "Name", "City").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity41", Driver.GetGridCell(0, "Name", "City").Text, "line 41");
            VerifyAreEqual("TestCity50", Driver.GetGridCell(9, "Name", "City").Text, "line 50");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "line 1");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(9, "Name", "City").Text, "line 10");
        }

        [Test]
        public void PageCityToEnd()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "line 1");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(9, "Name", "City").Text, "line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity101", Driver.GetGridCell(0, "Name", "City").Text, "line 101");
        }

        [Test]
        public void PageCityToNext()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "line 1");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(9, "Name", "City").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity11", Driver.GetGridCell(0, "Name", "City").Text, "line 11");
            VerifyAreEqual("TestCity20", Driver.GetGridCell(9, "Name", "City").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity21", Driver.GetGridCell(0, "Name", "City").Text, "line 21");
            VerifyAreEqual("TestCity30", Driver.GetGridCell(9, "Name", "City").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity31", Driver.GetGridCell(0, "Name", "City").Text, "line 31");
            VerifyAreEqual("TestCity40", Driver.GetGridCell(9, "Name", "City").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity41", Driver.GetGridCell(0, "Name", "City").Text, "line 41");
            VerifyAreEqual("TestCity50", Driver.GetGridCell(9, "Name", "City").Text, "line 50");
        }

        [Test]
        public void PageCityToPrevious()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "line 1");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(9, "Name", "City").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity11", Driver.GetGridCell(0, "Name", "City").Text, "line 11");
            VerifyAreEqual("TestCity20", Driver.GetGridCell(9, "Name", "City").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity21", Driver.GetGridCell(0, "Name", "City").Text, "line 21");
            VerifyAreEqual("TestCity30", Driver.GetGridCell(9, "Name", "City").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity11", Driver.GetGridCell(0, "Name", "City").Text, "line 11");
            VerifyAreEqual("TestCity20", Driver.GetGridCell(9, "Name", "City").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "line 1");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(9, "Name", "City").Text, "line 10");
        }

        [Test]
        public void CityPresent()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            Driver.GetGridCell(0, "Name", "Region").Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "find elem 101");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "line 1 10");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(9, "Name", "City").Text, "line 10");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "line 1 100");
            VerifyAreEqual("TestCity100", Driver.GetGridCell(99, "Name", "City").Text, "line 100");

            Driver.GridPaginationSelectItems("10");
        }
    }
}