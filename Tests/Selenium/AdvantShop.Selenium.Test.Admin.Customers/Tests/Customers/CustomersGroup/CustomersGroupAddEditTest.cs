using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.CustomersGroup
{
    [TestFixture]
    public class CustomersGroupAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Customers\\CustomersGroup\\Catalog.Product.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Catalog.Offer.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Catalog.Category.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Catalog.ProductCategories.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Customers.CustomerGroup.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Customers.Customer.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Customers.Departments.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Customers.Managers.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Customers.ManagerTask.csv"
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
        public void CustomersGroupAdd()
        {
            GoToAdmin("settingscustomers#?tab=customerGroups");
            Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCustomerAdd\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroupName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroupName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroupName\"]")).SendKeys("CustomersGroupNew");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroupDiscount\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroupDiscount\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroupDiscount\"]")).SendKeys("50");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroupMinOrderPrice\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroupMinOrderPrice\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroupMinOrderPrice\"]")).SendKeys("100");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroupOK\"]")).Click();

            GoToAdmin("settingscustomers#?tab=customerGroups");
            Driver.GridFilterSendKeys("CustomersGroupNew");
            Driver.DropFocusCss("[data-e2e=\"GroupCustomerTitle\"]");

            VerifyAreEqual("CustomersGroupNew", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("50", Driver.GetGridCell(0, "GroupDiscount", "CustomerGroups").Text);
            VerifyAreEqual("100", Driver.GetGridCell(0, "MinimumOrderPrice", "CustomerGroups").Text);
        }

        [Test]
        public void CustomersGroupEditInplace()
        {
            GoToAdmin("settingscustomers#?tab=customerGroups");
            Driver.GridFilterSendKeys("CustomerGroup100");
            Driver.DropFocusCss("[data-e2e=\"GroupCustomerTitle\"]");

            VerifyAreEqual("CustomerGroup100", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);

            //check input name
            Driver.SendKeysGridCell("CustomerGroup100ChangedName", 0, "GroupName", "CustomerGroups");

            //check input discount
            Driver.SendKeysGridCell("7", 0, "GroupDiscount", "CustomerGroups");

            //check input minimum order price
            Driver.SendKeysGridCell("5000", 0, "MinimumOrderPrice", "CustomerGroups");

            GoToAdmin("settingscustomers#?tab=customerGroups");
            Driver.GridFilterSendKeys("CustomerGroup100ChangedName");
            Driver.DropFocusCss("[data-e2e=\"GroupCustomerTitle\"]");

            VerifyAreEqual("CustomerGroup100ChangedName", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("7", Driver.GetGridCell(0, "GroupDiscount", "CustomerGroups").Text);
            VerifyAreEqual("5000", Driver.GetGridCell(0, "MinimumOrderPrice", "CustomerGroups").Text);
        }
    }
}