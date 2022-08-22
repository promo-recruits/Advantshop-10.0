using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.Region
{
    [TestFixture]
    public class SettingsRegionSortTest : BaseSeleniumTest
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
            GoToAdmin("settingssystem#?systemTab=countries");
            Driver.GetGridCell(0, "Name", "Country").Click();
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
        public void RegionSortName()
        {
            Driver.GetGridCell(-1, "Name", "Region").Click();
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "sort Name 1 asc");
            VerifyAreEqual("TestRegion16", Driver.GetGridCell(9, "Name", "Region").Text, "sort Name 10 asc");

            Driver.GetGridCell(-1, "Name", "Region").Click();
            VerifyAreEqual("TestRegion99", Driver.GetGridCell(0, "Name", "Region").Text, "sort Name 1 desc");
            VerifyAreEqual("TestRegion90", Driver.GetGridCell(9, "Name", "Region").Text, "sort Name 10 desc");
        }

        [Test]
        public void RegionSortCode()
        {
            Driver.GetGridCell(-1, "RegionCode", "Region").Click();
            VerifyAreEqual("TestRegion90", Driver.GetGridCell(0, "Name", "Region").Text, "sort Code 1 asc");
            VerifyAreEqual("TestRegion99", Driver.GetGridCell(9, "Name", "Region").Text, "sort Code 10 asc");

            Driver.GetGridCell(-1, "RegionCode", "Region").Click();
            VerifyAreEqual("TestRegion89", Driver.GetGridCell(0, "Name", "Region").Text, "sort Code 1 desc");
            VerifyAreEqual("TestRegion80", Driver.GetGridCell(9, "Name", "Region").Text, "sort Code 10 desc");
        }

        [Test]
        public void RegionSortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder", "Region").Click();
            VerifyAreEqual("TestRegion1", Driver.GetGridCell(0, "Name", "Region").Text, "sort SortOrder 1 asc");
            VerifyAreEqual("TestRegion10", Driver.GetGridCell(9, "Name", "Region").Text, "sort SortOrder 10 asc");

            Driver.GetGridCell(-1, "SortOrder", "Region").Click();
            VerifyAreEqual("TestRegion101", Driver.GetGridCell(0, "Name", "Region").Text, "sort SortOrder 1 desc");
            VerifyAreEqual("TestRegion92", Driver.GetGridCell(9, "Name", "Region").Text, "sort SortOrder 10 desc");
        }
    }
}