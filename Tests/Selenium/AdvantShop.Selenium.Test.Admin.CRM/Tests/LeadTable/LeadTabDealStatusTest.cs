using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadTabDealStatusTest : BaseSeleniumTest
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
            GoToAdmin("leads?salesFunnelId=1");
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
        public void TabNew()
        {
            TabClick(".tasks-navbar", "Новый", "Еще");

            VerifyIsTrue(Driver.Url.Contains("leads"), "lead page");
            VerifyAreEqual("Найдено записей: 49",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count leads");
            VerifyAreEqual("49",
                Driver.FindElement(By.CssSelector("[data-e2e=\"Новый\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"LeadsCount\"]")).Text, "count leads in tab");
        }

        [Test]
        [Order(1)]
        public void CallToClient()
        {
            TabClick(".tasks-navbar", "Созвон с клиентом", "Еще");

            VerifyIsTrue(Driver.Url.Contains("leads"), "lead page");
            VerifyAreEqual("Найдено записей: 21",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count leads");
            VerifyAreEqual("21",
                Driver.FindElement(By.CssSelector("[data-e2e=\"Созвон с клиентом\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"LeadsCount\"]")).Text, "count leads in tab");
        }

        [Test]
        [Order(1)]
        public void PlanToClient()
        {
            TabClick(".tasks-navbar", "Выставление КП", "Еще");

            VerifyIsTrue(Driver.Url.Contains("leads"), "lead page");
            VerifyAreEqual("Найдено записей: 26",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count leads");
            VerifyAreEqual("26",
                Driver.FindElement(By.CssSelector("[data-e2e=\"Выставление КП\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"LeadsCount\"]")).Text, "count leads in tab");
        }


        [Test]
        [Order(1)]
        public void WaitClient()
        {
            TabClick(".tasks-navbar", "Ожидание решения клиента", "Еще");

            VerifyIsTrue(Driver.Url.Contains("leads"), "lead page");
            VerifyAreEqual("Найдено записей: 14",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count leads");
            VerifyAreEqual("14",
                Driver.FindElement(By.CssSelector("[data-e2e=\"Ожидание решения клиента\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"LeadsCount\"]")).Text, "count leads in tab");
        }

        [Test]
        [Order(1)]
        public void SuccessDeal()
        {
            TabClick(".tasks-navbar", "Сделка заключена", "Еще");

            VerifyIsTrue(Driver.Url.Contains("leads"), "lead page");
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count leads");
            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector("[data-e2e=\"Сделка заключена\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"LeadsCount\"]")).Text, "count leads in tab");
        }

        [Test]
        [Order(10)]
        public void DealCanceled()
        {
            TabClick(".tasks-navbar", "Сделка отклонена", "Еще");

            VerifyIsTrue(Driver.Url.Contains("leads"), "lead page");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "count leads");
        }


        protected void TabClick(string navClass = "", string tabText = "", string tablink = "")
        {
            var tabName = Driver.FindElement(By.CssSelector(navClass))
                .FindElement(By.CssSelector("[data-e2e=\"" + tabText + "\"]"));

            if (tabName.Displayed)
            {
                tabName.Click();
            }
            else
            {
                Driver.FindElement(By.PartialLinkText(tablink)).Click();
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + tabText + "\"]")).Click();
            }
        }
    }
}