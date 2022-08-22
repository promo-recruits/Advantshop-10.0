using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.Region
{
    [TestFixture]
    public class SettingsRegionPageTest : BaseSeleniumTest
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
        public void PageRegion()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "line 1");
            VerifyAreEqual("TestRegion10", Driver.GetGridCell(9, "Name", "Region").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion11", Driver.GetGridCell(0, "Name", "Region").Text, "line 11");
            VerifyAreEqual("TestRegion20", Driver.GetGridCell(9, "Name", "Region").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion21", Driver.GetGridCell(0, "Name", "Region").Text, "line 21");
            VerifyAreEqual("TestRegion30", Driver.GetGridCell(9, "Name", "Region").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion31", Driver.GetGridCell(0, "Name", "Region").Text, "line 31");
            VerifyAreEqual("TestRegion40", Driver.GetGridCell(9, "Name", "Region").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion41", Driver.GetGridCell(0, "Name", "Region").Text, "line 41");
            VerifyAreEqual("TestRegion50", Driver.GetGridCell(9, "Name", "Region").Text, "line 50");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion51", Driver.GetGridCell(0, "Name", "Region").Text, "line 51");
            VerifyAreEqual("TestRegion60", Driver.GetGridCell(9, "Name", "Region").Text, "line 60");
        }

        [Test]
        public void PageRegionToBegin()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "line 1");
            VerifyAreEqual("TestRegion10", Driver.GetGridCell(9, "Name", "Region").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion11", Driver.GetGridCell(0, "Name", "Region").Text, "line 11");
            VerifyAreEqual("TestRegion20", Driver.GetGridCell(9, "Name", "Region").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion21", Driver.GetGridCell(0, "Name", "Region").Text, "line 21");
            VerifyAreEqual("TestRegion30", Driver.GetGridCell(9, "Name", "Region").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion31", Driver.GetGridCell(0, "Name", "Region").Text, "line 31");
            VerifyAreEqual("TestRegion40", Driver.GetGridCell(9, "Name", "Region").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion41", Driver.GetGridCell(0, "Name", "Region").Text, "line 41");
            VerifyAreEqual("TestRegion50", Driver.GetGridCell(9, "Name", "Region").Text, "line 50");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "line 1");
            VerifyAreEqual("TestRegion10", Driver.GetGridCell(9, "Name", "Region").Text, "line 10");
        }

        [Test]
        public void PageRegionToEnd()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "line 1");
            VerifyAreEqual("TestRegion10", Driver.GetGridCell(9, "Name", "Region").Text, "line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion101", Driver.GetGridCell(0, "Name", "Region").Text, "line 101");
        }

        [Test]
        public void PageRegionToNext()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "line 1");
            VerifyAreEqual("TestRegion10", Driver.GetGridCell(9, "Name", "Region").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion11", Driver.GetGridCell(0, "Name", "Region").Text, "line 11");
            VerifyAreEqual("TestRegion20", Driver.GetGridCell(9, "Name", "Region").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion21", Driver.GetGridCell(0, "Name", "Region").Text, "line 21");
            VerifyAreEqual("TestRegion30", Driver.GetGridCell(9, "Name", "Region").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion31", Driver.GetGridCell(0, "Name", "Region").Text, "line 31");
            VerifyAreEqual("TestRegion40", Driver.GetGridCell(9, "Name", "Region").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion41", Driver.GetGridCell(0, "Name", "Region").Text, "line 41");
            VerifyAreEqual("TestRegion50", Driver.GetGridCell(9, "Name", "Region").Text, "line 50");
        }

        [Test]
        public void PageRegionToPrevious()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "line 1");
            VerifyAreEqual("TestRegion10", Driver.GetGridCell(9, "Name", "Region").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion11", Driver.GetGridCell(0, "Name", "Region").Text, "line 11");
            VerifyAreEqual("TestRegion20", Driver.GetGridCell(9, "Name", "Region").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion21", Driver.GetGridCell(0, "Name", "Region").Text, "line 21");
            VerifyAreEqual("TestRegion30", Driver.GetGridCell(9, "Name", "Region").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion11", Driver.GetGridCell(0, "Name", "Region").Text, "line 11");
            VerifyAreEqual("TestRegion20", Driver.GetGridCell(9, "Name", "Region").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "line 1");
            VerifyAreEqual("TestRegion10", Driver.GetGridCell(9, "Name", "Region").Text, "line 10");
        }

        [Test]
        public void RegionPresent()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "find elem 101");
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "line 1 10");
            VerifyAreEqual("TestRegion10", Driver.GetGridCell(9, "Name", "Region").Text, "line 10");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "line 1 100");
            VerifyAreEqual("TestRegion100", Driver.GetGridCell(99, "Name", "Region").Text, "line 100");

            Driver.GridPaginationSelectItems("10");
        }
    }
}