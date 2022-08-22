using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Discount
{
    [TestFixture]
    public class DiscountsPriceRangeClientTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\Discount\\DiscountClient\\Catalog.Product.csv",
                "data\\Admin\\Discount\\DiscountClient\\Catalog.Offer.csv",
                "data\\Admin\\Discount\\DiscountClient\\Catalog.Category.csv",
                "data\\Admin\\Discount\\DiscountClient\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Discount\\DiscountClient\\[Order].OrderSource.csv",
                "data\\Admin\\Discount\\DiscountClient\\[Order].OrderStatus.csv",
                "data\\Admin\\Discount\\DiscountClient\\[Order].OrderPriceDiscount.csv"
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
        [Order(1)]
        public void DiscountApplied()
        {
            GoToAdmin("settingscoupons#?couponsTab=discounts");

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"enableDiscounts\"]")).FindElement(By.TagName("input"))
                .Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"enableDiscounts\"]"))
                    .FindElement(By.CssSelector(".adv-checkbox-emul")).Click();
                Driver.WaitForToastSuccess();
            }

            Functions.CleanCart(Driver, BaseUrl);

            GoToClient("products/test-product102");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-payment-cell.details-payment-price")).Text
                    .Contains("1 102 руб."), "product offer");

            Driver.ScrollTo(By.CssSelector(".rating"));
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(500);

            GoToClient("cart");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".cart-full-result-block")).Text.Contains("-551 руб. (50%)"),
                "offer with discount in cart");
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();

            Driver.WaitForElem(By.CssSelector(".breads"));
            Driver.WaitForElem(By.Id("rightCell"));

            VerifyIsTrue(Driver.FindElement(By.Id("rightCell")).Text.Contains("-50%"),
                "offer PercentDiscount in checkout");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-result-price.cs-t-4")).Text.Contains("551 руб."),
                "offer with discount in checkout");
        }

        [Test]
        [Order(1)]
        public void DiscountNotApplied()
        {
            GoToAdmin("settingscoupons#?couponsTab=discounts");

            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"enableDiscounts\"]")).FindElement(By.TagName("input"))
                .Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"enableDiscounts\"]"))
                    .FindElement(By.CssSelector(".adv-checkbox-emul")).Click();
                Driver.WaitForToastSuccess();
            }

            Functions.CleanCart(Driver, BaseUrl);

            GoToClient("products/test-product61");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-payment-cell.details-payment-price")).Text
                    .Contains("1 061 руб."), "product offer");

            Driver.ScrollTo(By.CssSelector(".rating"));
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(500);

            GoToClient("cart");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".cart-full-result-block")).Text.Contains("(50%)"),
                "no discount in cart");
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();

            Driver.WaitForElem(By.CssSelector(".breads"));
            Driver.WaitForElem(By.Id("rightCell"));

            VerifyIsFalse(Driver.FindElement(By.Id("rightCell")).Text.Contains("-50%"),
                "offer PercentDiscount in checkout no");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-result-price.cs-t-4")).Text.Contains("1 061 руб."),
                "no offer in checkout");
        }

        [Test]
        [Order(2)]
        public void DiscountsDisabled()
        {
            Functions.CleanCart(Driver, BaseUrl);

            GoToAdmin("settingscoupons#?couponsTab=discounts");

            if (Driver.FindElement(By.CssSelector("[data-e2e=\"enableDiscounts\"]")).FindElement(By.TagName("input"))
                .Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"enableDiscounts\"]"))
                    .FindElement(By.CssSelector(".adv-checkbox-emul")).Click();
                Driver.WaitForToastSuccess();
            }

            //check admin
            Refresh();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"enableDiscounts\"]")).FindElement(By.TagName("input"))
                    .Selected, "discounts disabled");

            //check client
            GoToClient("products/test-product102");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".details-payment-cell.details-payment-price")).Text
                    .Contains("1 102 руб."), "product offer");

            Driver.ScrollTo(By.CssSelector(".rating"));
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before")).Click();
            Thread.Sleep(500);

            GoToClient("cart");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".cart-full-result-block")).Text.Contains("(50%)"),
                "cart discounts disabled");
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();

            Driver.WaitForElem(By.CssSelector(".breads"));
            Driver.WaitForElem(By.Id("rightCell"));

            VerifyIsFalse(Driver.FindElement(By.Id("rightCell")).Text.Contains("-50%"),
                "offer PercentDiscount in checkout no");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".checkout-cart-result-price.cs-t-4")).Text.Contains("1 102 руб."),
                "checkout discounts disabled");
        }
    }
}