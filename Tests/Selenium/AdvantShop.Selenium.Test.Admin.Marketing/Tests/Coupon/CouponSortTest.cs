using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Coupon
{
    [TestFixture]
    public class CouponSortTest : BaseSeleniumTest
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
        public void CouponSortCode()
        {
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "grid Code");
            VerifyAreEqual("100", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "grid Value");
            VerifyAreEqual("0", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text,
                "grid MinimalOrderPrice");
            VerifyAreEqual("Фиксированный", Driver.GetGridCell(0, "TypeFormatted", "Couponsdefault").Text,
                "grid TypeFormatted");
            VerifyAreEqual("Бессрочно", Driver.GetGridCell(0, "ExpirationDateFormatted", "Couponsdefault").Text,
                "grid ExpirationDate");
            VerifyAreEqual("0 / -", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text, "grid ActualUses");
            VerifyAreEqual("21.11.2016 11:10", Driver.GetGridCell(0, "AddingDateFormatted", "Couponsdefault").Text,
                "grid AddingDate");

            Driver.GetGridCell(-1, "Code", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "sort Code ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "sort Code DESC  value5");

            Driver.GetGridCell(-1, "Code", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test6", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "sort Code ASC  value1");
            VerifyAreEqual("test1", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "sort Code DESC  value5");
        }

        [Test]
        public void CouponSortValue()
        {
            Driver.GetGridCell(-1, "Value", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test3", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "sort Value ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "sort Value DESC  value5");

            Driver.GetGridCell(-1, "Value", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test2", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "sort Value ASC  value1");
            VerifyAreEqual("test3", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "sort Value DESC  value5");
        }

        [Test]
        public void CouponSortMinimalOrderPrice()
        {
            Driver.GetGridCell(-1, "MinimalOrderPrice", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text,
                "sort MinimalOrderPrice ASC  value1");
            VerifyAreEqual("test4", Driver.GetGridCell(5, "Code", "Couponsdefault").Text,
                "sort MinimalOrderPrice DESC  value5");

            Driver.GetGridCell(-1, "MinimalOrderPrice", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test4", Driver.GetGridCell(0, "Code", "Couponsdefault").Text,
                "sort MinimalOrderPrice ASC  value1");
            VerifyAreEqual("test3", Driver.GetGridCell(5, "Code", "Couponsdefault").Text,
                "sort MinimalOrderPrice DESC  value5");
        }

        [Test]
        public void CouponSortTypeFormatted()
        {
            Driver.GetGridCell(-1, "TypeFormatted", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text,
                "sort TypeFormatted ASC  value1");
            VerifyAreEqual("test4", Driver.GetGridCell(5, "Code", "Couponsdefault").Text,
                "sort TypeFormatted DESC  value5");

            Driver.GetGridCell(-1, "TypeFormatted", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test3", Driver.GetGridCell(0, "Code", "Couponsdefault").Text,
                "sort TypeFormatted ASC  value1");
            VerifyAreEqual("test2", Driver.GetGridCell(5, "Code", "Couponsdefault").Text,
                "sort TypeFormatted DESC  value5");
        }

        [Test]
        public void CouponSortDate()
        {
            Driver.GetGridCell(-1, "ExpirationDateFormatted", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text,
                "sort ExpirationDateFormatted ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text,
                "sort ExpirationDateFormatted DESC  value5");

            Driver.GetGridCell(-1, "ExpirationDateFormatted", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text,
                "sort ExpirationDateFormatted ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text,
                "sort ExpirationDateFormatted DESC  value5");
        }

        [Test]
        public void CouponSortActualUses()
        {
            Driver.GetGridCell(-1, "ActualUses", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text,
                "sort ActualUses ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text,
                "sort ActualUses DESC  value5");

            Driver.GetGridCell(-1, "ActualUses", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text,
                "sort ActualUses ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text,
                "sort ActualUses DESC  value5");
        }

        [Test]
        public void CouponSortAddingDate()
        {
            Driver.GetGridCell(-1, "AddingDateFormatted", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test6", Driver.GetGridCell(0, "Code", "Couponsdefault").Text,
                "sort AddingDateFormatted ASC  value1");
            VerifyAreEqual("test1", Driver.GetGridCell(5, "Code", "Couponsdefault").Text,
                "sort AddingDateFormatted DESC  value1");

            Driver.GetGridCell(-1, "AddingDateFormatted", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text,
                "sort AddingDateFormatted ASC  value1");
            VerifyAreEqual("test6", Driver.GetGridCell(5, "Code", "Couponsdefault").Text,
                "sort AddingDateFormatted DESC  value5");
        }

        [Test]
        public void CouponSortEnabled()
        {
            Driver.GetGridCell(-1, "Enabled", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test5", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "sort Enabled ASC  value1");
            VerifyAreEqual("test4", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "sort Enabled DESC  value5");

            Driver.GetGridCell(-1, "Enabled", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "sort Enabled ASC  value1");
            VerifyAreEqual("test5", Driver.GetGridCell(5, "Code", "Couponsdefault").Text, "sort Enabled DESC  value5");

            //back default
            Driver.GetGridCell(-1, "Code", "Couponsdefault").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
        }
    }
}