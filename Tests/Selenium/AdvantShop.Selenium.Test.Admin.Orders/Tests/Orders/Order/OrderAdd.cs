using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Orders.Tests.Orders.Order
{
    [TestFixture]
    public class OrderAdd : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Orders\\Order\\Catalog.Product.csv",
                "data\\Admin\\Orders\\Order\\Catalog.Offer.csv",
                "data\\Admin\\Orders\\Order\\Catalog.Category.csv",
                "data\\Admin\\Orders\\Order\\Catalog.ProductCategories.csv"
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

        [Order(0)]
        [Test]
        public void OrdersAdd()
        {
            ReindexSearch();
            GoToAdmin("orders/add");
            VerifyAreEqual("Создание нового заказа",
                Driver.FindElement(By.TagName("h1")).FindElement(By.TagName("span")).Text, "OrdersAdd");
            VerifyIsTrue(Driver.FindElement(By.Id("Order_OrderCustomer_Organization")).Enabled,
                "add order organization field enabled");
            (new SelectElement(Driver.FindElement(By.Id("Order_ManagerId")))).SelectByText("Администратор Магазина");
            (new SelectElement(Driver.FindElement(By.Id("Order_OrderSourceId")))).SelectByText("Оффлайн");

            Driver.FindElement(By.Id("Order_OrderCustomer_LastName")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_LastName")).SendKeys("TestSurname");

            Driver.FindElement(By.Id("Order_OrderCustomer_FirstName")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_FirstName")).SendKeys("TestName");

            Driver.FindElement(By.Id("Order_OrderCustomer_Patronymic")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Patronymic")).SendKeys("TestPatronymic");

            Driver.FindElement(By.Id("Order_OrderCustomer_Organization")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Organization")).SendKeys("TestOrganization");

            Driver.FindElement(By.Id("Order_OrderCustomer_Country")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Country")).SendKeys("Россия");

            Driver.FindElement(By.Id("Order_OrderCustomer_City")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_City")).SendKeys("Москва");

            Driver.FindElement(By.Id("Order_OrderCustomer_Region")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Region")).Clear();
            Driver.FindElement(By.Id("Order_OrderCustomer_Region")).SendKeys("Московская область");

            Driver.FindElement(By.Id("Order_OrderCustomer_Email")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Email")).SendKeys("customertest@mail.ru");

            Driver.FindElement(By.Id("Order_OrderCustomer_Street")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Street")).SendKeys("TestStreet1");

            Driver.FindElement(By.Id("Order_OrderCustomer_Zip")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Zip")).Clear();
            Driver.FindElement(By.Id("Order_OrderCustomer_Zip")).SendKeys("111111");

            Driver.FindElement(By.Id("Order_OrderCustomer_Phone")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Phone")).SendKeys("+79277777777");

            Driver.FindElement(By.Id("Order_OrderCustomer_House")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_House")).SendKeys("1");

            Driver.FindElement(By.Id("Order_OrderCustomer_Apartment")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Apartment")).SendKeys("2");

            Driver.FindElement(By.Id("Order_OrderCustomer_Structure")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Structure")).SendKeys("3");

            Driver.FindElement(By.Id("Order_OrderCustomer_Entrance")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Entrance")).SendKeys("4");

            Driver.FindElement(By.Id("Order_OrderCustomer_Floor")).Click();
            Driver.FindElement(By.Id("Order_OrderCustomer_Floor")).SendKeys("5");

            Driver.FindElement(By.Id("Order_OrderCustomer_LastName")).Click();
            Driver.DropFocus("h1");
            Driver.ClearToastMessages();
            Driver.FindElement(By.LinkText("Добавить товар")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector(".btn-cancel")).Click();

            GoToAdmin("orders?filterby=drafts");
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));
            Driver.ScrollTo(By.Id("Order_OrderCustomer_Phone"));
            DelElement(Driver);
            Driver.FindElement(By.LinkText("Добавить товар")).Click();
            Driver.GridFilterSendKeys("TestProduct100");
            VerifyAreEqual("TestProduct100", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text,
                " Search product in Grid");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr").Click();

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("TestProduct100\r\nАртикул: 100", Driver.GetGridCell(0, "Name", "OrderItems").Text,
                "Name product at order");
            VerifyAreEqual("100", Driver.GetGridCell(0, "PriceString", "OrderItems").Text, " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").Text, " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "OrderItems").Text.Contains("100 руб."),
                " Cost product at order");

            Driver.FindElement(By.Id("Order_StatusComment")).SendKeys("Comments orders");
            Driver.FindElement(By.Id("Order_AdminOrderComment")).SendKeys("Admin comment orders");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");
            VerifyAreEqual("TestName TestSurname (TestOrganization)", Driver.GetGridCell(0, "BuyerName").Text,
                " Grid orders BuyerName");
            
            if (Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[-1]['ManagerName']\"]")).Count == 0)
            {
                Functions.ChangeEnabledGridColumn(Driver, new List<string> { "Менеджер" });
            }

            VerifyAreEqual("Администратор Магазина", Driver.GetGridCell(0, "ManagerName").Text,
                " Grid orders ManagerName");
            VerifyIsTrue(Driver.GetGridCell(0, "SumFormatted").Text.Contains("100"), " Grid orders SumFormatted");

            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));
            //VerifyIsTrue(driver.FindElement(By.Id("Order_OrderCustomer_Organization")).Enabled, "order organization field enabled");
            //VerifyAreEqual("TestOrganization", driver.FindElement(By.Id("Order_OrderCustomer_Organization")).GetAttribute("value"), "\r\norder Organization");
            //VerifyAreEqual("TestSurname", driver.FindElement(By.Id("Order_OrderCustomer_LastName")).GetAttribute("value"), " order LastName");
            //VerifyAreEqual("TestName", driver.FindElement(By.Id("Order_OrderCustomer_FirstName")).GetAttribute("value"), "\r\norder FirstName");
            //VerifyAreEqual("TestPatronymic", driver.FindElement(By.Id("Order_OrderCustomer_Patronymic")).GetAttribute("value"), "\r\norder Patronymic");
            //VerifyAreEqual("Россия", driver.FindElement(By.Id("Order_OrderCustomer_Country")).GetAttribute("value"), "\r\norder Country");
            //VerifyAreEqual("Московская область", driver.F.Id("Order_OrderCustomer_City")).GetAttribute("value"), "\r\norder City");
            //VerifyAreEqual("111111", driver.FindElement(By.Id("Order_OrderCustomer_Zip")).GetAttribute("value"), "\r\norder Zip");
            //VerifyAreEqual("TestStreet1", driver.FindElement(By.Id("Order_OrderCustomer_Street")).GetAttribute("value"), "\r\norder Street");
            //VerifyAreEqual("1", driver.FindElement(By.Id("Order_OrderCustomer_House")).GetAttribute("value"), "\r\norder House");
            //VerifyAreEqual("2", driver.FindElement(By.Id("Order_OrderCustomer_Apartment")).GetAttribute("value"), "\r\norder Apartment");
            //VerifyAreEqual("3", driver.FindElement(By.Id("Order_OrderCustomer_Structure")).GetAttribute("value"), "\r\norder Structure");
            //VerifyAreEqual("4", driver.FindElement(By.Id("Order_OrderCustomer_Entrance")).GetAttribute("value"), "\r\norder Entrance");
            //VerifyAreEqual("5", driver.FindElement(By.Id("Order_OrderCustomer_Floor")).GetAttribute("value"), "\r\norder Floor");
            //VerifyAreEqual("customertest@mail.ru", driver.FindElement(By.Id("Order_OrderCustomer_Email")).GetAttribute("value"), "\r\norder Email");
            //VerifyAreEqual("+79277777777", driver.FindElement(By.Id("Order_OrderCustomer_Phone")).GetAttribute("value"), "\r\norder Phone");

            VerifyAreEqual(
                "Россия, Московская область, Москва, 111111, улица TestStreet1, д. 1, стр. 3, кв. 2, подъезд 4, эт. 5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerAdress\"]")).Text,
                "\r\norder FullCustomerAdress");
            VerifyAreEqual("customertest@mail.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerMail\"]")).Text, "\r\norder Email");
            VerifyAreEqual("79277777777", Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerTel\"]")).Text,
                "\r\norder Phone");
            VerifyAreEqual("TestSurname TestName TestPatronymic",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FullNameCustomer\"] a")).Text,
                "\r\norder FullCustomerName");

            IWebElement selectElem = Driver.FindElement(By.Id("Order_ManagerId"));
            SelectElement select = new SelectElement(selectElem);
            VerifyAreEqual("Администратор Магазина", (select.AllSelectedOptions[0].Text), "\r\norder Менеджер");
            selectElem = Driver.FindElement(By.Id("Order_OrderSourceId"));
            select = new SelectElement(selectElem);
            VerifyAreEqual("Оффлайн", (select.AllSelectedOptions[0].Text), "\r\norder Источник");

            VerifyAreEqual("TestProduct100\r\nАртикул: 100", Driver.GetGridCell(0, "Name", "OrderItems").Text,
                "order Name product at order");
            VerifyAreEqual("100", Driver.GetGridCell(0, "PriceString", "OrderItems").Text.ToString(),
                " order  product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").Text.ToString(),
                " order Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "OrderItems").Text.Contains("100"),
                "order Cost product at order");

            VerifyAreEqual("Comments orders", Driver.FindElement(By.Id("Order_StatusComment")).Text, "order Comment");
            VerifyAreEqual("Admin comment orders", Driver.FindElement(By.Id("Order_AdminOrderComment")).Text,
                "order AdminOrderComment");

            Driver.FindElement(By.LinkText("Изменить")).Click();
            Driver.WaitForElem(By.Name("popupOrderCustomerForm"));
            Driver.FindElement(By.LinkText("Карточка клиента")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            VerifyAreEqual("TestSurname TestName", Driver.FindElement(By.CssSelector("[data-e2e=\"clientName\"]")).Text,
                "customer cart h1");

            VerifyAreEqual("0 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"orderSum\"]")).Text,
                "customer cart orderSum");
            VerifyAreEqual("0 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"averageCheck\"]")).Text,
                "customer cart averageCheck");
            VerifyAreEqual("0д.", Driver.FindElement(By.CssSelector("[data-e2e=\"durationTime\"]")).Text,
                "customer cart durationTime");

            VerifyAreEqual("TestSurname TestName TestPatronymic",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientFio\"]")).Text, "customer cart clientFio ");
            VerifyAreEqual("TestOrganization",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientOrganization\"]")).Text,
                "customer cart organization ");
            VerifyAreEqual("customertest@mail.ru", Driver.FindElement(By.CssSelector("[data-e2e=\"clientMail\"]")).Text,
                "customer cart clientMail ");
            string str1 = Driver.FindElement(By.CssSelector("[data-e2e=\"clientAdress\"]")).Text;
            VerifyAreEqual("+7(927)777-77-77", Driver.FindElement(By.CssSelector("[data-e2e=\"clientPhone\"]")).Text,
                "customer cart clientPhone ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientAdress\"]")).Text
                    .Contains("Россия, Московская область, Москва"), "customer cart client Adress ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientAdress\"]")).Text
                    .Contains("улица TestStreet1, д. 1, кв. 2, стр. 3, подъезд 4, эт. 5"),
                "customer cart clientAdress city");
            VerifyAreEqual("Обычный покупатель",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroup\"]")).Text, "customer cart clientGroup ");
            //VerifyAreEqual("не указан", driver.FindElement(By.CssSelector("[data-e2e=\"clientManager\"]")).Text, "customer cart clientManager ");
            IWebElement selectElem1 = Driver.FindElement(By.Id("ManagerId"));
            SelectElement select1 = new SelectElement(selectElem1);
            VerifyAreEqual("-", (select1.AllSelectedOptions[0].Text), "\r\ncustomer Менеджер");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ClientInterests\"] span")).Text
                    .Contains("TestCategory4 (TestCategory5)"), "customer cart interests ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Text.Contains("1"),
                "customer cart count order ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Text.Contains("Заказы"),
                "customer cart order ");

            Driver.FindElement(By.LinkText("Редактировать")).Click();
            Driver.WaitForElem(By.TagName("customer-info"));
            VerifyAreEqual("TestSurname TestName TestPatronymic",
                Driver.FindElement(By.CssSelector(".lead-info")).FindElement(By.TagName("h1")).Text,
                "customer edit h1");
            VerifyAreEqual("TestSurname", Driver.FindElement(By.Id("Customer_LastName")).GetAttribute("value"),
                "customer edit LastName");
            VerifyAreEqual("TestName", Driver.FindElement(By.Id("Customer_FirstName")).GetAttribute("value"),
                "customer edit FirstName");
            VerifyAreEqual("TestPatronymic", Driver.FindElement(By.Id("Customer_Patronymic")).GetAttribute("value"),
                "customer edit Patronymic");
            VerifyAreEqual("TestOrganization", Driver.FindElement(By.Id("Customer_Organization")).GetAttribute("value"),
                "customer edit Organization");
            VerifyAreEqual("Россия", Driver.FindElement(By.Id("CustomerContact_Country")).GetAttribute("value"),
                "customer edit Country");
            VerifyAreEqual("Московская область",
                Driver.FindElement(By.Id("CustomerContact_Region")).GetAttribute("value"), "customer edit Region");
            VerifyAreEqual("Москва", Driver.FindElement(By.Id("CustomerContact_City")).GetAttribute("value"),
                "customer editCity");
            VerifyAreEqual("customertest@mail.ru", Driver.FindElement(By.Id("Customer_EMail")).GetAttribute("value"),
                "customer edit Email");
            VerifyAreEqual("111111", Driver.FindElement(By.Id("CustomerContact_Zip")).GetAttribute("value"),
                "customer edit Zip");
            VerifyAreEqual("+7(927)777-77-77", Driver.FindElement(By.Id("Customer_Phone")).GetAttribute("value"),
                "customer edit Phone");
            VerifyAreEqual("TestStreet1", Driver.FindElement(By.Id("CustomerContact_Street")).GetAttribute("value"),
                "customer edit Street");
            VerifyAreEqual("1", Driver.FindElement(By.Id("CustomerContact_House")).GetAttribute("value"),
                "customer edit House");
            VerifyAreEqual("2", Driver.FindElement(By.Id("CustomerContact_Apartment")).GetAttribute("value"),
                "customer edit Apartment");
            VerifyAreEqual("3", Driver.FindElement(By.Id("CustomerContact_Structure")).GetAttribute("value"),
                "customer edit Structure");
            VerifyAreEqual("4", Driver.FindElement(By.Id("CustomerContact_Entrance")).GetAttribute("value"),
                "customer edit Entrance");
            VerifyAreEqual("5", Driver.FindElement(By.Id("CustomerContact_Floor")).GetAttribute("value"),
                "customer edit Floor");
            Functions.CloseTab(Driver, BaseUrl);

            GoToAdmin("customers");
            VerifyAreEqual("TestOrganization", Driver.GetGridCell(0, "Name", "Customers").Text, "grid customer name");

            GoToAdmin("orders");
            Driver.GetGridCell(0, "Number").Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));
            Driver.FindElement(By.CssSelector(".switcher-state-label span")).Click();
            Driver.WaitForToastSuccess();
            
            Driver.FindElement(By.CssSelector("[data-e2e=\"FullNameCustomer\"] a")).Click();
            VerifyAreEqual("TestSurname TestName", Driver.FindElement(By.CssSelector("[data-e2e=\"clientName\"]")).Text,
                "customer cart h1");

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"orderSum\"]")).Text,
                "customer cart orderSum");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"averageCheck\"]")).Text,
                "customer cart averageCheck");
        }

        [Order(2)]
        [Test]
        public void OrdersAddCheckCustomer()
        {
            GoToAdmin("orders/add");

            VerifyAreEqual("Создание нового заказа",
                Driver.FindElement(By.TagName("h1")).FindElement(By.TagName("span")).Text, "OrdersAdd");
            (new SelectElement(Driver.FindElement(By.Id("Order_ManagerId")))).SelectByText("Администратор Магазина");
            (new SelectElement(Driver.FindElement(By.Id("Order_OrderSourceId")))).SelectByText("По телефону");
            Driver.FindElement(By.LinkText("Выбрать покупателя")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"selectUserGrid[0][\'_serviceColumn\']\"]"))
                .Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#orderForm .m-l-xs.link-invert")).Text
                    .Contains("TestName TestSurname"), "name client");
            VerifyAreEqual("TestSurname",
                Driver.FindElement(By.Id("Order_OrderCustomer_LastName")).GetAttribute("value"), " order LastName add");
            VerifyAreEqual("TestName", Driver.FindElement(By.Id("Order_OrderCustomer_FirstName")).GetAttribute("value"),
                "\r\norder FirstName add");
            VerifyAreEqual("TestPatronymic",
                Driver.FindElement(By.Id("Order_OrderCustomer_Patronymic")).GetAttribute("value"),
                "\r\norder Patronymic add");
            VerifyAreEqual("TestOrganization",
                Driver.FindElement(By.Id("Order_OrderCustomer_Organization")).GetAttribute("value"),
                "\r\norder Organization add");
            VerifyAreEqual("Россия", Driver.FindElement(By.Id("Order_OrderCustomer_Country")).GetAttribute("value"),
                "\r\norder Country add");
            VerifyAreEqual("Московская область",
                Driver.FindElement(By.Id("Order_OrderCustomer_Region")).GetAttribute("value"), "\r\norder Region add");
            VerifyAreEqual("Москва", Driver.FindElement(By.Id("Order_OrderCustomer_City")).GetAttribute("value"),
                "\r\norder City add");
            VerifyAreEqual("customertest@mail.ru",
                Driver.FindElement(By.Id("Order_OrderCustomer_Email")).GetAttribute("value"), "\r\norder Email add");
            VerifyAreEqual("111111", Driver.FindElement(By.Id("Order_OrderCustomer_Zip")).GetAttribute("value"),
                "\r\norder Zip add");
            VerifyAreEqual("+7(927)777-77-77",
                Driver.FindElement(By.Id("Order_OrderCustomer_Phone")).GetAttribute("value"), "\r\norder Phone add");
            VerifyAreEqual("TestStreet1", Driver.FindElement(By.Id("Order_OrderCustomer_Street")).GetAttribute("value"),
                "\r\norder Street add");
            VerifyAreEqual("1", Driver.FindElement(By.Id("Order_OrderCustomer_House")).GetAttribute("value"),
                "\r\norder House add");
            VerifyAreEqual("2", Driver.FindElement(By.Id("Order_OrderCustomer_Apartment")).GetAttribute("value"),
                "\r\norder Apartment add");
            VerifyAreEqual("3", Driver.FindElement(By.Id("Order_OrderCustomer_Structure")).GetAttribute("value"),
                "\r\norder Structure add");
            VerifyAreEqual("4", Driver.FindElement(By.Id("Order_OrderCustomer_Entrance")).GetAttribute("value"),
                "\r\norder Entrance add");
            VerifyAreEqual("5", Driver.FindElement(By.Id("Order_OrderCustomer_Floor")).GetAttribute("value"),
                "\r\norder Floor add");

            Driver.FindElement(By.CssSelector(".fas.fa-times")).Click();
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_LastName")).GetAttribute("value"),
                " order LastName del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_FirstName")).GetAttribute("value"),
                "\r\norder FirstName del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Patronymic")).GetAttribute("value"),
                "\r\norder Patronymic del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Organization")).GetAttribute("value"),
                "\r\norder Organization del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Country")).GetAttribute("value"),
                "\r\norder Country del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Region")).GetAttribute("value"),
                "\r\norder Region del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_City")).GetAttribute("value"),
                "\r\norder City del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Email")).GetAttribute("value"),
                "\r\norder Email del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Zip")).GetAttribute("value"),
                "\r\norder Zip del");
            VerifyAreEqual("+_(___)___-__-__",
                Driver.FindElement(By.Id("Order_OrderCustomer_Phone")).GetAttribute("value"), "\r\norder Phone del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Street")).GetAttribute("value"),
                "\r\norder Street del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_House")).GetAttribute("value"),
                "\r\norder House del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Apartment")).GetAttribute("value"),
                "\r\norder Apartment del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Structure")).GetAttribute("value"),
                "\r\norder Structure del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Entrance")).GetAttribute("value"),
                "\r\norder Entrance del");
            VerifyAreEqual("", Driver.FindElement(By.Id("Order_OrderCustomer_Floor")).GetAttribute("value"),
                "\r\norder Floor del");
        }

        [Order(1)]
        [Test]
        public void OrdersAddCustomer()
        {
            GoToAdmin("orders/add");

            VerifyAreEqual("Создание нового заказа",
                Driver.FindElement(By.TagName("h1")).FindElement(By.TagName("span")).Text, "OrdersAdd");
            (new SelectElement(Driver.FindElement(By.Id("Order_ManagerId")))).SelectByText("Администратор Магазина");
            (new SelectElement(Driver.FindElement(By.Id("Order_OrderSourceId")))).SelectByText("По телефону");
            Driver.FindElement(By.LinkText("Выбрать покупателя")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"selectUserGrid[0][\'_serviceColumn\']\"]"))
                .Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("TestSurname",
                Driver.FindElement(By.Id("Order_OrderCustomer_LastName")).GetAttribute("value"), " order LastName add");
            VerifyAreEqual("TestName", Driver.FindElement(By.Id("Order_OrderCustomer_FirstName")).GetAttribute("value"),
                "\r\norder FirstName add");
            VerifyAreEqual("TestPatronymic",
                Driver.FindElement(By.Id("Order_OrderCustomer_Patronymic")).GetAttribute("value"),
                "\r\norder Patronymic add");
            VerifyAreEqual("TestOrganization",
                Driver.FindElement(By.Id("Order_OrderCustomer_Organization")).GetAttribute("value"),
                "\r\norder Organization add");
            VerifyAreEqual("Россия", Driver.FindElement(By.Id("Order_OrderCustomer_Country")).GetAttribute("value"),
                "\r\norder Country add");
            VerifyAreEqual("Московская область",
                Driver.FindElement(By.Id("Order_OrderCustomer_Region")).GetAttribute("value"), "\r\norder Region add");
            VerifyAreEqual("Москва", Driver.FindElement(By.Id("Order_OrderCustomer_City")).GetAttribute("value"),
                "\r\norder City add");
            VerifyAreEqual("customertest@mail.ru",
                Driver.FindElement(By.Id("Order_OrderCustomer_Email")).GetAttribute("value"), "\r\norder Email add");
            VerifyAreEqual("111111", Driver.FindElement(By.Id("Order_OrderCustomer_Zip")).GetAttribute("value"),
                "\r\norder Zip add");
            VerifyAreEqual("+7(927)777-77-77",
                Driver.FindElement(By.Id("Order_OrderCustomer_Phone")).GetAttribute("value"), "\r\norder Phone add");
            VerifyAreEqual("TestStreet1", Driver.FindElement(By.Id("Order_OrderCustomer_Street")).GetAttribute("value"),
                "\r\norder Street add");
            VerifyAreEqual("1", Driver.FindElement(By.Id("Order_OrderCustomer_House")).GetAttribute("value"),
                "\r\norder House add");
            VerifyAreEqual("2", Driver.FindElement(By.Id("Order_OrderCustomer_Apartment")).GetAttribute("value"),
                "\r\norder Apartment add");
            VerifyAreEqual("3", Driver.FindElement(By.Id("Order_OrderCustomer_Structure")).GetAttribute("value"),
                "\r\norder Structure add");
            VerifyAreEqual("4", Driver.FindElement(By.Id("Order_OrderCustomer_Entrance")).GetAttribute("value"),
                "\r\norder Entrance add");
            VerifyAreEqual("5", Driver.FindElement(By.Id("Order_OrderCustomer_Floor")).GetAttribute("value"),
                "\r\norder Floor add");


            Driver.FindElement(By.LinkText("Добавить товар")).Click();
            Driver.GridFilterSendKeys("20");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr").Click();

            Driver.FindElement(By.CssSelector(".btn.btn-save.btn-primary")).Click();
            Driver.WaitForToastSuccess();
            VerifyAreEqual("TestProduct20\r\nАртикул: 20", Driver.GetGridCell(0, "Name", "OrderItems").Text,
                "Name product at order");
            VerifyAreEqual("200", Driver.GetGridCell(0, "PriceString", "OrderItems").Text, " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").Text, " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "OrderItems").Text.Contains("200"), " Cost product at order");

            Driver.FindElement(By.Id("Order_StatusComment")).SendKeys("Comments orders");
            Driver.FindElement(By.Id("Order_AdminOrderComment")).SendKeys("Admin comment orders");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();

            GoToAdmin("orders");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "StatusName").Text, " Grid orders StatusName");
            VerifyAreEqual("TestName TestSurname (TestOrganization)", Driver.GetGridCell(0, "BuyerName").Text,
                " Grid orders BuyerName");
            VerifyAreEqual("Администратор Магазина", Driver.GetGridCell(0, "ManagerName").Text,
                " Grid orders ManagerName");
            VerifyIsTrue(Driver.GetGridCell(0, "SumFormatted").Text.Contains("200"), " Grid orders SumFormatted");

            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Thread.Sleep(1000);
            //VerifyAreEqual("TestSurname", driver.FindElement(By.Id("Order_OrderCustomer_LastName")).GetAttribute("value"), " order LastName");
            //VerifyAreEqual("TestName", driver.FindElement(By.Id("Order_OrderCustomer_FirstName")).GetAttribute("value"), "\r\norder FirstName");
            //VerifyAreEqual("TestPatronymic", driver.FindElement(By.Id("Order_OrderCustomer_Patronymic")).GetAttribute("value"), "\r\norder Patronymic");
            //VerifyAreEqual("TestOrganization", driver.FindElement(By.Id("Order_OrderCustomer_Organization")).GetAttribute("value"), "\r\norder Organization");
            //VerifyAreEqual("Россия", driver.FindElement(By.Id("Order_OrderCustomer_Country")).GetAttribute("value"), "\r\norder Country");
            //VerifyAreEqual("Московская область", driver.FindElement(By.Id("Order_OrderCustomer_Region")).GetAttribute("value"), "\r\norder Region");
            //VerifyAreEqual("Москва", driver.FindElement(By.Id("Order_OrderCustomer_City")).GetAttribute("value"), "\r\norder City");
            //VerifyAreEqual("customertest@mail.ru", driver.FindElement(By.Id("Order_OrderCustomer_Email")).GetAttribute("value"), "\r\norder Email");
            //VerifyAreEqual("111111", driver.FindElement(By.Id("Order_OrderCustomer_Zip")).GetAttribute("value"), "\r\norder Zip");
            //VerifyAreEqual("+79277777777", driver.FindElement(By.Id("Order_OrderCustomer_Phone")).GetAttribute("value"), "\r\norder Phone");
            //VerifyAreEqual("TestStreet1", driver.FindElement(By.Id("Order_OrderCustomer_Street")).GetAttribute("value"), "\r\norder Street");
            //VerifyAreEqual("1", driver.FindElement(By.Id("Order_OrderCustomer_House")).GetAttribute("value"), "\r\norder House");
            //VerifyAreEqual("2", driver.FindElement(By.Id("Order_OrderCustomer_Apartment")).GetAttribute("value"), "\r\norder Apartment");
            //VerifyAreEqual("3", driver.FindElement(By.Id("Order_OrderCustomer_Structure")).GetAttribute("value"), "\r\norder Structure");
            //VerifyAreEqual("4", driver.FindElement(By.Id("Order_OrderCustomer_Entrance")).GetAttribute("value"), "\r\norder Entrance");
            //VerifyAreEqual("5", driver.FindElement(By.Id("Order_OrderCustomer_Floor")).GetAttribute("value"), "\r\norder Floor");
            VerifyAreEqual("TestSurname TestName TestPatronymic",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FullNameCustomer\"] a")).Text,
                "\r\norder FullCustomerName");
            VerifyAreEqual("customertest@mail.ru",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerMail\"]")).Text, "\r\norder Email");
            VerifyAreEqual("79277777777", Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerTel\"]")).Text,
                "\r\norder Phone");
            VerifyAreEqual(
                "Россия, Московская область, Москва, 111111, улица TestStreet1, д. 1, стр. 3, кв. 2, подъезд 4, эт. 5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerAdress\"]")).Text,
                "\r\norder FullCustomerAdress");

            IWebElement selectElem = Driver.FindElement(By.Id("Order_ManagerId"));
            SelectElement select = new SelectElement(selectElem);
            VerifyAreEqual("Администратор Магазина", (select.AllSelectedOptions[0].Text), "\r\norder Менеджер");
            selectElem = Driver.FindElement(By.Id("Order_OrderSourceId"));
            select = new SelectElement(selectElem);
            VerifyAreEqual("По телефону", (select.AllSelectedOptions[0].Text), "\r\norder Источник");

            VerifyAreEqual("TestProduct20\r\nАртикул: 20", Driver.GetGridCell(0, "Name", "OrderItems").Text,
                "order Name product at order");
            VerifyAreEqual("200", Driver.GetGridCell(0, "PriceString", "OrderItems").Text.ToString(),
                " order  product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "OrderItems").Text.ToString(),
                " order Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "OrderItems").Text.Contains("200"),
                "order Cost product at order");

            VerifyAreEqual("Comments orders", Driver.FindElement(By.Id("Order_StatusComment")).Text, "order Comment");
            VerifyAreEqual("Admin comment orders", Driver.FindElement(By.Id("Order_AdminOrderComment")).Text,
                "order AdminOrderComment");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FullNameCustomer\"] a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));

            VerifyAreEqual("TestSurname TestName", Driver.FindElement(By.CssSelector("[data-e2e=\"clientName\"]")).Text,
                "customer cart h1");

            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"orderSum\"]")).Text,
                "customer cart orderSum");
            VerifyAreEqual("100 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"averageCheck\"]")).Text,
                "customer cart averageCheck");
            VerifyAreEqual("0д.", Driver.FindElement(By.CssSelector("[data-e2e=\"durationTime\"]")).Text,
                "customer cart durationTime");

            VerifyAreEqual("TestSurname TestName TestPatronymic",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientFio\"]")).Text, "customer cart clientFio ");
            VerifyAreEqual("TestOrganization",
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientOrganization\"]")).Text,
                "customer cart organization ");
            VerifyAreEqual("customertest@mail.ru", Driver.FindElement(By.CssSelector("[data-e2e=\"clientMail\"]")).Text,
                "customer cart clientMail ");
            VerifyAreEqual("+7(927)777-77-77", Driver.FindElement(By.CssSelector("[data-e2e=\"clientPhone\"]")).Text,
                "customer cart clientPhone ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientAdress\"]")).Text
                    .Contains("Россия, Московская область, Москва"), "customer cart client Adress ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"clientAdress\"]")).Text
                    .Contains("улица TestStreet1, д. 1, кв. 2, стр. 3, подъезд 4, эт. 5"),
                "customer cart clientAdress city ");

            VerifyAreEqual("Обычный покупатель",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerGroup\"]")).Text, "customer cart clientGroup ");
            //VerifyAreEqual("не указан", driver.FindElement(By.CssSelector("[data-e2e=\"clientManager\"]")).Text, "customer cart clientManager ");
            IWebElement selectElem1 = Driver.FindElement(By.Id("ManagerId"));
            SelectElement select1 = new SelectElement(selectElem1);
            VerifyAreEqual("-", (select1.AllSelectedOptions[0].Text), "\r\ncustomer Менеджер");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ClientInterests\"]")).Text
                    .Contains("TestCategory4 (TestCategory5)"), "customer cart interests 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ClientInterests\"]")).Text.Contains("TestCategory1"),
                "customer cart interests 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Text.Contains("2"),
                "customer cart count order ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"TabOrders\"]")).Text.Contains("Заказы"),
                "customer cart order ");

            GoToAdmin("orders");
            Driver.GetGridCell(0, "Number").Click();
            Driver.WaitForElem(By.ClassName("order-header-item"));
            Driver.FindElement(By.CssSelector(".switcher-state-label span")).Click();
            Driver.WaitForToastSuccess();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FullNameCustomer\"] a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));

            VerifyAreEqual("TestSurname TestName", Driver.FindElement(By.CssSelector("[data-e2e=\"clientName\"]")).Text,
                "customer cart h1");

            VerifyAreEqual("300 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"orderSum\"]")).Text,
                "customer cart orderSum");
            VerifyAreEqual("150 руб.", Driver.FindElement(By.CssSelector("[data-e2e=\"averageCheck\"]")).Text,
                "customer cart averageCheck");

        }

        public static void DelElement(IWebDriver driver)
        {
            int count = driver.FindElements(By.CssSelector(".ui-grid-custom-service-icon")).Count;
            for (int i = 0; i < count; i++)
            {
                driver.FindElement(By.CssSelector(".ui-grid-custom-service-icon")).Click();
                driver.SwalConfirm();
            }
        }
    }
}