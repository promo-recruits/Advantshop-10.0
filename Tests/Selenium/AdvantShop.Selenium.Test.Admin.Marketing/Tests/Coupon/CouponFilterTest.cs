using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Coupon
{
    [TestFixture]
    public class CouponFilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Coupons\\Catalog.Brand.csv",
                "data\\Admin\\Coupons\\Catalog.Product.csv",
                "data\\Admin\\Coupons\\Catalog.Color.csv",
                "data\\Admin\\Coupons\\Catalog.Size.csv",
                "data\\Admin\\Coupons\\Catalog.Photo.csv",
                "data\\Admin\\Coupons\\Catalog.Offer.csv",
                "data\\Admin\\Coupons\\Catalog.Category.csv",
                "data\\Admin\\Coupons\\Catalog.ProductCategories.csv",
                "data\\Admin\\Coupons\\Catalog.Coupon.csv",
                "data\\Admin\\Coupons\\Catalog.CouponCategories.csv",
                "data\\Admin\\Coupons\\Catalog.CouponProducts.csv"
            );

            Init();
            GoToAdmin("settingscoupons#?couponsTab=coupons");
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
        DateTime date = DateTime.Now;

        [Test]
        [Order(0)]
        public void CouponFilterCode()
        {
            //Код купона
            Functions.GridFilterSet(Driver, BaseUrl, "Code");
            Driver.SetGridFilterValue("Code", "test2");
            Refresh();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                " count row");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count elem");
            VerifyAreEqual("test2", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem ");
            Functions.GridFilterClose(Driver, BaseUrl, "Code");
            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all elem");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "close value elem 1 ");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "close value elem 5 ");
        }

        [Test]
        [Order(3)]
        public void CouponFilterTypeFormatted()
        {
            //Тип 
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "TypeFormatted", filterItem: "Фиксированный");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 4,
                "count row ");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem 1 ");

            Driver.FindElement(AdvBy.DataE2E("gridFilterItemSelect")).Click();
            Driver.FindElement(AdvBy.DataE2E("Процентный")).Click();
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 2,
                "count row ");
            VerifyAreEqual("test3", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem 2 ");
            Functions.GridFilterClose(Driver, BaseUrl, "TypeFormatted");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "close value elem 1 ");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "close value elem 5 ");
        }

        [Test]
        [Order(4)]
        public void CouponFilterValue()
        {
            //Значение
            Functions.GridFilterSet(Driver, BaseUrl, "Value");
            Driver.SetGridFilterValue("Value", "30");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "count row ");
            VerifyAreEqual("test3", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem  ");
            Functions.GridFilterClose(Driver, BaseUrl, "Value");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "close value elem 1 ");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "close value elem 5 ");
        }

        [Test]
        [Order(1)]
        public void CouponFilterEnabled()
        {
            //Активность
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enabled", filterItem: "Активные");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 5,
                "count row ");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem  ");

            Driver.FindElement(AdvBy.DataE2E("gridFilterItemSelect")).Click();
            Driver.FindElement(AdvBy.DataE2E("Неактивные")).Click();
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "count row ");
            VerifyAreEqual("test5", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem 2 ");
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "close value elem 1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "close value elem 5");
        }

        [Test]
        [Order(2)]
        public void CouponFilterMinSumm()
        {
            //Min summ
            Functions.GridFilterSet(Driver, BaseUrl, "MinimalOrderPrice");
            Driver.SetGridFilterValue("MinimalOrderPrice", "100");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "count row ");
            VerifyAreEqual("test4", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem  ");
            Driver.SetGridFilterValue("MinimalOrderPrice", "300");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no count elem");

            Functions.GridFilterClose(Driver, BaseUrl, "MinimalOrderPrice");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "close value elem 1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "close value elem 5");
        }

        [Test]
        [Order(8)]
        public void CouponFilterDateStart()
        {
            //сперва создаем купон
            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Купон", Driver.FindElement(By.TagName("h2")).Text, "modal add h2");

            Driver.FindElement(AdvBy.DataE2E("couponCode")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponCode")).SendKeys("test123");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponType")))).SelectByText(
                "Процентный");
            Driver.FindElement(AdvBy.DataE2E("couponValue")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponValue")).SendKeys("50");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponCurrency"))))
                .SelectByText("Рубли");

            Driver.FindElement(AdvBy.DataE2E("couponStartDate")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(AdvBy.DataE2E("couponStartDate1")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponStartDate1")).SendKeys($"{date.AddYears(1):dd.MM.yyyy}");
            Driver.DropFocusCss(".control-label");

            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).SendKeys("500");

            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForToastSuccess();

            Functions.GridFilterSet(Driver, BaseUrl, "StartDateFormatted");
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterFrom")).Clear();
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterFrom")).SendKeys($"{date:dd.MM.yyyy}");
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterTo")).Clear();
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterTo")).SendKeys($"{date.AddYears(2):dd.MM.yyyy}");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "count row ");
            VerifyAreEqual("test123", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem ");
            Functions.GridFilterClose(Driver, BaseUrl, "StartDateFormatted");
            VerifyAreEqual("test123", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "close value elem 1");
            VerifyAreEqual("test6", Driver.GetGridCell(6, "Code", "Couponsdefault").Text, "close value elem 5");
        }

        [Test]
        [Order(5)]
        public void CouponFilterDateEnd()
        {
            //сперва отредакт. купон
            Driver.GetGridCell(0, "_serviceColumn", "Couponsdefault").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();
            Driver.FindElement(AdvBy.DataE2E("couponUseExpirationDate")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).SendKeys($"{date.AddYears(4):dd.MM.yyyy}");
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForToastSuccess();

            Functions.GridFilterSet(Driver, BaseUrl, "ExpirationDateFormatted");
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterFrom")).Clear();
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterFrom")).SendKeys($"{date.AddYears(3):dd.MM.yyyy}");
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterTo")).Clear();
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterTo")).SendKeys($"{date.AddYears(5):dd.MM.yyyy}");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "count row ");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem ");
            Functions.GridFilterClose(Driver, BaseUrl, "ExpirationDateFormatted");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "close value elem 1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "close value elem 5");
        }

        [Test]
        [Order(6)]
        public void CouponFilterDateAdded()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "AddingDateFormatted");
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterFrom")).Clear();
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterFrom")).SendKeys("15.11.2016");
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterTo")).Clear();
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterTo")).SendKeys("20.11.2016");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            Thread.Sleep(500);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 4,
                "count row ");
            VerifyAreEqual("test3", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem ");
            Functions.GridFilterClose(Driver, BaseUrl, "AddingDateFormatted");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "close value elem 1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "close value elem 5");
        }

        [Test]
        [Order(7)]
        public void CouponFilterDiscount()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnForFirstOrder", filterItem: "Да");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "count row ");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem ");
            Driver.FindElement(AdvBy.DataE2E("gridFilterItemSelect")).Click();
            Driver.FindElement(AdvBy.DataE2E("gridFilterItemSelect")).FindElement(By.TagName("input")).SendKeys("Нет" + Keys.Enter);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 5,
                "count row ");
            VerifyAreEqual("test2", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "value elem ");
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnForFirstOrder");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "close value elem 1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "close value elem 5");
        }
    }
}