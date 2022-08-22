using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Brands
{
    [TestFixture]
    public class BrandFilterCountryPageTest : BaseSeleniumTest
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
        public void BrandFilterCountryPage()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Южный Судан");

            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName110", Driver.GetGridCell(9, "BrandName").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("BrandName111", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName120", Driver.GetGridCell(9, "BrandName").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("BrandName121", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName130", Driver.GetGridCell(9, "BrandName").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("BrandName131", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName140", Driver.GetGridCell(9, "BrandName").Text);


            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("BrandName141", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName150", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterCountryPageToBegin()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Южный Судан");

            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName110", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName111", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName120", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName121", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName130", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName131", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName140", Driver.GetGridCell(9, "BrandName").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterCountryPageToEnd()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Южный Судан");

            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("BrandName191", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName193", Driver.GetGridCell(2, "BrandName").Text);
        }

        [Test]
        public void BrandFilterCountryPageToNext()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Южный Судан");

            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName110", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName111", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName120", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName121", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName130", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName131", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName140", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterCountryPageToPrevious()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "CountryName", filterItem: "Южный Судан");

            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName110", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName111", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName120", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName110", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);
        }
    }
}