﻿using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsFilterNameTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Reviews\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\Reviews\\Customers.CustomerGroup.csv",
                "data\\Admin\\Catalog\\Reviews\\Customers.Customer.csv",
                "data\\Admin\\Catalog\\Reviews\\CMS.Review.csv"
            );

            Init();
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
        [Order(1)]
        public void FilterName()
        {
            GoToAdmin("reviews");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            //search by exist name 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnName");
            Driver.SetGridFilterValue("_noopColumnName", "CustomerName2");
            Refresh();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName2"));
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Найдено записей: 196",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 20", Driver.GetGridCell(9, "Text").Text);

            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 10", Driver.GetGridCell(19, "Text").Text);

            //search by not exist name
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnName");
            Driver.SetGridFilterValue("_noopColumnName", "CustomerName2456");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnName", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnName", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnName");
            VerifyAreEqual("Найдено записей: 300",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Test]
        [Order(1)]
        public void Page()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnName");
            Driver.SetGridFilterValue("_noopColumnName", "CustomerName2");

            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 20", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Текст отзыва 21", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 10", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Текст отзыва 11", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 45", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("Текст отзыва 40", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 35", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Текст отзыва 32", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 192", Driver.GetGridCell(9, "Text").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 20", Driver.GetGridCell(9, "Text").Text);
        }

        [Test]
        [Order(1)]
        public void PageToPrevious()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnName");
            Driver.SetGridFilterValue("_noopColumnName", "CustomerName2");

            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 20", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 21", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 10", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 11", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 45", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 21", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 10", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 20", Driver.GetGridCell(9, "Text").Text);

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Текст отзыва 188", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 5", Driver.GetGridCell(5, "Text").Text);
        }

        [Test]
        [Order(10)]
        public void Delete()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnName");
            Driver.SetGridFilterValue("_noopColumnName", "CustomerName2");
            Refresh();
            VerifyAreEqual("Найдено записей: 196",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnName");

            VerifyAreEqual("Найдено записей: 104",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            GoToAdmin("reviews");

            VerifyAreEqual("Найдено записей: 104",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }
    }
}