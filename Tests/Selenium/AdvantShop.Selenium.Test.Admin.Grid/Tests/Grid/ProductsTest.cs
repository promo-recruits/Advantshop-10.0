using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Grid.Tests.Grid
{
    [TestFixture]
    public class GridProductsTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Grid\\GridProductsTest\\Catalog.Product.csv",
                "data\\Admin\\Grid\\GridProductsTest\\Catalog.Offer.csv",
                "data\\Admin\\Grid\\GridProductsTest\\Catalog.Category.csv",
                "data\\Admin\\Grid\\GridProductsTest\\Catalog.ProductCategories.csv");

            Init();
            GoToAdmin("home");
            if (Driver.FindElements(By.CssSelector(".sidebar sidebar--default")).Count == 0)
                Driver.FindElement(By.CssSelector(".burger")).Click();

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void ProductInCategory()
        {
            //Functions.RecalculateSearch(driver, baseURL);
            GoToAdmin("catalog?categoryid=1");

            /* check Activity*/
            //1 товар активен в CSV
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);

            //2 товар неактивен в CSV
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);

            //товар 2 был неактивный, сделать его активным
            Driver.GetGridCell(1, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();

            //товар 1 был активный, сделать его неактивным
            Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(1, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);

            //check go to edit
            Driver.GetGridCell(0, "Name").Click();
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Товар \"TestProduct1\""));

            Driver.FindElement(By.LinkText("TestCategory1")).Click();


            //VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestCategory1", Driver.FindElement(By.CssSelector(".sticky-page-name h2")).Text);
            VerifyIsTrue(Driver.Url.Contains("catalog?categoryId=1"));

            /* check search */
            GoToAdmin("catalog?categoryid=1");

            //check search exist product
            VerifyAreEqual("Найдено записей: 20",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            Driver.GridFilterSendKeys("TestProduct4", "h2");

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyAreEqual("TestProduct4", Driver.GetGridCellText(0, "Name"));

            //check search not exist product
            Driver.GridFilterSendKeys("TestProduct22", "h2");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check search too much symbols 
            Driver.GridFilterSendKeys(
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", "h2");

            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check search invalid symbols 
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..", "h2");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void ProductGridSort()
        {
            GoToAdmin("catalog?categoryid=1");

            //check sort by name
            Driver.GetGridCell(-1, "Name").Click();

            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct18", Driver.GetGridCellText(9, "Name"));

            Driver.GetGridCell(-1, "Name").Click();

            VerifyAreEqual("TestProduct9", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct19", Driver.GetGridCellText(9, "Name"));

            //check sort by ProductArtNo
            Driver.GetGridCell(-1, "ProductArtNo").Click();

            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("1", Driver.GetGridCellText(0, "ProductArtNo"));
            VerifyAreEqual("TestProduct18", Driver.GetGridCellText(9, "Name"));
            VerifyAreEqual("18", Driver.GetGridCellText(9, "ProductArtNo"));

            Driver.GetGridCell(-1, "ProductArtNo").Click();

            VerifyAreEqual("TestProduct9", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("9", Driver.GetGridCellText(0, "ProductArtNo"));
            VerifyAreEqual("TestProduct19", Driver.GetGridCellText(9, "Name"));
            VerifyAreEqual("19", Driver.GetGridCellText(9, "ProductArtNo"));

            //check sort by Price
            Driver.GetGridCell(-1, "PriceString").Click();

            VerifyAreEqual("1 руб.", Driver.GetGridCellText(0, "PriceString"));
            VerifyAreEqual("10 руб.", Driver.GetGridCellText(9, "PriceString"));

            Driver.GetGridCell(-1, "PriceString").Click();

            VerifyAreEqual("20 руб.", Driver.GetGridCellText(0, "PriceString"));
            VerifyAreEqual("11 руб.", Driver.GetGridCellText(9, "PriceString"));

            //check sort by Amount
            Driver.GetGridCell(-1, "Amount").Click();

            VerifyAreEqual("1", Driver.GetGridCellText(0, "Amount"));
            VerifyAreEqual("10", Driver.GetGridCellText(9, "Amount"));

            Driver.GetGridCell(-1, "Amount").Click();

            VerifyAreEqual("20", Driver.GetGridCellText(0, "Amount"));
            VerifyAreEqual("11", Driver.GetGridCellText(9, "Amount"));

            //check sort by Activity
            Driver.GetGridCell(-1, "Enabled").Click();

            VerifyIsFalse(Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(1, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]")).Selected);

            Driver.GetGridCell(-1, "Enabled").Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "Enabled").FindElement(By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                .Selected);

            //check sort by SortOrder
            Driver.GetGridCell(-1, "SortOrder").Click();

            VerifyAreEqual("1", Driver.GetGridCellText(0, "SortOrder"));
            VerifyAreEqual("10", Driver.GetGridCellText(9, "SortOrder"));

            Driver.GetGridCell(-1, "SortOrder").Click();

            VerifyAreEqual("20", Driver.GetGridCellText(0, "SortOrder"));
            VerifyAreEqual("11", Driver.GetGridCellText(9, "SortOrder"));
        }
    }
}