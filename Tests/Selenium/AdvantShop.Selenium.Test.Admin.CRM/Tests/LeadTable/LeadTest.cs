using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadTest : BaseSeleniumTest
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
            GoToAdmin("leads?salesFunnelId=-1");
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
        [Order(0)]
        public void LeadGrid()
        {
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".original-header-page")).Text.Contains("Лиды"),
                "h1 lead grid");

            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "lead Id");
            VerifyAreEqual("Основная воронка", Driver.GetGridCell(0, "SalesFunnelName").Text, "lead sales funnel");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName");
            VerifyAreEqual("FirstName (Organization Test)", Driver.GetGridCell(0, "FullName").Text, "FullName");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text, "ManagerName");
            VerifyAreEqual("20", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount");
            VerifyAreEqual("120 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum");

            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all leads");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-grid-custom-footer")).Text.Contains("Сумма сделок: 7 260 руб."),
                "count all sum");
        }

        [Test]
        [Order(1)]
        public void LeadGoToEditById()
        {
            Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            VerifyAreEqual("Lead Title 120", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead edit title");
            VerifyIsTrue(Driver.Url.Contains("leadIdInfo=120"), "url lead edit");

            GoToAdmin("leads?salesFunnelId=-1");
        }

        [Test]
        [Order(1)]
        public void LeadGoToEditByServiceCol()
        {
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            VerifyAreEqual("Lead Title 120", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead edit title");
            VerifyIsTrue(Driver.Url.Contains("leadIdInfo=120"), "url lead edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoClose\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".original-header-page")).Text.Contains("Лиды"),
                "h1 lead grid");
        }

        [Test]
        [Order(10)]
        public void LeadSelectDelete()
        {
            GoToAdmin("leads?salesFunnelId=-1");
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "1 grid delete");

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
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("116", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "selected 2 grid delete");
            VerifyAreEqual("115", Driver.GetGridCell(1, "Id").FindElement(By.TagName("a")).Text,
                "selected 3 grid delete");
            VerifyAreEqual("114", Driver.GetGridCell(2, "Id").FindElement(By.TagName("a")).Text,
                "selected 4 grid delete");

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
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("106", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "selected all on page 2 grid delete");
            VerifyAreEqual("97", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("106", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
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
            Functions.GridDropdownDelete(Driver, BaseUrl);

            GoToAdmin("leads");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");
        }
    }
}