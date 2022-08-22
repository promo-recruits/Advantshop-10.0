using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Services.servicesThreeColumns.Button.Form
{
    [TestFixture]
    public class LandingsServicesColumnsBtnFormActionPageMail : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing | ClearType.CRM);
            InitializeService.LoadData(
                //        "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.Color.csv",
                //        "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.Size.csv",
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
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\SendMail\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.DealStatus.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.SalesFunnel.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.SalesFunnel_DealStatus.csv"
            );

            Init();
            Functions.MailSmtp(Driver, BaseUrl);
        }

        private string mail = "";

        [Test]
        public void BtnServicesFormPageSendLead()
        {
            TestName = "BtnServicesFormPageSendLead";
            VerifyBegin(TestName);
            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу и отправка письма");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")).Displayed,
                "show field");
            FormActionPostMessageRedirectSelect("page1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlRedirect\"]")).Count == 0,
                "show field UrlRedirect");
            FormActionSendLead();
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 3");
            FormActionEmailSubject("Subject mail");
            Driver.SetCkText(
                "Text message: name: #NAME#, phone: #PHONE#, mail: #EMAIL#, comment: #COMMENTS#, city: #CITY#, country: #COUNTRY#, region: #REGION#, customerfield: #ADDITIONALCUSTOMERFIELDS#",
                "editor2");

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "name");
            checkField(1, "soname");
            checkField(3, "89012345672");
            checkField(4, "TestCountry");
            checkField(5, "TestRegion");
            checkField(6, "TestCity");
            new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field select")))
                .SelectByText("Customer Field 1 Value 2");
            checkField(7, "TestCustom1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormField\"] .lp-form__field textarea"))
                .SendKeys("TestComment");
            Thread.Sleep(1000);
            mail = DateTime.Now.ToString("ddMMHHmm") + "@test.mail";
            checkField(2, mail);

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Count == 0,
                " no SuccessText displayed");
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/test1/page1?lid="), "url by btn");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo")).Displayed, "logo on page");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
            VerifyAreEqual("Funnel 3 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("soname name", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead ");
            VerifyAreEqual("name", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual("soname", Driver.FindElement(By.Id("Lead_Customer_LastName")).GetAttribute("value"),
                "lastname lead ");
            VerifyAreEqual(mail, Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");
            VerifyAreEqual("89012345672", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"),
                "phone lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Выбрана услуга: Трансформер дизайна; Цена: 7 000 руб"), "description lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Лид из лендинга \"test1\""), "description lead 1 ");
            VerifyAreEqual("TestComment", Driver.FindElement(By.CssSelector(".pre-line")).Text, "comment text");

            VerifyAreEqual("Письма 1", Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Text,
                "countert mail");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 1, "count event");

            VerifyAreEqual("Subject mail",
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] [data-e2e=\"LeadEventType\"]")).Text,
                "subject mail");
            Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] ui-modal-trigger")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Subject mail", Driver.FindElement(By.CssSelector(".modal-body h2")).Text,
                "subject mail in modal");
            SwitcToIframe();
            Thread.Sleep(1000);
            var bodymail = Driver.FindElement(By.TagName("body")).Text;
            VerifyAreEqual(
                "Text message: name: soname name, phone: 89012345672, mail: " + mail +
                ", comment: TestComment, city: TestCity, country: TestCountry, region: TestRegion, customerfield:\r\nCustomer Field 1:Customer Field 1 Value 2\r\nCustomer Field 2:TestCustom1",
                bodymail, "mail text");


            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormPageSendLeadEmptyField()
        {
            TestName = "BtnServicesFormPageSendLeadEmptyField";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу и отправка письма");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")).Displayed,
                "show field");
            FormActionPostMessageRedirectSelect("page1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlRedirect\"]")).Count == 0,
                "show field UrlRedirect");
            FormActionSendLead();
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 3");
            FormActionEmailSubject("Subject mail empty");
            Driver.SetCkText(
                "Text message: name: #NAME#, phone: #PHONE#, mail: #EMAIL#, comment: #COMMENTS#, city: #CITY#, country: #COUNTRY#, region: #REGION#, customerfield: #ADDITIONALCUSTOMERFIELDS#",
                "editor1");

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "empty");
            mail = DateTime.Now.ToString("ddMMHHmm") + "@empty.mail";
            checkField(2, mail);

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Count == 0,
                " no SuccessText displayed");
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/test1/page1?lid="), "url by btn");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo")).Displayed, "logo on page");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
            VerifyAreEqual("Funnel 3 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("empty", Driver.GetGridCell(0, "FullName").Text, "FullName");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead ");
            VerifyAreEqual("empty", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_LastName")).GetAttribute("value"),
                "lastname lead ");
            VerifyAreEqual(mail, Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"), "phone lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Выбрана услуга: Трансформер дизайна; Цена: 7 000 руб"), "description lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Лид из лендинга \"test1\""), "description lead 1 ");

            VerifyAreEqual("Письма 1", Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Text,
                "countert mail");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 1,
                "count event");

            VerifyAreEqual("Subject mail empty",
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] [data-e2e=\"LeadEventType\"]")).Text,
                "subject mail");
            Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] ui-modal-trigger")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Subject mail empty", Driver.FindElement(By.CssSelector(".modal-body h2")).Text,
                "subject mail in modal");

            SwitcToIframe();
            var bodymail = Driver.FindElement(By.TagName("body")).Text;
            VerifyAreEqual(
                "Text message: name: empty, phone: , mail: " + mail +
                ", comment: , city: , country: , region: , customerfield:", bodymail, "mail text");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormPageSendLeadEmptyMail()
        {
            TestName = "BtnServicesFormPageSendLeadEmpty";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу и отправка письма");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")).Displayed,
                "show field");
            FormActionPostMessageRedirectSelect("page1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlRedirect\"]")).Count == 0,
                "show field UrlRedirect");
            FormActionSendLead();
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 3");
            FormActionEmailSubject("Subject mail");
            Driver.SetCkText(
                "Text message: name: #NAME#, phone: #PHONE#, mail: #EMAIL#, comment: #COMMENTS#, city: #CITY#, country: #COUNTRY#, region: #REGION#, customerfield: #ADDITIONALCUSTOMERFIELDS#",
                "editor1");

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "empty name");

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Count == 0,
                " no SuccessText displayed");
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/test1/page1?lid="), "url by btn");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo")).Displayed, "logo on page");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
            VerifyAreEqual("Funnel 3 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("empty name", Driver.GetGridCell(0, "FullName").Text, "FullName");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead ");
            VerifyAreEqual("empty name", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_LastName")).GetAttribute("value"),
                "lastname lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"), "phone lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Выбрана услуга: Трансформер дизайна; Цена: 7 000 руб"), "description lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Лид из лендинга \"test1\""), "description lead 1 ");

            VerifyAreEqual("Письма 0", Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Text,
                "no mail");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 0, "no event");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormPageSendNolead()
        {
            TestName = "BtnServicesFormPageSendNolead";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу и отправка письма");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")).Displayed,
                "show field");
            FormActionPostMessageRedirectSelect("page1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlRedirect\"]")).Count == 0,
                "show field UrlRedirect");
            FormActionDontSendLead();
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 1");
            FormActionEmailSubject("Subject mail no lead");
            Driver.SetCkText("Its Text message no lead", "editor1");
            BlockSettingsSave();


            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name page");
            mail = DateTime.Now.ToString("ddMMHHmm") + "@page.mail";
            checkField(2, mail);
            checkField(3, "89012345677");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Count == 0,
                " no SuccessText displayed");
            VerifyAreEqual(BaseUrl + "/lp/test1/page1", Driver.Url, "url by btn");
            VerifyIsFalse(Is404Page(BaseUrl + "/lp/test1/page1"), "not 404 page");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo")).Displayed, "logo on page");

            GoToAdmin("leads?salesFunnelId=1");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
            VerifyAreEqual("Funnel 1 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name page", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("ui-grid-custom .ui-grid-custom-filter-total")).Text.Contains("1"),
                "count lead 2");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead ");
            VerifyAreEqual("test name page", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual(mail, Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");
            VerifyAreEqual("89012345677", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"),
                "phone lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_LastName")).GetAttribute("value"),
                "lastname lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Выбрана услуга: Трансформер дизайна; Цена: 7 000 руб"), "description lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Лид из лендинга \"test1\""), "description lead 1 ");

            VerifyAreEqual("Письма 1", Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Text,
                " mail");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 1, " event");

            VerifyAreEqual("Subject mail no lead",
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] [data-e2e=\"LeadEventType\"]")).Text,
                "subject mail");
            Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] ui-modal-trigger")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Subject mail no lead", Driver.FindElement(By.CssSelector(".modal-body h2")).Text,
                "subject mail in modal");

            SwitcToIframe();
            var bodymail = Driver.FindElement(By.TagName("body")).Text;
            VerifyAreEqual("Its Text message no lead", bodymail, "mail text");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormPageSendUrlLead()
        {
            TestName = "BtnServicesFormPageSendUrlLead";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу и отправка письма");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")).Displayed,
                "show field");
            FormActionPostMessageRedirectSelect("Указать свой URL-адрес");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlRedirect\"]")).Count == 1,
                "show field UrlRedirect");

            var selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]"));
            var select = new SelectElement(selectElem);

            var allOptions = select.Options;
            VerifyIsTrue(allOptions.Any(item => item.Text.ToString() == "page1"), "another page landing in select");
            VerifyIsTrue(allOptions.Any(item => item.Text.ToString() == "Указать свой URL-адрес"), "own url in select");

            //unknown url
            FormActionUrlRedirect("unknownurl");

            FormActionSendLead();
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 3");
            FormActionEmailSubject("Subject mail to url lead");
            Driver.SetCkText("Its Text message to url lead", "editor1");

            BlockSettingsSave();


            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name unknownurl");
            mail = DateTime.Now.ToString("ddMMHHmm") + "@unknownurl.mail";
            checkField(2, mail);
            checkField(3, "89012345675");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.Url.Contains(BaseUrl + "/lp/"), "url by btn unknownurl");
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/unknownurl?lid="), "url by btn unknownurl 2");


            var lid = Driver.Url;
            var words = lid.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
            var count = words.Length;
            lid = words[count - 1];

            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
            VerifyAreEqual(lid, Driver.GetGridCell(0, "Id").Text, "Id ");
            VerifyAreEqual("Funnel 3 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name unknownurl", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");


            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            TabSelect("tabHeaderActions");
            FormActionPostMessageRedirectSelect("Указать свой URL-адрес");
            FormActionUrlRedirect("products/test-product1");
            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name enabled");
            mail = DateTime.Now.ToString("ddMMHHmm") + "@enabled.mail";
            checkField(2, mail);
            checkField(3, "89012345674");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/products/test-product1?lid="), "url by btn enabled");
            VerifyIsFalse(Is404Page(Driver.Url));
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyAreEqual("Funnel 3 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name enabled", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("test name enabled",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual(mail, Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");
            VerifyAreEqual("89012345674", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"),
                "phone lead ");

            VerifyAreEqual("Письма 1", Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Text,
                " mail");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 1, " event");

            VerifyAreEqual("Subject mail to url lead",
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] [data-e2e=\"LeadEventType\"]")).Text,
                "subject mail");
            Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] ui-modal-trigger")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Subject mail to url lead", Driver.FindElement(By.CssSelector(".modal-body h2")).Text,
                "subject mail in modal");

            SwitcToIframe();
            var bodymail = Driver.FindElement(By.TagName("body")).Text;
            VerifyAreEqual("Its Text message to url lead", bodymail, "mail text");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormPageSendUrlNoLead()
        {
            TestName = "BtnServicesFormPageSendUrlNoLead";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу и отправка письма");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")).Displayed,
                "show field");
            FormActionPostMessageRedirectSelect("Указать свой URL-адрес");
            FormActionUrlRedirect("products/test-product5");
            FormActionDontSendLead();

            FormActionEmailSubject("Subject mail to url no lead");
            Driver.SetCkText("Its Text message to url no lead", "editor1");

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name UrlNoLead");
            mail = DateTime.Now.ToString("ddMMHHmm") + "@UrlNoLead.mail";
            checkField(2, mail);
            checkField(3, "89012345673");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(BaseUrl + "/products/test-product5", Driver.Url, "url by btn enabled");
            VerifyIsFalse(Is404Page(Driver.Url));
            VerifyAreEqual("TestProduct5", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyAreEqual("Funnel 3 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name UrlNoLead", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            VerifyAreEqual("test name UrlNoLead",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual(mail, Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");
            VerifyAreEqual("89012345673", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"),
                "phone lead ");

            VerifyAreEqual("Письма 1", Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Text,
                " mail");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 1, " event");

            VerifyAreEqual("Subject mail to url no lead",
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] [data-e2e=\"LeadEventType\"]")).Text,
                "subject mail");
            Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] ui-modal-trigger")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Subject mail to url no lead", Driver.FindElement(By.CssSelector(".modal-body h2")).Text,
                "subject mail in modal");

            SwitcToIframe();
            var bodymail = Driver.FindElement(By.TagName("body")).Text;
            VerifyAreEqual("Its Text message to url no lead", bodymail, "mail text");
            VerifyFinally(TestName);
        }


        [Test]
        public void BtnServicesFormPageSendUrlProduct()
        {
            TestName = "BtnServicesFormPageSendUrlProduct";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу и отправка письма");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")).Displayed,
                "show field");
            FormActionPostMessageRedirectSelect("page1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlRedirect\"]")).Count == 0,
                "show field UrlRedirect");
            FormActionSendLead();
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 3");
            FormActionEmailSubject("Subject mail Product");
            Driver.SetCkText(
                "Text message Product: name: #NAME#, phone: #PHONE#, mail: #EMAIL#, comment: #COMMENTS#, city: #CITY#, country: #COUNTRY#, region: #REGION#, customerfield: #ADDITIONALCUSTOMERFIELDS#",
                "editor1");

            TabSelect("tabFormSetting_0");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product");

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "Product");
            mail = DateTime.Now.ToString("ddMMHHmm") + "@Product.mail";
            checkField(2, mail);

            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Count == 0,
                " no SuccessText displayed");
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/test1/page1?lid="), "url by btn");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo")).Displayed, "logo on page");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
            VerifyAreEqual("Funnel 3 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("Product", Driver.GetGridCell(0, "FullName").Text, "FullName");
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead ");
            VerifyAreEqual("Product", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_LastName")).GetAttribute("value"),
                "lastname lead ");
            VerifyAreEqual(mail, Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"), "phone lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Выбрана услуга: Трансформер дизайна; Цена: 7 000 руб"), "description lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Лид из лендинга \"test1\""), "description lead 1 ");

            //check product grid
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");
            VerifyAreEqual("TestProduct1\r\nАртикул: 1", Driver.GetGridCell(0, "Name", "LeadItems").Text,
                "Name product at order");
            VerifyAreEqual("100", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("100 руб."), " Cost product at order");

            VerifyAreEqual("Письма 1", Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Text,
                "countert mail");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 1,
                "count event");

            VerifyAreEqual("Subject mail Product",
                Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] [data-e2e=\"LeadEventType\"]")).Text,
                "subject mail");
            Driver.FindElement(By.CssSelector("[data-e2e=\"EventBlock-email\"] ui-modal-trigger")).Click();
            Thread.Sleep(1000);
            VerifyAreEqual("Subject mail Product", Driver.FindElement(By.CssSelector(".modal-body h2")).Text,
                "subject mail in modal");

            SwitcToIframe();
            var bodymail = Driver.FindElement(By.TagName("body")).Text;
            VerifyAreEqual(
                "Text message Product: name: Product, phone: , mail: " + mail +
                ", comment: , city: , country: , region: , customerfield:", bodymail, "mail text");

            VerifyFinally(TestName);
        }

        public void SwitcToIframe(string cssSelector = "")
        {
            var iframe = Driver.FindElement(By.CssSelector(cssSelector + " iframe"));
            Driver.SwitchTo().Frame(iframe);
            Thread.Sleep(1000);
        }
    }
}