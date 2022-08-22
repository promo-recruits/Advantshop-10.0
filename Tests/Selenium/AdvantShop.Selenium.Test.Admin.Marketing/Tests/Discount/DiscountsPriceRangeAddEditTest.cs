using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Discount
{
    [TestFixture]
    public class DiscountsPriceRangeAddEditTest : BaseSeleniumTest
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
        public void DiscountsPriceRangeAdd()
        {
            GoToAdmin("settingscoupons#?couponsTab=discounts");

            Driver.FindElement(By.CssSelector("[data-e2e=\"DiscountsAdd\"]")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Скидка из стоимости заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"priceRange\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"priceRange\"]")).SendKeys("5000");
            Driver.FindElement(By.CssSelector("[data-e2e=\"percentDiscount\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"percentDiscount\"]")).SendKeys("97");

            Driver.FindElement(By.CssSelector("[data-e2e=\"bntSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Refresh();
            VerifyAreEqual("Найдено записей: 171",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            Driver.GridFilterSendKeys("5000");
            VerifyAreEqual("5000", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text,
                "discount price range added");
            VerifyAreEqual("97", Driver.GetGridCell(0, "PercentDiscount", "PriceRange").Text,
                "discount percent discount added");
        }

        [Test]
        public void DiscountsPriceRangeEdit()
        {
            GoToAdmin("settingscoupons#?couponsTab=discounts");

            //pre check admin
            Driver.GridFilterSendKeys("100");
            Driver.DropFocusCss("[data-e2e=\"DiscountsTitle\"]");
            VerifyAreEqual("100", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text,
                "pre check discount price range");
            VerifyAreEqual("90", Driver.GetGridCell(0, "PercentDiscount", "PriceRange").Text,
                "pre check percent discount");

            Driver.GetGridCell(0, "_serviceColumn", "PriceRange")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            //pre check pop up
            VerifyAreEqual("100", Driver.FindElement(By.CssSelector("[data-e2e=\"priceRange\"]")).GetAttribute("value"),
                "pre check pop up discount price range");
            VerifyAreEqual("90",
                Driver.FindElement(By.CssSelector("[data-e2e=\"percentDiscount\"]")).GetAttribute("value"),
                "pre check pop up discount price range");

            Driver.FindElement(By.CssSelector("[data-e2e=\"priceRange\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"priceRange\"]")).SendKeys("20000");
            Driver.FindElement(By.CssSelector("[data-e2e=\"percentDiscount\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"percentDiscount\"]")).SendKeys("4");

            Driver.FindElement(By.CssSelector("[data-e2e=\"bntSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Refresh();

            Driver.GridFilterSendKeys("100");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "edited prev price range");

            //check admin
            Driver.GridFilterSendKeys("20000");
            VerifyAreEqual("20000", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text,
                "discount price range edited");
            VerifyAreEqual("4", Driver.GetGridCell(0, "PercentDiscount", "PriceRange").Text,
                "discount percent discount edited");

            //check pop up
            Driver.GetGridCell(0, "_serviceColumn", "PriceRange")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("20000",
                Driver.FindElement(By.CssSelector("[data-e2e=\"priceRange\"]")).GetAttribute("value"),
                "pop up discount price range edited");
            VerifyAreEqual("4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"percentDiscount\"]")).GetAttribute("value"),
                "pop up discount price range edited");
        }
    }
}