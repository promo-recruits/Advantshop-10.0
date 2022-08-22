using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.AdminPanelActions.Settings
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class CommonSettingsTest : MySitesFunctions
    {
        string siteUrl = "lp/testfunnel_1?inplace=false";

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\AdminPanelActions\\CMS.LandingSubBlock.csv"
            );

            Init(false);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToCommonFunnelSettings();
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        //проверять для всех страниц, что другая осталась без изменения.

        [Test]
        public void PageNameNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void PageNameValidationNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void PageTitleNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void PageTitleValidationNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void PageUrlNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void PageUrlValidationNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void PageKeywordsNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void PageKeywordsValidationNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void PageDescriptionNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void PageDescriptionValidationNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void PageHeadHtml()
        {
            string htmlToHead = "<meta name=\"customAttribute\" content=\"AdVantShop.NET\">";
            GoToClient(siteUrl);
            VerifyIsFalse(Driver.PageSource.Contains(htmlToHead), "html to head is not exists");

            GoToCommonFunnelSettings();
            Driver.FindElement(AdvBy.DataE2E("PageHeadHtml")).SendKeys(htmlToHead);
            SaveModalSettings();

            VerifyIsTrue(Driver.PageSource.Contains(htmlToHead), "html to head was added");
            VerifyAreEqual(1,
                Driver.FindElement(By.TagName("head")).FindElements(By.CssSelector("meta[name=\"customAttribute\"]"))
                    .Count,
                "html to head displayed in head");

            APItemPagesClick(1, By.ClassName("lp-block"));
            VerifyIsFalse(Driver.PageSource.Contains(htmlToHead), "html to head is not exists");

            GoToCommonFunnelSettings();
            Driver.FindElement(AdvBy.DataE2E("PageHeadHtml")).SendKeys(Keys.Control + "a" + Keys.Delete);
            SaveModalSettings();
        }

        [Test]
        public void CheckLpSettingsLink()
        {
            VerifyAreEqual("Общие для страниц настройки: домен, метрика, HTML-блоки и т.д.",
                Driver.FindElement(AdvBy.DataE2E("LpSiteUrl")).Text, "default settings link test");
            VerifyAreEqual(BaseUrl + "adminv2/funnels/site/1#?landingAdminTab=settings",
                Driver.FindElement(AdvBy.DataE2E("LpSiteUrl")).GetAttribute("href"), "default settings link url");
            Driver.FindElement(AdvBy.DataE2E("LpSiteUrl")).Click();
            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("funnel-page__name"));
            VerifyAreEqual(BaseUrl + "adminv3/funnels/site/1#?landingAdminTab=settings",
                Driver.Url, "settings link url");
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void CheckBlocksOnAllPagesSetting()
        {
            string mainPageUrl = "lp/funnelproductsmultipage";

            GoToClient(mainPageUrl + "?inplace=true");
            ClickBlocksOnAllPagesCheckBox(By.ClassName("lp-block-products-view"));

            APItemPagesClick(1, By.ClassName("lp-block-form"));
            ClickBlocksOnAllPagesCheckBox(By.ClassName("lp-block-form"));

            //проверить наличие блоков на страницах без применения настройки Отключить показ сквозных блоков
            GoToClient(mainPageUrl + "?inplace=false");
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("lp-block")).Count, "first page, common blocks count");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-block-form")).Count, "block not from first page");

            APItemPagesClick(1, By.ClassName("lp-block-form"));
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("lp-block")).Count, "second page, common blocks count");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-block-products-view")).Count,
                "block not from second page");

            APItemPagesClick(2, By.ClassName("lp-block-columns-two"));
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("lp-block")).Count, "third page, common blocks count");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-block-form")).Count, "block not from third page");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-block-products-view")).Count,
                "block not from third page");

            //отключить на третьей, проверить. что на третьей скрылись, а на второй - нет
            ClickDisableBlocksOnAllPagesCheckBox();
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-block")).Count,
                "third page - disable blocks all pages");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-block-columns-two")).Count,
                "block from third page is displayed");
            APItemPagesClick(1, By.ClassName("lp-block-form"));
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("lp-block")).Count,
                "second page when at third is hidden");
            APItemPagesClick(2, By.ClassName("lp-block-columns-two"));
            ClickDisableBlocksOnAllPagesCheckBox();
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("lp-block")).Count, "third page, common blocks count");

            //отключить на первой, проверить.
            APItemPagesClick(0, By.ClassName("lp-block-products-view"));
            ClickDisableBlocksOnAllPagesCheckBox();
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-block")).Count,
                "first page - disable blocks all pages");
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("lp-block-form")).Count,
                "block from first page is displayed");
            ClickDisableBlocksOnAllPagesCheckBox();
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("lp-block")).Count, "first page, common blocks count");

            //return default settings
            GoToClient(mainPageUrl + "?inplace=true");
            ClickBlocksOnAllPagesCheckBox(By.ClassName("lp-block-products-view"));

            APItemPagesClick(1, By.ClassName("lp-block-form"));
            ClickBlocksOnAllPagesCheckBox(By.ClassName("lp-block-form"));
        }

        [Test]
        public void CheckShowShoppingCartSettingNOTCOMPLETE()
        {
            VerifyIsTrue(false,
                "после включения корзины запоминается последний добавленный в заказ товар - проверить, ок ли");

            string mainPageUrl = "lp/funnelproductsmultipage";

            //check default settings
            GoToClient(mainPageUrl + "?inplace=false");
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("lp-cart-trigger")).Count, "default cart is not shown");
            Driver.FindElements(By.CssSelector(".lp-products-view-item .lp-btn"))[0].Click();
            Driver.WaitForElem(By.ClassName("checkout-title"));
            Driver.Navigate().Back();

            APItemPagesClick(1, By.CssSelector(".lp-products-view-item-price .lp-btn"));
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("lp-cart-trigger")).Count, "default cart is not shown");
            Thread.Sleep(500);
            Driver.FindElements(By.CssSelector(".lp-products-view-item-price .lp-btn"))[0].Click();
            Driver.WaitForElem(By.ClassName("checkout-title"));

            GoToClient(mainPageUrl + "?inplace=false");
            ClickShowShoppingCartCheckBox();
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-cart-trigger")).Count, "default cart is not shown");
            Driver.FindElements(By.CssSelector(".lp-products-view-item .lp-btn"))[0].Click();
            Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("lp-cart__item")).Count,
                "items in cart - including certificate input");

            Driver.Navigate().Back();
            APItemPagesClick(1, By.CssSelector(".lp-products-view-item-price .lp-btn"));
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("lp-cart-trigger")).Count, "default cart is not shown");
            Thread.Sleep(500);
            Driver.FindElements(By.CssSelector(".lp-products-view-item-price .lp-btn"))[0].Click();
            Driver.WaitForElem(By.ClassName("checkout-title"));
            Driver.Navigate().Back();
            ClickShowShoppingCartCheckBox();
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-cart-trigger")).Count, "default cart is not shown");
            Thread.Sleep(500);
            Driver.FindElements(By.CssSelector(".lp-products-view-item-price .lp-btn"))[0].Click();
            Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("lp-cart__item")).Count,
                "items in cart2 - including certificate input");

            Driver.FindElement(By.CssSelector(".lp-cart-modal__footer .lp-btn--primary")).Click();
            Driver.WaitForElem(By.ClassName("checkout-title"));
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("checkout-cart-item-row")).Count, "items at checkout");
            Driver.FindElement(By.ClassName("checkout__button-summary")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));

            //return to default settings
            GoToClient(mainPageUrl + "?inplace=false");
            ClickShowShoppingCartCheckBox();
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("lp-cart-trigger")).Count,
                "returned settings - cart is not shown");

            APItemPagesClick(1, By.ClassName("lp-block-form"));
            ClickShowShoppingCartCheckBox();
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("lp-cart-trigger")).Count,
                "returned settings2 - cart is not shown");
        }

        [Test]
        public void CheckShowShoppingCartSettingBetweenFunnelsNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not worked cause products are remembered between funnels");

            string mainPageUrl = "lp/funnelproductsmultipage";

            GoToClient(mainPageUrl + "?inplace=false");
            ClickShowShoppingCartCheckBox();
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-cart-trigger")).Count, "default cart1 is not shown");
            Driver.FindElements(By.CssSelector(".lp-products-view-item .lp-btn"))[0].Click();
            Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("lp-cart__item")).Count,
                "items in cart at first funnel");
            Driver.FindElement(By.CssSelector(".lp-cart-modal__footer .lp-btn--primary")).Click();
            Driver.WaitForElem(By.ClassName("checkout-title"));
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("checkout-cart-item-row")).Count, "items1 at checkout");

            GoToClient("lp/funnelproductssinglepage?inplace=false");
            ClickShowShoppingCartCheckBox();
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("lp-cart-trigger")).Count, "default cart2 is not shown");
            Driver.FindElements(By.CssSelector(".lp-products-view-item .lp-btn"))[0].Click();
            Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("lp-cart__item")).Count,
                "items in cart at second funnel");

            //return to default settings
            ClickShowShoppingCartCheckBox();
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("lp-cart-trigger")).Count,
                "returned settings - cart is not shown");

            GoToClient(mainPageUrl + "?inplace=false");
            ClickShowShoppingCartCheckBox();
            VerifyAreEqual(0, Driver.FindElements(By.ClassName("lp-cart-trigger")).Count,
                "returned settings - cart is not shown");
        }

        [Test]
        public void CheckShoppingCartHideShippingNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        [Test]
        public void CheckStartNewLpSettingNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }

        public void GoToCommonFunnelSettings()
        {
            GoToClient(siteUrl);
            APItemSettingsClick();
            Driver.FindElement(By.Id("tabHeaderMenu")).Click();
            Driver.WaitForElem(By.CssSelector(".tab-content-active [data-e2e=\"PageName\"]"));
        }

        public void ClickBlocksOnAllPagesCheckBox(By mouseFocusSelector)
        {
            Driver.MouseFocus(mouseFocusSelector);

            Driver.FindElement(By.ClassName("lp-blocks-constructor--hover"))
                .FindElement(AdvBy.DataE2E("BlockSettingsBtn")).Click();
            Driver.WaitForElem(By.ClassName("adv-modal-active"));
            Driver.FindElement(AdvBy.DataE2E("ShowOnAllPage")).Click();
            Driver.FindElement(AdvBy.DataE2E("SaveSettingsBtn")).Click();
            Thread.Sleep(100);
        }

        public void ClickDisableBlocksOnAllPagesCheckBox()
        {
            APItemSettingsClick();
            Driver.WaitForElem(By.CssSelector(".tab-content-active [data-e2e=\"PageName\"]"));
            Driver.FindElement(AdvBy.DataE2E("DisableBlocksOnAllPages")).Click();
            SaveModalSettings();
        }


        public void ClickShowShoppingCartCheckBox()
        {
            APItemSettingsClick();
            Driver.WaitForElem(By.CssSelector(".tab-content-active [data-e2e=\"PageName\"]"));
            Driver.FindElement(AdvBy.DataE2E("ShowShoppingCart")).Click();
            SaveModalSettings();
        }
    }
}