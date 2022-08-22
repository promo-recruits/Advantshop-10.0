using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Services.servicesThreeColumns.Button.Form
{
    [TestFixture]
    public class LandingsServicesColumnsBtnFormField : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerField.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSubBlock.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingForm.csv"
            );

            Init();
        }

        private IWebElement agree;
        private string colorRGB;

        [Test]
        public void BtnServicesFormHeader()
        {
            TestName = "BtnServicesFormHeader";
            VerifyBegin(TestName);
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block Form");
            BtnActionButtonSelect("Показ всплывающей формы");
            VerifyIsTrue(Driver.FindElement(By.Id("tabHeaderForm")).Displayed, "form setting");
            FormSetTitle("New title form");
            FormSetSubTitle("New SubTitle form");
            FormSetButtonText("New Btn form");

            DelAllColumns("FormGrid");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormGrid\"]")).Text
                    .Contains("Необходимо добавить значение в таблицу"), "No elem in grid");
            FormShowAgreement("new text user agreement");
            //check modal windows without fields
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElement(By.Id("modalSettingsBlock_1")).Displayed, "form setting display");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormGrid\"]")).Text
                    .Contains("Необходимо добавить значение в таблицу"), "No elem in grid after close");

            //добавление полей
            FormAddField("field name 1", "Имя", true);
            FormAddField("field mail 2", "Email", true);
            FormAddField("field phone 3", "Телефон", false);
            FormAddField("field comment 4", "Комментарий", false);

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);

            //admin check
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "form displayed admin");
            VerifyAreEqual("New title form", Driver.FindElement(By.CssSelector("[data-e2e=\"FormTitle\"]")).Text,
                "text title admin");
            VerifyAreEqual("New SubTitle form", Driver.FindElement(By.CssSelector("[data-e2e=\"FormSubTitle\"]")).Text,
                "text sub title admin");
            VerifyAreEqual("New Btn form", Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Text,
                "text btn admin");
            VerifyAreEqual("new text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement admin");
            //check form field
            //placeholder
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 4,
                "count field");
            VerifyAreEqual("field name 1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("placeholder"), "name placeholder 1");
            VerifyAreEqual("field mail 2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("placeholder"), "mail placeholder 1");
            VerifyAreEqual("field phone 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("placeholder"), "comment placeholder 1");
            VerifyAreEqual("field comment 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetAttribute("placeholder"), "comment placeholder 1");
            //type input
            VerifyAreEqual("text",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("type"), "name type 1");
            VerifyAreEqual("email",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("type"), "mail type 1");
            VerifyAreEqual("tel",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("type"), "comment type 1");
            //type crm field
            VerifyAreEqual("lpForm.form.fields[0].type=2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[0]
                    .GetAttribute("data-ng-init"), "name crm type 1");
            VerifyAreEqual("lpForm.form.fields[1].type=5",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[1]
                    .GetAttribute("data-ng-init"), "mail crm type 1");
            VerifyAreEqual("lpForm.form.fields[2].type=6",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[2]
                    .GetAttribute("data-ng-init"), "comment crm type 1");
            VerifyAreEqual("lpForm.form.fields[3].type=10",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[3]
                    .GetAttribute("data-ng-init"), "comment crm type 1");
            //required
            VerifyAreEqual("true",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("data-ng-required"), "name required 1");
            VerifyAreEqual("true",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("data-ng-required"), "mail required 1");
            VerifyAreEqual("false",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("data-ng-required"), "comment required 1");
            VerifyAreEqual("false",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetAttribute("data-ng-required"), "comment required 1");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormHeaderClient()
        {
            TestName = "BtnServicesFormHeaderClient";
            VerifyBegin(TestName);
            ReInitClient();
            GoToClient("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "form displayed client");

            VerifyAreEqual("New title form", Driver.FindElement(By.CssSelector("[data-e2e=\"FormTitle\"]")).Text,
                "text title");
            VerifyAreEqual("New SubTitle form", Driver.FindElement(By.CssSelector("[data-e2e=\"FormSubTitle\"]")).Text,
                "text sub title");
            VerifyAreEqual("New Btn form", Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Text,
                "text btn");
            VerifyAreEqual("new text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement");
            //check form field
            //placeholder
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 4,
                "count field");
            VerifyAreEqual("field name 1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("placeholder"), "name placeholder 1");
            VerifyAreEqual("field mail 2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("placeholder"), "mail placeholder 1");
            VerifyAreEqual("field phone 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("placeholder"), "comment placeholder 1");
            VerifyAreEqual("field comment 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetAttribute("placeholder"), "comment placeholder 1");
            //type input
            VerifyAreEqual("text",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("type"), "name type 1");
            VerifyAreEqual("email",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("type"), "mail type 1");
            VerifyAreEqual("tel",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("type"), "comment type 1");
            //type crm field
            VerifyAreEqual("lpForm.form.fields[0].type=2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[0]
                    .GetAttribute("data-ng-init"), "name crm type 1");
            VerifyAreEqual("lpForm.form.fields[1].type=5",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[1]
                    .GetAttribute("data-ng-init"), "mail crm type 1");
            VerifyAreEqual("lpForm.form.fields[2].type=6",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[2]
                    .GetAttribute("data-ng-init"), "comment crm type 1");
            VerifyAreEqual("lpForm.form.fields[3].type=10",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[3]
                    .GetAttribute("data-ng-init"), "comment crm type 1");
            //required
            VerifyAreEqual("true",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("data-ng-required"), "name required 1");
            VerifyAreEqual("true",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("data-ng-required"), "mail required 1");
            VerifyAreEqual("false",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("data-ng-required"), "comment required 1");
            VerifyAreEqual("false",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetAttribute("data-ng-required"), "comment required 1");

            //border color
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetCssValue("border-color"), "border-color 1");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetCssValue("border-color"), "border-color 2");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetCssValue("border-color"), "border-color 3");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetCssValue("border-color"), "border-color 4");

            //border color user agree (before)
            agree = Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__agreement label span"));
            colorRGB = ((IJavaScriptExecutor) Driver)
                .ExecuteScript(
                    "return window.getComputedStyle(arguments[0], ':before').getPropertyValue('border-color');", agree)
                .ToString();
            VerifyAreEqual("rgb(216, 216, 216)", colorRGB, "border-color agree");

            //empty form
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "form displayed empty form");
            VerifyAreEqual("new text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement empty form");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormBtn\"]")).Count == 1,
                "btn displayed empty form");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormTitle\"]")).Count == 1,
                "title displayed empty form");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 4,
                "count field empty form");
            //border color
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetCssValue("border-color"), "border-color 1 empty form");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetCssValue("border-color"), "border-color 2 empty form");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetCssValue("border-color"), "border-color 3 empty form");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetCssValue("border-color"), "border-color 4 empty form");
            colorRGB = ((IJavaScriptExecutor) Driver)
                .ExecuteScript(
                    "return window.getComputedStyle(arguments[0], ':before').getPropertyValue('border-color');", agree)
                .ToString();
            VerifyAreEqual("rgb(241, 89, 89)", colorRGB, "border-color agree empty form");

            //fill field
            checkField(0, "test name");
            checkField(2, "89012345678");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "form displayed no email");
            VerifyAreEqual("new text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement o email ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormBtn\"]")).Count == 1,
                "btn displayed no email ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormTitle\"]")).Count == 1,
                "title displayed no email ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 4,
                "count field no email ");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetCssValue("border-color"), "border-color 1 no email");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetCssValue("border-color"), "border-color 2 no email");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetCssValue("border-color"), "border-color 3 no email");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetCssValue("border-color"), "border-color 4 no email");
            colorRGB = ((IJavaScriptExecutor) Driver)
                .ExecuteScript(
                    "return window.getComputedStyle(arguments[0], ':before').getPropertyValue('border-color');", agree)
                .ToString();
            VerifyAreEqual("rgb(241, 89, 89)", colorRGB, "border-color agree no email");

            VerifyAreEqual("test name",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("value"), "name value 1");
            VerifyAreEqual("",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("value"), "mail value 1");
            VerifyAreEqual("89012345678",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("value"), "comment value 1");

            //no valid mail
            checkField(1, "testemail");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "form displayed no email");
            VerifyAreEqual("new text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement no valid email ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormBtn\"]")).Count == 1,
                "btn displayed no valid email ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormTitle\"]")).Count == 1,
                "title displayed no valid email ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 4,
                "count field no valid email ");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetCssValue("border-color"), "border-color 1 no valid email");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetCssValue("border-color"), "border-color 2 no valid email");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetCssValue("border-color"), "border-color 3 no valid email");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetCssValue("border-color"), "border-color 4 no valid email");
            colorRGB = ((IJavaScriptExecutor) Driver)
                .ExecuteScript(
                    "return window.getComputedStyle(arguments[0], ':before').getPropertyValue('border-color');", agree)
                .ToString();
            VerifyAreEqual("rgb(241, 89, 89)", colorRGB, "border-color agree no valid email");

            VerifyAreEqual("test name",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("value"), "name value 1 no valid email");
            VerifyAreEqual("testemail",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("value"), "mail value 1 no valid email");
            VerifyAreEqual("89012345678",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("value"), "comment value 1 no valid email");

            // no user argee
            checkField(1, "testemail@mail.com");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "form displayed no user argee");
            VerifyAreEqual("new text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement no user argee ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormBtn\"]")).Count == 1,
                "btn displayed no user argee ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormTitle\"]")).Count == 1,
                "title displayed no user argee ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 4,
                "count field no user argee ");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetCssValue("border-color"), "border-color 1 no user argee");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetCssValue("border-color"), "border-color 2 no user argee");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetCssValue("border-color"), "border-color 3 no user argee");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetCssValue("border-color"), "border-color 4 no user argee");
            colorRGB = ((IJavaScriptExecutor) Driver)
                .ExecuteScript(
                    "return window.getComputedStyle(arguments[0], ':before').getPropertyValue('border-color');", agree)
                .ToString();
            VerifyAreEqual("rgb(241, 89, 89)", colorRGB, "border-color agree no user agree");

            VerifyAreEqual("test name",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("value"), "name value 1 no user argee");
            VerifyAreEqual("testemail@mail.com",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("value"), "mail value 1 no user argee");
            VerifyAreEqual("89012345678",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("value"), "comment value 1 no user argee");

            //user agree
            Driver.FindElement(By.CssSelector(".lp-form__agreement span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormBtn\"]")).Count == 0,
                "btn no displayed after agree");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormTitle\"]")).Displayed,
                "title displayed after agree");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 0,
                "count field after agree");

            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "close form all");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Displayed, "no btn");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"FormTitle\"]")).Displayed, "no title");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Count == 0,
                "no SuccessText");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Displayed,
                "no field");

            //check lead grid
            ReInit();
            GoToAdmin("leads?salesFunnelId=1");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName");
            VerifyAreEqual("test name", Driver.GetGridCell(0, "FullName").Text, "FullName");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date");

            //chek lead cart
            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead");
            VerifyAreEqual("test name", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead");
            VerifyAreEqual("testemail@mail.com", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead");
            VerifyAreEqual("89012345678", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"),
                "phone lead");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_LastName")).GetAttribute("value"),
                "lastname lead");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Выбрана услуга: Трансформер дизайна; Цена: 7 000 руб"), "description lead");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Лид из лендинга \"test1\""), "description lead 1");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormHeaderMobile()
        {
            TestName = "BtnServicesFormHeaderMobile";
            VerifyBegin(TestName);
            ReInitClient();
            //Check mobile
            GoToMobile("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "form displayed mobile");

            VerifyAreEqual("New title form", Driver.FindElement(By.CssSelector("[data-e2e=\"FormTitle\"]")).Text,
                "text title mobile");
            VerifyAreEqual("New SubTitle form", Driver.FindElement(By.CssSelector("[data-e2e=\"FormSubTitle\"]")).Text,
                "text sub title mobile");
            VerifyAreEqual("New Btn form", Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Text,
                "text btn mobile");
            VerifyAreEqual("new text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement mobile");
            //check form field
            //placeholder
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 4,
                "count field mobile");
            VerifyAreEqual("field name 1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("placeholder"), "name placeholder 1 mobile");
            VerifyAreEqual("field mail 2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("placeholder"), "mail placeholder 1 mobile");
            VerifyAreEqual("field phone 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("placeholder"), "comment placeholder 1 mobile");
            VerifyAreEqual("field comment 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetAttribute("placeholder"), "comment placeholder 1 mobile");
            //type input
            VerifyAreEqual("text",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("type"), "name type 1 mobile");
            VerifyAreEqual("email",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("type"), "mail type 1 mobile");
            VerifyAreEqual("tel",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("type"), "comment type 1 mobile");
            //type crm field
            VerifyAreEqual("lpForm.form.fields[0].type=2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[0]
                    .GetAttribute("data-ng-init"), "name crm type 1 mobile");
            VerifyAreEqual("lpForm.form.fields[1].type=5",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[1]
                    .GetAttribute("data-ng-init"), "mail crm type 1 mobile");
            VerifyAreEqual("lpForm.form.fields[2].type=6",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[2]
                    .GetAttribute("data-ng-init"), "comment crm type 1 mobile");
            VerifyAreEqual("lpForm.form.fields[3].type=10",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[3]
                    .GetAttribute("data-ng-init"), "comment crm type 1 mobile");
            //required
            VerifyAreEqual("true",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("data-ng-required"), "name required 1 mobile");
            VerifyAreEqual("true",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("data-ng-required"), "mail required 1 mobile");
            VerifyAreEqual("false",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("data-ng-required"), "comment required 1 mobile");
            VerifyAreEqual("false",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetAttribute("data-ng-required"), "comment required 1 mobile");

            //border color
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetCssValue("border-color"), "border-color 1 mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetCssValue("border-color"), "border-color 2 mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetCssValue("border-color"), "border-color 3 mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetCssValue("border-color"), "border-color 4 mobile");

            //border color user agree (before)
            agree = Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__agreement label span"));
            colorRGB = ((IJavaScriptExecutor) Driver)
                .ExecuteScript(
                    "return window.getComputedStyle(arguments[0], ':before').getPropertyValue('border-color');", agree)
                .ToString();
            VerifyAreEqual("rgb(216, 216, 216)", colorRGB, "border-color agree mobile");

            //empty form
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "form displayed empty form mobile");
            VerifyAreEqual("new text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement empty form mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormBtn\"]")).Count == 1,
                "btn displayed empty form mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormTitle\"]")).Count == 1,
                "title displayed empty form mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 4,
                "count field empty form mobile");
            //border color
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetCssValue("border-color"), "border-color 1 empty form mobile");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetCssValue("border-color"), "border-color 2 empty form mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetCssValue("border-color"), "border-color 3 empty form mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetCssValue("border-color"), "border-color 4 empty form mobile");
            colorRGB = ((IJavaScriptExecutor) Driver)
                .ExecuteScript(
                    "return window.getComputedStyle(arguments[0], ':before').getPropertyValue('border-color');", agree)
                .ToString();
            VerifyAreEqual("rgb(241, 89, 89)", colorRGB, "border-color agree empty form mobile");

            //fill field
            checkField(0, "mobile");
            checkField(2, "999888");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "form displayed no email mobile");
            VerifyAreEqual("new text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement o email  mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormBtn\"]")).Count == 1,
                "btn displayed no email  mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormTitle\"]")).Count == 1,
                "title displayed no email  mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 4,
                "count field no email  mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetCssValue("border-color"), "border-color 1 no email mobile");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetCssValue("border-color"), "border-color 2 no email mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetCssValue("border-color"), "border-color 3 no email mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetCssValue("border-color"), "border-color 4 no email mobile");
            colorRGB = ((IJavaScriptExecutor) Driver)
                .ExecuteScript(
                    "return window.getComputedStyle(arguments[0], ':before').getPropertyValue('border-color');", agree)
                .ToString();
            VerifyAreEqual("rgb(241, 89, 89)", colorRGB, "border-color agree no email mobile");

            VerifyAreEqual("mobile",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("value"), "name value 1 mobile");
            VerifyAreEqual("",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("value"), "mail value 1 mobile");
            VerifyAreEqual("999888",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("value"), "comment value 1 mobile");

            //no valid mail
            checkField(1, "testmail");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "form displayed no email mobile");
            VerifyAreEqual("new text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement no valid email  mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormBtn\"]")).Count == 1,
                "btn displayed no valid email  mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormTitle\"]")).Count == 1,
                "title displayed no valid email  mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 4,
                "count field no valid email  mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetCssValue("border-color"), "border-color 1 no valid email mobile");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetCssValue("border-color"), "border-color 2 no valid email mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetCssValue("border-color"), "border-color 3 no valid email mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetCssValue("border-color"), "border-color 4 no valid email mobile");
            colorRGB = ((IJavaScriptExecutor) Driver)
                .ExecuteScript(
                    "return window.getComputedStyle(arguments[0], ':before').getPropertyValue('border-color');", agree)
                .ToString();
            VerifyAreEqual("rgb(241, 89, 89)", colorRGB, "border-color agree no valid email mobile");

            VerifyAreEqual("mobile",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("value"), "name value 1 no valid email mobile");
            VerifyAreEqual("testmail",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("value"), "mail value 1 no valid email mobile");
            VerifyAreEqual("999888",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("value"), "comment value 1 no valid email mobile");

            // no user argee
            checkField(1, "testmail@mail.com");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form")).Displayed,
                "form displayed no user argee mobile");
            VerifyAreEqual("new text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement no user argee  mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormBtn\"]")).Count == 1,
                "btn displayed no user argee  mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormTitle\"]")).Count == 1,
                "title displayed no user argee  mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 4,
                "count field no user argee  mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetCssValue("border-color"), "border-color 1 no user argee mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetCssValue("border-color"), "border-color 2 no user argee mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetCssValue("border-color"), "border-color 3 no user argee mobile");
            VerifyAreEqual("rgb(216, 216, 216)",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetCssValue("border-color"), "border-color 4 no user argee mobile");
            colorRGB = ((IJavaScriptExecutor) Driver)
                .ExecuteScript(
                    "return window.getComputedStyle(arguments[0], ':before').getPropertyValue('border-color');", agree)
                .ToString();
            VerifyAreEqual("rgb(241, 89, 89)", colorRGB, "border-color agree no user agree mobile");

            VerifyAreEqual("mobile",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("value"), "name value 1 no user argee mobile");
            VerifyAreEqual("testmail@mail.com",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("value"), "mail value 1 no user argee mobile");
            VerifyAreEqual("999888",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("value"), "comment value 1 no user argee mobile");

            //user agree
            Driver.FindElement(By.CssSelector(".lp-form__agreement span")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormBtn\"]")).Count == 0,
                "btn no displayed after agree mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormTitle\"]")).Displayed,
                "title displayed after agree mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 0,
                "count field after agree mobile");

            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "close form all mobile");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Displayed, "no btn mobile");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"FormTitle\"]")).Displayed, "no title mobile");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Count == 0,
                "no SuccessText mobile");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Displayed,
                "no field mobile");

            //check lead grid mobile
            ReInit();
            GoToAdmin("leads?salesFunnelId=1");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName mobile");
            VerifyAreEqual("mobile", Driver.GetGridCell(0, "FullName").Text, "FullName mobile");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount mobile");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum mobile");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date mobile");

            //chek lead cart mobile
            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead mobile");
            VerifyAreEqual("mobile", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead mobile");
            VerifyAreEqual("testmail@mail.com", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead mobile");
            VerifyAreEqual("999888", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"),
                "phone lead mobile");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_LastName")).GetAttribute("value"),
                "lastname lead mobile");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Выбрана услуга: Трансформер дизайна; Цена: 7 000 руб"), "description lead mobile");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Лид из лендинга \"test1\""), "description lead 1 mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormHeadersChange()
        {
            TestName = "BtnServicesFormHeadersChange";
            VerifyBegin(TestName);
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block Form");
            BtnActionButtonSelect("Показ всплывающей формы");
            DelAllColumns("FormGrid");
            FormAddField("field 1", "Имя", true);
            FormAddField("field 2", "Фамилия", true);
            FormAddField("field 3", "Отчество", false);
            FormAddField("field 4", "Страна", false);
            FormAddField("field 5", "Регион", false);
            FormAddField("field 6", "Город", false);
            FormAddField("field 7", "Адрес", false);
            FormAddField("field 8", "Многострочный текст", false);
            BlockSettingsSave();

            //placeholder
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 8,
                "count field");
            VerifyAreEqual("field 1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("placeholder"), "placeholder 1");
            VerifyAreEqual("field 2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("placeholder"), "placeholder 2");
            VerifyAreEqual("field 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("placeholder"), "placeholder 3");
            VerifyAreEqual("field 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[3]
                    .GetAttribute("placeholder"), "placeholder 4");
            VerifyAreEqual("field 5",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[4]
                    .GetAttribute("placeholder"), "placeholder 5");
            VerifyAreEqual("field 6",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[5]
                    .GetAttribute("placeholder"), "placeholder 6");
            VerifyAreEqual("field 7",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[6]
                    .GetAttribute("placeholder"), "placeholder 7");

            //crm type
            VerifyAreEqual("lpForm.form.fields[0].type=2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[0]
                    .GetAttribute("data-ng-init"), "crm type 1");
            VerifyAreEqual("lpForm.form.fields[1].type=1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[1]
                    .GetAttribute("data-ng-init"), "crm type 2");
            VerifyAreEqual("lpForm.form.fields[2].type=3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[2]
                    .GetAttribute("data-ng-init"), "crm type 3");
            VerifyAreEqual("lpForm.form.fields[3].type=7",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[3]
                    .GetAttribute("data-ng-init"), "crm type 4");
            VerifyAreEqual("lpForm.form.fields[4].type=8",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[4]
                    .GetAttribute("data-ng-init"), "crm type 5");
            VerifyAreEqual("lpForm.form.fields[5].type=9",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[5]
                    .GetAttribute("data-ng-init"), "crm type 6");
            VerifyAreEqual("lpForm.form.fields[6].type=12",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[6]
                    .GetAttribute("data-ng-init"), "crm type 7");
            VerifyAreEqual("lpForm.form.fields[7].type=11",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[7]
                    .GetAttribute("data-ng-init"), "crm type 8");

            //drag 'n drop
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            DragDrop(1, 7, "FormGrid");
            DragDrop(0, 6, "FormGrid");
            DragDrop(4, 0, "FormGrid");
            DragDrop(2, 3, "FormGrid");
            BlockSettingsSave();

            VerifyAreEqual("field 7",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("placeholder"), "placeholder 1");
            VerifyAreEqual("field 3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("placeholder"), "placeholder 2");
            VerifyAreEqual("field 5",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[2]
                    .GetAttribute("placeholder"), "placeholder 3");
            VerifyAreEqual("field 4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[3]
                    .GetAttribute("placeholder"), "placeholder 4");
            VerifyAreEqual("field 6",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[4]
                    .GetAttribute("placeholder"), "placeholder 5");
            VerifyAreEqual("field 8",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))[0]
                    .GetAttribute("placeholder"), "placeholder 6");
            VerifyAreEqual("field 1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[5]
                    .GetAttribute("placeholder"), "placeholder 7");
            VerifyAreEqual("field 2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[6]
                    .GetAttribute("placeholder"), "placeholder 7");

            VerifyAreEqual("lpForm.form.fields[0].type=12",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[0]
                    .GetAttribute("data-ng-init"), "crm type 1");
            VerifyAreEqual("lpForm.form.fields[1].type=3",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[1]
                    .GetAttribute("data-ng-init"), "crm type 2");
            VerifyAreEqual("lpForm.form.fields[2].type=8",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[2]
                    .GetAttribute("data-ng-init"), "crm type 3");
            VerifyAreEqual("lpForm.form.fields[3].type=7",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[3]
                    .GetAttribute("data-ng-init"), "crm type 4");
            VerifyAreEqual("lpForm.form.fields[4].type=9",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[4]
                    .GetAttribute("data-ng-init"), "crm type 5");
            VerifyAreEqual("lpForm.form.fields[5].type=11",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[5]
                    .GetAttribute("data-ng-init"), "crm type 6");
            VerifyAreEqual("lpForm.form.fields[6].type=2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[6]
                    .GetAttribute("data-ng-init"), "crm type 7");
            VerifyAreEqual("lpForm.form.fields[7].type=1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[7]
                    .GetAttribute("data-ng-init"), "crm type 8");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormHeadersCutomerFields()
        {
            TestName = "BtnServicesFormHeadersCutomerFields";
            VerifyBegin(TestName);
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block Form");
            BtnActionButtonSelect("Показ всплывающей формы");

            //Hidden user Agreement
            FormHiddenAgreement();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AgreementText\"]")).Count == 0,
                "no field user agree");

            //location test field
            FormTitleLocationSelect("Над полем");

            DelAllColumns("FormGrid");
            FormAddField("cust field 1 select", "Customer Field 1", false);
            FormAddField("cust field 2 text", "Customer Field 2", true);
            FormAddField("cust field 3 number", "Customer Field 3", false);
            FormAddField("cust field 4 date", "Customer Field 4", false);
            FormAddField("cust field 5 area", "Customer Field 5", false);
            FormAddField("cust field 6 name", "Имя", false);
            BlockSettingsSave();

            //open form
            Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]"))[1].Click();
            Thread.Sleep(1000);

            //title field
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 6,
                "count field");
            VerifyAreEqual("cust field 1 select",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field-label"))[0].Text,
                "placeholder 1");
            VerifyAreEqual("cust field 2 text",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field-label"))[1].Text,
                "placeholder 2");
            VerifyAreEqual("cust field 3 number",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field-label"))[2].Text,
                "placeholder 3");
            VerifyAreEqual("cust field 4 date",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field-label"))[3].Text,
                "placeholder 4");
            VerifyAreEqual("cust field 5 area",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field-label"))[4].Text,
                "placeholder 5");
            VerifyAreEqual("",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("placeholder"), "no placeholder 1 ");

            //crm type
            VerifyAreEqual("lpForm.form.fields[0].type=4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[0]
                    .GetAttribute("data-ng-init"), "crm type 1");
            VerifyAreEqual("lpForm.form.fields[1].type=4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[1]
                    .GetAttribute("data-ng-init"), "crm type 2");
            VerifyAreEqual("lpForm.form.fields[2].type=4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[2]
                    .GetAttribute("data-ng-init"), "crm type 3");
            VerifyAreEqual("lpForm.form.fields[3].type=4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[3]
                    .GetAttribute("data-ng-init"), "crm type 4");
            VerifyAreEqual("lpForm.form.fields[4].type=4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[4]
                    .GetAttribute("data-ng-init"), "crm type 5");

            //fill fields
            checkField(2, "07.09.2000");
            checkField(0, "text field 2");
            checkField(1, "3333");
            checkField(3, "name");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))
                .SendKeys("area field 5");
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field select")))
                .SelectByText("Customer Field 1 Value 1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-form__agreement")).Count == 0, "no agreement");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormBtn\"]")).Count == 0,
                "btn no displayed after send ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormTitle\"]")).Displayed,
                "title displayed after send ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after send ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 0,
                "count field after send ");

            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".lp-form")).Displayed, "close form all ");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Displayed, "no btn ");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"FormTitle\"]")).Displayed, "no title ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Count == 0,
                "no SuccessText ");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Displayed,
                "no field ");

            //check lead grid 
            GoToAdmin("leads?salesFunnelId=1");
            VerifyAreEqual("Новый", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("name", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            //chek lead cart 
            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead mobile");
            VerifyAreEqual("name", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"), "phone lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_LastName")).GetAttribute("value"),
                "lastname lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Выбрана услуга: CRM внутри; Цена: 10 000 руб"), "description lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Лид из лендинга \"test1\""), "description lead 1 ");

            selectElem1 = Driver.FindElement(By.Id("customerfields_0__value"));
            select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("Customer Field 1 Value 1"), "customerfields_0 lead");
            VerifyAreEqual("text field 2", Driver.FindElement(By.Id("customerfields_1__value")).GetAttribute("value"),
                "customerfields_1 lead ");
            VerifyAreEqual("3333", Driver.FindElement(By.Id("customerfields_2__value")).GetAttribute("value"),
                "customerfields_2 lead ");
            VerifyAreEqual("07.09.2000", Driver.FindElement(By.Id("customerfields_3_value")).GetAttribute("value"),
                "customerfields_3 lead ");
            VerifyAreEqual("area field 5", Driver.FindElement(By.Id("customerfields_4__value")).GetAttribute("value"),
                "customerfields_4 lead ");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormHeadersEdited()
        {
            TestName = "BtnServicesFormHeadersEdited";
            VerifyBegin(TestName);
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block Form");
            BtnActionButtonSelect("Показ всплывающей формы");
            FormSetTitle("Title");
            FormSetSubTitle("SubTitle");
            FormSetButtonText("Button");


            //Hidden user Agreement
            FormShowAgreement("Text user agreement");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AgreementText\"]")).Count == 1,
                "field user agree");
            //location test field
            FormTitleLocationSelect("Над полем");

            DelAllColumns("FormGrid");
            FormAddField("field 1 name", "Имя", true);
            FormAddField("field 2 lastname", "Фамилия", false);
            BlockSettingsSave();

            //open form
            Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]"))[1].Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Title", Driver.FindElement(By.CssSelector("[data-e2e=\"FormTitle\"]")).Text, "text title");
            VerifyAreEqual("SubTitle", Driver.FindElement(By.CssSelector("[data-e2e=\"FormSubTitle\"]")).Text,
                "text sub title");
            VerifyAreEqual("Button", Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Text, "text btn");
            VerifyAreEqual("Text user agreement", Driver.FindElement(By.CssSelector(".lp-form__agreement")).Text,
                "text agreement admin");

            //title field
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 2,
                "count field");
            VerifyAreEqual("field 1 name",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field-label"))[0].Text,
                "placeholder 1");
            VerifyAreEqual("field 2 lastname",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field-label"))[1].Text,
                "placeholder 2");
            VerifyAreEqual("",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("placeholder"), "no placeholder 1 ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-form__agreement")).Displayed, "field user agree");

            //crm type
            VerifyAreEqual("lpForm.form.fields[0].type=2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[0]
                    .GetAttribute("data-ng-init"), "crm type 1");
            VerifyAreEqual("lpForm.form.fields[1].type=1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[1]
                    .GetAttribute("data-ng-init"), "crm type 2");

            //required
            VerifyAreEqual("true",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("data-ng-required"), "name required 1");
            VerifyAreEqual("false",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("data-ng-required"), "mail required 1");
            BlockSettingsClose();

            //edit
            BlockSettingsBtn();
            FormSetTitle("New Title");
            FormSetSubTitle("New SubTitle");
            FormSetButtonText("New Button");

            //Hidden user Agreement
            FormHiddenAgreement();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"AgreementText\"]")).Count == 0,
                "field user agree no");
            //location test field
            FormTitleLocationSelect("Внутри поля");

            Driver.GetGridCell(0, "Title").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "Title").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "Title").FindElement(By.TagName("input")).SendKeys("new mail");
            new SelectElement(Driver.GetGridCell(0, "TitleCrm").FindElement(By.TagName("select"))).SelectByText("Email");
            Driver.GetGridCell(0, "Required").FindElement(By.TagName("span")).Click();
            Driver.GetGridCell(1, "Required").FindElement(By.TagName("span")).Click();
            BlockSettingsSave();

            //check edit
            Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]"))[1].Click();
            Thread.Sleep(1000);
            VerifyAreEqual("New Title", Driver.FindElement(By.CssSelector("[data-e2e=\"FormTitle\"]")).Text,
                "text title admin");
            VerifyAreEqual("New SubTitle", Driver.FindElement(By.CssSelector("[data-e2e=\"FormSubTitle\"]")).Text,
                "text sub title admin");
            VerifyAreEqual("New Button", Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Text,
                "text btn admin");

            //title field
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field")).Count == 2,
                "count field edit");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field-label")).Count == 0,
                " count lable edit");
            VerifyAreEqual("new mail",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("placeholder"), "placeholder 1 edit");
            VerifyAreEqual("field 2 lastname",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("placeholder"), "placeholder 2 edit");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".lp-form__agreement")).Count == 0, " no field user agree");

            //crm type
            VerifyAreEqual("lpForm.form.fields[0].type=5",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[0]
                    .GetAttribute("data-ng-init"), "crm type 1 edit");
            VerifyAreEqual("lpForm.form.fields[1].type=1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field"))[1]
                    .GetAttribute("data-ng-init"), "crm type 2 edit");

            //required
            VerifyAreEqual("false",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[0]
                    .GetAttribute("data-ng-required"), "name required 1 edit");
            VerifyAreEqual("true",
                Driver.FindElements(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field input"))[1]
                    .GetAttribute("data-ng-required"), "mail required 1 edit");

            VerifyFinally(TestName);
        }
    }
}