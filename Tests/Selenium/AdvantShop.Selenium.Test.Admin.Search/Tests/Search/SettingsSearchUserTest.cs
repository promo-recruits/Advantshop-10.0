using System.Net;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Search.Tests.Search
{
    [TestFixture]
    public class SettingsSearchUserTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin("settingssearch");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        [Ignore("Only for local runs")]
        public void SettingsSearchTest()
        {
            VerifyAreEqual("Поиск настроек", Driver.FindElement(By.TagName("h1")).Text, "h1 settings search page");
            VerifyAreEqual("Найдено записей: 129",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            while (Driver.FindElements(By.CssSelector(".pagination-next a[disabled=\"disabled\"]")).Count == 0)
            {
                for (var i = 0;
                    i < Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"] ui-modal-trigger")).Count;
                    i++)
                {
                    Driver.GetGridCell(i, "Link").Click();
                    CheckSettingSearch(Driver.GetGridCell(i, "Title").Text);
                }

                Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            }
        }

        private void CheckSettingSearch(string title)
        {
            Functions.OpenNewTab(Driver, BaseUrl);
            try
            {
                //VerifyIsFalse(Is404Page(driver.Url), "not 404 page: " + title);
                //VerifyIsFalse(Is500Error(), "not 500 error: " + title);
                //VerifyIsFalse(IsConsole500Error(), "not 500 console error: " + title);
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK,
                    "page status of " + title + " : " + GetPageStatus(Driver.Url));
                VerifyIsNull(CheckConsoleLog(true), "has not console errors: " + title);
                VerifyIsTrue(Driver.PageSource.Contains(title), "title on page: " + title);
            }
            catch (Exception ex)
            {
                VerifyAddErrors("any problems on: " + title + "; ex message: " + ex.Message);
            }

            Functions.CloseTab(Driver, BaseUrl);
        }
    }
}