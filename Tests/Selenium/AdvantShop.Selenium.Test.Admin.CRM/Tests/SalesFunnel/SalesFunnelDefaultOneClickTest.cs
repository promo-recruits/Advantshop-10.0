using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.SalesFunnel
{
    [TestFixture]
    public class CRMSalesFunnelDefaultOneClickTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\SalesFunnel\\Catalog.Product.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Catalog.Category.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.Customer.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.Departments.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.Managers.csv",
                "data\\Admin\\CRM\\SalesFunnel\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\SalesFunnel\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\SalesFunnel\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\SalesFunnel\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\SalesFunnel\\[Order].Lead.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Customers.Task.csv",
                "data\\Admin\\CRM\\SalesFunnel\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\SalesFunnel\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\SalesFunnel\\[Order].LeadItem.csv",
                "data\\Admin\\CRM\\SalesFunnel\\Settings.Settings.csv"
            );

            Init();
            EnableInplaceOff();

            GoToAdmin("settingscheckout#?checkoutTab=common");

            Driver.ScrollTo(By.Name("BuyInOneClickFirstText"));

            (new SelectElement(Driver.FindElement(By.Name("BuyInOneClickAction")))).SelectByText("Создавать лид");
            (new SelectElement(Driver.FindElement(By.Name("BuyInOneClickSalesFunnelId")))).SelectByText(
                "Sales Funnel 3");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
            Driver.WaitForToastSuccess();
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
        public void SalesFunnelSetOneClickDefault()
        {
            GoToAdmin("settingscheckout#?checkoutTab=common");

            IWebElement selectElem = Driver.FindElement(By.Name("BuyInOneClickSalesFunnelId"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Sales Funnel 3"), "check funnel default");
        }

        [Test]
        public void SalesFunnelCheckClientOneClickDefault()
        {
            //create lead
            GoToClient("products/test-product100");

            Driver.ScrollTo(By.CssSelector(".btn.btn-middle.btn-confirm.icon-bag-before"));
            Driver.FindElement(By.LinkText("Купить в один клик")).Click();
            Driver.WaitForElem(By.ClassName("buy-one-click-dialog"));
            Driver.FindElement(By.CssSelector("[value=\"Заказать\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-content"));

            //check admin default funnel
            GoToAdmin("leads?salesFunnelId=3");

            Driver.GetGridFilterTab(0, "Ad Admin");

            VerifyAreEqual("Ad Admin", Driver.GetGridCell(0, "FullName").Text, "lead grid default funnel");

            //check admin no default funnel
            GoToAdmin("leads?salesFunnelId=2");

            Driver.GetGridFilterTab(0, "Ad Admin");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead grid in no default 1 funnel");

            GoToAdmin("leads?salesFunnelId=4");

            Driver.GetGridFilterTab(0, "Ad Admin");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead grid in no default 2 funnel");
        }
    }
}