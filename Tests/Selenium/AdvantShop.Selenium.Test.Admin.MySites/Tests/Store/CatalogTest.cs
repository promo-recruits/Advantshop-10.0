using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Store
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    class CatalogTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.None);
            InitializeService.LoadData();

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
        public void DeleteDemoCatalogNOTCOMPLETE()
        {
            //проверить исходные данные по каждому пункту
            //проверить удаление без галки "я действительно хочу удалить"
            //Удалить демо-каталог (ничего не выбрано)
            //Удалить демо-каталог (только товары и категории) 
            //Удалить демо-каталог (все)
            //сложность: нужна триалка

            VerifyIsTrue(false, "test not work");
        }
    }
}