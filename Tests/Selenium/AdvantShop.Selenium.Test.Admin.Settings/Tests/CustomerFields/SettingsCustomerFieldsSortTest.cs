using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields
{
    [TestFixture]
    public class SettingsCustomerFieldsSortTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\CustomerFields\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\CustomerFields\\Customers.Customer.csv",
                "data\\Admin\\Settings\\CustomerFields\\Customers.CustomerField.csv",
                "data\\Admin\\Settings\\CustomerFields\\Customers.CustomerFieldValue.csv"
            );

            Init();

            GoToAdmin("settingscustomers#?tab=customerFields");
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

        string customerNameFirstStr = "";
        string customerNameLastStr = "";

        [Test]
        public void SortName()
        {
            Driver.GetGridCell(-1, "Name", "CustomerFields").Click();
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "sort Name 1 asc");
            VerifyAreEqual("Customer Field 107", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "sort Name 10 asc");

            Driver.GetGridCell(-1, "Name", "CustomerFields").Click();
            VerifyAreEqual("Customer Field 99", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "sort Name 1 desc");
            VerifyAreEqual("Customer Field 90", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "sort Name 10 desc");
        }

        [Test]
        public void SortFieldType()
        {
            Driver.GetGridCell(-1, "FieldTypeFormatted", "CustomerFields").Click();
            VerifyAreEqual("Выбор", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text,
                "sort type 1 asc");
            VerifyAreEqual("Текстовое поле", Driver.GetGridCell(9, "FieldTypeFormatted", "CustomerFields").Text,
                "sort type 10 asc");

            customerNameFirstStr = Driver.GetGridCell(0, "Name", "CustomerFields").Text;
            customerNameLastStr = Driver.GetGridCell(9, "Name", "CustomerFields").Text;

            VerifyIsFalse(customerNameFirstStr.Equals(customerNameLastStr), "sort type diff names asc");

            Driver.GetGridCell(-1, "FieldTypeFormatted", "CustomerFields").Click();
            VerifyAreEqual("Многострочное текстовое поле",
                Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text, "sort type 1 desc");
            VerifyAreEqual("Многострочное текстовое поле",
                Driver.GetGridCell(9, "FieldTypeFormatted", "CustomerFields").Text, "sort type 10 desc");

            customerNameFirstStr = Driver.GetGridCell(0, "Name", "CustomerFields").Text;
            customerNameLastStr = Driver.GetGridCell(9, "Name", "CustomerFields").Text;

            VerifyIsFalse(customerNameFirstStr.Equals(customerNameLastStr), "sort type diff names desc");
        }

        [Test]
        public void SortRequired()
        {
            Driver.GetGridCell(-1, "Required", "CustomerFields").Click();
            VerifyIsFalse(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort Required 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort Required 10 asc");

            customerNameFirstStr = Driver.GetGridCell(0, "Name", "CustomerFields").Text;
            customerNameLastStr = Driver.GetGridCell(9, "Name", "CustomerFields").Text;

            VerifyIsFalse(customerNameFirstStr.Equals(customerNameLastStr), "sort Required diff names asc");

            Driver.GetGridCell(-1, "Required", "CustomerFields").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort Required 1 desc");
            VerifyIsTrue(Driver.GetGridCell(9, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort Required 10 desc");

            customerNameFirstStr = Driver.GetGridCell(0, "Name", "CustomerFields").Text;
            customerNameLastStr = Driver.GetGridCell(9, "Name", "CustomerFields").Text;

            VerifyIsFalse(customerNameFirstStr.Equals(customerNameLastStr), "sort Required diff names desc");
        }

        [Test]
        public void SortSortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder", "CustomerFields").Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "sort SortOrder 1 asc");
            VerifyAreEqual("10", Driver.GetGridCell(9, "SortOrder", "CustomerFields").Text, "sort SortOrder 10 asc");

            Driver.GetGridCell(-1, "SortOrder", "CustomerFields").Click();
            VerifyAreEqual("150", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "sort SortOrder 1 desc");
            VerifyAreEqual("141", Driver.GetGridCell(9, "SortOrder", "CustomerFields").Text, "sort SortOrder 10 desc");
        }

        [Test]
        public void SortEnabled()
        {
            Driver.GetGridCell(-1, "Enabled", "CustomerFields").Click();
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort Enabled 1 asc");
            VerifyIsFalse(Driver.GetGridCell(9, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort Enabled 10 asc");

            customerNameFirstStr = Driver.GetGridCell(0, "Name", "CustomerFields").Text;
            customerNameLastStr = Driver.GetGridCell(9, "Name", "CustomerFields").Text;

            VerifyIsFalse(customerNameFirstStr.Equals(customerNameLastStr), "sort Enabled diff names asc");

            Driver.GetGridCell(-1, "Enabled", "CustomerFields").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort Enabled 1 desc");
            VerifyIsTrue(Driver.GetGridCell(9, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort Enabled 10 desc");

            customerNameFirstStr = Driver.GetGridCell(0, "Name", "CustomerFields").Text;
            customerNameLastStr = Driver.GetGridCell(9, "Name", "CustomerFields").Text;

            VerifyIsFalse(customerNameFirstStr.Equals(customerNameLastStr), "sort Enabled diff names desc");
        }

        [Test]
        public void SortShowInCheckout()
        {
            Driver.GetGridCell(-1, "ShowInCheckout", "CustomerFields").Click();
            VerifyIsFalse(
                Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort ShowInCheckout 1 asc");
            VerifyIsFalse(
                Driver.GetGridCell(9, "ShowInCheckout", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort ShowInCheckout 10 asc");

            customerNameFirstStr = Driver.GetGridCell(0, "Name", "CustomerFields").Text;
            customerNameLastStr = Driver.GetGridCell(9, "Name", "CustomerFields").Text;

            VerifyIsFalse(customerNameFirstStr.Equals(customerNameLastStr), "sort ShowInCheckout diff names asc");

            Driver.GetGridCell(-1, "ShowInCheckout", "CustomerFields").Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort ShowInCheckout 1 desc");
            VerifyIsTrue(
                Driver.GetGridCell(9, "ShowInCheckout", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort ShowInCheckout 10 desc");

            customerNameFirstStr = Driver.GetGridCell(0, "Name", "CustomerFields").Text;
            customerNameLastStr = Driver.GetGridCell(9, "Name", "CustomerFields").Text;

            VerifyIsFalse(customerNameFirstStr.Equals(customerNameLastStr), "sort ShowInCheckout diff names desc");
        }

        [Test]
        public void SortShowInRegistration()
        {
            Driver.GetGridCell(-1, "ShowInRegistration", "CustomerFields").Click();
            VerifyIsFalse(
                Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort ShowInRegistration 1 asc");
            VerifyIsFalse(
                Driver.GetGridCell(9, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort ShowInRegistration 10 asc");

            customerNameFirstStr = Driver.GetGridCell(0, "Name", "CustomerFields").Text;
            customerNameLastStr = Driver.GetGridCell(9, "Name", "CustomerFields").Text;

            VerifyIsFalse(customerNameFirstStr.Equals(customerNameLastStr), "sort ShowInRegistration diff names asc");

            Driver.GetGridCell(-1, "ShowInRegistration", "CustomerFields").Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort ShowInRegistration 1 desc");
            VerifyIsTrue(
                Driver.GetGridCell(9, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "sort ShowInRegistration 10 desc");

            customerNameFirstStr = Driver.GetGridCell(0, "Name", "CustomerFields").Text;
            customerNameLastStr = Driver.GetGridCell(9, "Name", "CustomerFields").Text;

            VerifyIsFalse(customerNameFirstStr.Equals(customerNameLastStr), "sort ShowInRegistration diff names desc");
        }
    }
}