using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadSearchTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadItem.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("leads");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void LeadSearchExistById()
        {
            Driver.GetGridFilterTab(0, "111");

            VerifyAreEqual("111", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "search exist lead id");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }


        [Test]
        public void LeadSearchExistByContact()
        {
            Driver.GetGridFilterTab(0, "Patronymic");

            VerifyAreEqual("FirstName (Organization Test)", Driver.GetGridCell(0, "FullName").Text,
                "search exist fullname");
            VerifyAreEqual("Найдено записей: 21",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void LeadSearchNotExistId()
        {
            Driver.GetGridFilterTab(0, "3333333333");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist lead id");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void LeadSearchMuchSymbols()
        {
            Driver.GetGridFilterTab(0,
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void LeadSearchInvalidSymbols()
        {
            Driver.GetGridFilterTab(0, "########@@@@@@@@&&&&&&&******,,,,..");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }
}