using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.CatalogProperties.Tag
{
    [TestFixture]
    public class TagFilter : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "Data\\Admin\\Catalog\\Tag\\Catalog.Category.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.Tag.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.Product.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Catalog\\Tag\\Catalog.TagMap.csv"
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
        public void ByName()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Functions.GridFilterSet(Driver, BaseUrl, "Name");
            Driver.SetGridFilterValue("Name", "New_tag10");

            VerifyAreEqual(7,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag105", Driver.GetGridCell(6, "Name", "Tags").Text);

            Driver.GetGridCell(0, "Name", "Tags").Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.Url.Contains("tags/edit/10"));
            GoBack();
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag105", Driver.GetGridCell(6, "Name", "Tags").Text);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
        }

        [Test]
        public void ByURLPath()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Functions.GridFilterSet(Driver, BaseUrl, "UrlPath");
            Driver.SetGridFilterValue("UrlPath", "teg10");

            VerifyAreEqual(7,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag105", Driver.GetGridCell(6, "Name", "Tags").Text);
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "UrlPath");
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
        }

        [Test]
        public void ByEnabledYes()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Functions.GridFilterSet(Driver, BaseUrl, "Enabled");
            Driver.SetGridFilterSelectValue("Enabled", "Активные");

            VerifyAreEqual(100,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag100", Driver.GetGridCell(99, "Name", "Tags").Text);
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
        }

        [Test]
        public void ByEnabledNo()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Functions.GridFilterSet(Driver, BaseUrl, "Enabled");
            Driver.SetGridFilterSelectValue("Enabled", "Неактивные");
            VerifyAreEqual(4,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            VerifyAreEqual("New_Tag102", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag105", Driver.GetGridCell(3, "Name", "Tags").Text);
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
        }

        [Test]
        public void ByNameAndActive()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Functions.GridFilterSet(Driver, BaseUrl, "Name");
            Driver.SetGridFilterValue("Name", "New_tag10");

            VerifyAreEqual("New_Tag10", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag105", Driver.GetGridCell(6, "Name", "Tags").Text);

            Functions.GridFilterSet(Driver, BaseUrl, "Enabled");
            Driver.SetGridFilterSelectValue("Enabled", "Активные");

            VerifyAreEqual(3,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag101", Driver.GetGridCell(2, "Name", "Tags").Text);

            //close name
            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);

            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(9, "Name", "Tags").Text);
        }

        [Test]
        public void DelByName()
        {
            GoToAdmin("settingscatalog#?catalogTab=tags");
            Functions.GridFilterSet(Driver, BaseUrl, "Name");
            Driver.SetGridFilterValue("Name", "New_tag10");

            VerifyAreEqual(7,
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count);
            VerifyAreEqual("New_Tag10", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag105", Driver.GetGridCell(6, "Name", "Tags").Text);
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"]"))
                .Displayed);

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"Name\"]"))
                .Displayed);

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "Name");
            VerifyAreEqual("New_Tag1", Driver.GetGridCell(0, "Name", "Tags").Text);
            VerifyAreEqual("New_Tag11", Driver.GetGridCell(9, "Name", "Tags").Text);
        }
    }
}