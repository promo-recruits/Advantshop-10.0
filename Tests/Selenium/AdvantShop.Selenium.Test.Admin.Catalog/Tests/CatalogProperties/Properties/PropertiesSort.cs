using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class PropertiesSort : BaseSeleniumTest
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
        public void OpenProperties()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.FindElement(By.CssSelector(".property-groups .link-invert")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void SortByName()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            Driver.GetGridCell(-1, "GroupName").Click();
            VerifyAreEqual("Property100", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property101", Driver.GetGridCell(1, "Name").Text);
            VerifyAreEqual("Property1", Driver.GetGridCell(2, "Name").Text);
            VerifyAreEqual("Property8", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "GroupName").Click();
            VerifyAreEqual("Property80", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property89", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "GroupName").Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void SortByGroup()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            Driver.GetGridCell(-1, "GroupName").Click();
            VerifyAreEqual("Property100", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property101", Driver.GetGridCell(1, "Name").Text);
            VerifyAreEqual("Group1",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[9][\'GroupName\']\"]")).Text);

            VerifyAreEqual("Property8", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "GroupName").Click();
            VerifyAreEqual("Property80", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property89", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("Group9",
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[1][\'GroupName\']\"]")).Text);

            Driver.GetGridCell(-1, "GroupName").Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void SortByFilter()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            Driver.GetGridCell(-1, "UseInFilter").Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "UseInFilter").Click();
            VerifyAreEqual("Property6", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property15", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "UseInFilter").Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void SortByViewInCart()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            Driver.GetGridCell(-1, "UseInDetails").Click();
            VerifyAreEqual("Property11", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property20", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "UseInDetails").Click();
            VerifyAreEqual("Property16", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property25", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "UseInDetails").Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void SortByBrief()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GetGridCell(-1, "UseInBrief").Click();
            VerifyAreEqual("Property21", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property30", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "UseInBrief").Click();
            VerifyAreEqual("Property26", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property35", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "UseInBrief").Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void SortBySort()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            Driver.GetGridCell(-1, "SortOrder").Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "SortOrder").Click();
            VerifyAreEqual("Property101", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property92", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "SortOrder").Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void SortByUseProduct()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            Driver.GetGridCell(-1, "ProductsCount").Click();
            VerifyAreEqual("Property6", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property15", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "ProductsCount").Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            Driver.GetGridCell(-1, "ProductsCount").Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }
    }
}