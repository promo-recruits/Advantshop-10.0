using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadKandan
{
    [TestFixture]
    public class CRMLeadKanbanFilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead_Kanban\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead_Kanban\\[Order].LeadItem.csv"
            );

            Init();
            Functions.KanbanOn(Driver, BaseUrl, url: "leads?salesFunnelId=1");
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("leads?salesFunnelId=1");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void LeadKanbanFilterManager()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "ManagerName", filterItem: "Elena El");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 12, "kanban filter manager 2");

            VerifyIsTrue(Driver.GetKanbanCard(1, 0, "FullName").Text.Contains("LastName51 FirstName51"),
                "kanban lead name filter manager 2 card 1 column 2");
            VerifyIsTrue(Driver.GetKanbanCard(2, 0, "FullName").Text.Contains("LastName61 FirstName61"),
                "kanban lead name filter manager 2 card 1 column 3");
            VerifyIsTrue(Driver.GetKanbanCard(2, 1, "FullName").Text.Contains("LastName62 FirstName62"),
                "kanban lead name filter manager 2 card 2 column 3");

            //check filter manager
            Driver.SetGridFilterSelectValue("ManagerName", "test testov");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 32, "kanban filter manager 1");

            VerifyIsTrue(Driver.GetKanbanCard(0, 0, "FullName").Text.Contains("LastName21 FirstName21"),
                "kanban lead name filter manager 1 card 1 column 1");
            VerifyIsTrue(Driver.GetKanbanCard(1, 0, "FullName").Text.Contains("LastName40 FirstName40"),
                "kanban lead name filter manager 1 card 1 column 2");

            //check all managers
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count == 3,
                "count managers in filter");

            //check go to edit and back
            Driver.GetKanbanCard(0, 0, "FullName").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));
            VerifyIsTrue(Driver.Url.Contains("leadIdInfo"), "lead edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoClose\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 32, "filter manager return");
            VerifyIsTrue(
                Driver.FindElement(
                        By.CssSelector(
                            "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"ManagerName\"]"))
                    .Displayed, "filter manager Displayed");

            //check close filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "ManagerName");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116, "filter ManagerName closed");
        }

        [Test]
        public void LeadKanbanFilterContact()
        {
            //check filter Contact
            Functions.GridFilterSet(Driver, BaseUrl, name: "FullName");

            //search by not exist Contact
            Driver.SetGridFilterValue("FullName", "123123123 name contact 3");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 0,
                "filter FullName kanban no such element count");

            //search too much symbols
            Driver.SetGridFilterValue("FullName",
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 0,
                "filter FullName kanban too much elements count");

            //search invalid symbols
            Driver.SetGridFilterValue("FullName", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 0,
                "filter FullName kanban invalid symbols count");

            //search by exist contact
            Driver.SetGridFilterValue("FullName", "FirstName1");

            VerifyIsTrue(Driver.GetKanbanCard(0, 0, "FullName").Text.Contains("LastName1 FirstName1"),
                "kanban lead name filter FullName 1 card");
            VerifyIsTrue(Driver.GetKanbanCard(0, 10, "FullName").Text.Contains("LastName19 FirstName19"),
                "kanban lead name filter FullName 1 card 11");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 11, "filter FullName");

            //check go to edit and back 
            Driver.GetKanbanCard(0, 0, "FullName").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));
            VerifyIsTrue(Driver.Url.Contains("leadIdInfo"), "lead edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoClose\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 11, "filter FullName return");
            VerifyIsTrue(
                Driver.FindElement(
                        By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"FullName\"]"))
                    .Displayed, "filter FullName Displayed");

            //check close filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "FullName");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116, "filter FullName close");
        }

        [Test]
        public void LeadKanbanFilterSum()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "SumFormatted");

            //check min too much symbols
            Driver.SetGridFilterRange("SumFormatted", "1111111111111111111111111111111", "");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter sum min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("SumFormatted", "", "1111111111111111111111111111111");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116,
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
            // GoToAdmin("leads?salesFunnelId=1");
            //  Functions.GridFilterSet(driver, baseURL, name: "SumFormatted");

            //check min invalid symbols
            Driver.SetGridFilterRange("SumFormatted", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter sum min imvalid symbols");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116,
                "filter sum count leads min many symbols");

            //  GoToAdmin("leads?salesFunnelId=1");
            // Functions.GridFilterSet(driver, baseURL, name: "SumFormatted");

            //check max invalid symbols

            Driver.SetGridFilterRange("SumFormatted", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter sum max imvalid symbols");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116,
                "filter sum count leads max many symbols");

            //check min and max invalid symbols
            // GoToAdmin("leads?salesFunnelId=1");
            // Functions.GridFilterSet(driver, baseURL, name: "SumFormatted");
            Driver.SetGridFilterRange("SumFormatted", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter sum both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter sum both max imvalid symbols");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116,
                "filter sum count leads min/max many symbols");

            //  GoToAdmin("leads?salesFunnelId=1");
            //  Functions.GridFilterSet(driver, baseURL, name: "SumFormatted");

            //check filter min not exist
            Driver.SetGridFilterRange("SumFormatted", "50000", "");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 0,
                "filter sum min not exist count");

            //check max not exist
            Driver.SetGridFilterRange("SumFormatted", "", "50000");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116, "filter sum max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("SumFormatted", "50000", "50000");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 0,
                "filter sum min/max not exist count");

            //check filter sum
            Driver.SetGridFilterRange("SumFormatted", "40", "99");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 60, "filter sum count");

            VerifyIsTrue(Driver.GetKanbanCard(1, 0, "FullName").Text.Contains("LastName40 FirstName40"),
                "kanban lead name filter sum card 1 column 2");
            VerifyIsTrue(Driver.GetKanbanCard(1, 1, "FullName").Text.Contains("LastName41 FirstName41"),
                "kanban lead name filter sum card 2 column 2");
            VerifyIsTrue(Driver.GetKanbanCard(2, 0, "FullName").Text.Contains("LastName61 FirstName61"),
                "kanban lead name filter sum card 1 column 3");

            //check close filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "SumFormatted");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116, "filter Sum after close");
        }

        [Test]
        public void LeadKanbanFilterAddDate()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "CreatedDateFormatted");

            //check filter min not exist
            Driver.SetGridFilterRange("CreatedDateFormatted", "31.12.2050 00:00", "");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 0,
                "filter add date min not exist count");

            //check max not exist
            Driver.SetGridFilterRange("CreatedDateFormatted", "", "31.12.2050 00:00");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116,
                "filter add date max not exist count");

            //check min and max not exist
            Driver.SetGridFilterRange("CreatedDateFormatted", "31.12.2020 00:00", "31.12.2020 00:00");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 0,
                "filter sum min/max not exist count");

            //check filter sum
            Functions.DataTimePickerFilter(Driver, BaseUrl, monthFrom: "Январь", yearFrom: "2015",
                dataFrom: "Январь 1, 2015", hourFrom: "00", minFrom: "00", monthTo: "Декабрь", yearTo: "2015",
                dataTo: "Декабрь 31, 2015", hourTo: "18", minTo: "00", dropFocusElem: ".ui-grid-filter-text",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]", fieldTo: "[data-e2e=\"datetimeFilterTo\"]");

            VerifyIsTrue(Driver.GetKanbanCard(0, 0, "FullName").Text.Contains("LastName1 FirstName1"),
                "kanban lead name filter add date card 1 column 1");
            VerifyIsTrue(Driver.GetKanbanCard(1, 0, "FullName").Text.Contains("LastName40 FirstName40"),
                "kanban lead name filter add date card 1 column 2");
            VerifyIsTrue(Driver.GetKanbanCard(1, 1, "FullName").Text.Contains("LastName41 FirstName41"),
                "kanban lead name filter add date card 2 column 2");

            //check close filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "CreatedDateFormatted");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116, "filter ManagerName closed");
        }

        [Test]
        public void LeadKanbanFilterSource()
        {
            //check filter source no
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnSources",
                filterItem: "Обратный звонок");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 0, "filter source no count");

            //check filter source
            //scroll in select
            Driver.SetGridFilterSelectValue("_noopColumnSources", "Корзина интернет магазина");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 12, "filter source");

            VerifyIsTrue(Driver.GetKanbanCard(0, 0, "FullName").Text.Contains("LastName1 FirstName1"),
                "kanban lead name filter add date card 1 column 1");
            VerifyIsTrue(Driver.GetKanbanCard(1, 0, "FullName").Text.Contains("LastName41 FirstName41"),
                "kanban lead name filter add date card 1 column 2");

            //check all leads
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count == 12,
                "count sources in filter");
            Driver.DropFocusCss("[data-e2e=\"gridFilterSearch\"]");

            //check go to edit and back
            Driver.GetKanbanCard(1, 0, "FullName").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));
            VerifyIsTrue(Driver.Url.Contains("leadIdInfo"), "lead edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoClose\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 12, "filter source return");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(
                        "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnSources\"]"))
                    .Displayed, "filter source Displayed");

            //check close filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnSources");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".kanban-task")).Count == 116, "filter source after close");
        }
    }
}