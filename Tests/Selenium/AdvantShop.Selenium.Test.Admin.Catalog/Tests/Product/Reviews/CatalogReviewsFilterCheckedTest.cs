using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsFilterCheckedTest : BaseSeleniumTest
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
        public void FilterChecked()
        {
            GoToAdmin("reviews");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Checked", filterItem: "Нет");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyIsFalse(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "Checked").FindElement(By.TagName("input")).Selected);

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Да\"]")).Click();
            Thread.Sleep(1000);
            Driver.DropFocus("h1");
            Refresh();
            VerifyAreEqual("Найдено записей: 199",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "Checked").FindElement(By.TagName("input")).Selected);

            Driver.GridPaginationSelectItems("100");
            Thread.Sleep(1000);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            VerifyIsTrue(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(99, "Checked").FindElement(By.TagName("input")).Selected);
        }

        [Order(1)]
        [Test]
        public void Page()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Checked", filterItem: "Да");
            Driver.Blur();

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
            VerifyAreEqual("Текст отзыва 191", Driver.GetGridCell(9, "Text").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 20", Driver.GetGridCell(9, "Text").Text);
        }

        [Order(1)]
        [Test]
        public void PageToPrevious()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Checked", filterItem: "Да");
            Driver.Blur();

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
            VerifyAreEqual("Текст отзыва 189", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 2", Driver.GetGridCell(8, "Text").Text);
        }

        [Order(10)]
        [Test]
        public void Delete()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Checked", filterItem: "Да");

            VerifyAreEqual("Найдено записей: 199",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Functions.GridFilterClose(Driver, BaseUrl, name: "Checked");

            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            GoToAdmin("reviews");

            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }
    }
}