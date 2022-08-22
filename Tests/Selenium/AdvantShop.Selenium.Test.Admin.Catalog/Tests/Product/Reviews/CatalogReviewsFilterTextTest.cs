using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsFilterTextTest : BaseSeleniumTest
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
        public void FilterText()
        {
            GoToAdmin("reviews");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            //search by exist text 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnText");
            Driver.SetGridFilterValue("_noopColumnText", "Текст отзыва 1");
            Refresh();

            VerifyAreEqual("Найдено записей: 111",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            VerifyAreEqual("Текст отзыва 16", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 12", Driver.GetGridCell(9, "Text").Text);

            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("Текст отзыва 16", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 100", Driver.GetGridCell(19, "Text").Text);

            //search by not exist text
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnText");
            Driver.SetGridFilterValue("_noopColumnText", "Текст отзыва999");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnText", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnText", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnText");
            VerifyAreEqual("Найдено записей: 300",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Test]
        [Order(1)]
        public void Page()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnText");
            Driver.SetGridFilterValue("_noopColumnText", "Текст отзыва 1");

            VerifyAreEqual("Текст отзыва 16", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 12", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Текст отзыва 199", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 100", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Текст отзыва 101", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 110", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("Текст отзыва 111", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 120", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Текст отзыва 121", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 130", Driver.GetGridCell(9, "Text").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Текст отзыва 16", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 12", Driver.GetGridCell(9, "Text").Text);
        }

        [Test]
        [Order(1)]
        public void PageToPrevious()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnText");
            Driver.SetGridFilterValue("_noopColumnText", "Текст отзыва 1");

            VerifyAreEqual("Текст отзыва 16", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 12", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 199", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 100", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 101", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 110", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 199", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 100", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 16", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 12", Driver.GetGridCell(9, "Text").Text);

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Текст отзыва 1", Driver.GetGridCell(0, "Text").Text);
        }

        [Test]
        [Order(10)]
        public void Delete()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnText");
            Driver.SetGridFilterValue("_noopColumnText", "Текст отзыва 1");
            Refresh();

            VerifyAreEqual("Найдено записей: 111",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnText");

            VerifyAreEqual("Найдено записей: 189",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            GoToAdmin("reviews");

            VerifyAreEqual("Найдено записей: 189",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }
    }
}