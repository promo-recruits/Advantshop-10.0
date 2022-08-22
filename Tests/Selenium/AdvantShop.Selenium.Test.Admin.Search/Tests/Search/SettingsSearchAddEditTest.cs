using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Search.Tests.Search
{
    [TestFixture]
    public class SettingsSearchAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.SettingsSearch);
            InitializeService.LoadData("data\\Admin\\SettingsSearch\\Settings.SettingsSearch.csv");

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
        public void SettingsSearchAdd()
        {
            Driver.GetButton(EButtonType.Add).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"nameSettingsSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameSettingsSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameSettingsSearch\"]")).SendKeys("NewSettingsSearch");

            Driver.FindElement(By.CssSelector("[data-e2e=\"linkSettingsSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"linkSettingsSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"linkSettingsSearch\"]")).SendKeys("coupons");

            Driver.FindElement(By.CssSelector("[data-e2e=\"keywordsSettingsSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"keywordsSettingsSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"keywordsSettingsSearch\"]")).SendKeys("купоны");

            Driver.FindElement(By.CssSelector("[data-e2e=\"sortOrderSettingsSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortOrderSettingsSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortOrderSettingsSearch\"]")).SendKeys("1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"saveSettingsSearch\"]")).Click();

            //check admin
            GoToAdmin("settingssearch");

            VerifyAreEqual("Найдено записей: 151",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after adding");

            Driver.GridFilterSendKeys("NewSettingsSearch", "h1");

            VerifyAreEqual("NewSettingsSearch", Driver.GetGridCellText(0, "Title"), "Title");
            VerifyAreEqual("coupons", Driver.GetGridCellText(0, "Link"), "Link");
            VerifyAreEqual("купоны", Driver.GetGridCellText(0, "KeyWords"), "KeyWords");
            VerifyAreEqual("1", Driver.GetGridCellText(0, "SortOrder"), "SortOrder");

            //check link
            Driver.GetGridCell(0, "Link").Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.WindowHandles.Count.Equals(2), "count tabs");

            VerifyIsTrue(Driver.Url.Contains("coupons"), "check url from settings search grid");
            VerifyAreEqual("Купоны", Driver.FindElement(By.TagName("h1")).Text, "h1 page from settings search grid");

            Functions.CloseTab(Driver, BaseUrl);

            //check search
            GoToAdmin();

            Driver.FindElement(By.CssSelector(".search-input")).Click();
            Driver.FindElement(By.CssSelector(".search-input")).SendKeys("купоны");

            Driver.WaitForElem(By.XPath("//span[contains(text(), 'NewSettingsSearch')]"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'NewSettingsSearch')]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridRow"));

            VerifyIsTrue(Driver.Url.Contains("coupons"), "check url from search");
            VerifyAreEqual("Купоны", Driver.FindElement(By.TagName("h1")).Text, "h1 page from search");
        }

        [Test]
        public void SettingsSearchEdit()
        {
            Driver.GridFilterSendKeys("test title 111", "h1");

            VerifyAreEqual("test title 111", Driver.GetGridCellText(0, "Title"), "Title grid before");

            Driver.GetGridCellElement(0, "_serviceColumn",
                by: By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();

            //pre check
            VerifyAreEqual("test title 111",
                Driver.FindElement(By.CssSelector("[data-e2e=\"nameSettingsSearch\"]")).GetAttribute("value"),
                "Title edit pop up");
            VerifyAreEqual("link110",
                Driver.FindElement(By.CssSelector("[data-e2e=\"linkSettingsSearch\"]")).GetAttribute("value"),
                "Link edit pop up");
            VerifyAreEqual("keywords 110",
                Driver.FindElement(By.CssSelector("[data-e2e=\"keywordsSettingsSearch\"]")).GetAttribute("value"),
                "KeyWords edit pop up");
            VerifyAreEqual("1110",
                Driver.FindElement(By.CssSelector("[data-e2e=\"sortOrderSettingsSearch\"]")).GetAttribute("value"),
                "SortOrder edit pop up");

            //edit
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameSettingsSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameSettingsSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameSettingsSearch\"]")).SendKeys("edit name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"linkSettingsSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"linkSettingsSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"linkSettingsSearch\"]")).SendKeys("orders");

            Driver.FindElement(By.CssSelector("[data-e2e=\"keywordsSettingsSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"keywordsSettingsSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"keywordsSettingsSearch\"]")).SendKeys("заказы");

            Driver.FindElement(By.CssSelector("[data-e2e=\"sortOrderSettingsSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortOrderSettingsSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortOrderSettingsSearch\"]")).SendKeys("2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"saveSettingsSearch\"]")).Click();

            //check admin
            GoToAdmin("settingssearch");

            Driver.GridFilterSendKeys("test title 111", "h1");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search old item");

            Driver.GridFilterSendKeys("edit name", "h1");

            VerifyAreEqual("edit name", Driver.GetGridCellText(0, "Title"), "Title");
            VerifyAreEqual("orders", Driver.GetGridCellText(0, "Link"), "Link");
            VerifyAreEqual("заказы", Driver.GetGridCellText(0, "KeyWords"), "KeyWords");
            VerifyAreEqual("2", Driver.GetGridCellText(0, "SortOrder"), "SortOrder");

            //check link
            Driver.GetGridCell(0, "Link").Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.WindowHandles.Count.Equals(2), "count tabs");

            VerifyIsTrue(Driver.Url.Contains("orders"), "check url from settings search grid");
            VerifyAreEqual("Заказы", Driver.FindElement(By.TagName("h1")).Text, "h1 page from settings search grid");

            Functions.CloseTab(Driver, BaseUrl);

            //check search
            GoToAdmin();

            Driver.FindElement(By.CssSelector(".search-input")).Click();
            Driver.FindElement(By.CssSelector(".search-input")).SendKeys("заказы");

            Driver.WaitForElem(By.XPath("//span[contains(text(), 'edit name')]"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'edit name')]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridRow"));

            VerifyIsTrue(Driver.Url.Contains("orders"), "check url from search");
            VerifyAreEqual("Заказы", Driver.FindElement(By.TagName("h1")).Text, "h1 page from search");
        }

        [Test]
        public void SettingsSearchAddWithout()
        {
            Driver.GetButton(EButtonType.Add).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"nameSettingsSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameSettingsSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"nameSettingsSearch\"]"))
                .SendKeys("NewSettingsSearchwithout");

            Driver.FindElement(By.CssSelector("[data-e2e=\"linkSettingsSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"linkSettingsSearch\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"linkSettingsSearch\"]")).SendKeys("brands");

            Driver.FindElement(By.CssSelector("[data-e2e=\"sortOrderSettingsSearch\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"sortOrderSettingsSearch\"]")).Clear();

            Driver.FindElement(By.CssSelector("[data-e2e=\"saveSettingsSearch\"]")).Click();

            //check admin
            GoToAdmin("settingssearch");

            Driver.GridFilterSendKeys("NewSettingsSearchwithout", "h1");

            VerifyAreEqual("NewSettingsSearchwithout", Driver.GetGridCellText(0, "Title"), "Title");
            VerifyAreEqual("brands", Driver.GetGridCellText(0, "Link"), "Link");
            VerifyAreEqual("", Driver.GetGridCellText(0, "KeyWords"), "KeyWords");
            VerifyAreEqual("0", Driver.GetGridCellText(0, "SortOrder"), "SortOrder");
        }
    }
}