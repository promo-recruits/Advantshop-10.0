using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.Card
{
    [TestFixture]
    public class BonusSystemCardFilterOrders : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses | ClearType.Orders);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\Customers.Customer.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\Customers.Contact.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\[Order].OrderSource.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\[Order].OrderStatus.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\[Order].[Order].csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\[Order].OrderContact.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\[Order].OrderCurrency.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\[Order].OrderItems.csv",
                "data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\[Order].OrderCustomer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\BonusCardFilterOrders\\Bonus.Card.csv"
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
        public void CardFilterOrdersCount()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnOrdersCount");

            //check min too much symbols
            Driver.SetGridFilterRange("_noopColumnOrdersCount", "1111111111", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("_noopColumnOrdersCount", "", "1111111111");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max many symbols");

            //check min and max too much symbols
            Driver.SetGridFilterRange("_noopColumnOrdersCount", "1111111111", "1111111111");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max many symbols");

            //check invalid symbols
            //check min invalid symbols
            Driver.SetGridFilterRange("_noopColumnOrdersCount", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter min imvalid symbols");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards min many symbols");

            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnOrdersCount");

            //check max invalid symbols
            Driver.SetGridFilterRange("_noopColumnOrdersCount", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter max imvalid symbols");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards max many symbols");

            //check min and max invalid symbols
            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnOrdersCount");

            Driver.SetGridFilterRange("_noopColumnOrdersCount", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards min/max many symbols");

            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnOrdersCount");

            //check filter min not exist
            Driver.SetGridFilterRange("_noopColumnOrdersCount", "1000", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min not exist");

            //check max not exist
            Driver.SetGridFilterRange("_noopColumnOrdersCount", "", "1000");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("_noopColumnOrdersCount", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max not exist");

            //check filter 
            Driver.SetGridFilterRange("_noopColumnOrdersCount", "2", "12");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter cards count");

            VerifyAreEqual("530805", Driver.GetGridCell(0, "CardNumber").Text, "orders count card num line 1");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnOrdersCount");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter orders count after deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter orders count after deleting 2");
        }

        [Test]
        public void CardFilterOrdersSum()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnOrdersSum");

            //check min too much symbols
            Driver.SetGridFilterRange("_noopColumnOrdersSum", "1111111111", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("_noopColumnOrdersSum", "", "1111111111");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max many symbols");

            //check min and max too much symbols
            Driver.SetGridFilterRange("_noopColumnOrdersSum", "1111111111", "1111111111");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max many symbols");

            //check invalid symbols

            //check min invalid symbols
            Driver.SetGridFilterRange("_noopColumnOrdersSum", "########@@@@@@@@&&&&&&&******", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter min imvalid symbols");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sum cards min many symbols");

            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnOrdersSum");

            //check max invalid symbols
            Driver.SetGridFilterRange("_noopColumnOrdersSum", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter max imvalid symbols");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sum cards max many symbols");

            //check min and max invalid symbols
            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnOrdersSum");

            Driver.SetGridFilterRange("_noopColumnOrdersSum", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sum cards min/max many symbols");

            GoToAdmin("cards");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnOrdersSum");

            //check filter min not exist
            Driver.SetGridFilterRange("_noopColumnOrdersSum", "1000", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min not exist");

            //check max not exist
            Driver.SetGridFilterRange("_noopColumnOrdersSum", "", "1000");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("_noopColumnOrdersSum", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max not exist");

            //check filter 
            Driver.SetGridFilterRange("_noopColumnOrdersSum", "18", "19");
            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter cards sum");

            VerifyAreEqual("530803", Driver.GetGridCell(0, "CardNumber").Text, "orders sum card num line 1");
            VerifyAreEqual("530804", Driver.GetGridCell(1, "CardNumber").Text, "orders sum card num line 1");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnOrdersSum");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter orders sum after deleting 1");

            GoToAdmin("cards");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter orders sum after deleting 2");
        }
    }
}