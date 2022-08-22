using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MainPage.Tests.Indicators
{
    [TestFixture]
    public class IndicatorsOrdersMainPageAdminTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.CMS | ClearType.CRM | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\MainPage\\IndicatorsTest\\Catalog.Product.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\Catalog.Offer.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\Catalog.Category.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\Catalog.ProductCategories.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\[Order].OrderSource.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\[Order].OrderStatus.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\[Order].[Order].csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\[Order].OrderContact.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\[Order].OrderCurrency.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\[Order].OrderItems.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\[Order].Lead.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\[Order].LeadItem.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\[Order].LeadCurrency.csv"
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

        /*orders today tests*/
        [Test]
        public void IndicatorsNoBeforeOrdersToday() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersTodayCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Заказов сегодня",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersTodayCount\"]")).Text);
            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"OrdersTodayCount\"]")).Text);
            VerifyAreEqual("55 руб.",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"OrdersTodayCount\"]")).Text);
        }

        [Test]
        public void IndicatorsOrdersToday() //выбор индикатора через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersTodayCount\"]")).Selected
            )
            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Заказов сегодня",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersTodayCount\"]")).Text);
                VerifyAreEqual("10",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"OrdersTodayCount\"]")).Text);
                VerifyAreEqual("55 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"OrdersTodayCount\"]")).Text);
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersTodayCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Заказов сегодня",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersTodayCount\"]")).Text);
                VerifyAreEqual("10",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"OrdersTodayCount\"]")).Text);
                VerifyAreEqual("55 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"OrdersTodayCount\"]")).Text);
            }
        }

        /*orders yesterday tests*/
        [Test]
        public void IndicatorsNoBeforeOrdersYesterday() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersYesterdayCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Заказов вчера",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersYesterdayCount\"]")).Text);
            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"OrdersYesterdayCount\"]")).Text);
            VerifyAreEqual("155 руб.",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"OrdersYesterdayCount\"]")).Text);
        }

        [Test]
        public void IndicatorsOrdersYesterday() //выбор индикатора через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if
            (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersYesterdayCount\"]")).Selected
            )
            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Заказов вчера",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersYesterdayCount\"]")).Text);
                VerifyAreEqual("10",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"OrdersYesterdayCount\"]")).Text);
                VerifyAreEqual("155 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"OrdersYesterdayCount\"]")).Text);
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersYesterdayCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Заказов вчера",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersYesterdayCount\"]")).Text);
                VerifyAreEqual("10",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"OrdersYesterdayCount\"]")).Text);
                VerifyAreEqual("155 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"OrdersYesterdayCount\"]")).Text);
            }
        }

        /*orders month tests*/
        [Test]
        public void IndicatorsNoBeforeOrdersMonth() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersMonthCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Заказов за месяц",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersMonthCount\"]")).Text);
            VerifyAreEqual("20",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"OrdersMonthCount\"]")).Text);
            VerifyAreEqual("210 руб.",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"OrdersMonthCount\"]")).Text);
        }

        [Test]
        public void IndicatorsOrdersMonth() //выбор индикатора через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if
            (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersMonthCount\"]")).Selected
            )
            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Заказов за месяц",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersMonthCount\"]")).Text);
                VerifyAreEqual("20",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"OrdersMonthCount\"]")).Text);
                VerifyAreEqual("210 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"OrdersMonthCount\"]")).Text);
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersMonthCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Заказов за месяц",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersMonthCount\"]")).Text);
                VerifyAreEqual("20",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"OrdersMonthCount\"]")).Text);
                VerifyAreEqual("210 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"OrdersMonthCount\"]")).Text);
            }
        }

        /*orders all time tests*/
        [Test]
        public void IndicatorsNoBeforeOrdersAllTime() //no indicators before
        {
            GoToAdmin("home");

            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersAllTimeCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Заказов за все время",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersAllTimeCount\"]")).Text);
            VerifyAreEqual("20",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"OrdersAllTimeCount\"]")).Text);
            VerifyAreEqual("210 руб.",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"OrdersAllTimeCount\"]")).Text);
        }

        [Test]
        public void IndicatorsOrdersAllTime() //выбор индикатора через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if (!Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersAllTimeCount\"]")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersAllTimeCount\"]")).Click();
            }

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();


            //не учитываются в индикаторах заказы со статусами "Отменен", "Отменен навсегда"
            VerifyAreEqual("Заказов за все время",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersAllTimeCount\"]")).Text);
            VerifyAreEqual("20",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"OrdersAllTimeCount\"]")).Text);
            VerifyAreEqual("210 руб.",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"OrdersAllTimeCount\"]")).Text);
        }
    }
}