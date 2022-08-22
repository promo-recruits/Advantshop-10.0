using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadAddEditCustomerFieldsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\CustomerFields\\[Order].LeadItem.csv"
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
        public void LeadAddCustomerFields()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            //check only number input
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]")).SendKeys("tttttt");
            Driver.FindElement(By.CssSelector(".modal-header-title")).Click();
            //VerifyAreEqual("", driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]")).GetAttribute("value"), "add lead customer field only number");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]"))
                    .GetCssValue("border-color"), "add lead customer field only number border color");

            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            //check customer fields
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).SendKeys("LeadCustomerFieldLastName");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).SendKeys("LeadCustomerFieldFirstName");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).SendKeys("LeadCustomerFieldPatron");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).Click();
            Driver.ClearInput(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).SendKeys("+71231218888");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).SendKeys("mailtest321@mail.ru");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"LeadManager\"]")))).SelectByText(
                "test testov");

            //customer fileds
            (new SelectElement(Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 1\"]"))))
                .SelectByText("Customer Field 1 Value 4");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]")).Clear();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                .SendKeys("text customer field test");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]")).Clear();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]")).SendKeys("1000");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 4\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 4\"]")).Clear();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 4\"]"))
                .SendKeys("big text customer field test\r\nline 2 test test\r\nline3test test test");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"LeadNoProducts\"]"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadItemAdd\"]")).Click();
            Driver.WaitForModal();

            Driver.XPathContainsText("span", "TestCategory1");

            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.XPathContainsText("button", "Выбрать");
            Thread.Sleep(100);

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadAdd\"]")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            //check admin grid
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetGridFilterTab(0, "LeadCustomerFieldFirstName");

            VerifyAreEqual("121", Driver.GetGridCell(0, "Id").Text, "lead added grid number");
            VerifyAreEqual("LeadCustomerFieldLastName LeadCustomerFieldFirstName LeadCustomerFieldPatron",
                Driver.GetGridCell(0, "FullName").Text, "lead added grid full name");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text, "lead grid DealStatusName");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName").Text, "lead grid added manager");
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductsCount").Text, "lead grid added products count");
            VerifyAreEqual("2 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "lead grid added sum");

            //check admin lead details
            Driver.GetGridCell(0, "Id").Click();
            Driver.WaitForElem(AdvBy.DataE2E("leadInfoTitle"));

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"lead_DealStatus\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Новый"), "lead details deal status");

            VerifyAreEqual("Лид №121", Driver.FindElement(By.CssSelector("[data-e2e=\"leadInfoTitle\"]")).Text,
                "lead no description");
            VerifyAreEqual("2", Driver.FindElement(By.Id("Lead_Sum")).GetAttribute("value"), "lead details sum");

            selectElem = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "lead manager");

            selectElem = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Другое"), "lead order source");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct2"), "lead product grid");

            //check customer info
            VerifyAreEqual("LeadCustomerFieldLastName",
                Driver.FindElement(By.Id("Lead_LastName")).GetAttribute("value"), "customer info last name");
            VerifyAreEqual("LeadCustomerFieldFirstName",
                Driver.FindElement(By.Id("Lead_FirstName")).GetAttribute("value"), "customer info first name");
            VerifyAreEqual("LeadCustomerFieldPatron",
                Driver.FindElement(By.Id("Lead_Patronymic")).GetAttribute("value"), "customer info patrominic");
            VerifyAreEqual("mailtest321@mail.ru", Driver.FindElement(By.Id("Lead_Email")).GetAttribute("value"),
                "customer info email");
            VerifyAreEqual("+7(123)121-88-88", Driver.FindElement(By.Id("Lead_Phone")).GetAttribute("value"),
                "customer info phone");

            //check customer fields
            selectElem = Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 1\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Customer Field 1 Value 4"), "customer info field select");

            VerifyAreEqual("text customer field test",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                    .GetAttribute("value"), "customer info field test");
            VerifyAreEqual("1000",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]"))
                    .GetAttribute("value"), "customer info field number");
            VerifyAreEqual("big text customer field test\r\nline 2 test test\r\nline3test test test",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 4\"]"))
                    .GetAttribute("value"), "customer info field big text");

            //check new customer
            GoToAdmin("customers");

            Driver.GridFilterSendKeys("LeadCustomerFieldFirstName");

            VerifyAreEqual("LeadCustomerFieldLastName LeadCustomerFieldFirstName LeadCustomerFieldPatron",
                Driver.GetGridCell(0, "Name", "Customers").Text, "new customer name grid saved");
            VerifyAreEqual("71231218888", Driver.GetGridCell(0, "Phone", "Customers").Text,
                "new customer Phone grid saved");
            VerifyAreEqual("mailtest321@mail.ru", Driver.GetGridCell(0, "Email", "Customers").Text,
                "new customer Email grid saved");
            VerifyAreEqual("0", Driver.GetGridCell(0, "OrdersCount", "Customers").Text,
                "new customer OrdersCount grid saved");
            VerifyAreEqual("", Driver.GetGridCell(0, "LastOrderNumber", "Customers").Text,
                "new customer LastOrderNumber grid saved");
            VerifyAreEqual("0", Driver.GetGridCell(0, "OrdersSum", "Customers").Text,
                "new customer OrdersSum grid saved");
            VerifyAreEqual("", Driver.GetGridCell(0, "ManagerName", "Customers").Text,
                "new customer lead's ManagerName, not customer");

            //check new customer cart lead grid
            Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Click();
            Driver.WaitForElem(AdvBy.DataE2E("clientName"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"TabLeads\"]")).Click();
            Thread.Sleep(100);

            VerifyAreEqual("121", Driver.GetGridCell(0, "Id", "CustomerLeads").Text, "customer cart grid lead id");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName", "CustomerLeads").Text,
                "customer cart grid lead deal status");
            VerifyAreEqual("LeadCustomerFieldLastName LeadCustomerFieldFirstName LeadCustomerFieldPatron",
                Driver.GetGridCell(0, "FullName", "CustomerLeads").Text, "customer cart grid lead full name");
            VerifyAreEqual("2", Driver.GetGridCell(0, "Sum", "CustomerLeads").Text, "customer cart grid lead sum");
            VerifyAreEqual("test testov", Driver.GetGridCell(0, "ManagerName", "CustomerLeads").Text,
                "customer cart grid lead manager");

            //check new customer edit
            Driver.FindElement(By.LinkText("Редактировать")).Click();
            Driver.WaitForElem(By.ClassName("order-header-item-name"));

            VerifyAreEqual("LeadCustomerFieldLastName",
                Driver.FindElement(By.Id("Customer_LastName")).GetAttribute("value"),
                "new customer first name edit saved");
            VerifyAreEqual("LeadCustomerFieldFirstName",
                Driver.FindElement(By.Id("Customer_FirstName")).GetAttribute("value"),
                "new customer last name edit saved");
            VerifyAreEqual("LeadCustomerFieldPatron",
                Driver.FindElement(By.Id("Customer_Patronymic")).GetAttribute("value"),
                "new customer patronymic edit saved");
            VerifyAreEqual("mailtest321@mail.ru", Driver.FindElement(By.Id("Customer_EMail")).GetAttribute("value"),
                "new customer email edit saved");
            VerifyAreEqual("+7(123)121-88-88", Driver.FindElement(By.Id("Customer_Phone")).GetAttribute("value"),
                "new customer phone edit saved");

            //check new customer edit fields
            VerifyAreEqual("text customer field test",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                    .GetAttribute("value"), "customer edit field text");
            VerifyAreEqual("1000",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]"))
                    .GetAttribute("value"), "customer edit field number");
            VerifyAreEqual("big text customer field test\r\nline 2 test test\r\nline3test test test",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 4\"]"))
                    .GetAttribute("value"), "customer edit field big text");

            selectElem = Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 1\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Customer Field 1 Value 4"), "customer edit field select");
        }

        [Test]
        public void LeadAddCustomerFieldDelete()
        {
            GoToAdmin("leads?salesFunnelId=-1");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).SendKeys("Last");

            Driver.XPathContainsText("span", "LastName FirstName Patronymic, mail@mail.com +7 495 800 20 01");

            Driver.XPathContainsText("h2", "Новый лид");

            VerifyAreEqual("LastName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).GetAttribute("value"),
                "existing customer LastName");
            VerifyAreEqual("FirstName",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).GetAttribute("value"),
                "existing customer FirstName");
            VerifyAreEqual("Patronymic",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).GetAttribute("value"),
                "existing customer Patronymic");
            VerifyAreEqual("+7(495)800-20-01",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).GetAttribute("value"),
                "existing customer PhoneNum");
            VerifyAreEqual("mail@mail.com",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).GetAttribute("value"),
                "existing customer Email");

            //customer fields
            IWebElement selectElem = Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 1\"]"));
            SelectElement select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Customer Field 1 Value 3"),
                "existing customer field select");
            VerifyAreEqual("pre check дополнительное поле текст",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                    .GetAttribute("value"), "existing customer field text");
            VerifyAreEqual("123123",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]"))
                    .GetAttribute("value"), "existing customer field number");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 4\"]"))
                    .GetAttribute("value"), "existing customer field big text");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".modal-content"))
                    .FindElement(By.CssSelector("[data-e2e=\"LeadCustomer\"]")).Text
                    .Contains("FirstName LastName 74958002001 mail@mail.com"), "customer added info");

            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadCustomerDelete\"]")).Click();
            Thread.Sleep(100);

            //check delete customer
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadLastName\"]")).GetAttribute("value"),
                "existing customer LastName deleted");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadFirstName\"]")).GetAttribute("value"),
                "existing customer FirstName deleted");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPatronymic\"]")).GetAttribute("value"),
                "existing customer Patronymic deleted");
            VerifyAreEqual("+_(___)___-__-__",
                Driver.FindElement(By.CssSelector("[data-e2e=\"LeadPhoneNum\"]")).GetAttribute("value"),
                "existing customer PhoneNum deleted");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"LeadEmail\"]")).GetAttribute("value"),
                "existing customer Email deleted");

            //check delete customer fields
            selectElem = Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 1\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Customer Field 1 Value 1"),
                "existing customer field select deleted");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                    .GetAttribute("value"), "existing customer field text deleted");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]"))
                    .GetAttribute("value"), "existing customer field number deleted");
            VerifyAreEqual("",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 4\"]"))
                    .GetAttribute("value"), "existing customer field big text deleted");
        }
    }
}