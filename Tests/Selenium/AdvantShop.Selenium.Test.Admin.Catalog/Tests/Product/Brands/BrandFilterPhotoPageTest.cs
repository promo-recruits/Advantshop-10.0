using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Brands
{
    [TestFixture]
    public class BrandFilterPhotoPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\Brands\\BrandFilters\\Catalog.Photo.csv",
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
        public void BrandFilterPhotoPageBrand()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "Без фотографии");
            VerifyAreEqual("BrandName71", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName80", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("BrandName81", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName90", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("BrandName131", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName140", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("BrandName141", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName150", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("BrandName151", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName160", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterPhotoPageBrandToBegin()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "Без фотографии");
            VerifyAreEqual("BrandName71", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName80", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName81", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName90", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName131", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName140", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName141", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName150", Driver.GetGridCell(9, "BrandName").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("BrandName71", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName80", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterPhotoPageBrandToEnd()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "Без фотографии");
            VerifyAreEqual("BrandName71", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName80", Driver.GetGridCell(9, "BrandName").Text);

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("BrandName191", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName200", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterPhotoPageBrandToNext()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "Без фотографии");
            VerifyAreEqual("BrandName71", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName80", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName81", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName90", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName131", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName140", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName141", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName150", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void BrandFilterPhotoPageBrandToPrevious()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "Без фотографии");
            VerifyAreEqual("BrandName71", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName80", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName81", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName90", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName91", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName100", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("BrandName81", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName90", Driver.GetGridCell(9, "BrandName").Text);


            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("BrandName71", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName80", Driver.GetGridCell(9, "BrandName").Text);
        }
    }
}