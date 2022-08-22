using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.CRM.Tests.LeadTable
{
    [TestFixture]
    public class CRMLeadToOrderTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CRM | ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\CRM\\Lead\\Catalog.Product.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Offer.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.Category.csv",
                "data\\Admin\\CRM\\Lead\\Catalog.ProductCategories.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Customer.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerField.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\CRM\\Lead\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Departments.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Managers.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Contact.csv",
                "data\\Admin\\CRM\\Lead\\CRM.DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel.csv",
                "data\\Admin\\CRM\\Lead\\CRM.SalesFunnel_DealStatus.csv",
                "data\\Admin\\CRM\\Lead\\[Order].OrderSource.csv",
                "data\\Admin\\CRM\\Lead\\[Order].Lead.csv",
                "data\\Admin\\CRM\\Lead\\Customers.TaskGroup.csv",
                "data\\Admin\\CRM\\Lead\\Customers.Task.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadCurrency.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadEvent.csv",
                "data\\Admin\\CRM\\Lead\\[Order].LeadItem.csv"
            );

            Init();
            GoToAdmin("settingscrm");
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
        public void OrderFromLead()
        {
            //Раньше данные в лид подтягивались из карточки покупателя. 
            //Сейчас логика изменилась: лид связан с покупателем, но данные отображаются только указанные в лиде
            //check order statuses count
            IWebElement selectElem = Driver.FindElement(By.Name("OrderStatusIdFromLead"));
            SelectElement select = new SelectElement(selectElem);

            IList<IWebElement> allOptions = select.Options;

            VerifyIsTrue(allOptions.Count == 6, "order statuses count");

            //set OrderStatusIdFromLead
            selectElem = Driver.FindElement(By.Name("OrderStatusIdFromLead"));
            select = new SelectElement(selectElem);
            if (!select.SelectedOption.Text.Contains("Доставлен"))
            {
                (new SelectElement(Driver.FindElement(By.Name("OrderStatusIdFromLead")))).SelectByText("Доставлен");
                Driver.WaitForToastSuccess();
            }

            //check order status selected
            GoToAdmin("settingscrm");

            selectElem = Driver.FindElement(By.Name("OrderStatusIdFromLead"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Доставлен"), "settings order status selected");

            //pre check customer pop up 
            GoToAdmin(
                "customers/view/cfc2c33b-1e84-415e-8482-e98156341604#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341604");

            selectElem = Driver.FindElement(By.Id("Customer_ManagerId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "customer manager");

            selectElem = Driver.FindElement(By.Id("Customer_CustomerGroupId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("CustomerGroup2 - 10%"), "customer group");

            VerifyAreEqual("Admin Comment Deafult",
                Driver.FindElement(By.Id("Customer_AdminComment")).GetAttribute("value"), "customer admin's comment");

            VerifyAreEqual("LastName", Driver.FindElement(By.Id("Customer_LastName")).GetAttribute("value"),
                "customer last name");
            VerifyAreEqual("FirstName", Driver.FindElement(By.Id("Customer_FirstName")).GetAttribute("value"),
                "customer first name");
            VerifyAreEqual("Patronymic", Driver.FindElement(By.Id("Customer_Patronymic")).GetAttribute("value"),
                "customer patronymic");
            VerifyAreEqual("Organization Test",
                Driver.FindElement(By.Id("Customer_Organization")).GetAttribute("value"),
                "customer customer Organization");
            VerifyAreEqual("Россия", Driver.FindElement(By.Id("CustomerContact_Country")).GetAttribute("value"),
                "customer country");
            VerifyAreEqual("Московская область",
                Driver.FindElement(By.Id("CustomerContact_Region")).GetAttribute("value"), "customer region");
            VerifyAreEqual("Москва", Driver.FindElement(By.Id("CustomerContact_City")).GetAttribute("value"),
                "customer city");
            VerifyAreEqual("mail@mail.com", Driver.FindElement(By.Id("Customer_EMail")).GetAttribute("value"),
                "customer Email");
            VerifyAreEqual("112233", Driver.FindElement(By.Id("CustomerContact_Zip")).GetAttribute("value"),
                "customer zip");
            VerifyAreEqual("+7(495)800-20-01", Driver.FindElement(By.Id("Customer_Phone")).GetAttribute("value"),
                "customer phone number");
            VerifyAreEqual("Арбат", Driver.FindElement(By.Id("CustomerContact_Street")).GetAttribute("value"),
                "customer street");
            VerifyAreEqual("1", Driver.FindElement(By.Id("CustomerContact_House")).GetAttribute("value"),
                "customer house");
            VerifyAreEqual("2", Driver.FindElement(By.Id("CustomerContact_Apartment")).GetAttribute("value"),
                "customer appartment");
            VerifyAreEqual("3", Driver.FindElement(By.Id("CustomerContact_Structure")).GetAttribute("value"),
                "customer structure");
            VerifyAreEqual("4", Driver.FindElement(By.Id("CustomerContact_Entrance")).GetAttribute("value"),
                "customer entrance");
            VerifyAreEqual("5", Driver.FindElement(By.Id("CustomerContact_Floor")).GetAttribute("value"),
                "customer floor");

            //pre check customer fields in customer
            VerifyAreEqual("pre check дополнительное поле текст",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                    .GetAttribute("value"), "pre check customer field text");
            VerifyAreEqual("123123",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]"))
                    .GetAttribute("value"), "pre check customer field number");
            VerifyAreEqual("line 1\r\nline 2\r\nline 3",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 4\"]"))
                    .GetAttribute("value"), "pre check customer field textarea");
            VerifyAreEqual("04.08.2016",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 6\"]"))
                    .GetAttribute("value"), "pre check customer field date");

            selectElem = Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 1\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Customer Field 1 Value 3"),
                "pre check customer field select");

            //pre check lead fields
            GoToAdmin("leads?#?leadIdInfo=115");

            VerifyIsTrue(Driver.GetGridCell(0, "Name", "LeadItems").Text.Contains("TestProduct15"),
                "lead product grid");

            VerifyAreEqual(string.Empty, Driver.FindElement(By.Id("Lead_LastName")).GetAttribute("value"),
                "lead LastName");
            VerifyAreEqual("FirstName", Driver.FindElement(By.Id("Lead_FirstName")).GetAttribute("value"),
                "lead FirstName");
            VerifyAreEqual(string.Empty, Driver.FindElement(By.Id("Lead_Patronymic")).GetAttribute("value"),
                "lead Patronymic");
            VerifyAreEqual(string.Empty, Driver.FindElement(By.Id("Lead_Organization")).GetAttribute("value"),
                "lead Organization");
            VerifyAreEqual("testmail@mail.ru115", Driver.FindElement(By.Id("Lead_Email")).GetAttribute("value"),
                "lead Email"); 
            VerifyAreEqual("+7(900)125-36-47", Driver.FindElement(By.Id("Lead_Phone")).GetAttribute("value"),
                "lead Phone");

            selectElem = Driver.FindElement(By.Id("Lead_ManagerId"));
            select = new SelectElement(selectElem);
            VerifyAreEqual("test testov", (select.SelectedOption.Text), "lead manager");
            selectElem = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            select = new SelectElement(selectElem);
            VerifyAreEqual("Оффлайн", (select.SelectedOption.Text), "lead source");

            VerifyAreEqual("TestProduct15\r\nАртикул: 15", Driver.GetGridCell(0, "Name", "LeadItems").Text,
                "lead Name product at order");
            VerifyAreEqual("15", Driver.GetGridCell(0, "Price", "LeadItems").Text.ToString(), "lead  product at order");
            VerifyAreEqual("15", Driver.GetGridCell(0, "Amount", "LeadItems").Text.ToString(),
                "lead Count product at order");

            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("225"), "lead Cost product");

            //pre check customer fields in lead
            VerifyAreEqual("pre check дополнительное поле текст",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                    .GetAttribute("value"), "lead customer field text");
            VerifyAreEqual("123123",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 3\"]"))
                    .GetAttribute("value"), "lead customer field number");
            VerifyAreEqual("line 1\r\nline 2\r\nline 3",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 4\"]"))
                    .GetAttribute("value"), "lead customer field textarea");
            VerifyAreEqual("04.08.2016",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 6\"]"))
                    .GetAttribute("value"), "lead customer field date");

            selectElem = Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 1\"]"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Customer Field 1 Value 3"), "lead customer field select");

            //check lead to order
            Driver.FindElement(By.CssSelector("[data-e2e=\"LeadCreateOrder\"]")).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"completeLead\"]")).Click();
            Driver.WaitForElem(By.ClassName("order-header-item-name"));

            VerifyIsTrue(Driver.Url.Contains("orders/edit/3"), "redirect to order");

            //GoToAdmin("orders/edit/8"); - for local machine
            GoToAdmin("orders/edit/3");
            //check created order
            Driver.WaitForElem(By.Id("Order_ManagerId"));

            VerifyAreEqual("LastName FirstName Patronymic",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FullNameCustomer\"]")).Text, "order Name");
            VerifyAreEqual("mail@mail.com", Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerMail\"]")).Text,
                "order Email");
            VerifyAreEqual(
                "Россия, Московская область, Москва, 112233, улица Арбат, д. 1, стр. 3, кв. 2, подъезд 4, эт. 5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"CustomerAdress\"]")).Text, "order adress");

            /*
            VerifyAreEqual("Organization Test", driver.FindElement(By.Id("Order_OrderCustomer_Organization")).GetAttribute("value"), "order Organization");
            VerifyAreEqual("+7(495)800-20-01", driver.FindElement(By.Id("Order_OrderCustomer_Phone")).GetAttribute("value"), "order Phone");
             */

            VerifyAreEqual("TestProduct15\r\nАртикул: 15\r\nВес: 15 кг",
                Driver.GetGridCell(0, "Name", "OrderItems").Text, "order Name product at order");
            VerifyAreEqual("15", Driver.GetGridCell(0, "PriceString", "OrderItems").Text.ToString(),
                "order  product at order");
            VerifyAreEqual("15", Driver.GetGridCell(0, "Amount", "OrderItems").Text.ToString(),
                "order Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "OrderItems").Text.Contains("225"),
                "order Cost product at order");

            VerifyAreEqual("Admin Comment Deafult",
                Driver.FindElement(By.CssSelector("[data-e2e=\"AdminCommentAboutCustomer\"]")).Text,
                "order Customer Admin Comment");

            selectElem = Driver.FindElement(By.Id("Order_ManagerId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("test testov"), "order manager selected");

            selectElem = Driver.FindElement(By.Id("Order_OrderSourceId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Оффлайн"), "order source selected");

            selectElem = Driver.FindElement(By.Id("Order_OrderStatusId"));
            select = new SelectElement(selectElem);
            VerifyIsTrue(select.SelectedOption.Text.Contains("Доставлен"), "order status selected");

            //check buyer card
            Driver.FindElement(AdvBy.DataE2E("ChangeCustomer")).Click();
            Driver.WaitForElem(By.ClassName("popup-order-customer"));

            VerifyAreEqual("LastName", Driver.FindElement(By.Id("Order_OrderCustomer_LastName")).GetAttribute("value"),
                "customer from order last name");
            VerifyAreEqual("FirstName", Driver.FindElement(By.Id("Order_OrderCustomer_FirstName")).GetAttribute("value"),
                "customer from order first name");
            VerifyAreEqual("Patronymic", Driver.FindElement(By.Id("Order_OrderCustomer_Patronymic")).GetAttribute("value"),
                "customer from order patronymic");
            VerifyAreEqual("Organization Test",
                Driver.FindElement(By.Id("Order_OrderCustomer_Organization")).GetAttribute("value"),
                "customer from order customer Organization");
            VerifyAreEqual("Россия", Driver.FindElement(By.Id("Order_OrderCustomer_Country")).GetAttribute("value"),
                "customer from order country");
            VerifyAreEqual("Московская область",
                Driver.FindElement(By.Id("Order_OrderCustomer_Region")).GetAttribute("value"), "customer region");
            VerifyAreEqual("Москва", Driver.FindElement(By.Id("Order_OrderCustomer_City")).GetAttribute("value"),
                "customer from order city");
            VerifyAreEqual("mail@mail.com", Driver.FindElement(By.Id("Order_OrderCustomer_Email")).GetAttribute("value"),
                "customer from order Email");
            VerifyAreEqual("112233", Driver.FindElement(By.Id("Order_OrderCustomer_Zip")).GetAttribute("value"),
                "customer from order zip");
            VerifyAreEqual("+7(495)800-20-01", Driver.FindElement(By.Id("Order_OrderCustomer_Phone")).GetAttribute("value"),
                "customer from order phone number");
            VerifyAreEqual("Арбат", Driver.FindElement(By.Id("Order_OrderCustomer_Street")).GetAttribute("value"),
                "customer from order street");
            VerifyAreEqual("1", Driver.FindElement(By.Id("Order_OrderCustomer_House")).GetAttribute("value"),
                "customer from order house");
            VerifyAreEqual("2", Driver.FindElement(By.Id("Order_OrderCustomer_Apartment")).GetAttribute("value"),
                "customer from order appartment");
            VerifyAreEqual("3", Driver.FindElement(By.Id("Order_OrderCustomer_Structure")).GetAttribute("value"),
                "customer from order structure");
            VerifyAreEqual("4", Driver.FindElement(By.Id("Order_OrderCustomer_Entrance")).GetAttribute("value"),
                "customer from order entrance");
            VerifyAreEqual("5", Driver.FindElement(By.Id("Order_OrderCustomer_Floor")).GetAttribute("value"),
                "customer from order floor");

            //check lead final deal status selected
            GoToAdmin("leads");

            Driver.GetGridFilterTab(0, "115");

            VerifyAreEqual("115", Driver.GetGridCell(0, "Id").Text, "lead final deal status lead's id");
            VerifyAreEqual("Сделка заключена", Driver.GetGridCell(0, "DealStatusName").Text, "lead final deal status");
        }
    }
}