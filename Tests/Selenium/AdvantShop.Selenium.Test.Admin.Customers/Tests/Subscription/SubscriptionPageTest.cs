using System.Drawing;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Subscription
{
    [TestFixture]
    public class SubscriptionPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Subscription\\ManySubscription\\Customers.CustomerGroup.csv",
                "data\\Admin\\Subscription\\ManySubscription\\Customers.Customer.csv",
                "data\\Admin\\Subscription\\ManySubscription\\Customers.Contact.csv",
                "data\\Admin\\Subscription\\ManySubscription\\Customers.Departments.csv",
                "data\\Admin\\Subscription\\ManySubscription\\Customers.Managers.csv",
                "data\\Admin\\Subscription\\ManySubscription\\Customers.CustomerField.csv",
                "data\\Admin\\Subscription\\ManySubscription\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\Subscription\\ManySubscription\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\Subscription\\ManySubscription\\Customers.Subscription.csv"
            );

            Init();
            GoToAdmin("subscription");
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
        public void testRezolution()
        {
            GoToAdmin("subscription");

            VerifyAddErrors(" initial size window Width:" + Driver.Manage().Window.Size.Width.ToString() +
                            " , height: " + Driver.Manage().Window.Size.Height.ToString());
            Driver.Manage().Window.Size = new Size(1920, 1080);
            VerifyAddErrors(" size window Width:" + Driver.Manage().Window.Size.Width.ToString() + " , height: " +
                            Driver.Manage().Window.Size.Height.ToString());

            Driver.Manage().Window.FullScreen();
            Driver.Manage().Window.Maximize();
            VerifyAddErrors(" max window Width:" + Driver.Manage().Window.Size.Width.ToString() + " , height: " +
                            Driver.Manage().Window.Size.Height.ToString());
        }

        [Test]
        public void SubscriptionqPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "line 1");
            VerifyAreEqual("testmail16@test.ru", Driver.GetGridCell(9, "Email").Text, "line 10");
        
            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "line 1");
            VerifyAreEqual("testmail25@test.ru", Driver.GetGridCell(19, "Email").Text, "line 20");
        }

        [Test]
        public void SubscriptionsPage()
        {
            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "page 1 line 1");
            VerifyAreEqual("testmail16@test.ru", Driver.GetGridCell(9, "Email").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("testmail17@test.ru", Driver.GetGridCell(0, "Email").Text, "page 2 line 1");
            VerifyAreEqual("testmail25@test.ru", Driver.GetGridCell(9, "Email").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("testmail26@test.ru", Driver.GetGridCell(0, "Email").Text, "page 3 line 1");
            VerifyAreEqual("testmail34@test.ru", Driver.GetGridCell(9, "Email").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("testmail35@test.ru", Driver.GetGridCell(0, "Email").Text, "page 4 line 1");
            VerifyAreEqual("testmail43@test.ru", Driver.GetGridCell(9, "Email").Text, "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("testmail44@test.ru", Driver.GetGridCell(0, "Email").Text, "page 5 line 1");
            VerifyAreEqual("testmail52@test.ru", Driver.GetGridCell(9, "Email").Text, "page 5 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "page 1 line 1");
            VerifyAreEqual("testmail16@test.ru", Driver.GetGridCell(9, "Email").Text, "page 1 line 10");
        }

        [Test]
        public void SubscriptionsPageToPrevious()
        {
            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "page 1 line 1");
            VerifyAreEqual("testmail16@test.ru", Driver.GetGridCell(9, "Email").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("testmail17@test.ru", Driver.GetGridCell(0, "Email").Text, "page 2 line 1");
            VerifyAreEqual("testmail25@test.ru", Driver.GetGridCell(9, "Email").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("testmail26@test.ru", Driver.GetGridCell(0, "Email").Text, "page 3 line 1");
            VerifyAreEqual("testmail34@test.ru", Driver.GetGridCell(9, "Email").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("testmail17@test.ru", Driver.GetGridCell(0, "Email").Text, "page 2 line 1");
            VerifyAreEqual("testmail25@test.ru", Driver.GetGridCell(9, "Email").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "page 1 line 1");
            VerifyAreEqual("testmail16@test.ru", Driver.GetGridCell(9, "Email").Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("testmail99@test.ru", Driver.GetGridCell(0, "Email").Text, "last page line 1");
        }

        [Test]
        public void SubscriptionzSelectDelete()
        {
            GoToAdmin("subscription");

            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl, dropdIndex: 2);
            VerifyAreEqual("testmail11@test.ru", Driver.GetGridCell(0, "Email").Text, "selected 2 grid delete");
            VerifyAreEqual("testmail12@test.ru", Driver.GetGridCell(1, "Email").Text, "selected 3 grid delete");
            VerifyAreEqual("testmail13@test.ru", Driver.GetGridCell(2, "Email").Text, "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl, dropdIndex: 2);
            VerifyAreEqual("testmail20@test.ru", Driver.GetGridCell(0, "Email").Text,
                "selected all on page 2 grid delete");
            VerifyAreEqual("testmail29@test.ru", Driver.GetGridCell(9, "Email").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("87", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl, dropdIndex: 2);

            GoToAdmin("subscription");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");
        }
    }
}