using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.AdminPanelActions
{
    //действия элементов из lp-admin-panel (серая панель сбоку)

    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class AdminPanelItemsBackTest : MySitesFunctions
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
            GoToClient("lp/testfunnel_1");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void BackFromMainPage()
        {
            APItemBackClick();
            VerifyAreEqual(BaseUrl + "adminv3/funnels/site/1", Driver.Url, "expected funnels url");
        }

        [Test]
        public void BackFromNotMainPage()
        {
            APItemPagesClick(1, By.ClassName("lp-block-text-single"));
            APItemBackClick();
            VerifyAreEqual(BaseUrl + "adminv3/funnels/site/1", Driver.Url, "expected funnels url");
        }

        [Test]
        public void BackFromMobileVersion()
        {
            GoToMobile("lp/testfunnel_1");
            APItemBackClick();
            VerifyAreEqual(BaseUrl + "adminv3/funnels/site/1", Driver.Url, "expected funnels url");
            Driver.Manage().Window.Maximize();
        }

        [Test]
        public void BackFromDisableInplaceMode()
        {
            APItemInplaceModeClick(true);
            APItemBackClick();
            VerifyAreEqual(BaseUrl + "adminv3/funnels/site/1", Driver.Url, "expected funnels url");

            GoToClient("lp/testfunnel_1");
            APItemInplaceModeClick(false);
        }
    }
}