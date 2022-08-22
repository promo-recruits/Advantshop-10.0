using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MainPage.Tests.Indicators
{
    [TestFixture]
    public class IndicatorsProductsMainPageAdminTest : BaseSeleniumTest
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

        /*products tests*/
        [Test]
        public void IndicatorsNoBeforeProducts() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            VerifyAreEqual("Индикаторы",
                Driver.FindElement(By.CssSelector("[data-e2e-header=\"IndicatorsTitle\"]")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ProductsCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Всего товаров",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ProductsCount\"]")).Text);
            VerifyAreEqual("100",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ProductsCount\"]")).Text);
            VerifyAreEqual("штук",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"ProductsCount\"]")).Text);
        }

        [Test]
        public void IndicatorsProducts() //открытие pop up с индикаторами через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            VerifyAreEqual("Индикаторы",
                Driver.FindElement(By.CssSelector("[data-e2e-header=\"IndicatorsTitle\"]")).Text);
            if (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ProductsCount\"]")).Selected
            )
            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Всего товаров",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ProductsCount\"]")).Text);
                VerifyAreEqual("100",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ProductsCount\"]")).Text);
                VerifyAreEqual("штук",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"ProductsCount\"]")).Text);
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ProductsCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Всего товаров",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ProductsCount\"]")).Text);
                VerifyAreEqual("100",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"ProductsCount\"]")).Text);
                VerifyAreEqual("штук",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-quantity=\"ProductsCount\"]")).Text);
            }
        }

        /* close indicators pop up */
        [Test]
        public void IndicatorsNoBeforeNotChooseClose() //no indicators before and do not choose
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Отмена')]")).Click();

            VerifyAreEqual("Выбрать индикаторы",
                Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Text);
        }

        /* more than 1 indicators */
        [Test]
        public void IndicatorsNoBeforeChooseMoreThanOne() //no indicators before and do not choose
        {
            Refresh();
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"ProductsCount\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"LeadsYesterdayCount\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"OrdersTodayCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();


            VerifyAreEqual("Заказов сегодня",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"OrdersTodayCount\"]")).Text);
            VerifyAreEqual("Всего товаров",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"ProductsCount\"]")).Text);
            VerifyAreEqual("Лиды вчера",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"LeadsYesterdayCount\"]")).Text);
        }
    }
}