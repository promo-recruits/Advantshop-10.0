using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.PriceRegulation
{
    [TestFixture]
    public class PriceRegulationIncreaseSupplyPriceTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseSupplyPriceTest\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseSupplyPriceTest\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseSupplyPriceTest\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseSupplyPriceTest\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseSupplyPriceTest\\Catalog.ProductCategories.csv"
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
        public void IncreaseSupplyPriceAllNumber()
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
            VerifyAreEqual("1 100 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("2 000 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("5 100 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("6 000 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSupplyPriceCategoryNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "AbsoluteValue");

            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationAllCheckbox\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".jstree-container-ul.jstree-children.jstree-no-dots"))
                .Count == 0);
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.WaitForElem(By.ClassName("ng-tree-search"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".jstree-container-ul.jstree-children.jstree-no-dots"))
                .Count == 1);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory2')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("2000");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory2')]")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("4 100 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct30", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("5 000 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSupplyPriceCategoryWithSubNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "AbsoluteValue");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.WaitForElem(By.ClassName("ng-tree-search"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("500");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("6 600 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("7 500 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.Id("4")).FindElement(By.CssSelector(".jstree-icon.jstree-ocl")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("8 600 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("9 500 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSupplyPriceSubCategoryNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "AbsoluteValue");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.WaitForElem(By.ClassName("ng-tree-search"));
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("1000");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));
            /* изменения только в подкатегории?

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));

           Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("61", driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'Price\']\"] input")).GetAttribute("value"));
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("70", driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[9][\'Price\']\"] input")).GetAttribute("value"));*/

            Driver.FindElement(By.Id("4")).FindElement(By.CssSelector(".jstree-icon.jstree-ocl")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("9 100 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 000 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }


        [Test]
        public void IncreaseSupplyPriceAllPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationAllCheckbox\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("5");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("105 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("1 050 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("4 305 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("5 250 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSupplyPriceCategoryPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.WaitForElem(By.ClassName("ng-tree-search"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory2')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("10");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory2')]")).Click();
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("2 310 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct30", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("3 300 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSupplyPriceCategoryWithSubPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.WaitForElem(By.ClassName("ng-tree-search"));
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("5");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("6 405 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("7 350 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.Id("4")).FindElement(By.CssSelector(".jstree-icon.jstree-ocl")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("8 505 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("9 450 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSupplyPriceSubCategoryPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "IncBySupply", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.WaitForElem(By.ClassName("ng-tree-search"));
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("7");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            /* изменени только в подкатегории???

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));

           Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("61", driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'Price\']\"] input")).GetAttribute("value"));
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("70", driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[9][\'Price\']\"] input")).GetAttribute("value")); */

            Driver.FindElement(By.Id("4")).FindElement(By.CssSelector(".jstree-icon.jstree-ocl")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("8 667 руб.", Driver.GetGridCell(0, "PriceString").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("9 630 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }
    }
}