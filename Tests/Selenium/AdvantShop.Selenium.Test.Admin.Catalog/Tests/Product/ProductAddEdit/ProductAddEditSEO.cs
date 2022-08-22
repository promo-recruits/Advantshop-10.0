using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditSEO : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Tag.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.PropertyGroup.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Property.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Color.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Size.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductPropertyValue.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.ProductCategories.csv"
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
        public void ProductEditCheckMeta()
        {
            GoToAdmin("product/edit/1");
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Thread.Sleep(1000);

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.FindElement(By.Id("SeoTitle")).SendKeys("New_Product_Title");
            Driver.FindElement(By.Id("SeoH1")).SendKeys("New_Product_H1");
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("New_Product_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("New_Product_SeoDescription");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/1");
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Thread.Sleep(1000);

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected);
            VerifyAreEqual("New_Product_Title", Driver.FindElement(By.Id("SeoTitle")).GetAttribute("value"));
            VerifyAreEqual("New_Product_H1", Driver.FindElement(By.Id("SeoH1")).GetAttribute("value"));
            VerifyAreEqual("New_Product_SeoKeywords", Driver.FindElement(By.Id("SeoKeywords")).GetAttribute("value"));
            VerifyAreEqual("New_Product_SeoDescription",
                Driver.FindElement(By.Id("SeoDescription")).GetAttribute("value"));

            //check client
            GoToClient("products/test-product1");
            VerifyAreEqual("New_Product_Title", Driver.Title);
            VerifyAreEqual("New_Product_H1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("New_Product_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"));
            VerifyAreEqual("New_Product_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"));
        }

        [Test]
        public void ProductEditCheckMetaParams()
        {
            GoToAdmin("product/edit/1");
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Thread.Sleep(1000);

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.FindElement(By.Id("SeoTitle")).Clear();
            Driver.FindElement(By.Id("SeoTitle")).SendKeys("#STORE_NAME#");
            Driver.FindElement(By.Id("SeoH1")).Clear();
            Driver.FindElement(By.Id("SeoH1")).SendKeys("#PRODUCT_NAME#");
            Driver.FindElement(By.Id("SeoKeywords")).Clear();
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("#CATEGORY_NAME#");
            Driver.FindElement(By.Id("SeoDescription")).Clear();
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("#CATEGORY_NAME# #PRODUCT_NAME#");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/1");
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Thread.Sleep(1000);

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected);
            VerifyAreEqual("#STORE_NAME#", Driver.FindElement(By.Id("SeoTitle")).GetAttribute("value"));
            VerifyAreEqual("#PRODUCT_NAME#", Driver.FindElement(By.Id("SeoH1")).GetAttribute("value"));
            VerifyAreEqual("#CATEGORY_NAME#", Driver.FindElement(By.Id("SeoKeywords")).GetAttribute("value"));
            VerifyAreEqual("#CATEGORY_NAME# #PRODUCT_NAME#",
                Driver.FindElement(By.Id("SeoDescription")).GetAttribute("value"));

            //check client
            GoToClient("products/test-product1");
            VerifyAreEqual("Мой магазин",
                Driver.FindElement(By.CssSelector("[property=\"og:title\"]")).GetAttribute("content"));
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("TestCategory1",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"));
            VerifyAreEqual("TestCategory1 TestProduct1",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"));
        }

        [Test]
        public void ProductEditMetaURL()
        {
            GoToAdmin("product/edit/1");
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Thread.Sleep(1000);

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("new_path");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/1");
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected);
            VerifyAreEqual("new_path", Driver.FindElement(By.Id("UrlPath")).GetAttribute("value"));

            VerifyIsTrue(Is404Page("products/test-product1"));

            GoToClient("products/new_path");
            VerifyAreEqual("Мой магазин - TestProduct1", Driver.Title);
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("Мой магазин - TestProduct1",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"));
            VerifyAreEqual("Мой магазин - TestProduct1",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"));

            GoToAdmin("product/edit/1");
            Driver.ScrollTo(By.Id("UrlPath"));
            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("test-product1");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
        }

        [Test]
        public void ProductEditMetazInstruction()
        {
            GoToAdmin("product/edit/2");
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Thread.Sleep(1000);

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"productDefaultMeta\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            Thread.Sleep(1000);

            Driver.ScrollTo(By.Id("SeoDescription"));

            Driver.XPathContainsText("a", "Инструкция. Настройка мета по умолчанию для магазина");
            Thread.Sleep(1000);

            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.Url.Contains("help") && Driver.Url.Contains("seo-module"));
            Functions.CloseTab(Driver, BaseUrl);
        }
    }
}