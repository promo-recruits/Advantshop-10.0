using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.Customer
{
    [TestFixture]
    public class CustomersNewCartTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Orders | ClearType.Catalog | ClearType.Payment |
                                        ClearType.Shipping);
            InitializeService.LoadData(
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.CustomerGroup.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.Customer.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.Contact.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.Departments.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.Managers.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.Subscription.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.CustomerField.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Catalog.Product.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Catalog.Offer.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Catalog.Category.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Catalog.ProductCategories.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].OrderContact.csv",
                "Data\\Admin\\Customers\\CustomerNewCart\\[Order].OrderSource.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].OrderStatus.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].PaymentMethod.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].ShippingMethod.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].[Order].csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].OrderCurrency.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].OrderItems.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].Lead.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].LeadCurrency.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].LeadEvent.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].LeadItem.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\[Order].OrderCustomer.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.CustomerSegment.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Customers.CustomerSegment_Customer.csv",
                "data\\Admin\\Customers\\CustomerNewCart\\Settings.MailTemplate.csv"
            );
            InitializeService.AppActive();
            InitializeService.BonusSystemActive();
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
        public void CustomerCartOpen()
        {
            GoToAdmin("customers");

            Driver.GetGridCell(1, "Name", "Customers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            VerifyAreEqual("TestSurname1 TestName1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientName\"]")).Text, "customer cart h1");
            VerifyAreEqual("70 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"orderSum\"]")).Text,
                "customer cart orderSum");
            VerifyAreEqual("10 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"averageCheck\"]")).Text,
                "customer cart averageCheck");
            VerifyAreEqual("7", Driver.FindElement(By.CssSelector("[data-e2e=\"OrdersCount\"]")).Text,
                "customer cart OrdersCount");
            VerifyAreEqual("1д.", Driver.FindElement(By.CssSelector("[data-e2e=\"durationTime\"]")).Text,
                "customer cart durationTime");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"clientOrganization\"]")).Count == 0,
                "customer cart organization ");
            VerifyAreEqual("testmail1@mail.com", Driver.FindElement(By.CssSelector("[data-e2e=\"clientMail\"]")).Text,
                "customer cart clientMail ");
            VerifyAreEqual("TestSurname1 TestName1 TestPatronymic1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientFio\"]")).Text, "customer cart clientFio ");
            VerifyAreEqual("+8 9998812345", Driver.FindElement(By.CssSelector("[data-e2e=\"clientPhone\"]")).Text,
                "customer cart clientPhone ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientAdress\"]")).Text
                    .Contains("222222, Россия, Московская область, Москва"), "customer cart client Adress ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientAdress\"]")).Text
                    .Contains("улица Улица, д. 1, кв. 2, стр. 3, подъезд 4, эт. 5"), "customer cart clientAdress city");

            VerifyAreEqual("CustomerGroup2", Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroup\"]")).Text,
                "customer cart clientGroup ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ClientInterests\"]")).Text.Contains("TestCategory1"),
                "customer cart interests ");

            IWebElement selectElem1 = Driver.FindElement(By.Id("ManagerId"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("test testov"), "manager order");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"segment-CustomerSegment1\"]")).Displayed,
                "customer cart segment 1 ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"segment-CustomerSegment2\"]")).Displayed,
                "customer cart segment 2 ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"segment-CustomerSegment1\"]")).Text
                    .Contains("CustomerSegment1"), "customer cart segment 1 text");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"segment-CustomerSegment2\"]")).Text
                    .Contains("CustomerSegment2"), "customer cart segment 2 text");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Text.Contains("21"),
                "customer cart count order ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Text.Contains("Заказы"),
                "customer cart order ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Text.Contains("21"),
                "customer cart count lead ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Text.Contains("Лиды"),
                "customer cart  lead ");

            VerifyAreEqual("Customer Field 1 Value 3",
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerField-1\"]")).Text, "cust field 1");
            VerifyAreEqual("additional text field",
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerField-2\"]")).Text, "cust field 2");
            VerifyAreEqual("123123", Driver.FindElement(By.CssSelector("[data-e2e=\"customerField-3\"]")).Text,
                "cust field 3");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"customerField-4\"]")).Count == 0,
                "cust field 4");
            VerifyAreEqual("Admin Comment Deafult",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientComment\"] textarea")).GetAttribute("value"),
                "admin comment");
        }

        [Test]
        public void CustomerCartOrg()
        {
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestOrganization");

            VerifyAreEqual("TestOrganization", Driver.GetGridCell(0, "Name", "Customers").Text, "grid customer name");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));

            VerifyAreEqual("TestSurname2 TestName2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientName\"]")).Text, "customer cart h1");

            VerifyAreEqual("0 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"orderSum\"]")).Text,
                "customer cart orderSum");
            VerifyAreEqual("0 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"averageCheck\"]")).Text,
                "customer cart averageCheck");
            VerifyAreEqual("0д.", Driver.FindElement(By.CssSelector("[data-e2e=\"durationTime\"]")).Text,
                "customer cart durationTime");

            VerifyAreEqual("TestSurname2 TestName2 TestPatronymic2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientFio\"]")).Text, "customer cart clientFio ");
            VerifyAreEqual("TestOrganization",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientOrganization\"]")).Text,
                "customer cart organization ");
            VerifyAreEqual("testmailimap@yandex.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientMail\"]")).Text, "customer cart clientMail ");
            VerifyAreEqual("+9 9988812345", Driver.FindElement(By.CssSelector("[data-e2e=\"clientPhone\"]")).Text,
                "customer cart clientPhone ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"clientAdress\"]")).Count == 0,
                "customer cart clientAdress ");
            VerifyAreEqual("Обычный покупатель",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroup\"]")).Text,
                "customer cart clientGroup for buyer without orders");
            VerifyIsTrue(Driver.FindElements(By.Id("ManagerId")).Count == 0,
                "customer cart clientManager for buyer without orders");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ClientInterests\"]")).Count == 0,
                "customer cart interests ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"segment-CustomerSegment1\"]")).Count == 0,
                "customer cart segment 1 ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"segment-CustomerSegment2\"]")).Count == 0,
                "customer cart segment 2 ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Text.Contains("0"),
                "customer cart count order ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Text.Contains("Заказы"),
                "customer cart order ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Text.Contains("0"),
                "customer cart count lead ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Text.Contains("Лиды"),
                "customer cart  lead ");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"customerField-1\"]")).Count == 0,
                "cust field 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"customerField-2\"]")).Count == 0,
                "cust field 2");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"customerField-3\"]")).Count == 0,
                "cust field 3");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"customerField-4\"]")).Count == 0,
                "cust field 4");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientComment\"] textarea")).GetAttribute("value"),
                "admin comment");

            Driver.FindElement(By.CssSelector("[data-e2e=\"clientComment\"] textarea")).SendKeys("new admin comment");
            Driver.FindElement(By.CssSelector("h3.category-title")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("new admin comment",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientComment\"] textarea")).GetAttribute("value"),
                "new admin comment");
            Refresh();
            VerifyAreEqual("new admin comment",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientComment\"] textarea")).GetAttribute("value"),
                " 2 new admin comment");
        }

        [Test]
        public void CustomerCartStatus()
        {
            GoToAdmin("customers/view/cfc2c33b-1e84-415e-8482-e98156341604");

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-left.vip")).Count == 0,
                "no vip status");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-right.bad")).Count == 0,
                "no bad status");

            Driver.FindElement(By.CssSelector("[data-e2e=\"statusVip\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-left.vip")).Count == 1,
                " vip status");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-right.bad")).Count == 0,
                "no bad status 1");

            Refresh();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-left.vip")).Count == 1,
                "refresh vip status");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-right.bad")).Count == 0,
                "refresh no bad status 1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"statusVip\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-left.vip")).Count == 0,
                "1 no vip status");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-right.bad")).Count == 0,
                "1 no bad status");

            Refresh();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-left.vip")).Count == 0,
                "refresh1 no vip status");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-right.bad")).Count == 0,
                "refresh1 no bad status");

            Driver.FindElement(By.CssSelector("[data-e2e=\"statusBad\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-left.vip")).Count == 0,
                "no vip status 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-right.bad")).Count == 1,
                " bad status");

            Refresh();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-left.vip")).Count == 0,
                "refresh2 no vip status");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-right.bad")).Count == 1,
                "refresh2  bad status");

            Driver.FindElement(By.CssSelector("[data-e2e=\"statusBad\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-left.vip")).Count == 0,
                "2 no vip status");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-right.bad")).Count == 0,
                "2 no bad status");

            Refresh();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-left.vip")).Count == 0,
                "refresh3 no vip status");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".customer-status-label-right.bad")).Count == 0,
                "refresh3 no bad status");
        }

        [Test]
        public void CustomerMailOpen()
        {
            GoToAdmin("customers");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            Driver.FindElement(By.CssSelector("[data-controller=\"'ModalSendLetterToCustomerCtrl'\"] a")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Письмо покупателю: testmailimap@yandex.ru", Driver.FindElement(By.TagName("h2")).Text,
                "h2 modal win ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed, "modal dialog display ");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"AnswerTemplate\"]"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Пустой"), "empty template");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMail\"]")).GetAttribute("value"),
                "empty subject");
            Driver.AssertCkText("", "editor1");

            IList<IWebElement> allOptions = select3.Options;
            VerifyIsTrue(allOptions.Count == 10, "count answer template");
        }

        [Test]
        public void CustomerMailSend()
        {
            //GoToAdmin("settingsmail#?notifyTab=emailsettings");
            //if (driver.FindElement(By.Id("myAccountContract")).Selected)
            //{
            //    driver.FindElement(By.CssSelector("[data-e2e=\"UseSmtpMail\"]")).Click();
            //    driver.FindElement(By.CssSelector("[data-e2e=\"BtnSaveMailSettings\"]")).Click();
            //}

            GoToAdmin("customers");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            Driver.FindElement(By.CssSelector("[data-controller=\"'ModalSendLetterToCustomerCtrl'\"] a")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Письмо покупателю: testmailimap@yandex.ru", Driver.FindElement(By.TagName("h2")).Text,
                "h2 modal win ");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AnswerTemplate\"]")))).SelectByText(
                "Template5");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"AnswerTemplate\"]"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Template5"), "check template");
            VerifyAreEqual("Subject5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"SubjectMail\"]")).GetAttribute("value"), "fill subject");
            Driver.AssertCkText("Text message5: TestName2, TestSurname2, TestPatronymic2, Мой магазин", "editor2");

            Driver.XPathContainsText("span", "Отправить");
            Driver.WaitForToastSuccess();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog")).Count == 0, "modal dialog display count");
        }

        [Test]
        public void CustomerMailTemplates()
        {
            GoToAdmin("customers");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            Driver.FindElement(By.CssSelector("[data-controller=\"'ModalSendLetterToCustomerCtrl'\"] a")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Письмо покупателю: testmailimap@yandex.ru", Driver.FindElement(By.TagName("h2")).Text,
                "h2 modal win ");

            Driver.FindElement(By.CssSelector("[data-e2e=\"EditTemplate\"]")).Click();
            Functions.OpenNewTab(Driver, BaseUrl);

            VerifyIsTrue(Driver.Url.Contains("settingsmail#?notifyTab=templates"), "check url template");
            VerifyAreEqual("Шаблоны ответов", Driver.FindElement(By.CssSelector(".tab-pane.active h1")).Text,
                "h1 templates");

            Functions.CloseTab(Driver, BaseUrl);
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"AnswerTemplate\"]")))).SelectByText(
                "Template5");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"EditTemplate\"]")).Displayed,
                "display href edit template");
        }

        string cartNum = "";

        [Test]
        public void CustomerGetBonusCart()
        {
            GoToAdmin("customers");
            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddCard\"]")).Click();
            Driver.WaitForModal();
            VerifyAreEqual("Добавить карту", Driver.FindElement(By.TagName("h2")).Text, "h2 modal win ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed, "modal dialog display ");

            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"SelectGrade\"]"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Гостевой"), "grade");

            cartNum = Driver.FindElement(By.CssSelector("[data-e2e=\"CardNumber\"]")).GetAttribute("value");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"SelectGrade\"]"))))
                .SelectByText("Золотой");
            VerifyAreEqual("TestName2 TestSurname2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"selectCustomer\"]")).Text, "select customer");

            Driver.FindElement(By.CssSelector("[data-e2e=\"addCard\"]")).Click();
            VerifyAreEqual("Карта TestSurname2 TestName2", Driver.FindElement(By.CssSelector(".balance-block h2")).Text,
                "select customer");
            GoToAdmin("customers");
            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            VerifyAreEqual("TestSurname2 TestName2 TestPatronymic2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientFio\"]")).Text, "customer cart clientFio ");
            VerifyAreEqual("testmailimap@yandex.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientMail\"]")).Text, "customer cart clientMail ");
            VerifyAreEqual("TestOrganization",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientOrganization\"]")).Text,
                "customer cart organization ");
        }

        /*
        [Test]
        public void CustomerPhoto()
        {
            GoToAdmin("customers");
            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            Driver.MouseFocus(driver, By.CssSelector("[data-e2e=\"TabLeads\"]"));
            Driver.MouseFocus(driver, By.CssSelector("[data-e2e=\"clientAvatar\"]"));
            VerifyIsTrue(driver.FindElement(By.CssSelector("[data-e2e=\"clientAvatar\"]")).GetAttribute("src").Contains("no-avatar"), "no avatar");
            driver.FindElement(By.LinkText("Загрузить фото")).Click(); ;

            attachFile(driver, By.CssSelector("input[data-e2e=\"LoadImg\"]"), GetPicturePath("avatar.jpg"));
            VerifyAreEqual("Загрузка изображения", driver.FindElement(By.TagName("h2")).Text, "h2 add new user img");

            driver.FindElement(By.XPath("//button[contains(text(),\"Сохранить\")]")).Click();
            VerifyIsFalse(driver.FindElement(By.CssSelector("[data-e2e=\"clientAvatar\"]")).GetAttribute("src").Contains("no-avatar"), "yes avatar");

            Driver.ScrollToTop();
            //del avatar
            Driver.MouseFocus(driver, By.CssSelector("[data-e2e=\"TabLeads\"]"));
            Driver.MouseFocus(driver, By.CssSelector("[data-e2e=\"clientAvatar\"]"));
            driver.FindElement(By.LinkText("Загрузить фото")).Click(); ;

            driver.FindElement(By.CssSelector("[data-e2e=\"DelImg\"]")).Click();
            VerifyIsTrue(driver.FindElement(By.CssSelector("[data-e2e=\"clientAvatar\"]")).GetAttribute("src").Contains("no-avatar"), "no del avatar");

            //by href
            Driver.MouseFocus(driver, By.CssSelector("[data-e2e=\"TabLeads\"]"));
            Driver.MouseFocus(driver, By.CssSelector("[data-e2e=\"clientAvatar\"]"));
            driver.FindElement(By.LinkText("Загрузить фото")).Click(); ;
            VerifyIsFalse(driver.FindElement(By.CssSelector("[data-e2e=\"PicturePath\"]")).Displayed, "no path img");
            VerifyIsFalse(driver.FindElement(By.CssSelector("[data-e2e=\"LoadImgByHref\"]")).Displayed, "no btn load img");
            driver.FindElement(By.CssSelector("[data-e2e=\"LoadByHref\"]")).Click(); ;

            VerifyIsTrue( driver.FindElement(By.CssSelector("[data-e2e=\"PicturePath\"]")).Displayed, "path img");
            VerifyIsTrue( driver.FindElement(By.CssSelector("[data-e2e=\"LoadImgByHref\"]")).Displayed, "btn load img");
            driver.FindElement(By.CssSelector("[data-e2e=\"PicturePath\"]")).SendKeys("https://cs8.pikabu.ru/post_img/2016/01/18/5/1453101821143319739.jpg");
            driver.FindElement(By.CssSelector("[data-e2e=\"LoadImgByHref\"]")).Click(); ;
            VerifyAreEqual("Загрузка изображения", driver.FindElement(By.TagName("h2")).Text, "h2 add new user img");

            driver.FindElement(By.XPath("//button[contains(text(),\"Сохранить\")]")).Click();
            VerifyIsFalse(driver.FindElement(By.CssSelector("[data-e2e=\"clientAvatar\"]")).GetAttribute("src").Contains("no-avatar"), "yes avatar by href");
                    }*/
        [Test]
        public void CustomerrTabs()
        {
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestOrganization");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("customer-orders [data-e2e=\"gridRow\"]")).Count == 0,
                " customer count no orders");

            Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("customer-leads [data-e2e=\"gridRow\"]")).Count == 0,
                " customer count no lead");

            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestName1 TestSurname1");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("customer-orders [data-e2e=\"gridRow\"]")).Count == 10,
                " customer count 10 orders");

            Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("customer-leads [data-e2e=\"gridRow\"]")).Count == 10,
                " customer count 10 lead");
        }

        [Test]
        public void CustomerLeadOpenPage()
        {
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestName1 TestSurname1");

            VerifyAreEqual("TestSurname1 TestName1 TestPatronymic1", Driver.GetGridCell(0, "Name", "Customers").Text,
                "grid customer name");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("customer-leads h2")).Text.Contains("Лиды"),
                " customer leads");

            VerifyAreEqual("120", Driver.GetGridCell(0, "Id", "CustomerLeads").Text, "page 1 line 1");
            VerifyAreEqual("111", Driver.GetGridCell(9, "Id", "CustomerLeads").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addLead\"]"));
            Driver.FindElement(By.CssSelector("customer-leads .pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("110", Driver.GetGridCell(0, "Id", "CustomerLeads").Text, "page 2 line 1");
            VerifyAreEqual("101", Driver.GetGridCell(9, "Id", "CustomerLeads").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addLead\"]"));
            Driver.FindElement(By.CssSelector("customer-leads .pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("100", Driver.GetGridCell(0, "Id", "CustomerLeads").Text, "page 3 line 1");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addLead\"]"));
            Driver.FindElement(By.CssSelector("customer-leads .pagination-first a")).Click();
            VerifyAreEqual("120", Driver.GetGridCell(0, "Id", "CustomerLeads").Text, "page 1 line 1");
            VerifyAreEqual("111", Driver.GetGridCell(9, "Id", "CustomerLeads").Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addLead\"]"));
            Driver.FindElement(By.CssSelector("customer-leads .pagination-last a")).Click();
            VerifyAreEqual("100", Driver.GetGridCell(0, "Id", "CustomerLeads").Text, "last page line 1");

            Driver.FindElement(By.CssSelector("customer-leads .pagination-prev a")).Click();
            VerifyAreEqual("110", Driver.GetGridCell(0, "Id", "CustomerLeads").Text, "page 2 line 1");
            VerifyAreEqual("101", Driver.GetGridCell(9, "Id", "CustomerLeads").Text, "page 2 line 10");
        }

        [Test]
        public void CustomerLeadOpenView()
        {
            GoToAdmin("customers");

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridFilterSendKeys("TestSurname1 TestName1");

            VerifyAreEqual("TestSurname1 TestName1 TestPatronymic1", Driver.GetGridCell(0, "Name", "Customers").Text,
                "grid customer name");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("customer-leads h2")).Text.Contains("Лиды"),
                " customer leads");

            Driver.GridPaginationSelectItems("20");

            VerifyAreEqual("120", Driver.GetGridCell(0, "Id", "CustomerLeads").Text, "present line 1");
            VerifyAreEqual("101", Driver.GetGridCell(19, "Id", "CustomerLeads").Text, "present line 10");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("customer-leads [data-e2e=\"gridRow\"]")).Count == 20,
                " customer count 20 orders");

            Driver.GridPaginationSelectItems("10");

            VerifyAreEqual("120", Driver.GetGridCell(0, "Id", "CustomerLeads").Text, "present line 1");
            VerifyAreEqual("111", Driver.GetGridCell(9, "Id", "CustomerLeads").Text, "present line 10");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("customer-leads [data-e2e=\"gridRow\"]")).Count == 10,
                " customer count 10 orders");
        }

        [Test]
        public void CustomerLeadsAdd()
        {
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestName1 TestSurname1");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"addLead\"]")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed, " modal win");

            Driver.XPathContainsText("h2", "Новый лид");

            VerifyAreEqual("TestSurname1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).GetAttribute("value"),
                "existing customer LastName");
            VerifyAreEqual("TestName1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).GetAttribute("value"),
                "existing customer FirstName");
            VerifyAreEqual("TestPatronymic1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).GetAttribute("value"),
                "existing customer Patronymic");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadOrganization\"]")).GetAttribute("value"),
                "existing customer org");
            VerifyAreEqual("+7(999)881-23-45",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).GetAttribute("value"),
                "existing customer PhoneNum");
            VerifyAreEqual("testmail1@mail.com",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).GetAttribute("value"),
                "existing customer Email");

            var sadf = Driver.FindElement(By.CssSelector(".modal-content"))
                .FindElement(By.CssSelector("[data-e2e=\"LeadCustomer\"]")).Text;
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".modal-content"))
                    .FindElement(By.CssSelector("[data-e2e=\"LeadCustomer\"]")).Text
                    .Contains("TestName1 TestSurname1 79998812345 testmail1@mail.com"), "customer info added");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-content")).Text.Contains("Выбран покупатель"),
                "customer added");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadDescription\"]")).SendKeys("TestDescription");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadSum\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadSum\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadSum\"]")).SendKeys("500");
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadManager\"]"))))
                .SelectByText("Elena El");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));
            GoToAdmin("customers/view/cfc2c33b-1e84-415e-8482-e98156341604#?customerOrdersTab=leadsCustomer");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Text.Contains("22"),
                "customer cart count lead ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Text.Contains("Лиды"),
                "customer cart  lead ");
            //driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Click();
            VerifyAreEqual("121", Driver.GetGridCell(0, "Id", "CustomerLeads").Text, "number new lead");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName", "CustomerLeads").Text, "status new lead");
            VerifyAreEqual("TestSurname1 TestName1 TestPatronymic1",
                Driver.GetGridCell(0, "FullName", "CustomerLeads").Text, "name new lead");
            VerifyAreEqual("500", Driver.GetGridCell(0, "Sum", "CustomerLeads").Text, "sum new lead");
            VerifyIsTrue(
                Driver.GetGridCell(0, "CreatedDateFormatted", "CustomerLeads").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "CreatedDateFormatted new lead");
            VerifyAreEqual("Elena El", Driver.GetGridCell(0, "ManagerName", "CustomerLeads").Text,
                "ManagerName new lead");
        }

        [Test]
        public void CustomerOrderOpenPage()
        {
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestName1 TestSurname1");

            VerifyAreEqual("TestSurname1 TestName1 TestPatronymic1", Driver.GetGridCell(0, "Name", "Customers").Text,
                "grid customer name");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("customer-orders h2")).Text.Contains("Заказы"),
                " customer orders");

            VerifyAreEqual("21", Driver.GetGridCell(0, "Number", "CustomerOrders").Text, "page 1 line 1");
            VerifyAreEqual("12", Driver.GetGridCell(9, "Number", "CustomerOrders").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addOrder\"]"));
            Driver.FindElement(By.CssSelector("customer-orders .pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("11", Driver.GetGridCell(0, "Number", "CustomerOrders").Text, "page 2 line 1");
            VerifyAreEqual("2", Driver.GetGridCell(9, "Number", "CustomerOrders").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addOrder\"]"));
            Driver.FindElement(By.CssSelector("customer-orders .pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "Number", "CustomerOrders").Text, "page 3 line 1");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addOrder\"]"));
            Driver.FindElement(By.CssSelector("customer-orders .pagination-first a")).Click();
            VerifyAreEqual("21", Driver.GetGridCell(0, "Number", "CustomerOrders").Text, "page 1 line 1");
            VerifyAreEqual("12", Driver.GetGridCell(9, "Number", "CustomerOrders").Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addOrder\"]"));
            Driver.FindElement(By.CssSelector("customer-orders .pagination-last a")).Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "Number", "CustomerOrders").Text, "last page line 1");

            Driver.FindElement(By.CssSelector("customer-orders .pagination-prev a")).Click();
            VerifyAreEqual("11", Driver.GetGridCell(0, "Number", "CustomerOrders").Text, "page 2 line 1");
            VerifyAreEqual("2", Driver.GetGridCell(9, "Number", "CustomerOrders").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"clientName\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("customer-leads h2")).Text.Contains("Лиды"),
                " customer leads");
        }

        [Test]
        public void CustomerOrderOpenView()
        {
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestName1 TestSurname1");

            VerifyAreEqual("TestSurname1 TestName1 TestPatronymic1", Driver.GetGridCell(0, "Name", "Customers").Text,
                "grid customer name");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("customer-orders h2")).Text.Contains("Заказы"),
                " customer orders");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addOrder\"]"));
            Driver.GridPaginationSelectItems("20", "gridCustomerOrders");

            VerifyAreEqual("21", Driver.GetGridCell(0, "Number", "CustomerOrders").Text, "present line 1");
            VerifyAreEqual("2", Driver.GetGridCell(19, "Number", "CustomerOrders").Text, "present line 10");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("customer-orders [data-e2e=\"gridRow\"]")).Count == 20,
                " customer count 20 orders");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addOrder\"]"));
            Driver.GridPaginationSelectItems("50", "gridCustomerOrders");

            VerifyAreEqual("21", Driver.GetGridCell(0, "Number", "CustomerOrders").Text, "present line 1");
            VerifyAreEqual("1", Driver.GetGridCell(20, "Number", "CustomerOrders").Text, "present line 10");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("customer-orders [data-e2e=\"gridRow\"]")).Count == 21,
                " customer count 50 orders");


            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addOrder\"]"));
            Driver.GridPaginationSelectItems("100", "gridCustomerOrders");

            VerifyAreEqual("21", Driver.GetGridCell(0, "Number", "CustomerOrders").Text, "present line 1");
            VerifyAreEqual("1", Driver.GetGridCell(20, "Number", "CustomerOrders").Text, "present line 10");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("customer-orders [data-e2e=\"gridRow\"]")).Count == 21,
                " customer count 100 orders");


            Driver.ScrollTo(By.CssSelector("[data-e2e=\"addOrder\"]"));
            Driver.GridPaginationSelectItems("10", "gridCustomerOrders");

            VerifyAreEqual("21", Driver.GetGridCell(0, "Number", "CustomerOrders").Text, "present line 1");
            VerifyAreEqual("12", Driver.GetGridCell(9, "Number", "CustomerOrders").Text, "present line 10");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("customer-orders [data-e2e=\"gridRow\"]")).Count == 10,
                " customer count 10 orders");
        }

        [Test]
        public void CustomerOrderGo()
        {
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestName1 TestSurname1");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Click();
            Driver.GetGridCell(0, "Number", "CustomerOrders").FindElement(By.TagName("a")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);

            VerifyIsTrue(Driver.FindElement(By.TagName("h1")).Text.Contains("Заказ № 21"), "h1 order");
            VerifyIsTrue(Driver.Url.Contains("orders/edit/21"), "order url");

            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void CustomerLeadGo()
        {
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestName1 TestSurname1");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Click();
            Driver.GetGridCell(0, "Id", "CustomerLeads").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            VerifyAreEqual("Лид №120", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "h1 lead");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lead-info")).Displayed, "lead modal");
        }

        [Test]
        public void CustomerOrdersAdd()
        {
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestName1 TestSurname1");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("customer-orders [data-e2e=\"gridRow\"]")).Count == 10,
                " customer count 10 orders");

            VerifyIsTrue(Driver.FindElement(By.CssSelector("customer-orders h2")).Text.Contains("Заказы"),
                " customer orders");
            Driver.FindElement(By.CssSelector("[data-e2e=\"addOrder\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual("TestSurname1",
                Driver.FindElement(By.Id("Order_OrderCustomer_LastName")).GetAttribute("value"), " order LastName add");
            VerifyAreEqual("TestName1",
                Driver.FindElement(By.Id("Order_OrderCustomer_FirstName")).GetAttribute("value"),
                "order FirstName add");
            VerifyAreEqual("TestPatronymic1",
                Driver.FindElement(By.Id("Order_OrderCustomer_Patronymic")).GetAttribute("value"),
                "order Patronymic add");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Organization")).GetAttribute("value"),
                "order Organization add");
            VerifyAreEqual("Россия", Driver.FindElement(By.Id("Order_OrderCustomer_Country")).GetAttribute("value"),
                "order Country add");
            VerifyAreEqual("Московская область",
                Driver.FindElement(By.Id("Order_OrderCustomer_Region")).GetAttribute("value"), "order Region add");
            VerifyAreEqual("Москва", Driver.FindElement(By.Id("Order_OrderCustomer_City")).GetAttribute("value"),
                "order City add");
            VerifyAreEqual("testmail1@mail.com",
                Driver.FindElement(By.Id("Order_OrderCustomer_Email")).GetAttribute("value"), "order Email add");
            VerifyAreEqual("222222", Driver.FindElement(By.Id("Order_OrderCustomer_Zip")).GetAttribute("value"),
                "order Zip add");
            VerifyAreEqual("+7(999)881-23-45",
                Driver.FindElement(By.Id("Order_OrderCustomer_Phone")).GetAttribute("value"), "order Phone add");
            VerifyAreEqual("Улица", Driver.FindElement(By.Id("Order_OrderCustomer_Street")).GetAttribute("value"),
                "order Street add");
            VerifyAreEqual("1", Driver.FindElement(By.Id("Order_OrderCustomer_House")).GetAttribute("value"),
                "order House add");
            VerifyAreEqual("2", Driver.FindElement(By.Id("Order_OrderCustomer_Apartment")).GetAttribute("value"),
                "order Apartment add");
            VerifyAreEqual("3", Driver.FindElement(By.Id("Order_OrderCustomer_Structure")).GetAttribute("value"),
                "order Structure add");
            VerifyAreEqual("4", Driver.FindElement(By.Id("Order_OrderCustomer_Entrance")).GetAttribute("value"),
                "order Entrance add");
            VerifyAreEqual("5", Driver.FindElement(By.Id("Order_OrderCustomer_Floor")).GetAttribute("value"),
                "order Floor add");
            Driver.FindElement(By.LinkText("Добавить товар")).Click();
            Driver.GridFilterSendKeys("TestProduct100");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr").Click();

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            IWebElement selectElem1 = Driver.FindElement(By.Id("customerfields_0__value"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Customer Field 1 Value 3"), "order custfield 1");

            VerifyAreEqual("additional text field",
                Driver.FindElement(By.Id("customerfields_1__value")).GetAttribute("value"), "order custfield 2");
            VerifyAreEqual("123123", Driver.FindElement(By.Id("customerfields_2__value")).GetAttribute("value"),
                "order custfield 3");
            VerifyAreEqual("", Driver.FindElement(By.Id("customerfields_3__value")).GetAttribute("value"),
                "order custfield 4");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Driver.WaitForToastSuccess();
            Functions.CloseTab(Driver, BaseUrl);
            Refresh();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Text.Contains("22"),
                "customer cart count order ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Text.Contains("Заказы"),
                "customer cart order ");
            VerifyAreEqual("31", Driver.GetGridCell(0, "Number", "CustomerOrders").Text, "number new lead");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName", "CustomerOrders").Text, "status new lead");
            VerifyAreEqual("Нет", Driver.GetGridCell(0, "IsPaid", "CustomerOrders").Text, "name new lead");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "SumFormatted", "CustomerOrders").Text, "sum new lead");
            VerifyIsTrue(
                Driver.GetGridCell(0, "OrderDateFormatted", "CustomerOrders").Text
                    .Contains(DateTime.Today.ToString("dd.MM.yyyy")), "CreatedDateFormatted new lead");
        }


        [Test]
        public void CustomerComment()
        {
            GoToAdmin("customers");

            Driver.GetGridCell(0, "Name", "Customers").Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEvent\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEvent\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEvent\"]")).SendKeys("New Comment");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventAdd\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("New Comment", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text,
                "new comment");

            Driver.XPathContainsText("span", "Комментарии");
            VerifyAreEqual("New Comment", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Text,
                "new commenton page");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"LeadEventType\"]")).Count == 1,
                "count comment");
        }
    }
}