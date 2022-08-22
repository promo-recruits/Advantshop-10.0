using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Client.Tests.Desktop.SmokeTests
{
    [TestFixture]
    public class ClientSmokeCatalogTests : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS | ClearType.Catalog | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Client\\SmokeTests\\CMS.StaticBlock.csv",
                "data\\Client\\SmokeTests\\CMS.StaticPage.csv",
                "data\\Client\\SmokeTests\\Catalog.Photo.csv",
                "data\\Client\\SmokeTests\\Catalog.Category.csv",
                "data\\Client\\SmokeTests\\Catalog.Brand.csv",
                "data\\Client\\SmokeTests\\Catalog.Product.csv",
                "data\\Client\\SmokeTests\\Catalog.ProductCategories.csv",
                "data\\Client\\SmokeTests\\Catalog.Tag.csv",
                "data\\Client\\SmokeTests\\Catalog.Property.csv",
                "data\\Client\\SmokeTests\\Catalog.PropertyValue.csv",
                "data\\Client\\SmokeTests\\Catalog.ProductPropertyValue.csv",
                "data\\Client\\SmokeTests\\Catalog.PropertyGroup.csv",
                "data\\Client\\SmokeTests\\Catalog.Color.csv",
                "data\\Client\\SmokeTests\\Catalog.Size.csv",
                "data\\Client\\SmokeTests\\Catalog.Offer.csv",
                "data\\Client\\SmokeTests\\CMS.Menu.csv",
                "data\\Client\\SmokeTests\\Settings.NewsCategory.csv",
                "data\\Client\\SmokeTests\\Settings.News.csv",
                "data\\Client\\SmokeTests\\Customers.Customer.csv",
                "data\\Client\\SmokeTests\\Customers.CustomerGroup.csv"
            );

            Init();
            EnableInplaceOff();
            ReindexSearch();
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
        public void ClientCategoryOpen()
        {
            GoToClient();

            VerifyIsTrue(Driver.PageSource.Contains("TestCategory1"));
            VerifyIsTrue(Driver.PageSource.Contains("TestCategory2"));

            Driver.FindElement(By.LinkText("TestCategory1")).Click();
            Thread.Sleep(4000);

            //check product in category
            VerifyAreEqual("TestCategory1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct1"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-product-id=\"1\"]"))
                .FindElement(By.CssSelector(".products-view-picture-link img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"1\"]"))
                .FindElement(By.CssSelector(".products-view-price")).Text.Contains("1"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-product-id=\"1\"]"))
                .FindElement(By.CssSelector(".products-view-buttons a")).Enabled);

            GoToClient();

            Driver.FindElement(By.LinkText("TestCategory6")).Click();
            Thread.Sleep(4000);

            VerifyAreEqual("TestCategory6", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("TestCategory7",
                Driver.FindElement(By.CssSelector(".product-categories-header-thin.h2")).Text);
            VerifyIsTrue(Driver.PageSource.Contains("TestProduct101"));
        }

        [Test]
        public void ClientCategoryView()
        {
            GoToClient();

            Driver.FindElement(By.LinkText("TestCategory1")).Click();
            Thread.Sleep(4000);

            //check view tile
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".row.products-view.products-view-tile")).Displayed);
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".row.products-view.products-view-list")).Count);
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".row.products-view.products-view-table")).Count);

            //check view list
            Driver.FindElement(By.CssSelector("[title=\"Список\"]")).Click();
            Thread.Sleep(4000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".row.products-view.products-view-list")).Displayed);
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".row.products-view.products-view-tile")).Count);
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".row.products-view.products-view-table")).Count);

            //check view table
            Driver.FindElement(By.CssSelector("[title=\"Таблица\"]")).Click();
            Thread.Sleep(4000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".row.products-view.products-view-table")).Displayed);
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".row.products-view.products-view-tile")).Count);
            VerifyAreEqual(0, Driver.FindElements(By.CssSelector(".row.products-view.products-view-list")).Count);
        }


        [Test]
        public void ClientProductOpen()
        {
            Functions.AdminSettingsProductCart(Driver, BaseUrl);

            GoToClient("categories/test-category1");

            Driver.ScrollTo(By.CssSelector("[data-product-id=\"1\"]"));
            Driver.FindElement(By.LinkText("TestProduct1")).Click();
            Thread.Sleep(4000);

            VerifyIsTrue(Driver.Url.Contains("test-product1"));
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("Скидка 50%",
                Driver.FindElement(By.CssSelector(".products-view-label-inner.products-view-label-discount")).Text);
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".gallery-picture.text-static img")).GetAttribute("src")
                .Contains("nophoto"));

            //check brief
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-meta-list")).Text.Contains("BrandName1"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-sku"))
                .FindElement(By.CssSelector(".details-param-value.inplace-offset")).Text.Contains("1"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-dimensions"))
                .FindElement(By.CssSelector(".details-param-value")).Text.Contains("1 x 1 x 1"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-weight"))
                .FindElement(By.CssSelector(".details-param-value")).Text.Contains("1"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-weight"))
                .FindElement(By.CssSelector(".details-param-name")).Text.Contains("Вес:"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-unit"))
                .FindElement(By.CssSelector(".details-param-value")).Text.Contains("unit"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-unit"))
                .FindElement(By.CssSelector(".details-param-name")).Text.Contains("Ед. измерения:"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-briefproperties"))
                .FindElement(By.CssSelector(".details-param-value")).Text.Contains("PropertyValue1"));

            //check cart
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-payment.cs-br-1")).Text.Contains("1"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-payment.cs-br-1")).Text.Contains("1"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-row.details-payment.cs-br-1"))
                .FindElement(By.CssSelector(".details-payment-cell a")).Enabled);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".color-viewer-inner.cs-br-1")).Displayed);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".sizes-viewer-inner.center-aligner.cs-l-1")).Displayed);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".sizes-viewer-inner.center-aligner.cs-l-1")).Text
                .Contains("SizeName1"));
            VerifyIsTrue(Driver.PageSource.Contains("Desc1"));
        }

        [Test]
        public void ClientProductColorAmount()
        {
            GoToAdmin("product/edit/4");
            //scroll
            Driver.FindElement(By.XPath("//div[contains(text(), 'Цена и наличие')]")).Click();
            Thread.Sleep(3000);
            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить цену')]")).Click();
            Thread.Sleep(3000);
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddSize\"]")))).SelectByText("SizeName1");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AddColor\"]")))).SelectByText("Color1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddPrice\"]")).SendKeys("44");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddAmount\"]")).SendKeys("44");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Thread.Sleep(4000);

            GoToClient("products/test-product4");

            //check client product cart
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[title=\"Color1\"]")).GetAttribute("class")
                .Contains("selected"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[title=\"Color4\"]")).GetAttribute("class")
                .Contains("selected"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[title=\"SizeName1\"] input")).Enabled);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".price")).Text.Contains("2"));

            Driver.ScrollTo(By.CssSelector("[title=\"Color1\"]"));
            Driver.FindElement(By.CssSelector("[title=\"Color1\"]")).Click();
            Thread.Sleep(4000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[title=\"Color1\"]")).GetAttribute("class")
                .Contains("selected"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[title=\"Color4\"]")).GetAttribute("class")
                .Contains("selected"));
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[title=\"SizeName4\"] input")).Enabled);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".price")).Text.Contains("22"));
        }

        [Test]
        public void ClientProductsOnMainPage()
        {
            GoToClient();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-best.cs-br-1"))
                .FindElement(By.LinkText("TestProduct7")).Displayed);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-specials-block.products-specials-new.cs-br-1"))
                .FindElement(By.LinkText("TestProduct4")).Displayed);
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(".products-specials-block.products-specials-discount.cs-br-1"))
                .FindElement(By.LinkText("TestProduct10")).Displayed);
        }


        [Test]
        public void ClientCategorySort()
        {
            GoToClient("categories/test-category4");

            //check sort by price
            (new SelectElement(Driver.FindElement(By.Id("Sorting")))).SelectByText("Сначала дешевле");
            Thread.Sleep(3000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                .FindElement(By.CssSelector(".products-view-info")).FindElement(By.TagName("a")).Text
                .Contains("TestProduct66"));

            (new SelectElement(Driver.FindElement(By.Id("Sorting")))).SelectByText("Сначала дороже");
            Thread.Sleep(3000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                .FindElement(By.CssSelector(".products-view-info")).FindElement(By.TagName("a")).Text
                .Contains("TestProduct61"));

            //check sort by new
            (new SelectElement(Driver.FindElement(By.Id("Sorting")))).SelectByText("Новинки");
            Thread.Sleep(3000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                .FindElement(By.CssSelector(".products-view-info")).FindElement(By.TagName("a")).Text
                .Contains("TestProduct80"));

            //check sort by sale
            (new SelectElement(Driver.FindElement(By.Id("Sorting")))).SelectByText("По размеру скидки");
            Thread.Sleep(3000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                .FindElement(By.CssSelector(".products-view-info")).FindElement(By.TagName("a")).Text
                .Contains("TestProduct66"));

            //check sort by rating
            (new SelectElement(Driver.FindElement(By.Id("Sorting")))).SelectByText("Высокий рейтинг");
            Thread.Sleep(3000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                .FindElement(By.CssSelector(".products-view-info")).FindElement(By.TagName("a")).Text
                .Contains("TestProduct70"));

            //check sort by hit
            (new SelectElement(Driver.FindElement(By.Id("Sorting")))).SelectByText("Популярные");
            Thread.Sleep(3000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-view-block.cs-br-1.js-products-view-block"))[0]
                .FindElement(By.CssSelector(".products-view-info")).FindElement(By.TagName("a")).Text
                .Contains("TestProduct61"));
        }

        [Test]
        public void ClientCategoryFilterPrice()
        {
            GoToClient("categories/test-category3");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text.Contains("20"));

            Driver.ScrollTo(By.Name("catalogFilterForm"));
            IWebElement elem = Driver.FindElement(By.CssSelector(".ngrs-handle.ngrs-handle-min"));
            Actions move = new Actions(Driver);
            move.DragAndDropToOffset(elem, 140, 0).Perform();
            Thread.Sleep(3000);
            // Driver.WaitForAjax();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-ng-bind=\"catalogFilter.foundCount\"]"))
                .GetAttribute("innerText").Contains("10"));
            Driver.ScrollTo(By.Name("catalogFilterForm"));
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Thread.Sleep(3000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text.Contains("10"));
            VerifyIsTrue(Driver.Url.Contains("pricefrom"));
            VerifyIsTrue(Driver.Url.Contains("priceto"));
        }

        [Test]
        public void ClientCategoryFilterBrand()
        {
            GoToClient("categories/test-category3");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-sort-result-number"))
                .GetAttribute("innerText").Contains("20"));

            Driver.ScrollTo(By.CssSelector(".catalog-filter-block"), 1);
            Driver.FindElements(By.CssSelector(".catalog-filter-block"))[1]
                .FindElements(By.CssSelector(".catalog-filter-row span"))[0].Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-ng-bind='catalogFilter.foundCount']"))
                .GetAttribute("innerText").Contains("1"));
            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector(".catalog-filter-popover-text span")).GetAttribute("innerText"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".adv-popover-content"))[0]
                .FindElement(By.CssSelector(".catalog-filter-popover-text")).FindElement(By.TagName("span"))
                .GetAttribute("innerText").Contains("1"));
        }

        [Test]
        public void ClientCategoryFilterSize()
        {
            GoToClient("categories/test-category3");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text.Contains("20"));

            Driver.ScrollTo(By.CssSelector("[title=\"Color1\"]"));
            Driver.FindElements(By.CssSelector(".catalog-filter-block"))[3]
                .FindElements(By.CssSelector(".catalog-filter-row span"))[0].Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-ng-bind=\"catalogFilter.foundCount\"]"))
                .GetAttribute("innerText").Contains("1"));
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Thread.Sleep(4000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text.Contains("1"));
        }

        [Test]
        public void ClientCategoryFilterColor()
        {
            GoToClient("categories/test-category3");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text.Contains("20"));

            Driver.ScrollTo(By.CssSelector("[title=\"Color1\"]"));
            Driver.FindElement(By.CssSelector("[title=\"Color1\"]")).Click();
            Driver.FindElement(By.CssSelector("[title=\"Color2\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-ng-bind=\"catalogFilter.foundCount\"]"))
                .GetAttribute("innerText").Contains("2"));
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Thread.Sleep(4000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-sort-result-number")).Text.Contains("2"));
        }

        [Test]
        public void ClientProductCompareList()
        {
            GoToClient("products/test-product4");

            Driver.FindElement(By.CssSelector(".compare-control.cs-l-2")).Click();
            Thread.Sleep(700);
            Driver.FindElement(By.LinkText("Просмотреть")).Click();
            Thread.Sleep(700);
            VerifyIsTrue(Driver.FindElement(By.ClassName("compareproduct-product-row")).Text.Contains("TestProduct4"));

            GoToClient("products/test-product5");

            Driver.FindElement(By.CssSelector(".compare-control.cs-l-2")).Click();
            Thread.Sleep(700);
            VerifyIsTrue(Driver.FindElement(By.ClassName("compare-text-added")).Text.Contains("Уже в сравнении"));
            Refresh();
            VerifyIsTrue(Driver.FindElement(By.ClassName("compare-text-added")).Text.Contains("Уже в сравнении"));
            Driver.FindElement(By.CssSelector(".compare-control.cs-l-2")).Click();
            Thread.Sleep(700);
            GoToClient("compare");
            Thread.Sleep(700);
            VerifyIsFalse(Driver.FindElement(By.ClassName("compareproduct-product-row")).Text.Contains("TestProduct5"));

            GoToClient("products/test-product6");

            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"compareCount.countObj.count\"]")).Text);
            Driver.FindElement(By.CssSelector(".compare-control.cs-l-2")).Click();
            Thread.Sleep(700);
            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"compareCount.countObj.count\"]")).Text);
            Driver.FindElements(By.ClassName("toolbar-bottom-block"))[1].Click();
            Thread.Sleep(700);

            Driver.FindElement(By.LinkText("Очистить список")).Click();
            VerifyAreEqual("display: none;",
                Driver.FindElement(By.CssSelector(".compareproduct-container")).GetAttribute("style"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".compareproduct-empty-title")).Text
                .Contains("Список сравнения пуст"));
            Driver.FindElement(By.LinkText("Перейдите в каталог")).Click();
            VerifyAreEqual("Каталог", Driver.FindElement(By.TagName("h1")).Text);
        }

        [Test]
        public void ClientProductWishList()
        {
            GoToClient("products/test-product60");

            Driver.FindElement(By.CssSelector(".wishlist-control.cs-l-2")).Click();
            Thread.Sleep(700);
            Driver.FindElement(By.LinkText("Просмотреть")).Click();
            Thread.Sleep(700);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-container")).Text.Contains("TestProduct60"));

            GoToClient("products/test-product5");

            Driver.FindElement(By.CssSelector(".wishlist-control.cs-l-2")).Click();
            Thread.Sleep(700);
            VerifyIsTrue(Driver.FindElement(By.ClassName("wishlist-text-added")).Text.Contains("В избранном"));
            Refresh();
            VerifyIsTrue(Driver.FindElement(By.ClassName("wishlist-text-added")).Text.Contains("В избранном"));
            Driver.FindElement(By.CssSelector(".wishlist-control.cs-l-2")).Click();
            Thread.Sleep(700);
            GoToClient("wishlist");
            Thread.Sleep(700);
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".products-view-container")).Text.Contains("TestProduct5"));

            GoToClient("products/test-product4");

            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"wishlistCount.countObj.count\"]")).Text);
            Driver.FindElement(By.CssSelector(".wishlist-control.cs-l-2")).Click();
            Thread.Sleep(700);
            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"wishlistCount.countObj.count\"]")).Text);
            Driver.FindElement(By.CssSelector(".wishlist-bottom-block")).Click();
            Thread.Sleep(700);

            GoToClient("products/test-product8");

            Driver.FindElement(By.CssSelector(".wishlist-control.cs-l-2")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.LinkText("Просмотреть")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Color4",
                Driver.FindElements(By.CssSelector(".color-viewer-item-block.cs-br-2"))[0].GetAttribute("title"));
            VerifyAreEqual("Color8",
                Driver.FindElements(By.CssSelector(".color-viewer-item-block.cs-br-2"))[1].GetAttribute("title"));

            VerifyAreEqual("wishlist-empty  ng-hide",
                Driver.FindElement(By.CssSelector(".wishlist-empty")).GetAttribute("class"));
            Driver.FindElements(By.CssSelector(".js-wishlist-remove"))[0].Click();
            Driver.FindElements(By.CssSelector(".js-wishlist-remove"))[0].Click();
            Driver.FindElements(By.CssSelector(".js-wishlist-remove"))[0].Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-ng-hide=\"wishlistPage.countObj.count === 0\"]"))
                .GetAttribute("class").Contains("ng-hide"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".wishlist-empty")).Text
                .Contains("Ваш список избранного пуст."));
        }
    }
}