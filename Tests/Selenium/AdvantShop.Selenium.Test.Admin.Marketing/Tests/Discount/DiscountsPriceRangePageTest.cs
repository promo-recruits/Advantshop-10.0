using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Discount
{
    [TestFixture]
    public class DiscountsPriceRangePageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
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
        public void PageDiscount()
        {
            GoToAdmin("settingscoupons#?couponsTab=discounts");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 1 line 1");
            VerifyAreEqual("20", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("21", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 2 line 1");
            VerifyAreEqual("30", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("31", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 3 line 1");
            VerifyAreEqual("40", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("41", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 4 line 1");
            VerifyAreEqual("50", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 4 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("51", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 5 line 1");
            VerifyAreEqual("60", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 5 line 10");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("61", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 6 line 1");
            VerifyAreEqual("70", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 6 line 10");
        }

        [Test]
        public void PageDiscountToBegin()
        {
            GoToAdmin("settingscoupons#?couponsTab=discounts");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 1 line 1");
            VerifyAreEqual("20", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 1 line 10");

            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("21", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 2 line 1");
            VerifyAreEqual("30", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("31", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 3 line 1");
            VerifyAreEqual("40", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("41", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 4 line 1");
            VerifyAreEqual("50", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 4 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("51", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 5 line 1");
            VerifyAreEqual("60", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 5 line 10");

            //to begin
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 1 line 1");
            VerifyAreEqual("20", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 1 line 10");
        }

        [Test]
        public void PageDiscountToEnd()
        {
            GoToAdmin("settingscoupons#?couponsTab=discounts");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 1 line 1");
            VerifyAreEqual("20", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 1 line 10");

            //to end
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("171", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "last page line 1");
            VerifyAreEqual("180", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "last page line 10");
        }

        [Test]
        public void PageDiscountToPrevious()
        {
            GoToAdmin("settingscoupons#?couponsTab=discounts");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 1 line 1");
            VerifyAreEqual("20", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("21", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 2 line 1");
            VerifyAreEqual("30", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("31", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 3 line 1");
            VerifyAreEqual("40", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("21", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 2 line 1");
            VerifyAreEqual("30", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 1 line 1");
            VerifyAreEqual("20", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 1 line 10");
        }

        [Test]
        public void PageDiscountToNext()
        {
            GoToAdmin("settingscoupons#?couponsTab=discounts");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 1 line 1");
            VerifyAreEqual("20", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 1 line 10");

            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("21", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 2 line 1");
            VerifyAreEqual("30", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 2 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("31", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 3 line 1");
            VerifyAreEqual("40", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 3 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("41", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 4 line 1");
            VerifyAreEqual("50", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 4 line 10");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("51", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 5 line 1");
            VerifyAreEqual("60", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 5 line 10");
        }

        [Test]
        public void DiscountPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "find elem 170");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 1 line 1");
            VerifyAreEqual("20", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "page 1 line 10");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "page 1 line 1");
            VerifyAreEqual("110", Driver.GetGridCell(99, "PriceRange", "PriceRange").Text, "page 1 line 100");

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void SelectAndDelete()
        {
            GoToAdmin("settingscoupons#?couponsTab=discounts");
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "PriceRange")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "1 grid canсel delete");
            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "PriceRange")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "PriceRange")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "PriceRange")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "PriceRange")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "PriceRange")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "PriceRange")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "PriceRange")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            Driver.DropFocusCss("[data-e2e=\"DiscountsTitle\"]");
            VerifyAreEqual("15", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "selected 2 grid delete");
            VerifyAreEqual("16", Driver.GetGridCell(1, "PriceRange", "PriceRange").Text, "selected 3 grid delete");
            VerifyAreEqual("17", Driver.GetGridCell(2, "PriceRange", "PriceRange").Text, "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "PriceRange")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "PriceRange")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("25", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text,
                "selected all on page 2 grid delete"); //default status
            VerifyAreEqual("34", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("156", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "PriceRange")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "PriceRange")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "1 delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "1 count all after deleting");

            Refresh();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "2 delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "2 count all after deleting");
        }
    }
}