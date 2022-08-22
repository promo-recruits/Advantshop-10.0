using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.Settings
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class DomainsSettingsTest : MySitesFunctions
    {
        string defaultFunnelUrl = "testfunnel";

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
        public void AddDomain()
        {
            string domainName = "test.domain123.ru";
            Driver.SendKeysInput(By.CssSelector(".funnel-domains input"), domainName);
            Driver.FindElement(By.CssSelector(".funnel-domains .btn-success")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));
            VerifyAreEqual(domainName,
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain name");
            VerifyAreEqual(domainName,
                Driver.FindElement(AdvBy.DataE2E("funnelAddedDomains")).FindElement(By.TagName("a")).Text,
                "domain text");

            RemoveDomain();
        }

        [Test]
        public void AddDomainEmpty()
        {
            SetDomain("", "toast-error");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-message")).Text.Contains("Укажите доменное имя"),
                "empty domain name");
            VerifyAreEqual(BaseUrl + "lp/testfunnel",
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain text");
        }

        [Test]
        public void AddDomainCyrillic()
        {
            string domainName = "тестдомен.ру";
            SetDomain(domainName);
            VerifyAreEqual(domainName,
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain name");
            VerifyAreEqual(domainName,
                Driver.FindElement(AdvBy.DataE2E("funnelAddedDomains")).FindElement(By.TagName("a")).Text,
                "domain text");

            RemoveDomain();
        }

        [Test]
        public void AddDomainSymbols()
        {
            string domainName = Functions.SymbolsString;
            SetDomain(domainName, "toast-error");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-message")).Text.Contains("Укажите доменное имя"),
                "empty domain name");
            VerifyAreEqual(BaseUrl + "lp/testfunnel",
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain text");
        }

        [Test]
        public void AddDomainWithoutDot()
        {
            string domainName = "testdomain123ru";
            SetDomain(domainName, "toast-error");
            VerifyIsTrue(Driver.FindElement(By.ClassName("toast-message")).Text.Contains("Укажите доменное имя"),
                "empty domain name");
            VerifyAreEqual(BaseUrl + "lp/testfunnel",
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain text");
        }

        [Test]
        public void SetSiteUrl()
        {
            string siteUrl = "newtestfunnelurl123";
            SetSiteUrl(siteUrl);
            VerifyAreEqual(BaseUrl + "lp/" + siteUrl,
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain text");
            VerifyAreEqual(siteUrl, Driver.GetValue(By.Name("SiteUrl")), "url");

            SetSiteUrl(defaultFunnelUrl);
        }

        [Test]
        public void SetSiteUrlEmpty()
        {
            SetSiteUrl("", "toast-success");
            VerifyAreEqual(BaseUrl + "lp/" + defaultFunnelUrl,
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain text");
            VerifyAreEqual(defaultFunnelUrl, Driver.GetValue(By.Name("SiteUrl")), "url");
        }

        [Test]
        public void SetSiteUrlSymbols()
        {
            string siteUrl = Functions.SymbolsString;
            SetSiteUrl(siteUrl, "toast-success");
            VerifyAreEqual(BaseUrl + "lp/" + "_",
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain text");
            VerifyAreEqual("_", Driver.GetValue(By.Name("SiteUrl")), "url");

            SetSiteUrl(defaultFunnelUrl);
        }

        [Test]
        public void SetSiteUrlLong()
        {
            string domainName = Functions.SymbolsLong;
            SetSiteUrl(domainName, "toast-success");
            VerifyAreEqual(
                BaseUrl + "lp/" + domainName.Substring(0, 100).ToLower().Replace(",", "").Replace(".", "")
                    .Replace(" ", "-"),
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain name");

            SetSiteUrl(defaultFunnelUrl);
        }


        [Test]
        public void SetSiteUrlWithDotOrSlash()
        {
            string siteUrl = "newtest.funnel/url123";
            SetSiteUrl(siteUrl);
            siteUrl = siteUrl.Replace(".", "-").Replace("/", "-");
            VerifyAreEqual(BaseUrl + "lp/" + siteUrl,
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain text");
            VerifyAreEqual(siteUrl, Driver.GetValue(By.Name("SiteUrl")), "url");

            SetSiteUrl(defaultFunnelUrl);
        }

        [Test]
        public void UseExistingDomain()
        {
            Driver.FindElement(By.CssSelector("[ng-click=\"landingSite.addDomainMode='reuse'\"]")).Click();
            Driver.WaitForAjax();
            VerifyIsTrue(
                Driver.FindElement(By.ClassName("funnel-domains")).Text
                    .Contains("Пока нет подключенных доменов для подключения"), "empty tab with existing domains");
        }

        [Test]
        public void RemoveDomainCancel()
        {
            string domainName = "testdomain.ru";
            SetDomain(domainName);
            VerifyAreEqual(domainName,
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain name");
            VerifyAreEqual(domainName,
                Driver.FindElement(AdvBy.DataE2E("funnelAddedDomains")).FindElement(By.TagName("a")).Text,
                "domain text");

            Driver.FindElement(By.ClassName("fa-times")).Click();
            Driver.SwalCancel();
            VerifyAreEqual(domainName,
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain text");

            RemoveDomain();
        }

        [Test]
        public void RemoveDomainConfirm()
        {
            string domainName = "testdomain.ru";
            SetDomain(domainName);
            VerifyAreEqual(domainName,
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain name");
            VerifyAreEqual(domainName,
                Driver.FindElement(AdvBy.DataE2E("funnelAddedDomains")).FindElement(By.TagName("a")).Text,
                "domain text");

            RemoveDomain();
            VerifyAreEqual(BaseUrl + "lp/testfunnel",
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text, "domain text");
        }

        public void SetDomain(string domainName, string waitElemClass = "toast-success")
        {
            Driver.SendKeysInput(By.CssSelector(".funnel-domains input"), domainName);
            Driver.FindElement(By.CssSelector(".funnel-domains .btn-success")).Click();
            Driver.WaitForElem(By.ClassName(waitElemClass));
        }

        public void RemoveDomain()
        {
            Driver.FindElement(By.ClassName("fa-times")).Click();
            Driver.SwalConfirm();
            Driver.WaitForElem(By.ClassName("toast-success"));
            Thread.Sleep(100);
        }

        public void SetSiteUrl(string siteUrl, string waitElemClass = "toast-success")
        {
            Driver.SendKeysInput(By.Name("SiteUrl"), siteUrl);
            SaveFunnelSettings(By.ClassName(waitElemClass));
            Refresh(); //чтоб обновилось значение value, иначе getattribute даст устаревшее
        }
    }
}