using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Brands
{
    [TestFixture]
    public class BrandFilterSortPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.ProductCategories.csv"
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
        public void BrandFilterSortPage()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");
            Functions.FilterPageFromTo(Driver, BaseUrl, "[data-e2e=\"BrandSettingTitle\"]");

            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("BrandName21", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName30", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("BrandName31", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName40", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("BrandName41", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName50", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("BrandName51", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName60", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterSortPageToBegin()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");
            Functions.FilterPageFromTo(Driver, BaseUrl, "[data-e2e=\"BrandSettingTitle\"]");

            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName21", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName30", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName31", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName40", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName41", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName50", Driver.GetGridCell(9, "BrandName").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterSortPageToEnd()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");
            Functions.FilterPageFromTo(Driver, BaseUrl, "[data-e2e=\"BrandSettingTitle\"]");

            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterSortPageToNext()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");
            Functions.FilterPageFromTo(Driver, BaseUrl, "[data-e2e=\"BrandSettingTitle\"]");

            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName21", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName30", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName31", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName40", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName41", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName50", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterSortPageToPrevious()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");
            Functions.FilterPageFromTo(Driver, BaseUrl, "[data-e2e=\"BrandSettingTitle\"]");

            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName21", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName30", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);
        }
    }
}