using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsFilterEmailTest : BaseSeleniumTest
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
        public void FilterEmail()
        {
            GoToAdmin("reviews");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            //search by exist mail 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnEmail");

            Driver.SetGridFilterValue("_noopColumnEmail", "asd2@asd.asd");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("asd2@asd.asd"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName3"));
            VerifyAreEqual("Текст отзыва 258", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 249", Driver.GetGridCell(9, "Text").Text);

            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("Текст отзыва 258", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 239", Driver.GetGridCell(19, "Text").Text);

            //search by not exist mail
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnEmail");
            Driver.SetGridFilterValue("_noopColumnEmail", "asd123453@asd.asd");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnEmail", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnEmail", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check delete filter
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnEmail");
            VerifyAreEqual("Найдено записей: 300",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Order(1)]
        [Test]
        public void Page()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnEmail");
            Driver.SetGridFilterValue("_noopColumnEmail", "asd2@asd.asd");

            VerifyAreEqual("Текст отзыва 258", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 249", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Текст отзыва 248", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 239", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Текст отзыва 238", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 229", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("Текст отзыва 228", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 219", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Текст отзыва 218", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 209", Driver.GetGridCell(9, "Text").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Текст отзыва 258", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 249", Driver.GetGridCell(9, "Text").Text);
        }

        [Order(1)]
        [Test]
        public void PageToPrevious()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnEmail");
            Driver.SetGridFilterValue("_noopColumnEmail", "asd2@asd.asd");

            VerifyAreEqual("Текст отзыва 258", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 249", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 248", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 239", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Текст отзыва 238", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 229", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 248", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 239", Driver.GetGridCell(9, "Text").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Текст отзыва 258", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 249", Driver.GetGridCell(9, "Text").Text);

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Текст отзыва 291", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 300", Driver.GetGridCell(9, "Text").Text);
        }


        [Order(10)]
        [Test]
        public void Delete()
        {
            GoToAdmin("reviews");

            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnEmail");
            Driver.SetGridFilterValue("_noopColumnEmail", "asd2@asd.asd");

            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnEmail");

            VerifyAreEqual("Найдено записей: 200",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            GoToAdmin("reviews");

            VerifyAreEqual("Найдено записей: 200",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }
    }
}