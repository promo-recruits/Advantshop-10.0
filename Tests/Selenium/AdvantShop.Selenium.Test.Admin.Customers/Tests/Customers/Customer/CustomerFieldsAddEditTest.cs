using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using AdvantShop.Selenium.Core.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.Customer
{
    [TestFixture]
    public class CustomerFieldsAddEditTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Customers\\CustomerFields\\Customers.CustomerGroup.csv",
                "data\\Admin\\Customers\\CustomerFields\\Customers.Customer.csv",
                "data\\Admin\\Customers\\CustomerFields\\Customers.CustomerField.csv",
                "data\\Admin\\Customers\\CustomerFields\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\Customers\\CustomerFields\\Customers.CustomerFieldValuesMap.csv"
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
        public void CustomerFieldsAddSelectTest()
        {
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();

            VerifyAreEqual("Новое поле", Driver.FindElement(By.TagName("h2")).Text, "h2 add pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]"))
                .SendKeys("New Customer Field Select");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldType\"]"))))
                .SelectByText("Выбор");

            Driver.SendKeysInput(AdvBy.DataE2E("customerFieldSortOrder"), "1");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldRequired\"]"))
                    .FindElement(By.TagName("input")).Selected, "default not required");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldShowInRegistration\"]"))
                    .FindElement(By.TagName("input")).Selected, "default show in registration");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldShowInCheckout\"]"))
                    .FindElement(By.TagName("input")).Selected, "default show in checkout");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldDisableCustomerEditing\"]"))
                    .FindElement(By.TagName("input")).Selected, "default disable customer editing");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldEnabled\"]"))
                    .FindElement(By.TagName("input")).Selected, "default enabled");
            //VerifyIsFalse(Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected, "customer field ShowInRegistration");
            //VerifyIsFalse(Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields").FindElement(By.TagName("input")).Selected, "customer field ShowInCheckout");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-body")).Text.Contains("Значения поля"),
                "select type");

            Driver.SendKeysInput(AdvBy.DataE2E("customerFieldValue"), "Value Added 1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldAddValue\"]")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("customerFieldValue"), "Value Added 2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldAddValue\"]")).Click();

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin settings
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("New Customer Field Select");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyAreEqual("New Customer Field Select", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "customer field name");
            VerifyAreEqual("Выбор", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text,
                "customer field type");
            VerifyAreEqual("Список значений", Driver.GetGridCell(0, "HasValues", "CustomerFields").Text,
                "customer field values");
            VerifyIsFalse(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field Required");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "customer field SortOrder");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field ShowInRegistration");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field ShowInCheckout");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field enabled");

            //check admin edit customer
            GoToAdmin("customers#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341604");

            VerifyIsTrue(Driver.PageSource.Contains("New Customer Field Select"), "customer edit field name");

            IWebElement selectElemCustomerField =
                Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Select\"]"));
            SelectElement select = new SelectElement(selectElemCustomerField);

            IList<IWebElement> allOptionsCustomerField = select.Options;

            VerifyIsTrue(allOptionsCustomerField.Count == 3,
                "customer edit count field's values"); //2 customer field's values + null select
        }

        [Test]
        public void CustomerFieldsAddTextDisabledTest()
        {
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]"))
                .SendKeys("Customer Field Added Text Disabled");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldType\"]")))).SelectByText(
                "Текстовое поле");

            Driver.SendKeysInput(AdvBy.DataE2E("customerFieldSortOrder"), "2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldRequired\"]")).FindElement(By.TagName("span"))
                .Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldEnabled\"]")).FindElement(By.TagName("span"))
                .Click();

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".modal-body")).Text.Contains("Значения поля"),
                "no select type");

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin settings
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field Added Text Disabled");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyAreEqual("Customer Field Added Text Disabled", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "customer field name");
            VerifyAreEqual("Текстовое поле", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text,
                "customer field type");
            VerifyAreEqual("", Driver.GetGridCell(0, "HasValues", "CustomerFields").Text, "customer field values");
            VerifyIsTrue(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field Required");
            VerifyAreEqual("2", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "customer field SortOrder");
            VerifyIsFalse(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field enabled");

            //check admin edit customer
            GoToAdmin("customers#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341604");

            VerifyIsFalse(Driver.PageSource.Contains("Customer Field Added Text Disabled"),
                "customer edit field name disabled");
            VerifyIsFalse(
                Driver.FindElements(By.CssSelector("[validation-input-text=\"Customer Field Added Text Disabled\"]"))
                    .Count > 0, "customer edit field disabled");
        }

        [Test]
        public void CustomerFieldsAddTextEnabledTest()
        {
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]"))
                .SendKeys("Customer Field Text Enabled");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldType\"]")))).SelectByText(
                "Текстовое поле");

            Driver.SendKeysInput(AdvBy.DataE2E("customerFieldSortOrder"), "3");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldRequired\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldShowInRegistration\"]"))
                .FindElement(By.TagName("span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldShowInCheckout\"]"))
                .FindElement(By.TagName("span")).Click();

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".modal-body")).Text.Contains("Значения поля"),
                "no select type");

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin settings
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field Text Enabled");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyAreEqual("Customer Field Text Enabled", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "customer field name");
            VerifyAreEqual("Текстовое поле", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text,
                "customer field type");
            VerifyAreEqual("", Driver.GetGridCell(0, "HasValues", "CustomerFields").Text, "customer field values");
            VerifyIsTrue(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field Required");
            VerifyAreEqual("3", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "customer field SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field enabled");
            VerifyIsFalse(
                Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field ShowInRegistration");
            VerifyIsFalse(
                Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field ShowInCheckout");

            //check admin edit customer
            GoToAdmin("customers#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341604");

            VerifyIsTrue(Driver.PageSource.Contains("Customer Field Text Enabled"), "customer edit field name enabled");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[validation-input-text=\"Customer Field Text Enabled\"]")).Count >
                0, "customer edit field enabled");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field Text Enabled\"]"))
                    .GetAttribute("ng-required").Equals("true"), "customer edit required");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field Text Enabled\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field Text Enabled\"]"))
                .SendKeys("test 1");
            Driver.FindElement(By.CssSelector(".order-header-item-name")).Click();

            VerifyAreEqual("test 1",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field Text Enabled\"]"))
                    .GetAttribute("value"), "customer edit field text");
            Driver.FindElement(AdvBy.DataE2E("customerInfoClose")).Click();

            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.ClassName("fa-pencil-alt")).Click();
            VerifyAreEqual(String.Empty, Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field Text Enabled\"]"))
                    .GetAttribute("value"), "customer edit field text without saving");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field Text Enabled\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field Text Enabled\"]"))
                .SendKeys("test 1");
            Thread.Sleep(500);
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            Refresh();
            VerifyAreEqual("test 1",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field Text Enabled\"]"))
                .GetAttribute("value"), "customer edit field text after saving");

        }

        [Test]
        public void CustomerFieldsEditTypeTest()
        {
            //pre check edit customer
            GoToAdmin("customers#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341604");

            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 2"), "pre check edit customer field name");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[validation-input-text=\"Customer Field 2\"]")).Count > 0,
                "pre check customer edit field enabled");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                    .GetAttribute("ng-required").Equals("true"), "pre check customer edit required");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]")).Clear();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]")).SendKeys("string");
            Driver.FindElement(By.CssSelector(".order-header-item-name")).Click();

            Driver.ScrollToTop();
            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            VerifyAreEqual("string",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 2\"]"))
                    .GetAttribute("value"), "pre check customer edit field print text");

            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field 2");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyAreEqual("Customer Field 2", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "customer field name");

            Driver.GetGridCell(0, "_serviceColumn", "CustomerFields")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            //pre check edit field pop up
            VerifyAreEqual("Редактирование поля", Driver.FindElement(By.TagName("h2")).Text, "h2 edit customer field");

            VerifyAreEqual("Customer Field 2",
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]")).GetAttribute("value"),
                "pre check edit pop up name");
            VerifyAreEqual("20",
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldSortOrder\"]")).GetAttribute("value"),
                "pre check edit pop up sort");

            IWebElement selectElemType = Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldType\"]"));
            SelectElement select = new SelectElement(selectElemType);
            VerifyIsTrue(select.AllSelectedOptions[0].Text.Contains("Текстовое поле"), "pre check edit pop up type");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldRequired\"]"))
                    .FindElement(By.TagName("input")).Selected, "pre check edit pop up required");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldEnabled\"]"))
                    .FindElement(By.TagName("input")).Selected, "pre check edit pop up enabled");

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".modal-body")).Text.Contains("Значения поля"),
                "pre check edit pop no select type");

            //edit
            Driver.SendKeysInput(AdvBy.DataE2E("customerFieldName"), "Edited Customer Field");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldType\"]"))))
                .SelectByText("Выбор");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-body")).Text.Contains("Значения поля"),
                "select type");

            Driver.SendKeysInput(AdvBy.DataE2E("customerFieldValue"), "Value 1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldAddValue\"]")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("customerFieldValue"), "Value 2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldAddValue\"]")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("customerFieldValue"), "Value 3");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldAddValue\"]")).Click();

            Driver.SendKeysInput(AdvBy.DataE2E("customerFieldSortOrder"), "100");

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldRequired\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin settings
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field 2");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "modified customer field");

            Driver.GridFilterSendKeys("Edited Customer Field");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyAreEqual("Edited Customer Field", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "customer field name");
            VerifyAreEqual("Выбор", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text,
                "customer field type");
            VerifyAreEqual("Список значений", Driver.GetGridCell(0, "HasValues", "CustomerFields").Text,
                "customer field values");
            VerifyIsFalse(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field Required");
            VerifyAreEqual("100", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text,
                "customer field SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field enabled");

            //check admin edit customer
            GoToAdmin("customers#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341604");

            VerifyIsTrue(Driver.PageSource.Contains("Edited Customer Field"), "customer edit field name");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[validation-input-text=\"Edited Customer Field\"]")).Count > 0,
                "customer edit field enabled");
            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 2"), "prev customer field name edited");

            IWebElement selectElemEditedType =
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Edited Customer Field\"]"));
            select = new SelectElement(selectElemEditedType);

            IList<IWebElement> allOptionsEditedCustomerField = select.Options;

            VerifyIsTrue(allOptionsEditedCustomerField.Count == 5,
                "customer edit count field's values"); //3 customer field's values + null select + previous text value
        }

        [Test]
        public void CustomerFieldsEditEnabledTest()
        {
            //pre check edit customer
            GoToAdmin("customers#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341604");

            VerifyIsFalse(Driver.PageSource.Contains("Customer Field 5"), "pre check edit customer field name");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[validation-input-text=\"Customer Field 5\"]")).Count > 0,
                "pre check customer edit field enabled");

            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field 5");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyAreEqual("Customer Field 5", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "customer field name");

            //edit
            Driver.GetGridCell(0, "_serviceColumn", "CustomerFields")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldEnabled\"]")).FindElement(By.TagName("span"))
                .Click();

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin settings
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("Customer Field 5");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyAreEqual("Customer Field 5", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "customer field name");
            VerifyAreEqual("Текстовое поле", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text,
                "customer field type");
            VerifyAreEqual("", Driver.GetGridCell(0, "HasValues", "CustomerFields").Text, "customer field values");
            VerifyIsTrue(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field Required");
            VerifyAreEqual("50", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "customer field SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field enabled");

            //check admin edit customer
            GoToAdmin("customers#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341604");

            VerifyIsTrue(Driver.PageSource.Contains("Customer Field 5"), "customer edit field name");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[validation-input-text=\"Customer Field 5\"]")).Count > 0,
                "customer edit field enabled");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 5\"]"))
                    .GetAttribute("ng-required").Equals("true"), "customer edit field required");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 5\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 5\"]"))
                .SendKeys("test 1");
            Driver.FindElement(By.CssSelector(".order-header-item-name")).Click();

            VerifyAreEqual("test 1",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 5\"]"))
                    .GetAttribute("value"), "customer edit field text");
            Driver.FindElement(AdvBy.DataE2E("customerInfoClose")).Click();

            Driver.GetGridCell(0, "_serviceColumn", "Customers").FindElement(By.ClassName("fa-pencil-alt")).Click();
            VerifyAreEqual(String.Empty, Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 5\"]"))
                    .GetAttribute("value"), "customer edit field text without saving");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 5\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 5\"]"))
                .SendKeys("test 1");
            Thread.Sleep(500);
            Driver.FindElement(AdvBy.DataE2E("btnSave")).Click();
            Driver.WaitForToastSuccess();

            Refresh();
            VerifyAreEqual("test 1",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"Customer Field 5\"]"))
                .GetAttribute("value"), "customer edit field text after saving");
        }

        [Test]
        public void CustomerFieldsAddDateTest()
        {
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GetButton(EButtonType.Add).Click();
            Driver.WaitForModal();

            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldName\"]")).SendKeys("New Customer Field Date");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldType\"]"))))
                .SelectByText("Дата");

            Driver.SendKeysInput(AdvBy.DataE2E("customerFieldSortOrder"), "4");

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".modal-body")).Text.Contains("Значения поля"),
                "no select type");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"customerFieldEnabled\"]"))
                    .FindElement(By.TagName("input")).Selected, "default enabled");

            Driver.GetButton(EButtonType.Save).Click();
            Driver.WaitForToastSuccess();

            //check admin settings
            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GridFilterSendKeys("New Customer Field Date");
            Driver.XPathContainsText("h2", "Дополнительные поля покупателя");

            VerifyAreEqual("New Customer Field Date", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "customer field name");
            VerifyAreEqual("Дата", Driver.GetGridCell(0, "FieldTypeFormatted", "CustomerFields").Text,
                "customer field type");
            VerifyAreEqual("", Driver.GetGridCell(0, "HasValues", "CustomerFields").Text, "customer field no values");
            VerifyIsFalse(Driver.GetGridCell(0, "Required", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field Required");
            VerifyAreEqual("4", Driver.GetGridCell(0, "SortOrder", "CustomerFields").Text, "customer field SortOrder");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field enabled");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ShowInRegistration", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field ShowInRegistration");
            VerifyIsTrue(
                Driver.GetGridCell(0, "ShowInCheckout", "CustomerFields").FindElement(By.TagName("input")).Selected,
                "customer field ShowInCheckout");

            //check admin edit customer
            GoToAdmin("customers#?customerIdInfo=cfc2c33b-1e84-415e-8482-e98156341604");

            VerifyIsTrue(Driver.PageSource.Contains("New Customer Field Date"), "customer edit field name enabled");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[validation-input-text=\"New Customer Field Date\"]")).Count > 0,
                "customer edit field enabled");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Date\"]"))
                    .GetAttribute("ng-required").Equals("false"), "customer edit not required");

            VerifyIsFalse(Driver.FindElements(By.CssSelector(".flatpickr-month"))[1].Displayed,
                "calender not displayed");

            Driver.FindElement(By.CssSelector(".custom-fields-row .fa-calendar-alt")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".flatpickr-month"))[1].Displayed, "calender displayed");

            Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Date\"]")).Click();
            Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Date\"]"))
                .SendKeys("03.03.2017");

            Driver.FindElement(By.CssSelector(".custom-fields-row .fa-calendar-alt")).Click();

            VerifyAreEqual("03.03.2017",
                Driver.FindElement(By.CssSelector("[validation-input-text=\"New Customer Field Date\"]"))
                    .GetAttribute("value"), "customer edit field date");
        }
    }
}