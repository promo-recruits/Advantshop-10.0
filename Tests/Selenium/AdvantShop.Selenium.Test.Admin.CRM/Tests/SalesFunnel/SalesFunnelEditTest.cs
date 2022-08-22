using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.SalesFunnel
{
    [TestFixture]
    public class CRMSalesFunnelEditTest : BaseSeleniumTest
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
        [Order(1)]
        public void SalesFunnelEdit()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".fa-pencil-alt")).Click();
            VerifyAreEqual("Sales Funnel 1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).GetAttribute("value"),
                "pre check funnel name");

            Driver.SendKeysInput(AdvBy.DataE2E("salesFunnelName"), "Edited Name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("Edited Name", Driver.GetGridCell(0, "Name").Text, "funnel name edited");
            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            VerifyAreEqual("Edited Name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).GetAttribute("value"),
                "funnel name edited");

            //check crm settings
            GoToAdmin("settingscrm#?crmTab=common");

            VerifyIsTrue(Driver.FindElement(By.Name("DefaultSalesFunnelId")).Text.Contains("Edited Name"),
                "funnel name edited in funnel default select");
            VerifyIsFalse(Driver.FindElement(By.Name("DefaultSalesFunnelId")).Text.Contains("Sales Funnel 1"),
                "funnel name prev no in funnel default select");

            //check crm leads
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.MouseFocus(By.CssSelector(".original-header-page"));
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".dropdown-menu.dropdown-menu--limited")).Text
                    .Contains("Edited Name"), "funnel edited in menu");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".dropdown-menu.dropdown-menu--limited")).Text
                    .Contains("Sales Funnel 1"), "prev funnel name in menu");

            //revert changes
            GoToAdmin("settingscrm#?crmTab=salesfunnels");
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".fa-pencil-alt")).Click();
            Driver.SendKeysInput(AdvBy.DataE2E("salesFunnelName"), "Sales Funnel 1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();
            Driver.WaitForToastSuccess();
        }

        [Test]
        [Order(1)]
        public void SalesFunnelDelete()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            IWebElement selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            SelectElement select = new SelectElement(selectElem);
            IList<IWebElement> allOptions = select.Options;
            int optionsCount = allOptions.Count;

            Driver.GetGridCell(3, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();

            selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            select = new SelectElement(selectElem);
            allOptions = select.Options;
            int optionsCountDelete = allOptions.Count;

            VerifyIsTrue(optionsCount - optionsCountDelete == 1, "funnel deleted in default funnel select");
            VerifyIsFalse(Driver.FindElement(By.TagName("sales-funnels")).Text.Contains("Sales Funnel 4"),
                "funnel deleted");

            //check crm settings
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            select = new SelectElement(selectElem);
            allOptions = select.Options;
            optionsCountDelete = allOptions.Count;

            VerifyIsTrue(optionsCount - optionsCountDelete == 1,
                "funnel deleted in default funnel select after refresh");
            VerifyIsFalse(Driver.FindElement(By.TagName("sales-funnels")).Text.Contains("Sales Funnel 4"),
                "funnel deleted after refresh");

            //check crm leads
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.MouseFocus(By.CssSelector(".original-header-page"));

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".dropdown-menu.dropdown-menu--limited")).Text
                    .Contains("Sales Funnel 4"), "deleted funnel name in menu");
        }

        [Test]
        [Order(1)]
        public void SalesFunnelDeleteDefault()
        {
            GoToAdmin("settingscrm#?crmTab=common");

            IWebElement selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            SelectElement select = new SelectElement(selectElem);
            IList<IWebElement> allOptions = select.Options;
            int optionsCount = allOptions.Count;
            string salesFunnelDefaultValue = select.SelectedOption.GetAttribute("value");
            string salesFunnelDefaultText = select.SelectedOption.Text;

            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(
                Convert.ToInt32(salesFunnelDefaultValue.Substring(salesFunnelDefaultValue.Length - 1)) - 1,
                "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();

            Driver.SwalConfirm();

            VerifyIsTrue(
                Driver.PageSource.Contains("Невозможно удалить список \"" + salesFunnelDefaultText +
                                           "\", указаный как список лидов по умолчанию"),
                "error when deleting default funnel");

            GoToAdmin("settingscrm");

            selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            select = new SelectElement(selectElem);
            allOptions = select.Options;
            int optionsCountDelete = allOptions.Count;

            VerifyIsTrue(optionsCount == optionsCountDelete, "default funnel not deleted count");
        }

        [Test]
        [Order(1)]
        public void SalesFunnelDeleteWithLeads()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            IWebElement selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            SelectElement select = new SelectElement(selectElem);
            IList<IWebElement> allOptions = select.Options;
            int optionsCount = allOptions.Count;

            if (select.SelectedOption.Text.Contains("Sales Funnel 1"))
            {
                select.SelectByText("Sales Funnel 3");
                Driver.WaitForToastSuccess();
            }

            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(
                Driver.PageSource.Contains(
                    "Вы не можете удалить список \"Sales Funnel 1\", пока в нем есть лиды. Перенесите лиды в другой список."),
                "error when deleting funnel with leads");

            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            selectElem = Driver.FindElement(By.Name("DefaultSalesFunnelId"));
            select = new SelectElement(selectElem);
            allOptions = select.Options;
            int optionsCountDelete = allOptions.Count;

            VerifyIsTrue(optionsCount == optionsCountDelete, "funnel with leads not deleted count");
        }

        [Test]
        [Order(0)]
        public void SalesFunnelLeads()
        {
            GoToAdmin("leads?salesFunnelId=1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text
                    .Contains("Найдено записей: 117"), "funnel 1 leads");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text
                    .Contains("Найдено записей: 3"), "funnel 2 leads");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "funnel 3 no leads");

            GoToAdmin("leads?salesFunnelId=4");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "funnel 4 no leads");

            GoToAdmin("leads?salesFunnelId=-1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text
                    .Contains("Найдено записей: 120"), "all leads");
        }
    }
}