using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditMainBrandAddPageViewSort : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Tag.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Property.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Color.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.PropertyValue.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductPropertyValue.csv"
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
        public void BrandAddView()
        {
            GoToAdmin("product/edit/1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBrand\"]")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName12", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName94", Driver.GetGridCell(99, "BrandName", "Brands").Text);

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void BrandAddSort()
        {
            GoToAdmin("product/edit/1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBrand\"]")).Click();
            Driver.WaitForModal();

            //check sort by products count
            Driver.GetGridCell(-1, "ProductsCount", "Brands").Click();
            VerifyAreEqual("BrandName31", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName40", Driver.GetGridCell(9, "BrandName", "Brands").Text);
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount", "Brands").Text);
            VerifyAreEqual("0", Driver.GetGridCell(9, "ProductsCount", "Brands").Text);

            Driver.GetGridCell(-1, "ProductsCount", "Brands").Click();
            VerifyAreEqual("BrandName13", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName10", Driver.GetGridCell(9, "BrandName", "Brands").Text);
            VerifyAreEqual("8", Driver.GetGridCell(0, "ProductsCount", "Brands").Text);
            VerifyAreEqual("1", Driver.GetGridCell(9, "ProductsCount", "Brands").Text);

            //check sort by brand name
            Driver.GetGridCell(-1, "BrandName", "Brands").Click();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName12", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            Driver.GetGridCell(-1, "BrandName", "Brands").Click();
            VerifyAreEqual("BrandName99", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName90", Driver.GetGridCell(9, "BrandName", "Brands").Text);


            Driver.GetGridCell(-1, "BrandName", "Brands").Click();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName12", Driver.GetGridCell(9, "BrandName", "Brands").Text);
        }


        [Test]
        public void BrandAddPage()
        {
            GoToAdmin("product/edit/1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBrand\"]")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName12", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("BrandName13", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName21", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("BrandName22", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName30", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("BrandName31", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName4", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("BrandName40", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName49", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName12", Driver.GetGridCell(9, "BrandName", "Brands").Text);
        }

        [Test]
        public void BrandAddPageToPrevious()
        {
            GoToAdmin("product/edit/1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddBrand\"]")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName12", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName13", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName21", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("BrandName22", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName30", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("BrandName13", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName21", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("BrandName1", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName12", Driver.GetGridCell(9, "BrandName", "Brands").Text);

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("BrandName95", Driver.GetGridCell(0, "BrandName", "Brands").Text);
            VerifyAreEqual("BrandName99", Driver.GetGridCell(4, "BrandName", "Brands").Text);
        }
    }
}