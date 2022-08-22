using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.SalesFunnel
{
    [TestFixture]
    public class SalesFunnelAddTest : BaseSeleniumTest
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
            ChangeSidebarState();
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
        public void SalesFunnelAddDisable()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            IWebElement selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            SelectElement select = new SelectElement(selectElem);
            IList<IWebElement> allOptions = select.Options;
            VerifyIsTrue(allOptions.Count == 4, "pre check funnel select count");

            Driver.FindElement(By.CssSelector("[data-e2e=\"crm_settings\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Новый список лидов"), "funnel add pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).SendKeys("Disabled Sales Funnel");

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();
            Driver.WaitForToastSuccess();

            selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            select = new SelectElement(selectElem);
            allOptions = select.Options;
            VerifyIsTrue(allOptions.Count == 5, "funnel added to default funnel select");

            VerifyIsTrue(Driver.PageSource.Contains("Disabled Sales Funnel"), "funnel added to tabs count");
            VerifyAreEqual("Disabled Sales Funnel", Driver.GetGridCell(0, "Name").Text, "name disabled name 1");
            Driver.FindElement(By.CssSelector(".ui-grid-render-container [data-e2e=\"switchOnOffLabel\"]")).Click();
            Driver.WaitForToastSuccess();

            int new_dis_funnel = Driver.FindElements(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Count - 1;
            VerifyAreEqual("Disabled Sales Funnel", Driver.GetGridCell(new_dis_funnel, "Name").Text,
                "name disabled name");

            //check crm settings
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            select = new SelectElement(selectElem);
            allOptions = select.Options;
            VerifyIsTrue(allOptions.Count == 5, "funnel added to default funnel select after refresh count");

            VerifyIsTrue(Driver.PageSource.Contains("Disabled Sales Funnel"), "funnel added to tabs after refresh");
            VerifyAreEqual("Disabled Sales Funnel", Driver.GetGridCell(new_dis_funnel, "Name").Text,
                "name disabled name after refresh");


            //check crm leads
            GoToAdmin("leads?salesFunnelId=-1");
            Driver.MouseFocus(By.CssSelector(".dropdown-toggle.header-bottom-menu-link"));
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".dropdown-menu.submenu.dropdown-menu--limited")).Text
                    .Contains("Disabled Sales Funnel"), "funnel added to menu 1");

            GoToAdmin("settingscrm#?crmTab=salesfunnels");
            Driver.GetGridCell(new_dis_funnel, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]"))
                .Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Disabled Sales Funnel", Driver.GetGridCell(0, "Name").Text, "name disabled name");

            //check crm settings
            GoToAdmin("settingscrm#?crmTab=salesfunnels");
            VerifyIsTrue(Driver.PageSource.Contains("Disabled Sales Funnel"), "funnel added to tabs after refresh");
            VerifyAreEqual("Disabled Sales Funnel", Driver.GetGridCell(0, "Name").Text,
                "name disabled name after refresh");


            //check crm leads
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.MouseFocus(By.CssSelector(".dropdown-toggle.header-bottom-menu-link"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".dropdown-menu.submenu.dropdown-menu--limited")).Text
                    .Contains("Disabled Sales Funnel"), "funnel added to menu 1");

            GoToAdmin("settingscrm#?crmTab=salesfunnels");
            Driver.GetGridCell(0, "Enable").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();
        }

        [Test]
        public void SalesFunnelAddEnabled()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelAdd\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-dialog"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Новый список лидов"), "funnel add pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).SendKeys("New Sales Funnel");

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();
            Driver.WaitForToastSuccess();


            VerifyIsTrue(Driver.PageSource.Contains("New Sales Funnel"), "funnel added to tabs count");

            //check crm settings
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            VerifyIsTrue(Driver.PageSource.Contains("New Sales Funnel"), "funnel added to tabs after refresh");

            //check crm leads
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.MouseFocus(By.CssSelector(".dropdown-toggle.header-bottom-menu-link"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".dropdown-menu.submenu.dropdown-menu--limited")).Text
                    .Contains("New Sales Funnel"), "funnel added to menu 1");
        }
    }
}