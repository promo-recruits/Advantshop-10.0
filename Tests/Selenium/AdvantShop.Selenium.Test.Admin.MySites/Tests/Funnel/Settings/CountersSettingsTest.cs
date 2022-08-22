using System;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.Settings
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class CountersSettingsTest : MySitesFunctions
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
        public void AddHtmlToHead()
        {
            string htmlToHead = "<meta name=\"customAttribute\" content=\"AdVantShop.NET\">";
            GoToClient("lp/testfunnel");
            VerifyIsFalse(Driver.PageSource.Contains(htmlToHead), "html to head is not exists");

            GoToFunnelSettingsTab(1, "Настройки", "Счетчики", By.Name("BlockInHead"));
            Driver.FindElement(By.Name("BlockInHead")).SendKeys(htmlToHead);
            SaveFunnelSettings();

            GoToClient("lp/testfunnel");
            VerifyIsTrue(Driver.PageSource.Contains(htmlToHead), "html to head was added");
            VerifyAreEqual(1,
                Driver.FindElement(By.TagName("head")).FindElements(By.CssSelector("meta[name=\"customAttribute\"]"))
                    .Count,
                "html to head displayed in head");
            try
            {
                ReInitClient();
                GoToClient("lp/testfunnel");
                VerifyIsTrue(Driver.PageSource.Contains(htmlToHead), "html to head in client");
                VerifyAreEqual(1,
                    Driver.FindElement(By.TagName("head"))
                        .FindElements(By.CssSelector("meta[name=\"customAttribute\"]")).Count,
                    "html to head displayed in head in client");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("AddHtmlToHead: " + ex.Message);
            }
            finally
            {
                ReInit();
            }

            GoToFunnelSettingsTab(1, "Настройки", "Счетчики", By.Name("BlockInHead"));
            Driver.ClearInput(By.Name("BlockInHead"));
            SaveFunnelSettings();
        }

        [Test]
        public void AddHtmlToBody()
        {
            string htmlToBody = "<div class=\"custom-class-for-body\">Custom block for body</div>";
            GoToClient("lp/testfunnel");
            VerifyIsFalse(Driver.PageSource.Contains(htmlToBody), "html to body is not exists");

            GoToFunnelSettingsTab(1, "Настройки", "Счетчики", By.Name("BlockInBodyBottom"));
            Driver.FindElement(By.Name("BlockInBodyBottom")).SendKeys(htmlToBody);
            SaveFunnelSettings();

            GoToClient("lp/testfunnel");
            VerifyIsTrue(Driver.PageSource.Contains(htmlToBody), "html to body was added");
            VerifyAreEqual(1,
                Driver.FindElement(By.TagName("body")).FindElements(By.ClassName("custom-class-for-body")).Count,
                "html to body displayed in body");

            try
            {
                ReInitClient();
                GoToClient("lp/testfunnel");
                VerifyIsTrue(Driver.PageSource.Contains(htmlToBody), "html to body in client");
                VerifyAreEqual(1,
                    Driver.FindElement(By.TagName("body")).FindElements(By.ClassName("custom-class-for-body")).Count,
                    "html to body displayed in body in client");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("AddHtmlToHead: " + ex.Message);
            }
            finally
            {
                ReInit();
            }

            GoToFunnelSettingsTab(1, "Настройки", "Счетчики", By.Name("BlockInBodyBottom"));
            Driver.ClearInput(By.Name("BlockInBodyBottom"));
            SaveFunnelSettings();
        }

        [Test]
        public void HideCopiright()
        {
            By linkSelector = By.CssSelector("a[href=\"https://www.advantshop.net/\"]");
            By imgSelector =
                By.CssSelector("img[src=\"https://cs71.advantshop.net/landing/landing_made_on_advantshop_ru.png\"]");
            try
            {
                ReInitClient();
                GoToClient("lp/testfunnel");
                VerifyAreEqual(1, Driver.FindElements(linkSelector).Count, "copyright link");
                VerifyAreEqual(1, Driver.FindElements(imgSelector).Count, "copyright img");

                ReInit();
                GoToFunnelSettingsTab(1, "Настройки", "Счетчики", By.Name("BlockInBodyBottom"));
                VerifyIsTrue(
                    Driver.FindElement(By.Name("HideAdvantshopCopyright")).GetAttribute("class").Contains("ng-empty"),
                    "copyright shown by default");
                Driver.CheckBoxCheck("HideAdvantshopCopyright");
                SaveFunnelSettings();

                ReInitClient();
                GoToClient("lp/testfunnel");
                VerifyAreEqual(0, Driver.FindElements(linkSelector).Count, "hidden copyright link");
                VerifyAreEqual(0, Driver.FindElements(imgSelector).Count, "hidden copyright link");

                ReInit();
                GoToFunnelSettingsTab(1, "Настройки", "Счетчики", By.Name("BlockInBodyBottom"));
                Driver.CheckBoxUncheck("HideAdvantshopCopyright");
                SaveFunnelSettings();
            }
            catch (Exception ex)
            {
                VerifyAddErrors("AddHtmlToHead: " + ex.Message);
            }
            finally
            {
                ReInit();
            }
        }
    }
}