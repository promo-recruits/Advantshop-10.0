using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.PriceRegulation
{
    [TestFixture]
    public class PriceRegulationIncreasePriceZeroTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreasePriceZeroTest\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreasePriceZeroTest\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreasePriceZeroTest\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreasePriceZeroTest\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreasePriceZeroTest\\Catalog.ProductCategories.csv"
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
        public void IncreasePriceZeroAllNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "AbsoluteValue");
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
            VerifyAreEqual("1 051 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct60", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("1 060 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 061 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("1 070 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreasePriceZeroCategoryNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "AbsoluteValue");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
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
            VerifyAreEqual("1 051 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct60", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("1 060 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreasePriceZeroSubCategoryNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "AbsoluteValue");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".jstree-icon.jstree-ocl"));
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'TestCategory5')]"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
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

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            /*изменение только в подкатегории?
            driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            //Driver.WaitForAjax();
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            //Driver.WaitForAjax();
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            //Driver.WaitForAjax();
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);

            Driver.WaitForAjax();*/
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'TestCategory5')]"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 081 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("1 090 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Refresh();
            VerifyAreEqual("TestProduct91", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 091 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("1 100 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Refresh();
            VerifyAreEqual("TestProduct101", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct110", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreasePriceZeroAllPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "Percent");
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
            VerifyAreEqual("67,10 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("77 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreasePriceZeroCategoryPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
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
            VerifyAreEqual("53,55 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct60", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("63 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreasePriceZeroSubCategoryPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".jstree-icon.jstree-ocl"));
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'TestCategory5')]"));
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
            //Driver.WaitForAjax();
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            //Driver.WaitForAjax();
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            //Driver.WaitForAjax();
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);

            Driver.WaitForAjax(); */
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'TestCategory5')]"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("86,67 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("96,30 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Refresh();
            VerifyAreEqual("TestProduct91", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("97,37 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("107 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("TestProduct101", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct110", Driver.GetGridCell(9, "Name").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }
    }
}