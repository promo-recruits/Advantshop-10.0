using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditSimilar : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Category.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Brand.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Property.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Offer.csv",
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

        protected void AddSimilarProduct(string name = null)
        {
            Driver.FindElements(By.CssSelector(".header-subtext .btn.btn-sm.btn-action"))[0].Click();
            Thread.Sleep(1000);
            Driver.GetGridFilter().SendKeys(name);
            Driver.DropFocus("h2");
            VerifyAreEqual(name, Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
        }

        [Test]
        public void AddSimilarBySearch()
        {
            GoToAdmin("product/edit/2#?tabsInProduct=2");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);

            AddSimilarProduct("TestProduct12");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct12"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 1);

            AddSimilarProduct("TestProduct13");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct13"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 2);

            AddSimilarProduct("TestProduct14");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct14"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 3);

            AddSimilarProduct("TestProduct15");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct15"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 4);

            GoToClient("products/test-product2");
            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));

            //Похожие товары
            VerifyAreEqual("Похожие товары", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyIsTrue(Driver
                .FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))
                .Count == 4);

            VerifyAreEqual("TestProduct12",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct14",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //next 
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct13",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct15",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //pre
            Driver.FindElement(By.CssSelector(".carousel-nav-prev.icon-left-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct12",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct14",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            /* GoToAdmin("product/edit/2#?tabsInProduct=2");
             Functions.DelElement(driver);*/
        }

        [Test]
        public void AddSimilarByPage()
        {
            GoToAdmin("product/edit/3#?tabsInProduct=2");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);

            Driver.FindElements(By.CssSelector(".header-subtext .btn.btn-sm.btn-action"))[0].Click();
            Thread.Sleep(1000);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(3, "Name", "ProductsSelectvizr").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[2][\'Name\']\"]"));
            Driver.GetGridCell(3, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(4, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(5, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(6, "Name", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct21"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct22"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct23"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct24"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 4);

            GoToClient("products/test-product3");
            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));

            //С этим товаром покупают
            VerifyAreEqual("Похожие товары", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyIsTrue(Driver
                .FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))
                .Count == 4);

            VerifyAreEqual("TestProduct21",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct23",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //next 
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct22",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct24",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //pre
            Driver.FindElement(By.CssSelector(".carousel-nav-prev.icon-left-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct21",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct23",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            /*   GoToAdmin("product/edit/3#?tabsInProduct=2");
               Functions.DelElement(driver);*/
        }

        [Test]
        public void AddSimilarByFilter()
        {
            GoToAdmin("product/edit/4#?tabsInProduct=2");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);

            Functions.AddProductToListByFilter(Driver, linkText: "Похожие товары", filter: "ProductArtNo", item: "12",
                tabIndex: 0, gridCell: "ProductArtNo");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct12"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 1);

            Functions.AddProductToListByFilter(Driver, linkText: "Похожие товары", filter: "ProductArtNo", item: "13",
                tabIndex: 0, gridCell: "ProductArtNo");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct13"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 2);

            Functions.AddProductToListByFilter(Driver, linkText: "Похожие товары", filter: "ProductArtNo", item: "14",
                tabIndex: 0, gridCell: "ProductArtNo");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct14"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 3);


            GoToClient("products/test-product4");
            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));

            //С этим товаром покупают
            VerifyAreEqual("Похожие товары", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyIsTrue(Driver
                .FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))
                .Count == 3);

            VerifyAreEqual("TestProduct12",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct14",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            /*
                        GoToAdmin("product/edit/4#?tabsInProduct=2");
                        Functions.DelElement(driver);*/
        }

        [Test]
        public void AddSimilarOnPage()
        {
            GoToAdmin("product/edit/5#?tabsInProduct=2");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);

            Driver.FindElements(By.CssSelector(".header-subtext .btn.btn-sm.btn-action"))[0].Click();
            Thread.Sleep(1000);
            Driver.GetGridCell(-1, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct1"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct10"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct11"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct12"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 10);

            GoToClient("products/test-product5");
            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));

            //С этим товаром покупают
            VerifyAreEqual("Похожие товары", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyIsTrue(Driver
                .FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))
                .Count == 10);

            VerifyAreEqual("TestProduct1",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct11",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //next 
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct10",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct12",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct11",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct13",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[4]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct12",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct14",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[5]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct13",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[4]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct15",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[6]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            /*   GoToAdmin("product/edit/5#?tabsInProduct=2");
               Functions.DelElement(driver);*/
        }

        [Test]
        public void AddSimilarzAll()
        {
            GoToAdmin("product/edit/6#?tabsInProduct=2");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);

            Driver.FindElements(By.CssSelector(".header-subtext .btn.btn-sm.btn-action"))[0].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory2')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.GetGridCell(-1, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text
                .Contains("28 - TestProduct28"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text
                .Contains("29 - TestProduct29"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text
                .Contains("30 - TestProduct30"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text
                .Contains("31 - TestProduct31"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 4);

            GoToClient("products/test-product6");
            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));

            //Похожие товары
            VerifyAreEqual("Похожие товары", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyIsTrue(Driver
                .FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))
                .Count == 4);

            VerifyAreEqual("TestProduct28",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct30",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //next 
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct29",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct31",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            /* driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
             Thread.Sleep(1000);
             VerifyAreEqual("TestProduct23", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2].FindElement(By.CssSelector(".products-view-name-link")).Text);
             VerifyAreEqual("TestProduct1", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[4].FindElement(By.CssSelector(".products-view-name-link")).Text);
           /*  driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
             Thread.Sleep(1000);
             VerifyAreEqual("TestProduct12", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3].FindElement(By.CssSelector(".products-view-name-link")).Text);
             VerifyAreEqual("TestProduct14", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[5].FindElement(By.CssSelector(".products-view-name-link")).Text);
             driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
             Thread.Sleep(1000);
             VerifyAreEqual("TestProduct13", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[4].FindElement(By.CssSelector(".products-view-name-link")).Text);
             VerifyAreEqual("TestProduct15", driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[6].FindElement(By.CssSelector(".products-view-name-link")).Text);
             GoToAdmin("product/edit/6#?tabsInProduct=2");
             Functions.DelElement(driver);*/
        }

        [Test]
        public void AddSimilarByPageDel()
        {
            GoToAdmin("product/edit/7#?tabsInProduct=2");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);

            Driver.FindElements(By.CssSelector(".header-subtext .btn.btn-sm.btn-action"))[0].Click();
            Thread.Sleep(1000);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(3, "Name", "ProductsSelectvizr").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[2][\'Name\']\"]"));
            Driver.GetGridCell(3, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(5, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(6, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(7, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(8, "Name", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct21"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct23"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct24"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct25"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 5);
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
            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));

            //С этим товаром покупают
            VerifyAreEqual("Похожие товары", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyIsTrue(Driver
                .FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))
                .Count == 4);

            VerifyAreEqual("TestProduct23",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct25",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //next 
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct24",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct26",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //pre
            Driver.FindElement(By.CssSelector(".carousel-nav-prev.icon-left-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct23",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct25",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            
            GoToAdmin("product/edit/7#?tabsInProduct=2");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);
            Functions.DelElement(Driver);

            GoToClient("products/test-product7");
            VerifyIsFalse(Driver.PageSource.Contains("Похожие товары"));
            VerifyIsTrue(Driver
                .FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))
                .Count == 0);
        }
    }
}