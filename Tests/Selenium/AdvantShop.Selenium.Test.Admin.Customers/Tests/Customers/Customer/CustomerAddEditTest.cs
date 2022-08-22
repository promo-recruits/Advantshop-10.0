using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.Customer
{
    [TestFixture]
    public class CustomerAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Orders | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Customers\\CustomerAddEdit\\Customers.CustomerGroup.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Customers.Customer.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Customers.Contact.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Customers.Departments.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Customers.Managers.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Customers.CustomerField.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Catalog.Product.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Catalog.Offer.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Catalog.Category.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Catalog.ProductCategories.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\[Order].OrderContact.csv",
                "Data\\Admin\\Customers\\CustomerAddEdit\\[Order].OrderSource.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\[Order].OrderStatus.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\[Order].[Order].csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\[Order].OrderCurrency.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\[Order].OrderItems.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\[Order].OrderCustomer.csv"
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
        public void CustomerAdd()
        {
            GoToAdmin("customers");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddNewClient\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".lead-info-inner"));

            VerifyAreEqual("Новый покупатель",
                Driver.FindElement(By.CssSelector(".lead-info-inner")).FindElement(By.TagName("h1"))
                    .FindElement(By.TagName("span")).Text, "add customer h1");

            Driver.SendKeysInput(By.Id("Customer_LastName"), "TestSurname");
            
            Driver.SendKeysInput(By.Id("Customer_FirstName"), "TestName");

            Driver.SendKeysInput(By.Id("Customer_Patronymic"), "TestPatronymic");

            Driver.SendKeysInput(By.Id("CustomerContact_Country"), "Россия");

            Driver.SendKeysInput(By.Id("CustomerContact_City"), "Москва");

            //Регион автоматически проставляется для города
            //Driver.SendKeysInput(By.Id("CustomerContact_Region"), "Московская область");

            Driver.SendKeysInput(By.Id("Customer_EMail"), "customertest@mail.ru");

            Driver.SendKeysInput(By.Id("CustomerContact_Zip"), "111111");

            Driver.SendKeysInput(By.Id("Customer_Phone"), "+79277777777");

            Driver.SendKeysInput(By.Id("CustomerContact_Street"), "улица Мира");

            Driver.SendKeysInput(By.Id("Customer_Password"), "123123");

            Driver.SendKeysInput(By.Id("CustomerContact_House"), "1");

            Driver.SendKeysInput(By.Id("CustomerContact_Apartment"), "2");

            Driver.SendKeysInput(By.Id("CustomerContact_Structure"), "3");

            Driver.SendKeysInput(By.Id("CustomerContact_Entrance"), "4");

            Driver.SendKeysInput(By.Id("CustomerContact_Floor"), "5");

            //check all managers
            IWebElement selectElemManager = Driver.FindElement(By.Id("Customer_ManagerId"));
            SelectElement select = new SelectElement(selectElemManager);

            IList<IWebElement> allOptionsManager = select.Options;

            VerifyIsTrue(allOptionsManager.Count == 3, "count managers"); //2 managers + null select

            IWebElement selectElem = Driver.FindElement(By.Id("Customer_ManagerId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("-"), "no manager ay first");

            (new SelectElement(Driver.FindElement(By.Id("Customer_ManagerId")))).SelectByText("test testov");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SubscribedNews\"] span")).Click();

            //check all customer groups
            IWebElement selectElemCustomerGroup = Driver.FindElement(By.Id("Customer_CustomerGroupId"));
            select = new SelectElement(selectElemCustomerGroup);

            IList<IWebElement> allOptionsCustomerGroup = select.Options;

            VerifyIsTrue(allOptionsCustomerGroup.Count == 2, "count customer groups"); //2 customer groups

            (new SelectElement(Driver.FindElement(By.Id("Customer_CustomerGroupId")))).SelectByValue("2");

            Driver.SendKeysInput(By.Id("Customer_AdminComment"), "Customer Admin Comment Test");

            //customer fileds
            Driver.SendKeysInput(By.CssSelector("[validation-input-text=\"Customer Field 3\"]"), "999");

            Driver.SendKeysInput(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"), "дополнительное поле");

            Functions.DataTimePicker(Driver, BaseUrl, month: "Август", year: "2014", data: "Август 21, 2014",
                field: "[validation-input-text=\"Customer Field 6\"]", dropFocusElem: "customer-info");

            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 1"), "customer field not required 1");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 4"), "customer field not required 2");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 5"), "customer field not disabled");

            Driver.GetButton(EButtonType.Save).Click();

            //check admin
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestName TestSurname");
            VerifyAreEqual("TestSurname TestName TestPatronymic", Driver.GetGridCell(0, "Name", "Customers").Text,
                "grid customer name");

            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));

            VerifyIsTrue(Driver.FindElement(By.Id("Customer_SubscribedForNews")).Selected, "subscribed for news");

            IWebElement selectElemEditManager = Driver.FindElement(By.Id("Customer_ManagerId"));
            select = new SelectElement(selectElemEditManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "manager");

            IWebElement selectElemEditCustomerGroup = Driver.FindElement(By.Id("Customer_CustomerGroupId"));
            select = new SelectElement(selectElemEditCustomerGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("CustomerGroup2 - 10%"), "customer group");

            VerifyAreEqual("Customer Admin Comment Test",
                Driver.FindElement(By.Id("Customer_AdminComment")).GetAttribute("value"), "admin's comment");

            VerifyAreEqual("TestSurname", Driver.FindElement(By.Id("Customer_LastName")).GetAttribute("value"),
                "last name");
            VerifyAreEqual("TestName", Driver.FindElement(By.Id("Customer_FirstName")).GetAttribute("value"),
                "first name");
            VerifyAreEqual("TestPatronymic", Driver.FindElement(By.Id("Customer_Patronymic")).GetAttribute("value"),
                "patronymic");
            VerifyAreEqual("Россия", Driver.FindElement(By.Id("CustomerContact_Country")).GetAttribute("value"),
                "country");
            VerifyAreEqual("Москва",
                Driver.FindElement(By.Id("CustomerContact_Region")).GetAttribute("value"), "region");
            VerifyAreEqual("Москва", Driver.FindElement(By.Id("CustomerContact_City")).GetAttribute("value"), "Город");
            VerifyAreEqual("customertest@mail.ru", Driver.FindElement(By.Id("Customer_EMail")).GetAttribute("value"),
                "Email");
            VerifyAreEqual("111111", Driver.FindElement(By.Id("CustomerContact_Zip")).GetAttribute("value"), "zip");
            VerifyAreEqual("+7(927)777-77-77", Driver.FindElement(By.Id("Customer_Phone")).GetAttribute("value"),
                "phone number");
            VerifyAreEqual("улица Мира", Driver.FindElement(By.Id("CustomerContact_Street")).GetAttribute("value"),
                "street");
            VerifyAreEqual("1", Driver.FindElement(By.Id("CustomerContact_House")).GetAttribute("value"), "house");
            VerifyAreEqual("2", Driver.FindElement(By.Id("CustomerContact_Apartment")).GetAttribute("value"),
                "appartment");
            VerifyAreEqual("3", Driver.FindElement(By.Id("CustomerContact_Structure")).GetAttribute("value"),
                "structure");
            VerifyAreEqual("4", Driver.FindElement(By.Id("CustomerContact_Entrance")).GetAttribute("value"),
                "entrance");
            VerifyAreEqual("5", Driver.FindElement(By.Id("CustomerContact_Floor")).GetAttribute("value"), "floor");

            //check customer fields
            VerifyAreEqual("999",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]"))
                    .GetAttribute("value"), "customer filed number");
            VerifyAreEqual("дополнительное поле",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                    .GetAttribute("value"), "customer filed text");
            VerifyAreEqual("21.08.2014",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 6\"]"))
                    .GetAttribute("value"), "customer field date");
        }

        [Test]
        public void CustomerEdit()
        {
            GoToAdmin("customers");

            VerifyAreEqual("Покупатели", Driver.FindElement(By.TagName("h1")).Text, "h1 customer grid");

            Driver.GridFilterSendKeys("FirstName");
            VerifyAreEqual("LastName FirstName Patronymic", Driver.GetGridCell(0, "Name", "Customers").Text,
                "customer name grid before edit");

            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));

            //pre check
            VerifyIsTrue(!Driver.FindElement(By.Id("Customer_SubscribedForNews")).Selected,
                "pre check subscribe for news");
            VerifyAreEqual("LastName", Driver.FindElement(By.Id("Customer_LastName")).GetAttribute("value"),
                "pre check last name");
            VerifyAreEqual("FirstName", Driver.FindElement(By.Id("Customer_FirstName")).GetAttribute("value"),
                "pre check first name");
            VerifyAreEqual("Patronymic", Driver.FindElement(By.Id("Customer_Patronymic")).GetAttribute("value"),
                "pre check patromymic");
            VerifyAreEqual("Россия", Driver.FindElement(By.Id("CustomerContact_Country")).GetAttribute("value"),
                "pre check Страна");
            VerifyAreEqual("Московская область",
                Driver.FindElement(By.Id("CustomerContact_Region")).GetAttribute("value"), "pre check Регион");
            VerifyAreEqual("Москва", Driver.FindElement(By.Id("CustomerContact_City")).GetAttribute("value"),
                "pre check сшен");
            VerifyAreEqual("mail@mail.com", Driver.FindElement(By.Id("Customer_EMail")).GetAttribute("value"),
                "pre check Email");
            VerifyAreEqual("222222", Driver.FindElement(By.Id("CustomerContact_Zip")).GetAttribute("value"),
                "pre check zip");
            VerifyAreEqual("+7(495)800-20-01", Driver.FindElement(By.Id("Customer_Phone")).GetAttribute("value"),
                "pre check phone number");
            VerifyAreEqual("Улица", Driver.FindElement(By.Id("CustomerContact_Street")).GetAttribute("value"),
                "pre check street");
            VerifyAreEqual("1", Driver.FindElement(By.Id("CustomerContact_House")).GetAttribute("value"),
                "pre check house");
            VerifyAreEqual("2", Driver.FindElement(By.Id("CustomerContact_Apartment")).GetAttribute("value"),
                "pre check apartment");
            VerifyAreEqual("3", Driver.FindElement(By.Id("CustomerContact_Structure")).GetAttribute("value"),
                "pre check structure");
            VerifyAreEqual("4", Driver.FindElement(By.Id("CustomerContact_Entrance")).GetAttribute("value"),
                "pre check entrance");
            VerifyAreEqual("5", Driver.FindElement(By.Id("CustomerContact_Floor")).GetAttribute("value"),
                "pre check floor");

            IWebElement selectElemManager = Driver.FindElement(By.Id("Customer_ManagerId"));
            SelectElement select = new SelectElement(selectElemManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "pre check manager");

            IWebElement selectElemCustomerGroup = Driver.FindElement(By.Id("Customer_CustomerGroupId"));
            select = new SelectElement(selectElemCustomerGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("CustomerGroup2 - 10%"), "pre check customer group");

            VerifyAreEqual("Admin Comment Deafult",
                Driver.FindElement(By.Id("Customer_AdminComment")).GetAttribute("value"), "pre check admin's comment");

            //pre check customer fields
            VerifyAreEqual("123123",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]"))
                    .GetAttribute("value"), "pre check customer field number");
            VerifyAreEqual("pre check дополнительное поле текст",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                    .GetAttribute("value"), "pre check customer field text");
            VerifyAreEqual("12.12.2017",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 6\"]"))
                    .GetAttribute("value"), "pre check customer field date");

            IWebElement selectElemField =
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 1\"]"));
            select = new SelectElement(selectElemField);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Customer Field 1 Value 3"),
                "pre check customer field select");

            //edit
            Driver.FindElement(By.Id("Customer_LastName")).Click();
            Driver.FindElement(By.Id("Customer_LastName")).Clear();
            Driver.FindElement(By.Id("Customer_LastName")).SendKeys("changedSurname");

            Driver.FindElement(By.Id("Customer_FirstName")).Click();
            Driver.FindElement(By.Id("Customer_FirstName")).Clear();
            Driver.FindElement(By.Id("Customer_FirstName")).SendKeys("changedName");

            Driver.FindElement(By.Id("Customer_Patronymic")).Click();
            Driver.FindElement(By.Id("Customer_Patronymic")).Clear();
            Driver.FindElement(By.Id("Customer_Patronymic")).SendKeys("changedPatronymic");

            Driver.FindElement(By.Id("CustomerContact_Country")).Click();
            Driver.FindElement(By.Id("CustomerContact_Country")).Clear();
            Driver.FindElement(By.Id("CustomerContact_Country")).SendKeys("Страна");

            Driver.FindElement(By.Id("CustomerContact_Region")).Click();
            Driver.FindElement(By.Id("CustomerContact_Region")).Clear();
            Driver.FindElement(By.Id("CustomerContact_Region")).SendKeys("Регион");

            Driver.FindElement(By.Id("CustomerContact_City")).Click();
            Driver.FindElement(By.Id("CustomerContact_City")).Clear();
            Driver.FindElement(By.Id("CustomerContact_City")).SendKeys("Ульяновск");

            Driver.FindElement(By.Id("Customer_EMail")).Click();
            Driver.FindElement(By.Id("Customer_EMail")).Clear();
            Driver.FindElement(By.Id("Customer_EMail")).SendKeys("editedtest@mail.ru");

            Driver.FindElement(By.Id("CustomerContact_Zip")).Click();
            Driver.FindElement(By.Id("CustomerContact_Zip")).Clear();
            Driver.FindElement(By.Id("CustomerContact_Zip")).SendKeys("555555");

            Driver.FindElement(By.Id("Customer_Phone")).Click();
            Driver.ClearInput(By.Id("Customer_Phone"));
            Driver.FindElement(By.Id("Customer_Phone")).SendKeys("+79308888888");

            Driver.FindElement(By.Id("CustomerContact_Street")).Click();
            Driver.FindElement(By.Id("CustomerContact_Street")).Clear();
            Driver.FindElement(By.Id("CustomerContact_Street")).SendKeys("улица Мира");

            Driver.FindElement(By.Id("CustomerContact_House")).Click();
            Driver.FindElement(By.Id("CustomerContact_House")).Clear();
            Driver.FindElement(By.Id("CustomerContact_House")).SendKeys("5");

            Driver.FindElement(By.Id("CustomerContact_Apartment")).Click();
            Driver.FindElement(By.Id("CustomerContact_Apartment")).Clear();
            Driver.FindElement(By.Id("CustomerContact_Apartment")).SendKeys("4");

            Driver.FindElement(By.Id("CustomerContact_Structure")).Click();
            Driver.FindElement(By.Id("CustomerContact_Structure")).Clear();
            Driver.FindElement(By.Id("CustomerContact_Structure")).SendKeys("2");

            Driver.FindElement(By.Id("CustomerContact_Entrance")).Click();
            Driver.FindElement(By.Id("CustomerContact_Entrance")).Clear();
            Driver.FindElement(By.Id("CustomerContact_Entrance")).SendKeys("3");

            Driver.FindElement(By.Id("CustomerContact_Floor")).Click();
            Driver.FindElement(By.Id("CustomerContact_Floor")).Clear();
            Driver.FindElement(By.Id("CustomerContact_Floor")).SendKeys("1");

            (new SelectElement(Driver.FindElement(By.Id("Customer_ManagerId")))).SelectByText("Elena El");

            (new SelectElement(Driver.FindElement(By.Id("Customer_CustomerGroupId")))).SelectByValue("1");

            Driver.FindElement(By.Id("Customer_AdminComment")).Click();
            Driver.FindElement(By.Id("Customer_AdminComment")).Clear();
            Driver.FindElement(By.Id("Customer_AdminComment")).SendKeys("Edited Test Comment");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SubscribedNews\"] span")).Click();

            //customer fileds
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]")).SendKeys("5000");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]")).Clear();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                .SendKeys("отредактированное доп. поле тест");

            (new SelectElement(Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 1\"]"))))
                .SelectByText("Customer Field 1 Value 5");

            Functions.DataTimePicker(Driver, BaseUrl, month: "Август", year: "2013", data: "Август 11, 2013",
                field: "[validation-input-text=\"Customer Field 6\"]", dropFocusElem: "customer-info");

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("FirstName LastName");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "grid previous customer");

            Driver.GridFilterSendKeys("changedName changedSurname");
            VerifyAreEqual("changedSurname changedName changedPatronymic",
                Driver.GetGridCell(0, "Name", "Customers").Text, "grid customer name");

            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));

            IWebElement selectElemEditManager = Driver.FindElement(By.Id("Customer_ManagerId"));
            select = new SelectElement(selectElemEditManager);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Elena El"), "manager");

            IWebElement selectElemEditCustomerGroup = Driver.FindElement(By.Id("Customer_CustomerGroupId"));
            select = new SelectElement(selectElemEditCustomerGroup);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Обычный покупатель - 0%"), "customer group");

            VerifyAreEqual("Edited Test Comment",
                Driver.FindElement(By.Id("Customer_AdminComment")).GetAttribute("value"), "admin's comment");

            VerifyIsTrue(Driver.FindElement(By.Id("Customer_SubscribedForNews")).Selected,
                "customer subscribed for news");
            VerifyAreEqual("changedSurname", Driver.FindElement(By.Id("Customer_LastName")).GetAttribute("value"),
                "last name");
            VerifyAreEqual("changedName", Driver.FindElement(By.Id("Customer_FirstName")).GetAttribute("value"),
                "first name");
            VerifyAreEqual("changedPatronymic", Driver.FindElement(By.Id("Customer_Patronymic")).GetAttribute("value"),
                "patronymic");
            VerifyAreEqual("Россия", Driver.FindElement(By.Id("CustomerContact_Country")).GetAttribute("value"),
                "country");
            VerifyAreEqual("Ульяновская область",
                Driver.FindElement(By.Id("CustomerContact_Region")).GetAttribute("value"), "region");
            VerifyAreEqual("Ульяновск", Driver.FindElement(By.Id("CustomerContact_City")).GetAttribute("value"),
                "city");
            VerifyAreEqual("editedtest@mail.ru", Driver.FindElement(By.Id("Customer_EMail")).GetAttribute("value"),
                "Email");
            VerifyAreEqual("555555", Driver.FindElement(By.Id("CustomerContact_Zip")).GetAttribute("value"), "zip");
            VerifyAreEqual("+7(930)888-88-88", Driver.FindElement(By.Id("Customer_Phone")).GetAttribute("value"),
                "phone number");
            VerifyAreEqual("улица Мира", Driver.FindElement(By.Id("CustomerContact_Street")).GetAttribute("value"),
                "street");
            VerifyAreEqual("5", Driver.FindElement(By.Id("CustomerContact_House")).GetAttribute("value"), "house");
            VerifyAreEqual("4", Driver.FindElement(By.Id("CustomerContact_Apartment")).GetAttribute("value"),
                "apartment");
            VerifyAreEqual("2", Driver.FindElement(By.Id("CustomerContact_Structure")).GetAttribute("value"),
                "structure");
            VerifyAreEqual("3", Driver.FindElement(By.Id("CustomerContact_Entrance")).GetAttribute("value"),
                "entrance");
            VerifyAreEqual("1", Driver.FindElement(By.Id("CustomerContact_Floor")).GetAttribute("value"), "floor");

            //check customer fields
            VerifyAreEqual("5000",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]"))
                    .GetAttribute("value"), "customer field number edited");
            VerifyAreEqual("отредактированное доп. поле тест",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                    .GetAttribute("value"), "customer field text edited");

            IWebElement selectElemFieldEdit =
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 1\"]"));
            select = new SelectElement(selectElemFieldEdit);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Customer Field 1 Value 5"),
                "customer field select edited");

            VerifyAreEqual("11.08.2013",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 6\"]"))
                    .GetAttribute("value"), "customer field date edited");
        }
    }

    [TestFixture]
    public class CustomerOrganizationAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Customers\\CustomerAddEdit\\Customers.CustomerGroup.csv",
                "data\\Admin\\Customers\\CustomerAddEdit\\Customers.Customer.csv"
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
        public void CustomerOrganizationAdd()
        {
            GoToAdmin("customers");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddNewClient\"]")).Click();
            Driver.WaitForElem(By.CssSelector(".lead-info-inner"));

            Driver.FindElement(By.Id("Customer_LastName")).Click();
            Driver.FindElement(By.Id("Customer_LastName")).SendKeys("Surname");

            Driver.FindElement(By.Id("Customer_FirstName")).Click();
            Driver.FindElement(By.Id("Customer_FirstName")).SendKeys("Name");

            Driver.FindElement(By.Id("Customer_Patronymic")).Click();
            Driver.FindElement(By.Id("Customer_Patronymic")).SendKeys("Patronymic");

            Driver.FindElement(By.Id("Customer_Organization")).Click();
            Driver.FindElement(By.Id("Customer_Organization")).SendKeys("Organizations");

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));

            //check admin
            GoToAdmin("customers");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCustomers\"]")).Text.Contains("Name Surname"),
                "no name in grid");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridCustomers\"]")).Text.Contains("Organizations"),
                "organization name in grid");

            Driver.GridFilterSendKeys("Organizations");
            VerifyAreEqual("Organizations", Driver.GetGridCell(0, "Name", "Customers").Text,
                "grid customer organization");

            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));

            VerifyAreEqual("Organizations", Driver.FindElement(By.Id("Customer_Organization")).GetAttribute("value"),
                "organization name");
        }

        [Test]
        public void CustomerOrganizationEdit()
        {
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestOrganization");
            VerifyAreEqual("TestOrganization", Driver.GetGridCell(0, "Name", "Customers").Text,
                "organization name grid before edit");

            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));

            //pre check
            VerifyAreEqual("TestSurname", Driver.FindElement(By.Id("Customer_LastName")).GetAttribute("value"),
                "pre check last name");
            VerifyAreEqual("TestName", Driver.FindElement(By.Id("Customer_FirstName")).GetAttribute("value"),
                "pre check first name");
            VerifyAreEqual("TestPatronymic", Driver.FindElement(By.Id("Customer_Patronymic")).GetAttribute("value"),
                "pre check patromymic");
            VerifyAreEqual("TestOrganization", Driver.FindElement(By.Id("Customer_Organization")).GetAttribute("value"),
                "pre check organization");

            //edit
            Driver.FindElement(By.Id("Customer_Organization")).Click();
            Driver.FindElement(By.Id("Customer_Organization")).Clear();
            Driver.FindElement(By.Id("Customer_Organization")).SendKeys("Edited Organiz");

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("TestOrganization");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "grid previous organization");

            Driver.GridFilterSendKeys("Edited Organiz");
            VerifyAreEqual("Edited Organiz", Driver.GetGridCell(0, "Name", "Customers").Text, "grid organization name");

            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));

            VerifyAreEqual("TestSurname", Driver.FindElement(By.Id("Customer_LastName")).GetAttribute("value"),
                "last name");
            VerifyAreEqual("TestName", Driver.FindElement(By.Id("Customer_FirstName")).GetAttribute("value"),
                "first name");
            VerifyAreEqual("TestPatronymic", Driver.FindElement(By.Id("Customer_Patronymic")).GetAttribute("value"),
                "patromymic");
            VerifyAreEqual("Edited Organiz", Driver.FindElement(By.Id("Customer_Organization")).GetAttribute("value"),
                "edited organization");
        }
    }
}