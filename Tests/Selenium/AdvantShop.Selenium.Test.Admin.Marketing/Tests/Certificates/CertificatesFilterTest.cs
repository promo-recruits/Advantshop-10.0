using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Certificates
{
    [TestFixture]
    public class CertificatesFilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Certificates\\Catalog.Brand.csv",
                "data\\Admin\\Certificates\\Catalog.Product.csv",
                "data\\Admin\\Certificates\\Catalog.Photo.csv",
                "data\\Admin\\Certificates\\Catalog.Color.csv",
                "data\\Admin\\Certificates\\Catalog.Size.csv",
                "data\\Admin\\Certificates\\Catalog.Offer.csv",
                "data\\Admin\\Certificates\\Catalog.Category.csv",
                "data\\Admin\\Certificates\\Catalog.ProductCategories.csv",
                "data\\Admin\\Certificates\\[Order].OrderStatus.csv",
                "Data\\Admin\\Certificates\\[Order].OrderSource.csv",
                "data\\Admin\\Certificates\\[Order].[Order].csv",
                "data\\Admin\\Certificates\\[Order].Certificate.csv",
                "data\\Admin\\Certificates\\[Order].OrderContact.csv",
                "data\\Admin\\Certificates\\[Order].OrderCurrency.csv",
                "data\\Admin\\Certificates\\[Order].OrderItems.csv"
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
        public void CertificatesFilterCode()
        {
            //Код сертификата
            Functions.GridFilterSet(Driver, BaseUrl, "CertificateCode");

            //search by not exist 
            Driver.SetGridFilterValue("CertificateCode", "50000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("CertificateCode", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("CertificateCode", "Certificate2");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 2,
                "filter CertificateCode row");
            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CertificateCode count");
            VerifyAreEqual("Certificate2", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter CertificateCode value");

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Certificates").FindElement(By.TagName("ui-modal-trigger")).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Редактировать подарочный сертификат", Driver.FindElement(By.TagName("h2")).Text,
                "pop up h2");
            Driver.XPathContainsText("button", "Отмена");

            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CertificateCode return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"CertificateCode\"]")).Displayed);


            Functions.GridFilterClose(Driver, BaseUrl, "CertificateCode");
            VerifyAreEqual("Найдено записей: 20",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CertificateCode exit");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter CertificateCode exit");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "filter CertificateCode exit");
        }

        [Test]
        public void CertificatesFilterOrder()
        {
            //OrderId 
            Functions.GridFilterSet(Driver, BaseUrl, "OrderId");
            //search by not exist 
            Driver.SetGridFilterValue("OrderId", "50000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("OrderId", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("OrderId", "6");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter OrderId count");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter OrderId row");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter OrderId value");
            Functions.GridFilterClose(Driver, BaseUrl, "OrderId");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter OrderId exit 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "filter OrderId exit 10");
        }

        [Test]
        public void CertificatesFilterApplyOrder()
        {
            //ApplyOrderNumber
            Functions.GridFilterSet(Driver, BaseUrl, "ApplyOrderNumber");
            //search by not exist 
            Driver.SetGridFilterValue("ApplyOrderNumber", "50000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("ApplyOrderNumber", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("ApplyOrderNumber", "7");
            Driver.DropFocusCss("[data-e2e=\"CertificatesTitle\"]");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter ApplyOrderNumber count");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter ApplyOrderNumber row");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter ApplyOrderNumber value");
            Functions.GridFilterClose(Driver, BaseUrl, "ApplyOrderNumber");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter ApplyOrderNumber exit 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "filter ApplyOrderNumber exit 10");
        }

        [Test]
        public void CertificatesFilterSum()
        {
            //FullSum
            Functions.GridFilterSet(Driver, BaseUrl, "FullSum");
            //search by not exist 
            Driver.SetGridFilterValue("FullSum", "50000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("FullSum", "111111111122222222222222222222222222222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search by exist
            Driver.SetGridFilterValue("FullSum", "200");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter FullSum count");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter FullSum row");
            VerifyAreEqual("Certificate3", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter FullSum value");
            Driver.DropFocusCss("[data-e2e=\"CertificatesTitle\"]");
            Driver.SetGridFilterValue("FullSum", "3000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter FullSum no find");

            Functions.GridFilterClose(Driver, BaseUrl, "FullSum");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter FullSum exit 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "filter FullSum exit 10");
        }

        [Test]
        public void CertificatesFilterPaid()
        {
            //Paid
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "OrderCertificatePaid", filterItem: "Да");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Paid count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter Paid row");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter Paid value");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();
            Driver.DropFocusCss("[data-e2e=\"CertificatesTitle\"]");
            VerifyAreEqual("Найдено записей: 19",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Paid 2 count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 10,
                "filter Paid 2 row");
            VerifyAreEqual("Certificate2", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter Paid 2 value");
            Functions.GridFilterClose(Driver, BaseUrl, "OrderCertificatePaid");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter Paid exit 1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "filter Paid exit 10");
        }

        [Test]
        public void CertificatesFilterEnable()
        {
            //Enable
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Enable", filterItem: "Да");
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enable count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 10,
                "filter Enable row");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter Enable value");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();
            Driver.DropFocusCss("[data-e2e=\"CertificatesTitle\"]");
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Enable count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 10,
                "filter Enable 2 row");
            VerifyAreEqual("Certificate11", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter Enable 2 value");
            Functions.GridFilterClose(Driver, BaseUrl, "Enable");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter Enable exit 1 ");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "filter Enable exit 10");
        }

        [Test]
        public void CertificatesFilterCreate()
        {
            //CreationDates
            Functions.GridFilterSet(Driver, BaseUrl, "CreationDates");
            Driver.SetGridFilterRange("CreationDates", "01.01.2013 00:00", "20.03.2013 00:00");
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter CreationDates count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 3,
                "filter CreationDates row");
            VerifyAreEqual("Certificate18", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter CreationDates value");

            Functions.GridFilterClose(Driver, BaseUrl, "CreationDates");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter CreationDates exit 1 ");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "filter CreationDates exit 10");
        }

        [Test]
        public void CertificatesFilterUsed()
        {
            //Used
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "Used", filterItem: "Нет");
            VerifyAreEqual("Найдено записей: 19",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Used count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 10,
                "filter Used row");
            VerifyAreEqual("Certificate2", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter Used value");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Да\"]")).Click();
            Driver.DropFocusCss("[data-e2e=\"CertificatesTitle\"]");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter Used  2 count");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1,
                "filter Used 2 row");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter Used 2 value");
            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Used");
            VerifyAreEqual("Найдено записей: 19",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "no filter count");
            VerifyAreEqual("Certificate2", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "filter Used exit1");
            VerifyAreEqual("Certificate7", Driver.GetGridCell(5, "CertificateCode", "Certificates").Text,
                "filter Used exit2");
        }
    }
}