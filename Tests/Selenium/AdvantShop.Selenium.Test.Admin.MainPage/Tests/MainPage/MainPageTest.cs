using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MainPage.Tests.MainPage
{
    [TestFixture]
    public class MainPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders | ClearType.Catalog | ClearType.CRM | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\MainPage\\Page\\Catalog.Product.csv",
                "data\\Admin\\MainPage\\Page\\Catalog.Offer.csv",
                "data\\Admin\\MainPage\\Page\\Catalog.Category.csv",
                "data\\Admin\\MainPage\\Page\\Catalog.ProductCategories.csv",
                "data\\Admin\\MainPage\\Page\\Customers.CustomerGroup.csv",
                "data\\Admin\\MainPage\\Page\\Customers.Customer.csv",
                "data\\Admin\\MainPage\\Page\\Customers.Contact.csv",
                "data\\Admin\\MainPage\\Page\\Customers.Departments.csv",
                "data\\Admin\\MainPage\\Page\\Customers.Managers.csv",
                "data\\Admin\\MainPage\\Page\\[Order].OrderSource.csv",
                "data\\Admin\\MainPage\\Page\\[Order].OrderStatus.csv",
                "data\\Admin\\MainPage\\Page\\[Order].[Order].csv",
                "data\\Admin\\MainPage\\Page\\[Order].OrderContact.csv",
                "data\\Admin\\MainPage\\Page\\[Order].OrderCurrency.csv",
                "data\\Admin\\MainPage\\Page\\[Order].OrderItems.csv",
                "data\\Admin\\MainPage\\Page\\[Order].Lead.csv",
                "data\\Admin\\MainPage\\Page\\[Order].LeadCurrency.csv",
                "data\\Admin\\MainPage\\Page\\[Order].LeadItem.csv"
            );

            Init();
            Functions.SetAdminStartPage(Driver, BaseUrl, "desktop");
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin();
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        [Order(1)]
        public void MainPageOrderStatus()
        {
            VerifyAreEqual("Все заказы", Driver.FindElement(By.CssSelector("[data-e2e=\"status-id-0\"]")).Text,
                "orders all");
            VerifyAreEqual("Новый", Driver.FindElement(By.CssSelector("[data-e2e=\"status-id-2\"]")).Text,
                "orders new");
            VerifyAreEqual("В обработке", Driver.FindElement(By.CssSelector("[data-e2e=\"status-id-3\"]")).Text,
                "orders check");
            VerifyAreEqual("Отправлен", Driver.FindElement(By.CssSelector("[data-e2e=\"status-id-4\"]")).Text,
                "orders shipping");
            VerifyAreEqual("Доставлен", Driver.FindElement(By.CssSelector("[data-e2e=\"status-id-5\"]")).Text,
                "orders teke");
            VerifyAreEqual("Отменён", Driver.FindElement(By.CssSelector("[data-e2e=\"status-id-27\"]")).Text,
                "orders cancel");
            VerifyAreEqual("Отменен навсегда", Driver.FindElement(By.CssSelector("[data-e2e=\"status-id-28\"]")).Text,
                "orders cancel forever");

            VerifyAreEqual("30", Driver.FindElement(By.CssSelector("[data-e2e=\"count-status-id-0\"]")).Text,
                "count orders all");
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[data-e2e=\"count-status-id-2\"]")).Text,
                "count orders new");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"count-status-id-3\"]")).Text,
                "count orders check");
            VerifyAreEqual("4", Driver.FindElement(By.CssSelector("[data-e2e=\"count-status-id-4\"]")).Text,
                "count orders shipping");
            VerifyAreEqual("5", Driver.FindElement(By.CssSelector("[data-e2e=\"count-status-id-5\"]")).Text,
                "count orders teke");
            VerifyAreEqual("6", Driver.FindElement(By.CssSelector("[data-e2e=\"count-status-id-27\"]")).Text,
                "count orders cancel");
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"count-status-id-28\"]")).Text,
                "count orders cancel forever");

            Driver.FindElement(By.CssSelector("[data-e2e=\"status-id-0\"]")).Click();

            VerifyIsTrue(Driver.Url.Contains("/orders"), "url link");
            VerifyAreEqual("1", Driver.GetGridCellText(0, "Number"), "check first item num");
            VerifyAreEqual("Новый", Driver.GetGridCellText(0, "StatusName"), "check first item status");
            VerifyAreEqual("Найдено записей: 30",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all order on page");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"gridFilterBlock\"]")).Count == 0);

            Driver.FindElement(By.CssSelector(".logo-block-cell-logo")).Click();

            VerifyIsTrue(Driver.Url.EndsWith("/adminv3/home/desktop"), "url link main page");
            Driver.FindElement(By.CssSelector("[data-e2e=\"status-id-4\"]")).Click();

            VerifyIsTrue(Driver.Url.Contains("/orders"), "url link 2");
            VerifyAreEqual("6", Driver.GetGridCellText(0, "Number"), "check first item num 2");
            VerifyAreEqual("Отправлен", Driver.GetGridCellText(0, "StatusName"), "check first item status 2");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all order on page 2");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnStatuses\"]"))
                .Displayed);

            VerifyAreEqual("Отправлен", Driver.FindElement(By.CssSelector(".ui-select-match-text span")).Text,
                "right selected item");
        }

        [Test]
        [Order(2)]
        public void MainPageOrderSource()
        {
            VerifyAreEqual("Корзина интернет магазина",
                Driver.FindElement(By.CssSelector("[data-e2e=\"source-id-1\"]")).Text, "orders Cart");
            VerifyAreEqual("Оффлайн", Driver.FindElement(By.CssSelector("[data-e2e=\"source-id-2\"]")).Text,
                "orders Offline");
            VerifyAreEqual("В один клик", Driver.FindElement(By.CssSelector("[data-e2e=\"source-id-3\"]")).Text,
                "orders OneClick");
            VerifyAreEqual("Посадочная страница", Driver.FindElement(By.CssSelector("[data-e2e=\"source-id-4\"]")).Text,
                "orders LandingPage");
            VerifyAreEqual("Мобильная версия", Driver.FindElement(By.CssSelector("[data-e2e=\"source-id-5\"]")).Text,
                "orders Mobile");
            VerifyAreEqual("По телефону", Driver.FindElement(By.CssSelector("[data-e2e=\"source-id-6\"]")).Text,
                "orders Phone");
            VerifyAreEqual("Онлайн консультант", Driver.FindElement(By.CssSelector("[data-e2e=\"source-id-7\"]")).Text,
                "orders LiveChat");
            VerifyAreEqual("Социальные сети", Driver.FindElement(By.CssSelector("[data-e2e=\"source-id-8\"]")).Text,
                "orders SocialNetworks");
            VerifyAreEqual("Нашли дешевле", Driver.FindElement(By.CssSelector("[data-e2e=\"source-id-9\"]")).Text,
                "orders FindCheaper");
            VerifyAreEqual("Брошенные корзины", Driver.FindElement(By.CssSelector("[data-e2e=\"source-id-10\"]")).Text,
                "orders AbandonedCart");

            VerifyAreEqual("20%", Driver.FindElement(By.CssSelector("[data-e2e=\"percent-source-id-1\"]")).Text,
                "percent orders Cart");
            VerifyAreEqual("17%", Driver.FindElement(By.CssSelector("[data-e2e=\"percent-source-id-2\"]")).Text,
                "percent orders Offline");
            VerifyAreEqual("13%", Driver.FindElement(By.CssSelector("[data-e2e=\"percent-source-id-3\"]")).Text,
                "percent orders OneClick");
            VerifyAreEqual("13%", Driver.FindElement(By.CssSelector("[data-e2e=\"percent-source-id-4\"]")).Text,
                "percent orders LandingPage");
            VerifyAreEqual("10%", Driver.FindElement(By.CssSelector("[data-e2e=\"percent-source-id-5\"]")).Text,
                "percent orders Mobile");
            VerifyAreEqual("10%", Driver.FindElement(By.CssSelector("[data-e2e=\"percent-source-id-6\"]")).Text,
                "percent orders Phone");
            VerifyAreEqual("3%", Driver.FindElement(By.CssSelector("[data-e2e=\"percent-source-id-7\"]")).Text,
                "percent orders LiveChat");
            VerifyAreEqual("3%", Driver.FindElement(By.CssSelector("[data-e2e=\"percent-source-id-8\"]")).Text,
                "percent orders SocialNetworks");
            VerifyAreEqual("3%", Driver.FindElement(By.CssSelector("[data-e2e=\"percent-source-id-9\"]")).Text,
                "percent orders FindCheaper");
            VerifyAreEqual("3%", Driver.FindElement(By.CssSelector("[data-e2e=\"percent-source-id-10\"]")).Text,
                "percent orders AbandonedCart");
        }

        [Test]
        [Order(4)]
        public void MainPageOrders()
        {
            Driver.ScrollTo(By.TagName("footer"));

            VerifyAreEqual("1", Driver.GetGridCellText(0, "№", "AllOrders"), "check first item num");
            VerifyAreEqual("7", Driver.GetGridCellText(6, "№", "AllOrders"), "check last item num");
            VerifyAreEqual(" Новый", Driver.GetGridCellText(0, "StatusName", "AllOrders"), "check first item status");
            VerifyAreEqual(" Отправлен", Driver.GetGridCellText(6, "StatusName", "AllOrders"),
                "check last item status");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 7,
                "count all orders row");

            Driver.FindElement(By.CssSelector("[heading=\"Назначенные мне\"]")).Click();

            VerifyAreEqual("1", Driver.GetGridCellText(0, "№", "MyOrders"), "check my first item num");
            VerifyAreEqual("5", Driver.GetGridCellText(4, "№", "MyOrders"), "check my last item num");
            VerifyAreEqual(" Новый", Driver.GetGridCellText(0, "StatusName", "MyOrders"), "check first my item status");
            VerifyAreEqual(" В обработке", Driver.GetGridCellText(4, "StatusName", "MyOrders"),
                "check last my item status");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 5,
                "count my orders row");

            Driver.FindElement(By.CssSelector("[heading=\"Не назначены\"]")).Click();

            VerifyAreEqual("10", Driver.GetGridCellText(0, "№", "NotMyOrders"), "check alien first item num");
            VerifyAreEqual("16", Driver.GetGridCellText(6, "№", "NotMyOrders"), "check alien last item num");
            VerifyAreEqual(" Доставлен", Driver.GetGridCellText(0, "StatusName", "NotMyOrders"),
                "check first alien item status");
            VerifyAreEqual(" Отменён", Driver.GetGridCellText(6, "StatusName", "NotMyOrders"),
                "check last alien item status");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 7,
                "count alien orders row");

            Driver.FindElement(By.CssSelector("[heading=\"Все заказы\"]")).Click();

            VerifyAreEqual("1", Driver.GetGridCellText(0, "№", "AllOrders"), "check first item num");
            VerifyAreEqual("7", Driver.GetGridCellText(6, "№", "AllOrders"), "check last item num");
            VerifyAreEqual(" Новый", Driver.GetGridCellText(0, "StatusName", "AllOrders"), "check first item status");
            VerifyAreEqual(" Отправлен", Driver.GetGridCellText(6, "StatusName", "AllOrders"),
                "check last item status");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 7,
                "count all orders row 2");
        }

        [Test]
        [Order(4)]
        public void MainPagePlannedSales()
        {
            VerifyAreEqual("210 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"totalSales\"]")).Text,
                "total sales");
            VerifyAreEqual("Цель: 200 000 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"plannedSales\"]")).Text,
                "planned sales");

            GoToAdmin("settings/common#?indexTab=plan");
            Driver.FindElement(By.Id("SalesPlan")).Clear();
            Driver.FindElement(By.Id("SalesPlan")).SendKeys("1000000");

            Driver.FindElement(By.Id("ProfitPlan")).Clear();
            Driver.FindElement(By.Id("ProfitPlan")).SendKeys("100");

            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"] input")).Click();

            GoToAdmin();
            VerifyAreEqual("Цель: 1 000 000 руб.",
                Driver.FindElement(By.CssSelector("[data-e2e=\"plannedSales\"]")).Text, "planned sales");
        }
    }
}