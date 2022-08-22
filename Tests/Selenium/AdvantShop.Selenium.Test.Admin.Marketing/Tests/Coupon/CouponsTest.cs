using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Coupon
{
    [TestFixture]
    public class CouponsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Coupons\\Catalog.Brand.csv",
                "data\\Admin\\Coupons\\Catalog.Color.csv",
                "data\\Admin\\Coupons\\Catalog.Size.csv",
                "data\\Admin\\Coupons\\Catalog.Product.csv",
                "data\\Admin\\Coupons\\Catalog.Photo.csv",
                "data\\Admin\\Coupons\\Catalog.Offer.csv",
                "data\\Admin\\Coupons\\Catalog.Category.csv",
                "data\\Admin\\Coupons\\Catalog.ProductCategories.csv",
                "data\\Admin\\Coupons\\Catalog.Coupon.csv",
                "data\\Admin\\Coupons\\Catalog.CouponCategories.csv",
                "data\\Admin\\Coupons\\Catalog.CouponProducts.csv"
            );

            Init();
            GoToAdmin("settingscoupons#?couponsTab=coupons");
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
        public void CouponSearch()
        {
            Driver.GridFilterSendKeys("test2");
            VerifyAreEqual("test2", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, " find value");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CouponSearchNotExist()
        {
            //search not exist product
            Driver.GridFilterSendKeys("test111");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CouponSearchMuch()
        {
            //search too much symbols
            Driver.GridFilterSendKeys("1111111111222222222223333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void CouponSearchInvalid()
        {
            //search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }


        [Test]
        public void xCouponInplace()
        {
            GoToAdmin("settingscoupons#?couponsTab=coupons");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "grid Code");
            Driver.SendKeysGridCell("edit1", 0, "Code", "Couponsdefault");
            VerifyAreEqual("edit1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "grid edit Code");
            VerifyAreEqual("100", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "grid  Value");
            Driver.SendKeysGridCell("1000", 0, "Value", "Couponsdefault");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "grid edit Value");
            VerifyAreEqual("0", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text, "grid Price ");
            Driver.SendKeysGridCell("10", 0, "MinimalOrderPrice", "Couponsdefault");
            VerifyAreEqual("10", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text, "grid edit Price");

            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "grid Enabled ");
            Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                .FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Thread.Sleep(500);
            VerifyIsFalse(
                Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "grid edit Enabled ");
            Refresh();
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyAreEqual("edit1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "Code ");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "Value ");
            VerifyAreEqual("10", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text, " Price");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, " Enabled");
        }
    }
}