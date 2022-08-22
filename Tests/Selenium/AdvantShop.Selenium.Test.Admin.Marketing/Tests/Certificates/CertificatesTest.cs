using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Certificates
{
    [TestFixture]
    public class CertificatesTest : BaseSeleniumTest
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
            GoToAdmin("settingscoupons#?couponsTab=certificates");
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
        public void CertificatesSearchNotexist()
        {
            //search not exist product
            Driver.GridFilterSendKeys("Certificate222");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        [Order(1)]
        public void CertificatesSearchMuch()
        {
            //search too much symbols
            Driver.GridFilterSendKeys(
                "1111111111222222222223333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww",
                By.ClassName("ui-grid-custom-filter-total"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        [Order(1)]
        public void CertificatesSearchInvalid()
        {
            //search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"),
                "search not exist discount price range");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        [Order(1)]
        public void SearchExist()
        {
            Driver.GridFilterSendKeys("Certificate2");
            VerifyAreEqual("Certificate2", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "find value");
            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        [Order(10)]
        public void CertificatesRedirect()
        {
            GoToAdmin("settingscoupons#?couponsTab=certificates");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text, "1 value");
            Driver.GetGridCell(0, "OrderId", "Certificates").Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Заказ № 6"), " go to orderId");

            GoToAdmin("settingscoupons#?couponsTab=certificates");
            Driver.GetGridCell(0, "ApplyOrderNumber", "Certificates").Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));
            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Заказ № 7"), " go to ApplyOrder");

            GoToAdmin("settingscoupons#?couponsTab=certificates");
            Driver.GetGridCell(0, "CertificateCode", "Certificates").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.TagName("h2")).Text.Contains("Редактировать подарочный сертификат"),
                " modal h2 ");
            VerifyAreEqual("Certificate1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CertCode\"]")).GetAttribute("value"), " modal cod");
            VerifyAreEqual("FromMe1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CertFrom\"]")).GetAttribute("value"), "modal from ");
            VerifyAreEqual("ToMe1", Driver.FindElement(By.CssSelector("[data-e2e=\"CertTo\"]")).GetAttribute("value"),
                " modal to");
            VerifyAreEqual("100", Driver.FindElement(By.CssSelector("[data-e2e=\"CertSum\"]")).GetAttribute("value"),
                " modal sum");
            VerifyAreEqual("me@gmail.com",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CertMailTo\"]")).GetAttribute("value"), " modal mail.");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"CertUsed\"]")).FindElement(By.CssSelector("input"))
                    .Selected, " modal used");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"CertEnabled\"]")).FindElement(By.CssSelector("input"))
                    .Selected, "modal enabled ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"CertPaid\"]")).FindElement(By.CssSelector("input"))
                    .Selected, " modal paid");
            Driver.AssertCkText("gift1", "editor1");
        }
    }
}