using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Discount
{
    [TestFixture]
    public class DiscountsPriceRangeFilterTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\Discount\\Catalog.Product.csv",
                "data\\Admin\\Discount\\Catalog.Offer.csv",
                "data\\Admin\\Discount\\Catalog.Category.csv",
                "data\\Admin\\Discount\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Discount\\[Order].OrderSource.csv",
                "data\\Admin\\Discount\\[Order].OrderStatus.csv",
                "data\\Admin\\Discount\\[Order].OrderPriceDiscount.csv"
            );

            Init();
            GoToAdmin("settingscoupons#?couponsTab=discounts");
        }

        [Test]
        public void FilterByPriceRange()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "PriceRange");

            //search by not exist 
            Driver.SetGridFilterValue("PriceRange", "50000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("PriceRange", "111111111122222222222222222222222222222");
            Driver.DropFocusCss("[data-e2e=\"DiscountsTitle\"]");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]")).GetCssValue("border-color"),
                "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("PriceRange", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]")).Text,
                "invalid symbols");

            //search by exist
            Driver.SetGridFilterValue("PriceRange", "11");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "filter PriceRange line 1");
            VerifyAreEqual("118", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "filter PriceRange line 10");
            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter PriceRange count");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "PriceRange")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Скидка из стоимости заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 11",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter PriceRange return");
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"PriceRange\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "PriceRange");
            VerifyAreEqual("Найдено записей: 159",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter PriceRange deleting 1");

            Refresh();
            VerifyAreEqual("Найдено записей: 159",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter PriceRange deleting 2");
        }


        [Test]
        public void FilterByPercentDiscount()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "PercentDiscount");

            //search by not exist 
            Driver.SetGridFilterValue("PercentDiscount", "50000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("PercentDiscount", "111111111122222222222222222222222222222");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]")).GetCssValue("border-color"),
                "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("PercentDiscount", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemInput\"]")).Text,
                "invalid symbols");

            //search by exist
            Driver.SetGridFilterValue("PercentDiscount", "20");
            VerifyAreEqual("20", Driver.GetGridCell(0, "PercentDiscount", "PriceRange").Text,
                "filter PercentDiscount line 1");
            VerifyAreEqual("20", Driver.GetGridCell(1, "PercentDiscount", "PriceRange").Text,
                "filter PercentDiscount line 2");
            VerifyAreEqual("30", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "filter PriceRange line 1");
            VerifyAreEqual("120", Driver.GetGridCell(1, "PriceRange", "PriceRange").Text, "filter PriceRange line 2");
            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter PercentDiscount count");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "PriceRange").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Скидка из стоимости заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter PercentDiscount return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"PercentDiscount\"]")).Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "PercentDiscount");
            VerifyAreEqual("Найдено записей: 168",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter PercentDiscount deleting 1");

            Refresh();
            VerifyAreEqual("Найдено записей: 168",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter PercentDiscount deleting 2");
        }
    }
}