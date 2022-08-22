using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.Country
{
    [TestFixture]
    public class CountriesPageTest : BaseSeleniumTest
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
        public void PageCounties()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "line 1");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry11", Driver.GetGridCell(0, "Name", "Country").Text, "line 11");
            VerifyAreEqual("TestCountry20", Driver.GetGridCell(9, "Name", "Country").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry21", Driver.GetGridCell(0, "Name", "Country").Text, "line 21");
            VerifyAreEqual("TestCountry30", Driver.GetGridCell(9, "Name", "Country").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry31", Driver.GetGridCell(0, "Name", "Country").Text, "line 31");
            VerifyAreEqual("TestCountry40", Driver.GetGridCell(9, "Name", "Country").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry41", Driver.GetGridCell(0, "Name", "Country").Text, "line 41");
            VerifyAreEqual("TestCountry50", Driver.GetGridCell(9, "Name", "Country").Text, "line 50");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry51", Driver.GetGridCell(0, "Name", "Country").Text, "line 51");
            VerifyAreEqual("TestCountry60", Driver.GetGridCell(9, "Name", "Country").Text, "line 60");
        }

        [Test]
        public void PageCountiesToBegin()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "line 1");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry11", Driver.GetGridCell(0, "Name", "Country").Text, "line 11");
            VerifyAreEqual("TestCountry20", Driver.GetGridCell(9, "Name", "Country").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry21", Driver.GetGridCell(0, "Name", "Country").Text, "line 21");
            VerifyAreEqual("TestCountry30", Driver.GetGridCell(9, "Name", "Country").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry31", Driver.GetGridCell(0, "Name", "Country").Text, "line 31");
            VerifyAreEqual("TestCountry40", Driver.GetGridCell(9, "Name", "Country").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry41", Driver.GetGridCell(0, "Name", "Country").Text, "line 41");
            VerifyAreEqual("TestCountry50", Driver.GetGridCell(9, "Name", "Country").Text, "line 50");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "line 1");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "line 10");
        }

        [Test]
        public void PageCountiesToEnd()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "line 1");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry101", Driver.GetGridCell(0, "Name", "Country").Text, "line 101");
        }

        [Test]
        public void PageCountiesToNext()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "line 1");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry11", Driver.GetGridCell(0, "Name", "Country").Text, "line 11");
            VerifyAreEqual("TestCountry20", Driver.GetGridCell(9, "Name", "Country").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry21", Driver.GetGridCell(0, "Name", "Country").Text, "line 21");
            VerifyAreEqual("TestCountry30", Driver.GetGridCell(9, "Name", "Country").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry31", Driver.GetGridCell(0, "Name", "Country").Text, "line 31");
            VerifyAreEqual("TestCountry40", Driver.GetGridCell(9, "Name", "Country").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry41", Driver.GetGridCell(0, "Name", "Country").Text, "line 41");
            VerifyAreEqual("TestCountry50", Driver.GetGridCell(9, "Name", "Country").Text, "line 50");
        }

        [Test]
        public void PageCountiesToPrevious()
        {
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "line 1");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry11", Driver.GetGridCell(0, "Name", "Country").Text, "line 11");
            VerifyAreEqual("TestCountry20", Driver.GetGridCell(9, "Name", "Country").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry21", Driver.GetGridCell(0, "Name", "Country").Text, "line 21");
            VerifyAreEqual("TestCountry30", Driver.GetGridCell(9, "Name", "Country").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry11", Driver.GetGridCell(0, "Name", "Country").Text, "line 11");
            VerifyAreEqual("TestCountry20", Driver.GetGridCell(9, "Name", "Country").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "line 1");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "line 10");
        }

        [Test]
        public void CountiesPresent()
        {
            GoToAdmin("settingssystem#?systemTab=countries");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "find elem 101");
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "line 1 10");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "line 10");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "line 1 100");
            VerifyAreEqual("TestCountry100", Driver.GetGridCell(99, "Name", "Country").Text, "line 100");

            Driver.GridPaginationSelectItems("10");
        }
    }
}