using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Coupon
{
    [TestFixture]
    public class CouponPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Coupons\\ManyCoupon\\Catalog.Brand.csv",
                "data\\Admin\\Coupons\\ManyCoupon\\Catalog.Color.csv",
                "data\\Admin\\Coupons\\ManyCoupon\\Catalog.Size.csv",
                "data\\Admin\\Coupons\\ManyCoupon\\Catalog.Product.csv",
                "data\\Admin\\Coupons\\ManyCoupon\\Catalog.Photo.csv",
                "data\\Admin\\Coupons\\ManyCoupon\\Catalog.Offer.csv",
                "data\\Admin\\Coupons\\ManyCoupon\\Catalog.Category.csv",
                "data\\Admin\\Coupons\\ManyCoupon\\Catalog.ProductCategories.csv",
                "data\\Admin\\Coupons\\ManyCoupon\\Catalog.Coupon.csv",
                "data\\Admin\\Coupons\\ManyCoupon\\Catalog.CouponCategories.csv",
                "data\\Admin\\Coupons\\ManyCoupon\\Catalog.CouponProducts.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("settingscoupons#?couponsTab=coupons");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void PageCoupon()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 1");
            VerifyAreEqual("test10", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 10");

            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("test11", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 11");
            VerifyAreEqual("test20", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 20");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("test21", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 21");
            VerifyAreEqual("test30", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 30");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("test31", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 31");
            VerifyAreEqual("test40", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 40");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("test41", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 41");
            VerifyAreEqual("test50", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 50");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("test51", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 51");
            VerifyAreEqual("test60", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 60");
        }

        [Test]
        public void PageCouponToBegin()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 1");
            VerifyAreEqual("test10", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 10");

            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test11", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 11");
            VerifyAreEqual("test20", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 20");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test21", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 21");
            VerifyAreEqual("test30", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 30");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test31", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 31");
            VerifyAreEqual("test40", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 40");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test41", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 41");
            VerifyAreEqual("test50", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 50");

            //to begin
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 1");
            VerifyAreEqual("test10", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 10");
        }

        [Test]
        public void PageCouponToEnd()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 1");
            VerifyAreEqual("test10", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 10");

            //to end
            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("test101", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 101");
        }

        [Test]
        public void PageCouponToNext()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 1");
            VerifyAreEqual("test10", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 10");

            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test11", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 11");
            VerifyAreEqual("test20", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 20");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test21", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 21");
            VerifyAreEqual("test30", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 30");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test31", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 31");
            VerifyAreEqual("test40", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 40");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test41", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 41");
            VerifyAreEqual("test50", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 50");
        }

        [Test]
        public void PageCouponToPrevious()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 1");
            VerifyAreEqual("test10", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 10");

            Driver.ScrollTo(AdvBy.DataE2E("gridPagination"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test11", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 11");
            VerifyAreEqual("test20", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 20");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test21", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 21");
            VerifyAreEqual("test30", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 30");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("test11", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 11");
            VerifyAreEqual("test20", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 20");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 1");
            VerifyAreEqual("test10", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 10");
        }

        [Test]
        public void CouponPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 1");
            VerifyAreEqual("test10", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "line 10");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "line 1");
            VerifyAreEqual("test100", Driver.GetGridCell(99, "Code", "Couponsdefault").Text, "line 100");
            
            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void SelectAndDelete()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            //check delete and cancel
            Driver.GetGridCell(0, "_serviceColumn", "Couponsdefault").FindElement(By.TagName("ui-grid-custom-delete"))
                .Click();
            Driver.SwalCancel();
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "cancel del");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Couponsdefault").FindElement(By.TagName("ui-grid-custom-delete"))
                .Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "del 1 elem");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Couponsdefault")
                .FindElement(AdvBy.DataE2E("gridCheckboxWrapSelect")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Couponsdefault")
                .FindElement(AdvBy.DataE2E("gridCheckboxWrapSelect")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Couponsdefault")
                .FindElement(AdvBy.DataE2E("gridCheckboxWrapSelect")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("gridCheckboxSelect")).Selected, "Selected 1");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("gridCheckboxSelect")).Selected, "Selected 2");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("gridCheckboxSelect")).Selected, "Selected 3");
            VerifyAreEqual("3", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreNotEqual("test2", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "after del  1");
            VerifyAreNotEqual("test3", Driver.GetGridCell(1, "Code", "Couponsdefault").Text, "after del  2");
            VerifyAreNotEqual("test4", Driver.GetGridCell(2, "Code", "Couponsdefault").Text, "after del  3");

            //check select all on page
            Driver.FindElement(AdvBy.DataE2E("gridHeaderCheckboxWrapSelectAll")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("gridCheckboxSelect")).Selected, "Selected on page 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("gridCheckboxSelect")).Selected, "Selected on page 10");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("test15", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "after del on page 1");
            VerifyAreEqual("test24", Driver.GetGridCell(9, "Code", "Couponsdefault").Text, "after del on page 10");

            //check select all
            Driver.FindElement(AdvBy.DataE2E("gridHeaderCheckboxWrapSelectAll")).Click();
            Driver.FindElement(AdvBy.DataE2E("gridSelectionSelectAllFn")).Click();
            VerifyAreEqual("87", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "count after del");

            //check deselect all
            Driver.FindElement(AdvBy.DataE2E("gridSelectionDeselectAllFn")).Click();
            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Couponsdefault").FindElement
                (AdvBy.DataE2E("gridCheckboxSelect")).Selected,
                "not Selected after del");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Couponsdefault").FindElement
                (AdvBy.DataE2E("gridCheckboxSelect")).Selected,
                "not Selected after del");

            //check delete all
            Driver.FindElement(AdvBy.DataE2E("gridHeaderCheckboxWrapSelectAll")).Click();
            Driver.FindElement(AdvBy.DataE2E("gridSelectionSelectAllFn")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no element");

            Refresh();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem after refresh");
        }
    }
}