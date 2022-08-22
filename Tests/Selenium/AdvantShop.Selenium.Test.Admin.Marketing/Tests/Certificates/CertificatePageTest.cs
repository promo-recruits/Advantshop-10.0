using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Certificates
{
    [TestFixture]
    public class CertificatePageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Orders);
            InitializeService.LoadData(
                "data\\Admin\\Certificates\\ManyCertificates\\Catalog.Brand.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\Catalog.Product.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\Catalog.Photo.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\Catalog.Color.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\Catalog.Size.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\Catalog.Offer.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\Catalog.Category.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Certificates\\ManyCertificates\\[Order].OrderSource.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\[Order].OrderStatus.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\[Order].[Order].csv",
                "data\\Admin\\Certificates\\ManyCertificates\\[Order].Certificate.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\[Order].OrderContact.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\[Order].OrderCurrency.csv",
                "data\\Admin\\Certificates\\ManyCertificates\\[Order].OrderItems.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("settingscoupons#?couponsTab=certificates");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void PageCertificate()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("Certificate11", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 11");
            VerifyAreEqual("Certificate20", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 20");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("Certificate21", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 21");
            VerifyAreEqual("Certificate30", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 30");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("Certificate31", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 31");
            VerifyAreEqual("Certificate40", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 40");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Certificate41", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 41");
            VerifyAreEqual("Certificate50", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 50");

            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("Certificate51", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 51");
            VerifyAreEqual("Certificate60", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 60");
        }

        [Test]
        public void PageCertificateToBegin()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Certificate11", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 11");
            VerifyAreEqual("Certificate20", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 20");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Certificate21", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 21");
            VerifyAreEqual("Certificate30", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 30");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Certificate31", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 31");
            VerifyAreEqual("Certificate40", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 40");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Certificate41", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 41");
            VerifyAreEqual("Certificate50", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 50");

            //to begin
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 10");
        }

        [Test]
        public void PageCertificateToEnd()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("Certificate101", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 101");
        }

        [Test]
        public void PageCertificateToNext()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Certificate11", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 11");
            VerifyAreEqual("Certificate20", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 20");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Certificate21", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 21");
            VerifyAreEqual("Certificate30", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 30");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Certificate31", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 31");
            VerifyAreEqual("Certificate40", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 40");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Certificate41", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 41");
            VerifyAreEqual("Certificate50", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 50");
        }

        [Test]
        public void PageCertificateToPrevious()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Certificate11", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 11");
            VerifyAreEqual("Certificate20", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 20");

            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("Certificate21", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 21");
            VerifyAreEqual("Certificate30", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 30");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Certificate11", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 11");
            VerifyAreEqual("Certificate20", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 20");

            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 10");
        }

        [Test]
        public void CertificatePresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "find elem 101");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text, "line 10");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "line 1");
            VerifyAreEqual("Certificate100", Driver.GetGridCell(99, "CertificateCode", "Certificates").Text,
                "line 100");

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void SelectAndDelete()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            //check delete cancel
            Driver.GetGridCell(0, "_serviceColumn", "Certificates").FindElement(By.TagName("ui-grid-custom-delete"))
                .Click();
            Driver.SwalCancel();
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "cancel del 1 element");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Certificates").FindElement(By.TagName("ui-grid-custom-delete"))
                .Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                " del 1 element");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Certificates")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Certificates")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Certificates")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Certificates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "Selected 1");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Certificates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "Selected 2");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Certificates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "Selected 3");
            VerifyAreEqual("3", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                " count Selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreNotEqual("Certificate2", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "del 2 element");
            VerifyAreNotEqual("Certificate3", Driver.GetGridCell(1, "CertificateCode", "Certificates").Text,
                "del 3 element");
            VerifyAreNotEqual("Certificate4", Driver.GetGridCell(2, "CertificateCode", "Certificates").Text,
                "del 4 element");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Certificates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "Selected page 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Certificates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "Selected pege 10");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Certificate15", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "del Selected page 1");
            VerifyAreEqual("Certificate24", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "gel Selected page 1");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("87", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text,
                "after del on page");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Certificates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "Selected all 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Certificates")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "Selected all 10");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no element");

            Refresh();
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "after refresh");
        }
    }
}