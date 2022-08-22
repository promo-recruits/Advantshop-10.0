using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSeo.Redirect
{
    [TestFixture]
    public class Redirect301Test : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Settings.Redirect.csv"
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
        public void RedirectGrid()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");

            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "RedirectFrom");
            VerifyAreEqual("products/test-product1", Driver.GetGridCellText(0, "RedirectTo", "301Red"), "RedirectTo");
            VerifyAreEqual("1", Driver.GetGridCellText(0, "ProductArtNo", "301Red"), "ProductArtNo");

            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            GoToClient("redirect1");
            VerifyIsTrue(Driver.Url.EndsWith("products/test-product1"), "new url");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1");
        }

        [Test]
        public void RedirectEnabled()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Functions.CheckNotSelected("EnableRedirect301", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            }
            catch
            {
            }

            GoToClient("redirect1");
            VerifyIsTrue(Driver.Url.EndsWith("redirect1"), "old url");
            VerifyIsTrue(Driver.PageSource.Contains("404"), "404 page");
            VerifyIsTrue(Driver.PageSource.Contains("Страница была удалена или перемещена"), "404 page2");
            VerifyIsTrue(Is404Page("/redirect1"), "404 page 1");

            GoToAdmin("settingsseo#?seoTab=seo301");
            Functions.CheckSelected("EnableRedirect301", Driver);
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();

            GoToClient("redirect1");
            VerifyIsTrue(Driver.Url.EndsWith("products/test-product1"), "new url");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1");
        }

        [Test]
        public void RedirectInplace()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");

            Driver.GridFilterSendKeys("redirect55");
            VerifyAreEqual("redirect55", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "RedirectFrom");

            Driver.SendKeysGridCell("redirect999", 0, "RedirectFrom", "301Red");
            Driver.SendKeysGridCell("redirect1", 0, "RedirectTo", "301Red");
            Driver.SendKeysGridCell("", 0, "ProductArtNo", "301Red");

            Driver.FindElement(By.CssSelector(".ui-grid-custom-footer")).Click();
            Thread.Sleep(1000);

            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridFilterSendKeys("redirect55", By.TagName("h2"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
            Driver.GridFilterSendKeys("redirect999");
            VerifyAreEqual("redirect999", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "RedirectFrom");
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectTo", "301Red"), "RedirectTo");
            VerifyAreEqual("", Driver.GetGridCellText(0, "ProductArtNo", "301Red"), "ProductArtNo");

            GoToClient("redirect999");
            VerifyIsTrue(Driver.Url.EndsWith("products/test-product1"), "new url");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1");
        }

        [Test]
        public void RedirectzSelectDelete()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridPaginationSelectItems("10");
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "301Red")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "301Red")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("redirect2", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "301Red")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "301Red")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "301Red")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "301Red")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "301Red")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "301Red")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("redirect5", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "selected 1 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "301Red")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "301Red")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("redirect15", Driver.GetGridCellText(0, "RedirectFrom", "301Red"),
                "selected all on page 1 grid delete");
            VerifyAreEqual("redirect24", Driver.GetGridCellText(9, "RedirectFrom", "301Red"),
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("87", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "301Red")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "301Red")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-grid-custom-selection-info")).Displayed,
                " display count selected");
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".ui-grid-custom-selection-info")).Displayed,
                "count 0 selected");

            GoToAdmin("settingsseo#?seoTab=seo301");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all 2");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 2");
        }
    }

    [TestFixture]
    public class Redirect301SortTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Settings.Redirect.csv"
            );

            Init();
            GoToAdmin("settingsseo#?seoTab=seo301");
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
        public void RedirectSortFrom()
        {
            Driver.GetGridCell(-1, "RedirectFrom", "301Red").Click();
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "sort RedirectFrom 1 asc");
            VerifyAreEqual("redirect16", Driver.GetGridCellText(9, "RedirectFrom", "301Red"),
                "sort RedirectFrom 10 asc");

            Driver.GetGridCell(-1, "RedirectFrom", "301Red").Click();
            VerifyAreEqual("redirect99", Driver.GetGridCellText(0, "RedirectFrom", "301Red"),
                "sort RedirectFrom 1 desc");
            VerifyAreEqual("redirect90", Driver.GetGridCellText(9, "RedirectFrom", "301Red"),
                "sort RedirectFrom 10 desc");
        }

        [Test]
        public void RedirectSortTo()
        {
            Driver.GetGridCell(-1, "RedirectTo", "301Red").Click();
            VerifyAreEqual("redirect100", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "sort RedirectTo 1 asc");
            VerifyAreEqual("redirect89", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "sort RedirectTo 10 asc");

            Driver.GetGridCell(-1, "RedirectTo", "301Red").Click();
            VerifyAreEqual("redirect93", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "sort RedirectTo 1 desc");
            VerifyAreEqual("redirect79", Driver.GetGridCellText(9, "RedirectFrom", "301Red"),
                "sort RedirectTo 10 desc");
        }

        [Test]
        public void RedirectSortProduct()
        {
            Driver.GetGridCell(-1, "ProductArtNo", "301Red").Click();
            VerifyAreEqual("redirect84", Driver.GetGridCellText(0, "RedirectFrom", "301Red"),
                "sort ProductArtNo 1 asc");
            VerifyAreEqual("redirect93", Driver.GetGridCellText(9, "RedirectFrom", "301Red"),
                "sort ProductArtNo 10 asc");

            Driver.GetGridCell(-1, "ProductArtNo", "301Red").Click();
            VerifyAreEqual("redirect9", Driver.GetGridCellText(0, "RedirectFrom", "301Red"),
                "sort ProductArtNo 1 desc");
            VerifyAreEqual("redirect76", Driver.GetGridCellText(9, "RedirectFrom", "301Red"),
                "sort ProductArtNo 10 desc");
        }
    }


    [TestFixture]
    public class Redirect301SearchTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Settings.Redirect.csv"
            );

            Init();
            GoToAdmin("settingsseo#?seoTab=seo301");
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
        public void RedirectSearchNotexist()
        {
            //search not exist product
            Driver.GridFilterSendKeys("redirect000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void RedirectSearchMuch()
        {
            //search too much symbols
            Driver.GridFilterSendKeys("1111111111222222222223333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void RedirectSearchInvalid()
        {
            //search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void RedirectSearchExistFrom()
        {
            Driver.GridFilterSendKeys("redirect10");
            VerifyAreEqual("redirect10", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "find value");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void RedirectSearchExistTo()
        {
            Driver.GridFilterSendKeys("products/test-product81");
            VerifyAreEqual("redirect81", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "find value");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void RedirectSearchExistLogin()
        {
            Driver.GridFilterSendKeys("login");
            VerifyAreEqual("redirect99", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "find value");
            VerifyAreEqual("login", Driver.GetGridCellText(0, "RedirectTo", "301Red"), "find value");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }

    [TestFixture]
    public class Redirect301PageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Settings.Redirect.csv"
            );
            Init();
            GoToAdmin("settingsseo#?seoTab=seo301");
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
        public void PageRedirect()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1");
            VerifyAreEqual("redirect10", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect11", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 11");
            VerifyAreEqual("redirect20", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect21", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 21");
            VerifyAreEqual("redirect30", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect31", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 31");
            VerifyAreEqual("redirect40", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect41", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 41");
            VerifyAreEqual("redirect50", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 50");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect51", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 51");
            VerifyAreEqual("redirect60", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 60");
        }

        [Test]
        public void PageRedirectToBegin()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1");
            VerifyAreEqual("redirect10", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect11", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 11");
            VerifyAreEqual("redirect20", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect21", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 21");
            VerifyAreEqual("redirect30", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect31", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 31");
            VerifyAreEqual("redirect40", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect41", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 41");
            VerifyAreEqual("redirect50", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 50");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1");
            VerifyAreEqual("redirect10", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 10");
        }

        [Test]
        public void PageRedirectToEnd()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1");
            VerifyAreEqual("redirect10", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect101", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 101");
        }

        [Test]
        public void PageRedirectToNext()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridPaginationSelectItems("10");
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1");
            VerifyAreEqual("redirect10", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect11", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 11");
            VerifyAreEqual("redirect20", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect21", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 21");
            VerifyAreEqual("redirect30", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect31", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 31");
            VerifyAreEqual("redirect40", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect41", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 41");
            VerifyAreEqual("redirect50", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 50");
        }

        [Test]
        public void PagRedirectToPrevious()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1");
            VerifyAreEqual("redirect10", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect11", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 11");
            VerifyAreEqual("redirect20", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect21", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 21");
            VerifyAreEqual("redirect30", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect11", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 11");
            VerifyAreEqual("redirect20", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Driver.FindElement(By.TagName("ui-grid-custom-footer")).Click();
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1");
            VerifyAreEqual("redirect10", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 10");
        }

        [Test]
        public void RedirectPresent()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "find elem 101");
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1 10");
            VerifyAreEqual("redirect10", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 10");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1 100");
            VerifyAreEqual("redirect100", Driver.GetGridCellText(99, "RedirectFrom", "301Red"), "line 100");

            Driver.GridPaginationSelectItems("10");
        }
    }

    [TestFixture]
    public class Redirect301FilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Settings.Redirect.csv"
            );

            Init();
            GoToAdmin("settingsseo#?seoTab=seo301");
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
        public void RedirectFilterFrom()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Functions.GridFilterSet(Driver, BaseUrl, "RedirectFrom");

            //search by not exist 
            Driver.SetGridFilterValue("RedirectFrom", "qwe");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("RedirectFrom", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("RedirectFrom", "redirect98");
            Driver.XPathContainsText("h2", "301 Редиректы");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter RedirectFrom count");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter RedirectFrom row");
            VerifyAreEqual("redirect98", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1 ");
            Functions.GridFilterClose(Driver, BaseUrl, "RedirectFrom");
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1 ex ");
            VerifyAreEqual("redirect10", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 10 ex ");
        }

        [Test]
        public void RedirectFilterProduct()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Functions.GridFilterSet(Driver, BaseUrl, "ProductArtNo");

            //search by not exist 
            Driver.SetGridFilterValue("ProductArtNo", "50000");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("ProductArtNo", "111111111122222222222222222222222222222");

            //search by exist
            GoToAdmin("settingsseo#?seoTab=seo301");
            Functions.GridFilterSet(Driver, BaseUrl, "ProductArtNo");
            Driver.SetGridFilterValue("ProductArtNo", "10");

            Driver.XPathContainsText("h2", "301 Редиректы");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ProductArtNo count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter ProductArtNo 2 row");
            VerifyAreEqual("products/test-product10", Driver.GetGridCellText(0, "RedirectTo", "301Red"),
                "ProductArtNo");

            Functions.GridFilterClose(Driver, BaseUrl, "ProductArtNo");
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1 ex ");
            VerifyAreEqual("redirect10", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 10 ex ");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ProductArtNo return");
        }

        [Test]
        public void RedirectFilterTo()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Functions.GridFilterSet(Driver, BaseUrl, "RedirectTo");

            //search by not exist 
            Driver.SetGridFilterValue("RedirectTo", "qwew");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("RedirectTo", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("RedirectTo", "products/test-product8");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 5,
                "filter RedirectTo row");
            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter RedirectTo count");
            VerifyAreEqual("products/test-product8", Driver.GetGridCellText(0, "RedirectTo", "301Red"), "RedirectTo");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "301Red").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter RedirectTo return");
            VerifyIsTrue(Driver
                .FindElement(
                    By.CssSelector("[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"RedirectTo\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            Driver.WaitForElem(By.ClassName("ui-grid-empty-text"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".ui-grid-custom-selection-info")).Displayed,
                "count 0 selected");

            Functions.GridFilterClose(Driver, BaseUrl, "RedirectTo");
            VerifyAreEqual("redirect1", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "line 1 ex ");
            VerifyAreEqual("redirect11", Driver.GetGridCellText(9, "RedirectFrom", "301Red"), "line 10 ex ");
            VerifyAreEqual("Найдено записей: 96",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter RedirectTo return");

            Driver.GridFilterSendKeys("test-product8", By.TagName("h2"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");
        }
    }

    [TestFixture]
    public class Redirect301AddTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Redirect\\Settings.Redirect.csv"
            );
            Init();
            GoToAdmin("settingsseo#?seoTab=seo301");
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
        public void RedirectAdd()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddRedirect\"]")).Click();
            VerifyAreEqual("Добавить запись", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectFrom\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectFrom pop up");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectTo pop up");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "ProductArtNo pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectFrom\"] input")).SendKeys("addNewRedirect");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).SendKeys("products/test-product3");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"] input")).SendKeys("");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectSave\"]")).Click();

            GoToClient("addnewredirect");
            VerifyIsTrue(Driver.Url.EndsWith("products/test-product3"), "new url");
            VerifyAreEqual("TestProduct3", Driver.FindElement(By.TagName("h1")).Text, "h1");
        }

        [Test]
        public void RedirectAddProduct()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddRedirect\"]")).Click();

            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectFrom\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectFrom pop up");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectTo pop up");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "ProductArtNo pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectFrom\"] input")).SendKeys("addnewredirecttoproduct");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).SendKeys("catalog");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"] input")).SendKeys("13");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectSave\"]")).Click();

            GoToClient("addnewredirecttoproduct");
            VerifyIsTrue(Driver.Url.EndsWith("products/test-product13"), "new url");
            VerifyAreEqual("TestProduct13", Driver.FindElement(By.TagName("h1")).Text, "h1");
        }

        [Test]
        public void RedirectEditFrom()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridFilterSendKeys("redirect8");

            VerifyAreEqual("redirect8", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "RedirectFrom");
            VerifyAreEqual("products/test-product8", Driver.GetGridCellText(0, "RedirectTo", "301Red"), "RedirectTo");
            VerifyAreEqual("8", Driver.GetGridCellText(0, "ProductArtNo", "301Red"), "ProductArtNo");
            Driver.GetGridCell(0, "_serviceColumn", "301Red").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("redirect8",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectFrom\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectFrom pop up");
            VerifyAreEqual("products/test-product8",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectTo pop up");
            VerifyAreEqual("8",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "ProductArtNo pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectFrom\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectFrom\"] input")).SendKeys("newredirectfrom");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input"))
                .SendKeys("products/test-product5?tab=tabOptions");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"] input")).SendKeys("");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectSave\"]")).Click();

            GoToClient("redirect8");
            VerifyIsTrue(Driver.Url.EndsWith("redirect8"), "old url");
            VerifyIsTrue(Driver.PageSource.Contains("404"), "404 page");
            VerifyIsTrue(Driver.PageSource.Contains("Страница была удалена или перемещена"), "404 page2");
            VerifyIsTrue(Is404Page("/redirect8"), "404 page 1");

            GoToClient("newredirectfrom");
            VerifyIsTrue(Driver.Url.ToLower().EndsWith("products/test-product5?tab=tabOptions".ToLower()), "new url");
            VerifyAreEqual("TestProduct5", Driver.FindElement(By.TagName("h1")).Text, "h1");
        }

        [Test]
        public void RedirectEditTo()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridFilterSendKeys("redirect6");

            VerifyAreEqual("redirect6", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "RedirectFrom");
            VerifyAreEqual("products/test-product6", Driver.GetGridCellText(0, "RedirectTo", "301Red"), "RedirectTo");
            VerifyAreEqual("6", Driver.GetGridCellText(0, "ProductArtNo", "301Red"), "ProductArtNo");
            Driver.GetGridCell(0, "_serviceColumn", "301Red").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("redirect6",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectFrom\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectFrom pop up");
            VerifyAreEqual("products/test-product6",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectTo pop up");
            VerifyAreEqual("6",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "ProductArtNo pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).SendKeys("products/test-product1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"] input")).SendKeys("1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectSave\"]")).Click();

            GoToClient("redirect6");
            VerifyIsTrue(Driver.Url.EndsWith("products/test-product1"), "new url");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1");
        }

        [Test]
        public void RedirectEditToArtProduct()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridFilterSendKeys("redirect11");

            VerifyAreEqual("redirect11", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "RedirectFrom");
            VerifyAreEqual("products/test-product11", Driver.GetGridCellText(0, "RedirectTo", "301Red"), "RedirectTo");
            VerifyAreEqual("11", Driver.GetGridCellText(0, "ProductArtNo", "301Red"), "ProductArtNo");
            Driver.GetGridCell(0, "_serviceColumn", "301Red").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            VerifyAreEqual("redirect11",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectFrom\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectFrom pop up");
            VerifyAreEqual("products/test-product11",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectTo pop up");
            VerifyAreEqual("11",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "ProductArtNo pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).SendKeys("");
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"] input")).SendKeys("2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectSave\"]")).Click();

            GoToClient("redirect11");
            VerifyIsTrue(Driver.Url.EndsWith("products/test-product2"), "new url");
            VerifyAreEqual("TestProduct2", Driver.FindElement(By.TagName("h1")).Text, "h1");
        }

        [Test]
        public void RedirectEditCancel()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridFilterSendKeys("redirect5");

            VerifyAreEqual("redirect5", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "RedirectFrom");
            VerifyAreEqual("products/test-product5", Driver.GetGridCellText(0, "RedirectTo", "301Red"), "RedirectTo");
            VerifyAreEqual("5", Driver.GetGridCellText(0, "ProductArtNo", "301Red"), "ProductArtNo");
            Driver.GetGridCell(0, "_serviceColumn", "301Red").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));
            VerifyAreEqual("Редактировать запись", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            VerifyAreEqual("redirect5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectFrom\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectFrom pop up");
            VerifyAreEqual("products/test-product5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectTo pop up");
            VerifyAreEqual("5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "ProductArtNo pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).SendKeys("products/test-product6");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectCancel\"]")).Click();

            GoToClient("redirect5");
            VerifyIsTrue(Driver.Url.EndsWith("products/test-product5"), "new url");
            VerifyAreEqual("TestProduct5", Driver.FindElement(By.TagName("h1")).Text, "h1");
        }

        [Test]
        public void RedirectDel()
        {
            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridFilterSendKeys("redirect25");

            VerifyAreEqual("redirect25", Driver.GetGridCellText(0, "RedirectFrom", "301Red"), "RedirectFrom");
            VerifyAreEqual("products/test-product25", Driver.GetGridCellText(0, "RedirectTo", "301Red"), "RedirectTo");
            VerifyAreEqual("25", Driver.GetGridCellText(0, "ProductArtNo", "301Red"), "ProductArtNo");

            Driver.GetGridCell(0, "_serviceColumn", "301Red").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();

            GoToClient("redirect25");
            VerifyIsTrue(Driver.Url.EndsWith("redirect25"), "old url");
            VerifyIsTrue(Driver.PageSource.Contains("404"), "404 page");
            VerifyIsTrue(Driver.PageSource.Contains("Страница была удалена или перемещена"), "404 page2");
            VerifyIsTrue(Is404Page("/redirect25"), "404 page 1");
        }
    }
}