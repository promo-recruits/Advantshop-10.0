using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.Settings
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class CssStylesSettingsTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\Default\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\Default\\CMS.LandingSubBlock.csv"
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
            string cssStyles =
                ".lp-block.lp-block-form{ padding: 150px; background-color: darkblue; } .lp-h2{ text-decoration: underline; }";

            GoToClient("lp/testfunnel");
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

            GoToFunnelSettingsTab(1, "Настройки", "CSS стили", By.TagName("ui-ace-textarea"));
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector(".tab-pane.active .ace_text-input")).SendKeys(cssStyles);
            SaveFunnelSettings(By.ClassName("toast-success"));
            GoToClient("lp/testfunnel");
            VerifyAreEqual("150px",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("paddingTop"),
                "default paddingTop");
            VerifyAreEqual("150px",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("paddingLeft"),
                "default paddingLeft");
            VerifyAreEqual("rgba(0, 0, 139, 1)",
                Driver.FindElement(By.CssSelector(".lp-block.lp-block-form")).GetCssValue("backgroundColor"),
                "default background");
            VerifyAreEqual("underline solid rgb(0, 0, 0)",
                Driver.FindElement(By.CssSelector(".lp-h2")).GetCssValue("textDecoration"), "default fontWeight");

            GoToFunnelSettingsTab(1, "Настройки", "CSS стили", By.TagName("ui-ace-textarea"));
            Driver.FindElement(By.CssSelector(".tab-pane.active .ace_text-input"))
                .SendKeys(Keys.Control + "a" + Keys.Delete);
            SaveFunnelSettings(By.ClassName("toast-success"));
        }
    }
}