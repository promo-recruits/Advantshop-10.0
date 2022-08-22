using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.ProductsOnMainPage.ProductLists
{
    [TestFixture]
    public class ProductListsNoTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\ProductListsNo\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\ProductListsNo\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\ProductListsNo\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\ProductsOnMainPage\\ProductLists\\ProductListsNo\\Catalog.ProductCategories.csv"
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
        public void ProductListNo()
        {
            GoToAdmin("mainpageproductsstore");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".fas.fa-times")).Count == 0);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"productListAdd\"]")).Count == 1);

            GoToClient();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".products-specials-list")).Count == 0);
        }
    }
}