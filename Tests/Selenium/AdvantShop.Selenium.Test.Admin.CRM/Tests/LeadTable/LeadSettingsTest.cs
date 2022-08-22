using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadSettingsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadItem.csv"
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
        public void DealEditList()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".fa-pencil-alt")).Click();
            VerifyAreEqual("Основная воронка",
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).GetAttribute("value"),
                "name sales funnel");
            IWebElement selectElem1 =
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelFinalSuccessAction\"]"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Создать заказ"), "action close");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSendMailNew\"] input")).Selected,
                " sales Funnel SendMailEdit");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSendMailEdit\"] input")).Selected,
                "sales Funnel SendMail ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelEnabled\"] input")).Selected,
                " sales Funnel Enabled");


            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).SendKeys("Edit funnel list name");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelFinalSuccessAction\"]"))))
                .SelectByText("Ничего не делать");
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSendMailNew\"] span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSendMailEdit\"] span")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();

            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".fa-pencil-alt")).Click();
            VerifyAreEqual("Edit funnel list name",
                Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelName\"]")).GetAttribute("value"),
                "name sales funnel edit");
            selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelFinalSuccessAction\"]"));
            select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Ничего не делать"), "action close edit");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSendMailNew\"] input")).Selected,
                " sales Funnel SendMailEdit edit");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSendMailEdit\"] input")).Selected,
                "sales Funnel SendMail  edit");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelEnabled\"] input")).Selected,
                " sales Funnel Enabled edit");
        }

        [Test]
        [Order(1)]
        public void DealAdd()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".fa-pencil-alt")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).SendKeys("Deal Status Added");

            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusAdd\"]")).Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();

            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".fa-pencil-alt")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("deal-statuses")).Text.Contains("Deal Status Added"),
                "deal status added");
        }

        [Test]
        [Order(0)]
        public void DealDelete()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".fa-pencil-alt")).Click();
            var dealStatuses = Driver.FindElements(By.CssSelector("[data-e2e-crm=\"DealStatus\"]"));
            VerifyIsTrue(dealStatuses.Count == 6, "statuses before deleting");

            Driver.FindElement(
                    By.CssSelector("[data-e2e-crm=\"DealStatusDelete\"][data-e2e-crm-deal-status-delete-id=\"2\"]"))
                .Click();
            Driver.SwalConfirm();
            var statusDeleted = Driver.FindElements(By.CssSelector("[data-e2e-crm=\"DealStatus\"]"));
            VerifyIsTrue(statusDeleted.Count == 5, "statuses after deleting 1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();

            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".fa-pencil-alt")).Click();
            statusDeleted = Driver.FindElements(By.CssSelector("[data-e2e-crm=\"DealStatus\"]"));
            VerifyIsTrue(statusDeleted.Count == 5, "statuses after deleting");
        }

        [Test]
        [Order(1)]
        public void DealEdit()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".fa-pencil-alt")).Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("deal-statuses")).Text.Contains("Сделка заключена"),
                "pre check deal status");

            Driver.FindElement(By.CssSelector("[data-e2e-crm=\"DealStatus\"][data-e2e-crm-deal-status-id=\"5\"]"))
                .Click();
            Driver.WaitForElem(AdvBy.DataE2E("dealStatusSave"));

            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Этап сделки"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).SendKeys("Deal Status Edited");

            Driver.XPathContainsText("span", "Сохранить");
            Driver.WaitForToastSuccess();

            Driver.FindElement(By.CssSelector("[data-e2e=\"salesFunnelSave\"]")).Click();

            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.CssSelector(".fa-pencil-alt")).Click();

            VerifyIsTrue(Driver.FindElement(By.TagName("deal-statuses")).Text.Contains("Deal Status Edited"),
                "deal status edited");
            VerifyIsFalse(Driver.FindElement(By.TagName("deal-statuses")).Text.Contains("Сделка заключена"),
                "deal status old");
        }
    }
}