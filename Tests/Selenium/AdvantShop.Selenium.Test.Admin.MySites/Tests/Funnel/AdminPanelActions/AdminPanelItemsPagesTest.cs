using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.AdminPanelActions
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class AdminPanelItemsPagesTest : MySitesFunctions
    {
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
        public void GoToAllPages()
        {
            GoToClient("lp/testfunnel_1/funnelpage2");
            APItemPagesClick();
            VerifyAreEqual("Все страницы", Driver.FindElements(By.ClassName("lp-admin-menu__link"))[3].Text,
                "all pages");
            Driver.FindElements(By.ClassName("lp-admin-menu__link"))[3].Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("funnel-page__name"));
            VerifyAreEqual(BaseUrl + "adminv3/funnels/site/1", Driver.Url, "expected page url");
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void GoToMainPage()
        {
            GoToClient("lp/testfunnel_1/funnelpage3");
            APItemPagesClick();
            VerifyAreEqual("FunnelPage1", Driver.FindElements(By.ClassName("lp-admin-menu__link"))[0].Text,
                "first page");
            Driver.FindElements(By.ClassName("lp-admin-menu__link"))[0].Click();
            Driver.WaitForElem(By.ClassName("block-type-form"));
            VerifyAreEqual(BaseUrl + "lp/testfunnel_1", Driver.Url, "expected page url");
        }

        [Test]
        public void GoToMainPageDisableInplaceMode()
        {
            GoToClient("lp/testfunnel_1/funnelpage3");
            APItemInplaceModeClick(true);
            Thread.Sleep(100);

            APItemPagesClick();
            VerifyAreEqual("FunnelPage1", Driver.FindElements(By.ClassName("lp-admin-menu__link"))[0].Text,
                "first page");
            Driver.FindElements(By.ClassName("lp-admin-menu__link"))[0].Click();
            Driver.WaitForElem(By.ClassName("block-type-form"));
            VerifyAreEqual(BaseUrl + "lp/testfunnel_1", Driver.Url, "expected page url");

            APItemInplaceModeClick(false);
        }

        [Test]
        public void GoToMainPageMobile()
        {
            GoToMobile("lp/testfunnel_1/funnelpage3");

            VerifyIsFalse(Driver.FindElement(AdvBy.DataE2E("LpAdminMenuItemPages")).Displayed,
                "menu item 'pages' is not enable in mobile");

            Driver.Manage().Window.Maximize();
        }

        [Test]
        public void GoToNotMainPage()
        {
            GoToClient("lp/testfunnel_1");
            APItemPagesClick();
            VerifyAreEqual("FunnelPage2", Driver.FindElements(By.ClassName("lp-admin-menu__link"))[1].Text,
                "second page");
            Driver.FindElements(By.ClassName("lp-admin-menu__link"))[1].Click();
            Driver.WaitForElem(By.ClassName("block-type-text"));
            VerifyAreEqual(BaseUrl + "lp/testfunnel_1/funnelpage2", Driver.Url, "expected page url");
        }
    }
}