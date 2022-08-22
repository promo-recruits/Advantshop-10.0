using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditGifts : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Brand.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Property.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Offer.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Category.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.ProductCategories.csv"
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

        protected void AddGiftsProduct(string name = null)
        {
            Driver.WaitForElemDisplayedAndClick(By.CssSelector(".header-subtext .btn-action"));
            Driver.WaitForModal();
            Driver.GetGridFilter().SendKeys(name);
            Driver.DropFocus("h2");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));
        }

        [Test]
        [Order(1)]
        public void AddGiftBySearch()
        {
            GoToAdmin("product/edit/2#?tabsInProduct=3");
            Driver.WaitForElem(By.ClassName("page-name-block-text"));
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();

            AddGiftsProduct("11");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct11"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 1);

            AddGiftsProduct("13");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct13"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 2);

            AddGiftsProduct("14");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct14"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 3);

            AddGiftsProduct("15");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct15"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 4);

            GoToClient("products/test-product2");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-gift-image")).Count == 1);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs-3.col-p-v.gift-img")).Count == 4);

            Driver.MouseFocus(By.CssSelector(".product-gift-image"));
            VerifyAreEqual("TestProduct11",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[0].Text);
            VerifyAreEqual("TestProduct13",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[1].Text);
            VerifyAreEqual("TestProduct14",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[2].Text);
            VerifyAreEqual("TestProduct15",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[3].Text);

            /*   GoToAdmin("product/edit/2#?tabsInProduct=3");
               Functions.DelElement(driver);*/
        }

        [Test]
        [Order(2)]
        public void AddGiftByPage()
        {
            GoToAdmin("product/edit/3#?tabsInProduct=3");
            Driver.WaitForElem(By.ClassName("page-name-block-text"));
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Driver.WaitForElemDisplayedAndClick(By.CssSelector(".header-subtext .btn-action"));
            Driver.WaitForModal();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[2][\'Name\']\"]"));
            Driver.GetGridCell(3, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.GetGridCell(4, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.GetGridCell(5, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.GetGridCell(6, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct21"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct22"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct23"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct24"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 4);

            GoToClient("products/test-product3");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-gift-image")).Count == 1);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs-3.col-p-v.gift-img")).Count == 4);

            //  driver.FindElement(By.CssSelector(".product-gift-image")).Click();
            // Thread.Sleep(500);
            Driver.MouseFocus(By.CssSelector(".product-gift-image"));
            VerifyAreEqual("TestProduct21",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[0].Text);
            VerifyAreEqual("TestProduct22",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[1].Text);
            VerifyAreEqual("TestProduct23",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[2].Text);
            VerifyAreEqual("TestProduct24",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[3].Text);

            GoToAdmin("product/edit/3#?tabsInProduct=3");
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Driver.WaitForElemDisplayedAndClick(By.CssSelector(".header-subtext .btn-action"));

            Driver.FindElement(By.ClassName("close")).Click();
            Functions.DelElement(Driver);
            GoToClient("products/test-product3");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-gift-image")).Count == 0);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs-3.col-p-v.gift-img")).Count == 0);
        }

        //[Test]
        //public void AddGiftByFilter()
        //{
        //    GoToAdmin("product/edit/4#?tabsInProduct=3");
        //     Driver.ScrollTo(By.XPath("//h2[contains(text(), 'Списки')]"));

        //    Functions.AddProductToListByFilter(driver, linkText: "Подарки", filter: "ProductArtNo", item: "12", tabIndex: 2);
        //    VerifyIsTrue(driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct12"));
        //    VerifyIsTrue(driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 1);

        //    Functions.AddProductToListByFilter(driver, linkText: "Подарки", filter: "ProductArtNo", item: "13", tabIndex: 2);
        //    VerifyIsTrue(driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct13"));
        //    VerifyIsTrue(driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 2);

        //    Functions.AddProductToListByFilter(driver, linkText: "Подарки", filter: "ProductArtNo", item: "14", tabIndex: 2);
        //    VerifyIsTrue(driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct14"));
        //    VerifyIsTrue(driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 3);

        //    Functions.AddProductToListByFilter(driver, linkText: "Подарки", filter: "ProductArtNo", item: "15", tabIndex: 2);
        //    VerifyIsTrue(driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct15"));
        //    VerifyIsTrue(driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 4);

        //    GoToClient("products/test-product4");
        //    Thread.Sleep(1000);
        //    VerifyIsTrue(driver.FindElements(By.CssSelector(".product-gift-image")).Count == 1);
        //    VerifyIsTrue(driver.FindElements(By.CssSelector(".col-xs-3.col-p-v.gift-img")).Count == 4);

        //    driver.FindElement(By.CssSelector(".product-gift-image")).Click();
        //    Thread.Sleep(1000);
        //    VerifyAreEqual("TestProduct12", driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[0].Text);
        //    VerifyAreEqual("TestProduct13", driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[1].Text);
        //    VerifyAreEqual("TestProduct14", driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[2].Text);
        //    VerifyAreEqual("TestProduct15", driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[3].Text);

        //    GoToAdmin("product/edit/4#?tabsInProduct=3");
        //    Functions.DelElement(driver);
        //}

        [Test]
        [Order(3)]
        public void AddGiftOnPage()
        {
            GoToAdmin("product/edit/5#?tabsInProduct=3");
            Driver.WaitForElem(By.ClassName("page-name-block-text"));
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Driver.WaitForElemDisplayedAndClick(By.CssSelector(".header-subtext .btn-action"));
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(
                    "[data-e2e-grid-cell=\"gridProductsSelectvizr[-1]['selectionRowHeaderCol']\"] [data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct1"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct10"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct11"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct12"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 10);

            GoToClient("products/test-product5");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-gift-image")).Count == 1);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs-3.col-p-v.gift-img")).Count == 10);

            Driver.MouseFocus(By.CssSelector(".product-gift-image"));
            VerifyAreEqual("TestProduct1", Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[0].Text);
            VerifyAreEqual("TestProduct10",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[1].Text);
            VerifyAreEqual("TestProduct11",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[2].Text);
            VerifyAreEqual("TestProduct12",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[3].Text);
            /*  GoToAdmin("product/edit/5#?tabsInProduct=3");
              Functions.DelElement(driver);*/
        }

        [Test]
        [Order(10)]
        public void AddGiftzAll()
        {
            GoToAdmin("product/edit/6#?tabsInProduct=3");
            Driver.WaitForElem(By.ClassName("page-name-block-text"));
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();
            Driver.WaitForElemDisplayedAndClick(By.CssSelector(".header-subtext .btn-action"));
            Driver.WaitForModal();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory2')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.GetGridCell(-1, "selectionRowHeaderCol", "ProductsSelectvizr").FindElement(
                    By.CssSelector(
                        "[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"] input[type=\"checkbox\"]~.adv-checkbox-emul"))
                .Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct28"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct29"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct30"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct31"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 4);

            GoToClient("products/test-product6");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-gift-image")).Count == 1);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs-3.col-p-v.gift-img")).Count == 4);

            Driver.MouseFocus(By.CssSelector(".product-gift-image"));
            VerifyAreEqual("TestProduct28",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[0].Text);
            VerifyAreEqual("TestProduct29",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[1].Text);
            VerifyAreEqual("TestProduct30",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[2].Text);
            VerifyAreEqual("TestProduct31",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[3].Text);

            /*  GoToAdmin("product/edit/6#?tabsInProduct=3");
              Functions.DelElement(driver);*/
        }

        [Test]
        [Order(4)]
        public void AddGiftByPageDel()
        {
            GoToAdmin("product/edit/7#?tabsInProduct=3");
            Driver.WaitForElem(By.ClassName("page-name-block-text"));
            Driver.FindElement(By.XPath("//div[contains(text(), 'SEO')]")).Click();

            Driver.WaitForElemDisplayedAndClick(By.CssSelector(".header-subtext .btn-action"));
            Driver.WaitForModal();
            Driver.ScrollTo(AdvBy.DataE2E("gridPaginationSelect"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[2][\'Name\']\"]"));
            Driver.GetGridCell(3, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.GetGridCell(4, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.GetGridCell(5, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.GetGridCell(6, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.GetGridCell(7, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.GetGridCell(8, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct21"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct22"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct23"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct24"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 6);
            Driver.FindElement(By.CssSelector(".pull-right a")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct22"));
            Driver.FindElement(By.CssSelector(".pull-right a")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct23"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct24"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct25"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct26"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 4);

            GoToClient("products/test-product7");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-gift-image")).Count == 1);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs-3.col-p-v.gift-img")).Count == 4);

            Driver.MouseFocus(By.CssSelector(".product-gift-image"));
            VerifyAreEqual("TestProduct23",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[0].Text);
            VerifyAreEqual("TestProduct24",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[1].Text);
            VerifyAreEqual("TestProduct25",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[2].Text);
            VerifyAreEqual("TestProduct26",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[3].Text);
            /*
                        GoToAdmin("product/edit/7#?tabsInProduct=3");
                        Functions.DelElement(driver);*/
        }
    }
}