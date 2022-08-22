using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.ProductAddEdit
{
    [TestFixture]
    public class ProductAddEditAlternative : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Brand.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.PropertyGroup.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Property.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.ProductPropertyValue.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.Offer.csv",
                "Data\\Admin\\Catalog\\ProductAddEdit\\Lists\\Catalog.ProductCategories.csv"
            );
            Init();
            //Functions.RecalculateProducts(driver, baseURL);
            //Functions.RecalculateSearch(driver, baseURL);
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

        protected void AddAlternativeProduct(string name = null)
        {
            Driver.FindElement(By.CssSelector(".header-subtext .btn.btn-sm.btn-action")).Click();
            Driver.WaitForModal();
            Driver.GetGridFilter().SendKeys(name);
            Driver.DropFocus("h2");
            VerifyAreEqual(name, Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text);
            Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
        }


        [Test]
        public void AddAlternativeBySearch()
        {
            GoToAdmin("product/edit/2");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);//костыль, иначе блок не успевает подгружаться

            AddAlternativeProduct("TestProduct12");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct12"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 1);

            AddAlternativeProduct("TestProduct13");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct13"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 2);

            AddAlternativeProduct("TestProduct14");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct14"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 3);

            AddAlternativeProduct("TestProduct15");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct15"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 4);
            GoToClient("products/test-product2");
            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));

            //С этим товаром покупают
            VerifyAreEqual("С этим товаром покупают", Driver.FindElement(By.CssSelector(".h2")).Text);
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
        }

        [Test]
        public void AddAlternativeByPage()
        {
            GoToAdmin("product/edit/25");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);//костыль, иначе блок не успевает подгружаться

            Driver.FindElement(By.CssSelector(".header-subtext .btn.btn-sm.btn-action")).Click();
            Driver.WaitForModal();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(3, "Name", "ProductsSelectvizr").Text);
            Driver.GetGridCell(2, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(3, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(4, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(5, "Name", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct20"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct21"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct22"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct23"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 4);

            GoToClient("products/test-product25");
            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));

            //С этим товаром покупают
            VerifyAreEqual("С этим товаром покупают", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyIsTrue(Driver
                .FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))
                .Count == 4);

            VerifyAreEqual("TestProduct20",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct22",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //next 
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct21",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct23",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //pre
            Driver.FindElement(By.CssSelector(".carousel-nav-prev.icon-left-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct20",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct22",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //GoToAdmin("product/edit/25");
            //Functions.DelElement(driver);
        }

        [Test]
        public void AddAlternativeByFilter()
        {
            GoToAdmin("product/edit/4");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);//костыль, иначе блок не успевает подгружаться

            Functions.AddProductToListByFilter(Driver, linkText: "С этим товаром покупают", filter: "Name",
                item: "TestProduct12", gridCell: "Name");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct12"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 1);
            Thread.Sleep(1000);
            Functions.AddProductToListByFilter(Driver, linkText: "С этим товаром покупают", filter: "Name",
                item: "TestProduct13", gridCell: "Name");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct13"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 2);
            Thread.Sleep(1000);
            Functions.AddProductToListByFilter(Driver, linkText: "С этим товаром покупают", filter: "Name",
                item: "TestProduct14", gridCell: "Name");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct14"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 3);
            Thread.Sleep(1000);
            Functions.AddProductToListByFilter(Driver, linkText: "С этим товаром покупают", filter: "Name",
                item: "TestProduct15", gridCell: "Name");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text.Contains("TestProduct15"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 4);

            GoToClient("products/test-product4");
            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));

            //С этим товаром покупают
            VerifyAreEqual("С этим товаром покупают", Driver.FindElement(By.CssSelector(".h2")).Text);
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
            //GoToAdmin("product/edit/4");
            //Functions.DelElement(driver);
        }

        [Test]
        public void AddAlternativeOnPage()
        {
            GoToAdmin("product/edit/5");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);//костыль, иначе блок не успевает подгружаться

            Driver.FindElement(By.CssSelector(".header-subtext .btn.btn-sm.btn-action")).Click();
            Driver.WaitForModal();
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
            VerifyAreEqual("С этим товаром покупают", Driver.FindElement(By.CssSelector(".h2")).Text);
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
            //GoToAdmin("product/edit/5");
            //Functions.DelElement(driver);
        }

        [Test]
        public void AddAlternativeAll()
        {
            GoToAdmin("product/edit/26");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);//костыль, иначе блок не успевает подгружаться

            Driver.FindElement(By.CssSelector(".header-subtext .btn.btn-sm.btn-action")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            Driver.GetGridCell(-1, "selectionRowHeaderCol", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 24);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct1"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text
                .Contains("TestProduct2"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text
                .Contains("TestProduct3"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link"))[3].Text
                .Contains("TestProduct4"));

            GoToClient("products/test-product26");
            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));

            //С этим товаром покупают
            VerifyAreEqual("С этим товаром покупают", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyIsTrue(Driver
                .FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))
                .Count == 24);

            VerifyAreEqual("TestProduct1",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct3",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //next 
            Driver.FindElement(By.CssSelector(".carousel-nav-next.icon-right-open-after.cs-l-1-interactive")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("TestProduct2",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[1]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct4",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            //GoToAdmin("product/edit/31");
            //Functions.DelElement(driver);
        }

        [Test]
        public void AddAlternativeByPageDel()
        {
            GoToAdmin("product/edit/7");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);//костыль, иначе блок не успевает подгружаться

            Driver.FindElement(By.CssSelector(".header-subtext .btn.btn-sm.btn-action")).Click();
            Driver.WaitForModal();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(3, "Name", "ProductsSelectvizr").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e-grid-cell=\"gridProductsSelectvizr[2][\'Name\']\"]"));
            Driver.GetGridCell(3, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(4, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(5, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(6, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(7, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(8, "Name", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);//костыль, иначе блок не успевает подгружаться
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
            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));

            //С этим товаром покупают
            VerifyAreEqual("С этим товаром покупают", Driver.FindElement(By.CssSelector(".h2")).Text);
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

            GoToAdmin("product/edit/7");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);//костыль, иначе блок не успевает подгружаться

            Functions.DelElement(Driver);

            GoToClient("products/test-product7");
            VerifyIsFalse(Driver.PageSource.Contains("С этим товаром покупают"));
            VerifyIsTrue(Driver
                .FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))
                .Count == 0);
        }

        [Test]
        public void AddAlternativeSimilarGiftzByPage()
        {
            GoToAdmin("product/edit/3");
            Driver.FindElement(By.XPath("//div[contains(text(), 'Списки')]")).Click();
            Thread.Sleep(1000);//костыль, иначе блок не успевает подгружаться

            Driver.FindElement(By.CssSelector(".header-subtext .btn.btn-sm.btn-action")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(3, "Name", "ProductsSelectvizr").Text);
            Driver.GetGridCell(3, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(4, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(5, "Name", "ProductsSelectvizr").Click();


            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".item-block__name-link")).Text.Contains("TestProduct21"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct22"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[2].Text.Contains("TestProduct23"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 3);

            Driver.ScrollTo(By.Id("lists"));
            Driver.FindElements(By.CssSelector(".uib-tab.nav-item.ng-tab"))[1].Click();

            Driver.FindElement(By.CssSelector("related-products[data-type='Alternative'] .btn")).Click();
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(1, "Name", "ProductsSelectvizr").Click();

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link"))[0].Text
                .Contains("TestProduct1"));
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[1].Text.Contains("TestProduct10"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 2);

            Driver.FindElements(By.CssSelector(".uib-tab.nav-item.ng-tab"))[2].Click();

            Driver.FindElement(By.CssSelector("product-gifts .btn")).Click();
            Thread.Sleep(1000);
            Driver.GetGridCell(2, "selectionRowHeaderCol", "OffersSelectvizr").Click();

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".item-block__name-link"))[0].Text.Contains("TestProduct11"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".item-block__name-link")).Count == 1);

            GoToClient("products/test-product3");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".product-gift-image")).Count == 1);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".col-xs-3.col-p-v.gift-img")).Count == 1);

            Driver.FindElement(By.CssSelector(".product-gift-image")).Click();
            Driver.MouseFocus(By.CssSelector(".product-gift-image"));
            VerifyAreEqual("TestProduct11",
                Driver.FindElements(By.CssSelector(".col-xs-9.col-p-v.gift-txt a"))[0].Text);

            Driver.ScrollTo(By.CssSelector(".tabs.tabs-horizontal.details-tabs"));
            VerifyAreEqual("С этим товаром покупают", Driver.FindElement(By.CssSelector(".h2")).Text);
            VerifyIsTrue(Driver
                .FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))
                .Count == 5);

            VerifyAreEqual("TestProduct21",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[0]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct23",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[2]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("Похожие товары", Driver.FindElements(By.CssSelector(".h2"))[1].Text);
            VerifyAreEqual("TestProduct1",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[3]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
            VerifyAreEqual("TestProduct10",
                Driver.FindElements(
                        By.CssSelector(".products-view-block.cs-br-1.js-products-view-block.js-carousel-item"))[4]
                    .FindElement(By.CssSelector(".products-view-name-link")).Text);
        }
    }
}