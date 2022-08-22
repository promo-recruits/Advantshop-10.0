using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Coupon
{
    [TestFixture]
    public class CouponAdd : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Coupons\\Catalog.Brand.csv",
                "data\\Admin\\Coupons\\Catalog.Product.csv",
                "data\\Admin\\Coupons\\Catalog.Photo.csv",
                "data\\Admin\\Coupons\\Catalog.Color.csv",
                "data\\Admin\\Coupons\\Catalog.Size.csv",
                "data\\Admin\\Coupons\\Catalog.Offer.csv",
                "data\\Admin\\Coupons\\Catalog.Category.csv",
                "data\\Admin\\Coupons\\Catalog.ProductCategories.csv",
                "data\\Admin\\Coupons\\Catalog.Coupon.csv",
                "data\\Admin\\Coupons\\Catalog.CouponCategories.csv",
                "data\\Admin\\Coupons\\Catalog.CouponProducts.csv"
            );
            Init();
            GoToAdmin("settingscheckout");
            if (!Driver.FindElement(By.Id("EnableGiftCertificateService")).Selected)
            {
                Driver.FindElement(AdvBy.DataE2E("EnableCertificate")).Click();
                Driver.WaitForToastSuccess();
            }

            if (!Driver.FindElement(By.Id("DisplayPromoTextbox")).Selected)
            {
                Driver.FindElement(AdvBy.DataE2E("DisplayPromoTextbox")).Click();
                Driver.WaitForToastSuccess();
            }

            try
            {
                Driver.FindElement(AdvBy.DataE2E("SettingsCheckoutSave")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Functions.CheckNotSelected("IsRequiredZip", Driver);
            try
            {
                Driver.FindElement(AdvBy.DataE2E("SettingsCheckoutSave")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }
        }
        DateTime date = DateTime.Now;

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
        [Order(0)]
        public void AddCoupon()
        {
            GoToAdmin("settingscoupons#?couponsTab=coupons");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Купон", Driver.FindElement(By.TagName("h2")).Text, "modal add h2");

            Driver.FindElement(AdvBy.DataE2E("couponCode")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponCode")).SendKeys("NewCoupons");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponType")))).SelectByText(
                "Процентный");
            Driver.FindElement(AdvBy.DataE2E("couponValue")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponValue")).SendKeys("50");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponCurrency"))))
                .SelectByText("Рубли");

            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).SendKeys("1500");

            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForToastSuccess();

            //cheking grid
            Refresh();
            Driver.GridFilterSendKeys("NewCoupons");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyAreEqual("NewCoupons", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "check grid code");
            VerifyAreEqual("50", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "check grid Value");
            VerifyAreEqual("1500", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text,
                "check grid MinimalOrderPrice");
            VerifyAreEqual("Процентный", Driver.GetGridCell(0, "TypeFormatted", "Couponsdefault").Text,
                "check grid TypeFormatted");
            VerifyAreEqual("Бессрочно", Driver.GetGridCell(0, "ExpirationDateFormatted", "Couponsdefault").Text,
                "check grid ExpirationDateFormatted");
            VerifyAreEqual("0 / -", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text,
                "check grid ActualUses");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "check grid Enabled");

            //cheking client
            ProductToCard("1");
            GoToClient("cart");
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client discount");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client result price");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("NewCoupons");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastError();
            VerifyAreEqual("Купон (NewCoupons):",
                Driver.FindElements(By.CssSelector(".cart-full-summary-name"))[1].Text, "client coupon");
            VerifyAreEqual("0 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[0].Text,
                "client 0 sum coupon");
            VerifyAreEqual("1 000 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client rezult");

            Driver.FindElement(By.CssSelector(".spinbox-more")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".price-discount")).Displayed, "client display discount");
            VerifyAreEqual("Купон (NewCoupons):",
                Driver.FindElements(By.CssSelector(".cart-full-summary-name"))[1].Text, "client 2 coupon");
            VerifyAreEqual("1 000 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[0].Text,
                "client sum discount");
            VerifyAreEqual("(50 %)", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[1].Text,
                "client percent discount ");
            VerifyAreEqual("1 000 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client rezult sum");

            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.WaitForElem(By.Id("rightCell"));
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));

            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-result")).Text.Contains("1 000 руб."),
                "checkout rezult");
            Driver.FindElement(AdvBy.DataE2E("btnCheckout")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));

            GoToAdmin("settingscoupons#?couponsTab=coupons");
            Driver.GridFilterSendKeys("NewCoupons");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyAreEqual("NewCoupons", Driver.GetGridCell(0, "Code", "Couponsdefault").Text,
                "grid name coupon after order");
            VerifyAreEqual("1 / -", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text, "ActualUses coupon");
        }

        [Test]
        [Order(2)]
        public void AddProductCoupon()
        {
            Functions.CleanCart(Driver, BaseUrl);
            GoToAdmin("settingscoupons#?couponsTab=coupons");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Купон", Driver.FindElement(By.TagName("h2")).Text, "");

            Driver.FindElement(AdvBy.DataE2E("couponCode")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponCode")).SendKeys("NewCoupons4");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponType")))).SelectByText("Фиксированный");
            Driver.FindElement(AdvBy.DataE2E("couponValue")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponValue")).SendKeys("10000");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponCurrency")))).SelectByText("Рубли");
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).SendKeys("300");
            Driver.FindElement(AdvBy.DataE2E("couponUseExpirationDate")).FindElement(By.TagName("span")).Click();

            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).SendKeys($"{date.AddYears(10):dd.MM.yyyy}");
            Driver.FindElement(AdvBy.DataE2E("couponUsePosibleUses")).FindElement(By.TagName("span")).Click();

            Driver.FindElement(AdvBy.DataE2E("couponPosibleUses")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponPosibleUses")).SendKeys("2");

            //category
            Driver.FindElement(AdvBy.DataE2E("couponCategories")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Выбор категорий')]"));
            Driver.FindElement(By.Id("1")).Click();
            VerifyAreEqual("true",
                Driver.FindElements(By.CssSelector(".category-item-active a"))[0].GetAttribute("aria-selected"),
                "check category true");
            VerifyAreEqual("false",
                Driver.FindElements(By.CssSelector(".category-item-active a"))[1].GetAttribute("aria-selected"),
                "check category false");
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("3 категории(й)", Driver.FindElement(AdvBy.DataE2E("couponCategoriesList")).Text,
                "count select category");
            Driver.FindElement(AdvBy.DataE2E("couponCategories")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Выбор категорий')]"));
            Driver.FindElement(By.Id("3")).Click();
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Thread.Sleep(500);
            VerifyAreEqual("4 категории(й)", Driver.FindElement(AdvBy.DataE2E("couponCategoriesList")).Text, 
                "new count category");
            Driver.FindElement(AdvBy.DataE2E("couponCategoriesReset")).Click();
            VerifyAreEqual("Все", Driver.FindElement(AdvBy.DataE2E("couponCategoriesAll")).Text, "all select category");
            Driver.FindElement(AdvBy.DataE2E("couponCategories")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Выбор категорий')]"));
            Driver.FindElement(By.Id("1")).Click();
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector(".btn-save")).Click();

            //cheking grid
            Refresh();
            Driver.GridFilterSendKeys("NewCoupons4");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyAreEqual("NewCoupons4", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "grid Code");
            VerifyAreEqual("10000", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "grid Value");
            VerifyAreEqual("300", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text,
                "grid MinimalOrderPrice");
            VerifyAreEqual("Фиксированный", Driver.GetGridCell(0, "TypeFormatted", "Couponsdefault").Text,
                "grid TypeFormatted");
            VerifyIsTrue(Driver.GetGridCell(0, "ExpirationDateFormatted", "Couponsdefault").Text.Contains($"{date.AddYears(10):dd.MM.yyyy}"),
                "grid ExpirationDateFormatted");
            VerifyAreEqual("0 / 2", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text, "grid ActualUses");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "grid Enabled");

            //checking client
            ProductToCard("15");
            GoToClient("cart");
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client discount percent");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client discount sum");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("NewCoupons4");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastError();
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "old rezult price");
            Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-remove a")).Click();
            
            ProductToCard("1");
            GoToClient("cart");
            VerifyAreEqual("Купон (NewCoupons4):",
                Driver.FindElements(By.CssSelector(".cart-full-summary-name"))[1].Text, "client coupon name");
            VerifyAreEqual("10 000 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[0].Text,
                "client coupon sum");
            VerifyAreEqual("0 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client cart price");

            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.WaitForElem(By.CssSelector(".breads"));
            Driver.WaitForElem(By.Id("rightCell"));

            Driver.ScrollTo(By.TagName("footer"));
            VerifyAreEqual("0 руб.", Driver.FindElement(By.CssSelector(".checkout-result-price")).Text,
                "checkout price");
            Driver.FindElement(AdvBy.DataE2E("btnCheckout")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));

            GoToAdmin("settingscoupons#?couponsTab=coupons");
            Driver.GridFilterSendKeys("NewCoupons4");
            VerifyAreEqual("NewCoupons4", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "admin name coupon");
            VerifyAreEqual("1 / 2", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text, "admin count used");


            //product
            GoToAdmin("settingscoupons#?couponsTab=coupons");
            Driver.GridFilterSendKeys("NewCoupons4");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            Driver.GetGridCell(0, "_serviceColumn", "Couponsdefault").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();
            Driver.FindElement(AdvBy.DataE2E("couponProducts")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Выбор товара')]"));

            //artNo
            Functions.GridFilterSet(Driver, BaseUrl, "ProductArtNo");
            Driver.SetGridFilterValue("ProductArtNo", "10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product artNo");
            Functions.GridFilterClose(Driver, BaseUrl, "ProductArtNo");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product artNoc lose 1");
            VerifyAreEqual("TestProduct18", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "fiter product artNo close 10");

            //Name
            Functions.GridFilterSet(Driver, BaseUrl, "Name");
            Driver.SetGridFilterValue("Name", "testproduct11");
            VerifyAreEqual("TestProduct11", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product Name 1");
            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product Name close 1");
            VerifyAreEqual("TestProduct18", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "fiter product Name close 10");

            //Brand
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "BrandId", filterItem: "BrandName1");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product BrandId 1");
            VerifyAreEqual("TestProduct11", Driver.GetGridCell(1, "Name", "ProductsSelectvizr").Text,
                "fiter product BrandId 10");
            Functions.GridFilterClose(Driver, BaseUrl, "BrandId");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product BrandId close 1");
            VerifyAreEqual("TestProduct18", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "fiter product BrandId close 10");

            //Price
            Functions.GridFilterSet(Driver, BaseUrl, "Price");
            Driver.SetGridFilterRange("Price", "1200", "2000");
            VerifyAreEqual("TestProduct18", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product Price 1");
            VerifyAreEqual("TestProduct19", Driver.GetGridCell(1, "Name", "ProductsSelectvizr").Text,
                "fiter product Price 10");
            Functions.GridFilterClose(Driver, BaseUrl, "Price");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product Price close 1");
            VerifyAreEqual("TestProduct18", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "fiter product Price close 10");

            //Amount
            Functions.GridFilterSet(Driver, BaseUrl, "Amount");
            Driver.SetGridFilterRange("Amount", "5", "6");

            VerifyAreEqual("TestProduct5", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product Amount 1");
            VerifyAreEqual("TestProduct6", Driver.GetGridCell(1, "Name", "ProductsSelectvizr").Text,
                "fiter product Amount 10");
            Functions.GridFilterClose(Driver, BaseUrl, "Amount");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product Amount close 1");
            VerifyAreEqual("TestProduct18", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "fiter product Amount close 10");

            //Enabled
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enabled", filterItem: "Неактивные");
            VerifyAreEqual("TestProduct2", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product Enabled 1");
            VerifyAreEqual("TestProduct3", Driver.GetGridCell(1, "Name", "ProductsSelectvizr").Text,
                "fiter product Enabled 10");
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Text,
                "fiter product Enabled close 1");
            VerifyAreEqual("TestProduct18", Driver.GetGridCell(9, "Name", "ProductsSelectvizr").Text,
                "fiter product Enabled close 10");

            Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(1, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(2, "Name", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            VerifyAreEqual("3 товар(ов)", Driver.FindElement(AdvBy.DataE2E("couponProductsList")).Text, "count product");
            Driver.FindElement(AdvBy.DataE2E("couponProductsReset")).Click();
            VerifyAreEqual("Все", Driver.FindElement(AdvBy.DataE2E("couponProductsAll")).Text, "all count roduct");
            Driver.FindElement(AdvBy.DataE2E("couponProducts")).Click();
            Driver.WaitForElem(By.XPath("//h2[contains(text(), 'Выбор товара')]"));
            Driver.GetGridCell(0, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(1, "Name", "ProductsSelectvizr").Click();
            Driver.GetGridCell(2, "Name", "ProductsSelectvizr").Click();
            Driver.FindElement(By.CssSelector(".btn-save")).Click();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForToastSuccess();

            //cheking grid
            Refresh();
            Driver.GridFilterSendKeys("NewCoupons4");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyAreEqual("NewCoupons4", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "grid Code");
            VerifyAreEqual("10000", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "grid Value");
            VerifyAreEqual("300", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text,
                "grid MinimalOrderPrice");
            VerifyAreEqual("Фиксированный", Driver.GetGridCell(0, "TypeFormatted", "Couponsdefault").Text,
                "grid TypeFormatted");
            VerifyIsTrue(Driver.GetGridCell(0, "ExpirationDateFormatted", "Couponsdefault").Text.Contains($"{date.AddYears(10):dd.MM.yyyy}"),
                "grid ExpirationDateFormatted");
            VerifyAreEqual("1 / 2", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text, "grid ActualUses");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "grid Enabled");

            //cheking client
            ProductToCard("15");
            GoToClient("cart");
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client discount percent");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client discount sum");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("NewCoupons4");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastError();
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "old rezult price");

            Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-remove a")).Click();
            ProductToCard("1");
            GoToClient("cart");
            VerifyAreEqual("Купон (NewCoupons4):",
                Driver.FindElements(By.CssSelector(".cart-full-summary-name"))[1].Text, "client coupon name");
            VerifyAreEqual("10 000 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[0].Text,
                "client coupon sum");
            VerifyAreEqual("0 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client cart price");

            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.WaitForElem(By.CssSelector(".breads"));
            Driver.WaitForElem(By.Id("rightCell"));

            Driver.ScrollTo(By.TagName("footer"));
            VerifyAreEqual("0 руб.", Driver.FindElement(By.CssSelector(".checkout-result-price")).Text,
                "checkout price");
            Driver.FindElement(AdvBy.DataE2E("btnCheckout")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));

            GoToAdmin("settingscoupons#?couponsTab=coupons");
            Driver.GridFilterSendKeys("NewCoupons4");
            VerifyAreEqual("NewCoupons4", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "admin name coupon");
            VerifyAreEqual("2 / 2", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text, "admin count used");

            ProductToCard("1");
            GoToClient("cart");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("NewCoupons4");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastError();
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "old price without coupon");
            Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-remove a")).Click();
        }

        [Test]
        [Order(1)]
        public void AddDisabledCoupon()
        {
            Functions.CleanCart(Driver, BaseUrl);
            GoToAdmin("settingscoupons#?couponsTab=coupons");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Купон", Driver.FindElement(By.TagName("h2")).Text);

            Driver.FindElement(AdvBy.DataE2E("couponCode")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponCode")).SendKeys("NewCoupons2");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponType")))).SelectByText("Фиксированный");
            Driver.FindElement(AdvBy.DataE2E("couponValue")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponValue")).SendKeys("10000");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponCurrency")))).SelectByText("Рубли");
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).SendKeys("15");

            Driver.FindElement(AdvBy.DataE2E("couponUseExpirationDate")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).SendKeys($"{date.AddYears(10):dd.MM.yyyy}");

            Driver.FindElement(AdvBy.DataE2E("couponUsePosibleUses")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(AdvBy.DataE2E("couponPosibleUses")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponPosibleUses")).SendKeys("5");
            Driver.FindElement(AdvBy.DataE2E("couponEnabled")).Click();

            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForToastSuccess();

            //cheking grid
            Refresh();
            Driver.GridFilterSendKeys("NewCoupons2");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyAreEqual("NewCoupons2", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "grid Code");
            VerifyAreEqual("10000", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "grid Value");
            VerifyAreEqual("15", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text,
                "grid MinimalOrderPrice");
            VerifyAreEqual("Фиксированный", Driver.GetGridCell(0, "TypeFormatted", "Couponsdefault").Text,
                "grid TypeFormatted");
            VerifyIsTrue(Driver.GetGridCell(0, "ExpirationDateFormatted", "Couponsdefault").Text.Contains($"{date.AddYears(10):dd.MM.yyyy}"),
                "grid ExpirationDateFormatted");
            VerifyAreEqual("0 / 5", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text, "grid ActualUses");
            VerifyIsFalse(
                Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                    .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected, "grid Enabled");

            //cheking client
            ProductToCard("1");
            GoToClient("cart");
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client percent discount");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client sum discont");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("NewCoupons2");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastError();
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client old cart price");

            Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-remove a")).Click();
        }

        [Test]
        [Order(3)]
        public void EditCoupon()
        {
            GoToAdmin("settingscoupons#?couponsTab=coupons");

            Driver.GridFilterSendKeys("test1");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            Driver.GetGridCell(0, "_serviceColumn", "Couponsdefault").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Купон", Driver.FindElement(By.TagName("h2")).Text, "modal edit h2");
            
            //cheking modal window
            VerifyAreEqual("test1", Driver.FindElement(AdvBy.DataE2E("couponCode")).GetAttribute("value"),
                "modal couponCode");
            VerifyAreEqual("100", Driver.FindElement(AdvBy.DataE2E("couponValue")).GetAttribute("value"),
                "modal couponValue");
            VerifyAreEqual("0", Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).GetAttribute("value"),
                "modal couponMinimalOrderPrice");

            VerifyAreEqual("true", Driver.FindElement(AdvBy.DataE2E("couponType"))
                .FindElement(By.CssSelector("[label=\"Фиксированный\"]")).GetAttribute("selected"));

            VerifyAreEqual("true", Driver.FindElement(AdvBy.DataE2E("couponCurrency"))
                .FindElement(By.CssSelector("[label=\"Рубли\"]")).GetAttribute("selected"));

            VerifyIsTrue(Driver.FindElement(AdvBy.DataE2E("couponUseExpirationDate"))
                    .FindElement(By.TagName("input")).Selected, "modal UseExpirationDate");
            VerifyIsTrue(Driver.FindElement(AdvBy.DataE2E("couponUsePosibleUses"))
                    .FindElement(By.TagName("input")).Selected, "modal UsePosibleUses");
            VerifyIsTrue(Driver.FindElement(AdvBy.DataE2E("couponEnabled")).FindElement(By.TagName("input"))
                    .Selected, "modal Enabled");

            Driver.FindElement(AdvBy.DataE2E("couponCode")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponCode")).SendKeys("NewCoupons3");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponType")))).SelectByText("Фиксированный");
            Driver.FindElement(AdvBy.DataE2E("couponValue")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponValue")).SendKeys("10000");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponCurrency")))).SelectByText("Евро");
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).SendKeys("15");

            Driver.FindElement(AdvBy.DataE2E("couponUseExpirationDate")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).SendKeys("31.12.2016");
            Driver.FindElement(AdvBy.DataE2E("couponUsePosibleUses")).FindElement(By.TagName("span")).Click();

            Driver.FindElement(AdvBy.DataE2E("couponPosibleUses")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponPosibleUses")).SendKeys("5");
            Driver.FindElement(AdvBy.DataE2E("couponEnabled")).Click();
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForToastSuccess();
            
            //cheking grid
            Refresh();
            Driver.GridFilterSendKeys("NewCoupons3");
            VerifyAreEqual("NewCoupons3", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "grid Code");
            VerifyAreEqual("10000", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "grid Value");
            VerifyAreEqual("15", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text,
                "grid MinimalOrderPrice");
            VerifyAreEqual("Фиксированный", Driver.GetGridCell(0, "TypeFormatted", "Couponsdefault").Text,
                "grid TypeFormatted");
            VerifyAreEqual("31.12.2016 23:59", Driver.GetGridCell(0, "ExpirationDateFormatted", "Couponsdefault").Text,
                "grid ExpirationDateFormatted");
            VerifyAreEqual("0 / 5", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text, "grid ActualUses");
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "grid Enabled");
        }

        [Test]
        [Order(4)]
        public void AddCouponStartDate()
        {
            GoToAdmin("settingscoupons#?couponsTab=coupons");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Купон", Driver.FindElement(By.TagName("h2")).Text, "modal add h2");

            Driver.FindElement(AdvBy.DataE2E("couponCode")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponCode")).SendKeys("NewCoupons5");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponType")))).SelectByText(
                "Процентный");
            Driver.FindElement(AdvBy.DataE2E("couponValue")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponValue")).SendKeys("50");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponCurrency"))))
                .SelectByText("Рубли");

            Driver.FindElement(AdvBy.DataE2E("couponStartDate")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(AdvBy.DataE2E("couponStartDate1")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponStartDate1")).SendKeys($"{date.AddDays(5):dd.MM.yyyy}");
            Driver.DropFocusCss(".control-label");

            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).SendKeys("500");

            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForToastSuccess();
            
            //cheking grid
            Refresh();
            Driver.GridFilterSendKeys("NewCoupons5");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyAreEqual("NewCoupons5", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "check grid code");
            VerifyAreEqual("50", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "check grid Value");
            VerifyAreEqual("500", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text,
                "check grid MinimalOrderPrice");
            VerifyIsTrue(Driver.GetGridCell(0, "StartDateFormatted", "Couponsdefault").Text.Contains($"{date.AddDays(5):dd.MM.yyyy}"),
                "grid ExpirationDateFormatted");
            VerifyAreEqual("Процентный", Driver.GetGridCell(0, "TypeFormatted", "Couponsdefault").Text,
                "check grid TypeFormatted");
            VerifyAreEqual("Бессрочно", Driver.GetGridCell(0, "ExpirationDateFormatted", "Couponsdefault").Text,
                "check grid ExpirationDateFormatted");
            VerifyAreEqual("0 / -", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text,
                "check grid ActualUses");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "check grid Enabled");

            //cheking client
            ProductToCard("1");
            GoToClient("cart");
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client percent discount");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client sum discont");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("NewCoupons5");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastError();
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client old cart price");

            Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-remove a")).Click();

            //создаем новый купон с датой начала действия задним числом
            GoToAdmin("settingscoupons#?couponsTab=coupons");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Купон", Driver.FindElement(By.TagName("h2")).Text, "modal add h2");

            Driver.FindElement(AdvBy.DataE2E("couponCode")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponCode")).SendKeys("NewCoupons6");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponType")))).SelectByText(
                "Процентный");
            Driver.FindElement(AdvBy.DataE2E("couponValue")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponValue")).SendKeys("50");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponCurrency"))))
                .SelectByText("Рубли");

            Driver.FindElement(AdvBy.DataE2E("couponStartDate")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(AdvBy.DataE2E("couponStartDate1")).Click();
            Driver.FindElement(AdvBy.DataE2E("couponStartDate1")).SendKeys($"{date.AddDays(-5):dd.MM.yyyy}");
            Driver.DropFocusCss(".control-label");

            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).SendKeys("500");

            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForToastSuccess();

            //cheking grid
            Refresh();
            Driver.GridFilterSendKeys("NewCoupons6");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyAreEqual("NewCoupons6", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "check grid code");
            VerifyAreEqual("50", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "check grid Value");
            VerifyAreEqual("500", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text,
                "check grid MinimalOrderPrice");
            VerifyIsTrue(Driver.GetGridCell(0, "StartDateFormatted", "Couponsdefault").Text.Contains($"{date.AddDays(-5):dd.MM.yyyy}"),
                "grid ExpirationDateFormatted");
            VerifyAreEqual("Процентный", Driver.GetGridCell(0, "TypeFormatted", "Couponsdefault").Text,
                "check grid TypeFormatted");
            VerifyAreEqual("Бессрочно", Driver.GetGridCell(0, "ExpirationDateFormatted", "Couponsdefault").Text,
                "check grid ExpirationDateFormatted");
            VerifyAreEqual("0 / -", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text,
                "check grid ActualUses");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "check grid Enabled");

            //cheking client
            ProductToCard("15");
            GoToClient("cart");
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client discount percent");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client discount sum");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("NewCoupons6");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            VerifyAreEqual("Купон (NewCoupons6):",
                Driver.FindElements(By.CssSelector(".cart-full-summary-name"))[1].Text, "client coupon");
            VerifyAreEqual("1 000 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price"))[0].Text,
                "client sum");
            VerifyAreEqual("500 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[0].Text,
                "client sum discount");
            VerifyAreEqual("(50 %)", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[1].Text,
                "client percent discount ");
            VerifyAreEqual("500 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client rezult sum");

            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.WaitForElem(By.Id("rightCell"));
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));

            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-result")).Text.Contains("500 руб."),
                "checkout rezult");

            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-remove a")).Click();
        }

        [Test]
        [Order(5)]
        public void AddCouponEndDate()
        {
            GoToAdmin("settingscoupons#?couponsTab=coupons");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Купон", Driver.FindElement(By.TagName("h2")).Text, "modal add h2");

            Driver.FindElement(AdvBy.DataE2E("couponCode")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponCode")).SendKeys("NewCoupons7");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponType")))).SelectByText(
                "Процентный");
            Driver.FindElement(AdvBy.DataE2E("couponValue")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponValue")).SendKeys("50");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponCurrency"))))
                .SelectByText("Рубли");

            Driver.FindElement(AdvBy.DataE2E("couponUseExpirationDate")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).SendKeys($"{date.AddDays(5):dd.MM.yyyy}");
            Driver.DropFocusCss(".control-label");

            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).SendKeys("500");

            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForToastSuccess();

            //cheking grid
            Refresh();
            Driver.GridFilterSendKeys("NewCoupons7");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyAreEqual("NewCoupons7", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "check grid code");
            VerifyAreEqual("50", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "check grid Value");
            VerifyAreEqual("500", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text,
                "check grid MinimalOrderPrice");
            VerifyAreEqual("С даты создания", Driver.GetGridCell(0, "StartDateFormatted", "Couponsdefault").Text,
                "grid ExpirationDateFormatted");
            VerifyAreEqual("Процентный", Driver.GetGridCell(0, "TypeFormatted", "Couponsdefault").Text,
                "check grid TypeFormatted");
            VerifyIsTrue(Driver.GetGridCell(0, "ExpirationDateFormatted", "Couponsdefault").Text.Contains($"{date.AddDays(5):dd.MM.yyyy}"),
                "check grid ExpirationDateFormatted");
            VerifyAreEqual("0 / -", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text,
                "check grid ActualUses");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "check grid Enabled");

            //cheking client
            ProductToCard("5");
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".icon-cancel-before")).Click();
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client discount percent");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client discount sum");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("NewCoupons7");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            VerifyAreEqual("Купон (NewCoupons7):",
                Driver.FindElements(By.CssSelector(".cart-full-summary-name"))[1].Text, "client coupon");
            VerifyAreEqual("1 000 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price"))[0].Text,
                "client sum");
            VerifyAreEqual("500 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[0].Text,
                "client sum discount");
            VerifyAreEqual("(50 %)", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[1].Text,
                "client percent discount ");
            VerifyAreEqual("500 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client rezult sum");

            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.WaitForElem(By.Id("rightCell"));
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));

            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-result")).Text.Contains("500 руб."),
                "checkout rezult");

            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-remove a")).Click();

            //создаем просроченный купон
            GoToAdmin("settingscoupons#?couponsTab=coupons");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Купон", Driver.FindElement(By.TagName("h2")).Text, "modal add h2");

            Driver.FindElement(AdvBy.DataE2E("couponCode")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponCode")).SendKeys("NewCoupons8");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponType")))).SelectByText(
                "Процентный");
            Driver.FindElement(AdvBy.DataE2E("couponValue")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponValue")).SendKeys("50");
            (new SelectElement(Driver.FindElement(AdvBy.DataE2E("couponCurrency"))))
                .SelectByText("Рубли");

            Driver.FindElement(AdvBy.DataE2E("couponUseExpirationDate")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponExpirationDate")).SendKeys($"{date.AddDays(-5):dd.MM.yyyy}");
            Driver.DropFocusCss(".control-label");

            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).Clear();
            Driver.FindElement(AdvBy.DataE2E("couponMinimalOrderPrice")).SendKeys("500");

            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForToastSuccess();

            //cheking grid
            Refresh();
            Driver.GridFilterSendKeys("NewCoupons8");
            Driver.DropFocusCss("[data-e2e=\"CouponsTitle\"]");
            VerifyAreEqual("NewCoupons8", Driver.GetGridCell(0, "Code", "Couponsdefault").Text, "check grid code");
            VerifyAreEqual("50", Driver.GetGridCell(0, "Value", "Couponsdefault").Text, "check grid Value");
            VerifyAreEqual("500", Driver.GetGridCell(0, "MinimalOrderPrice", "Couponsdefault").Text,
                "check grid MinimalOrderPrice");
            VerifyAreEqual("С даты создания", Driver.GetGridCell(0, "StartDateFormatted", "Couponsdefault").Text,
                "grid ExpirationDateFormatted");
            VerifyAreEqual("Процентный", Driver.GetGridCell(0, "TypeFormatted", "Couponsdefault").Text,
                "check grid TypeFormatted");
            VerifyIsTrue(Driver.GetGridCell(0, "ExpirationDateFormatted", "Couponsdefault").Text.Contains($"{date.AddDays(-5):dd.MM.yyyy}"),
                "check grid ExpirationDateFormatted");
            VerifyAreEqual("0 / -", Driver.GetGridCell(0, "ActualUses", "Couponsdefault").Text,
                "check grid ActualUses");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled", "Couponsdefault")
                    .FindElement(AdvBy.DataE2E("switchOnOffInput")).Selected, "check grid Enabled");

            //cheking client
            ProductToCard("15");
            GoToClient("cart");
            Driver.FindElement(By.CssSelector(".icon-cancel-before")).Click();
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client percent discount");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client sum discont");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("NewCoupons8");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastError();
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client old cart price");

            Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-remove a")).Click();
        }

        public void ProductToCard(string id)
        {
            GoToClient("products/test-product" + id);
            Driver.ScrollTo(By.CssSelector(".rating"));
            Driver.FindElement(By.CssSelector(".details-payment-inline a")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));
        }
    }
}