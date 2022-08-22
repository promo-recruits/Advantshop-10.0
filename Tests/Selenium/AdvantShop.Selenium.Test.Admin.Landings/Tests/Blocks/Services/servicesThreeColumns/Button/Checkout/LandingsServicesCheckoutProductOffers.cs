using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Services.servicesThreeColumns.Button.Checkout
{
    [TestClass]
    public class LandingsServicesCheckoutProductOffers : LandingsFunctions
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
            Driver.GridFilterSendKeys("11");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_11\"]")).Displayed, "display product");

            //change offer and count
            FormProdOfferPrice("1000");
            FormProdCount("4");
            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            VerifyAreEqual("TestProduct11", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 4", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Цвет: Color1"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Размер: SizeName1"),
                "size on checkout page");
            VerifyAreEqual("1 000 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");
            VerifyAreEqual("4 000 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("4 000 руб.",
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
            VerifyAreEqual("4 000 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct11\r\nАртикул: 11\r\nЦвет: Color1\r\nРазмер: SizeName1\r\nГабариты: 11 x 11 x 11 мм\r\nВес: 11 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("4", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("4 000 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(0, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text");

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
        public void ServicesFormAddOneAmount()
        {
            TestName = "ServicesFormAddOneAmount";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Переход на оплату");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            //check grid
            VerifyAreEqual("1", Driver.GetGridCell(0, "ArtNo", "OffersSelectvizr").Text, "ArtNo product at grid");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product");

            //change offer and count
            FormProdOfferPrice("999");
            FormProdCount("5");
            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            VerifyAreEqual("TestProduct1", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 5", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Цвет: Color1"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Размер: SizeName1"),
                "size on checkout page");
            VerifyAreEqual("999 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");
            VerifyAreEqual("4 995 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("4 995 руб.",
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
            VerifyAreEqual("4 995 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1\r\nдоступно ещё 1\r\nГабариты: 1 x 1 x 1 мм\r\nВес: 1 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("999", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("5", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("4 995 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(0, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text");

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
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            //check grid
            VerifyAreEqual("1", Driver.GetGridCell(0, "ArtNo", "OffersSelectvizr").Text, "ArtNo product at grid");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid");
            Driver.GridFilterSendKeys("TestProduct3");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_3\"]")).Displayed, "display product");

            //change offer and count
            FormProdOfferPrice("2000");
            FormProdCount("3");
            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            VerifyAreEqual("TestProduct3", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 3", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Цвет: Color3"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Размер: SizeName3"),
                "size on checkout page");
            VerifyAreEqual("2 000 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");
            VerifyAreEqual("6 000 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("6 000 руб.",
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
            VerifyAreEqual("6 000 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct3\r\nАртикул: 3\r\nЦвет: Color3\r\nРазмер: SizeName3\r\nГабариты: 3 x 3 x 3 мм\r\nВес: 3 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("2000", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("3", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("6 000 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Name", "OrderItems").FindElements(By.CssSelector(".order-item-not-enabled")).Count == 1,
                " Available product at order");
            VerifyAreEqual("rgba(160, 160, 160, 1)",
                Driver.GetGridCell(0, "Name", "OrderItems").FindElement(By.CssSelector(".order-item-not-enabled"))
                    .GetCssValue("color"), " Available product at order text");

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
            Thread.Sleep(2000);

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_4\"]")).Displayed, "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_10\"]")).Displayed,
                "display product 3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 4");
            Thread.Sleep(1000);

            FormProdOfferPrice("1000");
            FormProdCount("10");

            FormProdCount("2", 1);

            FormProdOfferPrice("3000", 2);

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
            VerifyAreEqual("Количество: 10", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Цвет: Color1"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Размер: SizeName1"),
                "size on checkout page");
            VerifyAreEqual("1 000 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");

            //product 2
            VerifyAreEqual("TestProduct4", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[1].Text,
                "product on checkout page 2");
            VerifyAreEqual("Количество: 2", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[1].Text,
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
            VerifyAreEqual("3 000 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[2].Text,
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

            VerifyAreEqual("13 317 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("13 317 руб.",
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
            VerifyAreEqual("13 317 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1\r\nдоступно ещё 1\r\nГабариты: 1 x 1 x 1 мм\r\nВес: 1 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("10", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("10 000 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(0, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text");

            VerifyAreEqual(
                "TestProduct4\r\nАртикул: 4\r\nЦвет: Color4\r\nРазмер: SizeName4\r\nГабариты: 4 x 4 x 4 мм\r\nВес: 4 кг",
                Driver.GetGridCell(1, "Name", "OrderItems").Text, "Name product at order 2");
            VerifyAreEqual("103", Driver.GetGridCell(1, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("2", Driver.GetGridCell(1, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyAreEqual("206 руб.", Driver.GetGridCell(1, "Cost", "OrderItems").Text, " Cost product at order 2");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(1, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text 2");

            VerifyAreEqual(
                "TestProduct10\r\nАртикул: 10\r\nЦвет: Color10\r\nРазмер: SizeName10\r\nГабариты: 10 x 10 x 10 мм\r\nВес: 10 кг",
                Driver.GetGridCell(2, "Name", "OrderItems").Text, "Name product at order 3");
            VerifyAreEqual("3000", Driver.GetGridCell(2, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 3");
            VerifyAreEqual("1", Driver.GetGridCell(2, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 3");
            VerifyAreEqual("3 000 руб.", Driver.GetGridCell(2, "Cost", "OrderItems").Text, " Cost product at order 3");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(2, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text 3");

            VerifyAreEqual(
                "TestProduct10\r\nАртикул: 103\r\nЦвет: Color2\r\nРазмер: SizeName2\r\nГабариты: 2 x 2 x 2 мм\r\nВес: 2 кг",
                Driver.GetGridCell(3, "Name", "OrderItems").Text, "Name product at order 4");
            VerifyAreEqual("111", Driver.GetGridCell(3, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 4");
            VerifyAreEqual("1", Driver.GetGridCell(3, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 4");
            VerifyAreEqual("111 руб.", Driver.GetGridCell(3, "Cost", "OrderItems").Text, " Cost product at order 4");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(3, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text 4");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSomeEdit()
        {
            TestName = "ServicesFormAddSomeEdit";
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

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_4\"]")).Displayed, "display product 2");
            Thread.Sleep(1000);

            FormProdOfferPrice("1000");
            FormProdCount("10");
            FormProdCount("2", 1);
            BlockSettingsSave();
            Thread.Sleep(1000);
            Refresh();

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed,
                "display product 1 refresh");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_4\"]")).Displayed,
                "display product 2 refresh");
            VerifyAreEqual("1000",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NewPriceProduct\"]")).GetAttribute("value"),
                "new price");
            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input")).GetAttribute("value"),
                "new amount");
            VerifyAreEqual("",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewPriceProduct\"]"))[1].GetAttribute("value"),
                "new price 2");
            VerifyAreEqual("2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input"))[1].GetAttribute("value"),
                "new amount 2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);
            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed,
                "display product 1 new");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_4\"]")).Displayed,
                "display product 2 new");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_10\"]")).Displayed,
                "display product 3 new");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 4 new");
            VerifyAreEqual("1000",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NewPriceProduct\"]")).GetAttribute("value"),
                "new price after add");
            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input")).GetAttribute("value"),
                "new amount after add");
            VerifyAreEqual("",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewPriceProduct\"]"))[1].GetAttribute("value"),
                "new price 2 after add");
            VerifyAreEqual("2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input"))[1].GetAttribute("value"),
                "new amount 2 after add");
            VerifyAreEqual("",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewPriceProduct\"]"))[2].GetAttribute("value"),
                "new price 3 after add");
            VerifyAreEqual("1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input"))[2].GetAttribute("value"),
                "new amount 3 after add");
            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            //product 1
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 10", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Цвет: Color1"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Размер: SizeName1"),
                "size on checkout page");
            VerifyAreEqual("1 000 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");

            //product 2
            VerifyAreEqual("TestProduct4", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[1].Text,
                "product on checkout page 2");
            VerifyAreEqual("Количество: 2", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[1].Text,
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

            VerifyAreEqual("10 426 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("10 426 руб.",
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
            VerifyAreEqual("10 426 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1\r\nдоступно ещё 1\r\nГабариты: 1 x 1 x 1 мм\r\nВес: 1 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("10", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("10 000 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(0, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text");

            VerifyAreEqual(
                "TestProduct4\r\nАртикул: 4\r\nЦвет: Color4\r\nРазмер: SizeName4\r\nГабариты: 4 x 4 x 4 мм\r\nВес: 4 кг",
                Driver.GetGridCell(1, "Name", "OrderItems").Text, "Name product at order 2");
            VerifyAreEqual("103", Driver.GetGridCell(1, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("2", Driver.GetGridCell(1, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyAreEqual("206 руб.", Driver.GetGridCell(1, "Cost", "OrderItems").Text, " Cost product at order 2");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(1, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text 2");

            VerifyAreEqual(
                "TestProduct10\r\nАртикул: 10\r\nЦвет: Color10\r\nРазмер: SizeName10\r\nГабариты: 10 x 10 x 10 мм\r\nВес: 10 кг",
                Driver.GetGridCell(2, "Name", "OrderItems").Text, "Name product at order 3");
            VerifyAreEqual("109", Driver.GetGridCell(2, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 3");
            VerifyAreEqual("1", Driver.GetGridCell(2, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 3");
            VerifyAreEqual("109 руб.", Driver.GetGridCell(2, "Cost", "OrderItems").Text, " Cost product at order 3");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(2, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text 3");

            VerifyAreEqual(
                "TestProduct10\r\nАртикул: 103\r\nЦвет: Color2\r\nРазмер: SizeName2\r\nГабариты: 2 x 2 x 2 мм\r\nВес: 2 кг",
                Driver.GetGridCell(3, "Name", "OrderItems").Text, "Name product at order 4");
            VerifyAreEqual("111", Driver.GetGridCell(3, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 4");
            VerifyAreEqual("1", Driver.GetGridCell(3, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 4");
            VerifyAreEqual("111 руб.", Driver.GetGridCell(3, "Cost", "OrderItems").Text, " Cost product at order 4");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(3, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text 4");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSomePrice()
        {
            TestName = "ServicesFormAddSomePrice";
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
            Thread.Sleep(2000);

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_4\"]")).Displayed, "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_10\"]")).Displayed,
                "display product 3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 4");
            Thread.Sleep(1000);

            FormProdOfferPrice("1000");
            FormProdCount("10");

            FormProdCount("2", 1);

            FormProdOfferPrice("3000", 2);

            BlockSettingsSave();
            Thread.Sleep(1000);

            ChangePriceAdmin(1, "4000");
            ChangePriceAdmin(4, "3000");
            ChangePriceAdmin(10, "2000");
            ChangePriceAdmin(10, "1000", 1);

            GoToClient("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(BaseUrl + "/checkout/lp?lpid=1", Driver.Url, "url checkout");
            VerifyIsFalse(Is404Page(BaseUrl + "/checkout/lp?lpid=1"), "not 404 page");
            VerifyAreEqual("Оформление заказа", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            //product 1
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.CssSelector(".checkout-cart-name a")).Text,
                "product on checkout page");
            VerifyAreEqual("Количество: 10", Driver.FindElement(By.CssSelector(".checkout-cart-item-count")).Text,
                "count on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Цвет: Color1"),
                "color on checkout page");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-item-properties")).Text.Contains("Размер: SizeName1"),
                "size on checkout page");
            VerifyAreEqual("1 000 руб.", Driver.FindElement(By.CssSelector(".checkout-cart-price")).Text,
                "price on checkout page");

            //product 2
            VerifyAreEqual("TestProduct4", Driver.FindElements(By.CssSelector(".checkout-cart-name a"))[1].Text,
                "product on checkout page 2");
            VerifyAreEqual("Количество: 2", Driver.FindElements(By.CssSelector(".checkout-cart-item-count"))[1].Text,
                "count on checkout page 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[1].Text.Contains("Цвет: Color4"),
                "color on checkout page 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".checkout-cart-item-properties"))[1].Text
                    .Contains("Размер: SizeName4"), "size on checkout page 2");
            VerifyAreEqual("3 000 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[1].Text,
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
            VerifyAreEqual("3 000 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[2].Text,
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
            VerifyAreEqual("1 000 руб.", Driver.FindElements(By.CssSelector(".checkout-cart-price"))[3].Text,
                "price on checkout page 4");

            VerifyAreEqual("20 000 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Cost\"]")).Text,
                "price order on checkout page");
            VerifyAreEqual("Бесплатно",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                "price delivery on checkout page");
            VerifyAreEqual("20 000 руб.",
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
            VerifyAreEqual("20 000 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Number").Click();
            Thread.Sleep(1000);

            //check product grid
            VerifyAreEqual(
                "TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1\r\nдоступно ещё 1\r\nГабариты: 1 x 1 x 1 мм\r\nВес: 1 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("10", Driver.GetGridCell(0, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("10 000 руб.", Driver.GetGridCell(0, "Cost", "OrderItems").Text, " Cost product at order");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(0, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text");

            VerifyAreEqual(
                "TestProduct4\r\nАртикул: 4\r\nЦвет: Color4\r\nРазмер: SizeName4\r\nГабариты: 4 x 4 x 4 мм\r\nВес: 4 кг",
                Driver.GetGridCell(1, "Name", "OrderItems").Text, "Name product at order 2");
            VerifyAreEqual("3000", Driver.GetGridCell(1, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("2", Driver.GetGridCell(1, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyAreEqual("6 000 руб.", Driver.GetGridCell(1, "Cost", "OrderItems").Text, " Cost product at order 2");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(1, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text 2");

            VerifyAreEqual(
                "TestProduct10\r\nАртикул: 10\r\nЦвет: Color10\r\nРазмер: SizeName10\r\nГабариты: 10 x 10 x 10 мм\r\nВес: 10 кг",
                Driver.GetGridCell(2, "Name", "OrderItems").Text, "Name product at order 3");
            VerifyAreEqual("3000", Driver.GetGridCell(2, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 3");
            VerifyAreEqual("1", Driver.GetGridCell(2, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 3");
            VerifyAreEqual("3 000 руб.", Driver.GetGridCell(2, "Cost", "OrderItems").Text, " Cost product at order 3");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(2, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text 3");

            VerifyAreEqual(
                "TestProduct10\r\nАртикул: 103\r\nЦвет: Color2\r\nРазмер: SizeName2\r\nГабариты: 2 x 2 x 2 мм\r\nВес: 2 кг",
                Driver.GetGridCell(3, "Name", "OrderItems").Text, "Name product at order 4");
            VerifyAreEqual("1000", Driver.GetGridCell(3, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 4");
            VerifyAreEqual("1", Driver.GetGridCell(3, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 4");
            VerifyAreEqual("1 000 руб.", Driver.GetGridCell(3, "Cost", "OrderItems").Text, " Cost product at order 4");
            VerifyAreEqual("rgba(45, 156, 238, 1)",
                Driver.GetGridCell(3, "Name", "OrderItems").FindElement(By.CssSelector("a")).GetCssValue("color"),
                " Available product at order text 4");

            VerifyFinally(TestName);
        }
    }
}