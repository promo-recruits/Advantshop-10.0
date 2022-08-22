using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Subscription
{
    [TestFixture]
    public class SubscriptionSortTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Subscription\\Customers.CustomerGroup.csv",
                "data\\Admin\\Subscription\\Customers.Customer.csv",
                "data\\Admin\\Subscription\\Customers.Contact.csv",
                "data\\Admin\\Subscription\\Customers.Departments.csv",
                "data\\Admin\\Subscription\\Customers.Managers.csv",
                "data\\Admin\\Subscription\\Customers.CustomerField.csv",
                "data\\Admin\\Subscription\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\Subscription\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\Subscription\\Customers.Subscription.csv"
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
        public void SubscriptionSortByEmail()
        {
            GoToAdmin("subscription");

            Driver.GetGridCell(-1, "Email").Click();

            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "subscription 1 Email asc");
            VerifyAreEqual("testmail18@test.ru", Driver.GetGridCell(9, "Email").Text, "subscription 10 Email asc");

            Driver.GetGridCell(-1, "Email").Click();

            VerifyAreEqual("testmail9@test.ru", Driver.GetGridCell(0, "Email").Text, "subscription 1 Email desc");
            VerifyAreEqual("testmail20@test.ru", Driver.GetGridCell(9, "Email").Text, "subscription 10 Email desc");
        }

        [Test]
        public void SubscriptionSortByEnabled()
        {
            GoToAdmin("subscription");

            Driver.GetGridCell(-1, "Enabled").Click();

            VerifyAreEqual("testmail11@test.ru", Driver.GetGridCell(0, "Email").Text, "subscription 1 Enabled asc");
            VerifyAreEqual("testmail20@test.ru", Driver.GetGridCell(9, "Email").Text, "subscription 10 Enabled asc");

            Driver.GetGridCell(-1, "Enabled").Click();

            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "subscription 1 Enabled desc");
            VerifyAreEqual("testmail10@test.ru", Driver.GetGridCell(9, "Email").Text, "subscription 10 Enabled desc");
        }

        [Test]
        public void SubscriptionSortBySubDate()
        {
            GoToAdmin("subscription");

            Driver.GetGridCell(-1, "SubscribeDateStr").Click();

            VerifyAreEqual("testmail22@test.ru", Driver.GetGridCell(0, "Email").Text, "subscription 1 SubDate asc");
            VerifyAreEqual("testmail13@test.ru", Driver.GetGridCell(9, "Email").Text, "subscription 10 SubDate asc");

            Driver.GetGridCell(-1, "SubscribeDateStr").Click();

            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "subscription 1 SubDate desc");
            VerifyAreEqual("testmail10@test.ru", Driver.GetGridCell(9, "Email").Text, "subscription 10 SubDate desc");
        }

        [Test]
        public void SubscriptionSortByUnsubDate()
        {
            GoToAdmin("subscription");

            Driver.GetGridCell(-1, "UnsubscribeDateStr").Click();

            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "subscription 1 UnsubDate asc");
            VerifyAreEqual("testmail10@test.ru", Driver.GetGridCell(9, "Email").Text, "subscription 10 UnsubDate asc");

            Driver.GetGridCell(-1, "UnsubscribeDateStr").Click();

            VerifyAreEqual("testmail22@test.ru", Driver.GetGridCell(0, "Email").Text, "subscription 1 UnsubDate desc");
            VerifyAreEqual("testmail13@test.ru", Driver.GetGridCell(9, "Email").Text, "subscription 10 UnsubDate desc");
        }
    }
}