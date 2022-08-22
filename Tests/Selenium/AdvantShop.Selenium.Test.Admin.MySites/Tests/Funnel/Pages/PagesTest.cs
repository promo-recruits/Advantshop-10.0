using System;
using System.Net;
using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.Pages
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class PagesTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\Pages\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\Pages\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\Pages\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\Pages\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\Pages\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\Pages\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\Pages\\CMS.LandingSubBlock.csv"
            );

            Init(false);
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            GoToAdmin("funnels/site/1");
            Refresh();
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CreatePage()
        {
            int pagesCount = GetFunnelPagesCount();
            Driver.FindElement(AdvBy.DataE2E("funnelAdd")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.Name("name"), "NewFunnelPage");
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save")).Click();
            Driver.WaitForElem(By.ClassName("lp-main"));
            VerifyAreEqual(BaseUrl + "lp/testfunnel/newfunnelpage?inplace=true", Driver.Url, "created funnel page");
            VerifyIsTrue(
                String.IsNullOrWhiteSpace(Driver.FindElement(By.ClassName("lp-main")).GetAttribute("innerHTML")),
                "created funnel content");
            GoToAdmin("funnels/site/1");
            VerifyAreEqual(pagesCount + 1, GetFunnelPagesCount(), "new pages count");
            VerifyAreEqual("NewFunnelPage", Driver.GetGridCellText(pagesCount, "Name"), "added page name");

            Driver.GetGridCell(pagesCount, "_serviceColumn").FindElement(By.ClassName("fa-times")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void CreatePageCancel()
        {
            int pagesCount = GetFunnelPagesCount();
            Driver.FindElement(AdvBy.DataE2E("funnelAdd")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.Name("name"), "NewFunnelPage");
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-cancel")).Click();
            Thread.Sleep(100);
            VerifyAreEqual(BaseUrl + "adminv3/funnels/site/1", Driver.Url, "canceled funnel page");
            VerifyAreEqual(pagesCount, GetFunnelPagesCount(), "canceled pages count");
        }

        [Test]
        public void CreatePageClose()
        {
            int pagesCount = GetFunnelPagesCount();
            Driver.FindElement(AdvBy.DataE2E("funnelAdd")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.Name("name"), "NewFunnelPage");
            Driver.FindElement(By.CssSelector(".close")).Click();
            Thread.Sleep(100);
            VerifyAreEqual(BaseUrl + "adminv3/funnels/site/1", Driver.Url, "canceled funnel page");
            VerifyAreEqual(pagesCount, GetFunnelPagesCount(), "canceled pages count");
        }

        [Test]
        public void CreatePageNameEmpty()
        {
            int pagesCount = GetFunnelPagesCount();
            Driver.FindElement(AdvBy.DataE2E("funnelAdd")).Click();
            Driver.WaitForModal();
            Driver.ClearInput(By.Name("name"));
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save")).Click();
            Driver.WaitForElem(By.ClassName("toast-error"));
            VerifyAreEqual("Укажите название воронки", Driver.FindElement(By.ClassName("toast-message")).Text.Trim(),
                "empty name message");
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-cancel")).Click();
        }

        [Test]
        public void CreatePageNameLong()
        {
            int pagesCount = GetFunnelPagesCount();
            Driver.FindElement(AdvBy.DataE2E("funnelAdd")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.Name("name"), Functions.SymbolsLong);
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save")).Click();
            Driver.WaitForElem(By.ClassName("lp-main"));
            VerifyAreEqual(
                BaseUrl + "lp/testfunnel/" +
                Functions.SymbolsLong.Substring(0, 100).ToLower().Replace(",", "").Replace(".", "").Replace(" ", "-") +
                "?inplace=true",
                Driver.Url, "created funnel page");
            VerifyIsTrue(
                String.IsNullOrWhiteSpace(Driver.FindElement(By.ClassName("lp-main")).GetAttribute("innerHTML")),
                "created funnel content");
            GoToAdmin("funnels/site/1");
            VerifyAreEqual(pagesCount + 1, GetFunnelPagesCount(), "new pages count");
            VerifyAreEqual(Functions.SymbolsLong.Substring(0, 100), Driver.GetGridCellText(pagesCount, "Name"),
                "added page name");

            Driver.GetGridCell(pagesCount, "_serviceColumn").FindElement(By.ClassName("fa-times")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void CreatePageNameSymbols()
        {
            int pagesCount = GetFunnelPagesCount();
            Driver.FindElement(AdvBy.DataE2E("funnelAdd")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.Name("name"), Functions.SymbolsString);
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save")).Click();
            Driver.WaitForElem(By.ClassName("lp-main"));
            VerifyAreEqual(BaseUrl + "lp/testfunnel/_?inplace=true", Driver.Url, "created funnel page");
            VerifyIsTrue(
                String.IsNullOrWhiteSpace(Driver.FindElement(By.ClassName("lp-main")).GetAttribute("innerHTML")),
                "created funnel content");
            GoToAdmin("funnels/site/1");
            VerifyAreEqual(pagesCount + 1, GetFunnelPagesCount(), "new pages count");
            VerifyAreEqual(Functions.SymbolsString, Driver.GetGridCellText(pagesCount, "Name"), "added page name");

            Driver.GetGridCell(pagesCount, "_serviceColumn").FindElement(By.ClassName("fa-times")).Click();
            Driver.SwalConfirm();
        }

        //просто установить другую страницу главной
        [Test]
        public void ChangeMainPage()
        {
            VerifyAreEqual("FunnelPage1", Driver.GetGridCellText(0, "Name"), "main page name");
            VerifyAreEqual("true",
                Driver.GetGridCell(0, "IsMain").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "main page is main");
            VerifyAreEqual("FunnelPage2", Driver.GetGridCellText(1, "Name"), "second page name");
            VerifyAreEqual("false",
                Driver.GetGridCell(1, "IsMain").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "second page not main");
            Driver.GetGridCell(1, "IsMain").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("FunnelPage2", Driver.GetGridCellText(0, "Name"), "new main page name");
            VerifyAreEqual("true",
                Driver.GetGridCell(0, "IsMain").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "new main page is main");
            VerifyAreEqual("FunnelPage1", Driver.GetGridCellText(1, "Name"), "new second page name");
            VerifyAreEqual("false",
                Driver.GetGridCell(1, "IsMain").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "new second page not main");

            Driver.GetGridCell(1, "IsMain").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Thread.Sleep(100);
        }

        //установить главную страницу неактивной
        [Test]
        public void ChangeMainPageToNotActive()
        {
            VerifyAreEqual("FunnelPage1", Driver.GetGridCellText(0, "Name"), "main page name");
            VerifyAreEqual("true",
                Driver.GetGridCell(0, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "main page enabled");
            VerifyAreEqual("FunnelPage2", Driver.GetGridCellText(1, "Name"), "second page name");
            Driver.GetGridCell(0, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("FunnelPage1", Driver.GetGridCellText(0, "Name"), "main page name not changed");
            VerifyAreEqual("true",
                Driver.GetGridCell(0, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "main page enabled not changed");
            VerifyAreEqual("FunnelPage2", Driver.GetGridCellText(1, "Name"), "second page name not changed");

            Driver.GetGridCell(1, "IsMain").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Thread.Sleep(100);
        }

        //установить главную страницу не главной
        [Test]
        public void ChangeMainPageToNotMain()
        {
            VerifyAreEqual("FunnelPage1", Driver.GetGridCellText(0, "Name"), "main page name");
            VerifyAreEqual("FunnelPage2", Driver.GetGridCellText(1, "Name"), "second page name");
            Driver.GetGridCell(0, "IsMain").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("FunnelPage2", Driver.GetGridCellText(0, "Name"), "new main page name");
            VerifyAreEqual("FunnelPage1", Driver.GetGridCellText(1, "Name"), "new second page name");

            Driver.GetGridCell(1, "IsMain").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Thread.Sleep(100);
        }

        //установить главной страницей неактивную
        [Test]
        public void SetMainPageNotActive()
        {
            VerifyAreEqual("FunnelPage1", Driver.GetGridCellText(0, "Name"), "main page name");
            VerifyAreEqual("FunnelPage3", Driver.GetGridCellText(2, "Name"), "third page name");
            VerifyAreEqual("false",
                Driver.GetGridCell(2, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "main page enabled");
            Driver.GetGridCell(2, "IsMain").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("FunnelPage3", Driver.GetGridCellText(0, "Name"), "new main page");
            VerifyAreEqual("true",
                Driver.GetGridCell(0, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "not enabled page is enabled");
            VerifyAreEqual("FunnelPage1", Driver.GetGridCellText(1, "Name"), "old main page");

            Driver.GetGridCell(1, "IsMain").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Refresh();
            Driver.GetGridCell(2, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
        }

        [Test]
        public void CopyPage()
        {
            int pagesCount = GetFunnelPagesCount();
            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.ClassName("lp-main"));
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("blocks-constructor-container")).Count,
                "blocks etalon count");
            string innerHtml = Driver.FindElement(By.CssSelector(".blocks-constructor-container .lp-container"))
                .GetAttribute("innerHTML");

            GoToAdmin("funnels/site/1");
            Driver.GetGridCell(1, "_serviceColumn").FindElement(By.ClassName("fa-clone")).Click();
            Driver.SwalConfirm();
            Driver.WaitForElem(By.TagName("ui-grid-custom-filter"));

            VerifyAreEqual(pagesCount + 1, GetFunnelPagesCount(), "page was be copied");
            VerifyAreEqual("FunnelPage2 - Копия", Driver.GetGridCellText(pagesCount, "Name"), "copied page name");
            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.ClassName("lp-main"));
            VerifyAreEqual(1, Driver.FindElements(By.ClassName("blocks-constructor-container")).Count,
                "blocks in copy count");
            VerifyAreEqual(innerHtml,
                Driver.FindElement(By.CssSelector(".blocks-constructor-container .lp-container"))
                    .GetAttribute("innerHTML"), "inner html in copy");

            GoToAdmin("funnels/site/1");
            Driver.GetGridCell(pagesCount, "_serviceColumn").FindElement(By.ClassName("fa-times")).Click();
            Driver.SwalConfirm();
        }

        [Test]
        public void CopyPageCancel()
        {
            int pagesCount = GetFunnelPagesCount();
            Driver.GetGridCell(1, "_serviceColumn").FindElement(By.ClassName("fa-clone")).Click();
            Driver.SwalCancel();
            VerifyAreEqual(pagesCount, GetFunnelPagesCount(), "page was not be copied");
            Refresh();
            VerifyAreEqual(pagesCount, GetFunnelPagesCount(), "page was not be copied2");
        }

        [Test]
        public void GoToPageByLink()
        {
            VerifyAreEqual(BaseUrl + "lp/testfunnel/funnelpage2?inplace=true",
                Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).GetAttribute("href"), "page link");
            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.ClassName("lp-main"));
            VerifyAreEqual(BaseUrl + "lp/testfunnel/funnelpage2?inplace=true",
                Driver.Url, "page url");
            VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "page link page status is OK");
        }

        [Test]
        public void DeleteMainPage()
        {
            VerifyAreEqual(0, Driver.GetGridCell(0, "_serviceColumn").FindElements(By.ClassName("fa-times")).Count,
                "main page not deleted");
        }

        [Test]
        public void DeletePage()
        {
            //cansel and confirm
            AddEmptyPage();
            int pagesCount = GetFunnelPagesCount();
            VerifyAreEqual("NewFunnelPage", Driver.GetGridCellText(pagesCount - 1, "Name"), "added page name");

            Driver.GetGridCell(pagesCount - 1, "_serviceColumn").FindElement(By.ClassName("fa-times")).Click();
            Driver.SwalCancel();
            VerifyAreEqual(pagesCount, GetFunnelPagesCount(), "old pages count");

            Driver.GetGridCell(pagesCount - 1, "_serviceColumn").FindElement(By.ClassName("fa-times")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual(pagesCount - 1, GetFunnelPagesCount(), "new pages count");
        }

        [Test]
        public void SetPageActive()
        {
            VerifyAreEqual("FunnelPage3", Driver.GetGridCellText(2, "Name"), "third page name");
            VerifyAreEqual("false",
                Driver.GetGridCell(2, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "third page not enabled");

            Driver.GetGridCell(2, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            VerifyAreEqual("true",
                Driver.GetGridCell(2, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "third page enabled");
            Driver.GetGridCell(2, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.TagName("body"));
            VerifyIsTrue(GetPageStatus(Driver.Url, true) == HttpStatusCode.OK, "page is enabled for admin");

            try
            {
                ReInitClient();
                GoToClient("lp/testfunnel/funnelpage3");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "page is enabled for client");
                GoToClient("lp/testfunnel/funnelpage3");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.OK, "page is enabled client mobile");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("Funnels.SetPageActive: ", ex.Message);
            }
            finally
            {
                ReInit();
            }

            GoToAdmin("funnels/site/1");

            Driver.GetGridCell(2, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
        }

        [Test]
        public void SetPageNotActive()
        {
            VerifyAreEqual("FunnelPage2", Driver.GetGridCellText(1, "Name"), "third page name");
            VerifyAreEqual("true",
                Driver.GetGridCell(1, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "second page is enabled");

            Driver.GetGridCell(1, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            VerifyAreEqual("false",
                Driver.GetGridCell(1, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffInput")).GetAttribute("value"),
                "second page not enabled");
            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(By.TagName("body"));
            VerifyIsTrue(GetPageStatus(Driver.Url, true) == HttpStatusCode.OK, "page is disable funnel for admin");

            try
            {
                ReInitClient();
                GoToClient("lp/testfunnel/funnelpage2");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.NotFound, "page is disable for client");
                GoToClient("lp/testfunnel/funnelpage2");
                VerifyIsTrue(GetPageStatus(Driver.Url) == HttpStatusCode.NotFound, "page is disable for client mobile");
            }
            catch (Exception ex)
            {
                VerifyAddErrors("Funnels.SetPageNotActive: ", ex.Message);
            }
            finally
            {
                ReInit();
            }

            GoToAdmin("funnels/site/1");

            Driver.GetGridCell(1, "Enabled").FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
        }

        public int GetFunnelPagesCount()
        {
            return Driver.FindElements(AdvBy.DataE2E("gridRow")).Count;
        }

        public void AddEmptyPage()
        {
            Driver.FindElement(AdvBy.DataE2E("funnelAdd")).Click();
            Driver.WaitForModal();
            Driver.SendKeysInput(By.Name("name"), "NewFunnelPage");
            Driver.FindElement(By.CssSelector(".modal-dialog .btn-save")).Click();
            Driver.WaitForElem(By.ClassName("lp-main"));
            GoToAdmin("funnels/site/1");
        }
    }
}