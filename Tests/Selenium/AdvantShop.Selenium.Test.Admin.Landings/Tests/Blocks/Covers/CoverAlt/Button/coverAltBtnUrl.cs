using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Covers.CoverAlt.Button
{
    [TestFixture]
    public class coverAltBtnUrl : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private readonly string FormBtn1 = "CoversBtn1";
        private readonly string FormBtn2 = "CoversBtn2";

        [Test]
        public void Btn1SettingPage()
        {
            TestName = "Btn1SettingPage";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton2");
            BtnDisableButton(1);

            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Text Btn Page");
            BtnActionButtonSelect("Урл адрес");
            BtnActionUrlSelectUrl("page1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlInput\"]")).Count == 0, "no url field");
            BtnActionUrlNoTargetBlank();
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Page", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Text Btn Page", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target client");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn client");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Text Btn Page", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target mobile");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn mobile");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void Btn1SettingPageBlank()
        {
            TestName = "Btn1SettingPageBlank";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton2");
            BtnDisableButton(1);

            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Text Btn Page Blank");
            BtnActionButtonSelect("Урл адрес");
            BtnActionUrlSelectUrl("page1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlInput\"]")).Count == 0, "no url field");
            BtnActionUrlTargetBlank();
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Page Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text, "btn txt");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target");

            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
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
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text, "btn txt client");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target client");

            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1", Driver.Url, "old url by btn");
            VerifyIsTrue(Driver.WindowHandles.Count == 2, "new tab");
            Functions.OpenNewTab(Driver, BaseUrl);

            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn client");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page client");
            Functions.CloseTab(Driver, BaseUrl);

            GoToMobile("lp/test1");
            VerifyAreEqual("Text Btn Page Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text, "btn txt mobile");

            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target mobile");

            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
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
        public void Btn1SettingUrl()
        {
            TestName = "Btn1SettingUrl";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton2");
            BtnDisableButton(1);

            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Text Btn Url");
            BtnActionButtonSelect("Урл адрес");
            BtnActionUrlSelectUrl("Указать свой URL-адрес");
            BtnActionUrlSetUrl("products/test-product1");
            BtnActionUrlNoTargetBlank();
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Url", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual(BaseUrl + "/products/test-product1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/products/test-product1", Driver.Url, "url by btn");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product1"), "not 404 page");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Text Btn Url", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual(BaseUrl + "/products/test-product1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target client");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/products/test-product1", Driver.Url, "url by btn client");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product1"), "not 404 page client");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1 on page client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Text Btn Url", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual(BaseUrl + "/products/test-product1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target mobile");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/products/test-product1", Driver.Url, "url by btn mobile");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product1"), "not 404 page mobile");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1 on page mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void Btn1SettingUrlBlank()
        {
            TestName = "Btn1SettingUrlBlank";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton2");
            BtnDisableButton(1);

            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Text Btn Url Blank");
            BtnActionButtonSelect("Урл адрес");
            BtnActionUrlSelectUrl("Указать свой URL-адрес");
            BtnActionUrlSetUrl("products/test-product5");
            BtnActionUrlTargetBlank();
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Url Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text, "btn txt");
            VerifyAreEqual(BaseUrl + "/products/test-product5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target");
            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
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
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text, "btn txt client");
            VerifyAreEqual(BaseUrl + "/products/test-product5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target client");
            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab client");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
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
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text, "btn txt mobile");
            VerifyAreEqual(BaseUrl + "/products/test-product5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] a")).GetAttribute("target"),
                "btn target mobile");
            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab mobile");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
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

        [Test]
        public void Btn2SettingPage()
        {
            TestName = "Btn2SettingPage";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton2");
            BtnEnabledButton(1);
            BtnSetTextButton("Text Btn Page", 1);
            BtnActionButtonSelect("Урл адрес", 1);
            BtnActionUrlSelectUrl("page1", 1);
            BtnActionUrlNoTargetBlank(1);
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Page", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Text Btn Page", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target client");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn client");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Text Btn Page", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target mobile");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn mobile");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void Btn2SettingPageBlank()
        {
            TestName = "Btn2SettingPageBlank";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton2");
            BtnEnabledButton(1);
            BtnSetTextButton("Text Btn Page Blank", 1);
            BtnActionButtonSelect("Урл адрес", 1);
            BtnActionUrlSelectUrl("page1", 1);
            BtnActionUrlTargetBlank(1);
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Page Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text, "btn txt");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target");

            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
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
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text, "btn txt client");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target client");

            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/lp/test1", Driver.Url, "old url by btn");
            VerifyIsTrue(Driver.WindowHandles.Count == 2, "new tab");
            Functions.OpenNewTab(Driver, BaseUrl);

            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn client");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page client");
            Functions.CloseTab(Driver, BaseUrl);

            GoToMobile("lp/test1");
            VerifyAreEqual("Text Btn Page Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text, "btn txt mobile");

            VerifyAreEqual(BaseUrl + "/lp/test1/page1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target mobile");

            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
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
        public void Btn2SettingUrl()
        {
            TestName = "Btn2SettingUrl";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton2");
            BtnEnabledButton(1);
            BtnSetTextButton("Text Btn Url", 1);
            BtnActionButtonSelect("Урл адрес", 1);
            BtnActionUrlSelectUrl("Указать свой URL-адрес", 1);
            BtnActionUrlSetUrl("products/test-product1", 1);
            BtnActionUrlNoTargetBlank(1);
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Url", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual(BaseUrl + "/products/test-product1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/products/test-product1", Driver.Url, "url by btn");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product1"), "not 404 page");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Text Btn Url", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual(BaseUrl + "/products/test-product1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target client");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/products/test-product1", Driver.Url, "url by btn client");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product1"), "not 404 page client");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1 on page client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Text Btn Url", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual(BaseUrl + "/products/test-product1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target mobile");
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual(BaseUrl + "/products/test-product1", Driver.Url, "url by btn mobile");
            VerifyIsFalse(Is404Page(BaseUrl + "/products/test-product1"), "not 404 page mobile");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1 on page mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void Btn2SettingUrlBlank()
        {
            TestName = "Btn2SettingUrlBlank";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton2");
            BtnEnabledButton(1);
            BtnSetTextButton("Text Btn Url Blank", 1);
            BtnActionButtonSelect("Урл адрес", 1);
            BtnActionUrlSelectUrl("Указать свой URL-адрес", 1);
            BtnActionUrlSetUrl("products/test-product5", 1);
            BtnActionUrlTargetBlank(1);
            BlockSettingsSave();

            VerifyAreEqual("Text Btn Url Blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text, "btn txt");
            VerifyAreEqual(BaseUrl + "/products/test-product5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target");
            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
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
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text, "btn txt client");
            VerifyAreEqual(BaseUrl + "/products/test-product5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href client");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target client");
            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab client");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
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
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text, "btn txt mobile");
            VerifyAreEqual(BaseUrl + "/products/test-product5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("href"),
                "btn href mobile");
            VerifyAreEqual("_blank",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] a")).GetAttribute("target"),
                "btn target mobile");
            VerifyIsTrue(Driver.WindowHandles.Count == 1, "one tab mobile");

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
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