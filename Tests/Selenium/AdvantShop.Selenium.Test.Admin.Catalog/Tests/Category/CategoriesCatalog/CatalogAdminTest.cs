using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.CategoriesCatalog
{
    [TestFixture]
    public class AdminCatalogTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\CatalogTest\\Catalog.Tag.csv",
                "data\\Admin\\Catalog\\CatalogTest\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\CatalogTest\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\CatalogTest\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\CatalogTest\\Catalog.ProductCategories.csv");

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
        [Order(0)]
        public void CatalogCategoryOpenAndEdit()
        {
            GoToAdmin("catalog");

            //check correct count
            VerifyAreEqual("6",
                Driver.FindElement(By.CssSelector(
                        "[data-e2e-select=\"CategoryTop\"] [data-e2e-select=\"CategoryTopRightAll\"] [data-e2e-quantity=\"CategoryAllQuantity\"]"))
                    .Text);

            //check catalog open
            VerifyAreEqual("TestCategory1",
                Driver.FindElement(
                        By.CssSelector("[data-e2e=\"categoriesBlockItem\"][data-e2e-categories-block-item-id=\"1\"]"))
                    .Text);

            //check catalog go to edit by top
            Driver.FindElement(By.CssSelector("[data-e2e=\"EditCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));
            VerifyAreEqual("Категория \"Каталог\"", Driver.FindElement(By.TagName("h1")).Text);

            //check category open
            GoBack();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestCategory1", Driver.FindElement(By.TagName("h2")).Text);

            //check category go to edit
            GoBack();
            Driver.FindElement(
                    By.CssSelector(
                        "[data-e2e=\"categoriesBlockItemEdit\"][data-e2e-categories-block-item-edit-id=\"1\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));
            VerifyAreEqual("Категория \"TestCategory1\"", Driver.FindElement(By.TagName("h1")).Text);

            //check category go to edit by top
            GoBack();
            Driver.FindElement(
                By.CssSelector("[data-e2e=\"categoriesBlockItem\"][data-e2e-categories-block-item-id=\"1\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"EditCategory\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));
            VerifyAreEqual("Категория \"TestCategory1\"", Driver.FindElement(By.TagName("h1")).Text);
        }

        [Test]
        [Order(1)]
        public void CatalogCategorySearch()
        {
            GoToAdmin("catalog");

            //pre check correct count
            VerifyAreEqual("6",
                Driver.FindElement(By.CssSelector(
                        "[data-e2e-select=\"CategoryTop\"] [data-e2e-select=\"CategoryTopRightAll\"] [data-e2e-quantity=\"CategoryAllQuantity\"]"))
                    .Text);

            //check search
            Driver.FindElement(By.CssSelector("input.form-control")).Click();
            Driver.FindElement(By.CssSelector("input.form-control")).SendKeys("TestCategory3");
            Driver.FindElement(By.CssSelector("input.form-control")).SendKeys(Keys.Enter);

            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".categories-block-content")).Count);
            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector(
                        "[data-e2e-select=\"CategoryTop\"] [data-e2e-select=\"CategoryTopRightAll\"] [data-e2e-quantity=\"CategoryAllQuantity\"]"))
                    .Text);
            VerifyAreEqual("TestCategory3",
                Driver.FindElement(
                        By.CssSelector("[data-e2e=\"categoriesBlockItem\"][data-e2e-categories-block-item-id=\"3\"]"))
                    .Text);
        }

        [Test]
        [Order(2)]
        public void CategorySelectAndDelete()
        {
            GoToAdmin("catalog");

            //check select item
            Driver.FindElement(By.CssSelector(
                "[data-e2e=\"categoriesBlockItemSelect\"][data-e2e-categories-block-item-select-id=\"1\"]")).Click();
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"categoriesBlockItemSelect\"][data-e2e-categories-block-item-select-id=\"1\"] [data-e2e-select=\"CategorySelectTrue\"]"))
                .Selected);

            //check cancel delete item
            Driver.FindElement(By.CssSelector(
                "[data-e2e=\"categoriesBlockItemDelete\"][data-e2e-categories-block-item-delete-id=\"2\"]")).Click();
            Driver.SwalCancel();
            VerifyIsTrue(6 == Driver.FindElements(By.CssSelector(".categories-block-wrap.as-sortable-item")).Count);

            //check delete item
            Driver.FindElement(By.CssSelector(
                "[data-e2e=\"categoriesBlockItemDelete\"][data-e2e-categories-block-item-delete-id=\"2\"]")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(5 == Driver.FindElements(By.CssSelector(".categories-block-wrap.as-sortable-item")).Count);

            //check selected items
            Driver.FindElement(By.CssSelector(
                "[data-e2e=\"categoriesBlockItemSelect\"][data-e2e-categories-block-item-select-id=\"1\"]")).Click();
            Driver.FindElement(By.CssSelector(
                "[data-e2e=\"categoriesBlockItemSelect\"][data-e2e-categories-block-item-select-id=\"3\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".btn.btn-sm.btn-default")).Text
                .Contains("2 категории выбрано"));

            //check delete selected items
            Driver.FindElement(By.CssSelector("[data-e2e-select=\"CategorySelectedDelete\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("TestCategory1",
                Driver.FindElement(By.CssSelector("[data-e2e-block=\"Category\"]")).Text);
            VerifyAreNotEqual("TestCategory3",
                Driver.FindElement(By.CssSelector("[data-e2e-block=\"Category\"]")).Text);

            //check item delete from edit
            Driver.FindElement(
                    By.CssSelector(
                        "[data-e2e=\"categoriesBlockItemEdit\"][data-e2e-categories-block-item-edit-id=\"7\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("brandLinkLook"));
            VerifyAreEqual("Категория \"TestCategory7\"", Driver.FindElement(By.TagName("h1")).Text);
            Driver.FindElement(By.XPath("//a[contains(text(), 'Удалить')]")).Click();
            Driver.SwalConfirm();
            GoToAdmin("catalog");
            VerifyIsTrue(2 == Driver.FindElements(By.CssSelector(".categories-block-wrap.as-sortable-item")).Count);

            //check all items selected 
            Driver.FindElement(By.CssSelector("[data-e2e-select=\"CategorySelect\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"categoriesBlockItemSelect\"][data-e2e-categories-block-item-select-id=\"5\"] [data-e2e-select=\"CategorySelectTrue\"]"))
                .Selected);
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"categoriesBlockItemSelect\"][data-e2e-categories-block-item-select-id=\"6\"] [data-e2e-select=\"CategorySelectTrue\"]"))
                .Selected);

            //check all items selected delete
            Driver.FindElement(By.CssSelector("[data-e2e-select=\"CategorySelectedDelete\"]")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(0 == Driver.FindElements(By.CssSelector("[data-e2e=\"categoriesBlockItem\"]")).Count);

            GoToAdmin("catalog");
            //VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"));
            VerifyIsTrue(0 == Driver.FindElements(By.CssSelector("[data-e2e=\"categoriesBlockItem\"]")).Count);
        }
    }
}