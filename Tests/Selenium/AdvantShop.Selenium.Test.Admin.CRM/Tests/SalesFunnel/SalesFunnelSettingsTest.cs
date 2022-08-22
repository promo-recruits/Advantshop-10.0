using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.SalesFunnel
{
    [TestFixture]
    public class CRMSalesFunnelSuccessActionTest : BaseSeleniumTest
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
        public void FinalSuccessActionOrder()
        {
            //pre check orders count
            GoToAdmin("orders");

            int ordersCountBegin = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;

            //check FinalSuccessAction
            GoToAdmin("settingscrm#?crmTab=salesfunnels");
            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();
            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelFinalSuccessAction\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Создать заказ"), "check funnel success action");

            //create order from lead
            GoToAdmin("leads?salesFunnelId=1#?leadIdInfo=117");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadCreateOrder\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"completeLead\"]")).Click();

            Driver.WaitForElem(By.Id("Order_OrderSourceId"));
            VerifyIsTrue(Driver.Url.Contains("orders"), "url order created");
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Заказ №"), "url order created");

            GoToAdmin("leads?salesFunnelId=1");
            VerifyAreEqual("117", Driver.GetGridCell(0, "Id").Text, "lead id");
            VerifyAreEqual("Сделка заключена", Driver.GetGridCell(0, "DealStatusName").Text, "lead deal status");

            GoToAdmin("orders");
            int ordersCountEnd = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;
            VerifyIsFalse(ordersCountBegin.Equals(ordersCountEnd), "no orders added");
        }

        [Test]
        public void FinalSuccessActionNothing()
        {
            //pre check orders count
            GoToAdmin("orders");

            int ordersCountBegin = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;

            //check FinalSuccessAction
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelFinalSuccessAction\"]"));
            SelectElement select = new SelectElement(selectElem);

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelFinalSuccessAction\"]"))))
                .SelectByText("Ничего не делать");
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ничего не делать"), "check funnel success action");
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();


            //create order from lead
            GoToAdmin("leads?salesFunnelId=2#?leadIdInfo=119");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadCreateOrder\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"completeLead\"]")).Click();
            Thread.Sleep(500);

            VerifyIsFalse(Driver.Url.Contains("orders"), "url order created");

            GoToAdmin("leads?salesFunnelId=2");

            Driver.GridFilterSendKeys("119");

            VerifyAreEqual("119", Driver.GetGridCell(0, "Id").Text, "lead id");
            VerifyAreEqual("Сделка заключена", Driver.GetGridCell(0, "DealStatusName").Text, "lead deal status");

            GoToAdmin("orders");
            int ordersCountEnd = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;
            VerifyIsTrue(ordersCountBegin.Equals(ordersCountEnd), "no orders added");
        }

        [Test]
        public void FinalSuccessActionEdit()
        {
            //pre check orders count
            GoToAdmin("orders");

            int ordersCountBegin = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;

            //check FinalSuccessAction
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelFinalSuccessAction\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Создать заказ"), "check funnel success action");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelFinalSuccessAction\"]"))))
                .SelectByText("Ничего не делать");
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();

            //check admin
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelFinalSuccessAction\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Ничего не делать"), "check funnel success action edited");

            //create order from lead
            GoToAdmin("leads?salesFunnelId=2#?leadIdInfo=120");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadCreateOrder\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"completeLead\"]")).Click();
            Thread.Sleep(500);

            VerifyIsFalse(Driver.Url.Contains("orders"), "url order created");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("120", Driver.GetGridCell(0, "Id").Text, "lead id");
            VerifyAreEqual("Сделка заключена", Driver.GetGridCell(0, "DealStatusName").Text, "lead deal status");

            GoToAdmin("orders");
            int ordersCountEnd = Driver.FindElements(By.CssSelector("[data-e2e=\"gridRow\"]")).Count;
            VerifyIsTrue(ordersCountBegin.Equals(ordersCountEnd), "no orders added");
        }
    }

    [TestFixture]
    public class CRMSalesFunnelManagersTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\SalesFunnelSettings\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Customers.Departments.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Customers.Customer.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Customers.Managers.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\CRM.SalesFunnel_Manager.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Catalog.Product.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Catalog.Category.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\[Order].Lead.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Customers.Task.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\[Order].LeadItem.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\CRM\\SalesFunnelSettings\\Settings.Settings.csv"
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
        public void SalesFunnelManagerAdd()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelManagers\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelManagers\"] input")).SendKeys("Manager");
            Driver.FindElement(By.CssSelector(".ui-select-choices-row-inner")).Click();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelManagers\"]")).Text.Contains("Manager Name"),
                "manager added");

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();

            Refresh();
            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelManagers\"]")).Text.Contains("Manager Name"),
                "manager added after refresh");

            //check manager with access
            ReInitClient();
            Functions.LogCustomer(Driver, BaseUrl, customerId: "cfc2c33b-1e84-415e-8482-e98156341605",
                auth:
                "232E2C4925A89C0C80EFD8BD732CFCD146913D4CEF091BC6B01E3C784D3249C95F4DD67640DB23DC7B69B5A53BBE2C16C43188A1BDBC873E6B315DF23351E5B5FDE86DDD9AB5BA6A95C03396AD2E01822E81EBFA58F3D41E9171221E9364A4B43F6575E4937B491F29967D6BEF82357723ED88FD03D0E824DAE18DFFB618359A81FD1B668FDDEC6670413FDDBF49E7449B2E67E4");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".original-header-page")).Text.Contains("Sales Funnel 2"),
                "sales funnel available");
            GoToAdmin("leads?salesFunnelId=-1");
            Driver.MouseFocus(By.CssSelector(".original-header-page"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".dropdown-menu.dropdown-menu--limited")).Text
                    .Contains("Sales Funnel 2"), "sales funnel in menu available");

            GoToAdmin("settingscrm#?crmTab=salesfunnels");
            VerifyIsTrue(Driver.PageSource.Contains("Sales Funnel 2"), "sales funnel in crm settings available");

            //check manager with no access
            ReInitClient();
            Functions.LogCustomer(Driver, BaseUrl, customerId: "cfc2c33b-1e84-415e-8482-e98156341602",
                auth:
                "7E1506A60315881E5528A4C64895BB0AEB940C40FF816815F5598958A4572DA54921759D3D5BDEDB9298F6DB8F97D73A8211B9E0F1266E90FF92DC5FAC17FE31F4B0E904C82CD7A34CF02D30E0B716ED433A8016AF98010E80F02DB9A5EBCD7A456DFB06513980B834E548317E148854A4B3F23651DD94A35BE9978719C85938210CCDD5DCB191343E6F7A2B4D5B16E49D7B5B8C58D4CC3C0A8A417E721D5FCE5EC4CFC7");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".original-header-page")).Text.Contains("Sales Funnel 2"),
                "sales funnel not available");

            GoToAdmin("leads?salesFunnelId=-1");
            Driver.MouseFocus(By.CssSelector(".original-header-page"));
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".dropdown-menu.dropdown-menu--limited")).Text
                    .Contains("Sales Funnel 2"), "sales funnel in menu not available");
        }

        [Test]
        public void SalesFunnelManagerDelete()
        {
            //pre check manager with no access
            ReInitClient();
            Functions.LogCustomer(Driver, BaseUrl, customerId: "cfc2c33b-1e84-415e-8482-e98156341601",
                auth:
                "CFCF735F1576CD08EBFAEABE900AD76C39BDBF0AC82563E4A0C2C7DDFFAF4EC6D24024BB2EB215D1F998730D560442322F9DF9ACD9442902C26483700F67BCE8ADD705EB9DF3154BA33F635B76863DF4794BDD784F8BA3284EBE0BCC81D939BA1D0D2581ED44366CB836D4B704A469CD02A21C8893BCC3968FA43E633ADA8D1E82F25EDBAEE43DD1B6B3B28E34F14610B80DEC8E");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".original-header-page")).Text.Contains("Sales Funnel 3"),
                "pre check sales funnel not available");

            GoToAdmin("leads?salesFunnelId=-1");
            Driver.MouseFocus(By.CssSelector(".original-header-page"));
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".dropdown-menu.dropdown-menu--limited")).Text
                    .Contains("Sales Funnel 3"), "pre check sales funnel in menu not available");

            //delete manager from sales funnel
            ReInit();
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(2, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelManagers\"] .close.ui-select-match-close"))
                .Click();

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelManagers\"]")).Text.Contains("test testov"),
                "manager deleted");
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();

            //after refresh
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(2, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelManagers\"]")).Text.Contains("test testov"),
                "manager deleted after refresh");

            //check access to all managers 
            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".original-header-page")).Text.Contains("Sales Funnel 3"),
                "sales funnel available to admin");

            GoToAdmin("leads?salesFunnelId=-1");
            Driver.MouseFocus(By.CssSelector(".original-header-page"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".dropdown-menu.dropdown-menu--limited")).Text
                    .Contains("Sales Funnel 3"), "sales funnel in menu available to admin");

            ReInitClient();
            Functions.LogCustomer(Driver, BaseUrl, customerId: "cfc2c33b-1e84-415e-8482-e98156341601",
                auth:
                "CFCF735F1576CD08EBFAEABE900AD76C39BDBF0AC82563E4A0C2C7DDFFAF4EC6D24024BB2EB215D1F998730D560442322F9DF9ACD9442902C26483700F67BCE8ADD705EB9DF3154BA33F635B76863DF4794BDD784F8BA3284EBE0BCC81D939BA1D0D2581ED44366CB836D4B704A469CD02A21C8893BCC3968FA43E633ADA8D1E82F25EDBAEE43DD1B6B3B28E34F14610B80DEC8E");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".original-header-page")).Text.Contains("Sales Funnel 3"),
                "sales funnel available to manager");

            GoToAdmin("leads?salesFunnelId=-1");
            Driver.MouseFocus(By.CssSelector(".original-header-page"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".dropdown-menu.dropdown-menu--limited")).Text
                    .Contains("Sales Funnel 3"), "sales funnel in menu available to manager");
        }
    }
}