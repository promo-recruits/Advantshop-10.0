using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsTemplate
{
    [TestFixture]
    public class SettingsTemplateBrandsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsTemplate\\Brands\\Catalog.Color.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Brands\\Catalog.Size.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Brands\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Brands\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Brands\\Catalog.Brand.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Brands\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Brands\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsTemplate\\Brands\\Catalog.Offer.csv"
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
        public void BrandsPerPage()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=brands");

            Driver.FindElement(By.Id("BrandsPerPage")).Click();
            Driver.FindElement(By.Id("BrandsPerPage")).Clear();
            Driver.FindElement(By.Id("BrandsPerPage")).SendKeys("3");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=brands");
            VerifyAreEqual("3", Driver.FindElement(By.Id("BrandsPerPage")).GetAttribute("value"),
                "brands per page value admin");

            //check client
            GoToClient("manufacturers");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".brand-name")).Count == 3, "brands per page value client");
        }

        [Test]
        public void ShowProductsInBrandOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=brands");
            Driver.CheckSelected("ShowProductsInBrand", "SettingsTemplateSave");

            GoToClient("manufacturers/advanced-micro-devices-amd");

            VerifyIsTrue(Driver.PageSource.Contains("Список товаров бренда AMD"), "show products in brand h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-name-link")).Displayed,
                "show products in brand");
        }

        [Test]
        public void ShowProductsInBrandOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=brands");
            Driver.CheckNotSelected("ShowProductsInBrand", "SettingsTemplateSave");

            GoToClient("manufacturers/advanced-micro-devices-amd");

            VerifyIsFalse(Driver.PageSource.Contains("Список товаров бренда AMD"), "show products in brand h1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector(".products-view-name-link")).Count > 0,
                "show products in brand");
        }

        [Test]
        public void ShowCategoryTreeInBrandOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=brands");
            Driver.CheckSelected("ShowCategoryTreeInBrand", "SettingsTemplateSave");

            GoToClient("manufacturers/apple");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".menu-dropdown.menu-dropdown-accordion.menu-dropdown-expanded"))
                    .Displayed, "show category tree in brand");
        }

        [Test]
        public void ShowCategoryTreeInBrandOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=brands");
            Driver.CheckNotSelected("ShowCategoryTreeInBrand", "SettingsTemplateSave");

            GoToClient("manufacturers/apple");

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".menu-dropdown.menu-dropdown-accordion.menu-dropdown-expanded"))
                    .Count > 0, "show category tree in brand");
        }
    }
}