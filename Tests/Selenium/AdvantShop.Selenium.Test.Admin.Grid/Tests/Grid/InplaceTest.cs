using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Grid.Tests.Grid
{
    [TestFixture]
    public class GridProductsInplaceTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Grid\\InplaceGrid\\Catalog.Category.csv",
                "Data\\Admin\\Grid\\InplaceGrid\\Catalog.Product.csv",
                "Data\\Admin\\Grid\\InplaceGrid\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Grid\\InplaceGrid\\Catalog.Size.csv",
                "Data\\Admin\\Grid\\InplaceGrid\\Catalog.Offer.csv"
            );

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

        [Test]
        public void InplaceCorrectDataTest()
        {
            GoToAdmin("catalog?categoryId=2");

            //check price
            VerifyAreEqual("TestProduct26", Driver.GetGridCell(0, "Name").FindElement(By.TagName("a")).Text);
            Driver.SendKeysGridCell("123", 0, "PriceString");
            //check amount
            Driver.SendKeysGridCell("123", 0, "Amount");
            //check sort
            Driver.SendKeysGridCell("123", 0, "SortOrder");
            //check do active
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();

            Refresh();

            VerifyAreEqual("TestProduct26", Driver.GetGridCell(9, "Name").FindElement(By.TagName("a")).Text);
            VerifyIsTrue(Driver.GetGridCell(9, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyAreEqual("123 руб.", Driver.GetGridCellText(9, "PriceString"));
            VerifyAreEqual("123", Driver.GetGridCellText(9, "Amount"));
            VerifyAreEqual("123", Driver.GetGridCellText(9, "SortOrder"));

            //check do not active
            VerifyAreEqual("TestProduct31", Driver.GetGridCell(4, "Name").FindElement(By.TagName("a")).Text);
            VerifyIsTrue(Driver.GetGridCell(4, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            Driver.GetGridCell(4, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();

            Refresh();

            VerifyAreEqual("TestProduct31", Driver.GetGridCell(4, "Name").FindElement(By.TagName("a")).Text);
            VerifyIsFalse(Driver.GetGridCell(4, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
        }

        [Test]
        public void InplaceEditIncorrectDataLongTest()
        {
            GoToAdmin("catalog?categoryId=3");

            VerifyAreEqual("TestProduct37", Driver.GetGridCellText(1, "Name"));

            //check no edit price
            Driver.MouseFocus(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['PriceString']\"]"));
            Driver.MouseFocus(By.CssSelector("[data-e2e-grid-cell=\"grid[1]['PriceString']\"]"));
            Driver.MouseFocus(By.CssSelector("[data-e2e-grid-cell=\"grid[2]['PriceString']\"]"));

            VerifyIsTrue(Driver.GetGridCell(0, "PriceString").FindElements(By.Name("inputForm")).Count == 0);
            VerifyIsTrue(Driver.GetGridCell(1, "PriceString").FindElements(By.Name("inputForm")).Count == 1);
            VerifyIsTrue(Driver.GetGridCell(2, "PriceString").FindElements(By.Name("inputForm")).Count == 1);

            //check no edit amount
            VerifyIsTrue(Driver.GetGridCell(0, "Amount").FindElements(By.Name("inputForm")).Count == 0);
            VerifyIsTrue(Driver.GetGridCell(1, "Amount").FindElements(By.Name("inputForm")).Count == 1);
            VerifyIsTrue(Driver.GetGridCell(2, "Amount").FindElements(By.Name("inputForm")).Count == 1);

            //check long price
            Driver.SendKeysGridCell("10000000000", 1, "PriceString");

            //check long amount
            Driver.SendKeysGridCell("1000000000000", 1, "Amount");

            //check long sort
            Driver.SendKeysGridCell("1000000000", 1, "SortOrder");

            GoToAdmin("catalog?categoryId=3");
            VerifyAreEqual("TestProduct37", Driver.GetGridCellText(2, "Name"));
            VerifyAreEqual("10000 000 000 руб.", Driver.GetGridCellText(2, "PriceString"));
            VerifyAreEqual("37", Driver.GetGridCellText(2, "Amount"));
            VerifyAreEqual("1000000000", Driver.GetGridCellText(2, "SortOrder"));
        }

        [Test]
        public void InplaceEditIncorrectDataInvalidTest()
        {
            GoToAdmin("catalog?categoryId=5");

            //check invalid price
            Driver.SendKeysGridCell("hgvjhlhlhk", 0, "PriceString");

            //check invalid amount
            Driver.SendKeysGridCell("hgvjhlhlhk", 0, "Amount");

            //check invalid sort
            Driver.SendKeysGridCell("hgvjhlhlhk", 0, "SortOrder");

            GoToAdmin("catalog?categoryId=5");

            VerifyAreEqual("39 руб.", Driver.GetGridCellText(0, "PriceString"));
            VerifyAreEqual("39", Driver.GetGridCellText(0, "Amount"));
            VerifyAreEqual("39", Driver.GetGridCellText(0, "SortOrder"));
        }
    }
}