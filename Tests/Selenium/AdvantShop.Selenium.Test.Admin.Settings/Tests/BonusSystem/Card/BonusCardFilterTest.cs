using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.Card
{
    [TestFixture]
    public class BonusSystemCardFilter : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilter\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilter\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilter\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilter\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilter\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilter\\Customers.Customer.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilter\\Customers.Contact.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilter\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilter\\Bonus.Card.csv"
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
        public void CardFilterNum()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "CardNumber");

            //search by not exist card
            Driver.SetGridFilterValue("CardNumber", "53624351");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("CardNumber", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrr");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("CardNumber", "########@@@@@@@@&&&&&&&******,,,,..");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist card
            Driver.SetGridFilterValue("CardNumber", "530899");
            Driver.Blur();

            VerifyAreEqual("530899", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter result Card Number");
            VerifyAreEqual("LastName99 FirstName99", Driver.GetGridCell(0, "FIO").Text, "filter result FIO");
            VerifyAreEqual("Платиновый", Driver.GetGridCell(0, "GradeName").Text, "filter result GradeName");
            VerifyAreEqual("30", Driver.GetGridCell(0, "GradePersent").Text, "filter result Grade Percent");
            VerifyAreEqual("12.01.2017 15:40", Driver.GetGridCell(0, "CreatedFormatted").Text,
                "filter result card date");

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
                "filter card num return");
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"CardNumber\"]"))
                .Displayed);

            //check delete with filter
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "CardNumber");
            Refresh();
            VerifyAreEqual("Найдено записей: 119",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter card num deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 119",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter card num deleting 2");
        }


        [Test]
        public void CardFilterGrade()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "GradeName");

            //check filter GradeName no items
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Серебряный\"]")).Click();
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter GradeName no items count");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter GradeName no items");

            //check filter GradeName
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Золотой\"]")).Click();
            VerifyAreEqual("Найдено записей: 23",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter GradeName");

            VerifyAreEqual("530868", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "filter result GradeName");
            VerifyAreEqual("LastName68 FirstName68", Driver.GetGridCell(0, "FIO").Text, "filter result FIO");
            VerifyAreEqual("Золотой", Driver.GetGridCell(0, "GradeName").Text, "filter result GradeName");
            VerifyAreEqual("10", Driver.GetGridCell(0, "GradePersent").Text, "filter result Grade Percent");
            VerifyAreEqual("12.02.2017 15:40", Driver.GetGridCell(0, "CreatedFormatted").Text,
                "filter result card date");

            //check all GradeName
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count == 5,
                "count managers");
            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Карта')]"));
            VerifyIsTrue(Driver.Url.Contains("edit"), "card edit");

            Driver.Navigate().GoToUrl(strUrl);

            Driver.WaitForElem(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total"));
            VerifyAreEqual("Найдено записей: 23",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter GradeName return");
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"GradeName\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "GradeName");
            Refresh();
            VerifyAreEqual("Найдено записей: 97",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter GradeName deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 97",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter GradeName deleting 2");
        }

        [Test]
        public void CardFilterCreatedDate()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "CreatedFormatted");

            //check filter min not exist
            Driver.SetGridFilterRange("CreatedFormatted", "31.12.2050 00:00", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter add date min not exist");

            //check max not exist
            Driver.SetGridFilterRange("CreatedFormatted", "", "31.12.2050 00:00");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("CreatedFormatted", "31.12.2050 00:00", "31.12.2050 00:00");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "filter add date  min/max not exist");

            //check filter add date 
            Driver.SetGridFilterRange("CreatedFormatted", "01.03.2017 00:00", "05.05.2017 00:00");
            VerifyAreEqual("Найдено записей: 51",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date");

            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "card num add date line 1");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedFormatted").Text.Contains("2017"), "add date line 1");
            VerifyAreEqual("530810", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "card num add date line 10");
            VerifyIsTrue(Driver.GetGridCell(9, "CreatedFormatted").Text.Contains("2017"), "add date line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "CreatedFormatted");
            VerifyAreEqual("Найдено записей: 69",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date after deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 69",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter add date after deleting 2");
        }

        [Test]
        public void CardFilterBonusAmount()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnBonusAmount");

            //check min too much symbols
            Driver.SetGridFilterRange("_noopColumnBonusAmount", "1111111111", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("_noopColumnBonusAmount", "", "1111111111"); 
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max many symbols");

            //check min and max too much symbols
            Driver.SetGridFilterRange("_noopColumnBonusAmount", "1111111111", "1111111111");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max many symbols");

            //check invalid symbols

            //check min invalid symbols
            Driver.SetGridFilterRange("_noopColumnBonusAmount", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter min imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards min many symbols");

            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnBonusAmount");

            //check max invalid symbols
            Driver.SetGridFilterRange("_noopColumnBonusAmount", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards max many symbols");

            //check min and max invalid symbols

            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnBonusAmount");

            Driver.SetGridFilterRange("_noopColumnBonusAmount", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards min/max many symbols");

            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnBonusAmount");

            //check filter min not exist
            Driver.SetGridFilterRange("_noopColumnBonusAmount", "1000", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min not exist");

            //check max not exist
            Driver.SetGridFilterRange("_noopColumnBonusAmount", "", "1000");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("_noopColumnBonusAmount", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max not exist");

            //check filter 
            Driver.SetGridFilterRange("_noopColumnBonusAmount", "40", "99");
            VerifyAreEqual("Найдено записей: 60",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter bonus amount");

            VerifyAreEqual("530840", Driver.GetGridCell(0, "CardNumber").Text, "bonus amount card num line 1");
            VerifyAreEqual("530849", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "bonus amount card num line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnBonusAmount");
            VerifyAreEqual("Найдено записей: 60",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter bonus amount after deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 60",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter bonus amount after deleting 2");
        }

        [Test]
        public void CardFilterFIO()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnFIO");

            //search by not exist Contact
            Driver.SetGridFilterValue("_noopColumnFIO", "123123123 name contact 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnFIO", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnFIO", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist FIO
            Driver.SetGridFilterValue("_noopColumnFIO", "LastName2");

            VerifyAreEqual("530802", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "CardNumber line 1");
            VerifyAreEqual("LastName2 FirstName2", Driver.GetGridCell(0, "FIO").Text, "FIO line 1");
            VerifyAreEqual("530828", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "CardNumber line 10");
            VerifyAreEqual("LastName28 FirstName28", Driver.GetGridCell(9, "FIO").Text, "FIO line 10");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter FIO");

            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Карта')]"));
            VerifyIsTrue(Driver.Url.Contains("edit"), "card edit");

            Driver.Navigate().GoToUrl(strUrl);

            Driver.WaitForElem(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total"));
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter FIO return");
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector(
                        "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnFIO\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnFIO");
            VerifyAreEqual("Найдено записей: 109",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter FIO deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 109",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter FIO deleting 2");
        }

        [Test]
        public void CardFilterEmail()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnEmail");

            //search by not exist Contact
            Driver.SetGridFilterValue("_noopColumnEmail", "LastName2 FirstName2");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnEmail", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnEmail", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist email
            Driver.SetGridFilterValue("_noopColumnEmail", "test@mail.ru3");

            VerifyAreEqual("530803", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "CardNumber line 1");
            VerifyAreEqual("LastName3 FirstName3", Driver.GetGridCell(0, "FIO").Text, "FIO line 1");
            VerifyAreEqual("530838", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "CardNumber line 10");
            VerifyAreEqual("LastName38 FirstName38", Driver.GetGridCell(9, "FIO").Text, "FIO line 10");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter email");

            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Карта')]"));
            VerifyIsTrue(Driver.Url.Contains("edit"), "card edit");

            Driver.Navigate().GoToUrl(strUrl);

            Driver.WaitForElem(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total"));
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter email return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnEmail\"]")).Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnEmail");
            VerifyAreEqual("Найдено записей: 109",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter email deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 109",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter email deleting 2");
        }

        [Test]
        public void CardFilterMobile()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnMobilePhone");

            //search by not exist Contact
            Driver.SetGridFilterValue("_noopColumnMobilePhone", "89279272727");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnMobilePhone", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnMobilePhone", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist mobile
            Driver.SetGridFilterValue("_noopColumnMobilePhone", "3");

            VerifyAreEqual("530803", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "CardNumber line 1");
            VerifyAreEqual("LastName3 FirstName3", Driver.GetGridCell(0, "FIO").Text, "FIO line 1");
            VerifyAreEqual("530836", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "CardNumber line 9");
            VerifyAreEqual("LastName36 FirstName36", Driver.GetGridCell(9, "FIO").Text, "FIO line 9");
            VerifyAreEqual("Найдено записей: 21",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter mobile");

            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Карта')]"));
            VerifyIsTrue(Driver.Url.Contains("edit"), "card edit");

            Driver.Navigate().GoToUrl(strUrl);

            Driver.WaitForElem(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total"));
            VerifyAreEqual("Найдено записей: 21",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter mobile return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnMobilePhone\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnMobilePhone");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter mobile deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 99",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter mobile deleting 2");
        }

        [Test]
        public void CardFilterRegisterDate()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnRegDate");

            //check filter min not exist
            Driver.SetGridFilterRange("_noopColumnRegDate", "31.12.2050 00:00", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter reg date min not exist");

            //check max not exist
            Driver.SetGridFilterRange("_noopColumnRegDate", "", "31.12.2050 00:00"); 
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter reg date max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("_noopColumnRegDate", "31.12.2050 00:00", "31.12.2050 00:00");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter reg date min/max not exist");

            //check filter reg date 
            Driver.SetGridFilterRange("_noopColumnRegDate", "01.07.2017 12:00", "15.08.2017 21:00");
            VerifyAreEqual("Найдено записей: 46",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter reg date");

            VerifyAreEqual("530875", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "card num reg date line 1");
            VerifyAreEqual("530884", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "card num reg date line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnRegDate");
            VerifyAreEqual("Найдено записей: 74",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter reg date after deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 74",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter reg date after deleting 2");
        }

        [Test]
        public void CardFilterLocation()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnLocation");

            //search by not exist Contact
            Driver.SetGridFilterValue("_noopColumnLocation", "Самара");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnLocation", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnLocation", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist location
            Driver.SetGridFilterValue("_noopColumnLocation", "Москва");

            VerifyAreEqual("530801", Driver.GetGridCell(0, "CardNumber").FindElement(By.TagName("a")).Text,
                "CardNumber line 1");
            VerifyAreEqual("LastName1 FirstName1", Driver.GetGridCell(0, "FIO").Text, "FIO line 1");
            VerifyAreEqual("530810", Driver.GetGridCell(9, "CardNumber").FindElement(By.TagName("a")).Text,
                "CardNumber line 10");
            VerifyAreEqual("LastName10 FirstName10", Driver.GetGridCell(9, "FIO").Text, "FIO line 10");
            VerifyAreEqual("Найдено записей: 47",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter location");

            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Карта')]"));
            VerifyIsTrue(Driver.Url.Contains("edit"), "card edit");

            Driver.Navigate().GoToUrl(strUrl);

            Driver.WaitForElem(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total"));
            VerifyAreEqual("Найдено записей: 47",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter location return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnLocation\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnLocation");
            VerifyAreEqual("Найдено записей: 73",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter location deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 73",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter location deleting 2");
        }
    }
}