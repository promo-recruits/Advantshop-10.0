using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Brands
{
    [TestFixture]
    public class BrandPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Brands\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\Brands\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Brands\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Brands\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Brands\\Catalog.ProductCategories.csv"
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
        public void PageBrand()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4)")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5)")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName21", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName30", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6)")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName31", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName40", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7)")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName41", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName50", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7)")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName51", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName60", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void PageBrandToBegin()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName21", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName30", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName31", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName40", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName41", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName50", Driver.GetGridCell(9, "BrandName").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void PageBrandToEnd()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName101", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName105", Driver.GetGridCell(4, "BrandName").Text);
        }

        [Test]
        public void PageBrandToNext()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName21", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName30", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName31", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName40", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName41", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName50", Driver.GetGridCell(9, "BrandName").Text);
        }

        [Test]
        public void PageBrandToPrevious()
        {
            GoToAdmin("settingscatalog#?catalogTab=brand");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-next")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName21", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName30", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-prev")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName11", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName20", Driver.GetGridCell(9, "BrandName").Text);

            Driver.FindElement(By.CssSelector(".pagination-prev")).FindElement(By.TagName("a")).Click();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName").Text);
        }
    }
}