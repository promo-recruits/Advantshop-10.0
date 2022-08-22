using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.AdminPanelActions.Settings
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class CssStylesSettingsTest : MySitesFunctions
    {
        string cssStyles =
            ".lp-block.lp-block-form{ padding: 150px; background-color: darkblue; } .lp-h2{ text-decoration: underline; }";

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
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void AddCssStyles()
        {
            GoToClient("lp/testfunnel_2?inplace=false");
            VerifyAreEqual("85px",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("paddingTop"),
                "default paddingTop");
            VerifyAreEqual("0px",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("paddingLeft"),
                "default paddingLeft");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("backgroundColor"),
                "default background");
            VerifyAreEqual("none solid rgb(0, 0, 0)",
                Driver.FindElement(By.CssSelector(".lp-h2")).GetCssValue("textDecoration"), "default fontWeight");

            SetPageStyles(cssStyles);

            GoToClient("lp/testfunnel_2?inplace=false");
            VerifyAreEqual("150px",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("paddingTop"),
                "new paddingTop");
            VerifyAreEqual("150px",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("paddingLeft"),
                "new paddingLeft");
            VerifyAreEqual("rgba(0, 0, 139, 1)",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("backgroundColor"),
                "new background");
            VerifyAreEqual("underline solid rgb(0, 0, 0)",
                Driver.FindElement(By.CssSelector(".lp-h2")).GetCssValue("textDecoration"), "new fontWeight");

            SetPageStyles(Keys.Control + "a" + Keys.Delete);
        }

        [Test]
        public void AddCssStylesForNotMainPage()
        {
            string cssStylesForNotMainPage =
                "div.lp-block{ padding: 150px; background-color: rgba(226, 59, 59, 0.39); } .lp-h2{ text-decoration: underline; }";

            GoToClient("lp/testfunnel_1/funnelpage2?inplace=false");
            VerifyAreEqual("45px", Driver.FindElement(By.CssSelector(".lp-block")).GetCssValue("paddingTop"),
                "default paddingTop");
            VerifyAreEqual("0px", Driver.FindElement(By.CssSelector(".lp-block")).GetCssValue("paddingLeft"),
                "default paddingLeft");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.CssSelector(".lp-block")).GetCssValue("backgroundColor"), "default background");
            VerifyAreEqual("none solid rgb(0, 0, 0)",
                Driver.FindElement(By.CssSelector(".lp-h2")).GetCssValue("textDecoration"), "default fontWeight");

            SetPageStyles(cssStylesForNotMainPage, "lp/testfunnel_1/funnelpage2");

            GoToClient("lp/testfunnel_1/funnelpage2?inplace=false");
            VerifyAreEqual("150px", Driver.FindElement(By.CssSelector(".lp-block")).GetCssValue("paddingTop"),
                "new paddingTop");
            VerifyAreEqual("150px", Driver.FindElement(By.CssSelector(".lp-block")).GetCssValue("paddingLeft"),
                "new paddingLeft");
            VerifyAreEqual("rgba(226, 59, 59, 0.39)",
                Driver.FindElement(By.CssSelector(".lp-block")).GetCssValue("backgroundColor"), "new background");
            VerifyAreEqual("underline solid rgb(0, 0, 0)",
                Driver.FindElement(By.CssSelector(".lp-h2")).GetCssValue("textDecoration"), "new fontWeight");

            GoToClient("lp/testfunnel_1?inplace=false");
            VerifyAreEqual("85px", Driver.FindElement(By.CssSelector(".lp-block")).GetCssValue("paddingTop"),
                "default paddingTop");
            VerifyAreEqual("0px", Driver.FindElement(By.CssSelector(".lp-block")).GetCssValue("paddingLeft"),
                "default paddingLeft");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.CssSelector(".lp-block")).GetCssValue("backgroundColor"), "default background");
            VerifyAreEqual("none solid rgb(0, 0, 0)",
                Driver.FindElement(By.CssSelector(".lp-h2")).GetCssValue("textDecoration"), "default fontWeight");

            SetPageStyles(Keys.Control + "a" + Keys.Delete, "lp/testfunnel_1/funnelpage2");
        }

        [Test]
        public void CheckCssPriority()
        {
            string cssGeneralStyles =
                ".lp-block.lp-block-form{ border: 4px solid rgb(51, 51, 51); background-color: rgb(42, 93, 102); }";

            GoToClient("lp/testfunnel_2?inplace=false");
            VerifyAreEqual("85px",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("paddingTop"),
                "default paddingTop");
            VerifyAreEqual("0px",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("paddingLeft"),
                "default paddingLeft");
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("backgroundColor"),
                "default background");

            SetFunnelStyles(cssGeneralStyles);
            SetPageStyles(cssStyles);

            VerifyAreEqual("150px",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("paddingTop"),
                "new paddingTop from pageCSS");
            VerifyAreEqual("150px",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("paddingLeft"),
                "new paddingLeft from pageCSS");
            VerifyAreEqual("4px solid rgb(51, 51, 51)",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("border"),
                "new border from commonCSS");
            VerifyAreEqual("rgba(0, 0, 139, 1)",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("backgroundColor"),
                "new background from pageCSS");

            SetFunnelStyles(Keys.Control + "a" + Keys.Delete);
            SetPageStyles(Keys.Control + "a" + Keys.Delete);
        }

        public void SetFunnelStyles(string cssString)
        {
            GoToFunnelSettingsTab(2, "Настройки", "CSS стили", By.TagName("ui-ace-textarea"));
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector(".tab-pane.active .ace_text-input")).SendKeys(cssString);
            SaveFunnelSettings(By.ClassName("toast-success"));
        }

        public void SetPageStyles(string cssString, string pageUrl = "lp/testfunnel_2")
        {
            GoToClient(pageUrl + "?inplace=false");
            APItemSettingsClick();
            Driver.FindElement(By.Id("tabHeaderCss")).Click();
            Driver.WaitForElem(By.CssSelector(".tab-content-active .ace_text-input"));
            Driver.FindElement(By.CssSelector(".tab-content-active .ace_text-input")).SendKeys(cssString);
            Driver.FindElement(By.CssSelector(".adv-modal-active .blocks-constructor-btn-confirm")).Click();
        }
    }
}