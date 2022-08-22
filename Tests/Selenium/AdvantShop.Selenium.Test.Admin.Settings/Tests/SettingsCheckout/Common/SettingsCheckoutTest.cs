using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsCheckout.Common
{
    [TestFixture]
    public class SettingsCheckoutTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders | ClearType.Shipping | ClearType.Payment);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingCheckout\\[Order].ShippingMethod.csv",
                "data\\Admin\\Settings\\SettingCheckout\\Payment\\[Order].PaymentMethod.csv",
                "data\\Admin\\Settings\\SettingCheckout\\Payment\\[Order].PaymentParam.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductGifts.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderContact.csv",
                "Data\\Admin\\Settings\\SettingCheckout\\[Order].OrderSource.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderStatus.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].[Order].csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].Certificate.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderCurrency.csv",
                "data\\Admin\\Settings\\SettingCheckout\\[Order].OrderItems.csv"
            );
            Init();
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Functions.CheckNotSelected("IsRequiredZip", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }
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
        public void ControlAmountLimitation()
        {
            GoToAdmin("settingscheckout");
            VerifyIsTrue(Driver.PageSource.Contains("Контролировать наличие товара при оформлении заказа"),
                "label AmountLimitation");

            if (!Driver.FindElement(By.Id("AmountLimitation")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AmountLimitation\"]")).Click();
            }

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("products/test-product1");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));
            GoToClient("cart");
            Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).SendKeys("5");
            Driver.DropFocus("h1");

            VerifyAreEqual("5",
                Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).GetAttribute("value"),
                "Amount change cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cart-full-buttons .btn-disabled")).Count == 1,
                " btn not enable");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".cart-full-error")).Displayed, "error masange");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".cart-amount-error")).Displayed, " error amount");
            VerifyAreEqual("доступно 1", Driver.FindElement(By.CssSelector(".cart-amount-error")).Text,
                "Amount avalible");
            VerifyAreEqual("Заказ содержит недоступное количество товаров.",
                Driver.FindElement(By.CssSelector(".cart-full-error")).Text, "error masange text");
        }

        [Test]
        public void ControlAmountLimitationNo()
        {
            GoToAdmin("settingscheckout");

            if (Driver.FindElement(By.Id("AmountLimitation")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"AmountLimitation\"]")).Click();
            }

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("products/test-product1");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");
            Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).SendKeys("5");
            Driver.DropFocus("h1");

            VerifyAreEqual("5",
                Driver.FindElement(By.CssSelector("[data-value=\"item.Amount\"] input")).GetAttribute("value"),
                "Amount change cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cart-full-buttons .btn-disabled")).Count == 0,
                " btn not enable");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cart-full-error")).Count == 0, "error masange");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".cart-amount-error")).Displayed, " error amount");

            Driver.FindElement(By.CssSelector(".icon-cancel-circled-before")).Click();
        }

        [Test]
        public void GoToPayment()
        {
            GoToAdmin("settingscheckout");

            if (!Driver.FindElement(By.Id("ProceedToPayment")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProceedToPayment\"]")).Click();
            }

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("cart");
            try
            {
                Driver.FindElement(By.CssSelector(".icon-cancel-circled-before")).Click();
                Thread.Sleep(100);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            //Driver.ScrollTo(By.Id("checkoutpayment"));
            Driver.ScrollTo(By.ClassName("payment-list"));
            Driver.XPathContainsText("span", "Пластиковая карта");
            Driver.ScrollTo(By.Id("CustomerComment"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".form-content"));
            VerifyIsTrue(Driver.Url.Contains("walletone.com/checkout"), " url payment");
        }

        [Test]
        public void GoToPaymentNo()
        {
            GoToAdmin("settingscheckout");

            if (Driver.FindElement(By.Id("ProceedToPayment")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ProceedToPayment\"]")).Click();
            }

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("cart");
            try
            {
                Driver.FindElement(By.CssSelector(".icon-cancel-circled-before")).Click();
                Thread.Sleep(100);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("checkout");
            Driver.WaitForElem(By.ClassName("checkout-block"));
            Driver.ScrollTo(By.Id("checkoutpayment"));
            Driver.XPathContainsText("span", "Пластиковая карта");
            Driver.ScrollTo(By.Id("CustomerComment"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), " url checkout");
            VerifyIsFalse(Driver.Url.Contains("walletone.com/checkout"), " url payment");
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");
        }

        [Test]
        public void LinkCustomerGroup()
        {
            GoToAdmin("settingscheckout");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"MinimalPriceCustomerGroups\"]")).GetAttribute("href")
                    .Contains("/settingscustomers"), "link");
            Driver.FindElement(By.CssSelector("[data-e2e=\"MinimalPriceCustomerGroups\"]")).Click();
            VerifyAreEqual("Группы покупателей", Driver.GetByE2E("GroupCustomerTitle").Text, "success");
        }

        [Test]
        public void MinSumOder()
        {
            GoToAdmin("settingscheckout");

            Driver.FindElement(By.Id("MinimalOrderPriceForDefaultGroup")).Clear();
            Driver.FindElement(By.Id("MinimalOrderPriceForDefaultGroup")).SendKeys("1000");

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("cart");
            try
            {
                Driver.FindElement(By.CssSelector(".icon-cancel-circled-before")).Click();
                Thread.Sleep(100);
            }
            catch
            {
            }

            GoToClient("products/test-product5");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cart-full-buttons .btn-disabled")).Count == 1,
                " btn not enable");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".cart-full-error")).Displayed, "error masange");
            VerifyAreEqual(
                "Минимальная сумма заказа: 1 000 руб. Вам необходимо приобрести еще товаров на сумму: 500 руб.",
                Driver.FindElement(By.CssSelector(".cart-full-error")).Text, "error masange text");

            GoToClient("products/test-product10");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cart-full-buttons .btn-disabled")).Count == 0,
                " btn not enable");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".cart-full-error")).Count == 0, "error masange");


            GoToAdmin("settingscheckout");
            Driver.FindElement(By.Id("MinimalOrderPriceForDefaultGroup")).Clear();
            Driver.FindElement(By.Id("MinimalOrderPriceForDefaultGroup")).SendKeys("100");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            Driver.WaitForToastSuccess();
        }

        [Test]
        public void NumberCart()
        {
            GoToAdmin("settingscheckout");

            if (!Driver.FindElement(By.Id("ShowClientId")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowClientId\"]")).Click();
            }

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".site-head-userid")).Displayed, " dispday cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".site-head-userid")).Text.Contains("Номер корзины:"),
                " number cart");
        }

        [Test]
        public void NumberCartNo()
        {
            GoToAdmin("settingscheckout");

            if (Driver.FindElement(By.Id("ShowClientId")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShowClientId\"]")).Click();
            }

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient();

            VerifyIsFalse(Driver.FindElements(By.CssSelector(".site-head-userid")).Count > 0, " dispday cart");
            VerifyIsFalse(Driver.PageSource.Contains("Номер корзины:"), " number cart");
        }

        [Test]
        public void PaymentAfterManagerConfirmed()
        {
            GoToAdmin("settingscheckout");

            if (!Driver.FindElement(By.Id("ManagerConfirmed")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ManagerConfirmed\"]")).Click();
            }

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("products/test-product10");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("checkout");
            //Driver.ScrollTo(By.Id("checkoutpayment"));
            Driver.ScrollTo(By.ClassName("payment-list"));
            Driver.WaitForElem(By.ClassName("payment-item"));
            Driver.XPathContainsText("span", "Пластиковая карта");

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            VerifyIsTrue(
                Driver.PageSource.Contains(
                    "После подтверждения заказа менеджером, Вам будет отправлена ссылка на оплату."), "contain text");

            GoToClient("myaccount#?tab=orderhistory");
            Driver.FindElement(By.CssSelector(".order-history-body-item-row")).Click();
            Thread.Sleep(100);
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-order-history-details]"))
                    .FindElements(By.CssSelector(".btn.btn-confirm.btn-middle")).Count > 0, "no btn");

            GoToAdmin("orders");
            Driver.GetGridCell(0, "StatusName").Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));

            VerifyIsTrue(Driver.PageSource.Contains("Заказ подтвержден, разрешить оплату"), "contain text in order");
            VerifyIsFalse(Driver.FindElement(By.Id("Order_ManagerConfirmed")).Selected, "no confirm in order");
            Driver.FindElement(By.CssSelector(".edit-text .adv-checkbox-emul")).Click();
            Driver.WaitForToastSuccess();
            //driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            //Thread.Sleep(2000);

            GoToClient("myaccount#?tab=orderhistory");
            Driver.FindElement(By.CssSelector(".order-history-body-item-row")).Click();
            Thread.Sleep(100);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-order-history-details]"))
                    .FindElement(By.CssSelector(".btn.btn-confirm.btn-middle")).Displayed, "yes btn");
        }

        [Test]
        public void PaymentNoManagerConfirmed()
        {
            GoToAdmin("settingscheckout");

            if (Driver.FindElement(By.Id("ManagerConfirmed")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ManagerConfirmed\"]")).Click();
            }

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("products/test-product10");
            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("checkout");
            //Driver.ScrollTo(By.Id("checkoutpayment"));
            Driver.ScrollTo(By.ClassName("payment-list"));
            Driver.WaitForElem(By.ClassName("payment-item"));
            Driver.XPathContainsText("span", "Пластиковая карта");

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            VerifyIsFalse(
                Driver.PageSource.Contains(
                    "После подтверждения заказа менеджером, Вам будет отправлена ссылка на оплату."), "contain text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".btn.btn-confirm.btn-middle")).Displayed, "btn payment");

            GoToClient("myaccount#?tab=orderhistory");
            Driver.FindElement(By.CssSelector(".order-history-body-item-row")).Click();
            Thread.Sleep(100);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-order-history-details]"))
                    .FindElement(By.CssSelector(".btn.btn-confirm.btn-middle")).Displayed, "yes btn");

            GoToAdmin("orders");
            Driver.GetGridCell(0, "StatusName").Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));

            VerifyIsFalse(Driver.PageSource.Contains("Заказ подтвержден, разрешить оплату"), "contain text in order");
            VerifyIsFalse(Driver.FindElements(By.Id("Order_ManagerConfirmed")).Count > 0, "no confirm in order");
        }

        [Test]
        public void PreOrder()
        {
            GoToAdmin("settingscheckout");

            (new SelectElement(Driver.FindElement(By.Id("OutOfStockAction")))).SelectByText("Создавать заказ");

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("products/test-product99");
            VerifyIsFalse(Driver.FindElements(By.XPath("//a[contains(text(), 'Под заказ')]")).Count > 0,
                "no btn preorder");
            VerifyIsTrue(Driver.FindElement(By.XPath("//div[contains(text(), 'Нет в наличии')]")).Displayed,
                "text  no preorder");

            GoToClient("products/test-product100");
            VerifyIsTrue(Driver.FindElement(By.XPath("//div[contains(text(), 'Нет в наличии')]")).Displayed,
                "text preorder");
            VerifyIsTrue(Driver.FindElement(By.XPath("//a[contains(text(), 'Под заказ')]")).Displayed, "btn preorder");
            Driver.XPathContainsText("a", "Под заказ");

            VerifyIsTrue(Driver.Url.Contains("preorder/100"), "url pre order");
            VerifyAreEqual("Оформление под заказ - TestProduct100", Driver.FindElement(By.TagName("h1")).Text,
                "h1 pre order");
            VerifyAreEqual("TestProduct100", Driver.FindElement(By.CssSelector(".h1")).Text, "h1 pre order product");

            Driver.FindElement(By.Id("firstName")).Clear();
            Driver.FindElement(By.Id("firstName")).SendKeys("TestName");
            Driver.FindElement(By.Id("lastName")).Clear();
            Driver.FindElement(By.Id("lastName")).SendKeys("lastName");

            Driver.FindElement(By.Id("email")).Click();
            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test@mail.mail");
            Driver.ScrollTo(By.Id("comment"));
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Driver.WaitForElem(By.ClassName("preorder-success"));
            VerifyAreEqual("Покупка товара под заказ", Driver.FindElement(By.TagName("h1")).Text, "pre order h1");
            VerifyIsTrue(
                Driver.PageSource.Contains(
                    "Благодарим за Вашу заявку! После её обработки наш менеджер сразу же свяжется с Вами и сообщит о возможности и сроках поступления данной позиции в наш интернет-магазин."),
                "pre order text");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "status");
            VerifyAreEqual("TestName lastName", Driver.GetGridCell(0, "BuyerName").Text, "name");
            VerifyAreEqual("10 000 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "sum");
            Driver.GetGridCell(0, "StatusName").Click();

            IWebElement selectCatalogView = Driver.FindElement(By.Id("Order_OrderSourceId"));
            SelectElement select = new SelectElement(selectCatalogView);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Под заказ"), "source order");
        }

        [Test]
        public void PreOrderLead()
        {
            GoToAdmin("settingscheckout");

            (new SelectElement(Driver.FindElement(By.Id("OutOfStockAction")))).SelectByText("Создавать лид");

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("products/test-product99");
            VerifyIsFalse(Driver.FindElements(By.XPath("//a[contains(text(), 'Под заказ')]")).Count > 0,
                " no btn preorder");
            VerifyIsTrue(Driver.FindElement(By.XPath("//div[contains(text(), 'Нет в наличии')]")).Displayed,
                "text  no preorder");

            GoToClient("products/test-product100");
            VerifyIsTrue(Driver.FindElement(By.XPath("//div[contains(text(), 'Нет в наличии')]")).Displayed,
                "text preorder");
            VerifyIsTrue(Driver.FindElement(By.XPath("//a[contains(text(), 'Под заказ')]")).Displayed, "btn preorder");
            Driver.XPathContainsText("a", "Под заказ");

            VerifyIsTrue(Driver.Url.Contains("preorder/100"), "url pre order");
            VerifyAreEqual("Оформление под заказ - TestProduct100", Driver.FindElement(By.TagName("h1")).Text,
                "h1 pre order");
            VerifyAreEqual("TestProduct100", Driver.FindElement(By.CssSelector(".h1")).Text, "h1 pre order product");

            Driver.FindElement(By.Id("firstName")).Clear();
            Driver.FindElement(By.Id("firstName")).SendKeys("TestNameLead");
            Driver.FindElement(By.Id("lastName")).Clear();
            Driver.FindElement(By.Id("lastName")).SendKeys("lastName");

            Driver.FindElement(By.Id("email")).Clear();
            Driver.FindElement(By.Id("email")).SendKeys("test@mail.mail");
            Driver.ScrollTo(By.Id("comment"));
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-submit")).Click();
            Driver.WaitForElem(By.ClassName("preorder-success"));
            VerifyAreEqual("Покупка товара под заказ", Driver.FindElement(By.TagName("h1")).Text, "pre order h1");
            VerifyIsTrue(
                Driver.PageSource.Contains(
                    "Благодарим за Вашу заявку! После её обработки наш менеджер сразу же свяжется с Вами и сообщит о возможности и сроках поступления данной позиции в наш интернет-магазин."),
                "pre order text");

            GoToAdmin("leads");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text, "status");
            VerifyAreEqual("10 000 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "sum");
            Driver.GetGridCell(0, "DealStatusName").Click();

            IWebElement selectCatalogView = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            SelectElement select = new SelectElement(selectCatalogView);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Под заказ"), "source lead");
        }

        [Test]
        public void PreOrderToCart()
        {
            Functions.CleanCart(Driver, BaseUrl);
            GoToAdmin("settingscheckout");
            (new SelectElement(Driver.FindElement(By.Id("OutOfStockAction")))).SelectByText(
                "Разрешить добавлять в корзину");

            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("products/test-product99");
            VerifyIsFalse(Driver.FindElements(By.XPath("//a[contains(text(), 'Под заказ')]")).Count > 0,
                "no btn preorder");
            VerifyIsTrue(Driver.FindElement(By.XPath("//div[contains(text(), 'Нет в наличии')]")).Displayed,
                "text  no preorder");
            VerifyIsTrue(Driver.FindElements(By.XPath("//a[contains(text(), 'Добавить')]")).Count > 0, " btn order");

            Driver.FindElement(By.CssSelector(".btn-confirm.icon-bag-before")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));

            GoToClient("cart");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-amount")).Text.Contains("Под заказ"),
                "text pre order");

            GoToClient("checkout");
            Driver.ScrollTo(By.Id("CustomerComment"));
            Driver.WaitForElemEnabled(AdvBy.DataE2E("btnCheckout"));
            Driver.FindElement(AdvBy.DataE2E("btnCheckout")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));
            VerifyAreEqual("Спасибо, ваш заказ оформлен!", Driver.FindElement(By.TagName("h1")).Text, "success");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, "status");
            VerifyAreEqual("9 900 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "sum");
        }

        [Test]
        public void TabSettingsCheckout()
        {
            GoToAdmin("settingscheckout");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"AmountLimitation\"]")).Displayed, "common tab");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"IsShowLastName\"]")).Displayed,
                "common 1 tab");

            //statuses
            Driver.FindElements(By.CssSelector(".nav-link"))[1].Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/settingscheckout#?checkoutTab=orderStatuses"), "url statuses");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridOrderStatuses\"]")).Displayed,
                "grid statuses displayed");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"AmountLimitation\"]")).Displayed,
                "tab not displayed in statuses");

            //checkoutfields
            Driver.FindElements(By.CssSelector(".nav-link"))[2].Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/settingscheckout#?checkoutTab=checkoutfields"), "url checkoutfields");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"IsShowLastName\"]")).Displayed,
                "checkoutfields tab");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[grid-unique-id=\"gridOrderStatuses\"]")).Count > 0,
                "grid not displayed in checkoutfields");

            //orderSources
            Driver.FindElements(By.CssSelector(".nav-link"))[3].Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/settingscheckout#?checkoutTab=orderSources"), "url orderSources");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridOrderSources\"]")).Displayed,
                "grid orderSources displayed");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"IsShowLastName\"]")).Displayed,
                "tab not displayed in orderSources");

            //taxes
            Driver.FindElements(By.CssSelector(".nav-link"))[4].Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/settingscheckout#?checkoutTab=taxes"), "url taxes");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTaxes\"]")).Displayed,
                "gridTaxes displayed");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[grid-unique-id=\"gridOrderSources\"]")).Count > 0,
                "grid not displayed in taxes");

            //script
            Driver.FindElements(By.CssSelector(".nav-link"))[5].Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/settingscheckout#?checkoutTab=scripts"), "url script");
            VerifyIsTrue(Driver.FindElement(By.TagName("ui-ace-textarea")).Displayed, "script tab");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[grid-unique-id=\"gridTaxes\"]")).Count > 0,
                "gridTaxes not displayed in script");

            //thankyoupage
            Driver.FindElements(By.CssSelector(".nav-link"))[6].Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/settingscheckout#?checkoutTab=thankyoupage"), "url thankyoupage");
            VerifyIsTrue(Driver.FindElement(By.Id("TYPageAction")).Displayed, "select displayed");
            VerifyIsFalse(Driver.FindElement(By.TagName("ui-ace-textarea")).Displayed,
                "tab not displayed in thankyoupage");

            //exportOrders
            Driver.FindElements(By.CssSelector(".nav-link"))[7].Click();
            Thread.Sleep(500);
            VerifyIsTrue(Driver.Url.Contains("/settingscheckout#?checkoutTab=exportOrders"), "url exportOrders");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Export\"]")).Displayed, "btn displayed");
            VerifyIsFalse(Driver.FindElement(By.Id("TYPageAction")).Displayed, "select not displayed in exportOrders");
        }
    }
}