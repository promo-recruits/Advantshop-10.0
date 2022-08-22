using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.Customer
{
    [TestFixture]
    public class CustomerFieldsAddTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Customers\\CustomerFields\\CustomerFieldAdd\\Customers.CustomerGroup.csv",
                "data\\Admin\\Customers\\CustomerFields\\CustomerFieldAdd\\Customers.Customer.csv",
                "data\\Admin\\Customers\\CustomerFields\\CustomerFieldAdd\\Customers.CustomerField.csv",
                "data\\Admin\\Customers\\CustomerFields\\CustomerFieldAdd\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\Customers\\CustomerFields\\CustomerFieldAdd\\Customers.CustomerFieldValuesMap.csv"
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
        public void CustomerFieldsAddNumberTest()
        {
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]"))
                .SendKeys("New Customer Field Number");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldType\"]")))).SelectByText(
                "Числовое поле");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldSortOrder\"]")).SendKeys("4");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldRequired\"]")).FindElement(By.TagName("span"))
                .Click();

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".modal-body")).Text.Contains("Значения поля"),
                "no select type");

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin settings
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("New Customer Field Number");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyAreEqual("New Customer Field Number", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "customer field name");
            VerifyAreEqual("Числовое поле", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text,
                "customer field type");
            VerifyAreEqual("", Driver.GetGridCell(0, "HasValues", "CustomerFields").Text, "customer field values");
            VerifyIsTrue(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field Required");
            VerifyAreEqual("4", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "customer field SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field enabled");

            //check admin edit customer
            GoToAdmin("customers#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341601");

            VerifyIsTrue(Driver.PageSource.Contains("New Customer Field Number"), "customer edit field name");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[validation-input-text=\"New Customer Field Number\"]")).Count > 0,
                "customer edit field enabled");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Number\"]"))
                    .GetAttribute("ng-required").Equals("true"), "customer edit required");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Number\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Number\"]")).Clear();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Number\"]"))
                .SendKeys("string");
            Driver.FindElement(By.CssSelector(".order-header-item-name")).Click();
            Thread.Sleep(500);//костыль
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Number\"]"))
                    .GetCssValue("border-color"), "customer edit field print text");

            GoToAdmin("customers#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341601");
            Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Number\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Number\"]")).Clear();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Number\"]")).SendKeys("111");
            Driver.FindElement(By.CssSelector(".order-header-item-name")).Click();
            Thread.Sleep(100); 
            VerifyAreNotEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Number\"]"))
                    .GetCssValue("border-color"), "customer edit field print number");

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("111",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Number\"]"))
                    .GetAttribute("value"), "customer edit field print number");
        }

        [Test]
        public void CustomerFieldsAddTextAreaTest()
        {
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]")).SendKeys("New Customer Field Area");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldType\"]")))).SelectByText(
                "Многострочное текстовое поле");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldSortOrder\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldSortOrder\"]")).SendKeys("5");

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".modal-body")).Text.Contains("Значения поля"),
                "no select type");

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin settings
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("New Customer Field Area");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyAreEqual("New Customer Field Area", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "customer field name");
            VerifyAreEqual("Многострочное текстовое поле",
                Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text, "customer field type");
            VerifyAreEqual("", Driver.GetGridCell(0, "HasValues", "CustomerFields").Text, "customer field values");
            VerifyIsFalse(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field Required");
            VerifyAreEqual("5", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "customer field SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field enabled");

            //check admin edit customer
            GoToAdmin("customers#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341601");

            VerifyIsTrue(Driver.PageSource.Contains("New Customer Field Area"), "customer edit field name");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[validation-input-text=\"New Customer Field Area\"]")).Count > 0,
                "customer edit field enabled");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Area\"]"))
                    .GetAttribute("ng-required").Equals("false"), "customer edit not required");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Area\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Area\"]"))
                .SendKeys("test 1" + Keys.Enter + "test 2" + Keys.Enter + "test 3");
            Driver.FindElement(By.CssSelector(".order-header-item-name")).Click();

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("test 1\r\ntest 2\r\ntest 3",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Area\"]"))
                    .GetAttribute("value"), "customer edit field textarea");
        }
    }
}