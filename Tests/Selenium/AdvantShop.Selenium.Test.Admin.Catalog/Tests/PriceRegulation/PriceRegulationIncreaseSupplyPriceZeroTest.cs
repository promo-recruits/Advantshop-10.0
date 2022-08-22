using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.PriceRegulation
{
    [TestFixture]
    public class PriceRegulationIncreaseSupplyPriceZeroTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseSupplyPriceZeroTest\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseSupplyPriceZeroTest\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseSupplyPriceZeroTest\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseSupplyPriceZeroTest\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseSupplyPriceZeroTest\\Catalog.ProductCategories.csv"
            );

            Init();

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void IncreaseSupplyPriceZeroAllNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "AbsoluteValue");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationAllCheckbox\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("1000");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Refresh();
            VerifyAreEqual("TestProduct51", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("6 100 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct60", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("7 000 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("7 100 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("8 000 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSupplyPriceZeroCategoryNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "AbsoluteValue");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.WaitForElem(By.ClassName("ng-tree-search"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("2000");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Refresh();
            VerifyAreEqual("TestProduct51", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("7 100 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct60", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("8 000 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSupplyPriceZeroSubCategoryNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "AbsoluteValue");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("500");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            /*изменение только в подкатегории?
            driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();

          Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            Thread.Sleep(1000);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            Thread.Sleep(1000);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);

            Driver.WaitForAjax();*/
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("8 600 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("9 500 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Refresh();
            VerifyAreEqual("TestProduct91", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("9 600 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 500 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Refresh();
            VerifyAreEqual("TestProduct101", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct110", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSupplyPriceZeroAllPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationAllCheckbox\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("10");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("6 710 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("7 700 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSupplyPriceZeroCategoryPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.WaitForElem(By.ClassName("ng-tree-search"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("5");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Refresh();
            VerifyAreEqual("TestProduct51", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("5 355 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct60", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("6 300 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSupplyPriceZeroSubCategoryPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("7");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            /*изменение только в подкатегории?
            driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();

          Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            Thread.Sleep(1000);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            Thread.Sleep(1000);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);

            Driver.WaitForAjax(); */
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("8 667 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("9 630 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Refresh();
            VerifyAreEqual("TestProduct91", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("9 737 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 700 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct101", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("TestProduct110", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }
    }
}