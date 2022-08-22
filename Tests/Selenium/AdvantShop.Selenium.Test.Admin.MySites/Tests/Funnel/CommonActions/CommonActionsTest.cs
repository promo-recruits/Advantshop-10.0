using System;
using System.Net;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.CommonActions
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class CommonActionsTest : MySitesFunctions
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
            GoToAdmin("funnels/site/1");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CheckBreadCrumb()
        {
            VerifyAreEqual("Мои сайты", Driver.FindElement(By.ClassName("breadcrumb__link--admin")).Text.Trim(),
                "breadcrumb text");
            VerifyAreEqual(BaseUrl + "adminv3/dashboard",
                Driver.FindElement(By.ClassName("breadcrumb__link--admin")).GetAttribute("href"),
                "breadcrumb link url");
            Driver.FindElement(By.ClassName("breadcrumb__link--admin")).Click();
            Driver.WaitForElem(By.ClassName("title-page__dashboard"));
            VerifyAreEqual(BaseUrl + "adminv3/dashboard", Driver.Url, "breadcrumb link url");
        }

        [Test]
        public void SetInplaceEditorValid()
        {
            VerifyAreEqual("TestFunnel", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(),
                "default header");

            Driver.SendKeysInput(AdvBy.DataE2E("funnelTitle"), "New test funnel name");
            Driver.FindElement(By.TagName("simple-edit-trigger")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));
            VerifyAreEqual("New test funnel name", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(),
                "new header");
            Refresh();
            VerifyAreEqual("New test funnel name", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(),
                "new header after refresh");
            GoToAdmin("dashboard");
            VerifyAreEqual("New test funnel name", Driver.FindElement(By.CssSelector(".dashboard-site-name a")).Text,
                "new header at dashboard");

            SetDefaultFunnelName();
        }

        [Test]
        public void SetInplaceEditorEmpty()
        {
            VerifyAreEqual("TestFunnel", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(),
                "default header");

            Driver.ClearInput(AdvBy.DataE2E("funnelTitle"));
            Driver.FindElement(By.TagName("simple-edit-trigger")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));
            VerifyAreEqual("TestFunnel", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(), "new header");
            Refresh();
            VerifyAreEqual("TestFunnel", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(),
                "new header after refresh");
            GoToAdmin("dashboard");
            VerifyAreEqual("TestFunnel", Driver.FindElement(By.CssSelector(".dashboard-site-name a")).Text,
                "new header at dashboard");
        }

        [Test]
        public void SetInplaceEditorSymbols()
        {
            VerifyAreEqual("TestFunnel", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(),
                "default header");

            Driver.SendKeysInput(AdvBy.DataE2E("funnelTitle"), Functions.SymbolsString);
            Driver.FindElement(By.TagName("simple-edit-trigger")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));
            VerifyAreEqual(Functions.SymbolsString, Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(),
                "new header");
            Refresh();
            VerifyAreEqual("`~!@#№$;%^:&amp;?*()-_=+[{]};:'&quot;\\|/.,,&lt;.&gt;/?",
                Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(), "new header after refresh");
            GoToAdmin("dashboard");
            VerifyAreEqual("`~!@#№$;%^:&amp;?*()-_=+[{]};:'&quot;\\|/.,,&lt;.&gt;/?",
                Driver.FindElement(By.CssSelector(".dashboard-site-name a")).Text, "new header at dashboard");

            SetDefaultFunnelName();
        }

        [Test]
        public void SetInplaceEditorLongName()
        {
            VerifyAreEqual("TestFunnel", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(),
                "default header");

            Driver.SendKeysInput(AdvBy.DataE2E("funnelTitle"), Functions.SymbolsLong);
            Driver.FindElement(By.TagName("simple-edit-trigger")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));
            VerifyAreEqual(Functions.SymbolsLong.Substring(0, 100),
                Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(), "new header");
            Refresh();
            VerifyAreEqual(Functions.SymbolsLong.Substring(0, 100),
                Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(), "new header after refresh");
            GoToAdmin("dashboard");
            VerifyAreEqual(Functions.SymbolsLong.Substring(0, 100),
                Driver.FindElement(By.CssSelector(".dashboard-site-name a")).Text, "new header at dashboard");

            SetDefaultFunnelName();
        }

        [Test]
        public void CreateFunnelCopy()
        {
            GoToAdmin("dashboard");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("dashboard-site__block")).Count, "blocks before copy");
            GoToAdmin("funnels/site/1");
            VerifyAreEqual("Создать копию воронки", Driver.FindElement(AdvBy.DataE2E("funnelCopy")).Text,
                "copy link text");
            VerifyAreEqual(3, Driver.FindElements(AdvBy.DataE2E("gridRow")).Count, "default pages count");
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Text,
                "default funnel page name");

            Driver.FindElement(AdvBy.DataE2E("funnelCopy")).Click();
            Driver.SwalConfirm();
            Thread.Sleep(100);
            Driver.WaitForElem(By.ClassName("funnel-page__name"));
            VerifyAreEqual("TestFunnel - Копия", Driver.FindElement(AdvBy.DataE2E("funnelTitle")).Text.Trim(),
                "header after copy");
            VerifyAreEqual(3, Driver.FindElements(AdvBy.DataE2E("gridRow")).Count, "copied funnel pages count");
            VerifyAreEqual("FunnelPage1", Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Text,
                "copied funnel page name");
            GoToAdmin("dashboard");
            VerifyAreEqual(2, Driver.FindElements(By.ClassName("dashboard-site__block")).Count, "blocks after copy");
            Driver.FindElements(By.ClassName("dashboard-site__block"))[1]
                .FindElement(By.ClassName("link-alternative--delete")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void CreateFunnelCopyCancel()
        {
            GoToAdmin("dashboard");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("dashboard-site__block")).Count, "blocks before copy");
            GoToAdmin("funnels/site/1");
            VerifyAreEqual("Создать копию воронки", Driver.FindElement(AdvBy.DataE2E("funnelCopy")).Text,
                "copy link text");
            Driver.FindElement(AdvBy.DataE2E("funnelCopy")).Click();
            Driver.SwalCancel();
            Thread.Sleep(100);
            GoToAdmin("dashboard");
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("dashboard-site__block")).Count, "blocks after copy");
        }

        [Test]
        public void SetFunnelPublished()
        {
            VerifyAreEqual("Опубликован", Driver.FindElement(AdvBy.DataE2E("funnelEnablerBtn")).Text,
                "enabled btn text");
            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.TagName("body"));
            VerifyIsTrue(GetPageStatus(Driver.Url, true) == HttpStatusCode.OK, "enabled funnel for admin");

            try
            {
                ReInitClient();
                GoToClient("lp/testfunnel");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "enabled funnel for client");
                GoToMobile("lp/testfunnel");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "enabled funnel for client mobile");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("Funnels.SetFunnelPublished: ", ex.Message);
            }
            finally
            {
                ReInit();
            }
        }

        [Test]
        public void SetFunnelNotPublished()
        {
            VerifyAreEqual("Опубликован", Driver.FindElement(AdvBy.DataE2E("funnelEnablerBtn")).Text,
                "default btn text");
            Driver.FindElement(AdvBy.DataE2E("funnelEnablerBtn")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("Не опубликован", Driver.FindElement(AdvBy.DataE2E("funnelEnablerBtn")).Text,
                "not enabled btn text");
            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.TagName("body"));
            VerifyIsTrue(GetPageStatus(Driver.Url, true) == HttpStatusCode.OK, "not enabled funnel for admin");

            try
            {
                ReInitClient();
                GoToClient("lp/testfunnel");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.NotFound, "not enabled funnel for client");
                GoToMobile("lp/testfunnel");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.NotFound,
                    "not enabled funnel for client mobile");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("Funnels.SetFunnelNotPublished: ", ex.Message);
            }
            finally
            {
                ReInit();
                GoToAdmin("funnels/site/1");
                Driver.FindElement(AdvBy.DataE2E("funnelEnablerBtn")).Click();
                Thread.Sleep(100);
            }
        }

        [Test]
        public void CheckChangeDomainPage()
        {
            VerifyAreEqual("Изменить домен", Driver.FindElement(AdvBy.DataE2E("funnelDomainChange")).Text,
                "domain page link text");
            Driver.FindElement(AdvBy.DataE2E("funnelDomainChange")).Click();
            Driver.WaitForAjax();
            VerifyAreEqual(BaseUrl + "adminv3/funnels/site/1#?landingAdminTab=settings&landingSettingsTab=domains",
                Driver.Url, "domain page link");
            VerifyAreEqual("Домены", Driver.FindElement(By.CssSelector(".nav-item.active a")).Text.Trim(),
                "active tab text");
            VerifyAreEqual("testfunnel", GetInputValue("SiteUrl"), "domain url");
        }

        [Test]
        public void CheckGoToSite()
        {
            VerifyAreEqual("Перейти на сайт", Driver.FindElement(AdvBy.DataE2E("funnelGoToSite")).Text,
                "gotosite link text");
            VerifyAreEqual(BaseUrl + "lp/testfunnel?inplace=true",
                Driver.FindElement(AdvBy.DataE2E("funnelGoToSite")).GetAttribute("href"), "gotosite link url");
            Driver.FindElement(AdvBy.DataE2E("funnelGoToSite")).Click();
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual(BaseUrl + "lp/testfunnel?inplace=true", Driver.Url, "site url");
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void CheckGoToSiteByDomain()
        {
            VerifyAreEqual(BaseUrl + "lp/testfunnel",
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).Text.Trim(),
                "site domain link url");
            VerifyAreEqual(BaseUrl + "lp/testfunnel",
                Driver.FindElement(AdvBy.DataE2E("funnelDomains")).FindElement(By.TagName("a")).GetAttribute("href"),
                "site domain link url");
            Driver.FindElement(AdvBy.DataE2E("funnelDomains")).Click();
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual(BaseUrl + "lp/testfunnel", Driver.Url, "site domain url");
            Functions.CloseTab(Driver, BaseUrl);
        }

        public void SetDefaultFunnelName()
        {
            GoToAdmin("funnels/site/1");
            Driver.SendKeysInput(AdvBy.DataE2E("funnelTitle"), "TestFunnel");
            Driver.FindElement(By.TagName("simple-edit-trigger")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));
        }
    }
}