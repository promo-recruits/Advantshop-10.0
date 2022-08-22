using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Catalog.Tests.Product.Reviews
{
    [TestFixture]
    public class CatalogReviewsFilterAddDateCalendarTest : BaseSeleniumTest
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
        public void FilterAddDateCalendarMinMax()
        {
            GoToAdmin("reviews");
            Functions.GridFilterSet(Driver, BaseUrl, name: "AddDateFormatted");
            Functions.DataTimePickerFilter(Driver, BaseUrl, monthFrom: "Январь", yearFrom: "2013",
                dataFrom: "Январь 17, 2013", hourFrom: "13", minFrom: "30", monthTo: "Декабрь", yearTo: "2013",
                dataTo: "Декабрь 17, 2013", hourTo: "13", minTo: "30", dropFocusElem: "h1",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]", fieldTo: "[data-e2e=\"datetimeFilterTo\"]");
            VerifyAreEqual("Найдено записей: 210",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Test]
        public void FilterAddDateCalendarNotExistMin()
        {
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "AddDateFormatted");
            Functions.DataTimePickerFilter(Driver, BaseUrl, monthFrom: "Январь", yearFrom: "2009",
                dataFrom: "Январь 17, 2009", hourFrom: "13", minFrom: "30", dropFocusElem: "h1",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]", fieldTo: "[data-e2e=\"datetimeFilterTo\"]");
            VerifyAreEqual("Найдено записей: 300",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
        }

        [Test]
        public void FilterAddDateCalendarNotExistMax()
        {
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "AddDateFormatted");
            Functions.DataTimePickerFilter(Driver, BaseUrl, monthTo: "Декабрь", yearTo: "2009",
                dataTo: "Декабрь 17, 2009", hourTo: "13", minTo: "30", dropFocusElem: "h1",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]", fieldTo: "[data-e2e=\"datetimeFilterTo\"]");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }

        [Test]
        public void FilterAddDateCalendarNotExistMinMax()
        {
            GoToAdmin("reviews");

            Functions.GridFilterSet(Driver, BaseUrl, name: "AddDateFormatted");
            Functions.DataTimePickerFilter(Driver, BaseUrl, monthFrom: "Январь", yearFrom: "2009",
                dataFrom: "Январь 17, 2009", hourFrom: "13", minFrom: "30", monthTo: "Декабрь", yearTo: "2009",
                dataTo: "Декабрь 17, 2009", hourTo: "13", minTo: "30", dropFocusElem: "h1",
                fieldFrom: "[data-e2e=\"datetimeFilterFrom\"]", fieldTo: "[data-e2e=\"datetimeFilterTo\"]");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }
    }
}