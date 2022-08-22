using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MainPage.Tests.Indicators
{
    [TestFixture]
    public class IndicatorsLeadsMainPageAdminTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders | ClearType.Catalog | ClearType.CMS | ClearType.CRM);
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
                "data\\Admin\\MainPage\\IndicatorsTest\\[Order].LeadCurrency.csv",
                "data\\Admin\\MainPage\\IndicatorsTest\\CMS.Review.csv"
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

        /*leads today tests*/
        [Test]
        public void IndicatorsNoBeforeLeadsToday() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsTodayCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Лиды сегодня",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"LeadsTodayCount\"]")).Text);
            VerifyAreEqual("1",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"LeadsTodayCount\"]")).Text);
            VerifyAreEqual("10 руб.",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"LeadsTodayCount\"]")).Text);
        }

        [Test]
        public void IndicatorsLeadsToday() //выбор индикатора через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if
            (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsTodayCount\"]")).Selected
            )
            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Лиды сегодня",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"LeadsTodayCount\"]")).Text);
                VerifyAreEqual("1",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"LeadsTodayCount\"]")).Text);
                VerifyAreEqual("10 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"LeadsTodayCount\"]")).Text);
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsTodayCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Лиды сегодня",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"LeadsTodayCount\"]")).Text);
                VerifyAreEqual("1",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"LeadsTodayCount\"]")).Text);
                VerifyAreEqual("10 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"LeadsTodayCount\"]")).Text);
            }
        }

        /*leads yesterday tests*/
        [Test]
        public void IndicatorsNoBeforeLeadsYesterday() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsYesterdayCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Лиды вчера",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"LeadsYesterdayCount\"]")).Text);
            VerifyAreEqual("2",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"LeadsYesterdayCount\"]")).Text);
            VerifyAreEqual("23 руб.",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"LeadsYesterdayCount\"]")).Text);
        }

        [Test]
        public void IndicatorsLeadsYesterday() //выбор индикатора через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if
            (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsYesterdayCount\"]")).Selected
            )
            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Лиды вчера",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"LeadsYesterdayCount\"]")).Text);
                VerifyAreEqual("2",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"LeadsYesterdayCount\"]")).Text);
                VerifyAreEqual("23 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"LeadsYesterdayCount\"]")).Text);
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsYesterdayCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Лиды вчера",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"LeadsYesterdayCount\"]")).Text);
                VerifyAreEqual("2",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"LeadsYesterdayCount\"]")).Text);
                VerifyAreEqual("23 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"LeadsYesterdayCount\"]")).Text);
            }
        }

        /*leads month tests*/
        [Test]
        public void IndicatorsNoBeforeLeadsMonth() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsMonthCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Лиды за месяц",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"LeadsMonthCount\"]")).Text);
            VerifyAreEqual("3",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"LeadsMonthCount\"]")).Text);
            VerifyAreEqual("33 руб.",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"LeadsMonthCount\"]")).Text);
        }

        [Test]
        public void IndicatorsLeadsMonth() //выбор индикатора через ссылку в верхнем правом углу
        {
            GoToAdmin("home");
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if
            (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsMonthCount\"]")).Selected
            )
            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Лиды за месяц",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"LeadsMonthCount\"]")).Text);
                VerifyAreEqual("3",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"LeadsMonthCount\"]")).Text);
                VerifyAreEqual("33 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"LeadsMonthCount\"]")).Text);
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsMonthCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Лиды за месяц",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"LeadsMonthCount\"]")).Text);
                VerifyAreEqual("3",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"LeadsMonthCount\"]")).Text);
                VerifyAreEqual("33 руб.",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"LeadsMonthCount\"]")).Text);
            }
        }
    }
}