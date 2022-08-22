using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Properties
{
    [TestFixture]
    public class PropertiesInPlace : BaseSeleniumTest
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
        public void EditUseInFilter()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GetGridCellInputForm(0, "UseInFilter").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Click();
            Driver.WaitForToastSuccess();
            VerifyIsTrue(Driver.GetGridCell(0, "UseInFilter")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
        }

        [Test]
        public void EditUseInDetails()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GetGridCellInputForm(0, "UseInDetails")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyIsFalse(Driver.GetGridCell(0, "UseInDetails")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
        }

        [Test]
        public void EditUseInBrief()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GetGridCellInputForm(0, "UseInBrief").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Click();
            Driver.WaitForToastSuccess();
            VerifyIsFalse(Driver.GetGridCell(0, "UseInBrief")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] input")).Selected);
        }

        [Test]
        public void EditSortOrder()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.SendKeysGridCell("10", 0, "SortOrder");
            Refresh();
            VerifyAreEqual("Property2", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("Property1", Driver.GetGridCell(9, "Name").Text);
        }

        [Test]
        public void PropertyValueInplaceEdit()
        {
            GoToAdmin("settingscatalog#?catalogTab=properties");
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".fa-list")).Click();
            Thread.Sleep(1000);
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"PropertyValueSettingTitle\"]"));
            Driver.MouseFocus(
                By.CssSelector("[data-e2e-grid-cell=\"gridPropertyValues[0][\'Value\']\"] .ui-grid-custom-edit-form"));
            Driver.SendKeysGridCell("new_name", 0, "Value", "PropertyValues");
            Driver.SendKeysGridCell("1", 0, "SortOrder", "PropertyValues");
            Refresh();
            VerifyAreEqual("0", Driver.GetGridCell(0, "SortOrder", "PropertyValues").Text);
            VerifyAreEqual("1", Driver.GetGridCell(2, "SortOrder", "PropertyValues").Text);
            VerifyAreEqual("new_name", Driver.GetGridCell(2, "Value", "PropertyValues").Text);
        }
    }
}