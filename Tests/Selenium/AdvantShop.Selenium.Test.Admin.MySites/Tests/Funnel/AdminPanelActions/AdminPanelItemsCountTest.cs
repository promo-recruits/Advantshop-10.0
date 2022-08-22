using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.AdminPanelActions
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class AdminPanelItemsCountTest : MySitesFunctions
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
        public void CountForFunnelWithSinglePage()
        {
            GoToClient("lp/testfunnel_2");
            VerifyAreEqual(3, Driver.FindElements(By.ClassName("lp-admin-panel-link")).Count,
                "admin panel items count for funnel with single page");
            VerifyAreEqual(0, Driver.FindElements(AdvBy.DataE2E("LpAdminMenuItemPages")).Count, "change page link");
        }

        [Test]
        public void CountForFunnelWithMultiplePages()
        {
            GoToClient("lp/testfunnel_1");
            VerifyAreEqual(4, Driver.FindElements(By.ClassName("lp-admin-panel-link")).Count,
                "admin panel items count for funnel with multiple pages");
            VerifyAreEqual(1, Driver.FindElements(AdvBy.DataE2E("LpAdminMenuItemPages")).Count,
                "change page link count");
            VerifyAreEqual(4, Driver.FindElements(By.ClassName("lp-admin-menu__link")).Count, "pages link count");
        }
    }
}