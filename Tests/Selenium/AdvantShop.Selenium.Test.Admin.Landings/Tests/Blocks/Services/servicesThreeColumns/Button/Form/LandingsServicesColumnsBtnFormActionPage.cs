using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Services.servicesThreeColumns.Button.Form
{
    [TestClass]
    public class LandingsServicesColumnsBtnFormActionPage : LandingsFunctions
    {
        private string mail;

        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing | ClearType.CRM);
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
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.DealStatus.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.SalesFunnel.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.SalesFunnel_DealStatus.csv"
            );
            Init();
        }

        [Test]
        public void BtnServicesFormPageLead()
        {
            TestName = "BtnServicesFormPageLead";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")).Displayed,
                "show field");
            FormActionPostMessageRedirectSelect("page1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlRedirect\"]")).Count == 0,
                "show field UrlRedirect");
            FormActionSendLead();
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 3");
            BlockSettingsSave();


            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name PageLead");
            mail = DateTime.Now.ToString("ddMMHHmm") + "@PageLead.mail";
            checkField(1, mail);
            checkField(2, "89012345676");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Count == 0,
                " no SuccessText displayed");
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/lp/test1/page1?lid="), "url by btn");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".lp-header-logo")).Displayed, "logo on page");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
            VerifyAreEqual("Funnel 3 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name PageLead", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead ");
            VerifyAreEqual("test name PageLead",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual(mail, Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");
            VerifyAreEqual("89012345676", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"),
                "phone lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_LastName")).GetAttribute("value"),
                "lastname lead ");
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
        public void BtnServicesFormPageNolead()
        {
            TestName = "BtnServicesFormPageNolead";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")).Displayed,
                "show field");
            FormActionPostMessageRedirectSelect("page1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"UrlRedirect\"]")).Count == 0,
                "show field UrlRedirect");
            FormActionDontSendLead();
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 1");
            BlockSettingsSave();


            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name page");
            mail = DateTime.Now.ToString("ddMMHHmm") + "@page.mail";
            checkField(1, mail);
            checkField(2, "89012345677");
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
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".pre-line")).Count == 0, "no coment lead ");
            VerifyAreEqual("Письма 0", Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Text,
                "no mail");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 0, "no event");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormPageUrlLead()
        {
            TestName = "BtnServicesFormPageUrlLead";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу");
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
            BlockSettingsSave();


            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name unknownurl");
            mail = DateTime.Now.ToString("ddMMHHmm") + "@unknownurl.mail";
            checkField(1, mail);
            checkField(2, "89012345675");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsFalse(Driver.Url.Contains(BaseUrl + "/lp/"), "url by btn unknownurl");
            VerifyIsTrue(Driver.Url.Contains(BaseUrl + "/unknownurl?lid="), "url by btn unknownurl 2");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
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
            checkField(1, mail);
            checkField(2, "89012345674");
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
            VerifyAreEqual("Письма 0", Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Text,
                "no mail");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 0, "no event");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormPageUrlNoLead()
        {
            TestName = "BtnServicesFormPageUrlNoLead";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")).Displayed,
                "show field");
            FormActionPostMessageRedirectSelect("Указать свой URL-адрес");
            FormActionUrlRedirect("products/test-product5");
            FormActionDontSendLead();
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 3");

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name UrlNoLead");
            mail = DateTime.Now.ToString("ddMMHHmm") + "@UrlNoLead.mail";
            checkField(1, mail);
            checkField(2, "89012345673");
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
            VerifyAreEqual("Письма 0", Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Text,
                "no mail");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 0, "no event");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnServicesFormPageUrlProduct()
        {
            TestName = "BtnServicesFormPageUrlProduct";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Переход на страницу");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"PostMessageRedirect\"]")).Displayed,
                "show field");
            FormActionPostMessageRedirectSelect("Указать свой URL-адрес");
            FormActionUrlRedirect("products/test-product1");
            FormActionDontSendLead();
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 3");

            //add product
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
            checkField(0, "test name product");
            mail = DateTime.Now.ToString("ddMMHHmm") + "@product.mail";
            checkField(1, mail);
            checkField(2, "89012345671");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);

            VerifyAreEqual(BaseUrl + "/products/test-product1", Driver.Url, "url by btn enabled");
            VerifyIsFalse(Is404Page(Driver.Url));
            VerifyAreEqual("TestProduct1", Driver.FindElement(By.TagName("h1")).Text, "h1 on page");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyAreEqual("Funnel 3 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name product", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("1", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("100 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead ");
            VerifyAreEqual("test name product",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual(mail, Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");
            VerifyAreEqual("89012345671", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"),
                "phone lead ");
            VerifyAreEqual("", Driver.FindElement(By.Id("Lead_Customer_LastName")).GetAttribute("value"),
                "lastname lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Выбрана услуга: Трансформер дизайна; Цена: 7 000 руб"), "description lead ");
            VerifyIsTrue(
                Driver.FindElement(By.Id("Lead_Description")).GetAttribute("value")
                    .Contains("Лид из лендинга \"test1\""), "description lead 1 ");
            VerifyAreEqual("Письма 0", Driver.FindElement(By.CssSelector("[data-e2e=\"eventTypeEmail\"]")).Text,
                "no mail");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"EventBlock-email\"]")).Count == 0, "no event");

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

            VerifyFinally(TestName);
        }
    }
}