using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Category.CategoriesCatalog
{
    [TestFixture]
    public class CategoriesNoTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\NoCategoryTest\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\NoCategoryTest\\Catalog.Offer.csv"
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
        public void OpenNoCategories()
        {
            GoToAdmin("catalog");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной категории не найдено"));
        }
    }
}