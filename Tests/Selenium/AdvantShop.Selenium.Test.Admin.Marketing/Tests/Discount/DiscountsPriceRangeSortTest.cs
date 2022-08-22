using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Discount
{
    [TestFixture]
    public class DiscountsPriceRangeSortTest : BaseSeleniumTest
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
        public void ByPriceRange()
        {
            Driver.GetGridCell(-1, "PriceRange", "PriceRange").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "Price Range 1 asc");
            VerifyAreEqual("20", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "Price Range 10 asc");

            Driver.GetGridCell(-1, "PriceRange", "PriceRange").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("180", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "Price Range 1 desc");
            VerifyAreEqual("171", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "Price Range 10 desc");

            Driver.GetGridCell(-1, "PriceRange", "PriceRange").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "Price Range 1 asc 2");
            VerifyAreEqual("20", Driver.GetGridCell(9, "PriceRange", "PriceRange").Text, "Price Range 10 asc 2");
        }


        [Test]
        public void ByPercentDiscount()
        {
            Driver.GetGridCell(-1, "PercentDiscount", "PriceRange").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("1", Driver.GetGridCell(0, "PercentDiscount", "PriceRange").Text, "Percent Discount 1 asc");
            VerifyAreEqual("5", Driver.GetGridCell(9, "PercentDiscount", "PriceRange").Text, "Percent Discount 10 asc");

            Driver.GetGridCell(-1, "PercentDiscount", "PriceRange").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("90", Driver.GetGridCell(0, "PercentDiscount", "PriceRange").Text,
                "Percent Discount 1 desc");
            VerifyAreEqual("81", Driver.GetGridCell(9, "PercentDiscount", "PriceRange").Text,
                "Percent Discount 10 desc");

            Driver.GetGridCell(-1, "PercentDiscount", "PriceRange").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("1", Driver.GetGridCell(0, "PercentDiscount", "PriceRange").Text, "Percent Discount 1 asc 2");
            VerifyAreEqual("10", Driver.GetGridCell(9, "PercentDiscount", "PriceRange").Text, "Percent Discount 10 asc 2");
        }
    }
}