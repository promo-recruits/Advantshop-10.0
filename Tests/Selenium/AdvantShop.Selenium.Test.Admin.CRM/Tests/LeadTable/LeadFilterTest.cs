using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadFilterTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\LeadFilter\\[Order].LeadItem.csv"
            );

            Init();

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("leads?salesFunnelId=-1");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void LeadFilterManager()
        {
            //check filter manager 
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "ManagerName", filterItem: "test testov");

            VerifyAreEqual("Найдено записей: 36",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter manager");

            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "lead id manager line 1");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text, "ManagerName line 1");
            VerifyAreEqual("47", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "lead id manager line 10");
            VerifyAreEqual("test testov", Driver.GetGridCell(9, "ManagerName").Text, "ManagerName line 10");

            //check all managers
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count == 3,
                "count managers");
            Driver.DropFocusCss("[data-e2e=\"gridFilterSearch\"]");

            //check go to edit and back
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));
            VerifyIsTrue(Driver.Url.Contains("leadIdInfo"), "lead edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoClose\"]")).Click();

            VerifyAreEqual("Найдено записей: 36",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter manager return");
            VerifyIsTrue(
                Driver.FindElement(
                        By.CssSelector(
                            "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"ManagerName\"]"))
                    .Displayed, "filter manager Displayed");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "ManagerName");
            VerifyAreEqual("Найдено записей: 84",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter manager after deleting 1");

            GoToAdmin("leads?salesFunnelId=-1");
            VerifyAreEqual("Найдено записей: 84",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter manager after deleting 2");
        }

        [Test]
        public void LeadFilterContact()
        {
            //check filter Contact
            Functions.GridFilterSet(Driver, BaseUrl, name: "FullName");

            //search by not exist Contact
            Driver.SetGridFilterValue("FullName", "123123123 name contact 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("FullName",
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("FullName", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist contact
            Driver.SetGridFilterValue("FullName", "Patron1");

            VerifyAreEqual("19", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "lead id fullname line 1");
            VerifyAreEqual("LastName19 FirstName19 Patron19", Driver.GetGridCell(0, "FullName").Text,
                "fullname line 1");
            VerifyAreEqual("10", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "lead id fullname line 10");
            VerifyAreEqual("LastName10 FirstName10 Patron10", Driver.GetGridCell(9, "FullName").Text,
                "fullname line 10");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter FullName");

            //check go to edit and back 
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));
            VerifyIsTrue(Driver.Url.Contains("leadIdInfo"), "lead edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoClose\"]")).Click();
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter FullName return");
            VerifyIsTrue(
                Driver.FindElement(
                        By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"FullName\"]"))
                    .Displayed, "filter FullName Displayed");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "FullName");
            VerifyAreEqual("Найдено записей: 109",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter FullName deleting 1");

            GoToAdmin("leads?salesFunnelId=-1");
            VerifyAreEqual("Найдено записей: 109",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter FullName deleting 2");
        }

        [Test]
        public void LeadFilterSum()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "SumFormatted");

            //check min too much symbols
            Driver.SetGridFilterRange("SumFormatted", "1111111111111111111111111111111", "");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter sum min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("SumFormatted", "", "1111111111111111111111111111111");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sum max many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "filter max many symbols border color");

            //check min and max too much symbols
            Driver.SetGridFilterRange("SumFormatted", "1111111111111111111111111111111", "1111111111111111111111111111111");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter sum min many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "filter sum max many symbols");

            //check invalid symbols
            //check min invalid symbols
            Driver.SetGridFilterRange("SumFormatted", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter sum min imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sum count leads min many symbols");

            //check max invalid symbols
            Driver.SetGridFilterRange("SumFormatted", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter sum max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sum count leads max many symbols");

            //check min and max invalid symbols

            Driver.SetGridFilterRange("SumFormatted", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter sum both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter sum both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sum count leads min/max many symbols");

            //check filter min not exist
            Driver.SetGridFilterRange("SumFormatted", "1000", "");
            //Driver.DropFocusCss("[data-e2e=\"gridFilterSearch\"]");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter sum min not exist");

            //check max not exist
            Driver.SetGridFilterRange("SumFormatted", "", "1000");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sum max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("SumFormatted", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter sum min/max not exist");

            //check filter sum
            Driver.SetGridFilterRange("SumFormatted", "40", "99");
            VerifyAreEqual("Найдено записей: 60",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sum");

            VerifyAreEqual("99", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text, "lead id sum line 1");
            VerifyAreEqual("99 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "sum line 1");
            VerifyAreEqual("90", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text, "lead id sum line 10");
            VerifyAreEqual("90 руб.", Driver.GetGridCell(9, "SumFormatted").Text, "sum line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "SumFormatted");
            VerifyAreEqual("Найдено записей: 60",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Sum after deleting 1");

            GoToAdmin("leads?salesFunnelId=-1");
            VerifyAreEqual("Найдено записей: 60",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Sum after deleting 2");
        }

        [Test]
        public void LeadFilterAddDate()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "CreatedDateFormatted");

            //check filter min not exist
            Driver.SetGridFilterRange("CreatedDateFormatted", "31.12.2050 00:00", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter add date min not exist");

            //check max not exist
            Driver.SetGridFilterRange("CreatedDateFormatted", "", "31.12.2050 00:00");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("CreatedDateFormatted", "31.12.2020 00:00", "31.12.2050 00:00");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter sum min/max not exist");

            //check filter sum
            Driver.SetGridFilterRange("CreatedDateFormatted", "01.01.2015 00:00", "31.12.2015 18:00");
            Thread.Sleep(500);//костыль
            VerifyAreEqual("Найдено записей: 87",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date");

            VerifyAreEqual("87", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "lead id add date line 1");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains("2015"), "add date line 1");
            VerifyAreEqual("78", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "lead id add date line 10");
            VerifyIsTrue(Driver.GetGridCell(9, "CreatedDateFormatted").Text.Contains("2015"), "add date line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "CreatedDateFormatted");
            VerifyAreEqual("Найдено записей: 33",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date after deleting 1");

            GoToAdmin("leads?salesFunnelId=-1");
            VerifyAreEqual("Найдено записей: 33",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date after deleting 2");
        }

        [Test]
        public void LeadFilterSource()
        {
            //check filter source no
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnSources",
                filterItem: "Брошенные корзины");

            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter source no count");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter source no");

            //check filter source
            //scroll in select
            Driver.SetGridFilterSelectValue("_noopColumnSources", "Корзина интернет магазина");

            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter source");

            VerifyAreEqual("99", Driver.GetGridCell(0, "Id").FindElement(By.TagName("a")).Text,
                "lead id source filter line 1");
            VerifyAreEqual("LastName99 FirstName99 Patron99", Driver.GetGridCell(0, "FullName").Text,
                "manager source filter line 1");
            VerifyAreEqual("90", Driver.GetGridCell(9, "Id").FindElement(By.TagName("a")).Text,
                "lead id source filter line 10");
            VerifyAreEqual("LastName90 FirstName90 Patron90", Driver.GetGridCell(9, "FullName").Text,
                "manager source filter line 10");

            //check all leads
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count == 12,
                "count sources in filter");
            Driver.DropFocusCss("[data-e2e=\"gridFilterSearch\"]");

            //check go to edit and back
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));
            VerifyIsTrue(Driver.Url.Contains("leadIdInfo"), "lead edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoClose\"]")).Click();
            Refresh();

            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter source return");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(
                        "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnSources\"]"))
                    .Displayed, "filter source Displayed");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnSources");
            VerifyAreEqual("Найдено записей: 21",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter source after deleting 1");

            GoToAdmin("leads?salesFunnelId=-1");
            VerifyAreEqual("Найдено записей: 21",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter source after deleting 2");
        }

        [Test]
        public void LeadFilterSalesFunnel()
        {
            //check filter sales funnel 
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "SalesFunnelName", filterItem: "Лиды");

            VerifyAreEqual("Найдено записей: 15",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sales funnel 1");

            VerifyAreEqual("Лиды", Driver.GetGridCell(0, "SalesFunnelName").Text, "lead id sales funnel 1 line 1");
            VerifyAreEqual("Лиды", Driver.GetGridCell(9, "SalesFunnelName").Text, "lead id sales funnel 1 line 10");

            //check all sales funnel
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count == 2,
                "count sales funnel");
            Driver.FindElement(By.CssSelector("[data-e2e=\"Основная воронка\"]")).Click();

            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sales funnel 2");

            VerifyAreEqual("Основная воронка", Driver.GetGridCell(0, "SalesFunnelName").Text,
                "lead id sales funnel 2 line 1");
            VerifyAreEqual("Основная воронка", Driver.GetGridCell(9, "SalesFunnelName").Text,
                "lead id sales funnel 2 line 10");

            //check go to edit and back
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(By.Id("Lead_SalesFunnelId"));
            VerifyIsTrue(Driver.Url.Contains("leadIdInfo"), "lead edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoClose\"]")).Click();
            VerifyAreEqual("Найдено записей: 105",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sales funnel return");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"SalesFunnelName\"]")).Displayed,
                "filter sales funnel Displayed");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "SalesFunnelName");
            VerifyAreEqual("Найдено записей: 15",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sales funnel after deleting 1");

            GoToAdmin("leads?salesFunnelId=-1");
            VerifyAreEqual("Найдено записей: 15",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sales funnel after deleting 2");
        }
    }
}