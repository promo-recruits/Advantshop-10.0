using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Subscription
{
    [TestFixture]
    public class SubscriptionTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Subscription\\Customers.CustomerGroup.csv",
                "data\\Admin\\Subscription\\Customers.Customer.csv",
                "data\\Admin\\Subscription\\Customers.Contact.csv",
                "data\\Admin\\Subscription\\Customers.Departments.csv",
                "data\\Admin\\Subscription\\Customers.Managers.csv",
                "data\\Admin\\Subscription\\Customers.CustomerField.csv",
                "data\\Admin\\Subscription\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\Subscription\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\Subscription\\Customers.Subscription.csv"
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
        [Order(0)]
        public void SubscriptionGridTest()
        {
            GoToAdmin("subscription");

            VerifyAreEqual("Подписчики на новости", Driver.FindElement(By.TagName("h1")).Text, "h1 grid");
            VerifyAreEqual("testmail1@test.ru", Driver.GetGridCell(0, "Email").Text, "subscription Email grid");
            VerifyAreEqual("20.07.2017 10:54", Driver.GetGridCell(0, "SubscribeDateStr").Text,
                "subscription SubscribeDateStr grid");
            VerifyAreEqual("", Driver.GetGridCell(0, "UnsubscribeDateStr").Text,
                "subscription UnsubscribeDateStr grid");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .GetAttribute("value") == "true", "subscription Enabled grid");

            VerifyAreEqual("Найдено записей: 22",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all subscription");
        }

        [Test]
        [Order(1)]
        public void SearchExist()
        {
            GoToAdmin("subscription");

            Driver.GridFilterSendKeys("testmail12");

            VerifyAreEqual("testmail12@test.ru", Driver.GetGridCell(0, "Email").Text, "search exist subscription");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }


        [Test]
        [Order(1)]
        public void SearchNotExist()
        {
            GoToAdmin("subscription");

            Driver.GridFilterSendKeys("Unknown");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        [Order(1)]
        public void SearchMuchSymbols()
        {
            GoToAdmin("subscription");

            Driver.GridFilterSendKeys(
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", By.ClassName("ui-grid-custom-filter-total"));

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        [Order(1)]
        public void SearchInvalidSymbols()
        {
            GoToAdmin("subscription");

            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        [Order(1)]
        public void InplaceTest()
        {
            GoToAdmin("subscription");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .GetAttribute("value") == "true", "subscription Enabled grid");
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .GetAttribute("value") == "false", "subscription not Enabled grid");
        }

        [Test]
        [Order(1)]
        public void SubscriptionAddTest()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=mainpage");
            //driver.FindElement(By.CssSelector(".other-button button")).Click();
            //Driver.ScrollTo(By.CssSelector("[data-e2e=\"Количество товаров в строке\"]"));
            try
            {
                Driver.CheckBoxCheck("NewsSubscriptionVisibility");
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
            }
            catch (Exception)
            {
            }
            //if (!driver.FindElement(By.CssSelector("[data-e2e=\"Подписка на новости\"] input")).Selected)
            //{
            //    driver.FindElement(By.CssSelector("[data-e2e=\"Подписка на новости\"] span")).Click();
            //    driver.FindElement(By.CssSelector(".modal-footer button")).Click();
            //}

            GoToClient();
            Driver.ScrollTo(By.CssSelector(".subscribe-block"));
            Driver.FindElement(By.CssSelector(".subscribe-block input")).SendKeys("newtestmail");
            Driver.FindElement(By.CssSelector(".btn-subscribe")).Click();
            VerifyAreEqual("newtestmail",
                Driver.FindElement(By.CssSelector(".subscribe-block input")).GetAttribute("value"),
                "fail subscription");

            Driver.FindElement(By.CssSelector(".subscribe-block input")).Clear();
            Driver.FindElement(By.CssSelector(".subscribe-block input")).SendKeys("newtestmail@test.test");
            Driver.FindElement(By.CssSelector(".btn-subscribe")).Click();
            VerifyAreEqual("", Driver.FindElement(By.CssSelector(".subscribe-block input")).GetAttribute("value"),
                "suscess subscription");

            GoToAdmin("subscription");
            VerifyAreEqual("Найдено записей: 23",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all subscription");
            VerifyAreEqual("Подписчики на новости", Driver.FindElement(By.TagName("h1")).Text, "h1 grid");
            Driver.GridFilterSendKeys("newtestmail");
            VerifyAreEqual("newtestmail@test.test", Driver.GetGridCell(0, "Email").Text, "subscription Email grid");
            VerifyIsTrue(Driver.GetGridCell(0, "SubscribeDateStr").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "subscription SubscribeDateStr grid");
            VerifyAreEqual("", Driver.GetGridCell(0, "UnsubscribeDateStr").Text,
                "subscription UnsubscribeDateStr grid");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .GetAttribute("value") == "true", "subscription Enabled grid");

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count new subscription");
        }

        //[Test]
        //public void ExportSubscription()
        //{

        //    GoToAdmin("subscription");
        //   // driver.FindElement(By.CssSelector("[data-e2e=\"subscriptionExport\"]")).Click();
        //    GoToAdmin("subscription/export?openinbrowser=true");

        //    //VerifyIsTrue(driver.PageSource.Contains("Email;Активен;Дата подписки;Дата отписки;Подписка;\r\ntestmail1@test.ru;0;20.07.2017 10:54:00;;;\r\ntestmail2@test.ru;1;19.07.2017 10:54:00;;;\r\ntestmail3@test.ru;1;18.07.2017 10:54:00;;;\r\ntestmail4@test.ru;1;17.07.2017 10:54:00;;;\r\ntestmail5@test.ru;1;16.07.2017 10:54:00;;;\r\ntestmail6@test.ru;1;15.07.2017 10:54:00;;;\r\ntestmail7@test.ru;1;14.07.2017 10:54:00;;;\r\ntestmail8@test.ru;1;13.07.2017 10:54:00;;;\r\ntestmail9@test.ru;1;12.07.2017 10:54:00;;;\r\ntestmail10@test.ru;1;11.07.2017 10:54:00;;;\r\ntestmail11@test.ru;0;10.07.2017 10:54:00;30.06.2017 10:54:00;;\r\ntestmail12@test.ru;0;09.07.2017 10:54:00;01.07.2017 10:54:00;;\r\ntestmail13@test.ru;0;08.07.2017 10:54:00;02.07.2017 10:54:00;;\r\ntestmail14@test.ru;0;07.07.2017 10:54:00;03.07.2017 10:54:00;;\r\ntestmail15@test.ru;0;06.07.2017 10:54:00;04.07.2017 10:54:00;;\r\ntestmail16@test.ru;0;05.07.2017 10:54:00;05.07.2017 10:54:00;;\r\ntestmail17@test.ru;0;04.07.2017 10:54:00;06.07.2017 10:54:00;Reason1;\r\ntestmail18@test.ru;0;03.07.2017 10:54:00;07.07.2017 10:54:00;Reason2;\r\ntestmail19@test.ru;0;02.07.2017 10:54:00;08.07.2017 10:54:00;Reason3;\r\ntestmail20@test.ru;0;01.07.2017 10:54:00;09.07.2017 10:54:00;Reason4;\r\ntestmail21@test.ru;0;30.06.2017 10:54:00;10.07.2017 10:54:00;Reason5;\r\ntestmail22@test.ru;0;29.06.2017 10:54:00;11.07.2017 10:54:00;Reason6;"), "subscription export page");
        //    /*
        //     * Email;Активен;Дата подписки;Дата отписки;Подписка;\r\ntestmail1@test.ru;0;20.07.2017 10:54:00;;;\r\ntestmail2@test.ru;1;19.07.2017 10:54:00;;;\r\ntestmail3@test.ru;1;18.07.2017 10:54:00;;;\r\ntestmail4@test.ru;1;17.07.2017 10:54:00;;;\r\ntestmail5@test.ru;1;16.07.2017 10:54:00;;;\r\ntestmail6@test.ru;1;15.07.2017 10:54:00;;;\r\ntestmail7@test.ru;1;14.07.2017 10:54:00;;;\r\ntestmail8@test.ru;1;13.07.2017 10:54:00;;;\r\ntestmail9@test.ru;1;12.07.2017 10:54:00;;;\r\ntestmail10@test.ru;1;11.07.2017 10:54:00;;;\r\ntestmail11@test.ru;0;10.07.2017 10:54:00;30.06.2017 10:54:00;;\r\ntestmail12@test.ru;0;09.07.2017 10:54:00;01.07.2017 10:54:00;;\r\ntestmail13@test.ru;0;08.07.2017 10:54:00;02.07.2017 10:54:00;;\r\ntestmail14@test.ru;0;07.07.2017 10:54:00;03.07.2017 10:54:00;;\r\ntestmail15@test.ru;0;06.07.2017 10:54:00;04.07.2017 10:54:00;;\r\ntestmail16@test.ru;0;05.07.2017 10:54:00;05.07.2017 10:54:00;;\r\ntestmail17@test.ru;0;04.07.2017 10:54:00;06.07.2017 10:54:00;Reason1;\r\ntestmail18@test.ru;0;03.07.2017 10:54:00;07.07.2017 10:54:00;Reason2;\r\ntestmail19@test.ru;0;02.07.2017 10:54:00;08.07.2017 10:54:00;Reason3;\r\ntestmail20@test.ru;0;01.07.2017 10:54:00;09.07.2017 10:54:00;Reason4;\r\ntestmail21@test.ru;0;30.06.2017 10:54:00;10.07.2017 10:54:00;Reason5;\r\ntestmail22@test.ru;0;29.06.2017 10:54:00;11.07.2017 10:54:00;Reason6;
        //     * */
        //    VerifyIsTrue(driver.PageSource.Contains("testmail1@test.ru;0;20.07.2017 10:54:00;;;"), "subscription export page");
        //    VerifyIsTrue(driver.FindElement(By.TagName("pre")).Text.Contains("testmail22@test.ru;0;29.06.2017 10:54:00;11.07.2017 10:54:00;Reason6;"), "subscription export page");
        //}
    }
}