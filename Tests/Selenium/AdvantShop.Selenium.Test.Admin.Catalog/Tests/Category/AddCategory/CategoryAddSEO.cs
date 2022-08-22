using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.AddCategory
{
    [TestFixture]
    public class CategoryAddSEO : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\AddCategory\\Catalog.Brand.csv",
                "Data\\Admin\\Catalog\\AddCategory\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\AddCategory\\Catalog.Offer.csv",
                "Data\\Admin\\Catalog\\AddCategory\\Catalog.Category.csv",
                "Data\\Admin\\Catalog\\AddCategory\\Catalog.ProductCategories.csv");


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
        public void AddNewCategoryCheckSEO()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_SEO_Title");
            Driver.DropFocus("h1");
            Driver.ScrollTo(By.Name("DefaultMeta"));
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]")).FindElement(By.TagName("span"))
                    .Click();
            }

            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"categoryDefaultMeta\"]"))
                .FindElement(By.Id("DefaultMeta")).Selected);

            Thread.Sleep(1000);
            Driver.FindElement(By.Id("UrlPath")).Click();
            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("new_seo_title");
            Driver.FindElement(By.Id("SeoTitle")).SendKeys("New_Category_Title");
            Driver.FindElement(By.Id("SeoH1")).SendKeys("New_Category_H1");
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("New_Category_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("New_Category_SeoDescription");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToClient("categories/new_seo_title");
            VerifyAreEqual("New_Category_Title", Driver.Title);
            VerifyAreEqual("New_Category_H1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("New_Category_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"));
            VerifyAreEqual("New_Category_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"));
        }
    }
}