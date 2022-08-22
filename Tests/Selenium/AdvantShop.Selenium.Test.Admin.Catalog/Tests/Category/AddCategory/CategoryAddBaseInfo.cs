using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.AddCategory
{
    [TestFixture]
    public class CategoryAddBaseInfo : BaseSeleniumTest
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
        public void AddCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            VerifyAreEqual("Категория \"Новая категория\"", Driver.FindElement(By.TagName("h1")).Text);
            //имя
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category");
            //описание
            Driver.SetCkText("New_Category_Description_here", "BriefDescription");
            Driver.SetCkText("New_Category_Brief_Description_here", "Description");
            Thread.Sleep(1000);
            //урл
            Driver.ScrollTo(By.Name("DefaultMeta"));
            Driver.FindElement(By.Id("UrlPath")).Click();
            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");
            Driver.DropFocus("h1");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //в админке 
            GoToAdmin("catalog");
            VerifyAreEqual("5",
                Driver.FindElement(By.CssSelector(
                        "[data-e2e-select=\"CategoryTop\"] [data-e2e-select=\"CategoryTopRightAll\"] [data-e2e-quantity=\"CategoryAllQuantity\"]"))
                    .Text);
            VerifyIsTrue(Driver.PageSource.Contains("New_Category"));
            //в клиентке
            GoToClient("catalog");
            VerifyIsTrue(Driver.PageSource.Contains("New_Category"));
            Driver.FindElement(By.CssSelector(".product-categories-header-slim")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.PageSource.Contains("New_Category_Description_here"));
            VerifyIsTrue(Driver.Url.Contains("categories/newcategory"));
            VerifyIsTrue(Driver.PageSource.Contains("New_Category_Brief_Description_here"));

            VerifyAreEqual("Мой магазин - New_Category", Driver.Title);
            VerifyAreEqual("New_Category", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("Мой магазин - New_Category",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"));
            VerifyAreEqual("Мой магазин - New_Category",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"));
        }

        [Test]
        public void CategoryCheckEnabled()
        {
            GoToAdmin("category/add?parentId=0");
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Disabled");
            Driver.DropFocus("h1");
            Driver.FindElement(By.CssSelector(".adv-checkbox-emul")).Click();
            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory_disabled");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
            GoToClient("catalog");
            VerifyIsFalse(Driver.PageSource.Contains("New_Category_Disabled"));
            VerifyIsTrue(Is404Page("categories/newcategory_disabled"));
        }

        [Test]
        public void SaveParent()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Save_Parent1");
            Driver.FindElement(By.ClassName("edit")).Click();
            Driver.FindElement(By.Id("2")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategorysvp");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog");
            VerifyIsFalse(Driver.PageSource.Contains("New_Category_Save_Parent1"));
            Driver.FindElement(
                By.CssSelector("[data-e2e=\"categoriesBlockItem\"][data-e2e-categories-block-item-id=\"2\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.PageSource.Contains("New_Category_Save_Parent1"));
            GoToClient("catalog");
            Driver.FindElements(By.CssSelector(".product-categories-header-slim"))[2].Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.PageSource.Contains("New_Category_Save_Parent1"));
        }
    }
}