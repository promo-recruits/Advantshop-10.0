using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.CustomersGroup
{
    [TestFixture]
    public class CustomersGroupSortTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.ClearData(ClearType.Customers);
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

            GoToAdmin("settingscustomers#?tab=customerGroups");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CustomerGroupSort()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");

            //check sort by name
            Driver.GetGridCell(-1, "GroupName", "CustomerGroups").Click();
            VerifyAreEqual("CustomerGroup10", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup108", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text);

            Driver.GetGridCell(-1, "GroupName", "CustomerGroups").Click();
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup91", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text);

            //check sort by group discount
            Driver.GetGridCell(-1, "GroupDiscount", "CustomerGroups").Click();
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup109", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("0", Driver.GetGridCell(0, "GroupDiscount", "CustomerGroups").Text);
            VerifyAreEqual("3", Driver.GetGridCell(9, "GroupDiscount", "CustomerGroups").Text);

            Driver.GetGridCell(-1, "GroupDiscount", "CustomerGroups").Click();
            VerifyAreEqual("CustomerGroup121", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup130", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("50", Driver.GetGridCell(0, "GroupDiscount", "CustomerGroups").Text);
            VerifyAreEqual("50", Driver.GetGridCell(9, "GroupDiscount", "CustomerGroups").Text);

            //check sort by minimum order price 
            Driver.GetGridCell(-1, "MinimumOrderPrice", "CustomerGroups").Click();
            VerifyAreEqual("CustomerGroup21", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup30", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("10", Driver.GetGridCell(0, "MinimumOrderPrice", "CustomerGroups").Text);
            VerifyAreEqual("10", Driver.GetGridCell(9, "MinimumOrderPrice", "CustomerGroups").Text);

            Driver.GetGridCell(-1, "MinimumOrderPrice", "CustomerGroups").Click();
            VerifyAreEqual("CustomerGroup101", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup110", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("300", Driver.GetGridCell(0, "MinimumOrderPrice", "CustomerGroups").Text);
            VerifyAreEqual("300", Driver.GetGridCell(9, "MinimumOrderPrice", "CustomerGroups").Text);
       

            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();

            //check sort by name
            Driver.GetGridCell(-1, "GroupName", "CustomerGroups").Click();
            VerifyAreEqual("CustomerGroup10", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup19", Driver.GetGridCell(99, "GroupName", "CustomerGroups").Text);

            Driver.GetGridCell(-1, "GroupName", "CustomerGroups").Click();
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup190", Driver.GetGridCell(99, "GroupName", "CustomerGroups").Text);

            //check sort by group discount
            Driver.GetGridCell(-1, "GroupDiscount", "CustomerGroups").Click();
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("0", Driver.GetGridCell(0, "GroupDiscount", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup70", Driver.GetGridCell(99, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("25", Driver.GetGridCell(99, "GroupDiscount", "CustomerGroups").Text);

            Driver.GetGridCell(-1, "GroupDiscount", "CustomerGroups").Click();
            VerifyAreEqual("CustomerGroup121", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("50", Driver.GetGridCell(0, "GroupDiscount", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup80", Driver.GetGridCell(99, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("25", Driver.GetGridCell(99, "GroupDiscount", "CustomerGroups").Text);

            //check sort by minimum order price
            Driver.GetGridCell(-1, "MinimumOrderPrice", "CustomerGroups").Click();
            VerifyAreEqual("CustomerGroup21", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("10", Driver.GetGridCell(0, "MinimumOrderPrice", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup20", Driver.GetGridCell(99, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("100", Driver.GetGridCell(99, "MinimumOrderPrice", "CustomerGroups").Text);

            Driver.GetGridCell(-1, "MinimumOrderPrice", "CustomerGroups").Click();
            VerifyAreEqual("CustomerGroup101", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("300", Driver.GetGridCell(0, "MinimumOrderPrice", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup200", Driver.GetGridCell(99, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("190", Driver.GetGridCell(99, "MinimumOrderPrice", "CustomerGroups").Text);
        }
    }
}