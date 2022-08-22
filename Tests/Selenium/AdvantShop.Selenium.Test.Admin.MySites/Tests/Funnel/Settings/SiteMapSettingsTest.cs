using System;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.Settings
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class SiteMapSettingsTest : MySitesFunctions
    {
        string dateTime = "";

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
            GoToFunnelSettingsTab(1, "Настройки", "Домены", By.Name("SiteUrl"));
            Driver.SendKeysInput(By.CssSelector(".funnel-domains input"), "testdomain.ru");
            Driver.FindElement(By.CssSelector(".funnel-domains .btn-success")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToFunnelSettingsTab(1, "Настройки", "Карта сайта и robots.txt", By.Name("UseHttpsForSitemap"));
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void GenerateSiteMap()
        {
            Driver.FindElement(AdvBy.DataE2E("generateSitemapXml")).Click();
            dateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            Driver.WaitForElem(By.ClassName("toast-success"));
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("http://testdomain.ru/sitemap.xml", Driver.FindElement(AdvBy.DataE2E("SitemapXmlUrl")).Text,
                "sitemap xml name");
            VerifyAreEqual(dateTime, Driver.FindElement(AdvBy.DataE2E("lastDateXml")).Text.Trim(), "last date xml");

            Thread.Sleep(1000); //для избежание ошибки со второй датой 

            Driver.FindElement(AdvBy.DataE2E("generateSitemapHtml")).Click();
            dateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            Driver.WaitForElem(By.ClassName("toast-success"));
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("http://testdomain.ru/sitemap.html",
                Driver.FindElement(AdvBy.DataE2E("SitemapHtmlUrl")).Text, "sitemap html name");
            VerifyAreEqual(dateTime, Driver.FindElement(AdvBy.DataE2E("lastDateHtml")).Text.Trim(), "last date html");
        }

        [Test]
        public void GenerateSiteMapWithHttps()
        {
            Driver.CheckBoxCheck("UseHttpsForSitemap");
            SaveFunnelSettings();

            Driver.FindElement(AdvBy.DataE2E("generateSitemapXml")).Click();
            dateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            Driver.WaitForElem(By.ClassName("toast-success"));
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("https://testdomain.ru/sitemap.xml", Driver.FindElement(AdvBy.DataE2E("SitemapXmlUrl")).Text,
                "sitemap xml name");
            VerifyIsTrue(Driver.FindElement(AdvBy.DataE2E("lastDateXml")).Text.Contains(dateTime), "last date xml");

            Driver.FindElement(AdvBy.DataE2E("generateSitemapHtml")).Click();
            dateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            Driver.WaitForElem(By.ClassName("toast-success"));
            Driver.FindElement(By.ClassName("toast-close-button")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("https://testdomain.ru/sitemap.html",
                Driver.FindElement(AdvBy.DataE2E("SitemapHtmlUrl")).Text, "sitemap html name");
            VerifyIsTrue(Driver.FindElement(AdvBy.DataE2E("lastDateHtml")).Text.Contains(dateTime), "last date html");

            Driver.CheckBoxUncheck("UseHttpsForSitemap");
            SaveFunnelSettings();
        }

        [Test]
        public void SetRobotsTxt()
        {
            string robots = @"User-agent: * " + Keys.Enter + "Disallow: /";
            Driver.FindElement(By.CssSelector(".tab-pane.active .ace_text-input")).SendKeys(robots);
            SaveFunnelSettings();
            Refresh();

            VerifyAreEqual("User-agent: * \r\nDisallow: /",
                Driver.FindElement(By.CssSelector(".tab-pane.active .ace_scroller")).Text.Trim(), "robots text");
            Driver.FindElement(By.CssSelector(".tab-pane.active .ace_text-input"))
                .SendKeys(Keys.Control + "a" + Keys.Delete);
            SaveFunnelSettings();
        }
    }
}