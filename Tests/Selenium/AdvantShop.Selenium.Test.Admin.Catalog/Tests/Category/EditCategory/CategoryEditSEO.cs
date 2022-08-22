using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.EditCategory
{
    [TestFixture]
    public class CategoryEditSEO : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\EditCategory\\Catalog.Category.csv",
                "Data\\Admin\\EditCategory\\Catalog.Brand.csv",
                "Data\\Admin\\EditCategory\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\EditCategory\\Catalog.Property.csv",
                "Data\\Admin\\EditCategory\\Catalog.PropertyValue.csv",
                "Data\\Admin\\EditCategory\\Catalog.Product.csv",
                "Data\\Admin\\EditCategory\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\EditCategory\\Catalog.Offer.csv",
                "Data\\Admin\\EditCategory\\Catalog.ProductCategories.csv"
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
        public void ChangeCheckSEOnew()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            var element = Driver.FindElements(By.TagName("figure"))[2];
            IJavaScriptExecutor jse = (IJavaScriptExecutor) Driver;
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected);

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("new");

            Driver.FindElement(By.Id("SeoTitle")).SendKeys("New_Category_Title");
            Driver.FindElement(By.Id("SeoH1")).SendKeys("New_Category_H1");
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("New_Category_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("New_Category_SeoDescription");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("New_Category_Title", Driver.FindElement(By.Id("SeoTitle")).GetAttribute("value"));
            VerifyAreEqual("New_Category_SeoDescription",
                Driver.FindElement(By.Id("SeoDescription")).GetAttribute("value"));
            VerifyAreEqual("New_Category_H1", Driver.FindElement(By.Id("SeoH1")).GetAttribute("value"));
            VerifyAreEqual("New_Category_SeoKeywords", Driver.FindElement(By.Id("SeoKeywords")).GetAttribute("value"));

            GoToClient("categories/new");
            VerifyAreEqual("New_Category_Title", Driver.Title);
            VerifyAreEqual("New_Category_H1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("New_Category_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"));
            VerifyAreEqual("New_Category_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"));
        }

        [Test]
        public void ChangeCheckzMetaold()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("new");
            var element = Driver.FindElements(By.TagName("figure"))[2];
            IJavaScriptExecutor jse = (IJavaScriptExecutor) Driver;
            jse.ExecuteScript("arguments[0].scrollIntoView(true)", element);

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected);
            VerifyIsTrue(Driver.FindElements(By.Id("SeoTitle")).Count == 0);
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("categories/new");
            VerifyAreEqual("Мой магазин - TestCategory1", Driver.Title);
            VerifyAreEqual("TestCategory1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("Мой магазин - TestCategory1",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"));
            VerifyAreEqual("Мой магазин - TestCategory1",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"));
        }
    }
}