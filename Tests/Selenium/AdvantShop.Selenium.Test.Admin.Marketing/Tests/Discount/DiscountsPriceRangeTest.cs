using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Discount
{
    [TestFixture]
    public class DiscountsPriceRangeTest : BaseSeleniumTest
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
        [Order(1)]
        public void Grid()
        {
            VerifyAreEqual("Найдено записей: 170",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "grid count all");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "grid discount price range");
            VerifyAreEqual("1", Driver.GetGridCell(0, "PercentDiscount", "PriceRange").Text, "grid percent discount");
        }


        [Test]
        [Order(0)]
        public void GoToEdit()
        {
            Driver.GetGridCell(0, "_serviceColumn", "PriceRange")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Скидка из стоимости заказа", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.XPathContainsText("button", "Отмена");
        }


        [Test]
        [Order(3)]
        public void InplacePriceRange()
        {
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"DiscountsTitle\"]"));

            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text,
                "pre check inplace price range");

            Driver.SendKeysGridCell("5", 0, "PriceRange", "PriceRange");
            VerifyAreEqual("5", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "inplace edit price range");

            Refresh();
            VerifyAreEqual("5", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text,
                "inplace edit price range after refresh");

            //back default
            Driver.SendKeysGridCell("11", 0, "PriceRange", "PriceRange");
            VerifyAreEqual("11", Driver.GetGridCell(0, "PriceRange", "PriceRange").Text, "back default");
        }

        [Test]
        [Order(2)]
        public void InplacePercentDiscount()
        {
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"DiscountsTitle\"]"));

            VerifyAreEqual("2", Driver.GetGridCell(1, "PercentDiscount", "PriceRange").Text,
                "pre check inplace percent discount");

            Driver.SendKeysGridCell("1", 1, "PercentDiscount", "PriceRange");

            VerifyAreEqual("1", Driver.GetGridCell(1, "PercentDiscount", "PriceRange").Text,
                "inplace edit percent discount");

            Refresh();
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"DiscountsTitle\"]"));
            VerifyAreEqual("1", Driver.GetGridCell(1, "PercentDiscount", "PriceRange").Text,
                "inplace edit percent discount after refresh");

            //back default
            Driver.SendKeysGridCell("2", 1, "PercentDiscount", "PriceRange");
            VerifyAreEqual("2", Driver.GetGridCell(1, "PercentDiscount", "PriceRange").Text, "back default");
            VerifyAreEqual("12", Driver.GetGridCell(1, "PriceRange", "PriceRange").Text, "back default price range");
        }
    }
}