using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.GeneratedCoupons
{
    [TestFixture]
    public class GeneratedCouponsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\GeneratedCoupons\\Catalog.Brand.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Color.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Size.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Product.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Photo.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Offer.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Category.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.ProductCategories.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.Coupon.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.CouponCategories.csv",
                "data\\Admin\\GeneratedCoupons\\Catalog.CouponProducts.csv"
            );

            Init();
            GoToAdmin("settingscoupons#?couponsTab=couponsGenerated");
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
        public void GeneratedCouponsSearch()
        {
            Driver.GridFilterSendKeys("test2");
            Thread.Sleep(100);
            VerifyAreEqual("test2", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, " find value");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void GeneratedCouponsSearchNotExist()
        {
            //search not exist product
            Driver.GridFilterSendKeys("test111");
            Thread.Sleep(100);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void GeneratedCouponsSearchMuch()
        {
            //search too much symbols
            Driver.GridFilterSendKeys("1111111111222222222223333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Thread.Sleep(100);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void GeneratedCouponsSearchInvalid()
        {
            //search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            Thread.Sleep(100);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }


        [Test]
        public void xGeneratedCouponsInplace()
        {
            GoToAdmin("settingscoupons#?couponsTab=couponsGenerated");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "grid Code");
            Driver.SendKeysGridCell("edit1", 0, "Code", "Couponsgenerated");
            VerifyAreEqual("edit1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "grid edit Code");
            VerifyAreEqual("100", Driver.GetGridCell(0, "Value", "Couponsgenerated").Text, "grid  Value");
            Driver.SendKeysGridCell("1000", 0, "Value", "Couponsgenerated");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Value", "Couponsgenerated").Text, "grid edit Value");
            VerifyAreEqual("0", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsgenerated").Text, "grid Price ");
            Driver.SendKeysGridCell("10", 0, "MinimalOrderPrice", "Couponsgenerated");
            VerifyAreEqual("10", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsgenerated").Text, "grid edit Price");

            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled", "Couponsgenerated")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "grid Enabled ");
            Driver.GetGridCell(0, "Enabled", "Couponsgenerated")
                .FindElement(AdvBy.DataE2E("switchOnOffLabel")).Click();
            Thread.Sleep(500);
            VerifyIsFalse(
                Driver.GetGridCell(0, "Enabled", "Couponsgenerated")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "grid edit Enabled ");
            Refresh();
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyAreEqual("edit1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "Code ");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Value", "Couponsgenerated").Text, "Value ");
            VerifyAreEqual("10", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsgenerated").Text, " Price");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Enabled", "Couponsgenerated")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, " Enabled");
        }
    }
}