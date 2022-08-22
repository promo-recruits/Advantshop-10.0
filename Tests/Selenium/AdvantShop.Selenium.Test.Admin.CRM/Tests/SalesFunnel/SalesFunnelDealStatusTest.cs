using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.SalesFunnel
{
    [TestFixture]
    public class CRMSalesFunnelDealStatusTest : BaseSeleniumTest
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
        public void SalesFunnelDealStatuaSort()
        {
            //pre check crm leads
            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".tasks-navbar .nav-item"))[1].Text
                    .Contains("Funnel 3 Deal Status 1"), "pre check deal status 1 in funnel"); // [0] - all leads tab
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".tasks-navbar .nav-item"))[2].Text
                    .Contains("Funnel 3 Deal Status 2"), "pre check deal status 2 in funnel");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".tasks-navbar .nav-item"))[3].Text
                    .Contains("Funnel 3 Deal Status 3"), "pre check deal status 3 in funnel");

            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(2, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            //pre check crm settings
            int dealStatusesCount = Driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count;
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".row.as-sortable-item"))[0].Text.Contains("Funnel 3 Deal Status 1"),
                "pre check deal status 1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".row.as-sortable-item"))[1].Text.Contains("Funnel 3 Deal Status 2"),
                "pre check deal status 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".row.as-sortable-item"))[2].Text.Contains("Funnel 3 Deal Status 3"),
                "pre check deal status 3");

            Functions.DragDropElement(Driver,
                Driver.FindElement(By.CssSelector(".row.as-sortable-item .as-sortable-item-handle")),
                Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]"))); //1 element to last

            int dealStatusesCountSorted = Driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count;
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".row.as-sortable-item"))[0].Text.Contains("Funnel 3 Deal Status 2"),
                "sorted deal status 1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".row.as-sortable-item"))[1].Text.Contains("Funnel 3 Deal Status 3"),
                "sorted deal status 2");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".row.as-sortable-item"))[2].Text.Contains("Funnel 3 Deal Status 1"),
                "sorted deal status 3");
            VerifyIsTrue(dealStatusesCount == dealStatusesCountSorted, "sorted deal statuses count the same");

            //check crm settings
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(2, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            dealStatusesCountSorted = Driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count;
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".row.as-sortable-item"))[0].Text.Contains("Funnel 3 Deal Status 2"),
                "sorted deal status 1 after refresh");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".row.as-sortable-item"))[1].Text.Contains("Funnel 3 Deal Status 3"),
                "sorted deal status 2 after refresh");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".row.as-sortable-item"))[2].Text.Contains("Funnel 3 Deal Status 1"),
                "sorted deal status 3 after refresh");
            VerifyIsTrue(dealStatusesCount == dealStatusesCountSorted,
                "sorted deal statuses count the same after refresh");

            //check crm leads
            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".tasks-navbar .nav-item"))[1].Text
                    .Contains("Funnel 3 Deal Status 2"), "sorted deal status 1 in funnel");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".tasks-navbar .nav-item"))[2].Text
                    .Contains("Funnel 3 Deal Status 3"), "sorted deal status 2 in funnel");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector(".tasks-navbar .nav-item"))[3].Text
                    .Contains("Funnel 3 Deal Status 1"), "sorted deal status 3 in funnel");

            GoToAdmin("leads?salesFunnelId=1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Funnel 3 Deal Status 1"),
                "sorted deal status not added to sales funnel 1 tabs");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Funnel 3 Deal Status 1"),
                "sorted deal status not added to sales funnel 2 tabs");

            GoToAdmin("leads?salesFunnelId=4");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Funnel 3 Deal Status 1"),
                "sorted deal status not added to sales funnel 4 tabs");
        }


        [Test]
        public void SalesFunnelDealStatusAdd()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(2, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            int dealStatusesCount = Driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count;

            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).SendKeys("New Deal Status Test");

            Driver.FindElement(By.TagName("color-picker")).FindElement(By.TagName("input")).Click();
            Driver.ClearInput(Driver.FindElement(By.TagName("color-picker")).FindElement(By.TagName("input")));
            Driver.FindElement(By.TagName("color-picker")).FindElement(By.TagName("input")).SendKeys("000000");

            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusAdd\"]")).Click();
            Driver.WaitForToastSuccess();

            int dealStatusesCountAdded = Driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count;

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"24\"]")).Text
                    .Contains("New Deal Status Test"), "new deal status added");
            VerifyAreEqual("color: rgb(0, 0, 0);",
                Driver.FindElement(By.TagName("deal-statuses")).FindElements(By.CssSelector(".row.as-sortable-item"))
                    [dealStatusesCountAdded - 2].FindElement(By.CssSelector(".fa.fa-circle")).GetAttribute("style"),
                "new deal status color");

            //check crm settings
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(2, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"24\"]")).Text
                    .Contains("New Deal Status Test"), "new deal status added after refresh");
            VerifyAreEqual("color: rgb(0, 0, 0);",
                Driver.FindElement(By.TagName("deal-statuses")).FindElements(By.CssSelector(".row.as-sortable-item"))
                    [dealStatusesCountAdded - 2].FindElement(By.CssSelector(".fa.fa-circle")).GetAttribute("style"),
                "new deal status color after refresh");
            VerifyIsTrue(dealStatusesCountAdded - dealStatusesCount == 1, "new deal status added count");

            //check crm leads
            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("New Deal Status Test"),
                "new deal status added to sales funnel 3 tabs");

            GoToAdmin("leads?salesFunnelId=1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("New Deal Status Test"),
                "new deal status not added to sales funnel 1 tabs");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("New Deal Status Test"),
                "new deal status not added to sales funnel 2 tabs");

            GoToAdmin("leads?salesFunnelId=4");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("New Deal Status Test"),
                "new deal status not added to sales funnel 4 tabs");
        }


        [Test]
        public void SalesFunnelDealStatusEdit()
        {
            //pre check check crm leads
            GoToAdmin("leads?salesFunnelId=2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Funnel 2 Deal Status 1"),
                "pre check deal status in sales funnel tabs");

            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            int dealStatusesCount = Driver.FindElements(By.CssSelector("#tab-2 .row.as-sortable-item")).Count;

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"7\"]")).Text
                    .Contains("Funnel 2 Deal Status 1"), "pre check deal status name");

            Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"7\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).SendKeys("Edited Deal Status");

            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("color-picker"))
                .FindElement(By.TagName("input")).Click();
            Driver.ClearInput(Driver.FindElement(By.CssSelector(".modal-content"))
                .FindElement(By.TagName("color-picker")).FindElement(By.TagName("input")));
            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("color-picker"))
                .FindElement(By.TagName("input")).SendKeys("FF4500");
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"dealStatusSave\"]")).Click();
            Driver.WaitForToastSuccess();

            int dealStatusesCountEdit = Driver.FindElements(By.CssSelector("#tab-2 .row.as-sortable-item")).Count;

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"7\"]")).Text
                    .Contains("Edited Deal Status"), "deal status name edited");
            VerifyAreEqual("color: rgb(255, 69, 0);",
                Driver.FindElement(By.TagName("deal-statuses")).FindElements(By.CssSelector(".row.as-sortable-item"))[0]
                    .FindElement(By.CssSelector(".fa.fa-circle")).GetAttribute("style"), "deal status color edited");

            //check crm settings
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"7\"]")).Text
                    .Contains("Edited Deal Status"), "deal status name edited after refresh");
            VerifyAreEqual("color: rgb(255, 69, 0);",
                Driver.FindElement(By.TagName("deal-statuses")).FindElements(By.CssSelector(".row.as-sortable-item"))[0]
                    .FindElement(By.CssSelector(".fa.fa-circle")).GetAttribute("style"),
                "deal status color edited after refresh");

            VerifyIsTrue(dealStatusesCount == dealStatusesCountEdit, "no deal status addeed");

            //check crm leads
            GoToAdmin("leads?salesFunnelId=2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Edited Deal Status"),
                "edited deal status name in sales funnel tabs");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Funnel 2 Deal Status 1"),
                "prev deal status name in sales funnel tabs");
        }

        [Test]
        public void SalesFunnelDealStatusDelete()
        {
            //pre check check crm leads
            GoToAdmin("leads?salesFunnelId=1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Funnel 1 Deal Status 4"),
                "pre check deal status in sales funnel tabs");

            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();

            int dealStatusesCount = Driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count;

            Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-delete-id=\"4\"]")).Click();
            Driver.SwalConfirm();

            int dealStatusesCountDelete = Driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count;
            VerifyIsFalse(Driver.PageSource.Contains("Funnel 1 Deal Status 4"), "deal status deleted");
            VerifyIsTrue(dealStatusesCount - dealStatusesCountDelete == 1, "deal status deleted count");

            //check crm settings
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            VerifyIsFalse(Driver.PageSource.Contains("Funnel 1 Deal Status 4"), "deal status deleted after refresh");
            VerifyIsTrue(dealStatusesCount - dealStatusesCountDelete == 1, "deal status deleted count after refresh");

            //check crm leads
            GoToAdmin("leads?salesFunnelId=1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Funnel 1 Deal Status 4"),
                "deleted deal status from sales funnel tabs");
        }


        [Test]
        public void SalesFunnelDealStatusSystemEdit()
        {
            //pre check crm leads
            GoToAdmin("leads?salesFunnelId=1");
            if (Driver.FindElement(By.CssSelector("li .btn.dropdown-toggle")).Displayed)
            {
                Driver.XPathContainsText("a", "Еще");
            }

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Сделка заключена"),
                "pre check system deal status 1 in funnel");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Сделка отклонена"),
                "pre check system deal status 2 in funnel");

            //pre check crm settings
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"16\"]")).Text
                    .Contains("Сделка заключена"), "pre check system deal status 1 name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"17\"]")).Text
                    .Contains("Сделка отклонена"), "pre check system deal status 2 name");

            int allDealStatuses = Driver.FindElements(By.CssSelector(".flex.payment-row")).Count;

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"DealStatusAdd\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"16\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]"))
                .SendKeys("Edited System Deal Status Success");

            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("color-picker"))
                .FindElement(By.TagName("input")).Click();
            Driver.ClearInput(Driver.FindElement(By.CssSelector(".modal-content"))
                .FindElement(By.TagName("color-picker")).FindElement(By.TagName("input")));
            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("color-picker"))
                .FindElement(By.TagName("input")).SendKeys("FFFF00");
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"dealStatusSave\"]")).Click();
            Driver.WaitForToastSuccess();

            Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"17\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]"))
                .SendKeys("Edited System Deal Status Fail");

            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("color-picker"))
                .FindElement(By.TagName("input")).Click();
            Driver.ClearInput(Driver.FindElement(By.CssSelector(".modal-content"))
                .FindElement(By.TagName("color-picker")).FindElement(By.TagName("input")));
            Driver.FindElement(By.CssSelector(".modal-content")).FindElement(By.TagName("color-picker"))
                .FindElement(By.TagName("input")).SendKeys("800080");
            Driver.FindElement(By.CssSelector("[data-e2e=\"DealStatusName\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"dealStatusSave\"]")).Click();
            Driver.WaitForToastSuccess();

            int allDealStatusesEdit = Driver.FindElements(By.CssSelector(".flex.payment-row")).Count;

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"16\"]")).Text
                    .Contains("Edited System Deal Status Success"), "system deal status 1 name edited");
            VerifyAreEqual("color: rgb(255, 255, 0);",
                Driver.FindElements(By.CssSelector(".flex.payment-row"))[allDealStatuses - 2]
                    .FindElement(By.CssSelector(".fa.fa-circle")).GetAttribute("style"),
                "system deal status 1 color edited");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"17\"]")).Text
                    .Contains("Edited System Deal Status Fail"), "system deal status 2 name edited");
            VerifyAreEqual("color: rgb(128, 0, 128);",
                Driver.FindElements(By.CssSelector(".flex.payment-row"))[allDealStatuses - 1]
                    .FindElement(By.CssSelector(".fa.fa-circle")).GetAttribute("style"),
                "system deal status 2 color edited");

            VerifyIsTrue(allDealStatuses == allDealStatusesEdit, "system deal statuses' count the same");

            //check crm settings 
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            allDealStatusesEdit = Driver.FindElements(By.CssSelector(".flex.payment-row")).Count;

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"16\"]")).Text
                    .Contains("Edited System Deal Status Success"), "system deal status 1 name edited after refresh");
            VerifyAreEqual("color: rgb(255, 255, 0);",
                Driver.FindElements(By.CssSelector(".flex.payment-row"))[allDealStatuses - 2]
                    .FindElement(By.CssSelector(".fa.fa-circle")).GetAttribute("style"),
                "system deal status 1 color edited after refresh");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-id=\"17\"]")).Text
                    .Contains("Edited System Deal Status Fail"), "system deal status 2 name edited after refresh");
            VerifyAreEqual("color: rgb(128, 0, 128);",
                Driver.FindElements(By.CssSelector(".flex.payment-row"))[allDealStatuses - 1]
                    .FindElement(By.CssSelector(".fa.fa-circle")).GetAttribute("style"),
                "system deal status 2 color edited after refresh");

            VerifyIsTrue(allDealStatuses == allDealStatusesEdit, "system deal statuses' count the same  after refresh");

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".modal-content")).Text.Contains("Сделка заключена"),
                "prev system deal status 1 name");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".modal-content")).Text.Contains("Сделка отклонена"),
                "prev system deal status 2 name");

            //crm leads
            GoToAdmin("leads?salesFunnelId=1");
            if (Driver.FindElement(By.CssSelector("li .btn.dropdown-toggle")).Displayed)
            {
                Driver.XPathContainsText("a", "Еще");
            }

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Сделка заключена"),
                "prev system deal status 1 name in funnel");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Сделка отклонена"),
                "prev system deal status 2 name in funnel");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Edited System Deal Status Success"),
                "edited system deal status 1 in funnel");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".tasks-navbar")).Text.Contains("Edited System Deal Status Fail"),
                "edited system deal status 2 in funnel");
        }
    }

    [TestFixture]
    public class CRMSalesFunnelDealStatusDeleteTest : BaseSeleniumTest
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
        public void SalesFunnelDealStatusSystemNotDelete()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();
            int notSystemDealStatuses = Driver.FindElements(By.XPath("//a[@data-e2e-crm-deal-status-delete-id]")).Count;
            int allDealStatuses = Driver.FindElements(By.CssSelector("[data-e2e-crm=\"DealStatus\"]")).Count;


            VerifyIsTrue(allDealStatuses - notSystemDealStatuses == 2,
                "system deal status can't be deleted"); //2 system statuses at all
        }

        [Test]
        public void SalesFunnelDealStatusDeleteAllNoLeads()
        {
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(2, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count == 4,
                "pre check deal statuses + adding status field");

            deleteDealStatuses(Driver, 13);

            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(2, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count == 1,
                "deal statuses deleted except adding status field");
        }

        [Test]
        public void SalesFunnelDealStatusDeleteAllWithLeads()
        {
            //pre check
            GoToAdmin("leads?salesFunnelId=2");
            VerifyIsTrue(Driver.GetGridCell(0, "Id").Text.Contains("120"), "pre check leads id with deal statuses");
            VerifyAreEqual("Funnel 2 Deal Status 3", Driver.GetGridCell(0, "DealStatusName").Text,
                "pre check leads with deleted deal statuses");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "pre check count all leads in funnel");

            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count == 7,
                "deal statuses + adding status field");

            deleteDealStatuses(Driver, 7);

            //check crm settings
            GoToAdmin("settingscrm#?crmTab=salesfunnels");

            Driver.GetGridCell(1, "Name").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count == 1,
                "deal statuses deleted except adding status field");

            //check crm leads
            GoToAdmin("leads?salesFunnelId=2");
            VerifyIsTrue(Driver.GetGridCell(0, "Id").Text.Contains("120"),
                "leads not deleted from deleted deal statuses");
            VerifyAreEqual("", Driver.GetGridCell(0, "DealStatusName").Text, "leads without deleted deal statuses");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all leads in funnel not deleted");

            GoToAdmin("leads?salesFunnelId=-1");
            Driver.GetGridFilterTab(0, "120");
            VerifyIsTrue(Driver.GetGridCell(0, "Id").Text.Contains("120"), "leads with deleted deal statuses to all");
            VerifyAreEqual("", Driver.GetGridCell(0, "DealStatusName").Text, "leads with deleted deal statuses");
        }

        public void deleteDealStatuses(IWebDriver driver, int element)
        {
            int count = driver.FindElements(By.CssSelector(".row.as-sortable-item")).Count;
            for (int i = 0; i < count - 1; i++, element++)
            {
                driver.FindElement(By.CssSelector("[data-e2e-crm-deal-status-delete-id=\"" + element + "\"]")).Click();
                Driver.SwalConfirm();
            }
        }
    }
}