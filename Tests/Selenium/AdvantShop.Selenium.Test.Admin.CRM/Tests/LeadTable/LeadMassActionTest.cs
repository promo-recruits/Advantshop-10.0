using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadMassActionManagersTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\[Order].LeadItem.csv"
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
        public void AppointManager()
        {
            GoToAdmin("leads?salesFunnelId=2");

            VerifyAreEqual("Elena El", Driver.GetGridCell(1, "ManagerName").Text, "pre check ManagerName line 2");
            VerifyAreEqual("Elena El", Driver.GetGridCell(2, "ManagerName").Text, "pre check ManagerName line 3");

            //select 
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"leadsAddManager\"]")))).SelectByText(
                "test testov");
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadsAddManagerSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("test testov", Driver.GetGridCell(1, "ManagerName").Text, "ManagerName line 2");
            VerifyAreEqual("test testov", Driver.GetGridCell(2, "ManagerName").Text, "ManagerName line 3");

            //after refresh
            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("test testov", Driver.GetGridCell(1, "ManagerName").Text,
                "ManagerName line 2 after refresh");
            VerifyAreEqual("test testov", Driver.GetGridCell(2, "ManagerName").Text,
                "ManagerName line 3 after refresh");

            //check manager in lead pop up
            Driver.GetGridCell(1, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();

            IWebElement selectElem = Driver.FindElement(By.Id("Lead_ManagerId"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "manager in lead pop up");
        }

        [Test]
        public void AppointManagerAllOnPage()
        {
            GoToAdmin("leads?salesFunnelId=1");

            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text,
                "pre check ManagerName page 1 line 1");
            VerifyAreEqual("", Driver.GetGridCell(9, "ManagerName").Text, "pre check ManagerName page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("", Driver.GetGridCell(0, "ManagerName").Text, "pre check ManagerName page 2 line 1");
            VerifyAreEqual("", Driver.GetGridCell(9, "ManagerName").Text, "pre check ManagerName page 2 line 10");

            GoToAdmin("leads?salesFunnelId=1");

            //select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected page 1");

            //select all on second page
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyAreEqual("20", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected page 2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"leadsAddManager\"]"));
            SelectElement select = new SelectElement(selectElem);
            IList<IWebElement> allOptions = select.Options;
            VerifyIsTrue(allOptions.Count == 3, "count all managers in modal"); //2 managers + null select

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"leadsAddManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadsAddManagerSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerName").Text, "ManagerName page 2 line 1");
            VerifyAreEqual("Elena El", Driver.GetGridCell(9, "ManagerName").Text, "ManagerName page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerName").Text, "ManagerName page 1 line 1");
            VerifyAreEqual("Elena El", Driver.GetGridCell(9, "ManagerName").Text, "ManagerName page 1 line 10");

            //after refresh
            GoToAdmin("leads?salesFunnelId=1");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerName").Text,
                "ManagerName page 1 line 1 after refresh");
            VerifyAreEqual("Elena El", Driver.GetGridCell(9, "ManagerName").Text,
                "ManagerName page 1 line 10 after refresh");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerName").Text,
                "ManagerName page 2 line 1 after refresh");
            VerifyAreEqual("Elena El", Driver.GetGridCell(9, "ManagerName").Text,
                "ManagerName page 2 line 10 after refresh");

            //check manager in lead pop up
            Driver.GetGridCell(7, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();

            selectElem = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Elena El"), "manager in lead pop up");
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoClose\"]")).Click();

            //check not all leads changed managers
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("", Driver.GetGridCell(0, "ManagerName").Text, "ManagerName page 3 line 1");
            VerifyAreEqual("", Driver.GetGridCell(9, "ManagerName").Text, "ManagerName page 3 line 10");
        }


        [Test]
        public void AppointManagerAll()
        {
            GoToAdmin("leads?salesFunnelId=3");

            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerName").Text,
                "pre check ManagerName page 1 line 1");
            VerifyAreEqual("Elena El", Driver.GetGridCell(9, "ManagerName").Text,
                "pre check ManagerName page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerName").Text,
                "pre check ManagerName page 2 line 1");
            VerifyAreEqual("Elena El", Driver.GetGridCell(9, "ManagerName").Text,
                "pre check ManagerName page 2 line 10");

            //select all 
            GoToAdmin("leads?salesFunnelId=3");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("32", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected all");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"1\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"leadsAddManager\"]")))).SelectByText(
                "test testov");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadsAddManagerSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text, "ManagerName page 2 line 1");
            VerifyAreEqual("test testov", Driver.GetGridCell(9, "ManagerName").Text, "ManagerName page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text, "ManagerName page 1 line 1");
            VerifyAreEqual("test testov", Driver.GetGridCell(9, "ManagerName").Text, "ManagerName page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text,
                "pre check ManagerName page 3 line 1");
            VerifyAreEqual("test testov", Driver.GetGridCell(9, "ManagerName").Text,
                "pre check ManagerName page 3 line 10");

            //after refresh
            GoToAdmin("leads?salesFunnelId=3");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text,
                "ManagerName page 1 line 1 after refresh");
            VerifyAreEqual("test testov", Driver.GetGridCell(9, "ManagerName").Text,
                "ManagerName page 1 line 10 after refresh");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text,
                "ManagerName page 2 line 1 after refresh");
            VerifyAreEqual("test testov", Driver.GetGridCell(9, "ManagerName").Text,
                "ManagerName page 2 line 10 after refresh");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text,
                "pre check ManagerName page 3 line 1 after refresh");
            VerifyAreEqual("test testov", Driver.GetGridCell(9, "ManagerName").Text,
                "pre check ManagerName page 3 line 10 after refresh");

            //check manager in lead pop up
            Driver.GetGridCell(9, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();

            IWebElement selectElem = Driver.FindElement(By.Id("Lead_ManagerId"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "manager in lead pop up");
        }
    }

    [TestFixture]
    public class CRMLeadMassActionSalesFunnelTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\[Order].LeadItem.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\LeadMassAction\\[Order].LeadEvent.csv"
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
        public void SalesFunnelChange()
        {
            GoToAdmin("leads?salesFunnelId=4");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".original-header-page")).Text.Contains("Funnel 4"),
                "pre check sales funnel 1");
            VerifyAreEqual("45", Driver.GetGridCell(3, "Id").FindElement(By.TagName("a")).Text, "pre check lead id 1");
            VerifyAreEqual("Новый", Driver.GetGridCell(3, "DealStatusName").Text, "pre check lead deal status 1");
            VerifyAreEqual("44", Driver.GetGridCell(4, "Id").FindElement(By.TagName("a")).Text, "pre check lead id 2");
            VerifyAreEqual("Новый", Driver.GetGridCell(4, "DealStatusName").Text, "pre check lead deal status 2");
            VerifyAreEqual("Найдено записей: 48",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "pre check leads count sales funnel 1");

            //select 
            Driver.GetGridCell(3, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(4, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"2\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"leadsEditSalesFunnel\"]"))))
                .SelectByText("Funnel 5");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"leadsEditDealStatus\"]")))).SelectByText(
                "Сделка заключена");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadsEditSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("Найдено записей: 46",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "leads count sales funnel 1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]")).Text.Contains("45"),
                "lead id 1 funnel 1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]")).Text.Contains("44"),
                "lead id 2 funnel 1");

            //after refresh
            GoToAdmin("leads?salesFunnelId=4");
            VerifyAreEqual("Найдено записей: 46",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "leads count sales funnel 1 after refresh");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]")).Text.Contains("45"),
                "lead id 1 funnel 1 after refresh");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[grid-unique-id=\"grid\"]")).Text.Contains("44"),
                "lead id 2 funnel 1 after refresh");

            //check to another funnel
            GoToAdmin("leads?salesFunnelId=5");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".original-header-page")).Text.Contains("Funnel 5"),
                "sales funnel 2");
            VerifyAreEqual("45", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "lead id 1 funnel 2");
            VerifyAreEqual("Сделка заключена", Driver.GetGridCell(0, "DealStatusName").Text,
                "lead deal status 1 funnel 2");
            VerifyAreEqual("44", Driver.GetGridCell(1, "Id").FindElement(By.TagName("a")).Text, "lead id 2 funnel 2");
            VerifyAreEqual("Сделка заключена", Driver.GetGridCell(1, "DealStatusName").Text,
                "lead deal status 2 funnel 2");
            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "leads count sales funnel 2");

            //check lead pop up
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Сделка заключена"), "deal status in lead pop up");

            selectElem = Driver.FindElement(By.Id("Lead_SalesFunnelId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Funnel 5"), "sales funnel in lead pop up");
        }

        [Test]
        public void DealStatusChange()
        {
            GoToAdmin("leads?salesFunnelId=2");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".original-header-page")).Text.Contains("Funnel 2"),
                "pre check sales funnel");
            VerifyAreEqual("Новый", Driver.GetGridCell(1, "DealStatusName").Text, "pre check lead deal status 1");
            VerifyAreEqual("89", Driver.GetGridCell(1, "Id").FindElement(By.TagName("a")).Text, "pre check lead id 1");
            VerifyAreEqual("Новый", Driver.GetGridCell(2, "DealStatusName").Text, "pre check lead deal status 2");
            VerifyAreEqual("88", Driver.GetGridCell(2, "Id").FindElement(By.TagName("a")).Text, "pre check lead id 2");
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "pre check leads count");

            //select 
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"2\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"leadsEditDealStatus\"]")))).SelectByText(
                "Выставление КП");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadsEditSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("Выставление КП", Driver.GetGridCell(1, "DealStatusName").Text, "lead deal status 1");
            VerifyAreEqual("89", Driver.GetGridCell(1, "Id").FindElement(By.TagName("a")).Text, "pre check lead id 1");
            VerifyAreEqual("Выставление КП", Driver.GetGridCell(2, "DealStatusName").Text, "lead deal status 2");
            VerifyAreEqual("88", Driver.GetGridCell(2, "Id").FindElement(By.TagName("a")).Text, "pre check lead id 2");
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "leads count");

            //after refresh
            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Выставление КП", Driver.GetGridCell(1, "DealStatusName").Text,
                "lead deal status 1 after refresh");
            VerifyAreEqual("89", Driver.GetGridCell(1, "Id").FindElement(By.TagName("a")).Text,
                "pre check lead id 1 after refresh");
            VerifyAreEqual("Выставление КП", Driver.GetGridCell(2, "DealStatusName").Text,
                "lead deal status 2 after refresh");
            VerifyAreEqual("88", Driver.GetGridCell(2, "Id").FindElement(By.TagName("a")).Text,
                "pre check lead id 2 after refresh");
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "leads count after refresh");

            //check lead pop up
            Driver.GetGridCell(2, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Выставление КП"), "deal status in lead pop up");

            selectElem = Driver.FindElement(By.Id("Lead_SalesFunnelId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Funnel 2"), "sales funnel in lead pop up");
        }

        [Test]
        public void DealStatusChangeAllOnPage()
        {
            GoToAdmin("leads?salesFunnelId=1");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".original-header-page")).Text.Contains("Funnel 1"),
                "pre check sales funnel");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text,
                "pre check lead deal status 1 page 1");
            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "pre check lead id 1 page 1");
            VerifyAreEqual("Новый", Driver.GetGridCell(9, "DealStatusName").Text,
                "pre check lead deal status 10 page 1");
            VerifyAreEqual("111", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "pre check lead id 10 page 1");
            VerifyAreEqual("Найдено записей: 30",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "pre check leads count");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Сделка заключена", Driver.GetGridCell(0, "DealStatusName").Text,
                "pre check lead deal status 1 page 2");
            VerifyAreEqual("110", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "pre check lead id 1 page 2");
            VerifyAreEqual("Сделка заключена", Driver.GetGridCell(9, "DealStatusName").Text,
                "pre check lead deal status 10 page 2");
            VerifyAreEqual("101", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "pre check lead id 10 page 2");

            GoToAdmin("leads?salesFunnelId=1");

            //select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected page 1");

            //select all on second page
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyAreEqual("20", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected page 2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"2\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"leadsEditSalesFunnel\"]"));
            SelectElement select = new SelectElement(selectElem);
            IList<IWebElement> allOptions = select.Options;
            VerifyIsTrue(allOptions.Count == 5, "count all sales funnels in modal");

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"leadsEditDealStatus\"]"));
            select = new SelectElement(selectElem);
            allOptions = select.Options;
            VerifyIsTrue(allOptions.Count == 6, "count all deal statuses in modal");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"leadsEditDealStatus\"]")))).SelectByText(
                "Сделка отклонена");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadsEditSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("Сделка отклонена", Driver.GetGridCell(0, "DealStatusName").Text,
                "lead deal status 1 page 2");
            VerifyAreEqual("110", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "lead id 1 page 2");
            VerifyAreEqual("Сделка отклонена", Driver.GetGridCell(9, "DealStatusName").Text,
                "lead deal status 10 page 2");
            VerifyAreEqual("101", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "lead id 10 page 2");
            VerifyAreEqual("Найдено записей: 30",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "leads count");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Сделка отклонена", Driver.GetGridCell(0, "DealStatusName").Text,
                "lead deal status 1 page 1");
            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "lead id 1 page 1");
            VerifyAreEqual("Сделка отклонена", Driver.GetGridCell(9, "DealStatusName").Text,
                "lead deal status 10 page 1");
            VerifyAreEqual("111", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "lead id 10 page 1");

            //after refresh
            GoToAdmin("leads?salesFunnelId=1");
            VerifyAreEqual("Сделка отклонена", Driver.GetGridCell(0, "DealStatusName").Text,
                "lead deal status 1 page 1 after refresh");
            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "lead id 1 page 1 after refresh");
            VerifyAreEqual("Сделка отклонена", Driver.GetGridCell(9, "DealStatusName").Text,
                "lead deal status 10 page 1 after refresh");
            VerifyAreEqual("111", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "lead id 10 page 1 after refresh");
            VerifyAreEqual("Найдено записей: 30",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "leads count after refresh");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Сделка отклонена", Driver.GetGridCell(0, "DealStatusName").Text,
                "lead deal status 1 page 2 after refresh");
            VerifyAreEqual("110", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "lead id 1 page 2 after refresh");
            VerifyAreEqual("Сделка отклонена", Driver.GetGridCell(9, "DealStatusName").Text,
                "lead deal status 10 page 2 after refresh");
            VerifyAreEqual("101", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "lead id 10 page 2 after refresh");

            //check lead pop up
            Driver.GetGridCell(7, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Сделка отклонена"), "deal status in lead pop up");

            selectElem = Driver.FindElement(By.Id("Lead_SalesFunnelId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Funnel 1"), "sales funnel in lead pop up");
            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoClose\"]")).Click();

            //check not all leads changed deal status
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Ожидание решения клиента", Driver.GetGridCell(0, "DealStatusName").Text,
                "lead deal status 1 page 3");
            VerifyAreEqual("100", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "lead id 1 page 3");
            VerifyAreEqual("Ожидание решения клиента", Driver.GetGridCell(9, "DealStatusName").Text,
                "lead deal status 10 page 3");
            VerifyAreEqual("91", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "lead id 10 page 3");
        }


        [Test]
        public void DealStatusChangeAll()
        {
            GoToAdmin("leads?salesFunnelId=3");

            VerifyAreEqual("Выставление КП", Driver.GetGridCell(0, "DealStatusName").Text,
                "pre check lead deal status 1 page 1");
            VerifyAreEqual("80", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "pre check lead id 1 page 1");
            VerifyAreEqual("Выставление КП", Driver.GetGridCell(9, "DealStatusName").Text,
                "pre check lead deal status 10 page 1");
            VerifyAreEqual("71", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "pre check lead id 10 page 1");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Выставление КП", Driver.GetGridCell(0, "DealStatusName").Text,
                "pre check lead deal status 1 page 2");
            VerifyAreEqual("70", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "pre check lead id 1 page 2");
            VerifyAreEqual("Выставление КП", Driver.GetGridCell(9, "DealStatusName").Text,
                "pre check lead deal status 10 page 2");
            VerifyAreEqual("61", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "pre check lead id 10 page 2");

            //select all 
            GoToAdmin("leads?salesFunnelId=3");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("32", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected all");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"2\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"leadsEditDealStatus\"]"))))
                .SelectByText("Новый");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadsEditSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text, "lead deal status 1 page 1");
            VerifyAreEqual("80", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "lead id 1 page 1");
            VerifyAreEqual("Новый", Driver.GetGridCell(9, "DealStatusName").Text, "lead deal status 10 page 1");
            VerifyAreEqual("71", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "lead id 10 page 1");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text, "lead deal status 1 page 2");
            VerifyAreEqual("70", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "lead id 1 page 2");
            VerifyAreEqual("Новый", Driver.GetGridCell(9, "DealStatusName").Text, "lead deal status 10 page 2");
            VerifyAreEqual("61", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "lead id 10 page 2");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text, "lead deal status 1 page 3");
            VerifyAreEqual("60", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "lead id 1 page 3");
            VerifyAreEqual("Новый", Driver.GetGridCell(9, "DealStatusName").Text, "lead deal status 10 page 3");
            VerifyAreEqual("51", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "lead id 10 page 3");

            //after refresh
            GoToAdmin("leads?salesFunnelId=3");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text,
                "lead deal status 1 page 1 after refresh");
            VerifyAreEqual("80", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "lead id 1 page 1 after refresh");
            VerifyAreEqual("Новый", Driver.GetGridCell(9, "DealStatusName").Text,
                "lead deal status 10 page 1 after refresh");
            VerifyAreEqual("71", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "lead id 10 page 1 after refresh");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text,
                "lead deal status 1 page 2 after refresh");
            VerifyAreEqual("70", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "lead id 1 page 2 after refresh");
            VerifyAreEqual("Новый", Driver.GetGridCell(9, "DealStatusName").Text,
                "lead deal status 10 page 2 after refresh");
            VerifyAreEqual("61", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "lead id 10 page 2 after refresh");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text,
                "lead deal status 1 page 3 after refresh");
            VerifyAreEqual("60", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "lead id 1 page 3 after refresh");
            VerifyAreEqual("Новый", Driver.GetGridCell(9, "DealStatusName").Text,
                "lead deal status 10 page 3 after refresh");
            VerifyAreEqual("51", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "lead id 10 page 3 after refresh");

            //check manager in lead pop up
            Driver.GetGridCell(9, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Новый"), "deal status in lead pop up");
        }
    }
}