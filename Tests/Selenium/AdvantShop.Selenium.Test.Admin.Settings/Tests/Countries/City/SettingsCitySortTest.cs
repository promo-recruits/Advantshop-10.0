using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Countries.City
{
    [TestFixture]
    public class SettingsCitySortTest : BaseSeleniumTest
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
        public void CitySortDisplay()
        {
            Driver.GetGridCell(-1, "DisplayInPopup", "City").Click();
            VerifyAreEqual("TestCity15", Driver.GetGridCell(0, "Name", "City").Text, "sort DisplayInPopup 1 asc");
            VerifyAreEqual("TestCity24", Driver.GetGridCell(9, "Name", "City").Text, "sort DisplayInPopup 10 asc");

            Driver.GetGridCell(-1, "DisplayInPopup", "City").Click();
            VerifyAreEqual("TestCity10", Driver.GetGridCell(0, "Name", "City").Text, "sort DisplayInPopup 1 desc");
            VerifyAreEqual("TestCity19", Driver.GetGridCell(9, "Name", "City").Text, "sort DisplayInPopup 10 desc");
        }

        [Test]
        public void CitySortMobile()
        {
            Driver.GetGridCell(-1, "MobilePhoneNumber", "City").Click();
            VerifyAreEqual("TestCity101", Driver.GetGridCell(0, "Name", "City").Text, "sort MobilePhoneNumber 1 asc");
            VerifyAreEqual("TestCity92", Driver.GetGridCell(9, "Name", "City").Text, "sort MobilePhoneNumber 10 asc");

            Driver.GetGridCell(-1, "MobilePhoneNumber", "City").Click();
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "sort MobilePhoneNumber 1 desc");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(9, "Name", "City").Text, "sort MobilePhoneNumber 10 desc");
        }

        [Test]
        public void CitySortName()
        {
            Driver.GetGridCell(-1, "Name", "City").Click();
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "sort Name 1 asc");
            VerifyAreEqual("TestCity16", Driver.GetGridCell(9, "Name", "City").Text, "sort Name 10 asc");

            Driver.GetGridCell(-1, "Name", "City").Click();
            VerifyAreEqual("TestCity99", Driver.GetGridCell(0, "Name", "City").Text, "sort Name 1 desc");
            VerifyAreEqual("TestCity90", Driver.GetGridCell(9, "Name", "City").Text, "sort Name 10 desc");
        }

        [Test]
        public void CitySortPhone()
        {
            Driver.GetGridCell(-1, "PhoneNumber", "City").Click();
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "sort PhoneNumber 1 asc");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(9, "Name", "City").Text, "sort PhoneNumber 10 asc");

            Driver.GetGridCell(-1, "PhoneNumber", "City").Click();
            VerifyAreEqual("TestCity101", Driver.GetGridCell(0, "Name", "City").Text, "sort PhoneNumber 1 desc");
            VerifyAreEqual("TestCity92", Driver.GetGridCell(9, "Name", "City").Text, "sort PhoneNumber 10 desc");
        }


        [Test]
        public void CitySortSortOrder()
        {
            Driver.GetGridCell(-1, "CitySort", "City").Click();
            VerifyAreEqual("TestCity1", Driver.GetGridCell(0, "Name", "City").Text, "sort CitySort 1 asc");
            VerifyAreEqual("TestCity10", Driver.GetGridCell(9, "Name", "City").Text, "sort CitySort 10 asc");

            Driver.GetGridCell(-1, "CitySort", "City").Click();
            VerifyAreEqual("TestCity101", Driver.GetGridCell(0, "Name", "City").Text, "sort CitySort 1 desc");
            VerifyAreEqual("TestCity92", Driver.GetGridCell(9, "Name", "City").Text, "sort CitySort 10 desc");
        }
    }
}