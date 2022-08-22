using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Team.teamThreeColumns.Button
{
    [TestFixture]
    public class teamThreeColumnsBtnUrl : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Team\\teamThreeColumns\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private readonly string FormBtn = "teamThreeColumnsBtn";

        [Test]
        public void BtnSettingPage()
        {
            TestName = "BtnSettingPage";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabTeamButton");
            BtnEnabledButton();
            BtnSetTextButton("Text Btn Page");
            BtnActionButtonSelect("Урл адрес");
            BtnActionUrlSelectUrl("page1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlInput\"]")).Count == 0, "no url field");
            BtnActionUrlNoTargetBlank();
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Page", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Text Btn Page", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target client");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn client");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Text Btn Page", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target mobile");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn mobile");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnSettingPageBlank()
        {
            TestName = "BtnSettingPageBlank";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabTeamButton");
            BtnEnabledButton();
            BtnSetTextButton("Text Btn Page Blank");
            BtnActionButtonSelect("Урл адрес");
            BtnActionUrlSelectUrl("page1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlInput\"]")).Count == 0, "no url field");
            BtnActionUrlTargetBlank();
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Page Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text, "btn txt");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target");

            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.Url.Contains(BaseUrl + "/lp/test1/page1"), "old url by btn");
            VerifyIsTrue(Driver.WindowHandles.Count == 2, "new tab");
            Functions.OpenNewTab(Driver, BaseUrl);

            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page");
            Functions.CloseTab(Driver, BaseUrl);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Text Btn Page Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text, "btn txt client");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target client");

            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1", Driver.Url, "old url by btn");
            VerifyIsTrue(Driver.WindowHandles.Count == 2, "new tab");
            Functions.OpenNewTab(Driver, BaseUrl);

            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn client");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page client");
            Functions.CloseTab(Driver, BaseUrl);

            GoToMobile("lp/test1");
            VerifyAreEqual("Text Btn Page Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text, "btn txt mobile");

            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target mobile");

            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.Url.Contains(BaseUrl + "/lp/test1/page1"), "old url by btn mobile");
            VerifyIsTrue(Driver.WindowHandles.Count == 2, "new tab");
            Functions.OpenNewTab(Driver, BaseUrl);

            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn mobile");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page mobile");
            Functions.CloseTab(Driver, BaseUrl);

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnSettingUrl()
        {
            TestName = "BtnSettingUrl";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabTeamButton");
            BtnEnabledButton();
            BtnSetTextButton("Text Btn Url");
            BtnActionButtonSelect("Урл адрес");
            var selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"UrlSelect\"]"));
            var select = new SelectElement(selectElem);
            var allOptions = select.Options;
            VerifyIsTrue(allOptions.Any(item => item.Text.ToString() == "page1"), "another page landing in select");
            VerifyIsTrue(allOptions.Any(item => item.Text.ToString() == "Указать свой URL-адрес"), "own url in select");
            BtnActionUrlSelectUrl("Указать свой URL-адрес");
            BtnActionUrlSetUrl("products/test-product1");
            BtnActionUrlNoTargetBlank();
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Url", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt");
            VerifyAreEqual(BaseUrl + "/products/test-product1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/products/test-product1", Driver.Url, "url by btn");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product1"), "not 404 page");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Text Btn Url", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual(BaseUrl + "/products/test-product1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target client");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/products/test-product1", Driver.Url, "url by btn client");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product1"), "not 404 page client");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1 on page client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Text Btn Url", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual(BaseUrl + "/products/test-product1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target mobile");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/products/test-product1", Driver.Url, "url by btn mobile");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product1"), "not 404 page mobile");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1 on page mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnSettingUrlBlank()
        {
            TestName = "BtnSettingUrlBlank";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabTeamButton");
            BtnEnabledButton();
            BtnSetTextButton("Text Btn Url Blank");
            BtnActionButtonSelect("Урл адрес");
            BtnActionUrlSelectUrl("Указать свой URL-адрес");
            BtnActionUrlSetUrl("products/test-product5");
            BtnActionUrlTargetBlank();
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Url Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text, "btn txt");
            VerifyAreEqual(BaseUrl + "/products/test-product5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target");
            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/test1"), "old url by btn");
            VerifyIsFalse(Driver.Url.Contains("/products/test-product5"), "old url by btn 1");

            VerifyIsTrue(Driver.WindowHandles.Count == 2, "new tab");
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual(BaseUrl + "/products/test-product5", Driver.Url, "url by btn");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product5"), "not 404 page");
            VerifyAreEqual("TestProduct5", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");
            Functions.CloseTab(Driver, BaseUrl);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Text Btn Url Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text, "btn txt client");
            VerifyAreEqual(BaseUrl + "/products/test-product5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target client");
            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab client");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/test1"), "old url by btn client");
            VerifyIsFalse(Driver.Url.Contains("/products/test-product5"), "old url by btn client 1");

            VerifyIsTrue(Driver.WindowHandles.Count == 2, "new tab client");
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual(BaseUrl + "/products/test-product5", Driver.Url, "url by btn client");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product5"), "not 404 page client");
            VerifyAreEqual("TestProduct5", Driver.FindElement(By.TagName("h1")).Text, "h1 on page client");
            Functions.CloseTab(Driver, BaseUrl);

            GoToMobile("lp/test1");
            VerifyAreEqual("Text Btn Url Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text, "btn txt mobile");
            VerifyAreEqual(BaseUrl + "/products/test-product5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] a")).GetAttribute("target"),
                "btn target mobile");
            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab mobile");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/test1"), "old url by btn mobile");
            VerifyIsFalse(Driver.Url.Contains("/products/test-product5"), "old url by btn mobile 1");

            VerifyIsTrue(Driver.WindowHandles.Count == 2, "new tab mobile");
            Functions.OpenNewTab(Driver, BaseUrl);

            VerifyAreEqual(BaseUrl + "/products/test-product5", Driver.Url, "url by btn mobile");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product5"), "not 404 page mobile");
            VerifyAreEqual("TestProduct5", Driver.FindElement(By.TagName("h1")).Text, "h1 on page mobile");
            Functions.CloseTab(Driver, BaseUrl);

            VerifyFinally(TestName);
        }
    }
}