using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Subscription
{
    [TestFixture]
    public class SubscriptionFilterTest : BaseSeleniumTest
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
        public void FilterzByEmail()
        {
            GoToAdmin("subscription");

            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "Email");

            //search by not exist 
            Driver.SetGridFilterValue("Email", "123123123 name customer 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Email", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Email", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist
            Driver.SetGridFilterValue("Email", "testmail2");

            VerifyAreEqual("testmail2@test.ru", Driver.GetGridCell(0, "Email").Text,
                "customer Name line 1 filter Email");
            VerifyAreEqual("testmail22@test.ru", Driver.GetGridCell(3, "Email").Text,
                "customer Name line 4 filter Email");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Email");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl, dropdIndex: 2);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Email");
            VerifyAreEqual("Найдено записей: 18",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Email deleting 1");

            GoToAdmin("subscription");
            VerifyAreEqual("Найдено записей: 18",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Email deleting 2");
        }

        [Test]
        public void FilterByEnabled()
        {
            GoToAdmin("subscription");

            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "Enabled");
            Driver.SetGridFilterSelectValue("Enabled", "Активные");
            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text,
                "customer Name line 1 filter Enabled");
            VerifyAreEqual("testmail9@test.ru", Driver.GetGridCell(9, "Email").Text,
                "customer Name line 10 filter Enabled");
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enabled");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Неактивные\"]")).Click();
            Driver.Blur();
            VerifyAreEqual("testmail11@test.ru", Driver.GetGridCell(0, "Email").Text,
                "customer Name line 1 filter Enabled");
            VerifyAreEqual("testmail20@test.ru", Driver.GetGridCell(9, "Email").Text,
                "customer Name line 10 filter Enabled");
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enabled");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "Enabled");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enabled deleting 1");

            GoToAdmin("subscription");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enabled deleting 2");
        }

        [Test]
        public void FilterBySubDate()
        {
            GoToAdmin("subscription");

            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "SubscribeDateStr");

            //check filter min not exist
            Driver.SetGridFilterRange("SubscribeDateStr", "31.12.2050 00:00", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter add date min not exist");

            //check max not exist
            Driver.SetGridFilterRange("SubscribeDateStr", "", "31.12.2050 00:00");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("SubscribeDateStr", "31.12.2050 00:00", "31.12.2050 00:00");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter add date  min/max not exist");

            //check filter add date 
            Driver.SetGridFilterRange("SubscribeDateStr", "10.07.2017 00:00", "15.07.2017 00:00");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date");
            //в хроме
            //   VerifyAreEqual("testmail7@test.ru", Driver.GetGridCell(0, "Email").Text, "customer Name line 1 filter SubscribeDate");
            //   VerifyAreEqual("testmail11@test.ru", Driver.GetGridCell(4, "Email").Text, "customer Name line 4 filter SubscribeDate");
            //в фф
            VerifyAreEqual("testmail10@test.ru", Driver.GetGridCell(0, "Email").Text,
                "customer Name line 1 filter SubscribeDate");
            VerifyAreEqual("testmail9@test.ru", Driver.GetGridCell(4, "Email").Text,
                "customer Name line 4 filter SubscribeDate");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "SubscribeDateStr");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SubscribeDate 1");

            GoToAdmin("subscription");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter SubscribeDate 2");
        }

        [Test]
        public void FilterByUnSubDate()
        {
            GoToAdmin("subscription");

            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "UnsubscribeDateStr");

            //check filter min not exist
            Driver.SetGridFilterRange("UnsubscribeDateStr", "31.12.2050 00:00", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter add date min not exist");

            //check max not exist
            Driver.SetGridFilterRange("UnsubscribeDateStr", "", "31.12.2050 00:00");
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("UnsubscribeDateStr", "31.12.2050 00:00", "31.12.2050 00:00");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter add date  min/max not exist");

            //check filter add date 
            Driver.SetGridFilterRange("UnsubscribeDateStr", "01.07.2017 00:00", "05.07.2017 00:00");
            VerifyAreEqual("testmail12@test.ru", Driver.GetGridCell(0, "Email").Text,
                "customer Name line 1 filter UnSubDate");
            VerifyAreEqual("testmail15@test.ru", Driver.GetGridCell(3, "Email").Text,
                "customer Name line 3 filter UnSubDate");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter UnSubDate");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "UnsubscribeDateStr");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter UnSubDate ");

            GoToAdmin("subscription");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter UnSubDate 2");
        }
    }
}