using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSeo.SeoMeta
{
    [TestFixture]
    public class SettingsSeoCategoryTest : BaseSeleniumTest
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
        public void DefaultSeoCategory()
        {
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("DefaultMetaDescription"));

            Driver.FindElement(By.Id("CategoriesDefaultTitle")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultTitle")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultTitle")).SendKeys("New title Category");

            Driver.FindElement(By.Id("CategoriesDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultMetaKeywords")).SendKeys(
                "New meta keywords 1 Category, New meta keywords 2 Category, New meta keywords 3 Category");

            Driver.FindElement(By.Id("CategoriesDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultMetaDescription")).SendKeys("New description Category");

            Driver.FindElement(By.Id("CategoriesDefaultH1")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultH1")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultH1")).SendKeys("New h1 Category");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("New title Category",
                Driver.FindElement(By.Id("CategoriesDefaultTitle")).GetAttribute("value"),
                "default seo category title admin");
            VerifyAreEqual("New meta keywords 1 Category, New meta keywords 2 Category, New meta keywords 3 Category",
                Driver.FindElement(By.Id("CategoriesDefaultMetaKeywords")).GetAttribute("value"),
                "default seo category keywords admin");
            VerifyAreEqual("New description Category",
                Driver.FindElement(By.Id("CategoriesDefaultMetaDescription")).GetAttribute("value"),
                "default seo category description admin");
            VerifyAreEqual("New h1 Category", Driver.FindElement(By.Id("CategoriesDefaultH1")).GetAttribute("value"),
                "default seo category h1 admin");

            //check client
            GoToClient("categories/test-category-1");
            VerifyAreEqual("New title Category", Driver.Title, "default seo category title client");
            VerifyAreEqual("New meta keywords 1 Category, New meta keywords 2 Category, New meta keywords 3 Category",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo category keywords client");
            VerifyAreEqual("New description Category",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo category description client");
            VerifyAreEqual("New h1 Category", Driver.FindElement(By.TagName("h1")).Text,
                "default seo category h1 client");
        }

        [Test]
        public void DefaultSeoCategoryVariables()
        {
            //set default meta 
            GoToAdmin("category/edit/2");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"imgAddCategoryBig\"]"));
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]")).FindElement(By.TagName("span"))
                    .Click();
                Driver.ScrollToTop();
                Driver.GetButton(EButtonType.Save).Click();
                Thread.Sleep(2000);
            }

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("DefaultMetaDescription"));

            Driver.FindElement(By.Id("CategoriesDefaultTitle")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultTitle")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultTitle"))
                .SendKeys("#STORE_NAME# - #CATEGORY_NAME##PAGE# - #TAGS#");

            Driver.FindElement(By.Id("CategoriesDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultMetaKeywords"))
                .SendKeys("#STORE_NAME# - #CATEGORY_NAME##PAGE# - #TAGS#");

            Driver.FindElement(By.Id("CategoriesDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultMetaDescription"))
                .SendKeys("#STORE_NAME# - #CATEGORY_NAME##PAGE# - #TAGS#");

            Driver.FindElement(By.Id("CategoriesDefaultH1")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultH1")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultH1")).SendKeys("#STORE_NAME# - #CATEGORY_NAME##PAGE# - #TAGS#");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("#STORE_NAME# - #CATEGORY_NAME##PAGE# - #TAGS#",
                Driver.FindElement(By.Id("CategoriesDefaultTitle")).GetAttribute("value"),
                "default seo category title admin");
            VerifyAreEqual("#STORE_NAME# - #CATEGORY_NAME##PAGE# - #TAGS#",
                Driver.FindElement(By.Id("CategoriesDefaultMetaKeywords")).GetAttribute("value"),
                "default seo category keywords admin");
            VerifyAreEqual("#STORE_NAME# - #CATEGORY_NAME##PAGE# - #TAGS#",
                Driver.FindElement(By.Id("CategoriesDefaultMetaDescription")).GetAttribute("value"),
                "default seo category description admin");
            VerifyAreEqual("#STORE_NAME# - #CATEGORY_NAME##PAGE# - #TAGS#",
                Driver.FindElement(By.Id("CategoriesDefaultH1")).GetAttribute("value"),
                "default seo category h1 admin");

            //check client
            GoToClient("categories/test-category-2");
            VerifyAreEqual("Мой магазин - TestCategory2 - TagName1 TagName3", Driver.Title,
                "default seo category title client page 1");
            VerifyAreEqual("Мой магазин - TestCategory2 - TagName1 TagName3",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo category keywords client page 1");
            VerifyAreEqual("Мой магазин - TestCategory2 - TagName1 TagName3",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo category description client page 1");
            VerifyAreEqual("Мой магазин - TestCategory2 - TagName1 TagName3", Driver.FindElement(By.TagName("h1")).Text,
                "default seo category h1 client page 1");

            GoToClient("categories/test-category-2?page=2");
            VerifyAreEqual("Мой магазин - TestCategory2, страница № 2 - TagName1 TagName3", Driver.Title,
                "default seo category title client page 2");
            VerifyAreEqual("Мой магазин - TestCategory2, страница № 2 - TagName1 TagName3",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo category keywords client page 2");
            VerifyAreEqual("Мой магазин - TestCategory2, страница № 2 - TagName1 TagName3",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo category description client page 2");
            VerifyAreEqual("Мой магазин - TestCategory2, страница № 2 - TagName1 TagName3",
                Driver.FindElement(By.TagName("h1")).Text, "default seo category h1 client page 2");
        }

        [Test]
        public void DefaultSeoCategoryReset()
        {
            //admin set meta for category
            GoToAdmin("category/edit/1");
            Driver.ScrollTo(By.Id("UrlPath"));
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.WaitForElem(By.Id("SeoTitle"));
            Driver.FindElement(By.Id("SeoTitle")).Clear();
            Driver.FindElement(By.Id("SeoTitle")).SendKeys("Category_Title");
            Driver.FindElement(By.Id("SeoKeywords")).Clear();
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("Category_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).Clear();
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("Category_SeoDescription");
            Driver.FindElement(By.Id("SeoH1")).Clear();
            Driver.FindElement(By.Id("SeoH1")).SendKeys("Category_H1");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Thread.Sleep(2000);

            //pre check client
            GoToClient("categories/test-category-1");
            VerifyAreEqual("Category_Title", Driver.Title, "pre check seo category title client");
            VerifyAreEqual("Category_H1", Driver.FindElement(By.TagName("h1")).Text,
                "pre check seo category h1 client");
            VerifyAreEqual("Category_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "pre check seo category keywords client");
            VerifyAreEqual("Category_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "pre check seo category description client");

            //reset meta
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("DefaultMetaDescription"));

            Driver.FindElement(By.Id("CategoriesDefaultTitle")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultTitle")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultTitle")).SendKeys("1");

            Driver.FindElement(By.Id("CategoriesDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultMetaKeywords")).SendKeys("2");

            Driver.FindElement(By.Id("CategoriesDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultMetaDescription")).SendKeys("3");

            Driver.FindElement(By.Id("CategoriesDefaultH1")).Click();
            Driver.FindElement(By.Id("CategoriesDefaultH1")).Clear();
            Driver.FindElement(By.Id("CategoriesDefaultH1")).SendKeys("4");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("CategoriesDefaultTitle"));

            Driver.FindElement(By.LinkText("Сбросить мета информацию для всех категорий")).Click();
            Driver.SwalConfirm();

            //check client
            GoToClient("categories/test-category-2");
            VerifyAreEqual("1", Driver.Title, "reset seo category title client");
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "reset seo category keywords client");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "reset seo category description client");
            VerifyAreEqual("4", Driver.FindElement(By.TagName("h1")).Text, "reset seo category h1 client");
        }
    }
}