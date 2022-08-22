using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Services.servicesThreeColumns.Button.CheckoutUpsell
{
    [TestFixture]
    public class LandingServicesUpsell : LandingsFunctions
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
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CrossSell\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CrossSell\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CrossSell\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CrossSell\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CrossSell\\CMS.LandingSite_Product.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CrossSell\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CrossSell\\CMS.LandingSubBlock.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CrossSell\\CMS.LandingForm.csv",
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

            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

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
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/cross-testproduct1?code="), "url success checkout Upsell");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode Upsell");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout Upsell");
            VerifyAreEqual("Upsell page", Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text,
                "h1 on page Upsell page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButton2Block\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/cross-testproduct1/down?code="),
                "url success checkout Downsell");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode Downsell");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout Downsell");
            VerifyAreEqual("Downsell page", Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text,
                "h1 on page Downsell page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButton2Block\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode");
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
        public void ServicesFormAddOneDownsell()
        {
            TestName = "ServicesFormAddOneDownsell";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Переход на оплату");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

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
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/cross-testproduct1?code="), "url success checkout Upsell");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode Upsell");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout Upsell");
            VerifyAreEqual("Upsell page", Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text,
                "h1 on page Upsell page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButton2Block\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/cross-testproduct1/down?code="),
                "url success checkout Downsell");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode Downsell");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout Downsell");
            VerifyAreEqual("Downsell page", Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text,
                "h1 on page Downsell page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButton\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 on page success checkout");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "StatusName ");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, "BuyerName ");
            VerifyAreEqual("213 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
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
                "TestProduct14\r\nАртикул: 14\r\nЦвет: Color4\r\nРазмер: SizeName4\r\nГабариты: 14 x 14 x 14 мм\r\nВес: 14 кг",
                Driver.GetGridCell(1, "Name", "OrderItems").Text, "Name product at order");
            VerifyAreEqual("113", Driver.GetGridCell(1, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyAreEqual("113 руб.", Driver.GetGridCell(1, "Cost", "OrderItems").Text, " Cost product at order");

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
        public void ServicesFormAddOneUpsell()
        {
            TestName = "ServicesFormAddOneUpsell";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Переход на оплату");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

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
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/cross-testproduct1?code="), "url success checkout Upsell");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode Upsell");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout Upsell");
            VerifyAreEqual("Upsell page", Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text,
                "h1 on page Upsell page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButton\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 on page success checkout");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "StatusName ");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, "BuyerName ");
            VerifyAreEqual("214 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
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
                "TestProduct15\r\nАртикул: 15\r\nЦвет: Color5\r\nРазмер: SizeName5\r\nГабариты: 15 x 15 x 15 мм\r\nВес: 15 кг",
                Driver.GetGridCell(1, "Name", "OrderItems").Text, "Name product at order 2");
            VerifyAreEqual("114", Driver.GetGridCell(1, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyAreEqual("114 руб.", Driver.GetGridCell(1, "Cost", "OrderItems").Text, " Cost product at order 2");

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
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/cross-testproduct1?code="), "url success checkout Upsell");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode Upsell");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout Upsell");
            VerifyAreEqual("Upsell page", Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text,
                "h1 on page Upsell page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButton2Block\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/cross-testproduct1/down?code="),
                "url success checkout Downsell");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode Downsell");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout Downsell");
            VerifyAreEqual("Downsell page", Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text,
                "h1 on page Downsell page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButton2Block\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode");
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
        public void ServicesFormAddSomeDownsell()
        {
            TestName = "ServicesFormAddSomeDownsell";
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
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/cross-testproduct1?code="), "url success checkout Upsell");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode Upsell");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout Upsell");
            VerifyAreEqual("Upsell page", Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text,
                "h1 on page Upsell page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButton2Block\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/cross-testproduct1/down?code="),
                "url success checkout Downsell");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode Downsell");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout Downsell");
            VerifyAreEqual("Downsell page", Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text,
                "h1 on page Downsell page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButton\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 on page success checkout");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "StatusName ");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, "BuyerName ");
            VerifyAreEqual("637 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
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

            VerifyAreEqual(
                "TestProduct14\r\nАртикул: 14\r\nЦвет: Color4\r\nРазмер: SizeName4\r\nГабариты: 14 x 14 x 14 мм\r\nВес: 14 кг",
                Driver.GetGridCell(5, "Name", "OrderItems").Text, "Name product at order 5");
            VerifyAreEqual("113", Driver.GetGridCell(5, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 5");
            VerifyAreEqual("1", Driver.GetGridCell(5, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 5");
            VerifyAreEqual("113 руб.", Driver.GetGridCell(5, "Cost", "OrderItems").Text, " Cost product at order 5");


            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSomeUpsell()
        {
            TestName = "ServicesFormAddSomeUpsell";
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
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/cross-testproduct1?code="), "url success checkout Upsell");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode Upsell");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout Upsell");
            VerifyAreEqual("Upsell page", Driver.FindElement(By.CssSelector("[data-e2e=\"TitleBlock\"]")).Text,
                "h1 on page Upsell page");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormButton\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/checkout/success"), "url success checkout");
            VerifyIsTrue(Driver.Url.Contains("mode=lp"), "url success checkout lp mode");
            VerifyIsFalse(Is404Page(Driver.Url), "not 404 page checkout");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text,
                "h1 on page success checkout");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "StatusName ");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, "BuyerName ");
            VerifyAreEqual("638 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
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

            VerifyAreEqual(
                "TestProduct15\r\nАртикул: 15\r\nЦвет: Color5\r\nРазмер: SizeName5\r\nГабариты: 15 x 15 x 15 мм\r\nВес: 15 кг",
                Driver.GetGridCell(5, "Name", "OrderItems").Text, "Name product at order 5");
            VerifyAreEqual("114", Driver.GetGridCell(5, "Price", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 5");
            VerifyAreEqual("1", Driver.GetGridCell(5, "Amount", "OrderItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 5");
            VerifyAreEqual("114 руб.", Driver.GetGridCell(5, "Cost", "OrderItems").Text, " Cost product at order 5");

            VerifyFinally(TestName);
        }
    }
}