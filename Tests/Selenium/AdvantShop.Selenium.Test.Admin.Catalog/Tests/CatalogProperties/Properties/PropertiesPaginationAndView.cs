using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class PropertiesPaginationAndView : BaseSeleniumTest
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
        public void Present10Page()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Property11", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property20", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Property21", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property30", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void Present10PageToNext()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property11", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property20", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property21", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property30", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void Present10PageToPrevious()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property11", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property20", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void Present10PageToEnd()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Property101", Driver.GetGridCell(0, "Name").Text);
        }

        [Test]
        public void Present10PageToBegin()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property11", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property20", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property21", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property30", Driver.GetGridCell(9, "Name").Text);

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }
    }
}