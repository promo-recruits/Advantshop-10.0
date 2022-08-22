using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Grid.Tests.Grid
{
    [TestFixture]
    public class GridAllProductsSelectedInCategoryTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Grid\\AllProdSelectInCatTest\\Catalog.Product.csv",
                "data\\Admin\\Grid\\AllProdSelectInCatTest\\Catalog.Offer.csv",
                "data\\Admin\\Grid\\AllProdSelectInCatTest\\Catalog.Category.csv",
                "data\\Admin\\Grid\\AllProdSelectInCatTest\\Catalog.ProductCategories.csv");

            Init();
            GoToAdmin("catalog?categoryid=1");
            if (Driver.FindElements(By.CssSelector(".sidebar sidebar--default")).Count == 0)
                Driver.FindElement(By.CssSelector(".burger")).Click();
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

        // все товары в категории TestCategory1 активны
        [Test]
        public void ProductsAllInCategorySelectedNotActive()
        {
            GoToAdmin("catalog?categoryid=1");

            //выбрать все товары на странице
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            //выбрать все товары категории
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();

            Driver.XPathContainsText("span", "Сделать неактивными");

            Refresh();

            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
        }

        // все товары в категории TestCategory2 неактивны
        [Test]
        public void ProductsAllInCategorySelectedActive()
        {
            GoToAdmin("catalog?categoryid=2");
            //выбрать все товары на странице
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            //выбрать все товары категории
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();

            Driver.XPathContainsText("span", "Сделать активными");

            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
        }
    }
}