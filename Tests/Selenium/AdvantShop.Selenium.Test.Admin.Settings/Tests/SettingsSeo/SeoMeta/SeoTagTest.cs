using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSeo.SeoMeta
{
    [TestFixture]
    public class SettingsSeoTagTest : BaseSeleniumTest
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
        public void DefaultSeoTag()
        {
            GoToAdmin("tags/edit/3");
            Driver.ScrollTo(By.Id("URL"));
            if (!Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"tagDefaultMeta\"]")).Click();
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SaveTag\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("CategoriesDefaultH1"));

            Driver.FindElement(By.Id("TagsDefaultTitle")).Click();
            Driver.FindElement(By.Id("TagsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("TagsDefaultTitle")).SendKeys("New title Tag");

            Driver.FindElement(By.Id("TagsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("TagsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("TagsDefaultMetaKeywords"))
                .SendKeys("New meta keywords 1 Tag, New meta keywords 2 Tag, New meta keywords 3 Tag");

            Driver.FindElement(By.Id("TagsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("TagsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("TagsDefaultMetaDescription")).SendKeys("New description Tag");

            Driver.FindElement(By.Id("TagsDefaultH1")).Click();
            Driver.FindElement(By.Id("TagsDefaultH1")).Clear();
            Driver.FindElement(By.Id("TagsDefaultH1")).SendKeys("New h1 Tag");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("New title Tag", Driver.FindElement(By.Id("TagsDefaultTitle")).GetAttribute("value"),
                "default seo product title admin");
            VerifyAreEqual("New meta keywords 1 Tag, New meta keywords 2 Tag, New meta keywords 3 Tag",
                Driver.FindElement(By.Id("TagsDefaultMetaKeywords")).GetAttribute("value"),
                "default seo tag keywords admin");
            VerifyAreEqual("New description Tag",
                Driver.FindElement(By.Id("TagsDefaultMetaDescription")).GetAttribute("value"),
                "default seo tag description admin");
            VerifyAreEqual("New h1 Tag", Driver.FindElement(By.Id("TagsDefaultH1")).GetAttribute("value"),
                "default seo tag h1 admin");

            //check client
            GoToClient("categories/test-category-2/tag/tag-name1");
            VerifyAreEqual("New title Tag", Driver.Title, "default seo tag title client");
            VerifyAreEqual("New meta keywords 1 Tag, New meta keywords 2 Tag, New meta keywords 3 Tag",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo tag keywords client");
            VerifyAreEqual("New description Tag",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo tag description client");
            VerifyAreEqual("New h1 Tag", Driver.FindElement(By.TagName("h1")).Text, "default seo tag h1 client");
        }

        [Test]
        public void DefaultSeoTagVariables()
        {
            //set default meta
            GoToAdmin("tags/edit/1");
            Driver.ScrollTo(By.Id("URL"));
            if (!Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"tagDefaultMeta\"]")).Click();
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"SaveTag\"]")).Click();
                Driver.WaitForToastSuccess();
            }

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("CategoriesDefaultH1"));

            Driver.FindElement(By.Id("TagsDefaultTitle")).Click();
            Driver.FindElement(By.Id("TagsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("TagsDefaultTitle")).SendKeys("#STORE_NAME# - #TAG_NAME# - #CATEGORY_NAME##PAGE#");

            Driver.FindElement(By.Id("TagsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("TagsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("TagsDefaultMetaKeywords"))
                .SendKeys("#STORE_NAME# - #TAG_NAME# - #CATEGORY_NAME##PAGE#");

            Driver.FindElement(By.Id("TagsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("TagsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("TagsDefaultMetaDescription"))
                .SendKeys("#STORE_NAME# - #TAG_NAME# - #CATEGORY_NAME##PAGE#");

            Driver.FindElement(By.Id("TagsDefaultH1")).Click();
            Driver.FindElement(By.Id("TagsDefaultH1")).Clear();
            Driver.FindElement(By.Id("TagsDefaultH1")).SendKeys("#STORE_NAME# - #TAG_NAME# - #CATEGORY_NAME##PAGE#");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("#STORE_NAME# - #TAG_NAME# - #CATEGORY_NAME##PAGE#",
                Driver.FindElement(By.Id("TagsDefaultTitle")).GetAttribute("value"), "default seo tag title admin");
            VerifyAreEqual("#STORE_NAME# - #TAG_NAME# - #CATEGORY_NAME##PAGE#",
                Driver.FindElement(By.Id("TagsDefaultMetaKeywords")).GetAttribute("value"),
                "default seo tag keywords admin");
            VerifyAreEqual("#STORE_NAME# - #TAG_NAME# - #CATEGORY_NAME##PAGE#",
                Driver.FindElement(By.Id("TagsDefaultMetaDescription")).GetAttribute("value"),
                "default seo tag description admin");
            VerifyAreEqual("#STORE_NAME# - #TAG_NAME# - #CATEGORY_NAME##PAGE#",
                Driver.FindElement(By.Id("TagsDefaultH1")).GetAttribute("value"), "default seo tag h1 admin");

            //check client
            GoToClient("categories/test-category-2/tag/tag-name1");
            VerifyAreEqual("Мой магазин - TagName1 - TestCategory2", Driver.Title, "default seo tag title client");
            VerifyAreEqual("Мой магазин - TagName1 - TestCategory2",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo tag keywords client");
            VerifyAreEqual("Мой магазин - TagName1 - TestCategory2",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo tag description client");
            VerifyAreEqual("Мой магазин - TagName1 - TestCategory2", Driver.FindElement(By.TagName("h1")).Text,
                "default seo tag h1 client");

            GoToClient("categories/test-category-2/tag/tag-name1?page=2");
            VerifyAreEqual("Мой магазин - TagName1 - TestCategory2, страница № 2", Driver.Title,
                "default seo tag title client page 2");
            VerifyAreEqual("Мой магазин - TagName1 - TestCategory2, страница № 2",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo tag keywords client page 2");
            VerifyAreEqual("Мой магазин - TagName1 - TestCategory2, страница № 2",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo tag description client page 2");
            VerifyAreEqual("Мой магазин - TagName1 - TestCategory2, страница № 2",
                Driver.FindElement(By.TagName("h1")).Text, "default seo tag h1 client page 2");
        }

        [Test]
        public void DefaultSeoTagReset()
        {
            //admin set meta for tag
            GoToAdmin("tags/edit/3");
            Driver.ScrollTo(By.Id("URL"));
            if (Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"tagDefaultMeta\"]")).Click();
            }

            Driver.WaitForElem(By.Id("SeoTitle"));
            Driver.FindElement(By.Id("SeoTitle")).Clear();
            Driver.FindElement(By.Id("SeoTitle")).SendKeys("Tag_Title");
            Driver.FindElement(By.Id("SeoKeywords")).Clear();
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("Tag_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).Clear();
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("Tag_SeoDescription");
            Driver.FindElement(By.Id("SeoH1")).Clear();
            Driver.FindElement(By.Id("SeoH1")).SendKeys("Tag_H1");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SaveTag\"]")).Click();
            Driver.WaitForToastSuccess();

            //pre check client
            GoToClient("categories/test-category-2/tag/tag-name3");
            VerifyAreEqual("Tag_Title", Driver.Title, "pre check seo tag title client");
            VerifyAreEqual("Tag_H1", Driver.FindElement(By.TagName("h1")).Text, "pre check seo tag h1 client");
            VerifyAreEqual("Tag_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "pre check seo tag keywords client");
            VerifyAreEqual("Tag_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "pre check seo tag description client");

            //reset meta
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("CategoriesDefaultMetaDescription"));

            Driver.FindElement(By.Id("TagsDefaultTitle")).Click();
            Driver.FindElement(By.Id("TagsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("TagsDefaultTitle")).SendKeys("1");

            Driver.FindElement(By.Id("TagsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("TagsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("TagsDefaultMetaKeywords")).SendKeys("2");

            Driver.FindElement(By.Id("TagsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("TagsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("TagsDefaultMetaDescription")).SendKeys("3");

            Driver.FindElement(By.Id("TagsDefaultH1")).Click();
            Driver.FindElement(By.Id("TagsDefaultH1")).Clear();
            Driver.FindElement(By.Id("TagsDefaultH1")).SendKeys("4");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("TagsDefaultMetaDescription"));

            Driver.FindElement(By.LinkText("Сбросить мета информацию для всех тегов")).Click();
            Driver.SwalConfirm();

            //check client
            GoToClient("categories/test-category-2/tag/tag-name3");
            VerifyAreEqual("1", Driver.Title, "reset seo tag title client");
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "reset seo tag keywords client");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "reset seo tag description client");
            VerifyAreEqual("4", Driver.FindElement(By.TagName("h1")).Text, "reset seo tag h1 client");
        }
    }
}