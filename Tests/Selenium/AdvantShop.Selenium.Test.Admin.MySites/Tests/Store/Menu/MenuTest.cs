using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store.Menu
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class MenuTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Store\\Menu\\CMS.Menu.csv",
                "data\\Admin\\Store\\Menu\\CMS.StaticBlock.csv",
                "data\\Admin\\Store\\Menu\\CMS.StaticPage.csv"
            );

            Init();
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