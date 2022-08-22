using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsFilterAddDateTest : BaseSeleniumTest
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
        public void FilterAddDateMinMax()
        {
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "AddDateFormatted");
            Driver.SetGridFilterRange("AddDateFormatted", "01.01.2013 00:00", "31.12.2013 00:00");
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            VerifyAreEqual("Текст отзыва 258", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 249", Driver.GetGridCell(9, "Text").Text);
            VerifyAreEqual("01.11.2013 14:22", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyAreEqual("23.10.2013 14:22", Driver.GetGridCell(9, "AddDateFormatted").Text);
            VerifyAreEqual("Найдено записей: 210",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "AddDateFormatted");
            VerifyAreEqual("Найдено записей: 300",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Order(1)]
        [Test]
        public void FilterAddDateMinMaxNotExist()
        {
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "AddDateFormatted");
            //check min not exist
            Driver.SetGridFilterRange("AddDateFormatted", "27.09.2016 10:00", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check max not exist
            Driver.SetGridFilterRange("AddDateFormatted", "", "27.09.2018 10:00"); 
            VerifyAreEqual("Найдено записей: 300",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //check min and max not exist
            Driver.SetGridFilterRange("AddDateFormatted", "11.09.2018 13:48", "31.01.2019 13:48"); 
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Order(1)]
        [Test]
        public void FilterAddDateMinMaxInvalidSymbols()
        {
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "AddDateFormatted");
            //check min invalid symbols
            Driver.SetGridFilterRange("AddDateFormatted", "########@@@@@@@@&&&&&&&******", ""); 
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Text);

            //check max invalid symbols
            Driver.SetGridFilterRange("AddDateFormatted", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Text);

            //check min and max invalid symbols
            Driver.SetGridFilterRange("AddDateFormatted", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Text);
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Text);
        }

        [Order(1)]
        [Test]
        public void FilterAddDateMinMaxTooMuchSymbols()
        {
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "AddDateFormatted");
            //check min too much symbols
            Driver.SetGridFilterRange("AddDateFormatted", "111111111111111111111111111111111111111111111111111111111111111", "");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Text);

            //check max too much symbols
            Driver.SetGridFilterRange("AddDateFormatted", "", "111111111111111111111111111111111111111111111111111111111111111");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Text);

            //check min and max too much symbols
            Driver.SetGridFilterRange("AddDateFormatted", "111111111111111111111111111111111111111111111111111111111111111", "111111111111111111111111111111111111111111111111111111111111111");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Text);
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Text);
        }

        [Order(10)]
        [Test]
        public void FilterAddDateDelete()
        {
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "AddDateFormatted");
            Driver.SetGridFilterRange("AddDateFormatted", "01.01.2013 00:00", "31.12.2013 00:00");
            VerifyAreEqual("Найдено записей: 210",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Functions.GridFilterClose(Driver, BaseUrl, name: "AddDateFormatted");

            VerifyAreEqual("Найдено записей: 90",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            GoToAdmin("reviews");

            VerifyAreEqual("Найдено записей: 90",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }
    }
}