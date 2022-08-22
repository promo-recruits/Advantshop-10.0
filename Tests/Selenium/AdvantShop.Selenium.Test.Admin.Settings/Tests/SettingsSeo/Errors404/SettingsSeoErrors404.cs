using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSeo.Errors404
{
    [TestFixture]
    public class SettingsSeoErrors404 : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Redirect.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Error404.csv"
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
        public void ErrorGrid()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");

            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "Url");
            VerifyAreEqual("http://localhost:8821/1", Driver.GetGridCell(0, "UrlReferer", "ErrorLog404").Text, "ref");
            VerifyAreEqual("products/test-product2", Driver.GetGridCell(0, "RedirectTo", "ErrorLog404").Text,
                "redirect");
            VerifyAreEqual("27.10.2017 12:30", Driver.GetGridCell(0, "DateAddedFormatted", "ErrorLog404").Text, "date");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            GoToAdmin("settingsseo#?seoTab=seo301");
            VerifyAreEqual("error", Driver.GetGridCell(0, "RedirectFrom", "301Red").Text, "RedirectFrom");
            VerifyAreEqual("products/test-product2", Driver.GetGridCell(0, "RedirectTo", "301Red").Text, "RedirectTo");
            VerifyAreEqual("2", Driver.GetGridCell(0, "ProductArtNo", "301Red").Text, "ProductArtNo");


            GoToClient("error");
            VerifyIsTrue(Driver.Url.EndsWith("products/test-product2"), "new url");
            VerifyAreEqual("TestProduct2", Driver.FindElement(By.TagName("h1")).Text, "h1");
        }

        [Test]
        public void ErrorzSelectDelete()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");
            //check delete by search 
            Driver.GridFilterSendKeys("error21");
            VerifyAreEqual("error21", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "find value");
            Driver.GetGridCell(0, "_serviceColumn", "ErrorLog404")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no find value");
            Driver.GridFilterSendKeys("");

            GoToAdmin("settingsseo#?seoTab=seo404");
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "ErrorLog404")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "ErrorLog404")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("error2", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "ErrorLog404")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "ErrorLog404")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "ErrorLog404")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "ErrorLog404")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "ErrorLog404")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "ErrorLog404")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyAreEqual("3",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text, "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl, "gridErrorLog404");
            VerifyAreEqual("error5", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "selected 1 grid delete");
            VerifyAreEqual("error6", Driver.GetGridCell(1, "Url", "ErrorLog404").Text, "selected 2 grid delete");
            VerifyAreEqual("error7", Driver.GetGridCell(2, "Url", "ErrorLog404").Text, "selected 3 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "ErrorLog404")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "ErrorLog404")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl, "gridErrorLog404");
            VerifyAreEqual("error15", Driver.GetGridCell(0, "Url", "ErrorLog404").Text,
                "selected all on page 1 grid delete");
            VerifyAreEqual("error25", Driver.GetGridCell(9, "Url", "ErrorLog404").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("86",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "ErrorLog404")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "ErrorLog404")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl, "gridErrorLog404");

            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");

            GoToAdmin("settingsseo#?seoTab=seo404");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 2");
        }
    }

    [TestFixture]
    public class SettingsSeoErrors404SortTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Redirect.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Error404.csv"
            );
            Init();
            GoToAdmin("settingsseo#?seoTab=seo404");
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
        public void ErrorSortFrom()
        {
            Driver.GetGridCell(-1, "Url", "ErrorLog404").Click();
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "sort Url 1 asc");
            VerifyAreEqual("error16", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "sort Url 10 asc");

            Driver.GetGridCell(-1, "Url", "ErrorLog404").Click();
            VerifyAreEqual("error99", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "sort Url 1 desc");
            VerifyAreEqual("error90", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "sort Url 10 desc");
        }

        [Test]
        public void ErrorSortTo()
        {
            Driver.GetGridCell(-1, "RedirectTo", "ErrorLog404").Click();
            VerifyAreEqual("error12", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "sort RedirectTo 1 asc");
            VerifyAreEqual("error21", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "sort RedirectTo 10 asc");

            Driver.GetGridCell(-1, "RedirectTo", "ErrorLog404").Click();
            VerifyAreEqual("error8", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "sort RedirectTo 1 desc");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "sort RedirectTo 10 desc");
        }

        [Test]
        public void ErrorSortReferer()
        {
            Driver.GetGridCell(-1, "UrlReferer", "ErrorLog404").Click();
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "sort UrlReferer 1 asc");
            VerifyAreEqual("error16", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "sort UrlReferer 10 asc");

            Driver.GetGridCell(-1, "UrlReferer", "ErrorLog404").Click();
            VerifyAreEqual("error99", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "sort UrlReferer 1 desc");
            VerifyAreEqual("error90", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "sort UrlReferer 10 desc");
        }

        [Test]
        public void ErrorSortDate()
        {
            Driver.GetGridCell(-1, "DateAddedFormatted", "ErrorLog404").Click();
            VerifyAreEqual("error101", Driver.GetGridCell(0, "Url", "ErrorLog404").Text,
                "sort DateAddedFormatted 1 asc");
            VerifyAreEqual("error92", Driver.GetGridCell(9, "Url", "ErrorLog404").Text,
                "sort DateAddedFormatted 10 asc");

            Driver.GetGridCell(-1, "DateAddedFormatted", "ErrorLog404").Click();
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "sort DateAddedFormatted 1 desc");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text,
                "sort DateAddedFormatted 10 desc");
        }
    }

    [TestFixture]
    public class SettingsSeoErrors404SearchTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Redirect.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Error404.csv"
            );
            Init();
            GoToAdmin("settingsseo#?seoTab=seo404");
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
        public void ErrorSearchNotexist()
        {
            //search not exist product
            Driver.GridFilterSendKeys("error000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void ErrorSearchMuch()
        {
            //search too much symbols
            Driver.GridFilterSendKeys("1111111111222222222223333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void ErrorSearchInvalid()
        {
            //search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void ErrorSearchExistFrom()
        {
            Driver.GridFilterSendKeys("error10");
            VerifyAreEqual("error10", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "find value");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void ErrorSearchExistLogin()
        {
            Driver.GridFilterSendKeys("8821/101");
            VerifyAreEqual("error101", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "find value");
            VerifyAreEqual("http://localhost:8821/101", Driver.GetGridCell(0, "UrlReferer", "ErrorLog404").Text,
                "find  UrlReferer value");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }

    [TestFixture]
    public class SettingsSeoErrors404PageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Redirect.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Error404.csv"
            );
            Init();
            GoToAdmin("settingsseo#?seoTab=seo404");
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
        public void PageError()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");

            (new SelectElement(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]")))).SelectByText("10");
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error11", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 11");
            VerifyAreEqual("error20", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error21", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 21");
            VerifyAreEqual("error30", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error31", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 31");
            VerifyAreEqual("error40", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error41", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 41");
            VerifyAreEqual("error50", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 50");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error51", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 51");
            VerifyAreEqual("error60", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 60");
        }

        [Test]
        public void PageErrorToBegin()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");
            (new SelectElement(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]")))).SelectByText("10");
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error11", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 11");
            VerifyAreEqual("error20", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error21", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 21");
            VerifyAreEqual("error30", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error31", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 31");
            VerifyAreEqual("error40", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error41", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 41");
            VerifyAreEqual("error50", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 50");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 10");
        }

        [Test]
        public void PageErrorToEnd()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");
            (new SelectElement(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]")))).SelectByText("10");
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error101", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 101");
        }

        [Test]
        public void PageErrorToNext()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");
            (new SelectElement(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]")))).SelectByText("10");
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error11", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 11");
            VerifyAreEqual("error20", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error21", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 21");
            VerifyAreEqual("error30", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error31", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 31");
            VerifyAreEqual("error40", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error41", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 41");
            VerifyAreEqual("error50", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 50");
        }

        [Test]
        public void PageErrorToPrevious()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");
            (new SelectElement(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]")))).SelectByText("10");
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error11", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 11");
            VerifyAreEqual("error20", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error21", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 21");
            VerifyAreEqual("error30", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error11", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 11");
            VerifyAreEqual("error20", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 10");
        }

        [Test]
        public void PresentError()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");
            (new SelectElement(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]")))).SelectByText("10");
            Thread.Sleep(2000);
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "find elem 101");
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1 10");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 10");

            (new SelectElement(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]")))).SelectByText("20");
            Thread.Sleep(2000);
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1 20");
            VerifyAreEqual("error20", Driver.GetGridCell(19, "Url", "ErrorLog404").Text, "line 20");

            (new SelectElement(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]")))).SelectByText("50");
            Thread.Sleep(2000);
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1 50 ");
            VerifyAreEqual("error50", Driver.GetGridCell(49, "Url", "ErrorLog404").Text, "line 50");

            (new SelectElement(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]")))).SelectByText("100");
            Thread.Sleep(2000);
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1 100");
            VerifyAreEqual("error100", Driver.GetGridCell(99, "Url", "ErrorLog404").Text, "line 100");
        }
    }

    [TestFixture]
    public class SettingsSeoErrors404FilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Redirect.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Error404.csv"
            );
            Init();
            GoToAdmin("settingsseo#?seoTab=seo404");
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
        public void ErrorFilterFrom()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");
            Functions.GridFilterTabSet(Driver, BaseUrl, "Url", "gridErrorLog404");

            //search by not exist 
            Driver.SetGridFilterValue("Url", "qwe");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Url", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("Url", "error98");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter RedirectFrom count");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter RedirectFrom row");
            VerifyAreEqual("error98", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1 ");
            Functions.GridFilterTabClose(Driver, BaseUrl, "Url", "gridErrorLog404");
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1 ex ");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 10 ex ");
        }

        [Test]
        public void ErrorFilterReferer()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");
            Functions.GridFilterTabSet(Driver, BaseUrl, "UrlReferer", "gridErrorLog404");

            //search by not exist 
            Driver.SetGridFilterValue("UrlReferer", "50000");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("UrlReferer", "111111111122222222222222222222222222222");

            //search by exist
            GoToAdmin("settingsseo#?seoTab=seo404");
            Functions.GridFilterTabSet(Driver, BaseUrl, "UrlReferer", "gridErrorLog404");
            Driver.SetGridFilterValue("UrlReferer", "25");

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter UrlReferer count");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter UrlReferer 2 row");
            VerifyAreEqual("http://localhost:8821/25", Driver.GetGridCell(0, "UrlReferer", "ErrorLog404").Text,
                "UrlReferer");

            Functions.GridFilterTabClose(Driver, BaseUrl, "UrlReferer", "gridErrorLog404");
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1 ex ");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 10 ex ");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter UrlReferer return");
        }

        [Test]
        public void ErrorFilterTo()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");
            Functions.GridFilterTabSet(Driver, BaseUrl, "RedirectTo", "gridErrorLog404");

            //search by not exist 
            Driver.SetGridFilterValue("RedirectTo", "qwew");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("RedirectTo", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("RedirectTo", "products/test-product12");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter RedirectTo row");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter RedirectTo count");
            VerifyAreEqual("products/test-product12", Driver.GetGridCell(0, "RedirectTo", "ErrorLog404").Text,
                "RedirectTo");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "ErrorLog404").FindElement(By.TagName("ui-grid-custom-delete"))
                .Click();
            Driver.SwalConfirm();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".ui-grid-custom-selection-info")).Displayed, "count 0 selected");

            Functions.GridFilterTabClose(Driver, BaseUrl, "RedirectTo", "gridErrorLog404");
            VerifyAreEqual("error", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "line 1 ex ");
            VerifyAreEqual("error10", Driver.GetGridCell(9, "Url", "ErrorLog404").Text, "line 10 ex ");
            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter RedirectTo return");
            Driver.GridFilterSendKeys("error11");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "delete filtered items count");
        }
    }

    [TestFixture]
    public class SettingsSeoErrors404AddTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Redirect);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Catalog.Photo.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Redirect.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\404Error\\Settings.Error404.csv"
            );
            Init();
            GoToAdmin("settingsseo#?seoTab=seo404");
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
        public void ErrorAddRedirect()
        {
            GoToAdmin("settingsseo#?seoTab=seo404");
            Driver.GridFilterSendKeys("error101");
            VerifyAreEqual("error101", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "find value");
            Driver.GetGridCell(0, "RedirectTo", "ErrorLog404").FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Добавить запись", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            VerifyAreEqual("error101",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectFrom\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "errorFrom pop up");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "RedirectTo pop up");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectArtProduct\"]")).FindElement(By.TagName("input"))
                    .GetAttribute("value"), "UrlReferer pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectTo\"] input")).SendKeys("products/test-product1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"RedirectSave\"]")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("error101", Driver.GetGridCell(0, "Url", "ErrorLog404").Text, "Url");
            VerifyAreEqual("products/test-product1", Driver.GetGridCell(0, "RedirectTo", "ErrorLog404").Text,
                "redirect");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridErrorLog404\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            GoToAdmin("settingsseo#?seoTab=seo301");
            Driver.GridFilterSendKeys("error101");
            VerifyAreEqual("error101", Driver.GetGridCell(0, "RedirectFrom", "301Red").Text, "RedirectFrom");
            VerifyAreEqual("products/test-product1", Driver.GetGridCell(0, "RedirectTo", "301Red").Text, "RedirectTo");
            VerifyAreEqual("", Driver.GetGridCell(0, "ProductArtNo", "301Red").Text, "ProductArtNo");

            GoToClient("error101");
            VerifyIsTrue(Driver.Url.EndsWith("products/test-product1"), "new url");
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1");
        }
    }
}