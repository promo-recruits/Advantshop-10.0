using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Size
{
    [TestFixture]
    public class SizePageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\CatalogSize\\Catalog.ProductCategories.csv"
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
        public void SizePresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            VerifyAreEqual("Размеры", Driver.FindElement(By.CssSelector("[data-e2e=\"SizeSettingTitle\"]")).Text);
            Driver.GridFilterSendKeys("SizeName197");
            VerifyAreEqual("4", Driver.GetGridCell(0, "ProductsCount", "Sizes").Text);

            GoToAdmin("settingscatalog#?catalogTab=sizes");
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName10", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName100", Driver.GetGridCell(99, "SizeName", "Sizes").Text);
            
            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void Page()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            Driver.GridPaginationSelectItems("10");

            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName10", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName11", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName20", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName21", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName30", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName31", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName40", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName41", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName50", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName51", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName60", Driver.GetGridCell(9, "SizeName", "Sizes").Text);
        }

        [Test]
        public void PageToBegin()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName10", Driver.GetGridCell(9, "SizeName", "Sizes").Text);


            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName11", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName20", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName21", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName30", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName31", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName40", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName41", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName50", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            //to begin

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName10", Driver.GetGridCell(9, "SizeName", "Sizes").Text);
        }

        [Test]
        public void PageToEnd()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName10", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            //to end

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName191", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName200", Driver.GetGridCell(9, "SizeName", "Sizes").Text);
        }

        [Test]
        public void PageToNext()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName10", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName11", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName20", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName21", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName30", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName31", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName40", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName41", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName50", Driver.GetGridCell(9, "SizeName", "Sizes").Text);
        }

        [Test]
        public void PageToPrevious()
        {
            GoToAdmin("settingscatalog#?catalogTab=sizes");
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName10", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName11", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName20", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName21", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName30", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName11", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName20", Driver.GetGridCell(9, "SizeName", "Sizes").Text);

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "Sizes").Text);
            VerifyAreEqual("SizeName10", Driver.GetGridCell(9, "SizeName", "Sizes").Text);
        }
    }
}