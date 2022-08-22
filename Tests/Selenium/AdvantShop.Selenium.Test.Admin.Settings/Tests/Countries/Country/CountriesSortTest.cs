using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.Country
{
    [TestFixture]
    public class CountriesSortTest : BaseSeleniumTest
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
            GoToAdmin("settingssystem#?systemTab=countries");
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
        public void CountriesSortName()
        {
            Driver.GetGridCell(-1, "Name", "Country").Click();
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "sort Name 1 asc");
            VerifyAreEqual("TestCountry16", Driver.GetGridCell(9, "Name", "Country").Text, "sort Name 10 asc");

            Driver.GetGridCell(-1, "Name", "Country").Click();
            VerifyAreEqual("TestCountry99", Driver.GetGridCell(0, "Name", "Country").Text, "sort Name 1 desc");
            VerifyAreEqual("TestCountry90", Driver.GetGridCell(9, "Name", "Country").Text, "sort Name 10 desc");
        }

        [Test]
        public void CountriesSortIso2()
        {
            Driver.GetGridCell(-1, "Iso2", "Country").Click();
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "sort Iso2 1 asc");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "sort Iso2 10 asc");

            Driver.GetGridCell(-1, "Iso2", "Country").Click();
            VerifyAreEqual("TestCountry101", Driver.GetGridCell(0, "Name", "Country").Text, "sort Iso2 1 desc");
            VerifyAreEqual("TestCountry92", Driver.GetGridCell(9, "Name", "Country").Text, "sort Iso2 10 desc");
        }

        [Test]
        public void CountriesSortIso3()
        {
            Driver.GetGridCell(-1, "Iso3", "Country").Click();
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "sort Iso3 1 asc");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "sort Iso3 10 asc");

            Driver.GetGridCell(-1, "Iso3", "Country").Click();
            VerifyAreEqual("TestCountry101", Driver.GetGridCell(0, "Name", "Country").Text, "sort Iso3 1 desc");
            VerifyAreEqual("TestCountry92", Driver.GetGridCell(9, "Name", "Country").Text, "sort Iso3 10 desc");
        }


        [Test]
        public void CountriesSortDisplay()
        {
            Driver.GetGridCell(-1, "DisplayInPopup", "Country").Click();
            VerifyAreEqual("TestCountry7", Driver.GetGridCell(0, "Name", "Country").Text, "sort DisplayInPopup 1 asc");
            VerifyAreEqual("TestCountry16", Driver.GetGridCell(9, "Name", "Country").Text,
                "sort DisplayInPopup 10 asc");

            Driver.GetGridCell(-1, "DisplayInPopup", "Country").Click();
            VerifyAreEqual("TestCountry2", Driver.GetGridCell(0, "Name", "Country").Text, "sort DisplayInPopup 1 desc");
            VerifyAreEqual("TestCountry11", Driver.GetGridCell(9, "Name", "Country").Text,
                "sort DisplayInPopup 10 desc");
        }


        [Test]
        public void CountriesSortCode()
        {
            Driver.GetGridCell(-1, "DialCode", "Country").Click();
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "sort DialCode 1 asc");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "sort DialCode 10 asc");

            Driver.GetGridCell(-1, "DialCode", "Country").Click();
            VerifyAreEqual("TestCountry101", Driver.GetGridCell(0, "Name", "Country").Text, "sort DialCode 1 desc");
            VerifyAreEqual("TestCountry92", Driver.GetGridCell(9, "Name", "Country").Text, "sort DialCode 10 desc");
        }

        [Test]
        public void CountriesSortSortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder", "Country").Click();
            VerifyAreEqual("TestCountry1", Driver.GetGridCell(0, "Name", "Country").Text, "sort SortOrder 1 asc");
            VerifyAreEqual("TestCountry10", Driver.GetGridCell(9, "Name", "Country").Text, "sort SortOrder 10 asc");

            Driver.GetGridCell(-1, "SortOrder", "Country").Click();
            VerifyAreEqual("TestCountry101", Driver.GetGridCell(0, "Name", "Country").Text, "sort SortOrder 1 desc");
            VerifyAreEqual("TestCountry92", Driver.GetGridCell(9, "Name", "Country").Text, "sort SortOrder 10 desc");
        }
    }
}