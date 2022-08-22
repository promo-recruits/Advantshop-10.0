using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditMainMarkersTagsTest : BaseSeleniumTest
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
                "data\\Admin\\Catalog\\ProductAddEdit\\Catalog.TagMap.csv",
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

        /* markers tests */
        [Test]
        public void ProductEditMarkerAdd()
        {
            GoToClient("products/test-product106");
            VerifyIsFalse(Driver.PageSource.Contains("Хит"));
            VerifyIsFalse(Driver.PageSource.Contains("Новинка"));
            VerifyIsFalse(Driver.PageSource.Contains("Рекомендуем"));
            VerifyIsFalse(Driver.PageSource.Contains("Распродажа"));

            GoToAdmin("product/edit/106");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxRecomendedClick\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxSalesClick\"]")).Click();
            Thread.Sleep(1000);

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/106");

            //check admin
            VerifyIsTrue(Driver.FindElement(By.Id("Recomended")).Selected);
            VerifyIsTrue(Driver.FindElement(By.Id("Sales")).Selected);
            VerifyIsFalse(Driver.FindElement(By.Id("BestSeller")).Selected);
            VerifyIsFalse(Driver.FindElement(By.Id("New")).Selected);

            //check client product card
            GoToClient("products/test-product106");

            VerifyIsTrue(Driver.PageSource.Contains("Рекомендуем"));
            VerifyIsTrue(Driver.PageSource.Contains("Распродажа"));
            VerifyIsFalse(Driver.PageSource.Contains("Хит"));
            VerifyIsFalse(Driver.PageSource.Contains("Новинка"));

            //check client category
            GoToClient("categories/test-category7");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"106\"]")).Text.Contains("Рекомендуем"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"106\"]")).Text.Contains("Распродажа"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"106\"]")).Text.Contains("Хит"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"106\"]")).Text.Contains("Новинка"));

            //check client subcategory
            GoToClient("categories/test-category6");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"106\"]")).Text.Contains("Рекомендуем"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"106\"]")).Text.Contains("Распродажа"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"106\"]")).Text.Contains("Хит"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"106\"]")).Text.Contains("Новинка"));
        }

        [Test]
        public void ProductEditMarkerAddAdmin()
        {
            GoToClient("products/test-product107");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Хит"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Новинка"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Рекомендуем"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Распродажа"));

            GoToAdmin("product/edit/107");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxBestSellerClick\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxNewClick\"]")).Click();
            Thread.Sleep(1000);

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/107");

            //check admin product card
            VerifyIsTrue(Driver.FindElement(By.Id("BestSeller")).Selected);
            VerifyIsTrue(Driver.FindElement(By.Id("New")).Selected);
            VerifyIsFalse(Driver.FindElement(By.Id("Recomended")).Selected);
            VerifyIsFalse(Driver.FindElement(By.Id("Sales")).Selected);

            //check admin catalog
            GoToAdmin("catalog");
            VerifyAreEqual("1/1",
                Driver.FindElements(By.CssSelector(".aside-menu-inner"))[2]
                    .FindElement(By.CssSelector(".aside-menu-count-inner")).Text);
            VerifyAreEqual("1/1",
                Driver.FindElements(By.CssSelector(".aside-menu-inner"))[3]
                    .FindElement(By.CssSelector(".aside-menu-count-inner")).Text);

            Driver.FindElement(By.XPath("//div[contains(text(), 'Хиты продаж')]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(0, "Name").Text);

            Driver.FindElement(By.XPath("//div[contains(text(), 'Новинки')]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct107", Driver.GetGridCell(0, "Name").Text);

            //check client product card
            GoToClient("products/test-product107");
            VerifyIsTrue(Driver.PageSource.Contains("Хит"));
            VerifyIsTrue(Driver.PageSource.Contains("Новинка"));
            VerifyIsFalse(Driver.PageSource.Contains("Рекомендуем"));
            VerifyIsFalse(Driver.PageSource.Contains("Распродажа"));

            //check client category
            GoToClient("categories/test-category7");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Хит"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Новинка"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Рекомендуем"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Распродажа"));

            //check client subcategory
            GoToClient("categories/test-category6");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Хит"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Новинка"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Рекомендуем"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"107\"]")).Text.Contains("Распродажа"));
        }

        [Test]
        public void ProductEditMarkerAddAll()
        {
            GoToClient("products/test-product108");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Хит"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Новинка"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Рекомендуем"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Распродажа"));

            GoToAdmin("product/edit/108");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxBestSellerClick\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxNewClick\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxRecomendedClick\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"CheckBoxSalesClick\"]")).Click();
            Thread.Sleep(1000);

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("product/edit/108");

            //check admin product card
            VerifyIsTrue(Driver.FindElement(By.Id("BestSeller")).Selected);
            VerifyIsTrue(Driver.FindElement(By.Id("New")).Selected);
            VerifyIsTrue(Driver.FindElement(By.Id("Recomended")).Selected);
            VerifyIsTrue(Driver.FindElement(By.Id("Sales")).Selected);

            //check admin catalog
            GoToAdmin("catalog");

            Driver.FindElement(By.XPath("//div[contains(text(), 'Хиты продаж')]")).Click();
            Driver.GridFilterSendKeys("TestProduct108");
            VerifyAreEqual("TestProduct108", Driver.GetGridCell(0, "Name").Text);

            Driver.FindElement(By.XPath("//div[contains(text(), 'Новинки')]")).Click();
            Thread.Sleep(1000);
            Driver.GridFilterSendKeys("TestProduct108");
            VerifyAreEqual("TestProduct108", Driver.GetGridCell(0, "Name").Text);

            //check client product card
            GoToClient("products/test-product108");
            VerifyIsTrue(Driver.PageSource.Contains("Хит"));
            VerifyIsTrue(Driver.PageSource.Contains("Новинка"));
            VerifyIsTrue(Driver.PageSource.Contains("Рекомендуем"));
            VerifyIsTrue(Driver.PageSource.Contains("Распродажа"));

            //check client category
            GoToClient("categories/test-category7");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Хит"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Новинка"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Рекомендуем"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Распродажа"));

            //check client subcategory
            GoToClient("categories/test-category6");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Хит"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Новинка"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Рекомендуем"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"108\"]")).Text.Contains("Распродажа"));
        }

        /* tags tests */
        [Test]
        public void ProductEditTagAddFromSelect()
        {
            GoToAdmin("product/edit/1");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"SelectTag\"]")).Text.Contains("TagName2"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectTag\"]")).FindElement(By.TagName("input")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TagName3')]")).Click();
            Thread.Sleep(1000);
            Driver.DropFocus("h1");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("product/edit/1");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"SelectTag\"]")).Text.Contains("TagName2"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"SelectTag\"]")).Text.Contains("TagName3"));

            //check client
            GoToClient("categories/test-category1");
            VerifyAreEqual("TagName2",
                Driver.FindElement(By.CssSelector(".tags")).FindElements(By.TagName("a"))[1].Text);
            VerifyAreEqual("TagName3",
                Driver.FindElement(By.CssSelector(".tags")).FindElements(By.TagName("a"))[2].Text);

            VerifyIsTrue(Driver.PageSource.Contains("TestProduct1"));
            VerifyAreEqual("18", Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text);

            Driver.FindElement(By.LinkText("TagName3")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.PageSource.Contains("TagName3"));
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct1"));

            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text);
        }

        [Test]
        public void ProductEditTagDelete()
        {
            GoToAdmin("product/edit/6");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"SelectTag\"]")).Text.Contains("TagName1"));

            //check client
            GoToClient("categories/test-category1");
            VerifyAreEqual("TagName1",
                Driver.FindElement(By.CssSelector(".tags")).FindElements(By.TagName("a"))[0].Text);

            VerifyIsTrue(Driver.PageSource.Contains("TestProduct6"));
            VerifyAreEqual("18", Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text);

            Driver.FindElement(By.LinkText("TagName1")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.PageSource.Contains("TagName1"));
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct6"));

            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text);

            //check tag delete
            GoToAdmin("product/edit/6");

            Driver.FindElement(By.CssSelector(".close.ui-select-match-close")).Click();
            Thread.Sleep(1000);

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check client
            GoToClient("categories/test-category1/tag/tag-name1");

            VerifyIsFalse(Driver.PageSource.Contains("TestProduct6"));
        }
    }
}