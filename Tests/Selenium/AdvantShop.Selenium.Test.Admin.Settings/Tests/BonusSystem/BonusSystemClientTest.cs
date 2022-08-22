using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem
{
    [TestFixture]
    public class BonusSystemClientTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders | ClearType.Customers | ClearType.Payment |
                                        ClearType.Shipping);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Customers.Contact.csv",
                "Data\\Admin\\Settings\\BonusSystem\\[Order].OrderSource.csv",
                "Data\\Admin\\Settings\\BonusSystem\\[Order].OrderStatus.csv",
                "Data\\Admin\\Settings\\BonusSystem\\[Order].PaymentMethod.csv",
                "Data\\Admin\\Settings\\BonusSystem\\[Order].ShippingMethod.csv"
            );
            InitializeService.BonusSystemActive();
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

        string numberCart = "";

        [Order(0)]
        [Test]
        public void CheckPlatinBonusCart()
        {
            GoToAdmin("settingsbonus");
            (new SelectElement(Driver.FindElement(By.Id("BonusGradeId")))).SelectByText("Платиновый");
            Driver.FindElement(By.Id("CardNumTo")).SendKeys("1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();
            IWebElement selectElem1 = Driver.FindElement(By.Id("BonusGradeId"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Платиновый"), "select item Platin");

            ProductToCard("+30 руб. на бонусную карту");
            GoToClient("cart");
            VerifyAreEqual("Сумма баллов начисляемых на бонусную карту: +30 руб.",
                Driver.FindElement(By.CssSelector(".bonus-card-block-inline")).Text, "icon adn text bonus card ");
            VerifyAreEqual("30 руб.", Driver.FindElements(By.CssSelector(".bonus-card-block-inline span>span"))[1].Text,
                "Count bonus to card");

            Driver.ScrollTo(By.CssSelector(".bonus-card-block-inline span"));
            Driver.FindElement(By.CssSelector("[data-ng-href=\"checkout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-page"));
            Driver.WaitForElem(By.CssSelector("[data-shipping-list]"));
            Driver.XPathContainsText("span", "Самовывоз");
            Driver.ScrollTo(By.Name("WantBonusCard"));
            Driver.FindElement(By.CssSelector(".custom-input-checkbox")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("+30 бонусов", Driver.FindElement(By.CssSelector(".bonus-card-plus-price")).Text,
                "bonus count in checkout checkbox. Platin");

            //Сделать провеку в блоке
            //VerifyIsTrue(driver.FindElement(By.CssSelector(".checkout-cart-wrap")).Text.Contains("Бонусов на карту"), "bonus name in checkout. Platin");
            //VerifyIsTrue(driver.FindElement(By.CssSelector(".checkout-cart-wrap")).Text.Contains("30  руб."), "bonus count in checkout. Platin");

            Driver.ScrollTo(By.CssSelector(".footer-menu-head"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), "success checkout order");
            VerifyIsTrue(Driver.PageSource.Contains("30 бонусов"), "Count bonus after order");
        }

        [Order(1)]
        [Test]
        public void CheckBeforePay()
        {
            GoToAdmin("cards");
            numberCart = Driver.GetGridCell(0, "CardNumber").Text;
            VerifyAreEqual("Ad Admin", Driver.GetGridCell(0, "FIO").Text, " Grid  BuyerName");
            VerifyAreEqual("Платиновый", Driver.GetGridCell(0, "GradeName").Text, " Grid graid ");
            VerifyAreEqual("30", Driver.GetGridCell(0, "GradePersent").Text, " Grid percent");

            Driver.GetGridCell(0, "CardNumber").Click();
            Driver.WaitForElem(By.ClassName("balance__padding-page"));

            VerifyAreEqual(numberCart, Driver.FindElement(By.CssSelector("[data-e2e=\"numberCardBonus\"]")).Text,
                " in str card");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"gradeBonus\"] select"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Платиновый"), "select item Platin");

            VerifyAreEqual("0 бонусов\r\n0 основных и 0 дополнительных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus")).Text, " in str card");

            VerifyAreEqual("Ad", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardSuname\"]")).Text,
                " bonusCardSuname in str card");
            VerifyAreEqual("Admin", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardName\"]")).Text,
                " bonusCardName in str card");
            VerifyAreEqual("1", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[0].Text,
                " number operition");
            //номер заказа на локале - 4, при выполнении на тачке  - 3
            VerifyAreEqual("3", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[1].Text,
                " number order");
            VerifyAreEqual("100", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[3].Text,
                " sum order");
            VerifyAreEqual("30", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[8].Text,
                " count bonus order");
            VerifyAreEqual("Ожидание платежа",
                Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[10].Text, " complite order");

            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.Id("viewTableAdditionalBonus")).FindElements(By.TagName("td"))[0].Text,
                " no  bonus table additional");
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[0].Text,
                " no  bonus table transaction");

            GoToClient("myaccount?tab=bonusTab");

            VerifyAreEqual("0",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"bonusInfo.bonusData.BonusAmount\"]")).Text,
                " amount Bonus in lk");
            VerifyAreEqual("30",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"bonusInfo.bonusData.BonusPercent\"]")).Text,
                " percent Bonus in lk");
            VerifyAreEqual(numberCart,
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"bonusInfo.bonusData.CardNumber\"]")).Text,
                " number BonusCart in lk");
        }

        [Order(2)]
        [Test]
        public void CheckInAdminPay()
        {
            GoToAdmin("orders");
            VerifyAreEqual("3", Driver.GetGridCell(0, "Number").Text, "Grid Gifts Number");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, " Grid orders BuyerName");

            Driver.GetGridCell(0, "Number").Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));

            //VerifyAreEqual("Ad", driver.FindElement(By.Id("Order_OrderCustomer_LastName")).GetAttribute("value"), "\r\n surename in cart");
            //VerifyAreEqual("Admin", driver.FindElement(By.Id("Order_OrderCustomer_FirstName")).GetAttribute("value"), "\r\n name in cart");
            VerifyAreEqual("Ad Admin", Driver.FindElement(By.CssSelector("[data-e2e=\"FullNameCustomer\"] a")).Text,
                "\r\n name and surname in cart");

            IWebElement selectElem = Driver.FindElement(By.Id("Order_OrderSourceId"));
            SelectElement select = new SelectElement(selectElem);
            VerifyAreEqual("Корзина интернет магазина", (select.AllSelectedOptions[0].Text), "\r\n Source");

            VerifyAreEqual("TestProduct1\r\nАртикул: 1", Driver.GetGridCell(0, "Name", "OrderItems").Text,
                " Name product at order");
            VerifyAreEqual("100", Driver.GetGridCell(0, "PriceString", "OrderItems").Text.ToString(),
                " product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").Text.ToString(),
                " Count product at order");
            //VerifyAreEqual("в наличии", Driver.GetGridCell(0, "Available", "OrderItems").Text, " Available product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "OrderItems").Text.Contains("100 "), " Cost product at order");

            VerifyAreEqual(numberCart, Driver.FindElement(By.CssSelector("[data-e2e=\"numberBonusCart\"]")).Text,
                " number Cart in order");
            VerifyAreEqual("Баллы", Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text,
                " product at order");
            VerifyAreEqual("0 (Платиновый 30 %)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"percentBonus\"]")).Text, " percent Bonus in order");
            Refresh();
            Driver.FindElement(By.CssSelector(".switcher-state-label span")).Click();
            Thread.Sleep(100);
            Refresh();

            VerifyAreEqual(numberCart, Driver.FindElement(By.CssSelector("[data-e2e=\"numberBonusCart\"]")).Text,
                " Name product at order");
            VerifyAreEqual("Баллы", Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text,
                " product at order");
            VerifyAreEqual("30 (Платиновый 30 %)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"percentBonus\"]")).Text, " percent Bonus in order");
        }

        [Order(3)]
        [Test]
        public void CheckInLkAfterPay()
        {
            GoToClient("myaccount?tab=bonusTab");
            VerifyAreEqual("30",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"bonusInfo.bonusData.BonusAmount\"]")).Text,
                " amount Bonus in lk");
            VerifyAreEqual("30",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"bonusInfo.bonusData.BonusPercent\"]")).Text,
                " percent Bonus in lk");
            VerifyAreEqual(numberCart,
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"bonusInfo.bonusData.CardNumber\"]")).Text,
                " number BonusCart in lk");

            GoToAdmin("cards");

            numberCart = Driver.GetGridCell(0, "CardNumber").Text;
            VerifyAreEqual("Ad Admin", Driver.GetGridCell(0, "FIO").Text, " Grid  BuyerName");
            VerifyAreEqual("Платиновый", Driver.GetGridCell(0, "GradeName").Text, " Grid graid ");
            VerifyAreEqual("30", Driver.GetGridCell(0, "GradePersent").Text, " Grid percent");

            Driver.GetGridCell(0, "CardNumber").Click();
            Driver.WaitForElem(By.ClassName("balance__padding-page"));

            VerifyAreEqual(numberCart, Driver.FindElement(By.CssSelector("[data-e2e=\"numberCardBonus\"]")).Text,
                " in str card");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"gradeBonus\"] select"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Платиновый"), "select item Platin");

            VerifyAreEqual("30 бонусов\r\n30 основных и 0 дополнительных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus")).Text, " in str card");

            VerifyAreEqual("Ad", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardSuname\"]")).Text,
                " bonusCardSuname in str card");
            VerifyAreEqual("Admin", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardName\"]")).Text,
                " bonusCardName in str card");

            VerifyAreEqual("1", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[0].Text,
                " number operition table sells");
            VerifyAreEqual("3", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[1].Text,
                " number order table sells");
            VerifyAreEqual("100", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[3].Text,
                " sum order table sells");
            VerifyAreEqual("30", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[8].Text,
                " count bonus order table sells");
            VerifyAreEqual("Завершена",
                Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[10].Text,
                " complite order table sells");


            VerifyAreEqual("+30",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[2].Text,
                " count bonus  table transaction");
            VerifyAreEqual("30",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[3].Text,
                "  saldo table transaction");
            VerifyAreEqual("1",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[8].Text,
                " number sell table transaction");
        }

        [Order(4)]
        [Test]
        public void CheckPaymentByBonus()
        {
            ReInitClient();
            Refresh();
            ProductToCard("+30 руб. на бонусную карту");
            GoToClient("cart");
            VerifyAreEqual("Сумма баллов начисляемых на бонусную карту: +30 руб.",
                Driver.FindElement(By.CssSelector(".bonus-card-block-inline")).Text, "icon adn text bonus card ");
            VerifyAreEqual("30 руб.", Driver.FindElements(By.CssSelector(".bonus-card-block-inline span>span"))[1].Text,
                "Count bonus to card");

            Driver.ScrollTo(By.CssSelector(".bonus-card-block-inline span"));
            Driver.FindElement(By.CssSelector("[data-ng-href=\"checkout\"]")).Click();
            Thread.Sleep(100);
            Refresh();
            Driver.WaitForElem(By.ClassName("checkout-page"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".bonus-card-plus-price")).Count == 0,
                "Undisplay sum bonus");
            VerifyIsTrue(Driver.FindElements(By.Id("email")).Count == 0, "unDisplay email registration");
            VerifyIsTrue(Driver.FindElements(By.Id("password")).Count == 0, "unDisplay password registration");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".checkout-usertype-label input")).Selected,
                "Unselect checkBox registration");
            //Сделать!!!
            Driver.FindElement(By.CssSelector(".checkout-usertype-label span")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-usertype-label input")).Selected,
                "select checkBox registration");
            VerifyIsTrue(Driver.FindElements(By.Id("email")).Count == 1, "Display email registration");
            VerifyIsTrue(Driver.FindElements(By.Id("password")).Count == 1, "Display password registration");

            Driver.FindElement(By.Id("email")).SendKeys("admin");
            Driver.FindElement(By.Id("password")).SendKeys("123123");
            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_WantRegist")).Count == 0,
                "Display checkBox registration");
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Driver.WaitForElem(By.ClassName("address-list-item"));

            Driver.ScrollTo(By.Id("isBonusApply"));
            VerifyAreEqual("Бонусами по карте (у вас 30,0 бонусов)",
                Driver.FindElement(By.CssSelector(".custom-input-text")).Text,
                "bonus count in checkout checkbox. Platin");
            Driver.FindElement(By.CssSelector(".bonus-form-label span")).Click();
            Thread.Sleep(1000);

            Driver.XPathContainsText("span", "Самовывоз");
            Driver.ScrollTo(By.CssSelector(".footer-menu-head"));
            VerifyAreEqual("70 руб.", Driver.FindElement(By.CssSelector(".checkout-result-price")).Text,
                " checkout rezult. Platin");
            VerifyAreEqual("21 руб.",
                Driver.FindElement(By.CssSelector(".checkout-bonus-result"))
                    .FindElement(By.CssSelector(".checkout-result-price")).Text,
                "bonus count in checkout rezult. Platin");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("21 бонус", Driver.FindElement(By.TagName("h2")).Text, "Count bonus after order");

            GoToAdmin("cards");
            Driver.GetGridCell(0, "CardNumber").Click();
            Driver.WaitForElem(By.ClassName("balance__padding-page"));

            VerifyAreEqual("0 бонусов\r\n0 основных и 0 дополнительных",
                Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus")).Text, " in str card");
            VerifyAreEqual("Ad", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardSuname\"]")).Text,
                " bonusCardSuname in str card");
            VerifyAreEqual("Admin", Driver.FindElement(By.CssSelector("[data-e2e=\"bonusCardName\"]")).Text,
                " bonusCardName in str card");
            VerifyAreEqual("2", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[0].Text,
                " number operition");
            VerifyAreEqual("4", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[1].Text,
                " number order");
            VerifyAreEqual("100", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[3].Text,
                " sum order");
            VerifyAreEqual("70", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[5].Text,
                " count pay money order");
            VerifyAreEqual("30", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[6].Text,
                " count pay bonus order");
            VerifyAreEqual("21", Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[8].Text,
                " count bonus order");
            VerifyAreEqual("Ожидание платежа",
                Driver.FindElement(By.Id("viewTableSells")).FindElements(By.TagName("td"))[10].Text, " complite order");

            VerifyAreEqual("-30",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[1].Text,
                " count bonus  table transaction");
            VerifyAreEqual("0",
                Driver.FindElement(By.Id("viewTableTransaction")).FindElements(By.TagName("td"))[3].Text,
                "  saldo table transaction");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");
            VerifyAreEqual("Admin Ad", Driver.GetGridCell(0, "BuyerName").Text, " Grid orders BuyerName");

            Driver.GetGridCell(0, "Number").Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));
            //VerifyAreEqual("Ad", driver.FindElement(By.Id("Order_OrderCustomer_LastName")).GetAttribute("value"), " surename in cart");
            //VerifyAreEqual("Admin", driver.FindElement(By.Id("Order_OrderCustomer_FirstName")).GetAttribute("value"), " name in cart");
            VerifyAreEqual("Ad Admin", Driver.FindElement(By.CssSelector("[data-e2e=\"FullNameCustomer\"] a")).Text,
                "name and surname in cart");
            VerifyAreEqual("Баллы", Driver.FindElement(By.CssSelector("[data-e2e=\"countBonus\"]")).Text,
                " product at order");
            VerifyAreEqual("0 (Платиновый 30 %)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"percentBonus\"]")).Text, " percent Bonus in order");
            string str = Driver.FindElement(By.TagName("order-items-summary")).Text;
            VerifyIsTrue(Driver.FindElement(By.TagName("order-items-summary")).Text.Contains("Бонусы\r\n- 30"),
                " bonus from order");

            GoToClient("myaccount?tab=bonusTab");

            VerifyAreEqual("0",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"bonusInfo.bonusData.BonusAmount\"]")).Text,
                " amount Bonus in lk");
            VerifyAreEqual("30",
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"bonusInfo.bonusData.BonusPercent\"]")).Text,
                " percent Bonus in lk");
            VerifyAreEqual(numberCart,
                Driver.FindElement(By.CssSelector("[data-ng-bind=\"bonusInfo.bonusData.CardNumber\"]")).Text,
                " number BonusCart in lk");
        }

        [Order(5)]
        [Test]
        public void NewClientCheck()
        {
            ReInitClient();
            Refresh();
            ProductToCard("+30 руб. на бонусную карту");
            GoToClient("cart");
            VerifyAreEqual("Сумма баллов начисляемых на бонусную карту: +30 руб.",
                Driver.FindElement(By.CssSelector(".bonus-card-block-inline")).Text, "icon adn text bonus card ");
            VerifyAreEqual("30 руб.", Driver.FindElements(By.CssSelector(".bonus-card-block-inline span>span"))[1].Text,
                "Count bonus to card");

            Driver.ScrollTo(By.CssSelector(".bonus-card-block-inline span"));
            Driver.FindElement(By.CssSelector("[data-ng-href=\"checkout\"]")).Click();
            Thread.Sleep(1000);
            Refresh();
            Thread.Sleep(1000);
            Driver.FindElement(By.Id("Data_User_Email")).SendKeys("newlogin@log.in");
            Driver.FindElement(By.Id("Data_User_FirstName")).SendKeys("newname");
            Driver.FindElement(By.Id("Data_User_LastName")).SendKeys("newsuname");
            Driver.FindElement(By.Id("Data_User_Phone")).SendKeys(Keys.Control + "a" + Keys.Delete);
            Driver.FindElement(By.Id("Data_User_Phone")).SendKeys("79998887766");

            VerifyIsTrue(Driver.FindElements(By.Id("Data_User_WantRegist")).Count == 1,
                "Display checkBox registration");
            VerifyIsTrue(Driver.FindElements(By.Id("password")).Count == 0, "Undisplay checkBox registration");
            VerifyIsTrue(Driver.FindElements(By.Id("passwordRepeat")).Count == 0, "Undisplay checkBox registration");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".bonus-card-plus-price")).Count == 0,
                "Undisplay sum bonus");
            VerifyAreEqual("30", Driver.FindElement(By.CssSelector(".nowrap")).Text, "display sum bonus in checkBox");

            VerifyIsFalse(Driver.FindElement(By.Id("Data_User_WantRegist")).Selected, "Unselect checkBox registration");

            Driver.FindElements(By.CssSelector(".custom-input-checkbox"))[1].Click();
            VerifyIsTrue(Driver.FindElement(By.Id("Data_User_WantRegist")).Selected, "Select checkBox registration");
            VerifyIsTrue(Driver.FindElements(By.Id("password")).Count == 1, "Display checkBox registration");
            VerifyIsTrue(Driver.FindElements(By.Id("passwordRepeat")).Count == 1, "Display checkBox registration");

            Driver.FindElement(By.Id("password")).SendKeys("123123");
            Driver.FindElement(By.Id("passwordRepeat")).SendKeys("123123");

            Driver.ScrollTo(By.Id("passwordRepeat"));
            Driver.XPathContainsText("span", "Самовывоз");
            Driver.ScrollTo(By.Name("CustomerComment"));

            VerifyAreEqual("30 руб.",
                Driver.FindElement(By.CssSelector(".checkout-bonus-result"))
                    .FindElement(By.CssSelector(".checkout-result-price")).Text,
                "bonus count in checkout rezult. Platin");

            Driver.ScrollTo(By.CssSelector(".footer-menu-head"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
            VerifyIsTrue(Driver.Url.Contains("checkout/success"), "success url");

            VerifyAreEqual("30 бонусов", Driver.FindElement(By.TagName("h2")).Text, "Count bonus after order");
        }

        public void ProductToCard(string bonus)
        {
            GoToClient("products/test-product1");
            if (Driver.FindElement(By.CssSelector(".cart-mini a")).Text.Contains("пусто"))
            {
                Refresh();
                Driver.ScrollTo(By.CssSelector(".rating"));
                VerifyAreEqual(bonus, Driver.FindElement(By.CssSelector(".bonus-string-sum")).Text,
                    "Count bonus in product cart");
                Driver.FindElement(By.CssSelector(".details-payment-inline a")).Click();
                Driver.WaitForElem(By.ClassName("cart-mini-block"));
            }
        }
    }
}