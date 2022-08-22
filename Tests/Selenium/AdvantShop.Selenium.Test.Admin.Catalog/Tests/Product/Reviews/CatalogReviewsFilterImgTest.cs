using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsFilterImgTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Reviews\\Catalog.Photo.csv",
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
        public void FilterImg()
        {
            GoToAdmin("reviews");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            //check without img 
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoName", filterItem: "Без фотографии");
            VerifyAreEqual("Найдено записей: 200",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.GetGridCell(9, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));

            //check with img 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"С фотографией\"]")).Click();
            Thread.Sleep(1000);
            Driver.DropFocus("h1");
            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            VerifyAreEqual("Текст отзыва 200", Driver.GetGridCell(0, "Text").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsFalse(Driver.GetGridCell(9, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));

            Driver.GridPaginationSelectItems("20");
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsFalse(Driver.GetGridCell(19, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "PhotoName");
            VerifyAreEqual("Найдено записей: 300",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Test]
        [Order(1)]
        public void Page()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoName", filterItem: "Без фотографии");
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
            VerifyAreEqual("Текст отзыва 250", Driver.GetGridCell(9, "Text").Text);

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
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoName", filterItem: "Без фотографии");
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
            VerifyAreEqual("Текст отзыва 291", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 300", Driver.GetGridCell(9, "Text").Text);
        }

        [Test]
        [Order(10)]
        public void Delete()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoName", filterItem: "Без фотографии");

            VerifyAreEqual("Найдено записей: 200",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Functions.GridFilterClose(Driver, BaseUrl, name: "PhotoName");

            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            GoToAdmin("reviews");

            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }
    }
}