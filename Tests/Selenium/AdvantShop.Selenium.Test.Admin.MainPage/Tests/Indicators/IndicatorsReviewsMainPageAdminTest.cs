using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MainPage.Tests.Indicators
{
    [TestFixture]
    public class IndicatorsReviewsMainPageAdminTest : BaseSeleniumTest
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

        /*reviews today tests*/
        [Test]
        public void IndicatorsNoBeforeReviewsToday() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsTodayCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Отзывов сегодня",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ReviewsTodayCount\"]")).Text);
            VerifyAreEqual("20",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ReviewsTodayCount\"]")).Text);
        }

        [Test]
        public void IndicatorsReviewsToday() //выбор индикатора через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if
            (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsTodayCount\"]")).Selected
            )
            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Отзывов сегодня",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ReviewsTodayCount\"]")).Text);
                VerifyAreEqual("20",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ReviewsTodayCount\"]")).Text);
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsTodayCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Отзывов сегодня",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ReviewsTodayCount\"]")).Text);
                VerifyAreEqual("20",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ReviewsTodayCount\"]")).Text);
            }
        }

        /*reviews yesterday tests*/
        [Test]
        public void IndicatorsNoBeforeReviewsYesterday() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsYesterdayCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Отзывов вчера",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ReviewsYesterdayCount\"]")).Text);
            VerifyAreEqual("31",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ReviewsYesterdayCount\"]")).Text);
        }

        [Test]
        public void IndicatorsReviewsYesterday() //выбор индикатора через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if
            (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsYesterdayCount\"]")).Selected
            )

            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Отзывов вчера",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ReviewsYesterdayCount\"]")).Text);
                VerifyAreEqual("31",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ReviewsYesterdayCount\"]")).Text);
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsYesterdayCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Отзывов вчера",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ReviewsYesterdayCount\"]")).Text);
                VerifyAreEqual("31",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ReviewsYesterdayCount\"]")).Text);
            }
        }

        /*reviews month tests*/
        [Test]
        public void IndicatorsNoBeforeReviewsMonth() //no indicators before
        {
            Refresh();
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsMonthCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Отзывы за месяц",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ReviewsMonthCount\"]")).Text);
            VerifyAreEqual("51",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ReviewsMonthCount\"]")).Text);
        }

        [Test]
        public void IndicatorsReviewsMonth() //выбор индикатора через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if
            (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsMonthCount\"]")).Selected
            )
            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Отзывы за месяц",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ReviewsMonthCount\"]")).Text);
                VerifyAreEqual("51",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ReviewsMonthCount\"]")).Text);
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ReviewsMonthCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Отзывы за месяц",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ReviewsMonthCount\"]")).Text);
                VerifyAreEqual("51",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ReviewsMonthCount\"]")).Text);
            }
        }
    }
}