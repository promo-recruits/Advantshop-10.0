using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MainPage.Tests.Indicators
{
    [TestFixture]
    public class IndicatorsCallsMainPageAdminTest : BaseSeleniumTest
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

        /*calls today tests*/
        [Test]
        public void IndicatorsNoBeforeCallsToday() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsTodayCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Звонков сегодня",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"CallsTodayCount\"]")).Text);
            VerifyAreEqual("0",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"CallsTodayCount\"]")).Text);
            VerifyAreEqual("Настроить телефонию",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsToday\"] a")).Text);
        }

        [Test]
        public void IndicatorsCallsToday() //выбор индикатора через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if
            (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsTodayCount\"]")).Selected
            )

            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Звонков сегодня",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"CallsTodayCount\"]")).Text);
                VerifyAreEqual("0",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"CallsTodayCount\"]")).Text);
                VerifyAreEqual("Настроить телефонию",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsToday\"] a"))
                        .Text);
                string linkToday = Driver
                    .FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsToday\"] a"))
                    .GetAttribute("href");
                VerifyIsTrue(linkToday.Contains("adminv3/settingstelephony"));
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsTodayCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Звонков сегодня",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"CallsTodayCount\"]")).Text);
                VerifyAreEqual("0",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"CallsTodayCount\"]")).Text);
                VerifyAreEqual("Настроить телефонию",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsToday\"] a"))
                        .Text);
                string linkToday = Driver
                    .FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsToday\"] a"))
                    .GetAttribute("href");
                VerifyIsTrue(linkToday.Contains("adminv3/settingstelephony"));
            }
        }

        /*calls yesterday tests*/
        [Test]
        public void IndicatorsNoBeforeCallsYesterday() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsYesterdayCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Звонков вчера",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"CallsYesterdayCount\"]")).Text);
            VerifyAreEqual("0",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"CallsYesterdayCount\"]")).Text);
            VerifyAreEqual("Настроить телефонию",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsYesterday\"] a"))
                    .Text);
        }

        [Test]
        public void IndicatorsCallsYesterday() //выбор индикатора через ссылку в верхнем правом углу
        {
            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if
            (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsYesterdayCount\"]")).Selected
            )
            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Звонков вчера",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"CallsYesterdayCount\"]")).Text);
                VerifyAreEqual("0",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"CallsYesterdayCount\"]")).Text);
                VerifyAreEqual("Настроить телефонию",
                    Driver.FindElement(
                        By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsYesterday\"] a")).Text);
                string linkYesterday = Driver
                    .FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsYesterday\"] a"))
                    .GetAttribute("href");
                VerifyIsTrue(linkYesterday.Contains("adminv3/settingstelephony"));
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsYesterdayCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Звонков вчера",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"CallsYesterdayCount\"]")).Text);
                VerifyAreEqual("0",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"CallsYesterdayCount\"]")).Text);
                VerifyAreEqual("Настроить телефонию",
                    Driver.FindElement(
                        By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsYesterday\"] a")).Text);
                string linkYesterday = Driver
                    .FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsYesterday\"] a"))
                    .GetAttribute("href");
                VerifyIsTrue(linkYesterday.Contains("adminv3/settingstelephony"));
            }
        }

        /*calls month tests*/
        [Test]
        public void IndicatorsNoBeforeCallsMonth() //no indicators before
        {
            Functions.IndicatorsNoBeforeMainPageAdmin(Driver, BaseUrl);
            Driver.FindElement(By.CssSelector("[data-e2e-no-indicators=\"IndicatorsShow\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsMonthCount\"]")).Click();

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

            VerifyAreEqual("Звонков за месяц",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"CallsMonthCount\"]")).Text);
            VerifyAreEqual("0",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"CallsMonthCount\"]")).Text);
            VerifyAreEqual("Настроить телефонию",
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsMonth\"] a")).Text);
        }

        [Test]
        public void IndicatorsCallsMonth() //выбор индикатора через ссылку в верхнем правом углу
        {
            GoToAdmin("home");

            Driver.FindElement(By.CssSelector("[data-e2e-indicators=\"IndicatorsShow\"]")).Click();

            if
            (
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsMonthCount\"]")).Selected
            )
            {
                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Звонков за месяц",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"CallsMonthCount\"]")).Text);
                VerifyAreEqual("0",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"CallsMonthCount\"]")).Text);
                VerifyAreEqual("Настроить телефонию",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsMonth\"] a"))
                        .Text);
                string linkMonth = Driver
                    .FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsMonth\"] a"))
                    .GetAttribute("href");
                VerifyIsTrue(linkMonth.Contains("adminv3/settingstelephony"));
            }
            else
            {
                Driver.FindElement(By.CssSelector("[data-e2e-dashboard-check=\"CallsMonthCount\"]")).Click();

                Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();

                VerifyAreEqual("Звонков за месяц",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard=\"CallsMonthCount\"]")).Text);
                VerifyAreEqual("0",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-content=\"CallsMonthCount\"]")).Text);
                VerifyAreEqual("Настроить телефонию",
                    Driver.FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsMonth\"] a"))
                        .Text);
                string linkMonth = Driver
                    .FindElement(By.CssSelector("[data-e2e-dashboard-service=\"ConnectServiceCallsMonth\"] a"))
                    .GetAttribute("href");
                VerifyIsTrue(linkMonth.Contains("adminv3/settingstelephony"));
            }
        }
    }
}