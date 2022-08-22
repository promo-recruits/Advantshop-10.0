using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Discount
{
    [TestFixture]
    public class DiscountsPriceRangePresentTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\Discount\\Catalog.Product.csv",
                "data\\Admin\\Discount\\Catalog.Offer.csv",
                "data\\Admin\\Discount\\Catalog.Category.csv",
                "data\\Admin\\Discount\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Discount\\[Order].OrderSource.csv",
                "data\\Admin\\Discount\\[Order].OrderStatus.csv",
                "data\\Admin\\Discount\\[Order].OrderPriceDiscount.csv"
            );

            Init();
            GoToAdmin("settingscoupons#?couponsTab=discounts");
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
        public void Present10()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.PageSelectItems("10");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "line 1");
            VerifyAreEqual("20", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "line 10");
        }

        [Test]
        public void Present20()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.PageSelectItems("20");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "line 1");
            VerifyAreEqual("30", Driver.GetGridCell(19, "PriceRange", "PriceRange").Text, "line 20");
        }

        [Test]
        public void Present50()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.PageSelectItems("50");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "line 1");
            VerifyAreEqual("60", Driver.GetGridCell(49, "PriceRange", "PriceRange").Text, "line 50");
        }

        [Test]
        public void Present100()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.PageSelectItems("100");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "line 1");
            VerifyAreEqual("110", Driver.GetGridCell(99, "PriceRange", "PriceRange").Text, "line 100");
        }
    }
}