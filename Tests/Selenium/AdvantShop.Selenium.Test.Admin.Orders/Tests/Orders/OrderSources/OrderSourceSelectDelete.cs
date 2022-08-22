using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.OrderSources
{
    [TestFixture]
    public class OrderSourceSelect : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "Data\\Admin\\Orders\\OrderSources\\[Order].OrderSource.csv"
            );

            Init();
            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
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
        public void SelectItemPresent10()
        {
            gridDeSelect();
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
        }

        [Test]
        public void SelectAllItemPresent()
        {
            gridDeSelect();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("101", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        
            gridDeSelect();
            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(99, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("101", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        }

        [Test]
        public void SelectAllOnPageItemPresent()
        {
            gridDeSelect();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        
            gridDeSelect();
            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(99, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("100", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
        }

        [Test]
        public void DeSelectAllItemPresent()
        {
            gridDeSelect();
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridPaginationSelectItems("10");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("101", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Selected);
        
            gridDeSelect();
            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Selected);
        }

        public void gridDeSelect()
        {
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxSelectAll\"]")).Selected)
            {
                if (Driver.FindElement(By.CssSelector(".btn-group-vertical.ui-grid-custom-selection-info")).Text
                    .Contains("Снять выделение со всех"))

                {
                    Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
                }

                else
                {
                    Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"] span")).Click();
                }
            }
        }
    }

    [TestFixture]
    public class OrderSourceSelectDelete : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "Data\\Admin\\Orders\\OrderSources\\[Order].OrderSource.csv"
            );

            Init();
            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
            Driver.GetGridCell(2, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Thread.Sleep(2000);
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
        public void SelectItemDeleteCancel()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Source3", Driver.GetGridCell(2, "Name", "OrderSources").Text);
        }

        [Test]
        public void SelectItemDeleteOk()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Source4", Driver.GetGridCell(2, "Name", "OrderSources").Text);
        }
    }

    [TestFixture]
    public class OrderSourceDeleteWithoutSelect : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "Data\\Admin\\Orders\\OrderSources\\[Order].OrderSource.csv"
            );

            Init();
            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
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
        public void ItemDeleteWithoutSelectCancel()
        {
            Driver.GetGridCell(0, "_serviceColumn", "OrderSources")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
        }

        [Test]
        public void ItemDeleteWithoutSelectOk()
        {
            Driver.GetGridCell(0, "_serviceColumn", "OrderSources")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
            VerifyAreEqual("Source2", Driver.GetGridCell(0, "Name", "OrderSources").Text);
        }
    }

    [TestFixture]
    public class OrderSourceSelectDeleteOnPage : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "Data\\Admin\\Orders\\OrderSources\\[Order].OrderSource.csv"
            );

            Init();
            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
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
        public void ItemDeleteOnPageCancel()
        {
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "OrderSources")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Source1", Driver.GetGridCell(0, "Name", "OrderSources").Text);
        }

        [Test]
        public void ItemDeleteOnPageOk()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Source11", Driver.GetGridCell(0, "Name", "OrderSources").Text);
        }
    }

    [TestFixture]
    public class OrderSourceSelectDeleteAll : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "Data\\Admin\\Orders\\OrderSources\\[Order].OrderSource.csv"
            );

            Init();
            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
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
        public void ItemDeleteAllCancel()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalCancel();
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"));
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text);
        }

        [Test]
        public void ItemDeleteAllOk()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text);
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text);

            GoToAdmin("settingscheckout#?checkoutTab=orderSources");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".ui-grid-custom-filter-total")).Text);
            VerifyAreEqual("Ни одной записи не найдено",
                Driver.FindElement(By.CssSelector(".ui-grid-empty-text")).Text);
        }
    }
}