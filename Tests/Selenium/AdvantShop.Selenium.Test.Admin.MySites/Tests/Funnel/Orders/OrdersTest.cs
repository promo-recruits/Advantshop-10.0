using System.Threading;
using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.MySites.Tests.Funnel.Orders
{
    [TestFixture]
    [Author("irina.ba@advantshop.net")]
    public class OrdersTest : MySitesFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\Funnel\\Orders\\CMS.Landing.csv",
                "data\\Admin\\Funnel\\Orders\\CMS.LandingSettings.csv",
                "data\\Admin\\Funnel\\Orders\\CMS.LandingSite.csv",
                "data\\Admin\\Funnel\\Orders\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\Funnel\\Orders\\CMS.LandingBlock.csv",
                "data\\Admin\\Funnel\\Orders\\CMS.LandingForm.csv",
                "data\\Admin\\Funnel\\Orders\\CMS.LandingSubBlock.csv"
            );

            Init(false);
            GoToAdmin("dashboard");
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
            ReInit();
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void AddOrderFromForm()
        {
            GoToAdmin("funnels/site/1");
            GoToFunnelTab(1, "Заказы", By.TagName("ui-grid-custom"));
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.ClassName("ui-grid-empty-text")).Text.Trim(), "default orders");

            //рубашка
            ReInitClient();
            GoToClient("lp/orderfromformfunnel");
            FillFunnelForm();
            Driver.WaitForElem(By.ClassName("checkout-title"));
            FillCheckoutForm();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));

            ReInit();
            GoToFunnelTab(1, "Заказы", By.TagName("ui-grid-custom"));
            VerifyAreEqual("FirstName TestLastName", Driver.GetGridCellText(0, "BuyerName", "FunnelOrders"),
                "order name");
            Driver.GetGridCell(0, "Number", "FunnelOrders").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("order-header-item"));
            VerifyAreEqual("TestLastName FirstName", Driver.FindElement(By.ClassName("customer-fullname")).Text.Trim(),
                "order name");
            VerifyAreEqual("Воронка продаж \"OrderFromFormFunnel\"", GetSelectedOptionText("Order_OrderSourceId"),
                "order sourse");
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector("[grid-unique-id=\"gridOrderItems\"] [data-e2e=\"gridRow\"]")).Count,
                "products in order");
            VerifyAreEqual("Рубашка", Driver.GetGridCell(0, "Name", "OrderItems").FindElement(By.TagName("a")).Text,
                "product name");
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void AddOrderFromButton()
        {
            GoToFunnelTab(2, "Заказы", By.TagName("ui-grid-custom"));
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.ClassName("ui-grid-empty-text")).Text.Trim(), "default orders");

            //часы
            ReInitClient();
            GoToClient("lp/orderfrombuttonfunnel");
            Driver.FindElement(AdvBy.DataE2E("ButtonForm")).Click();
            Driver.WaitForElem(By.ClassName("checkout-title"));
            FillCheckoutForm();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));

            ReInit();
            GoToFunnelTab(2, "Заказы", By.TagName("ui-grid-custom"));
            VerifyAreEqual("FirstName TestLastName", Driver.GetGridCellText(0, "BuyerName", "FunnelOrders"),
                "order name");
            Driver.GetGridCell(0, "Number", "FunnelOrders").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("order-header-item"));
            VerifyAreEqual("TestLastName FirstName", Driver.FindElement(By.ClassName("customer-fullname")).Text.Trim(),
                "order name");
            VerifyAreEqual("Воронка продаж \"OrderFromButtonFunnel\"", GetSelectedOptionText("Order_OrderSourceId"),
                "order sourse");
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector("[grid-unique-id=\"gridOrderItems\"] [data-e2e=\"gridRow\"]")).Count,
                "products in order");
            VerifyAreEqual("Часы", Driver.GetGridCell(0, "Name", "OrderItems").FindElement(By.TagName("a")).Text,
                "product name");
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void AddOrderFromProductBlock()
        {
            GoToFunnelTab(3, "Заказы", By.TagName("ui-grid-custom"));
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.ClassName("ui-grid-empty-text")).Text.Trim(), "default orders");

            //платье
            ReInitClient();
            GoToClient("lp/orderfromproductblockfunnel");
            Driver.FindElement(By.CssSelector(".lp-btn[data-modal-id=\"modalProductViewBlock_3_1649\"]")).Click();
            Driver.WaitForElem(By.ClassName("adv-modal-active"));
            Driver.FindElement(By.CssSelector(".lp-product-info__payment-item a")).Click();
            Driver.WaitForElem(By.ClassName("checkout-title"));
            FillCheckoutForm();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));

            ReInit();
            GoToFunnelTab(3, "Заказы", By.TagName("ui-grid-custom"));
            VerifyAreEqual("FirstName TestLastName", Driver.GetGridCellText(0, "BuyerName", "FunnelOrders"),
                "order name");
            Driver.GetGridCell(0, "Number", "FunnelOrders").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("order-header-item"));
            VerifyAreEqual("TestLastName FirstName", Driver.FindElement(By.ClassName("customer-fullname")).Text.Trim(),
                "order name");
            VerifyAreEqual("Воронка продаж \"OrderFromProductBlockFunnel\"",
                GetSelectedOptionText("Order_OrderSourceId"), "order sourse");
            VerifyAreEqual(1,
                Driver.FindElements(By.CssSelector("[grid-unique-id=\"gridOrderItems\"] [data-e2e=\"gridRow\"]")).Count,
                "products in order");
            VerifyAreEqual("Платье", Driver.GetGridCell(0, "Name", "OrderItems").FindElement(By.TagName("a")).Text,
                "product name");
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void AddOrderFromMultiProductBlock()
        {
            GoToFunnelTab(4, "Заказы", By.TagName("ui-grid-custom"));
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.ClassName("ui-grid-empty-text")).Text.Trim(), "default orders");

            //туфли, шорты
            ReInitClient();
            GoToClient("lp/orderfrommultiproductblockfunnel");
            Thread.Sleep(100);
            Driver.ScrollTo(By.ClassName("product-view-landing-alt"), 1);
            Driver.WaitForElemDisplayedAndClick(By.CssSelector(".lp-product-info__payment-item .lp-btn--primary"), 1);
            Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
            Driver.FindElement(By.ClassName("adv-modal-close")).Click();
            Thread.Sleep(500);
            Driver.FindElements(By.CssSelector(".lp-product-info__payment-item .lp-btn--primary"))[2].Click();
            Driver.WaitForElem(By.CssSelector(".adv-modal:not(.ng-hide)"));
            Driver.FindElement(By.CssSelector(".lp-cart-btn-confirm .lp-btn--primary")).Click();
            Driver.WaitForElem(By.ClassName("checkout-title"));
            FillCheckoutForm();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));

            ReInit();
            GoToFunnelTab(4, "Заказы", By.TagName("ui-grid-custom"));
            VerifyAreEqual("FirstName TestLastName", Driver.GetGridCellText(0, "BuyerName", "FunnelOrders"),
                "order name");
            Driver.GetGridCell(0, "Number", "FunnelOrders").FindElement(By.TagName("a")).Click();
            Thread.Sleep(500);

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(By.ClassName("order-header-item"));
            VerifyAreEqual("TestLastName FirstName", Driver.FindElement(By.ClassName("customer-fullname")).Text.Trim(),
                "order name");
            VerifyAreEqual("Воронка продаж \"OrderFromMultiProductBlockFunnel\"",
                GetSelectedOptionText("Order_OrderSourceId"), "order sourse");
            VerifyAreEqual(2,
                Driver.FindElements(By.CssSelector("[grid-unique-id=\"gridOrderItems\"] [data-e2e=\"gridRow\"]")).Count,
                "products in order");
            VerifyAreEqual("Туфли", Driver.GetGridCell(0, "Name", "OrderItems").FindElement(By.TagName("a")).Text,
                "product name1");
            VerifyAreEqual("Шорты", Driver.GetGridCell(1, "Name", "OrderItems").FindElement(By.TagName("a")).Text,
                "product name2");
            Functions.CloseTab(Driver, BaseUrl);
        }
    }
}