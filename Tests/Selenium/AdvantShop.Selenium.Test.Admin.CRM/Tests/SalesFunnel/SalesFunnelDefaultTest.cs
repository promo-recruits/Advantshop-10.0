using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.SalesFunnel
{
    [TestFixture]
    public class CRMSalesFunnelDefaultTest : BaseSeleniumTest
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

            GoToAdmin("settingscrm");

            IWebElement selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Sales Funnel 2"), "pre check funnel default");

            (new SelectElement(Driver.FindElement(By.Name("DefaultSalesFunnelId")))).SelectByText("Sales Funnel 4");
            Driver.WaitForToastSuccess();

            Functions.EnableCapcha(Driver, BaseUrl);
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
        [Order(0)]
        public void SalesFunnelSetDefault()
        {
            GoToAdmin("settingscrm");

            IWebElement selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Sales Funnel 4"), "check funnel default");
        }

        [Test]
        [Order(2)]
        public void SalesFunnelCheckClientDefault()
        {
            //create lead
            GoToClient("feedback");

            Driver.FindElement(By.Name("Message")).Click();
            Driver.FindElement(By.Name("Message")).Clear();
            Driver.FindElement(By.Name("Message")).SendKeys("Feed Back Lead");

            Driver.ScrollTo(By.Id("OrderNumber"));
            Driver.FindElement(By.Name("Name")).Click();
            Driver.FindElement(By.Name("Name")).Clear();
            Driver.FindElement(By.Name("Name")).SendKeys("Customer Name");

            Driver.ScrollTo(By.Id("Message"));
            Driver.FindElement(By.Name("Email")).Click();
            Driver.FindElement(By.Name("Email")).Clear();
            Driver.FindElement(By.Name("Email")).SendKeys("test@test.test");

            Driver.FindElement(By.Name("Phone")).Click();
            Driver.ClearInput(By.Name("Phone"));
            Driver.FindElement(By.Name("Phone")).SendKeys("79279272727");

            Driver.ScrollTo(By.Id("Name"));
            Driver.FindElement(By.CssSelector(".btn.btn-submit.btn-middle")).Click();
            Driver.WaitForElem(By.ClassName("feedbackSuccess-block"));

            //check admin default funnel
            GoToAdmin("leads?salesFunnelId=4");

            Driver.GridFilterSendKeys("Customer Name");

            VerifyAreEqual("Customer Name", Driver.GetGridCell(0, "FullName").Text, "lead grid default funnel");

            //check admin no default funnel
            GoToAdmin("leads?salesFunnelId=2");

            Driver.GridFilterSendKeys("Customer Name");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead grid in no default funnel");
        }


        [Test]
        [Order(1)]
        public void SalesFunnelCheckAdminDefault()
        {
            //create lead
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"LeadSalesFunnel\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Sales Funnel 4"), "check funnel default");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).SendKeys("Lead from Admin Check Default");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71237712143");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).SendKeys("mailtqweest123@mail.ru");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(By.Id("Lead_SalesFunnelId"));

            //check admin default funnel
            GoToAdmin("leads?salesFunnelId=4");

            Driver.GridFilterSendKeys("Lead from Admin Check Default Name");

            VerifyAreEqual("Lead from Admin Check Default Name", Driver.GetGridCell(0, "FullName").Text,
                "lead grid default funnel");

            //check admin no default funnel
            GoToAdmin("leads?salesFunnelId=2");

            Driver.GridFilterSendKeys("Lead from Admin Check Default Name");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "lead grid in no default funnel");
        }
    }
}