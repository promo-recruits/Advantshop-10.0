using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.MethodShippingPayment
{
    [TestFixture]
    public class ShippingTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Shipping | ClearType.Payment | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Shipping\\[Order].PaymentMethod.csv",
                "data\\Admin\\Settings\\Shipping\\[Order].ShippingMethod.csv",
                "data\\Admin\\Settings\\Shipping\\Catalog.Category.csv",
                "data\\Admin\\Settings\\Shipping\\Catalog.Product.csv",
                "data\\Admin\\Settings\\Shipping\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\Shipping\\Catalog.ProductCategories.csv"
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

        [Test]
        [Order(1)]
        public void SettingsMethodShippingEditCourier()
        {
            GoToAdmin("settings/shippingmethods");

            VerifyAreEqual("FixedRateMailMen Фиксированная стоимость доставки",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingName\"]")).Text,
                " Shiping Name 1 Method in Setting");
            VerifyAreEqual("true",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnabled\"] input")).GetAttribute("value"),
                " Shiping Enabled 1 Method in Setting");
            VerifyAreEqual("FreeShipping Бесплатная доставка",
                Driver.FindElements(By.CssSelector("[data-e2e=\"ShippingName\"]"))[1].Text,
                " Shiping Name 2 Method in Setting");
            VerifyAreEqual("false",
                Driver.FindElements(By.CssSelector("[data-e2e=\"ShippingEnabled\"] input"))[1].GetAttribute("value"),
                " Shiping Enabled 2 Method in Setting");

            GoToAdmin("shippingmethods/edit/1");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingReturn"));
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("FixedRateMailMen"),
                " h1 teg edit shipping");
            VerifyAreEqual("FixedRateMailMen", Driver.FindElement(By.Id("Name")).GetAttribute("value"),
                " Shiping Name 1 Method in edit ");
            VerifyIsTrue(Driver.FindElement(By.Name("Enabled")).Selected, " Shiping Enabled 1 Method in edit");

            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("FixedRateCourier"); //.col-xs-9 a

            AttachFile(By.CssSelector("input[type=\"file\"]"), GetPicturePath("brand_logo.jpg"));
            //country
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).SendKeys("Россия");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryAdd\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).SendKeys("Беларусь");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryAdd\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryDel\"]")).Click();
            Thread.Sleep(1000);
            //city
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).SendKeys("Анапа");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityAdd\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).SendKeys("Москва");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityAdd\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityDel\"]")).Click();
            Thread.Sleep(1000);
            // VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCity\"]")).Text.Contains("Москва"), " Shiping 1 city in edit ");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountry\"]")).Text.Contains("Беларусь"),
                " Shiping country 1 Method in edit ");

            //city
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCity\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCity\"]")).SendKeys("Казань");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCityAdd\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCityList\"]")).Text.Contains("Казань"),
                " Shiping enabled city in edit ");
            //description
            Driver.FindElement(By.Id("Description")).Clear();
            Driver.FindElement(By.Id("Description")).SendKeys("New Description here");
            Driver.FindElement(By.Id("ZeroPriceMessage")).Clear();
            Driver.FindElement(By.Id("ZeroPriceMessage")).SendKeys("New ZeroPriceMessage here");
            Driver.FindElement(By.Id("SortOrder")).Clear();
            Driver.FindElement(By.Id("SortOrder")).SendKeys("0");

            Driver.ScrollTo(By.CssSelector(".btn.btn-sm.btn-action"));

            VerifyIsTrue(Driver.FindElement(By.Name("Enabled")).Selected, " Shiping Enabled 1 Method in edit");
            VerifyIsTrue(Driver.FindElement(By.Name("DisplayCustomFields")).Selected,
                " Shiping DisplayCustomFields 1 Method in edit");
            VerifyIsTrue(Driver.FindElement(By.Name("DisplayIndex")).Selected,
                " Shiping DisplayIndex 1 Method in edit");
            VerifyIsTrue(!Driver.FindElement(By.Name("ShowInDetails")).Selected,
                " Shiping ShowInDetails 1 Method in edit");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingAddittional\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingIndex\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingDetais\"]")).Click();
            Driver.FindElements(By.CssSelector("[data-e2e=\"PaymentMethods\"]"))[0].Click();
            Driver.FindElements(By.CssSelector("[data-e2e=\"PaymentMethods\"]"))[1].Click();
            Functions.SelectItem(Driver, "CurrencyId", 0);
            Thread.Sleep(2000);
            Driver.FindElement(By.Id("ShippingPrice")).Clear();
            Driver.FindElement(By.Id("ShippingPrice")).SendKeys("100");
            Driver.FindElement(By.Id("DeliveryTime")).Clear();
            Driver.FindElement(By.Id("DeliveryTime")).SendKeys("30");
            Thread.Sleep(2000);

            Driver.ScrollTo(By.ClassName("search-input-wrap"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingAdd\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingReturn\"] a")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.Contains("settings/shippingmethods"), " return from edit shipping");

            VerifyAreEqual("FixedRateCourier Фиксированная стоимость доставки",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingName\"]")).Text,
                " edited Shiping Name 1 Method in Setting");
            VerifyAreEqual("true",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnabled\"] input")).GetAttribute("value"),
                "edited  Shiping Enabled 1 Method in Setting");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".shipping-icon img")).GetAttribute("src").Contains("nophoto"),
                "image method in Setting");

            ProductToCard();

            GoToClient("checkout");
            Refresh();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'FixedRateCourier')]")); //
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            VerifyAreEqual("FixedRateCourier", Driver.FindElement(By.CssSelector(".shipping-item-title")).Text,
                "Name edited shipping Method in cart");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".shipping-item-description.cs-t-3")).Text.Contains("100 руб."),
                " price edited shipping Method in cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".shipping-item-description.cs-t-3")).Text.Contains("30"),
                " time edited shipping Method in cart");
            VerifyAreEqual("100 руб.",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                " cost edited shipping Method in cart");
            VerifyAreEqual("New Description here", Driver.FindElement(By.CssSelector(".readmore-content")).Text,
                " Description edited shipping Method in cart");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".shipping-item-icon img")).GetAttribute("src").Contains("nophoto"),
                "image method in cart");

            VerifyAreEqual("Нет доступных методов оплаты", Driver.FindElement(By.Id("checkoutpayment")).Text,
                " no method payment in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " no method shipping for city in cart");
            VerifyAreEqual("Нет доступных методов оплаты", Driver.FindElement(By.Id("checkoutpayment")).Text,
                " no method payment in cart");

            //переименование метода FixedRateCourier в FixedRateMailMen
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(By.Id("Name")).Clear();
            Driver.FindElement(By.Id("Name")).SendKeys("FixedRateMailMen");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
        }

        [Test]
        [Order(0)]
        public void SettingsMethodShippingAdd()
        {
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Метод доставки", Driver.FindElement(By.TagName("h2")).Text, " h2 add shipping method");
            Driver.FindElement(AdvBy.DataE2E("ShippingAddName")).SendKeys("New Shipping Method");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingAddSelect\"]")))).SelectByText(
                "Бесплатная доставка");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingAddDecs\"]"))
                .SendKeys("New Description Shipping Method");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Thread.Sleep(2000);
            if (!Driver.FindElement(By.Name("Enabled")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnabled\"]")).Click();
            }

            GoToAdmin("settings/shippingmethods");
            VerifyIsTrue(Driver.PageSource.Contains("New Shipping Method"), "Show on page shipping method in list");

            ProductToCard();
            //отображение методов в корзине
            GoToClient("checkout");
            VerifyIsTrue(Driver.PageSource.Contains("New Shipping Method"), "Show on page shipping method in cart");
            //удаление New Shipping Method
            GoToAdmin("settings/shippingmethods");
            Driver.FindElements(AdvBy.DataE2E("DeleteShippingMethod"))[0].Click();
            Driver.WaitForElem(By.CssSelector(".swal2-modal"));
            Driver.FindElement(By.CssSelector(".swal2-modal .btn-success")).Click();
        }

        /// <summary>
        /// если корзина пустая, то добавляет товар
        /// </summary>
        public void ProductToCard()
        {
            GoToClient();
            if (Driver.FindElement(By.CssSelector(".cart-mini a")).Text.Contains("пусто"))
            {
                Driver.ScrollTo(By.CssSelector(".products-specials-more"));
                Driver.FindElement(By.CssSelector(".products-view-buttons")).Click();
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// если изначально выбран не тот город для доставки, то выбирает нужный
        /// </summary>
        public void CityinCard(string city) 
        {
            if(!Driver.FindElement(By.CssSelector(".checkout-shipping__zone span")).Text.Contains(city))
            {
                Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
                Driver.WaitForElem(By.ClassName("modal-content"));
                Driver.FindElement(By.XPath("//a[contains(text(), '" + city + "')]")).Click();
                Thread.Sleep(500);
            } 
        }

        /// <summary>
        /// если есть на странице метод FixedRateMailMen, то удаляет его, чтобы не мешал
        /// </summary>
        public void DeleteExtraMethod()
        {
            if (Driver.PageSource.Contains("FixedRateMailMen"))
            {
                Driver.FindElements(AdvBy.DataE2E("DeleteShippingMethod"))[0].Click();
                Driver.WaitForElem(By.CssSelector(".swal2-modal"));
                Driver.FindElement(By.CssSelector(".swal2-modal .btn-success")).Click();
            }
        }

        [Test]
        [Order(3)]
        public void SettingsMethodShippingRussianPost()
        {
            //R1
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            DeleteExtraMethod();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.WaitForModal();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddSelect"))
                .FindElement(By.CssSelector("[label=\"Почта России\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddName")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddName")).SendKeys("Почта России");
            Driver.FindElement(By.ClassName("modal-dialog")).FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingReturn"));
            Driver.ScrollTo(By.Id("StatusesSync"));
            Driver.FindElement(By.Id("Login")).Click();
            Driver.FindElement(By.Id("Login")).SendKeys("tma85@inbox.ru");
            Driver.FindElement(By.Id("Password")).Click();
            Driver.FindElement(By.Id("Password")).SendKeys("оооитмкамиль");
            Driver.FindElement(By.Id("Token")).Click();
            Driver.FindElement(By.Id("Token")).SendKeys("u04uXP7exHfOiDjiuAh8MdhQILiG020b");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingEnabled")).Click();
            Driver.ScrollTo(By.Id("Token"));
            Driver.FindElement(By.Id("LocalDeliveryTypes")).FindElements(By.TagName("option"))[0].Click();
            Driver.FindElement(By.Id("InternationalDeliveryTypes")).FindElements(By.TagName("option"))[0].Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingReturn\"] a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            VerifyAreEqual("Почта России Почта России",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingName\"]")).Text,
                "Shiping Name 1 Method in Setting");
            VerifyAreEqual("true",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnabled\"] input")).GetAttribute("value"),
                "Shiping Enabled 1 Method in Setting");
            //R2
            ProductToCard();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            CityinCard("Казань");
            string num8 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            VerifyAreEqual("Почта России (Бандероль заказное)",
                Driver.FindElement(By.CssSelector(".shipping-item-title")).Text, "Name edited shipping Method in cart");
            //R3
            VerifyIsNotNull(Driver.FindElements(By.CssSelector(".shipping-item-description.cs-t-3 span"))[1].Text,
                " price shipping Method not displayed in cart");
            VerifyIsNotNull(Driver.FindElements(By.CssSelector(".shipping-item-description.cs-t-3 span"))[2].Text,
                " time shipping Method not displayed in cart");
            VerifyIsNotNull(Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                " cost shipping Method not displayed in cart");
            //R4
            VerifyAreEqual(1, Driver.FindElements(By.Id("AddressContact_Street")).Count);
            //R5
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).SendKeys("Беларусь");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionName\"]")).SendKeys("Самарская область");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).SendKeys("Семей");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Самара')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[2].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Минск')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[3].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Семей')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[3].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Алматы')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingCountryDel")).Click();
            Thread.Sleep(400);
            Driver.FindElement(AdvBy.DataE2E("ShippingRegionDel")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(AdvBy.DataE2E("ShippingCityDel")).Click();
            //R6
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountry\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountry\"]")).SendKeys("Казахстан");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountryAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegion\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegion\"]")).SendKeys("Минская область");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegionAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCity\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCity\"]")).SendKeys("Москва");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCityAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Москва')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[2].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Минск')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[3].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Караганда')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingEnCountryDel")).Click();
            Thread.Sleep(400);
            Driver.FindElement(AdvBy.DataE2E("ShippingEnRegionDel")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(AdvBy.DataE2E("ShippingEnCityDel")).Click();
            //R7
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            GoToClient("categories/test-category1");
            Driver.ScrollTo(By.CssSelector(".products-view .btn-buy"), 1);
            Driver.FindElements(By.CssSelector(".products-view .btn-buy"))[1].Click();
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(By.CssSelector(".ibox ui-modal-trigger"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("1_anchor")).Click();
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Driver.FindElements(By.CssSelector(".ibox ui-modal-trigger"))[1].Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("2_anchor")).Click();
            Thread.Sleep(500);
            Driver.MouseFocus(Driver.GetGridCell(0, "LinkShipping", "ProductsSelectvizr"));
            Driver.GetGridCell(0, "LinkShipping", "ProductsSelectvizr").FindElement(AdvBy.DataE2E("switchOnOffInput"))
                .Click();
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("close")).Click();
            //проверка корзины с товарами из TestCategory1
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //проверка корзины с товаром TestProduct28 из TestCategory2
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            GoToClient("categories/test-category2");
            Driver.ScrollTo(By.CssSelector(".products-view .btn-buy"), 0);
            Driver.FindElements(By.CssSelector(".products-view .btn-buy"))[0].Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //доп.проверка с доступным товаром из TestCategory2 (TestProduct29)
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            GoToClient("categories/test-category2");
            Driver.ScrollTo(By.CssSelector(".products-view .btn-buy"), 1);
            Driver.FindElements(By.CssSelector(".products-view .btn-buy"))[1].Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.MouseFocus(By.CssSelector(".ibox a"), 3);
            Driver.FindElements(By.CssSelector(".ibox a"))[3].Click();
            Driver.MouseFocus(By.CssSelector(".ibox a"), 4);
            Driver.FindElements(By.CssSelector(".ibox a"))[4].Click();
            //R8
            Driver.ScrollTo(By.Id("Description"));
            Thread.Sleep(100);
            Driver.FindElement(AdvBy.DataE2E("ShippingIndex")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("products/test-product2");
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".block-exuding .middle-xs")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".block-exuding .shipping-variants")).Count);
            //R9->R10
            Driver.WaitForElem(By.CssSelector(".details-shipping .shipping-variants-row"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-shipping .shipping-variants-row")).Text
                .Contains("Почта России (Бандероль заказное)"));
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingIndex")).Click();
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("ShippingDetais")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("products/test-product2");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-shipping .shipping-variants-row")).Count < 1);
            //R11->R12
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("ExtrachargeInPercents")).Clear();
            Driver.FindElement(By.Id("ExtrachargeInPercents")).SendKeys("10");
            Driver.FindElement(By.Id("ExtraDeliveryTime")).Clear();
            Driver.FindElement(By.Id("ExtraDeliveryTime")).SendKeys("5");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            string num9 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num9) - Int32.Parse(num8) == Math.Round(Int32.Parse(num8) * 0.1),
                "extra charge is wrong in % delivery");
            VerifyAreEqual(", 8 дн.", Driver.FindElements(By.CssSelector(".shipping-item-description.cs-t-3 span"))[2].Text,
                " time edited shipping Method in cart");
            //R13
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("ExtrachargeFromOrder")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            string num10 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num10) - Int32.Parse(num8) == 1000 * 0.1,
                "extra charge is wrong in % order"); //1000 - стоимость заказа
            //R14
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("ExtrachargeInNumbers")).Clear();
            Driver.FindElement(By.Id("ExtrachargeInNumbers")).SendKeys("100");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            string num11 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num11) - Int32.Parse(num10) == 100, "extra charge is wrong");
            //R15
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("4");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("1 168 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                    " cost edited shipping Method in cart");
            //R16
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("5");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //R17
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("4");
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("200");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("1 168 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                    " cost shipping Method in cart didn't change");
            //R18
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("201");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //R19
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("2");
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("200");
            Driver.FindElement(By.Id("BaseDefaultWeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWeight")).SendKeys("2");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("988 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                    " cost edited shipping Method in cart");
            //R20
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".payment-item-active")).Count, 
                "no method payment in cart");
            //R21
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("PaymentMethods")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Почта России')]"), TimeSpan.FromSeconds(45));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".payment-item-active")).Count < 1, "method payment in cart");
            VerifyAreEqual("Нет доступных методов оплаты", Driver.FindElement(By.Id("checkoutpayment")).Text);
            //R22
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("BaseDefaultLength")).Clear();
            Driver.FindElement(By.Id("BaseDefaultLength")).SendKeys("101");
            Driver.FindElement(By.Id("BaseDefaultHeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultHeight")).SendKeys("101");
            Driver.FindElement(By.Id("BaseDefaultWidth")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWidth")).SendKeys("101");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //R23
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("StatusesSync"));
            Driver.FindElement(AdvBy.DataE2E("StatusesSyncRussianPost")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.Id("TrackingLogin")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("TrackingPassword")).Count);
            VerifyAreEqual(2, Driver.FindElements(By.CssSelector(".ui-select-container")).Count);
            
            //удаление метода Почта России
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("DeleteShippingMethod"))[0].Click();
            Driver.WaitForElem(By.CssSelector(".swal2-modal"));
            Driver.FindElement(By.CssSelector(".swal2-modal .btn-success")).Click();
        }

        [Test]
        [Order(2)]
        public void SettingsMethodShippingBoxberry()
        {
            //B1
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            DeleteExtraMethod();
            Driver.FindElements(AdvBy.DataE2E("DeleteShippingMethod"))[0].Click();
            Driver.WaitForElem(By.CssSelector(".swal2-modal"));
            Driver.FindElement(By.CssSelector(".swal2-modal .btn-success")).Click();

            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.WaitForModal();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddSelect")).FindElement(By.CssSelector("[label=\"Boxberry\"]"))
                .Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddName")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddName")).SendKeys("Boxberry");
            Driver.FindElement(By.ClassName("modal-dialog")).FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingReturn"));
            Driver.ScrollTo(By.CssSelector(".adv-panel-info"));
            Driver.FindElement(By.Id("Token")).Click();
            Driver.FindElement(By.Id("Token")).SendKeys("6f53cc0f565c884c51d946f6ff9bc722");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.ScrollTo(By.CssSelector(".form-group .flex-grow-n"), 30);
            Driver.FindElements(By.CssSelector(".form-group select"))[2]
                .FindElement(By.CssSelector("[value=\"30071\"]")).Click();
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("ShippingEnabled")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingReturn\"] a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            VerifyAreEqual("Boxberry Boxberry", Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingName\"]")).Text,
                "Shiping Name 1 Method in Setting");
            VerifyAreEqual("true",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnabled\"] input")).GetAttribute("value"),
                "Shiping Enabled 1 Method in Setting");
            //B2
            ProductToCard();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            CityinCard("Казань");
            string num0 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            VerifyAreEqual("Boxberry (постаматы и пункты выдачи)",
                Driver.FindElement(By.CssSelector(".shipping-item-title")).Text, "Name edited shipping Method in cart");
            //B3
            VerifyIsNotNull(Driver.FindElements(By.CssSelector(".shipping-item-description.cs-t-3 span"))[1].Text,
                " price shipping Method not displayed in cart");
            VerifyIsNotNull(Driver.FindElements(By.CssSelector(".shipping-item-description.cs-t-3 span"))[2].Text,
                " time shipping Method not displayed in cart");
            VerifyIsNotNull(Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                " cost shipping Method not displayed in cart");
            //B4
            Driver.FindElement(By.CssSelector(".checkout-block button")).Click();
            Driver.WaitForElem(By.ClassName("boxberry_content"));
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("boxberry_content")).Count);
            //B5
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).SendKeys("Беларусь");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionName\"]")).SendKeys("Самарская область");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).SendKeys("Семей");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Самара')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[2].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Минск')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[3].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Семей')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[3].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Алматы')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingCountryDel")).Click();
            Thread.Sleep(400);
            Driver.FindElement(AdvBy.DataE2E("ShippingRegionDel")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(AdvBy.DataE2E("ShippingCityDel")).Click();
            //B6
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountry\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountry\"]")).SendKeys("Казахстан");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountryAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegion\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegion\"]")).SendKeys("Минская область");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegionAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCity\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCity\"]")).SendKeys("Москва");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCityAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Москва')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[2].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Минск')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[3].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Караганда')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingEnCountryDel")).Click();
            Thread.Sleep(400);
            Driver.FindElement(AdvBy.DataE2E("ShippingEnRegionDel")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(AdvBy.DataE2E("ShippingEnCityDel")).Click();
            //B7
            Driver.FindElements(By.CssSelector(".ibox ui-modal-trigger"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("1_anchor")).Click();
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Driver.FindElements(By.CssSelector(".ibox ui-modal-trigger"))[1].Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("2_anchor")).Click();
            Thread.Sleep(500);
            Driver.MouseFocus(Driver.GetGridCell(0, "LinkShipping", "ProductsSelectvizr"));
            Driver.GetGridCell(0, "LinkShipping", "ProductsSelectvizr").FindElement(AdvBy.DataE2E("switchOnOffInput"))
                .Click();
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("close")).Click();
            //проверка корзины с товарами из TestCategory1
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //проверка корзины с товаром TestProduct28 из TestCategory2
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            GoToClient("categories/test-category2");
            Driver.ScrollTo(By.CssSelector(".products-view .btn-buy"), 0);
            Driver.FindElements(By.CssSelector(".products-view .btn-buy"))[0].Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //доп.проверка с доступным товаром из TestCategory2 (TestProduct29)
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            GoToClient("categories/test-category2");
            Driver.ScrollTo(By.CssSelector(".products-view .btn-buy"), 1);
            Driver.FindElements(By.CssSelector(".products-view .btn-buy"))[1].Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.MouseFocus(By.CssSelector(".ibox a"), 3);
            Driver.FindElements(By.CssSelector(".ibox a"))[3].Click();
            Driver.MouseFocus(By.CssSelector(".ibox a"), 4);
            Driver.FindElements(By.CssSelector(".ibox a"))[4].Click();
            //B8
            Driver.ScrollTo(By.Id("Description"));
            Thread.Sleep(100);
            Driver.FindElement(AdvBy.DataE2E("ShippingIndex")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("products/test-product2");
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".block-exuding .middle-xs")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".block-exuding .shipping-variants")).Count);
            //B9->B10
            Driver.WaitForElem(By.CssSelector(".details-shipping .shipping-variants-row"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-shipping .shipping-variants-row")).Text
                .Contains("Boxberry (постаматы и пункты выдачи)"));
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingIndex")).Click();
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("ShippingDetais")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("products/test-product2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-shipping")).Text.Contains("Нет доступных методов доставки"),
                " method shipping for city in cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-shipping .shipping-variants-row")).Count < 1);
            //B11->B12
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("ExtrachargeInPercents")).Clear();
            Driver.FindElement(By.Id("ExtrachargeInPercents")).SendKeys("10");
            Driver.FindElement(By.Id("ExtraDeliveryTime")).Clear();
            Driver.FindElement(By.Id("ExtraDeliveryTime")).SendKeys("5");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            string num1 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num1) - Int32.Parse(num0) == Math.Round(Int32.Parse(num0) * 0.1),
                "extra charge is wrong in % delivery");
            VerifyAreEqual(", 14 дн.", Driver.FindElements(By.CssSelector(".shipping-item-description.cs-t-3 span"))[2].Text,
                " time edited shipping Method in cart");
            //B13
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("ExtrachargeFromOrder")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            string num2 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num2) - Int32.Parse(num0) ==  1000 * 0.1,
                "extra charge is wrong in % order"); //1000 - стоимость заказа
            //B14
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("ExtrachargeInNumbers")).Clear();
            Driver.FindElement(By.Id("ExtrachargeInNumbers")).SendKeys("100");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            string num3 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num3) - Int32.Parse(num2) == 100, "extra charge is wrong"); 
            //B15
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("30");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            string s0 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 1);
            string s1 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(2, 3);
            string num4 = s0 + s1;
            VerifyIsTrue(Int32.Parse(num4) > 2000, " cost edited shipping Method in cart");
            //B16
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("31");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //B17
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("30");
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("400");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            string s2 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 1);
            string s3 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(2, 3);
            string num5 = s2 + s3;
            VerifyIsTrue(Int32.Parse(num5) == Int32.Parse(num4), " cost edited shipping Method in cart");
            //B18
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("401");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //B19
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("5");
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("400");
            Driver.FindElement(By.Id("BaseDefaultWeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWeight")).SendKeys("2");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            string s4 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(s4) > 800, " cost edited shipping Method in cart");
            //B20
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Token"));
            Driver.FindElement(By.Id("DeliveryTypes")).FindElement(By.CssSelector("[value=\"0\"]")).Click();
            Driver.FindElement(By.Id("DeliveryTypes")).FindElement(By.CssSelector("[value=\"1\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            string s5 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 1);
            string s6 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(2, 3);
            string num6 = s5 + s6;
            VerifyIsTrue(Int32.Parse(num6) > 900, " cost edited shipping Method in cart");
            VerifyAreEqual(1, Driver.FindElements(By.Id("AddressContact_Street")).Count);
            //B21
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Token"));
            Driver.FindElement(AdvBy.DataE2E("MethodWithInsure")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            string s7 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 1);
            string s8 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(2, 3);
            string num7 = s7 + s8;
            VerifyIsTrue(Int32.Parse(num7) - Int32.Parse(num6) == 5, " cost edited shipping Method in cart");
            //B22
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".payment-item-active")).Count, 
                "no method payment in cart");
            //B23
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("PaymentMethods")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Boxberry')]"), TimeSpan.FromSeconds(45));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".payment-item-active")).Count < 1, "method payment in cart");
            VerifyAreEqual("Нет доступных методов оплаты", Driver.FindElement(By.Id("checkoutpayment")).Text);
            //B24
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("BaseDefaultLength")).Clear();
            Driver.FindElement(By.Id("BaseDefaultLength")).SendKeys("101");
            Driver.FindElement(By.Id("BaseDefaultHeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultHeight")).SendKeys("101");
            Driver.FindElement(By.Id("BaseDefaultWidth")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWidth")).SendKeys("101");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //B25
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("StatusesSync"));
            Driver.FindElement(AdvBy.DataE2E("StatusesSyncBoxberry")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_AcceptedForDelivery")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SentToSorting")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_TransferredToSorting")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SentToDestinationCity")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_Courier")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_PickupPoint")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_Delivered")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_ReturnPreparing")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_ReturnSentToReceivingPoint")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_ReturnReturnedToReceivingPoint")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_ReturnByCourier")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_ReturnReturned")).Count);

            //удаление метода Boxberry
            GoToAdmin("settings/shippingmethods");
            Driver.FindElements(AdvBy.DataE2E("DeleteShippingMethod"))[0].Click();
            Driver.WaitForElem(By.CssSelector(".swal2-modal"));
            Driver.FindElement(By.CssSelector(".swal2-modal .btn-success")).Click();
        }

        [Test]
        [Order(4)]
        public void SettingsMethodShippingSDEK()
        {
            //S1
			GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            DeleteExtraMethod();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.WaitForModal();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddSelect"))
                .FindElement(By.CssSelector("[label=\"СДЭК (Служба доставки)\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddName")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddName")).SendKeys("СДЭК");
            Driver.FindElement(By.ClassName("modal-dialog")).FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingReturn"));
            Driver.ScrollTo(By.CssSelector(".form-group .flex-grow-n"), 20);
            Driver.FindElement(By.Id("AuthLogin")).Click();
            Driver.FindElement(By.Id("AuthLogin")).SendKeys("boSwLmJDHoC118CDfEwsfHrBddvKVtqQ");
            Driver.FindElement(By.Id("AuthPassword")).Click();
            Driver.FindElement(By.Id("AuthPassword")).SendKeys("VypSY6AQhFdH9Tip5IbPpGSEG96J12JN");
            Driver.FindElement(By.Id("CityFrom")).Click();
            Driver.FindElement(By.Id("CityFrom")).SendKeys("Москва");
            Driver.FindElements(By.CssSelector(".form-group select"))[2].FindElement(By.CssSelector("[value=\"10\"]"))
                .Click();
            Driver.FindElement(By.Id("YaMapsApiKey")).Click();
            Driver.FindElement(By.Id("YaMapsApiKey")).SendKeys("a47a35f8-31a7-4769-89e6-c34f8d069786");
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("ShippingEnabled")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
			Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingReturn\"] a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            VerifyAreEqual("СДЭК СДЭК (Служба доставки)", Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingName\"]")).Text,
                "Shiping Name 1 Method in Setting");
            VerifyAreEqual("true",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnabled\"] input")).GetAttribute("value"),
                "Shiping Enabled 1 Method in Setting");
            //S2
            ProductToCard();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            CityinCard("Казань");
            string num12 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            VerifyAreEqual("СДЭК (пункты выдачи)",
                Driver.FindElement(By.CssSelector(".shipping-item-title")).Text, "Name edited shipping Method in cart");
            //S3
            VerifyIsNotNull(Driver.FindElements(By.CssSelector(".shipping-item-description.cs-t-3 span"))[1].Text,
                " price shipping Method not displayed in cart");
            VerifyIsNotNull(Driver.FindElements(By.CssSelector(".shipping-item-description.cs-t-3 span"))[2].Text,
                " time shipping Method not displayed in cart");
            VerifyIsNotNull(Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                " cost shipping Method not displayed in cart");
            //S4
            Driver.FindElement(By.CssSelector(".btn-xsmall")).Click();
            Driver.WaitForElem(By.ClassName("ymaps-2-1-79-map"));
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("ymaps-2-1-79-map")).Count);
            //S5
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).SendKeys("Беларусь");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionName\"]")).SendKeys("Самарская область");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).SendKeys("Алматы");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Самара')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[2].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Минск')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[3].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Алматы')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[3].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Нур-Султан')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingCountryDel")).Click();
            Thread.Sleep(400);
            Driver.FindElement(AdvBy.DataE2E("ShippingRegionDel")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(AdvBy.DataE2E("ShippingCityDel")).Click();
            //S6
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountry\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountry\"]")).SendKeys("Казахстан");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountryAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegion\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegion\"]")).SendKeys("Минская область");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegionAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCity\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCity\"]")).SendKeys("Москва");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCityAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Москва')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[2].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Минск')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.FindElements(By.CssSelector(".modal-content a"))[3].Click();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Караганда')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingEnCountryDel")).Click();
            Thread.Sleep(400);
            Driver.FindElement(AdvBy.DataE2E("ShippingEnRegionDel")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(AdvBy.DataE2E("ShippingEnCityDel")).Click();
            //S7
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            GoToClient("categories/test-category1");
            Driver.ScrollTo(By.CssSelector(".products-view .btn-buy"), 1);
            Driver.FindElements(By.CssSelector(".products-view .btn-buy"))[1].Click();
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(By.CssSelector(".ibox ui-modal-trigger"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("1_anchor")).Click();
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Driver.FindElements(By.CssSelector(".ibox ui-modal-trigger"))[1].Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("2_anchor")).Click();
            Thread.Sleep(500);
            Driver.MouseFocus(Driver.GetGridCell(0, "LinkShipping", "ProductsSelectvizr"));
            Driver.GetGridCell(0, "LinkShipping", "ProductsSelectvizr").FindElement(AdvBy.DataE2E("switchOnOffInput"))
                .Click();
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("close")).Click();
            //проверка корзины с товарами из TestCategory1
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //проверка корзины с товаром TestProduct28 из TestCategory2
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            GoToClient("categories/test-category2");
            Driver.ScrollTo(By.CssSelector(".products-view .btn-buy"), 0);
            Driver.FindElements(By.CssSelector(".products-view .btn-buy"))[0].Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //доп.проверка с доступным товаром из TestCategory2 (TestProduct29)
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            GoToClient("categories/test-category2");
            Driver.ScrollTo(By.CssSelector(".products-view .btn-buy"), 1);
            Driver.FindElements(By.CssSelector(".products-view .btn-buy"))[1].Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.MouseFocus(By.CssSelector(".ibox a"), 3);
            Driver.FindElements(By.CssSelector(".ibox a"))[3].Click();
            Driver.MouseFocus(By.CssSelector(".ibox a"), 4);
            Driver.FindElements(By.CssSelector(".ibox a"))[4].Click();
            //S8
            Driver.ScrollTo(By.Id("Description"));
            Thread.Sleep(100);
            Driver.FindElement(AdvBy.DataE2E("ShippingIndex")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("products/test-product2");
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".block-exuding .middle-xs")).Count);
            //S9->S10
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".block-exuding .shipping-variants")).Count);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".details-shipping .shipping-variants-row")).Text
                .Contains("СДЭК (пункты выдачи)"));
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingIndex")).Click();
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("ShippingDetais")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("products/test-product2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-shipping")).Text.Contains("Нет доступных методов доставки"),
                " method shipping for city in cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-shipping .shipping-variants-row")).Count < 1);
            //S11->S12
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("ExtrachargeInPercents")).Clear();
            Driver.FindElement(By.Id("ExtrachargeInPercents")).SendKeys("10");
            Driver.FindElement(By.Id("ExtraDeliveryTime")).Clear();
            Driver.FindElement(By.Id("ExtraDeliveryTime")).SendKeys("5");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            string num13 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num13) - Int32.Parse(num12) == Math.Round(Int32.Parse(num12) * 0.1),
                "extra charge is wrong in % delivery");
            VerifyAreEqual(", 6-7 дн.", Driver.FindElements(By.CssSelector(".shipping-item-description.cs-t-3 span"))[2].Text, 
                " time edited shipping Method in cart");//было 1-2 дн.
            //S13
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("ExtrachargeFromOrder")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            string num14 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num14) - Int32.Parse(num12) == 1000 * 0.1,
                "extra charge is wrong in % order"); //1000 - стоимость заказа
            //S14
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("ExtrachargeInNumbers")).Clear();
            Driver.FindElement(By.Id("ExtrachargeInNumbers")).SendKeys("100");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            string num15 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num15) - Int32.Parse(num14) == 100, "extra charge is wrong");
            //S15
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("29");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("3 340 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                    " cost shipping Method in cart didn't change");
            //S16
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("30");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //S17
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("29");
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("430");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("3 340 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                    " cost shipping Method in cart didn't change");
            //S18
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("431");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("3 520 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                     " cost shipping Method in cart didn't change");
            //S19
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("7831");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //S20
            //сначала сбрасываем поля увелич. вес и габариты и проверяем стоимость доставки
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("0");
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("0");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("730 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                " cost edited shipping Method in cart");

            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(AdvBy.DataE2E("ShippingIndex"));
            Driver.FindElement(By.Id("BaseDefaultWeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWeight")).SendKeys("2");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("820 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text, 
                " cost edited shipping Method in cart");
            //S21
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".payment-item-active")).Count,
                "no method payment in cart");
            //S22
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("PaymentMethods")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".payment-item-active")).Count < 1, "method payment in cart");
            VerifyAreEqual("Нет доступных методов оплаты", Driver.FindElement(By.Id("checkoutpayment")).Text);
            
            //S23
            //пока что проверка бесполезна (20.08.2021), нужны доступы к методу у которых будет в сдэке возможна страховка
            
            /*GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("AuthPassword"));
            Driver.FindElement(AdvBy.DataE2E("MethodWithInsure")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                    .Contains("828 руб."), " cost edited shipping Method in cart"); //было 820 руб*/
            
            //S24
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("AuthPassword"));
            Driver.FindElement(AdvBy.DataE2E("ShowPointsAsList")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".select-custom")).Count);
            //S25
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("AuthPassword"));
            Driver.FindElement(By.Id("Tariff")).FindElement(By.CssSelector("[value=\"10\"]")).Click();
            Driver.FindElement(By.Id("Tariff")).FindElement(By.CssSelector("[value=\"11\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'СДЭК')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("СДЭК (курьером)", Driver.FindElement(By.CssSelector(".shipping-item-title")).Text);
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text, 
                " cost edited shipping Method in cart");
            //S26
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("DisplayIndex"));
            Driver.FindElement(By.Id("BaseDefaultLength")).Clear();
            Driver.FindElement(By.Id("BaseDefaultLength")).SendKeys("7931");
            Driver.FindElement(By.Id("BaseDefaultHeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultHeight")).SendKeys("7931");
            Driver.FindElement(By.Id("BaseDefaultWidth")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWidth")).SendKeys("7931");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //S27
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("AuthPassword"));
            Driver.FindElement(AdvBy.DataE2E("MethodUseSeller")).Click();
            VerifyAreEqual(1, Driver.FindElements(By.Id("SellerAddress")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("SellerName")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("SellerINN")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("SellerPhone")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("SellerOwnershipForm")).Count);
            //S28
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("AuthPassword"));
            Driver.FindElement(AdvBy.DataE2E("StatusesSyncSDEK")).Click();
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusCreated")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusAcceptedAtWarehouseOfSender")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusIssuedForShipmentFromSenderWarehouse")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusReturnedToWarehouseOfSender")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusDeliveredToCarrierFromSenderWarehouse")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusSentToTransitWarehouse")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusMetAtTransitWarehouse")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusAcceptedAtTransitWarehouse")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusReturnedToTransitWarehouse")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusIssuedForShipmentInTransitWarehouse")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusDeliveredToCarrierInTransitWarehouse")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusSentToWarehouseOfRecipient")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusMetAtConsigneeWarehouse")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusAcceptedAtConsigneeWarehouse_AwaitingDelivery")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusAcceptedAtConsigneeWarehouse_AwaitingFenceByClient")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusIssuedForDelivery")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusReturnedToConsigneeWarehouse")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusAwarded")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("StatusNotAwarded")).Count);

            //удаление метода СДЭК
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("DeleteShippingMethod"))[0].Click();
            Driver.WaitForElem(By.CssSelector(".swal2-modal"));
            Driver.FindElement(By.CssSelector(".swal2-modal .btn-success")).Click();
        }

        [Test]
        [Order(5)]
        public void SettingsMethodShippingYandex()
        {
            //Y1
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            DeleteExtraMethod();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.WaitForModal();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddSelect"))
                .FindElement(By.CssSelector("[label=\"Агрегатор Яндекс.Доставка (Новый кабинет)\"]")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddName")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAddName")).SendKeys("Яндекс");
            Driver.FindElement(By.ClassName("modal-dialog")).FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingReturn"));
            Driver.ScrollTo(By.Id("CurrencyId"));
            Driver.FindElement(By.Id("AuthorizationToken")).Click();
            Driver.FindElement(By.Id("AuthorizationToken")).SendKeys("AgAAAABP6vDiAAbbSc8jeuSXrUThtMH6e7tOJCw");
            Driver.FindElement(By.Id("ShopId")).Click();
            Driver.FindElement(By.Id("ShopId")).SendKeys("500003614");
            Driver.FindElement(By.Id("WarehouseId")).Click();
            Driver.FindElement(By.Id("WarehouseId")).SendKeys("10001682928");
            Driver.FindElement(By.Id("CityFrom")).Click();
            Driver.FindElement(By.Id("CityFrom")).SendKeys("Москва");
            Driver.FindElement(By.Id("WidgetApiKey")).Click();
            Driver.FindElement(By.Id("WidgetApiKey")).SendKeys("600ea1ea-b929-42eb-9eb1-97bacc1f97df");
            Driver.ScrollToTop();
            Driver.FindElement(AdvBy.DataE2E("ShippingEnabled")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingReturn\"] a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            VerifyAreEqual("Яндекс Агрегатор Яндекс.Доставка (Новый кабинет)", Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingName\"]")).Text,
                "Shiping Name 1 Method in Setting");
            VerifyAreEqual("true",
                Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnabled\"] input")).GetAttribute("value"),
                "Shiping Enabled 1 Method in Setting");
            //Y2->Y3
            ProductToCard();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            CityinCard("Казань");
            string num16 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyAreEqual(2,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            VerifyAreEqual("Яндекс (Курьер)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[0].Text, "Name edited shipping Method in cart");
            VerifyAreEqual("Яндекс (Самовывоз)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[1].Text, "Name edited shipping Method in cart");
            //Y4->Y5
            VerifyIsNotNull(Driver.FindElements(By.CssSelector(".shipping-item-description.cs-t-3 span"))[1].Text,
                " price shipping Method not displayed in cart"); //цена у Яндекс (Курьер)
            VerifyIsNotNull(Driver.FindElements(By.CssSelector(".shipping-item-description.cs-t-3 span"))[5].Text,
                " price shipping Method not displayed in cart");//цена у Яндекс (Самовывоз)
            VerifyIsNotNull(Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text,
                " cost shipping Method not displayed in cart");
            //Y6
            Driver.FindElements(By.CssSelector(".btn-xsmall"))[0].Click();
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".ReactModal__Content")).Count);
            //Y7->Y8
            Driver.WaitForElems(By.CssSelector(".g6DSZfw"), 0); //ждем появления списка курьерских служб
            Driver.FindElements(By.CssSelector(".g6DSZfw"))[0].Click();//выбрал курьером КСЭ-МСК
            Thread.Sleep(1000);
            VerifyAreEqual("Яндекс (Курьер) (КСЭ - МСК)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[0].Text, "Name edited shipping Method in cart");
            VerifyAreEqual(1, Driver.FindElements(By.Id("AddressContact_Street")).Count);
            //Y9->Y10
            Driver.ScrollTo(By.CssSelector(".stretch-middle"));
            Driver.FindElements(By.CssSelector(".btn-xsmall"))[1].Click();
            Driver.WaitForElem(By.CssSelector(".g6DSZfw")); //ждем появление списка ПВЗ
            Driver.FindElements(By.CssSelector(".g6DSZfw"))[3].Click();//выбрал ПВЗ PickPoint
            Thread.Sleep(1000);
            VerifyAreEqual("Яндекс (Самовывоз) (PickPoint)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[1].Text, "Name edited shipping Method in cart");
            //Y11
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryName\"]")).SendKeys("Беларусь");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCountryAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //Y12
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingCountryDel")).Click(); //удаляем страну активности
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionName\"]")).SendKeys("Самарская область");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingRegionAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Самара')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(2,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count);
            VerifyAreEqual("Яндекс (Курьер)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[0].Text);
            VerifyAreEqual("Яндекс (Самовывоз)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[1].Text);
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Москва')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //Y13
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingRegionDel")).Click();//удаляем регион активности
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityName\"]")).SendKeys("Москва");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingCityAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Москва')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(2,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count);
            VerifyAreEqual("Яндекс (Курьер)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[0].Text);
            VerifyAreEqual("Яндекс (Самовывоз)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[1].Text);
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //Y14
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingCityDel")).Click();//удаляем город активности
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountry\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountry\"]")).SendKeys("Россия");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCountryAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //Y15
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingEnCountryDel")).Click();//удаляем страну недоступности
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegion\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegion\"]")).SendKeys("Самарская область");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnRegionAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Москва')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(2,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count);
            VerifyAreEqual("Яндекс (Курьер)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[0].Text);
            VerifyAreEqual("Яндекс (Самовывоз)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[1].Text);
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Самара')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //Y16
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingEnRegionDel")).Click();//удаляем регион недоступности
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCity\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCity\"]")).SendKeys("Краснодар");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ShippingEnCityAdd\"]")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual(2,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count);
            VerifyAreEqual("Яндекс (Курьер)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[0].Text);
            VerifyAreEqual("Яндекс (Самовывоз)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[1].Text);
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Краснодар')]")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingEnCityDel")).Click();//удаляем город недоступности
            //Y17
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            GoToClient("categories/test-category1");
            Driver.ScrollTo(By.CssSelector(".products-view .btn-buy"), 1);
            Driver.FindElements(By.CssSelector(".products-view .btn-buy"))[1].Click();
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(By.CssSelector(".ibox ui-modal-trigger"))[0].Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("1_anchor")).Click();
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Driver.FindElements(By.CssSelector(".ibox ui-modal-trigger"))[1].Click();
            Driver.WaitForModal();
            Driver.FindElement(By.Id("2_anchor")).Click();
            Thread.Sleep(500);
            Driver.MouseFocus(Driver.GetGridCell(0, "LinkShipping", "ProductsSelectvizr"));
            Driver.GetGridCell(0, "LinkShipping", "ProductsSelectvizr").FindElement(AdvBy.DataE2E("switchOnOffInput"))
                .Click();
            Thread.Sleep(500);
            Driver.FindElement(By.ClassName("close")).Click();
            //проверка корзины с товарами из TestCategory1
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //проверка корзины с товаром TestProduct28 из TestCategory2
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            GoToClient("categories/test-category2");
            Driver.ScrollTo(By.CssSelector(".products-view .btn-buy"), 0);
            Driver.FindElements(By.CssSelector(".products-view .btn-buy"))[0].Click();
            GoToClient("checkout");
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //доп.проверка с доступным товаром из TestCategory2 (TestProduct29)
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-remove a")).Click();
            GoToClient("categories/test-category2");
            Driver.ScrollTo(By.CssSelector(".products-view .btn-buy"), 1);
            Driver.FindElements(By.CssSelector(".products-view .btn-buy"))[1].Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(2,
                Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                    .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            VerifyAreEqual("Яндекс (Курьер)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[0].Text, "Name edited shipping Method in cart");
            VerifyAreEqual("Яндекс (Самовывоз)",
                Driver.FindElements(By.CssSelector(".shipping-item-title"))[1].Text, "Name edited shipping Method in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.MouseFocus(By.CssSelector(".ibox a"), 3);
            Driver.FindElements(By.CssSelector(".ibox a"))[3].Click();
            Driver.MouseFocus(By.CssSelector(".ibox a"), 4);
            Driver.FindElements(By.CssSelector(".ibox a"))[4].Click();
            //Y18
            Driver.ScrollTo(By.Id("Description"));
            Thread.Sleep(100);
            Driver.FindElement(AdvBy.DataE2E("ShippingIndex")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("products/test-product2");
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".block-exuding .middle-xs")).Count);
            //Y19-20
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".block-exuding .shipping-variants")).Count);
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-shipping .shipping-variants-row"))[0].Text
                .Contains("Яндекс (Курьер)"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-shipping .shipping-variants-row"))[1].Text
                .Contains("Яндекс (Самовывоз)"));
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElement(AdvBy.DataE2E("ShippingIndex")).Click();
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("ShippingDetais")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("products/test-product2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-shipping")).Text.Contains("Нет доступных методов доставки"),
                " method shipping for city in cart");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".details-shipping .shipping-variants-row")).Count < 1);
            //Y21
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("ExtrachargeInPercents")).Clear();
            Driver.FindElement(By.Id("ExtrachargeInPercents")).SendKeys("10");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.FindElement(By.CssSelector(".link-dotted-invert")).Click();
            Driver.WaitForElem(By.ClassName("modal-content"));
            Driver.FindElement(By.XPath("//a[contains(text(), 'Казань')]")).Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            string num17 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num17) - Int32.Parse(num16) == Math.Round(Int32.Parse(num16) * 0.1),
                "extra charge is wrong in % delivery");
            //Y22
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("ExtrachargeFromOrder")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            string num18 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num18) - Int32.Parse(num16) == 1000 * 0.1,
                "extra charge is wrong in % order"); //1000 - стоимость заказа
            //Y23
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("ExtrachargeInNumbers")).Clear();
            Driver.FindElement(By.Id("ExtrachargeInNumbers")).SendKeys("100");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            string num19 = Driver.FindElement(By.CssSelector("[data-ng-bind=\"checkout.Cart.Delivery\"]")).Text
                .Substring(0, 3);
            VerifyIsTrue(Int32.Parse(num19) - Int32.Parse(num18) == 100, "extra charge is wrong");
            //Y24
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("79");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("1 141 руб.", Driver.FindElements(By.CssSelector("[data-ng-bind=\"item.FormatRate\"]"))[0].Text,
                    " cost shipping Yandex (Courier) in cart didn't change");
            VerifyAreEqual("2 059 руб.", Driver.FindElements(By.CssSelector("[data-ng-bind=\"item.FormatRate\"]"))[1].Text,
                    " cost shipping Yandex (Pickup) in cart didn't change");
            //Y25
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("80");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
               Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                   .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            VerifyAreEqual("Яндекс (Самовывоз)",
               Driver.FindElement(By.CssSelector(".shipping-item-title")).Text, "Name edited shipping Method in cart");
            //Y26
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("250");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //Y27
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("0");
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("820");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("1 821 руб.", Driver.FindElements(By.CssSelector("[data-ng-bind=\"item.FormatRate\"]"))[0].Text,
                    " cost shipping Yandex (Courier) in cart didn't change");
            VerifyAreEqual("4 589 руб.", Driver.FindElements(By.CssSelector("[data-ng-bind=\"item.FormatRate\"]"))[1].Text,
                    " cost shipping Yandex (Pickup) in cart didn't change");
            //Y28
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("821");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            VerifyAreEqual(1,
               Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                   .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            VerifyAreEqual("Яндекс (Самовывоз)",
               Driver.FindElement(By.CssSelector(".shipping-item-title")).Text, "Name edited shipping Method in cart");
            //Y29
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("901");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //Y30
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(By.Id("WeightExtracharge")).Clear();
            Driver.FindElement(By.Id("WeightExtracharge")).SendKeys("78");
            Driver.FindElement(By.Id("CargoExtracharge")).Clear();
            Driver.FindElement(By.Id("CargoExtracharge")).SendKeys("820");
            Driver.FindElement(By.Id("BaseDefaultWeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWeight")).SendKeys("2");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual("1 821 руб.", Driver.FindElements(By.CssSelector("[data-ng-bind=\"item.FormatRate\"]"))[0].Text,
                    " cost shipping Yandex (Courier) in cart didn't change");
            VerifyAreEqual("4 589 руб.", Driver.FindElements(By.CssSelector("[data-ng-bind=\"item.FormatRate\"]"))[1].Text,
                    " cost shipping Yandex (Pickup) in cart didn't change");
            //Y31
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(AdvBy.DataE2E("ShippingIndex"));
            Driver.FindElement(By.Id("BaseDefaultWeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWeight")).SendKeys("3");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
               Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                   .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            VerifyAreEqual("Яндекс (Самовывоз)",
               Driver.FindElement(By.CssSelector(".shipping-item-title")).Text, "Name edited shipping Method in cart");
            //Y32-33
            VerifyAreEqual(1, Driver.FindElements(By.CssSelector(".payment-item-active")).Count,
                "no method payment in cart");
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("Description"));
            Driver.FindElement(AdvBy.DataE2E("PaymentMethods")).Click();
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".payment-item-active")).Count < 1, "method payment in cart");
            VerifyAreEqual("Нет доступных методов оплаты", Driver.FindElement(By.Id("checkoutpayment")).Text);
            //Y34
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(AdvBy.DataE2E("ShippingIndex"));
            Driver.FindElement(By.Id("BaseDefaultWeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWeight")).SendKeys("173");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //Y35
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(AdvBy.DataE2E("ShippingIndex"));
            Driver.FindElement(By.Id("BaseDefaultWeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWeight")).SendKeys("1");
            Driver.FindElement(By.Id("BaseDefaultLength")).Clear();
            Driver.FindElement(By.Id("BaseDefaultLength")).SendKeys("101");
            Driver.FindElement(By.Id("BaseDefaultHeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultHeight")).SendKeys("101");
            Driver.FindElement(By.Id("BaseDefaultWidth")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWidth")).SendKeys("101");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Яндекс')]"), TimeSpan.FromSeconds(45));
            VerifyAreEqual(1,
               Driver.FindElements(By.CssSelector(".checkout-block"))[1]
                   .FindElements(By.CssSelector(".custom-input-radio")).Count, "change shipping Method in cart");
            VerifyAreEqual("Яндекс (Самовывоз)",
               Driver.FindElement(By.CssSelector(".shipping-item-title")).Text, "Name edited shipping Method in cart");
            //Y36
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(AdvBy.DataE2E("ShippingIndex"));
            Driver.FindElement(By.Id("BaseDefaultLength")).Clear();
            Driver.FindElement(By.Id("BaseDefaultLength")).SendKeys("181");
            Driver.FindElement(By.Id("BaseDefaultHeight")).Clear();
            Driver.FindElement(By.Id("BaseDefaultHeight")).SendKeys("181");
            Driver.FindElement(By.Id("BaseDefaultWidth")).Clear();
            Driver.FindElement(By.Id("BaseDefaultWidth")).SendKeys("181");
            Driver.FindElement(AdvBy.DataE2E("ShippingAdd")).Click();
            GoToClient("checkout");
            Thread.Sleep(500);
            VerifyAreEqual("Нет доступных методов доставки", Driver.FindElement(By.Id("checkoutshippings")).Text,
                " method shipping for city in cart");
            //Y37
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("ShippingName"))[0].FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.ScrollTo(By.Id("BaseDefaultWeight"));
            Driver.FindElement(AdvBy.DataE2E("StatusesSyncYandex")).Click();
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DRAFT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_VALIDATING")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_VALIDATING_ERROR")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_CREATED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SENDER_SENT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_LOADED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SENDER_WAIT_FULFILMENT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SENDER_WAIT_DELIVERY")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_AT_START")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_AT_START_SORT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_TRANSPORTATION")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_ARRIVED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_TRANSPORTATION_RECIPIENT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_CUSTOMS_ARRIVED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_CUSTOMS_CLEARED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_STORAGE_PERIOD_EXTENDED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_STORAGE_PERIOD_EXPIRED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_UPDATED_BY_SHOP")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_UPDATED_BY_RECIPIENT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_UPDATED_BY_DELIVERY")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_ARRIVED_PICKUP_POINT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_TRANSMITTED_TO_RECIPIENT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_DELIVERED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_ATTEMPT_FAILED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_DELIVERY_CAN_NOT_BE_COMPLETED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_RETURN_STARTED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_RETURN_PREPARING")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_RETURN_ARRIVED_DELIVERY")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_RETURN_TRANSMITTED_FULFILLMENT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_CREATED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_LOADED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_AT_START")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_OUT_OF_STOCK")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_AWAITING_CLARIFICATION")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_PREPARED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_TRANSMITTED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_RETURN_PREPARING")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_RETURN_RFF_PREPARING_FULFILLMENT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_RETURN_RFF_TRANSMITTED_FULFILLMENT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_RETURN_RFF_ARRIVED_FULFILLMENT")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_RETURN_ARRIVED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_RETURN_PREPARING_SENDER")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_RETURN_TRANSFERRED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_RETURN_RETURNED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_CANCELED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_SORTING_CENTER_ERROR")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_LOST")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_CANCELLED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_CANCELLED_USER")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_FINISHED")).Count);
            VerifyAreEqual(1, Driver.FindElements(By.Id("Status_ERROR")).Count);

            //удаление метода Яндекс
            GoToAdmin("settings/shippingmethods");
            Driver.WaitForElem(AdvBy.DataE2E("ShippingAdd"));
            Driver.FindElements(AdvBy.DataE2E("DeleteShippingMethod"))[0].Click();
            Driver.WaitForElem(By.CssSelector(".swal2-modal"));
            Driver.FindElement(By.CssSelector(".swal2-modal .btn-success")).Click();
        }
    }
}
