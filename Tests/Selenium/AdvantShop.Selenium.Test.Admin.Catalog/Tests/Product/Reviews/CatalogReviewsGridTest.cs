using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsGridTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Catalog\\Reviews\\Catalog.Product.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.Offer.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.Category.csv",
                "data\\Admin\\Catalog\\Reviews\\Catalog.ProductCategories.csv",
                "data\\Admin\\Catalog\\Reviews\\Customers.CustomerGroup.csv",
                "data\\Admin\\Catalog\\Reviews\\Customers.Customer.csv",
                "data\\Admin\\Catalog\\Reviews\\CMS.Review.csv"
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
        [Order(1)]
        public void GridChecked()
        {
            Functions.AdminSettingsReviewsShowImgOn(Driver, BaseUrl);

            GoToAdmin("reviews");

            //check admin
            VerifyIsTrue(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName2"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("asd@asd.asd"));
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "ProductName").Text);
            VerifyAreEqual("26.07.2015 14:22", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);

            //check go to product
            Driver.GetGridCell(0, "ProductName").FindElement(By.TagName("a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Товар \"TestProduct1\"", Driver.FindElement(By.TagName("h1")).Text);

            //check client
            GoToClient("products/test-product1?tab=tabReviews");
            VerifyIsTrue(Driver.PageSource.Contains("CustomerName2"));
            VerifyIsTrue(Driver.FindElement(By.Id("tabReviews")).Text.Contains("Отзывы (49)"));
            VerifyIsTrue(Driver.PageSource.Contains("Текст отзыва 30"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".review-item")).FindElements(By.TagName("img")).Count == 0);
        }

        [Test]
        [Order(1)]
        public void GridNotChecked()
        {
            Functions.AdminSettingsReviewsModerateOn(Driver, BaseUrl);

            GoToAdmin("reviews");

            Driver.GridFilterSendKeys("Текст отзыва 200");

            //check admin
            VerifyIsTrue(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName2"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("asd@asd.asd"));
            VerifyAreEqual("Текст отзыва 200", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "ProductName").Text);
            VerifyAreEqual("04.09.2013 14:22", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);

            //check client
            GoToClient("products/test-product10?tab=tabReviews");
            VerifyAreEqual("Отзывы (99)", Driver.FindElement(By.Id("tabReviews")).Text);
            VerifyIsFalse(Driver.PageSource.Contains("Текст отзыва 200"));

            Functions.AdminSettingsReviewsModerateOff(Driver, BaseUrl);
            //check client
            GoToClient("products/test-product10?tab=tabReviews");
            VerifyAreEqual("Отзывы (100)", Driver.FindElement(By.Id("tabReviews")).Text);
            VerifyIsTrue(Driver.PageSource.Contains("Текст отзыва 200"));
        }

        [Test]
        [Order(1)]
        public void ReviewzEdit()
        {
            Functions.AdminSettingsReviewsModerateOff(Driver, BaseUrl);

            GoToAdmin("reviews");

            Driver.GridFilterSendKeys("Текст отзыва 33");
            VerifyAreEqual("Текст отзыва 33", Driver.GetGridCell(0, "Text").Text);

            VerifyAreEqual("Категории и товары", Driver.FindElement(By.TagName("h1")).Text);
            VerifyAreEqual("Отзывы о товарах",
                Driver.FindElement(By.CssSelector(".nav-tabs--indent-bottom .uib-tab.active a")).Text);


            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewName\"]")).SendKeys("Name111");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewEmail\"]")).SendKeys("mail@mail.ru");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewDateAdd\"]")).SendKeys("01.01.2001 01:01:01");
            Driver.SetCkText("Измененный текст отзыва", "ReviewText");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ReviewCheckedClick\"]")).Click();
            // driver.FindElement(By.CssSelector("input[type=\"file\"]")).Clear();
            AttachFile(By.CssSelector("input[type=\"file\"]"), GetPicturePath("icon.jpg"));

            Driver.FindElement(By.XPath("//button[contains(text(), 'Сохранить')]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("reviews");
            Driver.GridFilterSendKeys("Текст отзыва 33");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            Driver.GridFilterSendKeys("Измененный текст отзыва");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("Name111"));
            VerifyAreEqual("Измененный текст отзыва", Driver.GetGridCell(0, "Text").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "PhotoName").FindElement(By.TagName("img")).GetAttribute("src")
                .Contains("nophoto"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("mail@mail.ru"));
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "ProductName").Text);
            VerifyAreEqual("01.01.2001 01:01", Driver.GetGridCell(0, "AddDateFormatted").Text);
            VerifyIsFalse(Driver.GetGridCell(0, "Checked").FindElement(By.TagName("input")).Selected);

            //check client
            GoToClient("products/test-product1?tab=tabReviews");
            VerifyIsTrue(Driver.PageSource.Contains("Измененный текст отзыва"));
            VerifyIsTrue(Driver.PageSource.Contains("Name111"));
            VerifyIsFalse(Driver.PageSource.Contains("Текст отзыва 33"));
            VerifyIsTrue(Driver.PageSource.Contains("1 января 2001"));
        }

        [Test]
        [Order(1)]
        public void GridSearch()
        {
            GoToAdmin("reviews");

            //search by exist name
            Driver.GridFilterSendKeys("CustomerName3");

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("CustomerName3"));
            VerifyAreEqual("Найдено записей: 100",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);

            //search by not exist name
            Driver.GridFilterSendKeys("CustomerName10");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search by not review text
            Driver.GridFilterSendKeys("Текст отзыва 500");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search too much symbols
            Driver.GridFilterSendKeys(
                    "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Driver.Blur();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        [Order(1)]
        public void PresentSelectOnPage()
        {
            Functions.AdminSettingsReviewsModerateOff(Driver, BaseUrl);

            GoToAdmin("reviews");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            VerifyAreEqual("Найдено записей: 300",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 20", Driver.GetGridCell(9, "Text").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol").FindElement(By.TagName("input")).Selected);

            GoToAdmin("reviews");
            Driver.GridPaginationSelectItems("20");
            Thread.Sleep(1000);
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 10", Driver.GetGridCell(19, "Text").Text);
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol").FindElement(By.TagName("input")).Selected);
            VerifyIsTrue(Driver.GetGridCell(19, "selectionRowHeaderCol").FindElement(By.TagName("input")).Selected);
        }

        [Test]
        [Order(10)]
        public void Delete()
        {
            Functions.AdminSettingsReviewsModerateOff(Driver, BaseUrl);

            GoToAdmin("reviews");
            Driver.GridReturnDefaultView10(BaseUrl);

            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);

            //check delete
            Driver.GetGridCell(0, "_serviceColumn").FindElement(By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            GoToAdmin("reviews");
            VerifyAreNotEqual("Текст отзыва 30", Driver.GetGridCell(0, "Text").Text);

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreNotEqual("Текст отзыва 31", Driver.GetGridCell(0, "Text").Text);
            VerifyAreNotEqual("Текст отзыва 26", Driver.GetGridCell(1, "Text").Text);
            VerifyAreNotEqual("Текст отзыва 27", Driver.GetGridCell(2, "Text").Text);

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Текст отзыва 18", Driver.GetGridCell(0, "Text").Text);
            VerifyAreEqual("Текст отзыва 9", Driver.GetGridCell(9, "Text").Text);

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("286", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            GoToAdmin("reviews");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check client
            GoToClient("products/test-product1?tab=tabReviews");
            VerifyIsTrue(Driver.PageSource.Contains("Отзывы"));
            VerifyIsFalse(Driver.PageSource.Contains("CustomerName"));
            VerifyIsFalse(Driver.PageSource.Contains("Текст отзыва"));
            VerifyIsFalse(Driver.PageSource.Contains("1 января 2001"));
        }
    }
}