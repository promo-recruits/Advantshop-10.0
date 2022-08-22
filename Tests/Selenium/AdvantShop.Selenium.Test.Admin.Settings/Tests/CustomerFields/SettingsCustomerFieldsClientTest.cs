using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields
{
    [TestFixture]
    public class SettingsCustomerFieldsShowInClientTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\CustomerFieldsClient\\Catalog.Category.csv",
                "data\\Admin\\Settings\\CustomerFieldsClient\\Catalog.Product.csv",
                "data\\Admin\\Settings\\CustomerFieldsClient\\Catalog.Size.csv",
                "data\\Admin\\Settings\\CustomerFieldsClient\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\CustomerFieldsClient\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\CustomerFieldsClient\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\CustomerFieldsClient\\Customers.Customer.csv",
                "data\\Admin\\Settings\\CustomerFieldsClient\\Customers.CustomerField.csv",
                "data\\Admin\\Settings\\CustomerFieldsClient\\Customers.CustomerFieldValue.csv"
            );
            Init();

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void ClientRegister()
        {
            GoToClient("myaccount");

            Driver.FindElement(By.LinkText("Выйти")).Click();
            Driver.FindElement(By.LinkText("Регистрация")).Click();
            Driver.WaitForElem(By.ClassName("registration-block"));

            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 1"), "field 1 show in client");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 2"), "field 2 show in client");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 3"), "field 3 show in client");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 4"), "field 4 show in client");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 5"), "field 5 show in client");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 6"), "field 6 not show in client");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 7"), "field 7 disabled");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 8"), "field 8 disabled");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 9"), "field 9 disabled");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 10"), "field 10 disabled");

            VerifyIsTrue(
                Driver.FindElement(By.Id("registrationPage_CustomerFields_1__Value")).GetAttribute("required")
                    .Equals("true"), "field 2 show in client required");
            VerifyIsTrue(
                Driver.FindElement(By.Id("registrationPage_CustomerFields_2__Value")).GetAttribute("required")
                    .Equals("true"), "field 3 show in client required");

            VerifyIsTrue(
                Driver.FindElement(By.Id("registrationPage_CustomerFields_0__Value")).GetAttribute("ng-required")
                    .Equals("false"), "field 1 show in client not required");
            VerifyIsTrue(
                Driver.FindElement(By.Id("registrationPage_CustomerFields_3__Value")).GetAttribute("ng-required")
                    .Equals("false"), "field 4 show in client not required");
            VerifyIsTrue(
                Driver.FindElement(By.Id("registrationPage_CustomerFields_4__Value")).GetAttribute("ng-required")
                    .Equals("false"), "field 5 show in client not required");
        }

        [Test]
        public void ClientOrder()
        {
            GoToClient("products/test-product102");

            Driver.ScrollTo(By.CssSelector("[title=\"SizeName1\"]"));
            Driver.FindElement(By.CssSelector("[data-product-id=\"102\"]")).Click();

            GoToClient("cart");

            Driver.FindElement(By.XPath("//a[contains(text(), 'Оформить')]")).Click();
            Driver.WaitForElem(By.ClassName("checkout-page"));

            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 1"), "field 1 show in client");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 2"), "field 2 show in client");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 3"), "field 3 show in client");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 4"),
                "field 4 show in client"); //поле активно в регистрации и чекауте
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 5"),
                "field 5 show in client"); //поле активно в регистрации и чекауте
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 6"),
                "field 6 not show in client"); //поле активно в регистрации
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 7"),
                "field 7 disabled"); //поле активно в регистрации но отключено
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 8"),
                "field 8 disabled"); //поле активно в регистрации но отключено
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 9"),
                "field 9 disabled"); //поле активно в регистрации но отключено
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 10"),
                "field 10 disabled"); //поле активно в регистрации но отключено

            VerifyIsTrue(
                Driver.FindElement(By.Id("checkout_newCustomer_CustomerFields_2__Value")).GetAttribute("required")
                    .Equals("true"), "field 2 show in client required");
            VerifyIsTrue(
                Driver.FindElement(By.Id("checkout_newCustomer_CustomerFields_3__Value")).GetAttribute("required")
                    .Equals("true"), "field 3 show in client required");

            VerifyIsTrue(
                Driver.FindElement(By.Id("checkout_newCustomer_CustomerFields_0__Value")).GetAttribute("ng-required")
                    .Equals("false"), "field 1 show in client not required");
            VerifyIsTrue(
                Driver.FindElement(By.Id("checkout_newCustomer_CustomerFields_1__Value")).GetAttribute("ng-required")
                    .Equals("false"), "field 4 show in client not required");
            //VerifyIsTrue(driver.FindElement(By.Id("checkout_newCustomer_CustomerFields_4__Value")).GetAttribute("ng-required").Equals("false"), "field 5 show in client not required");
        }

        [Test]
        public void ClientMyAccount()
        {
            GoToClient("myaccount?tab=commoninf");

            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 1"), "field 1 show in client");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 2"), "field 2 show in client");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 3"), "field 3 show in client");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 4"), "field 4 show in client");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 5"), "field 5 show in client");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 6"), "field 6 not show in registration");
            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 7"), "field 7 disabled");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 8"), "field 8 disabled");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 9"), "field 9 disabled");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 10"), "field 10 disabled");

            VerifyIsTrue(
                Driver.FindElement(By.Id("myaccount_user_CustomerFields_1__Value")).GetAttribute("required")
                    .Equals("true"), "field 2 show in client required");
            VerifyIsTrue(
                Driver.FindElement(By.Id("myaccount_user_CustomerFields_2__Value")).GetAttribute("required")
                    .Equals("true"), "field 3 show in client required");

            VerifyIsTrue(
                Driver.FindElement(By.Id("myaccount_user_CustomerFields_0__Value")).GetAttribute("ng-required")
                    .Equals("false"), "field 1 show in client not required");
            VerifyIsTrue(
                Driver.FindElement(By.Id("myaccount_user_CustomerFields_3__Value")).GetAttribute("ng-required")
                    .Equals("false"), "field 4 show in client not required");
            VerifyIsTrue(
                Driver.FindElement(By.Id("myaccount_user_CustomerFields_4__Value")).GetAttribute("ng-required")
                    .Equals("false"), "field 5 show in client not required");
        }
    }
}