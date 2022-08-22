using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Marketing.Tests.Certificates
{
    [TestFixture]
    public class CertificateAddTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Certificates\\AddCertificate\\Catalog.Brand.csv",
                "data\\Admin\\Certificates\\AddCertificate\\Catalog.Product.csv",
                "data\\Admin\\Certificates\\AddCertificate\\Catalog.Photo.csv",
                "data\\Admin\\Certificates\\AddCertificate\\Catalog.Color.csv",
                "data\\Admin\\Certificates\\AddCertificate\\Catalog.Size.csv",
                "data\\Admin\\Certificates\\AddCertificate\\Catalog.Offer.csv",
                "data\\Admin\\Certificates\\AddCertificate\\Catalog.Category.csv",
                "data\\Admin\\Certificates\\AddCertificate\\Catalog.ProductCategories.csv",
                "data\\Admin\\Certificates\\AddCertificate\\[Order].[Order].csv",
                "data\\Admin\\Certificates\\AddCertificate\\[Order].Certificate.csv"
            );
            Init();
            GoToAdmin("settingscheckout#?checkoutTab=checkoutfields");
            Functions.CheckNotSelected("IsRequiredZip", Driver);
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsCheckoutSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }
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
        public void AddCertificate()
        {
            GoToAdmin("settingscoupons#?couponsTab=certificates");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCertificates\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Добавить подарочный сертификат", Driver.FindElement(By.TagName("h2")).Text, "modal h2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CertCode\"]")).SendKeys("New Certificate");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertFrom\"]")).SendKeys("New From me");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertTo\"]")).SendKeys("New To Me");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertSum\"]")).SendKeys("100000");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertMailTo\"]")).SendKeys("testTo@mail.ru");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertMailFrom\"]")).SendKeys("testFrom@mail.ru");

            Driver.SetCkText("It is a new certificate!", "editor1");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"CertPayment\"]")))).SelectByText(
                "При получении (наличными или банковской картой)");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CertSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check grid
            VerifyAreEqual("New Certificate", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "grid CertificateCode");
            VerifyAreNotEqual("", Driver.GetGridCell(0, "OrderId", "Certificates").Text, "grid OrderId");
            VerifyAreEqual("", Driver.GetGridCell(0, "ApplyOrderNumber", "Certificates").Text, "grid ApplyOrderNumber");
            VerifyAreEqual("100 000 руб.", Driver.GetGridCell(0, "FullSum", "Certificates").Text, "grid FullSum");
            VerifyIsFalse(
                Driver.GetGridCell(0, "OrderCertificatePaid", "Certificates").FindElement(By.CssSelector("input"))
                    .Selected, "grid Paid");
            VerifyIsTrue(Driver.GetGridCell(0, "Enable", "Certificates").FindElement(By.CssSelector("input")).Selected,
                "grid Enable");
            VerifyIsFalse(Driver.GetGridCell(0, "Used", "Certificates").FindElement(By.CssSelector(" input")).Selected,
                "grid Used");
            VerifyIsTrue(
                Driver.GetGridCell(0, "CreationDates", "Certificates").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "grid CreationDates");
            
            //check grid orders
            string orderId = Driver.GetGridCell(0, "OrderId", "Certificates").Text;
            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");
            VerifyAreEqual(orderId, Driver.GetGridCell(0, "Number").Text, " Grid orders Number");
            VerifyAreEqual("100 000 руб.", Driver.GetGridCell(0, "SumFormatted").Text, " Grid orders Sum");
            VerifyIsTrue(
                Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "grid:" + Driver.GetGridCell(0, "OrderDateFormatted").Text + "orders CreationDates: " +
                (DateTime.Today.ToString("dd.MM.yyyy")));
            
            //check order
            Driver.GetGridCell(0, "Number").Click();
            Driver.WaitForElem(By.Id("Order_OrderDate"));
            VerifyAreEqual("Сертификат", Driver.GetGridCell(0, "CustomName", "OrderCertificates").Text,
                " Grid in order CustomName");
            VerifyAreEqual("New Certificate", Driver.GetGridCell(0, "CertificateCode", "OrderCertificates").Text,
                " Grid in order CertificateCode");
            VerifyAreEqual("100000", Driver.GetGridCell(0, "Sum", "OrderCertificates").Text, " Grid in order Sum");
            VerifyAreEqual("", Driver.GetGridCell(0, "Price", "OrderCertificates").Text, " Grid in order Price");
            VerifyAreEqual("100 000 руб.", Driver.FindElement(AdvBy.DataE2E("OrderSum")).Text, "Sum in Order");
            //pay
            Driver.FindElement(By.CssSelector(".switcher-state-label span")).Click();
            Driver.WaitForToastSuccess();
            //check grid cert
            GoToAdmin("settingscoupons#?couponsTab=certificates");
            VerifyAreEqual("New Certificate", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "grid CertificateCode pay");
            VerifyIsTrue(
                Driver.GetGridCell(0, "OrderCertificatePaid", "Certificates").FindElement(By.CssSelector("input"))
                    .Selected, "grid Paid pay");
            VerifyIsTrue(Driver.GetGridCell(0, "Enable", "Certificates").FindElement(By.CssSelector("input")).Selected,
                "grid Enable pay");
            VerifyIsFalse(Driver.GetGridCell(0, "Used", "Certificates").FindElement(By.CssSelector(" input")).Selected,
                "grid Used pay");
            //cheking client
            ProductToCard("1");
            GoToClient("cart");
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client discount");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client result price");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("New Certificate");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForElem(By.ClassName("cart-full-summary-price"));

            VerifyAreEqual("Сертификат:", Driver.FindElements(By.CssSelector(".cart-full-summary-name"))[1].Text,
                "client coupon");
            VerifyAreEqual("100 000 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[0].Text,
                "client sum coupon");
            VerifyAreEqual("0 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client rezult");

            Driver.FindElement(By.CssSelector(".spinbox-more")).Click();
            VerifyAreEqual("Сертификат:", Driver.FindElements(By.CssSelector(".cart-full-summary-name"))[1].Text,
                "client 2 coupon");
            VerifyAreEqual("100 000 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[0].Text,
                "client sum discount");
            VerifyAreEqual("0 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client rezult sum");

            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.WaitForElem(By.CssSelector(".breads"));
            Driver.WaitForElem(By.Id("rightCell"));

            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-result")).Text.Contains("0 руб."),
                "checkout rezult");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".checkout-success-content"));
            //check used certificates
            GoToAdmin("settingscoupons#?couponsTab=certificates");
            Driver.GridFilterSendKeys("New Certificate");
            Driver.DropFocusCss("[data-e2e=\"CertificatesTitle\"]");
            VerifyAreEqual("New Certificate", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "grid CertificateCode used");
            VerifyAreEqual(orderId, Driver.GetGridCell(0, "OrderId", "Certificates").Text, "grid OrderId used");
            int applayId = Convert.ToInt32(orderId) + 1;
            VerifyAreEqual(applayId.ToString(), Driver.GetGridCell(0, "ApplyOrderNumber", "Certificates").Text,
                "grid ApplyOrderNumber used");
            VerifyIsTrue(
                Driver.GetGridCell(0, "OrderCertificatePaid", "Certificates").FindElement(By.CssSelector("input"))
                    .Selected, "grid Paid used");
            VerifyIsTrue(Driver.GetGridCell(0, "Enable", "Certificates").FindElement(By.CssSelector("input")).Selected,
                "grid Enable used");
            VerifyIsTrue(Driver.GetGridCell(0, "Used", "Certificates").FindElement(By.CssSelector(" input")).Selected,
                "grid Used used");
            //cheking repeat used client
            ProductToCard("1");
            GoToClient("cart");
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client discount");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client result price");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("New Certificate");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastError();
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "old price without coupon");

            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.WaitForElem(By.CssSelector(".breads"));
            Driver.WaitForElem(By.Id("rightCell"));
            Driver.WaitForElem(By.XPath("//span[contains(text(), 'Курьером')]"));

            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-result")).Text.Contains("900 руб."),
                "checkout rezult 2");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("New Certificate");
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-action.btn-expander")).Click();
            Driver.WaitForToastError();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-result")).Text.Contains("900 руб."),
                "checkout afred cert rezult");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".checkout-success-content"));
        }

        [Test]
        public void AddDisabledCertificate()
        {
            GoToAdmin("settingscoupons#?couponsTab=certificates");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCertificates\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Добавить подарочный сертификат", Driver.FindElement(By.TagName("h2")).Text, "modal h2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CertCode\"]")).SendKeys("New Disabled Certificate");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertFrom\"]")).SendKeys("New From Disabled me");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertTo\"]")).SendKeys("New To Disabled  Me");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertSum\"]")).SendKeys("100");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertMailTo\"]")).SendKeys("testTo@mail.ru");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertMailFrom\"]")).SendKeys("testFrom@mail.ru");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertEnabled\"]")).Click();

            Driver.SetCkText("It is a new Disabled certificate!", "editor1");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"CertPayment\"]")))).SelectByText(
                "При получении (наличными или банковской картой)");

            Driver.FindElement(By.CssSelector("[data-e2e=\"CertSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check grid
            VerifyAreEqual("New Disabled Certificate", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "grid CertificateCode");
            VerifyAreNotEqual("", Driver.GetGridCell(0, "OrderId", "Certificates").Text, "grid OrderId");
            VerifyAreEqual("", Driver.GetGridCell(0, "ApplyOrderNumber", "Certificates").Text, "grid ApplyOrderNumber");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "FullSum", "Certificates").Text, "grid FullSum");
            VerifyIsFalse(
                Driver.GetGridCell(0, "OrderCertificatePaid", "Certificates").FindElement(By.CssSelector("input"))
                    .Selected, "grid Paid");
            VerifyIsFalse(Driver.GetGridCell(0, "Enable", "Certificates").FindElement(By.CssSelector("input")).Selected,
                "grid Enable");
            VerifyIsFalse(Driver.GetGridCell(0, "Used", "Certificates").FindElement(By.CssSelector(" input")).Selected,
                "grid Used");
            VerifyIsTrue(
                Driver.GetGridCell(0, "CreationDates", "Certificates").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "grid CreationDates");

            //cheking client
            ProductToCard("1");
            GoToClient("cart");
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client percent discount");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client sum discont");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("New Disabled Certificate");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastError();
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client old cart price");

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "SumFormatted").Text, " Grid orders Sum");
            VerifyIsTrue(
                Driver.GetGridCell(0, "OrderDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "grid orders CreationDates");
            //check order
            Driver.GetGridCell(0, "Number").Click();
            Driver.WaitForElem(By.Id("Order_OrderDate"));
            VerifyAreEqual("Сертификат", Driver.GetGridCell(0, "CustomName", "OrderCertificates").Text,
                " Grid in order CustomName");
            VerifyAreEqual("New Disabled Certificate",
                Driver.GetGridCell(0, "CertificateCode", "OrderCertificates").Text, " Grid in order CertificateCode");
            VerifyAreEqual("100", Driver.GetGridCell(0, "Sum", "OrderCertificates").Text, " Grid in order Sum");
            VerifyAreEqual("", Driver.GetGridCell(0, "Price", "OrderCertificates").Text, " Grid in order Price");
            VerifyAreEqual("100 руб.", Driver.FindElement(AdvBy.DataE2E("OrderSum")).Text, "Sum in Order");
            
            //pay
            Driver.FindElement(By.CssSelector(".switcher-state-label span")).Click();
            Driver.WaitForToastSuccess();

            //checking client again
            GoToClient("cart");
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client percent discount");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client sum discont");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("New Disabled Certificate");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastError();
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client old cart price");

            Driver.FindElement(By.CssSelector(".cart-full-body-item.cart-full-remove a")).Click();
        }

        [Test]
        public void EditCertificate()
        {
            GoToAdmin("settingscoupons#?couponsTab=certificates");

            Driver.GridFilterSendKeys("Certificate1");
            Driver.DropFocusCss("[data-e2e=\"CertificatesTitle\"]");
            Driver.GetGridCell(0, "CertificateCode", "Certificates").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Редактировать подарочный сертификат", Driver.FindElement(By.TagName("h2")).Text,
                "modal edit h2");
            //cheking modal win

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

            Driver.FindElement(By.CssSelector("[data-e2e=\"CertFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertFrom\"]")).SendKeys("Meee");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertTo\"]")).SendKeys("Mee");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertSum\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertSum\"]")).SendKeys("1000");
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertUsed\"]")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertEnabled\"]")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertPaid\"]")).FindElement(By.TagName("span")).Click();
            
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertSave\"]")).Click();
            Driver.WaitForToastSuccess();            
            
            //cheking grid
            Refresh();
            Driver.GridFilterSendKeys("Certificate1");
            Driver.DropFocusCss("[data-e2e=\"CertificatesTitle\"]");
            VerifyAreEqual("Certificate1", Driver.GetGridCell(0, "CertificateCode", "Certificates").Text,
                "grid CertificateCode");
            VerifyAreEqual("1 000 руб.", Driver.GetGridCell(0, "FullSum", "Certificates").Text, "grid FullSum");
            VerifyIsFalse(
                Driver.GetGridCell(0, "OrderCertificatePaid", "Certificates").FindElement(By.CssSelector("input"))
                    .Selected, "grid Paid");
            VerifyIsFalse(Driver.GetGridCell(0, "Enable", "Certificates").FindElement(By.CssSelector("input")).Selected,
                "grid Enable");
            VerifyIsFalse(Driver.GetGridCell(0, "Used", "Certificates").FindElement(By.CssSelector(" input")).Selected,
                "grid Used");
        }

        [Test]
        public void UseCertificate()
        {
            ProductToCard("1");
            GoToClient("cart");
            VerifyAreEqual("Скидка: 10%", Driver.FindElement(By.CssSelector(".price-discount")).Text,
                "client discount");
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client result price");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("Certificate2");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastError();
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client old cart price");

            GoToAdmin("settingscoupons#?couponsTab=certificates");
            Driver.GridFilterSendKeys("Certificate2");
            Driver.GetGridCell(0, "CertificateCode", "Certificates").FindElement(By.TagName("a")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertPaid\"]")).FindElement(By.TagName("span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"CertSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToClient("cart");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("Certificate2");
            Driver.FindElement(By.CssSelector(".btn-action.btn-expander")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("Сертификат:", Driver.FindElements(By.CssSelector(".cart-full-summary-name"))[1].Text,
                "client coupon");
            VerifyAreEqual("150 руб.", Driver.FindElements(By.CssSelector(".cart-full-summary-price span"))[0].Text,
                "client sum coupon");
            VerifyAreEqual("750 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client rezult");

            Driver.FindElement(By.CssSelector(".cart-full-summary-price a")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("900 руб.", Driver.FindElement(By.CssSelector(".cart-full-result-price")).Text,
                "client rezult");

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector(".btn.btn-middle.btn-submit")).Click();
            Driver.WaitForElem(By.CssSelector(".breads"));
            Driver.WaitForElem(By.Id("rightCell"));

            Driver.ScrollTo(By.TagName("footer"));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-result")).Text.Contains("900 руб."),
                "checkout rezult");
            Driver.FindElement(By.Name("cardsFormBlock")).FindElement(By.CssSelector("input[type=text]"))
                .SendKeys("Certificate2");
            Driver.FindElement(By.CssSelector(".btn.btn-small.btn-action.btn-expander")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".checkout-result")).Text.Contains("750 руб."),
                "checkout afred cert rezult");
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnCheckout\"]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-success-title"));
        }

        public void ProductToCard(string id)
        {
            GoToClient("products/test-product" + id);
            Driver.ScrollTo(By.CssSelector(".rating"));
            Driver.FindElement(By.CssSelector(".details-payment-inline a")).Click();
            Driver.WaitForElem(By.ClassName("cart-mini-block"));
        }
    }
}