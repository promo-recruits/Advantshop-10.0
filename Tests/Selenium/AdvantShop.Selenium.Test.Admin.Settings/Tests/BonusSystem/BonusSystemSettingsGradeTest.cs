using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem
{
    [TestFixture]
    public class BonusSystemSettingsGradeTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\Catalog.ProductCategories.csv"
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

        [Test]
        public void CheckaGuestBonusCart()
        {
            GoToAdmin("settingsbonus");
            VerifyIsFalse(Is404Page("settingsbonus"), " 404 error");
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Бонусная программа"), "h1 page edit");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".balance-block h2")).Text.Contains("Бонусная система"),
                "h1 bonus edit");

            (new SelectElement(Driver.FindElement(By.Id("BonusGradeId")))).SelectByText("Гостевой");
            Driver.FindElement(By.Id("CardNumTo")).Click();
            Driver.FindElement(By.Id("CardNumTo")).SendKeys("0");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();

            IWebElement selectElem1 = Driver.FindElement(By.Id("BonusGradeId"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Гостевой"), "select item Guess");

            ProductToCard("+3 руб. на бонусную карту");
            GoToClient("cart");
            VerifyAreEqual("Сумма баллов начисляемых на бонусную карту: +3 руб.",
                Driver.FindElement(By.CssSelector(".bonus-card-block-inline")).Text, "icon adn text bonus card ");
            VerifyAreEqual("3 руб.", Driver.FindElements(By.CssSelector(".bonus-card-block-inline span>span"))[1].Text,
                "Count bonus to card");
        }

        [Test]
        public void CheckBronseBonusCart()
        {
            GoToAdmin("settingsbonus");
            (new SelectElement(Driver.FindElement(By.Id("BonusGradeId")))).SelectByText("Бронзовый");
            Driver.FindElement(By.Id("CardNumFrom")).Clear();
            Driver.FindElement(By.Id("CardNumFrom")).SendKeys("20000");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();

            IWebElement selectElem1 = Driver.FindElement(By.Id("BonusGradeId"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Бронзовый"), "select item Bronse");
            ProductToCard("+5 руб. на бонусную карту");
            GoToClient("cart");
            VerifyAreEqual("Сумма баллов начисляемых на бонусную карту: +5 руб.",
                Driver.FindElement(By.CssSelector(".bonus-card-block-inline")).Text, "icon adn text bonus card ");
            VerifyAreEqual("5 руб.", Driver.FindElements(By.CssSelector(".bonus-card-block-inline span>span"))[1].Text,
                "Count bonus to card");
        }

        [Test]
        public void CheckSilverBonusCart()
        {
            GoToAdmin("settingsbonus");
            (new SelectElement(Driver.FindElement(By.Id("BonusGradeId")))).SelectByText("Серебряный");
            Driver.FindElement(By.Id("CardNumFrom")).Clear();
            Driver.FindElement(By.Id("CardNumFrom")).SendKeys("20000");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();
            IWebElement selectElem1 = Driver.FindElement(By.Id("BonusGradeId"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Серебряный"), "select item Silver");
            ProductToCard("+7 руб. на бонусную карту");
            GoToClient("cart");
            VerifyAreEqual("Сумма баллов начисляемых на бонусную карту: +7 руб.",
                Driver.FindElement(By.CssSelector(".bonus-card-block-inline")).Text, "icon adn text bonus card ");
            VerifyAreEqual("7 руб.", Driver.FindElements(By.CssSelector(".bonus-card-block-inline span>span"))[1].Text,
                "Count bonus to card");
        }

        [Test]
        public void ChecktGoldBonusCart()
        {
            GoToAdmin("settingsbonus");
            (new SelectElement(Driver.FindElement(By.Id("BonusGradeId")))).SelectByText("Золотой");
            Driver.FindElement(By.Id("CardNumFrom")).Clear();
            Driver.FindElement(By.Id("CardNumFrom")).SendKeys("20000");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();

            IWebElement selectElem1 = Driver.FindElement(By.Id("BonusGradeId"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Золотой"), "select item Gold");
            ProductToCard("+10 руб. на бонусную карту");
            GoToClient("cart");
            VerifyAreEqual("Сумма баллов начисляемых на бонусную карту: +10 руб.",
                Driver.FindElement(By.CssSelector(".bonus-card-block-inline")).Text, "icon adn text bonus card ");
            VerifyAreEqual("10 руб.", Driver.FindElements(By.CssSelector(".bonus-card-block-inline span>span"))[1].Text,
                "Count bonus to card");
        }

        [Test]
        public void ChecktPlatinBonusCart()
        {
            GoToAdmin("settingsbonus");
            (new SelectElement(Driver.FindElement(By.Id("BonusGradeId")))).SelectByText("Платиновый");
            Driver.FindElement(By.Id("CardNumFrom")).Clear();
            Driver.FindElement(By.Id("CardNumFrom")).SendKeys("20000");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsBonusSave\"]")).Click();
            Driver.WaitForToastSuccess();
            IWebElement selectElem1 = Driver.FindElement(By.Id("BonusGradeId"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.AllSelectedOptions[0].Text.Contains("Платиновый"), "select item Platin");

            ProductToCard("+30 руб. на бонусную карту");
            GoToClient("cart");
            VerifyAreEqual("Сумма баллов начисляемых на бонусную карту: +30 руб.",
                Driver.FindElement(By.CssSelector(".bonus-card-block-inline")).Text, "icon and text bonus card ");
            VerifyAreEqual("30 руб.", Driver.FindElements(By.CssSelector(".bonus-card-block-inline span>span"))[1].Text,
                "Count bonus to card");
        }

        public void ProductToCard(string bonus)
        {
            GoToClient("products/test-product1");
            if (Driver.FindElement(By.CssSelector(".cart-mini a")).Text.Contains("пусто"))
            {
                Driver.ScrollTo(By.CssSelector(".rating"));
                VerifyAreEqual(bonus, Driver.FindElement(By.CssSelector(".bonus-string-sum")).Text,
                    "Count bonus in product cart");
                Driver.FindElement(By.CssSelector(".details-payment-inline a")).Click();
                Driver.WaitForElem(By.ClassName("cart-mini-block"));
            }
        }
    }
}