using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Services.servicesThreeColumns.Button.Form
{
    [TestFixture]
    public class LandingsServicesColumnsBtnFormActionMessage : LandingsFunctions
    {
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
        public void BtnServicesFormMessage()
        {
            TestName = "BtnServicesFormMessage";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            Driver.SetCkText("Text messange after form send", "editor1");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");
            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name");
            checkField(1, "test@name.test");
            checkField(2, "89012345678");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");
            VerifyAreEqual("Text messange after form send",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Text, "SuccessText ");


            GoToAdmin("leads?salesFunnelId=1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("ui-grid-custom .ui-grid-custom-filter-total")).Text.Contains("0"),
                "count lead ");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name", Driver.GetGridCell(0, "FullName").Text, "FullName ");
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
            VerifyAreEqual("test name", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual("test@name.test", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");
            VerifyAreEqual("89012345678", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"),
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
        public void BtnServicesFormMessageEdit()
        {
            TestName = "BtnServicesFormMessageEdit";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            Driver.SetCkText("New Text messange after form send", "editor1");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 3");

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name edit");
            checkField(1, "test@name.edit");
            checkField(2, "89012345679");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");
            VerifyAreEqual("New Text messange after form send",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Text, "SuccessText ");

            GoToAdmin("leads?salesFunnelId=1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("ui-grid-custom .ui-grid-custom-filter-total")).Text.Contains("0"),
                "count lead ");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyAreEqual("Funnel 3 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name edit", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("0", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("0 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("ui-grid-custom .ui-grid-custom-filter-total")).Text.Contains("1"),
                "count lead 3");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead ");
            VerifyAreEqual("test name edit", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual("test@name.edit", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");
            VerifyAreEqual("89012345679", Driver.FindElement(By.Id("Lead_Customer_Phone")).GetAttribute("value"),
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
        public void BtnServicesFormMessageProduct()
        {
            TestName = "BtnServicesFormMessageProduct";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            Driver.SetCkText("New Text messange after form send product", "editor1");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 3");

            //add product
            TabSelect("tabFormSetting_0");
            DelAllProduct();
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
            checkField(1, "test@name.product");
            checkField(2, "89012345671");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");
            VerifyAreEqual("New Text messange after form send product",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Text, "SuccessText ");

            GoToAdmin("leads?salesFunnelId=1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("ui-grid-custom .ui-grid-custom-filter-total")).Text.Contains("0"),
                "count lead ");

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
            VerifyAreEqual("test@name.product", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");
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

        [Test]
        public void BtnServicesFormMessageMultyProduct()
        {
            TestName = "BtnServicesFormMessageMultyProduct";
            VerifyBegin(TestName);
            GoToClient("lp/test1");

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            Driver.SetCkText("New Text messange after form send product", "editor1");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 3");

            //add product
            TabSelect("tabFormSetting_0");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_10\"]")).Displayed,
                "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_100\"]")).Displayed,
                "display product 3");
            FormProdOfferPrice("1000", 1);
            FormProdCount("10", 2);

            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name multyproduct");
            checkField(1, "test@name.multyproduct");
            checkField(2, "89012345677");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");
            VerifyAreEqual("New Text messange after form send product",
                Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Text, "SuccessText ");

            GoToAdmin("leads?salesFunnelId=1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no elem");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("ui-grid-custom .ui-grid-custom-filter-total")).Text.Contains("0"),
                "count lead ");

            GoToAdmin("leads?salesFunnelId=3");
            VerifyAreEqual("Funnel 3 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name multyproduct", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("12", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("3 090 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);
            var selectElem1 = Driver.FindElement(By.Id("Lead_OrderSourceId"));
            var select3 = new SelectElement(selectElem1);
            VerifyAreEqual("Воронка продаж \"test1\"", select3.SelectedOption.Text, "order source lead ");
            VerifyAreEqual("test name multyproduct",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual("test@name.multyproduct",
                Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"), "mail lead ");
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

            VerifyAreEqual("TestProduct10\r\nАртикул: 10", Driver.GetGridCell(1, "Name", "LeadItems").Text,
                "Name product at order 2");
            VerifyAreEqual("1000", Driver.GetGridCell(1, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("1", Driver.GetGridCell(1, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyIsTrue(Driver.GetGridCell(1, "Cost", "LeadItems").Text.Contains("1 000 руб."), " Cost product at order 2");

            VerifyAreEqual("TestProduct100\r\nАртикул: 100", Driver.GetGridCell(2, "Name", "LeadItems").Text,
                "Name product at order 3");
            VerifyAreEqual("199", Driver.GetGridCell(2, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 3");
            VerifyAreEqual("10", Driver.GetGridCell(2, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 3");
            VerifyIsTrue(Driver.GetGridCell(2, "Cost", "LeadItems").Text.Contains("1 990 руб."), " Cost product at order 3");

            VerifyFinally(TestName);
        }
    }
}