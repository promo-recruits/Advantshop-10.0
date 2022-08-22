using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsFilterArtNoTest : BaseSeleniumTest
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

        [Order(1)]
        [Test]
        public void FilterArtNo()
        {
            GoToAdmin("reviews");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            //search by exist art no 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnArtNo");
            Driver.SetGridFilterValue("_noopColumnArtNo", "10");
            Driver.DropFocus("h1");
            Refresh();
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "ProductName").Text);
            VerifyAreEqual("Текст отзыва 200", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            VerifyAreEqual("Текст отзыва 200", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 191", Driver.GetGridCell(9, "Text").Text);

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("Текст отзыва 200", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 190", Driver.GetGridCell(99, "Text").Text);

            //search by not exist art no
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnArtNo");
            Driver.SetGridFilterValue("_noopColumnArtNo", "15");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnArtNo", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnArtNo", "########@@@@@@@@&&&&&&&******,,,,..");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnArtNo");
            VerifyAreEqual("Найдено записей: 300",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Order(1)]
        [Test]
        public void Page()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnArtNo");
            Driver.SetGridFilterValue("_noopColumnArtNo", "10");

            VerifyAreEqual("Текст отзыва 200", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 191", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Текст отзыва 101", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 110", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Текст отзыва 111", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 120", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("Текст отзыва 121", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 130", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Текст отзыва 131", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 140", Driver.GetGridCell(9, "Text").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Текст отзыва 200", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 191", Driver.GetGridCell(9, "Text").Text);
        }

        [Order(1)]
        [Test]
        public void PageToPrevious()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnArtNo");
            Driver.SetGridFilterValue("_noopColumnArtNo", "10");

            VerifyAreEqual("Текст отзыва 200", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 191", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 101", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 110", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 111", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 120", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 101", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 110", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 200", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 191", Driver.GetGridCell(9, "Text").Text);

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Текст отзыва 181", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 190", Driver.GetGridCell(9, "Text").Text);
        }

        [Order(10)]
        [Test]
        public void Delete()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnArtNo");
            Driver.SetGridFilterValue("_noopColumnArtNo", "10");
            Refresh();
            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnArtNo");

            VerifyAreEqual("Найдено записей: 200",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            GoToAdmin("reviews");

            VerifyAreEqual("Найдено записей: 200",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }
    }
}