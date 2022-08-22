using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.BlockActions
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class BlockActionsTest : MySitesFunctions
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
            GoToAdmin("dashboard");
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
        public void TestNameNOTCOMPLETE()
        {
            VerifyIsTrue(false, "test not work!");
        }
    }
}