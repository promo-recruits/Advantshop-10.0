using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Services.servicesThreeColumns.Button.Checkout
{
    [TestFixture]
    public class LandingsServicesCheckoutProductAdd : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing | ClearType.CRM);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Color.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Size.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerField.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSubBlock.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.DealStatus.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.SalesFunnel.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.SalesFunnel_DealStatus.csv"
            );

            Init();
        }

        [Test]
        public void ServicesFormAddOne()
        {
            TestName = "ServicesFormAddOne";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Переход на оплату");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            //check grid
            VerifyAreEqual("1", Driver.GetGridCell(0, "ArtNo", "OffersSelectvizr").Text, "ArtNo product at grid");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid");
            VerifyAreEqual("Color1", Driver.GetGridCell(0, "ColorName", "OffersSelectvizr").Text, "color product at grid");
            VerifyAreEqual("SizeName1", Driver.GetGridCell(0, "SizeName", "OffersSelectvizr").Text, "size product at grid");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "PriceFormatted", "OffersSelectvizr").Text,
                "Price product at grid");

            VerifyAreEqual("rgba(0, 0, 0, 1)", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("color"),
                "ArtNo product at color font color");
            VerifyAreEqual("700", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("font-weight"),
                "ArtNo product at color font-weight");
            VerifyIsTrue(
                Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                    .FindElements(By.CssSelector(".ui-grid-icon-plus-squared")).Count == 0, "no btn tree");

            Driver.GridFilterSendKeys("TestProduct504");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no match search");

            Driver.GridFilterSendKeys("TestProduct2");
            VerifyAreEqual("TestProduct2", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            VerifyAreEqual("Color2", Driver.GetGridCell(0, "ColorName", "OffersSelectvizr").Text, "color product at grid 2");
            VerifyAreEqual("SizeName2", Driver.GetGridCell(0, "SizeName", "OffersSelectvizr").Text, "size product at grid 2");
            VerifyAreEqual("101 руб.", Driver.GetGridCell(0, "PriceFormatted", "OffersSelectvizr").Text,
                "Price product at grid 2");
            VerifyAreEqual("rgba(181, 180, 180, 1)", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("color"),
                "ArtNo product at color font color 2");
            VerifyAreEqual("700", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("font-weight"),
                "ArtNo product at color font-weight 2");
            VerifyIsTrue(
                Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                    .FindElements(By.CssSelector(".ui-grid-icon-plus-squared")).Count == 0, " btn tree 2");

            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            VerifyAreEqual("Color10", Driver.GetGridCell(0, "ColorName", "OffersSelectvizr").Text, "color product at grid 2");
            VerifyAreEqual("SizeName10", Driver.GetGridCell(0, "SizeName", "OffersSelectvizr").Text, "size product at grid 2");
            VerifyAreEqual("109 руб.", Driver.GetGridCell(0, "PriceFormatted", "OffersSelectvizr").Text,
                "Price product at grid 2");
            VerifyAreEqual("rgba(0, 0, 0, 1)", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("color"),
                "ArtNo product at color font color 3");
            VerifyAreEqual("700", Driver.GetGridCell(0, "Name", "OffersSelectvizr").GetCssValue("font-weight"),
                "ArtNo product at color font-weight 3");
            VerifyIsTrue(
                Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                    .FindElements(By.CssSelector(".ui-grid-icon-plus-squared")).Count == 1, " btn tree 3");

            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            VerifyAreEqual("103", Driver.GetGridCell(1, "ArtNo", "OffersSelectvizr").Text, "ArtNo product at grid 4");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text, "Name product at grid 4");
            VerifyAreEqual("Color2", Driver.GetGridCell(1, "ColorName", "OffersSelectvizr").Text, "color product at grid 4");
            VerifyAreEqual("SizeName2", Driver.GetGridCell(1, "SizeName", "OffersSelectvizr").Text, "size product at grid 4");
            VerifyAreEqual("111 руб.", Driver.GetGridCell(1, "PriceFormatted", "OffersSelectvizr").Text,
                "Price product at grid 4");
            VerifyAreEqual("rgba(0, 0, 0, 1)", Driver.GetGridCell(1, "Name", "OffersSelectvizr").GetCssValue("color"),
                "ArtNo product at color font color 4");
            VerifyAreEqual("400", Driver.GetGridCell(1, "Name", "OffersSelectvizr").GetCssValue("font-weight"),
                "ArtNo product at color font-weight 4");
            VerifyIsTrue(
                Driver.GetGridCell(1, "treeBaseRowHeaderCol", "OffersSelectvizr")
                    .FindElements(By.CssSelector(".ui-grid-icon-plus-squared")).Count == 0, " btn tree 4");

            VerifyAreEqual("106", Driver.GetGridCell(4, "ArtNo", "OffersSelectvizr").Text, "ArtNo product at grid 5");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(4, "Name", "OffersSelectvizr").Text, "Name product at grid 5");
            VerifyAreEqual("Color5", Driver.GetGridCell(4, "ColorName", "OffersSelectvizr").Text, "color product at grid 5");
            VerifyAreEqual("SizeName5", Driver.GetGridCell(4, "SizeName", "OffersSelectvizr").Text, "size product at grid 5");
            VerifyAreEqual("200 руб.", Driver.GetGridCell(4, "PriceFormatted", "OffersSelectvizr").Text,
                "Price product at grid 5");
            VerifyAreEqual("rgba(0, 0, 0, 1)", Driver.GetGridCell(4, "Name", "OffersSelectvizr").GetCssValue("color"),
                "product color font color 5");
            VerifyAreEqual("400", Driver.GetGridCell(4, "Name", "OffersSelectvizr").GetCssValue("font-weight"),
                "product color font-weight 5");
            VerifyIsTrue(
                Driver.GetGridCell(4, "treeBaseRowHeaderCol", "OffersSelectvizr")
                    .FindElements(By.CssSelector(".ui-grid-icon-plus-squared")).Count == 0, " btn tree 5");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-minus-squared")).Click();

            Driver.GridFilterSendKeys("10");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count grid 5");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[7]['Name']\"]")).Count ==
                0, " no 8 row");
            Driver.GetGridCell(-1, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[7]['Name']\"]")).Count ==
                1, " display 8 row");

            Driver.GridFilterSendKeys("TestProduct1");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).GetAttribute("href")
                    .Contains("product/edit/1"), "href product");

            Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Click();
            Thread.Sleep(2000);
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual("Товар \"TestProduct1\"", Driver.FindElement(By.TagName("h1")).Text, "name product on page");
            VerifyAreEqual(BaseUrl + "/adminv3/product/edit/1", Driver.Url, "url product on page");
            Functions.CloseTab(Driver, BaseUrl);

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            VerifyAreEqual("TestProduct1", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 1", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Цвет: Color1"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Размер: SizeName1"),
                "size on checkout page");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("100 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Result\"]")).Text,
                "price all on checkout page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success/"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("?mode=lp"), "url success checkout lp mode");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 on page success checkout");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "StatusName ");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, "BuyerName ");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1\r\nГабариты: 1 x 1 x 1 мм\r\nВес: 1 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("100", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");

            //link lp
            var selectElem = Driver.FindElement(By.Id("Order_OrderSourceId"));
            var select = new SelectElement(selectElem);
            VerifyAreEqual("Воронка продаж \"test1\"", @select.AllSelectedOptions[0].Text, "source");
            VerifyAreEqual(BaseUrl + "/lp/test1/", Driver.FindElement(By.CssSelector("[data-e2e=\"LpLink\"]")).Text,
                "link on lp");
            Driver.FindElement(By.CssSelector("[data-e2e=\"LpLink\"]")).Click();
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual(BaseUrl + "/lp/test1", Driver.Url, "url product on page");
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).Displayed, "open lp");
            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text, "title on page");
            Functions.CloseTab(Driver, BaseUrl);

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddOneColor()
        {
            TestName = "ServicesFormAddOneColor";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Переход на оплату");

            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_104\"]")).Displayed, "display product");
            Thread.Sleep(1000);
            BlockSettingsSave();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            VerifyAreEqual("TestProduct10", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 1", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Цвет: Color3"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Размер: SizeName3"),
                "size on checkout page");
            VerifyAreEqual("112 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");
            VerifyAreEqual("112 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("112 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Result\"]")).Text,
                "price all on checkout page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success/"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("?mode=lp"), "url success checkout lp mode");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 on page success checkout");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "StatusName ");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, "BuyerName ");
            VerifyAreEqual("112 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct10\r\nАртикул: 104\r\nЦвет: Color3\r\nРазмер: SizeName3\r\nГабариты: 3 x 3 x 3 мм\r\nВес: 3 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("112", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("112 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddOneDisabled()
        {
            TestName = "ServicesFormAddOneDisabled";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Переход на оплату");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct2");
            VerifyAreEqual("TestProduct2", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product");
            Thread.Sleep(1000);
            BlockSettingsSave();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            VerifyAreEqual("TestProduct2", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 1", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Цвет: Color2"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Размер: SizeName2"),
                "size on checkout page");
            VerifyAreEqual("101 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");
            VerifyAreEqual("101 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("101 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Result\"]")).Text,
                "price all on checkout page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success/"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("?mode=lp"), "url success checkout lp mode");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 on page success checkout");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "StatusName ");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, "BuyerName ");
            VerifyAreEqual("101 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct2\r\nАртикул: 2\r\nЦвет: Color2\r\nРазмер: SizeName2\r\nГабариты: 2 x 2 x 2 мм\r\nВес: 2 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("101", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("101 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSome()
        {
            TestName = "ServicesFormAddSome";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Переход на оплату");

            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct1");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct4");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct2");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Thread.Sleep(2000);

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_4\"]")).Displayed, "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_10\"]")).Displayed,
                "display product 3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 4");
            Thread.Sleep(1000);
            BlockSettingsSave();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            //product 1
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 1", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Цвет: Color1"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Размер: SizeName1"),
                "size on checkout page");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");

            //product 2
            VerifyAreEqual("TestProduct4", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[1].Text,
                "product on checkout page 2");
            VerifyAreEqual("Количество: 1", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[1].Text,
                "count on checkout page 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[1].Text.Contains("Цвет: Color4"),
                "color on checkout page 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[1].Text
                    .Contains("Размер: SizeName4"), "size on checkout page 2");
            VerifyAreEqual("103 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[1].Text,
                "price on checkout page 2");

            //product 3
            VerifyAreEqual("TestProduct10", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[2].Text,
                "product on checkout page 3");
            VerifyAreEqual("Количество: 1", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[2].Text,
                "count on checkout page 3");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[2].Text.Contains("Цвет: Color10"),
                "color on checkout page 3");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[2].Text
                    .Contains("Размер: SizeName10"), "size on checkout page 3");
            VerifyAreEqual("109 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[2].Text,
                "price on checkout page 3");

            //product 4
            VerifyAreEqual("TestProduct10", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[3].Text,
                "product on checkout page 4");
            VerifyAreEqual("Количество: 1", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[3].Text,
                "count on checkout page 4");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[3].Text.Contains("Цвет: Color2"),
                "color on checkout page 4");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[3].Text
                    .Contains("Размер: SizeName2"), "size on checkout page 4");
            VerifyAreEqual("111 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[3].Text,
                "price on checkout page 4");

            //product 5
            VerifyAreEqual("TestProduct2", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[4].Text,
                "product on checkout page 5");
            VerifyAreEqual("Количество: 1", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[4].Text,
                "count on checkout page 5");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[4].Text.Contains("Цвет: Color2"),
                "color on checkout page 5");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[4].Text
                    .Contains("Размер: SizeName2"), "size on checkout page 5");
            VerifyAreEqual("101 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[4].Text,
                "price on checkout page 5");

            VerifyAreEqual("524 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("524 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Result\"]")).Text,
                "price all on checkout page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success/"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("?mode=lp"), "url success checkout lp mode");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 on page success checkout");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "StatusName ");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, "BuyerName ");
            VerifyAreEqual("524 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1\r\nГабариты: 1 x 1 x 1 мм\r\nВес: 1 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("100", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");

            VerifyAreEqual(
                "TestProduct4\r\nАртикул: 4\r\nЦвет: Color4\r\nРазмер: SizeName4\r\nГабариты: 4 x 4 x 4 мм\r\nВес: 4 кг",
                Driver.GetGridCell(1, "Name", "OrderItems").Text, "Name product at order 2");
            VerifyAreEqual("103", Driver.GetGridCell(1, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyAreEqual("103 руб.", Driver.GetGridCell(1, "Cost", "OrderItems").Text, " Cost product at order 2");

            VerifyAreEqual(
                "TestProduct10\r\nАртикул: 10\r\nЦвет: Color10\r\nРазмер: SizeName10\r\nГабариты: 10 x 10 x 10 мм\r\nВес: 10 кг",
                Driver.GetGridCell(2, "Name", "OrderItems").Text, "Name product at order 3");
            VerifyAreEqual("109", Driver.GetGridCell(2, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 3");
            VerifyAreEqual("1", Driver.GetGridCell(2, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 3");
            VerifyAreEqual("109 руб.", Driver.GetGridCell(2, "Cost", "OrderItems").Text, " Cost product at order 3");

            VerifyAreEqual(
                "TestProduct10\r\nАртикул: 103\r\nЦвет: Color2\r\nРазмер: SizeName2\r\nГабариты: 2 x 2 x 2 мм\r\nВес: 2 кг",
                Driver.GetGridCell(3, "Name", "OrderItems").Text, "Name product at order 4");
            VerifyAreEqual("111", Driver.GetGridCell(3, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 4");
            VerifyAreEqual("1", Driver.GetGridCell(3, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 4");
            VerifyAreEqual("111 руб.", Driver.GetGridCell(3, "Cost", "OrderItems").Text, " Cost product at order 4");

            VerifyAreEqual(
                "TestProduct2\r\nАртикул: 2\r\nЦвет: Color2\r\nРазмер: SizeName2\r\nГабариты: 2 x 2 x 2 мм\r\nВес: 2 кг",
                Driver.GetGridCell(4, "Name", "OrderItems").Text, "Name product at order 2");
            VerifyAreEqual("101", Driver.GetGridCell(4, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("1", Driver.GetGridCell(4, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyAreEqual("101 руб.", Driver.GetGridCell(4, "Cost", "OrderItems").Text, " Cost product at order 2");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSomeCategory()
        {
            TestName = "ServicesFormAddSomeCategory";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Переход на оплату");

            DelAllProduct();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product 1 at grid");
            VerifyAreEqual("Найдено записей: 102",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count grid 2");

            Driver.GridFilterSendKeys("10");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count search");
            Driver.GetGridFilter().SendKeys(Keys.Backspace);
            Driver.GetGridFilter().SendKeys(Keys.Backspace);
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 1 at grid after del");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_2\"]")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 2 at grid 2");
            VerifyAreEqual("TestProduct22", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product 3 at grid 2");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_4\"]")).GetAttribute("class")
                    .Contains("jstree-leaf"), " btn tree 4");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_1\"]")).GetAttribute("class")
                    .Contains("jstree-leaf"), " btn tree 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-tree-id=\"categoryItemId_5\"]")).Count == 0,
                " no btn tree 5");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_4\"] i")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-tree-id=\"categoryItemId_5\"]")).Count == 1,
                " yes btn tree 5");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_5\"]")).GetAttribute("class")
                    .Contains("jstree-leaf"), " btn tree 5");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_5\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product 2 at grid");
            VerifyAreEqual("TestProduct82", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text, "Name product 3 at grid");
            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count grid 5");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count select 1");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_2\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count select 1 after ");
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 2 at grid 2");
            VerifyAreEqual("TestProduct22", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product 3 at grid 2");
            VerifyAreEqual("Найдено записей: 20",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count grid 2");
            Driver.GetGridCell(2, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count select 2");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_1\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product 2 at grid 1");
            VerifyAreEqual("TestProduct2", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text, "Name product 3 at grid 1");
            Driver.GetGridCell(3, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count select 3");

            Driver.GridFilterSendKeys("10");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count search in category");
            Driver.GetGridFilter().SendKeys(Keys.Backspace);
            Driver.GetGridFilter().SendKeys(Keys.Backspace);
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_4\"] .jstree-anchor")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 2 at grid 4");
            VerifyAreEqual("TestProduct62", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product 3 at grid 4");
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            VerifyAreEqual("4", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count select 4");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_0\"] .jstree-anchor")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 1 at grid all");
            VerifyAreEqual("Найдено записей: 102",
                Driver.FindElement(By.CssSelector("ui-grid-custom-filter .ui-grid-custom-filter-total")).Text,
                "count grid all");

            Driver.FindElement(By.CssSelector("[data-tree-id=\"categoryItemId_4\"] i")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-tree-id=\"categoryItemId_5\"]")).Count == 0,
                " hide btn tree 5");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product 1 at grid hide");

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_81\"]")).Displayed,
                "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_23\"]")).Displayed,
                "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_4\"]")).Displayed, "display product 3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_62\"]")).Displayed,
                "display product 4");

            BlockSettingsSave();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            //product 1
            VerifyAreEqual("TestProduct81", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 1", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyAreEqual("180 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");

            //product 2
            VerifyAreEqual("TestProduct23", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[1].Text,
                "product on checkout page 2");
            VerifyAreEqual("Количество: 1", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[1].Text,
                "count on checkout page 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[0].Text.Contains("Цвет: Color3"),
                "color on checkout page 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[0].Text
                    .Contains("Размер: SizeName3"), "size on checkout page 2");
            VerifyAreEqual("122 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[1].Text,
                "price on checkout page 2");

            //product 3
            VerifyAreEqual("TestProduct4", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[2].Text,
                "product on checkout page 3");
            VerifyAreEqual("Количество: 1", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[2].Text,
                "count on checkout page 3");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[1].Text.Contains("Цвет: Color4"),
                "color on checkout page 3");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[1].Text
                    .Contains("Размер: SizeName4"), "size on checkout page 3");
            VerifyAreEqual("103 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[2].Text,
                "price on checkout page 3");

            //product 4
            VerifyAreEqual("TestProduct62", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[3].Text,
                "product on checkout page 4");
            VerifyAreEqual("Количество: 1", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[3].Text,
                "count on checkout page 4");
            VerifyAreEqual("161 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[3].Text,
                "price on checkout page 4");

            VerifyAreEqual("566 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("566 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Result\"]")).Text,
                "price all on checkout page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success/"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("?mode=lp"), "url success checkout lp mode");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 on page success checkout");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "StatusName ");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, "BuyerName ");
            VerifyAreEqual("566 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual("TestProduct81\r\nАртикул: 81\r\nГабариты: 81 x 81 x 81 мм\r\nВес: 81 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("180", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("180 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");

            VerifyAreEqual(
                "TestProduct23\r\nАртикул: 23\r\nЦвет: Color3\r\nРазмер: SizeName3\r\nГабариты: 23 x 23 x 23 мм\r\nВес: 23 кг",
                Driver.GetGridCell(1, "Name", "OrderItems").Text, "Name product at order 2");
            VerifyAreEqual("122", Driver.GetGridCell(1, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyAreEqual("122 руб.", Driver.GetGridCell(1, "Cost", "OrderItems").Text, " Cost product at order 2");

            VerifyAreEqual(
                "TestProduct4\r\nАртикул: 4\r\nЦвет: Color4\r\nРазмер: SizeName4\r\nГабариты: 4 x 4 x 4 мм\r\nВес: 4 кг",
                Driver.GetGridCell(2, "Name", "OrderItems").Text, "Name product at order 3");
            VerifyAreEqual("103", Driver.GetGridCell(2, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 3");
            VerifyAreEqual("1", Driver.GetGridCell(2, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 3");
            VerifyAreEqual("103 руб.", Driver.GetGridCell(2, "Cost", "OrderItems").Text, " Cost product at order 3");

            VerifyAreEqual("TestProduct62\r\nАртикул: 62\r\nГабариты: 62 x 62 x 62 мм\r\nВес: 62 кг",
                Driver.GetGridCell(3, "Name", "OrderItems").Text, "Name product at order 4");
            VerifyAreEqual("161", Driver.GetGridCell(3, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 4");
            VerifyAreEqual("1", Driver.GetGridCell(3, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 4");
            VerifyAreEqual("161 руб.", Driver.GetGridCell(3, "Cost", "OrderItems").Text, " Cost product at order 4");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSomeDel()
        {
            TestName = "ServicesFormAddSomeDel";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Переход на оплату");

            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct1");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GridFilterSendKeys("TestProduct5");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_10\"]")).Displayed,
                "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_5\"]")).Displayed, "display product 4");
            Thread.Sleep(1000);

            //del elem
            Driver.FindElement(By.CssSelector("[data-e2e=\"DelProduct\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[data-e2e=\"DelProduct\"]")).Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_5\"]")).Displayed,
                "display product 2.2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 4.2");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_10\"]")).Count == 0,
                "no display product 3");

            //add elem
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct15");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 5");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_5\"]")).Displayed, "display product 6");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_15\"]")).Displayed,
                "display product 7");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product 11");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_10\"]")).Count == 0,
                "no display product 31");

            BlockSettingsSave();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            //product 1
            VerifyAreEqual("TestProduct10", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 1", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[0].Text.Contains("Цвет: Color2"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[0].Text
                    .Contains("Размер: SizeName2"), "size on checkout page");
            VerifyAreEqual("111 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");

            //product 2
            VerifyAreEqual("TestProduct5", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[1].Text,
                "product on checkout page 2");
            VerifyAreEqual("Количество: 1", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[1].Text,
                "count on checkout page 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[1].Text.Contains("Цвет: Color5"),
                "color on checkout page 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[1].Text
                    .Contains("Размер: SizeName5"), "size on checkout page 2");
            VerifyAreEqual("104 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[1].Text,
                "price on checkout page 2");

            //product 3
            VerifyAreEqual("TestProduct15", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[2].Text,
                "product on checkout page 3");
            VerifyAreEqual("Количество: 1", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[2].Text,
                "count on checkout page 3");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[2].Text.Contains("Цвет: Color5"),
                "color on checkout page 3");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[2].Text
                    .Contains("Размер: SizeName5"), "size on checkout page 3");
            VerifyAreEqual("114 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[2].Text,
                "price on checkout page 3");

            VerifyAreEqual("329 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("329 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Result\"]")).Text,
                "price all on checkout page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success/"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("?mode=lp"), "url success checkout lp mode");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 on page success checkout");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "StatusName ");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, "BuyerName ");
            VerifyAreEqual("329 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct10\r\nАртикул: 103\r\nЦвет: Color2\r\nРазмер: SizeName2\r\nГабариты: 2 x 2 x 2 мм\r\nВес: 2 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("111", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("111 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");

            VerifyAreEqual(
                "TestProduct5\r\nАртикул: 5\r\nЦвет: Color5\r\nРазмер: SizeName5\r\nГабариты: 5 x 5 x 5 мм\r\nВес: 5 кг",
                Driver.GetGridCell(1, "Name", "OrderItems").Text, "Name product at order 2");
            VerifyAreEqual("104", Driver.GetGridCell(1, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyAreEqual("104 руб.", Driver.GetGridCell(1, "Cost", "OrderItems").Text, " Cost product at order 2");

            VerifyAreEqual(
                "TestProduct15\r\nАртикул: 15\r\nЦвет: Color5\r\nРазмер: SizeName5\r\nГабариты: 15 x 15 x 15 мм\r\nВес: 15 кг",
                Driver.GetGridCell(2, "Name", "OrderItems").Text, "Name product at order 3");
            VerifyAreEqual("114", Driver.GetGridCell(2, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 3");
            VerifyAreEqual("1", Driver.GetGridCell(2, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 3");
            VerifyAreEqual("114 руб.", Driver.GetGridCell(2, "Cost", "OrderItems").Text, " Cost product at order 3");

            VerifyFinally(TestName);
        }


        [Test]
        public void ServicesFormAddSomeDelAll()
        {
            TestName = "ServicesFormAddSomeDelAll";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Переход на оплату");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(2000);

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GridFilterSendKeys("TestProduct2");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product 2");
            Thread.Sleep(1000);

            BlockSettingsSave();
            Thread.Sleep(1000);
            Refresh();
            BlockSettingsBtn();
            TabSelect("tabServiceButton");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed,
                "display product 1 after refresh");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed,
                "display product 2 after refresh");
            //del all elem
            DelAllProduct();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_2\"]")).Count == 0,
                "no display product 2");

            BlockSettingsSave();
            Thread.Sleep(1000);
            var path = Driver.Url;

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(path, Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page");
            VerifyAreEqual("Создавай с Advantshop",
                Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text, "h1 on page");
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).Displayed, "display block");
            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSomePagination()
        {
            TestName = "ServicesFormAddSomePagination";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Переход на оплату");

            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            //view
            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 1");
            VerifyAreEqual("TestProduct15", Driver.GetGridCell(9, "Name", "OffersSelectvizr").Text, "Name product at grid 10");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[10]['Name']\"]"))
                    .Count == 0, " no 11 row");

            Driver.GridPaginationSelectItems("100");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[50]['Name']\"]"))
                    .Count == 1, " 51 row");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 1");
            VerifyAreEqual("TestProduct97", Driver.GetGridCell(99, "Name", "OffersSelectvizr").Text,
                "Name product at grid 10");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[100]['Name']\"]"))
                    .Count == 0, " no 101 row");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 1");
            VerifyAreEqual("TestProduct15", Driver.GetGridCell(9, "Name", "OffersSelectvizr").Text, "Name product at grid 10");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"gridOffersSelectvizr[10]['Name']\"]"))
                    .Count == 0, " no 11 row 1");

            //pagination
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestProduct16", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 1");
            VerifyAreEqual("TestProduct17", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text, "Name product at grid 2");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product at grid 1 page");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product at grid 2 page");

            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestProduct98", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product at grid 1 page last");
            VerifyAreEqual("TestProduct99", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product at grid 2 page last");

            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product at grid 1 page first");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product at grid 2 page first");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("TestProduct25", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                "Name product at grid 1 page");
            VerifyAreEqual("TestProduct26", Driver.GetGridCell(1, "Name", "OffersSelectvizr").Text,
                "Name product at grid 2 page");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_25\"]")).Displayed,
                "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_26\"]")).Displayed,
                "display product 2");
            Thread.Sleep(1000);
            BlockSettingsSave();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            //product 1
            VerifyAreEqual("TestProduct25", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 1", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[0].Text.Contains("Цвет: Color5"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[0].Text
                    .Contains("Размер: SizeName5"), "size on checkout page");
            VerifyAreEqual("124 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");

            //product 2
            VerifyAreEqual("TestProduct26", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[1].Text,
                "product on checkout page 2");
            VerifyAreEqual("Количество: 1", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[1].Text,
                "count on checkout page 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[1].Text.Contains("Цвет: Color6"),
                "color on checkout page 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[1].Text
                    .Contains("Размер: SizeName6"), "size on checkout page 2");
            VerifyAreEqual("125 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[1].Text,
                "price on checkout page 2");

            VerifyAreEqual("249 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("249 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Result\"]")).Text,
                "price all on checkout page");


            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success/"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("?mode=lp"), "url success checkout lp mode");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 on page success checkout");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "StatusName ");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, "BuyerName ");
            VerifyAreEqual("249 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct25\r\nАртикул: 25\r\nЦвет: Color5\r\nРазмер: SizeName5\r\nГабариты: 25 x 25 x 25 мм\r\nВес: 25 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("124", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("124 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");

            VerifyAreEqual(
                "TestProduct26\r\nАртикул: 26\r\nЦвет: Color6\r\nРазмер: SizeName6\r\nГабариты: 26 x 26 x 26 мм\r\nВес: 26 кг",
                Driver.GetGridCell(1, "Name", "OrderItems").Text, "Name product at order 2");
            VerifyAreEqual("125", Driver.GetGridCell(1, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyAreEqual("125 руб.", Driver.GetGridCell(1, "Cost", "OrderItems").Text, " Cost product at order 2");

            VerifyFinally(TestName);
        }
    }
}