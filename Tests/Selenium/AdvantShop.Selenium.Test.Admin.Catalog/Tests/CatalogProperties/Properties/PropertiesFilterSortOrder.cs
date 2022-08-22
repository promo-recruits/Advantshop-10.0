using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class PropertiesFilterSortOrder : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Properties\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Properties\\Catalog.Category.csv",
                "Data\\Admin\\Properties\\Catalog.Brand.csv",
                "Data\\Admin\\Properties\\Catalog.Property.csv",
                "Data\\Admin\\Properties\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Properties\\Catalog.Product.csv",
                "Data\\Admin\\Properties\\Catalog.Offer.csv",
                "Data\\Admin\\Properties\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\Properties\\Catalog.ProductCategories.csv"
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
        public void BySortOrder()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "50");

            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property19", Driver.GetGridCell(9, "Name").Text);
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "SortOrder");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);

            Driver.GridPaginationSelectItems("50");
            Driver.ScrollToTop();
            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "50");

            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property50", Driver.GetGridCell(40, "Name").Text);
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "SortOrder");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);

            Driver.GridPaginationSelectItems("10");    
        }

        [Test]
        public void BySortOrderPage()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "50");

            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property19", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Property20", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property29", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Property30", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property39", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("Property40", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property49", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Property50", Driver.GetGridCell(0, "Name").Text);
            Driver.ScrollToTop();
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "SortOrder");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void BySortOrderPageToBegin()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "50");

            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property19", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property20", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property29", Driver.GetGridCell(9, "Name").Text);

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property19", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollToTop();
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "SortOrder");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void BySortOrderPageToEnd()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "50");

            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property19", Driver.GetGridCell(9, "Name").Text);

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Property50", Driver.GetGridCell(0, "Name").Text);
            Driver.ScrollToTop();
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "SortOrder");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void BySortOrderPageToNext()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "50");

            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property19", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property20", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property29", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollToTop();
            //Close
            Functions.GridFilterClose(Driver, BaseUrl, "SortOrder");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void BySortOrderPageToPrevious()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Functions.GridFilterSet(Driver, BaseUrl, "SortOrder");
            Driver.SetGridFilterRange("SortOrder", "10", "50");

            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property19", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property20", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property29", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property30", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property39", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Property20", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property29", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Property10", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property19", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollToTop();
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "SortOrder");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }
    }
}