using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Client.Tests.Desktop.MyAccount
{
    [TestFixture]
    public class ClientMyAccountOrderHistoryTests : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Orders | ClearType.Payment |
                                        ClearType.Shipping);
            InitializeService.LoadData(
                "data\\Client\\MyAccount\\Catalog.Photo.csv",
                "data\\Client\\MyAccount\\Catalog.Color.csv",
                "data\\Client\\MyAccount\\Catalog.Size.csv",
                "data\\Client\\MyAccount\\Catalog.Category.csv",
                "data\\Client\\MyAccount\\Customers.Customer.csv",
                "data\\Client\\MyAccount\\Customers.CustomerGroup.csv",
                "data\\Client\\MyAccount\\Customers.Contact.csv",
                "data\\Client\\MyAccount\\Catalog.Brand.csv",
                "data\\Client\\MyAccount\\Catalog.Product.csv",
                "data\\Client\\MyAccount\\Catalog.ProductCategories.csv",
                "data\\Client\\MyAccount\\Catalog.Offer.csv",
                "data\\Client\\MyAccount\\Catalog.Property.csv",
                "data\\Client\\MyAccount\\Catalog.PropertyValue.csv",
                "data\\Client\\MyAccount\\Catalog.ProductPropertyValue.csv",
                "data\\Client\\MyAccount\\Catalog.PropertyGroup.csv",
                "data\\Client\\MyAccount\\Customers.Managers.csv",
                "data\\Client\\MyAccount\\[Order].OrderStatus.csv",
                "data\\Client\\MyAccount\\[Order].ShippingMethod.csv",
                "data\\Client\\MyAccount\\[Order].PaymentMethod.csv",
                "data\\Client\\MyAccount\\[Order].[Order].csv",
                "data\\Client\\MyAccount\\[Order].OrderCustomer.csv",
                "data\\Client\\MyAccount\\[Order].OrderSource.csv",
                "data\\Client\\MyAccount\\[Order].OrderContact.csv",
                "data\\Client\\MyAccount\\[Order].OrderCurrency.csv",
                "data\\Client\\MyAccount\\[Order].OrderItems.csv"
            );

            Init();
            EnableInplaceOff();
            ReInitClient();
            Functions.LogCustomer(Driver, BaseUrl, "cfc2c33b-1e84-415e-8482-e98156341604",
                "D89AECD7F7EA88C7F48771E0B84EA4EFE152CDF1711095D9AB6EFB0E1600E2DCA6BFB7654ED2D93D4DB862739A62926FA2B4B956CCC2EF291DECB1B7C1AE4BA162A86398BA91E2838EF0498AE0EC23E50F2416F734012F0A5841DEA63F6441F02F4BE53305DFEB3FAA92BE966367C327C79FAFCB871C243C3BFC75948319C2C9952365F7C2768EE42458B111A35C03BDE2BCE19C");
            GoToClient("myaccount?tab=orderhistory");
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
        public void _00_OrdersOpened()
        {
            VerifyAreEqual("Личный кабинет", Driver.FindElement(By.TagName("h1")).Text, "my account h1");
            VerifyAreEqual("rgba(6, 98, 193, 1)",
                Driver.FindElement(By.Id("orderhistory")).GetCssValue("background-color"), "tab selected");
            VerifyAreEqual(102, Driver.FindElements(By.CssSelector(".order-history-body-item-row")).Count,
                "orders list");

            VerifyAreEqual("№ 100 31 августа 2016 23:00",
                Driver.FindElements(By.CssSelector(".order-history-body-item-row"))[0]
                    .FindElements(By.CssSelector(".order-history-body-item"))[0].Text, "order id, date and time");
            VerifyAreEqual("Отправлен",
                Driver.FindElements(By.CssSelector(".order-history-body-item-row"))[0]
                    .FindElements(By.CssSelector(".order-history-body-item"))[1].Text, "order status");
            VerifyAreEqual("Курьером по Москве",
                Driver.FindElements(By.CssSelector(".order-history-body-item-row"))[0]
                    .FindElements(By.CssSelector(".order-history-body-item"))[2].Text, "order shipping");
            VerifyAreEqual("Терминалы оплаты",
                Driver.FindElements(By.CssSelector(".order-history-body-item-row"))[0]
                    .FindElements(By.CssSelector(".order-history-body-item"))[3].Text, "order payment");
            VerifyAreEqual("Нет",
                Driver.FindElements(By.CssSelector(".order-history-body-item-row"))[0]
                    .FindElements(By.CssSelector(".order-history-body-item"))[4].Text, "order payment status");
            VerifyAreEqual("10 000 руб.",
                Driver.FindElements(By.CssSelector(".order-history-body-item-row"))[0]
                    .FindElements(By.CssSelector(".order-history-body-item"))[5].Text, "order sum");

            VerifyIsFalse(Driver.PageSource.Contains("Перейти в каталог"), "no go to castalog button");
        }

        [Test]
        public void _01_OrdersDetails()
        {
            Driver.FindElements(By.CssSelector(".order-history-body-item-row"))[0].Click();
            Thread.Sleep(2000);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".order-history-details-header")).GetAttribute("innerText")
                    .Contains("№100 Отправлен"), "orders details opened");
            VerifyIsTrue(Driver.PageSource.Contains("Name Customer"), "customer name");
            VerifyIsTrue(Driver.PageSource.Contains("Россия"), "customer country");
            VerifyIsTrue(Driver.PageSource.Contains("Московская область"), "customer region");
            VerifyIsTrue(Driver.PageSource.Contains("Москва"), "customer city");
            VerifyIsTrue(Driver.PageSource.Contains("Test Street д. 1"), "customer address");
            VerifyIsTrue(Driver.PageSource.Contains("111111"), "customer zip");
            VerifyIsTrue(Driver.PageSource.Contains("Курьером по Москве"), "customer shipping method");
            VerifyIsTrue(Driver.PageSource.Contains("Терминалы оплаты"), "customer payment method");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-product")).Text
                    .Contains("TestProduct100"), "order product name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-price")).Text.Contains("100 руб."),
                "order product price");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-amount")).Text.Contains("100"),
                "order product amount");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-cost")).Text.Contains("10 000 руб."),
                "order product cost");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".order-history-details-result-row"))[0].GetAttribute("outerText")
                    .Contains("Стоимость заказа:	10 000 руб."), "order result cost");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".order-history-details-result-row"))[1].GetAttribute("outerText")
                    .Contains("Стоимость доставки:	+ 100 руб."), "order result shipping cost");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".order-history-details-result-row"))[2].GetAttribute("outerText")
                    .Contains("Не указано:	0 руб."), "order result finally");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".order-history-details-result-row"))[3].GetAttribute("outerText")
                    .Contains("Итого:	10 000 руб."), "order result finally");

            //check go back to orders list
            Driver.FindElement(By.LinkText("Вернуться к списку заказов")).Click();
            Thread.Sleep(2000);

            VerifyAreEqual(102, Driver.FindElements(By.CssSelector(".order-history-body-item-row")).Count,
                "back to orders list");
        }

        [Test]
        public void _02_OrderCancel()
        {
            Driver.FindElements(By.CssSelector(".order-history-body-item-row"))[0].Click();
            Thread.Sleep(2000);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".order-history-details-header")).GetAttribute("innerText")
                    .Contains("№100 Отправлен"), "order id and status");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("a.btn.btn-confirm.btn-middle")).Text.Contains("Оплатить"),
                "payment button text");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("a.btn.btn-confirm.btn-middle")).Enabled,
                "payment button enabled");
            VerifyIsFalse(Driver.PageSource.Contains("Отменить"), "no cancel btn");

            Driver.FindElement(By.LinkText("Вернуться к списку заказов")).Click();

            int i = 3;
            Driver.ScrollTo(By.ClassName("order-history-body-item-row"), 15);
            Driver.FindElements(By.CssSelector(".order-history-body-item-row"))[15].Click();
            Thread.Sleep(2000);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".order-history-details-header")).GetAttribute("innerText")
                    .Contains("№83 В обработке"), "order2 id and status");
            VerifyIsFalse(Driver.PageSource.Contains("Оплатить"), "contain payment btn");
            VerifyIsTrue(Driver.PageSource.Contains("Отменить"), "no cancel btn2");

            Driver.FindElement(By.LinkText("Вернуться к списку заказов")).Click();

            Driver.ScrollTo(By.ClassName("order-history-body-item-row"), 97);
            Driver.FindElements(By.CssSelector(".order-history-body-item-row"))[97].Click();
            Thread.Sleep(2000);

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".order-history-details-header")).GetAttribute("innerText")
                    .Contains("№5 Новый"), "order2 id and status");
            VerifyIsFalse(Driver.PageSource.Contains("Оплатить"), "contain payment btn");
            VerifyIsTrue(Driver.PageSource.Contains("Отменить"), "no cancel btn2");
            /* Driver.XPathContainsText("a", "Отменить заказ");
             Thread.Sleep(2000);
             driver.FindElement(By.CssSelector(".swal2-confirm.btn")).Click();
             Thread.Sleep(2000);

             VerifyIsTrue(driver.FindElement(By.CssSelector(".order-history-details-header")).GetAttribute("innerText").Contains("№100 Отменён"), "order canceled");

             //after refresh
             GoToClient("myaccount?tab=orderhistory");
             driver.FindElements(By.CssSelector(".order-history-body-item-row"))[0].Click();
             Thread.Sleep(2000);
             VerifyIsTrue(driver.FindElement(By.CssSelector(".order-history-details-header")).GetAttribute("innerText").Contains("№100 Отменён"), "order canceled after refresh");
             */
        }

        [Test]
        public void _03_OrderGoToProduct()
        {
            GoToClient("myaccount?tab=orderhistory");
            Driver.FindElements(By.CssSelector(".order-history-body-item-row"))[0].Click();
            Thread.Sleep(2000);
            Driver.WaitForElem(By.CssSelector(".cart-full-name-link"));
            Driver.ScrollTo(By.CssSelector(".order-history-details-info"));
            Driver.FindElement(By.CssSelector(".cart-full-name-link")).Click();
            Driver.WaitForElem(By.ClassName("details-block"));

            VerifyIsTrue(Driver.Url.Contains("products/test-product100"), "product url");
            VerifyIsFalse(Is404Page("products/test-product100"), "not 404 error");
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("TestProduct100"), "product h1");
        }

        [Test]
        public void OrdersNoGoToCatalog()
        {
            ReInitClient();
            Functions.LogCustomer(Driver, BaseUrl, "CFC2C33B-1E84-415E-8482-E98156341605",
                "801620E21A1C6644012F041ABEBCB2D4379402108A133E902A9D7F4BE65B641FD195DC7EBCC1AED845D876C8A884B9108303CE26727BA27D6C7D7EE17249ABBEAC29AC3D5C1D0AE66D81BC29B2E7D2030C30FDE9A1B0AB511A28AE38EE7B6E38443AC0E3BA0FF9584E2FAA59677EF4EEB989D7B22E51F18FC940B92DD9626B2BF6EF33FAE4E0C6658E650CCFFA2C08AE99D2F2A2");

            GoToClient("myaccount?tab=orderhistory");

            VerifyIsTrue(Driver.PageSource.Contains("Вы не совершали покупок в нашем магазине"), "no orders text");
            VerifyAreEqual("Перейти в каталог", Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action")).Text,
                "go to catalog button text");
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Каталог", Driver.FindElement(By.TagName("h1")).Text, "catalog h1 my account");
            VerifyIsTrue(Driver.Url.Contains("catalog"), "catalog url from my account");
        }
    }

    [TestFixture]
    public class ClientMyAccountGeneralInfoTests : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Client\\MyAccount\\Catalog.Photo.csv",
                "data\\Client\\MyAccount\\Catalog.Color.csv",
                "data\\Client\\MyAccount\\Catalog.Size.csv",
                "data\\Client\\MyAccount\\Catalog.Category.csv",
                "data\\Client\\MyAccount\\Customers.Customer.csv",
                "data\\Client\\MyAccount\\Customers.CustomerGroup.csv",
                "data\\Client\\MyAccount\\Customers.Contact.csv",
                "data\\Client\\MyAccount\\Catalog.Brand.csv",
                "data\\Client\\MyAccount\\Catalog.Product.csv",
                "data\\Client\\MyAccount\\Catalog.ProductCategories.csv",
                "data\\Client\\MyAccount\\Catalog.Offer.csv",
                "data\\Client\\MyAccount\\Catalog.Property.csv",
                "data\\Client\\MyAccount\\Catalog.PropertyValue.csv",
                "data\\Client\\MyAccount\\Catalog.ProductPropertyValue.csv",
                "data\\Client\\MyAccount\\Catalog.PropertyGroup.csv",
                "data\\Client\\MyAccount\\Customers.Managers.csv",
                "data\\Client\\MyAccount\\[Order].OrderStatus.csv",
                "data\\Client\\MyAccount\\[Order].ShippingMethod.csv",
                "data\\Client\\MyAccount\\[Order].PaymentMethod.csv",
                "data\\Client\\MyAccount\\[Order].[Order].csv",
                "data\\Client\\MyAccount\\[Order].OrderCustomer.csv",
                "data\\Client\\MyAccount\\[Order].OrderSource.csv",
                "data\\Client\\MyAccount\\[Order].OrderContact.csv",
                "data\\Client\\MyAccount\\[Order].OrderCurrency.csv",
                "data\\Client\\MyAccount\\[Order].OrderItems.csv"
            );

            Init();
            EnableInplaceOff();
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
        public void ChangeGeneralInfo()
        {
            GoToClient("myaccount?tab=commoninf");

            Driver.FindElement(By.Id("FirstName")).Click();
            Driver.FindElement(By.Id("FirstName")).Clear();
            Driver.FindElement(By.Id("FirstName")).SendKeys("Boss");
            Driver.FindElement(By.Id("LastName")).Click();
            Driver.FindElement(By.Id("LastName")).Clear();
            Driver.FindElement(By.Id("LastName")).SendKeys("Magazine");
            Driver.FindElement(By.Id("Patronymic")).Click();
            Driver.FindElement(By.Id("Patronymic")).Clear();
            Driver.FindElement(By.Id("Patronymic")).SendKeys("Store");
            Driver.FindElement(By.Id("Phone")).Click();
            Driver.ClearInput(By.Id("Phone"));
            Driver.FindElement(By.Id("Phone")).SendKeys("89378745533");
            Driver.FindElement(By.Id("birthday")).Click();
            Driver.ClearInput(By.Id("birthday"));
            Driver.FindElement(By.Id("birthday")).SendKeys("16092001");
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".toast.toast-success")).Count > 0);
            Driver.FindElement(By.Id("orderhistory")).Click();
            Driver.FindElement(By.Id("commoninf")).Click();
            VerifyAreEqual("Boss", Driver.FindElement(By.Id("FirstName")).GetAttribute("value"));
            VerifyAreEqual("Magazine", Driver.FindElement(By.Id("LastName")).GetAttribute("value"));
            VerifyAreEqual("Store", Driver.FindElement(By.Id("Patronymic")).GetAttribute("value"));
            VerifyAreEqual("+7(937)874-55-33", Driver.FindElement(By.Id("Phone")).GetAttribute("value"));
            VerifyAreEqual("16.09.2001", Driver.FindElement(By.Id("birthday")).GetAttribute("value"));
        }
    }

    public class ClientMyAccountAddressBookTests : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Client\\MyAccount\\Catalog.Photo.csv",
                "data\\Client\\MyAccount\\Catalog.Color.csv",
                "data\\Client\\MyAccount\\Catalog.Size.csv",
                "data\\Client\\MyAccount\\Catalog.Category.csv",
                "data\\Client\\MyAccount\\Customers.Customer.csv",
                "data\\Client\\MyAccount\\Customers.CustomerGroup.csv",
                "data\\Client\\MyAccount\\Customers.Contact.csv",
                "data\\Client\\MyAccount\\Catalog.Brand.csv",
                "data\\Client\\MyAccount\\Catalog.Product.csv",
                "data\\Client\\MyAccount\\Catalog.ProductCategories.csv",
                "data\\Client\\MyAccount\\Catalog.Offer.csv",
                "data\\Client\\MyAccount\\Catalog.Property.csv",
                "data\\Client\\MyAccount\\Catalog.PropertyValue.csv",
                "data\\Client\\MyAccount\\Catalog.ProductPropertyValue.csv",
                "data\\Client\\MyAccount\\Catalog.PropertyGroup.csv",
                "data\\Client\\MyAccount\\Customers.Managers.csv",
                "data\\Client\\MyAccount\\[Order].OrderStatus.csv",
                "data\\Client\\MyAccount\\[Order].ShippingMethod.csv",
                "data\\Client\\MyAccount\\[Order].PaymentMethod.csv",
                "data\\Client\\MyAccount\\[Order].[Order].csv",
                "data\\Client\\MyAccount\\[Order].OrderCustomer.csv",
                "data\\Client\\MyAccount\\[Order].OrderSource.csv",
                "data\\Client\\MyAccount\\[Order].OrderContact.csv",
                "data\\Client\\MyAccount\\[Order].OrderCurrency.csv",
                "data\\Client\\MyAccount\\[Order].OrderItems.csv"
            );

            Init();
            EnableInplaceOff();
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
        public void AddressBook()
        {
            GoToClient("myaccount?tab=addressbook");
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-action")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".adv-modal-inner")).Displayed);
            Driver.FindElement(By.Id("addressCity")).Click();
            Driver.FindElement(By.Id("addressCity")).SendKeys("Киев");
            Driver.FindElement(By.Id("addressCity")).Click();
            Driver.FindElement(By.Id("addressCountry")).Click();
            Driver.ScrollTo(By.CssSelector("[label=\"Украина\"]"));
            Driver.FindElement(By.CssSelector("[label=\"Украина\"]")).Click();
            Driver.FindElement(By.Id("addressZip")).Click();
            Driver.FindElement(By.Id("addressZip")).SendKeys("457846");
            Driver.FindElement(By.Id("addressDetails")).Click();
            Driver.FindElement(By.Id("addressDetails")).SendKeys("Пушкина, 42");
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".address-list-address-text")).Text
                .Contains("457846, Украина, Киев, Киев, улица Пушкина, 42"));
            Driver.FindElement(By.CssSelector(".address-controls")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".adv-modal-inner")).Displayed);
            Driver.FindElement(By.Id("addressCity")).Click();
            Driver.FindElement(By.Id("addressCity")).Clear();
            Driver.FindElement(By.Id("addressCity")).SendKeys("Гомель");
            Driver.FindElement(By.Id("addressCity")).Click();
            Driver.FindElement(By.Id("addressCountry")).Click();
            Driver.ScrollTo(By.CssSelector("[label=\"Беларусь\"]"));
            Driver.FindElement(By.CssSelector("[label=\"Беларусь\"]")).Click();
            Driver.FindElement(By.Id("addressZip")).Click();
            Driver.FindElement(By.Id("addressZip")).Clear();
            Driver.FindElement(By.Id("addressZip")).SendKeys("888899");
            Driver.FindElement(By.Id("addressDetails")).Click();
            Driver.FindElement(By.Id("addressDetails")).Clear();
            Driver.FindElement(By.Id("addressDetails")).SendKeys("Гончарова, 35");
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".address-list-address-text")).Text
                .Contains("888899, Беларусь, Гомель, Гомельская область, улица Гончарова, 35"));
        }
    }

    public class ClientMyAccountWishListTests : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Client\\MyAccount\\Catalog.Photo.csv",
                "data\\Client\\MyAccount\\Catalog.Color.csv",
                "data\\Client\\MyAccount\\Catalog.Size.csv",
                "data\\Client\\MyAccount\\Catalog.Category.csv",
                "data\\Client\\MyAccount\\Customers.Customer.csv",
                "data\\Client\\MyAccount\\Customers.CustomerGroup.csv",
                "data\\Client\\MyAccount\\Customers.Contact.csv",
                "data\\Client\\MyAccount\\Catalog.Brand.csv",
                "data\\Client\\MyAccount\\Catalog.Product.csv",
                "data\\Client\\MyAccount\\Catalog.ProductCategories.csv",
                "data\\Client\\MyAccount\\Catalog.Offer.csv",
                "data\\Client\\MyAccount\\Catalog.Property.csv",
                "data\\Client\\MyAccount\\Catalog.PropertyValue.csv",
                "data\\Client\\MyAccount\\Catalog.ProductPropertyValue.csv",
                "data\\Client\\MyAccount\\Catalog.PropertyGroup.csv",
                "data\\Client\\MyAccount\\Customers.Managers.csv",
                "data\\Client\\MyAccount\\[Order].OrderStatus.csv",
                "data\\Client\\MyAccount\\[Order].ShippingMethod.csv",
                "data\\Client\\MyAccount\\[Order].PaymentMethod.csv",
                "data\\Client\\MyAccount\\[Order].[Order].csv",
                "data\\Client\\MyAccount\\[Order].OrderCustomer.csv",
                "data\\Client\\MyAccount\\[Order].OrderSource.csv",
                "data\\Client\\MyAccount\\[Order].OrderContact.csv",
                "data\\Client\\MyAccount\\[Order].OrderCurrency.csv",
                "data\\Client\\MyAccount\\[Order].OrderItems.csv"
            );

            Init();
            EnableInplaceOff();
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
        public void WishList()
        {
            GoToClient("myaccount?tab=wishlist");
            Driver.FindElement(By.CssSelector(".h2.myaccount-subtitle")).Text.Contains("Избранное");
            VerifyAreEqual("wishlist-empty  ",
                Driver.FindElement(By.CssSelector(".wishlist-empty")).GetAttribute("class"));
            GoToClient("products/test-product60");
            Driver.FindElement(By.CssSelector(".wishlist-control.cs-l-2")).Click();
            Thread.Sleep(700);
            GoToClient("myaccount?tab=wishlist");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".products-view-container")).Text.Contains("TestProduct60"));
        }
    }
}