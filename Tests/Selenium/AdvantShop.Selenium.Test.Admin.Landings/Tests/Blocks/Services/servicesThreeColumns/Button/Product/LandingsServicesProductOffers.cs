using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Services.servicesThreeColumns.Button.Product
{
    [TestFixture]
    public class LandingsServicesProductOffers : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing | ClearType.CRM);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Color.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Size.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\Catalog.ProductCategories.csv",
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
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Product\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.DealStatus.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.SalesFunnel.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CRM.SalesFunnel_DealStatus.csv"
            );

            Init();
        }

        [Test]
        public void ServicesFormAddOne()
        {
            TestName = "ServicesFormAddOne";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Product_1\"]")).Count == 0,
                " no display product");
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            //check grid
            VerifyAreEqual("1", Driver.GetGridCell(0, "ArtNo", "OffersSelectvizr").Text, "ArtNo product at grid");
            VerifyAreEqual("TestProduct1", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid");

            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product");

            //change offer and count
            FormProdOfferPrice("999");
            FormProdCount("5");
            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name product");
            checkField(2, "test@name.product");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name product", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("5", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("4 995 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("4995", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name product",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual("test@name.product", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");

            //check product grid
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");
            VerifyAreEqual("TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1",
                Driver.GetGridCell(0, "Name", "LeadItems").Text, "Name product at order");
            VerifyAreEqual("999", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("5", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("4 995 руб."), " Cost product at order");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSome()
        {
            TestName = "ServicesFormAddSome";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct1");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct2");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_10\"]")).Displayed,
                "display product 3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 4");
            Thread.Sleep(1000);

            FormProdOfferPrice("1000");
            FormProdCount("10");

            FormProdCount("2", 1);

            FormProdOfferPrice("3000", 2);

            BlockSettingsSave();
            Thread.Sleep(1000);

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name many");
            checkField(2, "test@name.many");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name many", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("14", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("13 313 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("13313", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name many", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual("test@name.many", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");

            //check product grid

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");
            VerifyAreEqual("TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1",
                Driver.GetGridCell(0, "Name", "LeadItems").Text, "Name product at order");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("10", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("10 000 руб."), " Cost product at order");

            VerifyAreEqual("TestProduct2\r\nАртикул: 2\r\nЦвет: Color2\r\nРазмер: SizeName2",
                Driver.GetGridCell(1, "Name", "LeadItems").Text, "Name product at order 2");
            VerifyAreEqual("101", Driver.GetGridCell(1, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("2", Driver.GetGridCell(1, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyIsTrue(Driver.GetGridCell(1, "Cost", "LeadItems").Text.Contains("202 руб."), " Cost product at order 2");

            VerifyAreEqual("TestProduct10\r\nАртикул: 10\r\nЦвет: Color10\r\nРазмер: SizeName10",
                Driver.GetGridCell(2, "Name", "LeadItems").Text, "Name product at order 3");
            VerifyAreEqual("3000", Driver.GetGridCell(2, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 3");
            VerifyAreEqual("1", Driver.GetGridCell(2, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 3");
            VerifyIsTrue(Driver.GetGridCell(2, "Cost", "LeadItems").Text.Contains("3 000 руб."), " Cost product at order 3");

            VerifyAreEqual("TestProduct10\r\nАртикул: 103\r\nЦвет: Color2\r\nРазмер: SizeName2",
                Driver.GetGridCell(3, "Name", "LeadItems").Text, "Name product at order 4");
            VerifyAreEqual("111", Driver.GetGridCell(3, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 4");
            VerifyAreEqual("1", Driver.GetGridCell(3, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 4");
            VerifyIsTrue(Driver.GetGridCell(3, "Cost", "LeadItems").Text.Contains("111 руб."), " Cost product at order 4");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSomeEdit()
        {
            TestName = "ServicesFormAddSomeEdit";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct1");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct2");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product 2");
            Thread.Sleep(1000);

            FormProdOfferPrice("1000");
            FormProdCount("10");
            FormProdCount("2", 1);
            BlockSettingsSave();
            Thread.Sleep(1000);
            Refresh();

            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            TabSelect("tabHeaderActions");
            //add new product
            TabSelect("tabFormSetting_0");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed,
                "display product 1 refresh");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed,
                "display product 2 refresh");
            VerifyAreEqual("1000",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NewPriceProduct\"]")).GetAttribute("value"),
                "new price");
            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input")).GetAttribute("value"),
                "new amount");
            VerifyAreEqual("",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewPriceProduct\"]"))[1].GetAttribute("value"),
                "new price 2");
            VerifyAreEqual("2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input"))[1].GetAttribute("value"),
                "new amount 2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);
            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed,
                "display product 1 new");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed,
                "display product 2 new");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_10\"]")).Displayed,
                "display product 3 new");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 4 new");
            VerifyAreEqual("1000",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NewPriceProduct\"]")).GetAttribute("value"),
                "new price after add");
            VerifyAreEqual("10",
                Driver.FindElement(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input")).GetAttribute("value"),
                "new amount after add");
            VerifyAreEqual("",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewPriceProduct\"]"))[1].GetAttribute("value"),
                "new price 2 after add");
            VerifyAreEqual("2",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input"))[1].GetAttribute("value"),
                "new amount 2 after add");
            VerifyAreEqual("",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewPriceProduct\"]"))[2].GetAttribute("value"),
                "new price 3 after add");
            VerifyAreEqual("1",
                Driver.FindElements(By.CssSelector("[data-e2e=\"NewAmountProduct\"] input"))[2].GetAttribute("value"),
                "new amount 3 after add");
            BlockSettingsSave();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name new");
            checkField(2, "test@name.new");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name new", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("14", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("10 422 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("10422", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name new", Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"),
                "name lead ");
            VerifyAreEqual("test@name.new", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");

            //check product grid

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");
            VerifyAreEqual("TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1",
                Driver.GetGridCell(0, "Name", "LeadItems").Text, "Name product at order");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("10", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("10 000 руб."), " Cost product at order");

            VerifyAreEqual("TestProduct2\r\nАртикул: 2\r\nЦвет: Color2\r\nРазмер: SizeName2",
                Driver.GetGridCell(1, "Name", "LeadItems").Text, "Name product at order 2");
            VerifyAreEqual("101", Driver.GetGridCell(1, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("2", Driver.GetGridCell(1, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyIsTrue(Driver.GetGridCell(1, "Cost", "LeadItems").Text.Contains("202 руб."), " Cost product at order 2");

            VerifyAreEqual("TestProduct10\r\nАртикул: 10\r\nЦвет: Color10\r\nРазмер: SizeName10",
                Driver.GetGridCell(2, "Name", "LeadItems").Text, "Name product at order 3");
            VerifyAreEqual("109", Driver.GetGridCell(2, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 3");
            VerifyAreEqual("1", Driver.GetGridCell(2, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 3");
            VerifyIsTrue(Driver.GetGridCell(2, "Cost", "LeadItems").Text.Contains("109 руб."), " Cost product at order 3");

            VerifyAreEqual("TestProduct10\r\nАртикул: 103\r\nЦвет: Color2\r\nРазмер: SizeName2",
                Driver.GetGridCell(3, "Name", "LeadItems").Text, "Name product at order 4");
            VerifyAreEqual("111", Driver.GetGridCell(3, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 4");
            VerifyAreEqual("1", Driver.GetGridCell(3, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 4");
            VerifyIsTrue(Driver.GetGridCell(3, "Cost", "LeadItems").Text.Contains("111 руб."), " Cost product at order 4");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServicesFormAddSomePrice()
        {
            TestName = "ServicesFormAddSomePrice";
            VerifyBegin(TestName);

            GoToClient("lp/test1");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnActionButtonSelect("Показ всплывающей формы");
            TabSelect("tabHeaderActions");
            FormActionAfterSendSelect("Показать сообщение");
            FormActionAfterSendSalesFunnelSelect("Sales Funnel 2");

            //add product
            TabSelect("tabFormSetting_0");
            DelAllProduct();
            Driver.FindElement(By.CssSelector("[data-e2e=\"SelectMultiProduct\"]")).Click();
            Thread.Sleep(1000);

            Driver.GridFilterSendKeys("TestProduct1");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct2");
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();

            Driver.GridFilterSendKeys("TestProduct10");
            VerifyAreEqual("TestProduct10", Driver.GetGridCell(0, "Name", "OffersSelectvizr").Text, "Name product at grid 2");
            Driver.GetGridCell(0, "treeBaseRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector(".ui-grid-icon-plus-squared")).Click();
            Driver.GetGridCell(0, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "OffersSelectvizr")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"] span")).Click();
            Thread.Sleep(2000);

            Driver.FindElement(By.CssSelector("#modalSelectOffer .blocks-constructor-btn-confirm")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_1\"]")).Displayed, "display product 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_2\"]")).Displayed, "display product 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_10\"]")).Displayed,
                "display product 3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Product_103\"]")).Displayed,
                "display product 4");
            Thread.Sleep(1000);

            FormProdOfferPrice("1000");
            FormProdCount("10");

            FormProdCount("2", 1);

            FormProdOfferPrice("3000", 2);

            BlockSettingsSave();
            Thread.Sleep(1000);

            ChangePriceAdmin(1, "4000");
            ChangePriceAdmin(2, "3000");
            ChangePriceAdmin(10, "2000");
            ChangePriceAdmin(10, "1000", 1);

            GoToClient("lp/test1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Click();
            Thread.Sleep(1000);
            checkField(0, "test name change");
            checkField(2, "test@name.change");
            Driver.FindElement(By.CssSelector("[data-e2e=\"FormBtn\"]")).Click();
            Thread.Sleep(1000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"FormSuccessText\"]")).Displayed,
                "SuccessText displayed after agree");

            GoToAdmin("leads?salesFunnelId=2");
            VerifyAreEqual("Funnel 2 Deal Status 1", Driver.GetGridCell(0, "DealStatusName").Text, "DealStatusName ");
            VerifyAreEqual("test name change", Driver.GetGridCell(0, "FullName").Text, "FullName ");
            VerifyAreEqual("14", Driver.GetGridCell(0, "ProductsCount").Text, "ProductsCount ");
            VerifyAreEqual("20 000 руб.", Driver.GetGridCell(0, "SumFormatted").Text, "Sum ");
            VerifyIsTrue(Driver.GetGridCell(0, "CreatedDateFormatted").Text.Contains(DateTime.Today.ToString("dd.MM.yyyy")),
                "date ");

            Driver.GetGridCell(0, "Id").Click();
            Thread.Sleep(1000);

            VerifyAreEqual("20000", Driver.FindElement(By.Name("leadSum")).GetAttribute("value"), "sum lead ");
            VerifyAreEqual("test name change",
                Driver.FindElement(By.Id("Lead_Customer_FirstName")).GetAttribute("value"), "name lead ");
            VerifyAreEqual("test@name.change", Driver.FindElement(By.Id("Lead_Customer_EMail")).GetAttribute("value"),
                "mail lead ");

            //check product grid

            VerifyIsTrue(Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridLeadItems\"]")).Displayed,
                "displayed product grid");
            VerifyAreEqual("TestProduct1\r\nАртикул: 1\r\nЦвет: Color1\r\nРазмер: SizeName1",
                Driver.GetGridCell(0, "Name", "LeadItems").Text, "Name product at order");
            VerifyAreEqual("1000", Driver.GetGridCell(0, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order");
            VerifyAreEqual("10", Driver.GetGridCell(0, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order");
            VerifyIsTrue(Driver.GetGridCell(0, "Cost", "LeadItems").Text.Contains("10 000 руб."), " Cost product at order");

            VerifyAreEqual("TestProduct2\r\nАртикул: 2\r\nЦвет: Color2\r\nРазмер: SizeName2",
                Driver.GetGridCell(1, "Name", "LeadItems").Text, "Name product at order 2");
            VerifyAreEqual("3000", Driver.GetGridCell(1, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 2");
            VerifyAreEqual("2", Driver.GetGridCell(1, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 2");
            VerifyIsTrue(Driver.GetGridCell(1, "Cost", "LeadItems").Text.Contains("6 000 руб."), " Cost product at order 2");

            VerifyAreEqual("TestProduct10\r\nАртикул: 10\r\nЦвет: Color10\r\nРазмер: SizeName10",
                Driver.GetGridCell(2, "Name", "LeadItems").Text, "Name product at order 3");
            VerifyAreEqual("3000", Driver.GetGridCell(2, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 3");
            VerifyAreEqual("1", Driver.GetGridCell(2, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 3");
            VerifyIsTrue(Driver.GetGridCell(2, "Cost", "LeadItems").Text.Contains("3 000 руб."), " Cost product at order 3");

            VerifyAreEqual("TestProduct10\r\nАртикул: 103\r\nЦвет: Color2\r\nРазмер: SizeName2",
                Driver.GetGridCell(3, "Name", "LeadItems").Text, "Name product at order 4");
            VerifyAreEqual("1000", Driver.GetGridCell(3, "Price", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Price product at order 4");
            VerifyAreEqual("1", Driver.GetGridCell(3, "Amount", "LeadItems").FindElement(By.Name("inputForm")).Text,
                " Count product at order 4");
            VerifyIsTrue(Driver.GetGridCell(3, "Cost", "LeadItems").Text.Contains("1 000 руб."), " Cost product at order 4");

            VerifyFinally(TestName);
        }
    }
}