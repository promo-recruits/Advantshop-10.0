using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.GeneratedCoupons
{
    [TestFixture]
    public class GeneratedCouponsFilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\GeneratedCoupons\\Catalog.Brand.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Product.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Color.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Size.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Photo.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Offer.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Category.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.ProductCategories.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Coupon.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.CouponCategories.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.CouponProducts.csv"
            );

            Init();
            GoToAdmin("settingscoupons#?couponsTab=couponsGenerated");
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
        public void GeneratedCouponsFilterCode()
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
            VerifyAreEqual("test2", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "value elem ");
            Functions.GridFilterClose(Driver, BaseUrl, "Code");
            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all elem");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "close value elem 1 ");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "close value elem 5 ");
        }

        [Test]
        [Order(3)]
        public void GeneratedCouponsFilterTypeFormatted()
        {
            //Тип 
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "TypeFormatted", filterItem: "Фиксированный");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 4,
                "count row ");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "value elem 1 ");

            Driver.FindElement(AdvBy.DataE2E("gridFilterItemSelect")).Click();
            Driver.FindElement(AdvBy.DataE2E("Процентный")).Click();
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 2,
                "count row ");
            VerifyAreEqual("test3", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "value elem 2 ");
            Functions.GridFilterClose(Driver, BaseUrl, "TypeFormatted");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "close value elem 1 ");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "close value elem 5 ");
        }

        [Test]
        [Order(4)]
        public void GeneratedCouponsFilterValue()
        {
            //Значение
            Functions.GridFilterSet(Driver, BaseUrl, "Value");
            Driver.SetGridFilterValue("Value", "30");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "count row ");
            VerifyAreEqual("test3", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "value elem  ");
            Functions.GridFilterClose(Driver, BaseUrl, "Value");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "close value elem 1 ");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "close value elem 5 ");
        }

        [Test]
        [Order(1)]
        public void GeneratedCouponsFilterEnabled()
        {
            //Активность
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enabled", filterItem: "Активные");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 5,
                "count row ");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "value elem  ");

            Driver.FindElement(AdvBy.DataE2E("gridFilterItemSelect")).Click();
            Driver.FindElement(AdvBy.DataE2E("Неактивные")).Click();
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "count row ");
            VerifyAreEqual("test5", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "value elem 2 ");
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "close value elem 1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "close value elem 5");
        }

        [Test]
        [Order(2)]
        public void GeneratedCouponsFilterMinSumm()
        {
            //Min summ
            Functions.GridFilterSet(Driver, BaseUrl, "MinimalOrderPrice");
            Driver.SetGridFilterValue("MinimalOrderPrice", "100");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "count row ");
            VerifyAreEqual("test4", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "value elem  ");
            Driver.SetGridFilterValue("MinimalOrderPrice", "300");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no count elem");

            Functions.GridFilterClose(Driver, BaseUrl, "MinimalOrderPrice");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "close value elem 1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "close value elem 5");
        }

        [Test]
        [Order(6)]
        public void GeneratedCouponsFilterDateEnd()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "ExpirationDateFormatted");
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterFrom")).Clear();
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterFrom")).SendKeys($"{date.AddYears(3):dd.MM.yyyy}");
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterTo")).Clear();
            Driver.FindElement(AdvBy.DataE2E("datetimeFilterTo")).SendKeys($"{date.AddYears(10):dd.MM.yyyy}");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "count row ");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "value elem ");
            Functions.GridFilterClose(Driver, BaseUrl, "ExpirationDateFormatted");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "close value elem 1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "close value elem 5");
        }

        [Test]
        [Order(5)]
        public void GeneratedCouponsFilterDateAdded()
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
            VerifyAreEqual("test3", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "value elem ");
            Functions.GridFilterClose(Driver, BaseUrl, "AddingDateFormatted");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "close value elem 1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "close value elem 5");
        }

        [Test]
        [Order(7)]
        public void GeneratedCouponsFilterDiscount()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnForFirstOrder", filterItem: "Да");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "count row ");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "value elem ");
            Driver.FindElement(AdvBy.DataE2E("gridFilterItemSelect")).Click();
            Driver.FindElement(AdvBy.DataE2E("gridFilterItemSelect")).FindElement(By.TagName("input")).SendKeys("Нет" + Keys.Enter);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 5,
                "count row ");
            VerifyAreEqual("test2", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "value elem ");
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnForFirstOrder");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "close value elem 1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "close value elem 5");
        }
    }
}