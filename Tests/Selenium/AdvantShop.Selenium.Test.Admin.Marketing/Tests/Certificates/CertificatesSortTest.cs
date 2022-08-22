using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Certificates
{
    [TestFixture]
    public class CertificatesSortTest : BaseSeleniumTest
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
                "Data\\Admin\\Certificates\\[Order].OrderSource.csv",
                "data\\Admin\\Certificates\\[Order].OrderStatus.csv",
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
        public void CertificatesSortCode()
        {
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "grid CertificateCode");
            VerifyAreEqual("6", Driver.GetGridCell(0, "OrderId", "Certificates").Text, "grid OrderId");
            VerifyAreEqual("7", Driver.GetGridCell(0, "ApplyOrderNumber", "Certificates").Text,
                "grid ApplyOrderNumber");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "FullSum", "Certificates").Text, "grid FullSum");
            VerifyIsTrue(
                Driver.GetGridCell(0, "OrderCertificatePaid", "Certificates").FindElement(By.CssSelector("input"))
                    .Selected, "grid Paid");
            VerifyIsTrue(Driver.GetGridCell(0, "Enable", "Certificates").FindElement(By.CssSelector("input")).Selected,
                "grid Enable");
            VerifyIsTrue(Driver.GetGridCell(0, "Used", "Certificates").FindElement(By.CssSelector(" input")).Selected,
                "grid Used");
            VerifyAreEqual("05.04.2013 17:16", Driver.GetGridCell(0, "CreationDates", "Certificates").Text,
                "grid CreationDates");

            Driver.GetGridCell(-1, "CertificateCode", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort CertificateCode ASC  value1");
            VerifyAreEqual("Certificate14", Driver.GetGridCell(5, "CertificateCode", "Certificates").Text,
                "sort CertificateCode DESC  value5");
            VerifyAreEqual("Certificate18", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort CertificateCode DESC  value9");

            Driver.GetGridCell(-1, "CertificateCode", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate9", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort CertificateCode ASC  value1");
            VerifyAreEqual("Certificate4", Driver.GetGridCell(5, "CertificateCode", "Certificates").Text,
                "sort CertificateCode DESC  value5");
            VerifyAreEqual("Certificate19", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort CertificateCode DESC  value9");
        }

        [Test]
        public void CertificatesSortOrder()
        {
            Driver.GetGridCell(-1, "OrderId", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate2", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort OrderId ASC  value1");
            VerifyAreEqual("Certificate6", Driver.GetGridCell(4, "CertificateCode", "Certificates").Text,
                "sort OrderId ASC  value4");
            VerifyAreEqual("Certificate11", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort OrderId DESC  value9");

            Driver.GetGridCell(-1, "OrderId", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort OrderId ASC  value1");
            VerifyAreEqual("Certificate17", Driver.GetGridCell(4, "CertificateCode", "Certificates").Text,
                "sort OrderId ASC  value4");
            VerifyAreEqual("Certificate12", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort OrderId DESC  value9");
        }

        [Test]
        public void CertificatesSortApplyOrder()
        {
            Driver.GetGridCell(-1, "ApplyOrderNumber", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate2", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort ApplyOrderNumber ASC  value1");
            VerifyAreEqual("Certificate6", Driver.GetGridCell(4, "CertificateCode", "Certificates").Text,
                "sort ApplyOrderNumber DESC  value4");
            VerifyAreEqual("Certificate11", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort ApplyOrderNumber DESC  value9");

            Driver.GetGridCell(-1, "ApplyOrderNumber", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort ApplyOrderNumber ASC  value1");
            VerifyAreEqual("Certificate5", Driver.GetGridCell(4, "CertificateCode", "Certificates").Text,
                "sort ApplyOrderNumber DESC  value4");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort ApplyOrderNumber DESC  value9");
        }

        [Test]
        public void CertificatesSortSum()
        {
            Driver.GetGridCell(-1, "FullSum", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort FullSum ASC  value1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort FullSum DESC  value9");

            Driver.GetGridCell(-1, "FullSum", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate20", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort FullSum ASC  value1");
            VerifyAreEqual("Certificate11", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort FullSum DESC  value9");
        }

        [Test]
        public void CertificatesSortPaid()
        {
            Driver.GetGridCell(-1, "OrderCertificatePaid", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate2", Driver.GetGridCell(0,  "CertificateCode", "Certificates").Text, 
               "sort Paid ASC  value1");
            VerifyIsFalse(
                Driver.GetGridCell(0, "OrderCertificatePaid", "Certificates").FindElement(By.CssSelector("input"))
                    .Selected, "grid Paid");
            VerifyAreEqual("Certificate7", Driver.GetGridCell(5,  "CertificateCode", "Certificates").Text, 
               "sort Paid DESC  value5");
            VerifyIsFalse(
                Driver.GetGridCell(5, "OrderCertificatePaid", "Certificates").FindElement(By.CssSelector("input"))
                    .Selected, "grid Paid");

            Driver.GetGridCell(-1, "OrderCertificatePaid", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0,  "CertificateCode", "Certificates").Text, 
               "sort Paid ASC  value1");
            VerifyIsTrue(
                Driver.GetGridCell(0, "OrderCertificatePaid", "Certificates").FindElement(By.CssSelector("input"))
                    .Selected, "grid Paid");
            VerifyAreEqual("Certificate6", Driver.GetGridCell(5,  "CertificateCode", "Certificates").Text, 
               "sort Paid DESC  value5");
            VerifyIsFalse(
                Driver.GetGridCell(5, "OrderCertificatePaid", "Certificates").FindElement(By.CssSelector("input"))
                    .Selected, "grid Paid");
        }

        [Test]
        public void CertificatesSortEnable()
        {
            Driver.GetGridCell(-1, "Enable", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate11", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort Enable ASC  value1");
            VerifyAreEqual("Certificate20", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort Enable DESC  value9");

            Driver.GetGridCell(-1, "Enable", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort Enable ASC  value1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort Enable DESC  value9");
        }

        [Test]
        public void CertificatesSortUsed()
        {
            Driver.GetGridCell(-1, "Used", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate2", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort Used ASC value1");
            VerifyAreEqual("Certificate11", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort Used DESC  value9");

            Driver.GetGridCell(-1, "Used", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort Used ASC  value1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort Used DESC  value9");
        }

        [Test]
        public void CertificatesSortCreate()
        {
            Driver.GetGridCell(-1, "CreationDates", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate20", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort CreationDates ASC  value1");
            VerifyAreEqual("Certificate11", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort CreationDates DESC  value9");

            Driver.GetGridCell(-1, "CreationDates", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "sort CreationDates ASC  value1");
            VerifyAreEqual("Certificate10", Driver.GetGridCell(9, "CertificateCode", "Certificates").Text,
                "sort CreationDates DESC  value9");
            //back default
            Driver.GetGridCell(-1, "FullSum", "Certificates").Click();
            Driver.WaitForElem(AdvBy.DataE2E("gridCell"));
        }
    }
}