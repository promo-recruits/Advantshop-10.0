using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.GeneratedCoupons
{
    [TestFixture]
    public class GeneratedCouponsSortTest : BaseSeleniumTest
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
        public void GeneratedCouponsSortCode()
        {
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "grid Code");
            VerifyAreEqual("100", Driver.GetGridCell(0, "Value", "Couponsgenerated").Text, "grid Value");
            VerifyAreEqual("0", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsgenerated").Text,
                "grid MinimalOrderPrice");
            VerifyAreEqual("Фиксированный", Driver.GetGridCell(0, "TypeFormatted", "Couponsgenerated").Text,
                "grid TypeFormatted");
            VerifyAreEqual("Бессрочно", Driver.GetGridCell(0, "ExpirationDateFormatted", "Couponsgenerated").Text,
                "grid ExpirationDate");
            VerifyAreEqual("0 / -", Driver.GetGridCell(0, "ActualUses", "Couponsgenerated").Text, "grid ActualUses");
            VerifyAreEqual("21.11.2016 11:10", Driver.GetGridCell(0, "AddingDateFormatted", "Couponsgenerated").Text,
                "grid AddingDate");

            Driver.GetGridCell(-1, "Code", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "sort Code ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "sort Code DESC  value5");

            Driver.GetGridCell(-1, "Code", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test6", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "sort Code ASC  value1");
            VerifyAreEqual("test1", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "sort Code DESC  value5");
        }

        [Test]
        public void GeneratedCouponsSortValue()
        {
            Driver.GetGridCell(-1, "Value", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test3", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "sort Value ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "sort Value DESC  value5");

            Driver.GetGridCell(-1, "Value", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test2", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "sort Value ASC  value1");
            VerifyAreEqual("test3", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "sort Value DESC  value5");
        }

        [Test]
        public void GeneratedCouponsSortMinimalOrderPrice()
        {
            Driver.GetGridCell(-1, "MinimalOrderPrice", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text,
                "sort MinimalOrderPrice ASC  value1");
            VerifyAreEqual("test4", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text,
                "sort MinimalOrderPrice DESC  value5");

            Driver.GetGridCell(-1, "MinimalOrderPrice", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test4", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text,
                "sort MinimalOrderPrice ASC  value1");
            VerifyAreEqual("test3", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text,
                "sort MinimalOrderPrice DESC  value5");
        }

        [Test]
        public void GeneratedCouponsSortTypeFormatted()
        {
            Driver.GetGridCell(-1, "TypeFormatted", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text,
                "sort TypeFormatted ASC  value1");
            VerifyAreEqual("test4", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text,
                "sort TypeFormatted DESC  value5");

            Driver.GetGridCell(-1, "TypeFormatted", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test3", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text,
                "sort TypeFormatted ASC  value1");
            VerifyAreEqual("test2", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text,
                "sort TypeFormatted DESC  value5");
        }

        [Test]
        public void GeneratedCouponsSortDate()
        {
            Driver.GetGridCell(-1, "ExpirationDateFormatted", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text,
                "sort ExpirationDateFormatted ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text,
                "sort ExpirationDateFormatted DESC  value5");

            Driver.GetGridCell(-1, "ExpirationDateFormatted", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text,
                "sort ExpirationDateFormatted ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text,
                "sort ExpirationDateFormatted DESC  value5");
        }

        [Test]
        public void GeneratedCouponsSortActualUses()
        {
            Driver.GetGridCell(-1, "ActualUses", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text,
                "sort ActualUses ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text,
                "sort ActualUses DESC  value5");

            Driver.GetGridCell(-1, "ActualUses", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text,
                "sort ActualUses ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text,
                "sort ActualUses DESC  value5");
        }

        [Test]
        public void GeneratedCouponsSortAddingDate()
        {
            Driver.GetGridCell(-1, "AddingDateFormatted", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test6", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text,
                "sort AddingDateFormatted ASC  value1");
            VerifyAreEqual("test1", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text,
                "sort AddingDateFormatted DESC  value1");

            Driver.GetGridCell(-1, "AddingDateFormatted", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text,
                "sort AddingDateFormatted ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text,
                "sort AddingDateFormatted DESC  value5");
        }

        [Test]
        public void GeneratedCouponsSortEnabled()
        {
            Driver.GetGridCell(-1, "Enabled", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test5", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "sort Enabled ASC  value1");
            VerifyAreEqual("test4", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "sort Enabled DESC  value5");

            Driver.GetGridCell(-1, "Enabled", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsgenerated").Text, "sort Enabled ASC  value1");
            VerifyAreEqual("test5", Driver.GetGridCell(5, "Code", "Couponsgenerated").Text, "sort Enabled DESC  value5");

            //back default
            Driver.GetGridCell(-1, "Code", "Couponsgenerated").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
        }
    }
}