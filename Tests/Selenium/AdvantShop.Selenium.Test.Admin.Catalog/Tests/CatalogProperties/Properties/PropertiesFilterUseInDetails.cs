using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class PropertiesFilterUseInDetails : BaseSeleniumTest
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
        public void ByUseInDetailsYesPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "UseInDetails", filterItem: "Да");

            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "UseInDetails");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);

            Driver.GridPaginationSelectItems("50");
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "UseInDetails", filterItem: "Да");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property55", Driver.GetGridCell(49, "Name").Text);
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "UseInDetails");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property50", Driver.GetGridCell(49, "Name").Text);
        }

        [Test]
        public void ByUseInDetailsNoPresent()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "UseInDetails", filterItem: "Нет");
            VerifyAreEqual("Property11", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property15", Driver.GetGridCell(4, "Name").Text);
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "UseInDetails");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);

            Driver.GridPaginationSelectItems("50");
            Driver.ScrollToTop();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "UseInDetails", filterItem: "Нет");

            VerifyAreEqual("Property11", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property15", Driver.GetGridCell(4, "Name").Text);
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "UseInDetails");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property50", Driver.GetGridCell(49, "Name").Text);
        }

        [Test]
        public void ByUseInDetailsYesPage()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "UseInDetails", filterItem: "Да");

            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Property16", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property25", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Property26", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property35", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollToTop();
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "UseInDetails");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void ByUseInDetailsYesPageToBegin()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "UseInDetails", filterItem: "Да");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property16", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property25", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property26", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property35", Driver.GetGridCell(9, "Name").Text);

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollToTop();
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "UseInDetails");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void ByUseInDetailsYesPageToEnd()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "UseInDetails", filterItem: "Да");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Property96", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property101", Driver.GetGridCell(5, "Name").Text);
            Driver.ScrollToTop();
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "UseInDetails");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void ByUseInDetailsYesPageToNext()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "UseInDetails", filterItem: "Да");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property16", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property25", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollToTop();
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "UseInDetails");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void ByUseInDetailsYesPageToPrevious()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GridReturnDefaultView10(BaseUrl);
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "UseInDetails", filterItem: "Да");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property16", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property25", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Property26", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property35", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Property16", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property25", Driver.GetGridCell(9, "Name").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollToTop();
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "UseInDetails");
            VerifyAreEqual("Property1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property10", Driver.GetGridCell(9, "Name").Text);
        }
    }
}