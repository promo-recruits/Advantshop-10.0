using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Grid.Tests.Grid
{
    [TestFixture]
    public class GridProductsFilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Grid\\FilterGrid\\Catalog.Brand.csv",
                "Data\\Admin\\Grid\\FilterGrid\\Catalog.Category.csv",
                "Data\\Admin\\Grid\\FilterGrid\\Catalog.Product.csv",
                "Data\\Admin\\Grid\\FilterGrid\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Grid\\FilterGrid\\Catalog.Offer.csv",
                "Data\\Admin\\Grid\\FilterGrid\\Catalog.Photo.csv"
            );

            Init();
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
        public void GridFilterImg()
        {
            GoToAdmin("catalog?categoryid=1");

            //check filter img
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "PhotoSrc", filterItem: "С фотографией");

            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"PhotoSrc\"]"))
                .Displayed);

            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Без фотографии\"]")).Click();


            VerifyAreEqual("TestProduct21", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct55", Driver.GetGridCellText(9, "Name"));

            //check go to edit and back
            Driver.GetGridCell(0, "Name").FindElement(By.CssSelector("[ng-href=\"product/edit/21\"]")).Click();
            Driver.WaitForElem(By.ClassName("product-setting-title"));

            GoBack();

            VerifyAreEqual("TestProduct21", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct55", Driver.GetGridCellText(9, "Name"));
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"PhotoSrc\"]"))
                .Displayed);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "PhotoSrc");
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));
        }

        [Test]
        public void GridFilterPrice()
        {
            GoToAdmin("catalog?categoryid=2");

            //check filter price
            Functions.GridFilterSet(Driver, BaseUrl, name: "PriceString");

            //ckeck initial count
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyAreEqual("26",
                Driver.FindElement(By.CssSelector(
                        "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"PriceString\"] [data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetAttribute("value"));
            VerifyAreEqual("35",
                Driver.FindElement(By.CssSelector(
                        "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"PriceString\"] [data-e2e=\"gridFilterItemRangeTo\"]"))
                    .GetAttribute("value"));

            Driver.SetGridFilterRange("PriceString", "30", "35");
            Refresh();

            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(5, "Name"));
            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //check go to edit and back
            Driver.GetGridCell(0, "Name").FindElement(By.CssSelector("[ng-href=\"product/edit/30\"]")).Click();
            Driver.WaitForElem(By.ClassName("product-setting-title"));

            GoBack();

            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(5, "Name"));
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"PriceString\"]"))
                .Displayed);
            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "PriceString");
            VerifyAreEqual("TestProduct26", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(9, "Name"));
            Refresh();
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Test]
        public void GridFilterAmount()
        {
            GoToAdmin("catalog?categoryid=2");

            //check filter amount
            Functions.GridFilterSet(Driver, BaseUrl, name: "Amount");

            //ckeck initial amount
            VerifyAreEqual("26",
                Driver.FindElement(By.CssSelector(
                        "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Amount\"] [data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetAttribute("value"));
            VerifyAreEqual("35",
                Driver.FindElement(By.CssSelector(
                        "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Amount\"] [data-e2e=\"gridFilterItemRangeTo\"]"))
                    .GetAttribute("value"));

            Driver.SetGridFilterRange("Amount", "30", "");

            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(5, "Name"));

            string strUrl = Driver.Url;

            //check go to edit and back
            Driver.GetGridCell(0, "Name").FindElement(By.CssSelector("[ng-href=\"product/edit/30\"]")).Click();
            Driver.WaitForElem(By.ClassName("product-setting-title"));

            Driver.Navigate().GoToUrl(strUrl);

            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(5, "Name"));
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Amount\"]"))
                .Displayed);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "Amount");
            VerifyAreEqual("TestProduct26", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(9, "Name"));
        }

        [Test]
        public void GridFilterSort()
        {
            GoToAdmin("catalog?categoryid=2");

            //check filter sort
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");

            //ckeck initial count
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector(
                        "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"SortOrder\"] [data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetAttribute("value"));
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector(
                        "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"SortOrder\"] [data-e2e=\"gridFilterItemRangeTo\"]"))
                    .GetAttribute("value"));

            Driver.SetGridFilterRange("SortOrder", "30", "35");

            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(5, "Name"));

            //check go to edit and back
            Driver.GetGridCell(0, "Name").FindElement(By.CssSelector("[ng-href=\"product/edit/30\"]")).Click();
            Driver.WaitForElem(By.ClassName("product-setting-title"));

            GoBack();

            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(5, "Name"));
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"SortOrder\"]"))
                .Displayed);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "SortOrder");
            VerifyAreEqual("TestProduct26", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(9, "Name"));
        }

        [Test]
        public void GridFilterActivity()
        {
            GoToAdmin("catalog?categoryid=2");

            //check activity on
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enabled", filterItem: "Активные");
            VerifyAreEqual("TestProduct31", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(4, "Name"));

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "Enabled");
            VerifyAreEqual("TestProduct26", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(9, "Name"));

            Refresh();

            //check activity off
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enabled", filterItem: "Неактивные");
            VerifyAreEqual("TestProduct26", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(4, "Name"));

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "Enabled");
            VerifyAreEqual("TestProduct26", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(9, "Name"));
        }

        [Test]
        public void GridFilterOfferAndCount()
        {
            //price
            GoToAdmin("catalog?categoryid=2");
            Functions.GridFilterSet(Driver, BaseUrl, name: "PriceString");

            Driver.SetGridFilterRange("PriceString", "30", "33");

            VerifyAreEqual("TestProduct30", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct33", Driver.GetGridCellText(3, "Name"));

            //amount
            Functions.GridFilterSet(Driver, BaseUrl, name: "Amount");

            Driver.SetGridFilterRange("Amount", "33", "35");

            VerifyAreEqual("TestProduct33", Driver.GetGridCellText(0, "Name"));

            //close price
            Functions.GridFilterClose(Driver, BaseUrl, name: "PriceString");
            VerifyAreEqual("TestProduct33", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct34", Driver.GetGridCellText(1, "Name"));
            VerifyIsTrue(Driver
                .FindElements(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Amount\"]"))
                .Count == 1);

            //close amount
            Functions.GridFilterClose(Driver, BaseUrl, name: "Amount");
            VerifyAreEqual("TestProduct26", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct35", Driver.GetGridCellText(9, "Name"));
        }

        [Test]
        public void GridFilterArtNo()
        {
            GoToAdmin("catalog?categoryId=1");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnArtNo");

            Driver.SetGridFilterValue("_noopColumnArtNo", "10");

            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(0, "Name"));

            //check go to edit and back
            Driver.FindElement(
                By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'Name\']\"] [ng-href=\"product/edit/10\"]")).Click();
            Driver.WaitForElem(By.ClassName("product-setting-title"));

            GoBack();

            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(0, "Name"));
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnArtNo\"]")).Displayed);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnArtNo");
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));
        }

        [Test]
        public void GridFilterName()
        {
            GoToAdmin("catalog?categoryId=1");
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnName");

            Driver.SetGridFilterValue("_noopColumnName", "TestProduct5");

            VerifyAreEqual("TestProduct5", Driver.GetGridCellText(0, "Name"));

            //check go to edit and back
            Driver.GetGridCell(0, "Name").FindElement(By.CssSelector("[ng-href=\"product/edit/5\"]")).Click();
            Driver.WaitForElem(By.ClassName("product-setting-title"));

            GoBack();

            VerifyAreEqual("TestProduct5", Driver.GetGridCellText(0, "Name"));
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnName\"]")).Displayed);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnName");
            VerifyAreEqual("TestProduct1", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct10", Driver.GetGridCellText(9, "Name"));
        }

        [Test]
        public void GridFilterBrand()
        {
            GoToAdmin("catalog?categoryid=5");

            //check brand
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "BrandId", filterItem: "BrandName11");
            VerifyAreEqual("TestProduct45", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct50", Driver.GetGridCellText(5, "Name"));

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "BrandId");
            VerifyAreEqual("TestProduct39", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct48", Driver.GetGridCellText(9, "Name"));

            Refresh();

            //check no brand
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "BrandId", filterItem: "BrandName20");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, name: "BrandId");
            VerifyAreEqual("TestProduct39", Driver.GetGridCellText(0, "Name"));
            VerifyAreEqual("TestProduct48", Driver.GetGridCellText(9, "Name"));
        }
    }
}