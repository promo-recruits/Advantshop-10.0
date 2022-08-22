using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.PriceRegulation
{
    [TestFixture]
    public class PriceRegulationDecreasePriceZeroTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationDecreasePriceZeroTest\\Catalog.Brand.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationDecreasePriceZeroTest\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationDecreasePriceZeroTest\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationDecreasePriceZeroTest\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\PriceRegulation\\PriceRegulationDecreasePriceZeroTest\\Catalog.ProductCategories.csv"
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
        public void DecreasePriceZeroAllNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Decrement", selectOption: "AbsoluteValue");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationAllCheckbox\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("500");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            Refresh();

            VerifyAreEqual("TestProduct51", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("50 500 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct60", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("59 500 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("60 500 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("69 500 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void DecreasePriceZeroCategoryNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Decrement", selectOption: "AbsoluteValue");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("500");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            Refresh();

            VerifyAreEqual("TestProduct51", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("50 500 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct60", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("59 500 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void DecreasePriceZeroSubCategoryNumber()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Decrement", selectOption: "AbsoluteValue");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("500");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            /*изменение только в подкатегории?
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));
            
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);*/
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("80 500 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("89 500 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            Refresh();

            VerifyAreEqual("TestProduct91", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("90 500 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("99 500 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            Refresh();

            VerifyAreEqual("TestProduct101", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct110", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void DecreasePriceZeroAllPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Decrement", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationAllCheckbox\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("10");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("54 900 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("63 000 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void DecreasePriceZeroCategoryPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Decrement", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("5");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory3')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct41", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct50", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            Refresh();

            VerifyAreEqual("TestProduct51", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("48 450 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct60", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("57 000 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }

        [Test]
        public void DecreasePriceZeroSubCategoryPercent()
        {
            Functions.PriceRegulation(Driver, BaseUrl, @select: "Decrement", selectOption: "Percent");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationCatCheckbox\"]")).Click();
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationSelectOption\"]"))))
                .SelectByText("%");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationValue\"]")).SendKeys("7");
            Driver.FindElement(By.CssSelector("[data-e2e=\"PriceRegulationOk\"]")).Click();

            //assert
            GoToAdmin("catalog");
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory1')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            /*изменение только в подкатегории?
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory4')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));

           Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridFilterDropdownButton\"]"));

            VerifyAreEqual("TestProduct61", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct70", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);*/
            Driver.FindElements(By.CssSelector(".jstree-icon.jstree-ocl"))[3].Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.XPath("//span[contains(text(), 'TestCategory5')]")).Click();
            Driver.WaitForElem(By.TagName("ui-grid-custom"));
            VerifyAreEqual("TestProduct81", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("75 330 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct90", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("83 700 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            Refresh();

            VerifyAreEqual("TestProduct91", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("84 630 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("93 000 руб.", Driver.GetGridCell(9, "PriceString").Text);

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyAreEqual("TestProduct101", Driver.GetGridCell(0, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "PriceString").Text);
            VerifyAreEqual("TestProduct110", Driver.GetGridCell(9, "Name").Text);
            VerifyAreEqual("0 руб.", Driver.GetGridCell(9, "PriceString").Text);
        }
    }
}