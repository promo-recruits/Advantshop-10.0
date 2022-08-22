using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.EditCategory
{
    [TestFixture]
    public class CategoryEditAdditional : BaseSeleniumTest
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
        public void BrandInCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(
                    By.CssSelector(
                        "[data-e2e=\"categoriesBlockItemEdit\"][data-e2e-categories-block-item-edit-id=\"3\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));
            Driver.FindElements(By.CssSelector(".block-additional-parameters-value .adv-radio-label input"))[0].Click();

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToClient("catalog");

            VerifyIsTrue(Driver.PageSource.Contains("Производители"));
            VerifyIsTrue(Driver.PageSource.Contains("BrandName3"));
        }

        [Test]
        public void BrandInCategoryNo()
        {
            GoToAdmin("catalog");
            Driver.FindElement(
                    By.CssSelector(
                        "[data-e2e=\"categoriesBlockItemEdit\"][data-e2e-categories-block-item-edit-id=\"3\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));
            Driver.FindElements(By.CssSelector(".block-additional-parameters-value .adv-radio-label input"))[1].Click();

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToClient("catalog");

            VerifyIsFalse(Driver.PageSource.Contains("Производители"));
            VerifyIsFalse(Driver.PageSource.Contains("BrandName3"));
        }

        [Test]
        public void TwoLevelInCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(
                    By.CssSelector(
                        "[data-e2e=\"categoriesBlockItemEdit\"][data-e2e-categories-block-item-edit-id=\"3\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));
            Driver.FindElements(By.CssSelector(".block-additional-parameters-value .adv-radio-label input"))[2].Click();

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToClient("catalog");

            VerifyIsTrue(Driver.PageSource.Contains("TestCategory4"));
            VerifyIsTrue(Driver.PageSource.Contains("TestCategory6"));
        }

        [Test]
        public void TwoLevelInCategoryNo()
        {
            GoToAdmin("catalog");
            Driver.FindElement(
                    By.CssSelector(
                        "[data-e2e=\"categoriesBlockItemEdit\"][data-e2e-categories-block-item-edit-id=\"3\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));

            Driver.FindElements(By.CssSelector(".block-additional-parameters-value .adv-radio-label input"))[3].Click();

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToClient("catalog");

            VerifyIsTrue(Driver.PageSource.Contains("TestCategory4"));
            VerifyIsFalse(Driver.PageSource.Contains("TestCategory6"));
        }

        [Test]
        public void HiddenCategoryInmenu()
        {
            GoToAdmin("catalog");
            Driver.FindElement(
                    By.CssSelector(
                        "[data-e2e=\"categoriesBlockItemEdit\"][data-e2e-categories-block-item-edit-id=\"3\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));
            Driver.FindElements(By.CssSelector(".block-additional-parameters-value .adv-radio-label input"))[4].Click();
            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToClient("catalog");
            VerifyIsFalse(Driver.PageSource.Contains("TestCategory3"));
        }

        [Test]
        public void HiddenCategoryInmenuNo()
        {
            GoToAdmin("catalog");
            Driver.FindElement(
                    By.CssSelector(
                        "[data-e2e=\"categoriesBlockItemEdit\"][data-e2e-categories-block-item-edit-id=\"3\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));

            Driver.FindElements(By.CssSelector(".block-additional-parameters-value .adv-radio-label input"))[5].Click();

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToClient("catalog");
            VerifyIsTrue(Driver.PageSource.Contains("TestCategory3"));
        }

        [Test]
        public void DisplayStyleListCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(
                    By.CssSelector(
                        "[data-e2e=\"categoriesBlockItemEdit\"][data-e2e-categories-block-item-edit-id=\"3\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");

            (new SelectElement(Driver.FindElement(By.Id("DisplayStyle")))).SelectByText("Список");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToClient("categories/newcategory");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-categories-thin")).Count > 0);
        }

        [Test]
        public void DisplayStyleTileCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(
                    By.CssSelector(
                        "[data-e2e=\"categoriesBlockItemEdit\"][data-e2e-categories-block-item-edit-id=\"3\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");

            (new SelectElement(Driver.FindElement(By.Id("DisplayStyle")))).SelectByText("Плитка");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToClient("categories/newcategory");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-categories-item-slim")).Count > 0);
        }

        [Test]
        public void DisplayStyleNoPreCategory()
        {
            GoToAdmin("catalog");
            Driver.FindElement(
                    By.CssSelector(
                        "[data-e2e=\"categoriesBlockItemEdit\"][data-e2e-categories-block-item-edit-id=\"3\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));

            Driver.FindElement(By.Id("UrlPath")).Clear();
            Driver.FindElement(By.Id("UrlPath")).SendKeys("newcategory");

            (new SelectElement(Driver.FindElement(By.Id("DisplayStyle")))).SelectByText("Не показывать");
            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToClient("categories/newcategory");
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".product-categories-item-slim")).Count);
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".product-categories-thin")).Count);
        }
    }
}