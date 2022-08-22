using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store.News
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class NewsTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Store\\News\\Settings.NewsCategory.csv",
                "data\\Admin\\Store\\News\\Settings.News.csv",
                "data\\Admin\\Store\\News\\Catalog.Photo.csv"
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