using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.Card
{
    [TestFixture]
    public class BonusSystemCardFilterCustomerFields : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Customers.Customer.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Customers.Contact.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Customers.CustomerField.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Customers.CustomerFieldValuesMap.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterCustFields\\Bonus.Card.csv"
            );
            InitializeService.BonusSystemActive();
            Init();

            GoToAdmin("cards");

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void FilterCustomerFieldSelect1()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_1");

            //check filter no item
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Customer Field 1 Value 2\"]")).Click();
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter no items count");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter no items");

            //check filter 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Customer Field 1 Value 1\"]")).Click();
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "filter");

            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter result card num 1");
            VerifyAreEqual("LastName1 FirstName1", Driver.GetGridCell(0, "FIO").Text, "filter result FIO 1");
            VerifyAreEqual("530802", Driver.GetGridCell(1, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter result card num 2");
            VerifyAreEqual("LastName2 FirstName2", Driver.GetGridCell(1, "FIO").Text, "filter result FIO 2");
            VerifyAreEqual("530803", Driver.GetGridCell(2, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter result card num 3");
            VerifyAreEqual("LastName3 FirstName3", Driver.GetGridCell(2, "FIO").Text, "filter result FIO 3");

            //check all  
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count == 2,
                "count filter select values");

            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Карта')]"));
            VerifyIsTrue(Driver.Url.Contains("edit"), "card edit");

            Driver.Navigate().GoToUrl(strUrl);

            Driver.WaitForElem(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total"));
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnCustomerField_1\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_1");
            Refresh();
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 2");
        }

        [Test]
        public void FilterCustomerFieldSelect2()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_2");

            //check filter value 1
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Customer Field 2 Value 1\"]")).Click();
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter select value 1");
            VerifyAreEqual("530803", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter select value 1 result card num");
            VerifyAreEqual("LastName3 FirstName3", Driver.GetGridCell(0, "FIO").Text,
                "filter select value 1 result FIO");

            //check filter value 2
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Customer Field 2 Value 2\"]")).Click();
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter select value 2");

            VerifyAreEqual("530804", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter select value 2 result card num");
            VerifyAreEqual("LastName4 FirstName4", Driver.GetGridCell(0, "FIO").Text,
                "filter select value 2 result FIO");

            //check all  
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count == 2,
                "count filter select values");

            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Карта')]"));
            VerifyIsTrue(Driver.Url.Contains("edit"), "card edit");

            Driver.Navigate().GoToUrl(strUrl);

            Driver.WaitForElem(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total"));
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnCustomerField_2\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_2");
            Refresh();
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 2");
        }


        [Test]
        public void FilterCustomerFieldText()
        {
            //check filter not exist
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_4");

            //search by not exist card
            Driver.SetGridFilterValue("_noopColumnCustomerField_4", "Field Text 5");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnCustomerField_4", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrr");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnCustomerField_4", "########@@@@@@@@&&&&&&&******,,,,..");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //check filter exist
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_4");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_3");

            //search by exist card
            Driver.SetGridFilterValue("_noopColumnCustomerField_3", "Field Text 5");
            Driver.Blur();

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count items");
            VerifyAreEqual("530805", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter result Card Number");
            VerifyAreEqual("LastName5 FirstName5", Driver.GetGridCell(0, "FIO").Text, "filter result FIO");

            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Карта')]"));
            VerifyIsTrue(Driver.Url.Contains("edit"), "card edit");

            Driver.Navigate().GoToUrl(strUrl);

            Driver.WaitForElem(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total"));
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnCustomerField_3\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_3");
            Refresh();
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 2");
        }

        [Test]
        public void FilterCustomerFieldNum()
        {
            //check filter not exist
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_6");

            //check min too much symbols
            Driver.SetGridFilterRange("_noopColumnCustomerField_6", "1111111111", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("_noopColumnCustomerField_6", "", "1111111111");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max many symbols");

            //check min and max too much symbols
            Driver.SetGridFilterRange("_noopColumnCustomerField_6", "1111111111", "1111111111");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max many symbols");

            //check invalid symbols
            //check min invalid symbols
            Driver.SetGridFilterRange("_noopColumnCustomerField_6", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter min imvalid symbols");
            VerifyAreEqual("Найдено записей: 13",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards min imvalid symbols");

            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_6");

            //check max invalid symbols
            Driver.SetGridFilterRange("_noopColumnCustomerField_6", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter max imvalid symbols");
            VerifyAreEqual("Найдено записей: 13",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards max imvalid symbols");

            //check min and max invalid symbols

            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_6");
            Driver.SetGridFilterRange("_noopColumnCustomerField_6", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");

            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 13",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards min/max imvalid symbols");

            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_6");

            //check filter min not exist
            Driver.SetGridFilterRange("_noopColumnCustomerField_6", "1000", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min not exist");

            //check max not exist
            Driver.SetGridFilterRange("_noopColumnCustomerField_6", "", "1000");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("_noopColumnCustomerField_6", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max not exist");

            //check filter 
            Driver.SetGridFilterRange("_noopColumnCustomerField_6", "8", "10");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter customer field number");

            VerifyAreEqual("530808", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter result card num 1");
            VerifyAreEqual("LastName8 FirstName8", Driver.GetGridCell(0, "FIO").Text, "filter result FIO 1");
            VerifyAreEqual("530809", Driver.GetGridCell(1, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter result card num 2");
            VerifyAreEqual("LastName9 FirstName9", Driver.GetGridCell(1, "FIO").Text, "filter result FIO 2");
            VerifyAreEqual("530810", Driver.GetGridCell(2, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter result card num 3");
            VerifyAreEqual("LastName10 FirstName10", Driver.GetGridCell(2, "FIO").Text, "filter result FIO 3");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_6");
            Refresh();
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 2");
        }


        [Test]
        public void FilterCustomerFieldMultiLinesText()
        {
            //check filter not exist
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_8");

            //search by not exist card
            Driver.SetGridFilterValue("_noopColumnCustomerField_8", "many");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnCustomerField_8", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrr");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnCustomerField_8", "########@@@@@@@@&&&&&&&******,,,,..");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //check filter exist
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_8");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_7");

            //search by exist card
            Driver.SetGridFilterValue("_noopColumnCustomerField_7", "many");
            Driver.Blur();

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count items");
            VerifyAreEqual("530811", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter result Card Number");
            VerifyAreEqual("LastName11 FirstName11", Driver.GetGridCell(0, "FIO").Text, "filter result FIO");

            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Карта')]"));
            VerifyIsTrue(Driver.Url.Contains("edit"), "card edit");

            Driver.Navigate().GoToUrl(strUrl);

            Driver.WaitForElem(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total"));
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnCustomerField_7\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_7");
            Refresh();
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 2");
        }

        [Test]
        public void FilterCustomerFieldDate()
        {
            //check filter not exist
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_10");

            //check filter min not exist
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).SendKeys("31.12.2050");
            Driver.DropFocus("h1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Clear();
            Driver.DropFocus("h1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter customer field date min not exist");

            //check max not exist
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Clear();
            Driver.DropFocus("h1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).SendKeys("31.12.2050");
            Driver.DropFocus("h1");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter customer field date max not exist");

            //check min and max not exist
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).SendKeys("31.12.2050");
            Driver.DropFocus("h1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).SendKeys("31.12.2050");
            Driver.DropFocus("h1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter customer field date min/max not exist");

            //check filter 
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).SendKeys("01.03.2017");
            Driver.DropFocus("h1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).SendKeys("25.05.2017");
            Driver.DropFocus("h1");
            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter customer field date");

            VerifyAreEqual("530812", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter customer field date line 1");
            VerifyAreEqual("LastName12 FirstName12", Driver.GetGridCell(0, "FIO").Text,
                "filter customer field date FIO line 1");
            VerifyAreEqual("530813", Driver.GetGridCell(1, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter field date line 2");
            VerifyAreEqual("LastName13 FirstName13", Driver.GetGridCell(1, "FIO").Text,
                "filter customer field date FIO line 2");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_10");
            Refresh();
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 2");
        }
    }
}