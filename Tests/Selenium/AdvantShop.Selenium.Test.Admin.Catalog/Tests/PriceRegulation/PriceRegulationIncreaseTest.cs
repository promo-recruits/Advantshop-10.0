using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.PriceRegulation
{
    [TestFixture]
    public class PriceRegulationIncreaseTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseTest\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseTest\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseTest\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseTest\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationIncreaseTest\\Catalog.ProductCategories.csv"
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
        public void IncreaseAllNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "AbsoluteValue");
            VerifyAreEqual("Регулирование цен",
                Driver.FindElement(By.CssSelector("[data-e2e=\"PriceSettingTitle\"]")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationAllCheckbox\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("1000");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 001 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("1 010 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 041 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("1 050 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseCategoryNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "AbsoluteValue");

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
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("1000");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory2')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 021 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct30", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("1 030 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseCategoryWithSubNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "AbsoluteValue");
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
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("561 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("570 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.Id("4")).FindElement(By.CssSelector(".jstree-icon.jstree-ocl")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("581 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("590 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSubCategoryNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "AbsoluteValue");
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
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
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
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 081 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("1 090 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseAllPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationAllCheckbox\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("5");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1,05 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10,50 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("43,05 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("52,50 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseCategoryPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "Percent");
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
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory2')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct21", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("23,10 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct30", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("33 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseCategoryWithSubPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "Percent");
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
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("1 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("10 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("64,05 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("73,50 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.Id("4")).FindElement(By.CssSelector(".jstree-icon.jstree-ocl")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("85,05 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("94,50 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void IncreaseSubCategoryPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Increment", selectOption: "Percent");
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
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
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
            VerifyAreEqual("61 руб.", driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'Price\']\"] input")).GetAttribute("value"));
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("70 руб.", driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[9][\'Price\']\"] input")).GetAttribute("value")); */

            Driver.FindElement(By.Id("4")).FindElement(By.CssSelector(".jstree-icon.jstree-ocl")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("86,67 руб.", Driver.GetGridCell(0, "PriceString").Text);
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.MouseFocus(By.TagName("ui-grid-custom-footer"));
            Thread.Sleep(1000);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("96,30 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }
    }
}