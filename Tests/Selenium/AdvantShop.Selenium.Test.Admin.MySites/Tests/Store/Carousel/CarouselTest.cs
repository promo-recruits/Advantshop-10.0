using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store.Carousel
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class CarouselTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Store\\Carousel\\Catalog.Product.csv",
                "data\\Admin\\Store\\Carousel\\Catalog.Offer.csv",
                "data\\Admin\\Store\\Carousel\\Catalog.Category.csv",
                "data\\Admin\\Store\\Carousel\\Catalog.ProductCategories.csv",
                "data\\Admin\\Store\\Carousel\\CMS.Carousel.csv",
                "data\\Admin\\Store\\Carousel\\Catalog.Photo.csv"
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