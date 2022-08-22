using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.EditCategory
{
    [TestFixture]
    public class CategoryEditBaseInfo : BaseSeleniumTest
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
        public void ChangeNameCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"categoriesBlockItemEdit\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("btnSave"));
            VerifyAreEqual("Категория \"TestCategory1\"", Driver.FindElement(By.TagName("h1")).Text);
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category_Save_Name");
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
            //Проверка
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("New_Category_Save_Name"));

            GoToAdmin("catalog");
            VerifyAreEqual("New_Category_Save_Name",
                Driver.FindElement(
                        By.CssSelector("[data-e2e=\"categoriesBlockItem\"][data-e2e-categories-block-item-id=\"1\"]"))
                    .Text);
            GoToClient();
            VerifyAreEqual("New_Category_Save_Name",
                Driver.FindElement(By.CssSelector(".menu-dropdown-link-text.text-floating")).Text);
        }

        [Test]
        public void ChangerURLCategory()
        {
            GoToAdmin("category/edit/2");
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("New_Category");
            Driver.DropFocus("h1");
            Driver.FindElement(By.Name("UrlPath")).Clear();
            Driver.FindElement(By.Name("UrlPath")).SendKeys("newurl");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
            Refresh();
            String Actualtext = Driver.FindElement(By.CssSelector("[data-e2e=\"brandLinkLook\"]")).GetAttribute("href");
            VerifyIsTrue(Actualtext.Contains("/categories/newurl"));
            GoToClient();
            Driver.FindElement(By.LinkText("New_Category")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains("/categories/newurl"));
        }

        [Test]
        public void ChangeDescription()
        {
            GoToAdmin("category/edit/3");

            Driver.SetCkText("New_Description_here", "BriefDescription");

            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("ChangeDescription");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
            GoToClient();
            Driver.FindElement(By.LinkText("ChangeDescription")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.PageSource.Contains("New_Description_here"));
        }

        [Test]
        public void ChangeBriefDescription()
        {
            GoToAdmin("category/edit/7");
            Driver.SetCkText("New_Brief_Description_here", "Description");

            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("ChangeBriefDescription");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
            GoToClient();
            Driver.FindElement(By.LinkText("ChangeBriefDescription")).Click();
            Thread.Sleep(1000);
            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.PageSource.Contains("New_Brief_Description_here"));
        }

        [Test]
        public void ChangeEnabledCategory()
        {
            GoToAdmin("category/edit/8");
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("ChangeEnabledCategory");
            Driver.FindElement(By.CssSelector(".adv-checkbox-emul")).Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("catalog");
            VerifyIsFalse(Driver.PageSource.Contains("ChangeEnabledCategory"));

            GoToAdmin("category/edit/8");
            Driver.FindElement(By.CssSelector(".adv-checkbox-emul")).Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToClient("catalog");
            VerifyIsTrue(Driver.PageSource.Contains("ChangeEnabledCategory"));
        }

        [Test]
        public void ChangeSaveParent()
        {
            GoToAdmin("category/edit/9");
            Driver.FindElement(By.ClassName("edit")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.Id("11")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("catalog");
            VerifyAreEqual(0,
                Driver.FindElements(
                        By.CssSelector("[data-e2e=\"categoriesBlockItem\"][data-e2e-categories-block-item-id=\"9\"]"))
                    .Count);
            Driver.FindElement(
                By.CssSelector("[data-e2e=\"categoriesBlockItem\"][data-e2e-categories-block-item-id=\"11\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"categoriesBlockItem\"][data-e2e-categories-block-item-id=\"9\"]"))
                .Displayed);
            GoToClient();
            Driver.FindElement(By.LinkText("TestCategory11")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.PageSource.Contains("TestCategory9"));
        }

        [Test]
        public void LinkLook()
        {
            GoToAdmin("category/edit/10");
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("LinkLook");
            Thread.Sleep(1000);
            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newnew");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();
            String Actualtext = Driver.FindElement(By.CssSelector("[data-e2e=\"brandLinkLook\"]")).GetAttribute("href");
            VerifyIsTrue(Actualtext.Contains("/categories/newnew"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"brandLinkLook\"]")).Click();
            Thread.Sleep(1000);

            Functions.OpenNewTab(Driver, BaseUrl);

            VerifyAreEqual("LinkLook", Driver.FindElement(By.TagName("h1")).Text);

            Functions.CloseTab(Driver, BaseUrl);
        }
    }
}