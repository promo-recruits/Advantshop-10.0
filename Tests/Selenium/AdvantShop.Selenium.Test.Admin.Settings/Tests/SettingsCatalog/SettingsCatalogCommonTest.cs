using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCatalog
{
    [TestFixture]
    public class SettingsCatalogCommonProductsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Color.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Size.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Brand.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Property.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductExt.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Tag.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductPropertyValue.csv"
            );
            Init();
            ReindexSearch();
            ReCalc();
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
        public void ShowQuickViewOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckSelected("ShowQuickView", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");

            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElements(By.CssSelector(".products-view-picture-link"))[0]);
            a.Perform();

            Driver.FindElement(By.LinkText("Быстрый просмотр")).Click();
            Thread.Sleep(2000);
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyIsTrue(Driver.FindElement(By.Name("form")).Enabled, "quick show pop up");
        }

        [Test]
        public void ShowQuickViewOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckNotSelected("ShowQuickView", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");

            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElements(By.CssSelector(".products-view-picture-link"))[0]);
            a.Perform();

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".products-view-picture-link"))[0].Text.Contains("Быстрый просмотр"),
                "quick show pop up");
        }

        [Test]
        public void ProductsPerPage()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.FindElement(By.Id("ProductsPerPage")).Click();
            Driver.FindElement(By.Id("ProductsPerPage")).Clear();
            Driver.FindElement(By.Id("ProductsPerPage")).SendKeys("2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            VerifyAreEqual("2", Driver.FindElement(By.Id("ProductsPerPage")).GetAttribute("value"),
                "products per page admin value");

            //check client
            GoToClient("categories/apple-phones");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block")).Count == 2,
                "products per page client count");
        }

        [Test]
        public void ShowProductsCountOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckSelected("ShowProductsCount", "SettingsTemplateSave");

            GoToClient();

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".menu-dropdown-link-wrap.cs-bg-i-7.icon-right-open-after-abs"))[1]
                    .Text.Contains("Техника (185)"), "products count in menu");

            GoToClient("categories/tech");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".product-categories-item-slim"))[1].Text
                    .Contains("Игровые приставки (36)"), "products count in category");
        }

        [Test]
        public void ShowProductsCountOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckNotSelected("ShowProductsCount", "SettingsTemplateSave");

            GoToClient();

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".menu-dropdown-link-wrap.cs-bg-i-7.icon-right-open-after-abs"))[1]
                    .Text.Contains("Техника (181)"), "products count in menu");

            GoToClient("categories/tech");

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".product-categories-item-slim"))[1].Text
                    .Contains("Игровые приставки (36)"), "products count in category");
        }

        [Test]
        public void DisplayCategoriesInBottomMenuOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckSelected("DisplayCategoriesInBottomMenu", "SettingsTemplateSave");

            GoToClient();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".site-footer")).Text.Contains("Категории"),
                "display categories in bottom menu");
        }

        [Test]
        public void DisplayCategoriesInBottomMenuOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckNotSelected("DisplayCategoriesInBottomMenu", "SettingsTemplateSave");

            GoToClient();

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".site-footer")).Text.Contains("Категории"),
                "display categories in bottom menu");
        }

        [Test]
        public void ShowProductArtNoOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckSelected("ShowProductArtNo", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0].Text
                    .Contains("966"), "show product art number in catalog");
        }

        [Test]
        public void ShowProductArtNoOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckNotSelected("ShowProductArtNo", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0].Text
                    .Contains("app939_4s"), "show product art number in catalog");
        }

        [Test]
        public void EnableProductRatingOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckSelected("EnableProductRating", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".rating")).Enabled, "enable product rating");
        }

        [Test]
        public void EnableProductRatingOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckNotSelected("EnableProductRating", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElements(By.CssSelector(".rating")).Count > 0, "enable product rating");
        }

        [Test]
        public void EnableCompareProductsOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckSelected("EnableCompareProducts", "SettingsTemplateSave");

            GoToClient();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".toolbar-bottom")).Text.Contains("Сравнение товаров"),
                "enable compare products");
        }

        [Test]
        public void EnableCompareProductsOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckNotSelected("EnableCompareProducts", "SettingsTemplateSave");

            GoToClient();

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".toolbar-bottom")).Text.Contains("Сравнение товаров"),
                "enable compare products");
        }

        [Test]
        public void EnablePhotoPreviewsOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Functions.CheckSelected("EnablePhotoPreviews", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
            }

            GoToClient("categories/apple-phones");
            Driver.FindElement(By.CssSelector("[title=\"Плитка\"]")).Click();
            Thread.Sleep(1000);
            /*
            Actions a = new Actions(driver);
            a.Build();
            a.MoveToElement(driver.FindElements(By.CssSelector(".products-view-picture-link"))[0]);
            a.Perform();*/
            Driver.MouseFocus(By.CssSelector(".products-view-picture-link"));
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElements(By.CssSelector(".products-view-photos-item"))[0].Enabled, "enable photo previews");
        }

        [Test]
        public void EnablePhotoPreviewsOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckNotSelected("EnablePhotoPreviews", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");
            Driver.FindElement(By.CssSelector("[title=\"Плитка\"]")).Click();
            Driver.MouseFocus(By.CssSelector(".products-view-picture-link"));
            /* Actions a = new Actions(driver);
             a.Build();
             a.MoveToElement(driver.FindElements(By.CssSelector(".products-view-picture-link"))[0]);
             a.Perform();*/

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElements(By.CssSelector(".products-view-photos-item")).Count > 0, "enable photo previews");
        }

        [Test]
        public void ShowCountPhotoOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckSelected("ShowCountPhoto", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");

            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElements(By.CssSelector(".products-view-picture-link"))[0]);
            a.Perform();

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".products-view-photos-count.cs-bg-1.cs-t-8")).Text.Contains("3"),
                "show count photo");
        }

        [Test]
        public void ShowCountPhotoOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckNotSelected("ShowCountPhoto", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");

            Actions a = new Actions(Driver);
            a.Build();
            a.MoveToElement(Driver.FindElements(By.CssSelector(".products-view-picture-link"))[0]);
            a.Perform();

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElements(By.CssSelector(".products-view-photos-count.cs-bg-1.cs-t-8")).Count > 0,
                "show count photo");
        }


        [Test]
        public void ShowOnlyAvailableOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckSelected("ShowOnlyAvalible", "SettingsTemplateSave");

            GoToClient("categories/igrushki");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text.Contains("7"),
                "show only available products");
        }

        [Test]
        public void ShowOnlyAvailableOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckNotSelected("ShowOnlyAvalible", "SettingsTemplateSave");

            GoToClient("categories/igrushki");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text.Contains("8"),
                "show only available products");
        }

        [Test]
        public void MoveNotAvailableToEndOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckSelected("MoveNotAvaliableToEnd", "SettingsTemplateSave");

            GoToClient("categories/igrushki");

            var lastElem = Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))
                .Count;
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[lastElem - 1]
                    .Text.Contains("Бабочка"), "move not available products to end");
        }

        [Test]
        public void MoveNotAvailableToEndOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            Driver.CheckNotSelected("MoveNotAvaliableToEnd", "SettingsTemplateSave");

            GoToClient("categories/igrushki");

            var lastElem = Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))
                .Count;
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[lastElem - 1]
                    .Text.Contains("Lego - Creator Highway Speedster (от 7 до 12 лет)"),
                "move not available products to end");
        }
    }

    [TestFixture]
    public class SettingsCatalogCommonFiltersTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Color.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Size.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Brand.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Property.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductExt.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Tag.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductPropertyValue.csv"
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
        public void FilterVisibilityOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckSelected("FilterVisibility", "SettingsTemplateSave");

            GoToClient("categories/igrushki");

            VerifyIsTrue(Driver.FindElements(By.Name("catalogFilterForm")).Count > 0, "filter visibility");
        }

        [Test]
        public void FilterVisibilityOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckNotSelected("FilterVisibility", "SettingsTemplateSave");

            GoToClient("categories/igrushki");

            VerifyIsFalse(Driver.FindElements(By.Name("catalogFilterForm")).Count > 0, "filter visibility");
        }

        [Test]
        public void ShowPriceFilterOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckSelected("ShowPriceFilter", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена"), "show price filter");
        }

        [Test]
        public void ShowPriceFilterOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckNotSelected("ShowPriceFilter", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsFalse(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цена"), "show price filter");
        }

        [Test]
        public void ShowProducerFilterOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckSelected("ShowProducerFilter", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Производители"),
                "show producer filter");
        }

        [Test]
        public void ShowProducerFilterOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckNotSelected("ShowProducerFilter", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsFalse(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Производители"),
                "show producer filter");
        }

        [Test]
        public void ShowSizeFilterOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckSelected("ShowSizeFilter", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Размер"), "show size filter");
        }

        [Test]
        public void ShowSizeFilterOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckNotSelected("ShowSizeFilter", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsFalse(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Размер"), "show size filter");
        }

        [Test]
        public void ShowColorFilterOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckSelected("ShowColorFilter", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цвет"), "show color filter");
        }

        [Test]
        public void ShowColorFilterOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckNotSelected("ShowColorFilter", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsFalse(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цвет"), "show color filter");
        }

        [Test]
        public void ExcludingFiltersOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckSelected("ShowProducerFilter", "SettingsTemplateSave");
            Driver.CheckSelected("ExcludingFilters", "SettingsTemplateSave");

            GoToClient("categories/platia");

            var textColorWithoutFilter = Driver.FindElement(By.Name("catalogFilterForm"))
                .FindElement(By.XPath("//span[contains(text(), 'Armani')]")).GetCssValue("color");

            GoToClient("categories/platia?pricefrom=2500&priceto=4600");

            var textColorWithFilter = Driver.FindElement(By.Name("catalogFilterForm"))
                .FindElement(By.XPath("//span[contains(text(), 'Armani')]")).GetCssValue("color");

            VerifyIsFalse(textColorWithoutFilter.Equals(textColorWithFilter), "excluding filters");
        }

        [Test]
        public void ExcludingFiltersOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowOnlyAvalible"));

            Driver.CheckSelected("ShowProducerFilter", "SettingsTemplateSave");
            Driver.CheckNotSelected("ExcludingFilters", "SettingsTemplateSave");

            GoToClient("categories/platia");

            var textColorWithoutFilter = Driver.FindElement(By.Name("catalogFilterForm"))
                .FindElement(By.XPath("//span[contains(text(), 'Armani')]")).GetCssValue("color");

            GoToClient("categories/platia?pricefrom=2500&priceto=4600");

            var textColorWithFilter = Driver.FindElement(By.Name("catalogFilterForm"))
                .FindElement(By.XPath("//span[contains(text(), 'Armani')]")).GetCssValue("color");

            VerifyIsTrue(textColorWithoutFilter.Equals(textColorWithFilter), "excluding filters");
        }
    }

    [TestFixture]
    public class SettingsCatalogCommonSizeColorTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Color.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Size.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Brand.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Property.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductExt.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Tag.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductPropertyValue.csv"
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
        public void SizesHeader()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowColorFilter"));

            Driver.FindElement(By.Id("SizesHeader")).Click();
            Driver.FindElement(By.Id("SizesHeader")).Clear();
            Driver.FindElement(By.Id("SizesHeader")).SendKeys("SizesHeader Test");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            VerifyAreEqual("SizesHeader Test", Driver.FindElement(By.Id("SizesHeader")).GetAttribute("value"),
                "size header admin value");

            //check client
            GoToClient("categories/platia");

            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("SizesHeader Test"),
                "size header client");
            VerifyIsFalse(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Размер"),
                "no previous size header client");
        }

        [Test]
        public void ColorsHeader()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowColorFilter"));

            Driver.FindElement(By.Id("ColorsHeader")).Click();
            Driver.FindElement(By.Id("ColorsHeader")).Clear();
            Driver.FindElement(By.Id("ColorsHeader")).SendKeys("ColorsHeader Test");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            VerifyAreEqual("ColorsHeader Test", Driver.FindElement(By.Id("ColorsHeader")).GetAttribute("value"),
                "color header admin value");

            //check client
            GoToClient("categories/platia");

            VerifyIsTrue(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("ColorsHeader Test"),
                "color header client");
            VerifyIsFalse(Driver.FindElement(By.Name("catalogFilterForm")).Text.Contains("Цвет"),
                "no previous color header client");
        }

        [Test]
        public void ColorIconWidthCatalog()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowColorFilter"));

            Driver.FindElement(By.Id("ColorIconWidthCatalog")).Click();
            Driver.FindElement(By.Id("ColorIconWidthCatalog")).Clear();
            Driver.FindElement(By.Id("ColorIconWidthCatalog")).SendKeys("50");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            VerifyAreEqual("50", Driver.FindElement(By.Id("ColorIconWidthCatalog")).GetAttribute("value"),
                "color icon catalog admin value");

            //check client
            GoToClient("categories/platia");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".color-viewer-inner.cs-br-1")).GetCssValue("width").Contains("50px"),
                "color icon catalog width client");
            VerifyIsTrue(
                Driver.FindElement(By.Name("catalogFilterForm"))
                    .FindElements(By.CssSelector(".color-viewer-inner.cs-br-1"))[0].GetCssValue("width")
                    .Contains("50px"), "color icon catalog width filter client");
            GoToClient("products/dress1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".details-row.details-colors"))
                    .FindElement(By.CssSelector(".color-viewer-inner.cs-br-1")).GetCssValue("width").Contains("50px"),
                "color icon product cart width client");
        }

        [Test]
        public void ColorIconWidthDetails()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowColorFilter"));

            Driver.FindElement(By.Id("ColorIconWidthDetails")).Click();
            Driver.FindElement(By.Id("ColorIconWidthDetails")).Clear();
            Driver.FindElement(By.Id("ColorIconWidthDetails")).SendKeys("35");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            VerifyAreEqual("35", Driver.FindElement(By.Id("ColorIconWidthDetails")).GetAttribute("value"),
                "color icon catalog admin value");

            //check client
            GoToClient("categories/platia");
            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".color-viewer-inner.cs-br-1")).GetCssValue("width").Contains("35px"),
                "color icon catalog width client");
            VerifyIsFalse(
                Driver.FindElement(By.Name("catalogFilterForm"))
                    .FindElements(By.CssSelector(".color-viewer-inner.cs-br-1"))[0].GetCssValue("width")
                    .Contains("35px"), "color icon catalog width filter client");
            GoToClient("products/dress1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-row.details-colors"))
                    .FindElement(By.CssSelector(".color-viewer-inner.cs-br-1")).GetCssValue("width").Contains("35px"),
                "color icon product cart width client");
        }

        [Test]
        public void ColorIconHeightCatalog()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowColorFilter"));

            Driver.FindElement(By.Id("ColorIconHeightCatalog")).Click();
            Driver.FindElement(By.Id("ColorIconHeightCatalog")).Clear();
            Driver.FindElement(By.Id("ColorIconHeightCatalog")).SendKeys("40");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            VerifyAreEqual("40", Driver.FindElement(By.Id("ColorIconHeightCatalog")).GetAttribute("value"),
                "color icon catalog admin value");

            //check client
            GoToClient("categories/platia");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".color-viewer-inner.cs-br-1")).GetCssValue("height").Contains("40px"),
                "color icon catalog height client");
            VerifyIsTrue(
                Driver.FindElement(By.Name("catalogFilterForm"))
                    .FindElements(By.CssSelector(".color-viewer-inner.cs-br-1"))[0].GetCssValue("height")
                    .Contains("40px"), "color icon catalog height filter client");
            GoToClient("products/dress1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".details-row.details-colors"))
                    .FindElement(By.CssSelector(".color-viewer-inner.cs-br-1")).GetCssValue("height").Contains("40px"),
                "color icon product cart height client");
        }

        [Test]
        public void ColorIconHeightDetails()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ShowColorFilter"));

            Driver.FindElement(By.Id("ColorIconHeightDetails")).Click();
            Driver.FindElement(By.Id("ColorIconHeightDetails")).Clear();
            Driver.FindElement(By.Id("ColorIconHeightDetails")).SendKeys("20");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            VerifyAreEqual("20", Driver.FindElement(By.Id("ColorIconHeightDetails")).GetAttribute("value"),
                "color icon catalog admin value");

            //check client
            GoToClient("categories/platia");
            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".color-viewer-inner.cs-br-1")).GetCssValue("height").Contains("20px"),
                "color icon catalog height client");
            VerifyIsFalse(
                Driver.FindElement(By.Name("catalogFilterForm"))
                    .FindElements(By.CssSelector(".color-viewer-inner.cs-br-1"))[0].GetCssValue("height")
                    .Contains("20px"), "color icon catalog height filter client");
            GoToClient("products/dress1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-row.details-colors"))
                    .FindElement(By.CssSelector(".color-viewer-inner.cs-br-1")).GetCssValue("height").Contains("20px"),
                "color icon product cart height client");
        }

        [Test]
        public void ComplexFilterOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("SizesHeader"));

            Driver.CheckSelected("ComplexFilter", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".color-viewer-inner.cs-br-1")).Enabled, "complex filter");
        }

        [Test]
        public void ComplexFilterOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("SizesHeader"));

            Driver.CheckNotSelected("ComplexFilter", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElements(By.CssSelector(".color-viewer-inner.cs-br-1")).Count > 0, "complex filter");
        }
    }

    [TestFixture]
    public class SettingsCatalogCommonProductButtonsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Color.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Size.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Brand.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Property.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductExt.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Tag.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductPropertyValue.csv"
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
        public void BuyButtonText()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ColorIconHeightDetails"));
            Driver.CheckSelected("DisplayBuyButton", "SettingsTemplateSave");

            Driver.ScrollTo(By.Id("ColorIconHeightDetails"));
            Driver.FindElement(By.Id("BuyButtonText")).Click();
            Driver.FindElement(By.Id("BuyButtonText")).Clear();
            Driver.FindElement(By.Id("BuyButtonText")).SendKeys("BuyButtonText Test");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            VerifyAreEqual("BuyButtonText Test", Driver.FindElement(By.Id("BuyButtonText")).GetAttribute("value"),
                "buy button text admin value");

            //check client
            GoToClient("categories/platia");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".products-view-buttons")).Text.Contains("BuyButtonText Test"),
                "buy button text catalog client");
            GoToClient("products/dress1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Text
                    .Contains("BuyButtonText Test"), "buy button text product cart client");
        }

        [Test]
        public void PreOrderButtonText()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ColorIconHeightDetails"));
            Driver.CheckSelected("DisplayPreOrderButton", "SettingsTemplateSave");

            Driver.ScrollTo(By.Id("ColorIconHeightDetails"));
            Driver.FindElement(By.Id("PreOrderButtonText")).Click();
            Driver.FindElement(By.Id("PreOrderButtonText")).Clear();
            Driver.FindElement(By.Id("PreOrderButtonText")).SendKeys("PreOrderButtonText Test");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            VerifyAreEqual("PreOrderButtonText Test",
                Driver.FindElement(By.Id("PreOrderButtonText")).GetAttribute("value"),
                "preorder button text admin value");

            //check client
            GoToClient("categories/apple-phones");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".products-view-buttons")).Text.Contains("PreOrderButtonText Test"),
                "preorder button text catalog client");
            GoToClient("products/apple_iphone_4s_64gb_chernyi");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-payment-block")).Text.Contains("PreOrderButtonText Test"),
                "preorder button text product cart client");
        }

        [Test]
        public void DisplayBuyButtonOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ColorIconHeightDetails"));

            Driver.CheckSelected("DisplayBuyButton", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".products-view-buttons")).Enabled, "display buy button catalog");
        }

        [Test]
        public void DisplayBuyButtonOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ColorIconHeightDetails"));

            Driver.CheckNotSelected("DisplayBuyButton", "SettingsTemplateSave");

            GoToClient("categories/platia");

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElements(By.CssSelector(".products-view-buttons")).Count > 0, "display buy button");
        }

        [Test]
        public void DisplayPreOrderButtonOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ColorIconHeightDetails"));

            Driver.CheckSelected("DisplayPreOrderButton", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElement(By.CssSelector(".products-view-buttons")).Enabled, "display preorder button");
        }

        [Test]
        public void DisplayPreOrderButtonOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("ColorIconHeightDetails"));

            Driver.CheckNotSelected("DisplayPreOrderButton", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");

            VerifyIsFalse(
                Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                    .FindElements(By.CssSelector(".products-view-buttons")).Count > 0, "display preorder button");
        }
    }

    [TestFixture]
    public class SettingsCatalogCommonProductsViewTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Color.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Size.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Brand.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Property.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductExt.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.RelatedProducts.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.Tag.csv",
                "Data\\Admin\\Settings\\Catalog\\Common\\Catalog.ProductPropertyValue.csv"
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
        public void EnableCatalogViewChangeOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("BuyButtonText"));

            Driver.CheckSelected("EnableCatalogViewChange", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-variants")).Displayed,
                "enable catalog view");
        }

        [Test]
        public void EnableCatalogViewChangeOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("BuyButtonText"));

            Driver.CheckNotSelected("EnableCatalogViewChange", "SettingsTemplateSave");

            GoToClient("categories/apple-phones");

            VerifyIsFalse(Driver.FindElements(By.CssSelector(".products-view-variants")).Count > 0,
                "enable catalog view");
        }

        [Test]
        public void EnableSearchViewChangeOn()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("BuyButtonText"));

            Driver.CheckSelected("EnableSearchViewChange", "SettingsTemplateSave");

            GoToClient("search?q=apple");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-variants")).Displayed, "enable search view");
        }

        [Test]
        public void EnableSearchViewChangeOff()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("BuyButtonText"));

            Driver.CheckNotSelected("EnableSearchViewChange", "SettingsTemplateSave");

            GoToClient("search?q=apple");

            VerifyIsFalse(Driver.FindElements(By.CssSelector(".products-view-variants")).Count > 0,
                "enable search view");
        }

        [Test]
        public void DefaultCatalogView()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("BuyButtonText"));

            (new SelectElement(Driver.FindElement(By.Id("DefaultCatalogView")))).SelectByText("Таблица");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            IWebElement selectCatalogView = Driver.FindElement(By.Id("DefaultCatalogView"));
            SelectElement select = new SelectElement(selectCatalogView);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Таблица"), "default catalog view admin");

            //check client
            GoToClient("categories/platia");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".row.products-view.products-view-table")).Displayed,
                "default catalog view client");
        }

        [Test]
        public void DefaultSearchView()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");
            Driver.ScrollTo(By.Id("BuyButtonText"));

            (new SelectElement(Driver.FindElement(By.Id("DefaultSearchView")))).SelectByText("Список");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingstemplate#?settingsTemplateTab=catalog");

            IWebElement selectCatalogView = Driver.FindElement(By.Id("DefaultSearchView"));
            SelectElement select = new SelectElement(selectCatalogView);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Список"), "default search view admin");

            //check clients
            GoToClient("search?q=apple");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".products-view.products-view-container.products-view-list"))
                    .Displayed, "default search view client");
        }
    }
}