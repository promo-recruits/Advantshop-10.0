using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSeo.SeoMeta
{
    [TestFixture]
    public class SettingsSeoProductTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.SettingsProductsPerPage | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Brand.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Tag.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.PropertyGroup.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Property.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Catalog.Color.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Size.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Product.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Category.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.ProductPropertyValue.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.TagMap.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Settings.Settings.csv"
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
        public void DefaultSeoProduct()
        {
            GoToAdmin("product/edit/1");
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.WaitForElemDisplayedAndClick(By.CssSelector("[data-e2e=\"productDefaultMeta\"] span"));
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("TagsDefaultH1"));

            Driver.FindElement(By.Id("ProductsDefaultTitle")).Click();
            Driver.FindElement(By.Id("ProductsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultTitle")).SendKeys("New title Product");

            Driver.FindElement(By.Id("ProductsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("ProductsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultMetaKeywords"))
                .SendKeys("New meta keywords 1 Product, New meta keywords 2 Product, New meta keywords 3 Product");

            Driver.FindElement(By.Id("ProductsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("ProductsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultMetaDescription")).SendKeys("New description Product");

            Driver.FindElement(By.Id("ProductsDefaultH1")).Click();
            Driver.FindElement(By.Id("ProductsDefaultH1")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultH1")).SendKeys("New h1 Product");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("New title Product", Driver.FindElement(By.Id("ProductsDefaultTitle")).GetAttribute("value"),
                "default seo product title admin");
            VerifyAreEqual("New meta keywords 1 Product, New meta keywords 2 Product, New meta keywords 3 Product",
                Driver.FindElement(By.Id("ProductsDefaultMetaKeywords")).GetAttribute("value"),
                "default seo product keywords admin");
            VerifyAreEqual("New description Product",
                Driver.FindElement(By.Id("ProductsDefaultMetaDescription")).GetAttribute("value"),
                "default seo product description admin");
            VerifyAreEqual("New h1 Product", Driver.FindElement(By.Id("ProductsDefaultH1")).GetAttribute("value"),
                "default seo product h1 admin");

            //check client
            GoToClient("products/test-product-1");
            VerifyAreEqual("New title Product", Driver.Title, "default seo product title client");
            VerifyAreEqual("New meta keywords 1 Product, New meta keywords 2 Product, New meta keywords 3 Product",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo product keywords client");
            VerifyAreEqual("New description Product",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo product description client");
            VerifyAreEqual("New h1 Product", Driver.FindElement(By.TagName("h1")).Text,
                "default seo product h1 client");
        }

        [Test]
        public void DefaultSeoProductVariables()
        {
            //set default meta
            GoToAdmin("product/edit/16");
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.WaitForElemDisplayedAndClick(By.CssSelector("[data-e2e=\"productDefaultMeta\"] span"));
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("TagsDefaultH1"));

            Driver.FindElement(By.Id("ProductsDefaultTitle")).Click();
            Driver.FindElement(By.Id("ProductsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultTitle"))
                .SendKeys("#STORE_NAME# - #PRODUCT_NAME# - #CATEGORY_NAME# - бренд #BRAND_NAME# - #PRICE# - #TAGS#");

            Driver.FindElement(By.Id("ProductsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("ProductsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultMetaKeywords")).SendKeys(
                "#STORE_NAME# - #PRODUCT_NAME# - #CATEGORY_NAME# - бренд #BRAND_NAME# - #PRICE# - #TAGS#");

            Driver.FindElement(By.Id("ProductsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("ProductsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultMetaDescription")).SendKeys(
                "#STORE_NAME# - #PRODUCT_NAME# - #CATEGORY_NAME# - бренд #BRAND_NAME# - #PRICE# - #TAGS#");

            Driver.FindElement(By.Id("ProductsDefaultH1")).Click();
            Driver.FindElement(By.Id("ProductsDefaultH1")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultH1"))
                .SendKeys("#STORE_NAME# - #PRODUCT_NAME# - #CATEGORY_NAME# - бренд #BRAND_NAME# - #PRICE# - #TAGS#");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("#STORE_NAME# - #PRODUCT_NAME# - #CATEGORY_NAME# - бренд #BRAND_NAME# - #PRICE# - #TAGS#",
                Driver.FindElement(By.Id("ProductsDefaultTitle")).GetAttribute("value"),
                "default seo product title admin");
            VerifyAreEqual("#STORE_NAME# - #PRODUCT_NAME# - #CATEGORY_NAME# - бренд #BRAND_NAME# - #PRICE# - #TAGS#",
                Driver.FindElement(By.Id("ProductsDefaultMetaKeywords")).GetAttribute("value"),
                "default seo product keywords admin");
            VerifyAreEqual("#STORE_NAME# - #PRODUCT_NAME# - #CATEGORY_NAME# - бренд #BRAND_NAME# - #PRICE# - #TAGS#",
                Driver.FindElement(By.Id("ProductsDefaultMetaDescription")).GetAttribute("value"),
                "default seo product description admin");
            VerifyAreEqual("#STORE_NAME# - #PRODUCT_NAME# - #CATEGORY_NAME# - бренд #BRAND_NAME# - #PRICE# - #TAGS#",
                Driver.FindElement(By.Id("ProductsDefaultH1")).GetAttribute("value"), "default seo product h1 admin");

            //check client
            GoToClient("products/test-product-16");
            VerifyAreEqual("Мой магазин - TestProduct16 - TestCategory2 - бренд BrandName6 - 10 руб. - TagName1",
                Driver.Title, "default seo product title client");
            VerifyAreEqual("Мой магазин - TestProduct16 - TestCategory2 - бренд BrandName6 - 10 руб. - TagName1",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo product keywords client");
            VerifyAreEqual("Мой магазин - TestProduct16 - TestCategory2 - бренд BrandName6 - 10 руб. - TagName1",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo product description client");
            VerifyAreEqual("Мой магазин - TestProduct16 - TestCategory2 - бренд BrandName6 - 10 руб. - TagName1",
                Driver.FindElement(By.TagName("h1")).Text, "default seo product h1 client");
        }

        [Test]
        public void DefaultSeoProductReset()
        {
            //admin set meta for product
            GoToAdmin("product/edit/5");
            Driver.WaitForElemDisplayedAndClick(By.XPath("//div[contains(text(), 'SEO')]"));
            Driver.WaitForElemDisplayedAndClick(By.CssSelector("[data-e2e=\"productDefaultMeta\"] span"));
            Driver.WaitForElemDisplayedAndClick(By.XPath("//div[contains(text(), 'SEO')]"));
            Driver.WaitForElemDisplayedAndClick(By.Id("SeoTitle"));
            Driver.FindElement(By.Id("SeoTitle")).Clear();
            Driver.FindElement(By.Id("SeoTitle")).SendKeys("Product_Title");
            Driver.FindElement(By.Id("SeoKeywords")).Clear();
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("Product_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).Clear();
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("Product_SeoDescription");
            Driver.FindElement(By.Id("SeoH1")).Clear();
            Driver.FindElement(By.Id("SeoH1")).SendKeys("Product_H1");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //pre check client
            GoToClient("products/test-product-5");
            VerifyAreEqual("Product_Title", Driver.Title, "pre check seo product title client");
            VerifyAreEqual("Product_H1", Driver.FindElement(By.TagName("h1")).Text, "pre check seo product h1 client");
            VerifyAreEqual("Product_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "pre check seo product keywords client");
            VerifyAreEqual("Product_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "pre check seo product description client");

            //reset meta
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("TagsDefaultMetaDescription"));

            Driver.FindElement(By.Id("ProductsDefaultTitle")).Click();
            Driver.FindElement(By.Id("ProductsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultTitle")).SendKeys("1");

            Driver.FindElement(By.Id("ProductsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("ProductsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultMetaKeywords")).SendKeys("2");

            Driver.FindElement(By.Id("ProductsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("ProductsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultMetaDescription")).SendKeys("3");

            Driver.FindElement(By.Id("ProductsDefaultH1")).Click();
            Driver.FindElement(By.Id("ProductsDefaultH1")).Clear();
            Driver.FindElement(By.Id("ProductsDefaultH1")).SendKeys("4");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("ProductsDefaultH1"));

            Driver.FindElement(By.LinkText("Сбросить мета информацию для всех товаров")).Click();
            Driver.SwalConfirm();

            //check client
            GoToClient("products/test-product-5");
            VerifyAreEqual("1", Driver.Title, "reset seo product title client");
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "reset seo product keywords client");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "reset seo product description client");
            VerifyAreEqual("4", Driver.FindElement(By.TagName("h1")).Text, "reset seo product h1 client");
        }
    }
}